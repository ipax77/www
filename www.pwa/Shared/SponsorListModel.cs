
using System;
namespace www.pwa.Shared {
    public class SponsorListModel {
        public int ID { get; set; }
        public string Pseudonym { get; set; }
        public string Klasse { get; set; }
        public string Sponsor { get; set; }
        public double CentPerKm { get; set; }
        public bool Verified { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class SponsorRequest {
        public int Skip {get; set;}
        public int Take {get; set;}
        public string Interest { get; set; }
        public string Klasse {get; set;}
        public bool Order {get; set;} = true;
        public string Search {get; set;}  
        public int isVerified { get; set; } = (int)TableBoolRequest.All;
    }

    public enum TableBoolRequest {
        Include,
        Exclude,
        All
    }
}