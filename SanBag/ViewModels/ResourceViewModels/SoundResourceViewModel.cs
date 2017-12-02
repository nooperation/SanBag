using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SanBag.Annotations;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using LibSanBag.FileResources;
using LibSanBag.ResourceUtils;

namespace SanBag.ViewModels.ResourceViewModels
{
    class SoundResourceViewModel : BaseViewModel
    {
        private string _soundName;
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
        public string PositionText => Player.Position.ToString("hh:mm:ss");

        [NotNull]
        public string MaxPositionText
        {
            get
            {
                if (Player.NaturalDuration.HasTimeSpan == false)
                {
                    return "--:--:--";
                }
                return Player.NaturalDuration.TimeSpan.ToString("hh:mm:ss");
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
        }
    }
}
