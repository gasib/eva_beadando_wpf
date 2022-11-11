using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public class SizeArgs: EventArgs
    {
        public Size Size { get; private set; }
        public SizeArgs(Size size): base()
        {
            Size = size;
        }
    }
}
