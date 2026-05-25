using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using filmy.Helpers;
using filmy.Models;
using filmy.Services;

namespace filmy.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IMovieRepository _repository;
        public ObservableCollection<Movie> Movies { get; } = new();
        public ICollectionView MoviesView { get; }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText == value) return;
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                MoviesView.Refresh();
            }
        }

        private string _selectedGenre = "Wszystkie";
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre == value) return;
                _selectedGenre = value;
                OnPropertyChanged(nameof(SelectedGenre));
                MoviesView.Refresh();
            }
        }

        public ObservableCollection<string> Genres { get; } = new ObservableCollection<string>();

        private string _newTitle = string.Empty;
        public string NewTitle
        {
            get => _newTitle;
            set
            {
                if (_newTitle == value) return;
                _newTitle = value;
                OnPropertyChanged(nameof(NewTitle));
                if (AddCommand is RelayCommand addRc) addRc.RaiseCanExecuteChanged();
            }
        }

        private int _newYear = DateTime.Now.Year;
        public int NewYear
        {
            get => _newYear;
            set
            {
                if (_newYear == value) return;
                _newYear = value;
                OnPropertyChanged(nameof(NewYear));
            }
        }

        private string _newGenre = string.Empty;
        public string NewGenre
        {
            get => _newGenre;
            set
            {
                if (_newGenre == value) return;
                _newGenre = value;
                OnPropertyChanged(nameof(NewGenre));
            }
        }

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
                if (RemoveCommand is RelayCommand remRc) remRc.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }

        public MainViewModel(IMovieRepository repository)
        {
            _repository = repository;

            foreach (var m in _repository.GetAll())
                Movies.Add(m);

            MoviesView = CollectionViewSource.GetDefaultView(Movies);
            MoviesView.Filter = FilterPredicate;

            UpdateGenres();

            AddCommand = new RelayCommand(_ => AddMovie(), _ => !string.IsNullOrWhiteSpace(NewTitle));
            RemoveCommand = new RelayCommand(_ => RemoveSelected(), _ => SelectedMovie != null);
            ClearFilterCommand = new RelayCommand(_ => { FilterText = string.Empty; SelectedGenre = "Wszystkie"; });
        }

        private bool FilterPredicate(object? obj)
        {
            if (obj is not Movie m) return false;

            var matchText = string.IsNullOrWhiteSpace(FilterText) ||
                            m.Title.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;

            var matchGenre = SelectedGenre == "Wszystkie" || string.IsNullOrEmpty(SelectedGenre) ||
                             string.Equals(m.Genre, SelectedGenre, StringComparison.OrdinalIgnoreCase);

            return matchText && matchGenre;
        }

        private void AddMovie()
        {
            var movie = new Movie
            {
                Title = NewTitle.Trim(),
                Year = NewYear,
                Genre = NewGenre?.Trim() ?? string.Empty
            };
            _repository.Add(movie);
            Movies.Add(movie);
            UpdateGenres();

            NewTitle = string.Empty;
            NewYear = DateTime.Now.Year;
            NewGenre = string.Empty;

            if (AddCommand is RelayCommand addRc) addRc.RaiseCanExecuteChanged();
        }

        private void RemoveSelected()
        {
            if (SelectedMovie == null) return;
            _repository.Remove(SelectedMovie);
            Movies.Remove(SelectedMovie);
            SelectedMovie = null;
            UpdateGenres();
        }

        private void UpdateGenres()
        {
            var genres = Movies.Select(m => m.Genre).Where(g => !string.IsNullOrWhiteSpace(g)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(g => g).ToList();
            Genres.Clear();
            Genres.Add("Wszystkie");
            foreach (var g in genres) Genres.Add(g);
            if (!Genres.Contains(SelectedGenre)) SelectedGenre = "Wszystkie";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
