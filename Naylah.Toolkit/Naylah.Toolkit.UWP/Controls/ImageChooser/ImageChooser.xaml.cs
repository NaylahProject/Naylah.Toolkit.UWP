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
using WinRTXamlToolkit.Imaging;
using WinRTXamlToolkit.Net;

namespace Naylah.Toolkit.UWP.Controls.ImageChooser
{
    

    public sealed partial class ImageChooser : UserControl, INotifyPropertyChanged
    {

        #region Design Customizations



        public Brush TopCommandBarBackground
        {
            get { return (Brush)GetValue(TopCommandBarBackgroundProperty); }
            set { SetValue(TopCommandBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty TopCommandBarBackgroundProperty =
            DependencyProperty.Register("TopCommandBarBackground", typeof(Brush), typeof(ImageChooser), new PropertyMetadata(TryGetDefaultBrushByKey("SystemControlBackgroundChromeMediumBrush")));

        

        public Brush TopCommandBarForeground
        {
            get { return (Brush)GetValue(TopCommandBarForegroundProperty); }
            set { SetValue(TopCommandBarForegroundProperty, value); }
        }

        public static readonly DependencyProperty TopCommandBarForegroundProperty =
            DependencyProperty.Register("TopCommandBarForeground", typeof(Brush), typeof(ImageChooser), new PropertyMetadata(TryGetDefaultBrushByKey("SystemControlForegroundBaseHighBrush")));



        public Brush BottomCommandBarBackground
        {
            get { return (Brush)GetValue(BottomCommandBarBackgroundProperty); }
            set { SetValue(BottomCommandBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty BottomCommandBarBackgroundProperty =
            DependencyProperty.Register("BottomCommandBarBackground", typeof(Brush), typeof(ImageChooser), new PropertyMetadata(TryGetDefaultBrushByKey("SystemControlBackgroundChromeMediumBrush")));



        public Brush BottomCommandBarForeground
        {
            get { return (Brush)GetValue(BottomCommandBarForegroundProperty); }
            set { SetValue(BottomCommandBarForegroundProperty, value); }
        }

        public static readonly DependencyProperty BottomCommandBarForegroundProperty =
            DependencyProperty.Register("BottomCommandBarForeground", typeof(Brush), typeof(ImageChooser), new PropertyMetadata(TryGetDefaultBrushByKey("SystemControlForegroundBaseHighBrush")));


        private static object TryGetDefaultBrushByKey(string v)
        {
            try
            {
                return (Brush)Application.Current.Resources[v];
            }
            catch (Exception)
            {
                return default(Brush);
            }

        }

        #endregion



        #region SelectedImage DP

        public WriteableBitmap SelectedImage
        {
            get { return (WriteableBitmap)GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }

        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.Register("SelectedImage", typeof(WriteableBitmap), typeof(ImageChooser), new PropertyMetadata(default(WriteableBitmap), OnSelectedImageChanged));

        private static void OnSelectedImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageChooser)d;
            WriteableBitmap oldSelectedImage = (WriteableBitmap)e.OldValue;
            WriteableBitmap newSelectedImage = target.SelectedImage;
            target.DoSelectedImageBackup(oldSelectedImage);

        }

        private void DoSelectedImageBackup(WriteableBitmap oldSelectedImage)
        {
            if (oldSelectedImage != null)
            {
                BackupSelectedImage = oldSelectedImage;

            }
        }



        #endregion

        #region CroppedImage DP

        public BitmapImage CroppedImage
        {
            get { return (BitmapImage)GetValue(CroppedImageProperty); }
            set { SetValue(CroppedImageProperty, value); }
        }

        public static readonly DependencyProperty CroppedImageProperty =
            DependencyProperty.Register("CroppedImage", typeof(BitmapImage), typeof(ImageChooser), new PropertyMetadata(default(WriteableBitmap)));

        #endregion

        #region BackupSelectedImage DP


        //TODO: History as list :D
        public WriteableBitmap BackupSelectedImage
        {
            get { return (WriteableBitmap)GetValue(BackupSelectedImageProperty); }
            set { SetValue(BackupSelectedImageProperty, value); }
        }

        public static readonly DependencyProperty BackupSelectedImageProperty =
            DependencyProperty.Register("BackupSelectedImage", typeof(WriteableBitmap), typeof(ImageChooser), new PropertyMetadata(default(WriteableBitmap)));



        #endregion



        #region ActualPhase DP

        public ImageChooserPhase ActualPhase
        {
            get { return (ImageChooserPhase)GetValue(ActualPhaseProperty); }
            set { SetValue(ActualPhaseProperty, value); }
        }

        public static readonly DependencyProperty ActualPhaseProperty =
            DependencyProperty.Register("ActualPhase", typeof(ImageChooserPhase), typeof(ImageChooser), new PropertyMetadata(ImageChooserPhase.ImagePreview, OnActualPhaseChangedCallback));

        private static void OnActualPhaseChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl = (ImageChooser)d;

            userControl.RaisePropertyChanged(() => userControl.ImagePreviewPhase);
            userControl.RaisePropertyChanged(() => userControl.CroppingPhase);
        }

        #endregion

        #region AspectRation DP

        public ImageChooserAspectRatio AspectRatio
        {
            get { return (ImageChooserAspectRatio)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }

        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register("AspectRatio", typeof(ImageChooserAspectRatio), typeof(ImageChooser), new PropertyMetadata(ImageChooserAspectRatio.NoRatio));

        #endregion



        #region ImageSource DP

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSource",
                typeof(ImageSource),
                typeof(ImageChooser),
                new PropertyMetadata(null, OnOriginalImageSourceChanged));

        private static void OnOriginalImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ImageChooser)d;
            ImageSource oldImageSource = (ImageSource)e.OldValue;
            ImageSource newImageSource = target.ImageSource;
            target.OnImageSourceChanged(oldImageSource, newImageSource);
        }

        private void OnImageSourceChanged(
            ImageSource oldImageSource,
            ImageSource newImageSource
            )
        {
            LoadAndCacheImageSource();
        }

        #endregion

        #region IsBusy DP

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(ImageChooser), new PropertyMetadata(false, IsBusyChangedCallback));

        private static void IsBusyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl = (ImageChooser)d;

            userControl.RaisePropertyChanged(() => userControl.ImagePreviewPhase);
            userControl.RaisePropertyChanged(() => userControl.CroppingPhase);
        }

        

        #endregion


        #region Phases

        public bool ImagePreviewPhase { get { return ActualPhase == ImageChooserPhase.ImagePreview && !IsBusy; } }
        public bool CroppingPhase { get { return ActualPhase == ImageChooserPhase.Cropping && !IsBusy; } }

        #endregion


        #region Pickers (Allows end-user customization) :)

        public FileOpenPicker DefaultFileOpenPicker { get; set; }
        public CameraCaptureUI DefaultCameraCaptureUI { get; set; }
        public FileSavePicker DefaultFileSavePicker { get; set; }

        #endregion


        #region Others

        public Action SelectionCallback { get; set; }

        private StorageFolder ImageChooserTempFolder { get; set; }

        private bool _isValidAspectRatio;

        public bool IsValidAspectRatio
        {
            get { return _isValidAspectRatio; }
            set { _isValidAspectRatio = value; }
        }

        #endregion


        

        public ImageChooser()
        {
            this.InitializeComponent();
            this.SizeChanged += ImageChooser_SizeChanged;
            this.InitializePickers();
            
        }


        private void InitializePickers()
        {

            DefaultFileOpenPicker = new FileOpenPicker();
            DefaultFileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            DefaultFileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            DefaultFileOpenPicker.FileTypeFilter.Clear();
            //DefaultFileOpenPicker.FileTypeFilter.Add(".bmp"); // really?? lol
            DefaultFileOpenPicker.FileTypeFilter.Add(".png");
            DefaultFileOpenPicker.FileTypeFilter.Add(".jpeg");
            DefaultFileOpenPicker.FileTypeFilter.Add(".jpg");

            DefaultCameraCaptureUI = new CameraCaptureUI();
            DefaultCameraCaptureUI.PhotoSettings.AllowCropping = false;
            DefaultCameraCaptureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;

            DefaultFileSavePicker = new FileSavePicker();
            DefaultFileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            DefaultFileSavePicker.FileTypeChoices.Add("Jpeg", new List<string>() { ".jpg" });
            DefaultFileSavePicker.FileTypeChoices.Add("Png", new List<string>() { ".png" });
        }


        #region Aspect Ration things

        


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

        #endregion

        





        private async Task BrowsePhotos()
        {
            try
            {
                
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                // Open the file picker.
                StorageFile file = await DefaultFileOpenPicker.PickSingleFileAsync();

                await LoadSelectedImageFromExternalStorage(file);
                

            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        

        private async Task LoadSelectedImageFromExternalStorage(StorageFile file)
        {

            if (file == null)
            {
                return;
            }

            file = await CopyToTemp(file);

            var sImage = await WriteableBitmapLoadExtensions.LoadAsync(file);
            await LoadAndCheckSelectedImage(sImage);

        }

        private async Task<StorageFile> CopyToTemp(StorageFile storageFile1)
        {
            var newFile = await ImageChooserTempFolder.CreateTempFileAsync();

            await storageFile1.CopyAndReplaceAsync(newFile);

            return newFile;
        }

        private async Task LoadAndCacheImageSource()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                ResetProps();


                ActualPhase = ImageChooserPhase.ImagePreview;
                

                await PrepareImageCropperTempFolder();

                var bitmapImage = (BitmapImage)ImageSource;

                if (bitmapImage == null)
                {
                    return;
                }

                var uriSource = bitmapImage.UriSource;

                var downloadedStorageFile = await LoadStorageFileFromUri(uriSource);

                var sImage = await WriteableBitmapLoadExtensions.LoadAsync(downloadedStorageFile);

                await LoadAndCheckSelectedImage(sImage);

                DoSelectedImageBackup(SelectedImage);

            
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadAndCheckSelectedImage(WriteableBitmap sImage)
        {
            SelectedImage = sImage;

            CheckRatio();
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

                    await CropImage();

                 
                }

                if (ImagePreviewPhase)
                {
                    if (SelectionCallback != null)
                    {
                        SelectionCallback();
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

        private async Task CropImage()
        {

            var croppedImage = SelectedImage.Crop(
                imageCropper.CropLeft, imageCropper.CropTop, imageCropper.CropRight, imageCropper.CropBottom
                );

            await LoadAndCheckSelectedImage(croppedImage);

        }

        private void ImageChooser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                imageCropper.DoFullLayout();
            }
            catch (Exception)
            {
            }
        }

        private async Task CheckRatio()
        {
            try
            {
                await Task.Delay(100);

                IsValidAspectRatio = false;

                if (AspectRatio != ImageChooserAspectRatio.NoRatio)
                {
                    double d = (double)SelectedImage.PixelWidth / (double)SelectedImage.PixelHeight;

                    d = Math.Round(d, 1, MidpointRounding.ToEven);

                    imageCropper.DesiredAspectRatio = GetRatio();

                    IsValidAspectRatio = (Math.Abs(imageCropper.DesiredAspectRatio - d) < 0.19);
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
                var s = await WebFile.SaveAsync(uriSource, ImageChooserTempFolder);
                return s;
            }
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

                CroppedImage = null;

                var cropFile = await SelectedImage.Copy().SaveToFile(ImageChooserTempFolder);

                CroppedImage = new BitmapImage(new Uri(cropFile.Path));
                
            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PrepareImageCropperTempFolder()
        {
            ImageChooserTempFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("ImageChooserTemp", CreationCollisionOption.OpenIfExists);
            await ImageChooserTempFolder.DeleteFilesAsync(true);
        }

        private void ResetProps()
        {
            BackupSelectedImage = null;
            SelectedImage = null;

            CroppedImage = null;

           
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

                if (SelectedImage == null)
                {
                    return;
                }

                

                StorageFile saveImageFile = await DefaultFileSavePicker.PickSaveFileAsync();

                if (saveImageFile != null)
                {
                    var file = await SelectedImage.SaveToFile(ImageChooserTempFolder, saveImageFile.Name, CreationCollisionOption.ReplaceExisting);
                    await file.CopyAndReplaceAsync(saveImageFile);
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

      

        private async Task TakeAPicture()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;

                StorageFile capturedMedia =
                    await DefaultCameraCaptureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                await LoadSelectedImageFromExternalStorage(capturedMedia);

            }
            catch (Exception)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task<WriteableBitmap> GetSelectedImageAsWritableImage()
        {
            return SelectedImage;
        }

        public async Task<BitmapImage> GetSelectedImageAsBitmapImage()
        {
            var file = await GetSelectedImageAsStorageFile();

            return new BitmapImage(new Uri(file.Path));
        }

        public async Task<StorageFile> GetSelectedImageAsStorageFile()
        {
            return await SelectedImage.Copy().SaveToFile(ImageChooserTempFolder, Guid.NewGuid().ToString() + ".png", CreationCollisionOption.ReplaceExisting);
        }



        #region Buttons events

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

        private void btResetToOriginalBackup_Click(object sender, RoutedEventArgs e)
        {
            SelectedImage = BackupSelectedImage.Copy();
            BackupSelectedImage = SelectedImage;
            //WTF? uhahuaahu

        }

        private void btBrowsePhotos_Click(object sender, RoutedEventArgs e)
        {
            BrowsePhotos();
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


        #endregion


        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

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

        #endregion

    }


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

}