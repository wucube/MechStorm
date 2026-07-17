using System;
using MechStorm.Battle.Foundation;
using MechStorm.Battle.Units;

namespace MechStorm.Battle.Snapshots
{
    /// <summary>
    /// 某一时刻的战斗单位数据。
    /// </summary>
    public sealed class BattleUnitSnapshot
    {
        public int UnitId { get; }

        public CombatFaction Faction { get; }

        public Vector2Int Position { get; }

        public int CurrentDurability { get; }

        public int MaxDurability { get; }

        public bool IsDead => CurrentDurability <= 0;

        public BattleUnitSnapshot(int unitId, CombatFaction faction, Vector2Int position, int currentDurability, int maxDurability)
        {
            if (unitId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unitId), unitId, "Unit ID must be greater than zero.");
            }

            if (maxDurability <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDurability), maxDurability, "Maximum durability must be greater than zero.");
            }

            if (currentDurability < 0 || currentDurability > maxDurability)
            {
                throw new ArgumentOutOfRangeException(nameof(currentDurability), currentDurability, "Current durability must be between zero and maximum durability.");
            }

            UnitId = unitId;
            Faction = faction;
            Position = position;
            CurrentDurability = currentDurability;
            MaxDurability = maxDurability;
        }
    }
}