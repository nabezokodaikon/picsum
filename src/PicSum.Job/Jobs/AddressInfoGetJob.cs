using PicSum.Job.Logics;
using PicSum.Job.Results;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// アドレスの情報を取得します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class AddressInfoGetJob
        : AbstractTwoWayJob<ValueParameter<string>, AddressInfoGetResult>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var addressInfo = new AddressInfoGetResult();

            try
            {
                var logic = new FileShallowInfoGetLogic(this);

                addressInfo.DirectoryList = [];

                if (FileUtil.IsSystemRoot(param.Value))
                {
                    addressInfo.DirectoryPath = FileUtil.ROOT_DIRECTORY_PATH;
                    addressInfo.DirectoryList.Add(logic.Execute(param.Value, false));
                    addressInfo.HasSubDirectory = true;
                }
                else
                {
                    var directory = string.Empty;
                    if (FileUtil.IsExistsFile(param.Value))
                    {
                        directory = FileUtil.GetParentDirectoryPath(param.Value);
                    }
                    else if (FileUtil.IsExistsDirectory(param.Value))
                    {
                        directory = param.Value;
                    }
                    else
                    {
                        throw new FileUtilException(
                            $"'{param.Value}'を開けませんでした。");
                    }

                    addressInfo.DirectoryPath = directory;

                    this.CheckCancel();
                    var subDirectory = FileUtil.GetSubDirectories(directory).FirstOrDefault(FileUtil.CanAccess);
                    addressInfo.HasSubDirectory = !string.IsNullOrEmpty(subDirectory);

                    while (!FileUtil.IsSystemRoot(directory))
                    {
                        this.CheckCancel();
                        addressInfo.DirectoryList.Insert(0, logic.Execute(directory, false));
                        directory = FileUtil.GetParentDirectoryPath(directory);
                    }

                    addressInfo.DirectoryList.Insert(0, logic.Execute(FileUtil.ROOT_DIRECTORY_PATH, false));
                }

                this.Callback(addressInfo);
            }
            catch (FileUtilException ex)
            {
                throw new JobException(this.ID, ex);
            }
        }
    }
}
