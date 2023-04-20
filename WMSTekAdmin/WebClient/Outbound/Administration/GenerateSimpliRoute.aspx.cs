using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using System.Windows.Forms;
using System.Linq;
using CheckBox = System.Web.UI.WebControls.CheckBox;
using Label = System.Web.UI.WebControls.Label;
using System.Text;
using Binaria.WMSTek.Framework.Entities.Utility;
using Binaria.WMSTek.DataAccess.Utility;
using TextBox = System.Web.UI.WebControls.TextBox;
using Binaria.WMSTek.AdminApp.Warehousing;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class GenerateSimpliRoute : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<PackageConsult> packageViewDTO = new GenericViewDTO<PackageConsult>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();


        private bool isValidViewDTO = false;

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
        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return -1;
            }

            set { ViewState["index"] = value; }
        }

        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
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
                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    //// Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.ToolTip = this.lblDetailOrder.Text;
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    //e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);

                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadPackageDetail(index);
                }
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                //Rescata el indice de la grilla seleccionado
                int index = int.Parse(this.hdIndexGrd.Value);

                LoadPackageDetail(index);
                //ShowModalCloseOrder(index);

                isValidViewDTO = false;
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");

                }
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            //string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                //int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                //if (index != -1)
                //{
                //    LoadPackageDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }


        protected void LoadPackageDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                string idCode = packageViewDTO.Entities[index].LPN.IdCode;

                stockViewDTO = iDispatchingMGR.GetDetailPackageByIdLpnCode(idCode, context);
                this.lblNroDoc.Text = (packageViewDTO.Entities[index].OutboundOrder.Number);
                this.lblGridDetail.Visible = true;

                if (stockViewDTO != null && stockViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!stockViewDTO.hasConfigurationError() && stockViewDTO.Configuration != null && stockViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, stockViewDTO.Configuration);

                    // Detalle de Recepciones
                    grdDetail.DataSource = stockViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }


        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        /// 
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            ContextViewDTO contextAux = new ContextViewDTO();
            GenericViewDTO<PackageConsult> packageAuxViewDTO = new GenericViewDTO<PackageConsult>();
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgrAux = grdMgr;
                    contextAux.MainFilter = this.Master.ucMainFilter.MainFilter;
                    contextAux.SessionInfo = context.SessionInfo;
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Add(new FilterItem(packageViewDTO.Entities[index].LPN.IdCode));
                    //packageAuxViewDTO = iDispatchingMGR.FindSimpliRoutePackage(contextAux);
                    grdMgrAux.DataSource = packageAuxViewDTO.Entities;
                    List<PackageConsult> listTemp = new List<PackageConsult>();
                    listTemp.Add(packageViewDTO.Entities[index]);
                    grdMgrAux.DataSource = listTemp;

                    grdMgrAux.DataBind();
                    LoadPackageDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                    base.ExportToExcel(grdMgrAux, grdDetail, detailTitle);
                }

                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("chkSelectLpn");

                    if (chk.Checked == true)
                    {
                        isValid = true;
                    }
                }

                if (isValid)
                {
                    this.ShowConfirm(this.lblConfirmCreateSimpliRouteHeader.Text, lblConfirmCreateSimpliRoute.Text);
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblConfirmCreateSimpliRouteHeader.Text, this.lblNotSelectedRow.Text, "confirm");
                }
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }

        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                modalPopUpDialog.Hide();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }

        }

        private List<PackageConsult> GetAllRowsSelected()
        {
            List<PackageConsult> listOutboundOrderSelected = new List<PackageConsult>();


            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                var row = grdMgr.Rows[i];
                var chkSelectOutboundOrder = (CheckBox)row.FindControl("chkSelectLpn");
                //var lblReferenceNumber = (Label)row.FindControl("lblReferenceNumber");
                var lblIdOutboundOrder = (Label)row.FindControl("lblIdOutboundOrder");
                var lblIdWhs = (Label)row.FindControl("lblIdWhs");
                var lblLPN = (Label)row.FindControl("lblLPN");
                var lblDeliveryAddress1 = (Label)row.FindControl("lblDeliveryAddress1");
                var lblOutboundNumber = (Label)row.FindControl("lblOutboundNumber");

                if (chkSelectOutboundOrder.Checked)
                {
                    PackageConsult packageConsultObj = new PackageConsult();
                    packageConsultObj.Warehouse = new Warehouse();
                    packageConsultObj.OutboundOrder = new OutboundOrder();
                    packageConsultObj.LPN = new LPN();
                    packageConsultObj.Warehouse.Id = Int32.Parse(lblIdWhs.Text.Trim());
                    packageConsultObj.OutboundOrder.Owner.Id = Int32.Parse(lblIdWhs.Text.Trim());
                    packageConsultObj.OutboundOrder.Id = Int32.Parse(lblIdOutboundOrder.Text.Trim());
                    packageConsultObj.LPN.Code = lblLPN.Text.Trim();
                    packageConsultObj.OutboundOrder.DeliveryAddress1 = lblDeliveryAddress1.Text.Trim();
                    packageConsultObj.OutboundOrder.Number = lblOutboundNumber.Text.Trim();
                    listOutboundOrderSelected.Add(packageConsultObj);
                }
            }

            return listOutboundOrderSelected;
        }

        public void ShowAlert(string title, string message)
        {
            Encoding encod = Encoding.ASCII;
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            //string script = "setTimeout(ShowMessage('" + title + "','" + message.Replace("'","") + "'),2000);";
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "loadingAjaxStart", "loadingAjaxStart();", true);

                var listElementsSelected = GetAllRowsSelected();
                modalPopUpDialog.Hide();

                try
                {


                    if (listElementsSelected.Count > 0)
                    {
                        
                        
                        var selectedLpns = listElementsSelected.Select(ele => ele.LPN.Code).ToList();
                        ClearFilter("IdLpnCode");
                        CreateFilterByList("IdLpnCode",   selectedLpns );

                        var taskQueueViewDTOExisting = iDispatchingMGR.GetOrdersInQueue(context);

                        bool isInserted = false;
                        bool incorrectLoc = false;
                        bool missingAddress = false;

                        foreach (var elementSelected in listElementsSelected)
                        {
                            // Validacion de que el LPN no este previamente en la lista de tareas
                            var checkExistence = taskQueueViewDTOExisting.Entities.Where(ele => ele.IdLpnCode == elementSelected.LPN.Code).Select(ele => ele.IdLpnCode).ToArray();
                            if(checkExistence.Length > 0) {
                                isInserted = true;
                            } else
                            {


                                /*
                                // Validacion de ubicacion 
                                var stockTemp = iDispatchingMGR.GetStockByPackageConsult(elementSelected,context);
                                if (stockTemp != null && stockTemp.Entities.Count > 0)
                                {
                                    iDispatchingMGR.CreateTaskQueueSimpliRoute(elementSelected, context);
                                }
                                else
                                {
                                    incorrectLoc = true;
                                }
                                */

                                // Validacion de direccion de envio
                                /*
                                if (string.IsNullOrEmpty(elementSelected.OutboundOrder.DeliveryAddress1))
                                {
                                    missingAddress = true;
                                }
                                */
                                OutboundOrder filterOutbound = new OutboundOrder();
                                filterOutbound.Owner.Id = elementSelected.OutboundOrder.Owner.Id;
                                filterOutbound.Number = elementSelected.OutboundOrder.Number;
                                var outboundViewDTOExisting = iDispatchingMGR.GetOutboundByNumberAndOwner(context, filterOutbound);
                                if(!outboundViewDTOExisting.hasError() && outboundViewDTOExisting.Entities != null)
                                {
                                    if (string.IsNullOrEmpty(outboundViewDTOExisting.Entities[0].DeliveryAddress1))
                                    {
                                        missingAddress = true;
                                    }
                                    else
                                    {
                                        iDispatchingMGR.CreateTaskQueueSimpliRoute(elementSelected, context);
                                    }

                                }


                            }


                        }

                        if (isInserted)
                        {
                            ShowAlert("Creación de ruta(s) registrada", "Algunos bultos ya estaban previamente registrados");
                            ucStatus.ShowWarning("Rutas Ingresadas, algunas ya estaban previamente registradas");
                        } else if (incorrectLoc)
                        {
                            ShowAlert("Se presento un problema", "Alguno de los bultos no estan en úbicacion de Anden");
                            ucStatus.ShowWarning("Alguno de los bultos no estan en úbicacion de Anden");
                        }
                        else if (missingAddress)
                        {
                            ShowAlert("Se presento un problema", "Alguno de los bultos no tienen dirección de destino");
                            ucStatus.ShowWarning("Alguno de los bultos no tienen dirección de destino");
                        }
                        else
                        {
                            ShowAlert("Creación de ruta(s) registrada", "Bultos ingresados correctamente");
                            ucStatus.ShowMessage("Bultos ingresados correctamente");
                        }
                        divDetail.Visible = false;
                        UpdateSession();
                        modalPopUpDialog.Hide();
                    }
                }

                catch (Exception ex)
                {
                    packageViewDTO.Errors = baseControl.handleException(ex, context);
                    this.Master.ucError.ShowError(packageViewDTO.Errors);
                }

            }


        }

        protected void btnSaveConfirm_Click(object sender, EventArgs e)
        {
            
        }

        private void ShowConfirm(string title, string message)
        {
            this.divConfirm.Visible = true;

            //this.lblDialogTitle.Text = title;
            //this.divDialogMessage.InnerHtml = message;

            //this.Visible = true;
            modalPopUpDialog.Show();
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "GenerateSimpliRoute";
            //this.Master.ucDialog.BtnOkClick += new EventHandler(btnOkClick);
            //this.Master.ucDialog.BtnCancelClick +=new EventHandler(btnCancelClick);

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            PopulateList();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.PackageList))
                {
                    packageViewDTO = (GenericViewDTO<PackageConsult>)Session[WMSTekSessions.OutboundConsult.PackageList];
                    isValidViewDTO = true;
                }

                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                    isValidViewDTO = false;

                    if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptDetailList))
                    {
                        stockViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.InboundConsult.ReceiptDetailList];
                        isValidViewDTO = true;
                    }

                    if (isValidViewDTO)
                    {
                        PopulateGridDetail();
                    }
                }
            }
        }
       

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.listTrackOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listTrackOutboundType = GetConst("PackagesConsultTrackOutboundType");
            this.Master.ucMainFilter.listOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listOutboundType = GetConst("OutboundType");

            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            this.Master.ucMainFilter.trackOutboundTypeVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblCodeLpn.Text;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirm_Click);
            this.Master.ucTaskBar.BtnDownloadClick += new EventHandler(btnConfirm_Click);
            
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
            this.Master.ucTaskBar.btnDownloadVisible = true;
            this.Master.ucTaskBar.btnDownloadToolTip = this.lblBtnSaveToolTip.Text;
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void PopulateList()
        {
            base.LoadConst(this.ddlTypeRejection, "ReceiptTypeRejection", true, this.Master.EmptyRowText);
            base.LoadConst(this.ddlMotiveRejection, "ReceiptTypeMotive", true, this.Master.EmptyRowText);
        }

        private void UpdateSession()
        {


            // carga consulta de Lpn
            TextBox txtLpnCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.LpnSource)].FilterValues.Clear();

            if (!string.IsNullOrEmpty(txtLpnCode.Text.Trim()))
                context.MainFilter[Convert.ToInt16(EntityFilterName.LpnSource)].FilterValues.Add(new FilterItem(txtLpnCode.Text.Trim()));

            // Carga lista de InboundOrders
            packageViewDTO = iDispatchingMGR.FindSimpliRoutePackage(context);

            if (!packageViewDTO.hasError() && packageViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.PackageList, packageViewDTO);

                isValidViewDTO = true;
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!packageViewDTO.hasConfigurationError() && packageViewDTO.Configuration != null && packageViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, packageViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = null;
            grdMgr.DataBind();
            grdMgr.DataSource = packageViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(packageViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        private void PopulateGridDetail()
        {
            // Configura ORDEN de las columnas de la grilla
            if (!stockViewDTO.hasConfigurationError() && stockViewDTO.Configuration != null && stockViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdDetail, stockViewDTO.Configuration);

            // Detalle de Documentos de Entrada
            grdDetail.DataSource = stockViewDTO.Entities;
            grdDetail.DataBind();

            CallJsGridViewDetail();
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        protected void imbDeleteReceipt_Click(object sender, ImageClickEventArgs e)
        {
          
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('GetReceiptHeaderToConfirm', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDropCustom();", true);
        }
        #endregion
    }
}
