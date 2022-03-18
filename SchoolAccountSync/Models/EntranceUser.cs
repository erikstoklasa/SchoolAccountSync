using System.ComponentModel.DataAnnotations;

namespace SchoolAccountSync.Models
{
    public class EntranceUser
    {
        public int DataType { get; set; }
        public int IdDescribe { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Fdate { get; set; }
        [MaxLength(14)]
        public string FromDate { get; set; }
        [MaxLength(14)]
        public string ToDate { get; set; }
        public int IdUser { get; set; }
        public int IdUserLoc { get; set; }
        public IEnumerable<EntranceCard> EntranceCards { get; set; }
    }
}
