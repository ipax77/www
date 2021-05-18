using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www.pwa.Shared
{
    public class RunInfo
    {
        public string Ent { get; set; }
        public float Dist { get; set; }
        public int Count { get; set; }
    }

    public class EntRunInfo
    {
        public float Dist { get; set; }
        public string Date { get; set; }
    }

    public class RunEntry
    {
        public string Entity { get; set; }
        public float Distance { get; set; }
        public DateTime Time { get; set; }
        public string Credential { get; set; }
    }

    public class EntityRunFormData
    {
        public string Walk { get; set; }
        public string School { get; set; }

        [Required(ErrorMessage = "Das Feld Pseudonym wird benötigt")]
        [CustomValidation(typeof(EntityRunFormData), nameof(EntityRunFormData.ValidatePseudonym))]
        public string Identifier { get; set; }

        [Required(ErrorMessage = "Das Feld Klasse wird beötigt")]
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
    }

    public class WwwWalkInfo
    {
        public float CurrentDistance { get; set; }
        public string NextTarget { get; set; }
    }

    public class WwwFeedback
    {
        public int EntPosition { get; set; }
        public float EntPercentage { get; set; }
        public float EntTotal { get; set; }
        public int ClassPosition { get; set; }
        public float ClassPercentage { get; set; }
        public float ClassTotal { get; set; }
        public int YearPosition { get; set; }
        public float YearPercentage { get; set; }
        public float YearTotal { get; set; }
        public int SchoolPosition { get; set; }
        public float SchoolPercentage { get; set; }
        public float SchoolTotal { get; set; }
        public string Error { get; set; }

    }

    public class WwwCreate
    {
        public string Name { get; set; } = String.Empty;
        public string SchoolName { get; set; } = String.Empty;
        public List<WwwPoint> Points { get; set; } = new List<WwwPoint>();
        public float TotalDistance { get; set; }
    }

    public class WwwPathPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class WwwPoint
    {
        public string Name { get; set; } = String.Empty;
        public decimal X { get; set; } = 0;
        public decimal Y { get; set; } = 0;
    }

    public class WwwChartInfo
    {
        public List<string> Lables { get; set; }
        public List<double> Data { get; set; }
    }

    public class WwwInterest
    {
        public string Name { get; set; }
        public float Distance { get; set; }
        public float[] Percentage { get; set; } = new float[3] { 0, 0, 0 };
    }
}
