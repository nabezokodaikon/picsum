using PicSum.Core.Task.AsyncTask;
using PicSum.Task.AsyncLogic;
using PicSum.Task.Entity;
using System;
using System.Collections.Generic;

namespace PicSum.Task.AsyncFacade
{
    /// <summary>
    /// サブフォルダ取得非同期ファサード
    /// </summary>
    /// <remarks>フォルダパスが空文字の場合、ドライブリストを取得します。</remarks>
    public sealed class GetSubDirectoryAsyncFacade
        : TwoWayFacadeBase<SingleValueEntity<string>, ListEntity<FileShallowInfoEntity>>
    {
        public override void Execute(SingleValueEntity<string> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var subDirectorys = (new GetSubDirectorysAsyncLogic(this)).Execute(param.Value);

            var logic = new GetFileShallowInfoAsyncLogic(this);
            var result = new ListEntity<FileShallowInfoEntity>();
            foreach (var subDirectory in subDirectorys)
            {
                this.CheckCancel();
                result.Add(logic.Execute(subDirectory));
            }

            this.OnCallback(result);
        }
    }
}
