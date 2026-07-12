using System;
using System.Collections.Generic;
using MechStorm.Battle.Combat;

namespace MechStorm.Presentation
{
    public sealed class BattlePresentationController
    {
        private readonly BattleSession _battleSession;
        private readonly IReadOnlyDictionary<CombatUnit, CombatUnitVisual>
            _unitVisualsByCombatUnit;
        private readonly BattleBoardInputter _battleBoardInputter;

        public BattlePresentationController(
            BattleSession battleSession,
            IReadOnlyDictionary<CombatUnit, CombatUnitVisual> unitVisualsByCombatUnit,
            BattleBoardInputter battleBoardInputter)
        {
            _battleSession = battleSession ?? throw new ArgumentNullException(nameof(battleSession));
            _unitVisualsByCombatUnit = unitVisualsByCombatUnit
                ?? throw new ArgumentNullException(nameof(unitVisualsByCombatUnit));
            _battleBoardInputter = battleBoardInputter ?? throw new ArgumentNullException(nameof(battleBoardInputter));
        }

        public bool Tick()
        {
            if (!_battleBoardInputter.Tick(out var targetGridPos, out _))
            {
                return false;
            }

            var currentCombatUnit = _battleSession.CurrentCombatUnit;
            if (!_unitVisualsByCombatUnit.TryGetValue(
                    currentCombatUnit,
                    out var currentUnitVisual))
            {
                throw new InvalidOperationException(
                    "Current combat unit does not have a presentation visual.");
            }

            if (!_battleSession.TryMoveCurrentCombatUnit(targetGridPos))
            {
                return false;
            }

            currentUnitVisual.RefreshPosition(currentCombatUnit.Position);
            return true;
        }
    }
}
