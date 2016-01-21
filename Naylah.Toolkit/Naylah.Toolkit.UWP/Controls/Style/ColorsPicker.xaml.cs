using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Naylah.Toolkit.UWP.Controls.Style
{
    public sealed partial class ColorsPicker : UserControl
    {

        public Action<Color> callback { get; set; }

        public ColorsPickerViewModel Vm { get; set; }


        public ColorsPicker()
        {
            this.InitializeComponent();

            Vm = new ColorsPickerViewModel();

            this.DataContext = Vm;


        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var color = Vm.ColorSelected(e.ClickedItem);

            if (callback != null)
            {
                callback(color);
            }
        }
    }

    public class ColorsPickerViewModel : BindableBase
    {
        public ColorsPickerViewModel()
        {
            LoadScreenDataSources();

        }

        private List<Color> _flatUIColorsList;

        public List<Color> FlatUIColorsList
        {
            get { return _flatUIColorsList; }
            set { SetProperty(ref _flatUIColorsList, value); }
        }

        private List<Color> _systemColorsList;

        public List<Color> SystemColorsList
        {
            get { return _systemColorsList; }
            set { SetProperty(ref _systemColorsList, value); }
        }


        public async Task LoadScreenDataSources()
        {
            try
            {
                var styleColorsList = new List<Color>();
                styleColorsList.AddRange(Helpers.ColorHelper.GetNamedColors());

                SystemColorsList = styleColorsList;

                var flatUIColorsList = new List<Color>();
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#1abc9c"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#16a085"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#f1c40f"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#f39c12"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#2ecc71"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#27ae60"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#e67e22"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#d35400"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#3498db"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#2980b9"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#e74c3c"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#c0392b"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#9b59b6"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#8e44ad"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#ecf0f1"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#bdc3c7"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#34495e"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#2c3e50"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#95a5a6"));
                flatUIColorsList.Add(Helpers.ColorHelper.FromString("#7f8c8d"));

                FlatUIColorsList = flatUIColorsList;

            }
            catch (Exception)
            {

            }
        }

        internal Color ColorSelected(object clickedItem)
        {
            Color color = Colors.Transparent;

            try
            {
                color = (Color)clickedItem;
            }
            catch (Exception)
            {

            }

            return color;
        }

    }
}
