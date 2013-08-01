using System.ServiceProcess;

namespace TServiceSetup
{
    class Program
    {
        static void Main(string[] args)
        { 
            ServiceBase.Run(new ServiceBase[]
            {
                new TService()
            });
        }
    }
}
