using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Result;
using SWF.Common;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// アドレスの情報を取得します。
    /// </summary>
    public class GetAddressInfoAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, GetAddressInfoResult>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            var addressInfo = new GetAddressInfoResult();

            try
            {
                IList<string> l = new List<string>();
                
                GetFileShallowInfoAsyncLogic logic = new GetFileShallowInfoAsyncLogic(this);

                addressInfo.DirectoryList = new ListEntity<FileShallowInfoEntity>();

                if (FileUtil.IsSystemRoot(param.Value))
                {
                    addressInfo.DirectoryPath = FileUtil.ROOT_DIRECTORY_PATH;
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

                    while (!FileUtil.IsSystemRoot(directory))
                    {
                        CheckCancel();
                        addressInfo.DirectoryList.Insert(0, logic.Execute(directory));
                        directory = FileUtil.GetParentDirectoryPath(directory);
                    }

                    addressInfo.DirectoryList.Insert(0, logic.Execute(FileUtil.ROOT_DIRECTORY_PATH));
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
