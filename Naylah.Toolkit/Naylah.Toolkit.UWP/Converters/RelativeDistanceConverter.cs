using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converters
{
    public class RelativeDistanceConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {

            try
            {
                var val = System.Convert.ToDouble(value);

                return Math.Round((val / 1000), 2).ToString() + " KMs";
            }
            catch (Exception)
            {
                return "";

            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
