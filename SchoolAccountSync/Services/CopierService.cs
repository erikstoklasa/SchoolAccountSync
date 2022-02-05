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
        /// <summary>
        /// Adds user with its cards
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="ArgumentException">Thrown when school email is null</exception>
        public async Task AddUser(LocalUser user)
        {
            string login;
            if (user.SchoolEmail != null)
            {
                login = CopierUser.GenerateLogin(user.SchoolEmail);
            }
            else
            {
                throw new ArgumentException("School email can not be null", nameof(user.SchoolEmail));
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
                    new() { Value = CopierUser.GenerateOuId(user.UserType) },
                    new() { Value = login },
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
                long? id = (long?)await cmd2.ExecuteScalarAsync();

                if (id == null)
                {
                    throw new Exception("Added user was unexpectedly deleted");
                }

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
                throw;
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
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
                ExtId = reader.IsDBNull(0) ? null : reader.GetString(0),
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
                ExtId = reader.IsDBNull(0) ? null : reader.GetString(0),
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
        public async Task<int> UpdateUser(CopierUser user)
        {
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            await using NpgsqlCommand cmd = new("UPDATE users SET ext_id = $1, name = $2, surname = $3, login = $4, email = $5, " +
                "ou_id = $6, name_ascii = $7, surname_ascii = $8, login_ascii = $9, pass = $10 WHERE id = $11", con)
            {
                Parameters =
                {
                    new() { Value = user.ExtId != null ? user.ExtId : DBNull.Value },
                    new() { Value = user.FirstName },
                    new() { Value = user.LastName },
                    new() { Value = user.Login },
                    new() { Value = user.SchoolEmail },
                    new() { Value = user.OuId != null ? user.OuId : DBNull.Value },
                    new() { Value = user.FirstNameAscii != null ? user.FirstNameAscii : DBNull.Value },
                    new() { Value = user.LastNameAscii != null ? user.LastNameAscii : DBNull.Value },
                    new() { Value = user.LoginAscii },
                    new() { Value = user.TempPassword != null ? user.TempPassword : DBNull.Value },
                    new() { Value = user.Id },
                }
            };
            return await cmd.ExecuteNonQueryAsync();
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
        public async Task<int> AddCard(CopierCard copierCard)
        {
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            await using NpgsqlCommand cmd = new("INSERT INTO users_cards (user_id,card) VALUES ($1,$2)", con)
            {
                Parameters =
                {
                    new() { Value = copierCard.UserId},
                    new() { Value = copierCard.CardId },
                },
            };
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteCards(long userInternalId)
        {
            await using NpgsqlConnection con = new(configuration["CopiersDatabase:DevelopmentConn"]);
            await con.OpenAsync();
            await using NpgsqlCommand cmd = new("DELETE FROM users_cards WHERE user_id = $1", con)
            {
                Parameters =
                {
                    new() { Value = userInternalId },
                }
            };
            return await cmd.ExecuteNonQueryAsync();
        }

    }
}
