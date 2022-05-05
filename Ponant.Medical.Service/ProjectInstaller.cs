using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace Ponant.Medical.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += ProjectInstaller_AfterInstall;
        }

        /// <summary>
        /// Démarre automatiquement le service après l'installation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController service = new ServiceController(serviceInstaller.ServiceName))
            {
                service.Start();
            }

            SetRecoveryOptions(serviceInstaller.ServiceName);
        }

        /// <summary>
        /// Paramètre le service pour redémarrer automatiquement en cas d'arrêt
        /// </summary>
        /// <param name="serviceName">Nom du service</param>
        private static void SetRecoveryOptions(string serviceName)
        {
            int exitCode;

            using (Process process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // tell Windows that the service should restart if it fails
                startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/60000", serviceName);

                process.Start();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
