using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Models
{
    public class PagoEfectivo
    {
        public string idPago { get; set; }
        public int folioXCash { get; set; }
        public string TicketXCash { get; set; }
        public DateTime fechaCreacion { get; set; }
        public byte[] codigoBarra { get; set; }
        public long Monto { get; set; }
    }
}