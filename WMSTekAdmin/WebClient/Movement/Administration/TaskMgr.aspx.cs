using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using System.Data.OleDb;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Integration;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Profile;

namespace Binaria.WMSTek.WebClient.Movement.Consult
{
    public partial class TaskMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskConsult> taskConsultViewDTO = new GenericViewDTO<TaskConsult>();
        private GenericViewDTO<Task> taskViewDTO = new GenericViewDTO<Task>();
        private MiscUtils util;
        private bool isValidViewDTO = false;
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

                    if (ValidateSession(WMSTekSessions.Shared.TaskTypeList))
                    {
                        taskConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.Shared.TaskTypeList];
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

                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnSaveTaskDetail_Click(object sender, EventArgs e)
        {
            try
            {
                var idTaskDetailBeingUsed = int.Parse(txtTaskDetailId.Text.Trim());
                var taskDetailBeingUsed = taskConsultViewDTO.Entities.Where(tc => tc.TaskDetail.Id == idTaskDetailBeingUsed).FirstOrDefault();

                if (!TypeTaskAllowedToCompleteWithoutValidation().Contains(taskDetailBeingUsed.Task.TypeCode))
                {
                    if (TaskIsInProcess(int.Parse(txtTaskDetailIdTask.Text.Trim())))
                    {
                        if (TaskDetailIsClose(int.Parse(txtTaskDetailId.Text.Trim())))
                        {
                            modalPopUpDetail.Hide();
                            this.Master.ucDialog.ShowAlert("Advertencia", lblWarningEditTaskDetailFromActiveClose.Text, "");
                        }
                        else
                        {
                            modalPopUpDetail.Hide();
                            this.Master.ucDialog.ShowConfirm("Advertencia", lblWarningEditTaskFromActiveProcess.Text, "");
                        }
                    }
                    else
                    {
                        if (TaskIsClose(int.Parse(txtTaskDetailIdTask.Text.Trim())))
                        {
                            modalPopUpDetail.Hide();
                            this.Master.ucDialog.ShowAlert("Advertencia", lblWarningEditTaskFromActiveClose.Text, "");
                        }
                        else
                        {
                            grdMgr.Enabled = true;
                            SaveChangesDetail(currentIndex);
                            modalPopUpDetail.Hide();
                        }
                    }
                }
                else
                {
                    grdMgr.Enabled = true;
                    SaveChangesDetail(currentIndex);
                    modalPopUpDetail.Hide();
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
        //        taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
        //    }
        //}

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var btnCreateExcelForPickVoicing = e.Row.FindControl("btnCreateExcelForPickVoicing") as ImageButton;
                    var btnReceiveExcelForPickVoicing = e.Row.FindControl("btnReceiveExcelForPickVoicing") as ImageButton;

                    //var typePick = new List<string>() { "PIKOR", "PIKWV" };
                    var typePick = new List<string>() { "SORT" };

                    if (btnCreateExcelForPickVoicing != null && taskConsultViewDTO.Entities.Count > 0 && typePick.Contains(taskConsultViewDTO.Entities[e.Row.DataItemIndex].Task.TypeCode)
                        && taskConsultViewDTO.Entities[e.Row.DataItemIndex].Task.IsComplete == false)
                    {
                        btnCreateExcelForPickVoicing.Visible = true;
                        btnReceiveExcelForPickVoicing.Visible = true;
                    }
                    else
                    {
                        btnCreateExcelForPickVoicing.Visible = false;
                        btnReceiveExcelForPickVoicing.Visible = false;
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "TaskActions"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    }
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //try
            //{
            //    // Calcula la posicion en el ViewDTO de la fila a editar
            //    int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

            //    ShowModal(editIndex, CRUD.Update);
            //}
            //catch (Exception ex)
            //{
            //    taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
            //    this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            //}
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Label lblTaskId = e.Row.FindControl("lblTaskId") as Label;
                ImageButton imgEdit = e.Row.FindControl("btnEditTask") as ImageButton;

                base.grdMgr_RowDataBound(sender, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var btnCreateExcelForPickVoicing = e.Row.FindControl("btnCreateExcelForPickVoicing") as ImageButton;

                    if (btnCreateExcelForPickVoicing != null)
                        ScriptManager.GetCurrent(this).RegisterPostBackControl(btnCreateExcelForPickVoicing);

                    if (taskConsultViewDTO.Entities.Count >= 0 && e.Row.RowIndex >= 0)
                    {
                        int index = e.Row.RowIndex + (grdMgr.PageSize * grdMgr.PageIndex);

                        if (taskConsultViewDTO.Entities[index].Id != idAux)
                        {
                            idAux = taskConsultViewDTO.Entities[index].Id;
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
                {
                    idAux = 0;
                }

            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                //int index = ((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex;
                int index = grdMgr.PageSize * grdMgr.PageIndex + ((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex;

                if (e.CommandName == "CreateExcelForPickVoicing")
                {
                    CreateExcelForPTL(index);
                    //CreateExcelForVoicing(index);
                }
                else if (e.CommandName == "ReceiveExcelForPickVoicing")
                {
                    OpenPopUpForReceiveExcelForPickVoicing(index);
                }
                else
                {
                    ShowModal(index, e.CommandName);
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnAcceptWarningEditTaskDetail_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChangesDetail(currentIndex);
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }  
        }

        #endregion

        #region "Eventos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "TaskMgr";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
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
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
                taskConsultViewDTO.ClearError();
            }

            AddFiltersControls();
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            int notCompleted = 0;
            int completed = 1;

            var filterCompleted = new List<int>() { notCompleted };
            var chkTaskComplete = (CheckBox)this.Master.ucMainFilter.FindControl("chkTaskComplete");

            if (chkTaskComplete != null && chkTaskComplete.Checked)
                filterCompleted.Add(completed);

            ClearFilter("Completed");
            CreateFilterByList("Completed", filterCompleted);

            taskConsultViewDTO = iTasksMGR.FindAllTaskMgr(context);

            if (!taskConsultViewDTO.hasError() && taskConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.TaskTypeList, taskConsultViewDTO);
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

        // Configuracion inicial del filtro de busqueda
        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de Tareas
            this.Master.ucMainFilter.listTaskTypeCode = new System.Collections.Generic.List<string>();
            this.Master.ucMainFilter.listTaskTypeCode = GetConst("TypeTaskCodes");

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;

            this.Master.ucMainFilter.dateVisible = true;
            //this.Master.ucMainFilter.dateToVisible = false;
            //this.Master.ucMainFilter.chkFilterDateToChecked = false;
            this.Master.ucMainFilter.trackTaskTypeCode = lstTrackTaskType();
            this.Master.ucMainFilter.taskTypeVisible = true;
            this.Master.ucMainFilter.taskTypeNotIncludeAll = false;

            this.Master.ucMainFilter.trackTaskTypeVisible = true;
            this.Master.ucMainFilter.trackTaskTypeNotIncludeAll = false;

            Master.ucMainFilter.codeNumericVisible = true;
            Master.ucMainFilter.codeNumericLabel = lblBatchNbr.Text;

            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            //Tab Reception
            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionSourceLpn = true;
            this.Master.ucMainFilter.tabReceptionTargetLpn = false;
            this.Master.ucMainFilter.tabReceptionReferenceNbr = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.setDateLabel = true;
            this.Master.ucMainFilter.dateLabel = lblFilterName.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.TaskDaysAfterQuery;//hoy

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.divTaskCompleteVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucDialog.BtnOkClick += new EventHandler(btnAcceptWarningEditTaskDetail_Click);
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
            //grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            //grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!taskConsultViewDTO.hasConfigurationError() && taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskConsultViewDTO.Configuration);

            grdMgr.DataSource = taskConsultViewDTO.Entities;
            grdMgr.DataBind();
            upGrid.Update();

            ucStatus.ShowRecordInfo(taskConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, string Action)
        {
            GenericViewDTO<Location> locStageSourceViewDTO = null;
            GenericViewDTO<Location> locStageTargetViewDTO = null;
            int idTaskDetail = 0;
            int idTask = 0;

            // Configura ventana modal
            if (taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
            {
                base.ConfigureModal(taskConsultViewDTO.Configuration, false);
            }

            //Rescata nuevamente la tarea seleccionada
            GenericViewDTO<TaskConsult> taskEit = iTasksMGR.GetByIdTaskMgr(taskConsultViewDTO.Entities[index].Id, context);

            if(Action == "EditTaskDetail")
            {
                idTask = taskConsultViewDTO.Entities[index].Id;
                idTaskDetail = taskConsultViewDTO.Entities[index].TaskDetail.Id;
            }

            index = 0;

            if (Action == "EditTask")
            {
                // Editar Tarea

                //Recupera los datos de la entidad a editar
                hidEditId.Value = taskEit.Entities[index].Id.ToString();

                //el dato IsBaseUser es de solo lectura
                //hidIsBaseUser.Value = taskEit.Entities[index].IsBaseUser.ToString();

                //Carga controles
                //LoadTrackTaskType(ddlTaskTrackTaskType, false, lblEmptyRow.Text);
                LoadTrackTaskTypeFilter(ddlTaskTrackTaskType, false, lblEmptyRow.Text, lstTrackTaskType());

                //LoadStagingLocations(ddlTaskLocStageSource, taskEit.Entities[index].Task.Warehouse.Id, lblEmptyRow.Text);
                //LoadStagingLocations(ddlTaskLocStageTarget, taskEit.Entities[index].Task.Warehouse.Id, lblEmptyRow.Text);
                switch (taskEit.Entities[index].Task.TypeCode.Trim())
                {
                    case "REPL":
                        if (taskEit.Entities[index].IdLocStageSource != null)
                            locStageSourceViewDTO = iLayoutMGR.GetLocationByIdAndWhs(taskEit.Entities[index].IdLocStageSource, taskEit.Entities[index].Task.Warehouse.Id, context);

                        if (taskEit.Entities[index].IdLocStageTarget != null)
                            locStageTargetViewDTO = iLayoutMGR.GetLocationByIdAndWhs(taskEit.Entities[index].IdLocStageTarget, taskEit.Entities[index].Task.Warehouse.Id, context);

                        if (taskEit.Entities[index].IdLocStageSource != null
                            && locStageSourceViewDTO != null && locStageSourceViewDTO.Entities.Count > 0)
                        {
                            ddlTaskLocStageSource.Items.Clear();
                            ddlTaskLocStageSource.Items.Add(new ListItem(locStageSourceViewDTO.Entities[0].Code, locStageSourceViewDTO.Entities[0].IdCode));
                            ddlTaskLocStageSource.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));
                            ddlTaskLocStageSource.Items[1].Selected = true;
                        }

                        if (taskEit.Entities[index].IdLocStageTarget != null
                            && locStageTargetViewDTO != null && locStageTargetViewDTO.Entities.Count > 0)
                        {
                            ddlTaskLocStageTarget.Items.Clear();
                            ddlTaskLocStageTarget.Items.Add(new ListItem(locStageTargetViewDTO.Entities[0].Code, locStageTargetViewDTO.Entities[0].IdCode));
                            ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));
                            ddlTaskLocStageTarget.Items[1].Selected = true;
                        }
                        break;

                    case "CCNT":

                        ddlTaskLocStageSource.Items.Clear();
                        ddlTaskLocStageSource.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));
                        //ddlTaskLocStageSource.Items[0].Selected = true;

                        ddlTaskLocStageTarget.Items.Clear();
                        ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageTarget))
                        {
                            ddlTaskLocStageTarget.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageTarget, taskEit.Entities[index].IdLocStageTarget));
                            ddlTaskLocStageTarget.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageTarget.Items[0].Selected = true;
                        }

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageSource))
                        {
                            ddlTaskLocStageSource.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageSource, taskEit.Entities[index].IdLocStageSource));
                            ddlTaskLocStageSource.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageSource.Items[0].Selected = true;
                        }

                        break;
                    case "CCLOC":

                        ddlTaskLocStageSource.Items.Clear();
                        ddlTaskLocStageSource.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        ddlTaskLocStageTarget.Items.Clear();
                        ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageTarget))
                        {
                            ddlTaskLocStageTarget.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageTarget, taskEit.Entities[index].IdLocStageTarget));
                            ddlTaskLocStageTarget.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageTarget.Items[0].Selected = true;
                        }

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageSource))
                        {
                            ddlTaskLocStageSource.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageSource, taskEit.Entities[index].IdLocStageSource));
                            ddlTaskLocStageSource.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageSource.Items[0].Selected = true;
                        }

                        break;
                    case "RECO":

                        ddlTaskLocStageSource.Items.Clear();
                        ddlTaskLocStageSource.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));
                        //ddlTaskLocStageSource.Items[0].Selected = true;

                        ddlTaskLocStageTarget.Items.Clear();
                        ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageTarget))
                        {
                            ddlTaskLocStageTarget.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageTarget, taskEit.Entities[index].IdLocStageTarget));
                            ddlTaskLocStageTarget.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageTarget.Items[0].Selected = true;
                        }

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageSource))
                        {
                            ddlTaskLocStageSource.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageSource, taskEit.Entities[index].IdLocStageSource));
                            ddlTaskLocStageSource.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageSource.Items[0].Selected = true;
                        }

                        break;
                    case "AJU":

                        ddlTaskLocStageSource.Items.Clear();
                        ddlTaskLocStageSource.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        ddlTaskLocStageTarget.Items.Clear();
                        ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageTarget))
                        {
                            ddlTaskLocStageTarget.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageTarget, taskEit.Entities[index].IdLocStageTarget));
                            ddlTaskLocStageTarget.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageTarget.Items[0].Selected = true;
                        }

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageSource))
                        {
                            ddlTaskLocStageSource.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageSource, taskEit.Entities[index].IdLocStageSource));
                            ddlTaskLocStageSource.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageSource.Items[0].Selected = true;
                        }

                        break;
                    case "CCPC":

                        ddlTaskLocStageSource.Items.Clear();
                        ddlTaskLocStageSource.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));
                        ddlTaskLocStageSource.Items[0].Selected = true;

                        ddlTaskLocStageTarget.Items.Clear();
                        ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

                        if (!string.IsNullOrEmpty(taskEit.Entities[index].IdLocStageTarget))
                        {
                            ddlTaskLocStageTarget.Items.Add(new ListItem(taskEit.Entities[index].IdLocStageTarget, taskEit.Entities[index].IdLocStageTarget));
                            ddlTaskLocStageTarget.Items[1].Selected = true;
                        }
                        else
                        {
                            ddlTaskLocStageTarget.Items[0].Selected = true;
                        }

                        break;
                    default:
                        LoadLocationsByWhsAndListType(ddlTaskLocStageSource, taskEit.Entities[index].Task.Warehouse.Id,
                                                 GetConst("LocationTypeStagingForTask"), this.lblEmptyRow.Text, true);
                        LoadLocationsByWhsAndListType(ddlTaskLocStageTarget, taskEit.Entities[index].Task.Warehouse.Id,
                                                      GetConst("LocationTypeStagingForTask"), this.lblEmptyRow.Text, true);
                        FillDdlLocStageTarget(taskEit.Entities[index]);
                        break;
                }
                
                LoadConst(ddlTaskIsComplete, "Condition", false, lblEmptyRow.Text);
                LoadConst(ddlTaskAllowCrossDock, "Condition", true, lblEmptyRow.Text);
                LoadConst(ddlTaskStatus, "Status", false, lblEmptyRow.Text);

                txtTaskId.Text = taskEit.Entities[index].Task.Id.ToString();
                txtTaskWhsName.Text = taskEit.Entities[index].Task.Warehouse.Name;
                txtTaskOwnName.Text = taskEit.Entities[index].Task.Owner.Name;
                ddlTaskIsComplete.SelectedValue = Convert.ToInt32(taskEit.Entities[index].Task.IsComplete).ToString();
                txtTaskName.Text = taskEit.Entities[index].TaskTypeName;
                txtTaskDescription.Text = taskEit.Entities[index].Task.Description;
                txtTaskDocumentBound.Text = taskEit.Entities[index].OutboundOrder.Number;
                txtTaskPriority.Text = taskEit.Entities[index].Task.Priority.ToString();
                txtTaskRealStartDate.Text = taskEit.Entities[index].Task.RealStartDate.ToString();
                ddlTaskStatus.SelectedValue = Convert.ToInt32(taskEit.Entities[index].Task.Status).ToString();

                if (taskEit.Entities[index].Task.IdTrackTaskType != null)
                    ddlTaskTrackTaskType.SelectedValue = taskEit.Entities[index].Task.IdTrackTaskType.ToString();

                if (taskEit.Entities[index].IdLocStageSource != null)
                    ddlTaskLocStageSource.SelectedValue = taskEit.Entities[index].IdLocStageSource;

                if (taskEit.Entities[index].IdLocStageTarget != null)
                    ddlTaskLocStageTarget.SelectedValue = taskEit.Entities[index].IdLocStageTarget;

                if (taskEit.Entities[index].Task.AllowCrossDock != null)
                    ddlTaskAllowCrossDock.SelectedValue = Convert.ToInt32(taskEit.Entities[index].Task.AllowCrossDock).ToString();

                txtTaskDateTrackTask.Text = taskEit.Entities[index].Task.DateTrackTask.ToString();
                txtTaskWorkersRequired.Text = taskEit.Entities[index].Task.WorkersRequired.ToString();
                txtTaskDateCreated.Text = taskEit.Entities[index].Task.DateCreated.ToString();
                txtTaskUserCreated.Text = taskEit.Entities[index].Task.UserCreated;
                txtTaskDateModified.Text = taskEit.Entities[index].Task.DateModified.ToString();
                txtTaskUserModified.Text = taskEit.Entities[index].Task.UserModified;
                txtTaskId.Enabled = false;
                txtTaskWhsName.Enabled = false;
                txtTaskOwnName.Enabled = false;

                //if (ddlTaskIsComplete.SelectedValue == "1")
                //    ddlTaskIsComplete.Enabled = false;
                //else
                    ddlTaskIsComplete.Enabled = true;

                txtTaskName.Enabled = false;
                txtTaskDescription.Enabled = true;
                txtTaskDocumentBound.Enabled = false;
                txtTaskPriority.Enabled = true;
                txtTaskRealStartDate.Enabled = false;

                if (taskEit.Entities[index].Task.IdTrackTaskType > 20)
                    ddlTaskStatus.Enabled = false;
                else
                    if (taskEit.Entities[index].Task.Status == false)
                    ddlTaskStatus.Enabled = false;
                else
                    ddlTaskStatus.Enabled = true;
                if (ddlTaskTrackTaskType.SelectedValue == "101")
                    ddlTaskTrackTaskType.Enabled = false;
                else
                    ddlTaskTrackTaskType.Enabled = true;

                //if (taskEit.Entities[index].Task.TypeCode == "REPL")
                //{
                //    ddlTaskLocStageSource.Enabled = false;
                //    ddlTaskLocStageTarget.Enabled = false;
                //}else{
                ddlTaskLocStageSource.Enabled = false;
                ddlTaskLocStageTarget.Enabled = false;
                //}

                txtTaskDateTrackTask.Enabled = false;
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
            else
            {
                //hidEditId.Value = taskEit.Entities[index].Id.ToString();
                //hidEditIdDetail.Value = taskEit.Entities[index].TaskDetail.Id.ToString();
                hidEditId.Value = idTask.ToString();
                hidEditIdDetail.Value = idTaskDetail.ToString();

                TaskConsult taskEdit = taskEit.Entities.First(w => w.TaskDetail.Id.Equals(idTaskDetail));

                txtTaskDetailId.Text = taskEdit.TaskDetail.Id.ToString();
                txtTaskDetailIdTask.Text = taskEdit.Id.ToString();

                LoadConst(ddlTaskDetailIsComplete, "Condition", false, lblEmptyRow.Text);
                ddlTaskDetailIsComplete.SelectedValue = Convert.ToInt32(taskEdit.TaskDetail.IsComplete).ToString();

                txtTaskDetailDocumentBound.Text = taskEdit.OutboundOrder.Number;
                txtTaskDetailIdItem.Text = taskEdit.TaskDetail.Item.Code;
                txtTaskDetailLongItemName.Text = taskEdit.TaskDetail.Item.LongName;
                txtTaskDetailIdCtgItem.Text = taskEdit.TaskDetail.CategoryItem.Name;
                txtTaskDetailPriority.Text = taskEdit.TaskDetail.Priority.ToString();
                txtTaskDetailLocSourceProposal.Text = taskEdit.TaskDetail.IdLocSourceProposal;

                ddlTaskDetailLocForkliftProposal.Items.Clear();
                LoadLocationsByWhsAndType(ddlTaskDetailLocForkliftProposal, taskEdit.Task.Warehouse.Id, "FKL", lblEmptyRow.Text, true);

                if (taskEdit.TaskDetail.IdLocForkLiftProposal != null)
                    ddlTaskDetailLocForkliftProposal.SelectedValue = taskEdit.TaskDetail.IdLocForkLiftProposal;

                txtTaskDetailLocTargetProposal.Text = taskEdit.TaskDetail.IdLocTargetProposal;

                LoadConst(ddlTaskDetailStatus, "Status", false, lblEmptyRow.Text);
                ddlTaskDetailStatus.SelectedValue = Convert.ToInt32(taskEdit.TaskDetail.Status).ToString();
                txtTaskDetailProposalQty.Text = taskEdit.TaskDetail.ProposalQty.ToString();

                LoadUsersOperatorWihtOutDefaultWhs(ddlTaskDetailUserAssigned, true, lblEmptyRow.Text);
                ddlTaskDetailUserAssigned.SelectedValue = taskEdit.TaskDetail.UserAssigned;

                txtTaskDetailStartDate.Text = taskEdit.TaskDetail.StartDate.ToString();
                txtTaskDetailMadeCrossDock.Text = taskEdit.TaskDetail.MadeCrossDock.ToString();

                txtTaskDetailDateCreated.Text = taskEdit.TaskDetail.DateCreated.ToString();
                txtTaskDetailUserCreated.Text = taskEdit.TaskDetail.UserCreated;
                txtTaskDetailDateModified.Text = taskEdit.TaskDetail.DateModified.ToString();
                txtTaskDetailUserModified.Text = taskEdit.TaskDetail.UserModified;
                
                txtTaskDetailId.Enabled = false;
                txtTaskDetailIdTask.Enabled = false;

                if (TypeTaskAllowedToCompleteWithoutValidation().Contains(taskEdit.Task.TypeCode))
                {
                    ddlTaskDetailIsComplete.Enabled = true;
                }
                else
                {
                    if (taskEdit.TaskDetail.IsComplete)
                        ddlTaskDetailIsComplete.Enabled = false;
                    else
                        ddlTaskDetailIsComplete.Enabled = true;
                }

                txtTaskDetailDocumentBound.Enabled = false;
                txtTaskDetailIdItem.Enabled = false;
                txtTaskDetailLongItemName.Enabled = false;
                txtTaskDetailIdCtgItem.Enabled = false;
                txtTaskDetailPriority.Enabled = true;
                txtTaskDetailLocSourceProposal.Enabled = false;
                ddlTaskDetailLocForkliftProposal.Enabled = true;
                txtTaskDetailLocTargetProposal.Enabled = false;
                txtTaskDetailProposalQty.Enabled = false;

                //if (taskEdit.TaskDetail.IsComplete)
                //    ddlTaskDetailStatus.Enabled = false;
                //else
                    ddlTaskDetailStatus.Enabled = true;

                if (taskEdit.TaskDetail.IsComplete)
                    ddlTaskDetailUserAssigned.Enabled = false;
                else
                    ddlTaskDetailUserAssigned.Enabled = true;

                txtTaskDetailStartDate.Enabled = false;
                txtTaskDetailMadeCrossDock.Enabled = false;

                txtTaskDetailDateCreated.Enabled = false;
                txtTaskDetailUserCreated.Enabled = false;
                txtTaskDetailDateModified.Enabled = false;
                txtTaskDetailUserModified.Enabled = false;

                lblEditTaskDetail.Visible = true;
                divEditTaskDetail.Visible = true;
                currentIndex = index;
                modalPopUpDetail.Show();
            }
        }

        private void FillDdlLocStageTarget(TaskConsult task)
        {
            if (task == null)
                return;

            ddlTaskLocStageTarget.Items.Clear();
            ddlTaskLocStageTarget.Items.Insert(0, new ListItem(this.lblEmptyRow.Text, "-1"));

            if (!string.IsNullOrEmpty(task.IdLocStageTarget))
            {
                ddlTaskLocStageTarget.Items.Add(new ListItem(task.IdLocStageTarget, task.IdLocStageTarget));
                ddlTaskLocStageTarget.Items[1].Selected = true;
            }
            else
            {
                ddlTaskLocStageTarget.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Persiste los cambios en la entidad (modo Edit o New). 
        /// </summary>
        protected void SaveChanges(int index)
        {
            util = new MiscUtils();
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

            if(ddlTaskLocStageSource.SelectedValue != "-1")
                task.StageSource.IdCode = ddlTaskLocStageSource.SelectedValue;
            if (ddlTaskLocStageTarget.SelectedValue != "-1")
                task.StageTarget.IdCode = ddlTaskLocStageTarget.SelectedValue;

            task.WorkersRequired = Convert.ToInt16(txtTaskWorkersRequired.Text);
            task.AllowCrossDock = Convert.ToBoolean(Convert.ToInt16(ddlTaskAllowCrossDock.SelectedValue));
            
            // Editar Tarea
            taskConsultViewDTO = iTasksMGR.UpdateTaskMgr(task, taskConsultViewDTO.Entities[index], context);
            Session.Remove(WMSTekSessions.Shared.TaskTypeList);

            divEditTask.Visible = false;
            modalPopUp.Hide();

            if (taskConsultViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(taskConsultViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void SaveChangesDetail(int index)
        {
            util = new MiscUtils();
            TaskDetail taskDetail = new TaskDetail();
            
            //agrega los datos de la tarea detalle
            taskDetail.Id = Convert.ToInt32(hidEditIdDetail.Value);
            var taskConsultSelected = taskConsultViewDTO.Entities.Where(tc => tc.TaskDetail.Id == taskDetail.Id).FirstOrDefault();
            taskDetail.IsComplete = Convert.ToBoolean(Convert.ToInt16(ddlTaskDetailIsComplete.SelectedValue));
            taskDetail.Priority = Convert.ToInt16(txtTaskDetailPriority.Text);
            taskDetail.Status = Convert.ToBoolean(Convert.ToInt16(ddlTaskDetailStatus.SelectedValue));
            taskDetail.Task = taskConsultSelected.Task;
            taskDetail.Item = taskConsultSelected.TaskDetail.Item;

            if (ddlTaskDetailLocForkliftProposal.SelectedValue != "-1")
                taskDetail.IdLocForkLiftProposal = ddlTaskDetailLocForkliftProposal.SelectedValue;

            if (ddlTaskDetailUserAssigned.SelectedValue != "-1")
                taskDetail.UserAssigned = ddlTaskDetailUserAssigned.SelectedValue;

            var userUsingTaskDetail = UserUsingTaskDetail(taskDetail);
            if (!string.IsNullOrEmpty(userUsingTaskDetail))
            {
                ucStatus.ShowWarning(lblValidateChangeComplete.Text.Replace("[USER]", userUsingTaskDetail));
                return;
            }

            // Editar Tarea Detalle
            taskConsultViewDTO = iTasksMGR.UpdateTaskDetailMgr(taskDetail, taskConsultViewDTO.Entities[index], context);
            Session.Remove(WMSTekSessions.Shared.TaskTypeList);


            divEditTaskDetail.Visible = false;
           

            if (taskConsultViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(taskConsultViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        private string UserUsingTaskDetail(TaskDetail taskDetailToUpdate)
        {
            var userUsingTaskDetail = string.Empty;

            var getTaskDetailViewDTO = iTasksMGR.GetTaskDetailByAnyParameter(context, new TaskDetail() { Id = taskDetailToUpdate.Id }, (string)null);

            if (!getTaskDetailViewDTO.hasError() && getTaskDetailViewDTO.Entities.Count > 0)
            {
                var taskDetailBeforeUpdate = getTaskDetailViewDTO.Entities.First();

                ContextViewDTO newContext = new ContextViewDTO();
                newContext.MainFilter = new List<EntityFilter>(this.Master.ucMainFilter.MainFilter);

                foreach (EntityFilter entityFilter in newContext.MainFilter)
                {
                    entityFilter.FilterValues.Clear();
                }

                newContext.PathClassRemoting = context.PathClassRemoting;

                var monitorViewDTO = iDeviceMGR.FindAllTerminalMonitor(newContext);

                if (!monitorViewDTO.hasError() && monitorViewDTO.Entities.Count > 0)
                {
                    var taskDetailBeingUsed = monitorViewDTO.Entities.Where(m => m.IdTaskDetail == taskDetailToUpdate.Id.ToString()).ToList();

                    if (taskDetailBeingUsed.Count > 0)
                        userUsingTaskDetail = taskDetailBeingUsed.First().RfOperatorUserName;
                }
            }

            return userUsingTaskDetail;
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

        private bool TaskIsInProcess(int idTask)
        {
            taskViewDTO = iTasksMGR.GetById(idTask, context);

            if (taskViewDTO.hasError())
            {
                throw new Exception(taskConsultViewDTO.Errors.Message);
            }
            else
            {
                if (taskViewDTO.Entities.Count > 0)
                {
                    var task = taskViewDTO.Entities.FirstOrDefault();
                    if (task.IdTrackTaskType == (int)TrackTaskTypeName.EnProceso)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("No se encontro tarea padre");
                }
            }
        }

        private bool TaskDetailIsClose(int idTaskDetail)
        {
            TaskDetail taskDetail = new TaskDetail();
            GenericViewDTO<TaskDetail> taskdetailDTO = new GenericViewDTO<TaskDetail>();
            taskDetail.Id = idTaskDetail;     

            taskdetailDTO = iTasksMGR.GetTaskDetailByAnyParameter(context, taskDetail, "");

            if (taskdetailDTO.hasError())
            {
                throw new Exception(taskConsultViewDTO.Errors.Message);
            }
            else
            {
                if (taskdetailDTO.Entities.Count > 0)
                {
                    
                    if (taskdetailDTO.Entities[0].Status == false && taskdetailDTO.Entities[0].IsComplete == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("No se encontro tarea padre");
                }
            }
        }

        private bool TaskIsClose(int idTask)
        {
            taskViewDTO = iTasksMGR.GetById(idTask, context);

            if (taskViewDTO.hasError())
            {
                throw new Exception(taskConsultViewDTO.Errors.Message);
            }
            else
            {
                if (taskViewDTO.Entities.Count > 0)
                {
                    var task = taskViewDTO.Entities.FirstOrDefault();
                    if (task.IdTrackTaskType == (int)TrackTaskTypeName.Cerrada)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("No se encontro tarea padre");
                }
            }
        }


        private string[] lstTrackTaskType()
        {
            string[] lst = new string[] { ((int)TrackTaskTypeName.Retenida).ToString(),
                                        ((int)TrackTaskTypeName.Pausa).ToString(),
                                        ((int)TrackTaskTypeName.ColaPTL).ToString(),
                                         ((int)TrackTaskTypeName.EnPTL).ToString(),
                                        ((int)TrackTaskTypeName.SinStock).ToString(),
                                        ((int)TrackTaskTypeName.Liberada).ToString(),
                                        ((int)TrackTaskTypeName.EnProceso).ToString(),
                                        ((int)TrackTaskTypeName.ColaPTL).ToString(),
                                        ((int)TrackTaskTypeName.EnPTL).ToString(),
                                        ((int)TrackTaskTypeName.Cerrada).ToString(),
                                        ((int)TrackTaskTypeName.Anulada).ToString()};
            return lst;
        }

        private void CreateExcelForPTL(int index)
        {   
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Tareas Sort");

                worksheet.Cell(1, 1).Value = "NroOLA";
                worksheet.Cell(1, 2).Value = "IdWhs";
                worksheet.Cell(1, 3).Value = "IdOwn";
                worksheet.Cell(1, 4).Value = "IdTask";
                worksheet.Cell(1, 5).Value = "IdTaskDetail";
                worksheet.Cell(1, 6).Value = "IdDetailBound";
                worksheet.Cell(1, 7).Value = "IdLocSourceProposal";
                worksheet.Cell(1, 8).Value = "IdLocTargetProposal";
                worksheet.Cell(1, 9).Value = "IdItem";
                worksheet.Cell(1, 10).Value = "LineNumber";
                worksheet.Cell(1, 11).Value = "ProposalQty";
                worksheet.Cell(1, 12).Value = "IdDocumentBound";
                worksheet.Cell(1, 13).Value = "CustomerCode";
                worksheet.Cell(1, 14).Value = "CustomerName";
                worksheet.Cell(1, 15).Value = "OutBoundNumber";
                worksheet.Cell(1, 16).Value = "ReferenceNumber";
                worksheet.Cell(1, 17).Value = "DeliveryAdderss1";
                worksheet.Cell(1, 18).Value = "CityName";
                worksheet.Cell(1, 19).Value = "ItemCode";
                worksheet.Cell(1, 20).Value = "Description";
                worksheet.Cell(1, 21).Value = "BarCode";
                worksheet.Cell(1, 22).Value = "UomName";
                worksheet.Cell(1, 23).Value = "BigTicket";
                worksheet.Cell(1, 24).Value = "WhsName";
                worksheet.Cell(1, 25).Value = "OwnName";
                worksheet.Cell(1, 26).Value = "PrefixCodBarraBulto";
                worksheet.Cell(1, 27).Value = "PrefixCodBarraBigticket";
                worksheet.Cell(1, 28).Value = "CustomField1";
                worksheet.Cell(1, 29).Value = "CustomField2";
                worksheet.Cell(1, 30).Value = "CustomField3";
                worksheet.Cell(1, 31).Value = "CustomField4";


                var taskConsultSelected = taskConsultViewDTO.Entities[index];

                //var taskDetailViewDTO = iTasksMGR.GetTaskDetailByAnyParameter(context, new TaskDetail() { Task = new Task(taskConsultSelected.Task.Id) }, "");
                var taskDetailViewDTO = iTasksMGR.GetTaskDetailForPTLByIdTask(taskConsultSelected.Task.Id, context);

                if (!taskDetailViewDTO.hasError() && taskDetailViewDTO.Entities.Count > 0)
                {
                    int i = 2;
                    string nroOla = string.Empty;

                    foreach (var taskConsult in taskDetailViewDTO.Entities)
                    {
                        nroOla = taskConsult.Task.IdDocumentBound.ToString();

                        worksheet.Cell(i, 1).Value = taskConsult.Task.IdDocumentBound;
                        worksheet.Cell(i, 2).Value = taskConsultSelected.Task.Warehouse.Id;
                        worksheet.Cell(i, 3).Value = taskConsultSelected.Task.Owner.Id;
                        worksheet.Cell(i, 4).Value = taskConsult.Task.Id;
                        worksheet.Cell(i, 5).Value = taskConsult.TaskDetail.Id;
                        worksheet.Cell(i, 6).Value = taskConsult.TaskDetail.IdDetailBound;
                        worksheet.Cell(i, 7).Value = taskConsult.TaskDetail.IdLocSourceProposal;
                        worksheet.Cell(i, 8).Value = taskConsult.TaskDetail.IdLocTargetProposal;
                        worksheet.Cell(i, 9).Value = taskConsult.TaskDetail.Item.Id;
                        worksheet.Cell(i, 10).Value = taskConsult.TaskDetail.LineNumber;
                        worksheet.Cell(i, 11).Value = Convert.ToInt32(taskConsult.TaskDetail.ProposalQty);
                        worksheet.Cell(i, 12).Value = taskConsult.TaskDetail.IdDocumentBound;
                        worksheet.Cell(i, 13).Value = taskConsult.OutboundOrder.CustomerCode;
                        worksheet.Cell(i, 14).Value = taskConsult.OutboundOrder.CustomerName;
                        worksheet.Cell(i, 15).Value = taskConsult.OutboundOrder.Number;
                        worksheet.Cell(i, 16).Value = taskConsult.OutboundOrder.ReferenceNumber;
                        worksheet.Cell(i, 17).Value = taskConsult.OutboundOrder.DeliveryAddress1;
                        worksheet.Cell(i, 18).Value = taskConsult.OutboundOrder.CityDelivery.Name;
                        worksheet.Cell(i, 19).Value = taskConsult.TaskDetail.Item.Code;
                        worksheet.Cell(i, 20).Value = taskConsult.TaskDetail.Item.Description;
                        worksheet.Cell(i, 21).Value = Convert.ToString(taskConsult.TaskDetail.Item.ItemUom.BarCode);
                        worksheet.Cell(i, 22).Value = taskConsult.TaskDetail.Item.ItemUom.Name;
                        worksheet.Cell(i, 23).Value = taskConsult.TaskDetail.Item.ItemUom.SpecialField1;//De donde sacar BigTicket  Convert.ToString(itemUom.Entities[0].bi);
                        worksheet.Cell(i, 24).Value = taskConsultSelected.Task.Warehouse.Name;
                        worksheet.Cell(i, 25).Value = taskConsultSelected.Task.Owner.Name;
                        worksheet.Cell(i, 26).Value = taskConsult.OutboundOrder.SpecialField1;//PrefixCodBarraBulto;
                        worksheet.Cell(i, 27).Value = taskConsult.OutboundOrder.SpecialField2; //PrefixCodBarraBigticket ;
                        worksheet.Cell(i, 28).Value = taskConsult.OutboundOrder.SpecialField3;
                        worksheet.Cell(i, 29).Value = taskConsult.OutboundOrder.SpecialField4;
                        worksheet.Cell(i, 30).Value = null;
                        worksheet.Cell(i, 31).Value = null;

                        i++;
                    }

                    Response.Clear();

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = "Ola-" + nroOla + "-" + DateTime.Now.ToString("ddMMyy-hhmm");
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream, false);
                        memoryStream.WriteTo(Response.OutputStream);
                    }

                    Response.End();
                }            
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            } 
        }

        private void CreateExcelForVoicing(int index)
        {
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Tareas Pick");

                worksheet.Cell(1, 1).Value = "Número del Pedido";
                worksheet.Cell(1, 2).Value = "Fecha de Despacho";
                worksheet.Cell(1, 3).Value = "Muelle de Despacho";
                worksheet.Cell(1, 4).Value = "Ruta";
                worksheet.Cell(1, 5).Value = "Meta Tiempo";
                worksheet.Cell(1, 6).Value = "Secuencia de Viaje";
                worksheet.Cell(1, 7).Value = "Código de Artículo";
                worksheet.Cell(1, 8).Value = "Cantidad a Pickear";
                worksheet.Cell(1, 9).Value = "Unidad de Medida";
                worksheet.Cell(1, 10).Value = "Código de Ubicación";

                var taskConsultSelected = taskConsultViewDTO.Entities[index];

                var taskDetailViewDTO = iTasksMGR.GetTaskDetailByAnyParameter(context, new TaskDetail() { Task = new Task(taskConsultSelected.Task.Id) }, "");

                if (!taskDetailViewDTO.hasError() && taskDetailViewDTO.Entities.Count > 0)
                {
                    int i = 2;

                    foreach (var taskDetail in taskDetailViewDTO.Entities)
                    {
                        var locationViewDTO = iLayoutMGR.GetLocationByIdAndWhs(taskDetail.IdLocSourceProposal, taskConsultSelected.Task.Warehouse.Id, context);

                        int pickingFlow = 0;
                        if (!locationViewDTO.hasError() && locationViewDTO.Entities.Count > 0)
                        {
                            pickingFlow = locationViewDTO.Entities.First().PickingFlow;
                        }

                        worksheet.Cell(i, 1).Value = taskConsultSelected.OutboundOrder.Number;
                        worksheet.Cell(i, 2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                        worksheet.Cell(i, 3).Value = "5";
                        worksheet.Cell(i, 4).Value = 1;
                        worksheet.Cell(i, 5).Value = "30";
                        worksheet.Cell(i, 6).Value = pickingFlow;
                        worksheet.Cell(i, 7).Value = taskDetail.Item.Code;
                        worksheet.Cell(i, 8).Value = taskDetail.ProposalQty;
                        worksheet.Cell(i, 9).Value = "unidad";
                        worksheet.Cell(i, 10).Value = taskDetail.IdLocSourceProposal;

                        i++;
                    }

                    Response.Clear();

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = "Pedido-" + taskConsultSelected.OutboundOrder.Number + "-" + DateTime.Now.ToString("ddMMyy-hhmm");
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream, false);
                        memoryStream.WriteTo(Response.OutputStream);
                    }

                    Response.End();
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        private void OpenPopUpForReceiveExcelForPickVoicing(int index)
        {
            try
            {
                var taskConsultSelected = taskConsultViewDTO.Entities[index];
                hidIdTaskSelected.Value = taskConsultSelected.Task.Id.ToString();

                divLoadVoicePicking.Visible = true;
                modalPopUpLoadVoicePicking.Show();
                upLoadVoicePicking.Update();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnSubirVoicePicking_Click(object sender, EventArgs e)
        {
            try
            {
                ReceiveExcelForPickVoicing();
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        private void ReceiveExcelForPickVoicing()
        {
            string errorUp = "";
            string pathAux = "";
            var resultViewDTO = new GenericViewDTO<TaskDetailPtlResultIfz>();

            try
            {
                if (uploadFile.HasFile)
                {
                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Movement.Consult.TaskMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidDataException(ex.Message);
                    }

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                    "Data Source=" + savePath + ";" +
                                                    "Extended Properties=Excel 8.0;";

                    var dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", connectionString);
                    DataSet myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        dataAdapter.Fill(myDataSet);
                        dataTable = myDataSet.Tables["Table"];
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Movement.Consult.TaskMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    //var listDataFromPTL = from r in dataTable.AsEnumerable()
                    //                            select new
                    //                            {
                    //                                NroOLA = r.Field<object>("NroOla"),
                    //                                IdTaskDetail = r.Field<object>("IdTaskDetail"),
                    //                                IdDocumentBound = r.Field<object>("IdDocumentBound"),
                    //                                IdItem = r.Field<object>("IdItem"),
                    //                                IdLocSourceUsed = r.Field<object>("IdLocSourceUsed"),
                    //                                IdLocTargetUsed = r.Field<object>("IdLocTargetUsed"),
                    //                                IdLpnSourceUsed = r.Field<object>("IdLpnSourceUsed"),
                    //                                IdLpnTargetUsed = r.Field<object>("IdLpnTargetUsed"),
                    //                                RealQty = r.Field<object>("RealQty"),
                    //                                UserAssigned = r.Field<object>("UserAssigned"),
                    //                                RealStartDate = r.Field<object>("RealStartDate"),
                    //                                RealEndDate = r.Field<object>("RealEndDate")
                    //                            };

                    var listDataFromPTL = from r in dataTable.AsEnumerable()
                                          select new
                                          {
                                              NroOLA = r.Field<object>("NroOLA"),
                                              IdWhs = r.Field<object>("IdWhs"),
                                              IdOwn = r.Field<object>("IdOwn"),
                                              IdTask = r.Field<object>("IdTask"),
                                              IdTaskDetail = r.Field<object>("IdTaskDetail"),
                                              IdDocumentBound = r.Field<object>("IdDocumentBound"),
                                              IdDetailBound = r.Field<object>("IdDetailBound"),
                                              IdItem = r.Field<object>("IdItem"),
                                              RealQty = r.Field<object>("RealQty"),
                                              OutBoundNumber = r.Field<object>("OutBoundNumber"),
                                              ItemCode = r.Field<object>("ItemCode"),
                                              Description = r.Field<object>("Description"),
                                              IdLpnSourceUsed = r.Field<object>("IdLpnSourceUsed"),
                                              IdLpnCode = r.Field<object>("IdLpnCode"),
                                              BigTicketAsigned = r.Field<object>("BigTicketAsigned"),
                                              CustomField1 = r.Field<object>("CustomField1"),
                                              CustomField2 = r.Field<object>("CustomField2"),
                                              CustomField3 = r.Field<object>("CustomField3"),
                                              CustomField4 = r.Field<object>("CustomField4")
                                          };

                    var excelData = listDataFromPTL.ToList();

                    try
                    {
                        var list = new List<TaskDetailPtlResultIfz>();
                        string messageError = string.Empty;
                        int idTaskSelected = int.Parse(hidIdTaskSelected.Value);
                        var taskSort = iTasksMGR.GetById(idTaskSelected, context).Entities[0];
                        var nroOla = taskSort.IdDocumentBound;

                        foreach (var data in excelData)
                        {
                            if (int.Parse((data.NroOLA == null ? "-1" : data.NroOLA.ToString())) == nroOla)
                            {
                                var newData = new TaskDetailPtlResultIfz();
                                newData.NroOla = int.Parse(data.NroOLA.ToString());
                                //newData.IdWhs = int.Parse(data.IdWhs.ToString());
                                //newData.IdOwn = int.Parse(data.IdOwn.ToString());
                                //newData.IdTask = int.Parse(data.IdTask.ToString());


                                if (data.IdTaskDetail == null)
                                {
                                    messageError = "Campo IdTaskDetail no puede ser NULL";
                                    break;
                                }
                                else
                                    newData.IdTaskDetail = int.Parse(data.IdTaskDetail.ToString());

                                //newData.IdDocumentBound = int.Parse(data.IdDocumentBound.ToString());

                                if (data.IdItem != null)
                                {
                                    newData.IdItem = int.Parse(data.IdItem.ToString());
                                }

                                newData.IdLpnSourceUsed = data.IdLpnSourceUsed.ToString();


                                if (data.IdLpnSourceUsed == null)
                                {
                                    messageError = "Campo IdLpnTargetUsed no puede ser NULL";
                                    break;
                                }
                                else
                                    newData.IdLpnTargetUsed = data.IdLpnCode.ToString();

                                if (data.RealQty == null)
                                {
                                    messageError = "Campo RealQty no puede ser NULL";
                                    break;
                                }
                                else
                                    newData.RealQty = decimal.Parse(data.RealQty.ToString());

                                //newData.UserAssigned = data.UserAssigned.ToString();
                                newData.RealStartDate = DateTime.Now;
                                newData.IdLocSourceUsed = taskSort.StageSource.IdCode;
                                newData.IdLocTargetUsed = taskSort.StageSource.IdCode;

                                //Codigo antiguo
                                //var newData = new TaskDetailPtlResultIfz();
                                //newData.NroOla = int.Parse(data.NroOLA.ToString());
                                //newData.IdTaskDetail = int.Parse(data.IdTaskDetail.ToString());
                                //newData.IdDocumentBound = int.Parse(data.IdDocumentBound.ToString());
                                //newData.IdItem = int.Parse(data.IdItem.ToString());
                                //newData.IdLocSourceUsed = data.IdLocSourceUsed.ToString();
                                //newData.IdLocTargetUsed = data.IdLocTargetUsed.ToString();
                                //newData.IdLpnSourceUsed = data.IdLpnSourceUsed.ToString();
                                //newData.IdLpnTargetUsed = data.IdLpnTargetUsed.ToString();
                                //newData.RealQty = decimal.Parse(data.RealQty.ToString());
                                //newData.UserAssigned = data.UserAssigned.ToString();
                                //newData.RealStartDate = Convert.ToDateTime(data.RealStartDate);
                                //newData.RealEndDate = Convert.ToDateTime(data.RealEndDate);

                                list.Add(newData);
                            }
                            else
                            {
                                messageError = "Tarea seleccionada no corresponde con el archivo cargado";
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(messageError))
                        {
                            string LPNNumberSufix = GetConst("LPNNumberSufix")[0];
                            List<String> theListLocType = new List<string>();
                            theListLocType = GetConst("LocationTypeDispatch");

                            resultViewDTO = iDispatchingMGR.UpdateTasksFromPTL(list, theListLocType, LPNNumberSufix, context);

                            if (resultViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, resultViewDTO.Errors.Message);
                                divLoadVoicePicking.Visible = true;
                                modalPopUpLoadVoicePicking.Show();
                            }
                            else
                            {
                                UpdateSession(false);
                                upGrid.Update();

                                ucStatus.ShowMessage(resultViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                divLoadVoicePicking.Visible = false;
                                modalPopUpLoadVoicePicking.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, messageError.Trim());
                            divLoadVoicePicking.Visible = true;
                            modalPopUpLoadVoicePicking.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Movement.Consult.TaskMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }
                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divLoadVoicePicking.Visible = true;
                    modalPopUpLoadVoicePicking.Show();
                }

                #region "codigo antiguo"
                //var listDataFromVoiceLink = from r in dataTable.AsEnumerable()
                //               select new
                //               {
                //                   NroPedido = r.Field<object>("NroPedido"),
                //                   EstadoPedido = r.Field<object>("EstadoPedido"),
                //                   FecIniPedido = r.Field<object>("FecIniPedido"),
                //                   HoraIniPedido = r.Field<object>("HoraIniPedido"),
                //                   FecTermPedido = r.Field<object>("FecTermPedido"),
                //                   HoraTermPedido = r.Field<object>("HoraTermPedido"),
                //                   MinTotalPedido = r.Field<object>("MinTotalPedido"),
                //                   SegTotalPedido = r.Field<object>("SegTotalPedido"),
                //                   MinPicking = r.Field<object>("MinPicking"),
                //                   SegPicking = r.Field<object>("SegPicking"),
                //                   MetaTiempo = r.Field<object>("MetaTiempo"),
                //                   Ruta = r.Field<object>("Ruta"),
                //                   Muelle = r.Field<object>("Muelle"),
                //                   Region = r.Field<object>("Region"),
                //                   Secuencia = r.Field<object>("Secuencia"),
                //                   CodItem = r.Field<object>("CodItem"),
                //                   QtyPick = r.Field<object>("QtyPick"),
                //                   QtyPickeado = r.Field<object>("QtyPickeado"),
                //                   Ubicacion = r.Field<object>("Ubicacion"),
                //                   Operador = r.Field<object>("Operador"),
                //                   FecPick = r.Field<object>("FecPick"),
                //                   HoraPick = r.Field<object>("HoraPick"),
                //                   Contenedor = r.Field<object>("Contenedor")
                //               };

                //var excelData = listDataFromVoiceLink.ToList();

                //try
                //{
                //    var list = new List<DataFromVoiceLink>();

                //    foreach (var data in excelData)
                //    {
                //       var newData = new DataFromVoiceLink();
                //        newData.NroPedido = data.NroPedido.ToString();

                //        var fecTermPedidoTemp = data.FecTermPedido.ToString();
                //        newData.FecTermPedido = fecTermPedidoTemp.Split(' ')[0];

                //        var horaTermPedidoTemp = data.HoraTermPedido.ToString();
                //        newData.HoraTermPedido = horaTermPedidoTemp.Split(' ')[1];

                //        newData.Muelle = "ANDEN0" + data.Muelle.ToString();
                //        newData.CodItem = data.CodItem.ToString();
                //        newData.QtyPick = int.Parse(data.QtyPick.ToString());
                //        newData.QtyPickeado = int.Parse(data.QtyPickeado.ToString());
                //        newData.Ubicacion = "0" + data.Ubicacion.ToString(); //TODO
                //        newData.Operador = data.Operador.ToString();
                //        newData.Contenedor = data.Contenedor.ToString();
                //        list.Add(newData);
                //    }

                //    int idTaskSelected = int.Parse(hidIdTaskSelected.Value);
                //    var typeCode = taskConsultViewDTO.Entities.Where(t => t.Task.Id == idTaskSelected).FirstOrDefault().Task.TypeCode;

                //    if (typeCode == "PIKOR")
                //        resultViewDTO = iTasksMGR.UpdateTasksFromVoicePicking(list, context);
                //    else if (typeCode == "PIKWV")
                //        resultViewDTO = iTasksMGR.UpdateTasksWaveFromVoicePicking(list, context);

                //    if (resultViewDTO.hasError())
                //    {
                //        ShowAlertLocal(this.lblTitle.Text, resultViewDTO.Errors.Message);
                //        divLoadVoicePicking.Visible = true;
                //        modalPopUpLoadVoicePicking.Show();
                //    }
                //    else
                //    {
                //        UpdateSession(false);
                //        upGrid.Update();

                //        ucStatus.ShowMessage(resultViewDTO.MessageStatus.Message);
                //        ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                //        divLoadVoicePicking.Visible = false;
                //        modalPopUpLoadVoicePicking.Hide();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    theLog.exceptionMessage("Binaria.WMSTek.WebClient.Movement.Consult.TaskMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                //    throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                //}
                //}
                //else
                //{
                //    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                //    divLoadVoicePicking.Visible = true;
                //    modalPopUpLoadVoicePicking.Show();
                //}
                #endregion
            }
            catch (InvalidDataException ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoadVoicePicking.Visible = true;
                modalPopUpLoadVoicePicking.Show();
            }
            catch (InvalidOperationException ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoadVoicePicking.Visible = true;
                modalPopUpLoadVoicePicking.Show();
            }
            catch (Exception ex)
            {
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.ItemMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                resultViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, resultViewDTO.Errors.Message);
            }
            finally
            {
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
            }
        }

        private List<string> TypeTaskAllowedToCompleteWithoutValidation()
        {
            return new List<string>() { "PAKOR", "SORT" };
        }
        private new void LoadUsersOperatorWihtOutDefaultWhs(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();
            var newContext = NewContext();

            userViewDTO = iProfileMGR.GetOperatorWihtOutDefaultWhsByWhs(newContext);

            objControl.Items.Clear();
            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                objControl.Items.Add(new ListItem((user.UserName), user.UserName));
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }
        #endregion
    }
}
