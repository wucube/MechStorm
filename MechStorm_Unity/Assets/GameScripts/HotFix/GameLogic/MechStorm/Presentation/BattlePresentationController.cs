using System;
using System.Collections.Generic;
using MechStorm.Battle.Combat;
using Vector2Int = MechStorm.Battle.Foundation.Vector2Int;

namespace MechStorm.Presentation
{
    public enum BattleInputAction
    {
        None,
        CurrentUnitSelected,
        CurrentUnitMoved,
        TargetAttacked,
        ActionRejected,
    }

    public sealed class BattlePresentationController
    {
        private readonly BattleSession _battleSession;
        // 战斗单位到对应表现对象的映射。
        private readonly IReadOnlyDictionary<CombatUnit, CombatUnitVisual> _combatUnitVisuals;
        private readonly BattleBoardInputter _battleBoardInputter;

        private CombatUnit _selectedCombatUnit;

        public string LastRejectionReason { get; private set; }

        public BattlePresentationController(BattleSession battleSession,
            IReadOnlyDictionary<CombatUnit, CombatUnitVisual> combatUnitVisuals,
            BattleBoardInputter battleBoardInputter)
        {
            _battleSession = battleSession ?? throw new ArgumentNullException(nameof(battleSession));
            _combatUnitVisuals = combatUnitVisuals ?? throw new ArgumentNullException(nameof(combatUnitVisuals));
            _battleBoardInputter = battleBoardInputter ?? throw new ArgumentNullException(nameof(battleBoardInputter));
        }

        public BattleInputAction Tick(out CombatUnit actionUnit)
        {
            actionUnit = null;
            LastRejectionReason = null;

            if (!_battleBoardInputter.Tick(out var clickedCombatUnit, out var targetGridPosition))
            {
                return BattleInputAction.None;
            }

            var currentCombatUnit = _battleSession.CurrentCombatUnit;
            if (!ReferenceEquals(_selectedCombatUnit, currentCombatUnit))
            {
                _selectedCombatUnit = null;
            }

            if (clickedCombatUnit != null)
            {
                actionUnit = clickedCombatUnit;
                return HandleCombatUnitClick(currentCombatUnit, clickedCombatUnit);
            }

            actionUnit = currentCombatUnit;
            return HandleGridClick(currentCombatUnit, targetGridPosition);
        }

        public void ClearSelection()
        {
            _selectedCombatUnit = null;
        }

        private BattleInputAction HandleCombatUnitClick(CombatUnit currentCombatUnit, CombatUnit clickedCombatUnit)
        {
            if (ReferenceEquals(currentCombatUnit, clickedCombatUnit))
            {
                _selectedCombatUnit = currentCombatUnit;
                return BattleInputAction.CurrentUnitSelected;
            }

            if (_selectedCombatUnit == null)
            {
                LastRejectionReason = "Select the current combat unit before attacking.";
                return BattleInputAction.ActionRejected;
            }
            try
            {
                _battleSession.AttackTargetCombatUnit(clickedCombatUnit);
                return BattleInputAction.TargetAttacked;
            }
            catch (ArgumentException exception)
            {
                LastRejectionReason = exception.Message;
                return BattleInputAction.ActionRejected;
            }
            catch (InvalidOperationException exception)
            {
                LastRejectionReason = exception.Message;
                return BattleInputAction.ActionRejected;
            }
        }

        private BattleInputAction HandleGridClick(CombatUnit currentCombatUnit, Vector2Int targetGridPosition)
        {
            if (_selectedCombatUnit == null)
            {
                LastRejectionReason = "Select the current combat unit before moving.";
                return BattleInputAction.ActionRejected;
            }

            if (!_combatUnitVisuals.TryGetValue(currentCombatUnit, out var currentUnitVisual))
            {
                throw new InvalidOperationException("Current combat unit does not have a presentation visual.");
            }

            if (!_battleSession.TryMoveCurrentCombatUnit(targetGridPosition))
            {
                LastRejectionReason = "Current combat unit cannot move to the selected grid position.";
                return BattleInputAction.ActionRejected;
            }

            currentUnitVisual.RefreshPosition(currentCombatUnit.Position);
            return BattleInputAction.CurrentUnitMoved;
        }
    }
}
