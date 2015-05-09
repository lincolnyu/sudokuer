using System.Windows;

namespace Sudokuer.Views
{
    /// <summary>
    /// Interaction logic for MaxNumSolutionsDialog.xaml
    /// </summary>
    public partial class MaxNumSolutionsDialog
    {
        #region Constructors

        public MaxNumSolutionsDialog()
        {
            InitializeComponent();

            DataContext = this;
        }

        #endregion

        #region Properties

        public long MaxNumSols { get; set; }

        #endregion

        #region Methods

        private void OkOnClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
