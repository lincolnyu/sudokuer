using System;
using System.Globalization;
using System.Windows.Data;

namespace Sudokuer.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        #region Methods

        #region IValueConverter members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(long))
            {
                long lval;
                if (!long.TryParse((string)value, out lval))
                {
                    lval = 0;
                }
                return lval;
            }
            int ival;
            if (!int.TryParse((string)value, out ival))
            {
                ival = 0;
            }
            return ival;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value);
        }

        #endregion

        #endregion
    }
}
