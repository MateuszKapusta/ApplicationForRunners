using System;
using System.Collections.Generic;

using Microsoft.Azure.Mobile.Server;

namespace ApplicationForRunnersService.DataObjects
{
    public class UserItem:EntityData
    {
        public string Login { get; set; }
        public string Email { get; set; }

        public string Gender { get; set; }
        public DateTime Birth { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }

        public string UserId { get; set; }

        public virtual List<WorkoutItem> WorkoutItems { get; set; }
    }
}