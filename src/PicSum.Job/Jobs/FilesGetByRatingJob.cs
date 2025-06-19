using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
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
        protected override Task Execute(FilesGetByRatingParameter param)
        {
            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ListResult<FileShallowInfoEntity>();
            foreach (var dto in this.GetFiles(param.RatingValue))
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

            return Task.CompletedTask;
        }

        private FileByRatingDto[] GetFiles(int ratingValue)
        {
            using (var con = Instance<IFileInfoDB>.Value.Connect())
            {
                var logic = new FilesGetByRatingLogic(this);
                return logic.Execute(con, ratingValue);
            }
        }
    }
}
