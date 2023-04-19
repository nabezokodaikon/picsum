using PicSum.Core.Data.DatabaseAccessor;
using PicSum.Data.DatabaseAccessor.Dto;

namespace PicSum.Data.DatabaseAccessor.Sql
{
    /// <summary>
    /// 評価値を指定してファイルを読込みます。
    /// </summary>
    public class ReadFileByRatingSql : SqlBase<FileByRatingDto>
    {
        public ReadFileByRatingSql(int rating)
            : base()
        {
            base.ParameterList.Add(SqlParameterUtil.CreateParameter("rating", rating));
        }
    }
}