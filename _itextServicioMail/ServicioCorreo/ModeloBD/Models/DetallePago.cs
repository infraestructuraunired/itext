using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Models
{
    public class DetallePago
    {
        //Cabecera
        public string NombreCliente { get; set; }
        public DateTime? FechaPago { get; set; }
        public long NumeroTrx { get; set; }
        public string MedioPago { get; set; }
        public DateTime? HoraTrx { get; set; }
        public string CUB { get; set; }
        public string TotalPago { get; set; }
        public string tbkIdTrx { get; set; }


        //Detalle
        public string NombreEmpresa { get; set; }
        public string Identificador { get; set; }
        public IEnumerable<string> ListaIdentificador { get; set; }
        public long Monto { get; set; }
        public string FechaVencimiento { get; set; }
        public string CodAutorizacionTbk { get; set; }
        public string CodigoEmpresa { get; set; }

        public int IdEmpresaRubro { get; set; }
        public int IdEmpresa { get; set; }
        public long IdCuenta { get; set; }
    }
}
