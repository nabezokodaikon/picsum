using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PicSum.Core.Task.Base;

namespace PicSum.Task.Entity
{
    public class SearchFavoriteFolderParameterEntity : IEntity
    {
        public bool IsOnlyFolder { get; set; }
        public int Count { get; set; }
    }
}
