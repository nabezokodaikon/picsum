using PicSum.Job.Logics;
using PicSum.Job.Results;
using SWF.Core.FileAccessor;
using SWF.Core.Job;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// アドレスの情報を取得します。
    /// </summary>

    public sealed class AddressInfoGetJob
        : AbstractTwoWayJob<ValueParameter<string>, AddressInfoGetResult>
    {
        protected override ValueTask Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (string.IsNullOrEmpty(param.Value))
            {
                throw new ArgumentNullException(param.Value, nameof(param.Value));
            }

            var addressInfo = new AddressInfoGetResult();

            var logic = new FileShallowInfoGetLogic(this);

            addressInfo.DirectoryList = [];

            if (FileUtil.IsSystemRoot(param.Value))
            {
                addressInfo.DirectoryPath = FileUtil.ROOT_DIRECTORY_PATH;
                addressInfo.DirectoryList.Add(
                    logic.Get(param.Value, false));
                addressInfo.HasSubDirectory = true;
            }
            else
            {
                string directory;
                if (FileUtil.IsExistsFile(param.Value))
                {
                    directory = FileUtil.GetParentDirectoryPath(param.Value);
                }
                else if (FileUtil.IsExistsDirectory(param.Value)
                    || FileUtil.IsExistsDrive(param.Value))
                {
                    directory = param.Value;
                }
                else
                {
                    throw new NotSupportedException($"不正なパスが指定されました。'{param.Value}'");
                }

                addressInfo.DirectoryPath = directory;

                this.ThrowIfJobCancellationRequested();

                addressInfo.HasSubDirectory = FileUtil.HasSubDirectory(directory);

                while (!FileUtil.IsSystemRoot(directory))
                {
                    this.ThrowIfJobCancellationRequested();

                    var info = logic.Get(directory, false);
                    if (info.IsEmpty)
                    {
                        break;
                    }

                    addressInfo.DirectoryList.Insert(0, info);
                    directory = FileUtil.GetParentDirectoryPath(directory);
                }

                addressInfo.DirectoryList.Insert(
                    0, logic.Get(FileUtil.ROOT_DIRECTORY_PATH, false));
            }

            this.Callback(addressInfo);

            return ValueTask.CompletedTask;
        }
    }
}
