using PicSum.Core.Data.FileAccessor;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;
using System.Linq;

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

            var addressInfo = new AddressInfo();

            try
            {
                GetFileShallowInfoAsyncLogic logic = new GetFileShallowInfoAsyncLogic(this);

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
                    else if (FileUtil.IsFolder(param.Value))
                    {
                        folder = param.Value;
                    }
                    else
                    {
                        throw new FileException(string.Format("ファイル '{0}' が見つかりませんでした。", param.Value));
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

                addressInfo.GetAddressInfoException = null;

                OnCallback(addressInfo);
            }
            catch (FileException ex)
            {
                addressInfo.GetAddressInfoException = ex;
                OnCallback(addressInfo);
            }
        }
    }
}
