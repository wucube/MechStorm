using System;
using MechStorm.Battle.Spatial;
using MechStorm.Battle.Units;

namespace MechStorm.Battle.Rules
{
    public sealed class AttackResolver
    {
        private readonly SquareGrid _squareGrid;

        public AttackResolver(SquareGrid squareGrid)
        {
            _squareGrid = squareGrid ?? throw new ArgumentNullException(nameof(squareGrid));
        }

        public bool TryAttack(CombatUnit source, CombatUnit target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!IsTargetInRange(source, target))
            {
                return false;
            }

            var attack = source.Mech.BasicAttack;
            target.MechRuntime.TakeDamage(attack.Damage);
            return true;
        }

        private bool IsTargetInRange(CombatUnit source, CombatUnit target)
        {
            var distance = _squareGrid.GetManhattanDistance(source.Position, target.Position);
            var attack = source.Mech.BasicAttack;
            return distance >= attack.MinRange && distance <= attack.MaxRange;
        }
    }
}