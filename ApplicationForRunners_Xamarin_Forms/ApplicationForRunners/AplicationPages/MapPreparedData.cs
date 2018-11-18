using ApplicationForRunners.DataObjects;
using System;
using Xamarin.Forms.Maps;

namespace ApplicationForRunners.AplicationPages
{
    public class MapPreparedData
    {
        public string WorkoutId { get; set; }
        public DateTime StartTime { get; set; }
        public Distance RoadDistance { get; set; }
        public DateTime Time { get; set; }
        public double MaxSpeed { get; set; }
        public double MediumSpeed { get; set; }
        public double MaxAltitude {get;set;}
        public double MinAltitude { get; set; }

        public MapPreparedData()
        {
        }

        public MapPreparedData(MapPreparedData newOdject) {

            StartTime = new DateTime(newOdject.StartTime.Ticks);
            RoadDistance = new Distance(newOdject.RoadDistance.Meters);
            Time =new DateTime(newOdject.Time.Ticks);
            MaxSpeed = newOdject.MaxSpeed;
            MediumSpeed = newOdject.MediumSpeed;
            MaxAltitude = newOdject.MaxAltitude;
            MinAltitude = newOdject.MinAltitude;
        }

        static DateTime time40l = new DateTime(2010, 1, 1);

        public static MapPreparedData ConvertToPreparedData(WorkoutItem data ) {

            MapPreparedData CovertedObj = new MapPreparedData
            {
                WorkoutId = data.Id,
                StartTime = new DateTime(data.WorkoutDate + time40l.Ticks),
                RoadDistance = new Distance(data.Distance),
                Time = new DateTime(data.Time),
                MaxSpeed = data.MaxSpeed,
                MediumSpeed = data.MediumSpeed,
                MaxAltitude = data.MaxAltitude,
                MinAltitude = data.MinAltitude
            };

            return CovertedObj;
        }

        public void Reset() {
            WorkoutId = null;
            StartTime = new DateTime(0);
            RoadDistance = new Distance(0);
            Time = new DateTime(0);
            MaxSpeed = 0;
            MediumSpeed = 0;
            MaxAltitude = 0;
            MinAltitude = 0;
        }
    }
}
