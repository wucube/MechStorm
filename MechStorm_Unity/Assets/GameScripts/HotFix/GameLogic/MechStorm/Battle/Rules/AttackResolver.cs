using System;
using MechStorm.Battle.Spatial;
using MechStorm.Battle.Units;

namespace MechStorm.Battle.Rules
{
    public sealed class AttackResolver
    {
        private const int AdjacentDistance = 1;
        
        private SquareGrid SquareGrid { get; }

        public AttackResolver(SquareGrid squareGrid)
        {
            SquareGrid = squareGrid;
        }
        
        public void Attack(CombatUnit source, CombatUnit target)
        {
            int distance = SquareGrid.GetManhattanDistance(source.Position, target.Position);
            if (distance != AdjacentDistance)
            {
                throw new InvalidOperationException("Target must be adjacent.");
            }

            var attack = source.Mech.Attack;
            target.MechRuntime.TakeDamage(attack);
        }
    }
}