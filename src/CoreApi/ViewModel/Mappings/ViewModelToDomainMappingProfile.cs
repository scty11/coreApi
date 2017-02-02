using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Schedule.Model;

namespace CoreApi.ViewModel.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ScheduleViewModel, Schedule.Model.Schedule>()
               .ForMember(s => s.Creator, map => map.UseValue(null))
               .ForMember(s => s.Attendees, map => map.UseValue(new List<Attendee>()));

            Mapper.CreateMap<UserViewModel, User>();
        }
    }
}
