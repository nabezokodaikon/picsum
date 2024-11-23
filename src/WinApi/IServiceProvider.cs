using System;
using System.Runtime.InteropServices;

namespace WinApi
{
    [ComImport]
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        [PreserveSig]
        int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
    }
}
