using Q42.WinRT.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.IO.Extensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Naylah.Toolkit.UWP.Controls.ImageChooser
{
    public enum ImageChooserAspectRatio
    {
        NoRatio,
        ar1x1,
        ar4x3,
        ar16x9,
        ar21x9
    }

    public enum ImageChooserPhase
    {
        ImagePreview,
        Cropping
    }

    public sealed partial class ImageChooser : UserControl, INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for ActualPhase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualPhaseProperty =
            DependencyProperty.Register("ActualPhase", typeof(ImageChooserPhase), typeof(ImageChooser), new PropertyMetadata(ImageChooserPhase.ImagePreview, OnActualPhaseChangedCallback));

        // Using a DependencyProperty as the backing store for AspectRatio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register("AspectRatio", typeof(ImageChooserAspectRatio), typeof(ImageChooser), new PropertyMetadata(ImageChooserAspectRatio.NoRatio));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSource",
                typeof(ImageSource),
                typeof(ImageChooser),
                new PropertyMetadata(null, OnOriginalImageSourceChanged));

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(ImageChooser), new PropertyMetadata(false, IsBusyChangedCallback));

        private bool _isValidAspectRatio;

        public ImageChooser()
        {
            this.InitializeComponent();
            this.SizeChanged += ImageChooser_SizeChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageChooserPhase ActualPhase
        {
            get { return (ImageChooserPhase)GetValue(ActualPhaseProperty); }
            set { SetValue(ActualPhaseProperty, value); }
        }

        public ImageChooserAspectRatio AspectRatio
        {
            get { return (ImageChooserAspectRatio)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }

        public StorageFile BackupOriginalStorageFile { get; private set; }
        public StorageFile CroppedImageStorageFile { get; set; }
        public bool CroppingPhase { get { return ActualPhase == ImageChooserPhase.Cropping && !IsBusy; } }
        public StorageFolder ImageChooserTempFolder { get; private set; }
        public bool ImagePreviewPhase { get { return ActualPhase == ImageChooserPhase.ImagePreview && !IsBusy; } }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public StorageFile ImageStorageFile { get; set; }

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public bool IsValidAspectRatio
        {
            get { return _isValidAspectRatio; }
            set
            {
                _isValidAspectRatio = value;
            }
        }

        public Action<StorageFile> SelectionCallback { get; set; }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = propertyExpression.Body as MemberExpression;

            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }

            var property = body.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }

            return property.Name;
        }

        async public static Task SaveCroppedBitmapAsync(
            StorageFile originalImageFile,
            StorageFile newImageFile,
            Point startPoint, Size cropSize
            )
        {
            // Convert start point and size to integer.
            uint startPointX = (uint)Math.Floor(startPoint.X);
            uint startPointY = (uint)Math.Floor(startPoint.Y);
            uint height = (uint)Math.Floor(cropSize.Height);
            uint width = (uint)Math.Floor(cropSize.Width);

            using (IRandomAccessStream originalImgFileStream = await originalImageFile.OpenReadAsync())
            {
                // Create a decoder from the stream. With the decoder, we can get
                // the properties of the image.
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(originalImgFileStream);

                // Refine the start point and the size.
                if (startPointX + width > decoder.PixelWidth)
                {
                    startPointX = decoder.PixelWidth - width;
                }

                if (startPointY + height > decoder.PixelHeight)
                {
                    startPointY = decoder.PixelHeight - height;
                }

                // Get the cropped pixels.
                byte[] pixels = await GetPixelData(decoder, startPointX, startPointY, width, height,
                    decoder.PixelWidth, decoder.PixelHeight);

                using (IRandomAccessStream newImgFileStream = await newImageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Guid encoderID = Guid.Empty;

                    switch (newImageFile.FileType.ToLower())
                    {
                        case ".png":
                            encoderID = BitmapEncoder.PngEncoderId;
                            break;

                        case ".bmp":
                            encoderID = BitmapEncoder.BmpEncoderId;
                            break;

                        default:
                            encoderID = BitmapEncoder.JpegEncoderId;
                            break;
                    }

                    // Create a bitmap encoder

                    BitmapEncoder bmpEncoder = await BitmapEncoder.CreateAsync(
                        encoderID,
                        newImgFileStream);

                    // Set the pixel data to the cropped image.
                    bmpEncoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        width,
                        height,
                        decoder.DpiX,
                        decoder.DpiY,
                        pixels);

                    // Flush the data to file.
                    await bmpEncoder.FlushAsync();
                }
            }
        }

        public double GetRatio()
        {
            switch (AspectRatio)
            {
                case ImageChooserAspectRatio.ar1x1: return 1;
                case ImageChooserAspectRatio.ar4x3: return 1.3;
                case ImageChooserAspectRatio.ar16x9: return 1.7;
                case ImageChooserAspectRatio.ar21x9: return 2.3;
                default: return 0;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var propertyName = GetPropertyName(propertyExpression);
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        async static private Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY,
            uint width, uint height, uint scaledWidth, uint scaledHeight)
        {
            BitmapTransform transform = new BitmapTransform();
            BitmapBounds bounds = new BitmapBounds();
            bounds.X = startPointX;
            bounds.Y = startPointY;
            bounds.Height = height;
            bounds.Width = width;
            transform.Bounds = bounds;

            transform.ScaledWidth = scaledWidth;
            transform.ScaledHeight = scaledHeight;

            // Get the cropped pixels within the bounds of transform.
            PixelDataProvider pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);
            byte[] pixels = pix.DetachPixelData();
            return pixels;
        }

        private static void IsBusyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl = (ImageChooser)d;

            userControl.RaisePropertyChanged(() => userControl.ImagePreviewPhase);
            userControl.RaisePropertyChanged(() => userControl.CroppingPhase);
        }

        private static void OnActualPhaseChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl = (ImageChooser)d;

            userControl.RaisePropertyChanged(() => userControl.ImagePreviewPhase);
            userControl.RaisePropertyChanged(() => userControl.CroppingPhase);
        }

        private static void OnOriginalImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageChooser)d;
            ImageSource oldImageSource = (ImageSource)e.OldValue;
            ImageSource newImageSource = target.ImageSource;
            target.OnImageSourceChanged(oldImageSource, newImageSource);
        }

        private async Task BrowsePhotos()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.ViewMode = PickerViewMode.Thumbnail;

                // Filter to include a sample subset of file types.
                openPicker.FileTypeFilter.Clear();
                openPicker.FileTypeFilter.Add(".bmp");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".jpg");

                // Open the file picker.
                StorageFile file = await openPicker.PickSingleFileAsync();

                if (file != null)
                {
                    file = await CopyToTemp(file);

                    await LoadPreviewFromStorageFile(file);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void btBrowsePhotos_Click(object sender, RoutedEventArgs e)
        {
            await BrowsePhotos();
        }

        private void btResetToOriginalBackup_Click(object sender, RoutedEventArgs e)
        {
            LoadPreviewFromStorageFile(BackupOriginalStorageFile);
        }

        private void btSaveImageLocal_Click(object sender, RoutedEventArgs e)
        {
            SaveImageLocal();
        }

        private void btSelectImage_Click(object sender, RoutedEventArgs e)
        {
            DoSelection();
        }

        private void btTakeAPicture_Click(object sender, RoutedEventArgs e)
        {
            TakeAPicture();
        }

        private async Task<StorageFile> CopyToTemp(StorageFile storageFile1)
        {
            var newFile = await ImageChooserTempFolder.CreateTempFileAsync(Path.GetExtension(storageFile1.Name));

            await storageFile1.CopyAndReplaceAsync(newFile);

            return newFile;
        }

        private async Task DoOriginalImageLoad()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                ActualPhase = ImageChooserPhase.ImagePreview;


                ResetProps();

                await PrepareImageCropperTempFolter();


                var uriSource = ((BitmapImage)ImageSource).UriSource;

                var downloadedStorageFile = await LoadStorageFileFromUri(uriSource);

                var tempStorage = await CopyToTemp(downloadedStorageFile);

                await LoadPreviewFromStorageFile(tempStorage);

                BackupOriginalStorageFile = ImageStorageFile;
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DoSelection()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                if (CroppingPhase)
                {
                    IsBusy = true;

                    CroppedImageStorageFile = await ImageChooserTempFolder.CreateTempFileAsync(Path.GetExtension(ImageStorageFile.Name));

                    await SaveCroppedBitmapAsync(
                        ImageStorageFile,
                        CroppedImageStorageFile,
                        new Point(imgCropper.CropLeft, imgCropper.CropTop),
                        new Size(imgCropper.CropWidth, imgCropper.CropHeight)
                    );

                    await LoadPreviewFromStorageFile(CroppedImageStorageFile);
                }

                if (ImagePreviewPhase)
                {
                    if (SelectionCallback != null)
                    {
                        SelectionCallback(ImageStorageFile);
                    }
                }
            }
            catch (Exception)
            {
                
            }
            finally
            {
                ActualPhase = ImageChooserPhase.ImagePreview;
                IsBusy = false;
            }
        }

        private void ImageChooser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                imgCropper.DoFullLayout();
            }
            catch (Exception)
            {
            }
        }

        private async void ImagePreview_ImageOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Delay(100);

                IsValidAspectRatio = false;

                var bmp = (BitmapImage)((Image)sender).Source;

                if (AspectRatio != ImageChooserAspectRatio.NoRatio)
                {
                    double d = (double)bmp.PixelWidth / (double)bmp.PixelHeight;

                    d = Math.Round(d, 1, MidpointRounding.ToEven);

                    imgCropper.DesiredAspectRatio = GetRatio();

                    IsValidAspectRatio = (Math.Abs(imgCropper.DesiredAspectRatio - d) < 0.19);
                }
                else
                {
                    IsValidAspectRatio = true;
                }

                if (!IsValidAspectRatio)
                {
                    PrepareForCrop();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                imagePreview.ImageOpened -= ImagePreview_ImageOpened;
            }
        }

        private async Task LoadPreviewFromStorageFile(StorageFile imageStorageFile)
        {
            ImageStorageFile = imageStorageFile;

            imagePreview.Source = null;

            if (ImageStorageFile != null)
            {
                imagePreview.ImageOpened += ImagePreview_ImageOpened;
                imagePreview.Source = new BitmapImage(new Uri(ImageStorageFile.Path));
            }
        }

        private async Task<StorageFile> LoadStorageFileFromUri(Uri uriSource)
        {
            if (uriSource.IsFile)
            {
                return await StorageFile.GetFileFromPathAsync(uriSource.ToString());
            }
            else
            {
                var s = await WebDataCache.GetAsync(uriSource, true);
                await s.RenameAsync(s.Name + Path.GetExtension(uriSource.ToString()), NameCollisionOption.ReplaceExisting);
                return s;
            }
        }

        private void OnImageSourceChanged(
            ImageSource oldImageSource,
            ImageSource newImageSource
            )
        {
            Setup();
        }

        private async Task PrepareForCrop()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                ActualPhase = ImageChooserPhase.Cropping;

                CroppedImageStorageFile = null;
                imgCropper.ImageSource = null;

                imgCropper.ImageSource = new BitmapImage(new Uri(ImageStorageFile.Path));
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PrepareImageCropperTempFolter()
        {
            ImageChooserTempFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("ImageChooserTemp", CreationCollisionOption.OpenIfExists);
            await ImageChooserTempFolder.DeleteFilesAsync(true);
        }

        private void ResetProps()
        {
            ImageStorageFile = null;
            CroppedImageStorageFile = null;
            imagePreview.Source = null;
            imgCropper.ImageSource = null;
        }

        private async Task SaveImageLocal()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

                var ext = Path.GetExtension(ImageStorageFile.Name);

                savePicker.FileTypeChoices.Add("Imagem", new List<string>() { ext });

                StorageFile saveImageFile = await savePicker.PickSaveFileAsync();

                if (saveImageFile != null)
                {
                    await ImageStorageFile.CopyAndReplaceAsync(saveImageFile);
                }
                
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Setup()
        {
            DoOriginalImageLoad();
        }

        private async Task TakeAPicture()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                CameraCaptureUI cameraUI = new CameraCaptureUI();

                cameraUI.PhotoSettings.AllowCropping = false;
                cameraUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;

                StorageFile capturedMedia =
                    await cameraUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (capturedMedia != null)
                {
                    capturedMedia = await CopyToTemp(capturedMedia);

                    await LoadPreviewFromStorageFile(capturedMedia);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void tbtCrop_Click(object sender, RoutedEventArgs e)
        {
            if (CroppingPhase)
            {
                ActualPhase = ImageChooserPhase.ImagePreview;
            }
            else
            {
                PrepareForCrop();
            }
        }
    }
}