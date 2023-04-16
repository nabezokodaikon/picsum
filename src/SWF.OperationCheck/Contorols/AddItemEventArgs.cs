using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWF.OperationCheck.Contorols
{
    public sealed class AddItemEventArgs
        : EventArgs
    {
        public string Item { get; private set; }

        public AddItemEventArgs(string item)
        {
            if (item == null)
            { 
                throw new ArgumentNullException(nameof(item));
            }

            this.Item = item;
        }
    }
}
