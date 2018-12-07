# ApplicationForRunners

Server and client written in C # using Xamarin.Form and Azure App Service. Its main function is to help runner in training.<br /> The application works on Android, iOS and UWP.<br />

|![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/Androidstop.png)|                    ![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/iOSstop.png)|                         ![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/UWPstop.png)|
|:---:|:---:|:---:|
| Android|iOS|Windows Mobile|

## Setup

UWP application need key to work. Android application will start but if you dont have key, map will not appear. On iOS it should work without any problem <br />
Maps initialization https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map

## Running tests

You can test client on iOS and Android using AppUITest project, locally or on AppCenter. Remember to set you App Secret in code for testing on AppCenter. Usage of AppCenter requires NUnit 2.6.3 or 2.6.4 to run tests. AppUITest locally use NUnit 3.x. <br />
AppCenter instruction https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin<br />

## Usage
### Aplication

![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/mainMD.png)<br />

### Login and Registration

![alt text](https://github.com/MateuszKapusta/ApplicationForRunners/blob/master/Pictures/startPage.png)<br />




