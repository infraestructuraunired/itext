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
    
    public partial class TipoAbono
    {
        public TipoAbono()
        {
            this.EmpresaRubro = new HashSet<EmpresaRubro>();
        }
    
        public int IdTipoAbono { get; set; }
        public string NombreTipoAbono { get; set; }
    
        public virtual ICollection<EmpresaRubro> EmpresaRubro { get; set; }
    }
}
