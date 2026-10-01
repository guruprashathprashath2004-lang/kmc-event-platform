using AutoMapper;
using KMC_API_Guruprashath.Model;
using KMC_API_Guruprashath.DTO;

namespace KMC_API_Guruprashath.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventWriteDTO, Event>();
            CreateMap<Event, EventReadDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.EventCategory != null ? src.EventCategory.Name : ""))
                .ForMember(dest => dest.RegisteredCount, opt => opt.MapFrom(src => src.Registrations.Count))
                .ForMember(dest => dest.SeatsLeft, opt => opt.MapFrom(src => src.Capacity - src.Registrations.Count))
                .ForMember(dest => dest.OrganizerName, opt => opt.MapFrom(src => src.OrganizerUser != null ? src.OrganizerUser.FullName : "KMC Admin"));
        }
    }
}
