using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;

namespace SchoolAccountSync.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BakalariService studentService;
        public ICollection<User> Users { get; set; }

        public IndexModel(ILogger<IndexModel> logger, BakalariService studentService)
        {
            _logger = logger;
            this.studentService = studentService;
        }

        public async Task OnGet()
        {
            Users = await studentService.GetStudents();
        }
    }
}