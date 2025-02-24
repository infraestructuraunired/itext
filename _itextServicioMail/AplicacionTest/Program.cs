using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using ModeloBD.Procesos;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace AplicacionTest
{
    class Program
    {
        protected static ILog Log = LogManager.GetLogger(typeof(Program));
        static void Main()
        {
            Ciclo_Elapsed(null, null);
        }

        public static void Ciclo_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                    Log.Info("INICIANDO CICLO DE VERIFICACIÓN:: " + DateTime.Now);
                    var envio = new EnvioCorreo();

                    List<int> hilosDisponibles = envio.HilosDisponibles();
  
                        Log.Info("NUMERO DE HILOS DISPONIBLES   = " + hilosDisponibles.Count);
                        if (hilosDisponibles!=null && hilosDisponibles.Count > 0)
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
                                break;
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
                
            }
        }
    }

    }

