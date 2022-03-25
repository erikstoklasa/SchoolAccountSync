using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;
using System.Data.SqlClient;

namespace SchoolAccountSync.Pages.Sync
{
    public class EntranceModel : PageModel
    {
        private readonly EntranceService entranceService;
        private readonly LocalUserService localUserService;

        public EntranceModel(EntranceService entranceService, LocalUserService localUserService)
        {
            this.entranceService = entranceService;
            this.localUserService = localUserService;
            ErrorMessage = "";
        }
        public string ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {

        }
        public async Task<IActionResult> OnPostSynchronizeAsync()
        {
            IEnumerable<LocalUser> localUsers = await localUserService.GetUsers();
            foreach (var user in localUsers)
            {
                if (user.Rfid == null) continue;
                try
                {
                    await entranceService.AddUserAsync(new()
                    {
                        EntranceCards = new List<EntranceCard>() { new EntranceCard() { RfidDecimal = Convert.ToInt32(user.Rfid, 16).ToString() } },
                    });
                }
                catch (SqlException ex)
                {
                    ErrorMessage = user + " " + ex.Message;
                    return Page();
                }

            }
            return Page();
        }
    }
}
