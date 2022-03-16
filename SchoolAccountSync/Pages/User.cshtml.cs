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
        private readonly SyncService syncService;

        [BindProperty]
        public LocalUser? User { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SuccessMessage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ErrorMessage { get; set; }
        public bool IsSyncedWithCopiers { get; set; }

        public UserModel(LocalUserService localUserService,
                         CopierService copierService,
                         SyncService syncService)
        {
            this.localUserService = localUserService;
            this.copierService = copierService;
            this.syncService = syncService;
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
            if (User == null) return NotFound();

            CopierUser? copierUser = await copierService.GetUser(id);
            if (copierUser == null)
            {
                IsSyncedWithCopiers = false;
                return Page();
            }
            IsSyncedWithCopiers = CompareService.IsSynced(copierUser, User);
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (User == null) return BadRequest();
            string? rfid = User.Rfid;
            User = await localUserService.GetUser(User.Id);
            if (User == null) return NotFound();
            User.Rfid = rfid;
            await localUserService.UpdateUser(User);
            SuccessMessage = "Rfid úspěšně aktualozováno!";
            return Page();
        }
        public async Task<IActionResult> OnPostSyncToCopiersAsync()
        {
            if (User?.Id == null)
            {
                ErrorMessage = "Uživatel musí mít ID, Error: ID10T";
                return RedirectToPage("./Index", new { ErrorMessage });
            }
            User = await localUserService.GetUser(User.Id);
            if (User == null) return NotFound();
            try
            {
                await syncService.SyncLocalUserWithCopiers(User);
            }
            catch (ArgumentException)
            {
                ErrorMessage = "Uživatel musí mít Rfid a validní školní email";
                return RedirectToPage("./User", new { User.Id, ErrorMessage });
            }
            return RedirectToPage("./User", new { User.Id });
        }
    }
}
