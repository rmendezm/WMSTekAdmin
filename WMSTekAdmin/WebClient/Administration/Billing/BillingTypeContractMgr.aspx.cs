using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Reception;
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
    public partial class BillingTypeContractMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<BillingTypeContract> billingTypeContractViewDTO = new GenericViewDTO<BillingTypeContract>();
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

                    if (ValidateSession(WMSTekSessions.BillingTypeContractMgr.BillingTypeContractList))
                    {
                        billingTypeContractViewDTO = (GenericViewDTO<BillingTypeContract>)Session[WMSTekSessions.BillingTypeContractMgr.BillingTypeContractList];
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
            }
        }

        protected void ddlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlFilterItem = (DropDownList)sender;

            if (ddlOwner.SelectedValue != "-1")
                base.LoadBillingContract(ddlContract, Convert.ToInt32(ddlOwner.SelectedValue),this.Master.EmptyRowText,isNew);
            else
                ddlContract.Items.Clear();

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void ddlTransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowTime();

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void ddlTimeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BillingTransaction billingTransactionParam = new BillingTransaction();
            BillingType billingTypeParam = new BillingType();

            if (Convert.ToInt16(ddlTimeType.SelectedValue) == Convert.ToInt16(TimeTypeBilling.Diario))
            {
                divType.Visible = true;
                billingTransactionParam.Type = EnumsDescription(TransactionTypeBilling.Diario);
                base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);
                billingTypeParam.TimeType = MiscUtils.EnumsDescriptionTransactionTypeBilling(TransactionTypeBilling.Diario);
                base.LoadBillingType(this.ddlType, billingTypeParam, this.Master.EmptyRowText, isNew);
            }
            else
            {
                
                if (Convert.ToInt16(ddlTimeType.SelectedValue) == Convert.ToInt16(TimeTypeBilling.Fijo))
                {
                    divType.Visible = false;
                    billingTransactionParam.Type = EnumsDescription(TransactionTypeBilling.Fijo);
                    base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);
                }
                else
                {
                    divType.Visible = true;
                    billingTransactionParam.Type = EnumsDescription(TransactionTypeBilling.CadaVez);
                    base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);
                    billingTypeParam.TimeType = MiscUtils.EnumsDescriptionTransactionTypeBilling(TransactionTypeBilling.CadaVez);
                    base.LoadBillingType(this.ddlType, billingTypeParam, this.Master.EmptyRowText, isNew);
                }
                    
            }
                

            ShowTime();

            divEditNew.Visible = true;
            modalPopUp.Show();
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                billingTypeContractViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
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
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
                billingTypeContractViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            billingTypeContractViewDTO = iBillingMGR.FindAllTypeContract(context);

            if (!billingTypeContractViewDTO.hasError() && billingTypeContractViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.BillingTypeContractMgr.BillingTypeContractList, billingTypeContractViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(billingTypeContractViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(billingTypeContractViewDTO.Errors);
            }
        }

        //private void PopulateLists()
        //{
        //    if (this.ddlTransaction.SelectedValue != "-1")
        //        base.LoadBillingTransaction(this.ddlTransaction, this.Master.EmptyRowText, isNew);
        //}

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!billingTypeContractViewDTO.hasConfigurationError() && billingTypeContractViewDTO.Configuration != null && billingTypeContractViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, billingTypeContractViewDTO.Configuration);

            grdMgr.DataSource = billingTypeContractViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(billingTypeContractViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.nameLabel = lblName.Text;
            this.Master.ucMainFilter.descriptionLabel = lblDescription.Text;

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

        protected void SaveChanges()
        {
            //Agrega los datos del Contrato
            GenericViewDTO<BillingTransaction> billingTransactionDTO = new GenericViewDTO<BillingTransaction>();
            BillingTypeContract billingTypeContract = new BillingTypeContract(Convert.ToInt32(hidEditId.Value));

            billingTypeContract.BillingContract.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
            billingTypeContract.Warehouse.Id = Convert.ToInt32(this.ddlWarehouse.SelectedValue);
            billingTypeContract.BillingContract.Id = Convert.ToInt32(this.ddlContract.SelectedValue);
            billingTypeContract.BillingMoney.Id = Convert.ToInt32(this.ddlMoney.SelectedValue);
            billingTypeContract.BillingTimeType.Id = Convert.ToInt32(this.ddlTimeType.SelectedValue);
            billingTypeContract.BillingTransaction.Id = Convert.ToInt32(this.ddlTransaction.SelectedValue);
            
            if (this.ddlType.SelectedValue == "")
            {
                billingTypeContract.BillingType.Id = -1;
            }
            else
            {
                billingTypeContract.BillingType.Id = Convert.ToInt32(this.ddlType.SelectedValue);
            }
            billingTypeContract.Value = Convert.ToDecimal(this.txtValue.Text);
            billingTransactionDTO = iBillingMGR.BillingTransactionGetByAnyParameter(billingTypeContract.BillingTransaction, context);

            if(!billingTransactionDTO.hasError())
            {
                if(billingTransactionDTO.Entities[0].Type == EnumsDescription(TransactionTypeBilling.CadaVez))
                {
                    var today = DateTime.Now;
                    billingTypeContract.StartTime = new DateTime(today.Year, today.Month, today.Day, 00, 00, 00);
                    billingTypeContract.EndTime = new DateTime(today.Year, today.Month, today.Day, 00, 00, 00);

                    // Agrega las horas ingresadas
                    if (!string.IsNullOrEmpty(this.txtStartTimeHours.Text))
                        billingTypeContract.StartTime = billingTypeContract.StartTime.AddHours(Convert.ToInt16(this.txtStartTimeHours.Text));

                    // Agrega los minutos seleccionados
                    if (!string.IsNullOrEmpty(this.txtStartTimeMinutes.Text))
                        billingTypeContract.StartTime = billingTypeContract.StartTime.AddMinutes(Convert.ToInt16(this.txtStartTimeMinutes.Text));

                    // Agrega las horas ingresadas
                    if (!string.IsNullOrEmpty(this.txtEndTimeHours.Text))
                        billingTypeContract.EndTime = billingTypeContract.EndTime.AddHours(Convert.ToInt16(this.txtEndTimeHours.Text));
                    // Agrega los minutos seleccionados
                    if (!string.IsNullOrEmpty(this.txtEndTimeMinutes.Text))
                        billingTypeContract.EndTime = billingTypeContract.EndTime.AddMinutes(Convert.ToInt16(this.txtEndTimeMinutes.Text));
                }
                else
                {
                    billingTypeContract.StartTime = DateTime.ParseExact(txtStartDate.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);  
                    billingTypeContract.EndTime = DateTime.ParseExact(txtEndDate.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);     
                }
            }
            if (billingTypeContract.BillingType.Id < 0)
                billingTypeContract.BillingType.Id = 0;

            if (hidEditId.Value == "0")
                billingTypeContractViewDTO = iBillingMGR.MaintainTypeContract(CRUD.Create, billingTypeContract, context);
            else
                billingTypeContractViewDTO = iBillingMGR.MaintainTypeContract(CRUD.Update, billingTypeContract, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (billingTypeContractViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingTypeContractViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void ShowTime()
        {
            GenericViewDTO<BillingTransaction> theBillingTransactionDTO = new GenericViewDTO<BillingTransaction>();
            BillingTransaction billingTransactionParam;

            if (ddlTimeType.SelectedValue != "-1")
            {
                if (ddlTimeType.SelectedValue == Convert.ToInt16(TimeTypeBilling.CadaVez).ToString())
                {
                    divStartTime.Visible = true;
                    divEndTime.Visible = true;
                    divStartDate.Visible = false;
                    divEndDate.Visible = false;
                }
                else
                {
                    divStartTime.Visible = false;
                    divEndTime.Visible = false;
                    divStartDate.Visible = true;
                    divEndDate.Visible = true;
                }

            }

            if (ddlTransaction.SelectedValue != "-1")
            {
                divTimeDate.Visible = true;

                billingTransactionParam = new BillingTransaction();
                if(ddlTransaction.SelectedValue != "")
                {
                    billingTransactionParam.Id = Convert.ToInt16(ddlTransaction.SelectedValue);
                    theBillingTransactionDTO = iBillingMGR.BillingTransactionGetByAnyParameter(billingTransactionParam, context);

                    if (theBillingTransactionDTO.Entities[0].Type == "C")
                    {
                        divStartTime.Visible = true;
                        divEndTime.Visible = true;
                        divStartDate.Visible = false;
                        divEndDate.Visible = false;
                    }
                    else
                    {
                        divStartTime.Visible = false;
                        divEndTime.Visible = false;
                        divStartDate.Visible = true;
                        divEndDate.Visible = true;
                    }
                }
                

            }

            
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar bodega
            if (mode == CRUD.Update)
            {
                // TODO: ver propiedad 'required' para un drop-down list
                isNew = false;
                PopulateLists();

                //Recupera los datos de la entidad a editar
                hidEditId.Value = billingTypeContractViewDTO.Entities[index].Id.ToString();

                this.ddlOwner.SelectedValue = billingTypeContractViewDTO.Entities[index].BillingContract.Owner.Id.ToString();
                this.ddlWarehouse.SelectedValue = billingTypeContractViewDTO.Entities[index].Warehouse.Id.ToString();
                base.LoadBillingContract(this.ddlContract, Convert.ToInt32(this.ddlOwner.SelectedValue), this.Master.EmptyRowText, isNew);
                this.ddlContract.SelectedValue = billingTypeContractViewDTO.Entities[index].BillingContract.Id.ToString();
                this.ddlMoney.SelectedValue = billingTypeContractViewDTO.Entities[index].BillingMoney.Id.ToString();
                this.ddlTimeType.SelectedValue = billingTypeContractViewDTO.Entities[index].BillingTimeType.Id.ToString();
                BillingTransaction billingTransactionParam = new BillingTransaction();
                BillingType billingTypeParam = new BillingType();
                if (Convert.ToInt16(ddlTimeType.SelectedValue) == Convert.ToInt16(TimeTypeBilling.Diario))
                {
                    divType.Visible = true;
                    billingTransactionParam.Type = EnumsDescription(TransactionTypeBilling.Diario);
                    billingTypeParam.TimeType = EnumsDescription(TransactionTypeBilling.Diario);
                    LoadBillingType(ddlType, billingTypeParam, Master.EmptyRowText, true);
                }
                    
                else if (Convert.ToInt16(ddlTimeType.SelectedValue) == Convert.ToInt16(TimeTypeBilling.CadaVez))
                {
                    divType.Visible = true;
                    billingTransactionParam.Type = EnumsDescription(TransactionTypeBilling.CadaVez);
                    billingTypeParam.TimeType = EnumsDescription(TransactionTypeBilling.CadaVez);
                    LoadBillingType(ddlType, billingTypeParam, Master.EmptyRowText, true);
                }
                    
                else if (Convert.ToInt16(ddlTimeType.SelectedValue) == Convert.ToInt16(TimeTypeBilling.Fijo))
                {
                    divType.Visible = false;
                    billingTransactionParam.Type = EnumsDescription(TransactionTypeBilling.Fijo);
                }
                    

                base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);
                this.ddlTransaction.SelectedValue = billingTypeContractViewDTO.Entities[index].BillingTransaction.Id.ToString();

                if (billingTypeContractViewDTO.Entities[index].BillingType.Id > 0 && ddlType.Items.Count > 1)
                    this.ddlType.SelectedValue = billingTypeContractViewDTO.Entities[index].BillingType.Id.ToString();
                
                txtValue.Text = billingTypeContractViewDTO.Entities[index].Value.ToString();

                ShowTime();

                txtStartDate.Text = billingTypeContractViewDTO.Entities[index].StartTime.ToString("dd/MM/yyyy");
                txtEndDate.Text = billingTypeContractViewDTO.Entities[index].EndTime.ToString("dd/MM/yyyy");

                if (billingTypeContractViewDTO.Entities[index].BillingTransaction.Type != MiscUtils.EnumsDescriptionTransactionTypeBilling(TransactionTypeBilling.CadaVez))
                {
                    divStartDate.Visible = true;
                    divEndDate.Visible = true;
                    divStartTime.Visible = false;
                    divEndTime.Visible = false;
                }
                else
                {
                    txtStartTimeHours.Text = billingTypeContractViewDTO.Entities[index].StartTime.ToString("HH");
                    txtStartTimeMinutes.Text = billingTypeContractViewDTO.Entities[index].StartTime.ToString("mm");
                    txtEndTimeHours.Text = billingTypeContractViewDTO.Entities[index].EndTime.ToString("HH");
                    txtEndTimeMinutes.Text = billingTypeContractViewDTO.Entities[index].EndTime.ToString("mm");
                    divStartDate.Visible = false;
                    divEndDate.Visible = false;
                    divStartTime.Visible = true;
                    divEndTime.Visible = true;
                }
                

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                // TODO: revisar
                isNew = true;

                PopulateLists();

                hidEditId.Value = "0";
                this.ddlOwner.SelectedValue = "-1";
                this.ddlWarehouse.SelectedValue = "-1";
                //this.ddlContract.SelectedValue = "-1";
                this.ddlMoney.SelectedValue = "-1";
                this.ddlTimeType.SelectedValue = "-1";
                this.ddlTransaction.SelectedValue = "-1";
                this.ddlType.SelectedValue = "-1";
                txtValue.Text = "";
                txtStartTimeHours.Text = "00";
                txtStartTimeMinutes.Text = "00";
                txtEndTimeHours.Text = "23";
                txtEndTimeMinutes.Text = "59";
                txtStartDate.Text = "";
                txtEndDate.Text = "";

                divStartDate.Visible = false;
                divEndDate.Visible = false;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (billingTypeContractViewDTO.Configuration != null && billingTypeContractViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(billingTypeContractViewDTO.Configuration, true);
                else
                {
                    base.ConfigureModal(billingTypeContractViewDTO.Configuration, false);

                    //if (billingTypeContractViewDTO.Entities[index].BillingTransaction.Type != "N")
                    if (billingTypeContractViewDTO.Entities[index].BillingTransaction.Type != "C")
                    {
                        divStartDate.Visible = true;
                        divEndDate.Visible = true;
                        divStartTime.Visible = false;
                        divEndTime.Visible = false;
                    }
                    else
                    {
                        divStartDate.Visible = false;
                        divEndDate.Visible = false;
                        divStartTime.Visible = true;
                        divEndTime.Visible = true;
                    }
                }
                    
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            billingTypeContractViewDTO = iBillingMGR.MaintainTypeContract(CRUD.Delete, billingTypeContractViewDTO.Entities[index], context);

            if (billingTypeContractViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingTypeContractViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        private void PopulateLists()
        {
            if (this.ddlOwner.Items.Count <= 0)
                base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
            if (this.ddlWarehouse.SelectedValue != "-1")
                base.LoadWarehouses(this.ddlWarehouse, isNew, this.Master.EmptyRowText, "-1");
            if (this.ddlOwner.SelectedValue != "-1")
                base.LoadBillingContract(this.ddlContract, Convert.ToInt32(this.ddlOwner.SelectedValue), this.Master.EmptyRowText, isNew);
            if (this.ddlTransaction.SelectedValue != "-1")
            {
                BillingTransaction billingTransactionParam = new BillingTransaction();
                billingTransactionParam.Type = "C";
                base.LoadBillingTransaction(this.ddlTransaction, billingTransactionParam, this.Master.EmptyRowText, isNew);
            }
                
            if (this.ddlType.SelectedValue != "-1")
                base.LoadBillingType(this.ddlType, new BillingType(), this.Master.EmptyRowText, isNew);
            if (this.ddlMoney.SelectedValue != "-1")
                base.LoadBillingMoney(this.ddlMoney, this.Master.EmptyRowText, isNew);
            if (this.ddlTimeType.SelectedValue != "-1")
                base.LoadBillingTimeType(this.ddlTimeType, this.Master.EmptyRowText, isNew);

        }



        #endregion

        
    }
}