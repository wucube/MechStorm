namespace MechStorm.Battle.Units
{
    public sealed class PilotRuntime
    {
        public int CurrentActionPoint { get; private set; }

        public PilotRuntime(int currentActionPoint)
        {
            CurrentActionPoint = currentActionPoint;
        }
    }
}