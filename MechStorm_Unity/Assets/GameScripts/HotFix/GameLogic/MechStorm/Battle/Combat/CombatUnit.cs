using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    public sealed class CombatUnit
    {
        public PilotData Pilot { get; }
        
        public MechData Mech { get; }
        
        public Vector2Int Position { get; private set; }

        public CombatUnit(PilotData pilot, MechData mech, Vector2Int position)
        {
            Pilot = pilot;
            Mech = mech;
            Position = position;
        }

        public void MoveTo(Vector2Int position)
        {
            Position = position;
        }
    }
}