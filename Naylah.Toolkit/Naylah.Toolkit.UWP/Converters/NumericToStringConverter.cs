using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converters
{
    public class NumericToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double dvalue = 0;

            try
            {
                dvalue = System.Convert.ToDouble(value);
            }
            catch (Exception)
            {
            }

            return string.Format(CultureInfo.CurrentCulture, "{0:N}", dvalue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
