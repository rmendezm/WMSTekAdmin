using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class VasMgr : BasePage
    {
        #region "Declaracion de variables"

        private BaseViewDTO configurationViewDTO = new BaseViewDTO();
        private GenericViewDTO<RecipeVas> recipeVasViewDTO = new GenericViewDTO<RecipeVas>();
        private GenericViewDTO<Customer> customerSearchViewDTO;
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

        public int currentPageCustomer
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
                        //UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.RecipeVasMgr.RecipeVasList))
                    {
                        recipeVasViewDTO = (GenericViewDTO<RecipeVas>)Session[WMSTekSessions.RecipeVasMgr.RecipeVasList];
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }

        protected void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            try
            {

                SearchCustomer();

                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    divLookupCustomer.Visible = true;
                    mpLookupCustomer.Show();

                    InitializePageCountCustomer();
                }
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    divGrid.Visible = true;
                    divModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
        //        recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
        //    }
        //}

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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }

        protected void grdSearchCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            //string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                //int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                //if (index != -1)
                //{
                //    LoadOutboundOrderDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);

                base.ExportToExcel(grdMgr, null, null, true);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
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
            InitializeFilterCustomer();
            InitializeLayout();

            this.tabLayout.HeaderText = this.lbltabGeneral.Text;
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
                recipeVasViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            recipeVasViewDTO = iWarehousingMGR.FindAllRecipeVas(context);

            if (!recipeVasViewDTO.hasError() && recipeVasViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RecipeVasMgr.RecipeVasList, recipeVasViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(recipeVasViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;


            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!recipeVasViewDTO.hasConfigurationError() && recipeVasViewDTO.Configuration != null && recipeVasViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, recipeVasViewDTO.Configuration);

            grdMgr.DataSource = recipeVasViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(recipeVasViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            //Carga lista de Owner
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.codeVisible = true;

            this.Master.ucMainFilter.nameLabel = this.lblNameFilter.Text;
            this.Master.ucMainFilter.codeLabel = this.lblCustomer.Text;

            // TODO: personalizar vista del Filtro
            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeFilterCustomer()
        {
            ucFilterCustomer.Initialize();
            ucFilterCustomer.BtnSearchClick += new EventHandler(btnSearchCustomer_Click);

            ucFilterCustomer.FilterCode = this.lblFilterCode.Text;
            ucFilterCustomer.FilterDescription = this.lblFilterName.Text;

        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;

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

        /// <summary>
        /// Configuracion inicial del layout
        /// </summary>
        private void InitializeLayout()
        {
            if (recipeVasViewDTO.Configuration == null)
            {
                configurationViewDTO = iConfigurationMGR.GetLayoutConfiguration("Vas_FindAll", context);
                if (!configurationViewDTO.hasConfigurationError()) recipeVasViewDTO.Configuration = configurationViewDTO.Configuration;
            }
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
            // Editar Vas
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = recipeVasViewDTO.Entities[index].Id.ToString();
                if (recipeVasViewDTO.Entities[index].Status)
                    chkStatus.Checked = true;
                else
                    chkStatus.Checked = false;
                ddlOwner.SelectedValue = recipeVasViewDTO.Entities[index].Owner.Id.ToString();
                txtName.Text = recipeVasViewDTO.Entities[index].Name;
                txtDescription.Text = recipeVasViewDTO.Entities[index].Description;
                this.txtCustomerName.Text = recipeVasViewDTO.Entities[index].Customer.Name;
                this.txtCustomerCode.Text = recipeVasViewDTO.Entities[index].Customer.Code;
                this.txtCustomerName.Enabled = false;
                this.txtCustomerCode.Enabled = true;
                this.imgBtnCustmerSearch.Enabled = true;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Vas
            if (mode == CRUD.Create)
            {
                //General
                hidEditId.Value = "0";
                chkStatus.Checked = true;
                txtName.Text = string.Empty;
                txtDescription.Text = string.Empty;
                // Selecciona owner seleccionado en el Filtro
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                this.txtCustomerName.Text = string.Empty;
                this.txtCustomerCode.Text = string.Empty;
                this.txtCustomerName.Enabled = false;
                this.txtCustomerCode.Enabled = true;
                this.imgBtnCustmerSearch.Enabled = true;

                //De la página
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (recipeVasViewDTO.Configuration != null && recipeVasViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(recipeVasViewDTO.Configuration, true);
                else
                    base.ConfigureModal(recipeVasViewDTO.Configuration, false);
            }

            divGrid.Visible = false;
            divModal.Visible = true;
        }

        protected void SaveChanges()
        {

            RecipeVas recipeVas = new RecipeVas();

            recipeVas.Name = txtName.Text.Trim();
            recipeVas.Description = txtDescription.Text.Trim();
            recipeVas.Status = chkStatus.Checked;
            recipeVas.Owner.Id = Convert.ToInt32(ddlOwner.SelectedValue);

            if (string.IsNullOrEmpty(this.txtCustomerCode.Text.Trim()))
            {
                recipeVas.Customer = new Customer();

                if (hidEditId.Value == "0")
                {
                    recipeVasViewDTO = iWarehousingMGR.MaintainRecipeVas(CRUD.Create, recipeVas, context);
                }
                else
                {
                    recipeVas.Id = Convert.ToInt32(hidEditId.Value);
                    recipeVasViewDTO = iWarehousingMGR.MaintainRecipeVas(CRUD.Update, recipeVas, context);
                }
                
                divGrid.Visible = true;
                divModal.Visible = false;

                if (recipeVasViewDTO.hasError())
                {
                    UpdateSession(true);
                    divGrid.Visible = false;
                    divModal.Visible = true;
                }
                else
                {
                    //Muestra mensaje en la barra de status
                    crud = true;
                    ucStatus.ShowMessage(recipeVasViewDTO.MessageStatus.Message);
                    //Actualiza
                    UpdateSession(false);
                }
            }
            else
            {
                GenericViewDTO<Customer> customerDTO = iWarehousingMGR.GetCustomerByCodeAndOwn(context, this.txtCustomerCode.Text.Trim(), Convert.ToInt32(ddlOwner.SelectedValue));

                if (!customerDTO.hasError() && customerDTO.Entities != null && customerDTO.Entities.Count > 0)
                {
                    recipeVas.Customer = new Customer(customerDTO.Entities[0].Id);

                    if (hidEditId.Value == "0")
                    {
                        recipeVasViewDTO = iWarehousingMGR.MaintainRecipeVas(CRUD.Create, recipeVas, context);
                    }
                    else
                    {
                        recipeVas.Id = Convert.ToInt32(hidEditId.Value);
                        recipeVasViewDTO = iWarehousingMGR.MaintainRecipeVas(CRUD.Update, recipeVas, context);
                    }
                    
                    divGrid.Visible = true;
                    divModal.Visible = false;

                    if (recipeVasViewDTO.hasError())
                    {
                        UpdateSession(true);
                        divGrid.Visible = false;
                        divModal.Visible = true;
                    }
                    else
                    {
                        //Muestra mensaje en la barra de status
                        crud = true;
                        ucStatus.ShowMessage(recipeVasViewDTO.MessageStatus.Message);
                        //Actualiza
                        UpdateSession(false);
                    }
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblMessajeCustomer.Text, "");
                    divModal.Visible = true;
                }
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            recipeVasViewDTO = iWarehousingMGR.MaintainRecipeVas(CRUD.Delete, recipeVasViewDTO.Entities[index], context);

            //Muestra mensaje de status
            crud = true;
            ucStatus.ShowMessage(recipeVasViewDTO.MessageStatus.Message);

            //Actualiza la session
            if (recipeVasViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(recipeVasViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void imgBtnCustmerSearch_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (this.ddlOwner.SelectedValue == "-1")
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblMessajeSelectedOwner.Text, "");
                    }
                    else
                    {
                        bool validCustomer = false;

                        // Busca en base de datos el Customer ingresado 
                        //if (txtCustomerCode.Text.Trim() != string.Empty)
                        //{
                        //    customerSearchViewDTO = new GenericViewDTO<Customer>();
                        //    Customer customer = new Customer();
                        //    customer.Code = this.txtCustomerCode.Text;
                        //    //customer.Name = this.txtDescription.Text;

                        //    //customerSearchViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context,Convert.ToInt32(this.ddlOwner.SelectedValue));
                        //    validCustomer = true;

                        //}

                        // Si no es válido o no se ingresó, se muestra la lista de Customers para seleccionar uno
                        if (!validCustomer)
                        {
                            ucFilterCustomer.Clear();
                            ucFilterCustomer.Initialize();

                            // Setea el filtro con el Customer ingresado
                            if (txtCustomerCode.Text.Trim() != string.Empty)
                            {
                                FilterItem filterItem = new FilterItem("%" + txtCustomerCode.Text + "%");
                                filterItem.Selected = true;
                                ucFilterCustomer.FilterItems[0] = filterItem;
                                ucFilterCustomer.LoadCurrentFilter(ucFilterCustomer.FilterItems);
                                SearchCustomer();
                            }
                            // Si no se ingresó ningún customer, no se ejecuta la búsqueda
                            else
                            {
                                ClearGridCustomer();
                            }

                            // Esto evita un bug de ajax
                            rfvSummary.Enabled = false;
                            //valSearchCustomer.Enabled = false;

                            divLookupCustomer.Visible = true;
                            mpLookupCustomer.Show();

                            InitializePageCountCustomer();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }

        private void SearchCustomer()
        {
            try
            {
                customerSearchViewDTO = new GenericViewDTO<Customer>();
                GenericViewDTO<Customer> CustomerViewDTO = new GenericViewDTO<Customer>();

                int idOwn = Convert.ToInt32(this.ddlOwner.SelectedValue);

                CustomerViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context, idOwn);

                Session.Add(WMSTekSessions.Shared.CustomerList, CustomerViewDTO);
                //grdMgr.EmptyDataText = this.Master.EmptyGridText;
                InitializeGridCustomer();
                grdSearchCustomers.DataSource = CustomerViewDTO.Entities;
                grdSearchCustomers.DataBind();
                //}
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }

        private void InitializeGridCustomer()
        {
            grdSearchCustomers.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchCustomers.EmptyDataText = this.Master.EmptyGridText;
        }

        private void ClearGridCustomer()
        {
            Session.Remove(WMSTekSessions.Shared.CustomerList);
            grdSearchCustomers.DataSource = null;
            grdSearchCustomers.DataBind();
        }

        private void InitializePageCountCustomer()
        {
            if (grdSearchCustomers.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchCustomers.Visible = true;
                // Paginador
                ddlPagesSearchCustomers.Items.Clear();
                for (int i = 0; i < grdSearchCustomers.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageCustomer) lstItem.Selected = true;

                    ddlPagesSearchCustomers.Items.Add(lstItem);
                }
                this.lblPageCountSearchCustomers.Text = grdSearchCustomers.PageCount.ToString();

                ShowCustomerButtonsPage();
            }
            else
            {
                divPageGrdSearchCustomers.Visible = false;
            }
        }

        private void ShowCustomerButtonsPage()
        {
            if (currentPageCustomer == grdSearchCustomers.PageCount - 1)
            {
                btnNextGrdSearchCustomers.Enabled = false;
                btnNextGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchCustomers.Enabled = false;
                btnLastGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchCustomers.Enabled = true;
                btnPrevGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchCustomers.Enabled = true;
                btnFirstGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageCustomer == 0)
                {
                    btnPrevGrdSearchCustomers.Enabled = false;
                    btnPrevGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchCustomers.Enabled = false;
                    btnFirstGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchCustomers.Enabled = true;
                    btnNextGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchCustomers.Enabled = true;
                    btnLastGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchCustomers.Enabled = true;
                    btnNextGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchCustomers.Enabled = true;
                    btnLastGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchCustomers.Enabled = true;
                    btnPrevGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchCustomers.Enabled = true;
                    btnFirstGrdSearchCustomers.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
        }

        protected void btnFirstGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomers.SelectedIndex = 0;
            ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
        }
                
        protected void btnPrevGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer > 0)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer - 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
            }
        }
           

        protected void btnLastGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomers.SelectedIndex = grdSearchCustomers.PageCount - 1;
            ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
        }

        protected void ddlPagesSearchCustomersSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.CustomerList))
            {
                customerSearchViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];

                currentPageCustomer = ddlPagesSearchCustomers.SelectedIndex;
                grdSearchCustomers.PageIndex = currentPageCustomer;

                // Encabezado de Recepciones
                grdSearchCustomers.DataSource = customerSearchViewDTO.Entities;
                grdSearchCustomers.DataBind();

                divLookupCustomer.Visible = true;
                mpLookupCustomer.Show();

                ShowCustomerButtonsPage();
            }
        }

        protected void btnNextGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer < grdSearchCustomers.PageCount)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer + 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
            }
        }

        /// <summary>
        /// Agrega el item seleccionado de la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSearchCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchCustomers.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.CustomerList))
                {
                    customerSearchViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];

                    foreach (Customer customer in customerSearchViewDTO.Entities)
                    {
                        if (customer.Id == editIndex)
                        {
                            this.txtCustomerCode.Text = customer.Code;
                            this.txtCustomerName.Text = customer.Name;

                            hidCustomerId.Value = customer.Id.ToString();
                            Session.Add("ItemCustomerMgrCustomer", customer);

                            // Esto evita un bug de ajax
                            rfvSummary.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                recipeVasViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(recipeVasViewDTO.Errors);
            }
        }
        #endregion
     
    }
}
