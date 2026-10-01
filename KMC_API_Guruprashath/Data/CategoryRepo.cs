using KMC_API_Guruprashath.Model;
using Microsoft.EntityFrameworkCore;

namespace KMC_API_Guruprashath.Data
{
    public class CategoryRepo
    {
        private AppDBContext dbContext;

        public CategoryRepo(AppDBContext appDB)
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

        public bool AddCategory(Category category)
        {
            if (category != null)
            {
                dbContext.Categories.Add(category);
                return save();
            }
            return false;
        }

        public bool UpdateCategory(Category category)
        {
            if (category != null)
            {
                dbContext.Categories.Update(category);
                return save();
            }
            return false;
        }

        public bool RemoveCategory(Category category)
        {
            if (category != null)
            {
                dbContext.Categories.Remove(category);
                return save();
            }
            return false;
        }

        public List<Category> GetCategories()
        {
            return dbContext.Categories.Include(c => c.Events).ToList();
        }

        public Category? GetCategoryByID(int id)
        {
            return dbContext.Categories.Include(c => c.Events).FirstOrDefault(c => c.CategoryId == id);
        }
    }
}
