using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Tools;

namespace Naylah.Toolkit.UWP.Controls.Blade
{

    // "Shoud be refrac" - Yes i know 
    public class BladeStack : ContentControl
    {
       
        // Public for customization etc :P
        // Like for example a pull to refresh in horizontal =O
        public ScrollViewer ScrollViewer { get; private set; }

        public StackPanel StackPanel { get; private set; }

        private bool isBusy;
        private bool isLoaded;




        public BladeStackChangeType BladeStackChangeType
        {
            get { return (BladeStackChangeType)GetValue(BladeStackChangeTypeProperty); }
            set { SetValue(BladeStackChangeTypeProperty, value); }
        }

        public static readonly DependencyProperty BladeStackChangeTypeProperty =
            DependencyProperty.Register("BladeStackChangeType", typeof(BladeStackChangeType), typeof(BladeStack), new PropertyMetadata(BladeStackChangeType.BladeStackMinWidth, BladeStackChangeTypeChangedCallback));

        private static void BladeStackChangeTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public double BladeStackChangeMinWidth
        {
            get { return (double)GetValue(BladeStackChangeMinWidthProperty); }
            set { SetValue(BladeStackChangeMinWidthProperty, value); }
        }

        public static readonly DependencyProperty BladeStackChangeMinWidthProperty =
            DependencyProperty.Register("BladeStackChangeMinWidth", typeof(double), typeof(BladeStack), new PropertyMetadata((double)500, BladeStackMinWidthChangedCallback));


        private static void BladeStackMinWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public BladeMode BladeMode
        {
            get { return (BladeMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(BladeMode), typeof(BladeStack), new PropertyMetadata(BladeMode.BladeStack, BladeModeChanged));

        private static void BladeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bladeStack = (BladeStack)d;
            bladeStack.UpdateBladeStackView();
        }

        public void RaiseBladesChange(Blade blade)
        {
            UpdateBladeStackView();
        }



        public ObservableCollection<Blade> Blades
        {
            get { return (ObservableCollection<Blade>)GetValue(BladesProperty); }
            set { SetValue(BladesProperty, value); }
        }



        public static readonly DependencyProperty BladesProperty =
            DependencyProperty.Register("Blades", typeof(ObservableCollection<Blade>), typeof(BladeStack), new PropertyMetadata(default(ObservableCollection<Blade>), BladesChangedCallback));
        

        private static void BladesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bladeStack = (BladeStack)d;

            bladeStack.UpdateBladeStackView();

        }



        public BladeStack()
        {
            Configure();

            Blades = new ObservableCollection<Blade>();
            Blades.CollectionChanged += Blades_CollectionChanged;

            Loaded += BladeStack_Loaded;

            SizeChanged += BladeStack_SizeChanged;

            // This is a trick... When u manipulate elements in code-behind, Parent will not work to get DataContext
            // So this propagates the DataContext to objects inside...
            DataContextChanged += BladeStack_DataContextChanged;
        }

        private void BladeStack_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;

            UpdateBladeStackView();
        }

        private void BladeStack_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ScrollViewer != null)
            {
                ScrollViewer.DataContext = sender.DataContext;
            }

            if (StackPanel != null)
            {
                StackPanel.DataContext = sender.DataContext;
            }

            if (Blades != null)
            {

                Blades.ForEach(x =>
                    {
                        if (x.DataContext == null)
                        {
                            x.DataContext = this.DataContext;
                        }
                    }

                );

            }
        }


        private void Configure()
        {
            VerticalContentAlignment = VerticalAlignment.Stretch;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;

            ConfigureScrollViewer();
            ConfigureStackPanel();
        }

        private void BladeStack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FocusLastBlade();

            if (BladeStackChangeType == BladeStackChangeType.Manual)
            {
                return;
            }

            if (e.NewSize.Width <= BladeStackChangeMinWidth)
            {
                BladeMode = BladeMode.BladeContent;
            }
            else
            {
                BladeMode = BladeMode.BladeStack;
            }
        }

        private void Blades_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            try
            {
                var newblades = e.NewItems.Cast<Blade>();

                if (newblades != null)
                {
                    newblades.ForEach(
                        x =>
                        {
                            x.CurrentBladeStack = this;
                        }
                        );
                }

                UpdateBladeStackView();
            }
            catch (Exception)
            {

            }

        }

        private void ConfigureStackPanel()
        {
            if (StackPanel == null)
            {
                StackPanel = new StackPanel();
            }

            StackPanel.Orientation = Orientation.Horizontal;
            StackPanel.SizeChanged += StackPanel_SizeChanged;
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FocusLastBlade();
        }

        private void ConfigureScrollViewer()
        {
            if (ScrollViewer == null)
            {
                ScrollViewer = new ScrollViewer();
            }

            ScrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
            ScrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
            ScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
            ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            ScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

        }


        public void UpdateBladeStackView()
        {

            try
            {
                if (isBusy)
                {
                    return;
                }

                if (!isLoaded)
                {
                    return;
                }

                isBusy = true;

                StackPanel.Children.Clear();
                ScrollViewer.Content = null;
                Content = null;

                

                switch (BladeMode)
                {
                    case BladeMode.BladeStack:

                        var activeBlades = Blades.Where(x => x.IsBladeActive);

                        activeBlades.ForEach(
                            x =>
                            {

                                if ((x.BladeWidth == 0) || (double.IsNaN(x.BladeWidth)))
                                {
                                    x.BladeWidth = BladeStackChangeMinWidth;
                                }

                                x.Width = x.BladeWidth;

                                StackPanel.Children.Add(x);
                            }
                            );


                        ScrollViewer.Content = StackPanel;

                        Content = ScrollViewer;

                        break;

                    case BladeMode.BladeContent:

                        var lastActiveBlade = Blades.Where(x => x.IsBladeActive).LastOrDefault();

                        if (lastActiveBlade == null)
                        {
                            return;
                        }

                        lastActiveBlade.Width = double.NaN;
                        lastActiveBlade.HorizontalAlignment = HorizontalAlignment.Stretch;
                        lastActiveBlade.VerticalAlignment = VerticalAlignment.Stretch;

                        Content = lastActiveBlade;

                        break;

                }

            }
            catch (Exception)
            {
            }
            finally
            {
                isBusy = false;
            }



        }


        public void FocusLastBlade()
        {

            if ((ScrollViewer == null) || (ScrollViewer.Parent == null))
            {
                return;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                ScrollViewer.ChangeView(ScrollViewer.ScrollableWidth, null, null);
            }
            else
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { ScrollViewer.ChangeView(ScrollViewer.ScrollableWidth, null, null); });
            }



        }



    }

    public enum BladeMode
    {
        None,
        BladeStack,
        BladeContent

    }

    public enum BladeStackChangeType
    {
        Manual,
        BladeStackMinWidth
    }

}
