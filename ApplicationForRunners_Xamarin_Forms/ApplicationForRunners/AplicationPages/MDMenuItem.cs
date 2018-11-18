using System;

namespace ApplicationForRunners.AplicationPages
{
    public class MDMenuItem
    {
        public MDMenuItem()
        {
            TargetType = typeof(MapPage);
            OlwaysActive = false;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public Type TargetType { get; set; }
        public bool OlwaysActive { get; set; }
    }
}