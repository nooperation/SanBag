using Microsoft.Win32;
using System.IO;

namespace CommonUI.ResourceUtils
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
