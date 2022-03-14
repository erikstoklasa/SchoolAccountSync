namespace SchoolAccountSync.Models
{
    public class CopierUser
    {
        /// <summary>
        /// Initializes the copier user object
        /// </summary>
        /// <param name="localUser"></param>
        /// <exception cref="ArgumentException">Thrown when school email is null</exception>
        public CopierUser(LocalUser localUser)
        {
            ExtId = localUser.Id;
            FirstName = localUser.FirstName;
            LastName = localUser.LastName;
            if (localUser.SchoolEmail == null)
            {
                throw new ArgumentException("Argument can not be null", nameof(localUser.SchoolEmail));
            }
            Login = LocalUser.GenerateLogin(localUser.SchoolEmail);
            SchoolEmail = localUser.SchoolEmail;
            OuId = GenerateOuId(localUser.UserType);
            FirstNameAscii = LocalUser.RemoveDiacritic(localUser.FirstName);
            LastNameAscii = LocalUser.RemoveDiacritic(localUser.LastName);
            LoginAscii = LocalUser.RemoveDiacritic(Login);
            TempPassword = localUser.TempPassword;
        }
        public CopierUser()
        {

        }
        public long Id { get; set; }
        public string? ExtId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Login { get; set; }
        public string SchoolEmail { get; set; }
        public long? OuId { get; set; }
        public string? FirstNameAscii { get; set; }
        public string? LastNameAscii { get; set; }
        public string LoginAscii { get; set; }
        public string? TempPassword { get; set; }
        public IList<CopierCard>? CopierCards { get; set; }
        public static long GenerateOuId(UserTypes userType) => userType switch
        {
            UserTypes.Teacher => 1,
            UserTypes.Student => 2,
            _ => 2,
        };

    }
}
