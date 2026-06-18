namespace MechStorm.Battle.Combat
{
    public sealed class PilotData
    {
        public int Id { get; }
        
        public string Name { get; }
        
        public int MaxActionPoint { get; }

        public PilotData(int id, string name, int maxActionPoint)
        {
            Id = id;
            Name = name;
            MaxActionPoint = maxActionPoint;
        }
    }
}