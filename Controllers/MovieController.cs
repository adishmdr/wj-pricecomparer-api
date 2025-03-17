// wj-api/Controllers/MovieController.cs
using Microsoft.AspNetCore.Mvc;
using wj_api.Services;

namespace wj_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies()
        {
            Console.WriteLine("GetMovies endpoint hit from: " + HttpContext.Request.Headers["Origin"]);
            var movies = await _movieService.GetMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("compare")]
        public async Task<IActionResult> CompareMovie([FromQuery] string cinemaWorldId, [FromQuery] string filmWorldId)
        {
            try
            {
                var comparison = await _movieService.CompareMovieAsync(cinemaWorldId, filmWorldId);
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