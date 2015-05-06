using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Sudokuer.ViewModels;

namespace Sudokuer.Views
{
    /// <summary>
    /// Interaction logic for SudokuPanel.xaml
    /// </summary>
    public partial class SudokuPanel
    {
        #region Nested types

        private class ValueTypeToBrushConverter : IValueConverter
        {
            #region Fields

            public static readonly ValueTypeToBrushConverter Instance = new ValueTypeToBrushConverter();

            #endregion

            #region Methods

            #region IValueConverter members

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var t = (SudokuViewModel.ValueTypes) value;
                Color clr;
                switch (t)
                {
                    case SudokuViewModel.ValueTypes.Puzzle:
                        clr = Colors.Black;
                        break;
                    case SudokuViewModel.ValueTypes.Key:
                        clr = Colors.Blue;
                        break;
                    default: //case SudokuViewModel.ValueTypes.KeyJustUpdated:
                        clr = Colors.Red;
                        break;
                }
                return new SolidColorBrush(clr);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }

            #endregion

            #endregion
        }

        #endregion

        #region Fields

        public static readonly DependencyProperty SubregionSizeProperty = DependencyProperty.Register("SubregionSize",
            typeof (int), typeof (SudokuPanel), new PropertyMetadata(3, PropertyChangedCallback));

        private TextBox[,] _textBoxes;

        #endregion

        #region Constructors

        public SudokuPanel()
        {
            InitializeComponent();

            UpdateGrid();
        }

        #endregion

        #region Properties

        public int SubregionSize
        {
            get { return (int) GetValue(SubregionSizeProperty); }
            set { SetValue(SubregionSizeProperty, value); }
        }

        public int FocusedCellRow { get; private set; }

        public int FocusedCellCol { get; private set; }


        #endregion

        #region Methods

        public void SetFocus(int row, int col)
        {
            var size = SubregionSize*SubregionSize;
            if (row < 0 || row >= size || col < 0 || col >= size)
            {
                return;
            }
            _textBoxes[row, col].Focus();
        }

        private void UpdateGrid()
        {
            var srsize = SubregionSize;
            var size = srsize*srsize;
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();

            if (_textBoxes != null)
            {
                foreach (var text in _textBoxes)
                {
                    text.GotFocus -= TextGotFocus;
                }
            }

            MainGrid.Children.Clear();

            const double thickLine = 3;
            const double thinLine = 0;

            for (var i = 0; i < size; i++)
            {
                var thickness = i % srsize == 0 ? thickLine : thinLine;
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(thickness) });
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(thickness) });
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(thickLine) });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(thickLine) });

            var subregionBoundaryBrush = new SolidColorBrush(Colors.Black);
            var totalRowCount = size*2 + 1; 

            // colors for the subregion boundaries
            for (var i = 0; i < SubregionSize+1; i++)
            {
                var l = i*SubregionSize*2;
                
                var horizontal = new Grid {Background = subregionBoundaryBrush};
                horizontal.SetValue(Grid.ColumnSpanProperty, totalRowCount);
                horizontal.SetValue(Grid.RowProperty, l);
                MainGrid.Children.Add(horizontal);
                
                var vertical = new Grid {Background = subregionBoundaryBrush};
                vertical.SetValue(Grid.RowSpanProperty, totalRowCount);
                vertical.SetValue(Grid.ColumnProperty, l);
                MainGrid.Children.Add(vertical);
            }

            _textBoxes = new TextBox[size,size];
            FocusedCellRow = -1;
            FocusedCellCol = -1;

            var fontSize = 18*3/srsize;
            // adds text boxes
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var text = new TextBox();
                    text.SetValue(Grid.RowProperty, i*2+1);
                    text.SetValue(Grid.ColumnProperty, j*2+1);
                    text.HorizontalContentAlignment = HorizontalAlignment.Center;
                    text.VerticalContentAlignment= VerticalAlignment.Center;
                    text.FontFamily = new FontFamily("Arial");
                    text.FontSize = fontSize;

                    var s = string.Format("Cells[{0}][{1}].Value", i, j);
                    text.SetBinding(TextBox.TextProperty, new Binding(s) {Mode = BindingMode.TwoWay});

                    s = string.Format("Cells[{0}][{1}].ValueType", i, j);
                    text.SetBinding(ForegroundProperty, new Binding(s) { Converter = ValueTypeToBrushConverter.Instance });

                    text.GotFocus += TextGotFocus;
                    _textBoxes[i, j] = text;

                    MainGrid.Children.Add(text);
                }
            }            
        }

        private void TextGotFocus(object sender, RoutedEventArgs e)
        {
            var text = (TextBox) sender;
            FocusedCellRow = ((int)text.GetValue(Grid.RowProperty)-1)/2;
            FocusedCellCol = ((int)text.GetValue(Grid.ColumnProperty)-1)/2;
        }

        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((SudokuPanel)obj).UpdateGrid();
        }

        #endregion
    }
}
