using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FilesGetByTagJob
        : AbstractTwoWayJob<FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(FilesGetByTagParameter param)
        {
            if (string.IsNullOrEmpty(param.Tag))
            {
                throw new InvalidOperationException("タグが設定されていません。");
            }

            var logic = new FilesGetByTagLogic(this);
            var dtoList = logic.Execute(param.Tag);

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in dtoList)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Execute(
                        dto.FilePath, dto.RegistrationDate, param.IsGetThumbnail);
                    if (info != FileShallowInfoEntity.EMPTY)
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
