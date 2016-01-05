using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Naylah.Toolkit.UWP.Controls.CircularSlider
{
    public class CircularSliderControl : Canvas
    {
        private readonly IGeometryHelper _geometryHelper = new GeometryHelper();
        private readonly IMathHelper _mathHelper = new MathHelper();

        private Ellipse _outerGripControl;
        private Ellipse _innerGripControl;

        private Point _startPoint;

        private Point _outerCurrentPoint;
        private Point _innerCurrentPoint;

        private const int TopMargin = 40;
        private const int StartOuterAngle = 215;
        private const int StartInnerAngle = 120;

        public CircularSliderControl()
        {
            this.PointerReleased += RadialControl_PointerReleased;
            this.AddHandler(PointerReleasedEvent, new PointerEventHandler(RadialControl_PointerReleased), true);

            this.PointerMoved += RadialControl_PointerMoved;

            Loaded += OnLoaded;
        }

        private void CanvasOnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            IsOuterDrawing = false;
            IsInnerDrawing = false;

            _outerGripControl.Stroke = new SolidColorBrush(GripColor);
            _innerGripControl.Stroke = new SolidColorBrush(GripColor);
        }

        void RadialControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CanvasOnPointerReleased(sender, e);
        }

        void RadialControl_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if ((!IsOuterDrawing) && (!IsInnerDrawing))
                return;

            ClearOldStrips();

            if (IsOuterDrawing)
            {
                _outerCurrentPoint = e.GetCurrentPoint(this).Position;

                var circleStart = new Point(_startPoint.X, _startPoint.Y - OuterRingRadius);

                //Calculate the angle
                var angle = _mathHelper.AngleBetweenLine(_startPoint, circleStart, _outerCurrentPoint, OuterRingRadius);

                var rawValue = CreateSegment(Name + "OuterRadialStrip", OuterRingRadius, _outerCurrentPoint, OuterRingColor, angle);
                OuterCurrentValue = (int)(rawValue * (OuterRingMaxValue + 1));

                UpdateGripControlPosition(_outerGripControl, angle, OuterRingRadius);
            }
            else
            {
                _innerCurrentPoint = new Point(e.GetCurrentPoint(this).Position.X, e.GetCurrentPoint(this).Position.Y);

                var circleStart = new Point(_startPoint.X, _startPoint.Y - InnerRingRadius);

                //Calculate the angle
                var angle = _mathHelper.AngleBetweenLine(_startPoint, circleStart, _innerCurrentPoint, InnerRingRadius);

                var rawValue = CreateSegment(Name + "InnerRadialStrip", InnerRingRadius, _innerCurrentPoint, InnerRingColor, angle);
                InnerCurrentValue = (int)(rawValue * (InnerRingMaxValue + 1));

                UpdateGripControlPosition(_innerGripControl, angle, InnerRingRadius);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _startPoint = new Point(CentreX, CentreY);

            //The control starts at x=0, y=1 - in the middle of the ring strip
            _outerCurrentPoint = new Point(CentreX, CentreY - (OuterRingRadius - StripWidth / 2));
            _innerCurrentPoint = new Point(CentreX, CentreY - (InnerRingRadius - StripWidth / 2));

            InitialiseGripControls();

            AddBackgroundRings();

            _outerCurrentPoint = _mathHelper.ScaleUnitCirclePoint(new Point(CentreX, CentreY), StartOuterAngle, OuterRingRadius);

            var rawValue = CreateSegment(Name + "OuterRadialStrip", OuterRingRadius, _outerCurrentPoint, OuterRingColor, StartOuterAngle);
            OuterCurrentValue = (int)(rawValue * (OuterRingMaxValue + 1));

            UpdateGripControlPosition(_outerGripControl, StartOuterAngle, OuterRingRadius);

            _innerCurrentPoint = _mathHelper.ScaleUnitCirclePoint(new Point(CentreX, CentreY), StartInnerAngle, InnerRingRadius);

            rawValue = CreateSegment(Name + "InnerRadialStrip", InnerRingRadius, _innerCurrentPoint, InnerRingColor, StartInnerAngle);
            InnerCurrentValue = (int)(rawValue * (InnerRingMaxValue + 1));

            UpdateGripControlPosition(_innerGripControl, StartInnerAngle, InnerRingRadius);
        }

        private double CreateSegment(string name, double radius, Point currentPoint, Color color, double angle)
        {
            _geometryHelper.SetColours(new SolidColorBrush(color));

            var radialStrip = _geometryHelper.GetCircleSegment(
                _startPoint, //The centre of the circle
                currentPoint,
                radius,
                StripWidth,
                angle);

            radialStrip.IsHitTestVisible = false;
            radialStrip.Name = name;
            radialStrip.SetValue(ZIndexProperty, -2);

            Children.Add(radialStrip);

            //Return the angle translated into a number value
            return (angle / 360.0);
        }

        private void UpdateGripControlPosition(Ellipse gripControl, double angle, double radius)
        {
            //Set the gripControl position offset at a few degrees
            var gripPoint = _mathHelper.ScaleUnitCirclePoint(new Point(CentreX, CentreY), (angle) % 360,
                radius + (StripWidth / 2) + (GripSize / 2));

            SetLeft(gripControl, gripPoint.X - (GripSize / 2));
            SetTop(gripControl, gripPoint.Y - (GripSize / 2));
        }

        private void ClearOldStrips()
        {
            foreach (var child in Children.ToList())
            {
                if (IsOuterDrawing)
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == (Name + "OuterRadialStrip"))
                        Children.Remove(child);
                }
                else if (IsInnerDrawing)
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == (Name + "InnerRadialStrip"))
                        Children.Remove(child);
                }
            }
        }

        private void InitialiseGripControls()
        {
            _outerGripControl = new Ellipse
            {
                Width = GripSize,
                Height = GripSize,
                Fill = new SolidColorBrush(Colors.Transparent),
                Stroke = new SolidColorBrush(GripColor),
                StrokeThickness = GripThickness
            };

            _outerGripControl.PointerPressed += _outerGripControl_PointerPressed;
            //_outerGripControl.PointerPressed += OuterGripControlOnMouseLeftButtonDown;

            Children.Add(_outerGripControl);

            _innerGripControl = new Ellipse
            {
                Width = GripSize,
                Height = GripSize,
                Fill = new SolidColorBrush(Colors.Transparent),
                Stroke = new SolidColorBrush(GripColor),
                StrokeThickness = GripThickness
            };

            //_innerGripControl.MouseLeftButtonDown += InnerGripControlOnMouseLeftButtonDown;

            _innerGripControl.PointerPressed += _innerGripControl_PointerPressed;

            Children.Add(_innerGripControl);
        }

        void _innerGripControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsInnerDrawing = true;
            //IsOuterDrawing = false;

            _innerGripControl.Stroke = new SolidColorBrush(GripColorPressed);
        }

        void _outerGripControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsOuterDrawing = true;
            //IsInnerDrawing = false;

            _outerGripControl.Stroke = new SolidColorBrush(GripColorPressed);
        }

        private void AddBackgroundRings()
        {
            //Add outer background circle
            var outerRingBackground = new Ellipse
            {
                Width = (OuterRingRadius * 2) + StripWidth,
                Height = (OuterRingRadius * 2) + StripWidth,
                Stroke = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                StrokeThickness = StripWidth
            };

            //Ensure these are drawn behind everything
            outerRingBackground.SetValue(ZIndexProperty, -5);

            SetLeft(outerRingBackground, (CentreX - OuterRingRadius - (StripWidth / 2)));
            SetTop(outerRingBackground, (CentreY - OuterRingRadius - (StripWidth / 2)));

            outerRingBackground.IsHitTestVisible = false;

            Children.Add(outerRingBackground);

            //Add inner background circle
            var innerRingBackground = new Ellipse
            {
                Width = (InnerRingRadius * 2) + StripWidth,
                Height = (InnerRingRadius * 2) + StripWidth,
                Stroke = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50)),
                StrokeThickness = StripWidth
            };

            //Ensure these are drawn behind everything
            innerRingBackground.SetValue(ZIndexProperty, -5);

            SetLeft(innerRingBackground, (CentreX - InnerRingRadius - (StripWidth / 2)));
            SetTop(innerRingBackground, (CentreY - InnerRingRadius - (StripWidth / 2)));

            innerRingBackground.IsHitTestVisible = false;

            Children.Add(innerRingBackground);
        }


        private bool IsOuterDrawing { get; set; }

        private bool IsInnerDrawing { get; set; }

        private double CentreX
        {
            get { return Width / 2; }
        }

        private double CentreY
        {
            get { return OuterRingRadius + (StripWidth / 2) + TopMargin; }
        }

        #region Dependency Properties

        public static readonly DependencyProperty OuterRingRadiusProperty =
            DependencyProperty.Register("OuterRingRadius", typeof(double), typeof(CircularSliderControl), new PropertyMetadata(100.0));

        public double OuterRingRadius
        {
            get { return (double)GetValue(OuterRingRadiusProperty); }
            set { SetValue(OuterRingRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRingRadiusProperty =
            DependencyProperty.Register("InnerRingRadius", typeof(double), typeof(CircularSliderControl), new PropertyMetadata(80.0));

        public double InnerRingRadius
        {
            get { return (double)GetValue(InnerRingRadiusProperty); }
            set { SetValue(InnerRingRadiusProperty, value); }
        }

        public static readonly DependencyProperty GripSizeProperty =
            DependencyProperty.Register("GripSize", typeof(double), typeof(CircularSliderControl), new PropertyMetadata(20.0));

        public double GripSize
        {
            get { return (double)GetValue(GripSizeProperty); }
            set { SetValue(GripSizeProperty, value); }
        }

        public static readonly DependencyProperty StripWidthProperty =
            DependencyProperty.Register("StripWidth", typeof(double), typeof(CircularSliderControl), new PropertyMetadata(20.0));

        public double StripWidth
        {
            get { return (double)GetValue(StripWidthProperty); }
            set { SetValue(StripWidthProperty, value); }
        }

        public static readonly DependencyProperty GripColorProperty =
            DependencyProperty.Register("GripColor", typeof(Color), typeof(CircularSliderControl), new PropertyMetadata(Colors.Orange));

        public Color GripColor
        {
            get { return (Color)GetValue(GripColorProperty); }
            set { SetValue(GripColorProperty, value); }
        }

        public static readonly DependencyProperty GripColorPressedProperty =
            DependencyProperty.Register("GripColorPressed", typeof(Color), typeof(CircularSliderControl), new PropertyMetadata(Colors.Red));

        public Color GripColorPressed
        {
            get { return (Color)GetValue(GripColorPressedProperty); }
            set { SetValue(GripColorPressedProperty, value); }
        }

        public static readonly DependencyProperty OuterRingColorProperty =
            DependencyProperty.Register("OuterRingColor", typeof(Color), typeof(CircularSliderControl), new PropertyMetadata(Colors.Green));

        public Color OuterRingColor
        {
            get { return (Color)GetValue(OuterRingColorProperty); }
            set { SetValue(OuterRingColorProperty, value); }
        }

        public static readonly DependencyProperty InnerRingColorProperty =
            DependencyProperty.Register("InnerRingColor", typeof(Color), typeof(CircularSliderControl), new PropertyMetadata(Colors.Green));

        public Color InnerRingColor
        {
            get { return (Color)GetValue(InnerRingColorProperty); }
            set { SetValue(InnerRingColorProperty, value); }
        }

        public static readonly DependencyProperty OuterCurrentValueProperty =
            DependencyProperty.Register("OuterCurrentValue", typeof(int), typeof(CircularSliderControl), new PropertyMetadata(0));

        public int OuterCurrentValue
        {
            get { return (int)GetValue(OuterCurrentValueProperty); }
            set { SetValue(OuterCurrentValueProperty, value); }
        }

        public static readonly DependencyProperty InnerCurrentValueProperty =
            DependencyProperty.Register("InnerCurrentValue", typeof(int), typeof(CircularSliderControl), new PropertyMetadata(0));

        public int InnerCurrentValue
        {
            get { return (int)GetValue(InnerCurrentValueProperty); }
            set { SetValue(InnerCurrentValueProperty, value); }
        }

        public static readonly DependencyProperty OuterRingMaxValueProperty =
            DependencyProperty.Register("OuterRingMaxValue", typeof(int), typeof(CircularSliderControl), new PropertyMetadata(100));

        public int OuterRingMaxValue
        {
            get { return (int)GetValue(OuterRingMaxValueProperty); }
            set { SetValue(OuterRingMaxValueProperty, value); }
        }

        public static readonly DependencyProperty InnerRingMaxValueProperty =
            DependencyProperty.Register("InnerRingMaxValue", typeof(int), typeof(CircularSliderControl), new PropertyMetadata(8));

        public int InnerRingMaxValue
        {
            get { return (int)GetValue(InnerRingMaxValueProperty); }
            set { SetValue(InnerRingMaxValueProperty, value); }
        }

        public static readonly DependencyProperty GripThicknessProperty =
            DependencyProperty.Register("GripThickness", typeof(int), typeof(CircularSliderControl), new PropertyMetadata(8));

        public int GripThickness
        {
            get { return (int)GetValue(GripThicknessProperty); }
            set { SetValue(GripThicknessProperty, value); }
        }

        #endregion
    }
}
