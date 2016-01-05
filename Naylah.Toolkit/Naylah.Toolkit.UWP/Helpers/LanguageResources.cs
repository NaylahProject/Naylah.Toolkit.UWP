using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naylah.Toolkit.UWP.Helpers
{
    class LanguageResources
    {
        public static string GetStringResource(string value)
        {

            string result = "";
            //var loader2 = new Windows.ApplicationModel.Resources.ResourceLoader();
            var loader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            result = loader.GetString(value);


            return result;
        }
    }
}
