using Microsoft.EntityFrameworkCore;
using MovieProject.Data;
using MovieProject.Data.Models.Domain;
using MovieProject.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieProject.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext dbContext;

        public MovieRepository(MovieDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Movie> Create(Movie movie)
        {
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie?> Delete(int id)
        {
            var movie = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return null;
            }
            dbContext.Movies.Remove(movie);
            await dbContext.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie?> Get(int id)
        {
            var movie = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return null;
            }
            return movie;
        }

        public IQueryable<Movie> GetAll()
        {
            return dbContext.Movies.AsQueryable();
        }

        public async Task<Movie?> Update(int id, Movie movie)
        {
            var movieToUpdate = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieToUpdate == null)
            {
                return null;
            }

            movieToUpdate.ReleaseYear = movie.ReleaseYear;
            movieToUpdate.Title = movie.Title;
            movieToUpdate.Director = movie.Director;
            movieToUpdate.Genre = movie.Genre;
            movieToUpdate.Rating = movie.Rating;

            await dbContext.SaveChangesAsync();
            return movieToUpdate;
        }
    }
}
