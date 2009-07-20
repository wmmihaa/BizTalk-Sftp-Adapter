using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using Blogical.Shared.Adapters.Common;

[RunInstaller(true)]
public class MyEventLogInstaller : Installer
{
    public MyEventLogInstaller()
    {
        EventLogInstaller installer = new EventLogInstaller();
        installer.Log = "Application";
        installer.Source = EventLogSources.SFTPAdapter;

        Installers.Add(installer);
    }
}