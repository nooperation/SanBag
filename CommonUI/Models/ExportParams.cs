using LibSanBag;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUI.Models
{
    public class ExportParameters
    {
        public FileRecord FileRecord { get; set; }
        public string FileExtension { get; set; }
        public string OutputDirectory { get; set; }
        public FileStream BagStream { get; set; }
        public Action<FileRecord, uint> OnProgressReport { get; set; }
        public Func<bool> ShouldCancel { get; set; }
    }
}
