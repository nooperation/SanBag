﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using CommonUI.Annotations;
using System.IO;
using System.IO.Packaging;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;
using Microsoft.Win32;
using CommonUI.Commands;

namespace CommonUI.ViewModels.ResourceViewModels
{
    public class SoundResourceViewModel : BaseViewModel, ISavable
    {
        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (value == _isPlaying) return;
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public CommandPlaySound CommandPlaySound { get; }
        [NotNull]
        public CommandPauseSound CommandPauseSound { get; }
        [NotNull]
        public CommandSaveAs CommandSaveAs { get; }

        [NotNull]
        private readonly Timer _updateTimer;

        [NotNull]
        private string _soundName = "";

        [NotNull]
        public string SoundName
        {
            get => _soundName;
            set
            {
                _soundName = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
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

        public double MaxPosition
        {
            get
            {
                if (Player.NaturalDuration.HasTimeSpan == false)
                {
                    return 0;
                }
                return Player.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        public double Position
        {
            get => Player.Position.TotalSeconds;
            set
            {
                Player.Position = TimeSpan.FromSeconds(value);
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

        [CanBeNull]
        private string CompressedSoundPath { get; set; }

        public SoundResourceViewModel()
        {
            CommandPlaySound = new CommandPlaySound(this);
            CommandPauseSound = new CommandPauseSound(this);
            CommandSaveAs = new CommandSaveAs(this);

            _updateTimer = new Timer(100);
            _updateTimer.Elapsed += OnTimerTick;
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            lock (_updateTimer)
            {
                if (_updateTimer.Enabled)
                {
                    OnPropertyChanged(nameof(MaxPositionText));
                    OnPropertyChanged(nameof(PositionText));
                    OnPropertyChanged(nameof(Position));
                    OnPropertyChanged(nameof(MaxPosition));
                }
            }
        }

        protected override void LoadFromStream(Stream resourceStream, string version)
        {
            Version = version;

            CompressedSoundPath = Path.GetTempPath() + "CommonUITemp.bin";
            if (File.Exists(CompressedSoundPath))
            {
                File.Delete(CompressedSoundPath);
            }

            using (var outputStream = File.OpenWrite(CompressedSoundPath))
            {
                resourceStream.CopyTo(outputStream);
            }

            var audioPath = Path.Combine(Path.GetTempPath(), "CommonUITemp.wav");
            using (var compressedStream = File.OpenRead(CompressedSoundPath))
            {
                var soundResource = SoundResource.Create(version);
                soundResource.InitFromStream(compressedStream);

                var soundBytes = soundResource.Resource.Data.Data;
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

                SoundName = soundResource.Resource.Data.Name;
            }

            Player.Open(new Uri(audioPath));
            PlaySound();
        }

        public void PlaySound()
        {
            Player.Play();
            IsPlaying = true;
            _updateTimer.Start();
        }

        public void PauseSound()
        {
            _updateTimer.Stop();
            IsPlaying = false;
            Player.Pause();
        }

        public void SaveAs()
        {
            if (CompressedSoundPath == null || Version == null)
            {
                MessageBox.Show("Attempting to export a null sound resource", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.FileName = SoundName + ".wav";
            if (dialog.ShowDialog() == true)
            {
                using (var compressedStream = File.OpenRead(CompressedSoundPath))
                {
                    try
                    {
                        var soundResource = SoundResource.Create(Version);
                        soundResource.InitFromStream(compressedStream);

                        var soundBytes = soundResource.Resource.Data.Data;
                        LibFSB.SaveAs(soundBytes, dialog.FileName);

                        MessageBox.Show($"Successfully wrote {soundBytes.Length} byte(s) to {dialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save audio: " + ex.Message);
                        return;
                    }
                }
            }
        }

        public override void Unload()
        {
            base.Unload();

            lock (_updateTimer)
            {
                _updateTimer.Stop();
                Player.Stop();
                Player.Close();
            }
        }
    }
}
