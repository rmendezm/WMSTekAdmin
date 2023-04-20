using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.LogisticsResources
{
    public partial class WarehouseMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Warehouse> warehouseViewDTO = new GenericViewDTO<Warehouse>();
        private bool isValidViewDTO = false;
        private bool isNew = false;
        

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

        /// <summary>
        /// Propiedad que devuelve el id del centro
        /// </summary>
        /// 
        #region Propiedades

        public int IdCountry
        {
            get { return (int)(ViewState["IdCountry"] ?? -1); }
            set { ViewState["IdCountry"] = value; }
        }

        public int IdState
        {
            get { return (int)(ViewState["IdState"] ?? -1); }
            set { ViewState["IdState"] = value; }
        }

        public int IdCity
        {
            get { return (int)(ViewState["IdCity"] ?? -1); }
            set { ViewState["IdCity"] = value; }
        }

#endregion

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

                        if (isValidViewDTO)
                        {
                            IdCountry = 1;
                            IdState = 1;
                            IdCity = 1;
                            base.FindAllPlaces();
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.WarehouseMgr.WarehouseList))
                    {
                        warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.WarehouseMgr.WarehouseList];
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
            }
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        /// 
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
        //        warehouseViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(warehouseViewDTO.Errors);
        //    }
        //}

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el pais... cambiara el estado y la ciudad
                isNew = true;
                base.ConfigureDDlState(this.ddlState, isNew, IdState, Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                divEditNew.Visible = true;
                mpeWarehouse.Show();
            }
            catch (Exception ex)
            {
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el estado, solo cambia la ciudad.
                isNew = true;
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                divEditNew.Visible = true;
                mpeWarehouse.Show();
            }
            catch (Exception ex)
            {
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
                warehouseViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
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
        //            btnEdit.ToolTip = this.lblToolTipEdit.Text;
        //        }
        //        if (btnDelete != null)
        //        {
        //            btnDelete.ToolTip = this.lblToolTipDelete.Text;
        //        }
        //    }
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
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
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
                warehouseViewDTO.ClearError();
                warehouseViewDTO.MessageStatus.Message = string.Empty;
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            warehouseViewDTO = iLayoutMGR.FindAllWarehouse(context);

            if (!warehouseViewDTO.hasError() && warehouseViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.WarehouseMgr.WarehouseList, warehouseViewDTO);
                Session.Remove(WMSTekSessions.Shared.WarehouseList);
                isValidViewDTO = true;

                //Muestra el mensaje de status
                if (!crud)
                    ucStatus.ShowMessage(warehouseViewDTO.MessageStatus.Message);

            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(warehouseViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.nameLabel = lblFilterName.Text;

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

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!warehouseViewDTO.hasConfigurationError() && warehouseViewDTO.Configuration != null && warehouseViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, warehouseViewDTO.Configuration);

            grdMgr.DataSource = warehouseViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(warehouseViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            base.ConfigureDDlCountry(this.ddlCountry, isNew, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlState, isNew, IdState, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, IdState, IdCountry, this.Master.EmptyRowText);
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
            // Editar centro
            if (mode == CRUD.Update)
            {
                IdCountry = warehouseViewDTO.Entities[index].Country.Id;
                IdState = warehouseViewDTO.Entities[index].State.Id;
                IdCity = warehouseViewDTO.Entities[index].City.Id;
                isNew = false;

                PopulateLists();

                //Recupera los datos de la entidad a editar

                hidEditId.Value = warehouseViewDTO.Entities[index].Id.ToString();

                // TODO: ver propiedad 'required' para un drop-down list
                //Carga controles
                txtName.Text = warehouseViewDTO.Entities[index].Name;
                txtCode.Text = warehouseViewDTO.Entities[index].Code;
                txtShortName.Text = warehouseViewDTO.Entities[index].ShortName;
                txtAddress1.Text = warehouseViewDTO.Entities[index].Address1;
                txtAddress2.Text = warehouseViewDTO.Entities[index].Address2;
                txtEmail.Text = warehouseViewDTO.Entities[index].Email;
                ddlCountry.SelectedValue = (warehouseViewDTO.Entities[index].Country.Id).ToString();
                ddlState.SelectedValue = (warehouseViewDTO.Entities[index].State.Id).ToString();
                ddlCity.SelectedValue = (warehouseViewDTO.Entities[index].City.Id).ToString();
                txtPhone1.Text = warehouseViewDTO.Entities[index].Phone1;
                txtPhone2.Text = warehouseViewDTO.Entities[index].Phone2;
                txtFax1.Text = warehouseViewDTO.Entities[index].Fax1;
                txtFax2.Text = warehouseViewDTO.Entities[index].Fax2;
                txtEmail.Text = warehouseViewDTO.Entities[index].Email;
                txtZipCode.Text = warehouseViewDTO.Entities[index].ZipCode;
                txtGLN.Text = warehouseViewDTO.Entities[index].GLN;

                chkCodStatus.Checked = warehouseViewDTO.Entities[index].CodStatus;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva centro
            if (mode == CRUD.Create)
            {
                // TODO: revisar
                IdCountry = 1;
                IdState = 1;
                IdCity = 1;
                isNew = true;

                PopulateLists();

                hidEditId.Value = "0";
                txtName.Text = string.Empty;
                txtCode.Text = string.Empty;
                txtShortName.Text = string.Empty;
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtPhone1.Text = string.Empty;
                txtPhone2.Text = string.Empty;
                txtFax1.Text = string.Empty;
                txtFax2.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtZipCode.Text = string.Empty;
                this.chkCodStatus.Checked = true;
                lblNew.Visible = true;
                lblEdit.Visible = false;
                this.txtCode.ReadOnly = false;
                this.txtGLN.Text = string.Empty;
            }

            if (warehouseViewDTO.Configuration != null && warehouseViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(warehouseViewDTO.Configuration, true);
                else
                {
                    base.ConfigureModal(warehouseViewDTO.Configuration, false);
                    this.txtCode.Enabled = false;
                }
            }

            divEditNew.Visible = true;
            mpeWarehouse.Show();
        }

        protected void SaveChanges()
        {
            //agrega los datos del centro
            Warehouse warehouse = new Warehouse();
            warehouse.Country = new Country();
            warehouse.State = new State();
            warehouse.City = new City();

            warehouse.Id = Convert.ToInt32(hidEditId.Value);
            warehouse.Code = txtCode.Text.Trim();
            warehouse.Name = txtName.Text;
            warehouse.ShortName = txtShortName.Text;
            warehouse.Address1 = txtAddress1.Text;
            warehouse.Address2 = txtAddress2.Text;
            warehouse.Country.Id = Convert.ToInt32(ddlCountry.SelectedValue);
            warehouse.State.Id = Convert.ToInt32(ddlState.SelectedValue);
            warehouse.City.Id = Convert.ToInt32(ddlCity.SelectedValue);
            warehouse.Phone1 = txtPhone1.Text;
            warehouse.Phone2 = txtPhone2.Text;
            warehouse.Fax1 = txtFax1.Text;
            warehouse.Fax2 = txtFax2.Text;
            warehouse.Email = txtEmail.Text;
            warehouse.ZipCode = txtZipCode.Text;
            warehouse.CodStatus = this.chkCodStatus.Checked;

            if (!string.IsNullOrEmpty(txtGLN.Text.Trim()))
            {
                string resultGLN = ValidateCodeGLN(this.txtGLN.Text.Trim());

                if (string.IsNullOrEmpty(resultGLN))
                {
                    warehouse.GLN = txtGLN.Text;
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    val.ErrorMessage = resultGLN;
                    val.ControlToValidate = "ctl00$MainContent$txtGLN";
                    val.IsValid = false;
                    val.ValidationGroup = "EditNew";
                    this.Page.Validators.Add(val);
                    revtxtGLN.IsValid = false;
                    revtxtGLN.Validate();

                    divEditNew.Visible = true;
                    mpeWarehouse.Show();
                    return;
                }
            }

            if (hidEditId.Value == "0")
                warehouseViewDTO = iLayoutMGR.MaintainWarehouse(CRUD.Create, warehouse, context);
            else
                warehouseViewDTO = iLayoutMGR.MaintainWarehouse(CRUD.Update, warehouse, context);


            divEditNew.Visible = false;
            mpeWarehouse.Hide();

            if (warehouseViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                mpeWarehouse.Show();
            }
            else
            {
                //habilita para mostrar el msj status
                crud = true;
                ucStatus.ShowMessage(warehouseViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            warehouseViewDTO = iLayoutMGR.MaintainWarehouse(CRUD.Delete, warehouseViewDTO.Entities[index], context);

            if (warehouseViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //habilita para mostrar el msj status
                crud = true;
                ucStatus.ShowMessage(warehouseViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }
        #endregion
   }
}
