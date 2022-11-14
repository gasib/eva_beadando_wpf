using SubmarineGameModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class TextFileManager : IFilemanager
    {
        public TextFileManager()
        {

        }

        private string SaveToString(MineModel mine)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("M").Append(";");
            sb.Append(mine.Size.Width).Append(";");
            sb.Append(mine.Size.Height).Append(";");
            sb.Append(mine.MinBoundaries.X).Append(";");
            sb.Append(mine.MinBoundaries.Y).Append(";");
            sb.Append(mine.MaxBoundaries.X).Append(";");
            sb.Append(mine.MaxBoundaries.Y).Append(";");
            sb.Append(mine.Location.X).Append(";");
            sb.Append(mine.Location.Y).Append(";");
            sb.Append(mine.Speed).Append(";");
            sb.Append(mine.MineType).Append(";");
            return sb.ToString();
        }

        private string SaveToString(ShipModel ship)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("S").Append(";");
            sb.Append(ship.Size.Width).Append(";");
            sb.Append(ship.Size.Height).Append(";");
            sb.Append(ship.MinBoundaries.X).Append(";");
            sb.Append(ship.MinBoundaries.Y).Append(";");
            sb.Append(ship.MaxBoundaries.X).Append(";");
            sb.Append(ship.MaxBoundaries.Y).Append(";");
            sb.Append(ship.Location.X).Append(";");
            sb.Append(ship.Location.Y).Append(";");
            sb.Append(ship.Speed).Append(";");
            sb.Append(ship.Direction).Append(";");
            sb.Append(ship.MineType).Append(";");
            sb.Append(ship.AverageDropTime);
            return sb.ToString();
        }

        public string SaveToString(SubmarineModel player)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("P").Append(";");
            sb.Append(player.Size.Width).Append(";");
            sb.Append(player.Size.Height).Append(";");
            sb.Append(player.MinBoundaries.X).Append(";");
            sb.Append(player.MinBoundaries.Y).Append(";");
            sb.Append(player.MaxBoundaries.X).Append(";");
            sb.Append(player.MaxBoundaries.Y).Append(";");
            sb.Append(player.Location.X).Append(";");
            sb.Append(player.Location.Y).Append(";");
            sb.Append(player.Speed);
            return sb.ToString();
        }

        public void Save(string path, MainModel model)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            try
            {
                using (StreamWriter s = new StreamWriter(path))
                {
                    s.WriteLine(SaveToString(model.Player));
                    foreach (var ship in model.Ships)
                    {
                        s.WriteLine(SaveToString(ship));
                    }
                    foreach (var mine in model.Mines)
                    {
                        s.WriteLine(SaveToString(mine));
                    }
                }
            }
            catch
            {
                throw new DataAccessException("An error occured.");
            }
        }

        public void Load(string path, out List<MineModel> mines, out List<ShipModel> ships, out SubmarineModel player)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            try
            {
                player = new SubmarineModel();
                ships = new List<ShipModel>();
                mines = new List<MineModel>();
                using (StreamReader r = new StreamReader(path))
                {
                    while (!r.EndOfStream)
                    {
                        string? line = r.ReadLine();
                        if (line != null)
                        {
                            string[] data = line.Split(";");
                            switch (data[0])
                            {
                                case "P":
                                    player = ProcessPlayer(data);
                                    break;
                                case "M":
                                    mines.Add(ProcessMine(data));
                                    break;
                                case "S":
                                    ships.Add(ProcessShip(data));
                                    break;
                                default: break;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw new DataException("An error occured.");
            }
        }

        private SubmarineModel ProcessPlayer(string[] data)
        {
            SubmarineModel newSubmarine = new SubmarineModel();
            newSubmarine.Size = new System.Drawing.Size(int.Parse(data[1]), int.Parse(data[2]));
            newSubmarine.MinBoundaries = new System.Drawing.Point(int.Parse(data[3]), int.Parse(data[4]));
            newSubmarine.MaxBoundaries = new System.Drawing.Point(int.Parse(data[5]), int.Parse(data[6]));
            newSubmarine.Location = new System.Drawing.Point(int.Parse(data[7]), int.Parse(data[8]));
            newSubmarine.Speed = int.Parse(data[9]);
            return newSubmarine;
        }

        private ShipModel ProcessShip(string[] data)
        {
            ShipModel newShip = new ShipModel();
            newShip.Size = new System.Drawing.Size(int.Parse(data[1]), int.Parse(data[2]));
            newShip.MinBoundaries = new System.Drawing.Point(int.Parse(data[3]), int.Parse(data[4]));
            newShip.MaxBoundaries = new System.Drawing.Point(int.Parse(data[5]), int.Parse(data[6]));
            newShip.Location = new System.Drawing.Point(int.Parse(data[7]), int.Parse(data[8]));
            newShip.Speed = int.Parse(data[9]);
            newShip.Direction = (Direction)Enum.Parse(typeof(Direction), data[10]);
            newShip.MineType = (MineType)Enum.Parse(typeof(MineType), data[11]);
            newShip.AverageDropTime = int.Parse(data[12]);
            return newShip;
        }

        private MineModel ProcessMine(string[] data)
        {
            MineModel newMine = new MineModel();
            newMine.Size = new System.Drawing.Size(int.Parse(data[1]), int.Parse(data[2]));
            newMine.MinBoundaries = new System.Drawing.Point(int.Parse(data[3]), int.Parse(data[4]));
            newMine.MaxBoundaries = new System.Drawing.Point(int.Parse(data[5]), int.Parse(data[6]));
            newMine.Location = new System.Drawing.Point(int.Parse(data[7]), int.Parse(data[8]));
            newMine.Speed = int.Parse(data[9]);
            newMine.MineType = (MineType)Enum.Parse(typeof(MineType), data[10]);
            return newMine;
        }
    }
}
