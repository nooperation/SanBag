using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LibSanBag.Providers;

namespace CommonUI.ViewModels
{
    public class LoadingViewModel : INotifyPropertyChanged
    {
        private string _downloadUrl;
        public string DownloadUrl
        {
            get => _downloadUrl;
            set
            {
                _downloadUrl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressTextMinor));
            }
        }

        private long _bytesDownloaded;
        public long BytesDownloaded
        {
            get => _bytesDownloaded;
            set
            {
                _bytesDownloaded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressTextMinor));
            }
        }

        private long _totalBytes;
        public long TotalBytes
        {
            get => _totalBytes;
            set
            {
                _totalBytes = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressTextMinor));
            }
        }

        private int _currentResourceIndex;
        public int CurrentResourceIndex
        {
            get => _currentResourceIndex;
            set
            {
                _currentResourceIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressTextMajor));
            }
        }

        private int _totalResources;
        public int TotalResources
        {
            get => _totalResources;
            set
            {
                _totalResources = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressTextMajor));
            }
        }

        private ProgressStatus _status;
        public ProgressStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public Brush StatusColor
        {
            get
            {
                switch (Status)
                {
                    case ProgressStatus.Idling:
                        return Brushes.Gray;
                    case ProgressStatus.Connecting:
                        return Brushes.White;
                    case ProgressStatus.Downloading:
                    case ProgressStatus.Commpleted:
                        return Brushes.PaleGreen;
                    default:
                        return Brushes.Red;
                }
            }
        }

        public string ProgressTextMajor
        {
            get
            {
                return $"{CurrentResourceIndex} / {TotalResources} Resources";
            }
        }

        public string ProgressTextMinor => $"{BytesDownloaded} / {TotalBytes} Bytes";

        public LoadingViewModel()
        {
            BytesDownloaded = 0;
            TotalBytes = 1;
            TotalResources = 1;
            CurrentResourceIndex = 0;
            DownloadUrl = "Waiting...";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
