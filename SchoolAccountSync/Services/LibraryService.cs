using FirebirdSql.Data.FirebirdClient;

namespace SchoolAccountSync.Services
{
    public class LibraryService
    {
        private readonly IConfiguration configuration;

        public LibraryService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        /// <summary>
        /// Updates the library user
        /// </summary>
        /// <param name="bakalariId">The external id from Bakalari db</param>
        /// <param name="rfid">Rfid in hexadecimal</param>
        /// <returns></returns>
        public async Task UpdateUser(string bakalariId, string rfid)
        {
            using FbConnection? connection = new(configuration["LibraryDatabase:ProductionConn"]);
            connection.Open();
            using FbCommand? command = new("execute procedure KEPPLER_SPROC_NASTAV_BARCOD(@BAKAID,@RFID);", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BAKAID", bakalariId);
            command.Parameters.AddWithValue("@RFID", rfid);
            await command.ExecuteNonQueryAsync();
        }
    }
}
