using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public class MainModel
    {
        public SubmarineModel Player { get; private set; }
        public List<ShipModel> Ships { get; private set; }
        public List<MineModel> Mines { get; private set; }
        private const int SHIP_COUNT = 3;
        public int TimeElapsed { get; private set; }
        private System.Timers.Timer _elapsedTimer;
        private Size _currentSize;
        private bool _paused;
        public bool Paused { 
            get { return _paused; }
            set
            {
                _paused = value;
                GamePaused?.Invoke(this, _paused);
            }
        }

        public MainModel()
        {
            Player = new SubmarineModel();
            Ships = new List<ShipModel>();
            Mines = new List<MineModel>();
            TimeElapsed = 0;
            _elapsedTimer = new System.Timers.Timer(1000);
            _elapsedTimer.Elapsed += OnElapsedTimerTick;
        }

        public void InitObjects(Size size)
        {
            Paused = false;
            _currentSize = size;
            InitPlayerData(size);
            for (int i = 0; i < SHIP_COUNT; ++i)
            {
                InitShipsData(size);
            }
            ObjectsInitialized?.Invoke(this, EventArgs.Empty);
            TimeElapsed = 0;
            _elapsedTimer.Start();
        }

        private void InitPlayerData(Size size)
        {
            Player.Size = new Size(size.Width / 10, size.Width / 50);
            Player.Location = new Point((size.Width / 2) - (Player.Size.Width / 2), size.Height / 2 + size.Height / 8);
            Player.MinBoundaries = new Point(0, size.Height / 6 + size.Width / 24);
            Player.MaxBoundaries = new Point(size.Width, size.Height);
            Player.Speed = (int)(size.Width / 50);
        }

        private void InitShipsData(Size size)
        {
            var random = new Random();
            var ship = new ShipModel();
            ship.Size = new Size(size.Width / 8, size.Width / 24);
            int nextRand = random.Next() % (size.Width + ship.Size.Width);
            ship.Location = new Point(nextRand, size.Height / 6);
            ship.MinBoundaries = new Point(0 - (ship.Size.Width / 2), 0);
            ship.MaxBoundaries = new Point(size.Width + (ship.Size.Width / 2), size.Height);
            ship.Speed = random.Next() % 3 + 1;
            ship.AverageDropTime = 2000;
            ship.DropMine += OnDropMine;
            Ships.Add(ship);
        }

        private void CheckPlayerHit(MineModel mine)
        {
            if (mine != null)
            {
                var mineBoundaries = new Rectangle(mine.Location, mine.Size);
                var playerBoundaries = new Rectangle(Player.Location, Player.Size);
                if (playerBoundaries.IntersectsWith(mineBoundaries))
                {
                    // Player destroyed
                    Stop();
                    PlayerDestroyed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void StopObjects(object? sender, EventArgs e)
        {
            Stop();
        }

        public void StartObjects(object? sender, EventArgs e)
        {
            Start();
        }

        private void Stop()
        {
            if (Ships != null && Mines != null)
            {
                Paused = true;
                _elapsedTimer.Stop();
                foreach (var ship in Ships)
                {
                    ship.Stop();
                }
                foreach (var mine in Mines)
                {
                    mine.Stop();
                }
            }
        }

        private void Start()
        {
            if (Ships != null && Mines != null)
            {
                Paused = false;
                _elapsedTimer.Start();
                foreach (var ship in Ships)
                {
                    ship.Start();
                }
                foreach (var mine in Mines)
                {
                    mine.Start();
                }
            }
        }

        private void OnElapsedTimerTick(object? sender, EventArgs e)
        {
            TimeElapsed++;
            TimerUpdated?.Invoke(this, TimeElapsed.ToString());
            if (TimeElapsed != 0 && TimeElapsed % 30 == 0)
            {
                foreach (var ship in Ships)
                {
                    ship.BoostShip();
                }
            }
        }

        public void RestartRequest(object? sender, EventArgs e)
        {
            DeleteObjects();
            InitObjects(_currentSize);
        }

        public void DeleteObjects()
        {
            Paused = true;
            while (Ships.Count != 0)
            {
                var ship = Ships.First();
                ship.DropMine -= OnDropMine;
                Ships.Remove(ship);
            }
            while (Mines.Count != 0)
            {
                var mine = Mines.First();
                Mines.Remove(mine);
            }
            ObjectsRemoved?.Invoke(this, EventArgs.Empty);
        }

        private void OnDropMine(object? sender, DropMineEventArgs e)
        {

            var mine = InitMine();
            switch (e.MineType)
            {
                case MineType.Small:
                    mine.Size = new Size(_currentSize.Width / 50, _currentSize.Width / 50);
                    mine.Speed = 1;
                    break;
                case MineType.Medium:
                    mine.Size = new Size(_currentSize.Width / 45, _currentSize.Width / 45);
                    mine.Speed = 2;
                    break;
                case MineType.Heavy:
                    mine.Size = new Size(_currentSize.Width / 40, _currentSize.Width / 40);
                    mine.Speed = 3;
                    break;
            }
            mine.Location = e.Location;
            mine.Updated += OnMineModelUpdate;
            mine.OutOfVisibleArea += OnMineOutOfVisibleArea;
            Mines.Add(mine);
            MineCreated?.Invoke(this, mine);
        }

        private MineModel InitMine()
        {
            var mine = new MineModel();
            mine.MinBoundaries = new Point(0 - mine.Size.Width, 0);
            mine.MaxBoundaries = new Point(_currentSize.Width + mine.Size.Width, _currentSize.Height);
            return mine;
        }

        private void OnMineModelUpdate(object? sender, EventArgs e)
        {
            var mine = sender as MineModel;
            if (mine != null)
            {
                CheckPlayerHit(mine);
            }
        }

        private void OnMineOutOfVisibleArea(object? sender, EventArgs e)
        {
            var mine = sender as MineModel;
            if (mine != null)
            {
                if (Mines.Contains(mine))
                {
                    Mines.Remove(mine);
                    MineDestroyed?.Invoke(this, mine);
                }
            }
        }

        public void Save(string path, IFilemanager manager)
        {
            manager.Save(path, this);
        }

        public void Load(string path, IFilemanager manager)
        {
            DeleteObjects();
            SubmarineModel player = new SubmarineModel();
            List<ShipModel> ships = new List<ShipModel>();
            List<MineModel> mines = new List<MineModel>();
            manager.Load(path, out mines, out ships, out player);
            Player = player;
            Ships = ships;
            if (Ships != null)
            {
                foreach (var ship in Ships)
                {
                    ship.DropMine += OnDropMine;
                }
            } 
            Mines = mines;
            Stop();
            ObjectsInitialized?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? PlayerDestroyed;
        public event EventHandler? ObjectsRemoved;
        public event EventHandler? ObjectsInitialized;
        public event EventHandler<MineModel>? MineDestroyed;
        public event EventHandler<MineModel>? MineCreated;
        public event EventHandler<string>? TimerUpdated;
        public event EventHandler<bool>? GamePaused;
    }
}
