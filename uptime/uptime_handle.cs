using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace uptime
{
    partial class uptime
    {
        // https://stackoverflow.com/questions/1169591/check-if-output-is-redirected
        public enum STDHandle : uint
        {
            STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12),
        }
        public enum FileType : uint
        {
            FILE_TYPE_UNKNOWN = 0x0000,
            FILE_TYPE_DISK = 0x0001,
            FILE_TYPE_CHAR = 0x0002,
            FILE_TYPE_PIPE = 0x0003,
            FILE_TYPE_REMOTE = 0x8000,
        }

        [DllImport("Kernel32.dll")]
        static public extern UIntPtr GetStdHandle(STDHandle stdHandle);
        [DllImport("Kernel32.dll")]
        static public extern FileType GetFileType(UIntPtr hFile);

        static public bool IsOutputRedirected()
        {
            UIntPtr hOutput = GetStdHandle(STDHandle.STD_OUTPUT_HANDLE);
            FileType fileType = (FileType)GetFileType(hOutput);
            if (fileType == FileType.FILE_TYPE_CHAR)
                return false;
            return true;
        }
    }
}
