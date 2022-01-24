using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages.Sync
{
    public class RecoverDataFromCopiersModel : PageModel
    {
        private readonly LocalUserService localUserService;
        private readonly CopierService copierService;

        public RecoverDataFromCopiersModel(LocalUserService localUserService, CopierService copierService)
        {
            this.localUserService = localUserService;
            this.copierService = copierService;
        }
        public int NumberOfNullRfids { get; set; }
        public async Task OnGetAsync()
        {
            foreach (var localUser in await localUserService.GetUsers())
            {
                if (localUser.Rfid == null)
                {
                    NumberOfNullRfids++;
                }
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            foreach (var localUser in await localUserService.GetUsers())
            {
                if (localUser.Rfid == null)
                {
                    CopierUser? copierUser = await copierService.GetUser(localUser.Id);
                    if (copierUser == null) continue;
                    if (copierUser.CopierCards.Any())
                    {
                        localUser.Rfid = copierUser.CopierCards[0].CardId;
                        await localUserService.UpdateUser(localUser);
                    } else
                    {
                        //Get the copier users based on the login instead of the ext_id
                        if (localUser.SchoolEmail == null) continue;
                        string login = localUser.SchoolEmail.Split("@")[0];
                        copierUser = await copierService.GetUserByLogin(login);
                        if (copierUser == null) continue;
                        if (copierUser.CopierCards.Any())
                        {
                            localUser.Rfid = copierUser.CopierCards[0].CardId;
                            await localUserService.UpdateUser(localUser);
                        }
                    }
                }
            }
                return RedirectToPage("./RecoverDataFromCopiers");
        }
    }
}
