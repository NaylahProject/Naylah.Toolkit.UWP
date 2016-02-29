using Naylah.Toolkit.UWP.Extensions.Collections;
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
    public class BladeStack : ContentControl
    {

        private ScrollViewer scrollViewer;
        private StackPanel stackPanel;


        public BladeMode Mode
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

            SizeChanged += BladeStack_SizeChanged;

            // This is a trick... When u manipulate elements in code-behind, Parent will not work to get DataContext
            // So this propagates the DataContext to objects inside...
            DataContextChanged += BladeStack_DataContextChanged;
        }

        private void BladeStack_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (scrollViewer != null)
            {
                scrollViewer.DataContext = sender.DataContext;
            }

            if (stackPanel != null)
            {
                stackPanel.DataContext = sender.DataContext;
            }

            if (Blades != null)
            {

                Blades.ForEach(
                            x =>
                            {
                                x.DataContext = sender.DataContext;
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

            if (e.NewSize.Width <= 500)
            {
                Mode = BladeMode.BladeContent;
            }
            else
            {
                Mode = BladeMode.BladeStack;
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
            if (stackPanel == null)
            {
                stackPanel = new StackPanel();
            }

            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.SizeChanged += StackPanel_SizeChanged;
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FocusLastBlade();
        }

        private void ConfigureScrollViewer()
        {
            if (scrollViewer == null)
            {
                scrollViewer = new ScrollViewer();
            }

            scrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;
            scrollViewer.VerticalAlignment = VerticalAlignment.Stretch;
            scrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

        }


        public void UpdateBladeStackView()
        {

            stackPanel.Children.Clear();
            scrollViewer.Content = null;
            Content = null;

            switch (Mode)
            {
                case BladeMode.BladeStack:

                    var activeBlades = Blades.Where(x => x.IsBladeActive);
                    activeBlades.ForEach(x => x.Width = x.BladeWidth);

                    activeBlades.ForEach(x => stackPanel.Children.Add(x));

                    scrollViewer.Content = stackPanel;

                    Content = scrollViewer;

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


        private void FocusLastBlade()
        {

            if ((scrollViewer == null) || (scrollViewer.Parent == null))
            {
                return;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                scrollViewer.ChangeView(scrollViewer.ScrollableWidth, null, null);
            }
            else
            {
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { scrollViewer.ChangeView(scrollViewer.ScrollableWidth, null, null); });
            }



        }



    }

    public enum BladeMode
    {
        None,
        BladeStack,
        BladeContent

    }


}
