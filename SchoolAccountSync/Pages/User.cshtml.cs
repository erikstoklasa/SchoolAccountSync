using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages
{
    public class UserModel : PageModel
    {
        private readonly BakalariService bakalariService;
        [BindProperty]
        public User User { get; set; }
        public string StatusMessage { get; set; }

        public UserModel(BakalariService bakalariService)
        {
            this.bakalariService = bakalariService;
            User = new User();
        }
        public async Task OnGet(string id)
        {
            User = await bakalariService.GetStudent(id);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            StatusMessage = "Updated user Rfid!";
            User = await bakalariService.GetStudent(User.Id);
            return Page();
        }
    }
}
