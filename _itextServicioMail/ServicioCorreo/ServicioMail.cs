using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ModeloBD.Procesos;
using Timer = System.Timers.Timer;
using System.Configuration;
using log4net;
using System.IO;
using System.Globalization;

namespace ServicioMail
{
    public partial class ServicioMail : ServiceBase
    {   
        public static List<Tuple<int,int>> Hilo = new List<Tuple<int, int>>();
        private Timer _ciclo = null;
        protected static ILog Log = LogManager.GetLogger(typeof(ServicioMail));

        public ServicioMail()
        {
            log4net.Config.XmlConfigurator.Configure();
            var envio = new EnvioCorreo();
            envio.ResetearCorreosEnProceso();
            InitializeComponent();
            var tiempoCiclo =Convert.ToInt32(ConfigurationSettings.AppSettings["Inicia"])*1000;
            _ciclo = new Timer(tiempoCiclo);
            _ciclo.Elapsed += Ciclo_Elapsed;
            Log.Info("INI CONFIG :: " + DateTime.Now);
        }

        protected override void OnStart(string[] args)
        {
            _ciclo.Start();
        }

        protected override void OnStop()
        {
            _ciclo.Stop();
        }

        public void Ciclo_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ciclo.Enabled = false;
            try
            {
                Log.Info("INICIANDO CICLO DE VERIFICACIÓN:: " + DateTime.Now);
                    var envio = new EnvioCorreo();
                    List<int> hilosDisponibles = envio.HilosDisponibles();
                    Log.Info("NUMERO DE HILOS DISPONIBLES   = " + hilosDisponibles.Count);
                        if (hilosDisponibles != null && hilosDisponibles.Count > 0)
                        {
                            foreach (var idMail in hilosDisponibles)
                            {
                                Log.Info("ASIGNANDO HILO DISPONIBLE A ID MAIL  =" + idMail);
                                envio.MarcaEnProceso(idMail);
                                Log.Info("MARCADO EN PROCESO ID MAIL           =" + idMail);
                                var t = new Thread(envio.Enviar)
                                {
                                    Name = string.Format("Mail-{0}", Convert.ToString(idMail))
                                };
                                Log.Info("INICIAPROCESO ID MAIL           =" + idMail);
                                t.Start();
                            }
                        }
                Log.Info("CICLO DE PROCESO FINALIZADO :" + DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error("Ha ocurrido un error en ejecución del ciclo de envio :: fecha=> " + DateTime.Now);
                Log.Error(ex.Message, ex.InnerException);
            }
            finally
            {
                _ciclo.Enabled = true;    
            }
        }
    }
}

