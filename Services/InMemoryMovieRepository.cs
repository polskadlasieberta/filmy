using System.Collections.Generic;
using System.Linq;
using filmy.Models;

namespace filmy.Services
{
    public class InMemoryMovieRepository : IMovieRepository
    {
        private readonly List<Movie> _items = new()
        {
            new Movie { Title = "Incepcja", Year = 2010, Genre = "Sci-Fi" },
            new Movie { Title = "Matrix", Year = 1999, Genre = "Sci-Fi" },
            new Movie { Title = "Am�lie", Year = 2001, Genre = "Drama" }
        };

        public IEnumerable<Movie> GetAll() => _items.ToList();

        public void Add(Movie movie)
        {
            _items.Add(movie);
        }

        public void Remove(Movie movie)
        {
            var found = _items.FirstOrDefault(m => m.Id == movie.Id);
            if (found != null) _items.Remove(found);
        }
    }
}
