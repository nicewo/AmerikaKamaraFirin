using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Collections.Generic;

namespace AmerikaKamaraFirin
{
    public class ToplamSureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Adim> adimlar)
            {
                int toplamDakika = adimlar.Sum(a => a.SureDakika);
                int saat = toplamDakika / 60;
                int dakika = toplamDakika % 60;
                return $"{saat}:{dakika:D2}"; // Dakika çift haneli (örneğin 1:05)
            }
            return "0:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
