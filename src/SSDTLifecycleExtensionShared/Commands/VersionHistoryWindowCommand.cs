﻿namespace SSDTLifecycleExtension.Commands;

[UsedImplicitly]
[ExcludeFromCodeCoverage] // Test would require a Visual Studio shell.
internal sealed class VersionHistoryWindowCommand : WindowBaseCommand<VersionHistoryWindow, VersionHistoryViewModel>
{
    // ReSharper disable once MemberCanBePrivate.Global
    public const int CommandId = 0x0902;

    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly Guid CommandSet = new(Constants.CommandSetGuid);

    public VersionHistoryWindowCommand(SSDTLifecycleExtensionPackage package,
                                       OleMenuCommandService commandService,
                                       ICommandAvailabilityService commandAvailabilityService,
                                       ToolWindowInitializer toolWindowInitializer)
        : base(package,
               commandService,
               commandAvailabilityService,
               CommandId,
               CommandSet,
               toolWindowInitializer)
    {
    }

    public static VersionHistoryWindowCommand Instance
    {
        get;
        private set;
    }

    public static void Initialize(VersionHistoryWindowCommand instance)
    {
        Instance = instance;
    }
}