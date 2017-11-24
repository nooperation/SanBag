using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SanBag.Converters
{
    public class SizeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sizeInBytes = (uint)value;

            const float kGb = 1024 * 1024 * 1024;
            const float kMb = 1024 * 1024;
            const float kKb = 1024;

            if (sizeInBytes >= kGb)
            {
                return $"{sizeInBytes / kGb:0.00} GB";
            }
            else if (sizeInBytes >= kMb)
            {
                return $"{sizeInBytes / kMb:0.00} MB";
            }
            else if (sizeInBytes >= kKb)
            {
                return $"{sizeInBytes / kKb:0.00} KB";
            }

            return $"{sizeInBytes} B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
