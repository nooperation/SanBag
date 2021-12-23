using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonUI.Commands;
using CommonUI.Views;
using CommonUI.Views.ResourceViews;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using LibSanBag;
using LibSanBag.FileResources;
using LibSanBag.Providers;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class ScriptMetadataResourceViewModel : BaseViewModel, ISavable, IDisassemblable
    {
        public CommandSaveAs CommandSaveAs { get; set; }
        public CommandDisassembleDll CommandDisassembleDll { get; set; }

        private UserControl _currentResourceView;
        public UserControl CurrentResourceView
        {
            get => _currentResourceView;
            set
            {
                if (value == _currentResourceView)
                {
                    return;
                }
                _currentResourceView = value;
                OnPropertyChanged();
            }
        }

        private ScriptMetadataResource _resource;
        public ScriptMetadataResource Resource
        {
            get => _resource;
            set
            {
                if (value == _resource)
                {
                    return;
                }
                _resource = value;
                OnPropertyChanged();
            }
        }

        private ScriptMetadataResource.ScriptClass _currentScript = new ScriptMetadataResource.ScriptClass();
        public ScriptMetadataResource.ScriptClass CurrentScript
        {
            get => _currentScript;
            set
            {
                _currentScript = value;
                DumpScriptMetadata();
                OnPropertyChanged();
            }
        }

        public ScriptMetadataResourceViewModel()
        {
            CommandSaveAs = new CommandSaveAs(this);
            CommandDisassembleDll = new CommandDisassembleDll(this);
        }

        private void DumpScriptMetadata()
        {
            /*
            var sb = new StringBuilder();
            sb.AppendLine($"{CurrentScript.ClassName} ({CurrentScript.DisplayName})");
            if (CurrentScript.Tooltip?.Length > 0)
            {
                sb.AppendLine($"{CurrentScript.Tooltip}");
            }
            sb.AppendLine();

            if(CurrentScript.Properties.Count > 0)
            {
                foreach (var property in CurrentScript.Properties)
                {
                    sb.AppendLine($"    {property.Name} ({property.Type})");
                    if (property.Attributes.Count > 0)
                    {
                        foreach (var attribute in property.Attributes)
                        {
                            var padding = new string(' ', 8 + attribute.Name.Length + 3);
                            var attributeValue = attribute.Value.ToString().Replace("\n", "\n" + padding);

                            sb.AppendLine($"        {attribute.Name} = {attributeValue}");
                        }
                    }
                    sb.AppendLine();
                }
            }
            */
            var json = string.Empty;
            if (CurrentScript != null)
            {
                json = JsonConvert.SerializeObject(CurrentScript, Formatting.Indented, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }

            var viewModel = new RawTextResourceViewModel
            {
                CurrentText = json
            };

            CurrentResourceView = new RawTextResourceView
            {
                DataContext = viewModel
            };
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            var tempResource = ScriptMetadataResource.Create(version);
            tempResource.InitFromStream(resourceStream);

            Resource = tempResource;
        }

        private async Task<ScriptCompiledBytecodeResource> DownloadCompiledBytecodeResource()
        {
            var previousView = CurrentResourceView;

            try
            {
                var loadingViewModel = new LoadingViewModel();
                var progress = new Progress<ProgressEventArgs>(args =>
                {
                    loadingViewModel.BytesDownloaded = args.BytesDownloaded;
                    loadingViewModel.CurrentResourceIndex = args.CurrentResourceIndex;
                    loadingViewModel.TotalResources = args.TotalResources;
                    loadingViewModel.Status = args.Status;
                    loadingViewModel.TotalBytes = args.TotalBytes;
                    loadingViewModel.DownloadUrl = args.Resource;
                });

                CurrentResourceView = new LoadingView();
                CurrentResourceView.DataContext = loadingViewModel;

                var downloadResult = await FileRecordInfo.DownloadResourceAsync(
                    Hash,
                    FileRecordInfo.ResourceType.ScriptCompiledBytecodeResource,
                    FileRecordInfo.PayloadType.Payload,
                    FileRecordInfo.VariantType.NoVariants,
                    new LibSanBag.Providers.HttpClientProvider(),
                    progress
                );

                var  resource = ScriptCompiledBytecodeResource.Create(downloadResult.Version);
                resource.InitFromRawCompressed(downloadResult.Bytes);

                return resource;
            }
            finally
            {
                CurrentResourceView = previousView;
            }
        }

        // TODO: async void out of nowhere is so incredibly wrong. Make an async ICommandAsync interface and do it correctly...
        public async void SaveAs()
        {
            if (Resource == null)
            {
                MessageBox.Show("Attempting to export a null resource", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var downloadedAssembly = await DownloadCompiledBytecodeResource();

                var dialog = new SaveFileDialog();
                dialog.FileName = Resource.Resource.DefaultScript;
                dialog.Filter = "DLL|*.dll";
                if (dialog.ShowDialog() == true)
                {
                    using (var outFile = File.OpenWrite(dialog.FileName))
                    {
                        outFile.Write(downloadedAssembly.Resource.AssemblyBytes, 0, downloadedAssembly.Resource.AssemblyBytes.Length);
                        MessageBox.Show($"Successfully saved {downloadedAssembly.Resource.AssemblyBytes.Length} bytes.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download resource: {ex.Message}");
            }
        }

        // TODO: async void out of nowhere is so incredibly wrong. Make an async ICommandAsync interface and do it correctly...
        public async void Disassemble()
        {
            if (Resource == null)
            {
                MessageBox.Show("Attempting to export a null resource", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var downloadedAssembly = await DownloadCompiledBytecodeResource();
                var assemblyStream = new MemoryStream(downloadedAssembly.Resource.AssemblyBytes);

                var settings = new DecompilerSettings() {
                    ThrowOnAssemblyResolveErrors = false
                };
                var peFile = new PEFile(
                    CurrentScript.Name + ".dll",
                    reader: new PEReader(assemblyStream)
                );

                var resolver = new MyAssemblyResolver(
                    CurrentScript.Name + ".dll",
                    settings.ThrowOnAssemblyResolveErrors,
                    peFile.Reader.DetectTargetFrameworkId()
                );

                var gameDirectory = LibSanBag.ResourceUtils.Utils.GetSansarDirectory(new RegistryProvider());
                var additionalAssembliesDirectory = Path.Combine(gameDirectory, "Client", "ScriptApi", "Assemblies");
                resolver.AdditionalPathsToSearch.Add(additionalAssembliesDirectory);

                var decompiler = new CSharpDecompiler(peFile, resolver, settings);

                string source;
                if(CurrentScript.Name != null)
                {
                    source = decompiler.DecompileTypeAsString(
                        new FullTypeName(CurrentScript.Name)
                    );
                }
                else
                {
                    source = decompiler.DecompileWholeModuleAsString();
                }

                source = Regex.Replace(
                    source,
                    "long [a-zA-Z0-9]+ = (Sansar\\.)?(Microthreading\\.)?Microthread\\.GetCurrentThreadTicks\\(\\);\\s*",
                    "");
                source = Regex.Replace(
                    source,
                    "[a-zA-Z0-9]+ = (Sansar\\.)?(Microthreading\\.)?Microthread\\.YieldIfQuantaExceeded\\([a-zA-Z0-9]+\\);\\s*",
                    "");

                var viewModel = new RawTextResourceViewModel
                {
                    CurrentText = source
                };
                    
                CurrentResourceView = new RawTextResourceView
                {
                    DataContext = viewModel
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download resource: {ex.Message}");
            }
        }
    }
}
