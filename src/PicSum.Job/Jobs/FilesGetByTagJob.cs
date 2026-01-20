using PicSum.DatabaseAccessor.Connection;
using PicSum.DatabaseAccessor.Dto;
using PicSum.Job.Entities;
using PicSum.Job.Logics;
using PicSum.Job.Parameters;
using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.Collections.Concurrent;

namespace PicSum.Job.Jobs
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    public sealed class FilesGetByTagJob
        : AbstractTwoWayJob<FilesGetByTagParameter, ListResult<FileShallowInfoEntity>>
    {
        private const int MAX_DEGREE_OF_PARALLELISM = 8;

        protected override async ValueTask Execute(FilesGetByTagParameter param)
        {
            ArgumentNullException.ThrowIfNull(param, nameof(param));

            if (string.IsNullOrEmpty(param.Tag))
            {
                throw new NotSupportedException("タグが設定されていません。");
            }

            var getInfoLogic = new FileShallowInfoGetLogic(this);
            var infoList = new ConcurrentBag<FileShallowInfoEntity>();
            var dtos = await this.GetFiles(param.Tag).False();

            using (Measuring.Time(true, "FilesGetByTagJob Parallel.ForEach"))
            {
                Parallel.ForEach(
                    dtos,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = MAX_DEGREE_OF_PARALLELISM,
                    },
                    (dto, state) =>
                    {
                        if (this.IsJobCancel)
                        {
                            state.Stop();
                            return;
                        }

                        try
                        {
                            var info = getInfoLogic.Get(
                                dto.FilePath, param.IsGetThumbnail, dto.RegistrationDate)
                                .GetAwaiter().GetResult();
                            if (!info.IsEmpty)
                            {
                                infoList.Add(info);
                            }
                        }
                        catch (FileUtilException ex)
                        {
                            this.WriteErrorLog(ex);
                            return;
                        }
                        catch (ObjectDisposedException)
                        {
                            return;
                        }
                    });
            }

            this.Callback([.. infoList]);
        }

        private async ValueTask<FileByTagDto[]> GetFiles(string tag)
        {
            using (Measuring.Time(true, "FilesGetByTagJob.GetFiles"))
            {
                var con = await Instance<IFileInfoDao>.Value.Connect().False();
                try
                {
                    var logic = new FilesGetByTagLogic(this);
                    return await logic.Execute(con, tag).False();
                }
                finally
                {
                    await con.DisposeAsync().False();
                }
            }
        }
    }
}
