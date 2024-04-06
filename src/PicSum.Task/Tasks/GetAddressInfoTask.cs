using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Task.Logics;
using PicSum.Task.Entities;
using PicSum.Task.Results;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace PicSum.Task.Tasks
{
    /// <summary>
    /// アドレスの情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class GetAddressInfoTask
        : AbstractAsyncTask<ValueParameter<string>, GetAddressInfoResult>
    {
        protected override void Execute(ValueParameter<string> param)
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

                var logic = new GetFileShallowInfoLogic(this);

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
                        throw new FileUtilException(param.Value);
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

                this.Callback(addressInfo);
            }
            catch (FileUtilException ex)
            {
                throw new TaskException(this.ID, ex);
            }
        }
    }
}
