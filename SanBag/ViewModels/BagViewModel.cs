﻿using LibSanBag;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using CommonUI.Commands;
using CommonUI.Models;
using CommonUI.ResourceUtils;
using CommonUI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonUI.ViewModels;
using SanBag.Commands;
using SanBag.ViewModels.BagViewModels;
using SanBag.Views.BagViews;

namespace SanBag.ViewModels
{
    public class BagViewModel : INotifyPropertyChanged
    {
        public CommandOpenBag CommandOpenBag { get; set; }

        private string _bagPath = string.Empty;
        public string BagPath
        {
            get => _bagPath;
            set
            {
                _bagPath = value;
                OnPropertyChanged();
            }
        }

        private List<FileRecord> _records = new List<FileRecord>();
        public List<FileRecord> Records
        {
            get
            {
                if (CurrentView?.Filter == null)
                {
                    return _records;
                }

                return _records.FindAll(CurrentView.Filter);
            }
            set
            {
                _records = value;
                OnPropertyChanged();
            }
        }

        private string _recordNameFilter = string.Empty;
        public string RecordNameFilter
        {
            get => _recordNameFilter;
            set
            {
                _recordNameFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Records));
            }
        }

        private List<ViewType> _views = new List<ViewType>();
        public List<ViewType> Views
        {
            get => _views;
            set
            {
                _views = value;
                OnPropertyChanged();
            }
        }

        private ViewType _currentView;
        public ViewType CurrentView
        {
            get => _currentView;
            set
            {
                if (value != null)
                {
                    _currentView = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Records));
                }
            }
        }

        private bool _isFilterEnabled;
        public bool IsFilterEnabled
        {
            get => _isFilterEnabled;
            set
            {
                _isFilterEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isCurrentCurrentViewSelectionEnabled;
        public bool IsCurrentViewSelectionEnabled
        {
            get => _isCurrentCurrentViewSelectionEnabled;
            set
            {
                _isCurrentCurrentViewSelectionEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool RecordPassesNameFilter(FileRecord record)
        {
            return record.Name.IndexOf(RecordNameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public BagViewModel(string fileToOpen)
        {
            Init();
            OpenBag(fileToOpen);
        }

        public BagViewModel()
        {
            Init();
        }

        private void Init()
        {
            var newViews = new List<ViewType>();
            CommandOpenBag = new CommandOpenBag(this);

            var genericBagViewModel = new GenericBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = genericBagViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && genericBagViewModel.IsValidRecord(record)),
                Name = "Default"
            });

            if (LibDDS.IsAvailable && Unpacker.IsAvailable)
            {
                var textureResourceBagViewModel = new TextureResourceBagViewModel(this);
                newViews.Add(new ViewType
                {
                    View = new GenericBagView
                    {
                        DataContext = textureResourceBagViewModel
                    },
                    Filter = (record => RecordPassesNameFilter(record) && textureResourceBagViewModel.IsValidRecord(record)),
                    Name = "TextureResource"
                });
            }

            var scriptCompiledBytecodeResourceView = new ScriptCompiledBytecodeResourceViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = scriptCompiledBytecodeResourceView
                },
                Filter = (record => RecordPassesNameFilter(record) && scriptCompiledBytecodeResourceView.IsValidRecord(record)),
                Name = "ScriptCompiledBytecodeResource"
            });

            var scriptSourceTextResourceViewModel = new ScriptSourceTextResourceViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = scriptSourceTextResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && scriptSourceTextResourceViewModel.IsValidRecord(record)),
                Name = "ScriptSourceTextResource"
            });

            var scriptMetadataResourceViewModel = new ScriptMetadataResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = scriptMetadataResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && scriptMetadataResourceViewModel.IsValidRecord(record)),
                Name = "ScriptMetadataResource"
            });

            var luaScriptResourceViewModel = new LuaScriptResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = luaScriptResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && luaScriptResourceViewModel.IsValidRecord(record)),
                Name = "LuaScriptResource"
            });

            var manifestViewModel = new ManifestBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = manifestViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && manifestViewModel.IsValidRecord(record)),
                Name = "Manifest"
            });

            var soundViewModel = new SoundResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = soundViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && soundViewModel.IsValidRecord(record)),
                Name = "SoundResource"
            });

            var geometryViewModel = new GeometryResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = geometryViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && geometryViewModel.IsValidRecord(record)),
                Name = "GeometryResource"
            });

            var rawImageViewModel = new RawImageBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawImageViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawImageViewModel.IsValidRecord(record)),
                Name = "Png"
            });

            var rawBlueprintViewModel = new BlueprintResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawBlueprintViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawBlueprintViewModel.IsValidRecord(record)),
                Name = "Blueprint"
            });

            var rawWorldDefinitionViewModel = new WorldDefinitionBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawWorldDefinitionViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawWorldDefinitionViewModel.IsValidRecord(record)),
                Name = "WorldDefinition"
            });

            var rawWorldChunkDefinitionViewModel = new WorldChunkDefinitionBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawWorldChunkDefinitionViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawWorldChunkDefinitionViewModel.IsValidRecord(record)),
                Name = "WorldChunkDefinition"
            });
            var rawAudioGraphResourceViewModel = new AudioGraphResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawAudioGraphResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawAudioGraphResourceViewModel.IsValidRecord(record)),
                Name = "AudioGraphResource"
            });
            var rawAudioMaterialResourceViewModel = new AudioMaterialResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawAudioMaterialResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawAudioMaterialResourceViewModel.IsValidRecord(record)),
                Name = "AudioMaterialResource"
            });
            var rawClusterDefinitionResourceViewModel = new ClusterDefinitionResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawClusterDefinitionResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawClusterDefinitionResourceViewModel.IsValidRecord(record)),
                Name = "ClusterDefinitionResource"
            });
            var rawWorldChunkSourceViewModel = new WorldChunkSourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawWorldChunkSourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawWorldChunkSourceViewModel.IsValidRecord(record)),
                Name = "WorldChunkSource"
            });
            var rawPickableModelResourceViewModel = new PickableModelResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawPickableModelResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawPickableModelResourceViewModel.IsValidRecord(record)),
                Name = "PickableModelResource"
            });
            var rawBankResourceViewModel = new BankResourceBagViewModel(this);
            newViews.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = rawBankResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && rawBankResourceViewModel.IsValidRecord(record)),
                Name = "BankResource"
            });

            Records = new List<FileRecord>();
            Views = newViews;
            CurrentView = Views[0];

            IsFilterEnabled = true;
            IsCurrentViewSelectionEnabled = true;
        }

        public void OnOpenFile()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Bag files|*.bag|All files|*.*"
                };
                if (dialog.ShowDialog() == true)
                {
                    OpenBag(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open bag: {ex.Message}");
            }
        }

        public void OpenBag(string path)
        {
            Records = new List<FileRecord>();
            BagPath = path;

            if (path.IndexOf("userpreferences", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                IsCurrentViewSelectionEnabled = false;
                IsFilterEnabled = false;

                CurrentView = new ViewType
                {
                    View = new ResourceView
                    {
                        DataContext = new ResourceViewModel(path)
                    },
                };
            }
            else
            {
                if (IsCurrentViewSelectionEnabled == false || IsFilterEnabled == false)
                {
                    CurrentView = Views[0];
                }

                IsCurrentViewSelectionEnabled = true;
                IsFilterEnabled = true;

                using (var in_stream = File.OpenRead(path))
                {
                    Records = Bag.Read(in_stream).ToList();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
