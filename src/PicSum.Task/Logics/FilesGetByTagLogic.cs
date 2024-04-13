using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Core.Task.AsyncTaskV2;
using PicSum.Data.DatabaseAccessor.Connection;
using PicSum.Data.DatabaseAccessor.Dto;
using PicSum.Data.DatabaseAccessor.Sql;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace PicSum.Task.Logics
{
    /// <summary>
    /// ファイルをタグで検索します。
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class FilesGetByTagLogic(IAsyncTask task)
        : AbstractAsyncLogic(task)
    {
        public IList<FileByTagDto> Execute(string tag)
        {
            ArgumentException.ThrowIfNullOrEmpty(tag, nameof(tag));

            var sql = new ReadFileByTagSql(tag);
            var dtoList = DatabaseManager<FileInfoConnection>.ReadList<FileByTagDto>(sql);

            var list = new List<FileByTagDto>();
            foreach (var dto in dtoList)
            {
                this.CheckCancel();
                if (FileUtil.CanAccess(dto.FilePath))
                {
                    list.Add(dto);
                }
            }

            return list;
        }
    }
}
