using System;
using System.Collections.Generic;
using MechStorm.Battle;
using MechStorm.Battle.Actions;
using MechStorm.Battle.Units;
using MechStorm.Presentation.Board;
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
        private readonly BattleRangeHighlightMapper _highlightMapper;
        private readonly BattleRangeHighlighter _rangeHighlighter;

        private CombatUnit _selectedCombatUnit;

        public string LastRejectionReason { get; private set; }

        public BattleActionResult LastActionResult { get; private set; }

        public BattlePresentationController(BattleSession battleSession, IReadOnlyDictionary<CombatUnit, CombatUnitVisual> combatUnitVisuals,
            BattleBoardInputter battleBoardInputter, BattleRangeHighlighter rangeHighlighter)
        {
            _battleSession = battleSession ?? throw new ArgumentNullException(nameof(battleSession));
            _combatUnitVisuals = combatUnitVisuals ?? throw new ArgumentNullException(nameof(combatUnitVisuals));
            _battleBoardInputter = battleBoardInputter ?? throw new ArgumentNullException(nameof(battleBoardInputter));
            _rangeHighlighter = rangeHighlighter ?? throw new ArgumentNullException(nameof(rangeHighlighter));
            _highlightMapper = new BattleRangeHighlightMapper();
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
                ClearSelection();
            }

            if (clickedCombatUnit != null)
            {
                actionUnit = clickedCombatUnit;
                return HandleCombatUnitClick(currentCombatUnit, clickedCombatUnit);
            }

            actionUnit = currentCombatUnit;
            return HandleGridClick(currentCombatUnit, targetGridPosition);
        }
        /// <summary>
        /// 保持当前单位选中，并按最新战斗状态刷新范围。
        /// </summary>
        public void RefreshSelection()
        {
            if (_selectedCombatUnit == null)
            {
                return;
            }

            if (!ReferenceEquals(_selectedCombatUnit, _battleSession.CurrentCombatUnit))
            {
                ClearSelection();
                return;
            }

            RefreshRangeHighlights();
        }

        /// <summary>
        /// 清除当前选择并恢复棋盘格默认颜色。
        /// </summary>
        public void ClearSelection()
        {
            _selectedCombatUnit = null;
            _rangeHighlighter.Clear();
        }

        /// <summary>
        /// 结束当前单位行动并清除选择。
        /// </summary>
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
                RefreshSelection();
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

            RefreshSelection();
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
            RefreshSelection();
            return BattleInputAction.CurrentUnitMoved;
        }

        /// <summary>
        /// 查询 Battle 权威范围并刷新棋盘高亮。
        /// </summary>
        private void RefreshRangeHighlights()
        {
            var highlights = _highlightMapper.Build(
                _battleSession.GetCurrentCombatUnitReachablePositions(),
                _battleSession.GetCurrentCombatUnitBasicAttackPositions(),
                _battleSession.GetCurrentCombatUnitBasicAttackTargets());
            _rangeHighlighter.Show(highlights);
        }

        private static string GetFailureReason(BattleActionFailureReason failureReason)
        {
            return failureReason switch
            {
                BattleActionFailureReason.InvalidMoveTarget => "Current combat unit cannot move to the selected grid position.",
                BattleActionFailureReason.TargetOutOfRange => "Target combat unit is outside attack range.",
                BattleActionFailureReason.SameFactionTarget => "Cannot attack a combat unit from the same faction.",
                BattleActionFailureReason.TargetAlreadyDead => "Cannot attack a dead combat unit.",
                BattleActionFailureReason.ActorCannotAct => "Current combat unit cannot act.",
                _ => "Battle action was rejected.",
            };
        }
    }
}
