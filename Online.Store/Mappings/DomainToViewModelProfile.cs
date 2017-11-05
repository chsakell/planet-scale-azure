using Online.Store.Core.DTOs;
using Online.Store.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Online.Store.Mappings
{
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile(IConfiguration configuration)
        {
            string storageBlobEndpoint = string.Format("https://{0}.blob.core.windows.net/", configuration["Storage:AccountName"]);
            string cdnEndpoint = configuration["CDN:Endpoint"];

            CreateMap<ProductDTO, ProductViewModel>()
                .ForMember(vm => vm.Components, map => map.MapFrom(p => p.Components));
                // TODO.ForMember(vm => vm.Image, map => map.MapFrom(i => i.Image.Replace(storageBlobEndpoint, cdnEndpoint)));

            CreateMap<ProductComponentDTO, ProductComponentViewModel>()
                .ForMember(vm => vm.Medias, map => map.MapFrom(c => c.Medias));

            CreateMap<ProductMediaDTO, ProductMediaViewModel>();
            //.ForMember(vm => vm.Url, map => map.MapFrom(i => i.Url.Replace(storageBlobEndpoint, cdnEndpoint)));

            CreateMap<Order, OrderViewModel>();
            CreateMap<OrderDetail, OrderDetailViewModel>();
        }
    }
}
