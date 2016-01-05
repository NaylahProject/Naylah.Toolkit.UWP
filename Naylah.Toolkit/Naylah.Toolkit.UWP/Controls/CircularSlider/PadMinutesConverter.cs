using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Controls.CircularSlider
{
    public class PadMinutesConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int result;
            if (!int.TryParse(value.ToString(), out result))
                throw new NotImplementedException();

            if (result < 10)
                return "0" + result;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
