using System;
using MechStorm.Battle.Foundation;
using MechStorm.Battle.Units;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Spatial
{
    public class BattleOccupancyTests
    {
        [Test]
        public void PositionQueries_ReturnAliveUnitsAcrossFactionsAndEmptyForVacantCell()
        {
            var teamAUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var teamBUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            var neutralUnit = CreateCombatUnit(3, new Vector2Int(2, 3));
            var session = new BattleSession(
                5,
                5,
                new[] { teamAUnit },
                new[] { teamBUnit },
                new[] { neutralUnit });

            Assert.IsTrue(session.TryGetAliveCombatUnitAt(teamAUnit.Position, out var foundTeamAUnit));
            Assert.AreSame(teamAUnit, foundTeamAUnit);
            Assert.IsTrue(session.IsPositionOccupied(teamAUnit.Position));

            Assert.IsTrue(session.TryGetAliveCombatUnitAt(teamBUnit.Position, out var foundTeamBUnit));
            Assert.AreSame(teamBUnit, foundTeamBUnit);

            Assert.IsTrue(session.TryGetAliveCombatUnitAt(neutralUnit.Position, out var foundNeutralUnit));
            Assert.AreSame(neutralUnit, foundNeutralUnit);

            var vacantPosition = new Vector2Int(4, 4);
            Assert.IsFalse(session.TryGetAliveCombatUnitAt(vacantPosition, out var vacantUnit));
            Assert.IsNull(vacantUnit);
            Assert.IsFalse(session.IsPositionOccupied(vacantPosition));
        }

        [Test]
        public void Constructor_WithOverlappingAliveUnits_Throws()
        {
            var sharedPosition = new Vector2Int(1, 1);
            var teamAUnit = CreateCombatUnit(1, sharedPosition);
            var teamBUnit = CreateCombatUnit(2, sharedPosition);

            Assert.Throws<ArgumentException>(() =>
                CreateBattleSession(teamAUnit, teamBUnit));
        }

        [Test]
        public void PositionQueries_ReleasePosition_WhenUnitDies()
        {
            var teamAUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var teamBUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            var session = CreateBattleSession(teamAUnit, teamBUnit);

            Assert.IsTrue(session.IsPositionOccupied(teamBUnit.Position));

            teamBUnit.MechRuntime.TakeDamage(teamBUnit.Mech.MaxDurability);

            Assert.IsFalse(session.TryGetAliveCombatUnitAt(teamBUnit.Position, out var combatUnit));
            Assert.IsNull(combatUnit);
            Assert.IsFalse(session.IsPositionOccupied(teamBUnit.Position));
        }

        [Test]
        public void Constructor_AllowsDeadUnitToSharePositionWithAliveUnit()
        {
            var sharedPosition = new Vector2Int(1, 1);
            var teamAUnit = CreateCombatUnit(1, sharedPosition);
            var deadTeamBUnit = CreateCombatUnit(2, sharedPosition);
            deadTeamBUnit.MechRuntime.TakeDamage(deadTeamBUnit.Mech.MaxDurability);

            var session = CreateBattleSession(teamAUnit, deadTeamBUnit);

            Assert.IsTrue(session.TryGetAliveCombatUnitAt(sharedPosition, out var combatUnit));
            Assert.AreSame(teamAUnit, combatUnit);
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(5, 0)]
        [TestCase(0, 5)]
        public void PositionQueries_WithPositionOutsideGrid_Throw(int x, int y)
        {
            var teamAUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var teamBUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            var session = CreateBattleSession(teamAUnit, teamBUnit);
            var outsidePosition = new Vector2Int(x, y);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                session.TryGetAliveCombatUnitAt(outsidePosition, out _));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                session.IsPositionOccupied(outsidePosition));
        }

        private static BattleSession CreateBattleSession(CombatUnit teamAUnit, CombatUnit teamBUnit)
        {
            return new BattleSession(5, 5, new[] { teamAUnit }, new[] { teamBUnit });
        }

        private static CombatUnit CreateCombatUnit(int unitId, Vector2Int position)
        {
            var pilot = new PilotData(unitId, $"Pilot {unitId}", 10);
            var mech = new MechData(unitId, $"Mech {unitId}", 10, 100, 3);
            return new CombatUnitFactory().Create(unitId, pilot, mech, position);
        }
    }
}
