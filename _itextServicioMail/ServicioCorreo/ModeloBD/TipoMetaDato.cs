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
    
    public partial class TipoMetaDato
    {
        public TipoMetaDato()
        {
            this.Identificador = new HashSet<Identificador>();
        }
    
        public int IdTipoMetaDato { get; set; }
        public string TipoMetaDato1 { get; set; }
        public string ProcValidacion { get; set; }
    
        public virtual ICollection<Identificador> Identificador { get; set; }
    }
}
