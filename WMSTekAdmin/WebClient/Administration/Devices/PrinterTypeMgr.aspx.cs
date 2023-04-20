using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class PrinterTypeMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<PrinterType> printerTypeViewDTO = new GenericViewDTO<PrinterType>();
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

                    if (ValidateSession(WMSTekSessions.PrinterTypeMgr.PrinterTypeList))
                    {
                        printerTypeViewDTO = (GenericViewDTO<PrinterType>)Session[WMSTekSessions.PrinterTypeMgr.PrinterTypeList];
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
        //        printerViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(printerViewDTO.Errors);
        //    }
        //}

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    grdMgr.Enabled = true;
                    SaveChanges();
                }
            }
            catch (Exception ex)
            {
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Calcula la posicion en el ViewDTO de la fila a editar
                    int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                    ShowModal(editIndex, CRUD.Update);
                }
            }
            catch (Exception ex)
            {
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Calcula la posicion en el ViewDTO de la fila a eliminar
                    int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                    DeleteRow(deleteIndex);
                }
            }
            catch (Exception ex)
            {
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                printerTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
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
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
                printerTypeViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            printerTypeViewDTO = iDeviceMGR.FindAllPrinterType(context);

            if (!printerTypeViewDTO.hasError() && printerTypeViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.PrinterTypeMgr.PrinterTypeList, printerTypeViewDTO);
                //Session.Remove(WMSTekSessions.Shared.PrinterList);

                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(printerTypeViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(printerTypeViewDTO.Errors);
            }
        }


        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!printerTypeViewDTO.hasConfigurationError() && printerTypeViewDTO.Configuration != null && printerTypeViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, printerTypeViewDTO.Configuration);

            grdMgr.DataSource = printerTypeViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(printerTypeViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            //this.Master.ucMainFilter.warehouseVisible = false;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;

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

        protected void ReloadData()
        {
            crud = false;
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
            // Editar Impresora
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = printerTypeViewDTO.Entities[index].Id.ToString();

                this.txtPrinterTypeCode.Text = printerTypeViewDTO.Entities[index].Code;
                this.txtPrinterTypeName.Text = printerTypeViewDTO.Entities[index].Name;
                this.chkStatus.Checked = printerTypeViewDTO.Entities[index].Status;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Impresora
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";
                this.txtPrinterTypeCode.Text = string.Empty;
                this.txtPrinterTypeName.Text = string.Empty;
                this.chkStatus.Checked = false;
         
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (printerTypeViewDTO.Configuration != null && printerTypeViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(printerTypeViewDTO.Configuration, true);
                else
                    base.ConfigureModal(printerTypeViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            PrinterType printerType = new PrinterType();
            printerType.Id = (Convert.ToInt32(hidEditId.Value));
            printerType.Code = this.txtPrinterTypeCode.Text.Trim();
            printerType.Name = this.txtPrinterTypeName.Text.Trim();
            printerType.Status = this.chkStatus.Checked;   

            if (hidEditId.Value == "0")
                printerTypeViewDTO = iDeviceMGR.MaintainPrinterType(CRUD.Create, printerType, context);
            else
                printerTypeViewDTO = iDeviceMGR.MaintainPrinterType(CRUD.Update, printerType, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (printerTypeViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(printerTypeViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            printerTypeViewDTO = iDeviceMGR.MaintainPrinterType(CRUD.Delete, printerTypeViewDTO.Entities[index], context);

            if (printerTypeViewDTO.hasError())
                UpdateSession(true);
            else
            {
                ucStatus.ShowMessage(printerTypeViewDTO.MessageStatus.Message);
                crud = true;
                UpdateSession(false);
            }
        }
        #endregion
    }
}
