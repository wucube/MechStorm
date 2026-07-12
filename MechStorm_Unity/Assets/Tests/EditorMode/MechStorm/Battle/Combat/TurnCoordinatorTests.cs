using System;
using MechStorm.Battle.Combat;
using MechStorm.Battle.Foundation;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Combat
{
    public class TurnCoordinatorTests
    {
        [Test]
        public void Constructor_StartsFromTeamAAndRoundOne()
        {
            var teamAUnit = CreateCombatUnit(1);
            var teamBUnit = CreateCombatUnit(2);
            var coordinator = CreateTurnCoordinator(
                new[] { teamAUnit },
                new[] { teamBUnit });

            Assert.AreEqual(CombatFaction.TeamA, coordinator.CurrentFaction);
            Assert.AreSame(teamAUnit, coordinator.CurrentCombatUnit);
            Assert.AreEqual(1, coordinator.CurrentRoundNumber);
        }

        [Test]
        public void EndCurrentUnitAction_AdvancesToNextAliveFactionUnit()
        {
            var firstTeamAUnit = CreateCombatUnit(1);
            var deadTeamAUnit = CreateCombatUnit(2);
            var nextTeamAUnit = CreateCombatUnit(3);
            var teamBUnit = CreateCombatUnit(4);
            deadTeamAUnit.MechRuntime.TakeDamage(
                deadTeamAUnit.Mech.MaxDurability);
            var coordinator = CreateTurnCoordinator(
                new[] { firstTeamAUnit, deadTeamAUnit, nextTeamAUnit },
                new[] { teamBUnit });

            coordinator.EndCurrentUnitAction();

            Assert.AreEqual(CombatFaction.TeamA, coordinator.CurrentFaction);
            Assert.AreSame(nextTeamAUnit, coordinator.CurrentCombatUnit);
        }

        [Test]
        public void EndCurrentUnitAction_AfterFactionCompletes_SwitchesFaction()
        {
            var teamAUnit = CreateCombatUnit(1);
            var teamBUnit = CreateCombatUnit(2);
            var coordinator = CreateTurnCoordinator(
                new[] { teamAUnit },
                new[] { teamBUnit });

            coordinator.EndCurrentUnitAction();

            Assert.AreEqual(CombatFaction.TeamB, coordinator.CurrentFaction);
            Assert.AreSame(teamBUnit, coordinator.CurrentCombatUnit);
            Assert.AreEqual(1, coordinator.CurrentRoundNumber);
        }

        [Test]
        public void EndCurrentUnitAction_AfterFullRound_ResetsTeamAProgress()
        {
            var firstTeamAUnit = CreateCombatUnit(1);
            var secondTeamAUnit = CreateCombatUnit(2);
            var teamBUnit = CreateCombatUnit(3);
            var coordinator = CreateTurnCoordinator(
                new[] { firstTeamAUnit, secondTeamAUnit },
                new[] { teamBUnit });

            coordinator.EndCurrentUnitAction();
            coordinator.EndCurrentUnitAction();
            coordinator.EndCurrentUnitAction();

            Assert.AreEqual(CombatFaction.TeamA, coordinator.CurrentFaction);
            Assert.AreSame(firstTeamAUnit, coordinator.CurrentCombatUnit);
            Assert.AreEqual(2, coordinator.CurrentRoundNumber);
        }

        [Test]
        public void EndCurrentUnitAction_DoesNotAddNeutralToTurnOrder()
        {
            var teamAUnit = CreateCombatUnit(1);
            var teamBUnit = CreateCombatUnit(2);
            var neutralUnit = CreateCombatUnit(3);
            var registry = new CombatUnitRegistry(
                new[] { teamAUnit },
                new[] { teamBUnit },
                new[] { neutralUnit });
            var coordinator = new TurnCoordinator(registry);

            coordinator.EndCurrentUnitAction();
            coordinator.EndCurrentUnitAction();

            Assert.AreEqual(CombatFaction.TeamA, coordinator.CurrentFaction);
            Assert.AreSame(teamAUnit, coordinator.CurrentCombatUnit);
        }

        [Test]
        public void Constructor_WithNullRegistry_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TurnCoordinator(null));
        }

        private static TurnCoordinator CreateTurnCoordinator(
            CombatUnit[] teamAUnits,
            CombatUnit[] teamBUnits)
        {
            var registry = new CombatUnitRegistry(teamAUnits, teamBUnits);
            return new TurnCoordinator(registry);
        }

        private static CombatUnit CreateCombatUnit(int id)
        {
            var pilot = new PilotData(id, $"Pilot {id}", 10);
            var mech = new MechData(id, $"Mech {id}", 10, 100, 3);
            var factory = new CombatUnitFactory();
            return factory.Create(pilot, mech, new Vector2Int(id, 0));
        }
    }
}
