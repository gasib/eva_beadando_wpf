using SubmarineGameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public interface IFilemanager
    {
        public void Save(string path, MainModel model);
        public void Load(string path, out List<MineModel> mines, out List<ShipModel> ships, out SubmarineModel player);
    }
}
