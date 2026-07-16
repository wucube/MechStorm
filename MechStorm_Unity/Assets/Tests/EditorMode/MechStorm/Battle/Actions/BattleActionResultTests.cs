using MechStorm.Battle.Actions;
using MechStorm.Battle.Foundation;
using MechStorm.Battle.Units;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Actions
{
    public class BattleActionResultTests
    {
        [Test]
        public void MoveSucceeded_CreatesSuccessfulMovementResult()
        {
            var actorUnit = CreateCombatUnit(1);
            var positionBefore = new Vector2Int(1, 1);
            var positionAfter = new Vector2Int(2, 1);

            var result = BattleActionResult.MoveSucceeded(1, actorUnit, positionBefore, positionAfter);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(BattleActionType.Move, result.ActionType);
            Assert.AreEqual(BattleActionFailureReason.None, result.FailureReason);
            Assert.AreSame(actorUnit, result.ActorUnit);
            Assert.AreEqual(positionBefore, result.PositionBefore);
            Assert.AreEqual(positionAfter, result.PositionAfter);
            CollectionAssert.AreEqual(new[] { BattleActionChangeType.UnitMoved }, result.ChangeTypes);
        }

        [Test]
        public void AttackSucceeded_WithLethalDamage_OrdersDamageBeforeDeath()
        {
            var actorUnit = CreateCombatUnit(1);
            var targetUnit = CreateCombatUnit(2);

            var result = BattleActionResult.AttackSucceeded(2, actorUnit, targetUnit, 30, 0);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreSame(actorUnit, result.ActorUnit);
            Assert.AreSame(targetUnit, result.TargetUnit);
            Assert.AreEqual(30, result.DurabilityBefore);
            Assert.AreEqual(0, result.DurabilityAfter);
            CollectionAssert.AreEqual(
                new[]
                {
                    BattleActionChangeType.DamageApplied,
                    BattleActionChangeType.UnitDied
                },
                result.ChangeTypes);
        }

        [Test]
        public void ActionEnded_WhenNewRoundStarts_CreatesOrderedChangeTypes()
        {
            var actorUnit = CreateCombatUnit(1);
            var nextCombatUnit = CreateCombatUnit(2);

            var result = BattleActionResult.ActionEnded(3, actorUnit, nextCombatUnit, 1, 2,
                CombatFaction.TeamB, CombatFaction.TeamA);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreSame(nextCombatUnit, result.NextCombatUnit);
            CollectionAssert.AreEqual(
                new[]
                {
                    BattleActionChangeType.UnitActionEnded,
                    BattleActionChangeType.FactionTurnEnded,
                    BattleActionChangeType.RoundStarted,
                    BattleActionChangeType.FactionTurnStarted,
                    BattleActionChangeType.CurrentUnitChanged
                },
                result.ChangeTypes);
        }

        [Test]
        public void Failed_CreatesFailureWithoutChangeTypes()
        {
            var actorUnit = CreateCombatUnit(1);

            var result = BattleActionResult.Failed(4, BattleActionType.Move, actorUnit,
                BattleActionFailureReason.InvalidMoveTarget);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(BattleActionFailureReason.InvalidMoveTarget, result.FailureReason);
            Assert.IsEmpty(result.ChangeTypes);
        }

        private static CombatUnit CreateCombatUnit(int id)
        {
            var pilot = new PilotData(id, $"Pilot {id}", 10);
            var mech = new MechData(id, $"Mech {id}", 10, 100, 3);
            var factory = new CombatUnitFactory();

            return factory.Create(id, pilot, mech, new Vector2Int(id, 0));
        }
    }
}
