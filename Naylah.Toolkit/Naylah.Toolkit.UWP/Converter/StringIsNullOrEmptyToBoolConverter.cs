using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Naylah.Toolkit.UWP.Converter
{
    public class StringIsNullOrEmptyToBoolConverter : IValueConverter
    {

        public bool IsReversed { get; set; }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (string)value;

            var flag = string.IsNullOrEmpty(v);

            return flag & !IsReversed;

            //if (IsReversed)
            //{
            //    return (flag ? Visibility.Collapsed : Visibility.Visible);
            //}
            //else
            //{
            //    return (flag ? Visibility.Visible : Visibility.Collapsed);
            //}
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

    }
}
