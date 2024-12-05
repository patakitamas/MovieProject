using System.ComponentModel.DataAnnotations;

namespace MovieProject.API.DTOs
{
    public class CreateMovieDto
    {
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(100)]
        public string Director { get; set; }
        public int ReleaseYear { get; set; }
        [MaxLength(50)]
        public string Genre { get; set; }
        public decimal Rating { get; set; }
    }
}
