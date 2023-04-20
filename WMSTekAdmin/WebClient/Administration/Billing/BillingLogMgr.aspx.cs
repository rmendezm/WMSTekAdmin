using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Billing
{
    public partial class BillingLogMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<BillingLog> billingLogViewDTO = new GenericViewDTO<BillingLog>();
        private bool isValidViewDTO = false;
        private bool isNew = false;
        private string idTransactionAdd;

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

                    if (ValidateSession(WMSTekSessions.BillingLogMgr.BillingLogList))
                    {
                        billingLogViewDTO = (GenericViewDTO<BillingLog>)Session[WMSTekSessions.BillingLogMgr.BillingLogList];
                        //Session.Remove(WMSTekSessions.Shared.OwnerList);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;

                    //if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    if ((btnEdit != null))
                    {
                        if (btnEdit != null && billingLogViewDTO.Entities[e.Row.DataItemIndex].Invoiced)
                        {
                            btnEdit.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_edit_dis.png";
                            btnEdit.Enabled = false;
                        }
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
            }
        }

        protected void ddlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlFilterItem = (DropDownList)sender;

            if (ddlOwner.SelectedValue != "-1")
                LoadBillingContract(ddlContract, Convert.ToInt32(ddlOwner.SelectedValue));
            else
                ddlContract.Items.Clear();

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void ddlDocumentInOut_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlFilterItem = (DropDownList)sender;

                divDocumentType.Visible = true;
                divDocumentNumber.Visible = true;

                if (ddlFilterItem.SelectedValue == "IN")
                    LoadDocumentTypeIn(ddlDocumentType);
                else if (ddlFilterItem.SelectedValue == "OUT")
                    LoadDocumentTypeOut(ddlDocumentType);
                else
                {
                    divDocumentType.Visible = false;
                    divDocumentNumber.Visible = false;
                }

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
            }
        }

        #endregion

        #region "Metodos"

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
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
                billingLogViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            billingLogViewDTO = iBillingMGR.FindAllLog(context);

            if (!billingLogViewDTO.hasError() && billingLogViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.BillingLogMgr.BillingLogList, billingLogViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(billingLogViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            if (this.ddlOwner.SelectedValue != "-1")
                base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
            if (this.ddlWarehouse.SelectedValue != "-1")
                base.LoadWarehouses(this.ddlWarehouse, isNew, this.Master.EmptyRowText, "-1");
            if (ddlOwner.SelectedValue != "-1")
                LoadBillingContract(ddlContract, Convert.ToInt32(ddlOwner.SelectedValue));
            if (this.ddlType.SelectedValue != "-1")
                base.LoadBillingType(this.ddlType, new BillingType(), this.Master.EmptyRowText, isNew);
            if (this.ddlMoney.SelectedValue != "-1")
                base.LoadBillingMoney(this.ddlMoney, this.Master.EmptyRowText, isNew);
            if (this.ddlDocumentInOut.SelectedValue != "-1")
            {
                LoadDocumentInOut(this.ddlDocumentInOut);
                LoadDocumentType(this.ddlDocumentType);
            }
                
            if (this.ddlTransaction.SelectedValue != "-1")
            {
                BillingTransaction billingTransactionParam = new BillingTransaction();
                base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);
            }
                
        }

        public void LoadDocumentInOut(DropDownList objControl)
        {
            objControl.Items.Clear();
            ListItem oItem;
            oItem = new ListItem("Seleccione", "-1");
            objControl.Items.Add(oItem);
            oItem = new ListItem("Entrada","IN");
            objControl.Items.Add(oItem);
            oItem = new ListItem("Salida", "OUT");
            objControl.Items.Add(oItem);

        }

        public void LoadDocumentType(DropDownList objControl)
        {
            if (ddlDocumentInOut.SelectedValue == "IN")
                LoadDocumentTypeIn(objControl);
            else
                LoadDocumentTypeOut(objControl);

        }

        public void LoadDocumentTypeIn(DropDownList objControl)
        {
            GenericViewDTO<InboundType> theInboundTypeDTO = new GenericViewDTO<InboundType>();
            theInboundTypeDTO = iReceptionMGR.FindAllInboundType(context);

            objControl.DataSource = theInboundTypeDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();
        }

        public void LoadDocumentTypeOut(DropDownList objControl)
        {
            GenericViewDTO<OutboundType> theOutboundTypeDTO = new GenericViewDTO<OutboundType>();
            theOutboundTypeDTO = iDispatchingMGR.FindAllOutboundType(context);

            objControl.DataSource = theOutboundTypeDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();
        }

        public void LoadBillingContract(DropDownList objControl,int idOwn)
        {
            GenericViewDTO<BillingContract> theBillingContractDTO = new GenericViewDTO<BillingContract>();
            BillingContract theBillingContract = new BillingContract();
            theBillingContract.Owner.Id = idOwn;
            theBillingContractDTO = iBillingMGR.BillingContractGetByAnyParameter(theBillingContract,context);

            objControl.DataSource = theBillingContractDTO.Entities;
            objControl.DataTextField = "Description";
            objControl.DataValueField = "Id";
            objControl.DataBind();
        }

        public void LoadBillingTransactionAdd(DropDownList objControl)
        {
            GenericViewDTO<BillingTransaction> theBillingTransactionDTO = new GenericViewDTO<BillingTransaction>();
            BillingTransaction theBillingTransaction = new BillingTransaction();
            theBillingTransaction.Code = "TRADD";
            theBillingTransactionDTO = iBillingMGR.BillingTransactionGetByAnyParameter(theBillingTransaction, context);

            objControl.DataSource = theBillingTransactionDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!billingLogViewDTO.hasConfigurationError() && billingLogViewDTO.Configuration != null && billingLogViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, billingLogViewDTO.Configuration);

            grdMgr.DataSource = billingLogViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(billingLogViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblName.Text;
            this.Master.ucMainFilter.descriptionLabel = lblDescription.Text;
            this.Master.ucMainFilter.statusLabel = lblStatusName.Text;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

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

        protected void SaveChanges()
        {
            //Agrega los datos del Contrato
            BillingLog log = new BillingLog(Convert.ToInt32(hidEditId.Value));
            GenericViewDTO<BillingTransaction> billingTransactionDTO = new GenericViewDTO<BillingTransaction>();
            GenericViewDTO<BillingMoney> billingMoneyDTO = new GenericViewDTO<BillingMoney>();
            BillingTransaction billingTransactionParam = new BillingTransaction();
            
            log.Owner = new Owner(Convert.ToInt32(this.ddlOwner.SelectedValue));
            log.Owner.Name = this.ddlOwner.SelectedItem.Text;

            log.Warehouse = new Warehouse(Convert.ToInt32(this.ddlWarehouse.SelectedValue));
            log.Warehouse.Name = this.ddlWarehouse.SelectedItem.Text;

            log.BillingContract = new BillingContract();
            log.BillingContract.Description = this.ddlContract.SelectedItem.Text;
            log.BillingContract.Id = Convert.ToInt32(this.ddlContract.SelectedValue);

            log.BillingTransaction = new BillingTransaction();
            log.BillingTransaction.Name = this.ddlTransaction.SelectedItem.Text;
            log.BillingTransaction.Id = Convert.ToInt32(this.ddlTransaction.SelectedValue);
            log.BillingTransactionAddName = this.txtTransactionAdd.Text;

            billingTransactionParam.Id = log.BillingTransaction.Id;
            billingTransactionDTO = iBillingMGR.BillingTransactionGetByAnyParameter(billingTransactionParam, context);

            if (!billingTransactionDTO.hasError())
            {
                if (billingTransactionDTO.Entities.Count > 0)
                    log.BillingTransaction.Type = billingTransactionDTO.Entities[0].Type;
            }

            log.BillingTypeContract.Value = Convert.ToDecimal(txtValue.Text);

            log.BillingType = new BillingType();
            log.BillingType.Name = this.ddlType.SelectedItem.Text;
            log.BillingType.Id = Convert.ToInt32(this.ddlType.SelectedValue);

            //log.BillingType.Name = "";
            //if (log.BillingTransaction.Type != "A")
            //{
            //    log.BillingType.Name = this.ddlType.SelectedItem.Text;
            //    log.BillingType.Id = Convert.ToInt32(this.ddlType.SelectedValue);
            //}
            
            

            log.BillingMoney = new BillingMoney();
            log.BillingMoney.Id = Convert.ToInt32(this.ddlMoney.SelectedValue);
            billingMoneyDTO = iBillingMGR.BillingMoneyGetByAnyParameter(log.BillingMoney,context);
            if (!billingMoneyDTO.hasError())
                log.BillingMoney.Value = billingMoneyDTO.Entities[0].Value;

            log.Qty = Convert.ToDecimal(this.txtQty.Text);

            log.Lpn.IdCode = "";
            log.StateInterface = "";
            log.DocumentInOut = "";
            log.DocumentType = "";
            log.DocumentNumber = "";
            log.ReferenceDocNumber = "";

            if (this.ddlDocumentInOut.SelectedValue != "-1")
            {
                log.DocumentInOut = this.ddlDocumentInOut.SelectedValue;
                log.DocumentType = this.ddlDocumentType.SelectedValue;
                log.DocumentNumber = this.txtDocumentNumber.Text.Trim();
            }
            log.UserName = context.SessionInfo.User.UserName;
            
            if (!string.IsNullOrEmpty(this.txtStartDate.Text)) log.DateEntry = Convert.ToDateTime(this.txtStartDate.Text);

            // Agrega las horas ingresadas
            if (!string.IsNullOrEmpty(this.txtStartDateHours.Text))
                log.DateEntry = log.DateEntry.AddHours(Convert.ToInt16(this.txtStartDateHours.Text));
            // Agrega los minutos seleccionados
            if (!string.IsNullOrEmpty(this.txtStartDateMinutes.Text))
                log.DateEntry = log.DateEntry.AddMinutes(Convert.ToInt16(this.txtStartDateMinutes.Text));


            if (hidEditId.Value == "0")
                billingLogViewDTO = iBillingMGR.MaintainLog(CRUD.Create, log, context);
            else
                billingLogViewDTO = iBillingMGR.MaintainLog(CRUD.Update, log, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (billingLogViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingLogViewDTO.MessageStatus.Message);

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
            var hasCorrectTypeBilling = true;
            // Editar bodega
            if (mode == CRUD.Update)
            {
                // TODO: ver propiedad 'required' para un drop-down list
                isNew = false;
                //PopulateLists();
                base.LoadBillingTransaction(this.ddlTransaction, new BillingTransaction(), this.Master.EmptyRowText, isNew);

                //Recupera los datos de la entidad a editar
                hidEditId.Value = billingLogViewDTO.Entities[index].Id.ToString();

                ddlOwner.SelectedValue = billingLogViewDTO.Entities[index].Owner.Id.ToString();
                ddlWarehouse.SelectedValue = billingLogViewDTO.Entities[index].Warehouse.Id.ToString();
                if (ddlOwner.SelectedValue != "-1")
                    LoadBillingContract(ddlContract, Convert.ToInt32(ddlOwner.SelectedValue));
                ddlContract.SelectedValue = billingLogViewDTO.Entities[index].BillingContract.Id.ToString();
                ddlTransaction.SelectedValue = billingLogViewDTO.Entities[index].BillingTransaction.Id.ToString();

                if (billingLogViewDTO.Entities[index].BillingType.Id > 0)
                {
                    var billingTypeId = billingLogViewDTO.Entities[index].BillingType.Id.ToString();
                    var existsValue = ddlType.Items.FindByValue(billingTypeId);

                    if (existsValue != null)
                        ddlType.SelectedValue = billingTypeId;
                    else
                    {
                        hasCorrectTypeBilling = false;

                        ddlType.Enabled = true;
                        rfvType.Enabled = true;
                       
                        if (ddlType.Items.FindByValue("-1") == null)
                            ddlType.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));

                        ddlType.SelectedValue = "-1";
                    }
                }
                else
                {
                    ddlType.Enabled = true;
                    rfvType.Enabled = true;
                    ddlType.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                    ddlType.SelectedValue = "-1";
                }

                if (billingLogViewDTO.Entities[index].BillingTransaction.Type != "A")
                {
                    divTransactionAdd.Visible = false;
                    txtValue.Enabled = false;
                }
                else
                {
                    divTransactionAdd.Visible = true;
                    txtTransactionAdd.Text = billingLogViewDTO.Entities[index].BillingTransactionAddName;
                    txtValue.Enabled = true;
                }
                    
                txtQty.Text = billingLogViewDTO.Entities[index].Qty.ToString();
                txtValue.Text = billingLogViewDTO.Entities[index].BillingTypeContract.Value.ToString();
                ddlDocumentInOut.SelectedValue = billingLogViewDTO.Entities[index].DocumentInOut;
                if (this.ddlDocumentInOut.SelectedValue != "-1")
                {
                    LoadDocumentType(this.ddlDocumentType);
                }
                ddlDocumentType.SelectedValue = billingLogViewDTO.Entities[index].DocumentType;
                txtDocumentNumber.Text = billingLogViewDTO.Entities[index].DocumentNumber;
                txtStartDate.Text = billingLogViewDTO.Entities[index].DateEntry.ToString("dd-MM-yyyy");

                ddlOwner.Enabled = false;
                ddlWarehouse.Enabled = false;
                ddlTransaction.Enabled = false;
                //ddlType.Enabled = false;
                ddlDocumentInOut.Enabled = false;
                ddlDocumentType.Enabled = false;
                txtDocumentNumber.Enabled = false;
                calStartDate.Enabled = false;
                txtStartDate.Enabled = false;

                //if (billingLogViewDTO.Entities[index].BillingTransaction.Type == "A")
                //{
                //    divType.Visible = false;
                //}

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                // TODO: revisar
                isNew = true;

                //PopulateLists();
                BillingTransaction billingTransactionParam = new BillingTransaction();
                billingTransactionParam.Type = "A";
                base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);

                hidEditId.Value = "0";
                
                ddlOwner.SelectedValue = "-1";
                ddlWarehouse.SelectedValue = "-1";
                //ddlContract.SelectedValue = "-1";
                ddlTransaction.SelectedIndex = 1;
                //ddlType.SelectedValue = "-1";
                ddlDocumentInOut.SelectedValue = "-1";
                //ddlDocumentType.SelectedValue = "-1";
                //txtDocumentNumber.Text = string.Empty;
                txtStartDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtQty.Text = string.Empty;
                txtValue.Text = string.Empty;
                ddlContract.Items.Clear();
                
                ddlOwner.Enabled = true;
                ddlWarehouse.Enabled = true;
                ddlTransaction.Enabled = false;
                ddlType.Enabled = true;
                ddlDocumentInOut.Enabled = true;
                ddlDocumentType.Enabled = true;
                txtDocumentNumber.Enabled = true;
                calStartDate.Enabled = true;
                txtStartDate.Enabled = true;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (billingLogViewDTO.Configuration != null && billingLogViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                {
                    base.ConfigureModal(billingLogViewDTO.Configuration, true);
                    divDocumentType.Visible = false;
                    divDocumentNumber.Visible = false;
                    divType.Visible = true;
                    txtValue.Enabled = true;
                    ddlContract.Enabled = true;
                }
                    
                else
                {
                    base.ConfigureModal(billingLogViewDTO.Configuration, false);

                    ddlOwner.Enabled = false;
                    ddlWarehouse.Enabled = false;
                    ddlContract.Enabled = false;
                    ddlTransaction.Enabled = false;

                    if (hasCorrectTypeBilling)
                        ddlType.Enabled = billingLogViewDTO.Entities[index].BillingType.Id > 0 ? false : true;
                    else
                        ddlType.Enabled = true;

                    if (billingLogViewDTO.Entities[index].BillingTransaction.Type != "A")
                    {
                        ddlDocumentInOut.Enabled = false;
                        ddlDocumentType.Enabled = false;
                        txtDocumentNumber.Enabled = false;
                    }
                    else
                    {
                        ddlDocumentInOut.Enabled = true;
                        ddlDocumentType.Enabled = true;
                        txtDocumentNumber.Enabled = true;
                    }
                        
                    calStartDate.Enabled = false;
                    txtStartDate.Enabled = false;
                    if (billingLogViewDTO.Entities[index].DocumentInOut == null)
                    {
                        divDocumentType.Visible = false;
                        divDocumentNumber.Visible = false;
                    }
                }
                    
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
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
                billingLogViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingLogViewDTO.Errors);
            }
        }


        #endregion


    }
}