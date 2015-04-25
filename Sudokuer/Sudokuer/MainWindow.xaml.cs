using System.Windows;
using Sudokuer.ViewModels;

namespace Sudokuer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new SudokuViewModel();
        }

        #endregion

        #region Methods

        private void SolveClicked(object sender, RoutedEventArgs e)
        {
            var vm = (SudokuViewModel) DataContext;
            vm.Solve();
        }

        #endregion
    }
}
