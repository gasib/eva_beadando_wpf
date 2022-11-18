using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public class ShipModel: MovingObjectData
    {
        private System.Timers.Timer _moveTimer;
        private System.Timers.Timer _dropTimer;
        private int _tickTime = 20;
        private static Random _random = new Random();
        public Direction Direction { get; set; }
        public MineType MineType { get; set; }
        public int AverageDropTime { get; set; }

        public ShipModel(Size size, Point location
            , Point minBoundaries, Point maxBoundaries
            , int speed, Direction direction
            , MineType mineType
            , int averageDropTime)
        {
            Size = size;
            Location = location;
            MinBoundaries = minBoundaries;
            MaxBoundaries = maxBoundaries;
            Speed = speed;
            Direction = direction;
            MineType = mineType;
            AverageDropTime = averageDropTime;
            _moveTimer = new System.Timers.Timer(_tickTime);
            _moveTimer.Elapsed += Move;
            _dropTimer = new System.Timers.Timer(RandomDropTime(MineType, AverageDropTime));
            _dropTimer.Elapsed += OnDropTimerTick;
            _moveTimer.Start();
            _dropTimer.Start();
        }

        public ShipModel()
            : this(new Size(), new Point(), new Point(), new Point(), 0
                  , RandomDirection(), RandomMineType(), 0)
        {

        }

        private static Direction RandomDirection()
        {
            return (_random.Next() % 2 == 0) ? Direction.Left : Direction.Right;
        }

        private static MineType RandomMineType()
        {
            return (MineType)(_random.Next() % 3);
        }

        public void Stop()
        {
            _moveTimer.Stop();
            _dropTimer.Stop();
        }

        public void Start()
        {
            _moveTimer.Start();
            _dropTimer.Start();
        }

        private void Move(object? sender, EventArgs e)
        {
            switch (Direction)
            {
                case Direction.Right:
                    Location = new Point(Location.X + Speed, Location.Y);
                    if (Location.X > MaxBoundaries.X - Size.Width)
                    {
                        Location = new Point(MaxBoundaries.X - Size.Width, Location.Y);
                        if (Location.X == MaxBoundaries.X - Size.Width)
                        {
                            Direction = Direction.Left;
                        }
                    }
                    break;
                case Direction.Left:
                    Location = new Point(Location.X - Speed, Location.Y);
                    if (Location.X < MinBoundaries.X)
                    {
                        Location = new Point(MinBoundaries.X, Location.Y);
                        if (Location.X == MinBoundaries.X)
                        {
                            Direction = Direction.Right;
                        }
                    }
                    break;
            }
            Updated?.Invoke(this, EventArgs.Empty);
        }

        private static int RandomDropTime(MineType type, int averageDropTime)
        {
            switch(type)
            {
                case MineType.Small:
                    return _random.Next() % 500 + averageDropTime;
                case MineType.Medium:
                    return _random.Next() % 1000 + averageDropTime;
                case MineType.Heavy:
                    return _random.Next() % 1500 + averageDropTime;
                default: return 0;
            }
        }

        public void BoostShip()
        {
            if (AverageDropTime >= 500)
            {
                AverageDropTime = AverageDropTime - (int)(AverageDropTime * 0.1);
            }
        }

        private void OnDropTimerTick(object? sender, EventArgs e)
        {
            var mineLocation = new Point(Location.X + (Size.Width / 2), Location.Y + Size.Height);
            DropMine?.Invoke(this, new DropMineEventArgs(MineType, mineLocation));
            _dropTimer.Interval = RandomDropTime(MineType, AverageDropTime);
        }

        public event EventHandler? Updated;
        public event EventHandler<DropMineEventArgs>? DropMine;
    }
}
