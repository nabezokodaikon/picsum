using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.Core.Task.Base
{
    public abstract class AbstractResultEntity
        : IEntity
    {
        public TaskException TaskException { get; set; }
    }
}
