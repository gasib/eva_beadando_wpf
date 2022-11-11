using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public class SubmarineModel: MovingObjectData
    {
        public SubmarineModel(Size size, Point location
            , Point minBoundaries, Point maxBoundaries
            , int speed)
        {
            Size = size;
            Location = location;
            MinBoundaries = minBoundaries;
            MaxBoundaries = maxBoundaries;
            Speed = speed;
        }

        public SubmarineModel()
            : this(new Size(), new Point(), new Point(), new Point(), 0)
        {
            
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    Location = new Point(Location.X, Location.Y - Speed);
                    if (Location.Y < MinBoundaries.Y)
                    {
                        Location = new Point(Location.X, MinBoundaries.Y);
                    }
                    break;
                case Direction.Right:
                    Location = new Point(Location.X + Speed, Location.Y);
                    if (Location.X > MaxBoundaries.X - Size.Width)
                    {
                        Location = new Point(MaxBoundaries.X - Size.Width, Location.Y);
                    }
                    break;
                case Direction.Down:
                    Location = new Point(Location.X, Location.Y + Speed);
                    if (Location.Y > MaxBoundaries.Y - Size.Height)
                    {
                        Location = new Point(Location.X, MaxBoundaries.Y - Size.Height);
                    }
                    break;
                case Direction.Left:
                    Location = new Point(Location.X - Speed, Location.Y);
                    if (Location.X < MinBoundaries.X)
                    {
                        Location = new Point(MinBoundaries.X, Location.Y);
                    }
                    break;
                default: break;
            }
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? Updated;
    }
}
