using System;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Movement.Consult
{
    public partial class TaskPriorities : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskConsult> taskConsultViewDTO = new GenericViewDTO<TaskConsult>();
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
                        UpdateSession(false);
                    }

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
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges(-1);
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
                    // Agrega atributos para cambiar el color de la fila seleccionada
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int index = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                SaveChanges(index);
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
                ImageButton imgEdit = e.Row.FindControl("btnEdit") as ImageButton;

                base.grdMgr_RowDataBound(sender, e);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

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

        #endregion

        #region "Eventos"

        public void Initialize()
        {
            //context.SessionInfo.IdPage = "ResetPassword";
            context.SessionInfo.IdPage = "TaskPriorities";

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

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            taskConsultViewDTO = iTasksMGR.FindAllPriorityTask(context);

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
            this.Master.ucMainFilter.listTaskTypeCode= GetConst("TaskPriorities");

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
                        
            this.Master.ucMainFilter.dateVisible = true;
            this.Master.ucMainFilter.taskTypeVisible = true;
            this.Master.ucMainFilter.taskTypeNotIncludeAll = false;

            // Habilita el Filtro Avanzado
            this.Master.ucMainFilter.advancedFilterVisible = true;

            //Tab Reception
            this.Master.ucMainFilter.tabReceptionLogVisible = true;
            this.Master.ucMainFilter.tabReceptionSourceLpn = false;
            this.Master.ucMainFilter.tabReceptionTargetLpn = false;

            // Configura textos a mostar
            this.Master.ucMainFilter.setDateLabel = true;
            this.Master.ucMainFilter.dateLabel = lblFilterName.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.TaskDaysAfterQuery;//hoy
            
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
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
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
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!taskConsultViewDTO.hasConfigurationError() && taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskConsultViewDTO.Configuration);

            grdMgr.DataSource = taskConsultViewDTO.Entities;
            grdMgr.DataBind();


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
        protected void ShowModal(int index, CRUD mode)
        {
            // Configura ventana modal
            if (taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(taskConsultViewDTO.Configuration, true);
                else
                    base.ConfigureModal(taskConsultViewDTO.Configuration, false);
            }
            // Editar usuario
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = taskConsultViewDTO.Entities[index].Id.ToString();

                //el dato IsBaseUser es de solo lectura
                //hidIsBaseUser.Value = taskConsultViewDTO.Entities[index].IsBaseUser.ToString();

                //Carga controles
                txtIdTask.Text = taskConsultViewDTO.Entities[index].Task.Id.ToString();
                txtTaskName.Text = taskConsultViewDTO.Entities[index].Task.Description;
                txtActualPriority.Text = taskConsultViewDTO.Entities[index].Task.Priority.ToString();
                //carga el indice del enum
                //int i = Convert.ToInt16(CfgParameterName.DefaultPassword);
                //asigna el valor que tiene la variable en el indice i
                txtNewPriority.Text = string.Empty;

                this.txtIdTask.Enabled = false;
                this.txtTaskName.Enabled = false;
                this.txtActualPriority.Enabled = false;

                lblEdit.Visible = true;

            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        /// <summary>
        /// Persiste los cambios en la entidad (modo Edit o New). 
        /// </summary>
        protected void SaveChanges(int index)
        {
            util = new MiscUtils();
            Task task = new Task();

            //if (index != -1)
            //{
            //    hidEditId.Value = taskConsultViewDTO.Entities[index].Id.ToString();

            //    int i = Convert.ToInt16(CfgParameterName.DefaultPassword);
            //    user.Password = MiscUtils.Encrypt(context.CfgParameters[i].ParameterValue);
            //}
            //else
            //{  //Se encripta la pass
            //    user.Password = MiscUtils.Encrypt(txtPassword.Text);
            //}
            //agrega los datos del usuario

            task.Id = Convert.ToInt32(hidEditId.Value);
            task.Priority = Convert.ToInt16(txtNewPriority.Text);

            // Editar Password
            taskConsultViewDTO = iTasksMGR.UpdatePriority(task, context);

            divEditNew.Visible = false;
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

        #endregion


    }
}
