using Microsoft.Azure.Mobile.Server;
using System.Collections.Generic;

namespace ApplicationForRunnersService.DataObjects
{
    public class WorkoutItem : EntityData
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

        public string UserId { get; set; }

        public string UserItemId { get; set; }
        public virtual UserItem UserItem { get; set; }

        public virtual List<MapPointItem> MapPointItems { get; set; } 
    }
}