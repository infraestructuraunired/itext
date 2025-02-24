using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PagoPdf.Models
{
    public class PagoEfectivoResponde
    {
        public PagoEfectivoResponde()
        {
            header = new Header();
            body = new Body();
        }
        public Header header { get; set; }
        public Body body { get; set; }

        public class Body
        {
            public Body()
            {
                pagoEfectivo = new PagoEfectivo();
            }
            public PagoEfectivo pagoEfectivo { get; set; }
        }
    }
}