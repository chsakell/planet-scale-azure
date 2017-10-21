using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure(IConfiguration configuration)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile(new DomainToViewModelProfile(configuration));
            });
        }
    }
}
