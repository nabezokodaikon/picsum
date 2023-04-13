using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using SWF.Common;
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

                addressInfo.DirectoryList = new ListEntity<FileShallowInfoEntity>();

                if (string.IsNullOrEmpty(param.Value))
                {
                    addressInfo.DirectoryPath = string.Empty;
                    addressInfo.DirectoryList.Add(logic.Execute(param.Value));
                    addressInfo.HasSubDirectory = true;
                }
                else
                {
                    string directory = string.Empty;
                    if (FileUtil.IsFile(param.Value))
                    {
                        directory = FileUtil.GetParentDirectoryPath(param.Value);
                    }
                    else if (FileUtil.IsDirectory(param.Value))
                    {
                        directory = param.Value;
                    }
                    else
                    {
                        throw new FileUtilException(string.Format("ファイル '{0}' が見つかりませんでした。", param.Value));
                    }

                    addressInfo.DirectoryPath = directory;

                    string subDirectory = FileUtil.GetSubDirectorys(directory).FirstOrDefault(path => FileUtil.CanAccess(path));
                    addressInfo.HasSubDirectory = !string.IsNullOrEmpty(subDirectory);

                    while (!string.IsNullOrEmpty(directory))
                    {
                        CheckCancel();
                        addressInfo.DirectoryList.Insert(0, logic.Execute(directory));
                        directory = FileUtil.GetParentDirectoryPath(directory);
                    }

                    addressInfo.DirectoryList.Insert(0, logic.Execute(string.Empty));
                }

                addressInfo.GetAddressInfoException = null;

                OnCallback(addressInfo);
            }
            catch (FileUtilException ex)
            {
                addressInfo.GetAddressInfoException = ex;
                OnCallback(addressInfo);
            }
        }
    }
}
