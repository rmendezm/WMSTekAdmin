using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;




namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class UsefulLifeMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<UsefulLife> UsefulLifeViewDTO = new GenericViewDTO<UsefulLife>();
        private GenericViewDTO<Item> itemSearchViewDTO;
        private GenericViewDTO<Customer> customerSearchViewDTO;
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

        // Propiedad para controlar el nro de pagina activa en la grilla Item
        public int currentPageItems
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

        // Propiedad para controlar el indice activo en la grilla
        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return -1;
            }

            set { ViewState["index"] = value; }
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

                    if (ValidateSession(WMSTekSessions.UsefullLigeMgr.UsefullLigeList))
                    {
                        UsefulLifeViewDTO = (GenericViewDTO<UsefulLife>)Session[WMSTekSessions.UsefullLigeMgr.UsefullLigeList];
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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

        protected void btnNextGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer < grdSearchCustomers.PageCount)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer + 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
            }
        }

        protected void btnFirstGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = 0;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void btnPrevGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems > 0)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems - 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);
        }

        protected void btnNextGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems < grdSearchItems.PageCount)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems + 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
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

        protected void ddlPagesSearchItemsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.ItemList))
            {
                itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                currentPageItems = ddlPagesSearchItems.SelectedIndex;
                grdSearchItems.PageIndex = currentPageItems;

                grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                grdSearchItems.DataBind();

                divLookupItem.Visible = true;
                mpLookupItem.Show();

                ShowItemsButtonsPage();
            }
        }

        /// <summary>
        /// Agrega el customer seleccionado de la grilla
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
                            SearchBranch();
                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el item seleccionado de la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSearchItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchItems.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.ItemList))
                {
                    itemSearchViewDTO = (GenericViewDTO<Item>)Session[WMSTekSessions.Shared.ItemList];

                    foreach (Item item in itemSearchViewDTO.Entities)
                    {
                        if (item.Id == editIndex)
                        {
                            this.txtItemCode.Text = item.Code;
                            //this.txtItemName.Text = item.ShortName;
                            hidItemId.Value = item.Id.ToString();

                            Session.Add("ItemCustomerMgrItem", item);

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void ddlOwnerLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia a los clientes.               
                //base.LoadCustomerId(this.ddlCustomerLoad, Convert.ToInt32(ddlOwnerLoad.SelectedValue), true, string.Empty);
                //base.LoadCategoryItemByOwner
                //modalPopUp.Show();
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ClearGridItem();
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void btnCloseNewEdit_Click(object sender, EventArgs e)
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                //currentPage = grdMgr.PageCount - 1;
                //PopulateGrid();
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
            {
                SearchItem();

                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    //tabOwner.Visible = true;
                    divLookupItem.Visible = true;
                    mpLookupItem.Show();

                    InitializePageCountItems();
                }
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        private void SearchCustomer()
        {
            try
            {
                customerSearchViewDTO = new GenericViewDTO<Customer>();
                GenericViewDTO<Customer> CustomerViewDTO = new GenericViewDTO<Customer>();

                int idOwn = Convert.ToInt32(this.ddlOwnerLoad.SelectedValue);

                CustomerViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context, idOwn);

                Session.Add(WMSTekSessions.Shared.CustomerList, CustomerViewDTO);
                //grdMgr.EmptyDataText = this.Master.EmptyGridText;
                InitializeGridCustomer();
                grdSearchCustomers.DataSource = CustomerViewDTO.Entities;
                grdSearchCustomers.DataBind();

                //carga los branch asociados al customer
                SearchBranch();
                //}
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }
        #endregion

        #region "Eventos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeFilterItem();
            InitializeFilterCustomer();
            InitializeStatusBar();
            InitializeGrid();
         }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
                UsefulLifeViewDTO.ClearError();
            }

            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            UsefulLifeViewDTO = iWarehousingMGR.FindAlUsefulLife(context);

            if (!UsefulLifeViewDTO.hasError() && UsefulLifeViewDTO.Entities != null)
            {
                Session.Remove(WMSTekSessions.UsefullLigeMgr.UsefullLigeList);
                Session.Add(WMSTekSessions.UsefullLigeMgr.UsefullLigeList, UsefulLifeViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(UsefulLifeViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!UsefulLifeViewDTO.hasConfigurationError() && UsefulLifeViewDTO.Configuration != null && UsefulLifeViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, UsefulLifeViewDTO.Configuration);

            grdMgr.DataSource = UsefulLifeViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(UsefulLifeViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = true;
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = true;
            if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
            {
                this.Master.ucMainFilter.warehouseNotIncludeAll = false;
            }
            else
            {
                this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            }
            if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
            {
                this.Master.ucMainFilter.ownerNotIncludeAll = false;
            }
            else
            {
                this.Master.ucMainFilter.ownerNotIncludeAll = true;
            }
            
            
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblTitleCodigoCustomer.Text;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeFilterItem()
        {
            ucFilterItem.Initialize();
            ucFilterItem.BtnSearchClick += new EventHandler(btnSearchItem_Click);

            ucFilterItem.FilterCode = this.lblFilterCode.Text;
            ucFilterItem.FilterDescription = this.lblFilterName.Text;
        }

        private void InitializeFilterCustomer()
        {
            ucFilterCustomer.Initialize();
            ucFilterCustomer.BtnSearchClick += new EventHandler(btnSearchCustomer_Click);

            ucFilterCustomer.FilterCode = this.lblFilterCode.Text;
            ucFilterCustomer.FilterDescription = this.lblFilterName.Text;

            //ucFilterCustomerLoad.Initialize();
            //ucFilterCustomerLoad.BtnSearchClick += new EventHandler(btnSearchCustomerLoad_Click);

            //ucFilterCustomerLoad.FilterCode = this.lblFilterCode.Text;
            //ucFilterCustomerLoad.FilterDescription = this.lblFilterName.Text;
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

        private void InitializeGridItems()
        {
            grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridCustomer()
        {
            grdSearchCustomers.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchCustomers.EmptyDataText = this.Master.EmptyGridText;
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

        protected void SearchBranch()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.hidCustomerId.Value) && this.hidCustomerId.Value != "-1" && this.hidCustomerId.Value != "0")
                {
                    var listBranches = GetBranches(Convert.ToInt32(this.ddlOwnerLoad.SelectedValue), Convert.ToInt32(hidCustomerId.Value));

                    if (listBranches != null && listBranches.Count > 0)
                    {                       
                        FillDdlBranches(ddlBranch, listBranches);
                    }
                    else
                    {
                        ClearDdlBranches();
                    }
                }
                
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        protected void SearchBranch(int idBranch)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.hidCustomerId.Value) && this.hidCustomerId.Value != "-1" && this.hidCustomerId.Value != "0")
                {
                    var listBranches = GetBranches(Convert.ToInt32(this.ddlOwnerLoad.SelectedValue), Convert.ToInt32(hidCustomerId.Value));

                    if (listBranches != null && listBranches.Count > 0)
                    {
                        FillDdlBranches(ddlBranch, listBranches, idBranch);
                    }
                    else
                    {
                        ClearDdlBranches();
                    }
                }

            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        private void ClearDdlBranches()
        {
            ddlBranch.Items.Clear();
            ddlBranch.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
            ddlBranch.Items[0].Selected = true;
        }

        private List<Branch> GetBranches(int idOwn, int idCustomer)
        {
            var branchParameter = new Branch()
            {
                Owner = new Owner() { Id = idOwn },
                Customer = new Customer() { Id = idCustomer },
            }; 

            var branchesDTO = iWarehousingMGR.GetBranchByAnyParameter(branchParameter, context);

            if (branchesDTO.hasError())
            {
                this.Master.ucError.ShowError(branchesDTO.Errors);
                return null;
            }
            else
            {
                return branchesDTO.Entities;
            }
        }

        private void FillDdlBranches(DropDownList ddl, List<Branch> listBranches)
        {
            ddl.DataSource = listBranches;
            ddl.DataTextField = "Name";
            ddl.DataValueField = "Id";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("Seleccione", "-1"));
            ddl.Items[0].Selected = true;
        }

        private void FillDdlBranches(DropDownList ddl, List<Branch> listBranches, int idBranch)
        {
            ddl.DataSource = listBranches;
            int index = listBranches.FindIndex(p => p.Id == idBranch);
            ddl.DataTextField = "Name";
            ddl.DataValueField = "Id";
            ddl.DataBind();
            
            ddl.Items[index].Selected = true;
        }

        protected void SaveChanges()
        {
            //Agrega los datos del Dueño
           
            UsefulLife UsefulLife = new UsefulLife();
            
            UsefulLife.Customer.Id = Convert.ToInt32(hidCustomerId.Value);
            UsefulLife.Warehouse.Id = Convert.ToInt32(ddlWarehouse.SelectedValue);
            UsefulLife.Owner.Id= Convert.ToInt32(ddlOwnerLoad.SelectedValue);
            UsefulLife.Item.Id = Convert.ToInt32(hidItemId.Value);
            UsefulLife.DayQty = Convert.ToInt32(this.txtDayQty.Text);

            if (string.IsNullOrEmpty(ddlBranch.SelectedValue))
            {
                UsefulLife.Branch.Id = -1;
            }
            else
            {
                UsefulLife.Branch.Id = Convert.ToInt32(ddlBranch.SelectedValue);
            }           

            if (UsefulLife.DayQty > 0)
            {
                if (hidEditId.Value == "0" )
                {                    
                    UsefulLifeViewDTO = iWarehousingMGR.MaintainUsefulLife(CRUD.Create, UsefulLife, context);
                }
                else
                {
                    UsefulLife.Id = Convert.ToInt16(hidEditId.Value);
                    UsefulLifeViewDTO = iWarehousingMGR.MaintainUsefulLife(CRUD.Update, UsefulLife, context);
                }                
            }

            divGrid.Visible = true;
            divModal.Visible = false;

            if (UsefulLifeViewDTO.hasError())
            {
                UpdateSession(true);
                divGrid.Visible = false;
                divModal.Visible = true;
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(UsefulLifeViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // tabOwner.ActiveTabIndex = 0;

            base.LoadUserWarehouses(this.ddlWarehouse, this.Master.EmptyRowText, "-1", false);
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", false, string.Empty, false);
            
            // Editar Owner
            if (mode == CRUD.Update)
            {
                UsefulLifeViewDTO = (GenericViewDTO<UsefulLife>)Session[WMSTekSessions.UsefullLigeMgr.UsefullLigeList];

                //// TODO: ver propiedad 'required' para un drop-down list
                this.lblEdit.Visible = false;
                this.tabGeneral.HeaderText = lbltabGeneralModificar.Text;
                this.ddlWarehouse.SelectedValue = UsefulLifeViewDTO.Entities[index].Warehouse.Id.ToString();
                this.ddlOwnerLoad.SelectedValue = UsefulLifeViewDTO.Entities[index].Owner.Id.ToString();
                             
                this.hidCustomerId.Value = UsefulLifeViewDTO.Entities[index].Customer.Id.ToString();
                this.txtCustomerCode.Text = UsefulLifeViewDTO.Entities[index].Customer.Code;
                this.txtCustomerName.Text = UsefulLifeViewDTO.Entities[index].Customer.Name;
                this.txtDayQty.Text = UsefulLifeViewDTO.Entities[index].DayQty.ToString();
                this.txtItemCode.Text = UsefulLifeViewDTO.Entities[index].Item.Code;
                this.hidItemId.Value = UsefulLifeViewDTO.Entities[index].Item.Id.ToString();
                this.tabGeneral.HeaderText = lbltabGeneralNuevo.Text;
                hidEditId.Value = UsefulLifeViewDTO.Entities[index].Id.ToString();
                
                SearchBranch(UsefulLifeViewDTO.Entities[index].Branch.Id);

                isNew = false;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";
                this.tabGeneral.HeaderText = lbltabGeneralNuevo.Text;
                this.lblEdit.Visible = false;
                this.ddlOwnerLoad.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
                this.txtCustomerCode.Text = string.Empty;
                this.txtCustomerName.Text = string.Empty;
                this.txtItemCode.Text = string.Empty;
                this.txtDayQty.Text = "1";

                //base.LoadCustomerId(this.ddlCustomerLoad, Convert.ToInt32(ddlOwnerLoad.SelectedValue), true, string.Empty);
                ClearDdlBranches();
                isNew = true;
            }

            if (UsefulLifeViewDTO.Configuration != null && UsefulLifeViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(UsefulLifeViewDTO.Configuration, true);
                else
                    base.ConfigureModal(UsefulLifeViewDTO.Configuration, false);
            }

            divGrid.Visible = false;
            divModal.Visible = true;

        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            UsefulLifeViewDTO = iWarehousingMGR.MaintainUsefulLife(CRUD.Delete, UsefulLifeViewDTO.Entities[index], context);

            if (UsefulLifeViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(UsefulLifeViewDTO.MessageStatus.Message);

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
                    if (this.ddlOwnerLoad.SelectedValue == "-1")
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblMessajeSelectedOwner.Text, "");
                    }
                    else
                    {

                        bool validCustomer = false;

                        // Busca en base de datos el Customer ingresado 
                        if (txtCustomerCode.Text.Trim() != string.Empty)
                        {
                            customerSearchViewDTO = new GenericViewDTO<Customer>();
                            Customer customer = new Customer();
                            customer.Code = this.txtCustomerCode.Text;
                            //customer.Name = this.txtDescription.Text;

                            //customerSearchViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context,Convert.ToInt32(this.ddlOwner.SelectedValue));
                            validCustomer = false;

                        }

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
                            valEditNew.Enabled = false;
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
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el item ingresado, o abre una lista de selección si no se ingresó ninguno o el item ingresado no es válido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgBtnSearchItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {

                    if (this.ddlOwnerLoad.SelectedValue == "-1")
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblMessajeSelectedOwner.Text, "");
                    }
                    else
                    {
                        bool validItem = false;
                        bool existingItem = false;
                        //pnlError.Visible = false;

                        // Busca en base de datos el Item ingresado 
                        if (txtItemCode.Text.Trim() != string.Empty)
                        {
                            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtItemCode.Text.Trim(), Convert.ToInt16(ddlOwnerLoad.SelectedValue), false);
                            ShowItem(validItem, existingItem);
                        }

                        // Si no es válido o no se ingresó, se muestra la lista de Items para seleccionar uno
                        if (!validItem)
                        {
                            ucFilterItem.Clear();
                            ucFilterItem.Initialize();

                            // Setea el filtro con el Item ingresado
                            if (txtItemCode.Text.Trim() != string.Empty)
                            {
                                FilterItem filterItem = new FilterItem("%" + txtItemCode.Text + "%");
                                filterItem.Selected = true;
                                ucFilterItem.FilterItems[0] = filterItem;
                                ucFilterItem.LoadCurrentFilter(ucFilterItem.FilterItems);
                                SearchItem();
                            }
                            // Si no se ingresó ningún item, no se ejecuta la búsqueda
                            else
                            {
                                ClearGridItem();
                            }

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = false;
                            //valAddItem.Enabled = false;
                            //valSearchItem.Enabled = false;

                            divLookupItem.Visible = true;   
                            mpLookupItem.Show();

                            InitializePageCountItems();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        private void ShowItemsButtonsPage()
        {
            if (currentPageItems == grdSearchItems.PageCount - 1)
            {
                btnNextGrdSearchItems.Enabled = false;
                btnNextGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchItems.Enabled = false;
                btnLastGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchItems.Enabled = true;
                btnPrevGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchItems.Enabled = true;
                btnFirstGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageItems == 0)
                {
                    btnPrevGrdSearchItems.Enabled = false;
                    btnPrevGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchItems.Enabled = false;
                    btnFirstGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchItems.Enabled = true;
                    btnNextGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchItems.Enabled = true;
                    btnLastGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchItems.Enabled = true;
                    btnNextGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchItems.Enabled = true;
                    btnLastGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchItems.Enabled = true;
                    btnPrevGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchItems.Enabled = true;
                    btnFirstGrdSearchItems.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
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

        private void SearchItem()
        {
            try
            {
                itemSearchViewDTO = new GenericViewDTO<Item>();

                itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, Convert.ToInt16(ddlOwnerLoad.SelectedValue), true);
                Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
                InitializeGridItems();
                grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                grdSearchItems.DataBind();

            }
            catch (Exception ex)
            {
                UsefulLifeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(UsefulLifeViewDTO.Errors);
            }
        }

        private void ShowItem(bool validItem, bool existingItem)
        {
            // Si el codigo de Item ingresado es válido, lo carga directamente
            if (itemSearchViewDTO.Entities != null && itemSearchViewDTO.Entities.Count == 1)
            {
                validItem = true;
                Item item = new Item(itemSearchViewDTO.Entities[0].Id);

                item.Description = itemSearchViewDTO.Entities[0].Description;
                item.Code = itemSearchViewDTO.Entities[0].Code;

                // Mantiene en memoria los datos del Item a agregar
                Session.Add("ItemCustomerMgrItem", item);

                this.txtItemCode.Text = item.Code;
                //this.txtDescription.Text = item.Description;
                hidItemId.Value = item.Id.ToString();
            }
        }

        private void InitializePageCountItems()
        {
            if (grdSearchItems.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchItems.Visible = true;
                // Paginador
                ddlPagesSearchItems.Items.Clear();
                for (int i = 0; i < grdSearchItems.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageItems) lstItem.Selected = true;

                    ddlPagesSearchItems.Items.Add(lstItem);
                }
                this.lblPageCountSearchItems.Text = grdSearchItems.PageCount.ToString();

                ShowItemsButtonsPage();
            }
            else
            {
                divPageGrdSearchItems.Visible = false;
            }
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

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
            //hidEditId.Value = "-1";
            txtItemCode.Text = "";
        }

        private void ClearGridCustomer()
        {
            Session.Remove(WMSTekSessions.Shared.CustomerList);
            grdSearchCustomers.DataSource = null;
            grdSearchCustomers.DataBind();
        }

        protected void grdSearchItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSearchCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridWithNoDragAndDrop();", true);
        }
        #endregion
    }
}
