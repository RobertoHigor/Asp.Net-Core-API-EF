using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            // source -> target
            this.CreateMap<Camp, CampModel>();
                //.ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));
            this.CreateMap<CampModel, Camp>();

            this.CreateMap<Location, LocationModel>();
            this.CreateMap<LocationModel, Location>();
            
            this.CreateMap<Talk, TalkModel>();
            this.CreateMap<TalkModel, Talk>();

            this.CreateMap<Speaker, SpeakerModel>();
            this.CreateMap<SpeakerModel, Speaker>();
        }
    }
}