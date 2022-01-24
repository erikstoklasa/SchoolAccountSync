using Npgsql;
using SchoolAccountSync.Models;

namespace SchoolAccountSync.Services
{
    public class CopierService
    {
        private readonly IConfiguration configuration;

        public CopierService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task AddUser(LocalUser user)
        {
            int ouId = user.UserType switch
            {
                UserTypes.Teacher => 1,
                UserTypes.Student => 2,
                _ => 2,
            };
            string login = "";
            if (user.SchoolEmail != null)
            {
                login = user.SchoolEmail.Split("@")[0];
            }
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            NpgsqlTransaction trans = null;
            try
            {
                await con.OpenAsync();
                trans = con.BeginTransaction();

                await using NpgsqlCommand cmd1 = new("INSERT INTO users (login,pass,name,surname,email,flag,ou_id,login_ascii,name_ascii,surname_ascii,ext_id) " +
                    "VALUES($1, $2, $3, $4, $5, 1, $6, $7, $8, $9, $10)", con)
                {
                    Parameters =
                {
                    new() { Value = login},
                    new() { Value = user.TempPassword != null ? user.TempPassword : DBNull.Value },
                    new() { Value = user.FirstName },
                    new() { Value = user.LastName },
                    new() { Value = user.SchoolEmail != null ? user.SchoolEmail : DBNull.Value },
                    new() { Value = ouId },
                    new() { Value = login},
                    new() { Value = user.FirstName },
                    new() { Value = user.LastName },
                    new() { Value = user.Id },
                },
                    Transaction = trans,
                };
                await cmd1.ExecuteNonQueryAsync();

                await using NpgsqlCommand cmd2 = new("SELECT id FROM users WHERE ext_id = $1", con)
                {
                    Parameters =
                {
                    new() { Value = user.Id },
                },
                    Transaction = trans,
                };
                long id = (long)await cmd2.ExecuteScalarAsync();

                await using NpgsqlCommand cmd3 = new("INSERT INTO users_cards (user_id,card) VALUES ($1,$2)", con)
                {
                    Parameters =
                {
                    new() { Value = id},
                    new() { Value = user.Rfid },
                },
                    Transaction = trans,
                };
                await cmd3.ExecuteNonQueryAsync();

                trans.Commit();
            }
            catch (NpgsqlException)
            {
                trans.Rollback();
            }
        }
        public async Task<CopierUser?> GetUser(string externalId)
        {
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            CopierUser user;
            NpgsqlCommand cmd = new("SELECT ext_id, name, surname, login, email, ou_id, name_ascii, surname_ascii, login_ascii, pass, id FROM users WHERE ext_id = $1", con)
            {
                Parameters =
                {
                    new() { Value = externalId },
                }
            };
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            if (!reader.IsOnRow) return null;
            user = new()
            {
                ExtId = reader.GetString(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Login = reader.GetString(3),
                SchoolEmail = reader.GetString(4),
                OuId = reader.GetInt64(5),
                FirstNameAscii = reader.GetString(6),
                LastNameAscii = reader.GetString(7),
                LoginAscii = reader.GetString(8),
                TempPassword = reader.IsDBNull(9) ? null : reader.GetString(9),
                Id = reader.GetInt64(10),

            };

            long userId = reader.GetInt64(10);

            cmd.Dispose();
            reader.Close();

            user.CopierCards = new List<CopierCard>();

            await using NpgsqlCommand cmd2 = new("SELECT user_id, card FROM users_cards WHERE user_id = $1", con)
            {
                Parameters =
                {
                    new() { Value = userId},
                }
            };

            await using NpgsqlDataReader reader2 = await cmd2.ExecuteReaderAsync();
            while (await reader2.ReadAsync())
            {
                user.CopierCards.Add(new CopierCard() { UserId = reader2.GetInt64(0), CardId = reader2.GetString(1) });
            }

            return user;
        }
        public async Task<CopierUser?> GetUserByLogin(string login)
        {
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            CopierUser user;
            NpgsqlCommand cmd = new("SELECT ext_id, name, surname, login, email, ou_id, name_ascii, surname_ascii, login_ascii, pass, id FROM users WHERE login = $1", con)
            {
                Parameters =
                {
                    new() { Value = login },
                }
            };
            await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            if (!reader.IsOnRow) return null;
            user = new()
            {
                ExtId = reader.GetString(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Login = reader.GetString(3),
                SchoolEmail = reader.GetString(4),
                OuId = reader.GetInt64(5),
                FirstNameAscii = reader.GetString(6),
                LastNameAscii = reader.GetString(7),
                LoginAscii = reader.GetString(8),
                TempPassword = reader.IsDBNull(9) ? null : reader.GetString(9),
                Id = reader.GetInt64(10),

            };

            long userId = reader.GetInt64(10);

            cmd.Dispose();
            reader.Close();

            user.CopierCards = new List<CopierCard>();

            await using NpgsqlCommand cmd2 = new("SELECT user_id, card FROM users_cards WHERE user_id = $1", con)
            {
                Parameters =
                {
                    new() { Value = userId},
                }
            };

            await using NpgsqlDataReader reader2 = await cmd2.ExecuteReaderAsync();
            while (await reader2.ReadAsync())
            {
                user.CopierCards.Add(new CopierCard() { UserId = reader2.GetInt64(0), CardId = reader2.GetString(1) });
            }

            return user;
        }
        public async Task<int> DeleteUserWithCards(long internalId)
        {
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            await con.OpenAsync();

            await using NpgsqlCommand cmd1 = new("DELETE FROM users_cards WHERE user_id = $1", con)
            {
                Parameters =
                {
                    new() { Value = internalId},
                }
            };
            await cmd1.ExecuteNonQueryAsync();

            await using NpgsqlCommand cmd2 = new("DELETE FROM users WHERE id = $1", con)
            {
                Parameters =
                {
                    new() { Value = internalId},
                }
            };

            return await cmd2.ExecuteNonQueryAsync();
        }

    }
}
