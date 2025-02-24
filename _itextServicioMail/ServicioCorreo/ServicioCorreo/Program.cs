using System.ServiceProcess;

namespace ServicioMail
{
    static class Program
    {

        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new ServicioMail() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
