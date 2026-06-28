using System;
using MechStorm.Battle.Combat;

namespace MechStorm.Presentation
{
    public sealed class BattlePresentationController
    {
        private readonly CombatUnit _combatUnit;
        private readonly MovementResolver _movementResolver;
        private readonly CombatUnitVisual _unitVisual;
        private readonly BattleBoardInputter _battleBoardInputter;

        public BattlePresentationController(CombatUnit combatUnit, CombatUnitVisual combatUnitVisual, MovementResolver movementResolver, BattleBoardInputter battleBoardInputter)
        {
            _combatUnit = combatUnit ?? throw new ArgumentNullException(nameof(combatUnit));
            _movementResolver = movementResolver ?? throw new ArgumentNullException(nameof(movementResolver));
            _unitVisual = combatUnitVisual ?? throw new ArgumentNullException(nameof(combatUnitVisual));
            _battleBoardInputter = battleBoardInputter ?? throw new ArgumentNullException(nameof(battleBoardInputter));
        }

        public bool Tick()
        {
            if (!_battleBoardInputter.Tick(out var targetGridPos, out _))
            {
                return false;
            }

            if (!_movementResolver.TryMoveTo(_combatUnit, targetGridPos))
            {
                return false;
            }

            _unitVisual.RefreshPosition(_combatUnit.Position);
            return true;
        }
    }
}
