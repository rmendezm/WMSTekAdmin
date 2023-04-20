using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class ReleaseLpnWithoutDocument : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<PackageConsult> packageViewDTO = new GenericViewDTO<PackageConsult>();
        private GenericViewDTO<PackageConsult> selectedPackageViewDTO = new GenericViewDTO<PackageConsult>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<Task> taskViewDTO = new GenericViewDTO<Task>();
        private GenericViewDTO<TaskDetail> taskDetailViewDTO = new GenericViewDTO<TaskDetail>();
        private bool isValidViewDTO = false;
        
        //public GenericViewDTO<PackageConsult> selectedPackageViewDTO
        //{
        //    get
        //    {
        //        if (ValidateViewState("selectedPackageViewDTO"))
        //            return (GenericViewDTO<PackageConsult>)ViewState["selectedPackageViewDTO"];
        //        else
        //            return new GenericViewDTO<PackageConsult>();
        //    }

        //    set { ViewState["selectedPackageViewDTO"] = value; }
        //}

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

        // Propiedad para controlar el indice activo en la grilla
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

        private int currentWhs
        {
            get
            {
                if (ValidateViewState("currentWhs"))
                    return (int)ViewState["currentWhs"];
                else
                    return -1;
            }

            set { ViewState["currentWhs"] = value; }
        }

        private int currentOwn
        {
            get
            {
                if (ValidateViewState("currentOwn"))
                    return (int)ViewState["currentOwn"];
                else
                    return -1;
            }

            set { ViewState["currentOwn"] = value; }
        }

        private String dispatchType
        {
            get
            {
                return "PKLPN";
            }
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

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
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
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    //e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);

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
                    // Capturo la posicion de la fila 
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

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
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

        protected void btnRelease_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    ReleaseOrders();
                    
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


        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
        /// </summary>
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

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;
                bool isValidWhs = true;
                bool isValidOwn = true;
                int index = 0;
                int i = 0;
                int idWhs = -1;
                int idOwn = -1;
                PackageConsult selectedLPN = new PackageConsult();
                selectedPackageViewDTO = new GenericViewDTO<PackageConsult>();

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("chkReleaseConfirm");

                    if (chk.Checked == true)
                    {
                        index = (grdMgr.PageIndex * grdMgr.PageSize) + i;

                        if (idWhs == -1)
                            idWhs = packageViewDTO.Entities[index].Warehouse.Id;
                        else
                        {
                            if (idWhs != packageViewDTO.Entities[index].Warehouse.Id)
                                isValidWhs = false;
                        }
                        if (idOwn == -1)
                            idOwn = packageViewDTO.Entities[index].LPN.Owner.Id;
                        else
                        {
                            if (idOwn != packageViewDTO.Entities[index].LPN.Owner.Id)
                                isValidOwn = false;
                        }

                        isValid = true;
                        currentWhs = idWhs;
                        currentOwn = idOwn;

                        selectedLPN = packageViewDTO.Entities[index];
                        selectedPackageViewDTO.Entities.Add(selectedLPN);
                    }
                    i++;
                }

                Session["selectedPackageViewDTO"] = selectedPackageViewDTO;

                if (isValid)
                {
                    if (!isValidOwn)
                        this.Master.ucDialog.ShowAlert(this.lblConfirmReleaseHeader.Text, this.lblMixedOwner.Text, "confirm");
                    else
                    {
                        if (!isValidWhs)
                            this.Master.ucDialog.ShowAlert(this.lblConfirmReleaseHeader.Text, this.lblMixedWarehouse.Text, "confirm");
                        else
                            this.ShowConfirm(this.lblConfirmReleaseHeader.Text, lblConfirmRelease.Text);
                    }
                    
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblConfirmReleaseHeader.Text, this.lblNotSelectedRelease.Text, "confirm");
                }
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }

        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
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

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);

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

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
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
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Add(new FilterItem(packageViewDTO.Entities[index].LPN.IdCode));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.InboundType)].FilterValues.Clear();
                    foreach (String item in this.Master.ucMainFilter.listInboundType)
                    {
                        contextAux.MainFilter[Convert.ToInt16(EntityFilterName.InboundType)].FilterValues.Add(new FilterItem(item));
                    }
                    
                    packageAuxViewDTO = iDispatchingMGR.FindAllReleaseLpnWithoutDocument(contextAux);
                    grdMgrAux.DataSource = packageAuxViewDTO.Entities;
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

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
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
                divDetail.Visible = false;
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
                divDetail.Visible = false;
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
                divDetail.Visible = false;
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
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //        currentIndex = -1;
        //        divDetail.Visible = false;
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        packageViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(packageViewDTO.Errors);
        //    }
        //}

        //protected void imgbtnSearchVendor_Click(object sender, ImageClickEventArgs e)
        //{
        //    pnlPanelPoUp.Visible = true;
        //    mpeModalPopUpVendor.Show();
        //}
        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReleaseLpnWithoutDocument";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.ReleaseLpnWithoutDocumentList))
                {
                    packageViewDTO = (GenericViewDTO<PackageConsult>)Session[WMSTekSessions.OutboundConsult.ReleaseLpnWithoutDocumentList];
                    isValidViewDTO = true;
                }
                if (isValidViewDTO)
                {
                    // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                }
            }

            
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            //this.Master.ucMainFilter.listTrackInboundType = new System.Collections.Generic.List<String>();
            //this.Master.ucMainFilter.listTrackInboundType = GetConst("ReleaseLpnWithoutDocumentTrackInboundType");
            //this.Master.ucMainFilter.listInboundType = new System.Collections.Generic.List<String>();
            //this.Master.ucMainFilter.listInboundType = GetConst("ReleaseLpnWithoutDocumentInboundType");

            //this.Master.ucMainFilter.trackInboundTypeVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true; 
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblCodeLpn.Text;

            // Configura parámetros de fechas
            //this.Master.ucMainFilter.setDateLabel = true;
            //this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.PackagesDaysBefore;
            //this.Master.ucMainFilter.DateAfter = CfgParameterName.PackagesDaysAfter;
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.TaskDaysAfterQuery;//hoy;
            //this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            //Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            this.Master.ucMainFilter.tabDatesVisible = true;
            this.Master.ucMainFilter.tabDatesHeaderText = this.lblTabLote.Text;
            this.Master.ucMainFilter.lotNumberVisible = true;

            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionOperator = false;
            this.Master.ucMainFilter.tabReceptionTargetLocation = false;
            this.Master.ucMainFilter.tabReceptionTargetLpn = false;
            this.Master.ucMainFilter.tabReceptionTaskPriority = false;
            this.Master.ucMainFilter.tabReceptionItemName = false;
            this.Master.ucMainFilter.tabReceptionItemCode = false;
            this.Master.ucMainFilter.tabReceptionSourceLpn = false;
            this.Master.ucMainFilter.tabLPNVisible = true;
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirm_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
            this.Master.ucTaskBar.btnSaveVisible = true;
            this.Master.ucTaskBar.btnSaveToolTip = this.lblBtnSaveToolTip.Text;
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

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // carga consulta de ExpirationConsult
            TextBox txtLpnCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.LpnSource)].FilterValues.Clear();

            if (!string.IsNullOrEmpty(txtLpnCode.Text.Trim()))
                context.MainFilter[Convert.ToInt16(EntityFilterName.LpnSource)].FilterValues.Add(new FilterItem(txtLpnCode.Text.Trim()));

            //foreach (String item in this.Master.ucMainFilter.listInboundType)
            //{
            //    context.MainFilter[Convert.ToInt16(EntityFilterName.InboundType)].FilterValues.Add(new FilterItem(item));
            //}

            packageViewDTO = iDispatchingMGR.FindAllReleaseLpnWithoutDocument(context);

            if (!packageViewDTO.hasError() && packageViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.ReleaseLpnWithoutDocumentList, packageViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(packageViewDTO.MessageStatus.Message);
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

            // Configura ORDEN de las columnas de la grilla
            if (!packageViewDTO.hasConfigurationError() && packageViewDTO.Configuration != null && packageViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, packageViewDTO.Configuration);

            grdMgr.DataSource = packageViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(packageViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
            
        }

        protected void ReloadData()
        {
            UpdateSession();

            lblGridDetail.Visible = false;
            lblNroDoc.Visible = false;

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
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

        private void ShowConfirm(string title, string message)
        {
            this.ddlLocDock.SelectedIndex = -1;
            //base.LoadLocationsByWhsAndType(this.ddlLocDock, currentWhs, LocationTypeName.DOCK.ToString(), this.Master.EmptyRowText, true);
            List<string> locationType = GetConst("ReleaseLpnWithoutDocumentLocationType");

            if(locationType == null)
            {
                base.LoadLocationsByWhsAndType(this.ddlLocDock, currentWhs, LocationTypeName.DOCK.ToString(), this.Master.EmptyRowText, true);
            }
            else
            {
                base.LoadLocationsByWhsAndListType(this.ddlLocDock, currentWhs, locationType, this.Master.EmptyRowText, true);
            }

            divReleaseDispatch.Visible = true;
            mpReleaseDispatch.Show();

        }

        protected void ReleaseOrders()
        {
            Task taskInfo = new Task();

            // Información general de todas las Tareas a generar
            if (divUserNbr.Visible)
                taskInfo.WorkersRequired = Convert.ToInt16(txtUserNbr.Text);
            else
                taskInfo.WorkersRequired = 1;

            taskInfo.WorkersAssigned = 0;

            taskInfo.Priority = Convert.ToInt16(txtPriority.Text);
            taskInfo.StageTarget = new Location(this.ddlLocDock.SelectedValue);
            taskInfo.Warehouse = new Warehouse(currentWhs);
            taskInfo.Owner = new Owner(currentOwn);
            taskInfo.IsComplete = false;
            taskInfo.Status = true;
            taskInfo.IdTrackTaskType = (int)TrackTaskTypeName.Liberada;
            taskInfo.TypeCode = dispatchType;

            List<LPN> lstLpn = new List<LPN>();
            TaskDetail taskDetail;
            selectedPackageViewDTO = (GenericViewDTO<PackageConsult>)Session["selectedPackageViewDTO"];

            System.Collections.Generic.Dictionary<string,string> subQuery = new Dictionary<string, string>();

            subQuery.Add("SubQueryCode", "NotComplete");



            foreach (var item in selectedPackageViewDTO.Entities)
            {
                taskDetail = new TaskDetail();
                taskDetail.IdLpnSourceProposal = item.LPN.IdCode;
                taskDetailViewDTO = iTasksMGR.GetTaskDetailByAnyParameter(context, taskDetail, subQuery);

                if (taskDetailViewDTO.hasError())
                {
                    this.Master.ucDialog.ShowAlert(this.lblConfirmReleaseHeader.Text, this.lblTaskLpn.Text, "confirm");
                    return;
                }

                lstLpn.Add(item.LPN);
            }

            taskViewDTO = iDispatchingMGR.CreateDispatchTaskLpnNotOrder(taskInfo, lstLpn, context);

            //Muestra mensaje en la barra de status
            crud = true;
            ucStatus.ShowMessage(taskViewDTO.MessageStatus.Message);

            // Actualiza la lista de Ordenes Pendientes
            if (taskViewDTO.hasError())
            {
                //UpdateSession(true);
                isValidViewDTO = false;
                this.Master.ucError.ShowError(taskViewDTO.Errors);
                divReleaseDispatch.Visible = true;
                mpReleaseDispatch.Show();
            }
            else
            {
                divReleaseDispatch.Visible = false;
                mpReleaseDispatch.Hide();

                OutboundOrder outbound = new OutboundOrder();
                GenericViewDTO<OutboundOrder> outboundDTO = new GenericViewDTO<OutboundOrder>();
                outbound.Id = taskViewDTO.Entities[0].IdDocumentBound;
                outboundDTO = iDispatchingMGR.GetOutboundByAnyParameter(context, outbound, null);

                this.Master.ucDialog.ShowAlert(this.lblConfirmReleaseHeader.Text, this.lblConfirmedRelease.Text + " " + outboundDTO.Entities[0].Number.ToString(), "confirm");

                //this.Master.ucMainFilter.Initialize(false, true);
                var idOwn = this.Master.ucMainFilter.idOwn;
                var idWhs = this.Master.ucMainFilter.idWhs;
                CreateFilterByList("Owner", new List<int> { idOwn });
                CreateFilterByList("Warehouse", new List<int> { idWhs });
                UpdateSession();

                // Limpia
                selectedPackageViewDTO = new GenericViewDTO<PackageConsult>();
                //Initialize();

            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('GetStockByIdLPNCode', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }

        #endregion

        protected void btnCancelRelease_Click(object sender, EventArgs e)
        {
            try
            {
                divReleaseDispatch.Visible = false;
                mpReleaseDispatch.Hide();
                InitializeTaskBar();
            }
            catch (Exception ex)
            {
                packageViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(packageViewDTO.Errors);
            }
        }
    }
}