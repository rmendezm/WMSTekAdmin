using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Stocks
{
    public partial class StateStockConsultPage : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<StateStockConsult> stateStockViewDTO = new GenericViewDTO<StateStockConsult>();
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
            }
            catch (Exception ex)
            {
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                    e.Row.TableSection = TableRowSection.TableHeader;

                //base.grdMgr_RowDataBound(sender, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {                    
                    //busca controles
                    Label lblQtyTotal = (Label)e.Row.FindControl("lblQtyTotal");//lo real en stock

                    //me aseguro que no venga ninguno vacio
                    if (!string.IsNullOrEmpty(lblQtyTotal.Text.Trim()))
                    {
                        //si el stock esta es menor o igual a cero lo pinta en rojo
                        if (Convert.ToDecimal(lblQtyTotal.Text.Trim()) <= 0)
                        {
                            lblQtyTotal.ForeColor = System.Drawing.Color.Red;
                        }
                        //si el stock es mayor a cero lo pinta en azul
                        else if (Convert.ToDecimal(lblQtyTotal.Text.Trim()) > 0)
                        {
                            lblQtyTotal.ForeColor = System.Drawing.Color.Blue;
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        stateStockViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(stateStockViewDTO.Errors);
        //    }
        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
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
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
               //no hace nada
            }
            catch (Exception ex)
            {
                
                stateStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
            }

        }

        #endregion

        #region "Métodos"


        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.StockConsult.StateStockConsultList))
                {
                    stateStockViewDTO = (GenericViewDTO<StateStockConsult>)Session[WMSTekSessions.StockConsult.StateStockConsultList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //FILTRO BASICO
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            //this.Master.ucMainFilter.documentVisible = true;
            //this.Master.ucMainFilter.dateFromVisible = true;
            //this.Master.ucMainFilter.dateToVisible = true;

            // Configura parámetros de fechas
            //this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.StockDaysBefore;
            //this.Master.ucMainFilter.DateAfter = CfgParameterName.StockDaysAfter;

            //FILTRO AVANZADO
            // Habilita el Filtro Avanzado
            //this.Master.ucMainFilter.advancedFilterVisible = true;

            //Tab Layout
            //this.Master.ucMainFilter.tabLayoutVisible= true;

            //Tab Location
            //this.Master.ucMainFilter.tabLocationVisible = true;

            //Tab Fecha
            //this.Master.ucMainFilter.tabDatesVisible = true;
            //this.Master.ucMainFilter.expirationDateVisible = true;
            //this.Master.ucMainFilter.fabricationDateVisible = true;
            
            //Tab Documento
            //this.Master.ucMainFilter.tabDocumentVisible = true;
            //this.Master.ucMainFilter.vendorVisible = true;
            //this.Master.ucMainFilter.carrierVisible = true;
            //this.Master.ucMainFilter.driverVisible = true;

            //Tab Grupos
            //this.Master.ucMainFilter.tabItemGroupVisible = true;

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
            // carga consulta de Stock
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            stateStockViewDTO = iWarehousingMGR.GetStateStockByFilters(context);

            if (!stateStockViewDTO.hasError() && stateStockViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.StateStockConsultList, stateStockViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(stateStockViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(stateStockViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!stateStockViewDTO.hasConfigurationError() && stateStockViewDTO.Configuration != null && stateStockViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, stateStockViewDTO.Configuration);

            grdMgr.DataSource = stateStockViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(stateStockViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        #endregion
    }
}
