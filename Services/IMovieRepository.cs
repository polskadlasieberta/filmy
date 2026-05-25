using System.Collections.Generic;
using filmy.Models;

namespace filmy.Services
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> GetAll();
        void Add(Movie movie);
        void Remove(Movie movie);
    }
}
