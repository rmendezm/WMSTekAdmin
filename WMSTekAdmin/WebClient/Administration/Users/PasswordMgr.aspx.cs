using System;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Users
{
    public partial class PasswordMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();
        private MiscUtils util;
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

                    if (ValidateSession(WMSTekSessions.Shared.UserList))
                    {
                        userViewDTO = (GenericViewDTO<User>)Session[WMSTekSessions.Shared.UserList];
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
        //        userViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
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
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            base.grdMgr_RowDataBound(sender, e);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Carga los textos del tool tip
                ImageButton btnReset = e.Row.FindControl("btnReset") as ImageButton;
                if (btnReset != null)
                {
                    btnReset.ToolTip = this.lblToolTipReset.Text;
                }
            }
        }

        #endregion

        #region "Eventos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "ResetPassword";

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
                this.Master.ucError.ShowError(userViewDTO.Errors);
                userViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            userViewDTO = iProfileMGR.FindAllUser(context);

            if (!userViewDTO.hasError() && userViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.Shared.UserList, userViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(userViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        // Configuracion inicial del filtro de busqueda
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;
            this.Master.ucMainFilter.descriptionLabel = lblFilterDescription.Text;

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
            if (!userViewDTO.hasConfigurationError() && userViewDTO.Configuration != null && userViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, userViewDTO.Configuration);

            grdMgr.DataSource = userViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(userViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
            if (userViewDTO.Configuration != null && userViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(userViewDTO.Configuration, true);
                else
                    base.ConfigureModal(userViewDTO.Configuration, false);
            }
            // Editar usuario
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = userViewDTO.Entities[index].Id.ToString();

                //el dato IsBaseUser es de solo lectura
                hidIsBaseUser.Value = userViewDTO.Entities[index].IsBaseUser.ToString();

                //Carga controles
                txtUserName.Text = userViewDTO.Entities[index].UserName;

                //carga el indice del enum
                //int i = Convert.ToInt16(CfgParameterName.DefaultPassword);
                //asigna el valor que tiene la variable en el indice i
                txtPassword.Text = string.Empty;
                txtPasswordRepeat.Text = string.Empty;
                txtFirstName.Text = userViewDTO.Entities[index].FirstName;
                txtLastName.Text = userViewDTO.Entities[index].LastName;
                this.txtFirstName.Enabled = false;
                this.txtLastName.Enabled = false;
                this.txtUserName.Enabled = false;

                lblEdit.Visible = true;

             }

            // Nueva usuario
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";
                txtUserName.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtFirstName.Text = string.Empty;
                txtLastName.Text = string.Empty;
                lblEdit.Visible = false;
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
            User user = new User();

            if (index != -1)
            {
                hidEditId.Value = userViewDTO.Entities[index].Id.ToString();

                user.Password = GetCfgParameter(CfgParameterName.DefaultPassword.ToString());
            }
            else
            {
                user.Password = txtPassword.Text;
            }

            var passPolicyErrors = PasswordPolicy.IsValid(user.Password);

            if (passPolicyErrors != null && passPolicyErrors.Count > 0)
            {
                var errorMsg = string.Join("<br>", passPolicyErrors.ToArray());

                this.Master.ucDialog.ShowAlert(lblPoliciyPasswordTitle.Text, errorMsg, string.Empty);
                return;
            }

            user.Password = MiscUtils.Encrypt(user.Password);

           //agrega los datos del usuario

            user.Id = Convert.ToInt32(hidEditId.Value);

                // Editar Password
                userViewDTO = iProfileMGR.UpdatePassword(user, context);

                divEditNew.Visible = false;
                modalPopUp.Hide();

                if (userViewDTO.hasError())
                    UpdateSession(true);
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(userViewDTO.MessageStatus.Message);

                    UpdateSession(false);
                }
        }

        #endregion


   }
}
