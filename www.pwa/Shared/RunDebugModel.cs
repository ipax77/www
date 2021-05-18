
using System;
using System.Collections.Generic;

namespace www.pwa.Shared
{
    public class RunDebugModel
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<RunDebugItemModel> RunDebugItems { get; set; }
        public List<string> Errors { get; set; }
    }

    public class RunDebugItemModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public double TimeStamp { get; set; }
        public double Speed { get; set; }
    }
}