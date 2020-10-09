using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonUI.ViewModels;
using CommonUI.Views;
using LibSanBag.Providers;

namespace CommonUI
{
    public class DependencyChecker
    {
        public static bool DownloadDependencies(string uri, string outputName)
        {
            // attempt download
            using (var wc = new WebClient())
            {
                var zippedBytes = wc.DownloadData(uri);

                using (var outStream = new MemoryStream())
                {
                    using (var ms = new MemoryStream(zippedBytes))
                    {
                        if (zippedBytes.Length < (5 + 8))
                        {
                            throw (new Exception("input .lzma is too short"));
                        }

                        var br = new BinaryReader(ms);
                        byte[] properties = br.ReadBytes(5);
                        var outSize = br.ReadInt64();

                        var decoder = new SevenZip.Compression.LZMA.Decoder();
                        decoder.SetDecoderProperties(properties);
                        decoder.Code(ms, outStream, ms.Length - ms.Position, outSize, null);
                    }

                    var outBytes = outStream.ToArray();
                    File.WriteAllBytes(outputName, outBytes);
                }
            }

            return File.Exists(outputName);
        }

        public static void CheckDependencies()
        {
            if (!LibSanBag.ResourceUtils.Unpacker.IsAvailable)
            {
                var dependencyUri = @"http://content.warframe.com/Tools/Oodle/x64/final/oo2core_7_win64.dll.B486C6F46A3D802966D04911A619B2ED.lzma";
                var outputName = "oo2core_7_win64.dll";

                var result = MessageBox.Show($"This program requires additional dependencies to run. Attempt to download the following resource?\n  {outputName}",
                    "Download required dependencies?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DownloadDependencies(dependencyUri, outputName);
                        MessageBox.Show($"Successfully downloaded {outputName} to {Environment.CurrentDirectory}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to download dependencies: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                LibSanBag.ResourceUtils.Unpacker.FindDependencies(new FileProvider());
            }

            if (!LibSanBag.ResourceUtils.Unpacker.IsAvailable)
            {
                MessageBox.Show(
                        "This program requires additional dependencies to run. Please obtain one of the following DLLs and place it in the directory containing " + System.AppDomain.CurrentDomain.FriendlyName + ":\n" +
                        "  oo2core_6_win64.dll\n" +
                        "  oo2core_7_win64.dll",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error
                );
            }
        }
    }
}
