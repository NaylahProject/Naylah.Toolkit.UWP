using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Naylah.Toolkit.UWP.Behaviors
{
    public class RegexTextBoxBehavior : DependencyObject, IBehavior
    {



        public string Regex
        {
            get { return (string)GetValue(RegexProperty); }
            set { SetValue(RegexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Regex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegexProperty =
            DependencyProperty.Register("Regex", typeof(string), typeof(RegexTextBoxBehavior), new PropertyMetadata("", RegexChangedCallback));

        private static void RegexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (RegexTextBoxBehavior)d;


        }

        public string CurrentTextValue { get; set; }

        /// 
        /// Used to attach this behavior to an element.
        /// Must be a TextBox.
        /// 
        ///TextBox to assocate this behavior with.
        public void Attach(DependencyObject associatedObject)
        {
            TextBox tb = associatedObject as TextBox;
            if (tb == null)
            {
                throw new ArgumentException("NumericTextBoxBehavior can only be used with a TextBox.");
            }

            AssociatedObject = associatedObject;
            AssociatedObjectAsTextBox = tb;

            CurrentTextValue = "";

            AssociatedObjectAsTextBox.Loaded += (s, e) => { TbOnTextChanging(s, null); };


        }



        public virtual void TbOnTextChanging(object sender, TextBoxTextChangingEventArgs e)
        {

            if (AssociatedObjectAsTextBox == null)
            {
                return;
            }

            try
            {

                AssociatedObjectAsTextBox.TextChanging -= TbOnTextChanging;

                if (IsValidValue())
                {
                    CurrentTextValue = AssociatedObjectAsTextBox.Text;
                }

                AssociatedObjectAsTextBox.Text = CurrentTextValue;

                AssociatedObjectAsTextBox.SelectionStart = AssociatedObjectAsTextBox.Text.Length;

            }
            catch (Exception)
            {
            }
            finally
            {
                AssociatedObjectAsTextBox.TextChanging += TbOnTextChanging;
            }



        }

        public virtual bool IsValidValue()
        {
            try
            {
                return System.Text.RegularExpressions.Regex.IsMatch(AssociatedObjectAsTextBox.Text, Regex);
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// 
        /// Detaches the behavior from the TextBox.
        /// 
        public void Detach()
        {
            TextBox tb = AssociatedObject as TextBox;
            if (tb != null)
            {
                tb.TextChanging -= this.TbOnTextChanging;
            }
        }

        /// 
        /// The associated object (TextBox).
        /// 
        public DependencyObject AssociatedObject { get; private set; }
        public TextBox AssociatedObjectAsTextBox { get; private set; }
    }

    
}
