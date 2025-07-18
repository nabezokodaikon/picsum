using SWF.Core.Base;
using SWF.UIComponent.Core;
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツコンテナ
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class PageContainer
        : BaseControl
    {
        public PageContainer()
        {

        }

        /// <summary>
        /// ページを設定します。
        /// </summary>
        /// <param name="page"></param>
        internal void SetPage(PagePanel page)
        {
            using (TimeMeasuring.Run(true, "PageContainer.SetPage"))
            {
                ArgumentNullException.ThrowIfNull(page, nameof(page));

                if (this.Controls.Count > 0)
                {
                    throw new InvalidOperationException("既にページが存在しているため、新たなページを設定できません。");
                }

                this.Controls.Add(page);
                var scale = WindowUtil.GetCurrentWindowScale(this);
                page.RedrawPage(scale);
                page.SetBounds(0, 0, this.Width, this.Height);
                page.Anchor
                    = AnchorStyles.Top
                    | AnchorStyles.Bottom
                    | AnchorStyles.Left
                    | AnchorStyles.Right;
                page.Active();
            }
        }

        /// <summary>
        /// ページをクリアします。
        /// </summary>
        internal void ClearPage()
        {
            this.Controls.Clear();
        }
    }
}
