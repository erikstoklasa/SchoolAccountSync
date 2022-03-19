using FirebirdSql.Data.FirebirdClient;

namespace SchoolAccountSync.Services
{
    public class LibraryService
    {

        public LibraryService()
        {
        }
        /// <summary>
        /// Updates the library user
        /// </summary>
        /// <param name="bakalariId">The external id from Bakalari db</param>
        /// <param name="rfid">Rfid in hexadecimal</param>
        /// <returns></returns>
        public async Task UpdateUser(string bakalariId, string rfid)
        {
            using FbConnection? connection = new(Environment.GetEnvironmentVariable("LibraryDatabase"));
            connection.Open();
            using FbCommand? command = new("execute procedure KEPPLER_SPROC_NASTAV_BARCOD(@BAKAID,@RFID);", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BAKAID", bakalariId);
            command.Parameters.AddWithValue("@RFID", rfid);
            await command.ExecuteNonQueryAsync();
        }
    }
}
