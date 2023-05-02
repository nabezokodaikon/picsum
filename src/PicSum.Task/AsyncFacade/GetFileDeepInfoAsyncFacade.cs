using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using PicSum.Task.Paramter;
using PicSum.Task.Result;
using SWF.Common;
using System;
using System.IO;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルの深い情報取得非同期ファサード
    /// </summary>
    public sealed class GetFileDeepInfoAsyncFacade
        : TwoWayFacadeBase<GetFileDeepInfoParameter, GetFileDeepInfoResult>
    {
        public override void Execute(GetFileDeepInfoParameter param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var result = new GetFileDeepInfoResult();

            try
            {
                result.FilePathList = param.FilePathList;

                if (param.FilePathList.Count == 1)
                {
                    var getInfoLogic = new GetFileDeepInfoAsyncLogic(this);
                    var filePath = param.FilePathList[0];
                    result.FileInfo = getInfoLogic.Execute(filePath, param.ThumbnailSize);
                }

                if (param.FilePathList.Count <= 997)
                {
                    var logic = new GetFileTagInfoAsyncLogic(this);
                    result.TagInfoList = logic.Execute(result.FilePathList);
                }
                else
                {
                    result.TagInfoList = new ListEntity<FileTagInfoEntity>();
                }

                this.CheckCancel();
            }
            catch (ArgumentException)
            {
                return;
            }
            catch (FileNotFoundException)
            {
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (DriveNotFoundException)
            {
                return;
            }
            catch (ImageUtilException)
            {
                return;
            }

            this.OnCallback(result);
        }
    }
}
