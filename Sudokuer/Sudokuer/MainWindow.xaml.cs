using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Sudokuer.ViewModels;
using Sudokuer.Views;

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
            var moved = false;
            var r = Sp.FocusedCellRow;
            var c = Sp.FocusedCellCol;

            var leftKeyState = Keyboard.GetKeyStates(Key.LeftShift);
            var rightKeyState = Keyboard.GetKeyStates(Key.RightShift);

            var shiftDown = (leftKeyState & KeyStates.Down) != 0 || (rightKeyState & KeyStates.Down) != 0;

            if (!shiftDown)
            {
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
            }
            
            if (moved)
            {
                if (Sp.FocusedCellRow < 0 || Sp.FocusedCellCol < 0)
                {
                    c = r = 0;
                }
                Sp.SetFocus(r, c);
            }
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

        private void ExportClicked(object sender, RoutedEventArgs e)
        {
            var vm = (SudokuViewModel)DataContext;
            var dlg = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            const long defaultMaxNumSols = 10000;
            if (dlg.ShowDialog() == true)
            {
                var mnsdlg = new MaxNumSolutionsDialog
                {
                    Owner = this,
                    Title = Title,
                    MaxNumSols = defaultMaxNumSols
                };
                mnsdlg.ShowDialog();
                var maxToShow = mnsdlg.MaxNumSols;

                var file = dlg.FileName;
                using (var sw = new StreamWriter(file))
                {
                    vm.Export(sw, maxToShow);
                }
                MessageBox.Show("Exporting has been completed.", Title);
            }
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
