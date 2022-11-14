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

        public MainModel()
        {
            Player = new SubmarineModel();
            Ships = new List<ShipModel>();
            Mines = new List<MineModel>();
            TimeElapsed = 0;
            _elapsedTimer = new System.Timers.Timer(1000);
            _elapsedTimer.Elapsed += OnElapsedTimerTick;
        }

        public void InitObjects(object? sender, SizeArgs e)
        {
            _currentSize = e.Size;
            InitPlayerData(e.Size);
            InitShipsData(e.Size);
            ObjectsInitialized?.Invoke(this, EventArgs.Empty);
            TimeElapsed = 0;
            _elapsedTimer.Start();
        }

        private void InitPlayerData(Size size)
        {
            Player.Size = new Size(size.Width / 10, size.Width / 50);
            Player.Location = new Point((size.Width / 2) - (Player.Size.Width / 2));
            Player.MinBoundaries = new Point(0, 0);
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
            ship.MinBoundaries = new Point(size.Width - (ship.Size.Width / 2), 0);
            ship.MaxBoundaries = new Point(size.Width + (ship.Size.Width / 2), size.Height);
            ship.Speed = 15;
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

        private void Start()
        {
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

        private void OnElapsedTimerTick(object? sender, EventArgs e)
        {
            TimeElapsed++;
        }

        public void RestartRequest(object? sender, EventArgs e)
        {
            DeleteObjects();
            InitObjects(this, new SizeArgs(_currentSize));
        }

        private void DeleteObjects()
        {
            while (Ships.Count != 0)
            {
                var ship = Ships.First();
                Ships.Remove(ship);
            }
            while (Mines.Count != 0)
            {
                var mine = Mines.First();
                Mines.Remove(mine);
            }
            ObjectsRemoved?.Invoke(this, EventArgs.Empty);
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
                Mines.Remove(mine);
                MineDestroyed?.Invoke(this, mine);
            }
        }

        public void Save(string path, IFilemanager manager)
        {
            manager.Save(path, this);
        }

        public void Load(string path, IFilemanager manager)
        {
            SubmarineModel player = new SubmarineModel();
            List<ShipModel> ships = new List<ShipModel>();
            List<MineModel> mines = new List<MineModel>();
            manager.Load(path, out mines, out ships, out player);
            Player = player;
            DeleteObjects();
            Ships = ships;
            Mines = mines;
            ObjectsInitialized?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? PlayerDestroyed;
        public event EventHandler? ObjectsRemoved;
        public event EventHandler? ObjectsInitialized;
        public event EventHandler<MineModel>? MineDestroyed;
    }
}
