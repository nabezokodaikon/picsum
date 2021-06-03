using System;

namespace SWF.UIComponent.FlowList
{
    internal class Cell
    {
        private int _row;
        private int _col;

        public int Row
        {
            get
            {
                return _row;
            }
        }

        public int Col
        {
            get
            {
                return _col;
            }
        }

        public Cell(int row, int col)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException("row");
            }

            if (col < 0)
            {
                throw new ArgumentOutOfRangeException("col");
            }

            _row = row;
            _col = col;
        }
    }
}
