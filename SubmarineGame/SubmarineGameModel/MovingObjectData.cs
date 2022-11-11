using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public enum Direction
    {
        Up, Right, Down, Left
    }
    public enum MineType
    {
        Small, Medium, Heavy
    }
    public class MovingObjectData
    {
        public Size Size { get; set; }
        public Point Location { get; set; }
        public Point MinBoundaries { get; set; }
        public Point MaxBoundaries { get; set; }
        public int Speed { get; set; }
    }
}
