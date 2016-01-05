using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Naylah.Toolkit.UWP.Helpers
{
    public static class XamlHelper
    {

        // Sample of use
        // vehiclesListView is a listview
        // var _Container = ((ItemsControl)vehiclesListView).ContainerFromItem(v);
        // var _Childrens = XamlHelper.AllChildren(_Container);
        // var _gdTemplate = _Childrens.OfType<Grid>().FirstOrDefault(x => x.Name == "gdVehicleItemTemplate");

        public static List<FrameworkElement> AllChildren(DependencyObject parent)
        {
            var _List = new List<FrameworkElement>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is FrameworkElement)
                    _List.Add(_Child as FrameworkElement);
                _List.AddRange(AllChildren(_Child));
            }
            return _List;
        }

        public static void ShowFrameWorkFlyout(object _frameworkElement)
        {
            FrameworkElement frameworkElement = _frameworkElement as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(frameworkElement);

            if (flyoutBase != null)
            {
                flyoutBase.ShowAt(frameworkElement);

            }
        }
    }
}
