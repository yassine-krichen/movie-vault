using AutoMapper;
using Tp3.Models;
using Tp3.ViewModels;

namespace Tp3.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Movie Mappings
            CreateMap<Movie, MovieViewModel>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre != null ? src.Genre.GenreName : null));

            CreateMap<MovieViewModel, Movie>()
                .ForMember(dest => dest.Genre, opt => opt.Ignore())
                .ForMember(dest => dest.Customers, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.Ignore()); // Not in VM

            // Customer Mappings
            CreateMap<Customer, CustomerViewModel>()
                .ForMember(dest => dest.MembershipInfo, opt => opt.MapFrom(src => 
                    src.Membership != null 
                    ? $"{src.Membership.DurationInMonths} months - {src.Membership.DiscountRate}% discount" 
                    : "No membership"));

            CreateMap<CustomerViewModel, Customer>()
                .ForMember(dest => dest.Membership, opt => opt.Ignore())
                .ForMember(dest => dest.Movies, opt => opt.Ignore())
                .ForMember(dest => dest.IsSubscribedToNewsletter, opt => opt.Ignore()); // Not in VM

            // Genre Mappings
            CreateMap<Genre, GenreViewModel>().ReverseMap();

            // Membership Mappings
            CreateMap<Membership, MembershipViewModel>().ReverseMap();
        }
    }
}
