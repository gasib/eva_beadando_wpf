using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubmarineGameModel
{
    public interface IFilemanager
    {
        public void Save(string path);
        public void Load();
    }
}
