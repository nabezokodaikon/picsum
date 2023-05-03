using System.Windows.Forms;

namespace SWF.UIComponent.TabOperation
{
    public class TabAreaDragEventArgs : DragEventArgs
    {
        private bool _isOverlap;
        private int _tabIndex;

        public bool IsOverlap
        {
            get
            {
                return _isOverlap;
            }
        }

        public int TabIndex
        {
            get
            {
                return _tabIndex;
            }
        }

        public TabAreaDragEventArgs(bool isOverlap, int tabIndex, DragEventArgs e)
            : base(e.Data, e.KeyState, e.X, e.Y, e.AllowedEffect, e.Effect)
        {
            _isOverlap = isOverlap;
            _tabIndex = tabIndex;
        }
    }
}
