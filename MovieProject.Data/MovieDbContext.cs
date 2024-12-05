using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MovieProject.Data.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieProject.Data
{
    public class MovieDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && optionsBuilder != null)
            {
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\MovieDB.mdf;Integrated Security=True");
            }
        }
    }
}