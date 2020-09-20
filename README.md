# ApplicationForRunners

Backend and client aplication written in C # using Xamarin.Form and ASP.NET . Its main function is to help runner in training.<br /> The application works on Android, iOS and UWP.<br />

|![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/Androidstop.png)|                    ![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/iOSstop.png)|                         ![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/UWPstop.png)|
|:---:|:---:|:---:|
| Android|iOS|Windows Mobile|

## Setup

Before you publish backend you mast create Mobile App on https://portal.azure.com and set it using Quickstart and Authentication / Authorization. Client on UWP need maps api key to work. Android application will start but if you dont have key map will not appear. On iOS it should work without any problem. Maps initialization https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map

## Running tests

You can test client on iOS and Android using AppUITest project, locally or on AppCenter. Remember to set you App Secret in code for testing on AppCenter. Usage of AppCenter requires NUnit 2.6 to run tests. AppUITest locally use NUnit 3.x AppCenter instruction https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin<br />

## Usage

### Login and Registration

![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/startPage.png)<br />

### Aplication

![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/mainMD.png)<br />

## Licence

The code is released under the [MIT license](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/LICENSE)



