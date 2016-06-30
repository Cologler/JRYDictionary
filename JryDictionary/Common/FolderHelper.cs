using System;
using System.Runtime.InteropServices;

namespace JryDictionary.Common
{
    public class FolderHelper
    {
        public static string GetOneDriveLocation() => GetKnownFolderPath(new Guid("A52BBA46-E9E1-435f-B3D9-28DAA648C0F6"));

        private static string GetKnownFolderPath(Guid knownFolderId)
        {
            var pszPath = IntPtr.Zero;
            try
            {
                var hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0)
                    return Marshal.PtrToStringAuto(pszPath);
                throw Marshal.GetExceptionForHR(hr);
            }
            finally
            {
                if (pszPath != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pszPath);
            }
        }

        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}