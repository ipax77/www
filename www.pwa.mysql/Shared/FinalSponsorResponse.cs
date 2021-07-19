using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www.pwa.Shared
{
    public class FinalSponsorResponse
    {
        public string Pseudonym {  get; set; }
        public double Distance { get; set; }
    }

    public class FinalSponsorInfoResponse
    {
        public string Sponsor {  get; set; }
        public double CentPerKm { get; set; }
    }
}
