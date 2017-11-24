using LibSanBag;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SanBag.Views.BagViews
{
    /// <summary>
    /// Interaction logic for TextureResourceView.xaml
    /// </summary>
    public partial class TextureResourceView : UserControl
    {
        public TextureResourceView()
        {
            InitializeComponent();

            this.DataContext = this;
        }
    }
}
