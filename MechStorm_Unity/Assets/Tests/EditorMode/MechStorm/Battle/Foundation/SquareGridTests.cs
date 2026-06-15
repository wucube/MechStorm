using System.Linq;
using MechStorm.Battle.Foundation;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Foundation
{
    public class SquareGridTests
    {
        [Test]
        public void Constructor_ThrowsException_WhenSizeIsInvalid()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new SquareGrid(0, 10));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new SquareGrid(10, 0));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new SquareGrid(-1, 10));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new SquareGrid(10, -1));
        }

        [Test]
        public void IsInside_ReturnsTrue_WhenPositionIsInside()
        {
            var grid = new SquareGrid(10, 10);

            Assert.IsTrue(grid.IsInside(new Vector2Int(0, 0)));
            Assert.IsTrue(grid.IsInside(new Vector2Int(9, 9)));
            Assert.IsTrue(grid.IsInside(new Vector2Int(5, 5)));
        }

        [Test]
        public void IsInside_ReturnsFalse_WhenPositionIsOutside()
        {
            var grid = new SquareGrid(10, 10);

            Assert.IsFalse(grid.IsInside(new Vector2Int(-1, 0)));
            Assert.IsFalse(grid.IsInside(new Vector2Int(0, -1)));
            Assert.IsFalse(grid.IsInside(new Vector2Int(10, 0)));
            Assert.IsFalse(grid.IsInside(new Vector2Int(0, 10)));
        }

        [Test]
        public void GetFourDirectionNeighbors_ReturnsFourNeighbors_WhenPositionIsCenter()
        {
            var grid = new SquareGrid(10, 10);

            var neighbors = grid.GetFourDirectionNeighbors(new Vector2Int(5, 5));

            Assert.AreEqual(4, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(new Vector2Int(5, 6)));
            Assert.IsTrue(neighbors.Contains(new Vector2Int(5, 4)));
            Assert.IsTrue(neighbors.Contains(new Vector2Int(4, 5)));
            Assert.IsTrue(neighbors.Contains(new Vector2Int(6, 5)));
        }

        [Test]
        public void GetFourDirectionNeighbors_ReturnsTwoNeighbors_WhenPositionIsCorner()
        {
            var grid = new SquareGrid(10, 10);

            var neighbors = grid.GetFourDirectionNeighbors(new Vector2Int(0, 0));

            Assert.AreEqual(2, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(new Vector2Int(0, 1)));
            Assert.IsTrue(neighbors.Contains(new Vector2Int(1, 0)));
        }

        [Test]
        public void GetFourDirectionNeighbors_ReturnsThreeNeighbors_WhenPositionIsEdge()
        {
            var grid = new SquareGrid(10, 10);

            var neighbors = grid.GetFourDirectionNeighbors(new Vector2Int(0, 5));

            Assert.AreEqual(3, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(new Vector2Int(0, 6)));
            Assert.IsTrue(neighbors.Contains(new Vector2Int(0, 4)));
            Assert.IsTrue(neighbors.Contains(new Vector2Int(1, 5)));
        }

        [Test]
        public void GetManhattanDistance_ReturnsAbsoluteDistance()
        {
            var grid = new SquareGrid(10, 10);

            Assert.AreEqual(7, grid.GetManhattanDistance(new Vector2Int(0, 0), new Vector2Int(3, 4)));
            Assert.AreEqual(7, grid.GetManhattanDistance(new Vector2Int(3, 4), new Vector2Int(0, 0)));
        }
    }
}
