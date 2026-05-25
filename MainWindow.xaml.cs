using System.Windows;
using filmy.Services;
using filmy.ViewModels;

namespace filmy
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new InMemoryMovieRepository());
        }
    }
}
