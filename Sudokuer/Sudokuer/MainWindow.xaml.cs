using System.Reflection;
using System.Windows;
using System.Windows.Input;
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

        #region Event handlers

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetTitle();
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var r = Sp.FocusedCellRow;
            var c = Sp.FocusedCellCol;
            var moved = false;

            switch (e.Key)
            {
                case Key.Up:
                    r--;
                    moved = true;
                    break;
                case Key.Down:
                    r++;
                    moved = true;
                    break;
                case Key.Left:
                    c--;
                    moved = true;
                    break;
                case Key.Right:
                    c++;
                    moved = true;
                    break;
            }

            if (moved && (Sp.FocusedCellRow < 0 || Sp.FocusedCellCol < 0))
            {
                c = r = 0;
            }
            Sp.SetFocus(r, c);
        }
        
        private void SolveClicked(object sender, RoutedEventArgs e)
        {
            var vm = (SudokuViewModel) DataContext;
            var res = vm.Solve();
            if (!res)
            {
                MessageBox.Show("No Solutions", Title);
            }
        }

        private void ResetClicked(object sender, RoutedEventArgs e)
        {
            var vm = (SudokuViewModel)DataContext;
            vm.Reset();
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            var vm = (SudokuViewModel)DataContext;
            vm.Clear();
        }

        #endregion

        private void SetTitle()
        {
            try
            {
                var ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.
                    CurrentVersion;
                Title = string.Format("Sudokuer (Ver {0}.{1})", ver.Major, ver.Minor);
            }
            catch (System.Deployment.Application.InvalidDeploymentException)
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                Title = string.Format("Sudokuer (Asm Ver {0}.{1})", ver.Major, ver.Minor);
            }
        }

        #endregion
    }
}
