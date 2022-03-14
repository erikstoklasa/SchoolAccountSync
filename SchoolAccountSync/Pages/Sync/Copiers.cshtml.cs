using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages.Sync
{
    public class CopiersModel : PageModel
    {
        private readonly LocalUserService localUserService;
        private readonly CopierService copierService;
        private readonly SyncService syncService;

        public CopiersModel(LocalUserService localUserService, CopierService copierService, SyncService syncService)
        {
            this.localUserService = localUserService;
            this.copierService = copierService;
            this.syncService = syncService;
            NotSyncedUsers = new List<LocalUser>();
        }
        public int NumOfSyncedUsers { get; set; }
        public IList<LocalUser> NotSyncedUsers { get; set; }
        public int NumOfUsersNotAbleToSync { get; set; }
        public async Task OnGetAsync()
        {
            IEnumerable<LocalUser> localUsers = await localUserService.GetUsers();
            foreach (var user in localUsers)
            {
                if (user.Rfid == null)
                {
                    NumOfUsersNotAbleToSync++;
                    continue;
                };
                if (user.SchoolEmail == null)
                {
                    NumOfUsersNotAbleToSync++;
                    continue;
                };
                CopierUser? copierUser = await copierService.GetUser(user.Id);
                if (copierUser == null)
                {
                    string login;
                    try
                    {
                        login = LocalUser.GenerateLogin(user.SchoolEmail);
                    }
                    catch (ArgumentException)
                    {
                        NumOfUsersNotAbleToSync++;
                        continue;
                    }
                    copierUser = await copierService.GetUserByLogin(login);
                    if (copierUser == null)
                    {
                        NotSyncedUsers.Add(user);
                        continue;
                    }
                }
                if (!CompareService.IsSynced(copierUser, user))
                {
                    NotSyncedUsers.Add(user);
                    continue;
                }
                NumOfSyncedUsers++;
                continue;
            }
        }
        public async Task<IActionResult> OnPostConfirmSyncAsync()
        {
            IEnumerable<LocalUser> localUsers = await localUserService.GetUsers();
            foreach (var localUser in localUsers)
            {
                if (localUser.SchoolEmail == null) continue;
                if (localUser.Rfid == null) continue;
                await syncService.SyncLocalUserWithCopiers(localUser);
            }
            return RedirectToPage("./Copiers");
        }
    }
}
