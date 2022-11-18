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
        private bool _modelPaused;
        public bool Paused
        {
            get { return _modelPaused; }
            set
            {
                _modelPaused = value;
                OnPropertyChanged();
            }
        }
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
            MoveCommand = new DelegateCommand(param => OnMove(param), (param) => { return !Paused; });
            PauseCommand = new DelegateCommand(param => OnPause());

            _model.ObjectsInitialized += OnModelObjectsInitialized;
            _model.ObjectsRemoved += OnModelObjectsRemoved;
            _model.PlayerDestroyed += OnModelPlayerDestroyed;
            _model.MineDestroyed += OnModelMineDestroyed;
            _model.MineCreated += OnMineDropped;
            _model.TimerUpdated += OnModelTimerUpdated;
            _model.GamePaused += OnModelGamePaused;
        }

        private void OnPause()
        {
            switch (Paused)
            {
                case false:
                    _model.StopObjects(this, EventArgs.Empty);
                    break;
                case true:
                    _model.StartObjects(this, EventArgs.Empty);
                    break;
            }
        }

        private void OnMove(object? param)
        {
            string? s = param?.ToString();
            if (s != null)
            {
                switch ((Direction)Enum.Parse(typeof(Direction), s))
                {
                    case Direction.Up:
                        _model.Player.Move(Direction.Up);
                        break;
                    case Direction.Down:
                        _model.Player.Move(Direction.Down);
                        break;
                    case Direction.Left:
                        _model.Player.Move(Direction.Left);
                        break;
                    case Direction.Right:
                        _model.Player.Move(Direction.Right);
                        break;
                }
            }
        }

        private void OnModelGamePaused(object? sender, bool e)
        {
            Paused = e;
        }

        private void OnModelTimerUpdated(object? sender, string e)
        {
            TimeElapsed = e;
        }

        public void Init(Size size)
        {
            _model.InitObjects(size);
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
        public event EventHandler<string>? PlayerDestroyed;
        private void OnModelPlayerDestroyed(object? sender, EventArgs e)
        {
            PlayerDestroyed?.Invoke(this, TimeElapsed);
        }

        private void OnModelObjectsRemoved(object? sender, EventArgs e)
        {
            _player = null;
            _ships.Clear();
            _mines.Clear();
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
        public DelegateCommand MoveCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }

        public event EventHandler? RestartRequest;
        public event EventHandler? LoadRequest;
        public event EventHandler? SaveRequest;
        public event EventHandler? DeleteViewRequest;

        private void OnSaveGame()
        {
            SaveRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewGame()
        {
            RestartRequest?.Invoke(this, EventArgs.Empty);
        }

        public void OnViewObjectsDeleted(object? sender, EventArgs e)
        {
            _model.DeleteObjects();
        }

        public void OnRestart()
        {
            _model.RestartRequest(this, EventArgs.Empty);
        }

        public void Save(string fileName)
        {
            if (fileName == null)
                return;

            try
            {
                IFilemanager fileman = new Persistence.TextFileManager();
                _model.Save(fileName, fileman);
            } 
            catch (Exception e)
            {
                FileErrorOccured?.Invoke(this, "An Error occured.\n" + e.Message);
            }
        }

        public void Load(string fileName)
        {
            if (fileName == null)
                return;

            try
            {
                DeleteViewRequest?.Invoke(this, EventArgs.Empty);
                IFilemanager fileman = new Persistence.TextFileManager();
                _model.Load(fileName, fileman);
            }
            catch (Exception e)
            {
                RestartRequest?.Invoke(this, EventArgs.Empty);
                FileErrorOccured?.Invoke(this, "An Error occured.\n" + e.Message);
            }
        }

        public event EventHandler<string>? FileErrorOccured;
    }
}
