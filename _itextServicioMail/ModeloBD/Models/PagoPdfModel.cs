using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Models
{
    public class PagoPdfModel
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public string PathImagenes { get; set; }
        public CabeceraPago Cabecera { get; set; }
        public List<DetallePago> Detalle { get; set; }

        public PagoPdfModel()
        {
            Cabecera = new CabeceraPago();
            Detalle = new List<DetallePago>();
        }
    }
}
