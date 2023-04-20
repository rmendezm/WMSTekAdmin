using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class PickingTaskConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskConsult> pickingViewDTO = new GenericViewDTO<TaskConsult>();

        private bool isValidViewDTO = false;
        private string filterMov = string.Empty;
        private string entityNameProperty = string.Empty;
        private int idAux;

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
                        // Carga inicial del ViewDTO
                        //UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.PickingTaskMgr.PickingTaskList))
                    {
                        pickingViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.PickingTaskMgr.PickingTaskList];
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = ((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex;
                //int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt16(e.CommandArgument);

                ShowModal(index, CRUD.Update);

            }
            catch (Exception ex)
            {
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int index = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

               // SaveChanges(index);
            }
            catch (Exception ex)
            {
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Label lblTaskId = e.Row.FindControl("lblTaskId") as Label;
            ImageButton imgEdit = e.Row.FindControl("btnEdit") as ImageButton;

            base.grdMgr_RowDataBound(sender, e);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (pickingViewDTO.Entities.Count >= 0 && e.Row.RowIndex >= 0)
                {
                    if (pickingViewDTO.Entities[e.Row.RowIndex].Id != idAux)
                    {
                        idAux = pickingViewDTO.Entities[e.Row.RowIndex].Id;
                        lblTaskId.Visible = true;
                        imgEdit.Visible = true;
                    }
                    else
                    {
                        lblTaskId.Visible = false;
                        imgEdit.Visible = false;
                    }
                }
            }
            else
                idAux = 0;

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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
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
        //        movementViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(movementViewDTO.Errors);
        //    }
        //}



        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {

            // Configura ventana modal
            if (pickingViewDTO.Configuration != null && pickingViewDTO.Configuration.Count > 0)
            {
                base.ConfigureModal(pickingViewDTO.Configuration, false);
            }

            if (mode == CRUD.Update)
            {
                // Editar Tarea

                //Recupera los datos de la entidad a editar
                hidEditId.Value = pickingViewDTO.Entities[index].Id.ToString();

                //Carga controles
                LoadTrackTaskType(ddlTaskTrackTaskType, false, this.lblEmptyRow.Text);

                LoadLocationsByWhsAndListType(ddlTaskLocStageSource, pickingViewDTO.Entities[index].Task.Warehouse.Id,
                                              GetConst("LocationTypeStagingForTask"), this.lblEmptyRow.Text, true);
                LoadLocationsByWhsAndListType(ddlTaskLocStageTarget, pickingViewDTO.Entities[index].Task.Warehouse.Id,
                                              GetConst("LocationTypeStagingForTask"), this.lblEmptyRow.Text, true);

                LoadConst(ddlTaskIsComplete, "Condition", false, lblEmptyRow.Text);
                LoadConst(ddlTaskAllowCrossDock, "Condition", true, lblEmptyRow.Text);
                LoadConst(ddlTaskStatus, "Status", false, lblEmptyRow.Text);

                txtTaskId.Text = pickingViewDTO.Entities[index].Task.Id.ToString();
                txtTaskWhsName.Text = pickingViewDTO.Entities[index].Task.Warehouse.Name;
                txtTaskOwnName.Text = pickingViewDTO.Entities[index].Task.Owner.Name;

                ddlTaskIsComplete.SelectedValue = Convert.ToInt32(pickingViewDTO.Entities[index].Task.IsComplete).ToString();

                txtTaskName.Text = pickingViewDTO.Entities[index].TaskTypeName;
                txtTaskDescription.Text = pickingViewDTO.Entities[index].Task.Description;
                txtTaskDocumentBound.Text = pickingViewDTO.Entities[index].OutboundOrder.Number;
                txtTaskPriority.Text = pickingViewDTO.Entities[index].Task.Priority.ToString();
                txtTaskRealStartDate.Text = pickingViewDTO.Entities[index].Task.RealStartDate.ToString();

                ddlTaskStatus.SelectedValue = Convert.ToInt32(pickingViewDTO.Entities[index].Task.Status).ToString();

                if (pickingViewDTO.Entities[index].Task.IdTrackTaskType != null)
                    ddlTaskTrackTaskType.SelectedValue = pickingViewDTO.Entities[index].Task.IdTrackTaskType.ToString();
                
                if (pickingViewDTO.Entities[index].IdLocStageSource != null)
                    ddlTaskLocStageSource.SelectedValue = pickingViewDTO.Entities[index].IdLocStageSource;

                if (pickingViewDTO.Entities[index].IdLocStageTarget != null)
                    ddlTaskLocStageTarget.SelectedValue = pickingViewDTO.Entities[index].IdLocStageTarget;
               
                if (pickingViewDTO.Entities[index].Task.AllowCrossDock != null)
                    ddlTaskAllowCrossDock.SelectedValue = Convert.ToInt32(pickingViewDTO.Entities[index].Task.AllowCrossDock).ToString();

                txtTaskDateTrackTask.Text = pickingViewDTO.Entities[index].Task.DateTrackTask.ToString();
                txtTaskWorkersRequired.Text = pickingViewDTO.Entities[index].Task.WorkersRequired.ToString();
                txtTaskDateCreated.Text = pickingViewDTO.Entities[index].Task.DateCreated.ToString();
                txtTaskUserCreated.Text = pickingViewDTO.Entities[index].Task.UserCreated;
                txtTaskDateModified.Text = pickingViewDTO.Entities[index].Task.DateModified.ToString();
                txtTaskUserModified.Text = pickingViewDTO.Entities[index].Task.UserModified;


                txtTaskId.Enabled = false;
                txtTaskWhsName.Enabled = false;
                txtTaskOwnName.Enabled = false;

                if (ddlTaskIsComplete.SelectedValue == "1")
                    ddlTaskIsComplete.Enabled = false;
                else
                    ddlTaskIsComplete.Enabled = true;

                txtTaskName.Enabled = false;
                txtTaskDescription.Enabled = true;
                txtTaskDocumentBound.Enabled = false;
                txtTaskPriority.Enabled = true;
                txtTaskRealStartDate.Enabled = false;

                if (pickingViewDTO.Entities[index].Task.IdTrackTaskType > 20)
                    ddlTaskStatus.Enabled = false;
                else
                    if (pickingViewDTO.Entities[index].Task.Status == false)
                        ddlTaskStatus.Enabled = false;
                    else
                        ddlTaskStatus.Enabled = true;
                if (ddlTaskTrackTaskType.SelectedValue == "101")
                    ddlTaskTrackTaskType.Enabled = false;
                else
                    ddlTaskTrackTaskType.Enabled = true;

                txtTaskDateTrackTask.Enabled = false;
                ddlTaskLocStageSource.Enabled = true;
                ddlTaskLocStageSource.Enabled = true;
                txtTaskWorkersRequired.Enabled = true;
                ddlTaskAllowCrossDock.Enabled = true;
                txtTaskDateCreated.Enabled = false;
                txtTaskUserCreated.Enabled = false;
                txtTaskDateModified.Enabled = false;
                txtTaskUserModified.Enabled = false;

                lblEditTask.Visible = true;

                divEditTask.Visible = true;
                currentIndex = index;
                modalPopUp.Show();
            }
            
        }

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges(currentIndex);
            }
            catch (Exception ex)
            {
                pickingViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
            }
        }


        /// <summary>
        /// Persiste los cambios en la entidad (modo Edit o New). 
        /// </summary>
        protected void SaveChanges(int index)
        {
            MiscUtils util = new MiscUtils();
            Task task = new Task();
            task.StageSource = new Location();
            task.StageTarget = new Location();

            //agrega los datos de la tarea
            task.Id = Convert.ToInt32(hidEditId.Value);
            task.IsComplete = Convert.ToBoolean(Convert.ToInt16(ddlTaskIsComplete.SelectedValue));
            task.Description = txtTaskDescription.Text;
            task.Priority = Convert.ToInt16(txtTaskPriority.Text);
            task.Status = Convert.ToBoolean(Convert.ToInt16(ddlTaskStatus.SelectedValue));
            task.IdTrackTaskType = Convert.ToInt32(ddlTaskTrackTaskType.SelectedValue);
            if (ddlTaskLocStageSource.SelectedValue != "-1")
                task.StageSource.IdCode = ddlTaskLocStageSource.SelectedValue;
            if (ddlTaskLocStageTarget.SelectedValue != "-1")
                task.StageTarget.IdCode = ddlTaskLocStageTarget.SelectedValue;
            task.WorkersRequired = Convert.ToInt16(txtTaskWorkersRequired.Text);
            task.AllowCrossDock = Convert.ToBoolean(Convert.ToInt16(ddlTaskAllowCrossDock.SelectedValue));

            // Editar Tarea
            pickingViewDTO = iTasksMGR.UpdateTaskMgr(task, pickingViewDTO.Entities[index], context);
            //Session.Remove(WMSTekSessions.Shared.TaskTypeList);

            divEditTask.Visible = false;
            modalPopUp.Hide();

            if (pickingViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(pickingViewDTO.MessageStatus.Message);

                UpdateSession(false);
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
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de Tareas
            this.Master.ucMainFilter.listTaskTypeCode = new System.Collections.Generic.List<string>();
            this.Master.ucMainFilter.listTaskTypeCode = GetConst("PickingTaskConsult");
                                   

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.chkDateFromVisible = false;
            this.Master.ucMainFilter.chkDateToVisible = false; 
            this.Master.ucMainFilter.taskTypeVisible = true;

            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            //Tab Reception
            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionReferenceNbr = true;

            //Tab Task
            this.Master.ucMainFilter.tabTaskVisible = true;
            this.Master.ucMainFilter.showDetailVisible = true;

            // Configura textos a mostrar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.TaskDaysBeforeQuery;//dias antes
            this.Master.ucMainFilter.DateAfter = CfgParameterName.TaskDaysAfterQuery;//dias despues

            this.Master.ucMainFilter.tabDocumentVisible = true;
            this.Master.ucMainFilter.documentTypeVisible = true;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

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
        private void UpdateSession(bool showError)
        {

            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
                pickingViewDTO.ClearError();
            }

            AddFiltersControls();
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            
            if (this.Master.ucMainFilter.showDetailCheck == false)
            {
                // carga las tareas
                pickingViewDTO = iTasksMGR.FindAllPickingTask(context);
            }
            else
            {
                // carga las tarea Detalle
                pickingViewDTO = iTasksMGR.FindAllPickingDetailTask(context);
            }

            if (!pickingViewDTO.hasError() && pickingViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.PickingTaskMgr.PickingTaskList, pickingViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(pickingViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(pickingViewDTO.Errors);
            }


        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!pickingViewDTO.hasConfigurationError() && pickingViewDTO.Configuration != null && pickingViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, pickingViewDTO.Configuration);

            grdMgr.DataSource = pickingViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(pickingViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

        protected void ReloadData()
        {
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void AddFiltersControls()
        {

            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                // Limpia el objeto 'Main Filter'
                this.Master.ucMainFilter.ClearFilterObject();

                // Carga el objeto 'Main Filter' con los valores seleccionados
                this.Master.ucMainFilter.LoadControlValuesToFilterObject();

                // Salva los criterios seleccionados
                Session[WMSTekSessions.Global.MainFilter] = (object)this.Master.ucMainFilter.MainFilter;
            }

        }

        
        #endregion


    }
}
