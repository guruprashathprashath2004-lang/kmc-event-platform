using AutoMapper;
using KMC_API_Guruprashath.Model;
using KMC_API_Guruprashath.DTO;

namespace KMC_API_Guruprashath.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryWriteDTO, Category>();
            CreateMap<Category, CategoryReadDTO>()
                .ForMember(dest => dest.EventCount, opt => opt.MapFrom(src => src.Events.Count));
        }
    }
}
