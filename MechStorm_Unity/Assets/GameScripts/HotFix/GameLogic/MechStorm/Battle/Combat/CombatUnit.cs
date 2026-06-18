using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    public sealed class CombatUnit
    {
        public PilotData Pilot { get; }
        
        public PilotRuntime PilotRuntime { get; }
        
        public MechData Mech { get; }
        
        public MechRuntime MechRuntime { get; }
        
        public Vector2Int Position { get; private set; }

        public CombatUnit(PilotData pilot, PilotRuntime pilotRuntime, MechData mech, MechRuntime mechRuntime, Vector2Int position)
        {
            Pilot = pilot;
            PilotRuntime = pilotRuntime;
            Mech = mech;
            MechRuntime = mechRuntime;
            Position = position;
        }

        public void MoveTo(Vector2Int position)
        {
            Position = position;
        }
    }
}