namespace KMC_API_Guruprashath.Data
{
    using KMC_API_Guruprashath.Model;
    using Microsoft.EntityFrameworkCore;

    public class RegistrationRepo
    {
        private AppDBContext dbContext;

        public RegistrationRepo(AppDBContext appDB)
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

        public bool AddRegistration(Registration reg)
        {
            if (reg != null)
            {
                dbContext.Registrations.Add(reg);
                return save();
            }
            return false;
        }

        public bool RemoveRegistration(Registration reg)
        {
            if (reg != null)
            {
                dbContext.Registrations.Remove(reg);
                return save();
            }
            return false;
        }

        public List<Registration> GetRegistrations()
        {
            return dbContext.Registrations.Include(r => r.RegisteredEvent).ToList();
        }

        public List<Registration> GetRegistrationsByEvent(int eventId)
        {
            return dbContext.Registrations
                .Include(r => r.RegisteredEvent)
                .Where(r => r.EventId == eventId)
                .ToList();
        }

        public List<Registration> GetRegistrationsByUser(int userId)
        {
            return dbContext.Registrations
                .Include(r => r.RegisteredEvent)
                .ThenInclude(e => e.EventCategory)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RegisteredOn)
                .ToList();
        }

        public Registration? GetRegistrationByID(int id)
        {
            return dbContext.Registrations.Include(r => r.RegisteredEvent)
                .FirstOrDefault(r => r.RegistrationId == id);
        }

        public bool IsAlreadyRegistered(int eventId, int userId)
        {
            return dbContext.Registrations.Any(r => r.EventId == eventId && r.UserId == userId);
        }

        public int CountForEvent(int eventId)
        {
            return dbContext.Registrations.Count(r => r.EventId == eventId);
        }
    }
}
