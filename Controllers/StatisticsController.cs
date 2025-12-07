using Microsoft.AspNetCore.Mvc;
using Tp3.Services.Interfaces;
using Tp3.ViewModels;

namespace Tp3.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICustomerService _customerService;
        private readonly IGenreService _genreService;

        public StatisticsController(IMovieService movieService, ICustomerService customerService, IGenreService genreService)
        {
            _movieService = movieService;
            _customerService = customerService;
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new StatisticsViewModel
            {
                AvailableActionMovies = await _movieService.GetAvailableActionMoviesAsync(),
                MoviesOrderedByDateAndName = await _movieService.GetMoviesOrderedByDateAndNameAsync(),
                TotalMovieCount = await _movieService.GetTotalMovieCountAsync(),
                SubscribedCustomersWithDiscount = await _customerService.GetSubscribedCustomersWithDiscountAsync(),
                MoviesWithGenres = await _movieService.GetMoviesWithGenresAsync(),
                TopPopularGenres = await _genreService.GetTopPopularGenresAsync()
            };

            return View(viewModel);
        }
    }
}
