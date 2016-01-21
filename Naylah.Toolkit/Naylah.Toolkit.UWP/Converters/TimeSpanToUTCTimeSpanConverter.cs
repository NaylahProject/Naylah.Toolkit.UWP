using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converters
{
    public class TimeSpanToUTCTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                TimeSpan t = (TimeSpan)value;

                var now = DateTime.Now;
                var oh = now.ToUniversalTime() - now;

                t = t.Subtract(oh);

                return t;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                TimeSpan t;
                if (!TimeSpan.TryParse((string)value, out t))
                {
                    throw new Exception();
                }

                var now = DateTime.Now;
                var oh = now.ToUniversalTime() - now;

                t = t.Add(oh);

                return t;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
