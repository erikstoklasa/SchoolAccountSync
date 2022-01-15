using SchoolAccountSync.Models;
using System;
using System.Data.SqlClient;

namespace SchoolAccountSync.Services
{
    public class BakalariService
    {
        private readonly IConfiguration configuration;

        public BakalariService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<ICollection<User>> GetStudents()
        {
            List<User> users = new();
            using SqlConnection con = new(configuration["BakalariService:DevelopmentConn"]);
            con.Open();
            using SqlCommand command = new("SELECT [INTERN_KOD],[JMENO],[PRIJMENI],[DATUM_NAR],[E_MAIL],[DELETED_RC] FROM zaci ORDER BY [PRIJMENI] ASC;", con);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                DateOnly date = DateOnly.ParseExact(reader.GetString(3).Replace(" ", ""), "d.M.yyyy");
                User user = new()
                {
                    Id = reader.GetString(0).Trim(),
                    FirstName = reader.GetString(1).Trim(),
                    LastName = reader.GetString(2).Trim(),
                    Birthdate = date,
                    PersonalEmail = reader.GetString(4).Trim(),
                    Status = reader.GetBoolean(5) ? StatusTypes.Abroad : StatusTypes.Normal,
                    UserType = UserTypes.Student,
                };
                user.SchoolEmail = User.GenerateSchoolEmail(user.FirstName, user.LastName, user.UserType);
                users.Add(user);
            }
            return users;
        }
        public async Task<User> GetStudent(string id)
        {

            using SqlConnection con = new(configuration["BakalariService:DevelopmentConn"]);
            con.Open();
            using SqlCommand command = new("SELECT TOP (1) [INTERN_KOD],[JMENO],[PRIJMENI],[DATUM_NAR],[E_MAIL],[DELETED_RC] FROM zaci WHERE [INTERN_KOD] = @Id;", con);
            SqlParameter parameterId = new("@Id", id);
            command.Parameters.Add(parameterId);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            DateOnly date = DateOnly.ParseExact(reader.GetString(3).Replace(" ", ""), "d.M.yyyy");
            User user = new()
            {
                Id = reader.GetString(0).Trim(),
                FirstName = reader.GetString(1).Trim(),
                LastName = reader.GetString(2).Trim(),
                Birthdate = date,
                PersonalEmail = reader.GetString(4).Trim(),
                Status = reader.GetBoolean(5) ? StatusTypes.Abroad : StatusTypes.Normal,
                UserType = UserTypes.Student,
            };
            user.SchoolEmail = User.GenerateSchoolEmail(user.FirstName, user.LastName, user.UserType);
            return user;
        }
    }
}
