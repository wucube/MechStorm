using MechStorm.Battle.Combat;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Combat
{
    public class TurnStateMachineTests
    {
        [Test]
        public void Constructor_StartsFromPlayerTurnAndRoundOne()
        {
            var stateMachine = new TurnStateMachine();

            Assert.AreEqual(TurnPhase.Player, stateMachine.CurrentPhase);
            Assert.AreEqual(1, stateMachine.CurrentRoundNumber);
        }

        [Test]
        public void AdvanceTurn_SwitchesFromPlayerToEnemy()
        {
            var stateMachine = new TurnStateMachine();

            stateMachine.AdvanceTurn();

            Assert.AreEqual(TurnPhase.Enemy, stateMachine.CurrentPhase);
            Assert.AreEqual(1, stateMachine.CurrentRoundNumber);
        }

        [Test]
        public void AdvanceTurn_SwitchesFromEnemyToPlayerAndIncreasesRoundNumber()
        {
            var stateMachine = new TurnStateMachine();

            stateMachine.AdvanceTurn();
            stateMachine.AdvanceTurn();

            Assert.AreEqual(TurnPhase.Player, stateMachine.CurrentPhase);
            Assert.AreEqual(2, stateMachine.CurrentRoundNumber);
        }

        [Test]
        public void AdvanceTurn_RepeatsPlayerEnemyLoop()
        {
            var stateMachine = new TurnStateMachine();

            stateMachine.AdvanceTurn();
            stateMachine.AdvanceTurn();
            stateMachine.AdvanceTurn();

            Assert.AreEqual(TurnPhase.Enemy, stateMachine.CurrentPhase);
            Assert.AreEqual(2, stateMachine.CurrentRoundNumber);
        }
    }
}
