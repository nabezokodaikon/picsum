using System;
using System.Linq;
using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// アドレスの情報を取得します。
    /// </summary>
    public class GetAddressInfoAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, AddressInfo>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            GetFileShallowInfoAsyncLogic logic = new GetFileShallowInfoAsyncLogic(this);

            AddressInfo addressInfo = new AddressInfo();
            addressInfo.FolderList = new ListEntity<FileShallowInfoEntity>();

            if (string.IsNullOrEmpty(param.Value))
            {
                addressInfo.FolderPath = string.Empty;
                addressInfo.FolderList.Add(logic.Execute(param.Value));
                addressInfo.HasSubFolder = true;
            }
            else
            {
                string folder = string.Empty;
                if (FileUtil.IsFile(param.Value))
                {
                    folder = FileUtil.GetParentFolderPath(param.Value);
                }
                else
                {
                    folder = param.Value;
                }

                addressInfo.FolderPath = folder;
                
                string subFolder = FileUtil.GetSubFolders(folder).FirstOrDefault(path => FileUtil.CanAccess(path));
                addressInfo.HasSubFolder = !string.IsNullOrEmpty(subFolder);

                while (!string.IsNullOrEmpty(folder))
                {
                    CheckCancel();
                    addressInfo.FolderList.Insert(0, logic.Execute(folder));
                    folder = FileUtil.GetParentFolderPath(folder);
                }

                addressInfo.FolderList.Insert(0, logic.Execute(string.Empty));
            }

            OnCallback(addressInfo);
        }
    }
}
