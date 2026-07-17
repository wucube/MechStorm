using System;
using MechStorm.Battle.Foundation;
using MechStorm.Battle.Rules;
using MechStorm.Battle.Spatial;
using MechStorm.Battle.Units;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Rules
{
    public class MovementResolverTests
    {
        [Test]
        public void TryMoveTo_MovesUnit_WhenTargetIsReachable()
        {
            var resolver = new MovementResolver(new SquareGrid(5, 5));
            var unit = CreateCombatUnit(3, new Vector2Int(1, 1));

            var moved = resolver.TryMoveTo(unit, new Vector2Int(3, 2));

            Assert.IsTrue(moved);
            Assert.AreEqual(new Vector2Int(3, 2), unit.Position);
        }

        [Test]
        public void TryMoveTo_DoesNotMoveUnit_WhenTargetIsOutOfRange()
        {
            var resolver = new MovementResolver(new SquareGrid(5, 5));
            var startPosition = new Vector2Int(1, 1);
            var unit = CreateCombatUnit(2, startPosition);

            var moved = resolver.TryMoveTo(unit, new Vector2Int(4, 4));

            Assert.IsFalse(moved);
            Assert.AreEqual(startPosition, unit.Position);
        }

        [Test]
        public void TryMoveTo_DoesNotMoveUnit_WhenTargetIsOutsideGrid()
        {
            var resolver = new MovementResolver(new SquareGrid(5, 5));
            var startPosition = new Vector2Int(1, 1);
            var unit = CreateCombatUnit(3, startPosition);

            var moved = resolver.TryMoveTo(unit, new Vector2Int(-1, 1));

            Assert.IsFalse(moved);
            Assert.AreEqual(startPosition, unit.Position);
        }

        [Test]
        public void TryMoveTo_ReturnsTrue_WhenTargetIsCurrentPosition()
        {
            var resolver = new MovementResolver(new SquareGrid(5, 5));
            var startPosition = new Vector2Int(1, 1);
            var unit = CreateCombatUnit(3, startPosition);

            var moved = resolver.TryMoveTo(unit, startPosition);

            Assert.IsTrue(moved);
            Assert.AreEqual(startPosition, unit.Position);
        }

        [Test]
        public void CanMoveTo_ReturnsFalse_WhenUnitIsNull()
        {
            var resolver = new MovementResolver(new SquareGrid(5, 5));

            var canMove = resolver.CanMoveTo(null, new Vector2Int(1, 1));

            Assert.IsFalse(canMove);
        }

        [Test]
        public void Constructor_Throws_WhenGridIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MovementResolver(null));
        }

        private static CombatUnit CreateCombatUnit(int moveRange, Vector2Int position)
        {
            var pilot = new PilotData(1, "Test Pilot", 10);
            var pilotRuntime = new PilotRuntime(6);
            var mech = new MechData(101, "Test Mech", 25, 120, moveRange);
            var mechRuntime = new MechRuntime(80);

            return new CombatUnit(1, pilot, pilotRuntime, mech, mechRuntime, position);
        }
    }
}
