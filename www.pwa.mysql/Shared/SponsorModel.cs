
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace www.pwa.Shared {

    public class CreateSponsoresModel {
        [Required(ErrorMessage = "Das Feld Pseudonym wird benötigt.")]
        [CustomValidation(typeof(EntityRunFormData), nameof(EntityRunFormData.ValidatePseudonym))]
        public string EntityName { get; set; }
        [Required(ErrorMessage = "Das Feld Klasse wird benötigt.")]
        public string SchoolClass { get; set; }
        public string School { get; set; }
        public string WalkGuid { get; set; }
        public List<SponsorModel> Sponsors { get; set; }
        [Required(ErrorMessage = "Das Feld Passwort wird benötigt.")]
        public string Credential { get; set; }

        public static ValidationResult ValidatePseudonym(string pseudonym, ValidationContext vc)
        {
            return pseudonym.Length < 17 && Char.IsDigit(pseudonym.First()) && !pseudonym.Contains(' ')
                ? ValidationResult.Success
                : new ValidationResult("ungültiges Pseudonym (maximal 16 Zeichen, muss mit einer Zahl beginnen, keine Leerzeichen)", new[] { vc.MemberName });
        }
    }

    public class SponsorModel {
        [Required(ErrorMessage = "Das Feld Sponsor wird benötigt.")]
        public string Sponsor { get; set; }
        [Range(0.01, 100, ErrorMessage = "Mindestens 0.01, höchstens 100.")]
        public double CentPerKm { get; set; }

        public static bool Validate<T>(T obj, out ICollection<string> results)
        {
            results = new List<string>();
            var valresults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, new ValidationContext(obj), valresults, true);
            if (result == false)
                results = valresults.Select(s => s.ErrorMessage).ToList();
            return result;
        }
    }
}