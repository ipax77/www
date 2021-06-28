
using System;
using System.Collections.Generic;
using System.Linq;

namespace www.pwa.Shared
{
    public class WalkAppModel
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Description { get; set; }
        public double TotalDistance { get; set; }
        public double CurrentDistance { get; set; }
        public string NextTarget { get; set; }
        public List<School> Schools { get; set; }
        public static TimeSpan MaxRunTime { get; set; }
        public List<WalkPoints> Points { get; set; }
        public double MaxSpeedInKmH { get; set; } = 40;
        public double MaxDuration { get; set;} = 2;

        public (WalkPoints, WalkPoints) GetNextAndCurrentPoint()
        {
            double addDistance = 0;
            NextTarget = String.Empty;
            WalkPoints NextPoint = null;
            WalkPoints LastPoint = null;
            WalkPoints CurrentPoint = null;
            var currentDistance = CurrentDistance;
            if (CurrentDistance > Points.Last().Distance)
                currentDistance = Points.Last().Distance - 1;
                
            foreach (var point in Points)
            {
                if (point.Distance > currentDistance)
                {
                    NextPoint = point;
                    NextTarget = NextPoint.Name;
                    addDistance = currentDistance - LastPoint.Distance;
                    break;
                }
                LastPoint = point;
            }

            if (NextPoint != null)
            {
                var d = RunService.GetDistance(LastPoint.Latitude, LastPoint.Longitude, NextPoint.Latitude, NextPoint.Longitude);
                var t = ((addDistance * 1000) / d);
                var x = ((1 - t) * LastPoint.Latitude + t * NextPoint.Latitude);
                var y = ((1 - t) * LastPoint.Longitude + t * NextPoint.Longitude);
                CurrentPoint = new WalkPoints()
                {
                    Name = "Current",
                    Latitude = x,
                    Longitude = y,
                    Distance = currentDistance
                };
            }
            return (NextPoint, CurrentPoint);
        }
    }

    public class School
    {
        public string Name { get; set; }
        public List<SchoolClass> SchoolClasses { get; set; }
    }

    public class SchoolClass
    {
        public string Name { get; set; }
    }

    public class WalkPoints
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
    }
}