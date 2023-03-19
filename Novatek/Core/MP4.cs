using NovaTrakt.SharedComponents;
using Novatek.Helpers;
using System;
using System.IO;
using System.Linq;

namespace Novatek.Core
{
    class MP4
    {
        private static double lastLat = 0;
        private static double lastLong = 0;
        private static bool slowSpeed = false;

        private static Log Log = new Log();

        // Test to make sure the selected file contains video data
        // @return long The location of the 'MOOV' box
        public static long LocateMoov(string _file)
        {
            try
            {
                // Open the file in Read mode
                FileStream fin = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Get the size of the first box and it's type
                int box_size = (int)Streams.ReadUInt(fin, true);
                string box_type = Streams.ReadString(fin, 4);

                // Loop while the box type is not 'MOOV'
                while (box_type.ToString() != "moov")
                {
                    // Check for end of box
                    // If not at the end, seek to the next box
                    if (!Streams.Seek(fin, box_size - 8))
                    {
                        return 0;
                    }

                    // Get the size and type of the next box
                    box_size = (int)Streams.ReadUInt(fin, true);
                    box_type = Streams.ReadString(fin, 4);
                }

                // Set the "MOOV" position
                long moov = fin.Position - 8;

                Log.WriteVerboseLine("'moov' box found at: " + moov);

                fin.Close();

                return moov;
            }
            catch
            {
                // Unable to open file
                return 0;
            }
        }

        // Get the start and end date/time and the duration of the video file
        public static Times fileTimes(string _file, long _moov)
        {
            Times t = new Times();

            try
            {
                // Open the file in Read mode
                FileStream fin = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Seek to the end of the MOOV box
                Streams.Seek(fin, _moov + 8);

                // Get the first sub box size and type
                int sub_box_size = (int)Streams.ReadUInt(fin, true);
                string sub_box_type = Streams.ReadString(fin, 4);

                // Loop while the sub box type is not "MVHD"
                while (sub_box_type.ToString() != "mvhd")
                {
                    // Check for end of box
                    // If not at end, seek to the next box
                    if (!Streams.Seek(fin, sub_box_size - 8))
                    {
                        return t;
                    }

                    // Get the next sub box size and type
                    sub_box_size = (int)Streams.ReadUInt(fin, true);
                    sub_box_type = Streams.ReadString(fin, 4);
                }

                // The "MVHD" binary blob consists of 1byte (version, either 0 or 1), 3bytes (flags),
                // and then either 4bytes (creation), 4bytes (modification)
                // or 8bytes (creation), 8bytes (modification)
                // If version=0, then it's the former, otherwise it's the latter.
                // In both cases "creation" and "modification" are big-endian number of seconds since 1st Jan 1904 UTC

                int version = (int)Streams.ReadUInt(fin, true);
                double endS = Convert.ToDouble(Streams.ReadUInt(fin, true));
                double secondsB = Convert.ToDouble(Streams.ReadUInt(fin, true));

                int timespan = (int)Streams.ReadUInt(fin, true);
                int duration = (int)Streams.ReadUInt(fin, true);

                double durationS = duration / timespan;

                DateTime fileTime = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                if (version == 0)
                {
                    double startS = (endS - durationS);

                    t.startTime = fileTime.AddSeconds(startS);
                    t.endTime = fileTime.AddSeconds(endS);
                    t.duration = (int)durationS;

                    return t;
                }
                else
                {
                    // VIOFO uses version 0, if a new firmware uses a different version, I'll work it out then!
                    // return fileTime.AddSeconds(((endS + secondsB) - secondsD));

                    // Returning emtpy object
                    return t;
                }
            }
            catch
            {
                // Unable to open file

                return t;
            }
        }

        // Get the GPS box location 
        public static GPSBox GetGPS(string _file, long _moov)
        {
            GPSBox box = new GPSBox();

            try
            {
                // Open the file in Read mode
                FileStream fin = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Seek to the end of the MOOV box
                Streams.Seek(fin, _moov + 8);

                // Get the first sub box size and type
                int sub_box_size = (int)Streams.ReadUInt(fin, true);
                string sub_box_type = Streams.ReadString(fin, 4);

                // Loop while the sub box type is not "GPS "
                while (sub_box_type.ToString() != "gps ")
                {
                    // Check for end of box
                    // If not at end, seek to the next box
                    if (!Streams.Seek(fin, sub_box_size - 8))
                    {
                        Log.WriteLine("ERROR", "File at " + _file + " does not contain valid GPS data!");
                        return box;
                    }

                    // Get the next sub box size and type
                    sub_box_size = (int)Streams.ReadUInt(fin, true);
                    sub_box_type = Streams.ReadString(fin, 4);
                }

                long gps = fin.Position - 8;

                fin.Close();

                // Set box position and size
                box.Pos = gps;
                box.Size = sub_box_size;

                return box;
            }
            catch
            {

                // Unable to open file

                return box;
            }
        }

        // Get the GPS data from the file
        public static GPSDataList ReadGPS(string _file, GPSBox _gpsBox)
        {
            GPSDataList GPSDataList = new GPSDataList();

            try
            {
                // Open the file in Read mode
                FileStream fin = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Seek to the end of the "GPS " boc
                // Skip the first 16 'header' bits
                Streams.Seek(fin, _gpsBox.Pos + 16);
                int gps_read = 16;

                // While we are reading in the sub box
                while (gps_read < _gpsBox.Size - 8)
                {
                    // Get the GPS box position and size
                    int gps_box_pos = (int)Streams.ReadUInt(fin, true);
                    int gps_box_size = (int)Streams.ReadUInt(fin, true);

                    // Update our position
                    gps_read = gps_read + 8;

                    // Save the current file seek position
                    long pos = fin.Position;

                    // Seek to this GPS data
                    fin.Seek(gps_box_pos, SeekOrigin.Begin);

                    // Get the GPS box, size, type and magic
                    int gps_box_size1 = (int)Streams.ReadUInt(fin, true);
                    string gps_box_type = Streams.ReadString(fin, 4);
                    string gps_box_magic = Streams.ReadString(fin, 4);

                    // Verify the read data is GPS data
                    if (gps_box_size != gps_box_size1 || gps_box_type != "free" || gps_box_magic != "GPS ")
                    {
                        // Log incorrect data format
                        Log.WriteLine("ERROR", "Box at position: " + gps_box_pos + " is not the expected format\r\n\t\t\t\t" +
                                        "(Expected size: " + gps_box_size + ", Actual size: " + gps_box_size1 + ", " +
                                        "Expected type: free, Actual type: " + gps_box_type + ", " +
                                        "Expected magic: GPS , Actual magic: " + gps_box_magic + ")\r\n");
                        continue;
                    }

                    // Extract GPS data
                    int version = (int)Streams.ReadUInt(fin);

                    // Maybe able to identify firmware version with this
                    // Firmware 1.1 and 2.02 = 76
                    // Firmware 2.0 = 15344
                    // Firmware 2.01 - NOT COMPATIBLE, BUG IN FIRMWARE

                    if (version == 76)
                    {
                        // Firmware 1.1 or 2.02
                        fin.Seek(32, SeekOrigin.Current);
                    }
                    else if (version == 15344)
                    {
                        // Firmware 2.0
                        // Nothing to do
                    }
                    else
                    {
                        // Unknown Firmware Version
                    }

                    int hour = (int)Streams.ReadUInt(fin);
                    int minute = (int)Streams.ReadUInt(fin);
                    int second = (int)Streams.ReadUInt(fin);
                    int year = (int)Streams.ReadUInt(fin);
                    int month = (int)Streams.ReadUInt(fin);
                    int day = (int)Streams.ReadUInt(fin);

                    string active = Streams.ReadString(fin, 1);
                    string latitude_b = Streams.ReadString(fin, 1);
                    string longitude_b = Streams.ReadString(fin, 1);
                    string unknown2 = Streams.ReadString(fin, 1);
                    float latitude = Streams.ReadFloat(fin);
                    float longitude = Streams.ReadFloat(fin);

                    // Check the GPS had a fix
                    if (active == "A")
                    {
                        // Correct the speed during load
                        // 1 knot = 0.514444 m/s
                        float speed = Streams.ReadFloat(fin) * (float)0.514444;

                        DateTime dateTime;

                        if (!Enumerable.Range(1, 31).Contains(day))
                        {
                            // TODO: Calculate day from somewhere
                            // Firmware 1.1 has an issue with day, used to get it from file name?
                        }

                        // Create DateTime object
                        if (year != 0 && month != 0 && day != 0)
                        {
                            dateTime = new DateTime((year + 2000), month, day, hour, minute, second);
                        }
                        else
                        {
                            // Date not valid
                            dateTime = new DateTime(1);
                            continue;
                        }

                        // Change coordinate from DDDmm.mmmm format to traditional lat/long
                        double flatitude = GPSHelpers.FixCoordinates(latitude_b, latitude);
                        double flongitude = GPSHelpers.FixCoordinates(longitude_b, longitude);

                        // Create new GPSData object
                        GPSData gps = new GPSData(AppSettings.getBool("MPH", false));

                        gps.DateTime = dateTime;
                        gps.Speed = speed;

                        // Kalman filter the GPS data to remove jumps
                        Kalman k = new Kalman(gps.Speed);
                        k.Process(flatitude, flongitude, 9.9f, 1000);
                        gps.Latitude = k.get_lat();
                        gps.Longitude = k.get_lng();

                        if (lastLat != 0 && lastLong != 0)
                        {
                            gps.Distance = GPSHelpers.Distance(lastLat, lastLong, flatitude, flongitude);
                            gps.Heading = GPSHelpers.Heading(lastLat, lastLong, flatitude, flongitude);
                        }
                        lastLat = gps.Latitude;
                        lastLong = gps.Longitude;

                        if (!slowSpeed)
                            GPSDataList.FilteredGPSData.Add(gps);
                        GPSDataList.AllGPSData.Add(gps);

                        // Remove a few points when speed is low to stop jagged lines
                        if (speed <= 1)
                            slowSpeed = true;
                        else
                            slowSpeed = false;
                    }
                    else
                        Log.WriteVerboseLine("GPS Location is not locked. Trying next block.");

                    // Return to original file seek
                    fin.Seek(pos, SeekOrigin.Begin);
                }
            }
            catch (Exception e)
            {
                // Unable to open file
                Log.WriteLine("ERROR", "Unable to process file: " + _file);
                Log.WriteLine("ERROR", e.Message.ToString());
            }

            return GPSDataList;
        }
    }
}
