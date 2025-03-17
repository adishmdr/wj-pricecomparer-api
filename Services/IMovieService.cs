// wj-api/Services/IMovieService.cs
using Microsoft.AspNetCore.Http;
using wj_api.Models;

namespace wj_api.Services
{
    public interface IMovieService
    {
        Task<List<MovieComparison>> GetMoviesAsync(IHttpContextAccessor httpContextAccessor, bool forceRefresh = false);
        Task<MovieComparison> CompareMovieAsync(string cinemaWorldId, string filmWorldId, IHttpContextAccessor httpContextAccessor);
    }
}