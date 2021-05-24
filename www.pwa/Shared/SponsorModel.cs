
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace www.pwa.Shared {

    public class CreateSponsoresModel {
        [Required(ErrorMessage = "Das Feld Name wird benötigt.")]
        public string EntityName { get; set; }
        [Required(ErrorMessage = "Das Feld Klasse wird benötigt.")]
        public string SchoolClass { get; set; }
        public string School { get; set; }
        public string WalkGuid { get; set; }
        public List<SponsorModel> Sponsors { get; set; }
    }

    public class SponsorModel {
        [Required(ErrorMessage = "Das Feld Sponsor wird benötigt.")]
        public string Sponsor { get; set; }
        [Range(0.01, 100, ErrorMessage = "Mindestens 0.01, höchstens 100.")]
        public double CentPerKm { get; set; }
    }
}