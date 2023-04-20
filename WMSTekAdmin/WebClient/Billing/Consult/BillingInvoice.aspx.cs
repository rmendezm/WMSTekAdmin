using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Web.Caching;

namespace Binaria.WMSTek.WebClient.Billing
{
    public partial class BillingInvoice : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<BillingLogConsult> billingLogConsultViewDTO = new GenericViewDTO<BillingLogConsult>();
        private bool isValidViewDTO = false;
        private bool isNew = false;

        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateViewState("page"))
                    return (int)ViewState["page"];
                else
                    return 0;
            }

            set { ViewState["page"] = value; }
        }

        public int countVisitPopUp
        {
            get
            {
                if (ValidateViewState("countVisitPopUp"))
                    return (int)ViewState["countVisitPopUp"];
                else
                    return 0;
            }

            set { ViewState["countVisitPopUp"] = value; }
        }
        #endregion




        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        //UpdateSession(false);
                        //PopulateLists();

                        //ImageButton btnGenerarExcel2 = (ImageButton)this.Master.ucTaskBar.FindControl("btnExcel");
                        //btnGenerarExcel2.OnClientClick = "return ShowProgress()";

                        countVisitPopUp = 0;
                    }
                    else
                    {

                        if (ValidateSession(WMSTekSessions.BillingInvoice.BillingLogList))
                        {
                            billingLogConsultViewDTO = (GenericViewDTO<BillingLogConsult>)Session[WMSTekSessions.BillingInvoice.BillingLogList];
                            //Session.Remove(WMSTekSessions.Shared.OwnerList);
                            isValidViewDTO = true;
                        }

                        // Si es un ViewDTO valido, carga la grilla y las listas
                        if (isValidViewDTO)
                        {
                            // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                            PopulateGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                    }
                }

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox txtFilterDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
                TextBox txtFilterDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");
                DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");

                DateTime dateFrom = Convert.ToDateTime(txtFilterDateFrom.Text.Trim());//.ToString(this.Master.ucMainFilter.FormatDate);
                DateTime dateTo = Convert.ToDateTime(txtFilterDateTo.Text.Trim());

                BillingContract searchContract = new BillingContract();
                searchContract.Owner = new Owner(int.Parse(ddlOwn.SelectedValue));
                searchContract.Status = true;
                GenericViewDTO<BillingContract> billingCOntract = iBillingMGR.BillingContractGetByAnyParameter(searchContract, context);

                if (dateFrom > dateTo)
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorDate.Text, "");
                }
                else if (billingCOntract.Entities == null && billingCOntract.Entities.Count == 0)
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorValidContract.Text, "");
                }
                else
                {
                    ReloadData();

                    //if (billingLogConsultViewDTO.Entities != null && billingLogConsultViewDTO.Entities.Count > 0)
                    //{
                    //    //Valida que el log no tenga registros ya facturados
                    //    var lstBillLog = billingLogConsultViewDTO.Entities.Where(w => !w.Invoiced && w.BillingFolio <= 0).ToList();

                    //    if (lstBillLog.Count > 0)
                    //    {
                    //        this.Master.ucTaskBar.btnSaveEnabled = true;
                    //    }
                    //    else
                    //    {
                    //        this.Master.ucTaskBar.btnSaveEnabled = false;
                    //    }
                    //}
                    //else
                    //{
                    //    this.Master.ucTaskBar.btnSaveEnabled = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        /// 
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)

        {
            try
            {
                if (ValidateSession(WMSTekSessions.BillingInvoice.BillingLogList))
                {
                    if (billingLogConsultViewDTO.Entities.Count > 0)
                    {
                        billingLogConsultViewDTO = (GenericViewDTO<BillingLogConsult>)Session[WMSTekSessions.BillingInvoice.BillingLogList];

                        DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
                        DropDownList ddlWhs = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
                        TextBox txtFilterDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
                        TextBox txtFilterDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");
                        DateTime dateFrom = Convert.ToDateTime(txtFilterDateFrom.Text.Trim());//.ToString(this.Master.ucMainFilter.FormatDate);
                        DateTime dateTo = Convert.ToDateTime(txtFilterDateTo.Text.Trim());

                        ////Actualiza el invoiced de los log por rango de fecha
                        //GenericViewDTO<BillingLog> upBillingLog = iBillingMGR.UpdateLogInvoided(int.Parse(ddlOwn.SelectedValue), true, dateFrom, dateTo, context);

                        //if (upBillingLog.hasError())
                        //{
                        //    isValidViewDTO = false;
                        //    this.Master.ucError.ShowError(upBillingLog.Errors);
                        //}
                        //else
                        //{

                            string fileName = ddlOwn.SelectedItem.ToString();
                            GenericViewDTO<Owner> ownerViewDTO = iWarehousingMGR.GetOwnerById(context, int.Parse(ddlOwn.SelectedValue));

                            //Buscar los Cobros asociados al contrato
                            BillingTypeContract btc = new BillingTypeContract();
                            btc.BillingContract = new BillingContract();
                            btc.BillingContract.Owner = new Owner(int.Parse(ddlOwn.SelectedValue));
                            GenericViewDTO<BillingTypeContract> btcViewDTO = iBillingMGR.BillingTypeContractGetByAnyParameter(btc, context);

                            //Crea el Nuevo Documento y una Nueva Hoja
                            XLWorkbook workbook = new XLWorkbook();
                            var worksheet = workbook.Worksheets.Add("Resultado Cobro");
                            worksheet.Style.Fill.SetBackgroundColor(XLColor.White);

                            //Setea para toda la pagina el tamaño y tipo del texto 
                            worksheet.Columns().AdjustToContents().Style.Font.SetFontName("Cambria");
                            worksheet.Columns().Style.Font.SetFontSize(10);

                            //llena los DatosCliente en la planilla
                            worksheet.Cell("A4").Value = "CLIENTE:";
                            worksheet.Cell("B4").Value = ownerViewDTO.Entities[0].Name;
                            worksheet.Cell("A5").Value = "RUT:";
                            worksheet.Cell("B5").Value = ownerViewDTO.Entities[0].Code;
                            worksheet.Cell("A6").Value = "DIRECCIÓN:";
                            worksheet.Cell("B6").Value = ownerViewDTO.Entities[0].Address1;
                            worksheet.Cell("A7").Value = "FONO:";
                            worksheet.Cell("B7").Value = ownerViewDTO.Entities[0].Phone1;
                            worksheet.Cell("A8").Value = "EMAIL:";
                            worksheet.Cell("B8").Value = ownerViewDTO.Entities[0].Email;
                            worksheet.Range("A4:A8")
                                     .Style.Font.SetBold(true)
                                     .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            worksheet.Range("B4:B8")
                                     .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                            //Llena Tabla Detalle Moneda
                            TablaDetalleMoneda(worksheet, txtFilterDateFrom.Text, txtFilterDateTo.Text);

                            int iniCell = 10;


                        BillingTransaction billTranstParam = new BillingTransaction();
                        GenericViewDTO<BillingTransaction> billTrasntViewDTO = iBillingMGR.BillingTransactionGetByAnyParameter(billTranstParam, context);


                        //Llena Tabla de Cobros Consolidados Por Transaccion y Realiza la facturacion.
                        //Ademas retorna la posicion inical para crear tabla con los cobros asocidados al contrato.
                        iniCell = TablaConsolidadoCobrosPorTransaccion(worksheet, billingLogConsultViewDTO, btcViewDTO, iniCell, int.Parse(ddlWhs.SelectedValue), int.Parse(ddlOwn.SelectedValue), dateFrom, dateTo, billTrasntViewDTO);


                            //Llena Tabla con Cobros Asociados al Contrato
                            TablaDetalleCobrosContrato(worksheet, btcViewDTO, iniCell);

                            //Llena Tabla con el Detalle de los cobros por fecha
                            TablaDetalleCobrosPorRago(worksheet, billingLogConsultViewDTO, txtFilterDateFrom.Text, txtFilterDateTo.Text, billTrasntViewDTO);


                            UpdateSession(false);
                            PopulateGrid();

                            //Response.Clear();
                            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            //Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

                            //using (var memoryStream = new MemoryStream())
                            //{
                            //    workbook.SaveAs(memoryStream, false);
                            //    memoryStream.WriteTo(Response.OutputStream);
                            //}

                            upGrid.Update();
                            //Response.End();
                            //HttpContext.Current.ApplicationInstance.CompleteRequest();

                            System.Threading.Thread.Sleep(3000);
                            MemoryStream memoryStream = new MemoryStream();
                            workbook.SaveAs(memoryStream, false);
                            //Session["BillingDownloadFile"] = memoryStream;
                            Session.Add(WMSTekSessions.Shared.BillingDownloadFile, memoryStream);

                            iframe.Attributes["src"] = "BillingDownload.aspx?FileName=" + fileName + ".xlsx";


                        //}
                    }
                    else
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorNotLog.Text, "");
                    }
                }
            }
            catch (System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }

        }

        protected void btnConfirmInvoiced_Click(object sender, EventArgs e)
        {
            try
            {
                if (ExistSpecialTagMarval() && countVisitPopUp == 0)
                {
                    SpecialPopup();
                }
                else
                {
                    this.Master.ucDialog.ShowConfirm(this.lblTitle.Text, this.lblMessageConfirmInvoiced.Text, "ConfirmInvoiced");
                }        
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Respuesta desde la ventana de diálogo
        /// </summary>
        protected void btnDialogOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Master.ucDialog.Caller == "ConfirmInvoiced")
                {
                    if (ValidateSession(WMSTekSessions.BillingInvoice.BillingLogList))
                    {
                        DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
                        DropDownList ddlWhs = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
                        TextBox txtFilterDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
                        TextBox txtFilterDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");
                        DateTime dateFrom = Convert.ToDateTime(txtFilterDateFrom.Text.Trim());
                        DateTime dateTo = Convert.ToDateTime(txtFilterDateTo.Text.Trim());

                        context.MainFilter = this.Master.ucMainFilter.MainFilter;
                        ContextViewDTO newContext = new ContextViewDTO();
                        newContext = context;
                        newContext.MainFilter = this.Master.ucMainFilter.MainFilter;

                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DateRange)].FilterValues.Clear();
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DateRange)].FilterValues.Add(new FilterItem(dateFrom.ToString(this.Master.ucMainFilter.FormatDate)));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DateRange)].FilterValues.Add(new FilterItem(dateTo.ToString(this.Master.ucMainFilter.FormatDate)));

                        int index = 0;
                        billingLogConsultViewDTO = (GenericViewDTO<BillingLogConsult>)Session[WMSTekSessions.BillingInvoice.BillingLogList];
                        List<BillingLogConsult> listSelectedInvoices = new List<BillingLogConsult>();

                        for (int i = 0; i < grdMgr.Rows.Count; i++)
                        {
                            GridViewRow row = grdMgr.Rows[i];

                            if (((CheckBox)row.FindControl("chkInvoiceConfirm")).Checked)
                            {
                                index = (grdMgr.PageIndex * grdMgr.PageSize) + i;
                                listSelectedInvoices.Add(billingLogConsultViewDTO.Entities[index]);
                            }
                        }

                        if (listSelectedInvoices.Count == 0)
                        {
                            ucStatus.ShowMessage(lblMustSelectAnyInvoice.Text);
                            return;
                        }

                        billingLogConsultViewDTO.Entities = listSelectedInvoices;

                        if (!billingLogConsultViewDTO.hasError() && billingLogConsultViewDTO.Entities != null)
                        {
                            //Valida que el log no tenga registros ya facturados
                            var lstBillLog = billingLogConsultViewDTO.Entities.Where(w => !w.Invoiced && w.BillingFolio <= 0).ToList();

                            if (lstBillLog.Count > 0)
                            {
                                //Generar Aqui logs del tipo fijo
                                GenericViewDTO<BillingTypeContract> lstBillingtypeContract = iBillingMGR.FindAllFixedTransaction(int.Parse(ddlWhs.SelectedValue), int.Parse(ddlOwn.SelectedValue), dateFrom.ToString(this.Master.ucMainFilter.FormatDate), dateTo.ToString(this.Master.ucMainFilter.FormatDate), context);

                                if (lstBillingtypeContract.Entities != null && lstBillingtypeContract.Entities.Count > 0)
                                {

                                    List<int> billLogCons = billingLogConsultViewDTO.Entities.ToList().Select(con => con.IdBillingTypeContract).ToList();

                                    //Here we get the list of our shifts
                                    List<BillingTypeContract> lstContracType = lstBillingtypeContract.Entities.Where(p => !billLogCons.Contains(p.Id)).ToList();

                                    if (lstContracType.Count > 0)
                                    {
                                        foreach (BillingTypeContract bill in lstContracType)
                                        {

                                            //Insertar BillingLog
                                            BillingLog log = new BillingLog();
                                            GenericViewDTO<BillingTransaction> billingTransactionDTO = new GenericViewDTO<BillingTransaction>();
                                            GenericViewDTO<BillingMoney> billingMoneyDTO = new GenericViewDTO<BillingMoney>();
                                            BillingTransaction billingTransactionParam = new BillingTransaction();

                                            log.Owner = new Owner(Convert.ToInt32(ddlOwn.SelectedValue));
                                            log.Warehouse = bill.Warehouse;
                                            log.BillingContract = bill.BillingContract;
                                            log.BillingTransaction = bill.BillingTransaction;
                                            log.BillingTypeContract = bill;
                                            log.BillingType = bill.BillingType;
                                            log.BillingMoney = bill.BillingMoney;
                                            log.Qty = 1;
                                            log.DateEntry = DateTime.Now;

                                            GenericViewDTO<BillingLog> billLog = iBillingMGR.MaintainLog(CRUD.Create, log, context);

                                        }
                                    }

                                }


                                BillingSpecialData specialData = null;

                                if (ExistSpecialTagMarval())
                                {
                                    specialData = new BillingSpecialData()
                                    {
                                       OcrCodeZone = ddlZone.SelectedValue,
                                       OcrCodeBusiness = ddlBusiness.SelectedValue,
                                       OcrCodeLorry = ddlLorry.SelectedValue,
                                       OcrCodeDepartment = ddlDepartment.SelectedValue
                                    };
                                }

                                GenericViewDTO<BillingMoney> listAllMoney = iBillingMGR.FindAllMoney(context);

                                if (listAllMoney.hasError())
                                {
                                    this.Master.ucError.ShowError(listAllMoney.Errors);
                                }
                                else
                                {
                                    GenericViewDTO<BillingLog> upBillingLog = iBillingMGR.UpdateLogInvoided(int.Parse(ddlOwn.SelectedValue), listAllMoney, listSelectedInvoices, context, specialData);

                                    if (upBillingLog.hasError())
                                    {
                                        this.Master.ucError.ShowError(upBillingLog.Errors);
                                    }
                                    else
                                    {
                                        ReloadData();
                                        countVisitPopUp = 0;

                                        if (ExistSpecialTagMarval())
                                        {
                                            ddlCompany.Items.Clear();
                                            ddlZone.Items.Clear();
                                            ddlBusiness.Items.Clear();
                                            ddlLorry.Items.Clear();
                                            ddlDepartment.Items.Clear();
                                        }
                                    }
                                }   
                            }
                            else
                            {
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorNotLogConfirm.Text, "");
                            }
                        }
                        else
                        {
                            this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }


        //protected void btnExcel_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (ValidateSession(WMSTekSessions.BillingInvoice.BillingLogList))
        //        {
        //            if (billingLogConsultViewDTO.Entities.Count > 0)
        //            {
        //                billingLogConsultViewDTO = (GenericViewDTO<BillingLogConsult>)Session[WMSTekSessions.BillingInvoice.BillingLogList];

        //                DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
        //                TextBox txtFilterDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
        //                TextBox txtFilterDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");
        //                DateTime dateFrom = Convert.ToDateTime(txtFilterDateFrom.Text.Trim());//.ToString(this.Master.ucMainFilter.FormatDate);
        //                DateTime dateTo = Convert.ToDateTime(txtFilterDateTo.Text.Trim());

        //                //Actualiza el invoiced de los log por rango de fecha
        //                GenericViewDTO<BillingLog> upBillingLog = iBillingMGR.UpdateLogInvoided(int.Parse(ddlOwn.SelectedValue), true, dateFrom, dateTo, context);

        //                if (upBillingLog.hasError())
        //                {
        //                    isValidViewDTO = false;
        //                    this.Master.ucError.ShowError(upBillingLog.Errors);
        //                }
        //                else
        //                {

        //                    string fileName = ddlOwn.SelectedItem.ToString();
        //                    GenericViewDTO<Owner> ownerViewDTO = iWarehousingMGR.GetOwnerById(context, int.Parse(ddlOwn.SelectedValue));

        //                    //Buscar los Cobros asociados al contrato
        //                    BillingTypeContract btc = new BillingTypeContract();
        //                    btc.BillingContract = new BillingContract();
        //                    btc.BillingContract.Owner = new Owner(int.Parse(ddlOwn.SelectedValue));
        //                    GenericViewDTO<BillingTypeContract> btcViewDTO = iBillingMGR.BillingTypeContractGetByAnyParameter(btc, context);

        //                    //Crea el Nuevo Documento y una Nueva Hoja
        //                    XLWorkbook workbook = new XLWorkbook();
        //                    var worksheet = workbook.Worksheets.Add("Resultado Cobro");
        //                    worksheet.Style.Fill.SetBackgroundColor(XLColor.White);

        //                    //Setea para toda la pagina el tamaño y tipo del texto 
        //                    worksheet.Columns().AdjustToContents().Style.Font.SetFontName("Cambria");
        //                    worksheet.Columns().Style.Font.SetFontSize(10);

        //                    //llena los DatosCliente en la planilla
        //                    worksheet.Cell("A4").Value = "CLIENTE:";
        //                    worksheet.Cell("B4").Value = ownerViewDTO.Entities[0].Name;
        //                    worksheet.Cell("A5").Value = "RUT:";
        //                    worksheet.Cell("B5").Value = ownerViewDTO.Entities[0].Code;
        //                    worksheet.Cell("A6").Value = "DIRECCIÓN:";
        //                    worksheet.Cell("B6").Value = ownerViewDTO.Entities[0].Address1;
        //                    worksheet.Cell("A7").Value = "FONO:";
        //                    worksheet.Cell("B7").Value = ownerViewDTO.Entities[0].Phone1;
        //                    worksheet.Cell("A8").Value = "EMAIL:";
        //                    worksheet.Cell("B8").Value = ownerViewDTO.Entities[0].Email;
        //                    worksheet.Range("A4:A8")
        //                             .Style.Font.SetBold(true)
        //                             .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        //                    worksheet.Range("B4:B8")
        //                             .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

        //                    //Llena Tabla Detalle Moneda
        //                    TablaDetalleMoneda(worksheet, txtFilterDateFrom.Text, txtFilterDateTo.Text);

        //                    int iniCell = 10;

        //                    //Llena Tabla de Cobros Consolidados por modulo y Realiza la facturacion.
        //                    //Ademas retorna la posicion inical para crear tabla con los cobros asocidados al contrato.
        //                    //int iniCell = TablaConsolidadoCobrosPorModulo(worksheet, billingLogConsultViewDTO, btcViewDTO, 10);

        //                    //Llena Tabla de Cobros Consolidados Por Transaccion y Realiza la facturacion.
        //                    //Ademas retorna la posicion inical para crear tabla con los cobros asocidados al contrato.
        //                    iniCell = TablaConsolidadoCobrosPorTransaccion(worksheet, billingLogConsultViewDTO, btcViewDTO, iniCell);


        //                    //Llena Tabla con Cobros Asociados al Contrato
        //                    TablaDetalleCobrosContrato(worksheet, btcViewDTO, iniCell);

        //                    //Llena Tabla con el Detalle de los cobros por fecha
        //                    TablaDetalleCobrosPorRago(worksheet, billingLogConsultViewDTO, txtFilterDateFrom.Text, txtFilterDateTo.Text);


        //                    UpdateSession(false);
        //                    PopulateGrid();

        //                    Response.Clear();
        //                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        workbook.SaveAs(memoryStream, false);
        //                        memoryStream.WriteTo(Response.OutputStream);
        //                    }

        //                    upGrid.Update();
        //                    //Response.End();

        //                    HttpContext.Current.ApplicationInstance.CompleteRequest();
        //                }
        //            }
        //            else
        //            {
        //                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorNotLog.Text, "");
        //            }
        //        }
        //    }
        //    catch (System.Threading.ThreadAbortException th)
        //    {
        //        //no hace nada
        //    }
        //    catch (Exception ex)
        //    {
        //        billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
        //    }
        //}


        private void TablaDetalleMoneda(IXLWorksheet worksheet, string dateFrom, string dateTo)
        {
            //Datos Moneda y Fecha Cobros
            //worksheet.Cell("E1").Value = "Valor:";
            //worksheet.Cell("F1").Value = "13123";
            worksheet.Cell("E2").Value = "Periodo:";
            worksheet.Cell("F2").Value = dateFrom.Trim();
            worksheet.Cell("G2")
                     .SetValue("Hasta")
                     .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell("H2").Value = dateTo.Trim();
            worksheet.Range("E1:H3")
                     .Style.Font.SetBold(true)
                     .Font.SetFontSize(10);
            worksheet.Range("E1:E2")
                     .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right); ;
            //worksheet.Cell("E1:F1")
            //         .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            //         .Fill.SetBackgroundColor(XLColor.Yellow);
            worksheet.Range("E2:H2")
                     .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                     .Fill.SetBackgroundColor(XLColor.Yellow);

        }

        //private int TablaConsolidadoCobrosPorModulo(IXLWorksheet worksheet, GenericViewDTO<BillingLogConsult> billingLogConsult, GenericViewDTO<BillingTypeContract> btcViewDTO, int inicioCell)
        //{
        //    decimal IVA = (GetConst("ValueIVA") == null || GetConst("ValueIVA").Count() == 0 ? 19 : decimal.Parse(GetConst("ValueIVA")[0]));


        //    //Agrupa la las transacciones generadas
        //    var GroupByTransaction = (from a in billingLogConsult.Entities.ToList()
        //                              select new
        //                              {
        //                                  WhereIn = a.WhereIn
        //                              }).ToList().Distinct();

        //    //Llena Consolidado de los Cobros y calcula el total a facturar
        //    var lstCobrosGroupBy = (from a in billingLogConsultViewDTO.Entities
        //                            from b in btcViewDTO.Entities
        //                            where a.WhereIn.Equals(b.BillingTransaction.WmsProcess.WhereIn) && a.IdBillingType.Equals(b.BillingType.Id) && a.IdBillingTransaction.Equals(b.BillingTransaction.Id)
        //                            group a by new { a.WhereIn, a.IdBillingType, a.BillingTypeName } into g
        //                            select new
        //                            {
        //                                WhereIn = g.Key.WhereIn,
        //                                IdBillingType = g.Key.IdBillingType,
        //                                BillingTypeName = g.Key.BillingTypeName,
        //                                BillingTypeContract = btcViewDTO.Entities.Where(w => w.BillingType.Id.Equals(g.Key.IdBillingType)).First(),
        //                                ValueTypeContract = btcViewDTO.Entities.Where(w => w.BillingType.Id.Equals(g.Key.IdBillingType) && w.BillingTransaction.WmsProcess.WhereIn.Equals(g.Key.WhereIn)).First().Value,
        //                                Qty = g.Sum(x => x.Qty)
        //                            }).ToList();

        //    //Detalle Cobro Facturado
        //    worksheet.Cell("A" + (inicioCell)).SetValue("Modulo").Comment.AddText("Cobros generados por rango de fechas.");
        //    worksheet.Cell("F" + (inicioCell)).Value = "Monto";
        //    worksheet.Range("A" + (inicioCell) + ":F" + (inicioCell))
        //            .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        //            .Font.SetBold(true)
        //            .Fill.SetBackgroundColor(XLColor.AliceBlue);

        //    string comillas = Convert.ToChar(34).ToString();
        //    int contCell = inicioCell + 1;
        //    foreach (var transaction in GroupByTransaction.Distinct())
        //    {
        //        //worksheet.Cell("A" + contCell).Value = transaction.Name;
        //        string rang = "A" + contCell;

        //        foreach (var item in lstCobrosGroupBy.Where(w => w.WhereIn.Equals(transaction.WhereIn)))
        //        {
        //            worksheet.Cell("B" + contCell).Value = item.BillingTypeName;
        //            worksheet.Cell("C" + contCell).SetValue(item.ValueTypeContract);
        //            worksheet.Cell("D" + contCell).Value = item.BillingTypeContract.BillingType.Name;
        //            worksheet.Cell("E" + contCell).SetValue(item.Qty).SetDataType(XLCellValues.Number);
        //            worksheet.Cell("F" + contCell)
        //                    .SetDataType(XLCellValues.Number)
        //                    .SetFormulaA1("(C" + contCell + "*E" + contCell + ")*" + item.BillingTypeContract.BillingMoney.Value)
        //                    .Style.NumberFormat.SetNumberFormatId(42);

        //            contCell++;
        //        }

        //        rang = rang + ":A" + (contCell - 1);
        //        worksheet.Range(rang)
        //                .Merge()
        //                .SetValue(transaction.WhereIn)
        //                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        //    }

        //    worksheet.Range("A" + (inicioCell + 1) + ":F" + (contCell - 1))
        //            .Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
        //            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

        //    worksheet.Cell("E" + (contCell)).SetValue("Total Neto").Style.Font.SetBold(true);
        //    worksheet.Cell("F" + (contCell))
        //            .SetFormulaA1("=SUMA(F" + (inicioCell + 1) + ":F" + (contCell - 1).ToString() + ")")
        //            .Style.NumberFormat.SetNumberFormatId(42)
        //            .Font.SetBold(true);

        //    worksheet.Cell("E" + (contCell + 1))
        //            .SetValue("IVA")
        //            .Style.Font.SetBold(true)
        //            .Fill.SetBackgroundColor(XLColor.Yellow)
        //            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        //            .Border.SetTopBorder(XLBorderStyleValues.Thin);
        //    worksheet.Cell("F" + (contCell + 1))
        //            .SetFormulaA1("=F" + contCell + "*" + IVA + "%")
        //            .Style.NumberFormat.SetNumberFormatId(42)
        //            .Fill.SetBackgroundColor(XLColor.Yellow)
        //            .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        //            .Border.SetTopBorder(XLBorderStyleValues.Thin)
        //            .Font.SetBold(true);

        //    worksheet.Cell("E" + (contCell + 2)).SetValue("Total Factura").Style.Font.SetBold(true);
        //    worksheet.Cell("F" + (contCell + 2))
        //            .SetFormulaA1("=F" + (contCell) + "+F" + (contCell + 1))
        //            .Style.NumberFormat.SetNumberFormatId(42)
        //            .Font.SetBold(true);

        //    return contCell + 3;

        //}

        private int TablaConsolidadoCobrosPorTransaccion(IXLWorksheet worksheet, GenericViewDTO<BillingLogConsult> billingLogConsult, GenericViewDTO<BillingTypeContract> btcViewDTO, int inicioCell, int idWhs, int idOwn, DateTime dateFrom, DateTime dateTo, GenericViewDTO<BillingTransaction> billTrasntViewDTO)
        {
            decimal IVA = (GetConst("ValueIVA") == null || GetConst("ValueIVA").Count() == 0 ? 19 : decimal.Parse(GetConst("ValueIVA")[0]));
                       
            //Generar Aqui logs del tipo fijo
            GenericViewDTO<BillingTypeContract> lstBillingtypeContract = iBillingMGR.FindAllFixedTransaction(idWhs, idOwn, dateFrom.ToString(this.Master.ucMainFilter.FormatDate), dateTo.ToString(this.Master.ucMainFilter.FormatDate), context);

            if (lstBillingtypeContract.Entities != null && lstBillingtypeContract.Entities.Count > 0)
            {

                List<int> billLogCons = billingLogConsult.Entities.ToList().Select(con => con.IdBillingTypeContract).ToList();

                //Here we get the list of our shifts
                List<BillingTypeContract> lstContracType = lstBillingtypeContract.Entities.Where(p => !billLogCons.Contains(p.Id)).ToList();

                if (lstContracType.Count > 0)
                {
                    foreach (BillingTypeContract bill in lstContracType)
                    {
                        //Insertar BillingLogConsult
                        BillingLogConsult logConsult = new BillingLogConsult();

                        logConsult.IdOwn = idOwn;
                        logConsult.IdBillingTransaction = bill.BillingTransaction.Id;
                        logConsult.BillingTransactionName = bill.BillingTransaction.Name;
                        logConsult.IdBillingType = bill.BillingType.Id;
                        logConsult.BillingTypeName = bill.BillingType.Name;
                        logConsult.BillingMoneyValue = bill.BillingMoney.Value;
                        logConsult.BillingTypeContractValue = bill.Value;
                        logConsult.Qty = 1;
                        logConsult.WhereIn = "F";
                        logConsult.DateEntry = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        var existBilling = billingLogConsult.Entities.Exists(w => w.IdOwn == idOwn && w.IdBillingTransaction == bill.BillingTransaction.Id &&
                                           w.BillingTransactionName == bill.BillingTransaction.Name &&
                                           w.IdBillingType == bill.BillingType.Id &&
                                           w.BillingTypeName == bill.BillingType.Name &&
                                           w.BillingMoneyValue == bill.BillingMoney.Value &&
                                           w.BillingTypeContractValue == bill.Value &&
                                           w.WhereIn == "F");

                        if (!existBilling)
                        {
                            billingLogConsult.Entities.Add(logConsult);
                        }
                    }
                }
            }                                          

            //Agrupa los modulos generados
            var GroupByModule = (from a in billingLogConsult.Entities.ToList()
                                 select new
                                 {
                                     OrderByModule = a.WhereIn == "INB" ? "1" :
                                                   a.WhereIn == "INT" ? "2" :
                                                   a.WhereIn == "OUB" ? "3" :
                                                   a.WhereIn == "A" ? "4" :
                                                   a.WhereIn == "F" ? "5" :
                                                   a.WhereIn == "C" ? "6" : "7",
                                     WhereIn = a.WhereIn
                                 }).ToList().Distinct();

            //Agrupa la las transacciones generadas
            var GroupByTransaction = (from a in billingLogConsult.Entities.ToList()
                                      join b in billTrasntViewDTO.Entities.ToList() on a.IdBillingTransaction equals b.Id
                                      select new
                                      {
                                          Id = a.IdBillingTransaction,
                                          Name = a.BillingTransactionName,
                                          WhereIn = a.WhereIn,
                                          OrderByTranst = a.WhereIn == "INB" ? "1" :
                                                   a.WhereIn == "INT" ? "2" :
                                                   a.WhereIn == "OUB" ? "3" :
                                                   a.WhereIn == "A" ? "4" :
                                                   a.WhereIn == "F" ? "5" :
                                                   a.WhereIn == "C" ? "6" : (b.WmsProcess.Code == "1" ? "7" : (b.WmsProcess.Code == "2" ? "8" : "9"))
                                      }).ToList().Distinct();

            var lstCobrosGroupBy = (from a in billingLogConsultViewDTO.Entities
                                    group a by new { a.WhereIn, a.IdBillingTransaction, a.BillingTransactionName, a.IdBillingType, a.BillingTypeName, a.BillingTypeContractValue, a.BillingMoneyValue } into g
                                    select new
                                    {
                                        WhereIn = g.Key.WhereIn,
                                        IdBillingTransaction = g.Key.IdBillingTransaction,
                                        BillingTransactionName = g.Key.BillingTransactionName,
                                        IdBillingType = g.Key.IdBillingType,
                                        BillingTypeName = g.Key.BillingTypeName,
                                        BillingMoneyValue = g.Key.BillingMoneyValue,
                                        BillingTypeContractValue = g.Key.BillingTypeContractValue,
                                        Qty = g.Sum(x => x.Qty)
                                    }).ToList();


            //Detalle Cobro Facturado
            worksheet.Cell("A" + (inicioCell)).SetValue("Módulo");
            worksheet.Cell("B" + (inicioCell)).SetValue("Transacción").Comment.AddText("Transacción en la cual se ejecuta el cobro.");
            worksheet.Cell("C" + (inicioCell)).Value = "Tipo Cobro";
            worksheet.Cell("D" + (inicioCell)).Value = "Valor Contrato";
            worksheet.Cell("E" + (inicioCell)).Value = "Valor Moneda";
            worksheet.Cell("F" + (inicioCell)).Value = "Cantidad";
            worksheet.Cell("G" + (inicioCell)).Value = "Monto";
            worksheet.Range("A" + (inicioCell) + ":G" + (inicioCell))
                    .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.AliceBlue);

            string comillas = Convert.ToChar(34).ToString();
            int contCell = inicioCell + 1;

            foreach (var module in GroupByModule.Distinct().OrderBy(w => w.OrderByModule))
            {
                string rangModule = "A" + contCell;

                foreach (var transaction in GroupByTransaction.Where(w => w.WhereIn.Equals(module.WhereIn)).OrderBy(w=>w.OrderByTranst))
                {
                    string rang = "B" + contCell;

                    foreach (var item in lstCobrosGroupBy.Where(w => w.IdBillingTransaction.Equals(transaction.Id)))
                    {
                        worksheet.Cell("C" + contCell).Value = item.BillingTypeName;
                        worksheet.Cell("D" + contCell).SetValue(item.BillingTypeContractValue);
                        worksheet.Cell("E" + contCell).SetValue(item.BillingMoneyValue)
                            .Style.NumberFormat.SetNumberFormatId(4)
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        worksheet.Cell("F" + contCell).SetValue(item.Qty)
                            .SetDataType(XLCellValues.Number)
                            .Style.NumberFormat.SetNumberFormatId(3);

                        worksheet.Cell("G" + contCell).SetValue(item.BillingTypeContractValue * item.Qty * item.BillingMoneyValue).SetDataType(XLCellValues.Number).Style.NumberFormat.SetNumberFormatId(42);
                        contCell++;
                    }

                    rang = rang + ":B" + (contCell - 1);
                    worksheet.Range(rang)
                            .Merge()
                            .SetValue(transaction.Name)
                            .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);


                }
                string whereIn = ((module.WhereIn == "INB") ? "Entrada" : ((module.WhereIn == "OUB") ? "Salida" : ((module.WhereIn == "INT") ? "Interno" : ((module.WhereIn == "A") ? "Adicional" : (module.WhereIn == "D") ? "Diario" : "Fijo"))));

                rangModule = rangModule + ":A" + (contCell - 1);
                worksheet.Range(rangModule)
                        .Merge()
                        .SetValue(whereIn)
                        .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            }


            worksheet.Range("A" + (inicioCell + 1) + ":G" + (contCell - 1))
                    .Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            worksheet.Cell("F" + (contCell)).SetValue("Total Neto").Style.Font.SetBold(true);
            worksheet.Cell("G" + (contCell))
                    .SetFormulaA1("=SUM(G" + (inicioCell + 1) + ":G" + (contCell - 1).ToString() + ")")
                    .Style.NumberFormat.SetNumberFormatId(42)
                    .Font.SetBold(true);

            worksheet.Cell("F" + (contCell + 1))
                    .SetValue("Impuesto")
                    .Style.Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.Yellow)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetTopBorder(XLBorderStyleValues.Thin);
            worksheet.Cell("G" + (contCell + 1))
                    .SetFormulaA1("=G" + contCell + "*" + IVA + "%")
                    .Style.NumberFormat.SetNumberFormatId(42)
                    .Fill.SetBackgroundColor(XLColor.Yellow)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Font.SetBold(true);

            worksheet.Cell("F" + (contCell + 2)).SetValue("Total Factura").Style.Font.SetBold(true);
            worksheet.Cell("G" + (contCell + 2))
                    .SetFormulaA1("=G" + (contCell) + "+G" + (contCell + 1))
                    .Style.NumberFormat.SetNumberFormatId(42)
                    .Font.SetBold(true);

            return contCell + 3;

        }

        private void TablaDetalleCobrosContrato(IXLWorksheet worksheet, GenericViewDTO<BillingTypeContract> btcViewDTO, int inicioCell)
        {
            // Agrupa la las transacciones generadas
            var GroupByTransaction = (from a in btcViewDTO.Entities.Select(s => s.BillingTransaction)
                                      select new
                                      {
                                          Id = a.Id,
                                          Code = a.Code,
                                          Name = a.Name,
                                          whereIn = a.WmsProcess.WhereIn,
                                          order = a.WmsProcess.WhereIn == "INB" ? "1" :
                                                   a.WmsProcess.WhereIn == "INT" ? "2" :
                                                   a.WmsProcess.WhereIn == "OUB" ? "3" :
                                                   a.WmsProcess.WhereIn == "A" ? "4" :
                                                   a.WmsProcess.WhereIn == "F" ? "5" :
                                                   a.WmsProcess.WhereIn == "C" ? "6" : (a.WmsProcess.Code == "1" ? "7" : (a.WmsProcess.Code == "2" ? "8" : "9")),
                                      }).ToList().Distinct();

            //Detalle Cobro Facturado
            worksheet.Cell("A" + (inicioCell + 1)).SetValue("Tarifado Logístico").Style.Font.SetBold(true);
            worksheet.Cell("A" + (inicioCell + 1)).Comment.AddText("Detalle de tipos de cobro asociados por contrato.");
            worksheet.Cell("B" + (inicioCell + 2)).Value = "Tarifado";
            worksheet.Cell("C" + (inicioCell + 2)).Value = "Valor";
            worksheet.Cell("D" + (inicioCell + 2)).Value = "Moneda";
            worksheet.Cell("E" + (inicioCell + 2)).Value = "Valor Moneda";
            worksheet.Cell("F" + (inicioCell + 2)).Value = "T. de Cobro";
            worksheet.Range("A" + (inicioCell + 2) + ":F" + (inicioCell + 2))
                     .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                     .Font.SetBold(true)
                     .Fill.SetBackgroundColor(XLColor.AliceBlue);

            int contCell = (inicioCell + 3);
            foreach (var transaction in GroupByTransaction.Distinct().OrderBy(d=>d.order))
            {
                //worksheet.Cell("A" + contCell).Value = transaction.Name;
                string rang = "A" + contCell;

                foreach (var item in btcViewDTO.Entities.Where(w => w.BillingTransaction.Id.Equals(transaction.Id)))
                {
                    worksheet.Cell("B" + contCell).Value = item.BillingType.Name;
                    worksheet.Cell("C" + contCell).SetValue(item.Value).SetDataType(XLCellValues.Number);
                    worksheet.Cell("D" + contCell).Value = item.BillingMoney.Description;
                    worksheet.Cell("E" + contCell).SetValue(item.BillingMoney.Value)
                        .SetDataType(XLCellValues.Number)
                        .Style.NumberFormat.SetNumberFormatId(4);
                    worksheet.Cell("F" + contCell).Value = item.BillingTimeType.Description;

                    contCell++;
                }

                rang = rang + ":A" + (contCell - 1);
                worksheet.Range(rang)
                                .Merge()
                                .SetValue(transaction.Name)
                                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            }
            worksheet.Range("A" + (inicioCell + 2) + ":F" + (contCell - 1))
                    .Style.Border.SetInsideBorder(XLBorderStyleValues.Thin)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

        }

        protected void TablaDetalleCobrosPorRago(IXLWorksheet worksheet, GenericViewDTO<BillingLogConsult> billingLogConsult, string dateFrom, string dateTo, GenericViewDTO<BillingTransaction> billTrasntViewDTO)
        {
            //Agrupa los modulos generados
            var GroupByModule = (from a in billingLogConsult.Entities.ToList()
                                 select new
                                 { 
                                     OrderByModule = a.WhereIn == "INB" ? "1" :
                                                   a.WhereIn == "INT" ? "2" :
                                                   a.WhereIn == "OUB" ? "3" :
                                                   a.WhereIn == "A" ? "4" :
                                                   a.WhereIn == "F" ? "5" :
                                                   a.WhereIn == "C" ? "6" : "7", //(b.WmsProcess.Code == "1" ? "7" : (b.WmsProcess.Code == "2" ? "8" : "9")),
                                     WhereIn = a.WhereIn
                                 }).ToList().Distinct();

            // Agrupa la las transacciones generadas
            var GroupByTransaction = (from a in billingLogConsult.Entities.ToList()
                                      join b in billTrasntViewDTO.Entities.ToList() on a.IdBillingTransaction equals b.Id
                                      select new
                                      {
                                          WhereIn = a.WhereIn,
                                          Id = a.IdBillingTransaction,
                                          Name = a.BillingTransactionName,
                                          OrderByTranst = a.WhereIn == "INB" ? "1" :
                                                   a.WhereIn == "INT" ? "2" :
                                                   a.WhereIn == "OUB" ? "3" :
                                                   a.WhereIn == "A" ? "4" :
                                                   a.WhereIn == "F" ? "5" :
                                                   a.WhereIn == "C" ? "6" : (b.WmsProcess.Code == "1" ? "7" : (b.WmsProcess.Code == "2" ? "8" : "9")),
                                      }).ToList().Distinct();

            //Optiene una lista de dias existente entre las fechas ingresadas
            List<DateTime> lstRangoFecha = MiscUtils.getDateRange(dateFrom, dateTo);


            //Agrupo los cobros por transaccion, tipo y por fecha
            var lstCobrosGroupBy = (from a in billingLogConsult.Entities
                                    group a by new { a.IdBillingTransaction, a.BillingTransactionName, a.IdBillingType, a.BillingTypeName, DateEn = new DateTime(a.DateEntry.Year, a.DateEntry.Month, a.DateEntry.Day) } into g
                                    select new
                                    {
                                        IdTransaction = g.Key.IdBillingTransaction,
                                        TransactionName = g.Key.BillingTransactionName,
                                        IdBillingType = g.Key.IdBillingType,
                                        BillingTypeName = g.Key.BillingTypeName,
                                        Date = g.Key.DateEn,
                                        Qty = g.Sum(x => x.Qty)
                                    }).ToList();

            var lstTiposCobro = (from a in billingLogConsult.Entities.ToList()
                                 group a by new { a.IdBillingTransaction, a.IdBillingType, a.BillingTypeName } into g
                                 select new
                                 {
                                     IdTransaction = g.Key.IdBillingTransaction,
                                     IdBillingType = g.Key.IdBillingType,
                                     BillingTypeName = g.Key.BillingTypeName
                                 }).ToList();


            List<object[]> lstArrCobrosFecha = new List<object[]>();

            foreach (var module in GroupByModule.OrderBy(w => w.OrderByModule))
            {
                foreach (var trans in GroupByTransaction.Where(w => w.WhereIn.Equals(module.WhereIn)).OrderBy(w=>w.OrderByTranst))
                {
                    foreach (var item in lstTiposCobro.Where(w => w.IdTransaction.Equals(trans.Id)))
                    {
                        object[] arrQty = new object[lstRangoFecha.Count() + 2];
                        arrQty[0] = trans.Name;
                        arrQty[1] = item.BillingTypeName;

                        for (int i = 0; i < lstRangoFecha.Count(); i++)
                        {
                            if (lstCobrosGroupBy.Exists(w => w.IdTransaction.Equals(trans.Id) && w.IdBillingType.Equals(item.IdBillingType) && w.Date.Equals(lstRangoFecha[i])))
                            {
                                arrQty[i + 2] = lstCobrosGroupBy.Where(w => w.IdTransaction.Equals(trans.Id) && w.IdBillingType.Equals(item.IdBillingType) && w.Date.Equals(lstRangoFecha[i])).First().Qty;
                            }
                            else
                            {
                                arrQty[i + 2] = string.Empty;
                            }
                        }

                        lstArrCobrosFecha.Add(arrQty);
                    }
                }
            }

            worksheet.Cell("I8").Comment.AddText("El orden de los cobros se encuentra definido por los modulos de Entrada, Interno, Salida, Adicional, Fijo y Diario");
            worksheet.Cell("I9")
                        .SetValue("Fecha")
                        .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                        .Font.SetBold(true)
                        .Fill.SetBackgroundColor(XLColor.AliceBlue);
            worksheet.Cell("I10").Value = lstRangoFecha;
            worksheet.Cell("I" + (lstRangoFecha.Count() + 10))
                        .SetValue("Total")
                        .Style.Font.SetBold(true)
                        .Fill.SetBackgroundColor(XLColor.AliceBlue)
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range("I10:I" + (lstRangoFecha.Count() + 10)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            int contCell = 10;
            string auxTipoTransaccion = string.Empty;

            foreach (var cb in lstArrCobrosFecha)
            {
                string cell = string.Empty;
                string rang = string.Empty;
                MiscUtils.toExcelIndexCol(contCell, ref cell);

                for (int i = 0; i < cb.Count(); i++)
                {
                    if (i <= 1)
                    {
                        //Transaccion
                        if (i == 0)
                        {
                            //Falta agrupar bien el nombre de la transaccion en la planilla

                            if (auxTipoTransaccion != (string)cb[i])
                            {
                                auxTipoTransaccion = (string)cb[i];

                                worksheet.Cell(cell + (8 + i))
                                           .SetValue(cb[i])
                                           .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                           .Font.SetBold(true);
                            }

                        }
                        else
                        {
                            //Tipo de Cobro
                            worksheet.Cell(cell + (8 + i))
                                            .SetValue(cb[i])
                                            .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                                            .Font.SetBold(true)
                                            .Fill.SetBackgroundColor(XLColor.AliceBlue);
                        }
                    }
                    else
                    {

                        worksheet.Cell(cell + (8 + i))
                                        .SetValue(cb[i])
                                        .Style.Border.SetRightBorder(XLBorderStyleValues.Thin)
                                        .NumberFormat.SetNumberFormatId(3);

                    }
                }

                worksheet.Cell(cell + (cb.Count() + 8))
                            .SetFormulaA1("=SUM(" + (cell + 8) + ":" + (cell + (cb.Count() + 7)) + ")")
                            .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                            .NumberFormat.SetNumberFormatId(3)
                            .Font.SetBold(true)
                            .Fill.SetBackgroundColor(XLColor.AliceBlue);

                contCell++;

            }


        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chk = (CheckBox)e.Row.FindControl("chkInvoiceConfirm");
                Label label = (Label)e.Row.FindControl("lblBillingFolio");

                if (label.Text.Trim() != "" && label.Text.Trim() != "0")
                {
                    chk.Enabled = false;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("SpecialData");
                if (Page.IsValid)
                {
                    countVisitPopUp++;
                    btnConfirmInvoiced_Click(null, null);
                }  
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlCompany.SelectedValue != "-1")
                {
                    fillDdlZones(ddlCompany.SelectedValue);
                    fillDdlBusiness(ddlCompany.SelectedValue);
                    fillDdlDepartment(ddlCompany.SelectedValue);
                }
                else
                {
                    ddlZone.Items.Clear();
                    ddlZone.Items.Insert(0, new ListItem("Seleccione", "-1"));

                    ddlBusiness.Items.Clear();
                    ddlBusiness.Items.Insert(0, new ListItem("Seleccione", "-1"));

                    ddlDepartment.Items.Clear();
                    ddlDepartment.Items.Insert(0, new ListItem("Seleccione", "-1"));               
                }

                ddlLorry.Items.Clear();
                ddlLorry.Items.Insert(0, new ListItem("Seleccione", "-1"));

                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        protected void ddlZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                fillDdlLorry(ddlCompany.SelectedValue, ddlZone.SelectedValue);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                billingLogConsultViewDTO.ClearError();
            }

            TextBox txtFilterDateFrom = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateFrom");
            TextBox txtFilterDateTo = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDateTo");
            DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");

            DateTime dateFrom = Convert.ToDateTime(txtFilterDateFrom.Text.Trim());
            DateTime dateTo = Convert.ToDateTime(txtFilterDateTo.Text.Trim());

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            ContextViewDTO newContext = new ContextViewDTO();
            newContext = context;
            newContext.MainFilter = this.Master.ucMainFilter.MainFilter;
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.DateRange)].FilterValues.Clear();
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.DateRange)].FilterValues.Add(new FilterItem(dateFrom.ToString(this.Master.ucMainFilter.FormatDate)));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.DateRange)].FilterValues.Add(new FilterItem(dateTo.ToString(this.Master.ucMainFilter.FormatDate)));

            billingLogConsultViewDTO = iBillingMGR.GetByFilterLogConsult(newContext);

            if (!billingLogConsultViewDTO.hasError() && billingLogConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.BillingInvoice.BillingLogList, billingLogConsultViewDTO);
                isValidViewDTO = true;

                //Valida que el log no tenga registros ya facturados
                var lstBillLog = billingLogConsultViewDTO.Entities.Where(w => !w.Invoiced && w.BillingFolio <= 0).ToList();

                if (lstBillLog.Count > 0)
                {
                    this.Master.ucTaskBar.btnSaveEnabled = true;
                }
                else
                {
                    this.Master.ucTaskBar.btnSaveEnabled = false;
                }

                if (!crud)
                    ucStatus.ShowMessage(billingLogConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                this.Master.ucTaskBar.btnSaveEnabled = false;
            }
        }

        private void PopulateLists()
        {
            //if (this.ddlOwner.SelectedValue != "-1")
            //    base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!billingLogConsultViewDTO.hasConfigurationError() && billingLogConsultViewDTO.Configuration != null && billingLogConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, billingLogConsultViewDTO.Configuration);

            grdMgr.DataSource = billingLogConsultViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(billingLogConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.chkDateFromVisible = false;
            this.Master.ucMainFilter.chkDateToVisible = false;
            this.Master.ucMainFilter.warehouseVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirmInvoiced_Click);
            this.Master.ucTaskBar.btnSaveToolTip = this.lblToolTipConfirmInvoiced.Text;
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.btnExcelToolTip = this.lblGenerateExcel.Text;

            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnSaveVisible = true;
            this.Master.ucTaskBar.btnSaveEnabled = false;

            this.Master.ucDialog.BtnOkClick += new EventHandler(btnDialogOk_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        private bool ExistSpecialTagMarval()
        {
            try
            {
                XElement root = readXml();

                return root.Element("OcrCode") == null ? false : true;
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                throw ex;
            }  
        }

        private List<CompanySpecialData> XmlAllCompanies()
        {
            try
            {
                XElement root = readXml(); 
                var companies = from comp in root.Element("OcrCode").Descendants("companies").Descendants("company")
                                select new CompanySpecialData
                                {
                                    Name = (string)comp.FirstAttribute
                                };

                return companies.ToList();
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                throw ex;
            }
        }

        private List<ZoneSpecialData> XmlZonesByCompanyName(string nameCompany)
        {
            try
            {
                XElement root = readXml(); 
                var companies = from comp in root.Element("OcrCode").Descendants("companies").Descendants("company")
                                where (string)comp.Attribute("name") == nameCompany
                                select comp;

                var zones = companies.Descendants("zones").Descendants("zone").ToList()
                    .Select(zone => new ZoneSpecialData()
                    {
                        Name = (string)zone.Attribute("text"),
                        Code = zone.Value
                    });

                return zones.ToList();

            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                throw ex;
            }
        }

        private List<BusinessSpecialData> XmlBusinessByCompanyName(string nameCompany)
        {
            try
            {
                XElement root = readXml(); 
                var companies = from comp in root.Element("OcrCode").Descendants("companies").Descendants("company")
                                where (string)comp.Attribute("name") == nameCompany
                                select comp;

                var businesses = companies.Descendants("businesses").Descendants("business").ToList()
                    .Select(business => new BusinessSpecialData()
                    {
                        Name = (string)business.Attribute("text"),
                        Code = business.Value
                    });

                return businesses.ToList();

            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                throw ex;
            }
        }

        private List<LorrySpecialData> XmlLorriesByCompanyName(string nameCompany, string zone)
        {
            try
            {
                XElement root = readXml(); 
                var companies = from comp in root.Element("OcrCode").Descendants("companies").Descendants("company")
                                where (string)comp.Attribute("name") == nameCompany
                                select comp;

                var lorries = companies.Descendants("lorries").Descendants("lorry").ToList();
                List<LorrySpecialData> list = new List<LorrySpecialData>();

                if (lorries != null)
                {
                    if (zone == "A0000002")
                    {
                        list = lorries.Select(lorry => new LorrySpecialData()
                        {
                            Name = (string)lorry.Attribute("text"),
                            Code = lorry.Value
                        }).ToList();

                        return list;
                    }
                    else
                    {
                        return new List<LorrySpecialData>() { new LorrySpecialData { Code = "", Name = "- Sin Camión -" } };
                    }
                }
                else
                {
                    return new List<LorrySpecialData>() { new LorrySpecialData { Code = "", Name = "- Sin Camión -" } };
                }   
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                throw ex;
            }
        }

        private List<DepartmentSpecialData> XmlDepartmentsByCompanyName(string nameCompany)
        {
            try
            {
                XElement root = readXml(); 
                var companies = from comp in root.Element("OcrCode").Descendants("companies").Descendants("company")
                                where (string)comp.Attribute("name") == nameCompany
                                select comp;

                var departments = companies.Descendants("departments").Descendants("department").ToList()
                    .Select(department => new DepartmentSpecialData()
                    {
                        Name = (string)department.Attribute("text"),
                        Code = department.Value
                    });

                return departments.ToList();

            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
                throw ex;
            }
        }

        private void FillDdlCompanies()
        {
            try
            {
                ddlCompany.DataSource = XmlAllCompanies();
                ddlCompany.DataTextField = "Name";
                ddlCompany.DataValueField = "Name";
                ddlCompany.DataBind();

                ddlCompany.Items.Insert(0, new ListItem("Seleccione", "-1"));
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }      
        }

        private void fillDdlZones(string nameCompany)
        {
            try
            { 
                ddlZone.DataSource = XmlZonesByCompanyName(nameCompany);
                ddlZone.DataTextField = "Name";
                ddlZone.DataValueField = "Code";
                ddlZone.DataBind();

                ddlZone.Items.Insert(0, new ListItem("Seleccione", "-1"));
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        private void fillDdlBusiness(string nameCompany)
        {
            try
            { 
                ddlBusiness.DataSource = XmlBusinessByCompanyName(nameCompany);
                ddlBusiness.DataTextField = "Name";
                ddlBusiness.DataValueField = "Code";
                ddlBusiness.DataBind();

                ddlBusiness.Items.Insert(0, new ListItem("Seleccione", "-1"));
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        private void fillDdlDepartment(string nameCompany)
        {
            try
            {
                ddlDepartment.DataSource = XmlDepartmentsByCompanyName(nameCompany);
                ddlDepartment.DataTextField = "Name";
                ddlDepartment.DataValueField = "Code";
                ddlDepartment.DataBind();

                ddlDepartment.Items.Insert(0, new ListItem("Seleccione", "-1"));
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        private void fillDdlLorry(string nameCompany, string zone)
        {
            try
            {
                ddlLorry.DataSource = XmlLorriesByCompanyName(nameCompany, zone);
                ddlLorry.DataTextField = "Name";
                ddlLorry.DataValueField = "Code";
                ddlLorry.DataBind();

                ddlLorry.Items.Insert(0, new ListItem("Seleccione", "-1"));
            }
            catch (Exception ex)
            {
                billingLogConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogConsultViewDTO.Errors);
            }
        }

        private void SpecialPopup()
        {
            upMarvalData.Update();
            FillDdlCompanies();
            divMarvalData.Visible = true;
            modalPopUp.Show();
        }

        class CompanySpecialData
        {
            public string Name { get; set; }
        }

        class ZoneSpecialData
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        class BusinessSpecialData
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        class DepartmentSpecialData
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        class LorrySpecialData
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }


        #endregion      
    }
}