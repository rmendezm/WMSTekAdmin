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
    public partial class CarrierMgr : BasePage
    {
        #region "Declaracion de variables"

        private GenericViewDTO<Carrier> carrierViewDTO;
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
                            base.FindAllPlaces();
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.CarrierMgr.CarrierList))
                    {
                        carrierViewDTO = (GenericViewDTO<Carrier>)Session[WMSTekSessions.CarrierMgr.CarrierList];
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el pais... cambiara el estado y la ciudad
                isNew = true;
                base.ConfigureDDlState(this.ddlState, isNew, IdState, Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                carrierViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
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
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
                carrierViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            carrierViewDTO = iWarehousingMGR.FindAllCarrier(context);

            if (!carrierViewDTO.hasError() && carrierViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CarrierMgr.CarrierList, carrierViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(carrierViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(carrierViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.ConfigureDDlCountry(this.ddlCountry, isNew, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlState, isNew, IdState, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, IdState, IdCountry, this.Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!carrierViewDTO.hasConfigurationError() && carrierViewDTO.Configuration != null && carrierViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, carrierViewDTO.Configuration);

            grdMgr.DataSource = carrierViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(carrierViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
        /// Muestra ventana modal con los datos de la entidad (transportista) a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar transportista
            if (mode == CRUD.Update)
            {
                // TODO: ver propiedad 'required' para un drop-down list
                IdCountry = carrierViewDTO.Entities[index].Country.Id;
                IdState = carrierViewDTO.Entities[index].State.Id;
                IdCity = carrierViewDTO.Entities[index].City.Id;
                isNew = false;


                //Recupera los datos de la entidad a editar
                hidEditId.Value = carrierViewDTO.Entities[index].Id.ToString();

                PopulateLists();

                txtName.Text = carrierViewDTO.Entities[index].Name;
                txtCode.Text = carrierViewDTO.Entities[index].Code;
                txtAddress1.Text = carrierViewDTO.Entities[index].Address1;
                txtAddress2.Text = carrierViewDTO.Entities[index].Address2;
                txtEmail.Text = carrierViewDTO.Entities[index].Email;
                ddlCountry.SelectedValue = (carrierViewDTO.Entities[index].Country.Id).ToString();
                ddlState.SelectedValue = (carrierViewDTO.Entities[index].State.Id).ToString();
                ddlCity.SelectedValue = (carrierViewDTO.Entities[index].City.Id).ToString();
                txtPhone.Text = carrierViewDTO.Entities[index].Phone;
                txtFax.Text = carrierViewDTO.Entities[index].Fax;
                txtEmail.Text = carrierViewDTO.Entities[index].Email;
                txtContactName.Text = carrierViewDTO.Entities[index].ContactName;
                txtOrganizationName.Text = carrierViewDTO.Entities[index].OrganizationName;
                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo transportista
            if (mode == CRUD.Create)
            {
                // TODO: revisar
                IdCountry = 1;
                IdState = 1;
                IdCity = 1;
                isNew = true;

                PopulateLists();

                hidEditId.Value = "0";
                chkStatus.Checked = true;
                txtName.Text = string.Empty;
                txtCode.Text = string.Empty;
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtFax.Text = string.Empty;
                txtEmail.Text = string.Empty;
                lblNew.Visible = true;
                lblEdit.Visible = false;
                txtContactName.Text = string.Empty;
                txtOrganizationName.Text = string.Empty;
            }

            if (carrierViewDTO.Configuration != null && carrierViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(carrierViewDTO.Configuration, true);
                else
                    base.ConfigureModal(carrierViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            //Agrega los datos del transportista
            Carrier carrier = new Carrier(Convert.ToInt32(hidEditId.Value));

            carrier.Code = txtCode.Text.Trim();
            carrier.Name = txtName.Text.Trim();
            carrier.Address1 = txtAddress1.Text.Trim();
            carrier.Address2 = txtAddress2.Text.Trim();
            carrier.Country = new Country(Convert.ToInt32(ddlCountry.SelectedValue));
            carrier.State = new State(Convert.ToInt32(ddlState.SelectedValue));
            carrier.City = new City(Convert.ToInt32(ddlCity.SelectedValue));
            carrier.Phone = txtPhone.Text.Trim();
            carrier.ContactName = txtContactName.Text.Trim();
            carrier.OrganizationName = txtOrganizationName.Text.Trim();
            carrier.Fax = txtFax.Text.Trim();
            carrier.Email = txtEmail.Text.Trim();
            carrier.Status = this.chkStatus.Checked;
           
            if (hidEditId.Value == "0")
                carrierViewDTO = iWarehousingMGR.MaintainCarrier(CRUD.Create, carrier, context);
            else
                carrierViewDTO = iWarehousingMGR.MaintainCarrier(CRUD.Update, carrier, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (carrierViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(carrierViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina el transportista
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            carrierViewDTO = iWarehousingMGR.MaintainCarrier(CRUD.Delete, carrierViewDTO.Entities[index], context);

            if (carrierViewDTO.hasError())
                UpdateSession(true);
            else
            {
                ucStatus.ShowMessage(carrierViewDTO.MessageStatus.Message);
                crud = true;
                UpdateSession(false);
            }
        }
        #endregion
    }
}

