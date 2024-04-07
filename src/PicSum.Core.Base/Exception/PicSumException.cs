using SWF.Common;

namespace PicSum.Core.Base.Exception
{
    /// <summary>
    /// アプリケーション例外
    /// </summary>
    public class PicSumException
        : SWFException
    {
        public PicSumException(string message)
            : base(message)
        {

        }
    }
}
