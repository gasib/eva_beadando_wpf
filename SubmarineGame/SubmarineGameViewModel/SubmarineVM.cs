using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameViewModel
{
    public class SubmarineVM: ViewModelBase
    {
        private SubmarineGameModel.SubmarineModel _model;
        private int _posx;
        private int _posy;
        private int _sizex;
        private int _sizey;
        public int PosX
        {
            get
            {
                return _posx;
            }
            set
            {
                _posx = value;
                OnPropertyChanged();
            }
        }
        public int PosY
        {
            get
            {
                return _posy;
            }
            set
            {
                _posy = value;
                OnPropertyChanged();
            }
        }

        public int SizeX
        {
            get
            {
                return _sizex;
            }
            set
            {
                _sizex = value;
                OnPropertyChanged();
            }
        }

        public int SizeY
        {
            get
            {
                return _sizey;
            }
            set
            {
                _sizey = value;
                OnPropertyChanged();
            }
        }

        public SubmarineVM(SubmarineGameModel.SubmarineModel model)
        {
            _model = model;
            _model.Updated += OnModelUpdated;
            _sizex = model.Size.Width;
            _sizey = model.Size.Height;
            _posx = model.Location.X;
            _posy = model.Location.Y;
        }

        public void OnModelUpdated(object? sender, EventArgs e)
        {
            PosX = _model.Location.X;
            PosY = _model.Location.Y;
        }
    }
}
