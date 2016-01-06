using System;
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
                //imageCropper.ImageSource = new BitmapImage(new Uri("C:\\Users\\BrenoS\\Pictures\\banner.jpg"));
            };
        }

        #endregion Public Constructors
    }
}