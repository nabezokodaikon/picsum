using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// アドレスの情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetAddressInfoAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, GetAddressInfoResult>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (string.IsNullOrEmpty(param.Value))
            {
                // TODO; ファイルパスに空白が指定された場合。
                //throw new ArgumentNullException("空白が指定されました。", nameof(param));
                return;
            }

            var addressInfo = new GetAddressInfoResult();

            try
            {
                var l = new List<string>();

                var logic = new GetFileShallowInfoAsyncLogic(this);

                addressInfo.DirectoryList = new ListEntity<FileShallowInfoEntity>();

                if (FileUtil.IsSystemRoot(param.Value))
                {
                    addressInfo.DirectoryPath = FileUtil.ROOT_DIRECTORY_PATH;
                    addressInfo.DirectoryList.Add(logic.Execute(param.Value));
                    addressInfo.HasSubDirectory = true;
                }
                else
                {
                    var directory = string.Empty;
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

                    var subDirectory = FileUtil.GetSubDirectorys(directory).FirstOrDefault(path => FileUtil.CanAccess(path));
                    addressInfo.HasSubDirectory = !string.IsNullOrEmpty(subDirectory);

                    while (!FileUtil.IsSystemRoot(directory))
                    {
                        this.CheckCancel();
                        addressInfo.DirectoryList.Insert(0, logic.Execute(directory));
                        directory = FileUtil.GetParentDirectoryPath(directory);
                    }

                    addressInfo.DirectoryList.Insert(0, logic.Execute(FileUtil.ROOT_DIRECTORY_PATH));
                }

                addressInfo.GetAddressInfoException = null;

                this.OnCallback(addressInfo);
            }
            catch (FileUtilException ex)
            {
                addressInfo.GetAddressInfoException = ex;
                this.OnCallback(addressInfo);
            }
        }
    }
}
