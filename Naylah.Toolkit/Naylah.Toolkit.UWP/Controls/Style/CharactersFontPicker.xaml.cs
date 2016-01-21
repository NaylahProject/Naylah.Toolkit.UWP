using Naylah.Toolkit.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Naylah.Toolkit.UWP.Controls.Style
{
    public sealed partial class CharactersFontPicker : UserControl
    {

        public Action<string> callback { get; set; }

        public CharactersFontPickerViewModel Vm { get; set; }

        public CharactersFontPicker()
        {
            this.InitializeComponent();

            Vm = new CharactersFontPickerViewModel();

            this.DataContext = Vm;


        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem != null)
                {
                    if (callback != null)
                    {
                        callback((string)e.ClickedItem);
                    }
                }
            }
            catch (Exception)
            {
            }

        }
    }

    public class CharactersFontPickerViewModel : BindableBase
    {
        private List<string> _mdl2charactersList;

        public List<string> SymbolsList
        {
            get { return _mdl2charactersList; }
            set { SetProperty(ref _mdl2charactersList, value); }
        }

        public CharactersFontPickerViewModel()
        {
            LoadData();
        }

        private async Task LoadData()
        {
            try
            {

                SymbolsList = null;

                SymbolsList = Mdl2.CharactersList.ToList();

            }
            catch (Exception)
            {

            }
        }
    }

    #region Bindable Base
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}
