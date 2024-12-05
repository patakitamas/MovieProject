using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.API.Attributes;
using MovieProject.API.DTOs;
using MovieProject.Data;
using MovieProject.Data.Models.Domain;
using MovieProject.Repository.Interfaces;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Xml.Serialization;

namespace MovieProject.API.Controllers
{
    public delegate void DataUploadSuccessHandler();

    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository movieRepository;
        private readonly ILogger<MovieController> logger;
        private readonly MovieDbContext dbContext;
        public event DataUploadSuccessHandler DataUploadedSuccessfully;
        public MovieController(IMovieRepository movieRepository, ILogger<MovieController> logger, MovieDbContext dbContext)
        {
            this.movieRepository = movieRepository;
            this.logger = logger;
            DataUploadedSuccessfully += MovieController_DataUploadedSuccessfully;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(movieRepository.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var movie = await movieRepository.Get(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMovieDto updateMovieDto)
        {
            Movie movie = new Movie()
            {
                Director = updateMovieDto.Director,
                Genre = updateMovieDto.Genre,
                Rating = updateMovieDto.Rating,
                ReleaseYear = updateMovieDto.ReleaseYear,
                Title = updateMovieDto.Title
            };

            movie = await movieRepository.Update(id, movie);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var movie = await movieRepository.Delete(id);
            if (movie == null)
            {
                NotFound();
            }

            return Ok(movie);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] CreateMovieDto createMovieDto)
        {
            Movie movie = new Movie()
            {
                Director = createMovieDto.Director,
                Genre = createMovieDto.Genre,
                ReleaseYear = createMovieDto.ReleaseYear,
                Rating = createMovieDto.Rating,
                Title = createMovieDto.Title
            };
            Movie createdMovie = await movieRepository.Create(movie);
            return CreatedAtAction(nameof(Get), new { id = movie.Id }, createdMovie);
        }

        private void MovieController_DataUploadedSuccessfully()
        {
            Console.WriteLine("Data has been successfully uploaded to the database.");
        }

        [HttpGet("export-all")]
        public async Task<IActionResult> ExportAllMovies()
        {
            try
            {
                var movies = await dbContext.Movies.ToListAsync();

                var jsonData = JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true });

                var byteArray = System.Text.Encoding.UTF8.GetBytes(jsonData);
                var fileContentResult = new FileContentResult(byteArray, "application/json")
                {
                    FileDownloadName = "ExportedMovies.json"
                };

                return fileContentResult;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while exporting movies.", Error = ex.Message });
            }
        }

        [HttpPost("upload-xml")]
        [ValidateModel]
        public async Task<IActionResult> UploadXML(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                return BadRequest();
            }

            try
            {
                MovieXml movieXml;
                using (var stream = xmlFile.OpenReadStream())
                {
                    var serializer = new XmlSerializer(typeof(MovieXml));
                    movieXml = (MovieXml)serializer.Deserialize(stream);
                }

                foreach (var moviesXml in movieXml.Movies)
                {
                    var createMovieDto = new CreateMovieDto
                    {
                        Title = moviesXml.Title,
                        Director = moviesXml.Director,
                        Genre = moviesXml.Genre,
                        ReleaseYear = moviesXml.ReleaseYear,
                        Rating = moviesXml.Rating,
                    };

                    await Create(createMovieDto);
                }
                OnDataUploadedSuccessfully();

                return Ok($"Successfully created {movieXml.Movies.Count} movies.");
            }
            catch (InvalidOperationException e)
            {
                return BadRequest($"XML format error: {e.Message}");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error uploading data.");
                return StatusCode(500);
            }
        }
        private void OnDataUploadedSuccessfully()
        {
            DataUploadedSuccessfully?.Invoke();
        }
    }
}
