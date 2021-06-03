﻿using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System.IO;
using PicSum.Core.Data.FileAccessor;
using SWF.Common;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// ファイルの深い情報取得非同期ファサード
    /// </summary>
    public class GetFileDeepInfoAsyncFacade
        : TwoWayFacadeBase<GetFileDeepInfoParameterEntity, GetFileDeepInfoResultEntity>
    {
        public override void Execute(GetFileDeepInfoParameterEntity param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            GetFileDeepInfoResultEntity result = new GetFileDeepInfoResultEntity();

            try
            {
                result.FilePathList = param.FilePathList;

                if (param.FilePathList.Count == 1)
                {
                    GetFileDeepInfoAsyncLogic getInfoLogic = new GetFileDeepInfoAsyncLogic(this);
                    string filePath = param.FilePathList[0];
                    result.FileInfo = getInfoLogic.Execute(filePath, param.ThumbnailSize);
                }

                if (param.FilePathList.Count <= 997)
                {
                    GetFileTagInfoAsyncLogic logic = new GetFileTagInfoAsyncLogic(this);
                    result.TagInfoList = logic.Execute(result.FilePathList);
                }
                else
                {
                    result.TagInfoList = new ListEntity<FileTagInfoEntity>();
                }

                CheckCancel();
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
            catch (ImageException)
            {
                return;
            }

            OnCallback(result);
        }
    }
}
