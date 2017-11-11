using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SanBag.Views.Converters
{
    public class TimestampToStringConverter : IValueConverter
    {
        public object Convert(object timestampNs, Type targetType, object parameter, CultureInfo culture)
        {
            var timestampMs = (long)((long)timestampNs / 1e6);
            var date = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs);
            return date.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
