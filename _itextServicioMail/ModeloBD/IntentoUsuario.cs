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
    
    public partial class IntentoUsuario
    {
        public long IdIntentoUsuario { get; set; }
        public int IdUsuario { get; set; }
        public string IpIntentoUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string UsuarioIntento { get; set; }
        public string PasswordIntento { get; set; }
        public Nullable<System.DateTime> FechaIntento { get; set; }
    
        public virtual Usuario Usuario { get; set; }
    }
}
