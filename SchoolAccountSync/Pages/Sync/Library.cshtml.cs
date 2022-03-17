using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages.Sync
{
    public class LibraryModel : PageModel
    {
        private readonly LibraryService libraryService;
        private readonly LocalUserService localUserService;

        public LibraryModel(LibraryService libraryService, LocalUserService localUserService)
        {
            this.libraryService = libraryService;
            this.localUserService = localUserService;
        }
        public void OnGet()
        {

        }
        public async Task OnPostSynchronizeAsync()
        {
            IEnumerable<LocalUser> localUsers = await localUserService.GetUsers();
            foreach (var user in localUsers)
            {
                if (user.Rfid == null) continue;
                await libraryService.UpdateUser(user.Id, user.Rfid);
            }
        }
    }
}
