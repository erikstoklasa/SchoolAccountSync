using Npgsql;
using SchoolAccountSync.Models;
using System;

namespace SchoolAccountSync.Services
{
    public class LocalUserService
    {
        private readonly IConfiguration configuration;

        public LocalUserService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<IEnumerable<LocalUser>> GetUsers()
        {
            List<LocalUser> users = new();
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();

            await using (NpgsqlCommand cmd = new("SELECT id, first_name, last_name, birthdate, class, school_email," +
                "personal_email, rfid, user_type, status, locker_number, temp_password FROM users ORDER BY last_name ASC", con))
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    LocalUser user = new()
                    {
                        Id = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Birthdate = reader.GetFieldValue<DateOnly>(3),
                        Class = reader.GetString(4),
                        SchoolEmail = reader.IsDBNull(5) ? null : reader.GetString(5),
                        PersonalEmail = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Rfid = reader.IsDBNull(7) ? null : reader.GetString(7),
                        UserType = (UserTypes)reader.GetValue(8),
                        Status = (StatusTypes)reader.GetValue(9),
                        LockerNumber = reader.GetString(10),
                        TempPassword = reader.IsDBNull(11) ? null : reader.GetString(11)
                    };
                    users.Add(user);
                }
            }
            return users;
        }
        public async Task<IEnumerable<LocalUser>> GetUsersByClass(string classFilter)
        {
            List<LocalUser> users = new();
            if (string.IsNullOrWhiteSpace(classFilter))
            {
                return users;
            }
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();

            await using (NpgsqlCommand cmd = new("SELECT id, first_name, last_name, birthdate, class, school_email," +
                "personal_email, rfid, user_type, status, locker_number, temp_password FROM users WHERE class = $1 ORDER BY last_name ASC", con)
            {
                Parameters =
                {
                    new() { Value = classFilter },
                }
            }
                )
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    LocalUser user = new()
                    {
                        Id = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Birthdate = reader.GetFieldValue<DateOnly>(3),
                        Class = reader.GetString(4),
                        SchoolEmail = reader.IsDBNull(5) ? null : reader.GetString(5),
                        PersonalEmail = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Rfid = reader.IsDBNull(7) ? null : reader.GetString(7),
                        UserType = (UserTypes)reader.GetValue(8),
                        Status = (StatusTypes)reader.GetValue(9),
                        LockerNumber = reader.GetString(10),
                        TempPassword = reader.IsDBNull(11) ? null : reader.GetString(11)
                    };
                    users.Add(user);
                }
            }
            return users;
        }
        public async Task<IEnumerable<LocalUser>> GetUsersByRfid(string rfidFilter)
        {
            List<LocalUser> users = new();
            if (string.IsNullOrWhiteSpace(rfidFilter))
            {
                return users;
            }
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();

            await using (NpgsqlCommand cmd = new("SELECT id, first_name, last_name, birthdate, class, school_email," +
                "personal_email, rfid, user_type, status, locker_number, temp_password FROM users WHERE rfid = $1 ORDER BY last_name ASC", con)
            {
                Parameters =
                {
                    new() { Value = rfidFilter },
                }
            }
                )
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    LocalUser user = new()
                    {
                        Id = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Birthdate = reader.GetFieldValue<DateOnly>(3),
                        Class = reader.GetString(4),
                        SchoolEmail = reader.IsDBNull(5) ? null : reader.GetString(5),
                        PersonalEmail = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Rfid = reader.IsDBNull(7) ? null : reader.GetString(7),
                        UserType = (UserTypes)reader.GetValue(8),
                        Status = (StatusTypes)reader.GetValue(9),
                        LockerNumber = reader.GetString(10),
                        TempPassword = reader.IsDBNull(11) ? null : reader.GetString(11)
                    };
                    users.Add(user);
                }
            }
            return users;
        }
        public async Task<IEnumerable<LocalUser>> GetUsersByName(string nameFilter)
        {
            List<LocalUser> users = new();
            if (string.IsNullOrWhiteSpace(nameFilter))
            {
                return users;
            }
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();

            await using (NpgsqlCommand cmd = new("SELECT id, first_name, last_name, birthdate, class, school_email," +
                "personal_email, rfid, user_type, status, locker_number, temp_password FROM users " +
                "WHERE LOWER(first_name) LIKE '%' || $1 || '%' OR LOWER(last_name) LIKE '%' || $1 || '%' ORDER BY last_name ASC", con)
            {
                Parameters =
                {
                    new() { Value = nameFilter.ToLower() },
                }
            }
                )
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    LocalUser user = new()
                    {
                        Id = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Birthdate = reader.GetFieldValue<DateOnly>(3),
                        Class = reader.GetString(4),
                        SchoolEmail = reader.IsDBNull(5) ? null : reader.GetString(5),
                        PersonalEmail = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Rfid = reader.IsDBNull(7) ? null : reader.GetString(7),
                        UserType = (UserTypes)reader.GetValue(8),
                        Status = (StatusTypes)reader.GetValue(9),
                        LockerNumber = reader.GetString(10),
                        TempPassword = reader.IsDBNull(11) ? null : reader.GetString(11)
                    };
                    users.Add(user);
                }
            }
            return users;
        }

        public async Task<LocalUser> GetUser(string id)
        {
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            LocalUser user;
            await using (NpgsqlCommand cmd = new("SELECT id, first_name, last_name, birthdate, class, school_email," +
                "personal_email, rfid, user_type, status, locker_number, temp_password FROM users WHERE id = $1", con)
            {
                Parameters =
                {
                    new() { Value = id },
                }
            })
            await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                await reader.ReadAsync();

                user = new()
                {
                    Id = reader.GetString(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Birthdate = reader.GetFieldValue<DateOnly>(3),
                    Class = reader.GetString(4),
                    SchoolEmail = reader.IsDBNull(5) ? null : reader.GetString(5),
                    PersonalEmail = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Rfid = reader.IsDBNull(7) ? null : reader.GetString(7),
                    UserType = (UserTypes)reader.GetValue(8),
                    Status = (StatusTypes)reader.GetValue(9),
                    LockerNumber = reader.GetString(10),
                    TempPassword = reader.IsDBNull(11) ? null : reader.GetString(11)
                };

            }
            return user;
        }
        public async Task<int> UpdateUser(LocalUser user)
        {
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            await using NpgsqlCommand cmd = new("UPDATE users SET first_name = $1, last_name = $2, birthdate = $3, class = $4, school_email = $5, " +
                "personal_email = $6, rfid = $7, user_type = $8, status = $9, locker_number = $10, temp_password = $11 WHERE id = $12", con)
            {
                Parameters =
                {
                    new() { Value = user.FirstName },
                    new() { Value = user.LastName },
                    new() { Value = user.Birthdate },
                    new() { Value = user.Class },
                    new() { Value = user.SchoolEmail != null ? user.SchoolEmail : DBNull.Value },
                    new() { Value = user.PersonalEmail != null ? user.PersonalEmail : DBNull.Value },
                    new() { Value = user.Rfid != null ? user.Rfid : DBNull.Value },
                    new() { Value = (int)user.UserType },
                    new() { Value = (int)user.Status },
                    new() { Value = user.LockerNumber },
                    new() { Value = user.TempPassword != null ? user.TempPassword : DBNull.Value },
                    new() { Value = user.Id },
                }
            };
            return await cmd.ExecuteNonQueryAsync();
        }
        public async Task<int> AddUser(LocalUser user)
        {
            await using NpgsqlConnection con = new(configuration["LocalDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            await using NpgsqlCommand cmd = new("INSERT INTO users (id, first_name, last_name, birthdate, class, " +
                "school_email, personal_email, rfid, user_type, status, locker_number, temp_password) VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12)", con)
            {
                Parameters =
                {
                    new() { Value = user.Id },
                    new() { Value = user.FirstName },
                    new() { Value = user.LastName },
                    new() { Value = user.Birthdate },
                    new() { Value = user.Class },
                    new() { Value = user.SchoolEmail != null ? user.SchoolEmail : DBNull.Value },
                    new() { Value = user.PersonalEmail != null ? user.PersonalEmail : DBNull.Value },
                    new() { Value = user.Rfid != null ? user.Rfid : DBNull.Value },
                    new() { Value = (int)user.UserType },
                    new() { Value = (int)user.Status },
                    new() { Value = user.LockerNumber },
                    new() { Value = user.TempPassword != null ? user.TempPassword : DBNull.Value },
                }
            };
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
