
using ExpertPdf.HtmlToPdf;

namespace ModeloBD.ObjetoConfig
{
    public static class PdfHelper
    {


        const string LICENCIA = "cVpDUUlRQ0FFQVFHX0FRQkBfQENfSEhISA==";

        public static byte[] GetBytesFromHtmlString(string htmlString)
        {
            var conversor = new PdfConverter();

            // set the license key - required
            conversor.LicenseKey = LICENCIA;

            // set the converter options - optional
            conversor.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            conversor.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Normal;
            conversor.PdfDocumentOptions.PdfPageOrientation = PDFPageOrientation.Landscape;


            // set if header and footer are shown in the PDF - optional - default is false 
            conversor.PdfDocumentOptions.ShowHeader = false;
            conversor.PdfDocumentOptions.ShowFooter = false;

            //set the PDF document margins
            conversor.PdfDocumentOptions.LeftMargin = 10;
            conversor.PdfDocumentOptions.RightMargin = 10;
            conversor.PdfDocumentOptions.TopMargin = 10;
            conversor.PdfDocumentOptions.BottomMargin = 10;

            // set to generate a pdf with selectable text or a pdf with embedded image - optional - default is true
            conversor.PdfDocumentOptions.GenerateSelectablePdf = true;

            // set if the HTML content is resized if necessary to fit the PDF page width - optional - default is true
            conversor.PdfDocumentOptions.FitWidth = false;

            // set the embedded fonts option - optional - default is false
            conversor.PdfDocumentOptions.EmbedFonts = false;

            // set the live HTTP links option - optional - default is true
            conversor.PdfDocumentOptions.LiveUrlsEnabled = false;

            if (conversor.PdfDocumentOptions.GenerateSelectablePdf)
            {
                // set if the JavaScript is enabled during conversion to a PDF with selectable text 
                // - optional - default is false
                conversor.ScriptsEnabled = false;
                // set if the ActiveX controls (like Flash player) are enabled during conversion 
                // to a PDF with selectable text - optional - default is false
                conversor.ActiveXEnabled = false;
            }
            else
            {
                // set if the JavaScript is enabled during conversion to a PDF with embedded image 
                // - optional - default is true
                conversor.ScriptsEnabledInImage = true;
                // set if the ActiveX controls (like Flash player) are enabled during conversion 
                // to a PDF with embedded image - optional - default is true
                conversor.ActiveXEnabledInImage = true;
            }

            // set if the images in PDF are compressed with JPEG to reduce the PDF document size - optional - default is true
            conversor.PdfDocumentOptions.JpegCompressionEnabled = false;

            //set PDF document description - optional
            //if (isapre == "B")
            //  conversor.PdfDocumentInfo.AuthorName = "Isapre Banmédica";
            //else
            //    conversor.PdfDocumentInfo.AuthorName = "Isapre VidaTres";

            // Performs the conversion and get the pdf document bytes that you can further 
            // save to a file or send as a browser response
            return conversor.GetPdfBytesFromHtmlString(htmlString);
        }
    }
}
