using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class SortParameter
    {
        private bool _isFileNameSortAscending = true;
        private bool _isFilePathSortAscending = true;
        private bool _isCreateDateSortAscending = true;
        private bool _isUpdateDateSortAscending = true;
        private bool _isTakenDateSortAscending = true;
        private bool _isAddDateSortAscending = true;

        public SortMode ActiveSortType { get; set; }

        public bool IsAscending(SortMode sortType)
        {
            return sortType switch
            {
                SortMode.FileName => this._isFileNameSortAscending,
                SortMode.FilePath => this._isFilePathSortAscending,
                SortMode.CreateDate => this._isCreateDateSortAscending,
                SortMode.UpdateDate => this._isUpdateDateSortAscending,
                SortMode.TakenDate => this._isTakenDateSortAscending,
                SortMode.AddDate => this._isAddDateSortAscending,
                _ => false,
            };
        }

        public void SetSortType(SortMode sortType, bool isAscending)
        {
            this.ActiveSortType = sortType;

            switch (sortType)
            {
                case SortMode.FileName:
                    this._isFileNameSortAscending = isAscending;
                    break;
                case SortMode.FilePath:
                    this._isFilePathSortAscending = isAscending;
                    break;
                case SortMode.CreateDate:
                    this._isCreateDateSortAscending = isAscending;
                    break;
                case SortMode.UpdateDate:
                    this._isUpdateDateSortAscending = isAscending;
                    break;
                case SortMode.TakenDate:
                    this._isTakenDateSortAscending = isAscending;
                    break;
                case SortMode.AddDate:
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

        public void ChangeSortDirection(SortMode sortType)
        {
            if (this.ActiveSortType == sortType)
            {
                this.SetSortType(sortType, !this.IsAscending(sortType));
            }
            else
            {
                this.SetSortType(sortType, this.IsAscending(sortType));
            }
        }
    }
}
