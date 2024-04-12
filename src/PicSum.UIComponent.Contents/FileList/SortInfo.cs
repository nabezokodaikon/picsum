using PicSum.Core.Base.Conf;
using PicSum.UIComponent.Contents.Properties;
using System.Drawing;

namespace PicSum.UIComponent.Contents.FileList
{
    public sealed class SortInfo
    {
        private readonly Image sortAscendingImage = Resources.SmallArrowUp;
        private readonly Image sortDescendingImage = Resources.SmallArrowDown;
        private bool isFileNameSortAscending = true;
        private bool isFilePathSortAscending = true;
        private bool isUpdateDateSortAscending = true;
        private bool isRgistrationDateSortAscending = true;

        public SortTypeID ActiveSortType { get; set; }

        public bool IsAscending(SortTypeID sortType)
        {
            return sortType switch
            {
                SortTypeID.FileName => this.isFileNameSortAscending,
                SortTypeID.FilePath => this.isFilePathSortAscending,
                SortTypeID.UpdateDate => this.isUpdateDateSortAscending,
                SortTypeID.RgistrationDate => this.isRgistrationDateSortAscending,
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
                case SortTypeID.RgistrationDate:
                    this.isRgistrationDateSortAscending = isAscending;
                    break;
                default:
                    break;
            }
        }

        public Image GetSortDirectionImage(bool isAcending)
        {
            if (isAcending)
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
