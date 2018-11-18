using System;
using System.Collections.Generic;
using ApplicationForRunners.AplicationPages;

namespace ApplicationForRunners.DataObjects
{
    public class WorkoutItem : DataObject
    {
        public long WorkoutDate { get; set; }
        public long Time { get; set; }
        public double Distance { get; set; }
        public double MaxSpeed { get; set; }
        public double MediumSpeed { get; set; }
        public double MaxAltitude { get; set; }
        public double MinAltitude { get; set; }
        //public double UpAltitude { get; set; }
        //public double DownAltitude { get; set; }


        public string UserItemId { get; set; }

        public  List<MapPointItem> MapPointItems { get; set; }

        public WorkoutItem() {
        }

        static DateTime time40l = new DateTime(2010, 1, 1);

        public WorkoutItem(MapPreparedData create)
        {        
            WorkoutDate = create.StartTime.Ticks - time40l.Ticks;
            Time = create.Time.Ticks;
            Distance = create.RoadDistance.Meters;
            MaxSpeed = create.MaxSpeed;
            MediumSpeed = create.MediumSpeed;
            MaxAltitude = create.MaxAltitude;
            MinAltitude = create.MinAltitude;
        }

        public static WorkoutItem ConvertToWorkoutItem(MapPreparedData use)
        {
            WorkoutItem data = new WorkoutItem
            {
                Id = use.WorkoutId,
                WorkoutDate = use.StartTime.Ticks - time40l.Ticks,
                Time = use.Time.Ticks,
                Distance = use.RoadDistance.Meters,
                MaxSpeed = use.MaxSpeed,
                MediumSpeed = use.MediumSpeed,
                MaxAltitude = use.MaxAltitude,
                MinAltitude = use.MinAltitude
            };

            return data;
        }
    }
}

