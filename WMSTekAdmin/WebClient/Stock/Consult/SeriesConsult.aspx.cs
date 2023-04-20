using System;
using System.Data.SqlTypes;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using System.Linq;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace Binaria.WMSTek.WebClient.Stocks
{
	public partial class SeriesConsult : BasePage
	{
        #region "Declaración de Variables"

        private GenericViewDTO<Serial> serialViewDTO = new GenericViewDTO<Serial>();
        private GenericViewDTO<Item> itemSearchViewDTO;
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
        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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

                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }


        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var currentSerial = serialViewDTO.Entities[e.Row.DataItemIndex];

                    var btnCreate = e.Row.FindControl("btnCreate") as ImageButton;
                    var btnDelete = e.Row.FindControl("btnDelete") as ImageButton;
                    var btnEdit = e.Row.FindControl("btnEdit") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    if (currentSerial.Id <= 0)
                    {
                        btnDelete.Visible = false;
                        btnEdit.Visible = false;
                        btnCreate.Visible = true;
                    }
                    else
                    {
                        btnDelete.Visible = true;
                        btnEdit.Visible = true;
                        btnCreate.Visible = false;
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    }  
                }
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;
                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
        //        serialViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(serialViewDTO.Errors);
        //    }
        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {

                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }

        }

        protected void btnMassiveLoad_Click(object sender, EventArgs e)
        {
            try
            {
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Create")
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                    ShowModal(index, CRUD.Create);
                }
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        protected void imgBtnSearchItem_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                bool validItem = false;
                bool existingItem = false;

                // Busca en base de datos el Item ingresado 
                if (txtItemCode.Text.Trim() != string.Empty)
                {
                    itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, txtItemCode.Text.Trim(), Master.ucMainFilter.idOwn, false);
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
                    rfvSummary.Enabled = false;

                    divLookupItem.Visible = true;
                    mpLookupItem.Show();

                    InitializePageCountItems();
                }

                CallJsGridNoDragAndDrop();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        private void ClearGridItem()
        {
            Session.Remove(WMSTekSessions.Shared.ItemList);
            grdSearchItems.DataSource = null;
            grdSearchItems.DataBind();
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
                            lblItemNameTitle.Visible = true;
                            this.lblItemName.Text = item.ShortName;
                            hidItemId.Value = item.Id.ToString();

                            Session.Add("ItemVendorMgrItem", item);

                            // Esto evita un bug de ajax
                            rfvSummary.Enabled = true;
                            break;
                        }
                    }
                }

                modalPopUp.Show();

                divLookupItem.Visible = false;
                mpLookupItem.Hide();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        protected void btnSubir_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile.HasFile)
                {
                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(ex.Message);
                    }


                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                "Data Source=" + savePath + ";" +
                                                "Extended Properties=Excel 8.0;";

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", connectionString);
                    DataSet myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        dataAdapter.Fill(myDataSet);
                        dataTable = myDataSet.Tables["Table"];
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var idOwn = Master.ucMainFilter.idOwn;
                    var idWhs = Master.ucMainFilter.idWhs;

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       SerialNumber = r.Field<object>("SerialNumber"),
                                       ItemCode = r.Field<object>("ItemCode"),
                                       LpnCode = r.Field<object>("LpnCode")
                                   };

                    serialViewDTO = new GenericViewDTO<Serial>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            var serial = new Serial();

                            serial.Owner = new Owner(idOwn);
                            serial.Warehouse = new Warehouse(idWhs);
                            serial.IdTrackSerialType = (int)eTrackSerialType.Inserted;

                            if (!ValidateIsNotNull(item.SerialNumber))
                            {
                                errorUp = "SerialNumber " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                serial.SerialNumber = item.SerialNumber.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.ItemCode))
                            {
                                errorUp = "SerialNumber " + item.SerialNumber + " - ItemCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                serial.Item = new Item();
                                serial.Item.Code = item.ItemCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.LpnCode))
                            {
                                errorUp = "SerialNumber " + item.SerialNumber + " - LpnCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                serial.Stock = new Framework.Entities.Warehousing.Stock();
                                serial.Stock.Lpn = new LPN();
                                serial.Stock.Lpn.Code = item.LpnCode.ToString().Trim();
                            }

                            serialViewDTO.Entities.Add(serial);
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
                        if (serialViewDTO.Entities.Count > 0)
                        {
                            serialViewDTO = iWarehousingMGR.MaintainSerialMassive(serialViewDTO, context);

                            if (serialViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, serialViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(serialViewDTO.MessageStatus.Message);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, serialViewDTO.Errors.Message);
            }
            finally
            {
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
            }
        }

        protected void grdSearchItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"


        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {

            }
            else
            {
                if (ValidateSession(WMSTekSessions.StockConsult.SerialList))
                {
                    serialViewDTO = (GenericViewDTO<Serial>)Session[WMSTekSessions.StockConsult.SerialList];
                    isValidViewDTO = true;
                }

                if (isValidViewDTO)
                {
                    PopulateGrid();
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //FILTRO BASICO
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = "Serie";
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = "LPN";

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnMassiveLoad_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            Master.ucTaskBar.btnAddVisible = true;
            //Master.ucTaskBar.btnNewVisible = true;
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
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // carga consulta de Serie
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            //var txtFilterCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");

            //if (string.IsNullOrEmpty(txtFilterCode.Text))
            //{
            //    ucStatus.ShowWarning("Debe ingresar un LPN");
            //    return;
            //}

            serialViewDTO = iWarehousingMGR.FindAllSerial(context);

            if (!serialViewDTO.hasError() && serialViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.SerialList, serialViewDTO);
                
                isValidViewDTO = true;

                ucStatus.ShowMessage(serialViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!serialViewDTO.hasConfigurationError() && serialViewDTO.Configuration != null && serialViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, serialViewDTO.Configuration);

            grdMgr.DataSource = serialViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(serialViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        protected void ShowModal(int index, CRUD mode)
        {
            serialViewDTO = (GenericViewDTO<Serial>)Session[WMSTekSessions.StockConsult.SerialList];

            var serialSelected = serialViewDTO.Entities[index];
            hidStockId.Value = serialSelected.Stock.Id.ToString();

            if (mode == CRUD.Update)
            {
                hidEditId.Value = serialSelected.Id.ToString();
                hidIdOwn.Value = serialSelected.Owner.Id.ToString();
                hidIdWhs.Value = serialSelected.Warehouse.Id.ToString();

                txtSerialNumber.Text = serialSelected.SerialNumber;
                txtItemCode.Text = serialSelected.Item.Code;
                lblItemNameTitle.Visible = true;
                lblItemName.Text = serialSelected.Item.ShortName;
                hidItemId.Value = serialSelected.Item.Id.ToString();
                //txtInboundOrder.Text = serialSelected.IdInboundOrderLast.ToString() == "-1" ? "" : serialSelected.IdInboundOrderLast.ToString();

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }
            else if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";
                hidIdOwn.Value = serialSelected.Owner.Id.ToString();
                hidIdWhs.Value = serialSelected.Warehouse.Id.ToString();

                txtSerialNumber.Text = string.Empty;
                txtItemCode.Text = serialSelected.Item.Code;
                lblItemNameTitle.Visible = true;
                lblItemName.Text = serialSelected.Item.ShortName;              
                hidItemId.Value = serialSelected.Item.Id.ToString();
                //txtInboundOrder.Text = string.Empty;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
            CallJsGridView();
        }

        private void DeleteRow(int index)
        {
            serialViewDTO = (GenericViewDTO<Serial>)Session[WMSTekSessions.StockConsult.SerialList];

            var serialSelected = serialViewDTO.Entities[index];

            var deleteSerialViewDTO = iWarehousingMGR.MaintainSerial(CRUD.Delete, serialSelected, context);

            if (deleteSerialViewDTO.hasError())
            {
                UpdateSession();
                upGrid.Update();
                Master.ucError.ShowError(deleteSerialViewDTO.Errors);
            }
            else
            {
                UpdateSession();
                upGrid.Update();
                ucStatus.ShowMessage(deleteSerialViewDTO.MessageStatus.Message);
            }
        }

        private void SaveChanges()
        {
            serialViewDTO = (GenericViewDTO<Serial>)Session[WMSTekSessions.StockConsult.SerialList];

            int editId = int.Parse(hidEditId.Value);
            int idOwn = int.Parse(hidIdOwn.Value);
            int idWhs = int.Parse(hidIdWhs.Value);

            var serial = new Serial();
            serial.Owner = new Owner(idOwn);
            serial.Warehouse = new Warehouse(idWhs);
            serial.SerialNumber = txtSerialNumber.Text.Trim();
            
            serial.Item = new Item(int.Parse(hidItemId.Value));
            //serial.IdInboundOrderLast = string.IsNullOrEmpty(txtInboundOrder.Text.Trim()) ? -1 : int.Parse(txtInboundOrder.Text.Trim());

            if (editId <= 0)
            {
                var stockIdSelected = int.Parse(hidStockId.Value.Trim());
                var stockSelected = serialViewDTO.Entities.Where(s => s.Stock.Id == stockIdSelected).First();

                serial.IdTrackSerialType = (int)eTrackSerialType.Inserted;
                serial.Stock = new Framework.Entities.Warehousing.Stock();
                serial.Stock.Lpn = new LPN();
                serial.Stock.Lpn.Code = stockSelected.Stock.Lpn.Code;
                serial.Stock.Location = new Location();
                serial.Stock.Location.Code = stockSelected.Stock.Location.Code;
                serial.IdStockLast = stockSelected.Stock.Id;
                serial.Stock.Qty = stockSelected.Stock.Qty;

                serial.IdReceiptLast = stockSelected.Stock.Receipt.Id; 
                serial.IdOutboundOrderLast = stockSelected.Stock.OutboundOrder.Id;
                serial.IdDispatchLast = -1;
                serial.IdInboundOrderLast = stockSelected.Stock.InboundOrder.Id;

                serialViewDTO = iWarehousingMGR.MaintainSerial(CRUD.Create, serial, context);
            }
            else
            {          
                var serialSelected = serialViewDTO.Entities.Where(s => s.Id == editId).First();

                serial.Id = editId;
                serial.IdTrackSerialType = serialSelected.IdTrackSerialType;
                serial.IdStockLast = serialSelected.Stock.Id;

                serial.IdReceiptLast = serialSelected.IdReceiptLast;
                serial.IdOutboundOrderLast = serialSelected.IdOutboundOrderLast;
                serial.IdDispatchLast = serialSelected.IdDispatchLast;
                serial.IdInboundOrderLast = serialSelected.IdInboundOrderLast;

                serialViewDTO = iWarehousingMGR.MaintainSerial(CRUD.Update, serial, context);
            }

            if (serialViewDTO.hasError())
            {             
                Master.ucError.ShowError(serialViewDTO.Errors);
                UpdateSession();
                upGrid.Update();
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                UpdateSession();
                upGrid.Update();
                ucStatus.ShowMessage(serialViewDTO.MessageStatus.Message);
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

        private void SearchItem()
        {
            try
            {
                itemSearchViewDTO = new GenericViewDTO<Item>();

                itemSearchViewDTO = iWarehousingMGR.GetItemByCodeAndOwnerFilter(ucFilterItem.FilterItems, context, Master.ucMainFilter.idOwn, true);
                Session.Add(WMSTekSessions.Shared.ItemList, itemSearchViewDTO);
                InitializeGridItems();
                grdSearchItems.DataSource = itemSearchViewDTO.Entities;
                grdSearchItems.DataBind();

            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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

        private void InitializeGridItems()
        {
            grdSearchItems.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchItems.EmptyDataText = this.Master.EmptyGridText;
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }

        private void CallJsGridNoDragAndDrop()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "initializeGridWithNoDragAndDrop", "initializeGridWithNoDragAndDrop();", true);
        }

        #endregion
       
    }
}
