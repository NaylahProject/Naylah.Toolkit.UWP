using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converters
{
    public class CharCaseConverter : IValueConverter
    {
        /// <summary>
        /// If set to True, conversion is reversed: True will become Collapsed.
        /// </summary>
        public bool Upper { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            try
            {
                var val = System.Convert.ToString(value);

                if (this.Upper)
                {
                    return val.ToUpper();
                }
                else
                {
                    return val.ToLower();
                }
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
