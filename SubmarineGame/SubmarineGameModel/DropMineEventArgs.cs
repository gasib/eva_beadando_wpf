using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SubmarineGameModel
{
    public class DropMineEventArgs: EventArgs
    {
        public MineType MineType { get; private set; }
        public Point Location { get; private set; }

        public DropMineEventArgs(MineType mineType, Point location)
            : base()
        {
            MineType = mineType;
            Location = location;
        }
    }
}
