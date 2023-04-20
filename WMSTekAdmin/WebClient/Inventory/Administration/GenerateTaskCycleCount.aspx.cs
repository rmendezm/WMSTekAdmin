using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Inventory;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Tasks;


namespace Binaria.WMSTek.WebClient.Inventory.Administration
{
    public partial class GenerateTaskCycleCount : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Task> taskViewDTO;
        private GenericViewDTO<TaskDetail> taskDetailViewDTO;
        private GenericViewDTO<LocationConsult> locationViewDTO = new GenericViewDTO<LocationConsult>();
        private GenericViewDTO<LocationConsult> locationSelectedViewDTO = new GenericViewDTO<LocationConsult>();
        private bool isValidViewDTO = false;
        private List<bool> checkedOrdersCurrentView = new List<bool>();

        private int cont = 1;

        bool existSelected = false;
        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateSession("pageGenerateTaskCycleCount"))
                    return (int)Session["pageGenerateTaskCycleCount"];
                else
                    return 0;
            }

            set { Session["pageGenerateTaskCycleCount"] = value; }
        }
        public List<bool> CheckedOrdersCurrentView
        {
            get
            {
                if (ValidateViewState("checkedOrdersCurrentView"))
                    return (List<bool>)ViewState["checkedOrdersCurrentView"];
                else
                    return checkedOrdersCurrentView;
            }

            set { ViewState["checkedOrdersCurrentView"] = value; }
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

                if (!Page.IsPostBack)
                {
                    //Esta instruccion limpia lo que hay en session si es que el usuario sale y vuelve a entrar
                    if (ValidateSession(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList))
                        Session.Remove(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList);
                    InicializeFilterLoc();
                }
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }

        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            bool IsVlidFilter = true;
            try
            {
                //if (this.Master.ucMainFilter.IsValidFilterLocation)
                //{
                    //if (Master.ucMainFilter.RowFromRangeValue != string.Empty && Master.ucMainFilter.RowToRangeValue != string.Empty)
                    //{
                    //    if (Convert.ToInt32(Master.ucMainFilter.RowFromRangeValue) > Convert.ToInt32(Master.ucMainFilter.RowToRangeValue))
                    //    {
                    //        locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.ListRowFromIsGreater, context));
                    //        IsVlidFilter = false;
                    //    }
                    //}

                    //if (Master.ucMainFilter.ColumnFromRangeValue != string.Empty && Master.ucMainFilter.ColumnToRangeValue != string.Empty)
                    //{
                    //    if (Convert.ToInt32(Master.ucMainFilter.ColumnFromRangeValue) > Convert.ToInt32(Master.ucMainFilter.ColumnToRangeValue))
                    //    {
                    //        IsVlidFilter = false;
                    //        locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.ListColumnFromIsGreater, context));
                    //    }
                    //}
                    //if (Master.ucMainFilter.LevelFromRangeValue != string.Empty && Master.ucMainFilter.LevelToRangeValue != string.Empty)
                    //{
                    //    if (Convert.ToInt32(Master.ucMainFilter.LevelFromRangeValue) > Convert.ToInt32(Master.ucMainFilter.LevelToRangeValue))
                    //    {
                    //        IsVlidFilter = false;
                    //        locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.ListLevelFromIsGreater, context));
                    //    }
                    //}
                    //if (IsVlidFilter)
                        ReloadData();
                    //else
                    //{
                    //    this.Master.ucError.ShowError(locationViewDTO.Errors);
                    //}
                //}
                //else
                //{
                //    //Debe seleccionar al menos un filtro de location
                //    locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.NoFilterSelected, context));
                //    this.Master.ucError.ShowError(locationViewDTO.Errors);
                //}
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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

                CheckBox chkFilter = (CheckBox)this.Master.ucMainFilter.FindControl("chkUseAdvancedFilter") as CheckBox;
                if (chkFilter != null)
                    chkFilter.Checked = true;

                if (locationViewDTO.Entities != null)
                {
                    locationViewDTO.Entities.Clear();
                    Session.Add(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList, locationViewDTO);
                    isValidViewDTO = true;
                }
                PopulateGrid();
                //ReloadData();
            }
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
            catch (Exception ex)
            {
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
                locationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(locationViewDTO.Errors);
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
        //        locationViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(locationViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "GenerateTaskCycleCount";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            //InitializeCheckedRows();

            if (Page.IsPostBack)
            {
                if (ValidateSession(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList))
                {
                    locationViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationGenerateTaskCycleCount.LocationList];
                    isValidViewDTO = true;
                }
            }
            else
            {
                //UpdateSession();
            }
            // Es necesario cargar las grillas SIEMPRE para evitar problemas con el orden dinamico de las columnas
            PopulateGrid();
        }
        /// <summary>
        /// Limpia la lista de checkboxes seleccionados
        /// </summary>
        protected void InitializeCheckedRows()
        {
            CheckedOrdersCurrentView.Clear();

            if (ValidateSession(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList))
                locationViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationGenerateTaskCycleCount.LocationList];

            if (locationViewDTO.Entities != null && CheckedOrdersCurrentView != null)
            {
                for (int i = 0; i < locationViewDTO.Entities.Count; i++)
                {
                    CheckedOrdersCurrentView.Add(false);
                }
            }
        }
        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            
            //Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabLocationVisible = true;
            this.Master.ucMainFilter.locationFilterVisible = true;

            this.Master.ucMainFilter.tabItemGroupVisible = true;

            //Esconde filtros no utilizados
            //this.Master.ucMainFilter.LocationEqualVisible = false;
            this.Master.ucMainFilter.LocationRangeVisible = true;
            this.Master.ucMainFilter.LocationRowColumnEqualVisible = false;
            this.Master.ucMainFilter.LocationLevelEqualVisible = false;
            //this.Master.ucMainFilter.ChkDisabledAndChequed = true;
            this.Master.ucMainFilter.LocationFilterType = context.SessionInfo.IdPage;

            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = lblItemCode.Text;
            this.Master.ucMainFilter.nameLabel = lblItemName.Text;

            this.Master.ucMainFilter.SaveOnIndexChanged = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
            this.Master.ucMainFilter.ddlWareHouseIndexChanged += new EventHandler(ddlWareHouseIndexChanged);
            //this.Master.ucMainFilter.ActivateAdvancedFilter();
        }
        protected void ddlWareHouseIndexChanged(object sender, EventArgs e)
        {
            InicializeFilterLoc();
        }

        private void InicializeFilterLoc()
        {
            var lstTypeLoc = GetConst("LocationTypeDif");
            //this.Master.ucMainFilter.InitializeFilterLoc();
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            context.MainFilter[Convert.ToInt16(EntityFilterName.LocationType)].FilterValues.Clear();

            foreach (var item in lstTypeLoc)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.LocationType)].FilterValues.Add(new FilterItem(item)); 
            }
            

            Session.Remove(WMSTekSessions.LocationRange.LocationListColumn);
            Session.Remove(WMSTekSessions.LocationRange.LocationListRow);
            Session.Remove(WMSTekSessions.LocationRange.LocationListLevel);
            this.Master.ucMainFilter.InitializeFilterLocWhs();
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
            // Carga consulta de Conteos Cíclicos            
            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            locationViewDTO = iLayoutMGR.GetLocationForCicleCount(context);

            if (!locationViewDTO.hasError() && locationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList, locationViewDTO);
                isValidViewDTO = true;
                ucStatus.ShowMessage(locationViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }

        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!locationViewDTO.hasConfigurationError() && locationViewDTO.Configuration != null && locationViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, locationViewDTO.Configuration);

            grdMgr.DataSource = locationViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(locationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                PopulateGrid();
                this.Master.ucError.ClearError();
            }
        }

        private void ShowPopUpGenerateTask()
        {
            GenericViewDTO<LocationConsult> selectedOrdersViewDTO = new GenericViewDTO<LocationConsult>();
            int indexTotal = 0;
            //Inicializa la lista de booleanos
            InitializeCheckedRows();

            // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                indexTotal = (grdMgr.PageSize * (currentPage + 1)) - (grdMgr.PageSize - i);
                GridViewRow row = grdMgr.Rows[i];
                CheckedOrdersCurrentView[indexTotal] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;

                Session.Add(WMSTekSessions.LocationGenerateTaskCycleCount.LocationListSelected, CheckedOrdersCurrentView);

                // Valida que se haya seleccionado al menos una orden
                if (CheckedOrdersCurrentView[indexTotal]) 
                    existSelected = true;
            }
            if (existSelected)
            {
                if (ValidateSession(WMSTekSessions.LocationGenerateTaskCycleCount.LocationList))
                {
                    locationViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationGenerateTaskCycleCount.LocationList];
                    isValidViewDTO = true;
                }
                //Recupera las ubicaciones
                for (int i = 0; i < locationViewDTO.Entities.Count; i++)
                {
                    //indexTotal = (grdMgr.PageSize * (currentPage + 1)) - (grdMgr.PageSize - 1);
                    if (CheckedOrdersCurrentView[i])
                    {
                        LocationConsult selectedOrder = new LocationConsult();
                        // Recupera la Orden seleccionada
                        selectedOrder = locationViewDTO.Entities[i];

                        // Agrega la Orden seleccionada a la lista de Ordenes seleccionadas
                        selectedOrdersViewDTO.Entities.Add(selectedOrder);
                    }
                }
                //Valida que no tengan tareas pendientes tanto para dejar ítems como para sacar
                if (ValidateOrders(selectedOrdersViewDTO))
                {
                    //Carga la lista de operadores
                    base.LoadUsersOperator(this.ddlOperator, true, Master.EmptyRowText);

                    txtDate.Text = DateTime.Now.ToShortDateString();
                    calDate.SelectedDate = DateTime.Now;

                    //Muestra el PopUp para asignar operador, fecha y prioridad
                    this.mpReleaseDispatch.Show();
                    this.divReleaseDispatch.Visible = true;
                }
                else
                {
                    locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.ExistLocationWithTaskPending, context));
                    this.Master.ucError.ShowError(locationViewDTO.Errors);
                }
            }
            else
            {
                isValidViewDTO = false;
                locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.NotLocationSelected, context));
                this.Master.ucError.ShowError(locationViewDTO.Errors);
            }
        }
        //Valida que no tengan tareas pendientes tanto para dejar ítems como para sacar
        private bool ValidateOrders(GenericViewDTO<LocationConsult> ListOrders)
        {
            bool IsValid = true;

            foreach (LocationConsult loc in ListOrders.Entities)
            {
                if (loc.CountTaskSource > -1 || loc.CountTaskTarget > -1)
                {
                    IsValid = false;
                    break;
                }
            }
            return IsValid;
        }

        private void GenerateTask()
        {
            taskViewDTO = new GenericViewDTO<Task>();
            taskDetailViewDTO = new GenericViewDTO<TaskDetail>();
            CheckedOrdersCurrentView = (List<bool>)Session[WMSTekSessions.LocationGenerateTaskCycleCount.LocationListSelected];
            locationSelectedViewDTO = new GenericViewDTO<LocationConsult>();
            locationViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationGenerateTaskCycleCount.LocationList];
            bool IsValid = false;
            DateTime date = DateTime.MinValue;
            string userOperator = string.Empty;
            int priority = 0;

            try
            {
                for (int i = 0; i < CheckedOrdersCurrentView.Count; i++)
                {
                    if (CheckedOrdersCurrentView[i])
                    {
                        locationSelectedViewDTO.Entities.Add(locationViewDTO.Entities[i]);
                        IsValid = true;
                    }
                }

                if (IsValid)
                {
                    //Recupero los datos del popUp
                    priority = Convert.ToInt16(this.txtPriority.Text);
                    date = Convert.ToDateTime(this.txtDate.Text);
                    userOperator = this.ddlOperator.SelectedValue;
                }
                else
                {
                    locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.NotLocationSelected, context));
                }
                if (IsValid)
                {
                    if (priority > 10 || priority < 1)
                    {
                        IsValid = false;
                        locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.PriorityNotValid, context));
                    }
                }
                if (IsValid)
                {
                    if (this.txtDate.Text == string.Empty)
                    {
                        IsValid = false;
                        locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.DateNotValid, context));
                    }
                }
                if (IsValid)
                {
                    if (this.Master.ucMainFilter.idWhs == -1)
                    {
                        IsValid = false;
                        locationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.WarehouseNotValid, context));
                    }

                }
                if (IsValid)
                {
                    if (locationSelectedViewDTO.Entities.Count > 0)
                    {
                        //Recupera el codigo reason y el codigo del tipo de tarea
                        string reasonCode = string.Empty;
                        string typeTaskCode = string.Empty;

                        List<string> listaConstReason = new List<string>();
                        listaConstReason = GetConst("ReasonCodes");
                        reasonCode = listaConstReason[Convert.ToInt16(ReasonCode.CCADM)];

                        //List<string> listaConstTypeTask = new List<string>();
                        //listaConstTypeTask = GetConst("TypeTaskCodes");
                        //typeTaskCode = listaConstTypeTask[Convert.ToInt16(TypeTaskCode.CCPC)];
                        typeTaskCode = TypeTaskCode.CCPC.ToString();
                     
                        taskViewDTO = iTasksMGR.CreateTaskCycleCount(context, locationSelectedViewDTO, this.Master.ucMainFilter.idWhs
                            ,  priority, date, userOperator, reasonCode, typeTaskCode);

                        if (taskViewDTO.hasError())
                        {
                            isValidViewDTO = false;
                            this.Master.ucError.ShowError(locationViewDTO.Errors);
                        }
                        else
                        {
                            //Limpia la lista de booleanos
                            CheckedOrdersCurrentView.Clear();

                            //Actualiza la grilla
                            UpdateSession();
                            PopulateGrid();

                            if (taskViewDTO.MessageStatus.Message != "")
                            {
                                ucStatus.ShowMessage(taskViewDTO.MessageStatus.Message);
                            }
                            else
                            {
                                ucStatus.ShowMessage(this.lblGenerateTask.Text);
                            }
                        }
                    }
                }
                else
                {
                    isValidViewDTO = false;
                    this.Master.ucError.ShowError(locationViewDTO.Errors);
                }
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void btnGenerateTask_Click(object sender, EventArgs e)
        {
            GenerateTask();
        }
        protected void btnReprocess_Click(object sender, EventArgs e)
        {
            ShowPopUpGenerateTask();
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Location_GetForCicleCount', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            base.grdMgr_RowDataBound(sender, e);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
               string editIndex = e.Row.DataItemIndex.ToString();

                Label lblCountTaskSource = e.Row.FindControl("lblCountTaskSource") as Label;
                Label lblCountTaskTarget = e.Row.FindControl("lblCountTaskTarget") as Label;
                TextBox txtValida = e.Row.FindControl("txtValida") as TextBox;
                CheckBox chkSelectOrder = e.Row.FindControl("chkSelectOrder") as CheckBox;

                if (lblCountTaskSource != null && lblCountTaskTarget != null)
                {
                    if (!string.IsNullOrEmpty(lblCountTaskSource.Text.Trim()) && !string.IsNullOrEmpty(lblCountTaskTarget.Text.Trim()))
                    {
                        if (Convert.ToInt32(lblCountTaskSource.Text) > 0 || Convert.ToInt32(lblCountTaskTarget.Text) > 0)
                        {
                            //Deshabilito el chekbox
                            chkSelectOrder.Enabled = false;

                            //muestro un mensaje de advertencia
                            System.Web.UI.WebControls.Image imgWarning = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgWarning") as System.Web.UI.WebControls.Image;
                            RequiredFieldValidator reqParameterValue1 = new RequiredFieldValidator();
                            reqParameterValue1.ID = "reqParameterValue";
                            reqParameterValue1.ControlToValidate = "txtValida";
                            reqParameterValue1.ErrorMessage = lblErrorValidate.Text;
                            reqParameterValue1.SetFocusOnError = true;
                            reqParameterValue1.ValidationGroup = "EditNew";
                            reqParameterValue1.Text = " req ";
                            reqParameterValue1.Display = ValidatorDisplay.None;
                            imgWarning.Visible = true;
                            imgWarning.ToolTip = lblErrorValidate.Text;
                            TableCell reqCell1 = new TableCell();
                            reqCell1.Controls.Add(reqParameterValue1);

                            e.Row.Cells.Add(reqCell1);
                        }
                    }
                }

            }
        }
    }
}
