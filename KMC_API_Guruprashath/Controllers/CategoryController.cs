using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMC_API_Guruprashath.Model;
using KMC_API_Guruprashath.Data;
using KMC_API_Guruprashath.DTO;
using AutoMapper;

namespace KMC_API_Guruprashath.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IMapper mapper;
        private CategoryRepo repo;

        public CategoryController(IMapper _mapper, CategoryRepo _repo)
        {
            mapper = _mapper;
            repo = _repo;
        }

        [HttpGet]
        public ActionResult<List<CategoryReadDTO>> GetCategories()
        {
            var categories = repo.GetCategories();
            return Ok(mapper.Map<List<CategoryReadDTO>>(categories));
        }

        [HttpGet("{id}")]
        public ActionResult<CategoryReadDTO> GetCategoryByID(int id)
        {
            var category = repo.GetCategoryByID(id);
            if (category != null)
                return Ok(mapper.Map<CategoryReadDTO>(category));
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddCategory(CategoryWriteDTO dto)
        {
            var category = mapper.Map<Category>(dto);
            if (category != null)
            {
                if (repo.AddCategory(category))
                    return Ok();
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult UpdateCategory(CategoryWriteDTO dto, int id)
        {
            var existing = repo.GetCategoryByID(id);
            if (existing == null)
                return NotFound();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.IconClass = dto.IconClass;

            if (repo.UpdateCategory(existing))
                return Ok();
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)
        {
            var category = repo.GetCategoryByID(id);
            if (category != null)
                if (repo.RemoveCategory(category))
                    return Ok();
            return NotFound();
        }
    }
}
