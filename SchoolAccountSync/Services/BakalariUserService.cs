using SchoolAccountSync.Models;
using System;
using System.Data.SqlClient;

namespace SchoolAccountSync.Services
{
    public class BakalariUserService
    {
        private readonly IConfiguration configuration;

        public BakalariUserService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /// <summary>
        /// Gets students from the Bakalari SQL Server
        /// </summary>
        /// <returns>List of students</returns>
        /// <exception cref="SqlException">Thrown when connection could not be established.</exception>
        public async Task<ICollection<BakalariUser>> GetStudents()
        {
            List<BakalariUser> users = new();
            using SqlConnection con = new(configuration["BakalariService:DevelopmentConn"]);
            con.Open();
            using SqlCommand command = new("SELECT [INTERN_KOD],[JMENO],[PRIJMENI],[DATUM_NAR],[E_MAIL],[DELETED_RC],[SKRINKA_C],[TRIDA] FROM zaci ORDER BY [PRIJMENI] ASC;", con);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                DateOnly date = DateOnly.ParseExact(reader.GetString(3).Replace(" ", ""), "d.M.yyyy");
                BakalariUser user = new()
                {
                    Id = reader.GetString(0).Trim(),
                    FirstName = reader.GetString(1).Trim(),
                    LastName = reader.GetString(2).Trim(),
                    Birthdate = date,
                    PersonalEmail = reader.GetString(4).Trim(),
                    Status = reader.GetBoolean(5) ? StatusTypes.Abroad : StatusTypes.Normal,
                    UserType = UserTypes.Student,
                    LockerNumber = reader.GetString(6).Replace(" ", ""),
                    Class = reader.GetString(7).Trim(),
                };
                users.Add(user);
            }
            return users;
        }
        /// <summary>
        /// Gets a student from the Bakalari SQL Server by id
        /// </summary>
        /// <returns>List of students</returns>
        /// <exception cref="SqlException">Thrown when connection could not be established.</exception>
        public async Task<BakalariUser> GetStudent(string id)
        {

            using SqlConnection con = new(configuration["BakalariService:DevelopmentConn"]);
            con.Open();
            using SqlCommand command = new("SELECT TOP (1) [INTERN_KOD],[JMENO],[PRIJMENI],[DATUM_NAR],[E_MAIL],[DELETED_RC],[SKRINKA_C],[TRIDA] FROM zaci WHERE [INTERN_KOD] = @Id;", con);
            SqlParameter parameterId = new("@Id", id);
            command.Parameters.Add(parameterId);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            DateOnly date = DateOnly.ParseExact(reader.GetString(3).Replace(" ", ""), "d.M.yyyy");
            BakalariUser user = new()
            {
                Id = reader.GetString(0).Trim(),
                FirstName = reader.GetString(1).Trim(),
                LastName = reader.GetString(2).Trim(),
                Birthdate = date,
                PersonalEmail = reader.GetString(4).Trim(),
                Status = reader.GetBoolean(5) ? StatusTypes.Abroad : StatusTypes.Normal,
                UserType = UserTypes.Student,
                LockerNumber = reader.GetString(6).Replace(" ", ""),
                Class = reader.GetString(7).Trim(),
            };
            return user;
        }
    }
}
