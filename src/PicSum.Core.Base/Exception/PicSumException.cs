using System;

namespace PicSum.Core.Base.Exception
{
    /// <summary>
    /// アプリケーション例外
    /// </summary>
    public class PicSumException
        : ApplicationException
    {
        public PicSumException(string message)
            : base(message) { }
    }
}
