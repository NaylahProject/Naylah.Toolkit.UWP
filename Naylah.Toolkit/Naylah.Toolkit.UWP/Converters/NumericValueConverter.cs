using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converters
{
    public class NumericValueConverter : IValueConverter
    {

        public bool AllowDecimal { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return value.ToString();
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
                var svalue = (string)value;

                if (AllowDecimal)
                {

                    double humValue = 0;

                    if (string.IsNullOrEmpty(svalue) || Double.TryParse(svalue, out humValue))
                    {
                        return humValue;
                    }

                    return value;

                }
                else
                {

                    long humValue = 0;

                    if (string.IsNullOrEmpty(svalue) || long.TryParse(svalue, out humValue))
                    {
                        return humValue;
                    }

                    return value;

                }


            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
