using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class TaskTypeMgr : BasePage
    {
        #region "Declaracion de variables"

        private GenericViewDTO<TaskType> taskTypeViewDTO;
        private bool isValidViewDTO = false;

        //Propiedad para controlar el nro de pagina activa en la grilla
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
                        PopulateLists();
                        UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.TaskTypeMgr.TaskTypeList))
                    {
                        taskTypeViewDTO = (GenericViewDTO<TaskType>)Session[WMSTekSessions.TaskTypeMgr.TaskTypeList];
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
        //        taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
        //    }
        //}

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    //if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
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
                taskTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
            }
        }


        #endregion

        #region "Métodos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
                taskTypeViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            taskTypeViewDTO = iTasksMGR.FindAllTaskType(context);

            if (!taskTypeViewDTO.hasError() && taskTypeViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TaskTypeMgr.TaskTypeList, taskTypeViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(taskTypeViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(taskTypeViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!taskTypeViewDTO.hasConfigurationError() && taskTypeViewDTO.Configuration != null && taskTypeViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskTypeViewDTO.Configuration);

            grdMgr.DataSource = taskTypeViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(taskTypeViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            //Carga lista de Owner
            //base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;

            this.Master.ucMainFilter.nameLabel = this.lblNameFilter.Text;

            // TODO: personalizar vista del Filtro
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

        protected void ReloadData()
        {
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;
            //Actualiza la grilla
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
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
            // Editar Proveedor
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = taskTypeViewDTO.Entities[index].Id.ToString();
                txtName.Text = taskTypeViewDTO.Entities[index].Name;
                txtPriority.Text = taskTypeViewDTO.Entities[index].Priority.ToString();

                //lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Cliente
            if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores

                //General
                hidEditId.Value = "0";
                txtName.Text = string.Empty;
                txtPriority.Text = string.Empty;

                //De la página
                //lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (taskTypeViewDTO.Configuration != null && taskTypeViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(taskTypeViewDTO.Configuration, true);
                else
                    base.ConfigureModal(taskTypeViewDTO.Configuration, false);
            }

            if (mode == CRUD.Update)
            {
                txtPriority.Enabled = true;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {

            TaskType taskType = new TaskType();

            taskType.Name = txtName.Text.Trim();
            taskType.Priority = Convert.ToInt32(txtPriority.Text);

            if (hidEditId.Value != "0")
            {
                taskType.Id = Convert.ToInt32(hidEditId.Value);
                taskTypeViewDTO = iTasksMGR.UpdateTaskTypePriority(taskType, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (taskTypeViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(taskTypeViewDTO.MessageStatus.Message);
                //Actualiza
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            //taskTypeViewDTO = iWarehousingMGR.MaintainTaskType(CRUD.Delete, taskTypeViewDTO.Entities[index], context);

            ////Muestra mensaje de status
            //crud = true;
            //ucStatus.ShowMessage(taskTypeViewDTO.MessageStatus.Message);

            ////Actualiza la session
            //if (taskTypeViewDTO.hasError())
            //    UpdateSession(true);
            //else
            //{
            //    crud = true;
            //    ucStatus.ShowMessage(taskTypeViewDTO.MessageStatus.Message);

            //    UpdateSession(false);
            //}
        }
        #endregion
    }
}
