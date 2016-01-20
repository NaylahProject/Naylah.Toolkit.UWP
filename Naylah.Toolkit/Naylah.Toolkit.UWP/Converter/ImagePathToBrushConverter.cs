using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Naylah.Toolkit.UWP.Converter
{
    public class ImagePathToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ImageBrush ib = null;

            try
            {
                if (value == null) return null;

                Uri uri = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);

                ib = new ImageBrush();

                ib.ImageSource = new BitmapImage(uri);

                return ib;
            }
            catch (Exception)
            {

            }

            return ib;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
