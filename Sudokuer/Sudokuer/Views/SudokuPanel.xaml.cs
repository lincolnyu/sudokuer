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

        #endregion

        #region Methods

        private void UpdateGrid()
        {
            var srsize = SubregionSize;
            var size = srsize*srsize;
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
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
                    
                    MainGrid.Children.Add(text);
                }
            }
        }

        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((SudokuPanel)obj).UpdateGrid();
        }

        #endregion
    }
}
