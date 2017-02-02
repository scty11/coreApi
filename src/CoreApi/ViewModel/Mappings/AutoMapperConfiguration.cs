using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace CoreApi.ViewModel.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.AddProfile<DomainToViewModelMappingProfile>();
            Mapper.AddProfile<ViewModelToDomainMappingProfile>();
        }
    }
}
