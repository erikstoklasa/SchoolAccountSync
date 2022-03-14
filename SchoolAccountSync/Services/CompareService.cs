using SchoolAccountSync.Models;

namespace SchoolAccountSync.Services
{
    public static class CompareService
    {
        public static IEnumerable<Change> GetDifferences(BakalariUser newUser, LocalUser oldUser)
        {
            List<Change> changes = new();
            if (ReferenceEquals(oldUser, newUser)) return changes;

            if (oldUser.Id != newUser.Id) changes.Add(new("Id", oldUser.Id, newUser.Id));
            if (oldUser.FirstName != newUser.FirstName) changes.Add(new("FirstName", oldUser.FirstName, newUser.FirstName));
            if (oldUser.LastName != newUser.LastName) changes.Add(new("LastName", oldUser.LastName, newUser.LastName));
            if (oldUser.Birthdate != newUser.Birthdate) changes.Add(new("Birthdate", oldUser.Birthdate, newUser.Birthdate));
            if (oldUser.Class != newUser.Class) changes.Add(new("Class", oldUser.Class, newUser.Class));
            if (oldUser.LockerNumber != newUser.LockerNumber) changes.Add(new("LockerNumber", oldUser.LockerNumber, newUser.LockerNumber));
            if (oldUser.Status != newUser.Status) changes.Add(new("Status", oldUser.Status, newUser.Status));
            if (oldUser.UserType != newUser.UserType) changes.Add(new("UserType", oldUser.UserType, newUser.UserType));

            if (oldUser.PersonalEmail != newUser.PersonalEmail)
            {
                changes.Add(new("PersonalEmail", oldUser.PersonalEmail ?? "", newUser.PersonalEmail ?? ""));
            }

            return changes;
        }
        public static bool IsSynced(CopierUser copierUser, LocalUser localUser)
        {
            if (localUser.Id != copierUser.ExtId) return false;
            if (localUser.FirstName != copierUser.FirstName) return false;
            if (localUser.LastName != copierUser.LastName) return false;
            if (copierUser.FirstNameAscii == null) return false;
            if (copierUser.LastNameAscii == null) return false;
            if (copierUser.LoginAscii == null) return false;
            if (LocalUser.RemoveDiacritic(localUser.FirstName) != copierUser.FirstNameAscii) return false;
            if (LocalUser.RemoveDiacritic(localUser.LastName) != copierUser.LastNameAscii) return false;
            if (localUser.TempPassword != copierUser.TempPassword) return false;
            string? login = localUser.SchoolEmail?.Split("@")[0];
            if (login != copierUser.Login) return false;
            if (LocalUser.RemoveDiacritic(login) != copierUser.LoginAscii) return false;

            if (localUser.SchoolEmail != copierUser.SchoolEmail)
            {
                return false;
            }
            switch (copierUser.OuId)
            {
                case 1:
                    if (localUser.UserType == UserTypes.Student) return false;
                    break;
                case 2:
                    if (localUser.UserType == UserTypes.Teacher) return false;
                    break;
                default:
                    return false;
            }

            if (copierUser.CopierCards == null || copierUser.CopierCards.Count != 1) return false;
            if (copierUser.CopierCards[0].CardId != localUser.Rfid) return false;

            return true;
        }
    }
}
