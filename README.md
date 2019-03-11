![NovaTrakt - GPS Video Player](Logos/NovaTrakt_FullLogo_Horizontal.png)

![GitHub Releases](https://img.shields.io/github/downloads/RobTrehy/NovaTrakt/latest/total.svg)
![GitHub release](https://img.shields.io/github/release/RobTrehy/NovaTrakt.svg)

## Play GPS Video files from your Novatek based Dash Cam

----
#### NovaTrakt is designed as an alternative to older or more clumsy GPS Video Players.
NovaTrakt is designed and tested to work specifically with GPS Videos created by a VIOFO A119. Although similar devices powered by a Novatek NT96660 processors, for example the VIOFO A119S, will also work.
(See the FAQ for a list of cameras known to work with NovaTrakt)

NovaTrakt provides all of your GPS Video information in one simple window, with an easy to use interface.
 With your journeys automatically presented to you as one trip.


## Table of contents
- [Features](#features)
- [About NovaTrakt](#about-novatrakt)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Frequently Asked Questions](#frequently-asked-questions)
- [Contributors](#contributors)
- [License](#license)


----
## Features

#### Journey Detection
NovaTrakt will detect when multiple video clips should be combined to make one longer journey. All clips will be played in order, with your position shown on the map!

#### Map Synchronisation
As the videos play the current location on the map is updated to reflect the current location in the video. The entire journey is displayed as one route on the map.

#### Bing Maps
NovaTrakt uses the Bing Maps API, to display your routes and current location. You can toggle between road maps and satellite imagery.


----
## About NovaTrakt
On release, the VIOFO A119 wasn't compatible with most of the GPS Video Players available as it used new technology (the Novatek NT96660 processor), which meant the GPS data was written to the file differently to any camera before it.

NovaTrakt was initially released as a way of extracting the data into a GPX file to make the GPS Video files compatible with an older GPS Video Player. Today NovaTrakt has become a new GPS Video Player itself!


----
## System Requirements
- Windows 7 or higher, 32-bit or 64-bit
- Microsoft .NET Framework 4.5.2 or higher
- An internet connection


----
## Installation
Simply download the latest setup file and follow the on-screen prompts.


----
## Frequently Asked Questions

#### What firmware versions are compatible?
The firmware versions listed below are currently known to be compatible with NovaTrakt

- 1.1
- 2.0
- 2.02

__Note: Firmware version 2.01 is NOT compatible with NovaTrakt due to a bug in the firmware!__

#### Do you support other cameras?
At present, only the VIOFO A119 is officially supported. 
NovaTrakt has been found to work with similar cameras based upon similar chipsets.

#### Why does my video playback stutter?
There could be many reasons for this, but it is usually related to codecs installed on your computer.

#### Why does the map location stutter?
GPS signals are only recorded every second, therefore the map location marker is only updated every second.

#### Why don't I see any GPS data?
Check your camera is set to save the GPS data to your video and that your camera has a compatible firmware version.
More information may be available in the log file, try turning on Verbose logging and opening your files again.

#### Is my location information secure?
NovaTrakt reads the data directly from your video files and displays it within itself. The data is never saved to external files unless you select to export the clip or journey to a GPX file.
NovaTrakt requires an internet connection only for the display of the map.

----
## Contributors
|**[RobTrehy](http://rob.trehy.co.uk)**|
|:--:|
|![RobTrehy](https://avatars3.githubusercontent.com/u/13102009?s=150)|
|[github.com/robtrehy](https://github.com/robtrehy)|
|**[Donate](https://paypal.me/RobTrehy)**|

----
## License
![GitHub](https://img.shields.io/github/license/RobTrehy/NovaTrakt.svg)

- **[GNU GPLv3 license](LICENSE.md)**