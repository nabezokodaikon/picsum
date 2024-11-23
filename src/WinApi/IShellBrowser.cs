using System;
using System.Runtime.InteropServices;
using static WinApi.WinApiMembers;

namespace WinApi
{
    [ComImport]
    [Guid("000214E2-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellBrowser
    {
        void GetWindow(out IntPtr phwnd);
        void ContextSensitiveHelp([In] bool fEnterMode);
        void InsertMenusSB([In] IntPtr hmenuShared, [In, Out] ref OLEMENUGROUPWIDTHS lpMenuWidths);
        void SetMenuSB([In] IntPtr hmenuShared, [In] IntPtr holemenuRes, [In] IntPtr hwndActiveObject);
        void RemoveMenusSB([In] IntPtr hmenuShared);
        void SetStatusTextSB([In, MarshalAs(UnmanagedType.BStr)] string pszStatusText);
        void EnableModelessSB([In] bool fEnable);
        void TranslateAcceleratorSB([In] ref MSG pmsg, [In] ushort wID);
        void BrowseObject([In] IntPtr pidl, [In] uint wFlags);
        void GetViewStateStream([In] uint grfMode, [MarshalAs(UnmanagedType.Interface)] out object ppStrm);
        void OnViewWindowActive([In, MarshalAs(UnmanagedType.Interface)] IShellView pshv);
        void SetToolbarItems([In] IntPtr lpButtons, [In] uint nButtons, [In] uint uFlags);
    }
}
