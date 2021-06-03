using System;
using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System.Collections;
using System.Collections.Generic;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// サブフォルダ取得非同期ファサード
    /// </summary>
    /// <remarks>フォルダパスが空文字の場合、ドライブリストを取得します。</remarks>
    public class GetSubFoldersAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            IList<string> subFolders = null;
            if (string.IsNullOrEmpty(param.Value))
            {
                subFolders = (new GetDrivesAsyncLogic(this)).Execute();
            }
            else
            {
                subFolders = (new GetSubFoldersAsyncLogic(this)).Execute(param.Value);
            }            

            GetFileShallowInfoAsyncLogic logic=new GetFileShallowInfoAsyncLogic(this);
            ListEntity<FileShallowInfoEntity> result = new ListEntity<FileShallowInfoEntity>();
            foreach (string subFolder in subFolders)
            {
                CheckCancel();
                result.Add(logic.Execute(subFolder));
            }
            
            OnCallback(result);
        }
    }
}
