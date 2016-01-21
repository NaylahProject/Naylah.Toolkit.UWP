using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converters
{
    public class NativeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // No format provided.
            if (parameter == null)
                return value;

            string[] parameters = parameter.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string format = (parameters.Length > 1 ? parameters[1] : "");
            return this.FormatString(value, format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string[] parameters = parameter.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string type = parameters[0];

            if (type == "Decimal")
                return decimal.Parse(value.ToString());
            if (type == "Int")
                return int.Parse(value.ToString());
            if (type == "DateTime")
                return DateTime.Parse(value.ToString());

            return value;
        }

        private string FormatString(object value, string format)
        {
            if (value != null && string.IsNullOrEmpty(format))
                return value.ToString();

            return string.Format(format, value);
        }
    }
}
