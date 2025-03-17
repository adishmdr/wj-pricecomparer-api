// wj-api/Services/MovieService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using wj_api.Models;
using Polly;
using System.Text.Json;

namespace wj_api.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _cinemaWorldClient;
        private readonly HttpClient _filmWorldClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MovieService> _logger;

        public MovieService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache cache,
            ILogger<MovieService> logger)
        {
            _configuration = configuration;
            _cache = cache;
            _logger = logger;

            _cinemaWorldClient = httpClientFactory.CreateClient("CinemaWorld");
            _filmWorldClient = httpClientFactory.CreateClient("FilmWorld");

            _logger.LogDebug("CinemaWorld Client Headers: {Headers}",
                JsonSerializer.Serialize(_cinemaWorldClient.DefaultRequestHeaders,
                new JsonSerializerOptions { WriteIndented = true }));
            _logger.LogDebug("FilmWorld Client Headers: {Headers}",
                JsonSerializer.Serialize(_filmWorldClient.DefaultRequestHeaders,
                new JsonSerializerOptions { WriteIndented = true }));
        }

        public async Task<List<MovieComparison>> GetMoviesAsync(IHttpContextAccessor httpContextAccessor)
        {
            const string cacheKey = "AllMovies";

            if (_cache.TryGetValue(cacheKey, out List<MovieComparison>? cachedMovies) && cachedMovies != null)
            {
                _logger.LogInformation("Returning cached movies:");
                _logger.LogInformation(JsonSerializer.Serialize(cachedMovies, new JsonSerializerOptions { WriteIndented = true }));
                return cachedMovies;
            }

            var token = httpContextAccessor.HttpContext?.Request.Headers["x-access-token"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No x-access-token provided in request headers.");
            }
            else
            {
                _logger.LogDebug("Using x-access-token: {Token}", token);
            }

            var cinemaWorldMovies = await FetchMoviesAsync(_cinemaWorldClient, "cinemaworld", token);
            var filmWorldMovies = await FetchMoviesAsync(_filmWorldClient, "filmworld", token);

            // Group movies by title
            var allTitles = cinemaWorldMovies.Select(m => m.Title)
                .Union(filmWorldMovies.Select(m => m.Title))
                .Distinct()
                .Where(t => !string.IsNullOrEmpty(t));  // Filter out null or empty titles

            var comparisons = new List<MovieComparison>();
            foreach (var title in allTitles)
            {
                var comparison = new MovieComparison
                {
                    Title = title,
                    CinemaWorldMovie = cinemaWorldMovies.FirstOrDefault(m => m.Title == title),
                    FilmWorldMovie = filmWorldMovies.FirstOrDefault(m => m.Title == title)
                };

                // Only add if at least one movie is non-null
                if (comparison.CinemaWorldMovie != null || comparison.FilmWorldMovie != null)
                {
                    comparisons.Add(comparison);
                }
            }

            _logger.LogInformation("All Movies (Grouped):");
            _logger.LogInformation(JsonSerializer.Serialize(comparisons, new JsonSerializerOptions { WriteIndented = true }));

            _cache.Set(cacheKey, comparisons, TimeSpan.FromMinutes(30));
            return comparisons;
        }

        public async Task<MovieComparison> CompareMovieAsync(string cinemaWorldId, string filmWorldId, IHttpContextAccessor httpContextAccessor)
        {
            if (string.IsNullOrEmpty(cinemaWorldId) || string.IsNullOrEmpty(filmWorldId))
            {
                throw new ArgumentException("Both CinemaWorld and FilmWorld IDs must be provided.");
            }

            var token = httpContextAccessor.HttpContext?.Request.Headers["x-access-token"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No x-access-token provided in request headers.");
            }
            else
            {
                _logger.LogDebug("Using x-access-token: {Token}", token);
            }

            var cinemaWorldMovie = await FetchMovieDetailsAsync(_cinemaWorldClient, "cinemaworld", cinemaWorldId, token);
            var filmWorldMovie = await FetchMovieDetailsAsync(_filmWorldClient, "filmworld", filmWorldId, token);

            if (cinemaWorldMovie == null && filmWorldMovie == null)
            {
                throw new ArgumentException("No movies found for the provided IDs.");
            }

            var comparison = new MovieComparison();

            if (cinemaWorldMovie?.Title == filmWorldMovie?.Title && cinemaWorldMovie != null && filmWorldMovie != null)
            {
                comparison.Title = cinemaWorldMovie.Title;
                comparison.CinemaWorldMovie = cinemaWorldMovie;
                comparison.FilmWorldMovie = filmWorldMovie;
            }
            else
            {
                if (cinemaWorldMovie != null)
                {
                    comparison.Title = cinemaWorldMovie.Title;
                    comparison.CinemaWorldMovie = cinemaWorldMovie;
                }
                else if (filmWorldMovie != null)
                {
                    comparison.Title = filmWorldMovie.Title;
                    comparison.FilmWorldMovie = filmWorldMovie;
                }
            }

            _logger.LogInformation("Movie Comparison:");
            _logger.LogInformation(JsonSerializer.Serialize(comparison, new JsonSerializerOptions { WriteIndented = true }));

            return comparison;
        }

        private async Task<List<Movie>> FetchMoviesAsync(HttpClient client, string provider, string? token)
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogDebug("Sending request to {Provider} API for movie list", provider);
                    var request = new HttpRequestMessage(HttpMethod.Get, "movies");
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Add("x-access-token", token);
                    }
                    var response = await client.SendAsync(request);
                    _logger.LogDebug("Movie List Response Status: {Status}", response.StatusCode);
                    Console.WriteLine("Movie List Response: " + await response.Content.ReadAsStringAsync());
                    response.EnsureSuccessStatusCode();
                    var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
                    var movies = movieResponse?.Movies ?? new List<Movie>();
                    foreach (var movie in movies)
                    {
                        movie.Provider = provider;
                    }
                    return movies;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movies from {Provider}", provider);
                return new List<Movie>();
            }
        }

        private async Task<Movie?> FetchMovieDetailsAsync(HttpClient client, string provider, string movieId, string? token)
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            try
            {
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogDebug("Fetching details for {Provider} movie {Id}", provider, movieId);
                    var request = new HttpRequestMessage(HttpMethod.Get, $"movie/{movieId}");
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Add("x-access-token", token);
                    }
                    var response = await client.SendAsync(request);
                    _logger.LogDebug("Movie Detail Response Status: {Status}", response.StatusCode);
                    Console.WriteLine($"Movie Detail Response ({movieId}): " + await response.Content.ReadAsStringAsync());
                    response.EnsureSuccessStatusCode();
                    var movie = await response.Content.ReadFromJsonAsync<Movie>();
                    if (movie != null)
                    {
                        movie.Provider = provider;
                    }
                    return movie;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie details from {Provider} for ID {Id}", provider, movieId);
                return null;
            }
        }
    }
}