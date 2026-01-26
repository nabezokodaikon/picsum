using System;

namespace SWF.UIComponent.SKFlowList
{
    internal sealed class Cell
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Cell(int row, int col)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(row, 0, nameof(row));
            ArgumentOutOfRangeException.ThrowIfLessThan(col, 0, nameof(col));

            this.Row = row;
            this.Col = col;
        }
    }
}
