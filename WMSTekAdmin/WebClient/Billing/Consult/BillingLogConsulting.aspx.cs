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

namespace Binaria.WMSTek.WebClient.Billing.Consult
{
    public partial class BillingLogConsulting : BasePage
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

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
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
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

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