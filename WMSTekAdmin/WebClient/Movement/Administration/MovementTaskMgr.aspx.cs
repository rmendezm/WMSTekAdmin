using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;

namespace Binaria.WMSTek.WebClient.Movement.Consult
{
    public partial class MovementTaskMgr : BasePage
    {
        private GenericViewDTO<TaskConsult> taskConsultViewDTO = new GenericViewDTO<TaskConsult>();
        private bool isValidViewDTO = false;
        //private GenericViewDTO<Item> itemSearchViewDTO;
        private GenericViewDTO<Location> locationSearchViewDTO;
      
        
        #region Propiedades
        
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

        public int currentPageLocations
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

        // Propiedad para controlar el nro de pagina activa en la grilla Customer
        public int currentPageCustomer
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

        // Propiedad para controlar el nro de pagina activa en la grilla Item
        public int currentPageItems
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

        #region Objects Page
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
                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                        PopulateLists();
                    }

                    if (ValidateSession(WMSTekSessions.MovementConsult.MovementTaskList))
                    {
                        taskConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.MovementConsult.MovementTaskList];
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        #endregion


        protected void Initialize()
        {
            context.SessionInfo.IdPage = "MovementTaskMgr";

            InitializeFilter(!Page.IsPostBack, false);        
            InitializeFilterLocation();
            InitializeTaskBar();
            InitializeStatusBar();
            InitializeGrid();

        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
                taskConsultViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            List<string> typeTasck = GetConst("TaskTypeOfMovementTaskMgr");
            context.MainFilter[Convert.ToInt16(EntityFilterName.TaskType)].FilterValues.Clear();

            foreach (string type in typeTasck)
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.TaskType)].FilterValues.Add(new FilterItem(type.Trim()));
            }
            

            taskConsultViewDTO = iTasksMGR.FindAllMovementTaskMgr(context);

            if (!taskConsultViewDTO.hasError() && taskConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.MovementConsult.MovementTaskList, taskConsultViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(taskConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!taskConsultViewDTO.hasConfigurationError() && taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskConsultViewDTO.Configuration);

            grdMgr.DataSource = taskConsultViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(taskConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwnerTask, this.Master.EmptyRowText, "-1", true, string.Empty, false); ;
            base.LoadUserWarehouses(this.ddlWarehouseTask, this.Master.EmptyRowText, "-1", true);            
        }

        private void LoadLpnList(DropDownList objControl, string locCode, string idWhs, string idOwner)
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>();

            foreach (var item in Enum.GetNames(typeof(EntityFilterName)))
            {
                EntityFilter entity = new EntityFilter(item.ToString(), new FilterItem());
                newContext.MainFilter.Add(entity);
            }
            foreach (EntityFilter entityFilter in newContext.MainFilter)
            {
                entityFilter.FilterValues.Clear();
            }
            
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(idWhs));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(idOwner));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Location)].FilterValues.Add(new FilterItem(locCode));
            
            stockViewDTO = iWarehousingMGR.FindAllStockLocation(newContext);

            if (stockViewDTO.Entities != null && stockViewDTO.Entities.Count > 0)
            {
                var lpnList = stockViewDTO.Entities.ToList().Select(s => s.Lpn.Code).Distinct();

                objControl.Enabled = true;
                objControl.Items.Clear();
                objControl.DataSource = lpnList;
                objControl.DataBind();

                objControl.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));

                if (lpnList.Count() > 1)
                {
                    objControl.Items[0].Selected = true;
                }
                else
                {
                    objControl.Items[1].Selected = true;                    
                }
            }
            else
            {
                objControl.ClearSelection();
                objControl.DataSource = null;
                objControl.DataBind();

                objControl.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                objControl.Items[0].Selected = true;

                objControl.Enabled = false;
            }

            //Session.Add(WMSTekSessions.MovementConsult.MovementTaskList, taskConsultViewDTO);
        }

        private void LoadItemList(string locCode, string idWhs, string idOwner, string lpnCode)
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
            List<Item> lstItem = new List<Item>();
            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>();
            this.txtTaskByItemDescription.Text = "";

            foreach (var item in Enum.GetNames(typeof(EntityFilterName)))
            {
                EntityFilter entity = new EntityFilter(item.ToString(), new FilterItem());
                newContext.MainFilter.Add(entity);
            }
            foreach (EntityFilter entityFilter in newContext.MainFilter)
            {
                entityFilter.FilterValues.Clear();
            }

            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(idWhs));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(idOwner));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Location)].FilterValues.Add(new FilterItem(locCode));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.LpnSource)].FilterValues.Add(new FilterItem(lpnCode));

            stockViewDTO = iWarehousingMGR.GetStockByFilters(newContext);

            
            if (stockViewDTO.Entities != null && stockViewDTO.Entities.Count > 0)
            {
                lstItem = stockViewDTO.Entities.ToList().Select(s => s.Item).Distinct().ToList();
                var newListItem = (from a in lstItem
                                          select new 
                                          {
                                              Id = a.Id,
                                              Code = a.Code,
                                              Description = a.Description                                              
                                              
                                          }).Distinct().ToList();

                ddlCodItem.Enabled = true;
                ddlCodItem.Items.Clear();
                ddlCodItem.DataSource = newListItem;
                ddlCodItem.DataTextField = "Code";
                ddlCodItem.DataValueField = "Id";
                ddlCodItem.DataBind();

                ddlCodItem.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));

                if (newListItem.Count() > 1)
                {
                    ddlCodItem.Items[0].Selected = true;
                }
                else
                {
                    ddlCodItem.Items[1].Selected = true;
                    this.txtTaskByItemDescription.Text = newListItem[0].Description;


                    GenericViewDTO<StockConsultTask> stockViewDto = new GenericViewDTO<StockConsultTask>();
                    stockViewDto = iWarehousingMGR.GetStockConsultTaskByParameters(this.hidtxtSourceLocItem.Value.Trim(), newListItem[0].Id.ToString(), -1, null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue,
                                    this.ddlWarehouseTask.SelectedValue.Trim(),
                                    this.ddlOwnerTask.SelectedValue.Trim(), context);

                    if (stockViewDto.hasError())
                    {
                        ErrorDTO error = stockViewDto.Errors;
                        this.Master.ucError.ShowError(error);
                    }
                    else
                    {
                        if (stockViewDto.Entities.Count > 0)
                        {
                            this.txtItemQtyDisp.Text = stockViewDto.Entities[0].ItemQtyDisp < 0 ? "0" : stockViewDto.Entities[0].ItemQtyDisp.ToString();
                            this.txtItemQtyTask.Text = stockViewDto.Entities[0].ItemQtyTask < 0 ? "0" : stockViewDto.Entities[0].ItemQtyTask.ToString(); ;
                        }
                        else
                        {
                            this.txtItemQtyDisp.Text = "0";
                            this.txtItemQtyTask.Text = "0";
                        }
                    }
                }
            }
            else
            {
                ddlCodItem.ClearSelection();
                ddlCodItem.DataSource = null;
                ddlCodItem.DataBind();

                ddlCodItem.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                ddlCodItem.Items[0].Selected = true;

                ddlCodItem.Enabled = false;
            }

            Session.Add(WMSTekSessions.MovementConsult.MovementTaskListItem, lstItem);
        }

        private Item TakeOneItem(string locCode, string idWhs, string idOwner, string lpnCode)
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock> stockViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();
            Item tkItem = new Item();
            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>();
   

            foreach (var item in Enum.GetNames(typeof(EntityFilterName)))
            {
                EntityFilter entity = new EntityFilter(item.ToString(), new FilterItem());
                newContext.MainFilter.Add(entity);
            }
            foreach (EntityFilter entityFilter in newContext.MainFilter)
            {
                entityFilter.FilterValues.Clear();
            }

            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(idWhs));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(idOwner));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Location)].FilterValues.Add(new FilterItem(locCode));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.LpnSource)].FilterValues.Add(new FilterItem(lpnCode));

            stockViewDTO = iWarehousingMGR.GetStockByFilters(newContext);

            if (stockViewDTO.Entities != null && stockViewDTO.Entities.Count > 0)
            {
                tkItem = stockViewDTO.Entities.ToList().Select(s => s.Item).Distinct().First();

                return tkItem;
            }
            else
            {
                return new Item();
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            this.Master.ucMainFilter.trackTaskTypeVisible = true;
            this.Master.ucMainFilter.trackTaskTypeNotIncludeAll = false;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.InboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.InboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }


        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            //this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            //this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnNewVisible = true;
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

        private void InitializeFilterLocation()
        {
            ucFilterLocation.Initialize();
            ucFilterLocation.BtnSearchClick += new EventHandler(btnSearchLocation_Click);

            ucFilterLocation.FilterCode = this.lblFilterCodeLoc.Text;
            ucFilterLocation.FilterDescription = this.lblFilterNameLoc.Text;
        }

        #region Buttons
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

       

        

        

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
                
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

       

        

        protected void RadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.rbTaskTypeItem.Checked == true)
            {
                this.divTaskByItem.Visible = true;
                this.divTaskByLpn.Visible = false;
            }
            else
            {
                this.divTaskByItem.Visible = false;
                this.divTaskByLpn.Visible = true;
            }

            ClearControlsTask();

            this.lblNew.Visible = true;
            this.divModal.Visible = true;
            this.divGrid.Visible = false;
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
            }
        }

        protected void ShowModal(int index, CRUD mode)
        {
            // Selecciona Warehouse y Owner seleccionados en el Filtro
            this.ddlWarehouseTask.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues[0].Value;
            this.ddlOwnerTask.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

            if (taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(taskConsultViewDTO.Configuration, true);
                else
                    base.ConfigureModal(taskConsultViewDTO.Configuration, false);
            }
                                    
            ClearControlsTask();

            this.rbTaskTypeItem.Checked = true;
            this.rbTaskTypeLpn.Checked = false;
            this.divTaskByItem.Visible = true;
            this.divTaskByLpn.Visible = false;

            this.lblNew.Visible = true;
            this.divModal.Visible = true;
            this.divGrid.Visible = false;           
        }

        

        protected void btnCloseNewEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    divGrid.Visible = true;
                    divModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        

        protected void SaveChanges()
        {
            try
            {
                string locOrigen;
                string locDestino;
                string lpnOrigen;
                string lpnDestino;
                string descriptionTask;
                string typeCode;
                DateTime dateCreate = DateTime.Now;

                Task newTask = new Task();     
                TaskDetail newTaskDetail = new TaskDetail();
                newTask.Warehouse = new Warehouse();
                newTask.Owner = new Owner();
                newTask.StageSource = new Location();
                newTask.StageTarget = new Location();
                newTask.TaskDetails = new List<TaskDetail>();
                
                if (this.rbTaskTypeItem.Checked){
                    typeCode = "MDITM";
                    descriptionTask = "Mov. Dirigido por Item";

                    locOrigen  = this.hidtxtSourceLocItem.Value.Trim();
                    locDestino = this.hidtxtDestLocItem.Value.Trim();
                    lpnOrigen = this.ddlSourceLpnItem.SelectedValue.Trim();
                    lpnDestino = (this.ddlDestLpnItem.SelectedValue.Trim()== "-1" ? null : this.ddlDestLpnItem.SelectedValue.Trim());
                    
                    newTaskDetail.Item = new Item();
                    newTaskDetail.Item.Id = int.Parse(this.ddlCodItem.SelectedValue.Trim());
                    newTaskDetail.ProposalQty = decimal.Parse(this.txtQtyItem.Text.Trim());                    
                    
                }else{
                    typeCode = "MDLPN";
                    descriptionTask = "Mov. Dirigido por Lpn";

                    locOrigen  = this.hidtxtSourceLocLpn.Value.Trim();
                    locDestino = this.hidtxtDestLocLpn.Value.Trim();
                    lpnOrigen  = this.ddlSourceLpn.SelectedValue.Trim();
                    lpnDestino = this.ddlSourceLpn.SelectedValue.Trim();

                    newTaskDetail.Item = TakeOneItem(locOrigen, this.ddlWarehouseTask.SelectedValue.Trim(), this.ddlOwnerTask.SelectedValue.Trim(), this.ddlSourceLpn.SelectedValue.Trim());
                }
                
                //Cabezera
                newTask.Warehouse.Id = int.Parse(this.ddlWarehouseTask.SelectedValue.Trim());
                newTask.Owner.Id = int.Parse(this.ddlOwnerTask.SelectedValue.Trim());
                newTask.IsComplete = false;
                newTask.TypeCode = typeCode;
                newTask.Description = descriptionTask;
                newTask.Priority = int.Parse(this.txtPriority.Text.Trim());
                newTask.CreateDate = dateCreate;
                newTask.ProposalStartDate = dateCreate;
                newTask.ProposalEndDate = dateCreate;
                newTask.Status = true;
                newTask.DateTrackTask = dateCreate;
                newTask.StageSource.IdCode = typeCode == "MDLPN" ? null : locOrigen;
                newTask.StageTarget.IdCode = typeCode == "MDLPN" ? null : locDestino;
                newTask.WorkersRequired = 1;
                newTask.WorkersAssigned = 0;
                newTask.AllowCrossDock = false;
                newTask.IdTrackTaskType = Convert.ToInt16(TrackTaskTypeName.Liberada);
          

                //Detalle
                newTaskDetail.Warehouse = new Warehouse();
                newTaskDetail.Warehouse.Id = int.Parse(this.ddlWarehouseTask.SelectedValue.Trim());
                newTaskDetail.IsComplete = false;
                newTaskDetail.LineNumber = 1;
                newTaskDetail.Priority = 0;
                newTaskDetail.IdLocSourceProposal = locOrigen;
                newTaskDetail.IdLocTargetProposal = locDestino;
                newTaskDetail.IdLpnSourceProposal = lpnOrigen;
                newTaskDetail.IdLpnTargetProposal = lpnDestino;
                newTaskDetail.Status = true;
                newTaskDetail.MadeCrossDock = false;

                //Agrega la nueva tarea de detalle
                newTask.TaskDetails.Add(newTaskDetail);

                taskConsultViewDTO = iTasksMGR.CreateMovementTaskMgr(newTask, context);
                
                if (taskConsultViewDTO.hasError())
                {
                    UpdateSession(true);

                    divGrid.Visible = false;
                    divModal.Visible = true;
                }
                else
                {
                    //Muestra mensaje en la barra de status
                    crud = true;
                    ucStatus.ShowMessage(taskConsultViewDTO.MessageStatus.Message);
                    UpdateSession(false);
                    
                    this.divGrid.Visible = true;
                    this.divModal.Visible = false;
                }

            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }

        }

        protected void btnSearchLocation_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    string txtDescpLoc = Session["MovementTaskMgrTxtSelectLoc"].ToString().Trim();
                    bool SearchTypeSource = false;

                    switch (txtDescpLoc)
                    {
                        case ("txtSourceLocItem"):
                            SearchTypeSource = true;
                            break;                        
                        case ("txtSourceLocLpn"):
                            SearchTypeSource = true;
                            break;                        
                    }

                    SearchLocation(SearchTypeSource);

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    //if (isValidViewDTO)
                    //{
                    divLookupLocation.Visible = true;
                    mpLookupLocation.Show();

                    InitializePageCountLocations();
                    //}
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void imgBtnSearchLocation_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    ucFilterLocation.Clear();
                    ucFilterLocation.Initialize();
                    Session.Remove("MovementTaskMgrTxtSelectLoc");

                    ImageButton imbLocation = (ImageButton)sender;
                    TextBox txtLoc = new TextBox();
                    bool SearchTypeSource = false;
                    
                    switch (imbLocation.CommandName.Trim())
                    {
                        case ("txtSourceLocItem"):
                            txtLoc = this.txtSourceLocItem;
                            SearchTypeSource = true;
                            Session.Add("MovementTaskMgrTxtSelectLoc", "txtSourceLocItem");
                            break;
                        case ("txtDestLocItem"):
                            txtLoc = this.txtDestLocItem;
                            Session.Add("MovementTaskMgrTxtSelectLoc", "txtDestLocItem");
                            break;
                        case ("txtSourceLocLpn"):
                            txtLoc = this.txtSourceLocLpn;
                            SearchTypeSource = true;
                            Session.Add("MovementTaskMgrTxtSelectLoc", "txtSourceLocLpn");
                            break;
                        case ("txtDestLocLpn"):
                            txtLoc = this.txtDestLocLpn;
                            Session.Add("MovementTaskMgrTxtSelectLoc", "txtDestLocLpn");
                            break;                        
                    }

                    // Setea el filtro con el Item ingresado
                    if (txtLoc.Text.Trim() != string.Empty)
                    {
                        FilterItem filterItem = new FilterItem("%" + txtLoc.Text + "%");
                        filterItem.Selected = true;
                        ucFilterLocation.FilterItems[0] = filterItem;

                        ucFilterLocation.LoadCurrentFilter(ucFilterLocation.FilterItems);
                        SearchLocation(SearchTypeSource);
                    }
                    // Si no se ingresó ningún item, no se ejecuta la búsqueda
                    else
                    {
                        ClearGridLocation();
                    }
                    // Esto evita un bug de ajax
                    //valAddItem.Enabled = false;

                    divLookupLocation.Visible = true;
                    mpLookupLocation.Show();

                    InitializePageCountLocations();

                }

            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnFirstGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchLocations.SelectedIndex = 0;
            ddlPagesSearchLocationsSelectedIndexChanged(sender, e);
        }

        protected void btnPrevGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems > 0)
            {
                ddlPagesSearchLocations.SelectedIndex = currentPageItems - 1; ;
                ddlPagesSearchLocationsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnNextGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems < grdSearchLocations.PageCount)
            {
                ddlPagesSearchLocations.SelectedIndex = currentPageItems + 1; ;
                ddlPagesSearchLocationsSelectedIndexChanged(sender, e);

            }
        }

        protected void btnLastGrdSearchLocations_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchLocations.SelectedIndex = grdSearchLocations.PageCount - 1;
            ddlPagesSearchLocationsSelectedIndexChanged(sender, e);

        }

        protected void ddlPagesSearchLocationsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.LocationList))
            {
                locationSearchViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Shared.LocationList];

                currentPageLocations = ddlPagesSearchLocations.SelectedIndex;
                grdSearchLocations.PageIndex = currentPageLocations;

                grdSearchLocations.DataSource = locationSearchViewDTO.Entities;
                grdSearchLocations.DataBind();

                divLookupLocation.Visible = true;
                divModal.Visible = true;
                mpLookupLocation.Show();

                ShowLocationsButtonsPage();

            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void grdSearchLocations_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string editIndex = (Convert.ToString(grdSearchLocations.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));
                string idWhs = this.ddlWarehouseTask.SelectedValue.Trim();
                string idOwn = this.ddlOwnerTask.SelectedValue.Trim();

                if (ValidateSession(WMSTekSessions.Shared.LocationList))
                {
                    locationSearchViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Shared.LocationList];

                    foreach (Location location in locationSearchViewDTO.Entities)
                    {
                        if (location.Code == editIndex)
                        {
                            //Rescata el control de la ubicacion a utilizar
                            string txtDescpLoc = Session["MovementTaskMgrTxtSelectLoc"].ToString().Trim();

                            switch (txtDescpLoc)
                            {
                                case ("txtSourceLocItem"):
                                    this.txtSourceLocItem.Text = location.Code;
                                    hidtxtSourceLocItem.Value = location.Code.ToString();

                                    LoadLpnList(this.ddlSourceLpnItem, location.Code, idWhs, idOwn);

                                    if (this.ddlSourceLpnItem.Items.Count > 1)
                                    {
                                        LoadItemList(location.Code, idWhs, idOwn, this.ddlSourceLpnItem.SelectedValue.Trim());
                                    }
                                    break;
                                case ("txtDestLocItem"):
                                    this.txtDestLocItem.Text = location.Code;
                                    hidtxtDestLocItem.Value = location.Code.ToString();

                                    LoadLpnList(this.ddlDestLpnItem, location.Code, idWhs, idOwn);
                                    
                                    break;
                                case ("txtSourceLocLpn"):
                                    this.txtSourceLocLpn.Text = location.Code;
                                    hidtxtSourceLocLpn.Value = location.Code.ToString();

                                    LoadLpnList(this.ddlSourceLpn, location.Code, idWhs, idOwn);

                                    break;
                                case ("txtDestLocLpn"):
                                    this.txtDestLocLpn.Text = location.Code;
                                    hidtxtDestLocLpn.Value = location.Code.ToString();
                                    break;
                            }

                            Session.Add("MovementTaskMgrSearchLocation", location);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }


        private void ShowLocationsButtonsPage()
        {
            if (currentPageItems == grdSearchLocations.PageCount - 1)
            {
                btnNextGrdSearchLocations.Enabled = false;
                btnNextGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchLocations.Enabled = false;
                btnLastGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchLocations.Enabled = true;
                btnPrevGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchLocations.Enabled = true;
                btnFirstGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageItems == 0)
                {
                    btnPrevGrdSearchLocations.Enabled = false;
                    btnPrevGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchLocations.Enabled = false;
                    btnFirstGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchLocations.Enabled = true;
                    btnNextGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchLocations.Enabled = true;
                    btnLastGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchLocations.Enabled = true;
                    btnNextGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchLocations.Enabled = true;
                    btnLastGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchLocations.Enabled = true;
                    btnPrevGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchLocations.Enabled = true;
                    btnFirstGrdSearchLocations.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
        }

        private void InitializePageCountLocations()
        {
            if (grdSearchLocations.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchLocations.Visible = true;
                // Paginador
                ddlPagesSearchLocations.Items.Clear();
                for (int i = 0; i < grdSearchLocations.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageItems) lstItem.Selected = true;

                    ddlPagesSearchLocations.Items.Add(lstItem);
                }
                this.lblPageCountSearchLocations.Text = grdSearchLocations.PageCount.ToString();

                ShowLocationsButtonsPage();
            }
            else
            {
                divPageGrdSearchLocations.Visible = false;
            }
        }

        private void ClearGridLocation()
        {
            Session.Remove(WMSTekSessions.Shared.LocationList);
            grdSearchLocations.DataSource = null;
            grdSearchLocations.DataBind();
        }

        private void SearchLocation(bool SearchTypeSource)
        {
            ContextViewDTO contViewDTO = new ContextViewDTO();
            contViewDTO.MainFilter = new List<EntityFilter>();

            foreach (var item in Enum.GetNames(typeof(EntityFilterName)))
            {
                EntityFilter entity = new EntityFilter(item.ToString(), new FilterItem());
                contViewDTO.MainFilter.Add(entity);
            }
            foreach (EntityFilter entityFilter in contViewDTO.MainFilter)
            {
                entityFilter.FilterValues.Clear();
            }

            if (!string.IsNullOrEmpty(this.ddlWarehouseTask.SelectedValue.Trim()))
                contViewDTO.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(this.ddlWarehouseTask.SelectedValue.Trim()));

            List<string> typeTasck = GetConst("LocationTypeOfMovementTaskMgr");
            foreach (string type in typeTasck)
            {
                contViewDTO.MainFilter[Convert.ToInt16(EntityFilterName.LocationType)].FilterValues.Add(new FilterItem(type.Trim()));
            }

            if (SearchTypeSource)
            {
                locationSearchViewDTO = iLayoutMGR.GetLocationNotBlockedByFilterItemsAndEntityFilter(ucFilterLocation.FilterItems, contViewDTO);
            }
            else
            {
                locationSearchViewDTO = iLayoutMGR.GetLocationNotExistsStockByFilterItemsAndEntityFilter(ucFilterLocation.FilterItems, contViewDTO);
            }

            Session.Add(WMSTekSessions.Shared.LocationList, locationSearchViewDTO);
            InitializeGrid();
            grdSearchLocations.DataSource = locationSearchViewDTO.Entities;
            grdSearchLocations.DataBind();
        }

        private void ClearControlsTask(){

            //this.txtCodItem.Text = "";            
            //this.txtSourceLpn.Text = "";
            //this.txtSourceLpnItem.Text = "";
            this.txtTaskByItemDescription.Text = "";
            this.txtDestLpnItem.Text = "";
            this.txtQtyItem.Text = "";
            this.txtItemQtyDisp.Text = "";
            this.txtItemQtyTask.Text = "";

            this.txtSourceLocItem.Text = "";
            this.txtDestLocItem.Text = "";
            this.txtSourceLocLpn.Text = "";
            this.txtDestLocLpn.Text = ""; 

            this.hidtxtSourceLocItem.Value = "-1";
            this.hidtxtDestLocItem.Value = "-1";
            this.hidtxtSourceLocLpn.Value = "-1";
            this.hidtxtDestLocLpn.Value = "-1";                
            //this.hidItemId.Value = "0";

            this.txtTaskByItemDescription.BackColor = System.Drawing.Color.Lavender;
            this.txtSourceLocItem.BackColor = System.Drawing.Color.Lavender;
            this.txtDestLocItem.BackColor = System.Drawing.Color.Lavender;
            this.txtSourceLocItem.ReadOnly = true;
            this.txtDestLocItem.ReadOnly = true;

            this.txtSourceLocLpn.BackColor = System.Drawing.Color.Lavender;
            this.txtDestLocLpn.BackColor = System.Drawing.Color.Lavender;
            this.txtSourceLocLpn.ReadOnly = true;
            this.txtDestLocLpn.ReadOnly = true;

            this.txtItemQtyDisp.BackColor = System.Drawing.Color.Lavender;
            this.txtItemQtyTask.BackColor = System.Drawing.Color.Lavender;
            this.txtItemQtyDisp.ReadOnly = true;
            this.txtItemQtyTask.ReadOnly = true;

            this.ddlSourceLpnItem.ClearSelection();
            this.ddlSourceLpnItem.DataSource = null;
            this.ddlSourceLpnItem.DataBind();
            this.ddlSourceLpnItem.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            this.ddlSourceLpnItem.Items[0].Selected = true;
            this.ddlSourceLpnItem.Enabled = false;

            this.ddlCodItem.ClearSelection();
            this.ddlCodItem.DataSource = null;
            this.ddlCodItem.DataBind();
            this.ddlCodItem.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            this.ddlCodItem.Items[0].Selected = true;
            this.ddlCodItem.Enabled = false;
            
            this.ddlSourceLpn.ClearSelection();
            this.ddlSourceLpn.DataSource = null;
            this.ddlSourceLpn.DataBind();
            this.ddlSourceLpn.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            this.ddlSourceLpn.Items[0].Selected = true;
            this.ddlSourceLpn.Enabled = false;

            this.ddlDestLpnItem.ClearSelection();
            this.ddlDestLpnItem.DataSource = null;
            this.ddlDestLpnItem.DataBind();
            this.ddlDestLpnItem.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            this.ddlDestLpnItem.Items[0].Selected = true;
            this.ddlDestLpnItem.Enabled = false;
            
        }

        protected void ddlSourceLpnItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadItemList(this.hidtxtSourceLocItem.Value, this.ddlWarehouseTask.SelectedValue, this.ddlOwnerTask.SelectedValue, this.ddlSourceLpnItem.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlWarehouseTask_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    ClearControlsTask();
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }

        protected void ddlCodItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    this.txtTaskByItemDescription.Text = "";
                    this.txtItemQtyDisp.Text = "";
                    this.txtItemQtyTask.Text = "";
                    int idItem = int.Parse(this.ddlCodItem.SelectedValue.Trim());

                    if (idItem != -1)
                    {
                        List<Item> lstItem = (List<Item>)Session[WMSTekSessions.MovementConsult.MovementTaskListItem];
                        this.txtTaskByItemDescription.Text = lstItem.Find(f => f.Id.Equals(idItem)).Description;
                        
                        GenericViewDTO<StockConsultTask> stockViewDto = new GenericViewDTO<StockConsultTask>();
                        stockViewDto = iWarehousingMGR.GetStockConsultTaskByParameters(this.hidtxtSourceLocItem.Value.Trim(), idItem.ToString(), -1, null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue,
                                        this.ddlWarehouseTask.SelectedValue.Trim(),
                                        this.ddlOwnerTask.SelectedValue.Trim(), context);

                        if (stockViewDto.hasError())
                        {
                            ErrorDTO error = stockViewDto.Errors;
                            this.Master.ucError.ShowError(error);
                        }
                        else
                        {
                            if (stockViewDto.Entities.Count > 0)
                            {
                                this.txtItemQtyDisp.Text = stockViewDto.Entities[0].ItemQtyDisp < 0 ? "0" : stockViewDto.Entities[0].ItemQtyDisp.ToString();
                                this.txtItemQtyTask.Text = stockViewDto.Entities[0].ItemQtyTask < 0 ? "0" : stockViewDto.Entities[0].ItemQtyTask.ToString(); ;
                            }
                            else
                            {
                                this.txtItemQtyDisp.Text = "0";
                                this.txtItemQtyTask.Text = "0";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDTO error = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(error);
            }
        }


        #endregion

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSearchLocations_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}
