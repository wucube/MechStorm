using System;
using System.Collections.Generic;
using System.Linq;
using MechStorm.Battle.Foundation;
using MechStorm.Battle.Spatial;
using MechStorm.Battle.Units;

namespace MechStorm.Battle.Rules
{
    public sealed class MovementResolver
    {
        private readonly SquareGrid _grid;
        private readonly Func<Vector2Int, bool> _isPositionBlocked;

        public MovementResolver(SquareGrid grid) : this(grid, _ => false)
        {
        }

        public MovementResolver(SquareGrid grid, Func<Vector2Int, bool> isPositionBlocked)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _isPositionBlocked = isPositionBlocked ?? throw new ArgumentNullException(nameof(isPositionBlocked));
        }

        public bool TryMoveTo(CombatUnit unit, Vector2Int targetPos)
        {
            if (!CanMoveTo(unit, targetPos))
            {
                return false;
            }

            unit.MoveTo(targetPos);
            return true;
        }

        public bool CanMoveTo(CombatUnit unit, Vector2Int targetPos)
        {
            if (unit == null)
            {
                return false;
            }

            if (!_grid.IsInside(unit.Position) || !_grid.IsInside(targetPos))
            {
                return false;
            }

            var reachablePositions = GetReachablePositions(unit);
            return reachablePositions.Contains(targetPos);
        }

        public IReadOnlyList<Vector2Int> GetReachablePositions(CombatUnit unit)
        {
            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            if (!_grid.IsInside(unit.Position))
            {
                throw new ArgumentOutOfRangeException(nameof(unit), unit.Position,
                    "Combat unit position must be inside the grid.");
            }

            return _grid.GetReachablePositions(unit.Position, unit.Mech.MoveRange, _isPositionBlocked);
        }
    }
}
