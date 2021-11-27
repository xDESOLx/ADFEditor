using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ADFEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ADFFileEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string FileName;
        public uint Offset;
        public uint Size;
    }
}
