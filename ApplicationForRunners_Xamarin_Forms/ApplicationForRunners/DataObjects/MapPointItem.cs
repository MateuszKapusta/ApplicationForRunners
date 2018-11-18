using Plugin.Geolocator.Abstractions;

namespace ApplicationForRunners.DataObjects
{
    public class MapPointItem : DataObject
    {
        public int Order { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string WorkoutItemId { get; set; }

        public static Position ConvertToPosition(MapPointItem data) {

            Position create = new Position(data.Latitude,data.Longitude);
            return create;
        }
    }
}
