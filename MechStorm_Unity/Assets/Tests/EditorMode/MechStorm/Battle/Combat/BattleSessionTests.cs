using System;
using MechStorm.Battle.Combat;
using MechStorm.Battle.Foundation;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Combat
{
    public class BattleSessionTests
    {
        [Test]
        public void Constructor_InitializesBattleStateAndUnits()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(3, 1));

            var session = CreateBattleSession(playerUnit, enemyUnit);

            Assert.AreEqual(5, session.Grid.Width);
            Assert.AreEqual(5, session.Grid.Height);
            Assert.AreEqual(2, session.CombatUnits.Count);
            Assert.AreSame(playerUnit, session.CurrentCombatUnit);
            Assert.AreEqual(CombatFaction.TeamA, session.CurrentFaction);
            Assert.AreEqual(1, session.CurrentRoundNumber);
        }

        [Test]
        public void Constructor_WithEmptyUnitList_Throws()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(3, 1));

            Assert.Throws<ArgumentException>(() =>
                CreateBattleSession(Array.Empty<CombatUnit>(), new[] { enemyUnit }));
            Assert.Throws<ArgumentException>(() =>
                CreateBattleSession(new[] { playerUnit }, Array.Empty<CombatUnit>()));
        }

        [Test]
        public void Constructor_WithUnitOutsideGrid_Throws()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(-1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(3, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                CreateBattleSession(playerUnit, enemyUnit));
        }

        [Test]
        public void Constructor_WithDuplicateUnit_Throws()
        {
            var sharedUnit = CreateCombatUnit(1, new Vector2Int(1, 1));

            Assert.Throws<ArgumentException>(() =>
                CreateBattleSession(sharedUnit, sharedUnit));
        }

        [Test]
        public void TryMoveCurrentCombatUnit_WithReachableTarget_MovesPlayerUnit()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1), moveRange: 3);
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(4, 4));
            var session = CreateBattleSession(playerUnit, enemyUnit);
            var target = new Vector2Int(2, 2);

            var moved = session.TryMoveCurrentCombatUnit(target);

            Assert.IsTrue(moved);
            Assert.AreEqual(target, playerUnit.Position);
        }

        [Test]
        public void AttackTargetCombatUnit_WithAdjacentTarget_AppliesDamage()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1), attack: 25);
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(2, 1), durability: 80);
            var session = CreateBattleSession(playerUnit, enemyUnit);

            session.AttackTargetCombatUnit(enemyUnit);

            Assert.AreEqual(55, enemyUnit.MechRuntime.CurrentDurability);
        }

        [Test]
        public void AttackTargetCombatUnit_WithUnknownTarget_Throws()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(2, 1));
            var unknownUnit = CreateCombatUnit(3, new Vector2Int(1, 2));
            var session = CreateBattleSession(playerUnit, enemyUnit);

            Assert.Throws<ArgumentException>(() =>
                session.AttackTargetCombatUnit(unknownUnit));
        }

        [Test]
        public void EndCurrentUnitAction_AdvancesUnitsTurnsAndRounds()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            var session = CreateBattleSession(playerUnit, enemyUnit);

            session.EndCurrentUnitAction();

            Assert.AreSame(enemyUnit, session.CurrentCombatUnit);
            Assert.AreEqual(CombatFaction.TeamB, session.CurrentFaction);
            Assert.AreEqual(1, session.CurrentRoundNumber);

            session.EndCurrentUnitAction();

            Assert.AreSame(playerUnit, session.CurrentCombatUnit);
            Assert.AreEqual(CombatFaction.TeamA, session.CurrentFaction);
            Assert.AreEqual(2, session.CurrentRoundNumber);
        }

        [Test]
        public void GetCurrentFactionCombatUnits_ReturnsUnitsForCurrentFaction()
        {
            var playerA = CreateCombatUnit(1, new Vector2Int(1, 1));
            var playerB = CreateCombatUnit(2, new Vector2Int(1, 2));
            var enemyUnit = CreateCombatUnit(3, new Vector2Int(3, 1));
            var session = CreateBattleSession(
                new[] { playerA, playerB },
                new[] { enemyUnit });

            var playerUnits = session.GetCurrentFactionCombatUnits();

            CollectionAssert.AreEqual(new[] { playerA, playerB }, playerUnits);
            CollectionAssert.AreEqual(
                new[] { enemyUnit },
                session.GetFactionCombatUnits(CombatFaction.TeamB));

            session.EndCurrentUnitAction();
            session.EndCurrentUnitAction();

            CollectionAssert.AreEqual(
                new[] { enemyUnit },
                session.GetCurrentFactionCombatUnits());
        }

        [Test]
        public void AliveAndDeadQueries_ReturnExpectedUnits()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            enemyUnit.MechRuntime.TakeDamage(enemyUnit.Mech.MaxDurability);
            var session = CreateBattleSession(playerUnit, enemyUnit);

            CollectionAssert.AreEqual(
                new[] { playerUnit },
                session.GetAllAliveCombatUnits());
            CollectionAssert.AreEqual(
                new[] { enemyUnit },
                session.GetAllDeadCombatUnits());
            Assert.IsFalse(session.AreFactionUnitsDead(CombatFaction.TeamA));
            Assert.IsTrue(session.AreFactionUnitsDead(CombatFaction.TeamB));
        }

        [Test]
        public void FactionQueries_IncludeNeutralUnits()
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

            Assert.AreEqual(3, session.CombatUnits.Count);
            CollectionAssert.AreEqual(
                new[] { neutralUnit },
                session.GetFactionCombatUnits(CombatFaction.Neutral));
            Assert.AreEqual(
                CombatFaction.Neutral,
                session.GetCombatUnitFaction(neutralUnit));
            Assert.IsFalse(session.AreFactionUnitsDead(CombatFaction.Neutral));

            neutralUnit.MechRuntime.TakeDamage(neutralUnit.Mech.MaxDurability);

            Assert.IsTrue(session.AreFactionUnitsDead(CombatFaction.Neutral));
        }

        [Test]
        public void CombatUnitRegistry_ProvidesFactionAndMembershipQueries()
        {
            var teamAUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var teamBUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            var neutralUnit = CreateCombatUnit(3, new Vector2Int(2, 3));
            var registry = new CombatUnitRegistry(
                new[] { teamAUnit },
                new[] { teamBUnit },
                new[] { neutralUnit });

            Assert.IsTrue(registry.Contains(teamAUnit));
            Assert.AreEqual(
                CombatFaction.TeamA,
                registry.GetFaction(teamAUnit));
            Assert.AreEqual(
                CombatFaction.TeamB,
                registry.GetFaction(teamBUnit));
            Assert.AreEqual(
                CombatFaction.Neutral,
                registry.GetFaction(neutralUnit));
        }

        [Test]
        public void ConstructorAndTurnAdvance_SkipDeadUnits()
        {
            var deadPlayer = CreateCombatUnit(1, new Vector2Int(1, 1));
            var alivePlayer = CreateCombatUnit(2, new Vector2Int(1, 2));
            var deadEnemy = CreateCombatUnit(3, new Vector2Int(3, 1));
            var aliveEnemy = CreateCombatUnit(4, new Vector2Int(3, 2));
            deadPlayer.MechRuntime.TakeDamage(deadPlayer.Mech.MaxDurability);
            deadEnemy.MechRuntime.TakeDamage(deadEnemy.Mech.MaxDurability);
            var session = CreateBattleSession(
                new[] { deadPlayer, alivePlayer },
                new[] { deadEnemy, aliveEnemy });

            Assert.AreSame(alivePlayer, session.CurrentCombatUnit);

            session.EndCurrentUnitAction();

            Assert.AreEqual(CombatFaction.TeamB, session.CurrentFaction);
            Assert.AreSame(aliveEnemy, session.CurrentCombatUnit);
        }

        [Test]
        public void DeadCurrentCombatUnit_CannotMoveOrAttack()
        {
            var playerUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var enemyUnit = CreateCombatUnit(2, new Vector2Int(2, 1));
            var session = CreateBattleSession(playerUnit, enemyUnit);
            playerUnit.MechRuntime.TakeDamage(playerUnit.Mech.MaxDurability);

            Assert.Throws<InvalidOperationException>(() =>
                session.TryMoveCurrentCombatUnit(new Vector2Int(1, 2)));
            Assert.Throws<InvalidOperationException>(() =>
                session.AttackTargetCombatUnit(enemyUnit));
        }

        private static BattleSession CreateBattleSession(
            CombatUnit playerUnit,
            CombatUnit enemyUnit)
        {
            return CreateBattleSession(
                new[] { playerUnit },
                new[] { enemyUnit });
        }

        private static BattleSession CreateBattleSession(
            CombatUnit[] playerUnits,
            CombatUnit[] enemyUnits)
        {
            return new BattleSession(5, 5, playerUnits, enemyUnits);
        }

        private static CombatUnit CreateCombatUnit(
            int id,
            Vector2Int position,
            int attack = 10,
            int durability = 100,
            int moveRange = 3)
        {
            var pilot = new PilotData(id, $"Pilot {id}", 10);
            var mech = new MechData(id, $"Mech {id}", attack, durability, moveRange);
            var factory = new CombatUnitFactory();

            return factory.Create(pilot, mech, position);
        }
    }
}
