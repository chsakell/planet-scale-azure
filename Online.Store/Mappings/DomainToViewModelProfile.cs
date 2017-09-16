using Online.Store.Core.DTOs;
using Online.Store.ViewModels;
using AutoMapper;

namespace Online.Store.Mappings
{
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile()
        {
            CreateMap<ProductDTO, ProductViewModel>()
                .ForMember(vm => vm.Components, map => map.MapFrom(p => p.Components));

            CreateMap<ProductComponentDTO, ProductComponentViewModel>()
                .ForMember(vm => vm.Medias, map => map.MapFrom(c => c.Medias));

            CreateMap<ProductMediaDTO, ProductMediaViewModel>();
        }
    }
}
