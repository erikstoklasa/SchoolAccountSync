using SchoolAccountSync.Models;
using System.Data.SqlClient;

namespace SchoolAccountSync.Services
{
    public class EntranceService
    {

        public EntranceService()
        {
        }
        public async Task UpdateUserCardAsync(string rfidOld, string rfidNew)
        {
            using SqlConnection con = new(Environment.GetEnvironmentVariable("EntranceDatabase"));
            con.Open();
            using SqlCommand command = new(
                "UPDATE [dbo].[ACCARD] SET [CRDNUM] = @NEWID WHERE [CRDNUM] LIKE '%'+@OLDID+'%';", con);
            command.Parameters.Add(new("@OLDID", rfidOld));
            command.Parameters.Add(new("@NEWID", rfidNew));
            await command.ExecuteNonQueryAsync();

            using SqlCommand command2 = new(
                "UPDATE [dbo].[CACARD] SET [CRDNUM] = @NEWID WHERE [CRDNUM] LIKE '%'+@OLDID+'%'; ", con);
            command2.Parameters.Add(new("@OLDID", rfidOld));
            command2.Parameters.Add(new("@NEWID", rfidNew));
            await command2.ExecuteNonQueryAsync();

            using SqlCommand command3 = new(
                "UPDATE [dbo].[CAELECRD] SET [CRDNUM] = @NEWID WHERE [CRDNUM] LIKE '%'+@OLDID+'%'; ", con);
            command3.Parameters.Add(new("@OLDID", rfidOld));
            command3.Parameters.Add(new("@NEWID", rfidNew));
            await command3.ExecuteNonQueryAsync();
        }
        public async Task AddUserAsync(EntranceUser user)
        {
            using SqlConnection con = new(Environment.GetEnvironmentVariable("EntranceDatabase"));
            con.Open();
            using SqlCommand command = new(
                "INSERT INTO [dbo].[ACCARD] ([CRDNUM],[IDEXAMPL],[TYTARGET],[IDGROUP],[IDREADER],[TODATE],[FROMDATE],[DATATYPE],[DATATYP2],[TMZONNUM],[IDUSER],[IDUSERLOC]) " +
                "VALUES(@RFID,1,' ',0,'        ','99999999999999',@TODAY,0,0,0,1,8); ", con);
            command.Parameters.Add(new("@RFID", user.EntranceCards.First().RfidDecimal));
            command.Parameters.Add(new("@TODAY", DateTime.Now.ToString("yyyyMMddHHmmss")));
            await command.ExecuteNonQueryAsync();

            using SqlCommand command2 = new(
                "INSERT INTO [dbo].[CACARD] ([CRDNUM],[CRDTYPE],[IDKERNEL],[FDATE1],[FDATE2],[IDACCTYP]) " +
                "VALUES(@RFID,3,1,@DATE,@DATE,0)", con);
            command2.Parameters.Add(new("@RFID", user.EntranceCards.First().RfidDecimal));
            command2.Parameters.Add(new("@DATE", DateTime.Now));
            await command2.ExecuteNonQueryAsync();

            using SqlCommand command3 = new(
                "INSERT INTO [dbo].[CAELECRD]([IDELELEA],[CRDNUM],[TODATE],[FROMDATE] " +
                "VALUES(555,@RFID,'99999999999999',@TODAY)", con);
            command3.Parameters.Add(new("@RFID", user.EntranceCards.First().RfidDecimal));
            command3.Parameters.Add(new("@TODAY", DateTime.Now.ToString("yyyyMMddHHmmss")));
            await command3.ExecuteNonQueryAsync();
        }
    }
}
