using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Tasks;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class BuildingReplacementTask : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<ItemLocation> itemLocationViewDTO = new GenericViewDTO<ItemLocation>();
        private bool isValidViewDTO = false;
        private List<bool> checkedOrdersCurrentView = new List<bool>();
        private List<bool> listCheckedOrders = new List<bool>();
        private GenericViewDTO<ItemLocation> locationSelectedViewDTO = new GenericViewDTO<ItemLocation>();
        bool existSelected = false;
        int countChequed = 0;
        int countRowsActualGrid = 0;
        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateSession("pageBuildingReplacementTask"))
                {
                    return (int)Session["pageBuildingReplacementTask"];
                }
                //if (ValidateViewState("pageBuildingReplacementTask"))
                //    return (int)ViewState["pageBuildingReplacementTask"];
                else
                    return 0;
            }

            set { Session["pageBuildingReplacementTask"] = value; }
        }
        // Propiedad para controlar el indice activo en la grilla
        public int CountRowsActualGrid
        {
            get
            {
                if (ValidateViewState("countRowsActualGrid"))
                    return (int)ViewState["countRowsActualGrid"];
                else
                    return -1;
            }

            set { ViewState["countRowsActualGrid"] = value; }
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
        public int currentIdWhs
        {
            get
            {
                if (ValidateViewState("idWhs"))
                    return (int)ViewState["idWhs"];
                else
                    return -1;
            }

            set { ViewState["idWhs"] = value; }
        }
        public int currentIdOwn
        {
            get
            {
                if (ValidateViewState("IdOwn"))
                    return (int)ViewState["IdOwn"];
                else
                    return -1;
            }

            set { ViewState["IdOwn"] = value; }
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
        public List<bool> ListCheckedOrders
        {
            get
            {
                if (ValidateViewState("listCheckedOrders"))
                    return (List<bool>)ViewState["listCheckedOrders"];
                else
                    return checkedOrdersCurrentView;
            }

            set { ViewState["listCheckedOrders"] = value; }
        }
        GenericViewDTO<Task> taskViewDTO;
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
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        if (ValidateSession(WMSTekSessions.ItemLocationMgr.ItemLocationList))
                        {
                            Session.Remove("WMSTekSessions.ItemLocationMgr.ItemLocationList");
                        }
                    }

                    if (ValidateSession(WMSTekSessions.ItemLocationMgr.ItemLocationList))
                    {
                        itemLocationViewDTO = (GenericViewDTO<ItemLocation>)Session[WMSTekSessions.ItemLocationMgr.ItemLocationList];
                        isValidViewDTO = true;
                    }

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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
                currentIndex = -1;
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    currentIndex = grdMgr.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
             base.grdMgr_RowDataBound(sender, e);

             if (e.Row.RowType == DataControlRowType.DataRow)
             {
                 string editIndex = e.Row.DataItemIndex.ToString();
                 //DataBinder.Eval(e.Row.DataItem, "ESTADO").ToString();
                 Label lblRepoPending = e.Row.FindControl("lblRepoPending") as Label;
                 Label lblStatusGeneration = e.Row.FindControl("lblStatusGeneration") as Label;
                 Label lblItemQty = e.Row.FindControl("lblItemQty") as Label;
                 Label lblReOrderQty = e.Row.FindControl("lblReOrderQty") as Label;
                 CheckBox chkSelectOrder = e.Row.FindControl("chkSelectOrder") as CheckBox;
                 
                 //Deshabilita el CheckBox
                 if (lblRepoPending != null)
                 {
                     if (Convert.ToDecimal(lblRepoPending.Text) > 0)
                     {
                         //Deshabilito el chekbox
                         chkSelectOrder.Enabled = false;
                         lblRepoPending.Text = "Si";
                     }
                     else
                         lblRepoPending.Text = "No";
                 }
                 if (lblItemQty != null )
                 {
                     if (lblItemQty.Text.Trim() != string.Empty)
                     {
                         //Deshabilita checkbox si cantidad en stock es mayor o igual que la cantidad maxima en itemlocation
                         if (Convert.ToDecimal(lblItemQty.Text) >= Convert.ToDecimal(lblReOrderQty.Text))
                         {
                             chkSelectOrder.Enabled = false;
                         }
                     }
                 }

                 if (lblStatusGeneration != null)
                 {
                     if (lblStatusGeneration.Text == "OK")
                     {
                         lblStatusGeneration.ForeColor = System.Drawing.Color.Blue;
                     }
                     if (lblStatusGeneration.Text == "Sin Stock")
                     {
                         lblStatusGeneration.ForeColor = System.Drawing.Color.Red;
                     }
                 }
             }
        }

        protected void btnReprocess_Click(object sender, EventArgs e)
        {
            GenerateTask();
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                //SaveChequedOrders();
                currentPage = currentPage - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                //SaveChequedOrders();
                currentPage = currentPage + 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
                itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
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
        //        itemLocationViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
        //    }
        //}
        #endregion

        #region "Metodos"
        public void Initialize()
        {
            try
            {
                //context.SessionInfo.IdPage = "MaxAndMinByLocationMgr";
                context.SessionInfo.IdPage = "BuildingReplacementTask";
                InitializeTaskBar();
                InitializeFilter(!Page.IsPostBack, false);
                InitializeStatusBar();
                InitializeGrid();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// Carga en sesion lista de Usuarios
        /// </summary>
        /// <param name="showError"></param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
                itemLocationViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            itemLocationViewDTO = iWarehousingMGR.BuildingReplacementTaskConsult(context);

            if (!itemLocationViewDTO.hasError() && itemLocationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemLocationMgr.ItemLocationList, itemLocationViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(itemLocationViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
                Session.Remove(WMSTekSessions.ItemLocationMgr.ItemLocationList);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            try
            {
                // Habilita criterios a usar Filtro Basico
                this.Master.ucMainFilter.warehouseVisible = true;
                this.Master.ucMainFilter.warehouseNotIncludeAll = true;//Centro Obligatorio
                this.Master.ucMainFilter.ownerVisible = true;
                this.Master.ucMainFilter.ownerNotIncludeAll = false;//Owner NO Obligatorio
                this.Master.ucMainFilter.codeVisible = true;
                this.Master.ucMainFilter.nameVisible = true;
                this.Master.ucMainFilter.SetDefaultOwner = true;
                //Filtro Avanzado
                this.Master.ucMainFilter.advancedFilterVisible = true;
                this.Master.ucMainFilter.tabLocationVisible = true;
                this.Master.ucMainFilter.locationFilterVisible = true;                

                //Esconde filtros no utilizados
                this.Master.ucMainFilter.LocationEqualVisible = false;
                this.Master.ucMainFilter.LocationRangeVisible = false;
                this.Master.ucMainFilter.LocationRowColumnEqualVisible = false;
                this.Master.ucMainFilter.LocationLevelEqualVisible = false;
                this.Master.ucMainFilter.LocationAisleEqualVisible = false;
                this.Master.ucMainFilter.ChkDisabledAndChequed = false;
                this.Master.ucMainFilter.LocationFilterType = context.SessionInfo.IdPage;

                // Configura textos a mostar
                this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
                this.Master.ucMainFilter.descriptionLabel = lblFilterDescription.Text;

                //Elimino los filtros con postback innecesarios
                this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;
                this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;

                //Cargo el Objeto Filtro antes de inicializar
                context.MainFilter = this.Master.ucMainFilter.MainFilter;

                this.Master.ucMainFilter.Initialize(init, refresh);
                this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);

            //Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
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

        private void PopulateGrid()
        {
            try
            {
                grdMgr.PageIndex = currentPage;

                // Configura ORDEN de las columnas de la grilla
                if (!itemLocationViewDTO.hasConfigurationError() && itemLocationViewDTO.Configuration != null && itemLocationViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, itemLocationViewDTO.Configuration);

                grdMgr.DataSource = itemLocationViewDTO.Entities;
                grdMgr.DataBind();

                ucStatus.ShowRecordInfo(itemLocationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
                CountRowsActualGrid = grdMgr.Rows.Count;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        protected void ReloadData()
        {
            crud = false;
            InitializeCheckedRows();
            
            UpdateSession(false);
            InitializeListChequedOrders();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Limpia la lista de checkboxes seleccionados
        /// </summary>
        protected void InitializeCheckedRows()
        {
            CheckedOrdersCurrentView.Clear();

            if (ValidateSession(WMSTekSessions.ItemLocationMgr.ItemLocationList))
                itemLocationViewDTO = (GenericViewDTO<ItemLocation>)Session[WMSTekSessions.ItemLocationMgr.ItemLocationList];

            if (itemLocationViewDTO.Entities != null && CheckedOrdersCurrentView != null)
            {
                for (int i = 0; i < itemLocationViewDTO.Entities.Count; i++)
                {
                    CheckedOrdersCurrentView.Add(false);
                }
            }
        }

        protected void InitializeListChequedOrders()
        {
            ListCheckedOrders.Clear();

            if (ValidateSession(WMSTekSessions.ItemLocationMgr.ItemLocationList))
                itemLocationViewDTO = (GenericViewDTO<ItemLocation>)Session[WMSTekSessions.ItemLocationMgr.ItemLocationList];

            if (itemLocationViewDTO.Entities != null && ListCheckedOrders != null)
            {
                for (int i = 0; i < itemLocationViewDTO.Entities.Count; i++)
                {
                    ListCheckedOrders.Add(false);
                }
            }
        }

        //Almacena en una lista de booleanos las ordenes que han sido seleccionadas
        //Esto es para cuando cambie de pagina
        private void SaveChequedOrders()
        {
            // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                int indexTotal = 0;

                indexTotal = (grdMgr.PageSize * (currentPage + 1)) - (grdMgr.PageSize - i);
                GridViewRow row = grdMgr.Rows[i];
                //ListCheckedOrders[indexTotal] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;
                ((CheckBox)row.FindControl("chkSelectOrder")).Checked = true;
                //Session.Add(WMSTekSessions.ItemLocationMgr.ItemLocListSelectedTotal, ListCheckedOrders);

                // Valida que se haya seleccionado al menos una orden
                //if (ListCheckedOrders[indexTotal])
                //{
                //    existSelected = true;
                //    countChequed = +1;
                //}
            }
            existSelected = false;
        }

        private void GenerateTask()
        {
            GenericViewDTO<ItemLocation> selectedOrdersViewDTO = new GenericViewDTO<ItemLocation>();
            int indexTotal = 0;

            try
            {

                //Inicializa la lista de booleanos
                InitializeCheckedRows();

                try
                {
                    // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
                    for (int i = 0; i < grdMgr.Rows.Count; i++)
                    {
                        indexTotal = (grdMgr.PageSize * (currentPage + 1)) - (grdMgr.PageSize - i);
                        GridViewRow row = grdMgr.Rows[i];

                       
                        CheckedOrdersCurrentView[indexTotal] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;

                        // Valida que se haya seleccionado al menos una orden
                        if (CheckedOrdersCurrentView[indexTotal])
                            existSelected = true;

                        Session.Add(WMSTekSessions.ItemLocationMgr.ItemLocationListSelected, CheckedOrdersCurrentView);

                    }
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                if (existSelected)
                {
                    if (ValidateSession(WMSTekSessions.ItemLocationMgr.ItemLocationList))
                    {
                        itemLocationViewDTO = (GenericViewDTO<ItemLocation>)Session[WMSTekSessions.ItemLocationMgr.ItemLocationList];
                        isValidViewDTO = true;
                    }

                    //Recupera las ubicaciones
                    for (int k = 0; k < itemLocationViewDTO.Entities.Count; k++)
                    {
                        if (CheckedOrdersCurrentView[k])
                        {
                            ItemLocation selectedOrder = new ItemLocation();
                            // Recupera la Orden seleccionada
                            selectedOrder = itemLocationViewDTO.Entities[k];

                            // Agrega la Orden seleccionada a la lista de Ordenes seleccionadas
                            selectedOrdersViewDTO.Entities.Add(selectedOrder);
                        }
                    }
                    string typeTaskCode = string.Empty;
                    //List<string> listaConstTypeTask = new List<string>();
                    //listaConstTypeTask = GetConst("TypeTaskCodes");
                    //typeTaskCode = listaConstTypeTask[Convert.ToInt16(TypeTaskCode.REPL)];
                    typeTaskCode = TypeTaskCode.REPL.ToString();

                    //Llama a Generar tareas de reposicion.
                    selectedOrdersViewDTO = iTasksMGR.GenerateTaskReplenishmentNew(context, selectedOrdersViewDTO, typeTaskCode);

                    if (selectedOrdersViewDTO.hasError())
                    {
                        isValidViewDTO = false;
                        itemLocationViewDTO.Errors = baseControl.handleError(new ErrorDTO(selectedOrdersViewDTO.Errors.Code, context));
                        this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
                    }
                    else
                    {
                        //Actualizo la Grilla con el resultado de la generación
                        this.grdMgr.DataSource = selectedOrdersViewDTO.Entities;
                        this.grdMgr.DataBind();
                    }
                }
                else
                {
                    isValidViewDTO = false;
                    itemLocationViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.NotLocationSelected, context));
                    this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
                }
            }
            catch (Exception ex)
            {
                isValidViewDTO = false;
                itemLocationViewDTO.Errors = baseControl.handleException(ex);
                this.Master.ucError.ShowError(itemLocationViewDTO.Errors);
            }
        }
        

        #endregion
    }
}
