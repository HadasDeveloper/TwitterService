using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace TServiceSetup
{
    [RunInstaller(true)]
    public partial class TServiceInstaller : System.Configuration.Install.Installer
    {
        private readonly ServiceInstaller serviceInstaller;
        private readonly ServiceProcessInstaller processInstaller;

        public TServiceInstaller()
        {

            serviceInstaller = new ServiceInstaller
                                   {
                                       DisplayName = "TServiceNew6",
                                       ServiceName = "TServiceNew6",
                                       StartType = ServiceStartMode.Automatic,
                                       Description = "T Service New6"
                                   };

            processInstaller = new ServiceProcessInstaller {Account = ServiceAccount.LocalSystem};

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
            
            InitializeComponent();

            AfterInstall += new InstallEventHandler(TServiceInstaller_AfterInstall);
        }

        void TServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller.ServiceName).Start();
        }

        
    }
}
