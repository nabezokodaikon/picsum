using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWF.OperationCheck.Contorols
{
    public class WideDropToolButtonItem
    {
        public string Text { get; private set; }

        public WideDropToolButtonItem(string text) 
        {
            if (text == null)
            { 
                throw new ArgumentNullException(nameof(text));
            }

            this.Text = text;
        }
    }
}
