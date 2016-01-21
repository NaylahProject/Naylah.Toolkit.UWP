using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;

namespace Naylah.Toolkit.UWP.Helpers
{
    public class ColorHelper
    {
        public static async Task<Color> GetDominantColorFromFile(StorageFile file)
        {
            //get the file

            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                //Create a decoder for the image
                var decoder = await BitmapDecoder.CreateAsync(stream);

                //Create a transform to get a 1x1 image
                var myTransform = new BitmapTransform { ScaledHeight = 1, ScaledWidth = 1 };

                //Get the pixel provider
                var pixels = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Ignore,
                    myTransform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                //Get the bytes of the 1x1 scaled image
                var bytes = pixels.DetachPixelData();

                //read the color 
                var myDominantColor = Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);

                return myDominantColor;
            }
        }



        public static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = (double)color.R;
            double green = (double)color.G;
            double blue = (double)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }


        public static Color FromString(string c)
        {
            if (string.IsNullOrEmpty(c))
                throw new ArgumentException("Invalid color string.", "c");

            if (c[0] == '#')
            {
                switch (c.Length)
                {
                    case 9:
                        {
                            //var cuint = uint.Parse(c.Substring(1), NumberStyles.HexNumber);
                            var cuint = Convert.ToUInt32(c.Substring(1), 16);
                            var a = (byte)(cuint >> 24);
                            var r = (byte)((cuint >> 16) & 0xff);
                            var g = (byte)((cuint >> 8) & 0xff);
                            var b = (byte)(cuint & 0xff);

                            return Color.FromArgb(a, r, g, b);
                        }
                    case 7:
                        {
                            var cuint = Convert.ToUInt32(c.Substring(1), 16);
                            var r = (byte)((cuint >> 16) & 0xff);
                            var g = (byte)((cuint >> 8) & 0xff);
                            var b = (byte)(cuint & 0xff);

                            return Color.FromArgb(255, r, g, b);
                        }
                    case 5:
                        {
                            var cuint = Convert.ToUInt16(c.Substring(1), 16);
                            var a = (byte)(cuint >> 12);
                            var r = (byte)((cuint >> 8) & 0xf);
                            var g = (byte)((cuint >> 4) & 0xf);
                            var b = (byte)(cuint & 0xf);
                            a = (byte)(a << 4 | a);
                            r = (byte)(r << 4 | r);
                            g = (byte)(g << 4 | g);
                            b = (byte)(b << 4 | b);

                            return Color.FromArgb(a, r, g, b);
                        }
                    case 4:
                        {
                            var cuint = Convert.ToUInt16(c.Substring(1), 16);
                            var r = (byte)((cuint >> 8) & 0xf);
                            var g = (byte)((cuint >> 4) & 0xf);
                            var b = (byte)(cuint & 0xf);
                            r = (byte)(r << 4 | r);
                            g = (byte)(g << 4 | g);
                            b = (byte)(b << 4 | b);

                            return Color.FromArgb(255, r, g, b);
                        }
                    default:
                        throw new FormatException(string.Format("The {0} string passed in the c argument is not a recognized Color format.", c));
                }
            }
            else if (
                c.Length > 3 &&
                c[0] == 's' &&
                c[1] == 'c' &&
                c[2] == '#')
            {
                var values = c.Split(',');

                if (values.Length == 4)
                {
                    var scA = double.Parse(values[0].Substring(3));
                    var scR = double.Parse(values[1]);
                    var scG = double.Parse(values[2]);
                    var scB = double.Parse(values[3]);

                    return Color.FromArgb(
                        (byte)(scA * 255),
                        (byte)(scR * 255),
                        (byte)(scG * 255),
                        (byte)(scB * 255));
                }
                else if (values.Length == 3)
                {
                    var scR = double.Parse(values[0].Substring(3));
                    var scG = double.Parse(values[1]);
                    var scB = double.Parse(values[2]);

                    return Color.FromArgb(
                        255,
                        (byte)(scR * 255),
                        (byte)(scG * 255),
                        (byte)(scB * 255));
                }
                else
                {
                    throw new FormatException(string.Format("The {0} string passed in the c argument is not a recognized Color format (sc#[scA,]scR,scG,scB).", c));
                }
            }
            else
            {
                var prop = typeof(Colors).GetTypeInfo().GetDeclaredProperty(c);
                return (Color)prop.GetValue(null);
            }
        }









        public static List<Color> GetNamedColors()
        {
            var colorsProperties = typeof(Colors).GetTypeInfo().DeclaredProperties;

            // Get defined colors
            return colorsProperties.Select(pi => (Color)pi.GetValue(null)).ToList();
        }





    }


}
