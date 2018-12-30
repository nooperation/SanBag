using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using LibSanBag.FileResources;

namespace CommonUI.Views.ResourceViews
{
    /// <summary>
    /// Interaction logic for ScriptMetadataControl.xaml
    /// </summary>
    public partial class ScriptMetadataControl : UserControl
    {
        public ScriptMetadataResource.ScriptMetadata Script
        {
            get => (ScriptMetadataResource.ScriptMetadata)GetValue(ScriptProperty);
            set => SetValue(ScriptProperty, value);
        }

        public static readonly DependencyProperty ScriptProperty = DependencyProperty.Register(
            "Script",
            typeof(ScriptMetadataResource.ScriptMetadata),
            typeof(ScriptMetadataControl),
            new PropertyMetadata(null)
        );

        public ScriptMetadataControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
