using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SchoolAccountSync.Pages
{
    public class Export : PageModel
    {
        private readonly LocalUserService localUserService;

        public Export(LocalUserService localUserService)
        {
            this.localUserService = localUserService;
        }
        public async Task<FileResult> OnGetCsvAsync()
        {
            StringBuilder sb = new();

            sb.AppendLine("id,jméno,příjmení,datum narození,třída,školní email,osobní email,rfid,skříňka,status,typ uživatele");
            foreach (var u in await localUserService.GetUsers())
            {
                sb.AppendLine(u.ToString());
            }
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", $"Export-{DateTime.Now}.csv");
        }
    }
}
