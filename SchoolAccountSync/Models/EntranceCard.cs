using System.ComponentModel.DataAnnotations;

namespace SchoolAccountSync.Models
{
    public class EntranceCard
    {
        [MaxLength(20)]
        public string RfidDecimal { get; set; }
        public char TyTarget { get; set; }
        public int IdGroup { get; set; }
        [MaxLength(8)]
        public string IdReader { get; set; }
        [MaxLength(14)]
        public string ToDate { get; set; }
        [MaxLength(14)]
        public string? FromDate { get; set; }
        public int DataType { get; set; }
        public int TimeZoneNum { get; set; }
        public int? IdUserLoc { get; set; }
        public int? IdUser { get; set; }

    }
}
