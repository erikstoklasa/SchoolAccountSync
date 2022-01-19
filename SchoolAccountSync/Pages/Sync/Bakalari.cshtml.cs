using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolAccountSync.Models;
using SchoolAccountSync.Services;
using System.Data.SqlClient;

namespace SchoolAccountSync.Pages.Sync
{
    public class BakalariModel : PageModel
    {
        private readonly BakalariUserService studentService;
        private readonly LocalUserService localUserService;

        public BakalariModel(BakalariUserService studentService, LocalUserService localUserService)
        {
            this.studentService = studentService;
            this.localUserService = localUserService;
            UsersWithChanges = new List<(LocalUser, IEnumerable<Change>)>();
            ErrorMessage = "";
            SuccessMessage = "";
            Users = new List<LocalUser>();
        }
        public IEnumerable<LocalUser> Users { get; set; }
        public List<(LocalUser, IEnumerable<Change>)> UsersWithChanges { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                IEnumerable<LocalUser> masterList = await studentService.GetStudents();
                Users = await localUserService.GetUsers();
                foreach (var bakaUser in masterList)
                {
                    LocalUser? localUser = Users.FirstOrDefault(u => u.Id == bakaUser.Id);
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
                IEnumerable<LocalUser> masterList = await studentService.GetStudents();
                Users = await localUserService.GetUsers();
                int updatedUsers = 0;
                foreach (var bakaUser in masterList)
                {
                    LocalUser? localUser = Users.FirstOrDefault(u => u.Id == bakaUser.Id);
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
