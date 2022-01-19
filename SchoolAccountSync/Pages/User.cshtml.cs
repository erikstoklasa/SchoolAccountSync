using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages
{
    public class UserModel : PageModel
    {
        private readonly LocalUserService localUserService;

        [BindProperty]
        public User User { get; set; }
        public string SuccessMessage { get; set; }

        public UserModel(LocalUserService localUserService)
        {
            this.localUserService = localUserService;
            User = new User();
            SuccessMessage = "";
        }
        public async Task OnGet(string id)
        {
            User = await localUserService.GetUser(id);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            SuccessMessage = "Rfid úspěšně aktualozováno!";
            int rfid = User.Rfid;
            User = await localUserService.GetUser(User.Id);
            User.Rfid = rfid;
            await localUserService.UpdateUser(User);
            return Page();
        }
    }
}
