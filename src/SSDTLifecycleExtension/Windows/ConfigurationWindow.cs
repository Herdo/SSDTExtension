﻿namespace SSDTLifecycleExtension.Windows
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [Guid("ee4cb0d9-81f5-408a-9867-e7c89f6b59d2")]
    public class ConfigurationWindow : ToolWindowPane
    {
        public ConfigurationWindow() : base(null)
        {
            Caption = "ConfigurationWindow";

            Content = new ConfigurationWindowControl();
        }
    }
}
