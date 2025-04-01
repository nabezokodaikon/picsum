using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class SortInfo
    {
        private bool isFileNameSortAscending = true;
        private bool isFilePathSortAscending = true;
        private bool isUpdateDateSortAscending = true;
        private bool isRegistrationDateSortAscending = true;

        public SortTypeID ActiveSortType { get; set; }

        public bool IsAscending(SortTypeID sortType)
        {
            return sortType switch
            {
                SortTypeID.FileName => this.isFileNameSortAscending,
                SortTypeID.FilePath => this.isFilePathSortAscending,
                SortTypeID.UpdateDate => this.isUpdateDateSortAscending,
                SortTypeID.RegistrationDate => this.isRegistrationDateSortAscending,
                _ => false,
            };
        }

        public void SetSortType(SortTypeID sortType, bool isAscending)
        {
            this.ActiveSortType = sortType;

            switch (sortType)
            {
                case SortTypeID.FileName:
                    this.isFileNameSortAscending = isAscending;
                    break;
                case SortTypeID.FilePath:
                    this.isFilePathSortAscending = isAscending;
                    break;
                case SortTypeID.UpdateDate:
                    this.isUpdateDateSortAscending = isAscending;
                    break;
                case SortTypeID.RegistrationDate:
                    this.isRegistrationDateSortAscending = isAscending;
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
