using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using System.Linq;
using System.Collections.Generic;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class DispatchAdvanceWaveConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<DispatchAdvanced> dispatchAdvanceViewDTO = new GenericViewDTO<DispatchAdvanced>();
        private GenericViewDTO<DispatchAdvancedDetail> dispatchDetailViewDTO = new GenericViewDTO<DispatchAdvancedDetail>();
        private bool isValidViewDTO = false;

        // Propiedad para controlar la pagina activa de la grilla
        public int currentPage
        {
            get
            {
                if (ValidateViewState("currentPage"))
                    return (int)ViewState["currentPage"];
                else
                    return 0;
            }

            set { ViewState["currentPage"] = value; }
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;
                    currentPage = grdMgr.PageIndex;
                    LoadReceiptDetail(index);
                }
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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
                //    LoadReceiptDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla detalle
        /// </summary>
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            string detailTitle;
            GridView grdMgrAux = new GridView();
            GenericViewDTO<DispatchAdvanced> dispatchAdvanceAuxViewDTO = new GenericViewDTO<DispatchAdvanced>();
            DispatchAdvanced theDispatchAdvanced = new DispatchAdvanced();

            try
            {
                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgr.AllowPaging = false;
                    PopulateGrid();

                    grdMgrAux = grdMgr;
                    theDispatchAdvanced.Outbound = new OutboundOrder();
                    theDispatchAdvanced.Outbound.Id = dispatchAdvanceViewDTO.Entities[index].Outbound.Id;
                    dispatchAdvanceAuxViewDTO = iDispatchingMGR.DispatchAdvancedGetByAnyParameter(theDispatchAdvanced, context);
                    grdMgrAux.DataSource = dispatchAdvanceAuxViewDTO.Entities;
                    grdMgrAux.DataBind();
                    LoadReceiptDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                    base.ExportToExcel(grdMgrAux, grdDetail, detailTitle);

                    grdMgr.AllowPaging = true;
                }

            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
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
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.DispatchWaveList))
                {
                    dispatchAdvanceViewDTO = (GenericViewDTO<DispatchAdvanced>)Session[WMSTekSessions.OutboundConsult.DispatchWaveList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabDatesVisible = true;
            this.Master.ucMainFilter.expirationDateVisible = true;
            this.Master.ucMainFilter.expectedDateVisible = true;

            this.Master.ucMainFilter.trackOutboundTypeVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DocumentNumberLabel = lblFilterDoc.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.DispatchAdvanceDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.DispatchAdvanceDaysAfter;

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
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
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

        private void UpdateSession()
        {
            // carga todas las recepciones 
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            dispatchAdvanceViewDTO = iDispatchingMGR.FindAllDispatchAdvancedWave(context);

            if (!dispatchAdvanceViewDTO.hasError() && dispatchAdvanceViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.DispatchWaveList, dispatchAdvanceViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(dispatchAdvanceViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!dispatchAdvanceViewDTO.hasConfigurationError() && dispatchAdvanceViewDTO.Configuration != null && dispatchAdvanceViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchAdvanceViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = dispatchAdvanceViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(dispatchAdvanceViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }

        protected void LoadReceiptDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                var dispatchSelected = dispatchAdvanceViewDTO.Entities[index];
                int id = dispatchSelected.Id;

                var idOutboundOrder = dispatchAdvanceViewDTO.Entities[index].Outbound.Id;

                var outboundOrderViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, idOutboundOrder);

                if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities.Count > 0)
                {
                    var existsVariableField = outboundOrderViewDTO.Entities.Any(dd => !string.IsNullOrEmpty(dd.LotNumber) ||
                                                                                      dd.FifoDate != DateTime.MinValue ||
                                                                                      dd.ExpirationDate != DateTime.MinValue ||
                                                                                      dd.FabricationDate != DateTime.MinValue ||
                                                                                      (dd.CategoryItem != null && dd.CategoryItem.Id != -1)
                                                                               );

                    if (existsVariableField)
                        dispatchDetailViewDTO = iDispatchingMGR.GetDetailByIdDispatchAdvancedWaveUsingVariableFields(context, id);
                    else
                        dispatchDetailViewDTO = iDispatchingMGR.GetDetailByIdDispatchAdvancedWave(context, id);

                    this.lblNroDoc.Text = (dispatchAdvanceViewDTO.Entities[index].Outbound.Number);

                    if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                    {
                        // Configura ORDEN de las columnas de la grilla
                        if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                            base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                        // Detalle de Recepciones
                        grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                        grdDetail.DataBind();

                        CallJsGridViewDetail();
                    }

                    divDetail.Visible = true;
                }
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('DispatchAdvanced_GetByWaveId', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }

        #endregion
    }
}