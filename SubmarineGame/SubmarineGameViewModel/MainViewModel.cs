using SubmarineGameModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameViewModel
{
    public class MainViewModel: ViewModelBase
    {
        private SubmarineGameModel.MainModel _model;
        private SubmarineVM _player;
        private List<ShipVM> _ships;
        private List<MineVM> _mines;
        private string _timeElapsedInSeconds;
        public String TimeElapsed 
        {
            get
            {
                return new String("Time elapsed: " + _timeElapsedInSeconds + " seconds.");
            }
            set
            {
                _timeElapsedInSeconds = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel(Size size)
        {
            _model = new SubmarineGameModel.MainModel();

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());

            _model.ObjectsInitialized += OnModelObjectsInitialized;
            _model.ObjectsRemoved += OnModelObjectsRemoved;
            _model.PlayerDestroyed += OnModelPlayerDestroyed;
            _model.MineDestroyed += OnModelMineDestroyed;
            _model.MineCreated += OnMineDropped;
            _model.TimerUpdated += OnModelTimerUpdated;
        }

        private void OnModelTimerUpdated(object? sender, string e)
        {
            TimeElapsed = e;
        }

        public void Init()
        {
            _model.InitObjects(new Size(1024,768));
        }

        public event EventHandler<SubmarineGameViewModel.ShipVM>? ShipRequest;
        public event EventHandler<SubmarineGameViewModel.SubmarineVM>? PlayerRequest;
        public event EventHandler<SubmarineGameViewModel.MineVM>? MineRequest;
        private void OnModelMineDestroyed(object? sender, MineModel e)
        {
            MineVM? vm = _mines.Find((param) => { return param == null ? false : param.Model == e; });
            if (vm != null)
            {
                _mines.Remove(vm);
                MineViewModelDestroyed?.Invoke(this, vm);
            }
        }
        public event EventHandler<MineVM>? MineViewModelDestroyed;
        private void OnModelPlayerDestroyed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnModelObjectsRemoved(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnModelObjectsInitialized(object? sender, EventArgs e)
        {
            _ships = new List<ShipVM>();
            _mines = new List<MineVM>();
            _player = new SubmarineVM(_model.Player);
            PlayerRequest?.Invoke(this, _player);
            foreach(var shipModel in _model.Ships)
            {
                var shipVM = new ShipVM(shipModel);
                ShipRequest?.Invoke(this, shipVM);
                _ships.Add(shipVM);
            }
            foreach(var mineModel in _model.Mines)
            {
                var mineVM = new MineVM(mineModel);
                MineRequest?.Invoke(this, mineVM);
                _mines.Add(mineVM);
            }
        }

        private void OnMineDropped(object? sender, MineModel model)
        {
            var mineVM = new MineVM(model);
            MineRequest?.Invoke(this, mineVM);
            _mines.Add(mineVM);
        }

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }

        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;

        private void OnSaveGame()
        {
            throw new NotImplementedException();
        }

        private void OnLoadGame()
        {
            throw new NotImplementedException();
        }

        private void OnNewGame()
        {
            throw new NotImplementedException();
        }


    }
}
