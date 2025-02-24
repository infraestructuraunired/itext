using System;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading;
using log4net.Config;
using ModeloBD.ObjetoConfig;
using log4net;
using System.Configuration;
using System.Net.Mime;
using System.Collections.Generic;
using PagoPdf.Models;
using PagoPdf.Imp;
using System.Text;
using Newtonsoft.Json;


namespace ModeloBD.Procesos
{
    public class EnvioCorreo
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(EnvioCorreo));
        protected static List<ImagenesCorreo> imagenes = null;
        readonly string agente = ConfigurationSettings.AppSettings["Agente"].ToString(CultureInfo.InvariantCulture);

        public MensajeCorreo Msj(proc_ObtenerDatosMail_Result mailing, ConfiguracionServer cnfsrv)
        {
            var cadena = string.Empty;
            MensajeCorreo msg = null;
            try
            {
                if (mailing != null && cnfsrv != null && !cnfsrv.esComprobante)
                {
                    using (var context = new MailingEntities())
                    {
                        var respuesta = context.proc_mail_leer1(agente, mailing.id_mail).ToList();
                        var html = (from r in respuesta where r.cl1 == "100" select r.cl3).ToList();
                        #region armaMail
                        foreach (var r in html)
                        {
                            cadena += r;
                        }
                        var asunto = (from r in respuesta where r.cl1 == "50" select r.cl3).FirstOrDefault();

                        msg = new MensajeCorreo
                        {
                            IdMailing = mailing.id_mail,
                            MensajeHtml = cadena,
                            Remitente = ConfigurationSettings.AppSettings["MAIL_REMITENTE"], //ConfigurationSettings.AppSettings["MAIL_CREDENCIAL_USER_CONTACTO"],
                            Destinatario = mailing.mail_to,
                            Asunto = asunto,
                            Tipomail = mailing.id_tipo_mail,
                            CantidadporTipo = 1
                        };
                        #endregion

                    }
                }
                else
                {
                    var listaHtml = new List<string>();
                    using (var context = new MailingEntities())
                    {
                        switch (mailing.id_tipo_mail)
                        {
                            case 6:
                                listaHtml = context.proc_mail_leer_tipo_06(mailing.mail_llave_externa, mailing.id_tipo_mail, mailing.id_mail).Select(x => x.msg_mail).ToList<string>();
                                break;
                            case 9:
                                listaHtml = context.proc_mail_leer_tipo_09(mailing.mail_llave_externa, mailing.id_tipo_mail, mailing.id_mail).Select(x => x.msg_mail).ToList<string>();
                                break;
                            case 10:
                                listaHtml = context.proc_mail_leer_tipo_10(mailing.mail_llave_externa, mailing.id_tipo_mail, mailing.id_mail).Select(x => x.msg_mail).ToList<string>();
                                break;
                            case 11:
                                listaHtml = context.proc_mail_leer_tipo_11(mailing.mail_llave_externa, mailing.id_tipo_mail, mailing.id_mail).Select(x => x.msg_mail).ToList<string>();
                                break;
                        }
                    }
                    foreach (var r in listaHtml)
                    {
                        cadena += r;
                    }

                    if (cadena == string.Empty)
                        return null;
                    msg = new MensajeCorreo
                    {
                        IdMailing = mailing.id_mail,
                        MensajeHtml = cadena,
                        Remitente = ConfigurationSettings.AppSettings["MAIL_REMITENTE"],
                        Destinatario = mailing.mail_to,
                        Asunto = "Comprobante de Pago",
                        Tipomail = mailing.id_tipo_mail,
                        CantidadporTipo = 1
                    };

                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN MSJ::: " + e.Message, e.InnerException);
            }
            return msg;
        }

        public void MarcaRetorno(int idmail, int retorno)
        {
            try
            {
                using (var context = new MailingEntities())
                {
                    context.proc_MarcaRespuestaEnvio(idmail, retorno);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN MARCARETORNO::: " + e.Message, e.InnerException);
            }
        }

        public void MarcaEnProceso(int idmail)
        {
            try
            {
                using (var context = new MailingEntities())
                {
                    context.proc_MarcaEnProceso(idmail);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN MARCAPROCESO::: " + e.Message, e.InnerException);
            }
        }

        public void Enviar()
        {
            proc_ObtenerDatosMail_Result mailingRow;
            MailMessage mail = null;
            if (Thread.CurrentThread.Name != null)
            {
                var idMail = Int32.Parse(Thread.CurrentThread.Name.Replace("Mail-", ""));
                Logger.Info("En proceso de envio de mail, proceso:" + idMail);
                #region obtenerdatosbaseConfiguraServer
                Logger.Debug("Obteniendo configuracion del mail : " + idMail);

                using (var context = new MailingEntities())
                {
                    mailingRow = (proc_ObtenerDatosMail_Result)context.proc_ObtenerDatosMail(idMail).FirstOrDefault();
                }

                if (mailingRow == null)
                {
                    Logger.Debug("Tipo Mail is null:" + idMail);
                    MarcaRetorno(idMail, 1);
                    return;
                }

                Logger.Debug("Tipo Mail : " + mailingRow.id_tipo_mail + " ,proceso: " + idMail);

                if (mailingRow.mail_to.Trim() == string.Empty)
                {
                    MarcaRetorno(idMail, 0);
                    return;
                }
                var cnfserv = Parametros.ConfServerConfig(mailingRow.id_tipo_mail);
                #region configuraSMTP
                var smtp = new SmtpClient { Host = cnfserv.smtp };
                if (cnfserv.requierePuerto == "S")
                    smtp.Port = Convert.ToInt32(cnfserv.puerto);
                if (cnfserv.requiereCredencial == "S")
                    smtp.Credentials = new NetworkCredential(cnfserv.CredencialUser, cnfserv.CredencialPass);
                if (cnfserv.requiereSSL == "S")
                    smtp.EnableSsl = true;

                var timeout = 20000;
                Logger.Debug("Obteniendo Time Out para el envio de mail : " + idMail);
                try
                {
                    timeout = Int32.Parse(ConfigurationSettings.AppSettings["MAIL_TIME_OUT"]) * 1000;
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR OBTENCION TIMEOUT correo::: " + mailingRow + "//idMailing::" + mailingRow.id_mail + " ::: " + e.Message, e.InnerException);
                }
                smtp.Timeout = timeout;
                #endregion
                #endregion
                try
                {
                    Logger.Debug(" Obteniendo Mensaje a Enviar: " + idMail);
                    var mensaje = Msj(mailingRow, cnfserv);
                    if (mensaje == null)
                    {
                        Logger.Debug(" Obteniendo Mensaje nulo, no cumple las condiciones para Enviar: " + idMail);
                        MarcaRetorno(idMail, 2);
                        return;
                    }

                    mail = new MailMessage();
                    if (mailingRow != null && cnfserv != null && !cnfserv.esComprobante && !cnfserv.esPagoEfectivo)
                    {
                        mail.Body = mensaje.MensajeHtml;
                        mail.IsBodyHtml = true;
                        mail.From = new MailAddress(cnfserv.MailRemitente);
                    }

                    var archivoCorreo = ConfigurationSettings.AppSettings["rutaImagenesCorreo"].ToString(CultureInfo.InvariantCulture);
                    Logger.Debug("cnfserv.esPagoEfectivo : " + cnfserv.esPagoEfectivo);
                    if (cnfserv.esPagoEfectivo)
                    {
                        string html = mensaje.MensajeHtml;
                        var newMsg = DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/").Replace("-", "/").Replace("-", "/");
                        Logger.Debug("XAN newMsg : " + newMsg);

                        //html = "<html><head></head><body> <table cellspacing='0' style='border: 0px solid black;max-width: 600px; width: 100%; margin: 0 auto;border-spacing: 0; '> <tr> <td> <table> <tr> <td> <img style='margin-bottom: 20px;' src='P1' alt='' /> </td></tr><tr> <td> <p style='text-align: center; text-transform: uppercase; font-weight: bold; font-family: verdana; font-size: 25px; padding: 0px; margin: 0px; margin-bottom: 20px;'>¡ya esta listo para pagar!</p></td></tr><tr> <td> <p style='text-align: center; text-transform: uppercase; font-weight: bold; font-family: verdana; font-size: 18px; padding: 0px; margin: 0px; margin-bottom: 20px;'>tu monto a pagar es :$#MONTO#</p></td></tr><tr> <td> <p style='text-align: center; font-weight: bold; font-family: verdana; font-size: 14px; padding: 0px; margin: 0px; margin-bottom: 20px;'>Ahora solo tienes que seguir estos pasos a continuación.</p></td></tr><tr> <td><p style=' width: 60%; text-align: center !important; font-weight: bold; font-family: verdana; font-size: 12px; padding: 0px; margin: 0px; text-align: left; margin: auto; line-height: 22px; margin-bottom: 20px'><span style='font-size: 14px; font-weight:bold; margin-right: 5px'>1 -</span>Presenta este código en un cajero en cualquier local: Unimarc, OK Market, Mayoriwsta 10 y ALVI</p><p style=' width: 60%; text-align: center !important; font-weight: bold; font-family: verdana; font-size: 12px; padding: 0px; margin: 0px; text-align: left; margin: auto; line-height: 22px; margin-bottom: 20px'><span style='font-size: 14px; font-weight:bold; margin-right: 5px'>2 -</span>Paga tu cuenta</p></td></tr><tr> <td> <p style=' width: 70%; text-align: center; font-family: verdana; font-size: 12px; padding: 0px; margin: 0px; text-align: center; margin: auto; margin-bottom: 20px;'>*Importante, tu cuenta aún no a sido pagada. Esto no es un comprobante de pago. No es obligación que lo imprimas, solo presenta esta pantalla con el cajero.</p></td></tr><tr> <td> <p style='width: 80%; text-align: center; font-family: verdana; font-size: 10px; padding: 0px; margin: 0px; text-align: center; margin: auto;'>#NEWMSG#</p></td></tr><tr> <td style='text-align: center;'> <img style='width: 50%' src='ImagenCodigoBarra' alt='' /> </td></tr><tr> <td style='text-align: center;'> <a href='https://www.unired.cl/Home/PuntosDeAtencion'><img src='P2' alt='' /></a> </td></tr><tr> <td> <img ssrc='P3' alt='' /> </td></tr></table> <table style='width: 100%; padding: 10px'> <tr> <td> <a href=''><img style='width:30px;' src='P4' alt='' /></a> <a href=''><img src='P5' alt='' /></a> </td><td style='float: right'> <img src='P6' alt='' /> <a href=''> <img style='width:30px;' src='P7' alt='' /></a> <a href=''><img src='P8' style='width:30px;'  alt='' /></a> <a href=''><img src='P9' style='width:30px;'  alt='' /></a> </td></tr></table> <table style='width: 100%'> <tr> <td style='line-height: 10px; width: 100%; height: 80px;background-color: #dbf5ff; text-align: center'> <a href=''><img src='P10' style='width: 50%;'  alt='' /></a> </td></tr></table> </td></tr></table></body></html>";
                       
                        string ruta = string.Empty;
                        var imagenes = ObtenerImagenesCorreoEfectivo(idMail);
                        html = html.Replace("#MONTO#", imagenes.body.pagoEfectivo.Monto.ToString("N0"));
                        html = html.Replace("#NEWMSG#", newMsg);
                        html = html.Replace("#CORREODESTINO#", mensaje.Destinatario);
                        var rutaFisica = ConfigurationSettings.AppSettings["RutaFisicaCodigoBarra"].ToString(CultureInfo.InvariantCulture);
                        if (imagenes != null && rutaFisica != null)
                        {
                            ruta = rutaFisica + imagenes.body.pagoEfectivo.idPago + ".jpeg";
                            try
                            {
                                rutaFisica = rutaFisica + imagenes.body.pagoEfectivo.idPago + ".jpeg";
                                if (!System.IO.File.Exists(rutaFisica))
                                {
                                    var stream = new FileStream(rutaFisica, FileMode.OpenOrCreate, FileAccess.Write);
                                    var buffer = imagenes.body.pagoEfectivo.codigoBarra;
                                    stream.Write(buffer, 0, imagenes.body.pagoEfectivo.codigoBarra.Length);
                                    stream.Close();
                                    Logger.Info("COPIAR IMAGEN :: " + rutaFisica);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Info("ERROR COPIAR IMAGEN :: " + ex.Message + "--- " + rutaFisica);
                            }
                        }
                        Logger.InfoFormat("Path ImagenCodigoBarra -> {0}", ruta);
                        Logger.InfoFormat("Path Imagenes Correo -> {0}", archivoCorreo);

                        html = html.Replace("ImagenCodigoBarra", ruta);
                        html = html.Replace("P10", archivoCorreo + "secure.png");
                        html = html.Replace("P1", archivoCorreo + "emailClubAhorro.png");
                        html = html.Replace("P2", archivoCorreo + "emailBtn.png");
                        html = html.Replace("P3", archivoCorreo + "lineaColor.png");
                        html = html.Replace("P4", archivoCorreo + "ws_email.png");
                        html = html.Replace("P5", archivoCorreo + "servicioCliente.png");
                        html = html.Replace("P6", archivoCorreo + "siguenos.png");
                        html = html.Replace("P7", archivoCorreo + "fb_email.png");
                        html = html.Replace("P8", archivoCorreo + "tt_email.png");
                        html = html.Replace("P9", archivoCorreo + "yt_email.png");
                        Logger.InfoFormat("html -> {0}  -- mail.From --> {1}", html, mail.From);

                        //mail.IsBodyHtml = true;
                        mail.Body = html;
                        mail.From = new MailAddress(cnfserv.MailRemitente);
                        mensaje.Asunto = "Listo! con este cupón de pago, puedes pagar tus cuentas en caja";
                    }
                    else if (cnfserv.esComprobante)
                    {
                        string html = mensaje.MensajeHtml;
                        var imagenes = ObtenerImagenesCorreo();
                        if (imagenes != null && archivoCorreo != null)
                        {
                            List<proc_mail_ImagenesCorreo_Result> imagenesTipo = new List<proc_mail_ImagenesCorreo_Result>();
                            if (mailingRow.id_tipo_mail == 6 || mailingRow.id_tipo_mail == 10)
                                imagenesTipo = imagenes.Where(t => t.Tipo == 6).ToList();
                            else if (mailingRow.id_tipo_mail == 9 || mailingRow.id_tipo_mail == 11)
                                imagenesTipo = imagenes.Where(t => t.Tipo == 9).ToList();

                            foreach (var imgreg in imagenesTipo.OrderBy(i => i.Posicion))
                            {
                                Logger.InfoFormat("PathArchivoCorreo -> {0}  NombreImagen -> {1}", archivoCorreo, imgreg.NombreImagen);
                                string ruta = archivoCorreo + imgreg.NombreImagen;
                                Logger.InfoFormat("ruta imagen -> {0}", ruta);
                                if (imgreg.Posicion == 1)
                                    html = html.Replace("imagen1", ruta);
                                else if (imgreg.Posicion == 2)
                                    html = html.Replace("imagen2", ruta);
                                else if (imgreg.Posicion == 3)
                                    html = html.Replace("imagen3", ruta);
                                else if (imgreg.Posicion == 4)
                                    html = html.Replace("imagen4", ruta);
                            }
                        }
                        mail.Body = html;
                        mail.From = new MailAddress(cnfserv.MailRemitente);
                    }
                    
                    Logger.Debug("Id Mail > 0 , proceso : " + idMail);
                    try
                    {
                        Attachment thisAttachment = null;
                        #region pdfPago
                        if (cnfserv.esComprobante)
                        {
                            if (cnfserv.adjuntaComprobante.ToUpper().Trim().Equals("S"))
                            {
                                var builder = new PdfBuilder();
                                RespuestaPdf resp;
                                if (mailingRow.id_tipo_mail == 6 || mailingRow.id_tipo_mail == 9)
                                    resp = builder.PdfPago(long.Parse(mailingRow.mail_llave_externa), 0);
                                else
                                    resp = builder.PdfPago(0, long.Parse(mailingRow.mail_llave_externa));

                                Logger.Debug("Antes evaluación PDF-> " + idMail);
                                if (resp != null && resp.PdfByte != null)
                                {
                                    Logger.Debug("PDF no nulo -> " + idMail);
                                    var memStream = new MemoryStream(resp.PdfByte);
                                    var streamWriter = new StreamWriter(memStream);
                                    Logger.Debug("Streamwriter asociado -> " + idMail);
                                    streamWriter.Flush();
                                    memStream.Position = 0;
                                    Logger.Debug("PDF Flush -> " + idMail);
                                    thisAttachment = new Attachment(memStream, "Comprobante de Pago.pdf");
                                    thisAttachment.ContentDisposition.FileName = "Comprobante de Pago.pdf";
                                    mail.Attachments.Add(thisAttachment);
                                }
                                else
                                {
                                    Logger.Debug("PDF nulo -> " + idMail);
                                    if (resp != null)
                                        Logger.Debug("PDF nulo -> " + idMail + ", Excp->" + resp.MsgError);
                                    MarcaRetorno(idMail, 4);
                                    return;
                                }
                            }
                        }
                        #endregion

                        mail.To.Add(mailingRow.id_tipo_mail == 4 ? mensaje.Remitente : mensaje.Destinatario);
                        mail.Subject = mensaje.Asunto;
                        Logger.Debug("Asunto XAN 1 : " + mail.Subject);

                        var enviado = false;
                        Logger.Debug("Intentando Enviar Mail , proceso : " + idMail);
                        try
                        {
                            Logger.Info("INICIO sendMail ::: " + mensaje.Destinatario + " || ID_MAIL ::" + mensaje.IdMailing + " ::: ", null);
                            if(smtp!=null){
                                smtp.Send(mail);
                                Logger.Info("FIN sendMail ::: " + mensaje.Destinatario + " || ID_MAIL ::" + mensaje.IdMailing + " ::: ", null);
                                enviado = true;
                            }
                        }
                        catch (Exception e)
                        {
                            enviado = false;
                            Logger.Error("Error enviar mail ::: " + mensaje.Destinatario + "//idMailing::" + mensaje.IdMailing + " ::: " + e.Message, e.InnerException);
                        }
                        if (mail != null)
                            mail.Dispose();
                        if (smtp != null)
                            smtp.Dispose();

                        if (!enviado)
                            throw new Exception("correo imposible de enviar " + mensaje.Destinatario + "//idMailing::" + mensaje.IdMailing + " ::: ");
                        else
                            MarcaRetorno(mensaje.IdMailing, 0);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("correo::: " + mensaje.Destinatario + "//idMailing::" + mensaje.IdMailing + " ::: " + e.Message, e.InnerException);
                        MarcaRetorno(mensaje.IdMailing, 3);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR EN ENVIAR::: " + e.Message, e.InnerException);
                }
            }
        }

        
        private PagoEfectivoResponde ObtenerImagenesCorreoEfectivo(int idMail)
        {
            try
            {
                PagoEfectivoRequest solicitud = new PagoEfectivoRequest();
                var monto = 0;
                string url = "http://localhost:4722//api/PagoEfectivo/SolicitarCodigoBarra";
                try
                {
                    using (var context = new MailingEntities())
                    {
                        var resp = context.proc_mail_leer_Efectivo(idMail);
                        if (resp == null)
                            return null;
                        var r = resp.First();
                        url = r.url;
                        monto = (int)r.Monto;
                        solicitud.idPago = r.mail_llave_externa;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR EN HilosDisponibles::: " + e.Message, e.InnerException);
                }

                Logger.Info("PagoEfectivoEnviar: getServiceRest" + solicitud);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(solicitud);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());

                string jsonrespuesta = streamReader.ReadToEnd();
                PagoEfectivoResponde deserializedProduct = JsonConvert.DeserializeObject<PagoEfectivoResponde>(jsonrespuesta);
                deserializedProduct.body.pagoEfectivo.Monto = monto;
                return deserializedProduct;
            }
            catch (Exception ex)
            {
                PagoEfectivoResponde res = new PagoEfectivoResponde();
                res.header.codigoRetorno = -999;
                res.header.menajeRetorno = "Ha ocurrido un error al intentar generar Ticket de pago efectivo.";

                Logger.Error("getServiceRest Ha ocurrido un error al invocar servicio PagoEfectivo SolicitarCodigoBarra, idMail:" + idMail + " descricpcion Error " + ex.Message);
                return res;
            }
        }
        

        public List<int> HilosDisponibles()
        {
            Logger.Info("INICIO HilosDisponibles::: " + DateTime.Now);
            var respuesta = new List<int>();
            try
            {
                using (var context = new MailingEntities())
                {
                    var resp = context.proc_ObtenerHilos();
                    foreach (var r in resp)
                    {
                        if (r != null)
                            respuesta.Add(r.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN HilosDisponibles::: " + e.Message, e.InnerException);
            }
            Logger.Info("INICIO HilosDisponibles::: " + DateTime.Now);
            return respuesta;
        }

        public void ResetearCorreosEnProceso()
        {
            Logger.Info("INICIO CORREOSENPROCESO::: " + DateTime.Now);
            try
            {
                using (var context = new MailingEntities())
                {
                    var correosEnProceso = (from m in context.Mailing
                                            where m.id_estado_mail == 2
                                            select m).ToList();

                    foreach (var mailing in correosEnProceso)
                    {
                        mailing.id_estado_mail = 1;
                        mailing.fecha_envio = null;
                        context.Mailing.AddOrUpdate(mailing);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN RESETEARCORREOS EN PROCESO::: " + e.Message, e.InnerException);
            }
            Logger.Info("FIN CORREOSENPROCESO::: " + DateTime.Now);
        }

        public string ObtenerPathPortal()
        {
            Logger.Info("INICIO ObtenerPathPortal::: " + DateTime.Now);
            try
            {
                using (var context2 = new UniredModelEntities())
                {
                    var config = (from m in context2.Configuracion where m.NombreConfiguracion == "SITIO_COMPROBANTE" select m).FirstOrDefault();
                    if(config!=null){
                        return config.ValorConfiguracion;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN ObtenerPathPortal ::: " + e.Message, e.InnerException);
            }
            return "https://www.unired.cl";
        }

        public List<proc_mail_ImagenesCorreo_Result> ObtenerImagenesCorreo() 
        {
            Logger.Info("INICIO DESCARGA IMAGENES::: " + DateTime.Now);
            try
            {
                using (var context = new MailingEntities())
                {
                    return context.proc_mail_ImagenesCorreo().ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR EN DESCARGA IMAGENES::: " + e.Message, e.InnerException);
                return null;
            }
        }

    }

}
