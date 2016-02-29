using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naylah.Toolkit.UWP.Helpers
{
    public static class ApplicationHelper
    {

        public static bool DesignModeEnabled { get { return Windows.ApplicationModel.DesignMode.DesignModeEnabled; } }
    }
}
