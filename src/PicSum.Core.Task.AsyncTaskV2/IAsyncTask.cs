using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSum.Core.Task.AsyncTaskV2
{
    public interface IAsyncTask
    {
        public void CheckCancel();
    }
}
