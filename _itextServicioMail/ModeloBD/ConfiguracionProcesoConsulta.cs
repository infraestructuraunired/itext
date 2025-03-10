//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModeloBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class ConfiguracionProcesoConsulta
    {
        public ConfiguracionProcesoConsulta()
        {
            this.ServiciosProceso = new HashSet<ServiciosProceso>();
            this.CuentaConsultada = new HashSet<CuentaConsultada>();
            this.CuentaProcesoConsulta = new HashSet<CuentaProcesoConsulta>();
        }
    
        public long idProcesoConsulta { get; set; }
        public string keyProcesoConsulta { get; set; }
        public Nullable<int> maximoConsulta { get; set; }
        public string correoReporte { get; set; }
        public Nullable<int> consultasSimultaneas { get; set; }
        public Nullable<int> FrecuenciaEjecucion { get; set; }
        public Nullable<bool> permiteSinFecha { get; set; }
        public Nullable<bool> reporteEnviado { get; set; }
        public Nullable<bool> permiteProceso { get; set; }
        public Nullable<System.DateTime> FechaInicioFiltro { get; set; }
        public Nullable<System.DateTime> fechaFinFiltro { get; set; }
        public Nullable<System.DateTime> fechaEnvioReporte { get; set; }
        public string observacionProceso { get; set; }
        public Nullable<bool> notificaErrores { get; set; }
        public int TipoConsultaActiva { get; set; }
        public Nullable<bool> CargaFinalizada { get; set; }
    
        public virtual ICollection<ServiciosProceso> ServiciosProceso { get; set; }
        public virtual ICollection<CuentaConsultada> CuentaConsultada { get; set; }
        public virtual ICollection<CuentaProcesoConsulta> CuentaProcesoConsulta { get; set; }
    }
}
