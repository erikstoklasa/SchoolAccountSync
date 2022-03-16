using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolAccountSync.Models
{
    public class LocalUser
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
        [MaxLength(8)]
        [RegularExpression("^[a-fA-F0-9]+$", ErrorMessage = "Rfid musí být zadáno v hexadecimální soustavě.")]
        public string? Rfid { get; set; }
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
        /// <summary>
        /// Generates the school email
        /// </summary>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="userType">User type</param>
        /// <param name="duplicateString">Used for conflicting school emails</param>
        /// <returns>Valid school email</returns>
        public static string GenerateSchoolEmail(string firstName, string lastName, UserTypes userType, IEnumerable<LocalUser> localUsers)
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
                string output = "x" + lastName[..lastNameMaxLength].ToLower() + firstName[..firstNameMaxLength].ToLower() + "0";
                int differentiatorIndex = 1;
                while (localUsers.Any(u =>
                {
                    if (u.SchoolEmail == null) return false;
                    try
                    {
                        return GenerateLogin(u.SchoolEmail) == output + differentiatorIndex;
                    }
                    catch (ArgumentException)
                    {
                        return false;
                    }
                }))
                {
                    differentiatorIndex++;
                }
                output += differentiatorIndex;
                output += "@gjk.cz";
                return RemoveDiacritic(output);
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
            letterPairs.Add('Č', 'C');
            letterPairs.Add('ď', 'd');
            letterPairs.Add('é', 'e');
            letterPairs.Add('ě', 'e');
            letterPairs.Add('í', 'i');
            letterPairs.Add('ň', 'n');
            letterPairs.Add('Ň', 'N');
            letterPairs.Add('ó', 'o');
            letterPairs.Add('ř', 'r');
            letterPairs.Add('Ř', 'R');
            letterPairs.Add('š', 's');
            letterPairs.Add('Š', 's');
            letterPairs.Add('ť', 't');
            letterPairs.Add('Ť', 'T');
            letterPairs.Add('ú', 'u');
            letterPairs.Add('ů', 'u');
            letterPairs.Add('ý', 'y');
            letterPairs.Add('ž', 'z');
            letterPairs.Add('Ž', 'Z');
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
        /// <summary>
        /// Gets the login from the username part of the email string
        /// </summary>
        /// <param name="schoolEmail"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when provided email is not valid</exception>
        public static string GenerateLogin(string schoolEmail)
        {
            string[] splitString = schoolEmail.Split("@");
            if (splitString.Length > 1)
            {
                return splitString[0];
            }
            else
            {
                throw new ArgumentException("Provided string did not contain an @ symbol", nameof(schoolEmail));
            }
        }
        public static string GenerateTempPassword()
        {
            Random random = new();
            char[] letters = new char[] { 'a','b','c','d','e','f','g','h','i','j','k','l','m','p','q','r','s','t','u','v','w','x','y','z',
                '1','2','3','4','5','6','7','8','9','0'
            };
            string output = "";
            for (int i = 0; i < 8; i++)
            {
                output += letters[random.Next(letters.Length)];
            }
            return output;
        }

    }
    public enum StatusTypes { Normal, Abroad, ToDelete }
    public enum UserTypes { Teacher, Student }

}
