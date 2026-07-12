using System;
using System.Collections.Generic;
using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    public sealed class BattleSession
    {
        private readonly SquareGrid _squareGrid;
        private readonly MovementResolver _movementResolver;
        private readonly AttackResolver _attackResolver;
        private readonly CombatUnitRegistry _unitRegistry;
        private readonly TurnCoordinator _turnCoordinator;

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
            _turnCoordinator = new TurnCoordinator(_unitRegistry);
        }

        public SquareGrid Grid => _squareGrid;

        public IReadOnlyList<CombatUnit> CombatUnits => _unitRegistry.CombatUnits;

        public int CurrentRoundNumber => _turnCoordinator.CurrentRoundNumber;

        public CombatFaction CurrentFaction => _turnCoordinator.CurrentFaction;

        public CombatUnit CurrentCombatUnit => _turnCoordinator.CurrentCombatUnit;

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
            _turnCoordinator.EndCurrentUnitAction();
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

    }
}
