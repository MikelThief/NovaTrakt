using BingGeocoder;
using Novatek.Core;
using Novatek.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novatek
{
    public class NovatekFile
    {
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public string FullNameAndPath
        {
            get { return filePath + @"\" + fileName; }
        }

        private string previousFile;
        public string PreviousFile
        {
            get { return previousFile; }
            set { previousFile = value; }
        }

        private string nextFile;
        public string NextFile
        {
            get { return nextFile; }
            set { nextFile = value; }
        }

        private long moovBox;
        public long MoovBox
        {
            get { return moovBox; }
            set { moovBox = value; }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        private DateTime endTime;
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        private GPSBox gpsBox;

        public GPSBox GPSBox { get => gpsBox; set => gpsBox = value; }

        private bool validGPS = false;
        public bool ValidGPS
        {
            get { return validGPS; }
            set { validGPS = value; }
        }

        private string startTown;
        public string StartTown
        {
            get { return startTown; }
            set { startTown = value; }
        }

        private string endTown;
        public string EndTown
        {
            get { return endTown; }
            set { endTown = value; }
        }

        private double distance;
        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        private List<GPSData> _GPSData;
        public List<GPSData> GPSData { get => _GPSData; set => _GPSData = value; }

        private List<GPSData> allGPSData;
        public List<GPSData> AllGPSData { get => allGPSData; set => allGPSData = value; }

        public NovatekFile(string _filePath, string _fileName)
        {
            FilePath = _filePath;
            FileName = _fileName;
            MoovBox = MP4.LocateMoov(FullNameAndPath);

            Times T = MP4.fileTimes(FullNameAndPath, MoovBox);
            StartTime = T.startTime;
            EndTime = T.endTime;
            Duration = T.duration;
        }

        public NovatekFile Process()
        {
            GPSBox = MP4.GetGPS(FullNameAndPath, MoovBox);

            // A basic check to see if the file has valid GPS data
            if (GPSBox.Pos != 0)
                ValidGPS = true;

            // Now get the GPS data from the file
            GPSDataList gpsDataList = MP4.ReadGPS(FullNameAndPath, GPSBox);
            GPSData = gpsDataList.FilteredGPSData;
            AllGPSData = gpsDataList.AllGPSData;

            if (ValidGPS)
            {
                foreach (GPSData data in allGPSData)
                {
                    if (!double.IsNaN(data.Distance) && data.Distance < data.Speed)
                        Distance = (Distance + data.Distance);
                }
            }

            return this;
        }

        public async Task SetStartTown()
        {
            var service = new GeoCoder("iQmb3gLgxu9xdmELC1ja~cijMCzvKYYV1X8O_PZ3eZw~AvNMPSVsdys8AXUE3oGx5byA-M3wJS8QcyyOU_0cMDJlSmTPzJMSFZBtbRycfUTV", "NovaTrakt3");

            GPSData gps = GPSData.FirstOrDefault();

            try
            {
                Address address = await service.GetAddress(gps.Latitude, gps.Longitude);
                StartTown = address.locality;
            }
            catch { }
        }

        public async Task SetEndTown()
        {
            var service = new GeoCoder("iQmb3gLgxu9xdmELC1ja~cijMCzvKYYV1X8O_PZ3eZw~AvNMPSVsdys8AXUE3oGx5byA-M3wJS8QcyyOU_0cMDJlSmTPzJMSFZBtbRycfUTV", "NovaTrakt3");

            GPSData gps = GPSData.LastOrDefault();

            try
            {
                Address address = await service.GetAddress(gps.Latitude, gps.Longitude);
                EndTown = address.locality;
            }
            catch { }
        }
    }
}
