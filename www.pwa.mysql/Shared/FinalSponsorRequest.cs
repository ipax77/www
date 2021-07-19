using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www.pwa.Shared
{
    public class FinalSponsorRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Interest { get; set; }
        public bool Order { get; set; } = true;
        public string Search { get; set; }
    }
}
