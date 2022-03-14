using SchoolAccountSync.Models;

namespace SchoolAccountSync.Services
{
    public class SyncService
    {
        private readonly LocalUserService localUserService;
        private readonly CopierService copierService;

        public SyncService(LocalUserService localUserService, CopierService copierService)
        {
            this.localUserService = localUserService;
            this.copierService = copierService;
        }
        /// <summary>
        /// Synchronizes local user with the copier database
        /// </summary>
        /// <param name="localUser"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when rfid or school email are null</exception>
        public async Task SyncLocalUserWithCopiers(LocalUser localUser)
        {
            if (localUser.Rfid == null)
            {
                throw new ArgumentException("Rfid can not be null", nameof(localUser.Rfid));
            }
            if (localUser.SchoolEmail == null)
            {
                throw new ArgumentException("School email can not be null", nameof(localUser.SchoolEmail));
            }

            string login;

            login = LocalUser.GenerateLogin(localUser.SchoolEmail);

            CopierUser? copierUser = await copierService.GetUser(localUser.Id);
            if (copierUser == null)
            {
                copierUser = await copierService.GetUserByLogin(login);
                if (copierUser == null)
                {
                    await copierService.AddUser(localUser);
                    return;
                }
            }
            if (!CompareService.IsSynced(copierUser, localUser))
            {
                copierUser.ExtId = localUser.Id;
                copierUser.FirstName = localUser.FirstName;
                copierUser.LastName = localUser.LastName;
                copierUser.Login = login;
                copierUser.SchoolEmail = localUser.SchoolEmail;
                copierUser.OuId = CopierUser.GenerateOuId(localUser.UserType);
                copierUser.FirstNameAscii = LocalUser.RemoveDiacritic(localUser.FirstName);
                copierUser.LastNameAscii = LocalUser.RemoveDiacritic(localUser.LastName);
                copierUser.LoginAscii = LocalUser.RemoveDiacritic(login);
                copierUser.TempPassword = localUser.TempPassword;

                await copierService.UpdateUser(copierUser);
                await copierService.DeleteCards(copierUser.Id);
                await copierService.AddCard(
                    new CopierCard
                    {
                        UserId = copierUser.Id,
                        CardId = localUser.Rfid
                    });
                return;
            }
        }
        public async Task SyncBakalariUserWithLocal(BakalariUser bakalariUser, IEnumerable<LocalUser> localUsers)
        {
            LocalUser? localUser = localUsers.FirstOrDefault(u => u.Id == bakalariUser.Id);
            if (localUser == null)
            {
                localUser = new LocalUser()
                {
                    Id = bakalariUser.Id,
                    FirstName = bakalariUser.FirstName,
                    LastName = bakalariUser.LastName,
                    Birthdate = bakalariUser.Birthdate,
                    PersonalEmail = bakalariUser.PersonalEmail,
                    SchoolEmail = LocalUser.GenerateSchoolEmail(bakalariUser.FirstName, bakalariUser.LastName, bakalariUser.UserType, localUsers),
                    Status = bakalariUser.Status,
                    UserType = bakalariUser.UserType,
                    LockerNumber = bakalariUser.LockerNumber,
                    Class = bakalariUser.Class,
                    TempPassword = LocalUser.GenerateTempPassword()
                };

                await localUserService.AddUser(localUser);
                return;
            };
            IEnumerable<Change> changes = CompareService.GetDifferences(oldUser: localUser, newUser: bakalariUser);
            if (changes.Any())
            {
                await localUserService.UpdateUser(
                    new LocalUser()
                    {
                        Id = bakalariUser.Id,
                        FirstName = bakalariUser.FirstName,
                        LastName = bakalariUser.LastName,
                        Birthdate = bakalariUser.Birthdate,
                        PersonalEmail = bakalariUser.PersonalEmail,
                        SchoolEmail = LocalUser.GenerateSchoolEmail(bakalariUser.FirstName, bakalariUser.LastName, bakalariUser.UserType, localUsers),
                        Status = bakalariUser.Status,
                        UserType = bakalariUser.UserType,
                        LockerNumber = bakalariUser.LockerNumber,
                        Class = bakalariUser.Class,
                    });
            }
        }
    }
}
