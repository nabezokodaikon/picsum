namespace SWF.Core.Base
{
    public sealed class SortInfo
    {
        private readonly Image sortAscendingImage = ResourceFiles.SmallArrowUpIcon.Value;
        private readonly Image sortDescendingImage = ResourceFiles.SmallArrowDownIcon.Value;
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

        public Image GetSortDirectionImage(bool isAscending)
        {
            if (isAscending)
            {
                return this.sortAscendingImage;
            }
            else
            {
                return this.sortDescendingImage;
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
