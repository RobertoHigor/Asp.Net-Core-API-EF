using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            // source -> target
            this.CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName))
                .ReverseMap();

            this.CreateMap<Location, LocationModel>()
                .ReverseMap();            
            
            this.CreateMap<Talk, TalkModel>()
                .ReverseMap();       

            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
           
        }
    }
}