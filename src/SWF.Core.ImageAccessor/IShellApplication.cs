using System.Runtime.InteropServices;

namespace SWF.Core.ImageAccessor
{
    [ComImport]
    [Guid("866738B9-6CF2-4DE8-8767-F794EBE74F4E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    internal interface IShellApplication
    {
        Folder NameSpace(string path);
    }

    [ComImport]
    [Guid("BBCBDE60-C3FF-11CE-8350-444553540000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    internal interface Folder
    {
        FolderItem ParseName(string name);
        string GetDetailsOf(object item, int index);
    }

    [ComImport]
    [Guid("744129E0-CBE5-11CE-8350-444553540000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    internal interface FolderItem
    {

    }
}
