using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    public sealed class CombatUnitFactory
    {
        public CombatUnit Create(PilotData pilot, MechData mech, Vector2Int position)
        {
            var pilotRuntime = new PilotRuntime(pilot.MaxActionPoint);
            var mechRuntime = new MechRuntime(mech.MaxDurability);
            return new CombatUnit(pilot, pilotRuntime, mech, mechRuntime, position);
        }
    }
}