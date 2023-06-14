﻿namespace SSDTLifecycleExtension.UnitTests.Shared.Enums;

[TestFixture]
public class ScriptModifierTest
{
    [Test]
    public void RemoveSqlCmdStatements_AlwaysHaveHighestValue_ForSortOrder()
    {
        // Act
        var intValue = (int)ScriptModifier.RemoveSqlCmdStatements;

        // Assert
        Assert.AreEqual(int.MaxValue, intValue);
    }
}