using System;
using System.Collections.Generic;

namespace WorldWideWalk.Models
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
        public double MaxSpeedInKmH { get; set; }
        public double MaxDuration { get; set; }
        public (WalkPoints, WalkPoints) GetNextAndCurrentPoint()
        {
            double addDistance = 0;
            NextTarget = String.Empty;
            WalkPoints NextPoint = null;
            WalkPoints LastPoint = null;
            WalkPoints CurrentPoint = null;
            foreach (var point in Points)
            {
                if (point.Distance > CurrentDistance)
                {
                    NextPoint = point;
                    NextTarget = NextPoint.Name;
                    addDistance = CurrentDistance - point.Distance;
                    break;
                }
                LastPoint = point;
            }

            if (NextPoint != null)
            {
                var d = WalkAppModel.GetDistance(LastPoint.Latitude, LastPoint.Longitude, NextPoint.Latitude, NextPoint.Longitude);
                var t = ((addDistance * 1000) / d) * -1;
                var x = ((1 - t) * LastPoint.Latitude + t * NextPoint.Latitude);
                var y = ((1 - t) * LastPoint.Longitude + t * NextPoint.Longitude);
                CurrentPoint = new WalkPoints()
                {
                    Name = "Current",
                    Latitude = x,
                    Longitude = y,
                    Distance = CurrentDistance
                };
            }
            return (NextPoint, CurrentPoint);
        }

        public static double GetDistance(double lat1, double long1, double lat2, double long2)
        {
            var d1 = lat1 * (Math.PI / 180.0);
            var num1 = long1 * (Math.PI / 180.0);
            var d2 = lat2 * (Math.PI / 180.0);
            var num2 = long2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
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
