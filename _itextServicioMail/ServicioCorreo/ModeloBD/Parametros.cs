using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using log4net;
using ModeloBD.ObjetoConfig;

namespace ModeloBD
{
    public class Parametros
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Parametros));

        public static ConfiguracionServer ConfServerConfig(int tipo)
        {
             var cnfSrv = new ConfiguracionServer();
                var esComprobante = false;
                cnfSrv.smtp = ConfigurationSettings.AppSettings["SMTP_MAIL"];
                var comprobantes = ConfigurationSettings.AppSettings["MAIL_REQUIERE_COMPROBANTE"];
                var presencial = ConfigurationSettings.AppSettings["MAIL_PRESENCIAL"];
                if (comprobantes != null)
                {
                    var arrComprobantes = comprobantes.Split(',');
                    if (arrComprobantes.Contains(tipo.ToString(CultureInfo.InvariantCulture)))
                        esComprobante = true;
                }

                if (presencial != null)
                {
                    var arrPresencial = presencial.Split(',');
                    if (arrPresencial.Contains(tipo.ToString(CultureInfo.InvariantCulture)))
                        cnfSrv.esPagoEfectivo = true;
                }

                if (esComprobante){
                    cnfSrv.esComprobante = true;   
                }



                cnfSrv.CredencialPass = ConfigurationSettings.AppSettings["MAIL_CREDENCIAL_PASS"];
                cnfSrv.CredencialUser = ConfigurationSettings.AppSettings["MAIL_CREDENCIAL_USER"];
                cnfSrv.MailRemitente = ConfigurationSettings.AppSettings["MAIL_REMITENTE"];
                cnfSrv.adjuntaComprobante = ConfigurationSettings.AppSettings["MAIL_ADJUNTA_COMPROBANTE"];
                cnfSrv.requiereCredencial = ConfigurationSettings.AppSettings["MAIL_REQUIERE_CREDENCIAL"];
                cnfSrv.requierePuerto = ConfigurationSettings.AppSettings["MAIL_REQUIERE_PUERTO"];
                cnfSrv.puerto = ConfigurationSettings.AppSettings["MAIL_PUERTO"];
                cnfSrv.requiereSSL = ConfigurationSettings.AppSettings["MAIL_REQUIERE_SSL"];
                cnfSrv.MailconComprobante = ConfigurationSettings.AppSettings["MAIL_REQUIERE_COMPROBANTE"];
            return cnfSrv;
        }
    }

}

