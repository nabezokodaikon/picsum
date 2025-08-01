using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class SortInfo
    {
        private bool _isFileNameSortAscending = true;
        private bool _isFilePathSortAscending = true;
        private bool _isCreateDateSortAscending = true;
        private bool _isUpdateDateSortAscending = true;
        private bool _isRegistrationDateSortAscending = true;

        public SortTypeID ActiveSortType { get; set; }

        public bool IsAscending(SortTypeID sortType)
        {
            return sortType switch
            {
                SortTypeID.FileName => this._isFileNameSortAscending,
                SortTypeID.FilePath => this._isFilePathSortAscending,
                SortTypeID.CreateDate => this._isCreateDateSortAscending,
                SortTypeID.UpdateDate => this._isUpdateDateSortAscending,
                SortTypeID.RegistrationDate => this._isRegistrationDateSortAscending,
                _ => false,
            };
        }

        public void SetSortType(SortTypeID sortType, bool isAscending)
        {
            this.ActiveSortType = sortType;

            switch (sortType)
            {
                case SortTypeID.FileName:
                    this._isFileNameSortAscending = isAscending;
                    break;
                case SortTypeID.FilePath:
                    this._isFilePathSortAscending = isAscending;
                    break;
                case SortTypeID.CreateDate:
                    this._isCreateDateSortAscending = isAscending;
                    break;
                case SortTypeID.UpdateDate:
                    this._isUpdateDateSortAscending = isAscending;
                    break;
                case SortTypeID.RegistrationDate:
                    this._isRegistrationDateSortAscending = isAscending;
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

        public void ChangeSortDirection(SortTypeID sortType)
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
