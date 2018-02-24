﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SanBag.Models;

namespace SanBag.ViewModels
{
    class ExperienceViewModel : INotifyPropertyChanged
    {
        public Datum Experience { get; set; }

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get => _thumbnailUrl;
            set
            {
                _thumbnailUrl = value;
                OnPropertyChanged();
            }
        }

        private string _experienceName;
        public string ExperienceName
        {
            get => _experienceName;
            set
            {
                _experienceName = value;
                OnPropertyChanged();
            }
        }

        private string _experienceAuthor;
        public string ExperienceAuthor
        {
            get => _experienceAuthor;
            set
            {
                _experienceAuthor = value;
                OnPropertyChanged();
            }
        }

        public ExperienceViewModel(Datum experienceData)
        {
            Experience = experienceData;
            ThumbnailUrl = Experience.Attributes.Images.Grid.Url;
            ExperienceAuthor = Experience.Attributes.PersonaName;
            ExperienceName = Experience.Attributes.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}