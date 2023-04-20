using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.DTO;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Stocks
{
    public partial class StockReserve : BasePage
    {

        #region "Declaración de Variables"

        //private GenericViewDTO<Role> roleViewDTO = new GenericViewDTO<Role>();
        //private GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();
        private bool isValidViewDTO = false;

        private GenericViewDTO<ReserveStock> stocksViewDTO = new GenericViewDTO<ReserveStock>();
        private GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();
        private GenericViewDTO<ReserveStock> reserveStocks = new GenericViewDTO<ReserveStock>();
        private GenericViewDTO<Item> itemSearchViewDTO;
        private Dictionary<String, String> titleParameters = new Dictionary<String, String>();
        private Dictionary<String, String> parameters = new Dictionary<String, String>();
        private const string sessionCustomers = "Customers";

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
        #endregion

        #region "Eventos"

        /// <summary>
        /// Los controles dinamicos deben ser CREADOS en Page_Init (antes de formarse el View State)
        /// </summary>
        //protected override void Page_Init(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        base.Page_Init(sender, e);

        //        // Si no esta en modo Configuration, sigue el curso normal
        //        if (base.webMode == WebMode.Normal)
        //        {
        //            Initialize();

        //            if (!Page.IsPostBack)
        //            {
        //                //hsVertical.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .6);

        //                // Carga inicial del ViewDTO
        //                UpdateSession(false);

        //                if (isValidViewDTO)
        //                {
        //                    PopulateData();
        //                    //LoadStock();
        //                    LoadReserve();
        //                    LoadCustomer();
        //                }
        //            }
        //            else
        //            {
        //                if (ValidateSession(WMSTekSessions.StockConsult.ReserveStock))
        //                {
        //                    stocksViewDTO = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.StockConsult.ReserveStock];
        //                    isValidViewDTO = true;
        //                }

        //                if (isValidViewDTO)
        //                {
        //                    // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
        //                    PopulateData();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        roleViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(roleViewDTO.Errors);
        //    }
        //}

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
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
                        PopulateData();
                    }

                    if (!Page.IsPostBack)
                        LoadCustomer();
                }

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
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
                    InitializeFilter(false, true);
                    //ReloadData();
                    upSelectItem.Update();
                    LoadReserve();
                    LoadCustomer();
                    upItem.Update();
                    upUser.Update();
                }
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }

        }



        protected void grdItemsReserve_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //envia el id de usuario desde el DataKeys                
                int idItem = int.Parse(grdItemsReserve.DataKeys[e.RowIndex].Values["IdItem"].ToString());
                int idCustomer = int.Parse(grdItemsReserve.DataKeys[e.RowIndex].Values["IdCustomer"].ToString());
                int idOwner = int.Parse(grdItemsReserve.DataKeys[e.RowIndex].Values["IdOwn"].ToString());
                int idWhs = int.Parse(grdItemsReserve.DataKeys[e.RowIndex].Values["Idwhs"].ToString());

                ReserveStock reserve = new ReserveStock();
                reserve.IdItem = idItem;
                reserve.IdCustomer = idCustomer;
                reserve.IdOwn = idOwner;
                reserve.Idwhs = idWhs;

                DeleteRow(reserve);


            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        private void DeleteRow(ReserveStock p_reserve)
        {
            stocksViewDTO = iWarehousingMGR.MaintainReserve(CRUD.Delete, p_reserve, context);

            if (stocksViewDTO.hasError())
                UpdateSession(false);
            else
            {
                crud = true;
                ucStatus.ShowMessage(stocksViewDTO.MessageStatus.Message);
                LoadReserve();
                isValidViewDTO = true;
                UpdateSession(false);
                upItem.Update();
            }
        }

        protected void grdItemsReserve_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int editIndex = grdItemsReserve.PageSize * grdItemsReserve.PageIndex + e.NewEditIndex;
                currentIndex = editIndex;
                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ShowModal(int index, CRUD mode)
        {
            if (mode == CRUD.Update)
            {
                txtQtyEdit.Text = string.Empty;
                decimal available = 0;

                reserveStocks = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.StockConsult.Reserve];
                var reserveToUpdate = reserveStocks.Entities[index];

                available += reserveToUpdate.Reserve;

                var newContext = new ContextViewDTO();
                newContext.MainFilter = new List<EntityFilter>();
                var arrEnum = Enum.GetValues(typeof(EntityFilterName));
                foreach (var item in arrEnum)
                {
                    newContext.MainFilter.Add(new EntityFilter(item.ToString(), new List<FilterItem>()));
                }
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(reserveToUpdate.IdOwn.ToString()));
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(reserveToUpdate.Idwhs.ToString()));
                newContext.MainFilter.Add(new EntityFilter() { Name = "ItemEqual", FilterValues = new List<FilterItem>() { new FilterItem() { Name = "ItemEqual", Value = reserveToUpdate.ItemCode } } });

                var availableStocksViewDTO = iWarehousingMGR.FindAllReserveStock(newContext);

                if (!availableStocksViewDTO.hasError())
                {
                    if (availableStocksViewDTO.Entities.Count > 0)
                    {
                        var availableStockByItem = availableStocksViewDTO.Entities.Where(s => s.ItemCode == reserveToUpdate.ItemCode).FirstOrDefault();

                        if (availableStockByItem != null)
                            available += availableStockByItem.Available;
                    }
                        
                    if (available > 0)
                    {
                        divEditNew.Visible = true;
                        lblItemEdit.Text = reserveToUpdate.ShortNameItem;
                        rvQtyEdit.Enabled = true;
                        rvQtyEdit.MaximumValue = available.ToString();
                        modalPopEditReserve.Show();
                        upEditNew.Update();
                    }
                    else
                    {
                        ucStatus.ShowWarning("No se puede editar porque no hay cantidad disponible");
                    }
                }
                else
                {
                    this.Master.ucError.ShowError(availableStocksViewDTO.Errors);
                }
            }
        }

        protected void btnUpdateReserve_Click(object sender, EventArgs e)
        {
            try
            {
                reserveStocks = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.StockConsult.Reserve];
                var reserveToUpdate = reserveStocks.Entities[currentIndex];
                reserveToUpdate.Reserve = int.Parse(txtQtyEdit.Text.Trim());

                UpdateRow(reserveToUpdate);
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        private void UpdateRow(ReserveStock reserve)
        {
            stocksViewDTO = iWarehousingMGR.MaintainReserve(CRUD.Update, reserve, context);

            if (stocksViewDTO.hasError())
            {
                //UpdateSession(false);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
            else
            {
                txtCode.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txtQtyAvailable.Text = "0";
                txtQty.Text = "0";
                crud = true;
                ucStatus.ShowMessage(stocksViewDTO.MessageStatus.Message);
                LoadReserve();
                UpdateSession(false);
                upItem.Update();
                txtQtyEdit.Text = string.Empty;
                isValidViewDTO = true;
                upUser.Update();
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            divEditNew.Visible = false;
            txtQtyEdit.Text = string.Empty;
            modalPopEditReserve.Hide();
            upEditNew.Update();
        }

        protected void grdItemsReserve_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                        if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdItemsReserve.ClientID + "')");
                        e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdItemsReserve.ClientID + "')");
                        e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdItemsReserve.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAddItem();
            LoadReserve();
            //Int32 idCustomer = Convert.ToInt32(ddlCustomer.SelectedItem.Value);

            //// Valida variable de sesion del Usuario Loggeado
            //if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            //{
            //    grdItemsReserve.EmptyDataText = this.Master.EmptyGridText;

            //    stocksViewDTO = iWarehousingMGR.FindReserveStockByCustomer(idCustomer, context);

            //    if (!stocksViewDTO.hasError() && stocksViewDTO.Entities != null)
            //    {
            //        grdItemsReserve.DataSource = stocksViewDTO.Entities;
            //        grdItemsReserve.DataBind();
            //    }
            //    else
            //    {
            //        grdItemsReserve.EmptyDataText = this.Master.EmptyGridText;
            //    }

            //}

        }


        #endregion

        #region "Métodos"

        /// <summary>
        /// Valida e Inicializa los objetos de Sesion
        /// </summary>
        protected void Initialize()
        {
            //InitializeTaskBar();
            //InitializeFilter(!Page.IsPostBack, false);
            //InitializeStatusBar();
            //InitializeGridItems();
            //InitializeFilterItem();


            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGridItems();
            InitializeFilterItem();

            if (!Page.IsPostBack)
            {
                Session[sessionCustomers] = null;
            }
            else
            {
                if (ValidateSession(WMSTekSessions.StockConsult.ReserveStock))
                {
                    stocksViewDTO = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.StockConsult.ReserveStock];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeFilterItem()
        {
            ucFilterItem.Initialize();
            ucFilterItem.BtnSearchClick += new EventHandler(btnSearchItem_Click);

            ucFilterItem.FilterCode = this.lblFilterCode.Text;
            ucFilterItem.FilterDescription = this.lblFilterName.Text;
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita el Filtro Avanzado
            //this.Master.ucMainFilter.advancedFilterVisible = true;

            //FILTRO BASICO
            //Centro
            this.Master.ucMainFilter.warehouseVisible = true;
            //Dueño
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;

            //Item
            this.Master.ucMainFilter.itemVisible = true;
            //InboundType
            //this.Master.ucMainFilter.inboundTypeVisible = true;
            //ReferenceDocType
            //this.Master.ucMainFilter.referenceDocTypeVisible = true;
            //NºDoc
            //this.Master.ucMainFilter.documentVisible = true;
            //Recepcion Desde
            //this.Master.ucMainFilter.dateFromVisible = true;
            //Recepcion Hasta
            //this.Master.ucMainFilter.dateToVisible = true;
            //Cod Item
            //this.Master.ucMainFilter.itemVisible = true;

            //FILTRO AVANZADO

            //Tab Fecha
            //this.Master.ucMainFilter.tabDatesVisible = true;
            //this.Master.ucMainFilter.expirationDateVisible = true;
            //this.Master.ucMainFilter.expectedDateVisible = true;

            //Tab Documento
            this.Master.ucMainFilter.tabDocumentVisible = true;
            this.Master.ucMainFilter.vendorVisible = true;
            this.Master.ucMainFilter.carrierVisible = true;
            this.Master.ucMainFilter.driverVisible = true;

            //Tab Proveedor
            //this.Master.ucMainFilter.tabProveedorVisible = true;

            //Tab Transportista
            //this.Master.ucMainFilter.tabTransportistaVisible = true;

            //Tab Chofer
            //this.Master.ucMainFilter.tabChoferVisible = true;


            //TabGrupos
            //this.Master.ucMainFilter.tabItemGroupVisible = true;

            //// Configura textos a mostar
            //this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;


            // Configura parámetros de fechas
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.ReceiptDaysBefore;
            //this.Master.ucMainFilter.DateAfter = CfgParameterName.ReceiptDaysAfter;

            //Setea los filtros para que no tengan la propiedad de autopostback
            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            Master.ucMainFilter.FilterOwnerAutoPostBack = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
            this.Master.ucMainFilter.ddlOwnerIndexChanged += new EventHandler(ddlOwnerIndexChanged);
        }

        protected void ddlOwnerIndexChanged(object sender, EventArgs e)
        {
            LoadCustomer();
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
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
                stocksViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            stocksViewDTO = iWarehousingMGR.FindAllReserveStock(context);//iReceptionMGR.FindAllReceipt(context);

            if (!stocksViewDTO.hasError() && stocksViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.ReserveStock, stocksViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(stocksViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        private void PopulateData()
        {
            grdItems.PageIndex = currentPage;
            grdItems.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!stocksViewDTO.hasConfigurationError() && stocksViewDTO.Configuration != null &&
                stocksViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdItems, stocksViewDTO.Configuration);


            // Encabezado de Recepciones
            grdItems.DataSource = stocksViewDTO.Entities;
            grdItems.DataBind();


            ucStatus.ShowRecordInfo(stocksViewDTO.Entities.Count, grdItems.PageSize, grdItems.PageCount, currentPage, grdItems.AllowPaging);
        }




        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    SearchItem();

                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        divLookupItem.Visible = true;
                        mpLookupItem.Show();

                        InitializePageCountItems();
                    }
                }
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }








        protected void LoadStock()
        {
            grdItems.PageIndex = currentPage;
            grdItems.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!stocksViewDTO.hasConfigurationError() && stocksViewDTO.Configuration != null &&
                stocksViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdItems, stocksViewDTO.Configuration);

            // Encabezado de Recepciones
            grdItems.DataSource = stocksViewDTO.Entities;
            grdItems.DataBind();


            ucStatus.ShowRecordInfo(stocksViewDTO.Entities.Count, grdItems.PageSize, grdItems.PageCount, currentPage, grdItems.AllowPaging);
        }

        protected void LoadReserve()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            var newContext = NewContext();
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(this.Master.ucMainFilter.idOwn.ToString()));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(this.Master.ucMainFilter.idWhs.ToString()));

            if (ddlCustomer.SelectedItem != null)
            {
                Int32 idCustomer = Convert.ToInt32(ddlCustomer.SelectedItem.Value);
                //reserveStocks = iWarehousingMGR.FindAllReserve(context);
                reserveStocks = iWarehousingMGR.FindReserveStockByCustomer(idCustomer, newContext);

                if (!reserveStocks.hasError() && reserveStocks.Entities != null)
                {
                    Session.Add(WMSTekSessions.StockConsult.Reserve, reserveStocks);

                    grdItemsReserve.DataSource = reserveStocks.Entities;
                    grdItemsReserve.DataBind();

                }
                else
                {
                    isValidViewDTO = false;
                    this.Master.ucError.ShowError(reserveStocks.Errors);
                }
            }

            //////divDetail.Visible = true;

            ////context.MainFilter = this.Master.ucMainFilter.MainFilter;

            ////reserveStocks = iWarehousingMGR.FindAllReserve(context);//iReceptionMGR.FindAllReceipt(context);

            ////if (!reserveStocks.hasError() && reserveStocks.Entities != null)
            ////{
            ////    grdItemsReserve.DataSource = reserveStocks.Entities;
            ////    grdItemsReserve.DataBind();

            ////}
            ////else
            ////{
            ////    //isValidViewDTO = false;
            ////    //this.Master.ucError.ShowError(stocks.Errors);
            ////}

        }

        private void LoadCustomer()
        {
            try
            {
                var newContext = NewContext();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(this.Master.ucMainFilter.idOwn.ToString()));

                customerViewDTO = iWarehousingMGR.FindAllCustomer(newContext);

                ddlCustomer.DataSource = customerViewDTO.Entities;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "Id";
                ddlCustomer.DataBind();
                upUser.Update();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ReloadData()
        {
            UpdateSession(false);
            
            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                //divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        #endregion


        protected void imgBtnSearchItem2_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    rvQty.Enabled = true;
                    ////bool validItem = false;
                    ////bool existingItem = false;
                    pnlError.Visible = false;
                    int idOwner = this.Master.ucMainFilter.idOwn;

                    ////// Busca en base de datos el Item ingresado 
                    ////if (txtCode.Text.Trim() != string.Empty)
                    ////{
                    ////    //itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtCode.Text.Trim(), Convert.ToInt16(ddlOwner.SelectedValue), false);
                    ////    itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtCode.Text.Trim(), idOwner, false);

                    ////    // Si el codigo de Item ingresado es válido, lo carga directamente
                    ////    if (itemSearchViewDTO.Entities != null && itemSearchViewDTO.Entities.Count == 1)
                    ////    {
                    ////        validItem = true;
                    ////        Item item = new Item(itemSearchViewDTO.Entities[0].Id);

                    ////        item.Description = itemSearchViewDTO.Entities[0].Description;
                    ////        item.Code = itemSearchViewDTO.Entities[0].Code;

                    ////        // Mantiene en memoria los datos del Item a agregar
                    ////        Session.Add("StockReserveNewItem", item);

                    ////        // Recorre los items ya agregados y compara con el que se quiere agregar
                    ////        //if (inboundDetails != null && inboundDetails.Count > 0)
                    ////        //{
                    ////        //    foreach (InboundDetail inboundDetail in inboundDetails)
                    ////        //    {
                    ////        //        // Si ya existe en la lista se marca
                    ////        //        if (inboundDetail.Item.Code == item.Code)
                    ////        //        {
                    ////        //            existingItem = true;
                    ////        //            pnlError.Visible = false;
                    ////        //        }
                    ////        //    }
                    ////        //}

                    ////        // Si no fue agregado, agrega el item 
                    ////        if (!existingItem)
                    ////        {
                    ////            this.txtCode.Text = item.Code;
                    ////            this.txtDescription.Text = item.Description;
                    ////            hidItemId.Value = item.Id.ToString();
                    ////        }
                    ////        else
                    ////        {
                    ////            //ClientScript.RegisterClientScriptBlock(typeof(OutboundOrderMgr), "ExistingItem", "Alert('Item existente')");
                    ////            pnlError.Visible = true;
                    ////        }
                    ////    }
                    ////}

                    // Si no es válido o no se ingresó, se muestra la lista de Items para seleccionar uno
                    ////if (!validItem)
                    ////{
                    ucFilterItem.Clear();
                    ucFilterItem.Initialize();

                    // Setea el filtro con el Item ingresado
                    if (txtCode.Text.Trim() != string.Empty)
                    {
                        FilterItem filterItem = new FilterItem("%" + txtCode.Text + "%");
                        filterItem.Selected = true;
                        ucFilterItem.FilterItems[0] = filterItem;

                        ucFilterItem.LoadCurrentFilter(ucFilterItem.FilterItems);
                        SearchItem();
                    }
                    // Si no se ingresó ningún item, no se ejecuta la búsqueda
                    else
                        ClearGridItem();

                    // Esto evita un bug de ajax
                    valAddItem.Enabled = false;

                    divLookupItem.Visible = true;
                    mpLookupItem.Show();

                    InitializePageCountItems();
                    ////}
                }

            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void imgBtnAddItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.StockConsult.Reserve))
                {
                    bool itemReservado = false;

                    // Recupera el Item a agregar
                    Item newItem = (Item)Session["StockReserveNewItem"];
                    reserveStocks = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.StockConsult.Reserve];

                    decimal qtyAvailable = decimal.Parse(this.txtQtyAvailable.Text);
                    decimal qytReserve = decimal.Parse(this.txtQty.Text);
                    int IdCustomer = int.Parse(this.ddlCustomer.SelectedValue);
                    int idOwner = this.Master.ucMainFilter.idOwn;
                    int idWhs = this.Master.ucMainFilter.idWhs;

                    ReserveStock newReserve = new ReserveStock();
                    newReserve.IdItem = newItem.Id;
                    newReserve.IdOwn = idOwner;
                    newReserve.Idwhs = idWhs;
                    newReserve.IdCustomer = IdCustomer;
                    newReserve.Reserve = qytReserve;

                    //Valida cantidad Disponible mayor a cantidad por reservar
                    if (qtyAvailable >= qytReserve)
                    {



                        //Valida que item no se encuentre asignado
                        foreach (var item in reserveStocks.Entities)
                        {
                            if (item.IdItem == newItem.Id)
                            {
                                ucStatus.ShowWarning(lblItemAsignado.Text);
                                itemReservado = true;
                            }
                        }

                        if (!itemReservado)
                        {
                            // Crea el nuevo detalle de la Orden
                            reserveStocks = iWarehousingMGR.MaintainReserve(CRUD.Create, newReserve, context);
                            if (!reserveStocks.hasError())
                            {
                                ucStatus.ShowMessage(reserveStocks.MessageStatus.Message);
                                LoadReserve();
                                UpdateSession(false);
                                upItem.Update();
                                isValidViewDTO = true;
                            }
                            else
                            {
                                //ucStatus.ShowError(reserveStocks.Errors.Message);
                                this.Master.ucError.ShowError(reserveStocks.Errors);
                            }
                        }

                        ClearAddItem();
                    }
                    else
                    {
                        stocksViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.StockNotEnough, context, titleParameters, parameters));
                        this.Master.ucError.ShowError(stocksViewDTO.Errors);
                    }
                }
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
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

        protected void btnNextGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems < grdSearchItems.PageCount)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems + 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);

            }
        }

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

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
                            this.txtCode.Text = item.Code;
                            this.txtDescription.Text = item.Description;
                            hidItemId.Value = item.Id.ToString();
                            Session.Add("StockReserveNewItem", item);

                            //this.Master.ucMainFilter.MainFilter[8].FilterValues.Add(filter);
                            //context.MainFilter = this.Master.ucMainFilter.MainFilter;

                            ContextViewDTO contexto = new ContextViewDTO();

                            contexto.MainFilter = new List<EntityFilter>();
                            contexto.MainFilter.Add(new EntityFilter() { Name = "Owner", FilterValues = new List<FilterItem>() { new FilterItem() { Name = "Owner", Value = Master.ucMainFilter.idOwn.ToString() } } });
                            contexto.MainFilter.Add(new EntityFilter() { Name = "Warehouse", FilterValues = new List<FilterItem>() { new FilterItem() { Name = "Warehouse", Value = Master.ucMainFilter.idWhs.ToString() } } });
                            contexto.MainFilter.Add(new EntityFilter() { Name = "ItemEqual", FilterValues = new List<FilterItem>() { new FilterItem() { Name = "ItemEqual", Value = item.Code } } });

                            GenericViewDTO<ReserveStock> stocksNew = iWarehousingMGR.FindAllReserveStock(contexto);
                            //contexto.MainFilter[Convert.ToInt16(EntityFilterName.Item)].FilterValues.Remove(filter);

                            if (stocksNew.Entities.Count > 0)
                            {
                                if (stocksNew.Entities[0].Available > 0)
                                    this.rvQty.MaximumValue = stocksNew.Entities[0].Available.ToString();
                                else
                                {
                                    decimal minValueInMaxValueValidator = 0.1M;
                                    this.rvQty.MaximumValue = minValueInMaxValueValidator.ToString();
                                    ucStatus.ShowWarning(lblUnableToReserve.Text);
                                }

                                this.txtQtyAvailable.Text = GetFormatedNumber(stocksNew.Entities[0].Available);

                            }
                            else
                            {
                                this.txtQtyAvailable.Text = "0";
                            }

                            // Esto evita un bug de ajax
                            //valEditNew.Enabled = true;
                            valAddItem.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        private void ClearAddItem()
        {
            this.txtCode.Text = string.Empty;
            this.txtDescription.Text = string.Empty;
            this.txtQty.Text = "0";
            this.txtQtyAvailable.Text = "0";
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
        }

        private void SearchItem()
        {
            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, this.Master.ucMainFilter.idOwn, true);
            Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
            InitializeGrid();

            grdSearchItems.DataSource = itemSearchViewDTO.Entities;
            grdSearchItems.DataBind();
            isValidViewDTO = true;
        }

        private void InitializeGridItems()
        {
            grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGrid()
        {
            grdItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdItems.EmptyDataText = this.Master.EmptyGridText;
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


        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateData();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateData();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateData();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateData();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdItems.PageCount - 1;
                currentIndex = -1;
                //divDetail.Visible = false;
                PopulateData();
            }
            catch (Exception ex)
            {
                stocksViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stocksViewDTO.Errors);
            }
        }

        protected void grdItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdItemsReserve_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}
