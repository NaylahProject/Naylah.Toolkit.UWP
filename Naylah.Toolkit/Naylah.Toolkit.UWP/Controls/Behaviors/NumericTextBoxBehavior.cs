using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Naylah.Toolkit.UWP.Behaviors
{
    public class NumericTextBoxBehavior : DependencyObject, IBehavior
    {

        public enum NumericTextBoxBehaviorType
        {
            Integer,
            Double,
        }


        public double NumericValue
        {
            get { return (double)GetValue(NumericDecimalValueProperty); }
            set { SetValue(NumericDecimalValueProperty, value); }
        }

        public static readonly DependencyProperty NumericDecimalValueProperty =
            DependencyProperty.Register("NumericValue", typeof(double), typeof(NumericTextBoxBehavior), new PropertyMetadata(default(double), NumericValueChangedCallback));

        private static void NumericValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (NumericTextBoxBehavior)d;
            behavior.SetTexts();

        }

        public void SetTexts()
        {
            AssociatedObjectAsTextBox.Text = NumericValue.ToString();
            AssociatedObjectAsTextBox.SelectionStart = AssociatedObjectAsTextBox.Text.Length;
        }

        public NumericTextBoxBehaviorType Type
        {
            get { return (NumericTextBoxBehaviorType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(NumericTextBoxBehaviorType), typeof(NumericTextBoxBehavior), new PropertyMetadata(NumericTextBoxBehaviorType.Integer));












        public void Attach(DependencyObject associatedObject)
        {
            TextBox tb = associatedObject as TextBox;
            if (tb == null)
            {
                throw new ArgumentException("NumericTextBoxBehavior can only be used with a TextBox.");
            }

            AssociatedObject = associatedObject;
            AssociatedObjectAsTextBox = tb;

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

                if (string.IsNullOrEmpty(AssociatedObjectAsTextBox.Text))
                {
                    NumericValue = 0;
                }

                switch (Type)
                {

                    case NumericTextBoxBehaviorType.Integer:
                        {
                            int numericValue = 0;
                            if (int.TryParse(AssociatedObjectAsTextBox.Text, out numericValue))
                            {
                                if (IsNumberRegexValid(numericValue.ToString()))
                                {
                                    NumericValue = numericValue;
                                }
                            }
                        }
                        break;

                    case NumericTextBoxBehaviorType.Double:
                        {
                            var decimalsSeparators = new char[] { '.', ',' };

                            if (decimalsSeparators.Contains(AssociatedObjectAsTextBox.Text[AssociatedObjectAsTextBox.Text.Length - 1]) && AssociatedObjectAsTextBox.Text.Where(x => decimalsSeparators.Contains(x)).Count() == 1)
                            {
                                return;
                            }

                            double numericValue = 0;
                            if (double.TryParse(AssociatedObjectAsTextBox.Text, out numericValue))
                            {
                                if (IsNumberRegexValid(numericValue.ToString()))
                                {
                                    NumericValue = numericValue;
                                }
                            }
                        }
                        break;

                }

            }
            catch (Exception)
            {
            }
            finally
            {
                SetTexts();
                AssociatedObjectAsTextBox.TextChanging += TbOnTextChanging;
            }



        }

        public static bool IsNumberRegexValid(string numberAsString)
        {
            return Regex.IsMatch(numberAsString, @"^[1-9][\.\d]*(,\d+)?$");
        }


  
        public void Detach()
        {
            TextBox tb = AssociatedObject as TextBox;
            if (tb != null)
            {
                tb.TextChanging -= this.TbOnTextChanging;
            }
        }


        public DependencyObject AssociatedObject { get; private set; }
        public TextBox AssociatedObjectAsTextBox { get; private set; }
    }
}
