using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages.Sync
{
    public class BulkRfidInsertModel : PageModel
    {
        private readonly LocalUserService localUserService;

        public int NumOfUsersWithoutRfid { get; set; }
        [BindProperty]
        public LocalUser localUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SuccessMessage { get; set; }
        public BulkRfidInsertModel(LocalUserService localUserService)
        {
            this.localUserService = localUserService;
            localUser = new(); 
            SuccessMessage = "";
        }
        public async Task OnGetAsync()
        {
            IEnumerable<LocalUser> localUsersWithoutRfid = (await localUserService.GetUsers())
                .Where(u => u.Rfid == null)
                .OrderBy(u => u.Class);
            NumOfUsersWithoutRfid = localUsersWithoutRfid.Count();
            if (NumOfUsersWithoutRfid > 0)
            {
                localUser = localUsersWithoutRfid.First();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string? rfid = localUser.Rfid;
            localUser = await localUserService.GetUser(localUser.Id);
            SuccessMessage = $"{localUser.FullName()} - úspěšně aktualizováno Rfid.";
            localUser.Rfid = rfid;
            await localUserService.UpdateUser(localUser);
            return RedirectToPage("./BulkRfidInsert", new { SuccessMessage });
        }
    }
}
