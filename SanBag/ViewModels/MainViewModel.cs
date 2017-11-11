﻿using LibSanBag;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using SanBag.Commands;
using SanBag.Models;
using SanBag.ResourceUtils;
using SanBag.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SanBag.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
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
            get => _records.FindAll(CurrentView.Filter);
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

        public List<ViewType> Views { get; set; } = new List<ViewType>();

        private ViewType _currentView;
        public ViewType CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Records));
            }
        }

        private bool RecordPassesNameFilter(FileRecord record)
        {
            return record.Name.IndexOf(RecordNameFilter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public MainViewModel()
        {
            CommandOpenBag = new CommandOpenBag(this);

            var genericBagViewModel = new GenericBagViewModel(this);
            Views.Add(new ViewType
            {
                View = new GenericBagView
                {
                    DataContext = genericBagViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && genericBagViewModel.IsValidRecord(record)),
                Name = "Default"
            });

            if (LibDDS.IsAvailable && OodleLz.IsAvailable)
            {
                var textureResourceViewModel = new TextureResourceViewModel(this);
                Views.Add(new ViewType
                {
                    View = new TextureResourceView
                    {
                        DataContext = textureResourceViewModel
                    },
                    Filter = (record => RecordPassesNameFilter(record) && textureResourceViewModel.IsValidRecord(record)),
                    Name = "TextureResource"
                });
            }

            var scriptCompiledBytecodeResourceView = new ScriptCompiledBytecodeResourceViewModel(this);
            Views.Add(new ViewType
            {
                View = new ScriptCompiledBytecodeResourceView
                {
                    DataContext = scriptCompiledBytecodeResourceView
                },
                Filter = (record => RecordPassesNameFilter(record) && scriptCompiledBytecodeResourceView.IsValidRecord(record)),
                Name = "ScriptCompiledBytecodeResource"
            });

            var scriptSourceTextResourceViewModel = new ScriptSourceTextResourceViewModel(this);
            Views.Add(new ViewType
            {
                View = new ScriptSourceTextResourceView
                {
                    DataContext = scriptSourceTextResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && scriptSourceTextResourceViewModel.IsValidRecord(record)),
                Name = "ScriptSourceTextResource"
            });

            var luaScriptResourceViewModel = new LuaScriptResourceViewModel(this);
            Views.Add(new ViewType
            {
                View = new ScriptSourceTextResourceView
                {
                    DataContext = luaScriptResourceViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && luaScriptResourceViewModel.IsValidRecord(record)),
                Name = "LuaScriptResource"
            });

            var manifestViewModel = new ManifestViewModel(this);
            Views.Add(new ViewType
            {
                View = new ManifestResourceView
                {
                    DataContext = manifestViewModel
                },
                Filter = (record => RecordPassesNameFilter(record) && manifestViewModel.IsValidRecord(record)),
                Name = "Manifest"
            });

            CurrentView = Views[0];
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
                Console.WriteLine($"Failed to open bag: {ex.Message}");
            }
        }

        public void OpenBag(string path)
        {
            BagPath = path;

            using (var in_stream = File.OpenRead(path))
            {
                Records = Bag.Read(in_stream).ToList();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
