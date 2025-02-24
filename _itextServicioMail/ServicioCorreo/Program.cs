using ModeloBD.Procesos;
using System;
using System.ServiceProcess;
using System.Threading;

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
            //var envio = new EnvioCorreo();
            //var t = new Thread(envio.Enviar)
            //{
            //    Name = string.Format("Mail-{0}", Convert.ToString("15225"))
            //};
            //t.Start();
        }
    }
}
