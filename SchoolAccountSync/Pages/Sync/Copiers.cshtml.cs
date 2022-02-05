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

        public CopiersModel(LocalUserService localUserService, CopierService copierService)
        {
            this.localUserService = localUserService;
            this.copierService = copierService;
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
                        login = CopierUser.GenerateLogin(user.SchoolEmail);
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
                if (copierUser.CopierCards.Count == 0 || copierUser.CopierCards.Count > 1)
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
                if (localUser.Rfid == null)
                {
                    continue;
                };
                if (localUser.SchoolEmail == null)
                {
                    continue;
                };
                string login = CopierUser.GenerateLogin(localUser.SchoolEmail);
                CopierUser? copierUser = await copierService.GetUser(localUser.Id);
                if (copierUser == null)
                {
                    copierUser = await copierService.GetUserByLogin(login);
                    if (copierUser == null)
                    {
                        await copierService.AddUser(localUser);
                        continue;
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
                    continue;
                }
                if (copierUser.CopierCards.Count == 0)
                {
                    await copierService.AddCard(
                        new CopierCard
                        {
                            UserId = copierUser.Id,
                            CardId = localUser.Rfid
                        });
                    continue;
                }
                if (copierUser.CopierCards.Count > 1)
                {
                    await copierService.DeleteCards(copierUser.Id);
                    await copierService.AddCard(
                        new CopierCard
                        {
                            UserId = copierUser.Id,
                            CardId = localUser.Rfid
                        });
                    continue;
                }
            }
            return RedirectToPage("./Copiers");
        }
    }
}
