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
    
    public partial class CodigoEmpresaInformar
    {
        public long IDCodigoEmpresaInformar { get; set; }
        public string Valor { get; set; }
        public long IdDetalleCuenta { get; set; }
        public long IdAtributoInformar { get; set; }
    
        public virtual AtributoInformar AtributoInformar { get; set; }
        public virtual DetalleCuenta DetalleCuenta { get; set; }
    }
}
