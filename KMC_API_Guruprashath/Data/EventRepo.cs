namespace KMC_API_Guruprashath.Data
{
    using KMC_API_Guruprashath.Model;
    using Microsoft.EntityFrameworkCore;

    public class EventRepo
    {
        private AppDBContext dbContext;

        public EventRepo(AppDBContext appDB)
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

        public bool AddEvent(Event ev)
        {
            if (ev != null)
            {
                dbContext.Events.Add(ev);
                return save();
            }
            return false;
        }

        public bool UpdateEvent(Event ev)
        {
            if (ev != null)
            {
                dbContext.Events.Update(ev);
                return save();
            }
            return false;
        }

        public bool RemoveEvent(Event ev)
        {
            if (ev != null)
            {
                dbContext.Events.Remove(ev);
                return save();
            }
            return false;
        }

        private IQueryable<Event> BaseQuery()
        {
            return dbContext.Events
                .Include(e => e.EventCategory)
                .Include(e => e.Registrations)
                .Include(e => e.OrganizerUser);
        }

        public List<Event> GetEvents()
        {
            return BaseQuery().OrderBy(e => e.EventDate).ToList();
        }

        // Public listing: only events an Admin has approved AND published.
        public List<Event> GetUpcomingPublishedEvents()
        {
            return BaseQuery()
                .Where(e => e.IsPublished && e.ApprovalStatus == "Approved" && e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToList();
        }

        public List<Event> GetFeaturedEvents()
        {
            return BaseQuery()
                .Where(e => e.IsPublished && e.ApprovalStatus == "Approved" && e.IsFeatured && e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToList();
        }

        public Event? GetEventByID(int id)
        {
            return BaseQuery().FirstOrDefault(e => e.EventId == id);
        }

        public List<Event> GetEventsByCategory(int categoryId)
        {
            return BaseQuery()
                .Where(e => e.CategoryId == categoryId && e.IsPublished && e.ApprovalStatus == "Approved")
                .OrderBy(e => e.EventDate)
                .ToList();
        }

        public List<Event> SearchEvents(string keyword)
        {
            return BaseQuery()
                .Where(e => e.IsPublished && e.ApprovalStatus == "Approved" &&
                    (EF.Functions.Like(e.Title, $"%{keyword}%")
                    || EF.Functions.Like(e.Description, $"%{keyword}%")
                    || EF.Functions.Like(e.Venue, $"%{keyword}%")))
                .OrderBy(e => e.EventDate)
                .ToList();
        }

        // Organizer's own events, any approval status - so they can track pending/rejected too.
        public List<Event> GetEventsByOrganizer(int organizerId)
        {
            return BaseQuery()
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.CreatedOn)
                .ToList();
        }

        // For Admin's approval queue.
        public List<Event> GetPendingEvents()
        {
            return BaseQuery()
                .Where(e => e.ApprovalStatus == "Pending")
                .OrderBy(e => e.CreatedOn)
                .ToList();
        }
    }
}
