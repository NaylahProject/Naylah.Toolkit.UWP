using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Naylah.Toolkit.UWP.Behaviors
{
    public class TransparentTextBoxBehavior : DependencyObject, IBehavior
    {
        private Brush oldTextBoxBackground { get; set; }
        private Brush oldTextBoxBorderBrush { get; set; }


        public DependencyObject AssociatedObject { get; set; }

        private TextBox textBox { get { return (TextBox)AssociatedObject; } }


        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;

            if (textBox != null)
            {
                oldTextBoxBackground = textBox.Background;
                oldTextBoxBorderBrush = textBox.BorderBrush;

                textBox.GotFocus += textBox_GotFocus;
                textBox.LostFocus += textBox_LostFocus;

                textBox_LostFocus(textBox, null);
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            textBox.Background = new SolidColorBrush(Colors.Transparent);
            textBox.BorderBrush = new SolidColorBrush(Colors.Transparent);

        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.Background = oldTextBoxBackground;
            textBox.BorderBrush = oldTextBoxBorderBrush;
        }

        public void Detach()
        {
            textBox.GotFocus -= textBox_GotFocus;
            textBox.LostFocus -= textBox_LostFocus;
        }
        
    }
}
