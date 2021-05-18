using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace WorldWideWalk.Models
{
    public class ColorLine
    {
        public string Color { get; set; }
        public List<double[]> Line { get; set; }

        public string GetJsonString()
        {
            return JsonSerializer.Serialize(Line, new JsonSerializerOptions() { WriteIndented = true });
        }
    }
}
