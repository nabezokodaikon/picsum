using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PicSum.Core.Base.Log
{
    /// <summary>
    /// ログ書込みクラス
    /// </summary>
    public static class LogWriter
    {
        private static bool _isWriteLog = false;

        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private static readonly string _outputFilePath =
            string.Format(@"{0}\log\{1}.log", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DateTime.Now.ToString("yyyyMMdd"));

        public static bool IsWriteLog
        {
            get
            {
                return _isWriteLog;
            }
            set
            {
                _isWriteLog = value;
            }
        }

        /// <summary>
        /// ログを書き込みます。
        /// </summary>
        /// <param name="logBody"></param>
        public static void WriteLog(string logBody)
        {
            if (!_isWriteLog)
            {
                return;
            }

            _lock.EnterWriteLock();

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(_outputFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_outputFilePath));
                }

                using (StreamWriter sw = new StreamWriter(_outputFilePath, true))
                {
                    StringBuilder text = new StringBuilder();
                    text.AppendLine("################################################################");
                    text.AppendLine(DateTime.Now.ToString());
                    text.AppendLine(logBody);
                    sw.Write(text.ToString());
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
