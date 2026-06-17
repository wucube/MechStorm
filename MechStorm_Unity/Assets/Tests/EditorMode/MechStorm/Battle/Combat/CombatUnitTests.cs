using MechStorm.Battle.Combat;
using MechStorm.Battle.Foundation;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Combat
{
    public class CombatUnitTests
    {
        [Test]
        public void PilotData_StoresActionPointValues()
        {
            var pilot = new PilotData(1, "Test Pilot", 10, 6);

            Assert.AreEqual(1, pilot.Id);
            Assert.AreEqual("Test Pilot", pilot.Name);
            Assert.AreEqual(10, pilot.MaxActionPoint);
            Assert.AreEqual(6, pilot.CurrentActionPoint);
        }

        [Test]
        public void MechData_StoresCombatValues()
        {
            var mech = new MechData(101, "Test Mech", 25, 120, 80, 5);

            Assert.AreEqual(101, mech.Id);
            Assert.AreEqual("Test Mech", mech.Name);
            Assert.AreEqual(25, mech.Attack);
            Assert.AreEqual(120, mech.MaxDurability);
            Assert.AreEqual(80, mech.CurrentDurability);
            Assert.AreEqual(5, mech.MoveRange);
        }

        [Test]
        public void CombatUnit_ComposesPilotMechAndPosition()
        {
            var pilot = new PilotData(1, "Test Pilot", 10, 6);
            var mech = new MechData(101, "Test Mech", 25, 120, 80, 5);
            var position = new Vector2Int(2, 3);

            var unit = new CombatUnit(pilot, mech, position);

            Assert.AreSame(pilot, unit.Pilot);
            Assert.AreSame(mech, unit.Mech);
            Assert.AreEqual(position, unit.Position);
        }

        [Test]
        public void MoveTo_UpdatesPosition()
        {
            var pilot = new PilotData(1, "Test Pilot", 10, 6);
            var mech = new MechData(101, "Test Mech", 25, 120, 80, 5);
            var unit = new CombatUnit(pilot, mech, new Vector2Int(2, 3));

            unit.MoveTo(new Vector2Int(4, 5));

            Assert.AreEqual(new Vector2Int(4, 5), unit.Position);
        }
    }
}
