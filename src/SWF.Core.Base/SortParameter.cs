namespace SWF.Core.Base
{

    public sealed class SortParameter
    {
        private bool _isFileNameSortAscending = true;
        private bool _isFilePathSortAscending = true;
        private bool _isCreateDateSortAscending = true;
        private bool _isUpdateDateSortAscending = true;
        private bool _isTakenDateSortAscending = true;
        private bool _isAddDateSortAscending = true;

        public FileSortMode ActiveSortMode { get; set; }

        public bool IsAscending(FileSortMode sortMode)
        {
            return sortMode switch
            {
                FileSortMode.FileName => this._isFileNameSortAscending,
                FileSortMode.FilePath => this._isFilePathSortAscending,
                FileSortMode.CreateDate => this._isCreateDateSortAscending,
                FileSortMode.UpdateDate => this._isUpdateDateSortAscending,
                FileSortMode.TakenDate => this._isTakenDateSortAscending,
                FileSortMode.AddDate => this._isAddDateSortAscending,
                _ => false,
            };
        }

        public void SetSortMode(FileSortMode sortMode, bool isAscending)
        {
            this.ActiveSortMode = sortMode;

            switch (sortMode)
            {
                case FileSortMode.FileName:
                    this._isFileNameSortAscending = isAscending;
                    break;
                case FileSortMode.FilePath:
                    this._isFilePathSortAscending = isAscending;
                    break;
                case FileSortMode.CreateDate:
                    this._isCreateDateSortAscending = isAscending;
                    break;
                case FileSortMode.UpdateDate:
                    this._isUpdateDateSortAscending = isAscending;
                    break;
                case FileSortMode.TakenDate:
                    this._isTakenDateSortAscending = isAscending;
                    break;
                case FileSortMode.AddDate:
                    this._isAddDateSortAscending = isAscending;
                    break;
                default:
                    break;
            }
        }

        public string GetSortDirectionArrow(bool isAscending)
        {
            if (isAscending)
            {
                return "▲";
            }
            else
            {
                return "▼";
            }
        }

        public void ChangeSortDirection(FileSortMode sortMode)
        {
            if (this.ActiveSortMode == sortMode)
            {
                this.SetSortMode(sortMode, !this.IsAscending(sortMode));
            }
            else
            {
                this.SetSortMode(sortMode, this.IsAscending(sortMode));
            }
        }
    }
}
