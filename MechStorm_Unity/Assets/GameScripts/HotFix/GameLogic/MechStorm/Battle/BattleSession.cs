using System;
using System.Collections.Generic;
using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    public sealed class BattleSession
    {
        private readonly SquareGrid _squareGrid;
        private readonly TurnStateMachine _turnStateMachine;
        private readonly MovementResolver _movementResolver;
        private readonly AttackResolver _attackResolver;
        private readonly CombatUnitRegistry _unitRegistry;

        private int _currentActionUnitIndex;

        public BattleSession(
            int boardWidth,
            int boardHeight,
            IEnumerable<CombatUnit> teamAUnits,
            IEnumerable<CombatUnit> teamBUnits,
            IEnumerable<CombatUnit> neutralUnits = null)
        {
            _squareGrid = new SquareGrid(boardWidth, boardHeight);
            _movementResolver = new MovementResolver(_squareGrid);
            _attackResolver = new AttackResolver(_squareGrid);
            _turnStateMachine = new TurnStateMachine();
            _unitRegistry = new CombatUnitRegistry(
                teamAUnits,
                teamBUnits,
                neutralUnits);
            ValidateUnitPositions(
                _unitRegistry.GetFactionUnits(CombatFaction.TeamA),
                nameof(teamAUnits));
            ValidateUnitPositions(
                _unitRegistry.GetFactionUnits(CombatFaction.TeamB),
                nameof(teamBUnits));
            ValidateUnitPositions(
                _unitRegistry.GetFactionUnits(CombatFaction.Neutral),
                nameof(neutralUnits));
            _currentActionUnitIndex = FindNextAliveUnitIndex(
                _unitRegistry.GetFactionUnits(CombatFaction.TeamA),
                -1);
        }

        public SquareGrid Grid => _squareGrid;

        public IReadOnlyList<CombatUnit> CombatUnits => _unitRegistry.CombatUnits;

        public int CurrentRoundNumber => _turnStateMachine.CurrentRoundNumber;

        public TurnPhase CurrentPhase => _turnStateMachine.CurrentPhase;

        public CombatFaction CurrentFaction => CurrentPhase == TurnPhase.Player
            ? CombatFaction.TeamA
            : CombatFaction.TeamB;

        public CombatUnit CurrentCombatUnit
        {
            get
            {
                var currentFactionUnits = GetCurrentFactionCombatUnits();
                if (_currentActionUnitIndex < 0 ||
                    _currentActionUnitIndex >= currentFactionUnits.Count)
                {
                    throw new InvalidOperationException("Current faction has no alive combat unit.");
                }

                var currentCombatUnit = currentFactionUnits[_currentActionUnitIndex];
                if (currentCombatUnit.IsDead())
                {
                    throw new InvalidOperationException("Dead combat unit cannot act.");
                }

                return currentCombatUnit;
            }
        }

        public bool TryMoveCurrentCombatUnit(Vector2Int target)
        {
            return _movementResolver.TryMoveTo(CurrentCombatUnit, target);
        }

        public void AttackTargetCombatUnit(CombatUnit targetUnit)
        {
            if (targetUnit == null)
            {
                throw new ArgumentNullException(nameof(targetUnit));
            }

            if (!_unitRegistry.Contains(targetUnit))
            {
                throw new ArgumentException("Target unit does not belong to this battle session.", nameof(targetUnit));
            }

            _attackResolver.Attack(CurrentCombatUnit, targetUnit);
        }

        public void EndCurrentUnitAction()
        {
            var currentFactionUnits = GetCurrentFactionCombatUnits();
            var nextActionUnitIndex = FindNextAliveUnitIndex(currentFactionUnits, _currentActionUnitIndex);
            if (nextActionUnitIndex >= 0)
            {
                _currentActionUnitIndex = nextActionUnitIndex;
                return;
            }

            _turnStateMachine.AdvanceTurn();
            _currentActionUnitIndex = FindNextAliveUnitIndex(GetCurrentFactionCombatUnits(), -1);
        }

        public IReadOnlyList<CombatUnit> GetCurrentFactionCombatUnits()
        {
            return GetFactionCombatUnits(CurrentFaction);
        }

        public IReadOnlyList<CombatUnit> GetFactionCombatUnits(CombatFaction faction)
        {
            return _unitRegistry.GetFactionUnits(faction);
        }

        public CombatFaction GetCombatUnitFaction(CombatUnit combatUnit)
        {
            return _unitRegistry.GetFaction(combatUnit);
        }

        public IReadOnlyList<CombatUnit> GetAllAliveCombatUnits()
        {
            return _unitRegistry.GetAliveUnits();
        }

        public IReadOnlyList<CombatUnit> GetAllDeadCombatUnits()
        {
            return _unitRegistry.GetDeadUnits();
        }

        public bool AreFactionUnitsDead(CombatFaction faction)
        {
            return _unitRegistry.AreFactionUnitsDead(faction);
        }

        private void ValidateUnitPositions(
            IReadOnlyList<CombatUnit> units,
            string parameterName)
        {
            foreach (var unit in units)
            {
                if (!_squareGrid.IsInside(unit.Position))
                {
                    throw new ArgumentOutOfRangeException(
                        parameterName,
                        unit.Position,
                        "Combat unit position must be inside the battle grid.");
                }
            }
        }

        private static int FindNextAliveUnitIndex(IReadOnlyList<CombatUnit> units, int currentUnitIndex)
        {
            for (var i = currentUnitIndex + 1; i < units.Count; i++)
            {
                if (units[i].IsAlive())
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
