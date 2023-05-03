using System;

namespace SWF.UIComponent.FlowList
{
    internal sealed class Cell
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Cell(int row, int col)
        {
            if (row < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (col < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(col));
            }

            this.Row = row;
            this.Col = col;
        }
    }
}
