using System.Collections.Generic;
using System.IO;
using PicSum.Core.Task.AsyncTask;
using SWF.Common;

namespace PicSum.Task.AsyncLogic
{
    /// <summary>
    /// 画像ファイルエクスポート非同期ロジック
    /// </summary>
    internal class ExportFileAsyncLogic : AsyncLogicBase
    {
        public ExportFileAsyncLogic(AsyncFacadeBase facade) : base(facade) { }

        public void Execute(string exportDirectory, IList<string> srcFilePathList)
        {
            foreach (string srcFilePath in srcFilePathList)
            {
                string ex = FileUtil.GetExtension(srcFilePath).ToLower();
                string name = FileUtil.GetFileName(srcFilePath);
                name = name.Substring(0, name.Length - ex.Length);

                int count = 0;

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
