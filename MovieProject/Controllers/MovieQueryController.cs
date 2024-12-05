using Microsoft.AspNetCore.Mvc;
using MovieProject.Repository.Interfaces;

namespace MovieProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieQueryController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MovieQueryController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet("by-director")]
        public IActionResult GetMoviesByDirector([FromQuery] string director)
        {
            var movies = (from m in _movieRepository.GetAll()
                          where m.Director == director
                          select m).ToList();
            return Ok(movies);
        }

        [HttpGet("released-in-year")]
        public IActionResult GetMoviesReleasedInYear([FromQuery] int year)
        {
            var movies = _movieRepository.GetAll()
                .Where(m => m.ReleaseYear == year)
                .ToList();
            return Ok(movies);
        }

        [HttpGet("by-genre")]
        public IActionResult GetMoviesByGenre([FromQuery] string genre)
        {
            var movies = (from m in _movieRepository.GetAll()
                          where m.Genre == genre
                          select m).ToList();
            return Ok(movies);
        }

        [HttpGet("highly-rated")]
        public IActionResult GetHighlyRatedMovies([FromQuery] decimal minRating = 8.5M)
        {
            var movies = _movieRepository.GetAll()
                .Where(m => m.Rating > minRating)
                .ToList();
            return Ok(movies);
        }

        [HttpGet("top-rated")]
        public IActionResult GetTopRatedMovies([FromQuery] int count = 5)
        {
            var movies = (from m in _movieRepository.GetAll()
                          orderby m.Rating descending
                          select m).Take(count).ToList();

            return Ok(movies);
        }

        [HttpGet("released-between")]
        public IActionResult GetMoviesReleasedBetween([FromQuery] int startYear, [FromQuery] int endYear)
        {
            var movies = _movieRepository.GetAll()
                .Where(m => m.ReleaseYear >= startYear && m.ReleaseYear <= endYear)
                .ToList();
            return Ok(movies);
        }

        [HttpGet("action-highly-rated")]
        public IActionResult GetActionMoviesWithHighRating()
        {
            var movies = (from m in _movieRepository.GetAll()
                          where m.Genre == "Action" && m.Rating > 8.0M
                          select m).ToList();
            return Ok(movies);
        }

        [HttpGet("sorted")]
        public IActionResult GetSortedMovies()
        {
            var movies = _movieRepository.GetAll()
                .OrderBy(m => m.ReleaseYear)
                .ThenByDescending(m => m.Rating)
                .ToList();
            return Ok(movies);
        }

        [HttpGet("count-by-genre")]
        public IActionResult GetMovieCountsByGenre()
        {
            var counts = (from m in _movieRepository.GetAll()
                          group m by m.Genre into g
                          select new { Genre = g.Key, Count = g.Count() }).ToList();
            return Ok(counts);
        }

        [HttpGet("top-tarantino")]
        public IActionResult GetTopTarantinoMovies([FromQuery] int count = 3)
        {
            var movies = _movieRepository.GetAll()
                .Where(m => m.Director == "Quentin Tarantino")
                .OrderByDescending(m => m.Rating)
                .Take(count)
                .ToList();
            return Ok(movies);
        }

        [HttpGet("average-rating-by-genre")]
        public IActionResult GetAverageRatingByGenre()
        {
            var averages = _movieRepository.GetAll()
                .GroupBy(m => m.Genre)
                .Select(g => new { Genre = g.Key, AverageRating = g.Average(m => m.Rating) })
                .ToList();
            return Ok(averages);
        }

        [HttpGet("nineties-highly-rated")]
        public IActionResult GetNinetiesHighlyRatedMovies()
        {
            var movies = (from m in _movieRepository.GetAll()
                          where m.ReleaseYear >= 1990 && m.ReleaseYear <= 1999 && m.Rating > 8.0M
                          select m).ToList();
            return Ok(movies);
        }

        [HttpGet("king-high-rated")]
        public IActionResult GetKingMovies()
        {
            var movies = _movieRepository.GetAll()
                .Where(m => m.Title.Contains("King") && m.Rating > 8.5M)
                .ToList();
            return Ok(movies);
        }

        [HttpGet("top-per-genre")]
        public IActionResult GetTopMoviesByGenre([FromQuery] int count = 5)
        {
            var movies = _movieRepository.GetAll()
                .AsEnumerable()
                .GroupBy(m => m.Genre)
                .SelectMany(g => g
                    .OrderByDescending(m => m.Rating)
                    .Take(count))
                .ToList();

            return Ok(movies);
        }

        [HttpGet("nolan-above-average")]
        public IActionResult GetNolanAboveAverageMovies()
        {
            var allMovies = _movieRepository.GetAll();
            var averageRating = allMovies.Average(m => m.Rating);
            var movies = allMovies
                .Where(m => m.Director == "Christopher Nolan" && m.Rating > averageRating)
                .ToList();
            return Ok(movies);
        }
    }
}
