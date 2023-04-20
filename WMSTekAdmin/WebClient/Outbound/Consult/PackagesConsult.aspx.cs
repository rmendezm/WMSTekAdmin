using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class LpnConsult : BasePage
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
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
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
                    packageAuxViewDTO = iDispatchingMGR.FindAllPackage(contextAux);
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

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
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
            context.SessionInfo.IdPage = "PackagesConsult";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.PackageList))
                {
                    packageViewDTO = (GenericViewDTO<PackageConsult>)Session[WMSTekSessions.OutboundConsult.PackageList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.listTrackOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listTrackOutboundType = GetConst("PackagesConsultTrackOutboundType");
            this.Master.ucMainFilter.listOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listOutboundType = GetConst("OutboundType");


            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.documentVisible = true;
            //this.Master.ucMainFilter.dateFromVisible = true;
            //this.Master.ucMainFilter.dateToVisible = true;
            //this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.trackOutboundTypeVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblCodeLpn.Text;
            //this.Master.ucMainFilter.dateVisible = true;

            // Configura parámetros de fechas
            //this.Master.ucMainFilter.setDateLabel = true;
            //this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.PackagesDaysBefore;
            //this.Master.ucMainFilter.DateAfter = CfgParameterName.PackagesDaysAfter;
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.TaskDaysAfterQuery;//hoy;
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            //Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabDispatchingVisible = true;
            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionOperator = false;
            this.Master.ucMainFilter.tabReceptionTargetLocation = false;
            this.Master.ucMainFilter.tabReceptionTargetLpn = false;
            this.Master.ucMainFilter.tabReceptionTaskPriority = false;
            this.Master.ucMainFilter.tabReceptionItemName = false;
            this.Master.ucMainFilter.tabReceptionItemCode = false;
            this.Master.ucMainFilter.divDispatchingPriorityVisible = false;
            this.Master.ucMainFilter.tabReceptionSourceLpn = false;
            this.Master.ucMainFilter.tabReceptionReferenceNbr = true;
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

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
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


            packageViewDTO = iDispatchingMGR.FindAllPackage(context);

            if (!packageViewDTO.hasError() && packageViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.PackageList, packageViewDTO);
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

            
            ucStatus.ShowRecordInfo(packageViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
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

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('GetStockByIdLPNCode', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }
        #endregion
    }
}
