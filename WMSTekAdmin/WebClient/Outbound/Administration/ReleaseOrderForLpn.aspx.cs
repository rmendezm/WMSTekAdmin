using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Utils;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Web.Services;
using Binaria.WMSTek.AdminApp.Manager;
using System.IO;
using Binaria.WMSTek.Framework.Entities;
using Binaria.WMSTek.Framework.Entities.Layout;
using System.Drawing;
using Binaria.WMSTek.Framework.Base;



namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class ReleaseOrderForLpn : BasePage
    {
        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> selectedLpnViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<OutboundDetail> outboundDetailViewDTO = new GenericViewDTO<OutboundDetail>();
        private GenericViewDTO<OutboundDetail> outboundDetailPopUpViewDTO = new GenericViewDTO<OutboundDetail>();
        private GenericViewDTO<OutboundDetail> selectedDetailViewDTO = new GenericViewDTO<OutboundDetail>();

        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> lpnViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> lpnDetailViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> detailOrderViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> detailOrderpopUpViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
        private GenericViewDTO<Task> taskViewDTO = new GenericViewDTO<Task>();

        private List<bool> checkedLpnCurrentView = new List<bool>();
        private List<bool> selectedLpnCurrentView = new List<bool>();
        private bool isValidViewDTO = false;
        
        private class itemQty
        {
            private DateTime fifoDate = DateTime.MinValue.Date;
            private DateTime expirationDate = DateTime.MinValue.Date;
            private DateTime fabricationDate = DateTime.MinValue.Date;
            private CategoryItem categoryItem = new CategoryItem(-1);
            public int idItem {get;set;}
            public decimal qty { get; set; }
            public decimal qtyLpn { get; set; }              
            public string lot { get; set; }

            public DateTime FabricationDate
            {
                get
                {
                    return fabricationDate.Date;
                }
                set
                {
                    fabricationDate = value;
                }
            }

            public DateTime ExpirationDate
            {
                get
                {
                    return expirationDate.Date;
                }
                set
                {
                    expirationDate = value;
                }
            }

            public DateTime FifoDate
            {
                get
                {
                    return fifoDate.Date;
                }
                set
                {
                    fifoDate = value;
                }
            }

            public CategoryItem CategoryItem
            {
                get
                {
                    return categoryItem;
                }
                set
                {
                    categoryItem = value;
                }
            }
        }

        // Propiedad para controlar el indice activo en la grilla
        private int currentIndex
        {
            get
            {
                if (ValidateViewState("indexOrder"))
                    return (int)ViewState["indexOrder"];
                else
                    return -1;
            }

            set { ViewState["indexOrder"] = value; }
        }       

        private int currentIndexLpn
        {
            get
            {
                if (ValidateViewState("currentIndexLpn"))
                    return (int)ViewState["currentIndexLpn"];
                else
                    return -1;
            }

            set { ViewState["currentIndexLpn"] = value; }
        }

        private int currentSelectedIndex
        {
            get
            {
                if (ValidateViewState("currentSelectedIndex"))
                    return (int)ViewState["currentSelectedIndex"];
                else
                    return -1;
            }

            set { ViewState["currentSelectedIndex"] = value; }
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

        private bool releaseEnabled
        {
            get
            {
                if (ValidateViewState("releaseEnabled"))
                    return (bool)ViewState["releaseEnabled"];
                else
                    return false;
            }

            set { ViewState["releaseEnabled"] = value; }
        }

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

        private int selectedPage
        {
            get { return (int)(ViewState["selectedPage"] ?? 0); }
            set { ViewState["selectedPage"] = value; }
        }

        private String dispatchType
        {
            get            
            {                
                return "PKLPN";
            }
        }

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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        //protected override void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (base.webMode == WebMode.Normal)
        //        {
        //            base.Page_Load(sender, e);
        //            //SaveCheckedRows();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
        //    }
        //}

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
                        LoadOutboundOrderDetails();
                        LoadOutboundOrderDetailsPopUp();
                        PopulateGridLpn();
                        LoadLpnDetail();
                        //UpdateGridLayout();
                    }
                }

                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReleaseOrderForLpn";
            
            InitializeSplitters();
            InitializeTaskBar();
            InitializeStatusBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeGrid();

            this.Master.ucError.BtnCloseErrorClick += new EventHandler(ucError_BtnCloseErrorClick);

            if (!Page.IsPostBack)
            {
                //hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                InitializeSession();
                UpdateSession(false);               
            }
            else
            {
                // Recupera Ordenes pendientes                
                if (ValidateSession(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListOrders))
                {
                    isValidViewDTO = true;
                    outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListOrders];
                }

                if (ValidateSession(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListLpn))
                {
                    lpnViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListLpn];
                }


            }

            //// Recupera lista de Ordenes seleccionadas hasta el momento para la Simulación            
            //if (ValidateSession(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedOrders))
            //{
            //    selectedOrdersViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedOrders];
            //}

            

            // Inicializa array de Ordenes seleccionadas en la vista actual
            InitializeCheckedRows();
            InitializeSelectedRows();

            if (ValidateSession(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn))
            {
                selectedLpnViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn];
            }

            if (ValidateSession(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListDetailOrdenSelected))
            {
                outboundDetailPopUpViewDTO = (GenericViewDTO<OutboundDetail>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListDetailOrdenSelected];
            }

            //SaveSelectedRowsLpn();

            // Es necesario cargar las grillas SIEMPRE para evitar problemas con el orden dinamico de las columnas
            PopulateGrid();
            //PopulateSelectedGrid();
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

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            outboundOrderViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Muestra filtro avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            //// Configura Filtro Básico
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;            
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            //this.Master.ucMainFilter.lpnCodeVisible = true;
            this.Master.ucMainFilter.outboundTypeNotIncludeAll = false;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.listOutboundType = new List<String>();
            this.Master.ucMainFilter.listOutboundType = GetConst("OutboundOrderTypeForDispatchLpn");
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            // Configura Filtro Avanzado

            this.Master.ucMainFilter.tabItemGroupVisible = true;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.dateLabel = this.lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridLpn()
        {
            grdLpn.PageSize = 100;//Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdLpn.EmptyDataText = this.Master.EmptyGridText;
        }
        
        
        protected void InitializeSession()
        {
            Session.Remove(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListOrders);
            Session.Remove(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListOrdersDetail);
            Session.Remove(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListLpn);
            Session.Remove(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListLpnDetail);
            Session.Remove(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn);
        }

        /// <summary>
        /// Configuración inicial de los Paneles
        /// </summary>
        private void InitializeSplitters()
        {
            // Splitter vertical
            //hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
            //hsMasterDetail.WidthAfter = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .5);
            //hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
        }

    

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                //InitializeSelectedOrders();
                ReloadData();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnCancelReleaseLpn_Click(object sender, EventArgs e)
        {
            // Valida variable de sesion del Usuario Loggeado
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                //divGrid.Visible = true;
                //divModal.Visible = false;
            }
        }

        protected void btnCancelLpn_Click(object sender, EventArgs e)
        {
            // Valida variable de sesion del Usuario Loggeado
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                this.divReleaseDispatch.Visible = true;
                this.mpReleaseDispatch.Show();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Valida variable de sesion del Usuario Loggeado
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                HideDetails();
                this.divReleaseDispatch.Visible = false;
                this.mpReleaseDispatch.Hide();
            }
        }
        

        /// <summary>
        /// Respuesta desde la ventana de diálogo
        /// </summary>
        protected void btnRelease_Click(object sender, EventArgs e)
        {
            try
            {               
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    selectedLpnViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn];

                    if (selectedLpnViewDTO == null || selectedLpnViewDTO.Entities.Count == 0)
                    {
                        //this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoLpnSelected.Text, string.Empty);
                        ShowAlertLpn(this.lblTitle.Text, this.lblNoLpnSelected.Text);
                                                
                        this.divReleaseDispatch.Visible = true;
                        this.mpReleaseDispatch.Show();
                    }
                    else
                    {

                        //Nuevas Ubicaciones de Anden y Embalaje
                        this.ddlLocStageDispatch.SelectedIndex = -1;
                        this.ddlLocDock.SelectedIndex = -1;
                        base.LoadLocationsByWhsAndType(this.ddlLocStageDispatch, int.Parse(this.hidIdWhs.Value), LocationTypeName.STGD.ToString(), this.Master.EmptyRowText, true);
                        base.LoadLocationsByWhsAndType(this.ddlLocDock, int.Parse(this.hidIdWhs.Value), LocationTypeName.DOCK.ToString(), this.Master.EmptyRowText, true);

                        this.divReleaseDispatch.Visible = false;
                        this.mpReleaseDispatch.Hide();

                        //this.divLocRelease.Visible = true;
                        //ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "index();", true);
                        this.mpeLocRelease.Show();

                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnReleaseLpn_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (this.ddlLocStageDispatch.SelectedValue == "-1" &&
                               this.ddlLocDock.SelectedValue == "-1")
                    {                        
                        this.lblErrorMessage.Text = this.lblMsgErrorUbic.Text;
                        this.divErrorMessage.Visible = true;
                        this.divReleaseDispatch.Visible = true;
                        this.mpReleaseDispatch.Show();
                        this.divLocRelease.Visible = true;
                        this.mpeLocRelease.Show();

                    }
                    else
                    {
                        ReleaseOrder();
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }


        }

        public void ShowAlertLpn(string title, string message)
        {
            string script = "ShowMessage('" + title + "','" + message + "');";
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        protected void btnCloseMessage_Click(object sender, EventArgs e)
        {
            this.divReleaseDispatch.Visible = true;
            this.mpReleaseDispatch.Show();

            //this.divModalPopUpDialog.Visible = false;
            //this.modalPopUpDialog.Hide();
        }

        protected void btnSearchLpn_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    context.MainFilter[Convert.ToInt16( EntityFilterName.LpnCode)].FilterValues.Clear();
                    if (this.txtLpnCode.Text.Trim() != "")
                    {
                        context.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Add(new FilterItem(this.txtLpnCode.Text.Trim()));
                    }

                    lpnViewDTO = iWarehousingMGR.GetLpnByIdOutboundOrderIdWhsIdOwn(int.Parse(this.txtIdOutboundOrder.Text.Trim()),
                         int.Parse(this.hidIdWhs.Value.Trim()), int.Parse(this.hidIdOwn.Value.Trim()), context);

                    if (lpnViewDTO.Entities != null && lpnViewDTO.Entities.Count > 0)
                    {
                        List<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> newListStock = new List<Framework.Entities.Warehousing.Stock>();
                        foreach (var st in lpnViewDTO.Entities)
                        {
                            if (!newListStock.Select(s => s.Lpn.IdCode).Contains(st.Lpn.IdCode)) 
                            {
                                newListStock.Add(st);
                            }
                        }

                        lpnViewDTO.Entities = newListStock;
                    }

                    if (!lpnViewDTO.hasError() && lpnViewDTO.Entities != null)
                    {
                        Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListLpn] = lpnViewDTO;

                    }
                    // Encabezado de Recepciones
                    grdLpn.DataSource = lpnViewDTO.Entities;
                    grdLpn.DataBind();

                    this.divReleaseDispatch.Visible = true;
                    this.mpReleaseDispatch.Show();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
       

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Oculta detalle de las grillas
            HideDetails();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
                outboundOrderViewDTO.ClearError();
            }

            // Carga lista de Pedidos Pendientes
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            outboundOrderViewDTO = iDispatchingMGR.GetPendingOutboundOrderFilter(context, dispatchType);

            if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities != null)
            {
                
                Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListOrders] = outboundOrderViewDTO;
                                    
                isValidViewDTO = true;
                InitializeSelectedRows();
                SaveSelectedRowsLpn();

                if (!crud)
                    ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Oculta detalles de las grillas
        /// </summary>
        protected void HideDetails()
        {
            currentIndex = -1;
            currentIndexLpn = -1;
            currentSelectedIndex = -1;
            divDetail.Visible = false;
        }

       
        private void PopulateGrid()
        {
            grdMgr.EmptyDataText = this.Master.EmptyGridText;

            //Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(outboundOrderViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        

        private void PopulateGridLpn()
        {
            try
            {

                grdLpn.EmptyDataText = this.Master.EmptyGridText;

                //Configura ORDEN y VISIBILIDAD de las columnas de la grilla
                //if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                //    base.ConfigureGridByProperties(grdMgr, outboundOrderViewDTO.Configuration);

                // Encabezado de Recepciones
                grdLpn.DataSource = lpnViewDTO.Entities;
                grdLpn.DataBind();
                grdLpn.SelectedIndex = currentIndexLpn;

                //grdLpn.Rows[currentIndex].TabIndex =16;

                if (grdLpn.Rows.Count > 0)
                {
                    if (ValidateSession(WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn))
                    {
                        selectedLpnViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn];

                        foreach (Binaria.WMSTek.Framework.Entities.Warehousing.Stock item in selectedLpnViewDTO.Entities)
                        {
                            foreach (GridViewRow row in grdLpn.Rows)
                            {
                                Label lbl = row.FindControl("lblIdLpnCode") as Label;

                                if (lbl.Text.Trim() == item.Lpn.Code)
                                {
                                    CheckBox check = row.FindControl("chkSelectOrder") as CheckBox;
                                    check.Checked = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        private void PopulateGridDetailPopUp()
        {
            grdDetailPopUp.EmptyDataText = this.Master.EmptyGridText;

            //Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridByProperties(grdMgr, outboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdDetailPopUp.DataSource = outboundDetailPopUpViewDTO.Entities;
            grdDetailPopUp.DataBind();            
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
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    
                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }

                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndex = grdMgr.SelectedIndex + (grdMgr.PageIndex * grdMgr.PageSize);
                    LoadOutboundOrderDetails();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "ReleaseLPN")
                {
                    grdLpn.EmptyDataText = this.Master.NoDetailsText;
                    grdLpnDetail.EmptyDataText = this.Master.NoDetailsText;

                    grdLpn.DataSource = null;
                    grdLpn.DataBind();

                    grdLpnDetail.DataSource = null;
                    grdLpnDetail.DataBind();

                    lpnViewDTO.Entities = new List<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
                    selectedLpnViewDTO.Entities = new List<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();

                    currentSelectedIndex = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                    this.txtIdOutboundOrder.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].Id.ToString();
                    this.txtOutboundNumber.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].Number;
                    this.txtNameWarehouse.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].Warehouse.ShortName;
                    this.txtNameOwner.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].Owner.Name;
                    this.txtOutboundType.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].OutboundType.Name;
                    this.txtCustomer.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].CustomerName;
                    this.txtEmissionDate.Text = outboundOrderViewDTO.Entities[currentSelectedIndex].EmissionDate.ToShortDateString(); ;

                    this.hidIdWhs.Value = outboundOrderViewDTO.Entities[currentSelectedIndex].Warehouse.Id.ToString();
                    this.hidIdOwn.Value = outboundOrderViewDTO.Entities[currentSelectedIndex].Owner.Id.ToString();
                    this.hidCustomerCode.Value = outboundOrderViewDTO.Entities[currentSelectedIndex].CustomerCode;

                    LoadOutboundOrderDetailsPopUp();

                    this.txtPrecent.Text = "0%";
                    this.txtLpnCode.Text = "";
                    this.txtLpnCode.Focus();
                    this.divReleaseDispatch.Visible = true;
                    this.mpReleaseDispatch.Show();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void chkSelOrder_CheckedChanged( object sender, EventArgs e)
        {
            //int editIndex = (Convert.ToInt32(grdLpn.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

            // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
            for (int i = 0; i < grdLpn.Rows.Count; i++)
            {
                if (checkedLpnCurrentView.Count > (grdLpn.PageIndex * grdMgr.PageSize) + i)
                {
                    GridViewRow row = grdMgr.Rows[i];
                    checkedLpnCurrentView[(grdLpn.PageIndex * grdLpn.PageSize) + i] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;

                    // Valida que se haya seleccionado al menos una orden
                    //if (checkedLpnCurrentView[(grdLpn.PageIndex * grdLpn.PageSize) + i]) existSelected = true;
                }
            }
        }


        protected void grdLpn_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdLpn.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdLpn.ClientID + "')");

                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes

                    e.Row.Cells[0].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdLpn, "CheckLpn$" + e.Row.RowIndex);
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdLpn, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdLpn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //int editIndex = (Convert.ToInt32(grdSearchItems.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));
                    currentIndexLpn = grdLpn.SelectedIndex + (grdLpn.PageIndex * grdLpn.PageSize);
                    LoadLpnDetail();

                    this.divReleaseDispatch.Visible = true;
                    this.mpReleaseDispatch.Show();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdLpn_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "CheckLpn")
                {
                    GenericViewDTO<OutboundDetail> outboundDetailView = new GenericViewDTO<OutboundDetail>();
                    GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> lpnDetailView = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
                    int idOutboundOrder = int.Parse(txtIdOutboundOrder.Text.Trim());
                    bool existLpn = false;
                    int index = int.Parse(e.CommandArgument.ToString());
                    string idCodeLpn = lpnViewDTO.Entities[index].Lpn.IdCode;
                    decimal percent = 0;
                                       
                    foreach (Binaria.WMSTek.Framework.Entities.Warehousing.Stock item in selectedLpnViewDTO.Entities)
                    {
                        if ( idCodeLpn== item.Lpn.Code)
                        {
                            existLpn = true;
                        }
                    }

                    //Valida el Lpn Existe para eliminarlo
                    if (existLpn)
                    {
                        selectedLpnViewDTO.Entities.Remove(lpnViewDTO.Entities[index]);
                    }

                    //Reacata el detalle del documento
                    outboundDetailView = outboundDetailPopUpViewDTO;                    
                    //outboundDetailView = iDispatchingMGR.GetDetailByIdOutboundWithStock(context, idOutboundOrder);
                    decimal qtyOutbound = outboundDetailView.Entities.Sum(f => f.ItemQty);

                    //Creamos una lista con los items del documento
                    List<itemQty> lstItemsQtyOutbound = (from d in outboundDetailView.Entities
                                                        select new itemQty() 
                                                        { idItem = d.Item.Id, 
                                                          lot = d.LotNumber, 
                                                          FifoDate = d.FifoDate,
                                                          FabricationDate = d.FabricationDate,
                                                          ExpirationDate = d.ExpirationDate,
                                                          CategoryItem = d.CategoryItem,
                                                          qty = d.ItemQty, 
                                                          qtyLpn = 0 
                                                        }).ToList();
                    	

                    foreach (var lp in selectedLpnViewDTO.Entities)
                    {
                        lpnDetailView = iDispatchingMGR.GetDetailPackageByIdLpnCode(lp.Lpn.Code, context);

                        foreach (itemQty item in lstItemsQtyOutbound)
                        {
                            item.qtyLpn += (lpnDetailView.Entities.Where(s => s.Item.Id.Equals(item.idItem)
                                            && (item.lot == null || s.Lot == item.lot)
                                            && (item.FifoDate == DateTime.MinValue.Date || s.FifoDate == item.FifoDate)
                                            && (item.FabricationDate == DateTime.MinValue.Date || s.FabricationDate == item.FabricationDate)
                                            && (item.ExpirationDate == DateTime.MinValue.Date || s.ExpirationDate == item.ExpirationDate)
                                            && (item.CategoryItem.Id == -1 || s.CategoryItem.Id == item.CategoryItem.Id)).Sum(f => f.Qty));
                        }
                    }

                    bool itemExceeds = false;

                    if (!existLpn)
                    {
                        lpnDetailView = iDispatchingMGR.GetDetailPackageByIdLpnCode(idCodeLpn, context);
                                                
                        foreach (itemQty item in lstItemsQtyOutbound)
                        {
                            decimal sumQty = (item.qtyLpn + (lpnDetailView.Entities.Where(s => s.Item.Id.Equals(item.idItem) 
                                && (item.lot == null || s.Lot == item.lot)
                                && (item.FifoDate == DateTime.MinValue.Date || s.FifoDate == item.FifoDate)
                                && (item.FabricationDate == DateTime.MinValue.Date || s.FabricationDate == item.FabricationDate)
                                && (item.ExpirationDate == DateTime.MinValue.Date || s.ExpirationDate == item.ExpirationDate)
                                && (item.CategoryItem.Id == -1 || s.CategoryItem.Id == item.CategoryItem.Id)).Sum(f => f.Qty)));

                            if (item.qty < sumQty)
                            {
                                itemExceeds = true;                                    
                            }

                            if (!itemExceeds)
                            {
                                item.qtyLpn = sumQty;
                            }
                        }                       
                    }

                    if (!itemExceeds)
                    {
                        //Valida el Lpn Existe
                        if (!existLpn)
                        {
                            selectedLpnViewDTO.Entities.Add(lpnViewDTO.Entities[index]);
                        }
                    }
                    else
                    {
                        ShowAlertLpn(this.lblTitle.Text, this.lblErrorLpnQty.Text);
                    }
                  
                    //Calcula el porcentaje de satisfaccion
                    percent = Math.Round((lstItemsQtyOutbound.Sum(s=>s.qtyLpn) * 100) / qtyOutbound);

                    Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn] = selectedLpnViewDTO;

                    string colorText = "";
                    if (percent < 50)
                    {
                        colorText = "#FF3300";
                    }
                    else if (percent >= 50 && percent <= 75)
                    {
                        colorText = "#FFCC00";
                    }
                    else if (percent > 75)
                    {
                        colorText = "#009900";
                    }

                    this.txtPrecent.Style.Add("color",colorText);
                    this.txtPrecent.Text = Math.Round(percent) + "%";
                    this.divReleaseDispatch.Visible = true;
                    this.mpReleaseDispatch.Show();
                }

            }
            catch(Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }


        protected void grdLpnDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdLpnDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdLpnDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdLpnDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdDetailPopUp_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdDetailPopUp.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdDetailPopUp.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdDetailPopUp.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }


        protected void LoadOutboundOrderDetails()
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (currentIndex != -1)
            {
                int id = outboundOrderViewDTO.Entities[grdMgr.SelectedIndex].Id;

                this.lblNroDoc.Text = outboundOrderViewDTO.Entities[grdMgr.SelectedIndex].Number;

                outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutboundWithStock(context, id);

                if (outboundDetailViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!outboundDetailViewDTO.hasConfigurationError() && outboundDetailViewDTO.Configuration != null && outboundDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, outboundDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = outboundDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                divDetail.Visible = true;
            }
            else
            {
                grdDetail.DataSource = null;
                grdDetail.DataBind();
                divDetail.Visible = false;
            }
        }

        protected void LoadOutboundOrderDetailsPopUp()
        {
            this.grdDetailPopUp.EmptyDataText = this.Master.NoDetailsText;

            if (currentSelectedIndex != -1)
            {
                //divDetail.Visible = true;

                int id = outboundOrderViewDTO.Entities[currentSelectedIndex].Id;

                outboundDetailPopUpViewDTO = iDispatchingMGR.GetDetailByIdOutboundWithStock(context, id);

                if (outboundDetailPopUpViewDTO.Entities != null)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!outboundDetailPopUpViewDTO.hasConfigurationError() && outboundDetailPopUpViewDTO.Configuration != null && outboundDetailPopUpViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(this.grdDetailPopUp, outboundDetailPopUpViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    this.grdDetailPopUp.DataSource = outboundDetailPopUpViewDTO.Entities;
                    this.grdDetailPopUp.DataBind();

                    if (!outboundDetailPopUpViewDTO.hasError() && outboundDetailPopUpViewDTO.Entities != null)
                    {
                        Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.ListDetailOrdenSelected] = outboundDetailPopUpViewDTO;
                    }
                                    
                }
            }
        }


        protected void LoadLpnDetail()
        {
            grdLpnDetail.EmptyDataText = this.Master.NoDetailsText;

            if (currentIndexLpn != -1 && lpnViewDTO.Entities !=null && lpnViewDTO.Entities.Count > 0)
            {
                string idCode = lpnViewDTO.Entities[currentIndexLpn].Lpn.IdCode;

                lpnDetailViewDTO = iDispatchingMGR.GetDetailPackageByIdLpnCode(idCode, context);

                if (lpnDetailViewDTO != null && lpnDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!stockViewDTO.hasConfigurationError() && lpnDetailViewDTO.Configuration != null && lpnDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdLpnDetail, lpnDetailViewDTO.Configuration);

                    // Detalle de Recepciones
                    grdLpnDetail.DataSource = lpnDetailViewDTO.Entities;
                    grdLpnDetail.DataBind();

                }

            }
            else
            {
                // Detalle de Recepciones
                grdLpnDetail.DataSource = null;
                grdLpnDetail.DataBind();
            }
        }


        protected void SaveSelectedRowsLpn()
        {
            if (lpnViewDTO.Entities.Count > 0)
            {
                for (int i = 0; i < lpnViewDTO.Entities.Count; i++)
                {
                    foreach (Binaria.WMSTek.Framework.Entities.Warehousing.Stock selectedLpn in lpnViewDTO.Entities)
                    {
                        if (selectedLpn.Lpn.Code == lpnViewDTO.Entities[i].Lpn.Code)
                        {
                            selectedLpnCurrentView[i] = true;
                            break;
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// Limpia la lista de checkboxes seleccionados
        /// </summary>
        protected void InitializeCheckedRows()
        {
            checkedLpnCurrentView.Clear();

            if (lpnViewDTO.Entities != null && checkedLpnCurrentView != null)
            {
                for (int i = 0; i < lpnViewDTO.Entities.Count; i++)
                {
                    checkedLpnCurrentView.Add(false);
                }
            }
        }

        /// <summary>
        /// Limpia la lista de Ordenes seleccionados
        /// </summary>
        protected void InitializeSelectedRows()
        {
            selectedLpnCurrentView.Clear();

            if (lpnViewDTO.Entities != null && selectedLpnCurrentView != null)
            {
                for (int i = 0; i < lpnViewDTO.Entities.Count; i++)
                {
                    selectedLpnCurrentView.Add(false);
                }
            }
        }

        /// <summary>
        /// Salva en un array los checkboxes seleccionados
        /// </summary>
        protected void SaveCheckedRows()
        {
            // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
            for (int i = 0; i < grdLpn.Rows.Count; i++)
            {
                if (checkedLpnCurrentView.Count > (grdLpn.PageIndex * grdLpn.PageSize) + i)
                {
                    GridViewRow row = grdLpn.Rows[i];
                    checkedLpnCurrentView[(grdLpn.PageIndex * grdLpn.PageSize) + i] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;

                    // Valida que se haya seleccionado al menos una orden
                    //if (checkedLpnCurrentView[(grdLpn.PageIndex * grdLpn.PageSize) + i]) existSelected = true;
                }
            }

        }

        /// <summary>
        /// Arma la lista de Ordenes seleccionadas para la simulación de Liberación de Pedidos
        /// </summary>
        protected void AddToSelectedLpns()
        {
            Binaria.WMSTek.Framework.Entities.Warehousing.Stock selectedLpn = new Binaria.WMSTek.Framework.Entities.Warehousing.Stock();

            for (int i = 0; i < lpnViewDTO.Entities.Count; i++)
            {
                if (checkedLpnCurrentView[i] && !selectedLpnCurrentView[i])
                {
                    // Recupera la Orden seleccionada
                    selectedLpn = lpnViewDTO.Entities[i];

                    // Agrega la Orden seleccionada a la lista de Ordenes seleccionadas
                    selectedLpnViewDTO.Entities.Add(selectedLpn);
                }
            }

            InitializeCheckedRows();
            InitializeSelectedRows();

            SaveSelectedRowsLpn();

            Session[WMSTekSessions.ReleaseOrdeForLpn.PIKLPN.SelectedLpn] = selectedLpnViewDTO;

            releaseEnabled = false;
        }


        protected void ReleaseOrder()
        {
            Task taskInfo = new Task();

            // Información general de la Tarea a generar
            //    taskInfo.Priority = Convert.ToInt16(txtPriority.Text);
            taskInfo.WorkersRequired = 1;
            taskInfo.WorkersAssigned = 0;
            taskInfo.StageSource = new Location(this.ddlLocStageDispatch.SelectedValue);
            taskInfo.StageTarget = new Location(this.ddlLocDock.SelectedValue);
            taskInfo.IdDocumentBound = int.Parse(this.txtIdOutboundOrder.Text.Trim());
            taskInfo.Warehouse = new Warehouse( int.Parse(this.hidIdWhs.Value.Trim()));
            taskInfo.Owner = new Owner(int.Parse(this.hidIdOwn.Value.Trim()));
            taskInfo.IsComplete = false;
            taskInfo.Status = true;
            taskInfo.IdTrackTaskType = (int)TrackTaskTypeName.Liberada;
            taskInfo.TypeCode = dispatchType;            
            
            
            OutboundOrder order = new OutboundOrder();
            order.Id = int.Parse(this.txtIdOutboundOrder.Text.Trim());
            order.Warehouse = new Warehouse(int.Parse(this.hidIdWhs.Value.Trim()));
            order.Owner = new Owner(int.Parse(this.hidIdOwn.Value.Trim()));
            order.CustomerCode = this.hidCustomerCode.Value.Trim();
            List<LPN> lstLpn = new List<LPN>();           
            
            foreach (var item in selectedLpnViewDTO.Entities)
            {
                lstLpn.Add(item.Lpn);
            }

            taskViewDTO = iDispatchingMGR.CreateDispatchTaskLpn(taskInfo, order, lstLpn, context);

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
                
                // Limpia
                selectedLpnViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
                InitializeSession();

                UpdateSession(false);

                grdDetail.DataSource = null;
                grdDetail.DataBind();

                // Oculta detalle de las grillas
                HideDetails();
            }
        }

        private void ucError_BtnCloseErrorClick(object sender, EventArgs e)
        {
            try
            {
                if (divReleaseDispatch.Visible)
                    mpReleaseDispatch.Show();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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

        protected void grdLpn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetailPopUp_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdLpnDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('OutboundOrderDetail_ById_ItemStock', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }

    }
}
