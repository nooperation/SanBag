using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanBag.ResourceUtils
{
    static class Utils
    {
        public static string GetSansarDirectory()
        {
            var installLocation = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Sansar", "InstallLocation", null) as string;
            if (installLocation == null)
            {
                var iconPath = Registry.GetValue(@"HKEY_CLASSES_ROOT\sansar\DefaultIcon", "", null) as string;
                installLocation = Path.GetFullPath(iconPath + @"\..\..");
            }

            return installLocation;
        }
    }
}
