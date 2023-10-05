using System;

namespace Wpf_Patientenverwaltung
{
    public class Patient
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsMale { get; set; }
        public bool IsBedWetter { get; set; }

        public static bool TryParse(string s, out Patient p)
        {
            string[]? split = s?.Split(' ');

            if (split == null ||
                split.Length < 6)
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
            p.IsMale = !split[3].Contains("[Female]");
            p.IsBedWetter = !split[5].Contains("No");

            return true;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} [{3}] - {4}bedwetter",
                                 Firstname, Lastname, Birthday.ToString("dd.MM.yyyy"), IsMale ? "Male" : "Female", IsBedWetter ? "" : "No ");
        }
    }
}
