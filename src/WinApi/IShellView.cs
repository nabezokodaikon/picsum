using System;
using System.Runtime.InteropServices;

namespace WinApi
{
    [ComImport]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellView
    {
        void GetWindow(out IntPtr phwnd);
        void ContextSensitiveHelp([In] bool fEnterMode);
        void TranslateAccelerator(IntPtr pmsg);
        void EnableModeless([In] bool fEnable);
        void UIActivate([In] uint uState);
        void Refresh();
        void CreateViewWindow([In] IShellView psvPrevious, [In] ref WinApiMembers.FOLDERSETTINGS pfs, [In] IShellBrowser psb, [In] ref WinApiMembers.RECT prcView, out IntPtr phWnd);
        void DestroyViewWindow();
        void GetCurrentInfo(out WinApiMembers.FOLDERSETTINGS pfs);
        void AddPropertySheetPages([In] uint dwReserved, [In] IntPtr lpfn, [In] IntPtr lparam);
        void SaveViewState();
        void SelectItem([In] IntPtr pidlItem, [In] uint uFlags);
        void GetItemObject([In] uint uItem, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
    }
}
