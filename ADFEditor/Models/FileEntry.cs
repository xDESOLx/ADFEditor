using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADFEditor.Models
{
    public class FileEntry : BindableBase
    {
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }
        private uint size;
        public uint Size
        {
            get { return size; }
            set { SetProperty(ref size, value); }
        }
        private byte[] content;
        public byte[] Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }
    }
}
