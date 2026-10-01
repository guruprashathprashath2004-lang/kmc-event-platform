namespace KMC_API_Guruprashath.Data
{
    using KMC_API_Guruprashath.Model;
    using Microsoft.EntityFrameworkCore;

    public class UserRepo
    {
        private AppDBContext dbContext;

        public UserRepo(AppDBContext appDB)
        {
            dbContext = appDB;
        }

        public bool save()
        {
            int count = dbContext.SaveChanges();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddUser(User user)
        {
            if (user != null)
            {
                dbContext.Users.Add(user);
                return save();
            }
            return false;
        }

        public User? GetByUsername(string username)
        {
            return dbContext.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
        }

        public User? GetByEmail(string email)
        {
            return dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public User? GetByID(int id)
        {
            return dbContext.Users.FirstOrDefault(u => u.UserId == id);
        }

        public bool UsernameOrEmailTaken(string username, string email)
        {
            return dbContext.Users.Any(u => u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == email.ToLower());
        }

        public List<User> GetAllUsers()
        {
            return dbContext.Users
                .Include(u => u.OrganizedEvents)
                .Include(u => u.Registrations)
                .OrderBy(u => u.Role).ThenBy(u => u.FullName)
                .ToList();
        }

        public List<User> GetUsersByRole(string role)
        {
            return dbContext.Users
                .Include(u => u.OrganizedEvents)
                .Include(u => u.Registrations)
                .Where(u => u.Role.ToLower() == role.ToLower())
                .OrderBy(u => u.FullName)
                .ToList();
        }

        // Returns false (without throwing) if the user still owns events or registrations,
        // since those are protected with Restrict delete behaviour at the DB level.
        public bool DeleteUser(User user)
        {
            if (user.OrganizedEvents.Any() || user.Registrations.Any())
                return false;

            dbContext.Users.Remove(user);
            return save();
        }
    }
}
