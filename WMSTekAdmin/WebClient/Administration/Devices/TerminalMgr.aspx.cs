using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Display;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class TerminalMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<Terminal> terminalViewDTO = new GenericViewDTO<Terminal>();
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
                        PopulateLists();
                    }

                    if (ValidateSession(WMSTekSessions.TerminalMgr.TerminalList))
                    {
                        terminalViewDTO = (GenericViewDTO<Terminal>)Session[WMSTekSessions.TerminalMgr.TerminalList];
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
        //        terminalViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(terminalViewDTO.Errors);
        //    }
        //}

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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;
                    ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;

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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
                terminalViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
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
        /// Carga en sesion la lista de la Entidad
        /// </summary>
        /// <param name="showError">Determina si se mostrara el error producido en una operacion anterior</param>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
                terminalViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            

            terminalViewDTO = iDeviceMGR.FindAllTerminal(context);

            if (!terminalViewDTO.hasError() && terminalViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TerminalMgr.TerminalList, terminalViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(terminalViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(terminalViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del Filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
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
            grdMgr.DataSource = null;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!terminalViewDTO.hasConfigurationError() && terminalViewDTO.Configuration != null && terminalViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, terminalViewDTO.Configuration);
            
            grdMgr.DataSource = terminalViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(terminalViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            // Configura VISIBILIDAD de las columnas de la grilla
            if (!terminalViewDTO.hasConfigurationError() && terminalViewDTO.Configuration != null && terminalViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, terminalViewDTO.Configuration);
        }

        protected void PopulateLists()
        {
            base.LoadDisplayType(ddlDisplayType, true, this.Master.EmptyRowText);
        }

        protected void ReloadData()
        {
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
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
            // Editar entidad
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = terminalViewDTO.Entities[index].Id.ToString();

                chkCodStatus.Checked = terminalViewDTO.Entities[index].CodStatus;
                txtName.Text = terminalViewDTO.Entities[index].Name;
                txtType.Text = terminalViewDTO.Entities[index].Type;
                ddlDisplayType.SelectedValue = (terminalViewDTO.Entities[index].DisplayType.Id).ToString();
                txtCode.Text = terminalViewDTO.Entities[index].Code;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";

                chkCodStatus.Checked = true;
                txtName.Text = string.Empty;
                txtType.Text = string.Empty;
                txtCode.Text = string.Empty;
                ddlDisplayType.SelectedValue = "-1";

                lblNew.Visible = true;
                lblEdit.Visible = false;


            }
            if (terminalViewDTO.Configuration != null && terminalViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(terminalViewDTO.Configuration, true);
                else
                    base.ConfigureModal(terminalViewDTO.Configuration, false);
            }

            // Muestra ventana modal
            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        /// <summary>
        /// Persiste los cambios realizados en la entidad
        /// </summary>
        /// <param name="index"></param>
        protected void SaveChanges()
        {
            Terminal terminal = new Terminal(Convert.ToInt32(hidEditId.Value));
                
            terminal.DisplayType = new DisplayType(Convert.ToInt32(ddlDisplayType.SelectedValue));
            terminal.Code = txtCode.Text.Trim();
            terminal.Name = txtName.Text;
            terminal.Type = txtType.Text;
            terminal.CodStatus = chkCodStatus.Checked;

            if (hidEditId.Value == "0")
                terminalViewDTO = iDeviceMGR.MaintainTerminal(CRUD.Create, terminal, context);
            else
                terminalViewDTO = iDeviceMGR.MaintainTerminal(CRUD.Update, terminal, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (terminalViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(terminalViewDTO.MessageStatus.Message);

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
            

            if (ValidateTerminalStatus(index))
            {
                ErrorDTO newError = new ErrorDTO();
                newError = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Terminal.InvalidInsert.User, context));
                //newError.Title = "Información Terminal";
                //newError.Message = this.lblErrorTerminalStatus.Text;
                //newError.Level = ErrorLevel.Warning;
                                
                this.Master.ucError.ShowError(newError );
            }
            else
            {

                terminalViewDTO = iDeviceMGR.MaintainTerminal(CRUD.Delete, terminalViewDTO.Entities[index], context);

                //Actualiza la session
                if (terminalViewDTO.hasError())
                    UpdateSession(true);
                else
                {
                    //Muestra mensaje de status
                    crud = true;
                    ucStatus.ShowMessage(terminalViewDTO.MessageStatus.Message);

                    //Actualiza la session
                    UpdateSession(false);
                }
            }
        }


        private Boolean ValidateTerminalStatus(int index)
        {
            bool validateTerminal = false;

            string codeTerminal = terminalViewDTO.Entities[index].Code;

            ContextViewDTO newContexto = new ContextViewDTO();
            newContexto = context;
            //newContexto.MainFilter = this.Master.ucMainFilter.MainFilter;

            newContexto.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();
            newContexto.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Add(new FilterItem("", codeTerminal));

            GenericViewDTO<Monitor> termViewDTO = new GenericViewDTO<Monitor>();
            termViewDTO = iDeviceMGR.FindAllTerminalMonitor(newContexto);

            if (termViewDTO.Entities != null)
            {
                if (termViewDTO.Entities[0].TerminalStatus == TerminalStatus.Connected)
                {
                    validateTerminal = true;
                }
            }
            
            return validateTerminal;
        }

        #endregion
    }
}