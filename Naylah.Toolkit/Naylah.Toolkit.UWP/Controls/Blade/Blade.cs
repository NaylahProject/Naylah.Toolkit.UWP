using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Naylah.Toolkit.UWP.Controls.Blade
{
    public class Blade : Grid
    {

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(Blade), new PropertyMetadata(Guid.NewGuid().ToString()));


        public BladeStack CurrentBladeStack { get; set; }

        public Blade()
        {
        }

        public bool IsBladeActive
        {
            get { return (bool)GetValue(IsBladeActiveProperty); }
            set { SetValue(IsBladeActiveProperty, value); }
        }

        public static readonly DependencyProperty IsBladeActiveProperty =
            DependencyProperty.Register("IsBladeActive", typeof(bool), typeof(Blade), new PropertyMetadata(false, IsBladeActiveChangedCallback));

        private static void IsBladeActiveChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var blade = (Blade)d;

            if (blade.CurrentBladeStack == null)
            {
                return;
            }

            blade.CurrentBladeStack.RaiseBladesChange(blade);
        }

        public double BladeWidth
        {
            get { return (double)GetValue(BladeWidthProperty); }
            set { SetValue(BladeWidthProperty, value); }
        }

        public static readonly DependencyProperty BladeWidthProperty =
            DependencyProperty.Register("BladeWidth", typeof(double), typeof(Blade), new PropertyMetadata(0));



    }

}
