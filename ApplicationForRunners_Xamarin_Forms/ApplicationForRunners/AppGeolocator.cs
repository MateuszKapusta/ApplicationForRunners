using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationForRunners
{
    class AppGeolocator:Plugin.Geolocator.CrossGeolocator
    {
        public static List<GLFlag> GlobalGeoFlag { get; set; } = new List<GLFlag>();
        public static double MinimumListTime { get;private set; }   //time you have wait for location variable     

        public static async Task<bool> StartListeningAsync(GLFlag flag,int StartDelay=1000,double minimumTime = 6.0,  bool includeHeading = false, ListenerSettings listenerSettings = null)
        {
            flag.Flag = true;
            Task<bool> startAnswer;

            if (AppGeolocator.Current.IsListening) {
                startAnswer = Task.FromResult(true);
            }
            else {
                startAnswer = Current.StartListeningAsync(TimeSpan.FromSeconds(minimumTime), 0, includeHeading, listenerSettings);//work if true
                MinimumListTime = minimumTime;                
                await Task.Delay(StartDelay);
            }
            return await startAnswer;
        }

        public static Task<bool> StopListeningAsync(GLFlag flag,bool stopGeolocator)//true=stop false dont stop
        {
            flag.Flag = false;

            if ((stopGeolocator && !GeolocatorInUse()) && AppGeolocator.Current.IsListening) {           
                MinimumListTime = 0;
                return Current.StopListeningAsync();
            }
            else
                return Task.FromResult(false);
        }


        public static bool GeolocatorInUse() {
            foreach (GLFlag flag in GlobalGeoFlag) {

                if (flag.Flag == true) 
                    return true;                
            }
            return false;
        }
    }

    public class GLFlag {
        public bool Flag { get; set; } = false;

        public GLFlag(bool value=false) {

            Flag = value;
            AppGeolocator.GlobalGeoFlag.Add(this);
        }
    }
}
