namespace SchoolAccountSync.Models
{
    public class CopierUser
    {
        public long Id { get; set; }
        public string ExtId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Login { get; set; }
        public string SchoolEmail { get; set; }
        public long? OuId { get; set; }
        public string? FirstNameAscii { get; set; }
        public string? LastNameAscii { get; set; }
        public string LoginAscii { get; set; }
        public string? TempPassword { get; set; }
        public IList<CopierCard> CopierCards { get; set; }
    }
}
