using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ADFEditor.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct ADFHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] Magic;
        public ushort FileEntryCount;
    }
}
