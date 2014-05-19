using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using System.Collections;

namespace Blogical.Shared.Adapters.Sftp.Management
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            string path = System.IO.Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            
            string scriptArgs = "AddResource /Type:System.BizTalk:BizTalkAssembly /Overwrite /Source:\"" + path + "/Blogical.Shared.Adapters.Sftp.Schemas.dll\" /Destination:\"" + path + "/Blogical.Shared.Adapters.Sftp.Schemas.dll\" /Options:GacOnAdd";

            string appName = Context.Parameters["appname"];
            if (!String.IsNullOrEmpty(appName))
            {
                scriptArgs += " /ApplicationName:" + appName;
            }

            SelectBizTalkApplication form = new SelectBizTalkApplication();
            form.TopMost = true;

            if (!String.IsNullOrEmpty(appName) || form.ShowDialog() == DialogResult.Yes)
            {
                System.Diagnostics.Process proc = System.Diagnostics.Process.Start("BTSTask.exe", scriptArgs);
                proc.WaitForExit();
            }

        }
    }
}