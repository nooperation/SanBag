using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LibSanBag;
using SanBag.Annotations;

namespace SanBag.ViewModels.ResourceViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        private string _currentPath;
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                Reload();
                OnPropertyChanged();
            }
        }

        public virtual void Reload()
        {

        }


        public virtual void ReloadFromStream(Stream resourceStream)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
