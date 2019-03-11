# Novatek.dll Readme

----
## What is Novatek.dll?
This Dynamic Link Library is a C# class library that contains the deep level code required for extracting the GPS data from Novatek MP4 video files.


----
## Novatek.dll Functions

### Constructor

##### Param: (string) File Path - Required
##### Param: (string) File Name - Required

This function will construct the class, open and confirm the video file is valid.

If the public long MoovBox is equal to 0, the video file is not valid.


### Process()

This function will scan the video file for valid GPS data and perform a [Kalman filter](https://en.wikipedia.org/wiki/Kalman_filter) to smooth the data points. 

The public List<GPSData> GPSData stores the filtered GPS data.
The public List<GPSData> AllGPSData stores the unfiltered GPS data.


### SetStartTown() async

This function will use the Bing Maps Geocoding API to reverse Geocode the first latitude and longitude of the video clip into a starting town.


### SetEndTown() async

This function will use the Bing Maps Geocoding API to reverse Geocode the last latitude and longitude of the video clip into a ending town.


----
## Novatek.dll Public Paramaters

*string* **FileName** - The name of the file.

*string* **FilePath** - The full path to the file.

*string* **FullNameAndPath** - The full path and name of the file.

*string* **PreviousFile** - Used for organising journeys, this string contains the FileName of the clip that should play prior to this one.

*string* **NextFile** - Used for organising journeys, this string contains the FileName of the clip that should play after this one.

*long* **MoovBox** - Contains the position of the box containing the Moov data.
If this is 0, the file does not contain valid video data.

*DateTime* **StartTime** - The date and time that the video clip starts.

*DateTime* **EndTime** - The date and time that the video clip ends.

*int* **Duration** - The length, in seconds, of the video clip.

*GPSBox* **GPSBox** - The GPS Box Data of the video clip.

*bool* **ValidGPS** - Whether or not the video file contain valid GPS data.

*string* **StartTown** - The starting town of the video clip, set by SetStartTown();

*string* **EndTown** - The ending town of the video clip, set by SetEndTown();

*double* **Distance** - The total distance traveled, in meters, in the video clip.

*List<GPSData>* **GPSData** - The [Kalman filtered](https://en.wikipedia.org/wiki/Kalman_filter) list of GPS data points found in the video file

*List<GPSData>* **AllGPSData** - The unfiltered list of GPS data points found in the video file