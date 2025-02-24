namespace ModeloBD.ObjetoConfig
{
    public class ConfiguracionServer
    {
        public string requiereCredencial { get; set; }
        public string CredencialUser { get; set; }
        public string CredencialPass { get; set; }
        public string requierePuerto { get; set;}
        public string puerto { get; set; }
        public string smtp { get; set; }
        public string requiereSSL { get; set; }
        public string MailconComprobante { get; set; }
        public string adjuntaComprobante { get; set; }
        public bool esComprobante { get; set; }
        public string MailRemitente { get; set; }
        public bool esPagoEfectivo { get; set; }
    }
}
