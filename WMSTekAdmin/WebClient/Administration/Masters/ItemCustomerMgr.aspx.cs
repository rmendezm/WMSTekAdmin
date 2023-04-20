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
using System.Data.OleDb;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using System.Threading;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class ItemCustomerMgr : BasePage
    {
        #region "Declaración de Variables"

        GenericViewDTO<ItemCustomer> itemCustomerViewDTO = new GenericViewDTO<ItemCustomer>();
        private GenericViewDTO<Item> itemSearchViewDTO;
        private GenericViewDTO<Customer> customerSearchViewDTO;
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

        public int currentPageCustomerLoad
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeFilterItem();
            InitializeFilterCustomer();
            InitializeStatusBar();
            InitializeGrid();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            if (!Page.IsPostBack)
            {
                UpdateSession(false);
                PopulateLists();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.ItemCustomerMgr.ItemCustomerList))
                {
                    itemCustomerViewDTO = (GenericViewDTO<ItemCustomer>)Session[WMSTekSessions.ItemCustomerMgr.ItemCustomerList];
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
            this.Master.ucMainFilter.codeLabel = this.lblCodeItemCustomer.Text;
            this.Master.ucMainFilter.nameLabel = this.lblCustomer.Text;

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

            ucFilterCustomerLoad.Initialize();
            ucFilterCustomerLoad.BtnSearchClick += new EventHandler(btnSearchCustomerLoad_Click);

            ucFilterCustomerLoad.FilterCode = this.lblFilterCode.Text;
            ucFilterCustomerLoad.FilterDescription = this.lblFilterName.Text;
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

            this.Master.ucTaskBar.btnAddVisible = true;
            this.Master.ucTaskBar.btnNewVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
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
            itemCustomerViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(itemCustomerViewDTO.MessageStatus.Message);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridCustomer()
        {
            grdSearchCustomers.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchCustomers.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridCustomerLoad()
        {
            grdSearchCustomersLoad.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdSearchCustomersLoad.EmptyDataText = this.Master.EmptyGridText;
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
            if (!itemCustomerViewDTO.hasConfigurationError() && itemCustomerViewDTO.Configuration != null && itemCustomerViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, itemCustomerViewDTO.Configuration);

            grdMgr.DataSource = itemCustomerViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(itemCustomerViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            try
            {
                base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", false, string.Empty, false);

                // Selecciona owner seleccionado en el Filtro
                this.ddlOwnerLoad.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                this.txtCustomerCodeLoad.Text = string.Empty;
                this.txtCustomerNameLoad.Text = string.Empty;

                divModal.Visible = false;
                divGrid.Visible = true;

                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        protected void btnLoadFile_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";
                
                if (uploadFile.HasFile)
                {
                    int idOwn = int.Parse(this.ddlOwnerLoad.SelectedValue);
                    int idcustomer = int.Parse(this.hidCustomerIdLoad.Value);

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

                    DataTable dataTable;
                    try
                    {
                        dataTable = ConvertXlsToDataTable(savePath, 1);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       IdCustomer = idcustomer,
                                       IdOwn = idOwn,
                                       ItemCode = r.Field<object>("ItemCode"),
                                       ItemCodeCustomer = r.Field<object>("ItemCodeCustomer"),
                                       LongItemName = r.Field<object>("LongItemName"),
                                       Capacity = r.Field<object>("Capacity"),
                                       Price = r.Field<object>("Price"),
                                       Status = r.Field<object>("Status"),
                                       DepartmentItem = r.Field<object>("DepartmentItem"),
                                       DepartmentDescription = r.Field<object>("DepartmentDescription"),
                                       BarCode = r.Field<object>("BarCode"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")

                                   };

                    GenericViewDTO<Item> itemViewDTO = iWarehousingMGR.GetItemByIdOwner(context, idOwn);
                    GenericViewDTO<Owner> ownViewDTOLoad = iWarehousingMGR.GetOwnerById(context, idOwn);
                    GenericViewDTO<Customer> customerViewDTOLoad = iWarehousingMGR.GetCustomerById(context, idcustomer);

                    try
                    {
                        itemCustomerViewDTO = new GenericViewDTO<ItemCustomer>();
                        foreach (var itemCustomer in lstExcel)
                        {
                            ItemCustomer newItem = new ItemCustomer();
                            newItem.Customer = customerViewDTOLoad.Entities[0];
                            newItem.Owner = ownViewDTOLoad.Entities[0];

                            if (!ValidateIsNotNull(itemCustomer.ItemCode))
                            {
                                errorUp = "ItemCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (itemViewDTO.Entities.Exists(w => w.Code.ToUpper().Equals(itemCustomer.ItemCode)))
                                {
                                    newItem.Item = itemViewDTO.Entities.Where(w => w.Code.ToUpper().Equals(itemCustomer.ItemCode)).First();
                                }
                                else
                                {
                                    errorUp = "El ItemCode " + itemCustomer.ItemCode + " no es valido para el dueño " + this.ddlOwnerLoad.SelectedItem.Text;
                                    break;
                                }

                            }

                            if (!ValidateIsNotNull(itemCustomer.ItemCodeCustomer))
                            {
                                errorUp = "ItemCodeCustomer " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.ItemCodeCustomer = itemCustomer.ItemCodeCustomer.ToString().Trim();
                            }

                            if (ValidateIsNotNull(itemCustomer.LongItemName))
                                newItem.LongItemName = itemCustomer.LongItemName.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.Capacity))
                                newItem.Capacity = Convert.ToDecimal(itemCustomer.Capacity);

                            if (!ValidateIsNotNull(itemCustomer.Status))
                            {
                                errorUp = "ItemCodeCustomer " + newItem.ItemCodeCustomer +  " - Status " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newItem.Status = itemCustomer.Status.ToString().Equals("1") ? true : false;
                            }

                            if (ValidateIsNotNull(itemCustomer.Price))
                                newItem.Price = Convert.ToDecimal(itemCustomer.Price);

                            if (ValidateIsNotNull(itemCustomer.SpecialField1))
                                newItem.SpecialField1 = itemCustomer.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.SpecialField2))
                                newItem.SpecialField2 = itemCustomer.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.SpecialField3))
                                newItem.SpecialField3 = itemCustomer.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.SpecialField4))
                                newItem.SpecialField4 = itemCustomer.SpecialField4.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.DepartmentItem))
                                newItem.DepartmentItem = itemCustomer.DepartmentItem.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.DepartmentDescription))
                                newItem.DepartmentDescription = itemCustomer.DepartmentDescription.ToString().Trim();

                            if (ValidateIsNotNull(itemCustomer.BarCode))
                                newItem.BarCode = itemCustomer.BarCode.ToString().Trim();

                            itemCustomerViewDTO.Entities.Add(newItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (itemCustomerViewDTO.Entities.Count > 0)
                        {
                            itemCustomerViewDTO = iWarehousingMGR.MaintainMassiveItemCustomer(itemCustomerViewDTO, context);

                            if (itemCustomerViewDTO.hasError())
                            {
                                //UpdateSession(true);
                                ShowAlertLocal(this.lblTitle.Text, itemCustomerViewDTO.Errors.Message);
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(itemCustomerViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                                                                
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }

                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divLoad.Visible = true;
                    modalPopUpLoad.Show();
                }

            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(itemViewDTO.Errors);
                ShowAlertLocal(this.lblTitle.Text, itemCustomerViewDTO.Errors.Message);
            }
            finally
            {
                //Pregunta si existe el archivo y lo elimina
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }

                UpdateSession(false);
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
                this.ddlOwner.SelectedValue = itemCustomerViewDTO.Entities[index].Owner.Id.ToString();
                this.hidCustomerId.Value = itemCustomerViewDTO.Entities[index].Customer.Id.ToString();                
                this.hidItemId.Value = itemCustomerViewDTO.Entities[index].Item.Id.ToString();
                this.txtCustomerCode.Text = itemCustomerViewDTO.Entities[index].Customer.Code;
                this.txtItemCode.Text = itemCustomerViewDTO.Entities[index].Item.Code;
                this.txtCustomerName.Text = itemCustomerViewDTO.Entities[index].Customer.Name;
                this.txtItemName.Text = itemCustomerViewDTO.Entities[index].Item.ShortName;
                this.txtItemCodeCustomer.Text = itemCustomerViewDTO.Entities[index].ItemCodeCustomer;
                this.txtLongItemName.Text = itemCustomerViewDTO.Entities[index].LongItemName;
                this.txtCapacity.Text = itemCustomerViewDTO.Entities[index].Capacity.ToString();
                this.txtPrice.Text = itemCustomerViewDTO.Entities[index].Price.ToString();
                this.chkStatus.Checked = itemCustomerViewDTO.Entities[index].Status;
                txtDepartmentItem.Text = itemCustomerViewDTO.Entities[index].DepartmentItem;
                txtDepartmentDescription.Text = itemCustomerViewDTO.Entities[index].DepartmentDescription;
                txtBarCode.Text = itemCustomerViewDTO.Entities[index].BarCode;

                this.txtSpecialField1.Text = itemCustomerViewDTO.Entities[index].SpecialField1;
                this.txtSpecialField2.Text = itemCustomerViewDTO.Entities[index].SpecialField2;
                this.txtSpecialField3.Text = itemCustomerViewDTO.Entities[index].SpecialField3;
                this.txtSpecialField4.Text = itemCustomerViewDTO.Entities[index].SpecialField4;

                this.ddlOwner.Enabled = false;
                this.txtItemCode.Enabled = false;
                this.txtCustomerName.Enabled = false;
                this.txtCustomerCode.Enabled = false;
                this.txtItemCode.Enabled = false;
                this.txtItemCodeCustomer.Enabled = false;
                this.imgbtnSearchItem.Enabled = false;
                this.imgBtnCustmerSearch.Enabled = false;

                lblNew.Visible = false;
                lblEdit.Visible = true;                
            }

            // Nuevo Documento
            if (mode == CRUD.Create)
            {
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                this.hidCustomerId.Value = "0";
                this.hidItemId.Value = "0";
                this.txtCustomerCode.Text = string.Empty;
                this.txtItemCode.Text = string.Empty;
                this.txtCustomerName.Text = string.Empty;
                this.txtItemName.Text = string.Empty;
                this.txtItemCodeCustomer.Text= string.Empty;
                this.txtLongItemName.Text = string.Empty;
                this.txtCapacity.Text = string.Empty;
                this.txtPrice.Text = string.Empty;
                this.chkStatus.Checked = false;

                this.txtSpecialField1.Text = string.Empty;
                this.txtSpecialField2.Text = string.Empty;
                this.txtSpecialField3.Text = string.Empty;
                this.txtSpecialField4.Text = string.Empty;

                txtDepartmentItem.Text = string.Empty;
                txtDepartmentDescription.Text = string.Empty;
                txtBarCode.Text = string.Empty;

                this.ddlOwner.Enabled = true;
                this.txtItemCode.Enabled = false;
                this.txtCustomerName.Enabled = false;
                this.txtCustomerCode.Enabled = true;
                this.txtItemCode.Enabled = true;
                this.txtItemCodeCustomer.Enabled = true;
                this.imgbtnSearchItem.Enabled = true;
                this.imgBtnCustmerSearch.Enabled = true;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (itemCustomerViewDTO != null && itemCustomerViewDTO.Configuration != null && itemCustomerViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(itemCustomerViewDTO.Configuration, true);
                else
                    base.ConfigureModal(itemCustomerViewDTO.Configuration, false);
            }

            //if(mode == CRUD.Update)
            //    txtItemCodeCustomer.Enabled = false;

            divGrid.Visible = false;
            divModal.Visible = true;
        }


        private void SaveChanges()
        {
            ItemCustomer newItemCustomer = new ItemCustomer();
            newItemCustomer.Customer = new Customer(int.Parse(this.hidCustomerId.Value.Trim()));
            newItemCustomer.Customer.Code = this.txtCustomerCode.Text.Trim();
            newItemCustomer.Owner = new Owner(int.Parse(this.ddlOwner.SelectedValue.Trim()));
            newItemCustomer.Item = new Item(int.Parse(this.hidItemId.Value.Trim()));
            newItemCustomer.Item.Code = this.txtItemCode.Text.Trim();
            newItemCustomer.ItemCodeCustomer = this.txtItemCodeCustomer.Text.Trim();
            newItemCustomer.LongItemName = this.txtLongItemName.Text.Trim();
            newItemCustomer.Capacity = decimal.Parse( this.txtCapacity.Text.Trim());
            newItemCustomer.Price = decimal.Parse(this.txtPrice.Text.Trim());
            newItemCustomer.Status = this.chkStatus.Checked;
            newItemCustomer.DepartmentItem = txtDepartmentItem.Text.Trim() == "" ? null : txtDepartmentItem.Text.Trim();
            newItemCustomer.DepartmentDescription = txtDepartmentDescription.Text.Trim() == "" ? null : txtDepartmentDescription.Text.Trim();
            newItemCustomer.BarCode = txtBarCode.Text.Trim() == "" ? null : txtBarCode.Text.Trim();

            if (this.txtSpecialField1.Enabled)
                newItemCustomer.SpecialField1 = this.txtSpecialField1.Text.Trim();

            if (this.txtSpecialField2.Enabled)
                newItemCustomer.SpecialField2 = this.txtSpecialField2.Text.Trim();

            if (this.txtSpecialField3.Enabled)
                newItemCustomer.SpecialField3 = this.txtSpecialField3.Text.Trim();

            if (this.txtSpecialField4.Enabled)
                newItemCustomer.SpecialField4 = this.txtSpecialField4.Text.Trim();
            
            if (lblNew.Visible)
                itemCustomerViewDTO = iWarehousingMGR.MaintainItemCustomer(CRUD.Create, newItemCustomer, context);
            else
                itemCustomerViewDTO = iWarehousingMGR.MaintainItemCustomer(CRUD.Update, newItemCustomer, context);

            divGrid.Visible = true;
            divModal.Visible = false;

            if (itemCustomerViewDTO.hasError())
            {
                UpdateSession(true);
                divGrid.Visible = false;
                divModal.Visible = true;
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(itemCustomerViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }

        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {

            //hidEditId.Value = itemCustomerViewDTO.Entities[index].Id.ToString();

            ////llena objeto detalle
            //int id = outboundOrderViewDTO.Entities[index].Id;


            //eliminar el item customer
            itemCustomerViewDTO = iWarehousingMGR.MaintainItemCustomer(CRUD.Delete, (itemCustomerViewDTO.Entities[index]), context);

            if (itemCustomerViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(itemCustomerViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
            //ucFilterVendor.Clear();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
                itemCustomerViewDTO.ClearError();
            }

            // Carga lista de outboundDetail por id de outboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            itemCustomerViewDTO = iWarehousingMGR.FindAllItemCustomer(context);

            if (!itemCustomerViewDTO.hasError() && itemCustomerViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ItemCustomerMgr.ItemCustomerList, itemCustomerViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(itemCustomerViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                            valEditNew.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        protected void grdSearchCustomersLoad_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int editIndex = (Convert.ToInt32(grdSearchCustomersLoad.DataKeys[((GridViewRow)((Control)e.CommandSource).Parent.Parent).RowIndex].Value));

                if (ValidateSession(WMSTekSessions.Shared.CustomerList))
                {
                    customerSearchViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];

                    foreach (Customer customer in customerSearchViewDTO.Entities)
                    {
                        if (customer.Id == editIndex)
                        {
                            this.txtCustomerCodeLoad.Text = customer.Code;
                            this.txtCustomerNameLoad.Text = customer.Name;

                            hidCustomerIdLoad.Value = customer.Id.ToString();
                            Session.Add("ItemCustomerMgrCustomerLoad", customer);

                            divLookupCustomerLoad.Visible = false;
                            mpLookupCustomerLoad.Hide();

                            divLoad.Visible = true;
                            modalPopUpLoad.Show();

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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

                            Session.Add("ItemCustomerMgrItem", item);

                            // Esto evita un bug de ajax
                            valEditNew.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        protected void btnSearchCustomerLoad_Click(object sender, EventArgs e)
        {
            try
            {
                SearchCustomerLoad();

                // Si es un ViewDTO valido, carga la grilla y las listas
                if (isValidViewDTO)
                {
                    divLookupCustomerLoad.Visible = true;
                    mpLookupCustomerLoad.Show();

                    InitializePageCountCustomerLoad();
                }
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        protected void btnFirstGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomers.SelectedIndex = 0;
            ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
        }

        protected void btnFirstGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = 0;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);

        }

        protected void btnPrevGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer > 0)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer - 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
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

        protected void btnLastGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomers.SelectedIndex = grdSearchCustomers.PageCount - 1;
            ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
        }

        protected void btnLastGrdSearchItems_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchItems.SelectedIndex = grdSearchItems.PageCount - 1;
            ddlPagesSearchItemsSelectedIndexChanged(sender, e);
        }

        protected void btnNextGrdSearchCustomers_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomer < grdSearchCustomers.PageCount)
            {
                ddlPagesSearchCustomers.SelectedIndex = currentPageCustomer + 1; ;
                ddlPagesSearchCustomersSelectedIndexChanged(sender, e);
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

        protected void btnFirstGrdSearchCustomersLoad_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomersLoad.SelectedIndex = 0;
            ddlPagesSearchCustomersSelectedIndexChangedLoad(sender, e);
        }
        protected void btnPrevGrdSearchCustomersLoad_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomerLoad > 0)
            {
                ddlPagesSearchCustomersLoad.SelectedIndex = currentPageCustomerLoad - 1; ;
                ddlPagesSearchCustomersSelectedIndexChangedLoad(sender, e);
            }
        }
        protected void btnLastGrdSearchCustomersLoad_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesSearchCustomersLoad.SelectedIndex = grdSearchCustomersLoad.PageCount - 1;
            ddlPagesSearchCustomersSelectedIndexChangedLoad(sender, e);
        }
        protected void btnNextGrdSearchCustomersLoad_Click(object sender, ImageClickEventArgs e)
        {
            if (currentPageCustomerLoad < grdSearchCustomersLoad.PageCount)
            {
                ddlPagesSearchCustomersLoad.SelectedIndex = currentPageCustomerLoad + 1; ;
                ddlPagesSearchCustomersSelectedIndexChangedLoad(sender, e);
            }
        }
        protected void ddlPagesSearchCustomersSelectedIndexChangedLoad(object sender, EventArgs e)
        {
            if (ValidateSession(WMSTekSessions.Shared.CustomerList))
            {
                customerSearchViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];

                currentPageCustomerLoad = ddlPagesSearchCustomersLoad.SelectedIndex;
                grdSearchCustomersLoad.PageIndex = currentPageCustomerLoad;

               // Encabezado de Recepciones
                grdSearchCustomersLoad.DataSource = customerSearchViewDTO.Entities;
                grdSearchCustomersLoad.DataBind();

                divLookupCustomerLoad.Visible = true;
                mpLookupCustomerLoad.Show();

                ShowCustomerButtonsPage();
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        protected void imgBtnCustmerSearchLoad_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (this.ddlOwnerLoad.SelectedValue == "-1")
                    {
                        //this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblMessajeSelectedOwner.Text, "");
                        ShowAlertLocal(this.lblTitle.Text, this.lblMessajeSelectedOwner.Text);

                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {

                        //bool validCustomer = false;

                        //// Busca en base de datos el Customer ingresado 
                        //if (txtCustomerCodeLoad.Text.Trim() != string.Empty)
                        //{
                        //    customerSearchViewDTO = new GenericViewDTO<Customer>();
                        //    Customer customer = new Customer();
                        //    customer.Code = this.txtCustomerCodeLoad.Text;

                        //    //customerSearchViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomer.FilterItems, context,Convert.ToInt32(this.ddlOwner.SelectedValue));
                        //    validCustomer = false;

                        //}

                        //// Si no es válido o no se ingresó, se muestra la lista de Customers para seleccionar uno
                        //if (!validCustomer)
                        //{
                            ucFilterCustomerLoad.Clear();
                            ucFilterCustomerLoad.Initialize();

                            // Setea el filtro con el Customer ingresado
                            if (txtCustomerCodeLoad.Text.Trim() != string.Empty)
                            {
                                FilterItem filterItem = new FilterItem("%" + txtCustomerCodeLoad.Text + "%");
                                filterItem.Selected = true;
                                ucFilterCustomerLoad.FilterItems[0] = filterItem;
                                ucFilterCustomerLoad.LoadCurrentFilter(ucFilterCustomerLoad.FilterItems);
                                SearchCustomerLoad();
                            }
                            // Si no se ingresó ningún customer, no se ejecuta la búsqueda
                            else
                            {
                                ClearGridCustomerLoad();
                            }

                            // Esto evita un bug de ajax
                           // valEditNew.Enabled = false;
                            //valSearchCustomer.Enabled = false;

                            divLookupCustomerLoad.Visible = true;
                            mpLookupCustomerLoad.Show();

                            divLoad.Visible = false;
                            modalPopUpLoad.Hide();

                            InitializePageCountCustomerLoad();
                       // }
                    }
                }
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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

        private void ShowCustomerButtonsPageLoad()
        {
            if (currentPageCustomerLoad == grdSearchCustomersLoad.PageCount - 1)
            {
                btnNextGrdSearchCustomersLoad.Enabled = false;
                btnNextGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastGrdSearchCustomersLoad.Enabled = false;
                btnLastGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
                btnPrevGrdSearchCustomersLoad.Enabled = true;
                btnPrevGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                btnFirstGrdSearchCustomersLoad.Enabled = true;
                btnFirstGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
            }
            else
            {
                if (currentPageCustomerLoad == 0)
                {
                    btnPrevGrdSearchCustomersLoad.Enabled = false;
                    btnPrevGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstGrdSearchCustomersLoad.Enabled = false;
                    btnFirstGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                    btnNextGrdSearchCustomersLoad.Enabled = true;
                    btnNextGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchCustomersLoad.Enabled = true;
                    btnLastGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnNextGrdSearchCustomersLoad.Enabled = true;
                    btnNextGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastGrdSearchCustomersLoad.Enabled = true;
                    btnLastGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevGrdSearchCustomersLoad.Enabled = true;
                    btnPrevGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstGrdSearchCustomersLoad.Enabled = true;
                    btnFirstGrdSearchCustomersLoad.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
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
                Session.Add("ItemCustomerMgrItem", item);                              
                
                this.txtItemCode.Text = item.Code;
                //this.txtDescription.Text = item.Description;
                hidItemId.Value = item.Id.ToString();
               
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

        private void InitializePageCountCustomerLoad()
        {
            if (grdSearchCustomersLoad.PageCount > 1)
            {
                int pageNumber;

                divPageGrdSearchCustomersLoad.Visible = true;
                // Paginador
                ddlPagesSearchCustomersLoad.Items.Clear();
                for (int i = 0; i < grdSearchCustomersLoad.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPageCustomerLoad) lstItem.Selected = true;

                    ddlPagesSearchCustomersLoad.Items.Add(lstItem);
                }
                this.lblPageCountSearchCustomersLoad.Text = grdSearchCustomersLoad.PageCount.ToString();

                ShowCustomerButtonsPageLoad();
            }
            else
            {
                divPageGrdSearchCustomersLoad.Visible = false;
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

        private void ClearGridCustomer()
        {
            Session.Remove(WMSTekSessions.Shared.CustomerList);
            grdSearchCustomers.DataSource = null;
            grdSearchCustomers.DataBind();
        }

        private void ClearGridCustomerLoad()
        {
            Session.Remove(WMSTekSessions.Shared.CustomerList);
            grdSearchCustomersLoad.DataSource = null;
            grdSearchCustomersLoad.DataBind();
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        private void SearchCustomerLoad()
        {
            try
            {
                customerSearchViewDTO = new GenericViewDTO<Customer>();
                GenericViewDTO<Customer> CustomerViewDTO = new GenericViewDTO<Customer>();

                int idOwn = Convert.ToInt32(this.ddlOwnerLoad.SelectedValue);

                CustomerViewDTO = iWarehousingMGR.FindByOwnerCodeOwnerName(ucFilterCustomerLoad.FilterItems, context, idOwn);

                Session.Add(WMSTekSessions.Shared.CustomerList, CustomerViewDTO);
                //grdMgr.EmptyDataText = this.Master.EmptyGridText;a
                InitializeGridCustomerLoad();
                grdSearchCustomersLoad.DataSource = CustomerViewDTO.Entities;
                grdSearchCustomersLoad.DataBind();
                //}
            }
            catch (Exception ex)
            {
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

        protected void btnCloseCustomerLoad_Click(object sender, ImageClickEventArgs e)
        {

            divLookupCustomerLoad.Visible = false;
            mpLookupCustomerLoad.Hide();

            divLoad.Visible = true;
            modalPopUpLoad.Show();
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

        protected void grdSearchCustomersLoad_RowDataBound(object sender, GridViewRowEventArgs e)
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
                itemCustomerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(itemCustomerViewDTO.Errors);
            }
        }

    }
}
