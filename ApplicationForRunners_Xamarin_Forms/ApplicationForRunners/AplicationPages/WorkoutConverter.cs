using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ApplicationForRunners.AplicationPages
{
    public enum TypeOfConventer { Timer/*Fake*/, Distance,Speed,MediumSpeed,Tempo,MediumTempo, Altitude};
    public enum TypeOfConvAfter { Distance, Speed, Tempo, Altitude };

    class WorkoutConventer
    {
        static string DistanceMarker = Constants.AppUnitsType; //KM//MI
        private Distance EarthRadius = Distance.FromMeters(6371000); //MI 3959

        public WorkoutConventer() {       
        }

        public static Array NrOfType()
        {
            return Enum.GetValues(typeof(TypeOfConventer)); ; 
        }

        public static Array NrOfTypeAfter()
        {
            return Enum.GetValues(typeof(TypeOfConvAfter)); ;
        }


        public MapPreparedData ValueObject(MapPreparedData GeoData, Plugin.Geolocator.Abstractions.Position oldValue, Plugin.Geolocator.Abstractions.Position newValue,int numberOfValue,double surveyTime) {

            if (newValue.Speed > GeoData.MaxSpeed)
                GeoData.MaxSpeed = newValue.Speed;
                      
            if (newValue.Altitude > GeoData.MaxAltitude)
                GeoData.MaxAltitude = newValue.Altitude;

            if (GeoData.MinAltitude==0 || newValue.Altitude < GeoData.MinAltitude)
                GeoData.MinAltitude = newValue.Altitude;

            if (oldValue != null)
            {
                GeoData.RoadDistance = Distance.FromMeters(GeoData.RoadDistance.Meters + CalculateDistance(oldValue, newValue).Meters);

                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        GeoData.MediumSpeed = GeoData.RoadDistance.Meters / (AppGeolocator.MinimumListTime * (numberOfValue ));
                        /*Use ideal number// ((srTime.Ticks / 10000000.0)- AppGeolocator.MinimumListTime * numberOfValue * 0.05); Delete timer mistake*/
                        break;
                    case Device.iOS:
                    case Device.UWP:                     
                        GeoData.MediumSpeed = GeoData.RoadDistance.Meters / surveyTime; 
                        break;
                }                
            }

            return GeoData;
        }

        public  static double  ValueTypeIn(TypeOfConventer tOConvert, MapPreparedData GeoData, Plugin.Geolocator.Abstractions.Position newValue)
        {
            switch (tOConvert) {
                case TypeOfConventer.Timer:
                    return 0;
                case TypeOfConventer.Distance:
                        return RoadDistance(GeoData.RoadDistance);

                case TypeOfConventer.Speed:
                    return Speed(newValue.Speed);
                
                case TypeOfConventer.MediumSpeed:
                    return Speed( GeoData.MediumSpeed);

                case TypeOfConventer.Tempo:
                    return Tempo(newValue.Speed);

                case TypeOfConventer.MediumTempo:
                    return Tempo(GeoData.MediumSpeed);

                case TypeOfConventer.Altitude:
                    return Altidude(newValue.Altitude);

                default:
                    return -1;          
            }         
        }

        public static double ValueTypeAfter(TypeOfConvAfter tOConvert, double data, Distance distance)
        {
            switch (tOConvert) {

                case TypeOfConvAfter.Distance:
                    return RoadDistance(distance);

                case TypeOfConvAfter.Speed:
                    return Speed(data);

                case TypeOfConvAfter.Tempo:
                    return Tempo(data);

                case TypeOfConvAfter.Altitude:
                    return Altidude(data);

                default:
                    return -1;
            }

        }

        static double RoadDistance(Distance data) {

            if (DistanceMarker.Equals("KM"))
                return data.Kilometers;
            else
                return data.Miles;
        }

        static double Speed(double data) {

            if (data == 0)
                return 0;

            if (DistanceMarker.Equals("KM"))
                return (data * 3600.0) / 1000.0;
            else
                return (data * 3600.0) / 1609.344;
        }

        static double Tempo(double data) {

            if (data == 0)
                return 0;

            if (DistanceMarker.Equals("KM"))
                return 1000.0 / (data * 60.0);
            else
                return 1609.344 / (data * 60.0);
        }

        static double Altidude(double data)
        {

            if (DistanceMarker.Equals("KM"))
                return data;
            else
                return data * 3.2808399;
        }


        private Distance CalculateDistance(Plugin.Geolocator.Abstractions.Position oldValue, Plugin.Geolocator.Abstractions.Position newValue) {

            double radians=Math.Acos(Math.Sin(ToRadians(oldValue.Latitude))
                *Math.Sin(ToRadians(newValue.Latitude))
                + Math.Cos(ToRadians(oldValue.Latitude))
                *Math.Cos(ToRadians(newValue.Latitude))
                *Math.Cos(ToRadians(oldValue.Longitude- newValue.Longitude)));


            return Distance.FromMeters(EarthRadius.Meters * radians) ;
        }


        static double ToRadians(double angle)
        {
            return  Math.PI  * angle/180;
        }

    }
}
