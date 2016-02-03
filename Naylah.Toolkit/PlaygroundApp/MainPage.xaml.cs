using GalaSoft.MvvmLight;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Xaml.Interactivity;
using Naylah.Toolkit.UWP.Behaviors;
using System;
using System.Globalization;
using System.Linq;
using Windows.Storage;
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
        #region Public Constructors

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new HueViewModel();
            

            //this.Loaded += async (s, a) =>
            //{
            //    //await Q42.WinRT.Data.WebDataCache.Init();

            //    //imageCropper.ImageSource = new BitmapImage(new Uri("https://infinitusservices.blob.core.windows.net/tempimages/AppServiceIcon.png"));

            //    imageCropper.SelectionCallback = ItemSelectionado;
            //    //imageCropper.ImageSource = new BitmapImage(new Uri("C:\\Users\\BrenoS\\Pictures\\banner.jpg"));
            //};
        }

        //private async void ItemSelectionado()
        //{
        //    t.Source = await imageCropper.GetSelectedImageAsBitmapImage();
        //    //var asd = await imageCropper.GetSelectedImageAsStorageFile();
        //    //CloudBlockBlob blob =
        //    //    new CloudBlockBlob(
        //    //        new Uri("https://infinitusstorage.blob.core.windows.net/files/da763926-e690-4a09-a377-6c7a501fb597.png?sv=2015-04-05&sr=b&sig=%2BgeyFaOi3OzfhiW0phdwO7WZQGtVMcVwnIQu8nr9uSg%3D&st=2016-01-06T17%3A10%3A46Z&se=2016-01-07T17%3A15%3A46Z&sp=rw"));

        //    //if (obj != null)
        //    //{
        //    //    await blob.UploadFromFileAsync(obj);
        //    //}


        //}

        #endregion Public Constructors

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
        }

        private void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
        }

       
    }

    public class HueViewModel : ViewModelBase
    {

        private int _myDoubleValue;

        public int MyDoubleValue
        {
            get { return _myDoubleValue; }
            set { Set(ref _myDoubleValue, value); RaisePropertyChanged(() => this.MyDoubleValueAsString); }
        }


        public string MyDoubleValueAsString
        {
            get { return MyDoubleValue.ToString(); }
        }



    }
}