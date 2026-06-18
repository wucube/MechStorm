namespace MechStorm.Battle.Combat
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