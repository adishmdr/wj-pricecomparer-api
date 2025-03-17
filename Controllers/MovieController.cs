// wj-api/Controllers/MovieController.cs
using Microsoft.AspNetCore.Mvc;
using wj_api.Services;
using Microsoft.AspNetCore.Http;

namespace wj_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovieController(IMovieService movieService, IHttpContextAccessor httpContextAccessor)
        {
            _movieService = movieService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetMoviesAsync(_httpContextAccessor);
            return Ok(movies);
        }

        [HttpGet("compare")]
        public async Task<IActionResult> CompareMovie([FromQuery] string cinemaWorldId, [FromQuery] string filmWorldId)
        {
            try
            {
                var comparison = await _movieService.CompareMovieAsync(cinemaWorldId, filmWorldId, _httpContextAccessor);
                return Ok(comparison);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("MovieController is working");
        }
    }
}