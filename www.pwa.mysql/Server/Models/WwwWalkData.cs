using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace www.pwa.Server.Models
{
    public class WwwWalkData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Position { get; set; }
        public double Distance { get; set; }
    }
}