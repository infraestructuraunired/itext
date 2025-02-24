using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Models
{
    public class RespuestaPdf
    {
        public Byte[] PdfByte { get; set; }
        public int CodError { get; set; }
        public string MsgError { get; set; }

        public RespuestaPdf()
        {
            CodError = 0;
            MsgError = "";
        }
    }
}
