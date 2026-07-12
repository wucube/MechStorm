using System;
using MechStorm.Battle.Combat;

namespace MechStorm.Presentation
{
    public sealed class BattlePresentationController
    {
        private readonly BattleSession _battleSession;
        private readonly CombatUnitVisual _unitVisual;
        private readonly BattleBoardInputter _battleBoardInputter;

        public BattlePresentationController(
            BattleSession battleSession,
            CombatUnitVisual combatUnitVisual,
            BattleBoardInputter battleBoardInputter)
        {
            _battleSession = battleSession ?? throw new ArgumentNullException(nameof(battleSession));
            _unitVisual = combatUnitVisual ?? throw new ArgumentNullException(nameof(combatUnitVisual));
            _battleBoardInputter = battleBoardInputter ?? throw new ArgumentNullException(nameof(battleBoardInputter));
        }

        public bool Tick()
        {
            if (!_battleBoardInputter.Tick(out var targetGridPos, out _))
            {
                return false;
            }

            if (!_battleSession.TryMoveCurrentCombatUnit(targetGridPos))
            {
                return false;
            }

            _unitVisual.RefreshPosition(_battleSession.CurrentCombatUnit.Position);
            return true;
        }
    }
}
