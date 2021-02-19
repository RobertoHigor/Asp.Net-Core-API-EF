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
            
            /*
             * O ForMember após o .ReverseMap somente se aplica de TalkModel para Talk.
             * o ForMember diz para não mapear quando estiver indo de Model para Talk.
             * Isso é feito para que no PUT, o Speaker e Camp não seja sobrescrita
             */
            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());        

            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
           
        }
    }
}