namespace MechStorm.Battle.Combat
{
    public sealed class PilotData
    {
        public int Id { get; }
        
        public string Name { get; }
        
        public int MaxActionPoint { get; }
        
        public int CurrentActionPoint { get; }

        public PilotData(int id, string name, int maxActionPoint, int currentActionPoint)
        {
            Id = id;
            Name = name;
            MaxActionPoint = maxActionPoint;
            CurrentActionPoint = currentActionPoint;
        }
    }
}