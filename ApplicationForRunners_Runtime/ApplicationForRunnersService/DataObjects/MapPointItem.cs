using Microsoft.Azure.Mobile.Server;

namespace ApplicationForRunnersService.DataObjects
{
    public class MapPointItem : EntityData
    {
        public int Order { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string UserId { get; set; }

        public string WorkoutItemId { get; set; }
        public virtual WorkoutItem WorkoutItem { get; set; }
    }
}