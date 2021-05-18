using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideWalk.Models
{
    public class Walk
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double TotalDistance { get; set; }
        public double CurrentDistance { get; set; }
        public string NextTarget { get; set; }
        public List<School> Schools { get; set; }
        public static TimeSpan MaxRunTime { get; set; }
        public double MaxSpeedInKmH { get; set; }
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
}
