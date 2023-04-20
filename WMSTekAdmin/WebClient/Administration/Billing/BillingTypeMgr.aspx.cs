using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Entities.Warehousing;
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
    public partial class BillingTypeMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<BillingType> billingTypeViewDTO = new GenericViewDTO<BillingType>();
        private GenericViewDTO<BillingStep> billingStepViewDTO = new GenericViewDTO<BillingStep>();
        
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

                    if (ValidateSession(WMSTekSessions.BillingTypeMgr.BillingTypeList))
                    {
                        billingTypeViewDTO = (GenericViewDTO<BillingType>)Session[WMSTekSessions.BillingTypeMgr.BillingTypeList];
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
            }
        }

        protected void ddlStep_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlFilterItem = (DropDownList)sender;

                BillingStep billingStep = new BillingStep();
                billingStep.Id = Convert.ToInt32(ddlFilterItem.SelectedValue);
                billingStepViewDTO = iBillingMGR.BillingStepGetByAnyParameter(billingStep,context);
                this.ddlLPNType.Visible = false;
                this.lblLPNType.Visible = false;
                this.ddlVAS.Visible = false;
                this.lblVAS.Visible = false;
                this.ddlOutboundType.Visible = false;
                this.lblOutboundType.Visible = false;

                if (!billingStepViewDTO.hasError())
                {
                    if (billingStepViewDTO.Entities[0].Code == "LPN")
                    {
                        this.ddlLPNType.Visible = true;
                        this.lblLPNType.Visible = true;
                        base.LoadLpnTypeNew(ddlLPNType, false, this.Master.EmptyRowText);
                    }
                    else
                    {
                        this.ddlLPNType.Visible = false;
                        this.lblLPNType.Visible = false;
                    }
                    if (billingStepViewDTO.Entities[0].Code == "IVAS")
                    {
                        this.ddlVAS.Visible = true;
                        this.lblVAS.Visible = true;
                        base.LoadVAS(ddlVAS, false, this.Master.EmptyRowText);
                    }
                    else
                    {
                        this.ddlVAS.Visible = false;
                        this.lblVAS.Visible = false;
                    }
                    if (billingStepViewDTO.Entities[0].Code == "ZONE")
                    {
                        this.ddlWorkZone.Visible = true;
                        this.lblWorkZone.Visible = true;
                        base.LoadWorkZone(this.ddlWorkZone, false, this.Master.EmptyRowText);
                    }
                    else
                    {
                        this.ddlWorkZone.Visible = false;
                        this.lblWorkZone.Visible = false;
                    }
                    if (billingStepViewDTO.Entities[0].Code == "FDOC")
                    {
                        this.ddlOutboundType.Visible = true;
                        this.lblOutboundType.Visible = true;
                        base.LoadAllOutboundTypeNew(ddlOutboundType, false, this.Master.EmptyRowText);
                    }
                    else
                    {
                        this.ddlOutboundType.Visible = false;
                        this.lblOutboundType.Visible = false;
                    }
            }
                
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
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
                billingTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
            }
        }

        protected void ddlTimeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlFilterItem = (DropDownList)sender;
            BillingMode billingModeParam = new BillingMode();

            if (ddlTimeType.SelectedValue == EnumsDescription(TransactionTypeBilling.Diario))
            {
                billingModeParam.TimeType = EnumsDescription(TransactionTypeBilling.Diario);
                LoadBillingMode(ddlMode, billingModeParam, Master.EmptyRowText, isNew);
            }
            else
            {
                billingModeParam.TimeType = EnumsDescription(TransactionTypeBilling.CadaVez);
                LoadBillingMode(ddlMode, billingModeParam, Master.EmptyRowText, isNew);
            }

            


            divEditNew.Visible = true;
            modalPopUp.Show();
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
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
                billingTypeViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            billingTypeViewDTO = iBillingMGR.FindAllType(context);

            if (!billingTypeViewDTO.hasError() && billingTypeViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.BillingTypeMgr.BillingTypeList, billingTypeViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(billingTypeViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(billingTypeViewDTO.Errors);
            }
        }


        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!billingTypeViewDTO.hasConfigurationError() && billingTypeViewDTO.Configuration != null && billingTypeViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, billingTypeViewDTO.Configuration);

            grdMgr.DataSource = billingTypeViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(billingTypeViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.descriptionVisible = true;

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
            BillingType billingType = new BillingType(Convert.ToInt32(hidEditId.Value));
            BillingStep billingStep = new BillingStep();

            billingType.Name = this.txtDescription.Text.Trim();
            billingType.Code = this.txtCode.Text.Trim();
            billingType.Status = this.chkStatus.Checked;
            billingType.TimeType = this.ddlTimeType.SelectedValue;
            billingType.BillingMode.Id = Convert.ToInt32(this.ddlMode.SelectedValue);
            billingType.BillingStep.Id = Convert.ToInt32(this.ddlStep.SelectedValue);
            billingType.LPNType.Code = null;

            billingStep.Id = billingType.BillingStep.Id;
            billingStepViewDTO = iBillingMGR.BillingStepGetByAnyParameter(billingStep, context);

            if (!billingStepViewDTO.hasError())
            {
                if (billingStepViewDTO.Entities[0].Code == "LPN")
                {
                    billingType.LPNType.Code = ddlLPNType.SelectedValue;
                }
                if (billingStepViewDTO.Entities[0].Code == "IVAS")
                {
                    billingType.RecipeVas.Id = Convert.ToInt32(ddlVAS.SelectedValue);
                }
                if (billingStepViewDTO.Entities[0].Code == "ZONE")
                {
                    billingType.WorkZone.Id = Convert.ToInt32(ddlWorkZone.SelectedValue);
                }
                if (billingStepViewDTO.Entities[0].Code == "FDOC")
                {
                    billingType.OutboundType.Code = this.ddlOutboundType.SelectedValue;
                }
            }

            if (hidEditId.Value == "0")
                billingTypeViewDTO = iBillingMGR.MaintainType(CRUD.Create, billingType, context);
            else
                billingTypeViewDTO = iBillingMGR.MaintainType(CRUD.Update, billingType, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (billingTypeViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingTypeViewDTO.MessageStatus.Message);

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
            // Editar bodega
            if (mode == CRUD.Update)
            {
                // TODO: ver propiedad 'required' para un drop-down list
                isNew = false;
                PopulateLists();

                //Recupera los datos de la entidad a editar
                hidEditId.Value = billingTypeViewDTO.Entities[index].Id.ToString();

                txtDescription.Text = billingTypeViewDTO.Entities[index].Name;
                txtCode.Text = billingTypeViewDTO.Entities[index].Code;
                ddlTimeType.SelectedValue = billingTypeViewDTO.Entities[index].TimeType;

                BillingMode billingModeParam = new BillingMode();
                if (ddlTimeType.SelectedValue == EnumsDescription(TransactionTypeBilling.Diario))
                {
                    billingModeParam.TimeType = EnumsDescription(TransactionTypeBilling.Diario);
                    LoadBillingMode(ddlMode, billingModeParam, Master.EmptyRowText, isNew);
                }
                else
                {
                    billingModeParam.TimeType = EnumsDescription(TransactionTypeBilling.CadaVez);
                    LoadBillingMode(ddlMode, billingModeParam, Master.EmptyRowText, isNew);
                }

                ddlMode.SelectedValue = billingTypeViewDTO.Entities[index].BillingMode.Id.ToString();
                ddlStep.SelectedValue = billingTypeViewDTO.Entities[index].BillingStep.Id.ToString();
                chkStatus.Checked = billingTypeViewDTO.Entities[index].Status;

                this.ddlLPNType.Visible = false;
                this.lblLPNType.Visible = false;
                this.ddlVAS.Visible = false;
                this.lblVAS.Visible = false;
                this.ddlWorkZone.Visible = false;
                this.lblWorkZone.Visible = false;
                BillingStep billingStep = new BillingStep();
                billingStep.Id = Convert.ToInt32(ddlStep.SelectedValue);
                billingStepViewDTO = iBillingMGR.BillingStepGetByAnyParameter(billingStep, context);

                if (!billingStepViewDTO.hasError())
                {
                    if (billingStepViewDTO.Entities[0].Code == "LPN")
                    {
                        this.ddlLPNType.Visible = true;
                        this.lblLPNType.Visible = true;
                        base.LoadLpnTypeNew(ddlLPNType, false, this.Master.EmptyRowText);
                        ddlLPNType.SelectedValue = billingTypeViewDTO.Entities[index].LPNType.Code;
                    }
                    else
                    {
                        this.ddlLPNType.Visible = false;
                        this.lblLPNType.Visible = false;
                    }
                    if (billingStepViewDTO.Entities[0].Code == "IVAS")
                    {
                        this.ddlVAS.Visible = true;
                        this.lblVAS.Visible = true;
                        base.LoadVAS(ddlVAS, false, this.Master.EmptyRowText);
                        ddlVAS.SelectedValue = billingTypeViewDTO.Entities[index].RecipeVas.Id.ToString();
                    }
                    else
                    {
                        this.ddlVAS.Visible = false;
                        this.lblVAS.Visible = false;
                    }
                    if (billingStepViewDTO.Entities[0].Code == "ZONE")
                    {
                        this.ddlWorkZone.Visible = true;
                        this.lblWorkZone.Visible = true;
                        base.LoadWorkZone(this.ddlWorkZone, false, this.Master.EmptyRowText);
                        this.ddlWorkZone.SelectedValue = billingTypeViewDTO.Entities[index].WorkZone.Id.ToString();
                    }
                    else
                    {
                        this.ddlWorkZone.Visible = false;
                        this.lblWorkZone.Visible = false;
                    }
                    if (billingStepViewDTO.Entities[0].Code == "FDOC")
                    {
                        this.ddlOutboundType.Visible = true;
                        this.lblOutboundType.Visible = true;
                        base.LoadAllOutboundTypeNew(ddlOutboundType, false, this.Master.EmptyRowText);
                        ddlOutboundType.SelectedValue = billingTypeViewDTO.Entities[index].OutboundType.Code;
                    }
                    else
                    {
                        this.ddlOutboundType.Visible = false;
                        this.lblOutboundType.Visible = false;
                    }
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
                txtDescription.Text = string.Empty;
                txtCode.Text = string.Empty;
                ddlTimeType.SelectedValue = "-1";
                ddlMode.SelectedValue = "-1";
                ddlStep.SelectedValue = "-1";
                ddlWorkZone.SelectedValue = "-1";
                chkStatus.Checked = true;
                ddlLPNType.Visible = false;
                lblLPNType.Visible = false;
                ddlVAS.Visible = false;
                lblVAS.Visible = false;
                ddlWorkZone.Visible = false;
                lblWorkZone.Visible = false;
                lblNew.Visible = true;
                lblEdit.Visible = false;
                ddlOutboundType.Visible = false;
                lblOutboundType.Visible = false;
            }

            if (billingTypeViewDTO.Configuration != null && billingTypeViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(billingTypeViewDTO.Configuration, true);
                else
                    base.ConfigureModal(billingTypeViewDTO.Configuration, false);
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
            billingTypeViewDTO = iBillingMGR.MaintainType(CRUD.Delete, billingTypeViewDTO.Entities[index], context);

            if (billingTypeViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingTypeViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        private void PopulateLists()
        {
            base.LoadTransactionTypeBilling(this.ddlTimeType, isNew, Master.EmptyRowText);

            if (this.ddlMode.SelectedValue != "-1")
            {
                BillingMode billingModeparam = new BillingMode();
                base.LoadBillingMode(this.ddlMode, billingModeparam, this.Master.EmptyRowText, isNew);
            }
                
            if (this.ddlStep.SelectedValue != "-1")
                base.LoadBillingStep(this.ddlStep, this.Master.EmptyRowText, isNew);

            if (this.ddlLPNType.SelectedValue != "-1")
                base.LoadLpnTypeNew(this.ddlLPNType, isNew, this.Master.EmptyRowText);

            if (this.ddlWorkZone.SelectedValue != "-1")
                base.LoadWorkZone(this.ddlWorkZone, isNew, this.Master.EmptyRowText);

            if (this.ddlOutboundType.SelectedValue != "-1")
                base.LoadAllOutboundTypeNew(this.ddlOutboundType, isNew, this.Master.EmptyRowText);

        }

        #endregion
    }
}