using AutoMapper;
using KMC_API_Guruprashath.Model;
using KMC_API_Guruprashath.DTO;

namespace KMC_API_Guruprashath.Profiles
{
    public class RegistrationProfile : Profile
    {
        public RegistrationProfile()
        {
            CreateMap<RegistrationWriteDTO, Registration>();
            CreateMap<Registration, RegistrationReadDTO>()
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.RegisteredEvent != null ? src.RegisteredEvent.Title : ""));
        }
    }
}
