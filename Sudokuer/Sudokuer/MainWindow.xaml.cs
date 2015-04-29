using System.Reflection;
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

        #region Event handlers

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetTitle();
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
