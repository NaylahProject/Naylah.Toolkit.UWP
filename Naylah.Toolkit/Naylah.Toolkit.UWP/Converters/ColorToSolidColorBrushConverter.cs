using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Naylah.Toolkit.UWP.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {

        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            Color color = Colors.Transparent;
            SolidColorBrush brush = new SolidColorBrush(color);

            try
            {

                if (!IsReversed)
                {
                    color = (Color)value;

                    brush = new SolidColorBrush(color);

                }
                else
                {
                    brush = (SolidColorBrush)value;

                    color = brush.Color;
                }



            }
            catch (Exception)
            {


            }

            if (!IsReversed)
            {
                return brush;
            }
            else
            {
                return color;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
