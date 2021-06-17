using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WorldWideWalk.Models
{
    public class EntityRunFormData
    {
        public string Walk { get; set; }
        public string School { get; set; }

        [Required(ErrorMessage = "Das Feld Pseudonym wird benötigt")]
        [CustomValidation(typeof(EntityRunFormData), nameof(EntityRunFormData.ValidatePseudonym))]
        public string Identifier { get; set; }

        [Required(ErrorMessage = "Das Feld Klasse wird benötigt")]
        public string SchoolClass { get; set; }

        [Required(ErrorMessage = "Das Feld Strecke wird benötigt")]
        [Range(0.1f, 42.195f, ErrorMessage = "Stecke ungültig (0.1-42.195)")]
        public float Distance { get; set; }

        [Required(ErrorMessage = "Das Feld Datum wird benötigt")]
        public DateTime Time { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "Das Feld Passwort wird benötigt")]
        [CustomValidation(typeof(EntityRunFormData), nameof(EntityRunFormData.ValidateCredential))]
        public string Credential { get; set; }

        public static ValidationResult ValidatePseudonym(string pseudonym, ValidationContext vc)
        {
            return pseudonym.Length < 17 && Char.IsDigit(pseudonym.First()) && !pseudonym.Contains(' ')
                ? ValidationResult.Success
                : new ValidationResult("ungültiges Pseudonym (maximal 16 Zeichen, muss mit einer Zahl beginnen, keine Leerzeichen)", new[] { vc.MemberName });
        }

        public static ValidationResult ValidateCredential(string credential, ValidationContext vc)
        {
            return ValidationResult.Success;
            //return credential == WwwData.Credential
            //    ? ValidationResult.Success
            //    : new ValidationResult("ungültiges Passwort", new[] { vc.MemberName });
        }

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
