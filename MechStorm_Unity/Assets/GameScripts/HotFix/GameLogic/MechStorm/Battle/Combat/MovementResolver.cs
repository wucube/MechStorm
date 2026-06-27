using System;
using System.Linq;
using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    public sealed class MovementResolver
    {
        private readonly SquareGrid _grid;

        public MovementResolver(SquareGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
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

            var reachablePositions = _grid.GetReachablePositions(unit.Position, unit.Mech.MoveRange);
            return reachablePositions.Contains(targetPos);
        }
    }
}
