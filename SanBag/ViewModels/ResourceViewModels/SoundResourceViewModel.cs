using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using SanBag.Annotations;
using System.IO;
using System.IO.Packaging;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;

namespace SanBag.ViewModels.ResourceViewModels
{
    class SoundResourceViewModel : BaseViewModel
    {
        private Timer _updateTimer;

        private string _soundName = "";
        public string SoundName
        {
            get => _soundName;
            set
            {
                _soundName = value;
                OnPropertyChanged();
            }
        }

        private MediaPlayer _player = new MediaPlayer();
        public MediaPlayer Player { get; set; } = new MediaPlayer();

        [NotNull]
        public string PositionText => Player.Position.ToString(@"hh\:mm\:ss");

        [NotNull]
        public string MaxPositionText
        {
            get
            {
                if (Player.NaturalDuration.HasTimeSpan == false)
                {
                    return "--:--:--";
                }
                return Player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            }
        }

        public int Position
        {
            get => Player.Position.Seconds;
            set
            {
                Player.Position = new TimeSpan(0, 0, value);
                OnPropertyChanged();
            }
        }

        public double Volume
        {
            get => Player.Volume;
            set
            {
                Player.Volume = value;
                OnPropertyChanged();
            }
        }


        public int MaxPosition => (int)Player.NaturalDuration.TimeSpan.TotalSeconds;

        public SoundResourceViewModel()
        {
            _updateTimer = new Timer(100);
            _updateTimer.Elapsed += OnTimerTick;
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            if (SoundName != "")
            {
                OnPropertyChanged(nameof(MaxPositionText));
                OnPropertyChanged(nameof(PositionText));
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(MaxPosition));
            }
        }

        public override void Reload()
        {
            var audioPath = Path.Combine(Path.GetTempPath(), "SanBagTemp.wav");

            using (var compressedStream = File.OpenRead(CurrentPath))
            {
                var soundResource = new SoundResource(compressedStream);
                var soundBytes = soundResource.SoundBytes;
                try
                {
                    File.Delete(audioPath);
                    LibFSB.SaveAs(soundBytes, audioPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save audio: " + ex.Message);
                    return;
                }

                SoundName = soundResource.Name;
            }

            Player.Open(new Uri(audioPath));
            Player.Play();

            _updateTimer.Start();
        }
    }
}
