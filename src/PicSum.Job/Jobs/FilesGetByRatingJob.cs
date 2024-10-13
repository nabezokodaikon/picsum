using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FilesGetByRatingJob
        : AbstractTwoWayJob<ValueParameter<int>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<int> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var logic = new FilesGetByRatingLogic(this);
            var fileList = logic.Execute(param.Value);

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in fileList)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Execute(dto.FilePath, dto.RegistrationDate);
                    if (info != null)
                    {
                        infoList.Add(info);
                    }
                }
                catch (FileUtilException ex)
                {
                    this.WriteErrorLog(new JobException(this.ID, ex));
                    continue;
                }
            }

            this.Callback(infoList);
        }
    }
}
