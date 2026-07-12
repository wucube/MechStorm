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
            Assert.AreEqual(TurnPhase.Player, session.CurrentPhase);
            Assert.AreEqual(1, session.CurrentRoundNumber);
        }

        [Test]
        public void Constructor_WithEmptyUnitList_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                CreateBattleSession(Array.Empty<CombatUnit>()));
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
            var firstUnit = CreateCombatUnit(1, new Vector2Int(1, 1));
            var secondUnit = CreateCombatUnit(2, new Vector2Int(3, 1));
            var session = CreateBattleSession(firstUnit, secondUnit);

            session.EndCurrentUnitAction();

            Assert.AreSame(secondUnit, session.CurrentCombatUnit);
            Assert.AreEqual(TurnPhase.Player, session.CurrentPhase);

            session.EndCurrentUnitAction();

            Assert.AreSame(firstUnit, session.CurrentCombatUnit);
            Assert.AreEqual(TurnPhase.Enemy, session.CurrentPhase);
            Assert.AreEqual(1, session.CurrentRoundNumber);

            session.EndCurrentUnitAction();
            session.EndCurrentUnitAction();

            Assert.AreSame(firstUnit, session.CurrentCombatUnit);
            Assert.AreEqual(TurnPhase.Player, session.CurrentPhase);
            Assert.AreEqual(2, session.CurrentRoundNumber);
        }

        private static BattleSession CreateBattleSession(params CombatUnit[] combatUnits)
        {
            return new BattleSession(5, 5, combatUnits);
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
