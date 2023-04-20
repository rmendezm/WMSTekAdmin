using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Inventory;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Inventory.Consult
{
    public partial class DetailInventoryConsult : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<InventoryOrder> inventoryViewDTO = new GenericViewDTO<InventoryOrder>();
        private GenericViewDTO<InventoryDetail> inventoryDetailViewDTO = new GenericViewDTO<InventoryDetail>();
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

        // Propiedad para actualizar o no grilla principal
        public bool showDetail
        {
            get
            {
                if (ValidateViewState("showDetail"))
                    return (bool)ViewState["showDetail"];
                else
                    return false;
            }

            set { ViewState["showDetail"] = value; }
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
                    context.SessionInfo.IdPage = "DetailInventoryConsult";

                    InitializeFilter(!Page.IsPostBack, false);
                    InitializeTaskBar();
                    InitializeStatusBar();
                    InitializeGrids();

                    if (!Page.IsPostBack)
                    {
                        //UpdateSession();
                        PopulateLists();
                    }
                    else
                    {
                        if (ValidateSession(WMSTekSessions.InventoryConsult.DetailInventoryConsultList))
                        {
                            inventoryDetailViewDTO = (GenericViewDTO<InventoryDetail>)Session[WMSTekSessions.InventoryConsult.DetailInventoryConsultList];
                            //inventoryViewDTO = (GenericViewDTO<InventoryOrder>)Session[WMSTekSessions.InventoryConsult.InventoryList];
                            isValidViewDTO = true;

                            //if (!showDetail) PopulateGrid();
                            //PopulateGrid();
                        }
                    }                
                }
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
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
                        if (!showDetail) PopulateGrid();
                        //PopulateInventoryDetail();
                    }
                }
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
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

                }
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
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
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
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
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                showDetail = true;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                showDetail = true;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                showDetail = true;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                showDetail = true;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                showDetail = true;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        
        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdDetail.PageSize = ucStatus.PageSize;
        //    }
        //    catch (Exception ex)
        //    {
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
        //    }
        //}

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
                inventoryDetailViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        /// <summary>
        /// Muestra el detalle de cada Inventario
        /// </summary>


        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            
            this.Master.ucMainFilter.codeNumericVisible = true;
            this.Master.ucMainFilter.codeNumericLabel = lblInventoryCode.Text;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            
            
            //FILTRO AVANZADO
            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;
            
            //Habilita Filtro de Recepcion
            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionDocumentNbr = true;
            this.Master.ucMainFilter.DocumentNbrLabel = this.lblLotNumber.Text;
            this.Master.ucMainFilter.tabReceptionSourceLocation = false;
            this.Master.ucMainFilter.tabReceptionTargetLocation = false;
            this.Master.ucMainFilter.tabReceptionTaskPriority = false;
            this.Master.ucMainFilter.tabReceptionTargetLpn = false;
            this.Master.ucMainFilter.tabReceptionLogHeaderText = this.lblFilterOther.Text;

            //Tab Layout
            //this.Master.ucMainFilter.tabLayoutVisible = true;

            //Tab Location
            this.Master.ucMainFilter.tabLocationVisible = true;
            
            //Tab Fecha
            this.Master.ucMainFilter.tabDatesVisible = true;
            this.Master.ucMainFilter.expirationDateVisible = true;
            this.Master.ucMainFilter.fabricationDateVisible = true;
            
                                   
            //Tab Grupos
            this.Master.ucMainFilter.tabItemGroupVisible = true;
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
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
        private void InitializeGrids()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            showDetail = false;
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de Inventarios
            //inventoryViewDTO = iInventoryMGR.FindAllInventory(context);
            inventoryDetailViewDTO = iInventoryMGR.GetInventoryDetailSpecialAll(context);

            if (!inventoryDetailViewDTO.hasError() && inventoryDetailViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InventoryConsult.DetailInventoryConsultList, inventoryDetailViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(inventoryViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inventoryDetailViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            //base.LoadLocationType(this.ddlLocationType, false, this.Master.AllRowsText);
        }

        private void PopulateGrid()
        {
            grdMgr.SelectedIndex = currentIndex;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!inventoryDetailViewDTO.hasConfigurationError() && inventoryDetailViewDTO.Configuration != null && inventoryDetailViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inventoryDetailViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = inventoryDetailViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(inventoryDetailViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                currentIndex = -1;
            }
        }



        #endregion

    }
}
