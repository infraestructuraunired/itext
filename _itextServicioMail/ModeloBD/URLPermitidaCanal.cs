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
    
    public partial class URLPermitidaCanal
    {
        public long idUrlCanal { get; set; }
        public int IdCanal { get; set; }
        public string url { get; set; }
        public string Descripcion { get; set; }
    
        public virtual Canal Canal { get; set; }
    }
}
