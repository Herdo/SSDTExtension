﻿namespace SSDTLifecycleExtension.UnitTests.Shared.Models;

[TestFixture]
public class ScriptCreationStateModelTests
{
    [Test]
    public void Constructor_CorrectInitialization()
    {
        // Arrange
        var project = new SqlProject("a", "b", "c");
        var configuration = ConfigurationModel.GetDefault();
        var previousVersion = new Version(1, 0);
        // ReSharper disable once ConvertToLocalFunction
        Func<bool, Task> changeHandler = b => Task.CompletedTask;

        // Act
        var model = new ScriptCreationStateModel(project, configuration, previousVersion, true, changeHandler);

        // Assert
        model.Project.Should().BeSameAs(project);
        model.Configuration.Should().BeSameAs(configuration);
        model.PreviousVersion.Should().BeSameAs(previousVersion);
        model.CreateLatest.Should().BeTrue();
        model.HandleWorkInProgressChanged.Should().BeSameAs(changeHandler);
    }

    [Test]
    public void FormattedTargetVersion_Get_Set()
    {
        // Arrange
        var project = new SqlProject("a", "b", "c");
        var configuration = ConfigurationModel.GetDefault();
        var previousVersion = new Version(1, 0);
        // ReSharper disable once ConvertToLocalFunction
        Func<bool, Task> changeHandler = b => Task.CompletedTask;
        var model = new ScriptCreationStateModel(project, configuration, previousVersion, true, changeHandler);
        var formattedVersion = new Version(1, 0, 0);

        // Act
        model.FormattedTargetVersion = formattedVersion;

        // Assert
        model.FormattedTargetVersion.Should().BeSameAs(formattedVersion);
    }

    [Test]
    public void Paths_Get_Set()
    {
        // Arrange
        var project = new SqlProject("a", "b", "c");
        var configuration = ConfigurationModel.GetDefault();
        var previousVersion = new Version(1, 0);
        // ReSharper disable once ConvertToLocalFunction
        Func<bool, Task> changeHandler = b => Task.CompletedTask;
        var model = new ScriptCreationStateModel(project, configuration, previousVersion, true, changeHandler);
        var directories = new DirectoryPaths("projectDirectory", "latestArtifactsDirectory", "newArtifactsDirectory");
        var sourcePaths = new DeploySourcePaths("newDacpacPath", "publishProfilePath", "previousDacpacPath");
        var targetPaths = new DeployTargetPaths("deployScriptPath", "deployReportPath");
        var paths = new PathCollection(directories, sourcePaths, targetPaths);

        // Act
        model.Paths = paths;

        // Assert
        model.Paths.Should().BeSameAs(paths);
    }
}
