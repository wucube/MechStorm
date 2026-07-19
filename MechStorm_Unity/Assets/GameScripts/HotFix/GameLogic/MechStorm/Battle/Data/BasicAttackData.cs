using System;

namespace MechStorm.Battle.Data
{
    public sealed class BasicAttackData
    {
        public int Damage { get; }

        public int MinRange { get; }

        public int MaxRange { get; }

        public BasicAttackData(int damage, int minRange, int maxRange)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), damage, "Damage must be non-negative.");
            }

            if (minRange < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minRange), minRange, "Minimum range must be at least one.");
            }

            if (maxRange < minRange)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRange), maxRange,
                    "Maximum range must be greater than or equal to minimum range.");
            }

            Damage = damage;
            MinRange = minRange;
            MaxRange = maxRange;
        }
    }
}
