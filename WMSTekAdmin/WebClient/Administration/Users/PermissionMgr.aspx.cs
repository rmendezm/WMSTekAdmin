using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Base;
using System.Web;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Administration.Users
{
    public partial class PermissionMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Role> roleViewDTO = new GenericViewDTO<Role>();
        private GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

        private GenericViewDTO<RoleModule> roleModuleViewDTO = new GenericViewDTO<RoleModule>();
        private GenericViewDTO<Permission> permissionViewDTO = new GenericViewDTO<Permission>();
        private GenericViewDTO<User> userAsigRolViewDTO = new GenericViewDTO<User>();
        private bool isValidViewDTO = false;

        #endregion

        #region "Eventos"

        /// <summary>
        /// Los controles dinamicos deben ser CREADOS en Page_Init (antes de formarse el View State)
        /// </summary>
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
                        hsVertical.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .5);

                        // Carga inicial del ViewDTO
                        UpdateSession(false);

                        if (isValidViewDTO)
                        {
                            PopulateData();
                            CreateMenuTree_New();
                            SetMenuTree_New(ddlRole.SelectedIndex);
                            LoadUsers_New(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));
                        }
                    }
                    else
                    {
                        if (ValidateSession(WMSTekSessions.PermissionMgr.RoleList))
                        {
                            roleViewDTO = (GenericViewDTO<Role>)Session[WMSTekSessions.PermissionMgr.RoleList];
                        }

                        if (ValidateSession(WMSTekSessions.PermissionMgr.PermissionList))
                        {
                            permissionViewDTO = (GenericViewDTO<Permission>)Session[WMSTekSessions.PermissionMgr.PermissionList];
                        }

                        if (ValidateSession(WMSTekSessions.PermissionMgr.UserList))
                        {
                            userViewDTO = (GenericViewDTO<User>)Session[WMSTekSessions.PermissionMgr.UserList];
                        }

                        if (ValidateSession(WMSTekSessions.PermissionMgr.UserAsigRolList))
                        {
                            userAsigRolViewDTO = (GenericViewDTO<User>)Session[WMSTekSessions.PermissionMgr.UserAsigRolList];
                        }

                        CreateMenuTree_New();
                    }
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message == "A child row has multiple parents.")
                //{
                //    ex.Message == "Existen permisos de opciones de menus  con idObjeto duplicados";
                //}
                //else
                //{
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
                //}
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                //Modifica el Ancho del Div Principal
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivPrincipal();", true);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
        /// </summary>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            //Función para refrescar cuando el combo del centro cambia, pero de deshabilita 
            //Al hacer un segundo cambio no despliega imagen de procesando
            try
            {
                UpdateSession(false);

                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    PopulateData();
                    LoadUsers_New(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));
                    CreateMenuTree_New();
                    SetMenuTree_New(ddlRole.SelectedIndex);
                    upRole.Update();
                    upUser.Update();
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
            //try
            //{
            //    UpdateSession(false);

            //    // Si es un ViewDTO valido, carga la grilla y las listas
            //    if (isValidViewDTO)
            //    {
            //        SetMenuTree(ddlRole.SelectedIndex);
            //        LoadUsers(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    roleViewDTO.Errors = baseControl.handleException(ex, context);
            //    this.Master.ucError.ShowError(roleViewDTO.Errors);
            //}
        }

        protected void grdUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //envia el id de rol desde el DataKeys
                RemoveUser(ddlRole.SelectedIndex, e.RowIndex);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void grdRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //envia el id de usuario desde el DataKeys
                RemoveRole(ddlUsers2.SelectedIndex - 1, e.RowIndex);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void grdUsers_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        // Deshabilita la opcion de Eliminar si es el Usuario Base del Rol Base
                        ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                        if (btnDelete != null && roleViewDTO.Entities.First(w => w.Id == int.Parse(ddlRole.SelectedValue)).IsBaseRole
                            && userAsigRolViewDTO.Entities[e.Row.RowIndex].IsBaseUser)
                        {
                            btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                            btnDelete.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    int idModule = int.Parse(this.ddlRoleModule.SelectedValue);
                    int idRole = int.Parse(this.ddlRole.SelectedValue);
                    permissionViewDTO = iProfileMGR.FindPermissionByRoleAndModule(idRole, objUser.Language.Id, idModule, context);

                    Session.Add(WMSTekSessions.PermissionMgr.PermissionList, permissionViewDTO);

                    CreateMenuTree_New();
                    SetMenuTree_New(ddlRole.SelectedIndex);
                    LoadUsers_New(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));


                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void ddlRoleModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    int roleModule = int.Parse(this.ddlRoleModule.SelectedValue);
                    roleViewDTO = new GenericViewDTO<Role>();
                    roleModuleViewDTO = iProfileMGR.FindRoleModuleByIdModule(roleModule, context);

                    foreach (var item in roleModuleViewDTO.Entities)
                    {
                        roleViewDTO.Entities.Add(item.Role);
                    }

                    Session.Add(WMSTekSessions.PermissionMgr.RoleList, roleViewDTO);

                    // Carga lista de Roles (panel superior)
                    ddlRole.DataSource = roleViewDTO.Entities;
                    ddlRole.DataTextField = "Name";
                    ddlRole.DataValueField = "Id";
                    ddlRole.DataBind();

                    int idModule = int.Parse(this.ddlRoleModule.SelectedValue);
                    permissionViewDTO = iProfileMGR.FindPermissionByRoleAndModule(roleViewDTO.Entities[0].Id, objUser.Language.Id, idModule, context);

                    CreateMenuTree_New();

                    SetMenuTree_New(ddlRole.SelectedIndex);
                    LoadUsers_New(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void ddlUsers2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (ddlUsers2.SelectedIndex > 0)
                    {
                        divRoles.Visible = true;
                        LoadRoles(ddlUsers2.SelectedIndex - 1, Convert.ToInt16(ddlUsers2.SelectedValue));
                    }
                    else
                    {
                        divRoles.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void btnSave1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChangesRole(ddlRole.SelectedIndex);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void btnSave2_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlUsers2.SelectedIndex > 0) SaveChangesUser(ddlUsers2.SelectedIndex - 1, ddlRole.SelectedIndex);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlUsers.SelectedIndex > 0)
                {
                    GenericViewDTO<RoleModule> roleModuleAuxViewDTO = new GenericViewDTO<RoleModule>();
                    roleModuleAuxViewDTO = iProfileMGR.FindRoleModuleByIdModuleIdUser(Convert.ToInt16(ddlRoleModule.SelectedValue), Convert.ToInt16(ddlUsers.SelectedValue), context);

                    if (roleModuleAuxViewDTO != null && roleModuleAuxViewDTO.Entities.Count > 0)
                    {
                        this.Master.ucDialog.ShowAlert(lblTitle.Text, lblAlreadyAssignedUser.Text, "");
                        return;
                    }
                }

                AddUser_New(ddlRole.SelectedIndex);
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void btnAddRole_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlUsers2.SelectedIndex > 0)
                {
                    AddRole(ddlUsers2.SelectedIndex - 1);
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void grdUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                LoadUsers(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }

        protected void grdRoles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                LoadRoles(ddlUsers2.SelectedIndex - 1, Convert.ToInt16(ddlUsers2.SelectedValue));
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"

        /// <summary>
        /// Valida e Inicializa los objetos de Sesion
        /// </summary>
        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);

            base.LoadModuleAssigned(this.ddlRoleModule, false, "");
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
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Este mantenedor no utiliza el filtro, pero es necesario inicializarlo de todas maneras
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.searchVisible = false;
            this.Master.ucMainFilter.ddlWareHouseIndexChanged += new EventHandler(ddlFilterWarehouse_SelectedIndexChanged);
            this.Master.ucMainFilter.Initialize(init, refresh);
        }

        protected void ddlFilterWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    UpdateSession(false);

            //    // Si es un ViewDTO valido, carga la grilla y las listas
            //    if (isValidViewDTO)
            //    {
            //        SetMenuTree_New(ddlRole.SelectedIndex);
            //        PopulateData();
            //        LoadUsers_New(ddlRole.SelectedIndex, Convert.ToInt16(ddlRole.SelectedValue));
            //        upRole.Update();
            //        upUser.Update();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    roleViewDTO.Errors = baseControl.handleException(ex, context);
            //    this.Master.ucError.ShowError(roleViewDTO.Errors);
            //}
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
                this.Master.ucError.ShowError(roleViewDTO.Errors);
                roleViewDTO.ClearError();
            }

            int idModule = int.Parse(ddlRoleModule.SelectedValue);

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            // Carga lista de Roles (con estructura de Permisos y Usuarios)
            //roleViewDTO = iProfileMGR.FindAllRoleByIdWhs(false, false, false, objUser.Language.Id, context);

            int roleModule = int.Parse(this.ddlRoleModule.SelectedValue);
            roleModuleViewDTO = iProfileMGR.FindRoleModuleByIdModule(roleModule, context);

            if (roleViewDTO.Entities == null)
            {
                roleViewDTO.Entities = new List<Role>();
            }
            else
            {
                roleViewDTO.Entities.Clear();
            }
            //List<Role> lstRoles = new List<Role>();
            foreach (var item in roleModuleViewDTO.Entities)
            {
                roleViewDTO.Entities.Add(item.Role);
            }

            if (!roleViewDTO.hasError() && roleViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.PermissionMgr.RoleList, roleViewDTO);


                permissionViewDTO = iProfileMGR.FindPermissionByRoleAndModule(roleViewDTO.Entities[0].Id, objUser.Language.Id, idModule, context);

                if (!permissionViewDTO.hasError() && permissionViewDTO.Entities != null)
                {
                    isValidViewDTO = true;
                    Session.Add(WMSTekSessions.PermissionMgr.PermissionList, permissionViewDTO);
                }

                // Carga lista de Usuarios activos (con lista de Roles asignados)
                //userViewDTO = iProfileMGR.GetUsersByCodStatus(CodStatus.Enabled, true, context);
                userViewDTO = iProfileMGR.GetUsersByCodStatusIdWhs(CodStatus.Enabled, true, context);

                if (!userViewDTO.hasError() && userViewDTO.Entities != null)
                {
                    isValidViewDTO = true;
                    Session.Add(WMSTekSessions.PermissionMgr.UserList, userViewDTO);
                }


                if (!crud)
                    ucStatus.ShowMessage(roleViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }

            // Limpia panel inferior
            /*
            ddlUsers2.SelectedIndex = 0;
            grdRoles.DataSource = null;
            grdRoles.DataBind();
             */
        }

        private void PopulateData()
        {
            // Carga lista de Roles (panel superior)
            ddlRole.DataSource = roleViewDTO.Entities;
            ddlRole.DataTextField = "Name";
            ddlRole.DataValueField = "Id";
            ddlRole.DataBind();

            // Carga lista de Usuarios(panel inferior)
            //base.LoadUsersByCodStatus(ddlUsers2, CodStatus.Enabled, false, true, lblSelectUser.Text);
            base.GetUsersByCodStatusIdWhs(ddlUsers2, CodStatus.Enabled, false, true, lblSelectUser.Text);
        }

        private void LoadUsers(int rolIndex, int rolId)
        {
            roleViewDTO.Entities[rolIndex].Users = iProfileMGR.GetByRoleAndIdWhs(rolId, context).Entities.ToList();

            // Usuarios asignados al Rol
            grdUsers.DataSource = roleViewDTO.Entities[rolIndex].Users;
            grdUsers.DataBind();

            // Usuarios no asignados al Rol
            ddlUsers.Items.Clear();
            //base.LoadUsersByNotInRole(ddlUsers, rolId, true, lblSelectUser.Text);
            base.LoadUsersByNotInRoleAndIdWhs(ddlUsers, rolId, true, lblSelectUser.Text);
        }

        private void LoadUsers_New(int rolIndex, int rolId)
        {

            userAsigRolViewDTO = iProfileMGR.GetByRoleAndIdWhs(rolId, context);

            if (!userAsigRolViewDTO.hasError() && userAsigRolViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.PermissionMgr.UserAsigRolList, userAsigRolViewDTO);
            }

            // Usuarios asignados al Rol
            grdUsers.DataSource = userAsigRolViewDTO.Entities;
            grdUsers.DataBind();

            // Usuarios no asignados al Rol
            ddlUsers.Items.Clear();
            //base.LoadUsersByNotInRole(ddlUsers, rolId, true, lblSelectUser.Text);
            base.LoadUsersByNotInRoleAndIdWhs(ddlUsers, rolId, true, lblSelectUser.Text);
        }


        private void LoadRoles(int userIndex, int userId)
        {
            roleViewDTO = iProfileMGR.GetRolesByInUser(userId, context);

            // Roles asignados al Usuario
            grdRoles.DataSource = roleViewDTO.Entities;
            grdRoles.DataBind();

            // Roles no asignados al Usuario
            ddlRole2.Items.Clear();
            base.loadRolesByNotInUser(ddlRole2, userId, true, lblSelectRole.Text);
        }



        private void CreateMenuTree_New()
        {
            // Crea estructura del Menu
            string chkId = string.Empty;
            XmlDocument xml = new XmlDocument();

            // Carga la estructura de menu base (Rol Amdminstrador)
            xml.LoadXml(GetXML_New(0));

            //Limpia todos los controles creados de forma dinamica
            phControls.Controls.Clear();

            // Si hay opciones de Menu para mostrar...
            if (xml.ChildNodes[0].HasChildNodes)
            {
                LoopCreateMenuTree(xml.ChildNodes[0], 1);
            }

        }

        protected void LoopCreateMenuTree(XmlNode node, int level)
        {
            string chkId = string.Empty;

            foreach (XmlNode childNode in node)
            {
                if (childNode.Name == "Menu")
                {
                    // Crea checkbox 'Parent'
                    chkId = "chk_" + childNode["IdMenu"].InnerText;

                    CheckBox chkParent = new CheckBox();
                    chkParent.ID = chkId;
                    chkParent.Text = childNode["TextValue"].InnerText;
                    chkParent.Checked = true;
                    chkParent.CheckedChanged += new EventHandler(chkParent_CheckedChanged);
                    chkParent.AutoPostBack = true;

                    for (int i = 1; i <= 5 * level; i++)
                        phControls.Controls.Add(new LiteralControl("&nbsp;"));

                    phControls.Controls.Add(chkParent);
                    phControls.Controls.Add(new LiteralControl("<br />"));


                    //AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                    //trigger.ControlID = chkParent.ID;
                    //trigger.EventName = "CheckedChanged";
                    //this.upRole.Triggers.Add(trigger);

                    if (childNode.HasChildNodes)
                        LoopCreateMenuTree(childNode, level + 1);
                }
            }



        }


        protected void chkParent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string idMenuObject = ((CheckBox)sender).ID;
                int rowIndex = ddlRole.SelectedIndex;
                string chkId = string.Empty;
                SetMenuTreeNew(rowIndex, idMenuObject);

                int cont = 1;
                bool existControl = false;
                string nameControlExist = "";
                CheckBox chkCheck = null;

                //Busca los control existentes  dentro de control phControls
                foreach (Control item in phControls.Controls)
                {
                    if ((cont == 5) && (item is CheckBox))
                    {
                        nameControlExist = item.ClientID;
                        chkCheck = (CheckBox)item;
                        break;
                    }

                    //Valida que el control sea CheckBox
                    if (item is CheckBox)
                    {
                        //Valida si el control el el mismo del checkeado
                        if (item.ID == idMenuObject)
                            existControl = true;

                        if (existControl)
                            cont++;
                    }
                }

                if (chkCheck != null)
                {
                    chkCheck.Focus();
                }
                else
                {
                    ((CheckBox)sender).Focus();
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }

        }

        private void SetMenuTree(int rowIndex)
        {
            // Setea estructura del Menu
            string chkId = string.Empty;

            // Carga la estructura de menu del Rol seleccionado
            DataSet ds = GetDataSet_New(rowIndex);

            // Si hay opciones de Menu para mostrar...
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow parentRow in ds.Tables[0].Rows)
                {
                    // Primer nivel (sin 'Parent')
                    if (parentRow.GetParentRows("ParentChild").Count() == 0)
                    {
                        // Busca checkbox 'Parent'
                        chkId = "chk_" + parentRow["IdMenu"];

                        CheckBox chk = (CheckBox)phControls.FindControl(chkId);

                        if (chk != null)
                        {
                            chk.Checked = Convert.ToInt32(parentRow["IdPermission"].ToString()) != -1 ? true : false;
                            chk.Enabled = roleViewDTO.Entities[ddlRole.SelectedIndex].IsBaseRole ? false : true;

                        }
                    }

                    if (parentRow.GetChildRows("ParentChild").Count() > 0)
                        LoopSetMenuTree_New(parentRow.GetChildRows("ParentChild"), 1);
                }
            }
        }

        private void SetMenuTree_New(int rowIndex)
        {
            // Setea estructura del Menu
            string chkId = string.Empty;

            // Carga la estructura de menu del Rol seleccionado
            DataSet ds = GetDataSet_New(rowIndex);

            // Si hay opciones de Menu para mostrar...
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow parentRow in ds.Tables[0].Rows)
                {
                    // Primer nivel (sin 'Parent')
                    if (parentRow.GetParentRows("ParentChild").Count() == 0)
                    {
                        // Busca checkbox 'Parent'
                        chkId = "chk_" + parentRow["IdMenu"];

                        CheckBox chk = (CheckBox)phControls.FindControl(chkId);

                        if (chk != null)
                        {
                            chk.Checked = Convert.ToInt32(parentRow["IdPermission"].ToString()) != -1 ? true : false;
                            chk.Enabled = roleViewDTO.Entities.First(w => w.Id == int.Parse(ddlRole.SelectedValue)).IsBaseRole ? false : true;

                        }
                    }

                    if (parentRow.GetChildRows("ParentChild").Count() > 0)
                        LoopSetMenuTree_New(parentRow.GetChildRows("ParentChild"), 1);
                }
            }
        }


        protected void LoopSetMenuTree_New(DataRow[] childRows, int level)
        {
            string chkId = string.Empty;
            //var lstIdObjectNotVisible = GetConst("MenuIdObjectNotVisiblePermission");

            foreach (DataRow childRow in childRows)
            {
                // Crea checkbox 'Child'
                chkId = "chk_" + childRow["IdMenu"];

                CheckBox chk = (CheckBox)phControls.FindControl(chkId);

                if (chk != null)
                {
                    chk.Checked = Convert.ToInt32(childRow["IdPermission"].ToString()) != -1 ? true : false;
                    chk.Enabled = roleViewDTO.Entities.First(w => w.Id == int.Parse(ddlRole.SelectedValue)).IsBaseRole ? false : true;
                }

                // Si tiene hijos, los recorre
                if (childRow.GetChildRows("ParentChild").Count() > 0)
                    LoopSetMenuTree_New(childRow.GetChildRows("ParentChild"), level + 1);
            }
        }
        /// <summary>
        /// Obtiene un Dataset del menu de cada rol, a partir de una List<Menu>
        /// </summary>
        /// <param name="menuViewDTO"></param>
        /// <returns></returns>
        private DataSet GetDataSet_New(int roleIndex)
        {
            if (permissionViewDTO == null)
                permissionViewDTO = (GenericViewDTO<Permission>)Session[WMSTekSessions.PermissionMgr.PermissionList];

            List<Permission> lstPermission = permissionViewDTO.Entities;
            DataSet dsPermission = new DataSet();

            if (lstPermission != null && lstPermission.Count() > 0)
            {
                dsPermission.Tables.Add("Menu");
                dsPermission.Tables["Menu"].Columns.Add("IdPermission");
                dsPermission.Tables["Menu"].Columns.Add("IdMenu");
                dsPermission.Tables["Menu"].Columns.Add("IDParentMenu");
                dsPermission.Tables["Menu"].Columns.Add("Name");
                dsPermission.Tables["Menu"].Columns.Add("TextValue");
                dsPermission.Tables["Menu"].Columns.Add("IdMenuPage");
                dsPermission.Tables["Menu"].Columns.Add("AspxPage");
                dsPermission.Tables["Menu"].Columns.Add("WinPath");

                foreach (Permission permissionItem in lstPermission)
                {
                    DataRow row = dsPermission.Tables["Menu"].NewRow();

                    row["IdPermission"] = permissionItem.Id;
                    row["IdMenu"] = permissionItem.MenuItem.Id;
                    row["IDParentMenu"] = permissionItem.MenuItem.IDParentMenuItem;
                    row["Name"] = permissionItem.MenuItem.Name;
                    row["TextValue"] = permissionItem.MenuItem.TextValue;
                    row["IdMenuPage"] = permissionItem.MenuItem.IdMenuItemPage;
                    row["AspxPage"] = permissionItem.MenuItem.AspxPage;
                    row["WinPath"] = permissionItem.MenuItem.WinPath;

                    dsPermission.Tables["Menu"].Rows.Add(row);
                }

                dsPermission.DataSetName = "Menus";
                DataRelation relation = new DataRelation("ParentChild",
                        dsPermission.Tables["Menu"].Columns["IdMenu"],
                        dsPermission.Tables["Menu"].Columns["IDParentMenu"], false);

                relation.Nested = true;
                dsPermission.Relations.Add(relation);
            }

            return dsPermission;
        }

        private String GetXML_New(int roleIndex)
        {
            List<Permission> lstPermission = permissionViewDTO.Entities.ToList();
            DataSet dsPermission = new DataSet();

            if (lstPermission != null && lstPermission.Count() > 0)
            {
                dsPermission.Tables.Add("Menu");
                dsPermission.Tables["Menu"].Columns.Add("IdPermission");
                dsPermission.Tables["Menu"].Columns.Add("IdMenu");
                dsPermission.Tables["Menu"].Columns.Add("IDParentMenu");
                dsPermission.Tables["Menu"].Columns.Add("Name");
                dsPermission.Tables["Menu"].Columns.Add("TextValue");
                dsPermission.Tables["Menu"].Columns.Add("IdMenuPage");
                dsPermission.Tables["Menu"].Columns.Add("AspxPage");
                dsPermission.Tables["Menu"].Columns.Add("WinPath");
                //

                foreach (Permission permissionItem in lstPermission)
                {
                    DataRow row = dsPermission.Tables["Menu"].NewRow();

                    //Comentado para Mostrar todo los menus de la web
                    //30-07-2015
                    //if (permissionItem.MenuItem.IDParentMenuItem != 1)
                    //{
                    row["IdPermission"] = permissionItem.Id;
                    row["IdMenu"] = permissionItem.MenuItem.Id;
                    row["IDParentMenu"] = permissionItem.MenuItem.IDParentMenuItem;
                    row["Name"] = permissionItem.MenuItem.Name;
                    row["TextValue"] = permissionItem.MenuItem.TextValue;
                    row["IdMenuPage"] = permissionItem.MenuItem.IdMenuItemPage;
                    row["AspxPage"] = permissionItem.MenuItem.AspxPage;
                    row["WinPath"] = permissionItem.MenuItem.WinPath;

                    dsPermission.Tables["Menu"].Rows.Add(row);
                    //}
                }

                dsPermission.DataSetName = "Menus";
                DataRelation relation = new DataRelation("ParentChild",
                        dsPermission.Tables["Menu"].Columns["IdMenu"],
                        dsPermission.Tables["Menu"].Columns["IDParentMenu"], false);

                relation.Nested = true;
                dsPermission.Relations.Add(relation);
            }

            return dsPermission.GetXml();
        }

        private String GetXML(int roleIndex)
        {
            List<Permission> lstPermission = roleViewDTO.Entities[roleIndex].PermissionList;
            DataSet dsPermission = new DataSet();

            if (lstPermission != null && lstPermission.Count() > 0)
            {
                dsPermission.Tables.Add("Menu");
                dsPermission.Tables["Menu"].Columns.Add("IdPermission");
                dsPermission.Tables["Menu"].Columns.Add("IdMenu");
                dsPermission.Tables["Menu"].Columns.Add("IDParentMenu");
                dsPermission.Tables["Menu"].Columns.Add("Name");
                dsPermission.Tables["Menu"].Columns.Add("TextValue");
                dsPermission.Tables["Menu"].Columns.Add("IdMenuPage");
                dsPermission.Tables["Menu"].Columns.Add("AspxPage");
                dsPermission.Tables["Menu"].Columns.Add("WinPath");
                //

                foreach (Permission permissionItem in lstPermission)
                {
                    DataRow row = dsPermission.Tables["Menu"].NewRow();

                    //Comentado para Mostrar todo los menus de la web
                    //30-07-2015
                    //if (permissionItem.MenuItem.IDParentMenuItem != 1)
                    //{
                    row["IdPermission"] = permissionItem.Id;
                    row["IdMenu"] = permissionItem.MenuItem.Id;
                    row["IDParentMenu"] = permissionItem.MenuItem.IDParentMenuItem;
                    row["Name"] = permissionItem.MenuItem.Name;
                    row["TextValue"] = permissionItem.MenuItem.TextValue;
                    row["IdMenuPage"] = permissionItem.MenuItem.IdMenuItemPage;
                    row["AspxPage"] = permissionItem.MenuItem.AspxPage;
                    row["WinPath"] = permissionItem.MenuItem.WinPath;

                    dsPermission.Tables["Menu"].Rows.Add(row);
                    //}
                }

                dsPermission.DataSetName = "Menus";
                DataRelation relation = new DataRelation("ParentChild",
                        dsPermission.Tables["Menu"].Columns["IdMenu"],
                        dsPermission.Tables["Menu"].Columns["IDParentMenu"], false);

                relation.Nested = true;
                dsPermission.Relations.Add(relation);
            }

            return dsPermission.GetXml();
        }

        /// <summary>
        /// Persiste los cambios realizados a la lista de Permisos del Rol seleccionado
        /// </summary>
        protected void SaveChangesRole(int rowIndex)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    Binaria.WMSTek.Framework.Entities.Profile.MenuItem menuItem;

                    // Setea estructura del Menu
                    string chkId = string.Empty;
                    List<string> arrMenus = new List<string>();

                    // Carga la estructura de menu del Rol seleccionado
                    DataSet ds = GetDataSet_New(rowIndex);

                    // Limpia la lista actual de Permisos
                    int idRole = int.Parse(this.ddlRole.SelectedValue);

                    if (roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList != null)
                    {
                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList.Clear();
                    }
                    else
                    {
                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList = new List<Permission>();
                    }
                    //roleViewDTO.Entities[ddlRole.SelectedIndex].PermissionList.Clear();

                    // Si hay opciones de Menu para guardar ...
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow parentRow in ds.Tables[0].Rows)
                        {
                            // Primer nivel (sin 'Parent')
                            if (parentRow.GetParentRows("ParentChild").Count() == 0)
                            {
                                // Busca checkbox 'Parent'
                                chkId = "chk_" + parentRow["IdMenu"];

                                CheckBox chk = (CheckBox)phControls.FindControl(chkId);

                                if (chk != null)
                                {
                                    if (chk.Checked)
                                    {
                                        menuItem = new Binaria.WMSTek.Framework.Entities.Profile.MenuItem();
                                        menuItem.Id = (Convert.ToInt32(parentRow["IdMenu"]));
                                        menuItem.AspxPage = parentRow["AspxPage"].ToString();

                                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList.Add(new Permission(menuItem));

                                        // Si el menuItem esta asociado a un pagina aspx,
                                        // agrega el idMenu de la pagina a la lista de permisos a crear
                                        if (parentRow["IdMenuPage"].ToString() != "0")
                                        {
                                            menuItem = new Binaria.WMSTek.Framework.Entities.Profile.MenuItem();
                                            menuItem.Id = Convert.ToInt32(parentRow["IdMenuPage"]);
                                            menuItem.AspxPage = parentRow["AspxPage"].ToString();

                                            roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList.Add(new Permission(menuItem));
                                        }
                                    }
                                }
                            }

                            if (parentRow.GetChildRows("ParentChild").Count() > 0)
                                LoopSaveMenuTree(parentRow.GetChildRows("ParentChild"), 1);
                        }
                    }

                    foreach (var item in roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList)
                    {
                        if (!string.IsNullOrEmpty(item.MenuItem.AspxPage.Trim()))
                            arrMenus.Add(item.MenuItem.AspxPage.Trim());
                    }


                    if (roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).RoleModule == null)
                    {
                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).roleModule = new RoleModule();
                    }

                    roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).roleModule.Module = new Module(int.Parse(this.ddlRoleModule.SelectedValue));
                    roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).roleModule.Module.Name = this.ddlRoleModule.SelectedItem.Text;


                    if (roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users == null)
                    {
                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users = new List<User>();
                    }
                    else
                    {
                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users.Clear();
                    }

                    foreach (var item in userAsigRolViewDTO.Entities)
                    {
                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users.Add(item);
                    }

                    //Si el modulo elegido es RF se debe validar que solo se pueda seleccionar UNA opcion del Menu
                    //if (Convert.ToInt16(EnumModule.RF) == Convert.ToInt16(this.ddlRoleModule.SelectedValue))
                    //{
                    //    if (arrMenus.Distinct().Count() > 1)
                    //    {
                    //        isValidViewDTO = false;
                    //        roleViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.Permisssion.SelectedMenusNro, context));
                    //        this.Master.ucError.ShowError(roleViewDTO.Errors);
                    //    }
                    //    else
                    //    {
                    //        roleViewDTO = iProfileMGR.MaintainRolePermission(roleViewDTO.Entities.First(w => w.Id.Equals(idRole)), context);
                    //    }
                    //}
                    //else
                    //{

                    DropDownList ddlFilterWarehouse = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
                    roleViewDTO = iProfileMGR.MaintainRolePermission(roleViewDTO.Entities.First(w => w.Id.Equals(idRole)), int.Parse(ddlFilterWarehouse.SelectedValue), context);

                    //}

                    if (roleViewDTO.hasError())
                        UpdateSession(true);
                    else
                    {
                        crud = true;
                        ucStatus.ShowMessage(roleViewDTO.MessageStatus.Message);

                        UpdateSession(false);
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        //PopulateData();
                        //SetMenuTree_New(rowIndex);
                        LoadUsers_New(rowIndex, Convert.ToInt16(ddlRole.SelectedValue));
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
        /// Persiste los cambios realizados a la lista de Permisos del Rol seleccionado
        /// </summary>
        protected void SaveChangesUser(int userIndex, int rowIndex)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    roleViewDTO = iProfileMGR.MaintainRoleUser(userViewDTO.Entities[userIndex], context);

                    if (roleViewDTO.hasError())
                        UpdateSession(true);
                    else
                    {
                        crud = true;
                        ucStatus.ShowMessage(roleViewDTO.MessageStatus.Message);

                        UpdateSession(false);
                    }

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        //PopulateData();
                        SetMenuTree(rowIndex);
                        //  LoadUsers(rowIndex, Convert.ToInt16(ddlRole.SelectedValue));
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
        /// Agrega el Usuario seleccionado a la grilla de Usuarios Asignados (panel superior)
        /// </summary>       

        protected void AddUser_New(int rolIndex)
        {
            if (ddlUsers.SelectedIndex > 0)
            {
                User user = new User(Convert.ToInt16(ddlUsers.SelectedValue));
                user.FirstName = ddlUsers.SelectedItem.Text.Substring(0, ddlUsers.SelectedItem.Text.LastIndexOf(' '));
                user.LastName = ddlUsers.SelectedItem.Text.Substring(ddlUsers.SelectedItem.Text.LastIndexOf(' '));

                // Si es el primer usuario a agregar, crea la lista
                int idRole = int.Parse(ddlRole.SelectedValue);
                if (userAsigRolViewDTO.Entities == null)
                {
                    userAsigRolViewDTO.Entities = new List<User>();
                }

                userAsigRolViewDTO.Entities.Add(user);

                grdUsers.DataSource = userAsigRolViewDTO.Entities;
                grdUsers.DataBind();

                if (roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users == null)
                {
                    roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users = new List<User>();
                }
                else
                {
                    roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users.Clear();
                }

                foreach (var item in userAsigRolViewDTO.Entities)
                {
                    roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users.Add(item);
                }

                // Quita el Usuario seleccionado de la lista de Usuarios a Asignar (drop-down list)
                ddlUsers.Items.RemoveAt(ddlUsers.SelectedIndex);
            }
        }

        /// <summary>
        /// Agrega el Rol seleccionado a la grilla de Roles Asignados (panel inferior)
        /// </summary>
        protected void AddRole(int userIndex)
        {
            if (ddlRole2.SelectedIndex > 0)
            {
                Role role = new Role(Convert.ToInt16(ddlRole2.SelectedValue));
                role.Name = ddlRole2.SelectedItem.Text;

                // Si es el primer rol a agregar, crea la lista
                if (userViewDTO.Entities[userIndex].Roles == null) userViewDTO.Entities[userIndex].Roles = new List<Role>();

                //GenericViewDTO<RoleModule> roleModuleAuxViewDTO2 = new GenericViewDTO<RoleModule>();
                var roleModuleAuxViewDTO2 = iProfileMGR.FindRoleModuleByIdRole(Convert.ToInt16(ddlRole2.SelectedValue), context);

                GenericViewDTO<RoleModule> roleModuleAuxViewDTO = new GenericViewDTO<RoleModule>();
                roleModuleAuxViewDTO = iProfileMGR.FindRoleModuleByIdModuleIdUser(Convert.ToInt16(roleModuleAuxViewDTO2.Entities[0].Module.Id), Convert.ToInt16(userViewDTO.Entities[userIndex].Id), context);

                bool denegar = false;

                if (roleModuleAuxViewDTO != null && roleModuleAuxViewDTO.Entities.Count > 0)
                {
                    this.Master.ucDialog.ShowAlert(lblTitle.Text, lblAlreadyAssignedUser.Text, "");
                    return;
                }
                else
                {

                    foreach (var i in userViewDTO.Entities[userIndex].Roles)
                    {
                        if (iProfileMGR.FindRoleModuleByIdRole(Convert.ToInt16(i.Id), context).Entities[0].Module.Id == iProfileMGR.FindRoleModuleByIdRole(Convert.ToInt16(ddlRole2.SelectedValue), context).Entities[0].Module.Id)
                        {
                            denegar = true;
                        }
                    }

                    if (!denegar)
                    {
                        userViewDTO.Entities[userIndex].Roles.Add(role);
                        grdRoles.DataSource = userViewDTO.Entities[userIndex].Roles;
                        grdRoles.DataBind();
                        // Quita el Rol seleccionado de la lista de Roles a Asignar (drop-down list)
                        ddlRole2.Items.RemoveAt(ddlRole2.SelectedIndex);
                    }
                    else
                    {
                        this.Master.ucDialog.ShowAlert(lblTitle.Text, lblAlreadyAssignedUser.Text, "");
                        return;
                    }

                }

            }
        }

        /// <summary>
        /// Quita el Usuario seleccionado de la grilla de Usuarios Asignados (panel superior)
        /// </summary>
        protected void RemoveUser(int rolIndex, int userIndex)
        {
            int idRole = int.Parse(this.ddlRole.SelectedValue);
            string userName = userAsigRolViewDTO.Entities[userIndex].FirstName + " " + userAsigRolViewDTO.Entities[userIndex].LastName;
            ddlUsers.Items.Add(new ListItem((userName), userAsigRolViewDTO.Entities[userIndex].Id.ToString()));
            //string userName = roleViewDTO.Entities..Users[userIndex].FirstName + " " + roleViewDTO.Entities[rolIndex].Users[userIndex].LastName;
            //ddlUsers.Items.Add(new ListItem((userName), roleViewDTO.Entities[rolIndex].Users[userIndex].Id.ToString()));

            userAsigRolViewDTO.Entities.RemoveAt(userIndex);

            if (roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users == null)
            {
                roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users = new List<User>();
            }
            else
            {
                roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users.Clear();
            }

            foreach (var item in userAsigRolViewDTO.Entities)
            {
                roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).Users.Add(item);
            }

            grdUsers.DataSource = userAsigRolViewDTO.Entities;
            grdUsers.DataBind();
        }

        /// <summary>
        /// Quita el Usuario seleccionado de la grilla de Usuarios Asignados (panel inferior)
        /// </summary>
        protected void RemoveRole(int userIndex, int roleIndex)
        {
            Role role = new Role();
            role.Id = userViewDTO.Entities[userIndex].Roles[roleIndex].Id;
            role.Name = userViewDTO.Entities[userIndex].Roles[roleIndex].Name;

            ddlRole2.Items.Add(new ListItem((role.Name), role.Id.ToString()));

            userViewDTO.Entities[userIndex].Roles.RemoveAt(roleIndex);
            grdRoles.DataSource = userViewDTO.Entities[userIndex].Roles;
            grdRoles.DataBind();
        }

        protected void LoopSaveMenuTree(DataRow[] childRows, int level)
        {
            string chkId = string.Empty;
            Binaria.WMSTek.Framework.Entities.Profile.MenuItem menuItem;

            foreach (DataRow childRow in childRows)
            {
                // Crea checkbox 'Child'
                //chkId = "chk" + childRow["IdPermission"] + "_" + childRow["IdMenu"]; 
                chkId = "chk_" + childRow["IdMenu"];

                CheckBox chk = (CheckBox)phControls.FindControl(chkId);
                int idRole = int.Parse(this.ddlRole.SelectedValue);

                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        menuItem = new Binaria.WMSTek.Framework.Entities.Profile.MenuItem();
                        menuItem.Id = Convert.ToInt32(childRow["IdMenu"]);
                        menuItem.AspxPage = childRow["AspxPage"].ToString();

                        roleViewDTO.Entities.First(w => w.Id.Equals(idRole)).PermissionList.Add(new Permission(menuItem));

                        // Si el menuItem esta asociado a un pagina aspx,
                        // agrega el idMenu de la pagina a la lista de permisos a crear
                        if (childRow["IdMenuPage"].ToString() != "0")
                        {
                            menuItem = new Binaria.WMSTek.Framework.Entities.Profile.MenuItem();
                            menuItem.Id = Convert.ToInt32(childRow["IdMenuPage"]);
                            menuItem.AspxPage = childRow["AspxPage"].ToString();

                            roleViewDTO.Entities[ddlRole.SelectedIndex].PermissionList.Add(new Permission(menuItem));
                        }
                    }
                }

                // Si tiene hijos, los recorre
                if (childRow.GetChildRows("ParentChild").Count() > 0)
                    LoopSaveMenuTree(childRow.GetChildRows("ParentChild"), level + 1);
            }
        }

        #endregion



        private void SetMenuTreeNew(int rowIndex, string idMenuObject)
        {
            // Setea estructura del Menu
            string chkId = string.Empty;
            string chkIdNoCheck = string.Empty;
            // Carga la estructura de menu del Rol seleccionado
            DataSet ds = GetDataSet_New(rowIndex);

            // Si hay opciones de Menu para mostrar...
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow parentRow in ds.Tables[0].Rows)
                {
                    // Busca checkbox 'Parent'
                    chkId = "chk_" + parentRow["IdMenu"];

                    if (chkId == idMenuObject)
                    {
                        string winPath = parentRow["WinPath"].ToString().Trim();
                        string idParentMenu = parentRow["IDParentMenu"].ToString().Trim();
                        string idMenu = parentRow["IdMenu"].ToString().Trim();
                        string aspxPage = parentRow["AspxPage"].ToString().Trim();

                        CheckBox chk = (CheckBox)phControls.FindControl(chkId);

                        if (chk != null && chk.Checked && int.Parse(idParentMenu) > 1)
                        {
                            LoopSetMenuTreeNew(ds, int.Parse(idParentMenu), 0, true);
                            break;
                        }
                        else if (chk != null && !chk.Checked)
                        {
                            if (string.IsNullOrEmpty(aspxPage))
                            {
                                foreach (DataRow parentRowNoCheck in ds.Tables[0].Rows)
                                {
                                    string idParentMenuNoCheck = parentRowNoCheck["IDParentMenu"].ToString().Trim();
                                    string idMenuNoCheck = parentRowNoCheck["IdMenu"].ToString().Trim();

                                    if (idParentMenuNoCheck == idMenu)
                                    {
                                        //Busca checkbox
                                        chkIdNoCheck = "chk_" + parentRowNoCheck["IdMenu"];
                                        CheckBox chkNoCheck = (CheckBox)phControls.FindControl(chkIdNoCheck);
                                        if (chkNoCheck != null)
                                        {
                                            chkNoCheck.Checked = false;
                                        }

                                        foreach (DataRow rowNoCheck in ds.Tables[0].Rows)
                                        {
                                            string idParentMenuNoCheckFinal = rowNoCheck["IDParentMenu"].ToString().Trim();

                                            if (idParentMenuNoCheckFinal == idMenuNoCheck)
                                            {
                                                //Busca checkbox
                                                chkIdNoCheck = "chk_" + rowNoCheck["IdMenu"];
                                                CheckBox chkNoCheckFinal = (CheckBox)phControls.FindControl(chkIdNoCheck);
                                                if (chkNoCheckFinal != null)
                                                {
                                                    chkNoCheckFinal.Checked = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                string script = "document.getElementById('ctl00_MainContent_hsVertical_leftPanel_ctl01_" + chkId + "').autofocus;";
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", script, true);
            }
        }


        protected void LoopSetMenuTreeNew(DataSet ds, int idParentMenu, int idMenu, bool check)
        {
            try
            {
                string chkId = string.Empty;

                foreach (DataRow parentRow in ds.Tables[0].Rows)
                {
                    // Crea checkbox 'Child'

                    string idParentMenuNew = parentRow["IDParentMenu"].ToString().Trim();
                    int idMenuNew = int.Parse(parentRow["IdMenu"].ToString());

                    if (int.Parse(idParentMenuNew) == 0 && check)
                    {
                        string a = "";
                        break;
                    }
                    else
                        if (idMenuNew == idParentMenu && idMenu != idMenuNew)
                    {
                        chkId = "chk_" + parentRow["IdMenu"];
                        CheckBox chk = (CheckBox)phControls.FindControl(chkId);
                        chk.Checked = check;
                        if (check)
                        {
                            LoopSetMenuTreeNew(ds, int.Parse(idParentMenuNew), idMenuNew, check);
                        }
                        else
                        {
                            LoopSetMenuTreeNew(ds, idMenuNew, idMenuNew, check);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                roleViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(roleViewDTO.Errors);
            }

        }


    }
}
