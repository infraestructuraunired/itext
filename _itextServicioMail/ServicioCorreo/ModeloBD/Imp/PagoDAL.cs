using ModeloBD;
using PagoPdf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoPdf.Imp
{
    public class PagoDAL
    {
        public string PagosPorPagina()
        {
            var cantidad = "10";
            try
            {
                //using (var contex = new UniredEntities())
                //{
                //    contex.Database.CommandTimeout = 30;
                //    var objConfiguración = contex.Configuration.Where(r => r.NombreConfiguracion.Equals("CANTIDAD_PAGOS_POR_PAGINA")).FirstOrDefault();
                //    if (objConfiguración == null)
                //        throw new Exception("No existe una configuración del número de pagos a motrar en PDF de pago.");

                //    cantidad = objConfiguración.ValorConfiguracion;
                //}
            }
            catch (Exception ex)
            {
                cantidad = "10";
            }
            return cantidad;
        }

        public List<DetallePago> BuscarPago(long idPago = 0, long idCuenta = 0)
        {
            var listaDetallePago = new List<DetallePago>();
            try
            {
                using (var context = new UniredModelEntities())
                {
                    var resultados = context.P_Portal_ComprobanteResumenPDF(idPago, idCuenta);
                    foreach (var resultado in resultados)
                    {
                        var detalle = new DetallePago();
                        detalle.NombreCliente = resultado.NombreCliente;
                        detalle.FechaPago = resultado.FechaPago;
                        detalle.FechaVencimiento = resultado.FechaVctoCuenta;
                        detalle.HoraTrx = resultado.FechaPago;
                        detalle.IdCuenta = resultado.IdCuenta;
                        detalle.IdEmpresa = 0;
                        detalle.CUB = resultado.Cub;
                        detalle.CodAutorizacionTbk = resultado.TbkCodAutorizacion;
                        detalle.Identificador = resultado.Identificador;
                        //detalle.MedioPago = resultado.MedioPago == "VD" ? "TDEBIT" : "TCREDI";
                        detalle.MedioPago = resultado.MedioPago;
                        detalle.Monto = (long)resultado.Monto;
                        detalle.NombreEmpresa = resultado.NombreEmpresaRubro;
                        detalle.NumeroTrx = resultado.IdPago;
                        detalle.tbkIdTrx = resultado.OrdenTienda;
                        detalle.CodigoEmpresa = resultado.CodigoEmpresa;
                        detalle.TotalPago = "0";
                        listaDetallePago.Add(detalle);
                    }
                }

                if (listaDetallePago == null || listaDetallePago.Count < 1)
                {
                    using (var context = new UniredModelEntities())
                    {
                        var resultados = context.P_Portal_ComprobanteResumenPDFHistorico(idPago, idCuenta);
                        foreach (var resultado in resultados)
                        {
                            var detalle = new DetallePago();
                            detalle.NombreCliente = resultado.NombreCliente;
                            detalle.FechaPago = resultado.FechaPago;
                            detalle.FechaVencimiento = resultado.FechaVctoCuenta;
                            detalle.HoraTrx = resultado.FechaPago;
                            detalle.IdCuenta = resultado.IdCuenta;
                            detalle.IdEmpresa = 0;
                            detalle.CUB = resultado.Cub;
                            detalle.CodAutorizacionTbk = resultado.TbkCodAutorizacion;
                            detalle.Identificador = resultado.Identificador;
                            //detalle.MedioPago = resultado.MedioPago == "VD" ? "TDEBIT" : "TCREDI";
                            detalle.MedioPago = resultado.MedioPago;
                            detalle.Monto = (long)resultado.Monto;
                            detalle.NombreEmpresa = resultado.NombreEmpresaRubro;
                            detalle.NumeroTrx = resultado.IdPago;
                            detalle.tbkIdTrx = resultado.OrdenTienda;
                            detalle.CodigoEmpresa = resultado.CodigoEmpresa;
                            detalle.TotalPago = "0";
                            listaDetallePago.Add(detalle);
                        }
                    }
                }


                if (listaDetallePago == null || listaDetallePago.Count < 1)
                {
                    using (var context = new Unired_HistoricaEntities())
                    {
                        //Obtener tipo cliente
                        var tipoCliente = (from pdb1 in context.PagoDetalleBoleta
                                           join p1 in context.Pago on pdb1.IdPago equals p1.IdPago
                                           join dc1 in context.DetalleCuenta on pdb1.IdDetalleCuenta equals dc1.IdDetalleCuenta
                                           join cta in context.Cuenta on dc1.IdCuenta equals cta.IdCuenta
                                           join cb in context.ConsultaBoleta on cta.IdConsultaBoleta equals cb.IdConsultaBoleta
                                           //where (p1.IdPago == idPago || dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                           where (p1.IdPago == idPago) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                           select new
                                           {
                                               cb.IdTipoCliente
                                           }).FirstOrDefault();

                        if(tipoCliente == null){
                            tipoCliente = (from pdb1 in context.PagoDetalleBoleta
                                           join p1 in context.Pago on pdb1.IdPago equals p1.IdPago
                                           join dc1 in context.DetalleCuenta on pdb1.IdDetalleCuenta equals dc1.IdDetalleCuenta
                                           join cta in context.Cuenta on dc1.IdCuenta equals cta.IdCuenta
                                           join cb in context.ConsultaBoleta on cta.IdConsultaBoleta equals cb.IdConsultaBoleta
                                           where (dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                           //where (p1.IdPago == idPago) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                           select new
                                           {
                                               cb.IdTipoCliente
                                           }).FirstOrDefault();
                        }

                        if (tipoCliente == null)
                            throw new Exception(string.Format("No existe tipo de cliente para el pago N° {0}.", idPago));

                        if (tipoCliente.IdTipoCliente == 1)
                        {
                            //Pagos usuario Registrado
                            if(idPago > 0){
                                listaDetallePago = (from pdb1 in context.PagoDetalleBoleta
                                                join p1 in context.Pago on pdb1.IdPago equals p1.IdPago
                                                join dc1 in context.DetalleCuenta on pdb1.IdDetalleCuenta equals dc1.IdDetalleCuenta
                                                join cta in context.Cuenta on dc1.IdCuenta equals cta.IdCuenta
                                                join cb in context.ConsultaBoleta on cta.IdConsultaBoleta equals cb.IdConsultaBoleta
                                                join cc in context.CuentaCliente on cta.IdCuentaCliente equals cc.IdCuentaCliente
                                                join e in context.EmpresaRubro on cc.IdEmpresaRubro equals e.IdEmpresaRubro
                                                join cl in context.Cliente on cc.IdCliente equals cl.IdCliente
                                                //where (p1.IdPago == idPago || dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                where (p1.IdPago == idPago) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                select new DetallePago
                                                {
                                                    //Cabecera
                                                    NombreCliente = cl.Nombre + " " + cl.Apellido,
                                                    FechaPago = p1.FechaPago,
                                                    NumeroTrx = p1.IdPago,
                                                    CUB = p1.Cub,
                                                    HoraTrx = p1.FechaPago,
                                                    MedioPago = pdb1.TbkTipoPago == "VD" ? "TDEBIT" : "TCREDIT",
                                                    TotalPago = "",
                                                    tbkIdTrx = pdb1.TbkIdTrx,
                                                    //Detalle
                                                    NombreEmpresa = e.NombreServicio,
                                                    ListaIdentificador = context.IdentificadorCtaCli.Where(i => i.IdCuentaCliente.Equals(cc.IdCuentaCliente)).AsEnumerable().Select(r => r.Identificador),
                                                    Monto = pdb1.MontoPagado.HasValue ? (long)pdb1.MontoPagado : 0,
                                                    FechaVencimiento = context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault() == null ? "no informada" : context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault().FechaVctoCuenta,
                                                    CodAutorizacionTbk = pdb1.TbkCodAutoriza,
                                                    CodigoEmpresa = (from cei in context.CodigoEmpresaImprimir where cei.IdCuenta == pdb1.IdDetalleCuenta select cei.Valor).FirstOrDefault(),

                                                    IdEmpresaRubro = e.IdEmpresaRubro,
                                                    IdEmpresa = e.IdEmpresa
                                                }

                                   ).ToList();
                            }

                            if (listaDetallePago == null || listaDetallePago.Count() == 0) {
                                listaDetallePago = (from pdb1 in context.PagoDetalleBoleta
                                                    join p1 in context.Pago on pdb1.IdPago equals p1.IdPago
                                                    join dc1 in context.DetalleCuenta on pdb1.IdDetalleCuenta equals dc1.IdDetalleCuenta
                                                    join cta in context.Cuenta on dc1.IdCuenta equals cta.IdCuenta
                                                    join cb in context.ConsultaBoleta on cta.IdConsultaBoleta equals cb.IdConsultaBoleta
                                                    join cc in context.CuentaCliente on cta.IdCuentaCliente equals cc.IdCuentaCliente
                                                    join e in context.EmpresaRubro on cc.IdEmpresaRubro equals e.IdEmpresaRubro
                                                    join cl in context.Cliente on cc.IdCliente equals cl.IdCliente
                                                    where (dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                    select new DetallePago
                                                    {
                                                        //Cabecera
                                                        NombreCliente = cl.Nombre + " " + cl.Apellido,
                                                        FechaPago = p1.FechaPago,
                                                        NumeroTrx = p1.IdPago,
                                                        CUB = p1.Cub,
                                                        HoraTrx = p1.FechaPago,
                                                        MedioPago = pdb1.TbkTipoPago == "VD" ? "TDEBIT" : "TCREDIT",
                                                        TotalPago = "",
                                                        tbkIdTrx = pdb1.TbkIdTrx,
                                                        //Detalle
                                                        NombreEmpresa = e.NombreServicio,
                                                        ListaIdentificador = context.IdentificadorCtaCli.Where(i => i.IdCuentaCliente.Equals(cc.IdCuentaCliente)).AsEnumerable().Select(r => r.Identificador),
                                                        Monto = pdb1.MontoPagado.HasValue ? (long)pdb1.MontoPagado : 0,
                                                        FechaVencimiento = context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault() == null ? "no informada" : context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault().FechaVctoCuenta,
                                                        CodAutorizacionTbk = pdb1.TbkCodAutoriza,
                                                        CodigoEmpresa = (from cei in context.CodigoEmpresaImprimir where cei.IdCuenta == pdb1.IdDetalleCuenta select cei.Valor).FirstOrDefault(),

                                                        IdEmpresaRubro = e.IdEmpresaRubro,
                                                        IdEmpresa = e.IdEmpresa
                                                    }

                                   ).ToList();
                            }
                        }
                        else
                        {
                            if(idPago > 0){
                                //Pagos usuario Express
                                listaDetallePago = (from pdb1 in context.PagoDetalleBoleta
                                                join p1 in context.Pago on pdb1.IdPago equals p1.IdPago
                                                join dc1 in context.DetalleCuenta on pdb1.IdDetalleCuenta equals dc1.IdDetalleCuenta
                                                join cta in context.Cuenta on dc1.IdCuenta equals cta.IdCuenta
                                                join cb in context.ConsultaBoleta on cta.IdConsultaBoleta equals cb.IdConsultaBoleta
                                                join cc in context.CuentaClienteExpress on cta.IdCuentaCliente equals cc.IdCuentaCliente
                                                join e in context.EmpresaRubro on cc.IdEmpresaRubro equals e.IdEmpresaRubro
                                                //where (p1.IdPago == idPago || dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                where (p1.IdPago == idPago) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                select new DetallePago
                                                {
                                                    //Cabecera
                                                    NombreCliente = "usuario",
                                                    FechaPago = p1.FechaPago,
                                                    NumeroTrx = p1.IdPago,
                                                    CUB = p1.Cub,
                                                    HoraTrx = p1.FechaPago,
                                                    MedioPago = pdb1.TbkTipoPago == "VD" ? "TDEBIT" : "TCREDIT",
                                                    TotalPago = "",
                                                    tbkIdTrx = pdb1.TbkIdTrx,
                                                    //Detalle
                                                    NombreEmpresa = e.NombreServicio,
                                                    ListaIdentificador = context.IdentificadorCtaCliExpress.Where(i => i.IdCuentaCliente.Equals(cc.IdCuentaCliente)).AsEnumerable().Select(r => r.Identificador),
                                                    Monto = pdb1.MontoPagado.HasValue ? (long)pdb1.MontoPagado : 0,
                                                    FechaVencimiento = context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault() == null ? "no informada" : context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault().FechaVctoCuenta,
                                                    CodAutorizacionTbk = pdb1.TbkCodAutoriza,
                                                    CodigoEmpresa = (from cei in context.CodigoEmpresaImprimir where cei.IdCuenta == pdb1.IdDetalleCuenta select cei.Valor).FirstOrDefault(),

                                                    IdEmpresaRubro = e.IdEmpresaRubro,
                                                    IdEmpresa = e.IdEmpresa
                                                }).ToList();
                            }

                            if (listaDetallePago == null || listaDetallePago.Count() == 0)
                            {

                                listaDetallePago = (from pdb1 in context.PagoDetalleBoleta
                                                    join p1 in context.Pago on pdb1.IdPago equals p1.IdPago
                                                    join dc1 in context.DetalleCuenta on pdb1.IdDetalleCuenta equals dc1.IdDetalleCuenta
                                                    join cta in context.Cuenta on dc1.IdCuenta equals cta.IdCuenta
                                                    join cb in context.ConsultaBoleta on cta.IdConsultaBoleta equals cb.IdConsultaBoleta
                                                    join cc in context.CuentaClienteExpress on cta.IdCuentaCliente equals cc.IdCuentaCliente
                                                    join e in context.EmpresaRubro on cc.IdEmpresaRubro equals e.IdEmpresaRubro
                                                    //where (p1.IdPago == idPago || dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                    where (dc1.IdCuenta == idCuenta) && pdb1.IdEstadoPago == 2 && p1.IdEstadoPago == 2
                                                    select new DetallePago
                                                    {
                                                        //Cabecera
                                                        NombreCliente = "usuario",
                                                        FechaPago = p1.FechaPago,
                                                        NumeroTrx = p1.IdPago,
                                                        CUB = p1.Cub,
                                                        HoraTrx = p1.FechaPago,
                                                        MedioPago = pdb1.TbkTipoPago == "VD" ? "TDEBIT" : "TCREDIT",
                                                        TotalPago = "",
                                                        tbkIdTrx = pdb1.TbkIdTrx,
                                                        //Detalle
                                                        NombreEmpresa = e.NombreServicio,
                                                        ListaIdentificador = context.IdentificadorCtaCliExpress.Where(i => i.IdCuentaCliente.Equals(cc.IdCuentaCliente)).AsEnumerable().Select(r => r.Identificador),
                                                        Monto = pdb1.MontoPagado.HasValue ? (long)pdb1.MontoPagado : 0,
                                                        FechaVencimiento = context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault() == null ? "no informada" : context.DetalleCuenta.Where(d => d.IdCuenta.Equals(cta.IdCuenta)).OrderBy(o => o.NumeroCuota).FirstOrDefault().FechaVctoCuenta,
                                                        CodAutorizacionTbk = pdb1.TbkCodAutoriza,
                                                        CodigoEmpresa = (from cei in context.CodigoEmpresaImprimir where cei.IdCuenta == pdb1.IdDetalleCuenta select cei.Valor).FirstOrDefault(),

                                                        IdEmpresaRubro = e.IdEmpresaRubro,
                                                        IdEmpresa = e.IdEmpresa
                                                    }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            if (listaDetallePago.FirstOrDefault() == null)
                throw new Exception(string.Format("No existen registros de pago para el pago N° {0}.", idPago));

            return listaDetallePago;
        }
    }
}
