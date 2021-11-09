using AutoMapper;
using RateLimiter.Domain.Entities;
using RateLimiter.ViewModels;

namespace RateLimiter.Application.MapperProfiles
{
    /// <summary>
    /// Automapper profile
    /// </summary>
    public class DomainToViewModelProfile : Profile
    {
        /// <summary>
        /// Configure domain/viewmodel mappings
        /// </summary>
        public DomainToViewModelProfile()
        {
            CreateMap<ResourceA, ResourceAReadViewModel>();
            CreateMap<ResourceAReadViewModel, ResourceA>();

            CreateMap<ResourceB, ResourceBReadViewModel>();
            CreateMap<ResourceBReadViewModel, ResourceB>();
        }
    }
}
