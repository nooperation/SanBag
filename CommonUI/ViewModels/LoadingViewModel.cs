using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
                OnPropertyChanged(nameof(ProgressText));
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
                OnPropertyChanged(nameof(ProgressText));
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
                OnPropertyChanged(nameof(ProgressText));
            }
        }

        public string ProgressText => $"{BytesDownloaded} / {TotalBytes}";

        public LoadingViewModel()
        {
            BytesDownloaded = 0;
            TotalBytes = 1;
            DownloadUrl = "Waiting...";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
