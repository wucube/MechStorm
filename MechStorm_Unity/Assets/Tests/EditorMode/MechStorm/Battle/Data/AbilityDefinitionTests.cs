using System;
using MechStorm.Battle.Data;
using NUnit.Framework;

namespace MechStorm.Battle.Tests.Data
{
    public class AbilityDefinitionTests
    {
        [Test]
        public void Constructor_WithValidValues_StoresValues()
        {
            var ability = new AbilityDefinition(101, "Test Ability", 2, 4, TargetRule.EnemyUnit);

            Assert.AreEqual(101, ability.AbilityId);
            Assert.AreEqual("Test Ability", ability.Name);
            Assert.AreEqual(2, ability.MinRange);
            Assert.AreEqual(4, ability.MaxRange);
            Assert.AreEqual(TargetRule.EnemyUnit, ability.TargetRule);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WithNonPositiveAbilityId_Throws(int abilityId)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AbilityDefinition(abilityId, "Test Ability", 1, 2, TargetRule.EnemyUnit));

            Assert.AreEqual("abilityId", exception.ParamName);
        }

        [Test]
        public void Constructor_WithNullName_Throws()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new AbilityDefinition(101, null, 1, 2, TargetRule.EnemyUnit));

            Assert.AreEqual("name", exception.ParamName);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void Constructor_WithEmptyOrWhitespaceName_Throws(string name)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new AbilityDefinition(101, name, 1, 2, TargetRule.EnemyUnit));

            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_WithMinimumRangeBelowOne_Throws()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AbilityDefinition(101, "Test Ability", 0, 2, TargetRule.EnemyUnit));

            Assert.AreEqual("minRange", exception.ParamName);
        }

        [Test]
        public void Constructor_WithMaximumRangeBelowMinimumRange_Throws()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AbilityDefinition(101, "Test Ability", 3, 2, TargetRule.EnemyUnit));

            Assert.AreEqual("maxRange", exception.ParamName);
        }

        [Test]
        public void Constructor_WithUndefinedTargetRule_Throws()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new AbilityDefinition(101, "Test Ability", 1, 2, (TargetRule)int.MaxValue));

            Assert.AreEqual("targetRule", exception.ParamName);
        }
    }
}
