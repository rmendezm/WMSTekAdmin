using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class ItemVendorMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<ItemVendor> itemVendorViewDTO = new GenericViewDTO<ItemVendor>();
        private GenericViewDTO<Item> itemSearchViewDTO;
        private GenericViewDTO<Vendor> VendorSearchViewDTO;
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

        public int currentPageVendor
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


        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();

                if (!Page.IsPostBack)
                {
                    this.tabLayout.HeaderText = this.lbltabGeneral.Text;
                }
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeFilterItem();
            InitializeFilterVendor();
            InitializeStatusBar();
            InitializeGrid();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            if (!Page.IsPostBack)
            {
                //UpdateSession(false);
                PopulateLists();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.ItemVendorMgr.ItemVendorList))
                {
                    itemVendorViewDTO = (GenericViewDTO<ItemVendor>)Session[WMSTekSessions.ItemVendorMgr.ItemVendorList];
                    isValidViewDTO = true;
                }                             

                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.codeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.codeLabel = this.lblCodeItemVendor.Text;
            this.Master.ucMainFilter.nameLabel = this.lblVendor.Text;

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

        private void InitializeFilterVendor()
        {
            ucFilterVendor.Initialize();
            ucFilterVendor.BtnSearchClick += new EventHandler(btnSearchVendor_Click);

            ucFilterVendor.FilterCode = this.lblFilterCode.Text;
            ucFilterVendor.FilterDescription = this.lblFilterName.Text;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnAddVisible = true;
            this.Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            itemVendorViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(itemVendorViewDTO.MessageStatus.Message);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridVendor()
        {
            grdSearchVendors.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchVendors.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridItems()
        {
            grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!itemVendorViewDTO.hasConfigurationError() && itemVendorViewDTO.Configuration != null && itemVendorViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, itemVendorViewDTO.Configuration);

            grdMgr.DataSource = itemVendorViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(itemVendorViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false); ;
        }

        protected void ReloadData()
        {
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;

            //Actualiza grilla
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                currentIndex = -1;
                this.Master.ucError.ClearError();
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                ReloadData();
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                // currentIndex --> Item actual.
                ShowModal(currentIndex, CRUD.Create);
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }


        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar Documento
            if (mode == CRUD.Update)
            {
                this.ddlOwner.SelectedValue = itemVendorViewDTO.Entities[index].Owner.Id.ToString();
                this.hidVendorId.Value = itemVendorViewDTO.Entities[index].Vendor.Id.ToString();                
                this.hidItemId.Value = itemVendorViewDTO.Entities[index].Item.Id.ToString();
                this.txtVendorCode.Text = itemVendorViewDTO.Entities[index].Vendor.Code;
                this.txtItemCode.Text = itemVendorViewDTO.Entities[index].Item.Code;
                this.txtVendorName.Text = itemVendorViewDTO.Entities[index].Vendor.Name;
                this.txtItemName.Text = itemVendorViewDTO.Entities[index].Item.ShortName;
                this.txtItemCodeVendor.Text = itemVendorViewDTO.Entities[index].ItemCodeVendor;
                this.txtLongItemName.Text = itemVendorViewDTO.Entities[index].LongItemName;
                this.chkStatus.Checked = itemVendorViewDTO.Entities[index].Status;

                this.txtSpecialField1.Text = itemVendorViewDTO.Entities[index].SpecialField1;
                this.txtSpecialField2.Text = itemVendorViewDTO.Entities[index].SpecialField2;
                this.txtSpecialField3.Text = itemVendorViewDTO.Entities[index].SpecialField3;
                this.txtSpecialField4.Text = itemVendorViewDTO.Entities[index].SpecialField4;

                this.ddlOwner.Enabled = false;
                this.txtItemCode.Enabled = false;
                this.txtVendorName.Enabled = false;
                this.txtVendorCode.Enabled = false;
                this.txtItemCode.Enabled = false;
                this.txtItemCodeVendor.Enabled = false;
                this.imgbtnSearchItem.Enabled = false;
                this.imgBtnCustmerSearch.Enabled = false;

                lblNew.Visible = false;
                lblEdit.Visible = true;                
            }

            // Nuevo Documento
            if (mode == CRUD.Create)
            {
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                this.hidVendorId.Value = "0";
                this.hidItemId.Value = "0";
                this.txtVendorCode.Text = string.Empty;
                this.txtItemCode.Text = string.Empty;
                this.txtVendorName.Text = string.Empty;
                this.txtItemName.Text = string.Empty;
                this.txtItemCodeVendor.Text= string.Empty;
                this.txtLongItemName.Text = string.Empty;
                this.chkStatus.Checked = false;

                this.txtSpecialField1.Text = string.Empty;
                this.txtSpecialField2.Text = string.Empty;
                this.txtSpecialField3.Text = string.Empty;
                this.txtSpecialField4.Text = string.Empty;

                this.ddlOwner.Enabled = true;
                this.txtItemCode.Enabled = false;
                this.txtVendorName.Enabled = false;
                this.txtVendorCode.Enabled = true;
                this.txtItemCode.Enabled = true;
                this.txtItemCodeVendor.Enabled = true;
                this.imgbtnSearchItem.Enabled = true;
                this.imgBtnCustmerSearch.Enabled = true;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (itemVendorViewDTO != null && itemVendorViewDTO.Configuration != null && itemVendorViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(itemVendorViewDTO.Configuration, true);
                else
                    base.ConfigureModal(itemVendorViewDTO.Configuration, false);
            }
            

            divGrid.Visible = false;
            divModal.Visible = true;
        }


        private void SaveChanges()
        {
            ItemVendor newItemVendor = new ItemVendor();
            newItemVendor.Vendor = new Vendor(int.Parse(this.hidVendorId.Value.Trim()));
            newItemVendor.Vendor.Code = this.txtVendorCode.Text.Trim();
            newItemVendor.Owner = new Owner(int.Parse(this.ddlOwner.SelectedValue.Trim()));
            newItemVendor.Item = new Item(int.Parse(this.hidItemId.Value.Trim()));
            newItemVendor.Item.Code = this.txtItemCode.Text.Trim();
            newItemVendor.ItemCodeVendor = this.txtItemCodeVendor.Text.Trim();
            newItemVendor.LongItemName = this.txtLongItemName.Text.Trim();
            newItemVendor.Status = this.chkStatus.Checked;

            if (this.txtSpecialField1.Enabled)
                newItemVendor.SpecialField1 = this.txtSpecialField1.Text.Trim();

            if (this.txtSpecialField2.Enabled)
                newItemVendor.SpecialField2 = this.txtSpecialField2.Text.Trim();

            if (this.txtSpecialField3.Enabled)
                newItemVendor.SpecialField3 = this.txtSpecialField3.Text.Trim();

            if (this.txtSpecialField4.Enabled)
                newItemVendor.SpecialField4 = this.txtSpecialField4.Text.Trim();
            
            if (lblNew.Visible)
                itemVendorViewDTO = iWarehousingMGR.MaintainItemVendor(CRUD.Create, newItemVendor, context);
            else
                itemVendorViewDTO = iWarehousingMGR.MaintainItemVendor(CRUD.Update, newItemVendor, context);

            divGrid.Visible = true;
            divModal.Visible = false;

            if (itemVendorViewDTO.hasError())
            {
                UpdateSession(true);
                divGrid.Visible = false;
                divModal.Visible = true;
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(itemVendorViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }

        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            
            //eliminar el item Vendor
            itemVendorViewDTO = iWarehousingMGR.MaintainItemVendor(CRUD.Delete, (itemVendorViewDTO.Entities[index]), context);

            if (itemVendorViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(itemVendorViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
            //ucFilterVendor.Clear();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
                itemVendorViewDTO.ClearError();
            }

            // Carga lista de outboundDetail por id de outboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            itemVendorViewDTO = iWarehousingMGR.FindAllItemVendor(context);

            if (!itemVendorViewDTO.hasError() && itemVendorViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemVendorMgr.ItemVendorList, itemVendorViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(itemVendorViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected void imgCloseNewEdit_Click(object sender, ImageClickEventArgs e)
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Agrega el item seleccionado de la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdSearchVendors_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchVendors.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.VendorList))
                {
                    VendorSearchViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.Shared.VendorList];

                    foreach (Vendor Vendor in VendorSearchViewDTO.Entities)
                    {
                        if (Vendor.Id == editIndex)
                        {
                            this.txtVendorCode.Text = Vendor.Code;
                            this.txtVendorName.Text = Vendor.Name;

                            hidVendorId.Value = Vendor.Id.ToString();
                            Session.Add("ItemVendorMgrVendor", Vendor);

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                            this.txtItemName.Text = item.ShortName;
                            hidItemId.Value = item.Id.ToString();

                            Session.Add("ItemVendorMgrItem", item);

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearchItem_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected void btnSearchVendor_Click(object sender, EventArgs e)
        {
            try
            {
                SearchVendor();

                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    divLookupVendor.Visible = true;
                    mpLookupVendor.Show();

                    InitializePageCountVendor();
                }
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected void btnFirstGrdSearchVendors_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchVendors.SelectedIndex = 0;
            ddlPagesSearchVendorsSelectedIndexChanged(sender, e);
        }

        protected void btnFirstGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = 0;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void btnPrevGrdSearchVendors_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageVendor > 0)
            {
                ddlPagesSearchVendors.SelectedIndex = currentPageVendor - 1; ;
                ddlPagesSearchVendorsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnPrevGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageItems > 0)
            {
                ddlPagesSearchItems.SelectedIndex = currentPageItems - 1; ;
                ddlPagesSearchItemsSelectedIndexChanged(sender, e);
            }
        }

        protected void btnLastGrdSearchVendors_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchVendors.SelectedIndex = grdSearchVendors.PageCount - 1;
            ddlPagesSearchVendorsSelectedIndexChanged(sender, e);
        }

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);
        }

        protected void btnNextGrdSearchVendors_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageVendor < grdSearchVendors.PageCount)
            {
                ddlPagesSearchVendors.SelectedIndex = currentPageVendor + 1; ;
                ddlPagesSearchVendorsSelectedIndexChanged(sender, e);
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

        protected void ddlPagesSearchVendorsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.VendorList))
            {
                VendorSearchViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.Shared.VendorList];

                currentPageVendor = ddlPagesSearchVendors.SelectedIndex;
                grdSearchVendors.PageIndex = currentPageVendor;

                // Configura ORDEN de las columnas de la grilla
                //if (!VendorSearchViewDTO.hasConfigurationError() && VendorSearchViewDTO.Configuration != null && VendorSearchViewDTO.Configuration.Count > 0)
                //    base.ConfigureGridOrder(grdSearchVendors, VendorSearchViewDTO.Configuration);

                // Encabezado de Recepciones
                grdSearchVendors.DataSource = VendorSearchViewDTO.Entities;
                grdSearchVendors.DataBind();

                divLookupVendor.Visible = true;
                mpLookupVendor.Show();

                ShowVendorButtonsPage();
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

        protected void imgBtnVendorSearch_Click(object sender, ImageClickEventArgs e)
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

                        bool validVendor = false;

                        // Busca en base de datos el Vendor ingresado 
                        if (txtVendorCode.Text.Trim() != string.Empty)
                        {
                            VendorSearchViewDTO = new GenericViewDTO<Vendor>();
                            Vendor Vendor = new Vendor();
                            Vendor.Code = this.txtVendorCode.Text;
                            //Vendor.Name = this.txtDescription.Text;

                            validVendor = false;
                        }

                        // Si no es válido o no se ingresó, se muestra la lista de Vendors para seleccionar uno
                        if (!validVendor)
                        {
                            ucFilterVendor.Clear();
                            ucFilterVendor.Initialize();

                            // Setea el filtro con el Vendor ingresado
                            if (txtVendorCode.Text.Trim() != string.Empty)
                            {
                                FilterItem filterItem = new FilterItem("%" + txtVendorCode.Text + "%");
                                filterItem.Selected = true;
                                ucFilterVendor.FilterItems[0] = filterItem;
                                ucFilterVendor.LoadCurrentFilter(ucFilterVendor.FilterItems);
                                SearchVendor();
                            }
                            // Si no se ingresó ningún Vendor, no se ejecuta la búsqueda
                            else
                            {
                                ClearGridVendor();
                            }

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = false;
                            //valSearchVendor.Enabled = false;

                            divLookupVendor.Visible = true;
                            mpLookupVendor.Show();

                            InitializePageCountVendor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
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

                    if (this.ddlOwner.SelectedValue == "-1")
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
                            itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtItemCode.Text.Trim(), Convert.ToInt16(ddlOwner.SelectedValue), false);
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            // Selecciona owner seleccionado en el Filtro
            this.ddlOwnerLoad.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

            divGrid.Visible = true;
            divModal.Visible = false;

            divLoad.Visible = true;
            modalPopUpLoad.Show();
        }

        protected void btnSubir2_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile2.HasFile)
                {
                    int idOwn = int.Parse(this.ddlOwnerLoad.SelectedValue);

                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile2.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile2.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(ex.Message);
                    }

                    DataTable dataTable;

                    try
                    {
                        dataTable = ConvertXlsToDataTable(savePath, 1);
                    }
                    catch(Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       VendorCode = r.Field<object>("VendorCode"),
                                       IdOwn = idOwn,
                                       ItemCode = r.Field<object>("ItemCode"),
                                       ItemCodeVendor = r.Field<object>("ItemCodeVendor"),
                                       LongItemName = r.Field<object>("LongItemName"),
                                       Status = r.Field<object>("Status"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")
                                   };

                    itemVendorViewDTO = new GenericViewDTO<ItemVendor>();

                    try
                    {

                        foreach (var item in lstExcel)
                        {
                            ItemVendor newItem = new ItemVendor();

                            newItem.Owner = new Owner(idOwn);

                            if (!ValidateIsNotNull(item.ItemCodeVendor))
                            {
                                errorUp = "ItemCodeVendor " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.ItemCodeVendor = item.ItemCodeVendor.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.VendorCode))
                            {
                                errorUp = "ItemCodeVendor " + newItem.ItemCodeVendor + " - VendorCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Vendor = new Vendor();
                                newItem.Vendor.Code = item.VendorCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.ItemCode))
                            {
                                errorUp = "ItemCodeVendor " + newItem.ItemCodeVendor + " - ItemCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Item = new Item();
                                newItem.Item.Code = item.ItemCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.LongItemName))
                            {
                                errorUp = "ItemCodeVendor " + newItem.ItemCodeVendor + " - LongItemName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.LongItemName = item.LongItemName.ToString().Trim();
                            }                       

                            if (!ValidateIsNotNull(item.Status))
                            {
                                errorUp = "ItemCodeVendor " + newItem.ItemCodeVendor + " - Status " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (item.Status.ToString().Trim() == "0")
                                {
                                    newItem.Status = false;
                                }
                                else if (item.Status.ToString().Trim() == "1")
                                {
                                    newItem.Status = true;
                                }
                                else
                                {
                                    bool Status;
                                    if (bool.TryParse(item.Status.ToString().Trim(), out Status))
                                    {
                                        newItem.Status = Status;
                                    }
                                    else
                                    {
                                        errorUp = "ItemCodeVendor " + newItem.ItemCodeVendor + " - Status " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }
                            }

                            if (ValidateIsNotNull(item.SpecialField1))
                                newItem.SpecialField1 = item.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField2))
                                newItem.SpecialField2 = item.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField3))
                                newItem.SpecialField3 = item.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField4))
                                newItem.SpecialField4 = item.SpecialField4.ToString().Trim();

                            itemVendorViewDTO.Entities.Add(newItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        divFondoPopupProgress.Visible = false;
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (itemVendorViewDTO.Entities.Count > 0)
                        {
                            itemVendorViewDTO = iWarehousingMGR.MaintainItemVendorMassive(itemVendorViewDTO, context); 

                            if (itemVendorViewDTO.hasError())
                            {
                                //UpdateSession(true);
                                ShowAlertLocal(this.lblTitle.Text, itemVendorViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(itemVendorViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                            divFondoPopupProgress.Visible = false;
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }

                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divFondoPopupProgress.Visible = false;
                    divLoad.Visible = true;
                    modalPopUpLoad.Show();
                }

            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(itemViewDTO.Errors);
                ShowAlertLocal(this.lblTitle.Text, itemVendorViewDTO.Errors.Message);
            }
            finally
            {
                //Pregunta si existe el archivo y lo elimina
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
            }
        }

        

        private void ShowVendorButtonsPage()
        {
            if (currentPageVendor == grdSearchVendors.PageCount - 1)
            {
                btnNextGrdSearchVendors.Enabled = false;
                btnNextGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchVendors.Enabled = false;
                btnLastGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchVendors.Enabled = true;
                btnPrevGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchVendors.Enabled = true;
                btnFirstGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageVendor == 0)
                {
                    btnPrevGrdSearchVendors.Enabled = false;
                    btnPrevGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchVendors.Enabled = false;
                    btnFirstGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchVendors.Enabled = true;
                    btnNextGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchVendors.Enabled = true;
                    btnLastGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchVendors.Enabled = true;
                    btnNextGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchVendors.Enabled = true;
                    btnLastGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchVendors.Enabled = true;
                    btnPrevGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchVendors.Enabled = true;
                    btnFirstGrdSearchVendors.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
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
                Session.Add("ItemVendorMgrItem", item);                              
                
                this.txtItemCode.Text = item.Code;
                //this.txtDescription.Text = item.Description;
                hidItemId.Value = item.Id.ToString();
               
            }
        }

        private void InitializePageCountVendor()
        {
            if (grdSearchVendors.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchVendors.Visible = true;
                // Paginador
                ddlPagesSearchVendors.Items.Clear();
                for (int i = 0; i < grdSearchVendors.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageVendor) lstItem.Selected = true;

                    ddlPagesSearchVendors.Items.Add(lstItem);
                }
                this.lblPageCountSearchVendors.Text = grdSearchVendors.PageCount.ToString();

                ShowVendorButtonsPage();
            }
            else
            {
                divPageGrdSearchVendors.Visible = false;
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

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
        }

        private void ClearGridVendor()
        {
            Session.Remove(WMSTekSessions.Shared.VendorList);
            grdSearchVendors.DataSource = null;
            grdSearchVendors.DataBind();
        }

        private void SearchVendor()
        {
            try
            {
                GenericViewDTO<Vendor> VendorViewDTO = new GenericViewDTO<Vendor>();

                int idOwn = Convert.ToInt32(this.ddlOwner.SelectedValue);

                VendorViewDTO = iWarehousingMGR.FindByVendorCodeVendorName(ucFilterVendor.FilterItems, context, idOwn);

                Session.Add(WMSTekSessions.Shared.VendorList, VendorViewDTO);

                InitializeGridVendor();
                grdSearchVendors.DataSource = VendorViewDTO.Entities;
                grdSearchVendors.DataBind();
                //}
            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        private void SearchItem()
        {
            try
            {
                itemSearchViewDTO = new GenericViewDTO<Item>();

                itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, Convert.ToInt16(ddlOwner.SelectedValue), true);
                Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
                InitializeGridItems();
                grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                grdSearchItems.DataBind();

            }
            catch (Exception ex)
            {
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }

        protected void grdSearchItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdSearchVendors_RowDataBound(object sender, GridViewRowEventArgs e)
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
                itemVendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemVendorViewDTO.Errors);
            }
        }
    }
}
