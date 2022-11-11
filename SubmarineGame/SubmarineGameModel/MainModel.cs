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
        private SubmarineModel _player;
        private List<ShipModel> _ships;
        private List<MineModel> _mines;
        private Size _currentSize;
        private const int SHIP_COUNT = 3;

        public MainModel()
        {
            _player = new SubmarineModel();
            _ships = GenerateShips();
            _mines = new List<MineModel>();
            _currentSize = new Size();
        }

        public void OnAreaChanged(object? sender, SizeArgs e)
        {
            double factorX = 1;
            double factorY = 1;
            if (_currentSize != new Size())
            {
                factorX = (double)e.Size.Width / (double)_currentSize.Width;
                factorY = (double)e.Size.Height / (double)_currentSize.Height;
            }
            _currentSize = e.Size;
            ChangePlayerData(factorX, factorY);

        }

        private void ChangePlayerData(double factorX, double factorY)
        {
            // Size
            if (_player.Size == new Size())
            {
                _player.Size = new Size(_currentSize.Width / 10, _currentSize.Width / 50);
            }
            else
            {
                _player.Size = new Size((int)(_player.Size.Width * factorX), (int)(_player.Size.Height * factorY));
            }
            // Location
            if (_player.Location == new Point())
            {
                _player.Location = new Point((_currentSize.Width / 2) - (_player.Size.Width / 2),
                    (_currentSize.Height / 2) - (_player.Size.Height / 2));
            }
            else
            {
                _player.Location = new Point((int)(_player.Location.X * factorX), (int)(_player.Location.Y * factorY));
            }
            // MinBoundaries - It's made like this because I don't know yet whether I change on window scale
            if (_player.MinBoundaries == new Point())
            {
                _player.MinBoundaries = new Point(0, 0);
            }
            else
            {
                _player.MinBoundaries = new Point(0, 0);
            }
            // MaxBoundaries
            _player.MaxBoundaries = new Point(_currentSize.Width, _currentSize.Height);
            // Speed
            _player.Speed = (int)(_currentSize.Width / 50);
        }

        private void ChangeShipData(ShipModel ship, double factorX, double factorY)
        {

        }

        private static List<ShipModel> GenerateShips()
        {
            var newShips = new List<ShipModel>();
            for (int i = 0; i < SHIP_COUNT; ++i)
            {
                var ship = new ShipModel();
                newShips.Add(ship);
            }
            return newShips;
        }
    }
}
