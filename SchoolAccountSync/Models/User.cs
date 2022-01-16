using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolAccountSync.Models
{
    public class User
    {
        public string Id { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateOnly Birthdate { get; set; }
        [MaxLength(4)]
        public string Class { get; set; }
        [EmailAddress]
        [MaxLength(35)]
        public string? SchoolEmail { get; set; }
        [EmailAddress]
        [MaxLength(35)]
        public string? PersonalEmail { get; set; }
        public int Rfid { get; set; }
        [MaxLength(4)]
        public string LockerNumber { get; set; }
        [MaxLength(20)]
        public string? TempPassword { get; set; }
        public StatusTypes Status { get; set; }
        public UserTypes UserType { get; set; }
        public string FullName() => FirstName + " " + LastName;
        public override string ToString()
        {
            return $"{Id},{FirstName},{LastName},{Birthdate},{Class},{SchoolEmail},{PersonalEmail},{Rfid},{LockerNumber},{Status},{UserType}";
        }
        public int GetAge()
        {
            int age = DateTime.Now.Year - Birthdate.Year;
            if (DateTime.Now.DayOfYear < Birthdate.DayOfYear)
            {
                age--;
            }
            return age;
        }
        public static string GenerateSchoolEmail(string firstName, string lastName, UserTypes userType)
        {
            if (userType == UserTypes.Student)
            {
                int lastNameMaxLength = 3;
                int firstNameMaxLength = 2;
                if (lastName.Length < lastNameMaxLength)
                {
                    lastNameMaxLength = lastName.Length;
                }
                if (firstName.Length < firstNameMaxLength)
                {
                    firstNameMaxLength = lastName.Length;
                }
                return RemoveDiacritic("x" + lastName[..lastNameMaxLength].ToLower() + firstName[..firstNameMaxLength].ToLower()) + "01@gjk.cz";
            }
            else
            {
                return RemoveDiacritic(lastName.ToLower()) + "@gjk.cz";
            }
        }
        public static string RemoveDiacritic(string input)
        {
            StringBuilder sb = new();
            Dictionary<char, char> letterPairs = new();
            letterPairs.Add('á', 'a');
            letterPairs.Add('č', 'c');
            letterPairs.Add('ď', 'd');
            letterPairs.Add('é', 'e');
            letterPairs.Add('ě', 'e');
            letterPairs.Add('í', 'i');
            letterPairs.Add('ň', 'n');
            letterPairs.Add('ó', 'o');
            letterPairs.Add('ř', 'r');
            letterPairs.Add('š', 's');
            letterPairs.Add('ť', 't');
            letterPairs.Add('ú', 'u');
            letterPairs.Add('ů', 'u');
            letterPairs.Add('ý', 'y');
            letterPairs.Add('ž', 'z');
            letterPairs.Add('ö', 'o');
            letterPairs.Add('ü', 'u');
            letterPairs.Add('ß', 's');
            letterPairs.Add('ä', 'a');
            letterPairs.Add('ą', 'a');
            letterPairs.Add('ć', 'c');
            letterPairs.Add('ę', 'e');
            letterPairs.Add('ł', 'l');
            letterPairs.Add('ń', 'n');
            letterPairs.Add('ś', 's');
            letterPairs.Add('ź', 'z');
            letterPairs.Add('ż', 'z');
            for (int i = 0; i < input.Length; i++)
            {
                if (letterPairs.ContainsKey(input[i]))
                {
                    sb.Append(letterPairs[input[i]]);
                }
                else
                {
                    sb.Append(input[i]);
                }
            }
            return sb.ToString();
        }

    }
    public enum StatusTypes { Normal, Abroad, ToDelete }
    public enum UserTypes { Teacher, Student }

}
