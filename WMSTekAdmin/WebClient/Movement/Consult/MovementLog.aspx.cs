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

namespace Binaria.WMSTek.WebClient.Movement.Consult
{
    public partial class MovementLog : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<MovementWeb> movementViewDTO = new GenericViewDTO<MovementWeb>();
        private bool isValidViewDTO = false;
        private string filterMov = string.Empty;
        private string entityNameProperty = string.Empty;
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
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
                movementViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
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
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        movementViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(movementViewDTO.Errors);
        //    }
        //}

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
                if (ValidateSession(WMSTekSessions.OutboundConsult.DispatchLogList))
                {
                    movementViewDTO = (GenericViewDTO<MovementWeb>)Session[WMSTekSessions.OutboundConsult.DispatchLogList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de movimientos
            this.Master.ucMainFilter.listWmsProcessType = new System.Collections.Generic.List<string>();
            this.Master.ucMainFilter.listWmsProcessType = GetConst("MovementLog");

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            
            this.Master.ucMainFilter.movementTypeVisible = true;


            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            //Tab Reception
            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionTaskPriority = false;
            
            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.LogDaysBeforeQuery;//dias antes
            this.Master.ucMainFilter.DateAfter = CfgParameterName.LogDaysAfterQuery;//dias despues

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);

            entityNameProperty = "MovementLog_FindAll";
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
            // carga todas las recepciones
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            movementViewDTO = iWarehousingMGR.FindAllGenericLog(context, entityNameProperty);

            if (!movementViewDTO.hasError() && movementViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.DispatchLogList, movementViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(movementViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(movementViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!movementViewDTO.hasConfigurationError() && movementViewDTO.Configuration != null && movementViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, movementViewDTO.Configuration);

            grdMgr.DataSource = movementViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(movementViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

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
