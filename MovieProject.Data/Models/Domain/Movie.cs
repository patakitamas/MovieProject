using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MovieProject.Data.Models.Domain
{
    [XmlRoot("MovieData")]
    public class MovieXml
    {
        [XmlElement("Movie")]
        public List<Movie> Movies { get; set;}
    }
    public class Movie
    {
        [Key]
        public int Id { get; set; }
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
