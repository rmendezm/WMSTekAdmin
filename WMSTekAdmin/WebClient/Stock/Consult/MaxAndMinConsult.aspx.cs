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
    public partial class MaxAndMinConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<MaxMinConsult> maxAndMinConsultViewDTO = new GenericViewDTO<MaxMinConsult>();
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// //Calcula los desbordes de maximos y minimos para cambiar el color de los numeros
        /// </summary>
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                base.grdMgr_RowDataBound(sender, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {


                    //busca controles
                    Label lblItemQty = (Label)e.Row.FindControl("lblItemQty");//lo real en stock
                    Label lblReOrderPoint = (Label)e.Row.FindControl("lblReOrderPoint");//minimo
                    Label lblReOrderQty = (Label)e.Row.FindControl("lblReOrderQty");//maximo

                    //me aseguro que no venga ninguno vacio
                    if (lblItemQty.Text != "" && lblReOrderPoint.Text != "" && lblReOrderQty.Text != "")
                    {
                        //si el stock esta por debajo del minimo saldrà en rojo
                        if (Convert.ToDecimal(lblItemQty.Text) < Convert.ToDecimal(lblReOrderPoint.Text))
                        {
                            lblItemQty.ForeColor = System.Drawing.Color.Red;
                        }
                        //si el stock esta por encima del maximo saldrà en azul
                        else if (Convert.ToDecimal(lblItemQty.Text) > Convert.ToDecimal(lblReOrderQty.Text))
                        {
                            lblItemQty.ForeColor = System.Drawing.Color.Blue;
                        }
                        //si el stock no se desborda ni falta saldrá negro.
                        else
                        {
                            lblItemQty.ForeColor = System.Drawing.Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                maxAndMinConsultViewDTO = (GenericViewDTO<MaxMinConsult>)Session[WMSTekSessions.StockConsult.MaxAndMinList];

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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
            }        }


        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
                maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
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
        //        maxAndMinConsultViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
        //    }
        //}

        // Carga la grilla, filtrada por el criterio de busqueda ingresado

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
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.StockConsult.MaxAndMinList))
                {
                    maxAndMinConsultViewDTO = (GenericViewDTO<MaxMinConsult>)Session[WMSTekSessions.StockConsult.MaxAndMinList];
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
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.MaxAndMinDaysBefore;
            //this.Master.ucMainFilter.DateAfter = CfgParameterName.MaxAndMinDaysAfter;

            //FILTRO AVANZADO
            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;
                        
            //Tab Layout
            this.Master.ucMainFilter.tabLayoutVisible = true;

            //Tab Location
            this.Master.ucMainFilter.tabLocationVisible = true;

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
            this.Master.ucMainFilter.tabItemGroupVisible = true;

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
            // carga consulta de MaxAndMinConsult
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            maxAndMinConsultViewDTO = iWarehousingMGR.FindAllMaxAndMin(context);

            if (!maxAndMinConsultViewDTO.hasError() && maxAndMinConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.MaxAndMinList, maxAndMinConsultViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(maxAndMinConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(maxAndMinConsultViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!maxAndMinConsultViewDTO.hasConfigurationError() && maxAndMinConsultViewDTO.Configuration != null && maxAndMinConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, maxAndMinConsultViewDTO.Configuration);

            grdMgr.DataSource = maxAndMinConsultViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(maxAndMinConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
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

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('MaxAndMin_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion
    }
}
