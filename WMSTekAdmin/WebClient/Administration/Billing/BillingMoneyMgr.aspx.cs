using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Billing
{
    public partial class BillingMoneyMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<BillingMoney> billingMoneyViewDTO = new GenericViewDTO<BillingMoney>();
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
                        //PopulateLists();
                    }

                    if (ValidateSession(WMSTekSessions.BillingMoneyMgr.BillingMoneyList))
                    {
                        billingMoneyViewDTO = (GenericViewDTO<BillingMoney>)Session[WMSTekSessions.BillingMoneyMgr.BillingMoneyList];
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                billingMoneyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
                billingMoneyViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            billingMoneyViewDTO = iBillingMGR.FindAllMoney(context);

            if (!billingMoneyViewDTO.hasError() && billingMoneyViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.BillingMoneyMgr.BillingMoneyList, billingMoneyViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(billingMoneyViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(billingMoneyViewDTO.Errors);
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
            if (!billingMoneyViewDTO.hasConfigurationError() && billingMoneyViewDTO.Configuration != null && billingMoneyViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, billingMoneyViewDTO.Configuration);

            grdMgr.DataSource = billingMoneyViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(billingMoneyViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
            BillingMoney billingMoney = new BillingMoney(Convert.ToInt32(hidEditId.Value));

            billingMoney.Description = this.txtDescription.Text.Trim();
            billingMoney.Code = this.txtCode.Text.Trim();
            billingMoney.Value = Convert.ToDecimal(this.txtValue.Text);

            if (hidEditId.Value == "0")
                billingMoneyViewDTO = iBillingMGR.MaintainMoney(CRUD.Create, billingMoney, context);
            else
                billingMoneyViewDTO = iBillingMGR.MaintainMoney(CRUD.Update, billingMoney, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (billingMoneyViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingMoneyViewDTO.MessageStatus.Message);

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
                //PopulateLists();

                //Recupera los datos de la entidad a editar
                hidEditId.Value = billingMoneyViewDTO.Entities[index].Id.ToString();

                txtDescription.Text = billingMoneyViewDTO.Entities[index].Description;
                txtCode.Text = billingMoneyViewDTO.Entities[index].Code;
                txtValue.Text = billingMoneyViewDTO.Entities[index].Value.ToString();

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva entidad
            if (mode == CRUD.Create)
            {
                // TODO: revisar
                isNew = true;

                //PopulateLists();

                hidEditId.Value = "0";
                txtDescription.Text = string.Empty;
                txtCode.Text = string.Empty;
                txtValue.Text = string.Empty;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (billingMoneyViewDTO.Configuration != null && billingMoneyViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(billingMoneyViewDTO.Configuration, true);
                else
                    base.ConfigureModal(billingMoneyViewDTO.Configuration, false);
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
            billingMoneyViewDTO = iBillingMGR.MaintainMoney(CRUD.Delete, billingMoneyViewDTO.Entities[index], context);

            if (billingMoneyViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(billingMoneyViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        #endregion
    }
}