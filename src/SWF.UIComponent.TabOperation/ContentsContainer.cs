using System;
using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    /// <summary>
    /// コンテンツコンテナ
    /// </summary>
    public sealed class ContentsContainer
        : Panel
    {
        public ContentsContainer()
        {
            this.DoubleBuffered = true;
        }

        private ContentsPanel contentsPanel
        {
            get
            {
                if (this.Controls.Count == 1)
                {
                    return (ContentsPanel)this.Controls[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// コンテンツを設定します。
        /// </summary>
        /// <param name="contents"></param>
        internal void SetContents(ContentsPanel contents)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            if (this.Controls.Count > 0)
            {
                throw new Exception("既にコンテンツが存在しているため、新たなコンテンツを設定できません。");
            }

            contents.SetBounds(0, 0, this.Width, this.Height, BoundsSpecified.All);
            contents.Dock = DockStyle.Fill;
            this.Controls.Add(contents);

            contents.Active();
        }

        /// <summary>
        /// コンテンツをクリアします。
        /// </summary>
        internal void ClearContents()
        {
            var contents = this.contentsPanel;
            this.Controls.Clear();
        }
    }
}
