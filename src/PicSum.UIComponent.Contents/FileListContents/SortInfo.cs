using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.UIComponent.Contents.Properties;
using System.Drawing;
using PicSum.Core.Base.Conf;

namespace PicSum.UIComponent.Contents.FileListContents
{
    class SortInfo
    {
        private readonly Image _sortAscendingImage = Resources.SmallArrowUp;
        private readonly Image _sortDescendingImage = Resources.SmallArrowDown;
        private SortTypeID _activeSortType = SortTypeID.Default;
        private bool _isFileNameSortAscending = true;
        private bool _isFilePathSortAscending = true;
        private bool _isUpdateDateSortAscending = true;
        private bool _isCreateDateSortAscending = true;

        public SortTypeID ActiveSortType
        {
            get
            {
                return _activeSortType;
            }
            set
            {
                _activeSortType = value;
            }
        }

        public bool IsAscending(SortTypeID sortType)
        {
            switch (sortType)
            {
                case SortTypeID.FileName:
                    return _isFileNameSortAscending;
                case SortTypeID.FilePath:
                    return _isFilePathSortAscending;
                case SortTypeID.UpdateDate:
                    return _isUpdateDateSortAscending;
                case SortTypeID.CreateDate:
                    return _isCreateDateSortAscending;
                default:
                    return false;
            }
        }

        public void SetSortType(SortTypeID sortType, bool isAscending)
        {
            _activeSortType = sortType;

            switch (sortType)
            {
                case SortTypeID.FileName:
                    _isFileNameSortAscending = isAscending;
                    break;
                case SortTypeID.FilePath:
                    _isFilePathSortAscending = isAscending;
                    break;
                case SortTypeID.UpdateDate:
                    _isUpdateDateSortAscending = isAscending;
                    break;
                case SortTypeID.CreateDate:
                    _isCreateDateSortAscending = isAscending;
                    break;
                default:
                    break;
            }
        }

        public Image GetSortDirectionImage(bool isAcending)
        {
            if (isAcending)
            {
                return _sortAscendingImage;
            }
            else
            {
                return _sortDescendingImage;
            }
        }

        public void ChangeSortDirection(SortTypeID sortType)
        {
            if (_activeSortType == sortType)
            {
                SetSortType(sortType, !IsAscending(sortType));
            }
            else
            {
                SetSortType(sortType, IsAscending(sortType));
            }
        }
    }
}
