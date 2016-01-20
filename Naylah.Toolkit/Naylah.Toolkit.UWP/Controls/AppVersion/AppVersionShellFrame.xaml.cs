using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Naylah.Toolkit.UWP.Controls.AppVersion
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppVersionShellFrame : UserControl, INotifyPropertyChanged
    {
        public AppVersionShellFrame()
        {
            this.InitializeComponent();
            this.LoadData();
        }

        private string _typeDescription;

        public string TypeDescription
        {
            get { return _typeDescription; }
            set { _typeDescription = value; OnPropertyChanged("TypeDescription"); }
        }

        private string _buildVersion;

        public string BuildVersion
        {
            get { return _buildVersion; }
            set { _buildVersion = value; OnPropertyChanged("BuildVersion"); }
        }

        #region AppName DP
        public string AppName
        {
            get { return (string)GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }

        public static readonly DependencyProperty AppNameProperty =
            DependencyProperty.Register("AppName", typeof(string), typeof(AppVersionShellFrame), new PropertyMetadata(default(string)));


        #endregion

        #region Type DP
        public AppBuildType Type
        {
            get { return (AppBuildType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(AppBuildType), typeof(AppVersionShellFrame), new PropertyMetadata(default(AppBuildType), TypeChangedCallback));

        private static void TypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = (AppVersionShellFrame)d;
            o.LoadData();
        }

        public enum AppBuildType
        {
            Dev,
            Test,
            Stag,
            Prod,
        }
        #endregion


        #region LoadData
        private void LoadData()
        {
            try
            {
                
                BuildVersion = string.Format("Build {0}.{1}.{2}.{3}",
                        Package.Current.Id.Version.Major,
                        Package.Current.Id.Version.Minor,
                        Package.Current.Id.Version.Build,
                        Package.Current.Id.Version.Revision);

                switch (Type)
                {
                    case AppBuildType.Dev:
                        TypeDescription = (string)Resources["descDev"];
                        break;
                    case AppBuildType.Test:
                        TypeDescription = (string)Resources["descTest"];
                        break;
                    case AppBuildType.Stag:
                        TypeDescription = (string)Resources["descStagging"];
                        break;
                    case AppBuildType.Prod:
                        TypeDescription = (string)Resources["descProd"];

                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
            finally
            {

            }

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
}
