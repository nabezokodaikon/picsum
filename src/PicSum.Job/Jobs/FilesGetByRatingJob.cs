using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルを評価値で検索します。
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class FilesGetByRatingJob
        : AbstractTwoWayJob<FilesGetByRatingParameter, ListResult<FileShallowInfoEntity>>
    {
        protected override void Execute(FilesGetByRatingParameter param)
        {

            var logic = new FilesGetByRatingLogic(this);
            var fileList = logic.Execute(param.RatingValue);

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in fileList)
            {
                this.CheckCancel();

                try
                {
                    var info = getInfoLogic.Get(
                        dto.FilePath, param.IsGetThumbnail, dto.RegistrationDate);
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
