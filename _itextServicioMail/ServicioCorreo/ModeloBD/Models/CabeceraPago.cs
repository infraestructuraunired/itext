using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Models
{
    public class CabeceraPago
    {
        public string NombreCliente { get; set; }
        public string FechaPago { get; set; }
        public string NumeroTrx { get; set; }
        public string MedioPago { get; set; }
        public string HoraTrx { get; set; }
        public string CUB { get; set; }
        public string TotalPago { get; set; }
    }
}
