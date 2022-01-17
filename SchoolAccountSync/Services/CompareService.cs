using SchoolAccountSync.Models;

namespace SchoolAccountSync.Services
{
    public static class CompareService
    {
        public static IEnumerable<Change> GetDifferences(User oldUser,User newUser)
        {
            List<Change> changes = new();
            if (ReferenceEquals(oldUser, newUser)) return changes;

            if (oldUser.Id != newUser.Id) changes.Add(new("Id", oldUser.Id, newUser.Id));
            if (oldUser.FirstName != newUser.FirstName) changes.Add(new("FirstName", oldUser.FirstName, newUser.FirstName));
            if (oldUser.LastName != newUser.LastName) changes.Add(new("LastName", oldUser.LastName, newUser.LastName));
            if (oldUser.Birthdate != newUser.Birthdate) changes.Add(new("Birthdate", oldUser.Birthdate, newUser.Birthdate));
            if (oldUser.Class != newUser.Class) changes.Add(new("Class", oldUser.Class, newUser.Class));
            if (oldUser.Rfid != newUser.Rfid) changes.Add(new("Rfid", oldUser.Rfid, newUser.Rfid));
            if (oldUser.LockerNumber != newUser.LockerNumber) changes.Add(new("LockerNumber", oldUser.LockerNumber, newUser.LockerNumber));
            if (oldUser.Status != newUser.Status) changes.Add(new("Status", oldUser.Status, newUser.Status));
            if (oldUser.UserType != newUser.UserType) changes.Add(new("UserType", oldUser.UserType, newUser.UserType));

            if (oldUser.SchoolEmail != newUser.SchoolEmail)
            {
                changes.Add(new("SchoolEmail", oldUser.SchoolEmail ?? "", newUser.SchoolEmail ?? ""));
            }

            if (oldUser.PersonalEmail != newUser.PersonalEmail)
            {
                changes.Add(new("PersonalEmail", oldUser.PersonalEmail ?? "", newUser.PersonalEmail ?? ""));
            }

            if (oldUser.TempPassword != newUser.TempPassword)
            {
                changes.Add(new("TempPassword", oldUser.TempPassword ?? "", newUser.TempPassword ?? ""));
            }

            return changes;
        }
    }
}
