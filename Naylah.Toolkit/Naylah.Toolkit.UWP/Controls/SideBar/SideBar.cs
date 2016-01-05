using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Naylah.Toolkit.UWP.Controls.SideBar
{
    [TemplatePart(Name = "PART_SideBarContent", Type = typeof(SideBar))]
    public sealed class SideBar : Control
    {

        private Grid RootGrid { get; set; }
        private Canvas SideBarCanvas { get; set; }
        private ContentPresenter SideBarContentContentPresenter { get; set; }
        private Grid SideBarGrid { get; set; }
        private Grid ExtensorPlaceHolder { get; set; }


        bool _triggerCompleted { get; set; }


        private const double SideMenuExpandedLeft = 0;

        private double SideMenuCollapsedLeft
        {
            get
            {
                double result = 0;

                try
                {
                    result = (SideBarWidth - SideBarIndicatorWidth) * -1;
                }
                catch (Exception)
                {

                }

                return result;
            }
        }



        public object SideBarContent
        {
            get { return (object)GetValue(SideBarContentProperty); }
            set { SetValue(SideBarContentProperty, value); }
        }

        public static readonly DependencyProperty SideBarContentProperty =
            DependencyProperty.Register("SideBarContent", typeof(object), typeof(SideBar), new PropertyMetadata(default(object)));



        public double SideBarWidth
        {
            get { return (double)GetValue(SideBarWidthProperty); }
            set { SetValue(SideBarWidthProperty, value); }
        }

        public static readonly DependencyProperty SideBarWidthProperty =
            DependencyProperty.Register("SideBarWidth", typeof(double), typeof(SideBar), new PropertyMetadata(360, SideBarWidthChangeCallback));

        private static void SideBarWidthChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = (SideBar)d;
            s.InvalidateMeasure();
        }



        public Brush SideBarBackground
        {
            get { return (Brush)GetValue(SideBarBackgroundProperty); }
            set { SetValue(SideBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SideBarBackgroundProperty =
            DependencyProperty.Register("SideBarBackground", typeof(Brush), typeof(SideBar), new PropertyMetadata(default(Brush)));



        public bool IsSideBarOpen
        {
            get { return (bool)GetValue(IsSideBarOpenProperty); }
            set { SetValue(IsSideBarOpenProperty, value); }
        }

        public static readonly DependencyProperty IsSideBarOpenProperty =
            DependencyProperty.Register("IsSideBarOpen", typeof(bool), typeof(SideBar), new PropertyMetadata(false, OnIsSideBarOpenChangedCallback));

        private static void OnIsSideBarOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            SideBar control = (SideBar)d;

            if ((bool)e.NewValue == true)
            {
                if (control.CanOpenSideBar)
                {
                    control.OpenSideBar();
                }
                else
                {
                    control.IsSideBarOpen = false;
                }
            }
            else
            {
                control.CloseMoveSideBar();
            }

        }



        public bool CanOpenSideBar
        {
            get { return (bool)GetValue(CanOpenSideBarProperty); }
            set { SetValue(CanOpenSideBarProperty, value); }
        }

        public static readonly DependencyProperty CanOpenSideBarProperty =
            DependencyProperty.Register("CanOpenSideBar", typeof(bool), typeof(SideBar), new PropertyMetadata(true, OnCanOpenSideBarChagedCallback));

        private static void OnCanOpenSideBarChagedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SideBar s = (SideBar)d;

            if ((bool)e.NewValue == false)
            {
                s.CloseMoveSideBar();
            };

        }



        public double SideBarIndicatorWidth
        {
            get { return (double)GetValue(SideBarIndicatorWidthProperty); }
            set { SetValue(SideBarIndicatorWidthProperty, value); }
        }

        public static readonly DependencyProperty SideBarIndicatorWidthProperty =
            DependencyProperty.Register("SideBarIndicatorWidth", typeof(double), typeof(SideBar), new PropertyMetadata(20, OnSideBarIndicatorWidthChangeCallback));

        private static void OnSideBarIndicatorWidthChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = (SideBar)d;
            s.InvalidateMeasure();
        }


        public double SideBarExtensorAreaWidth
        {
            get { return (double)GetValue(SideBarExtensorAreaWidthProperty); }
            set { SetValue(SideBarExtensorAreaWidthProperty, value); }
        }

        public static readonly DependencyProperty SideBarExtensorAreaWidthProperty =
            DependencyProperty.Register("SideBarExtensorAreaWidth", typeof(double), typeof(SideBar), new PropertyMetadata(20, SideBarExtensorAreaWidthPropertyChangeCallback));

        private static void SideBarExtensorAreaWidthPropertyChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = (SideBar)d;
            s.InvalidateMeasure();
        }



        public Brush SideBarIndicatorBackground
        {
            get { return (Brush)GetValue(SideBarIndicatorBackgroundProperty); }
            set { SetValue(SideBarIndicatorBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SideBarIndicatorBackgroundProperty =
            DependencyProperty.Register("SideBarIndicatorBackground", typeof(Brush), typeof(SideBar), new PropertyMetadata(default(Brush)));


        public Brush SideBarExtensionBackgroundColor
        {
            get { return (Brush)GetValue(SideBarExtensionBackgroundColorProperty); }
            set { SetValue(SideBarExtensionBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty SideBarExtensionBackgroundColorProperty =
            DependencyProperty.Register("SideBarExtensionBackgroundColor", typeof(Brush), typeof(SideBar), new PropertyMetadata(default(Brush)));




        private void SideBarGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _triggerCompleted = true;

            double finalLeft = Canvas.GetLeft(SideBarGrid) + e.Delta.Translation.X;
            if (finalLeft < SideMenuCollapsedLeft || finalLeft > 0)
                return;

            if (CanOpenSideBar)
            {
                Canvas.SetLeft(SideBarGrid, finalLeft);
            }

            if (e.IsInertial && e.Velocities.Linear.X > 1)
            {
                _triggerCompleted = false;
                e.Complete();
                OpenSideBar();
            }

            if (e.IsInertial && e.Velocities.Linear.X < -1)
            {
                _triggerCompleted = false;
                e.Complete();
                CloseSideBar();
            }

            if (e.IsInertial && Math.Abs(e.Velocities.Linear.X) <= 1)
                e.Complete();
        }

        private void SideBarGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_triggerCompleted == false)
                return;

            double finalLeft = Canvas.GetLeft(SideBarGrid);

            if (finalLeft > -170)
                OpenSideBar();
            else
                CloseSideBar();
        }


        #region OpenClose methods

        private void OpenMoveSideBar()
        {
            MoveLeft(SideMenuExpandedLeft);
        }

        private void CloseMoveSideBar()
        {
            MoveLeft(SideMenuCollapsedLeft);

        }

        public void OpenSideBar()
        {
            if (CanOpenSideBar)
            {
                OpenExtensorPlaceHolder();

                OpenMoveSideBar();

            }
        }



        public void CloseSideBar()
        {
            CloseExtensorPlaceHolder();

            CloseMoveSideBar();

        }

        private void OpenExtensorPlaceHolder()
        {

            ExtensorPlaceHolder.Width = RootGrid.Width;
            ExtensorPlaceHolder.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            SideBarExtensionBackgroundColor.Opacity = 0.3;
            ExtensorPlaceHolder.Background = SideBarExtensionBackgroundColor;
        }

        private void CloseExtensorPlaceHolder()
        {
            ExtensorPlaceHolder.Width = SideBarExtensorAreaWidth;
            ExtensorPlaceHolder.Background = new SolidColorBrush(Colors.Transparent) { Opacity = 1 };
            ExtensorPlaceHolder.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
        }


        #endregion

        private void MoveLeft(double left)
        {
            double finalLeft = Canvas.GetLeft(SideBarGrid);

            Storyboard moveAnivation = ((Storyboard)SideBarCanvas.Resources["MoveAnimation"]);
            DoubleAnimation direction = ((DoubleAnimation)((Storyboard)SideBarCanvas.Resources["MoveAnimation"]).Children[0]);

            direction.From = finalLeft;

            moveAnivation.SkipToFill();

            direction.To = left;

            moveAnivation.Begin();

            moveAnivation.Completed += moveAnivation_Completed;

        }

        private void moveAnivation_Completed(object sender, object e)
        {
            IsSideBarOpen = VerifyIsSideBarOpen();
        }

        private bool VerifyIsSideBarOpen()
        {
            return (Canvas.GetLeft(SideBarGrid) == SideMenuExpandedLeft);
        }





        public SideBar()
        {
            this.DefaultStyleKey = typeof(SideBar);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootGrid = this.GetTemplateChild("RootGrid") as Grid;

            SideBarContentContentPresenter = this.GetTemplateChild("PART_SideBarContent") as ContentPresenter;

            SideBarCanvas = this.GetTemplateChild("SideBarCanvas") as Canvas;


            SideBarGrid = this.GetTemplateChild("SideBarGrid") as Grid;

            if (SideBarGrid != null)
            {
                SideBarGrid.ManipulationDelta += SideBarGrid_ManipulationDelta;
                SideBarGrid.ManipulationCompleted += SideBarGrid_ManipulationCompleted;
                SideBarGrid.Tapped += SideBarGrid_Tapped;
                Canvas.SetLeft(SideBarGrid, SideMenuCollapsedLeft);
            }


            ExtensorPlaceHolder = this.GetTemplateChild("gdExtensorPlaceHolder") as Grid;
            if (ExtensorPlaceHolder != null)
            {
                ExtensorPlaceHolder.Tapped += ExtensorPlaceHolder_Tapped;
                ExtensorPlaceHolder.ManipulationDelta += SideBarGrid_ManipulationDelta;
                ExtensorPlaceHolder.ManipulationCompleted += SideBarGrid_ManipulationCompleted;

            }
        }

        private void SideBarGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!VerifyIsSideBarOpen())
            {
                OpenSideBar();
            }

            e.Handled = true;
        }

        void ExtensorPlaceHolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CloseSideBar();
        }


    }
}
