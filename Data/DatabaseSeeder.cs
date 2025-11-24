using Tp3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tp3.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private static readonly Random _random = new Random();

        public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Check if the database has any data already
        /// </summary>
        public async Task<bool> HasDataAsync()
        {
            var hasMemberships = await _context.Memberships.AnyAsync();
            var hasGenres = await _context.Genres.AnyAsync();
            var hasMovies = await _context.Movies.AnyAsync();
            var hasCustomers = await _context.Customers.AnyAsync();

            return hasMemberships || hasGenres || hasMovies || hasCustomers;
        }

        /// <summary>
        /// Reset the database by clearing all tables
        /// </summary>
        public async Task ResetDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Resetting database...");

                // Clear all tables (order matters due to foreign keys)
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"CustomerMovies\" CASCADE");
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Customers\" CASCADE");
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Movies\" CASCADE");
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Genres\" CASCADE");
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Memberships\" CASCADE");
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"AuditLogs\" CASCADE");

                _logger.LogInformation("Database reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting database");
                throw;
            }
        }

        /// <summary>
        /// Seed the database with sample data
        /// </summary>
        public async Task SeedDataAsync()
        {
            try
            {
                _logger.LogInformation("Seeding database with sample data...");

                // Check if data already exists
                if (await HasDataAsync())
                {
                    _logger.LogWarning("Database already contains data. Skipping seed operation.");
                    return;
                }

                // Seed memberships
                var memberships = SeedMemberships();
                await _context.Memberships.AddRangeAsync(memberships);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {memberships.Count} memberships");

                // Seed genres
                var genres = SeedGenres();
                await _context.Genres.AddRangeAsync(genres);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {genres.Count} genres");

                // Seed movies
                var seedGenres = await _context.Genres.ToListAsync();
                var movies = SeedMovies(seedGenres);
                await _context.Movies.AddRangeAsync(movies);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {movies.Count} movies");

                // Seed customers
                var seedMemberships = await _context.Memberships.ToListAsync();
                var seedMovies = await _context.Movies.ToListAsync();
                var customers = SeedCustomers(seedMemberships, seedMovies);
                await _context.Customers.AddRangeAsync(customers);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {customers.Count} customers");

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding database");
                throw;
            }
        }

        /// <summary>
        /// Generate dynamic membership plans
        /// </summary>
        private List<Membership> SeedMemberships()
        {
            var memberships = new List<Membership>
            {
                new Membership
                {
                    SignupFee = 0m,
                    DurationInMonths = 1,
                    DiscountRate = 0
                },
                new Membership
                {
                    SignupFee = 9.99m,
                    DurationInMonths = 3,
                    DiscountRate = 5
                },
                new Membership
                {
                    SignupFee = 19.99m,
                    DurationInMonths = 6,
                    DiscountRate = 10
                },
                new Membership
                {
                    SignupFee = 29.99m,
                    DurationInMonths = 12,
                    DiscountRate = 15
                },
                new Membership
                {
                    SignupFee = 49.99m,
                    DurationInMonths = 24,
                    DiscountRate = 20
                }
            };

            return memberships;
        }

        /// <summary>
        /// Generate diverse movie genres
        /// </summary>
        private List<Genre> SeedGenres()
        {
            var genres = new List<Genre>
            {
                new Genre { GenreName = "Action" },
                new Genre { GenreName = "Comedy" },
                new Genre { GenreName = "Drama" },
                new Genre { GenreName = "Horror" },
                new Genre { GenreName = "Romance" },
                new Genre { GenreName = "Sci-Fi" },
                new Genre { GenreName = "Thriller" },
                new Genre { GenreName = "Animation" },
                new Genre { GenreName = "Documentary" },
                new Genre { GenreName = "Fantasy" }
            };

            return genres;
        }

        /// <summary>
        /// Generate dynamic movies with varied dates and using default image
        /// </summary>
        private List<Movie> SeedMovies(List<Genre> genres)
        {
            var movieTitles = new[]
            {
                // Action
                "The Last Stand", "Fury", "Gladiator", "John Wick", "Mission Impossible",
                // Comedy
                "Superbad", "The Hangover", "Bridesmaids", "Tropic Thunder", "Step Brothers",
                // Drama
                "The Shawshank Redemption", "Forrest Gump", "The Pursuit of Happyness", "Interstellar", "Whiplash",
                // Horror
                "The Ring", "Insidious", "Conjuring", "Scary Movie", "A Quiet Place",
                // Romance
                "The Notebook", "Titanic", "La La Land", "Pride and Prejudice", "About Time",
                // Sci-Fi
                "Blade Runner", "The Matrix", "Inception", "Avatar", "Dune",
                // Thriller
                "Psycho", "The Dark Knight", "Se7en", "Shutter Island", "Memento",
                // Animation
                "Toy Story", "Frozen", "Inside Out", "Spirited Away", "Coco",
                // Documentary
                "Planet Earth", "Our Planet", "The Social Dilemma", "Free Solo", "Jane",
                // Fantasy
                "Harry Potter", "Lord of the Rings", "The Chronicles of Narnia", "Percy Jackson", "The Hobbit"
            };

            var movies = new List<Movie>();
            var defaultImagePath = "/images/movies/default.png";

            for (int i = 0; i < movieTitles.Length; i++)
            {
                var randomGenre = genres[_random.Next(genres.Count)];
                var daysOffset = _random.Next(-365, 0); // Random date within last year
                
                movies.Add(new Movie
                {
                    Name = movieTitles[i],
                    GenreId = randomGenre.Id,
                    ImageFile = defaultImagePath,
                    DateAjoutMovie = DateTime.Now.AddDays(daysOffset)
                });
            }

            return movies;
        }

        /// <summary>
        /// Generate dynamic customers with random memberships and movie associations
        /// </summary>
        private List<Customer> SeedCustomers(List<Membership> memberships, List<Movie> movies)
        {
            var firstNames = new[] { "John", "Jane", "Michael", "Emily", "David", "Sarah", "Chris", "Jessica", "Daniel", "Amanda", "James", "Jennifer", "Robert", "Lisa", "William", "Mary" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Taylor", "Anderson", "Thomas", "Moore" };

            var customers = new List<Customer>();

            for (int i = 0; i < 20; i++)
            {
                var firstName = firstNames[_random.Next(firstNames.Length)];
                var lastName = lastNames[_random.Next(lastNames.Length)];
                var randomMembership = memberships[_random.Next(memberships.Count)];

                var customer = new Customer
                {
                    Name = $"{firstName} {lastName}",
                    MembershipId = randomMembership.Id,
                    Movies = new List<Movie>()
                };

                // Assign random movies to customer (0-8 movies per customer)
                var movieCount = _random.Next(0, 9);
                var selectedMovies = new HashSet<Movie>();
                
                while (selectedMovies.Count < movieCount)
                {
                    selectedMovies.Add(movies[_random.Next(movies.Count)]);
                }

                customer.Movies = selectedMovies.ToList();
                customers.Add(customer);
            }

            return customers;
        }
    }
}
