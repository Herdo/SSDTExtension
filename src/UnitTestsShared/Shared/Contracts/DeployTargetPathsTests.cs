﻿namespace SSDTLifecycleExtension.UnitTests.Shared.Contracts;

[TestFixture]
public class DeployTargetPathsTests
{
    [Test]
    public void Constructor_CorrectSettingOfProperties()
    {
        // Arrange
        const string deployScriptPath = "deployScriptPath";
        const string deployReportPath = "deployReportPath";

        // Act
        var dtp = new DeployTargetPaths(deployScriptPath, deployReportPath);

        // Assert
        Assert.AreEqual(deployScriptPath, dtp.DeployScriptPath);
        Assert.AreEqual(deployReportPath, dtp.DeployReportPath);
    }
}