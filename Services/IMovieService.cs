// wj-api/Services/IMovieService.cs
using wj_api.Models;

namespace wj_api.Services
{
    public interface IMovieService
    {
        Task<List<MovieComparison>> GetMoviesAsync();  // Updated to return List<MovieComparison>
        Task<MovieComparison> CompareMovieAsync(string cinemaWorldId, string filmWorldId);
    }
}