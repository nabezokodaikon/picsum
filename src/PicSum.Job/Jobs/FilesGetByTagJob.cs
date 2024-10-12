using SWF.Core.Job;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using SWF.Core.FileAccessor;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class FilesGetByTagJob
        : AbstractTwoWayJob<ValueParameter<string>, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(ValueParameter<string> param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            var logic = new FilesGetByTagLogic(this);
            var dtoList = logic.Execute(param.Value);

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in dtoList)
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
