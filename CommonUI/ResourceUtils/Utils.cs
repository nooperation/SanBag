using Microsoft.Win32;
using System.IO;

namespace CommonUI.ResourceUtils
{
    static class Utils
    {
        public static string GetSansarDirectory()
        {
            const string installKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Sansar";

            var installLocation = Registry.GetValue(installKeyName, "InstallLocation", null) as string;
            if (installLocation == null)
            {
                const string defaultIconKeyName = @"HKEY_CLASSES_ROOT\sansar\DefaultIcon";

                var iconPath = Registry.GetValue(defaultIconKeyName, "", null) as string;
                installLocation = Path.GetFullPath(iconPath + @"\..\..");
            }

            return installLocation;
        }
    }
}
