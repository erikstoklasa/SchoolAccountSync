using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;
using System.Data.SqlClient;

namespace SchoolAccountSync.Pages.Sync
{
    public class BakalariModel : PageModel
    {
        private readonly BakalariService studentService;
        private readonly LocalUserService localUserService;

        public BakalariModel(BakalariService studentService, LocalUserService localUserService)
        {
            this.studentService = studentService;
            this.localUserService = localUserService;
            UsersWithChanges = new List<(User, IEnumerable<Change>)>();
            ErrorMessage = "";
            SuccessMessage = "";
            Users = new List<User>();
        }
        public IEnumerable<User> Users { get; set; }
        public List<(User, IEnumerable<Change>)> UsersWithChanges { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                IEnumerable<User> masterList = await studentService.GetStudents();
                Users = await localUserService.GetUsers();
                foreach (var bakaUser in masterList)
                {
                    User? localUser = Users.FirstOrDefault(u => u.Id == bakaUser.Id);
                    if (localUser == null) continue;
                    IEnumerable<Change> changes = CompareService.GetDifferences(oldUser: localUser, newUser: bakaUser);
                    if (changes.Any())
                    {
                        UsersWithChanges.Add(new(localUser, changes));
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = ex.Message;
            };
        }
        public async Task OnGetConfirmChangesAsync()
        {
            try
            {
                IEnumerable<User> masterList = await studentService.GetStudents();
                Users = await localUserService.GetUsers();
                int updatedUsers = 0;
                foreach (var bakaUser in masterList)
                {
                    User? localUser = Users.FirstOrDefault(u => u.Id == bakaUser.Id);
                    if (localUser == null) continue;
                    IEnumerable<Change> changes = CompareService.GetDifferences(oldUser: localUser, newUser: bakaUser);
                    if (changes.Any())
                    {
                        updatedUsers += await localUserService.UpdateUser(bakaUser);
                    }
                }
                SuccessMessage = $"Úspěšně aktualizováno ({updatedUsers})";
            }
            catch (SqlException ex)
            {
                ErrorMessage = ex.Message;
            };
        }
    }
}
