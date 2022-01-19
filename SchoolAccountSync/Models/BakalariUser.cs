using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolAccountSync.Models
{
    public class BakalariUser
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
        public string? PersonalEmail { get; set; }
        [MaxLength(4)]
        public string LockerNumber { get; set; }
        [MaxLength(20)]
        public StatusTypes Status { get; set; }
        public UserTypes UserType { get; set; }
    }
}
