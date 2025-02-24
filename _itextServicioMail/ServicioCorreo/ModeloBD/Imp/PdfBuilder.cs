using System.Configuration;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PagoPdf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Imp
{
    public class PdfBuilder
    {
        //Creamos el tipo de Font que vamos utilizar por defecto
        Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
        Font _boldFont = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);
        Font _boldFont10 = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
        Font _boldFont12 = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);

        #region Cabecera dcto. de pago
        public PdfPTable TablaCabecera(PagoPdfModel pagoPdf)
        {
            // Crear Tabla de la cabecera.
            float[] widths = new float[] { 35f, 45f, 35f, 60f };
            PdfPCell pdfPCell = null;
            PdfPTable tblHeader = new PdfPTable(4);
            tblHeader.WidthPercentage = 100;
            tblHeader.PaddingTop = 30f;
            tblHeader.SetWidths(widths);

            pdfPCell = new PdfPCell(new Phrase(string.Format("Estimado(a): {0}", pagoPdf.Cabecera.NombreCliente), _boldFont12));
            pdfPCell.FixedHeight = 30f;
            pdfPCell.BorderWidth = 0;
            pdfPCell.Colspan = 4;
            pdfPCell.BorderWidthTop = 0.5f;
            pdfPCell.BorderWidthLeft = 0.5f;
            pdfPCell.BorderWidthRight = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Fecha", _boldFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthLeft = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase(string.Format(": {0}", pagoPdf.Cabecera.FechaPago), _standardFont));
            pdfPCell.BorderWidth = 0f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("N° de transacción", _boldFont));
            pdfPCell.BorderWidth = 0f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase(string.Format(": {0}", pagoPdf.Cabecera.NumeroTrx), _standardFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthRight = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Medio de pago", _boldFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthLeft = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase(string.Format(": {0}", pagoPdf.Cabecera.MedioPago), _standardFont));
            pdfPCell.BorderWidth = 0f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Hora de transacción", _boldFont));
            pdfPCell.BorderWidth = 0f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase(string.Format(": {0}", pagoPdf.Cabecera.HoraTrx), _standardFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthRight = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("CUB", _boldFont));
            pdfPCell.FixedHeight = 20f;
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthLeft = 0.5f;
            pdfPCell.BorderWidthBottom = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase(string.Format(": {0}", pagoPdf.Cabecera.CUB), _standardFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthBottom = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("TOTAL", _boldFont12));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthBottom = 0.5f;
            tblHeader.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase(string.Format(":{0}", pagoPdf.Cabecera.TotalPago), _boldFont12));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthRight = 0.5f;
            pdfPCell.BorderWidthBottom = 0.5f;
            tblHeader.AddCell(pdfPCell);

            return tblHeader;
        }
        #endregion

        #region Detalle cartola pago
        public PdfPTable TablaDetallePago(List<DetallePago> listaDetalle, PagoPdfModel pagoPdf)
        {
            float[] widthsDetalle = new float[] { 50f, 20f, 20f, 20f, 25f };
            var numCol = 5;

            //Si no existe ningún codigo de empresa, se esconde la columna
            var ocultarColumna = true;
            if (listaDetalle.Any(r => r.CodigoEmpresa != null))
            {
                widthsDetalle = new float[] { 50f, 20f, 20f, 20f, 25f, 25f };
                numCol = 6;
                ocultarColumna = false;
            }

            //Configuración de tabla que contiene a la tabla de detalle
            float[] widthGeneral = new float[] { 700f };
            PdfPTable tblGeneral = new PdfPTable(1);
            tblGeneral.WidthPercentage = 100;
            tblGeneral.SetWidths(widthGeneral);

            //Configuración de Tabla que tendrá el detalle de los pagos.
            PdfPTable tblDetalle = new PdfPTable(numCol);
            tblDetalle.WidthPercentage = 100;
            tblDetalle.SetWidths(widthsDetalle);

            #region Cabecera de la tabla de detalle
            var pdfPCell = new PdfPCell(new Phrase("Empresa", _boldFont10));
            pdfPCell.BorderWidth = 0.2f;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.FixedHeight = 40f;
            tblDetalle.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Identificador", _boldFont10));
            pdfPCell.BorderWidth = 0.2f;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.FixedHeight = 40f;
            tblDetalle.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Monto", _boldFont10));
            pdfPCell.BorderWidth = 0.2f;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.FixedHeight = 40f;
            tblDetalle.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Fecha \n de vencimiento", _boldFont10));
            pdfPCell.BorderWidth = 0.2f;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.FixedHeight = 40f;
            tblDetalle.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Cod. autorización", _boldFont10));
            pdfPCell.BorderWidth = 0.2f;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.FixedHeight = 40f;
            tblDetalle.AddCell(pdfPCell);

            if (!ocultarColumna)
            {
                ocultarColumna = false;
                pdfPCell = new PdfPCell(new Phrase("Código empresas", _boldFont10));
                pdfPCell.BorderWidth = 0.2f;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfPCell.FixedHeight = 40f;
                tblDetalle.AddCell(pdfPCell);
            }
            #endregion

            #region Detalle Tabla
            foreach (var p in listaDetalle)
            {
                pdfPCell = new PdfPCell(new Phrase(string.Format("{0}", p.NombreEmpresa), _standardFont));
                pdfPCell.BorderWidth = 0.2f;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfPCell.FixedHeight = 30f;
                pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                tblDetalle.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase(string.Format("{0}", p.Identificador), _standardFont));
                pdfPCell.BorderWidth = 0.2f;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfPCell.FixedHeight = 30f;
                tblDetalle.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase(string.Format("{0}", p.Monto.ToString("C0", new System.Globalization.CultureInfo("es-CL"))), _standardFont));
                pdfPCell.BorderWidth = 0.2f;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfPCell.FixedHeight = 30f;
                tblDetalle.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase(string.Format("{0}", p.FechaVencimiento), _standardFont));
                pdfPCell.BorderWidth = 0.2f;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfPCell.FixedHeight = 30f;
                tblDetalle.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase(string.Format("{0}", p.CodAutorizacionTbk), _standardFont));
                pdfPCell.BorderWidth = 0.2f;
                pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfPCell.FixedHeight = 30f;
                tblDetalle.AddCell(pdfPCell);

                if (!ocultarColumna)
                {
                    pdfPCell = new PdfPCell(new Phrase(string.Format("{0}", p.CodigoEmpresa), _standardFont));
                    pdfPCell.BorderWidth = 0.2f;
                    pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    pdfPCell.FixedHeight = 30f;
                    tblDetalle.AddCell(pdfPCell);
                }
            }
            #endregion

            //Agregar tabla "tblDetalle" a la celda de la tabla "tblGeneral"
            pdfPCell = new PdfPCell(new Paragraph(Chunk.NEWLINE));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthTop = 0.2f;
            pdfPCell.BorderWidthLeft = 0.2f;
            pdfPCell.BorderWidthRight = 0.2f;
            pdfPCell.AddElement(tblDetalle);
            tblGeneral.AddCell(pdfPCell);

            //Agregas dos dos filas a la tabla, sola para espacios
            pdfPCell = new PdfPCell(new Paragraph(Chunk.NEWLINE));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthLeft = 0.2f;
            pdfPCell.BorderWidthRight = 0.2f;
            tblGeneral.AddCell(pdfPCell);
            tblGeneral.AddCell(pdfPCell);


            //Paginación
            var par = new Paragraph(string.Format("Página {0}/{1}", pagoPdf.PaginaActual, pagoPdf.TotalPaginas), _boldFont);
            //Si existe mas de una página, debe mostrar flechas
            if (pagoPdf.TotalPaginas > 1)
            {
                if (pagoPdf.PaginaActual < pagoPdf.TotalPaginas)
                {
                    //Agregar página derecha
                    //Buscar imagen y agregarla al párrafo
                    var imgFlechaDer = ImagenSegunOpcion(5, pagoPdf.PathImagenes);
                    par.Add(new Chunk(imgFlechaDer, 1, -2));
                    
                }
                else
                {
                    //Agregar página izquierda
                    //Buscar imagen y agregarla al párrafo
                    var imgFlechaIzq = ImagenSegunOpcion(4, pagoPdf.PathImagenes);
                    par.Add(new Chunk(imgFlechaIzq, -47, -2));
                }
            }

            pdfPCell = new PdfPCell(par);
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthBottom = 0.2f;
            pdfPCell.BorderWidthLeft = 0.2f;
            pdfPCell.BorderWidthRight = 0.2f;
            pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            pdfPCell.FixedHeight = 17f;
            tblGeneral.AddCell(pdfPCell);

            return tblGeneral;
        }
        #endregion

        #region Tabla Footer
        public PdfPTable TablaFooter(string strPath)
        {
            //Configuración de tabla Footer
            float[] widthsFooter = new float[] { 50f, 150f };
            PdfPTable tblFooter = new PdfPTable(2);
            tblFooter.WidthPercentage = 100;
            tblFooter.SetWidths(widthsFooter);

            //Agregar textos a tabla
            var pdfPCell = new PdfPCell(new Phrase("Dudas y/o consultas:", _standardFont));
            pdfPCell.FixedHeight = 17f;
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthTop = 0.2f;
            pdfPCell.BorderWidthLeft = 0.2f;
            tblFooter.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("", _standardFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthTop = 0.2f;
            pdfPCell.BorderWidthRight = 0.2f;
            tblFooter.AddCell(pdfPCell);


            //Imagen Telefono
            var p = new Paragraph();
            p.Add(new Phrase("        (02)28188970", _standardFont));
            var imagenFono = ImagenSegunOpcion(6, strPath);
            p.Add(new Chunk(imagenFono, -66, -5));
            pdfPCell = new PdfPCell(p);
            pdfPCell.FixedHeight = 17f;
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthLeft = 0.2f;
            tblFooter.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("", _standardFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthRight = 0.2f;
            tblFooter.AddCell(pdfPCell);


            //Imagen Logo Web
            p = new Paragraph();
            p.Add(new Phrase("        www.unired.cl", _standardFont));
            var imagenWeb = ImagenSegunOpcion(7, strPath);
            p.Add(new Chunk(imagenWeb, -66, -5));
            pdfPCell = new PdfPCell(p);

            pdfPCell.FixedHeight = 17f;
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthLeft = 0.2f;
            pdfPCell.BorderWidthBottom = 0.2f;
            tblFooter.AddCell(pdfPCell);

            pdfPCell = new PdfPCell(new Phrase("Este comprobante de pago acredita su pago ante la(s) empresa(s).", _standardFont));
            pdfPCell.BorderWidth = 0f;
            pdfPCell.BorderWidthBottom = 0.2f;
            pdfPCell.BorderWidthRight = 0.2f;
            pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            tblFooter.AddCell(pdfPCell);

            return tblFooter;
        }
        #endregion

        #region Buscapago y armar objeto
        public PagoPdfModel ArmarPagoPdf(long idPago = 0, long idCuenta = 0)
        {
            var objBuscaPago = new PagoDAL();
            var pagoPdf = new PagoPdfModel();

            try
            {
                //Buscar pagos
                var listaDetalle = objBuscaPago.BuscarPago(idPago, idCuenta);

                //Agrupar pagos por IdEmpresa-IdEmpresaRubro
                var listaDetalleAgrupado = listaDetalle.GroupBy(r => new
                                                        { 
                                                            r.IdEmpresaRubro
                                                            ,r.IdEmpresa
                                                            ,r.NombreEmpresa
                                                            ,r.CodAutorizacionTbk
                                                            ,r.FechaPago
                                                            ,r.NumeroTrx
                                                            ,r.CUB
                                                            ,r.tbkIdTrx
                                                            ,r.HoraTrx
                                                        }).Select(g => new DetallePago 
                                                                {
                                                                    //Cabecera
                                                                    NombreCliente = g.First().NombreCliente,
                                                                    FechaPago = g.First().FechaPago,
                                                                    NumeroTrx = g.First().NumeroTrx,
                                                                    CUB = g.First().CUB,
                                                                    HoraTrx = g.First().HoraTrx,
                                                                    MedioPago = g.First().MedioPago,
                                                                    TotalPago = g.First().TotalPago,
                                                                    tbkIdTrx = g.First().tbkIdTrx,
                                                                    //Detalle
                                                                    NombreEmpresa = g.First().NombreEmpresa,
                                                                    Identificador = g.First().Identificador,
                                                                    Monto = g.Sum(s => s.Monto),
                                                                    FechaVencimiento = string.IsNullOrEmpty(g.First().FechaVencimiento) ? "no informada" : g.First().FechaVencimiento,
                                                                    CodAutorizacionTbk = g.First().CodAutorizacionTbk,
                                                                    CodigoEmpresa = g.First().CodigoEmpresa
                                                                 }).ToList();


                pagoPdf.Detalle = listaDetalleAgrupado;
                //pagoPdf.Detalle = listaDetalle;

                if (listaDetalle.FirstOrDefault() != null)
                {
                    //Armar Datos de cabecera PDF
                    pagoPdf.Cabecera.NombreCliente = listaDetalle.First().NombreCliente;
                    pagoPdf.Cabecera.FechaPago = listaDetalle.First().FechaPago.HasValue ? ((DateTime)listaDetalle.First().FechaPago).ToShortDateString() : "";
                    pagoPdf.Cabecera.NumeroTrx = listaDetalle.First().NumeroTrx.ToString();
                    pagoPdf.Cabecera.MedioPago = listaDetalle.First().MedioPago;
                    pagoPdf.Cabecera.HoraTrx = listaDetalle.First().HoraTrx.HasValue ? ((DateTime)listaDetalle.First().HoraTrx).ToShortTimeString() : "";
                    pagoPdf.Cabecera.CUB = listaDetalle.First().CUB;
                    pagoPdf.Cabecera.TotalPago = listaDetalle.Sum(r => r.Monto).ToString("C0", new System.Globalization.CultureInfo("es-CL"));
                }
   
                pagoPdf.PathImagenes = ConfigurationSettings.AppSettings["ArchivoCorreo"].ToString(CultureInfo.InvariantCulture)+"\\";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return pagoPdf;
        }
        #endregion


        #region Devuelve el objeto imagen según opción enviada, para agregarla al PDF
        ///<summary>
        ///Devuelve el objeto imagen según opción enviada, para agregarla al PDF
        ///</summary>
        ///<param name="op">Indica la imagen a devolver
        ///1: Imagen del logo
        ///2: Imagen pagado superior
        ///3: Imagen pagado inferior
        ///4: Imagen flecha izquierda
        ///5: Imagen flecha derecha
        ///</param>
        ///<param name="pathImagenes">Ruta donde se encuentran las imagenes</param>
        public Image ImagenSegunOpcion(int op, string pathImagenes)
        {
            float porcentage;
            Image imagenResp = null;
            switch (op)
            {
                case 1:
                    //Imagen Logo
                    imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "logo.png"));
                    imagenResp.Alignment = Element.ALIGN_RIGHT;
                    porcentage = 0.0f;
                    porcentage = 180 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                break;

                case 2:
                    //Imagen Pagado 1
                imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "timbre-pagado-1.jpg"));
                    porcentage = 70 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                    imagenResp.SetAbsolutePosition(495f, 610f);
                break;

                case 3:
                    //Imagen Pagado 2
                imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "timbre-pagado-2.jpg"));
                    porcentage = 60 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                break;

                case 4:
                    //Imagen flecha izquierda
                    imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "flechaIzq.png"));
                    porcentage = 8 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                break;

                case 5:
                    //Imagen flecha derecha
                    imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "flechaDer.png"));
                    porcentage = 8 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                break;

                case 6:
                    //Imagen Telefono
                    imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "phone.png"));
                    porcentage = 16 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                break;

                case 7:
                    //Imagen Logo Web
                    imagenResp = iTextSharp.text.Image.GetInstance(string.Format("{0}{1}", pathImagenes, "world.png"));
                    porcentage = 16 / imagenResp.Width;
                    imagenResp.ScalePercent(porcentage * 100);
                break;
            }

            return imagenResp;
        }
        #endregion



        #region Armar PDF Pago
        public RespuestaPdf PdfPago(long idPago = 0, long idCuenta = 0, bool printDialog = false)
        {
            var respuesta = new RespuestaPdf();
            Byte[] bytes;

            try
            {
                //Buscar Pago
                PagoPdfModel pagoPdf = ArmarPagoPdf(idPago, idCuenta);
                
                using (var ms = new MemoryStream())
                {
                    using (var doc = new Document(PageSize.LETTER))
                    {
                        using (var writer = PdfWriter.GetInstance(doc, ms))
                        {
                            doc.Open();
                            try
                            {
                                //Valor que indica la cantidad de pagos a mostrar por página
                                var objBuscaPago = new PagoDAL();
                                double pagosPorPagina = double.Parse(objBuscaPago.PagosPorPagina());

                                Chunk glue = null;
                                Paragraph p = null;
                                double numRegistros = pagoPdf.Detalle.Count();
                                var div = numRegistros / pagosPorPagina;
                                var totPaginas = Math.Ceiling(div);
                                var c = 0;
                                for (var i = 0; i < numRegistros; i++)
                                {
                                    if ((i % pagosPorPagina) == 0)
                                    {
                                        c++;
                                        pagoPdf.TotalPaginas = (int)totPaginas;
                                        pagoPdf.PaginaActual = c;

                                        //Nueva pagina
                                        doc.NewPage();

                                        //Insertamos la imagen del logo
                                        var imagenLogo = ImagenSegunOpcion(1, pagoPdf.PathImagenes);
                                        doc.Add(imagenLogo);

                                        //Insertamos imagen de pagado superior
                                        var imagenPagado = ImagenSegunOpcion(2, pagoPdf.PathImagenes);
                                        doc.Add(imagenPagado);

                                        //Titulo principal de dcto.
                                        doc.Add(new Paragraph("COMPROBANTE DE PAGO", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD)));
                                        //Agregar parrafo pequeño
                                        p = new Paragraph("   ", new Font(Font.FontFamily.HELVETICA, 4));
                                        doc.Add(p);

                                        //Agregar tabla Cabecera
                                        var tblHeader = TablaCabecera(pagoPdf);
                                        doc.Add(tblHeader);

                                        //Agregar textos, uno alineado a la izquierda y el otro a la derecha
                                        doc.Add(new Paragraph("\n"));
                                        glue = new Chunk(new iTextSharp.text.pdf.draw.VerticalPositionMark());
                                        p = new Paragraph("Cuentas Pagadas:", _boldFont12);
                                        p.Add(new Chunk(glue));
                                        p.Add(new Phrase(string.Format("Página {0}/{1}", pagoPdf.PaginaActual, pagoPdf.TotalPaginas), _standardFont));

                                        //Paginación: Si existe mas de una página, debe mostrar flechas
                                        if (pagoPdf.TotalPaginas > 1)
                                        {
                                            if (pagoPdf.PaginaActual < pagoPdf.TotalPaginas)
                                            {
                                                //Agregar página derecha
                                                //Buscar imagen y agregarla al párrafo
                                                var imgFlechaDer = ImagenSegunOpcion(5, pagoPdf.PathImagenes);
                                                p.Add(new Chunk(imgFlechaDer, 1, -2));
                                            }
                                            else
                                            {
                                                //Agregar página izquierda
                                                //Buscar imagen y agregarla al párrafo
                                                var imgFlechaIzq = ImagenSegunOpcion(4, pagoPdf.PathImagenes);
                                                p.Add(new Chunk(imgFlechaIzq, -47, -2));
                                            }
                                        }

                                        doc.Add(p);
                                        p = new Paragraph("   ", new Font(Font.FontFamily.HELVETICA, 4));
                                        doc.Add(p);

                                        #region Detalle del pago
                                        //Agregar tabla detalle al documento
                                        var detalle = pagoPdf.Detalle.Skip(i).Take((int)pagosPorPagina).ToList();
                                        var tblGeneral = TablaDetallePago(detalle, pagoPdf);
                                        doc.Add(tblGeneral);

                                        //Saltos de linea
                                        doc.Add(Chunk.NEWLINE); doc.Add(Chunk.NEWLINE);
                                        doc.Add(Chunk.NEWLINE); doc.Add(Chunk.NEWLINE);
                                        doc.Add(new Chunk(glue));
                                        
                                        #endregion

                                        #region Footer documento
                                        //Agregar textos al footer
                                        p = new Paragraph();
                                        p.Add(new Phrase(string.Format("TOTAL PAGADO: {0}", pagoPdf.Cabecera.TotalPago), _boldFont12));
                                        //Agregar imagen de pagado inferior
                                        var imagenPagado2 = ImagenSegunOpcion(3, pagoPdf.PathImagenes);
                                        p.Add(new Chunk(imagenPagado2, 15, -23));
                                        doc.Add(p);
                                        p = new Paragraph("   ", new Font(Font.FontFamily.HELVETICA, 8));
                                        doc.Add(p);

                                        p = new Paragraph("");
                                        p.Add(new Chunk(glue));
                                        p.Add(string.Format("¡Gracias por preferirnos!"));
                                        doc.Add(p);

                                        //Agregar parrafo pequeño
                                        p = new Paragraph("   ", new Font(Font.FontFamily.HELVETICA, 4));
                                        doc.Add(p);

                                        //Agregar tabla footer al documento
                                        var tblFooter = TablaFooter(pagoPdf.PathImagenes);
                                        doc.Add(tblFooter);
                                        #endregion
                                    }
                                }

                                if (printDialog)
                                {
                                    //Para gerarar el pdf como print dialog
                                    PdfAction action = new PdfAction(PdfAction.PRINTDIALOG);
                                    writer.SetOpenAction(action);
                                    writer.AddJavaScript("this.print();");
                                }

                            }
                            finally
                            {
                                doc.Close();
                            }
                        }
                    }

                    bytes = ms.ToArray();
                }

            }
            catch (Exception ex)
            {
                respuesta.CodError = -1;
                respuesta.MsgError = string.Format("Error: {0}", ex.Message);
                bytes = null;
            }

            respuesta.PdfByte = bytes;

            return respuesta;
        }
        #endregion
    }
}