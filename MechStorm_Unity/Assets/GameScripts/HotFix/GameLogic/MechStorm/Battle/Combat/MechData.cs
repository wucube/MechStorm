namespace MechStorm.Battle.Combat
{
    public sealed class MechData
    {
        public int Id { get; }
        
        public string Name { get; }
        
        public int Attack { get; }
        
        public int MaxDurability { get; }

        public int MoveRange { get; }

        public MechData(int id, string name, int attack, int maxDurability, int moveRange)
        {
            Id = id;
            Name = name;
            Attack = attack;
            MaxDurability = maxDurability;
            MoveRange = moveRange;
        }
    }
}