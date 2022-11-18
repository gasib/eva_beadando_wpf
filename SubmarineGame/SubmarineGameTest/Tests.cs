using SubmarineGameModel;
using System.Drawing;
using Moq;

namespace SubmarineGameTest
{
    [TestClass]
    public class SubmarineModelTests
    {

        [TestMethod]
        public void TestCtorWithoutArgs()
        {
            SubmarineModel model = new SubmarineModel();
            Assert.AreEqual(new Size(), model.Size);
            Assert.AreEqual(new Point(), model.Location);
            Assert.AreEqual(new Point(), model.MinBoundaries);
            Assert.AreEqual(new Point(), model.MaxBoundaries);
            Assert.AreEqual(0, model.Speed);
        }

        [TestMethod]
        public void TestCtorWithArgs()
        {
            var expSize = new Size(10, 10);
            var expLocation = new Point(10, 10);
            var expMinBounds = new Point(11, 11);
            var expMaxBounds = new Point(12, 12);
            var expSpeed = 15;
            SubmarineModel model = new SubmarineModel(expSize, expLocation, expMinBounds, expMaxBounds, expSpeed);
            Assert.AreEqual(expSize, model.Size);
            Assert.AreEqual(expLocation, model.Location);
            Assert.AreEqual(expMinBounds, model.MinBoundaries);
            Assert.AreEqual(expMaxBounds, model.MaxBoundaries);
            Assert.AreEqual(expSpeed, model.Speed);
        }

        [TestMethod]
        [DataRow(Direction.Up, true, false, false, false)]
        [DataRow(Direction.Down, false, false, true, false)]
        [DataRow(Direction.Left, false, false, false, true)]
        [DataRow(Direction.Right, false, true, false, false)]
        public void TestNormalMove(Direction dir, bool movedUp, bool movedRight, bool movedDown, bool movedLeft)
        {
            var size = new Size(5, 5);
            var location = new Point(10, 10);
            var minBounds = new Point(0, 0);
            var maxBounds = new Point(20, 20);
            var speed = 1;
            SubmarineModel model = new SubmarineModel(size, location, minBounds, maxBounds, speed);
            var initLocation = model.Location;
            model.Move(dir);
            Assert.AreEqual(movedUp, initLocation.Y > model.Location.Y);
            Assert.AreEqual(movedRight, initLocation.X < model.Location.X);
            Assert.AreEqual(movedDown, initLocation.X < model.Location.Y);
            Assert.AreEqual(movedLeft, initLocation.X > model.Location.X);
        }

        [TestMethod]
        [DataRow(Direction.Up, 0, 0)]
        [DataRow(Direction.Down, 20, 20)]
        [DataRow(Direction.Left, 0, 0)]
        [DataRow(Direction.Right, 20, 20)]
        public void TestEdgeMove(Direction dir, int locx, int locy)
        {
            var size = new Size(0, 0);
            var location = new Point(locx, locy);
            var minBounds = new Point(0, 0);
            var maxBounds = new Point(20, 20);
            var speed = 1;
            SubmarineModel model = new SubmarineModel(size, location, minBounds, maxBounds, speed);
            var initLocation = model.Location;
            model.Move(dir);
            Assert.AreEqual(initLocation, model.Location);
        }
    }

    [TestClass]
    public class ShipModelTests
    {
        [TestMethod]
        public void TestCtorWithoutArgs()
        {
            ShipModel model = new ShipModel();
            Assert.AreEqual(new Size(), model.Size);
            Assert.AreEqual(new Point(), model.Location);
            Assert.AreEqual(new Point(), model.MinBoundaries);
            Assert.AreEqual(new Point(), model.MaxBoundaries);
            Assert.AreEqual(0, model.Speed);
            Assert.AreEqual(0, model.AverageDropTime);
            Assert.IsNotNull(model.Direction);
            Assert.IsNotNull(model.MineType);
        }

        [TestMethod]
        public void TestCtorWithArgs()
        {
            var expSize = new Size(10, 10);
            var expLocation = new Point(10, 10);
            var expMinBounds = new Point(11, 11);
            var expMaxBounds = new Point(12, 12);
            var expSpeed = 15;
            var expDirection = Direction.Left;
            var expMineType = MineType.Small;
            var expAvgDropTime = 14;
            ShipModel model = new ShipModel(expSize, expLocation, expMinBounds, expMaxBounds, expSpeed, expDirection, expMineType, expAvgDropTime);
            Assert.AreEqual(expSize, model.Size);
            Assert.AreEqual(expLocation, model.Location);
            Assert.AreEqual(expMinBounds, model.MinBoundaries);
            Assert.AreEqual(expMaxBounds, model.MaxBoundaries);
            Assert.AreEqual(expSpeed, model.Speed);
            Assert.AreEqual(expDirection, model.Direction);
            Assert.AreEqual(expMineType, model.MineType);
            Assert.AreEqual(expAvgDropTime, model.AverageDropTime);
        }

        [TestMethod]
        public void TestBoost()
        {
            ShipModel model = new ShipModel();
            int initAverageDropTime = 500;
            model.AverageDropTime = initAverageDropTime;
            Assert.AreEqual(initAverageDropTime, model.AverageDropTime);
            model.BoostShip();
            int secondAvgDropTime = model.AverageDropTime;
            Assert.AreNotEqual(initAverageDropTime, model.AverageDropTime);
            model.BoostShip();
            Assert.AreEqual(secondAvgDropTime, model.AverageDropTime);
        }
    }

    [TestClass]
    public class TestMainModel
    {
        private Mock<IFilemanager> _mock = null;
        [TestMethod]
        public void TestCtor()
        {
            var model = new MainModel();
            Assert.IsTrue(model.Player != null);
            Assert.IsTrue(model.Ships != null);
            Assert.IsTrue(model.Mines != null);
        }

        [TestMethod]
        public void TestInit()
        {
            Size s = new Size(10,10);
            var model = new MainModel();
            model.InitObjects(s);
            Assert.IsTrue(model.Player != null);
            Assert.IsTrue(model.Ships.Count != 0);
            Assert.IsTrue(model.Paused == false);
        }

        [TestMethod]
        public void TestDeleteObjects()
        {
            var model = new MainModel();
            model.InitObjects(new Size(10,10));
            model.DeleteObjects();
            Assert.IsTrue(model.Ships.Count == 0);
            Assert.IsTrue(model.Mines.Count == 0);
        }

        [TestMethod]
        public void TestLoad()
        {
            var model = new MainModel();
            _mock = new Mock<IFilemanager>();
            SubmarineModel t1 = new SubmarineModel();
            List<ShipModel> t2 = new List<ShipModel>();
            List<MineModel> t3 = new List<MineModel>();
            var mm = It.IsAny<List<MineModel>>();
            var sm = It.IsAny<List<ShipModel>>();
            var pm = It.IsAny<SubmarineModel>();
            _mock.Setup(p => p.Load(It.IsAny<string>(), out mm, out sm, out pm));

            model.Load(String.Empty, _mock.Object);
            _mock.Verify(p => p.Load(String.Empty, out t3, out t2, out t1));
        }
    }
}