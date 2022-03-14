using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;
using System.Data.SqlClient;

namespace SchoolAccountSync.Pages.Sync
{
    public class BakalariModel : PageModel
    {
        private readonly BakalariUserService bakalariUserService;
        private readonly LocalUserService localUserService;
        private readonly SyncService syncService;

        public BakalariModel(BakalariUserService bakalariUserService,
                             LocalUserService localUserService,
                             SyncService syncService)
        {
            this.bakalariUserService = bakalariUserService;
            this.localUserService = localUserService;
            this.syncService = syncService;
            UsersWithChanges = new List<(LocalUser, IEnumerable<Change>)>();
            ErrorMessage = "";
            SuccessMessage = "";
            Users = new List<LocalUser>();
            UsersToAdd = new List<LocalUser>();
        }
        public IEnumerable<LocalUser> Users { get; set; }
        public List<(LocalUser, IEnumerable<Change>)> UsersWithChanges { get; set; }
        public IList<LocalUser> UsersToAdd { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                IEnumerable<BakalariUser> masterList = await bakalariUserService.GetStudents();
                Users = await localUserService.GetUsers();

                foreach (var bakaUser in masterList)
                {
                    LocalUser? localUser = Users.FirstOrDefault(u => u.Id == bakaUser.Id);
                    if (localUser == null)
                    {
                        UsersToAdd.Add(
                            new LocalUser()
                            {
                                Id = bakaUser.Id,
                                FirstName = bakaUser.FirstName,
                                LastName = bakaUser.LastName,
                                Birthdate = bakaUser.Birthdate,
                                PersonalEmail = bakaUser.PersonalEmail,
                                SchoolEmail = LocalUser.GenerateSchoolEmail(bakaUser.FirstName, bakaUser.LastName, bakaUser.UserType, Users),
                                Status = bakaUser.Status,
                                UserType = bakaUser.UserType,
                                LockerNumber = bakaUser.LockerNumber,
                                Class = bakaUser.Class,
                            });
                        continue;
                    };
                    IEnumerable<Change> changes = CompareService.GetDifferences(oldUser: localUser, newUser: bakaUser);
                    if (changes.Any())
                    {
                        UsersWithChanges.Add(new(localUser, changes));
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = ex.Message;
            };
        }
        public async Task OnGetConfirmChangesAsync()
        {
            try
            {
                IEnumerable<BakalariUser> masterList = await bakalariUserService.GetStudents();
                Users = await localUserService.GetUsers();
                foreach (var bakaUser in masterList)
                {
                    await syncService.SyncBakalariUserWithLocal(bakaUser, Users);
                }
                SuccessMessage = $"Úspěšně synchronizováno!";
            }
            catch (SqlException ex)
            {
                ErrorMessage = ex.Message;
            };
        }
    }
}
