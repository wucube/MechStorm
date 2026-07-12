namespace MechStorm.Battle.Combat
{
    public sealed class TurnStateMachine
    {
        public int CurrentRoundNumber { get; private set; } = 1;

        public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Player;

        public void AdvanceTurn()
        {
            switch (CurrentPhase)
            {
                case TurnPhase.Player:
                    CurrentPhase = TurnPhase.Enemy;
                    break;
                case TurnPhase.Enemy:
                    CurrentPhase = TurnPhase.Player;
                    CurrentRoundNumber++;
                    break;
            }
        }
    }
}