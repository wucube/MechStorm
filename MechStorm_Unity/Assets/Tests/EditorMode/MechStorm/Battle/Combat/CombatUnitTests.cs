using System;
using MechStorm.Battle.Combat;
using MechStorm.Battle.Foundation;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Combat
{
    public class CombatUnitTests
    {
        [Test]
        public void PilotData_StoresStaticValues()
        {
            var pilot = new PilotData(1, "Test Pilot", 10);

            Assert.AreEqual(1, pilot.Id);
            Assert.AreEqual("Test Pilot", pilot.Name);
            Assert.AreEqual(10, pilot.MaxActionPoint);
        }

        [Test]
        public void PilotRuntime_StoresCurrentActionPoint()
        {
            var pilotRuntime = new PilotRuntime(6);

            Assert.AreEqual(6, pilotRuntime.CurrentActionPoint);
        }

        [Test]
        public void MechData_StoresStaticValues()
        {
            var mech = new MechData(101, "Test Mech", 25, 120, 5);

            Assert.AreEqual(101, mech.Id);
            Assert.AreEqual("Test Mech", mech.Name);
            Assert.AreEqual(25, mech.Attack);
            Assert.AreEqual(120, mech.MaxDurability);
            Assert.AreEqual(5, mech.MoveRange);
        }

        [Test]
        public void MechRuntime_StoresCurrentDurability()
        {
            var mechRuntime = new MechRuntime(80);

            Assert.AreEqual(80, mechRuntime.CurrentDurability);
            Assert.IsFalse(mechRuntime.IsDestroyed);
        }

        [Test]
        public void TakeDamage_ReducesCurrentDurability()
        {
            var mechRuntime = new MechRuntime(80);

            mechRuntime.TakeDamage(25);

            Assert.AreEqual(55, mechRuntime.CurrentDurability);
            Assert.IsFalse(mechRuntime.IsDestroyed);
        }

        [Test]
        public void TakeDamage_ClampsCurrentDurabilityToZero()
        {
            var mechRuntime = new MechRuntime(30);

            mechRuntime.TakeDamage(50);

            Assert.AreEqual(0, mechRuntime.CurrentDurability);
            Assert.IsTrue(mechRuntime.IsDestroyed);
        }

        [Test]
        public void TakeDamage_WithZeroDamage_DoesNotChangeCurrentDurability()
        {
            var mechRuntime = new MechRuntime(30);

            mechRuntime.TakeDamage(0);

            Assert.AreEqual(30, mechRuntime.CurrentDurability);
            Assert.IsFalse(mechRuntime.IsDestroyed);
        }

        [Test]
        public void TakeDamage_WithNegativeDamage_Throws()
        {
            var mechRuntime = new MechRuntime(30);

            Assert.Throws<ArgumentOutOfRangeException>(() => mechRuntime.TakeDamage(-1));
        }

        [Test]
        public void CombatUnit_ComposesDataRuntimeAndPosition()
        {
            var pilot = new PilotData(1, "Test Pilot", 10);
            var pilotRuntime = new PilotRuntime(6);
            var mech = new MechData(101, "Test Mech", 25, 120, 5);
            var mechRuntime = new MechRuntime(80);
            var position = new Vector2Int(2, 3);

            var unit = new CombatUnit(pilot, pilotRuntime, mech, mechRuntime, position);

            Assert.AreSame(pilot, unit.Pilot);
            Assert.AreSame(pilotRuntime, unit.PilotRuntime);
            Assert.AreSame(mech, unit.Mech);
            Assert.AreSame(mechRuntime, unit.MechRuntime);
            Assert.AreEqual(position, unit.Position);
        }

        [Test]
        public void CombatUnitFactory_CreatesUnitWithProvidedDataAndPosition()
        {
            var factory = new CombatUnitFactory();
            var pilot = new PilotData(1, "Test Pilot", 10);
            var mech = new MechData(101, "Test Mech", 25, 120, 5);
            var position = new Vector2Int(2, 3);

            var unit = factory.Create(pilot, mech, position);

            Assert.AreSame(pilot, unit.Pilot);
            Assert.AreSame(mech, unit.Mech);
            Assert.AreEqual(position, unit.Position);
        }

        [Test]
        public void CombatUnitFactory_InitializesRuntimeFromData()
        {
            var factory = new CombatUnitFactory();
            var pilot = new PilotData(1, "Test Pilot", 10);
            var mech = new MechData(101, "Test Mech", 25, 120, 5);

            var unit = factory.Create(pilot, mech, new Vector2Int(2, 3));

            Assert.AreEqual(10, unit.PilotRuntime.CurrentActionPoint);
            Assert.AreEqual(120, unit.MechRuntime.CurrentDurability);
        }

        [Test]
        public void CombatUnitFactory_CreatesIndependentRuntimeForEachUnit()
        {
            var factory = new CombatUnitFactory();
            var pilot = new PilotData(1, "Test Pilot", 10);
            var mech = new MechData(101, "Test Mech", 25, 120, 5);

            var firstUnit = factory.Create(pilot, mech, new Vector2Int(2, 3));
            var secondUnit = factory.Create(pilot, mech, new Vector2Int(3, 3));

            Assert.AreNotSame(firstUnit.PilotRuntime, secondUnit.PilotRuntime);
            Assert.AreNotSame(firstUnit.MechRuntime, secondUnit.MechRuntime);

            firstUnit.MechRuntime.TakeDamage(25);

            Assert.AreEqual(95, firstUnit.MechRuntime.CurrentDurability);
            Assert.AreEqual(120, secondUnit.MechRuntime.CurrentDurability);
        }

        [Test]
        public void MoveTo_UpdatesPosition()
        {
            var pilot = new PilotData(1, "Test Pilot", 10);
            var pilotRuntime = new PilotRuntime(6);
            var mech = new MechData(101, "Test Mech", 25, 120, 5);
            var mechRuntime = new MechRuntime(80);
            var unit = new CombatUnit(pilot, pilotRuntime, mech, mechRuntime, new Vector2Int(2, 3));

            unit.MoveTo(new Vector2Int(4, 5));

            Assert.AreEqual(new Vector2Int(4, 5), unit.Position);
        }

        [Test]
        public void Attack_ReducesTargetDurabilityWhenTargetIsAdjacent()
        {
            var grid = new SquareGrid(5, 5);
            var resolver = new AttackResolver(grid);
            var attacker = CreateCombatUnit(25, 100, new Vector2Int(1, 1));
            var target = CreateCombatUnit(10, 80, new Vector2Int(2, 1));

            resolver.Attack(attacker, target);

            Assert.AreEqual(55, target.MechRuntime.CurrentDurability);
            Assert.IsFalse(target.MechRuntime.IsDestroyed);
        }

        [Test]
        public void Attack_ClampsTargetDurabilityToZero()
        {
            var grid = new SquareGrid(5, 5);
            var resolver = new AttackResolver(grid);
            var attacker = CreateCombatUnit(50, 100, new Vector2Int(1, 1));
            var target = CreateCombatUnit(10, 30, new Vector2Int(2, 1));

            resolver.Attack(attacker, target);

            Assert.AreEqual(0, target.MechRuntime.CurrentDurability);
            Assert.IsTrue(target.MechRuntime.IsDestroyed);
        }

        [Test]
        public void Attack_WithNonAdjacentTarget_ThrowsAndDoesNotDamageTarget()
        {
            var grid = new SquareGrid(5, 5);
            var resolver = new AttackResolver(grid);
            var attacker = CreateCombatUnit(25, 100, new Vector2Int(1, 1));
            var target = CreateCombatUnit(10, 80, new Vector2Int(3, 1));

            Assert.Throws<InvalidOperationException>(() => resolver.Attack(attacker, target));
            Assert.AreEqual(80, target.MechRuntime.CurrentDurability);
        }

        [Test]
        public void Attack_WithSamePositionTarget_ThrowsAndDoesNotDamageTarget()
        {
            var grid = new SquareGrid(5, 5);
            var resolver = new AttackResolver(grid);
            var attacker = CreateCombatUnit(25, 100, new Vector2Int(1, 1));
            var target = CreateCombatUnit(10, 80, new Vector2Int(1, 1));

            Assert.Throws<InvalidOperationException>(() => resolver.Attack(attacker, target));
            Assert.AreEqual(80, target.MechRuntime.CurrentDurability);
        }

        private static CombatUnit CreateCombatUnit(int attack, int currentDurability, Vector2Int position)
        {
            var pilot = new PilotData(1, "Test Pilot", 10);
            var pilotRuntime = new PilotRuntime(6);
            var mech = new MechData(101, "Test Mech", attack, 120, 5);
            var mechRuntime = new MechRuntime(currentDurability);

            return new CombatUnit(pilot, pilotRuntime, mech, mechRuntime, position);
        }
    }
}
