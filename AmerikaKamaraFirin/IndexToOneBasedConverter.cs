using System;
using System.Globalization;
using System.Windows.Data;

namespace AmerikaKamaraFirin.View
{
    public class IndexToOneBasedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int index ? (index + 1).ToString() : "1";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
