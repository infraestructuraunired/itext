namespace ModeloBD.ObjetoConfig
{
    public class MensajeCorreo 
    {
        public int IdMailing { get; set; }
        public string MensajeHtml { get; set; }
        public string Remitente { get; set; }
        public string Destinatario { get; set; }
        public string CopiaCorreo { get; set; }
        public string ConCopiaSiNo { get; set; }
        public string Servidor { get; set; }
        public string Asunto { get; set; }
        public string Referencia { get; set; }
        public int Tipomail { get; set; }
        public int CantidadporTipo { get; set; }

    }
}
