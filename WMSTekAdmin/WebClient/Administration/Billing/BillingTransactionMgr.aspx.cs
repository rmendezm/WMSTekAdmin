using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Billing
{
    public partial class BillingTransactionMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<BillingTransaction> billingTransactionViewDTO = new GenericViewDTO<BillingTransaction>();
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

                    if (ValidateSession(WMSTekSessions.BillingTransactionMgr.BillingTransatcionList))
                    {
                        billingTransactionViewDTO = (GenericViewDTO<BillingTransaction>)Session[WMSTekSessions.BillingTransactionMgr.BillingTransatcionList];
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
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
                billingTransactionViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
            }
        }

        #endregion

        #region "Eventos"

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
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
                billingTransactionViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            billingTransactionViewDTO = iBillingMGR.FindAllTransaction(context);

            if (!billingTransactionViewDTO.hasError() && billingTransactionViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.BillingTransactionMgr.BillingTransatcionList, billingTransactionViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(billingTransactionViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(billingTransactionViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            // 1 => Modulo Web. 
            // 2 => Modulo RF.
            //int moduloRF = 2;
            //base.LoadWmsProcess(this.ddlWmsProcess, moduloRF,true, Master.EmptyRowText);
            base.LoadTransactionTypeBilling(this.ddlType, isNew, Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!billingTransactionViewDTO.hasConfigurationError() && billingTransactionViewDTO.Configuration != null && billingTransactionViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, billingTransactionViewDTO.Configuration);

            grdMgr.DataSource = billingTransactionViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(billingTransactionViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;

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
            BillingTransaction transaction = new BillingTransaction(Convert.ToInt32(hidEditId.Value));

            transaction.Code = this.txtTransactionCode.Text.Trim();
            transaction.Name = this.txtTransactionName.Text.Trim();
            transaction.Type = this.ddlType.SelectedValue;
            transaction.Status = this.chkStatus.Checked;
            transaction.WmsProcess = new WmsProcess();
            transaction.WmsProcess.Code = this.ddlWmsProcess.SelectedValue;

            if (hidEditId.Value == "0")
                billingTransactionViewDTO = iBillingMGR.MaintainTransaction(CRUD.Create, transaction, context);
            else
                billingTransactionViewDTO = iBillingMGR.MaintainTransaction(CRUD.Update, transaction, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (billingTransactionViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingTransactionViewDTO.MessageStatus.Message);

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
            int moduloRF = 2;
            // Editar bodega
            if (mode == CRUD.Update)
            {
                // TODO: ver propiedad 'required' para un drop-down list
                isNew = false;
                PopulateLists();

                //Recupera los datos de la entidad a editar
                hidEditId.Value = billingTransactionViewDTO.Entities[index].Id.ToString();

                txtTransactionCode.Text = billingTransactionViewDTO.Entities[index].Code;
                txtTransactionName.Text = billingTransactionViewDTO.Entities[index].Name;
                ddlType.SelectedValue = billingTransactionViewDTO.Entities[index].Type;
                chkStatus.Checked = billingTransactionViewDTO.Entities[index].Status;

                if (ddlType.SelectedValue != EnumsDescription(TransactionTypeBilling.CadaVez) && ddlType.SelectedValue != EnumsDescription(TransactionTypeBilling.Diario))
                    divWmsProcess.Visible = false;
                else
                {
                    divWmsProcess.Visible = true;
                    if (ddlType.SelectedValue == EnumsDescription(TransactionTypeBilling.CadaVez))
                    {
                        base.LoadWmsProcess(this.ddlWmsProcess, moduloRF, true, Master.EmptyRowText);
                    }
                    if (ddlType.SelectedValue == EnumsDescription(TransactionTypeBilling.Diario))
                    {
                        base.LoadTransactionProcessBillingInterface(this.ddlWmsProcess, isNew, Master.EmptyRowText);
                    }
                }
                ddlWmsProcess.SelectedValue = billingTransactionViewDTO.Entities[index].WmsProcess.Code;

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
                txtTransactionCode.Text = string.Empty;
                txtTransactionName.Text = string.Empty;
                chkStatus.Checked = true;
                ddlType.SelectedValue = "C";
                base.LoadWmsProcess(this.ddlWmsProcess, moduloRF, true, Master.EmptyRowText);
                ddlWmsProcess.SelectedValue = "-1";
                divWmsProcess.Visible = true;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (billingTransactionViewDTO.Configuration != null && billingTransactionViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(billingTransactionViewDTO.Configuration, true);
                else
                    base.ConfigureModal(billingTransactionViewDTO.Configuration, false);
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
            billingTransactionViewDTO = iBillingMGR.MaintainTransaction(CRUD.Delete, billingTransactionViewDTO.Entities[index], context);

            if (billingTransactionViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingTransactionViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlFilterItem = (DropDownList)sender;
            
            if (ddlType.SelectedValue != EnumsDescription(TransactionTypeBilling.CadaVez) && ddlType.SelectedValue != EnumsDescription(TransactionTypeBilling.Diario))
                divWmsProcess.Visible = false;
            else
            {
                divWmsProcess.Visible = true;
                if (ddlType.SelectedValue == EnumsDescription(TransactionTypeBilling.CadaVez))
                {
                    int moduloRF = 2;
                    base.LoadWmsProcess(this.ddlWmsProcess, moduloRF, true, Master.EmptyRowText);
                }
                if (ddlType.SelectedValue == EnumsDescription(TransactionTypeBilling.Diario))
                {
                    base.LoadTransactionProcessBillingInterface(this.ddlWmsProcess, isNew, Master.EmptyRowText);
                }
            }
                

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        #endregion

    }
}