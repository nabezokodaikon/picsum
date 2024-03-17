using PicSum.Core.Task.AsyncTask;
using SWF.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class ExportFileAsyncLogic
        : AbstractAsyncLogic
    {
        public ExportFileAsyncLogic(AbstractAsyncFacade facade)
            : base(facade)
        {

        }

        public void Execute(string exportDirectory, IList<string> srcFilePathList)
        {
            if (exportDirectory == null)
            {
                throw new ArgumentNullException(nameof(exportDirectory));
            }

            if (srcFilePathList == null)
            {
                throw new ArgumentNullException(nameof(srcFilePathList));
            }

            foreach (var srcFilePath in srcFilePathList)
            {
                var ex = FileUtil.GetExtension(srcFilePath).ToLower();
                var name = FileUtil.GetFileName(srcFilePath);
                name = name.Substring(0, name.Length - ex.Length);

                var count = 0;

                do
                {
                    string destFilePath;
                    if (count == 0)
                    {
                        destFilePath = string.Format("{0}\\{1}{2}", exportDirectory, name, ex);
                    }
                    else
                    {
                        destFilePath = string.Format("{0}\\{1}-({2}){3}", exportDirectory, name, count, ex);
                    }

                    if (!FileUtil.IsExists(destFilePath))
                    {
                        File.Copy(srcFilePath, destFilePath);
                        break;
                    }

                    count++;

                } while (true);
            }
        }
    }
}
