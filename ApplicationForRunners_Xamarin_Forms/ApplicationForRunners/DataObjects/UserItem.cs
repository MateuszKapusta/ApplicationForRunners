using System;
using System.Collections.Generic;

namespace ApplicationForRunners.DataObjects
{
    public class UserItem : DataObject
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; } = "M";
        public DateTime Birth { get; set; } = new DateTime(2000,1,1);
        public double Weight { get; set; } = 75;
        public double Height { get; set; } = 180;

        public  List<WorkoutItem> WorkoutItems { get; set; }
    }
}
