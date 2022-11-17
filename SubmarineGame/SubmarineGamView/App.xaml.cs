using SubmarineGameView;
using SubmarineGameViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SubmarineGamView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SubmarineGameViewModel.MainViewModel _mainModel;
        private SubmarineGameView.GameWindow _view;
        private Canvas _canvas;
        private List<SubmarineGameView.Ship> _ships;
        private SubmarineGameView.Submarine _player;
        private List<SubmarineGameView.Mine> _mines;
        public void OnStartup(object? sender, EventArgs e)
        {
            _view = new SubmarineGameView.GameWindow();

            var grid = _view.Content as Grid;
            _canvas = FirstCanvas(grid);
            if (_canvas == null)
            {
                throw new Exception();
            }

            _mainModel = new SubmarineGameViewModel.MainViewModel(new System.Drawing.Size((int)_canvas.ActualWidth, (int)_canvas.ActualHeight));
            _view.DataContext = _mainModel;
            
            _view.Show();

            _mainModel.ShipRequest += OnShipRequest;
            _mainModel.PlayerRequest += OnPlayerRequest;
            _mainModel.MineRequest += OnMineRequest;
            _mainModel.MineViewModelDestroyed += OnMineVMDestroyed;

            _mainModel.Init();
        }

        public App()
        {
            Startup += OnStartup;
            _ships = new List<Ship>();
            _mines = new List<Mine>();
        }

        public void OnShipRequest(object? sender, SubmarineGameViewModel.ShipVM model)
        {
            var shipview = new SubmarineGameView.Ship();
            Binding bindPosX = new Binding("PosX");
            Binding bindPosY = new Binding("PosY");
            Binding bindSizeX = new Binding("SizeX");
            Binding bindSizeY = new Binding("SizeY");
            shipview.DataContext = model;
            shipview.SetBinding(FrameworkElement.WidthProperty, bindSizeX);
            shipview.SetBinding(FrameworkElement.HeightProperty, bindSizeY);
            shipview.SetBinding(Canvas.LeftProperty, bindPosX);
            shipview.SetBinding(Canvas.TopProperty, bindPosY);
            _canvas.Children.Add(shipview);
            _ships.Add(shipview);
        }

        public void OnPlayerRequest(object? sender, SubmarineGameViewModel.SubmarineVM model)
        {
            var player = new SubmarineGameView.Submarine();
            Binding bindPosX = new Binding("PosX");
            Binding bindPosY = new Binding("PosY");
            Binding bindSizeX = new Binding("SizeX");
            Binding bindSizeY = new Binding("SizeY");
            player.DataContext = model;
            player.SetBinding(FrameworkElement.WidthProperty, bindSizeX);
            player.SetBinding(FrameworkElement.HeightProperty, bindSizeY);
            player.SetBinding(Canvas.LeftProperty, bindPosX);
            player.SetBinding(Canvas.TopProperty, bindPosY);
            _canvas.Children.Add(player);
            _player = player;
        }

        public void OnMineRequest(object? sender, SubmarineGameViewModel.MineVM model)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var mine = new SubmarineGameView.Mine();
                Binding bindPosX = new Binding("PosX");
                Binding bindPosY = new Binding("PosY");
                Binding bindSizeX = new Binding("SizeX");
                Binding bindSizeY = new Binding("SizeY");
                mine.DataContext = model;
                mine.SetBinding(FrameworkElement.WidthProperty, bindSizeX);
                mine.SetBinding(FrameworkElement.HeightProperty, bindSizeY);
                mine.SetBinding(Canvas.LeftProperty, bindPosX);
                mine.SetBinding(Canvas.TopProperty, bindPosY);
                _canvas.Children.Add(mine);
                _mines.Add(mine);
            });
        }

        public void OnMineVMDestroyed(object? sender, SubmarineGameViewModel.MineVM model)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Mine? mine = _mines.Find((param) => { return param.DataContext == model; });
                if (mine != null)
                {
                    _mines.Remove(mine);
                    _canvas.Children.Remove(mine);
                }
            });
        }

        public Canvas? FirstCanvas(Grid g)
        {
            foreach (var child in g.Children)
            {
                var canvas = child as Canvas;
                if (canvas != null)
                {
                    return canvas;
                }
            }
            return null;
        }
    }
}
