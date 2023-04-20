using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Inventory;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Administration.Inventory
{
    public partial class InventoryLocation : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<InventoryOrder> inventoryViewDTO = new GenericViewDTO<InventoryOrder>();
        GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation> inventoryLocationViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation>();
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
        // Propiedad para controlar el indice del objeto que se está editando
        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return 0;
            }

            set { ViewState["index"] = value; }
        }

        public int currentIdInventory
        {
            get
            {
                if (ValidateViewState("IdInventory"))
                    return (int)ViewState["IdInventory"];
                else
                    return 0;
            }
            set { ViewState["IdInventory"] = value; }
        }

        public int currentInventoryNumber
        {
            get
            {
                if (ValidateViewState("InventoryNumber"))
                    return (int)ViewState["InventoryNumber"];
                else
                    return 0;
            }
            set { ViewState["InventoryNumber"] = value; }
        }

        public int currentIdWarehouse
        {
            get
            {
                if (ValidateViewState("IdWarehouse"))
                    return (int)ViewState["IdWarehouse"];
                else
                    return 0;
            }
            set { ViewState["IdWarehouse"] = value; }
        }

        #endregion

        #region "Configuracion de Pagina"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {                

                //if (!IsPostBack)
                //{
                    if (Request.QueryString["IdWhs"] != string.Empty)
                    {
                        currentIdWarehouse = Convert.ToInt32(Request.QueryString["IdWhs"]);
                    }
                    if (Request.QueryString["IdInventory"] != string.Empty)
                    {
                        currentIdInventory = Convert.ToInt32(Request.QueryString["IdInventory"]);
                    }
                    if (Request.QueryString["InventoryNumber"] != string.Empty)
                    {
                        currentInventoryNumber = Convert.ToInt32(Request.QueryString["InventoryNumber"]);
                        lblEdit.Text = lblEdit.Text + " " + currentInventoryNumber;
                    }                    
                //}

                base.Page_Init(sender, e);
                
                //// Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearchAux_Click(object sender, EventArgs e)
        {
            try
            {
                //ReloadData();
                //this.context.MainFilter = this.Master.ucMainFilter.MainFilter;
                PopulateLists();
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
            }
        }

        protected void ddlHangar_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                PopulateLists();
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                }
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = true;
                int levelFrom = Convert.ToInt32(this.ddlLevelFrom.SelectedValue);
                int levelTo = Convert.ToInt32(this.ddlLevelTo.SelectedValue);
                int columnFrom = Convert.ToInt32(this.ddlColumnFrom.SelectedValue);
                int columnTo = Convert.ToInt32(this.ddlColumnTo.SelectedValue);
                int rowFrom = Convert.ToInt32(this.ddlRowFrom.SelectedValue);
                int rowTo = Convert.ToInt32(this.ddlRowTo.SelectedValue);
                var lstTypeLoc = GetConst("LocationTypeDif");
 
                this.Master.ucMainFilter.ClearFilterObject();

                if (this.Master.ucMainFilter.idHangar < 0)
                {
                    foreach (ListItem item in this.Master.ucMainFilter.hangarItems)
                    {
                        if (int.Parse(item.Value) > 0)
                        {
                            this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Add(new FilterItem(item.Value));
                        }
                    }
                }
                else
                {
                    this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Add(new FilterItem(this.Master.ucMainFilter.idHangar.ToString()));
                }

                //Valida que las ubicaciones sean distintas de Truck, Forklift, Stage
                for (int i = 0; i < lstTypeLoc.Count; i++)
                {
                    this.Master.ucMainFilter.MainFilter.Find(w => w.Name.ToUpper() == "LOCATIONTYPE").FilterValues.Add(new FilterItem(lstTypeLoc[i].ToString()));
                }


                context.MainFilter = this.Master.ucMainFilter.MainFilter;


                //validaciones
                if (levelTo < levelFrom)
                {
                    this.lblError.Text = this.lblErrorLevel.Text;
                    IsValid = false;
                }
                if (columnTo < columnFrom)
                {
                    this.lblError.Text = this.lblErrorColunm.Text;
                    IsValid = false;
                }
                if (rowTo < rowFrom)
                {
                    this.lblError.Text = this.lblErrorRow.Text;
                    IsValid = false;
                }
                if (IsValid)
                {

                    inventoryLocationViewDTO = iLayoutMGR.AddLocation(levelFrom, levelTo, rowFrom, rowTo, columnFrom, columnTo, this.currentIdWarehouse, currentIdInventory, context);

                    if (inventoryLocationViewDTO.Entities.Count != 0)
                    {
                        //Limpia el mensaje de error
                        this.lblError.Text = string.Empty;

                        //Actualiza la grilla
                        UpdateSession(false);
                    }
                    else
                    {
                        this.lblError.Text = lblErrorNoRowsAdd.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            
            //Response.Redirect("~/Inventory/Administration/InventoryMgr.aspx", false);
            ScriptManager.RegisterStartupScript(Page, GetType(), "Success", "HideScreemLocation();", true);   
            
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool IsValid = true;
                int levelFrom = Convert.ToInt32(this.ddlLevelFrom.SelectedValue);
                int levelTo = Convert.ToInt32(this.ddlLevelTo.SelectedValue);
                int columnFrom = Convert.ToInt32(this.ddlColumnFrom.SelectedValue);
                int columnTo = Convert.ToInt32(this.ddlColumnTo.SelectedValue);
                int rowFrom = Convert.ToInt32(this.ddlRowFrom.SelectedValue);
                int rowTo = Convert.ToInt32(this.ddlRowTo.SelectedValue);

                //validaciones
                if (levelTo < levelFrom)
                {
                    this.lblError.Text = this.lblErrorLevel.Text;
                    IsValid = false;
                }
                if (columnTo < columnFrom)
                {
                    this.lblError.Text = this.lblErrorColunm.Text;
                    IsValid = false;
                }
                if (rowTo < rowFrom)
                {
                    this.lblError.Text = this.lblErrorRow.Text;
                    IsValid = false;
                }
                if (IsValid)
                {
                    inventoryLocationViewDTO = iLayoutMGR.DeleteLocation(levelFrom, levelTo, rowFrom, rowTo, columnFrom, 
                        columnTo, this.currentIdWarehouse, this.currentIdInventory, context);

                    if (inventoryLocationViewDTO.Entities.Count != 0)
                    {
                        //Limpia el mensaje de error
                        this.lblError.Text = string.Empty;

                        //Actualiza la grilla
                        UpdateSession(false);
                    }
                    else
                    {
                        this.lblError.Text = lblErrorNoRowsDelete.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                inventoryLocationViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
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
        //        inventoryViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(inventoryViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "InventoryMgr";
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                UpdateSession(false);
                PopulateLists();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InventoryMgr.InventoryLocationList))
                {
                    inventoryLocationViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Inventory.InventoryLocation>)Session[WMSTekSessions.InventoryMgr.InventoryLocationList];
                    PopulateGrid();
                }
            }
        }

        /// <summary>
        /// Carga en sesion la lista de la Entidad
        /// </summary>
        /// <param name="showError">Determina si se mostrara el error producido en una operacion anterior</param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
                inventoryLocationViewDTO.ClearError();
            }
            inventoryLocationViewDTO = iInventoryMGR.GetLocationByWhsAndInventory(currentIdWarehouse, currentIdInventory, context);

            if (!inventoryLocationViewDTO.hasError() && inventoryLocationViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InventoryMgr.InventoryLocationList, inventoryLocationViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(inventoryLocationViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(inventoryLocationViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.Visible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseDisabled = true;
            this.Master.ucMainFilter.hangarVisible = true;
            this.Master.ucMainFilter.searchAuxVisible = false;
            this.Master.ucMainFilter.searchVisible = false;

            //this.Master.ucMainFilter.idWhs = currentIdWarehouse;
            //Setea los filtros para que no tengan la propiedad de autopostback
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterHangarAutoPostBack = true;
       

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.LoadHangarToWarehouseControls(currentIdWarehouse);
            
            this.Master.ucMainFilter.BtnSearchClickAux += new EventHandler(btnSearchAux_Click);
            this.Master.ucMainFilter.ddlHangarIndexChanged += new EventHandler(ddlHangar_SelectedIndexChanged);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            //this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            //this.Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            this.Master.ucTaskBar.btnNewVisible = false;
            this.Master.ucTaskBar.btnRefreshVisible = false;
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
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!inventoryLocationViewDTO.hasConfigurationError() && inventoryLocationViewDTO.Configuration != null && inventoryLocationViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, inventoryLocationViewDTO.Configuration);

            grdMgr.DataSource = inventoryLocationViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(inventoryLocationViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void PopulateLists()
        {

            FilterItem filterHangar = new FilterItem("", this.Master.ucMainFilter.idHangar.ToString());
            FilterItem filterWhs = new FilterItem("", this.Master.ucMainFilter.idWhs.ToString());
            
            ContextViewDTO contextoNew = new ContextViewDTO();
            contextoNew.MainFilter = this.Master.ucMainFilter.MainFilter;

            foreach (var item in contextoNew.MainFilter)
            {
                if (item.Name.ToUpper() == "WAREHOUSE" && item.FilterValues.Count > 0)
                {
                    contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "WAREHOUSE").FilterValues.Remove(item.FilterValues[0]);
                }else
                    if (item.Name.ToUpper() == "HANGAR" && item.FilterValues.Count > 0)
                    {
                        contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Remove(item.FilterValues[0]);
                    }
            }

            //Agrega los filtro ocupados
            if (this.Master.ucMainFilter.idHangar>0)
                contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Add(filterHangar);

            if (this.Master.ucMainFilter.idWhs > 0)
                contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "WAREHOUSE").FilterValues.Add(filterWhs);

            //Carga los combos Fila, Columna, Nivel
            base.LoadRowLocWihtEntities(this.ddlRowFrom, this.ddlRowTo, true, contextoNew);
            base.LoadColumnLocWihtEntities(this.ddlColumnFrom, this.ddlColumnTo, true,contextoNew);
            base.LoadLevelLocWihtEntities(this.ddlLevelFrom, this.ddlLevelTo, true,contextoNew);

            //Elimina los filtro ocupados
            if (this.Master.ucMainFilter.idHangar > 0)
                contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "HANGAR").FilterValues.Remove(filterHangar);

            if (this.Master.ucMainFilter.idWhs > 0)
                contextoNew.MainFilter.Find(w => w.Name.ToUpper() == "WAREHOUSE").FilterValues.Remove(filterWhs);

            //base.LoadColumnLocation(this.ddlColumnTo, this.Master.EmptyRowText, "-1", true);
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        #endregion
    }
}
