using AutoMapper;
using KMC_API_Guruprashath.Model;
using KMC_API_Guruprashath.DTO;

namespace KMC_API_Guruprashath.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadDTO>()
                .ForMember(dest => dest.EventCount, opt => opt.MapFrom(src => src.OrganizedEvents.Count))
                .ForMember(dest => dest.RegistrationCount, opt => opt.MapFrom(src => src.Registrations.Count));
        }
    }
}
