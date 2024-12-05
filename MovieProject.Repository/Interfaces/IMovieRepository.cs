using MovieProject.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieProject.Repository.Interfaces
{
    public interface IMovieRepository
    {
        IQueryable<Movie> GetAll();
        Task<Movie?> Get(int id);
        Task<Movie> Create(Movie movie);
        Task<Movie?> Update(int id, Movie movie);
        Task<Movie?> Delete(int id);
    }
}
