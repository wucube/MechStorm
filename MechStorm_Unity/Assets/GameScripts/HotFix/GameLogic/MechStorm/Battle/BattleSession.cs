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
        private readonly IReadOnlyList<CombatUnit> _combatUnits;

        private int _currentActionUnitIndex;

        public BattleSession(
            int boardWidth,
            int boardHeight,
            IEnumerable<CombatUnit> combatUnits)
        {
            _squareGrid = new SquareGrid(boardWidth, boardHeight);
            _combatUnits = CreateUnitList(combatUnits);
            _movementResolver = new MovementResolver(_squareGrid);
            _attackResolver = new AttackResolver(_squareGrid);
            _turnStateMachine = new TurnStateMachine();
        }

        public SquareGrid Grid => _squareGrid;

        public IReadOnlyList<CombatUnit> CombatUnits => _combatUnits;

        public int CurrentRoundNumber => _turnStateMachine.CurrentRoundNumber;

        public TurnPhase CurrentPhase => _turnStateMachine.CurrentPhase;

        public CombatUnit CurrentCombatUnit => _combatUnits[_currentActionUnitIndex];

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

            if (!ContainsUnit(_combatUnits, targetUnit))
            {
                throw new ArgumentException("Target unit does not belong to this battle session.", nameof(targetUnit));
            }

            _attackResolver.Attack(CurrentCombatUnit, targetUnit);
        }

        public void EndCurrentUnitAction()
        {
            _currentActionUnitIndex++;
            if (_currentActionUnitIndex < _combatUnits.Count)
            {
                return;
            }

            _currentActionUnitIndex = 0;
            _turnStateMachine.AdvanceTurn();
        }

        private IReadOnlyList<CombatUnit> CreateUnitList(IEnumerable<CombatUnit> combatUnits)
        {
            if (combatUnits == null)
            {
                throw new ArgumentNullException(nameof(combatUnits));
            }

            var unitList = new List<CombatUnit>();
            foreach (var unit in combatUnits)
            {
                if (unit == null)
                {
                    throw new ArgumentException("Battle session cannot contain a null unit.", nameof(combatUnits));
                }

                if (!_squareGrid.IsInside(unit.Position))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(combatUnits),
                        unit.Position,
                        "Combat unit position must be inside the battle grid.");
                }

                if (ContainsUnit(unitList, unit))
                {
                    throw new ArgumentException(
                        "A combat unit can only be added to a battle session once.",
                        nameof(combatUnits));
                }

                unitList.Add(unit);
            }

            if (unitList.Count == 0)
            {
                throw new ArgumentException("Battle session must contain at least one unit.", nameof(combatUnits));
            }

            return unitList.AsReadOnly();
        }

        private static bool ContainsUnit(IReadOnlyList<CombatUnit> units, CombatUnit targetUnit)
        {
            for (var i = 0; i < units.Count; i++)
            {
                if (ReferenceEquals(units[i], targetUnit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}