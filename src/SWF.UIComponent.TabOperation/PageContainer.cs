using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツコンテナ
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class PageContainer
        : Panel
    {
        public PageContainer()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
            this.UpdateStyles();
        }

        /// <summary>
        /// ページを設定します。
        /// </summary>
        /// <param name="page"></param>
        internal void SetPage(PagePanel page)
        {
            ArgumentNullException.ThrowIfNull(page, nameof(page));

            if (this.Controls.Count > 0)
            {
                throw new InvalidOperationException("既にページが存在しているため、新たなページを設定できません。");
            }

            page.SetBounds(0, 0, this.Width, this.Height, BoundsSpecified.All);
            page.Dock = DockStyle.Fill;
            this.Controls.Add(page);

            page.Active();
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
