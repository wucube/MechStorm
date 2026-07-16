using System;
using System.Collections.Generic;
using MechStorm.Battle;
using MechStorm.Battle.Actions;
using MechStorm.Battle.Units;
using MechStorm.Presentation.Input;
using MechStorm.Presentation.Units;
using Vector2Int = MechStorm.Battle.Foundation.Vector2Int;

namespace MechStorm.Presentation.Controllers
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

        public BattleActionResult LastActionResult { get; private set; }

        public BattlePresentationController(BattleSession battleSession, IReadOnlyDictionary<CombatUnit, CombatUnitVisual> combatUnitVisuals,
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
            LastActionResult = null;

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

        public BattleActionResult EndCurrentUnitAction()
        {
            LastRejectionReason = null;
            LastActionResult = _battleSession.EndCurrentUnitAction();
            ClearSelection();
            return LastActionResult;
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

            var attackResult = _battleSession.AttackTargetCombatUnit(clickedCombatUnit);
            LastActionResult = attackResult;
            if (!attackResult.IsSuccess)
            {
                LastRejectionReason = GetFailureReason(attackResult.FailureReason);
                return BattleInputAction.ActionRejected;
            }

            return BattleInputAction.TargetAttacked;
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

            var moveResult = _battleSession.TryMoveCurrentCombatUnit(targetGridPosition);
            LastActionResult = moveResult;
            if (!moveResult.IsSuccess)
            {
                LastRejectionReason = GetFailureReason(moveResult.FailureReason);
                return BattleInputAction.ActionRejected;
            }

            currentUnitVisual.RefreshPosition(moveResult.PositionAfter.Value);
            return BattleInputAction.CurrentUnitMoved;
        }

        private static string GetFailureReason(BattleActionFailureReason failureReason)
        {
            return failureReason switch
            {
                BattleActionFailureReason.InvalidMoveTarget => "Current combat unit cannot move to the selected grid position.",
                BattleActionFailureReason.TargetNotAdjacent => "Target combat unit must be adjacent.",
                BattleActionFailureReason.SameFactionTarget => "Cannot attack a combat unit from the same faction.",
                BattleActionFailureReason.TargetAlreadyDead => "Cannot attack a dead combat unit.",
                BattleActionFailureReason.ActorCannotAct => "Current combat unit cannot act.",
                _ => "Battle action was rejected.",
            };
        }
    }
}
