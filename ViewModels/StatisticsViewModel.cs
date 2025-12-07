using Tp3.Models;

namespace Tp3.ViewModels
{
    public class StatisticsViewModel
    {
        public List<Movie> AvailableActionMovies { get; set; } = new();
        public List<Movie> MoviesOrderedByDateAndName { get; set; } = new();
        public int TotalMovieCount { get; set; }
        public List<Customer> SubscribedCustomersWithDiscount { get; set; } = new();
        public List<MovieGenreViewModel> MoviesWithGenres { get; set; } = new();
        public List<Genre> TopPopularGenres { get; set; } = new();
    }
}
