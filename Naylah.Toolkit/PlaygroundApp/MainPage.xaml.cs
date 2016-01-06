using Microsoft.WindowsAzure.Storage.Blob;
using System;
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

            this.Loaded += async (s, a) =>
            {
                await Q42.WinRT.Data.WebDataCache.Init();

                imageCropper.ImageSource = new BitmapImage(new Uri("https://infinitusservices.blob.core.windows.net/tempimages/AppServiceIcon.png"));

                imageCropper.SelectionCallback = ItemSelectionado;
                //imageCropper.ImageSource = new BitmapImage(new Uri("C:\\Users\\BrenoS\\Pictures\\banner.jpg"));
            };
        }

        private async void ItemSelectionado(StorageFile obj)
        {
            CloudBlockBlob blob =
                new CloudBlockBlob(
                    new Uri("https://infinitusstorage.blob.core.windows.net/files/da763926-e690-4a09-a377-6c7a501fb597.png?sv=2015-04-05&sr=b&sig=%2BgeyFaOi3OzfhiW0phdwO7WZQGtVMcVwnIQu8nr9uSg%3D&st=2016-01-06T17%3A10%3A46Z&se=2016-01-07T17%3A15%3A46Z&sp=rw"));

            if (obj != null)
            {
                await blob.UploadFromFileAsync(obj);
            }

            
        }

        #endregion Public Constructors
    }
}