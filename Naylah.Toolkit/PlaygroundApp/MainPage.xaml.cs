using GalaSoft.MvvmLight;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Xaml.Interactivity;
using Naylah.Toolkit.UWP.Behaviors;
using PlaygroundApp.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PlaygroundApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainViewModel Vm => (MainViewModel)this.DataContext;

        public MainPage()
        {
            this.InitializeComponent();
           
        }

    


        

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Vm.SetJourney(e.ClickedItem as string);
        }

        private void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Vm.SetJourney("");
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AnotherPage));
        }

        private void btShowDialog_Click(object sender, RoutedEventArgs e)
        {
            Vm.ShowDialog("1");
        }
    }

}