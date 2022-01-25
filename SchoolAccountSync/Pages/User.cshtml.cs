using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages
{
    public class UserModel : PageModel
    {
        private readonly LocalUserService localUserService;
        private readonly CopierService copierService;

        [BindProperty]
        public LocalUser User { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSyncedWithCopiers { get; set; }

        public UserModel(LocalUserService localUserService, CopierService copierService)
        {
            this.localUserService = localUserService;
            this.copierService = copierService;
            User = new LocalUser();
            SuccessMessage = "";
            ErrorMessage = "";
        }
        public async Task<IActionResult> OnGet(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            User = await localUserService.GetUser(id);
            CopierUser? copierUser = await copierService.GetUser(id);
            if (copierUser == null)
            {
                IsSyncedWithCopiers = false;
                return Page();
            }
            IsSyncedWithCopiers = CompareService.Equals(copierUser, User);
            if (copierUser.CopierCards.Count == 0 || copierUser.CopierCards.Count > 1)
            {
                IsSyncedWithCopiers = false;
            }
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            SuccessMessage = "Rfid úspěšně aktualozováno!";
            string? rfid = User.Rfid;
            User = await localUserService.GetUser(User.Id);
            User.Rfid = rfid;
            await localUserService.UpdateUser(User);
            return Page();
        }
        public async Task<IActionResult> OnPostSyncToCopiersAsync()
        {
            if (User.Id == null)
            {
                ErrorMessage = "Uživatel musí mít ID";
                return RedirectToPage("./Index");
            }
            User = await localUserService.GetUser(User.Id);
            if (User.Rfid == null)
            {
                ErrorMessage = "Uživatel musí mít Rfid";
                return RedirectToPage("./User", new { User.Id , ErrorMessage });
            }
            CopierUser? copierUser = await copierService.GetUser(User.Id);
            while(copierUser != null)
            {
                await copierService.DeleteUserWithCards(copierUser.Id);
                copierUser = await copierService.GetUser(User.Id);
            }
            await copierService.AddUser(User);
            return RedirectToPage("./User", new { User.Id });
        }
    }
}
