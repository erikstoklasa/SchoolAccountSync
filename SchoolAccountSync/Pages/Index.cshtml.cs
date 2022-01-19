using Microsoft.AspNetCore.Mvc.RazorPages;
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
        }

        public async Task OnGetAsync()
        {
            Users = await localUserService.GetUsers();
        }
    }
}