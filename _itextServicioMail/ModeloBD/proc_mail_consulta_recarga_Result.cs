//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModeloBD
{
    using System;
    
    public partial class proc_mail_consulta_recarga_Result
    {
        public Nullable<long> NumeroTarjeta { get; set; }
        public Nullable<int> MontoCarga { get; set; }
        public string NombreComercio { get; set; }
        public string DescBienesServicios { get; set; }
        public long IdPago { get; set; }
        public string TipoPago { get; set; }
        public Nullable<int> UltimosDigitoTarjeta { get; set; }
        public Nullable<System.DateTime> FechaTransaccion { get; set; }
        public string CodigoAutorizacion { get; set; }
        public Nullable<int> NumeroCuotas { get; set; }
    }
}
