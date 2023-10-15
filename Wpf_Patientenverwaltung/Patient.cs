using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Wpf_Patientenverwaltung
{
    public class Patient
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsMale { get; set; }
        public bool IsBedWetter { get; set; }
        public List<string> Diseases { get; set; }

        public Patient()
        {
            Firstname = "";
            Lastname = "";
            Diseases = new List<string>();
        }

        public static bool TryParse(string s, out Patient p)
        {
            string[]? split = s?.Split(',');

            if (split == null ||
                split.Length < 5)
            {
                p = null;
                return false;
            }

            p = new Patient();
            p.Firstname = split[0];
            p.Lastname = split[1];

            DateTime parsedBirthday;
            if(!DateTime.TryParse(split[2], out parsedBirthday))
            {
                p = null;
                return false;
            }
            p.Birthday = parsedBirthday;
            p.IsMale = !split[3].Contains("Female");
            p.IsBedWetter = !split[4].Contains("No");

            if(split.Length > 5)
            {
                List<string> diseases = split.ToList()
                                             .GetRange(5, split.Length - 5)
                                             .Where(d => !String.IsNullOrEmpty(d))
                                             .ToList();

                foreach (string d in diseases)
                {
                    p.Diseases.Add(d);
                }
            }

            return true;
        }

        public override string ToString()
        {
            string diseases = String.Join(", ", Diseases);
            return $"{Firstname} {Lastname} {Birthday.ToString("dd.MM.yyyy")} [{(IsMale ? "Male" : "Female")}] - {(IsBedWetter ? "" : "No ")}bed-wetter, {{ {diseases} }}";
        }

        public string ToString(bool asCSV)
        {
            string diseases = String.Join(",", Diseases);
            return $"{Firstname},{Lastname},{Birthday.ToString("dd.MM.yyyy")},{(IsMale ? "Male" : "Female")},{(IsBedWetter ? "Bed-wetter" : "No Bed-wetter")},{diseases}";
        }
    }
}
