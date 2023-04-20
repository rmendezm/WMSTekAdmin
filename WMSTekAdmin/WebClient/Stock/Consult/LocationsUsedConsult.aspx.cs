using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Inbound.Consult
{
    public partial class LocationsUsedConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<LocationsUsed> locationsUsedViewDTO = new GenericViewDTO<LocationsUsed>();
        private GenericViewDTO<Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Framework.Entities.Warehousing.Stock>();
        //private GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        private GenericViewDTO<InboundDetail> inboundDetailViewDTO = new GenericViewDTO<InboundDetail>();
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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

                    LoadInboundOrderDetail(index);
                }
            }
            catch (Exception ex)
            {
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    LoadInboundOrderDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                }
                else
                {
                    detailTitle = null;
                }

                base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
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
                locationsUsedViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "LocationsUsedConsult";

            InitializeFilter(!Page.IsPostBack, false);
            InitializeTaskBar();
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InventoryConsult.LocationsUsedList))
                {
                    locationsUsedViewDTO = (GenericViewDTO<LocationsUsed>)Session[WMSTekSessions.InventoryConsult.LocationsUsedList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Retorna el detalle de cada doc de entrada
        /// </summary>
        /// <param name="index"></param>
        protected void LoadInboundOrderDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                locationsUsedViewDTO = (GenericViewDTO<LocationsUsed>)Session[WMSTekSessions.InventoryConsult.LocationsUsedList];

                string idCode = locationsUsedViewDTO.Entities[index].IdLocCode;

                stockViewDTO = iDispatchingMGR.GetDetalPackageByIdLocCode(idCode, context);
                //stockViewDTO = iDispatchingMGR.GetDetalPackageByIdLpnCode(idCode, context);
                this.lblNroDoc.Text = locationsUsedViewDTO.Entities[index].LocCode;

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
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.usoUbicacionesVisible = true;
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabLocationVisible = true;
            this.Master.ucMainFilter.tabLayoutVisible = true;
            
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

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
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
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de Inbound
            locationsUsedViewDTO = iInventoryMGR.FindAllLocationsUsed(context);

            if (!locationsUsedViewDTO.hasError() && locationsUsedViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InventoryConsult.LocationsUsedList, locationsUsedViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(locationsUsedViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(locationsUsedViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!locationsUsedViewDTO.hasConfigurationError() && locationsUsedViewDTO.Configuration != null && locationsUsedViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, locationsUsedViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = locationsUsedViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(locationsUsedViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('GetStockByIdLocCode', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }

        #endregion

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}

