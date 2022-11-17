using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public class MineModel: MovingObjectData
    {
        private System.Timers.Timer _moveTimer;
        private int _tickTime = 20;
        public MineType MineType { get; set; }

        public MineModel(Size size, Point location, Point minBounds, Point maxBounds, int speed, MineType mineType)
        {
            Size = size;
            Location = location;
            MinBoundaries = minBounds;
            MaxBoundaries = maxBounds;
            Speed = speed;
            MineType = mineType;
            _moveTimer = new System.Timers.Timer(_tickTime);
            _moveTimer.Elapsed += Move;
            _moveTimer.Start();
        }

        public MineModel()
            : this(new Size(), new Point(), new Point(), new Point(), 0, MineType.Small)
        {
            
        }

        private void Move(object? sender, EventArgs e)
        {
            Location = new Point(Location.X, Location.Y + Speed);
            if (Location.Y >= MaxBoundaries.Y + Size.Height)
            {
                OutOfVisibleArea?.Invoke(this, EventArgs.Empty);
            }
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            _moveTimer.Stop();
        }

        public void Start()
        {
            _moveTimer.Start();
        }

        public event EventHandler? Updated;
        public event EventHandler? OutOfVisibleArea;
    }
}
