using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.System;
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


        public string NumericFormat
        {
            get { return (string)GetValue(NumericFormatProperty); }
            set { SetValue(NumericFormatProperty, value); }
        }

        public static readonly DependencyProperty NumericFormatProperty =
            DependencyProperty.Register("NumericFormat", typeof(string), typeof(NumericTextBoxBehavior), new PropertyMetadata("N", NumericValueChangedCallback));



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
            behavior.SetTexts(behavior.GetNumericNumberFormated());

        }

        public void SetTexts(string text)
        {
            try
            {


                AssociatedObjectAsTextBox.Text = text;
                AssociatedObjectAsTextBox.SelectionStart = AssociatedObjectAsTextBox.Text.Length;

                //Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                //{

                //    AssociatedObjectAsTextBox.Text = text;

                //    AssociatedObjectAsTextBox.SelectionStart = AssociatedObjectAsTextBox.Text.Length;

                //});
            }
            catch (Exception)
            {
            }
            finally
            {
            }

        }

        private string GetNumericNumberFormated()
        {
            if (Type == NumericTextBoxBehaviorType.Double)
            {
                return NumericValue.ToString(NumericFormat);
            }
            else
            {
                return ((int)NumericValue).ToString(NumericFormat);
            }

        }

        public NumericTextBoxBehaviorType Type
        {
            get { return (NumericTextBoxBehaviorType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(NumericTextBoxBehaviorType), typeof(NumericTextBoxBehavior), new PropertyMetadata(NumericTextBoxBehaviorType.Integer, TypeChangedCallback));

        private static void TypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as NumericTextBoxBehavior;
            behavior.SetTexts(behavior.GetNumericNumberFormated());
        }

        private void AssociatedObjectAsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AssociatedObjectAsTextBox == null)
            {
                return;
            }

            try
            {
                AssociatedObjectAsTextBox.TextChanged -= AssociatedObjectAsTextBox_TextChanged;

                if (string.IsNullOrEmpty(AssociatedObjectAsTextBox.Text))
                {
                    NumericValue = 0;
                }

                if (GetNumericNumberFormated() != AssociatedObjectAsTextBox.Text)
                {
                    SetTexts(GetNumericNumberFormated());
                }
            }
            catch (Exception)
            {
                SetTexts(GetNumericNumberFormated());
            }
            finally
            {
                AssociatedObjectAsTextBox.TextChanged += AssociatedObjectAsTextBox_TextChanged;
            }

        }

        private void AssociatedObjectAsTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (AssociatedObjectAsTextBox == null)
            {
                return;
            }

            var num = GetDigitBykey(e.Key);

            if (num != null)
            {
                AddNumberStack(num.Value);
            }
            else
            {
                if (e.Key == VirtualKey.Back)
                {
                    var asii = AssociatedObjectAsTextBox.SelectionLength;
                    RemoveNumberStack();
                }
            }

            e.Handled = !(e.Key == VirtualKey.Tab);

        }

        private void AddNumberStack(int num)
        {
            var nv = GetCleanNumberStack();

            if (Double.Parse(nv) == 0)
            {
                if (nv.Length > 0)
                {
                    nv = nv.Remove(nv.Length - 1);
                }
            }

            nv += num.ToString();

            SetStackNumberToNumeric(nv);

        }

        private void SetStackNumberToNumeric(string nv)
        {


            if (Type == NumericTextBoxBehaviorType.Double)
            {
                nv = nv.Insert(nv.Length - CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits, CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
                NumericValue = Double.Parse(nv);
            }
            else
            {
                NumericValue = Int64.Parse(nv);
            }

            SetTexts(GetNumericNumberFormated());

        }

        public string GetCleanNumberStack()
        {

            string r = string.Empty;

            var nv = GetNumericNumberFormated();

            if (Type == NumericTextBoxBehaviorType.Integer)
            {
                nv = nv.Split(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0])[0];
            }

            for (int i = 0; i < nv.Length; i++)
            {
                if (Char.IsDigit(nv[i]))
                    r += nv[i];
            }

            return r;
        }

        private void RemoveNumberStack()
        {
            try
            {
                var nv = GetCleanNumberStack();

                if (nv.Length > 0)
                {
                    nv = nv.Remove(nv.Length - 1);
                }

                if (string.IsNullOrEmpty(nv)) { nv = "0"; }

                SetStackNumberToNumeric(nv);

            }
            catch (Exception)
            {
            }


        }

        private int? GetDigitBykey(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.NumberPad0:
                case VirtualKey.Number0:
                    return 0;

                case VirtualKey.NumberPad1:
                case VirtualKey.Number1:
                    return 1;

                case VirtualKey.NumberPad2:
                case VirtualKey.Number2:
                    return 2;

                case VirtualKey.NumberPad3:
                case VirtualKey.Number3:
                    return 3;

                case VirtualKey.NumberPad4:
                case VirtualKey.Number4:
                    return 4;

                case VirtualKey.NumberPad5:
                case VirtualKey.Number5:
                    return 5;

                case VirtualKey.NumberPad6:
                case VirtualKey.Number6:
                    return 6;

                case VirtualKey.NumberPad7:
                case VirtualKey.Number7:
                    return 7;

                case VirtualKey.NumberPad8:
                case VirtualKey.Number8:
                    return 8;

                case VirtualKey.NumberPad9:
                case VirtualKey.Number9:
                    return 9;



            }

            return null;
        }


        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObjectAsTextBox = associatedObject as TextBox;

            if (AssociatedObjectAsTextBox == null)
            {
                throw new ArgumentException("NumericTextBoxBehavior can only be used with a TextBox.");
            }

            AssociatedObjectAsTextBox.KeyDown += AssociatedObjectAsTextBox_KeyDown;
            AssociatedObjectAsTextBox.TextChanged += AssociatedObjectAsTextBox_TextChanged;

            if (AssociatedObjectAsTextBox.InputScope == null)
            {
                var inputScope = new InputScope();
                inputScope.Names.Add(new InputScopeName(InputScopeNameValue.Number));
                AssociatedObjectAsTextBox.InputScope = inputScope;
            }

            AssociatedObjectAsTextBox.Loaded += (s, e) => { AssociatedObjectAsTextBox_TextChanged(s, null); };


        }

        public void Detach()
        {
            if (AssociatedObjectAsTextBox != null)
            {
                AssociatedObjectAsTextBox.TextChanged -= AssociatedObjectAsTextBox_TextChanged;
            }
        }


        public DependencyObject AssociatedObject { get; private set; }
        public TextBox AssociatedObjectAsTextBox { get; private set; }
    }
}
