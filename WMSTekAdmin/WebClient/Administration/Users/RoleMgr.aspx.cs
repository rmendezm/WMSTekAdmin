using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Users
{
    public partial class RoleMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Role> roleViewDTO = new GenericViewDTO<Role>();
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

                    if (ValidateSession(WMSTekSessions.RoleMgr.RoleList))
                    {
                        roleViewDTO = (GenericViewDTO<Role>)Session[WMSTekSessions.RoleMgr.RoleList];
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                SaveChanges();
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
        //        roleViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(roleViewDTO.Errors);
        //    }
        //}

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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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

                    // Deshabilita la opcion de Eliminar si es el Rol Base
                    if (btnDelete != null && roleViewDTO.Entities[e.Row.DataItemIndex].IsBaseRole)
                    {
                        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        btnDelete.Enabled = false;
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        //protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        //{

        //    base.grdMgr_RowDataBound(sender, e);

        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        //Carga los textos del tool tip

        //        ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;
        //        ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

        //        if (btnEdit != null)
        //        {
        //            btnEdit.ToolTip = this.lblToolTipEditar.Text;
        //        }
        //        if (btnDelete != null)
        //        {
        //            btnDelete.ToolTip = this.lblToolTipEliminar.Text;
        //        }
        //    }
        //}

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
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
            PopulateLists();
        }

        /// <summary>
        /// Carga en sesion la lista de Roles
        /// </summary>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(roleViewDTO.Errors);
                roleViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;   

            // Carga lista de Roles (sin estructura de Permisos ni Usuarios)
            roleViewDTO = iProfileMGR.FindAllRole(false,false, true,objUser.Language.Id, context);

            if (!roleViewDTO.hasError() && roleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RoleMgr.RoleList, roleViewDTO);
                Session.Remove(WMSTekSessions.PermissionMgr.RoleList);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(roleViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            Master.ucTaskBar.btnNewVisible = true;
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

        private void PopulateLists()
        {
            base.LoadModule(this.ddlRolModule, true, this.Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!roleViewDTO.hasConfigurationError() && roleViewDTO.Configuration != null && roleViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, roleViewDTO.Configuration);

            grdMgr.DataSource = roleViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(roleViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Configura ventana modal
            if (roleViewDTO.Configuration != null && roleViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(roleViewDTO.Configuration, true);
                else
                    base.ConfigureModal(roleViewDTO.Configuration, false);
            }

            // Editar entidad
            if (mode == CRUD.Update)
            {
                //Recupera la fila a editar
                hidEditId.Value = roleViewDTO.Entities[index].Id.ToString();
                hidEditIdRoleModule.Value = roleViewDTO.Entities[index].roleModule.Id.ToString();

                //el dato IsBaseRole es de solo lectura
                hidIsBaseRole.Value = roleViewDTO.Entities[index].IsBaseRole.ToString();

                //Carga controles
                chkCodStatus.Checked = roleViewDTO.Entities[index].CodStatus;
                txtName.Text = roleViewDTO.Entities[index].Name;
                txtDescription.Text = roleViewDTO.Entities[index].Description;

                if (roleViewDTO.Entities[index].roleModule != null)
                {
                    ddlRolModule.SelectedValue = roleViewDTO.Entities[index].roleModule.Module.Id.ToString();
                }
                else
                {
                    ddlRolModule.SelectedValue = "-1";
                }

                // Si es el Rol Base, deshabilita la edicion de la opcion 'CodStatus'
                if (roleViewDTO.Entities[index].IsBaseRole)
                    chkCodStatus.Enabled = false;
                else
                    chkCodStatus.Enabled = true;

                lblNew.Visible = false;
                lblEdit.Visible = true;
                lblView.Visible = false;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";

                chkCodStatus.Checked = true;
                txtName.Text = string.Empty;
                txtDescription.Text = string.Empty;
                ddlRolModule.SelectedValue = "-1";

                lblNew.Visible = true;
                lblEdit.Visible = false;
                lblView.Visible = false;
            }

            // Ver entidad
            if (mode == CRUD.Read)
            {
                //Recupera la fila a editar
                hidEditId.Value = roleViewDTO.Entities[index].Id.ToString();
                hidEditIdRoleModule.Value = roleViewDTO.Entities[index].roleModule.Id.ToString();
               
                //el dato IsBaseRole es de solo lectura
                hidIsBaseRole.Value = roleViewDTO.Entities[index].IsBaseRole.ToString();

                //Carga controles
                chkCodStatus.Checked = roleViewDTO.Entities[index].CodStatus;
                txtName.Text = roleViewDTO.Entities[index].Name;
                txtDescription.Text = roleViewDTO.Entities[index].Description;

                //Deshabilita todos los controles
                // TODO: hacerlo en base.ConfigureModal
                chkCodStatus.Enabled = false;
                txtName.Enabled = false;
                txtDescription.Enabled = false;

                lblNew.Visible = false;
                lblEdit.Visible = false;
                lblView.Visible = true;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        /// <summary>
        /// Persiste los cambios realizados en la entidad
        /// </summary>
        /// <param name="index"></param>
        protected void SaveChanges()
        {
            Role role = new Role();

            role.Id = Convert.ToInt32(hidEditId.Value);
            role.IsBaseRole = Convert.ToBoolean(hidIsBaseRole.Value);
            role.Name = txtName.Text;
            role.Description = txtDescription.Text;
            role.CodStatus = chkCodStatus.Checked;
            role.roleModule = new RoleModule();
            role.roleModule.Id = Convert.ToInt32(hidEditIdRoleModule.Value);
            role.roleModule.IdRole = Convert.ToInt32(hidEditId.Value);
            role.roleModule.IdModule = int.Parse(ddlRolModule.SelectedValue); 

            if (hidEditId.Value == "0")
                roleViewDTO = iProfileMGR.MaintainRole(CRUD.Create, role, context);
            else
                roleViewDTO = iProfileMGR.MaintainRole(CRUD.Update, role, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (roleViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(roleViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            roleViewDTO = iProfileMGR.MaintainRole(CRUD.Delete, roleViewDTO.Entities[index], context);

            if (roleViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(roleViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        #endregion
    }
}
