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

            CreateMap<Product, ProductViewModel>()
                .ForMember(vm => vm.Components, map => map.MapFrom(p => p.Components))
                .ForMember(vm => vm.CdnImage, map => map.MapFrom(i => 
                    i.Image.Replace(i.Image.Substring(0, i.Image.LastIndexOf(".net") + 4), cdnEndpoint)));

            CreateMap<ProductComponent, ProductComponentViewModel>()
                .ForMember(vm => vm.Medias, map => map.MapFrom(c => c.Medias));

            CreateMap<ProductMedia, ProductMediaViewModel>()
                .ForMember(vm => vm.CdnUrl, map => map.MapFrom(i => 
                i.Url.Replace(i.Url.Substring(0, i.Url.LastIndexOf(".net") + 4), cdnEndpoint)));

            CreateMap<Order, OrderViewModel>();
            CreateMap<OrderDetail, OrderDetailViewModel>()
                .ForMember(vm => vm.ProductImage, map => map.MapFrom(od =>
                        storageBlobEndpoint + "product-images/" + od.ProductModel + "/1.jpg"))
                .ForMember(vm => vm.ProductCdnImage, map => map.MapFrom(od =>
                        cdnEndpoint + "/product-images/" + od.ProductModel + "/1.jpg"));
        }
    }
}
