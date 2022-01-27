using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly LocalUserService localUserService;

        public IEnumerable<LocalUser> Users { get; set; }
        public IndexModel(ILogger<IndexModel> logger, LocalUserService localUserService)
        {
            _logger = logger;
            this.localUserService = localUserService;
            Users = new List<LocalUser>();
            ErrorMessage = "";
        }

        [BindProperty(SupportsGet = true)]
        public string? ClassFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? RfidFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? NameFilter { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (NameFilter != null)
            {
                Users = (await localUserService.GetUsers()).Where(u => u.FullName().ToLower().Contains(NameFilter.ToLower()));
            }
            else if (RfidFilter != null)
            {
                Users = await localUserService.GetUsersByRfid(RfidFilter);
            }
            else if (ClassFilter != null)
            {
                Users = await localUserService.GetUsersByClass(ClassFilter);
            }
            else
            {
                try
                {
                    Users = await localUserService.GetUsers();
                }
                catch (NpgsqlException ex)
                {
                    ErrorMessage = "Connection to the PostgreSQL server could not be established.";
                }
            }
        }
    }
}