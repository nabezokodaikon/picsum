namespace PicSum.Core.Base.Exception
{
    /// <summary>
    /// アプリケーション例外
    /// </summary>
    public class PicSumException
        : System.Exception
    {
        public PicSumException(string message)
            : base(message)
        {

        }
    }
}
