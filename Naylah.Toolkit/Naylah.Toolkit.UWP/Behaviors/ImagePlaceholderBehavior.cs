using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Naylah.Toolkit.UWP.Behaviors
{
    public sealed class ImagePlaceholderBehavior
          : DependencyObject, IBehavior
    {

        private Image imgObj;
        private Ellipse elipseObj;



        public static readonly DependencyProperty PlaceholderUriProperty = DependencyProperty.Register(
             "PlaceholderUri",
             typeof(String),
             typeof(ImagePlaceholderBehavior),
             new PropertyMetadata(""));

        public String PlaceholderUri
        {
            get
            {
                return (String)base.GetValue(PlaceholderUriProperty);
            }

            set
            {
                base.SetValue(PlaceholderUriProperty, value);
            }
        }

        public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register(
             "ImageUri",
             typeof(String),
             typeof(ImagePlaceholderBehavior),
             new PropertyMetadata("", ImageUriChanged));


        private static void ImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ImagePlaceholderBehavior)d).ImageUri = (string)e.NewValue;
            ((ImagePlaceholderBehavior)d).SetImageImgSource();
        }

        public String ImageUri
        {
            get
            {
                return (String)base.GetValue(ImageUriProperty);
            }

            set
            {
                base.SetValue(ImageUriProperty, value);

            }
        }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            imgObj = associatedObject as Image;
            elipseObj = associatedObject as Ellipse;

            if ((imgObj == null) && (elipseObj == null))
            {
                throw new ArgumentException("ImagePlaceholderBehavior can only be used with a Image or Ellipse.");
            }

            SetPlaceholderImgSource();



        }


        private void SetPlaceholderImgSource()
        {
            if (!String.IsNullOrEmpty(PlaceholderUri))
            {
                Uri uri = new Uri(PlaceholderUri);

                if (imgObj != null)
                {
                    imgObj.Source = new BitmapImage(uri);

                }

                if (elipseObj != null)
                {
                    var ib = new ImageBrush();
                    ib.ImageSource = new BitmapImage(uri);
                    elipseObj.Fill = ib;
                }
            }
        }

        private void SetImageImgSource()
        {
            if (!String.IsNullOrEmpty(ImageUri))
            {
                Uri uri = new Uri(ImageUri);

                if (imgObj != null)
                {
                    imgObj.Source = new BitmapImage(uri);
                    imgObj.ImageFailed += Img_ImageFailed;
                }

                if (elipseObj != null)
                {
                    var ib = new ImageBrush();
                    ib.ImageSource = new BitmapImage(uri);
                    ib.ImageFailed += Img_ImageFailed;
                    elipseObj.Fill = ib;
                }
            }
        }

        private void Img_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {


            if (imgObj != null)
            {
                imgObj.ImageFailed -= Img_ImageFailed;
            }

            if (elipseObj != null)
            {
                ((ImageBrush)elipseObj.Fill).ImageFailed -= Img_ImageFailed;
            }

            SetPlaceholderImgSource();

        }

        public void Detach()
        {

        }


        public DependencyObject AssociatedObject { get; private set; }
    }
}
