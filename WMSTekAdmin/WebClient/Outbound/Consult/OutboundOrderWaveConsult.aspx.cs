using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class OutboundOrderWaveConsult : BasePage
    {

        #region "Declaración de Variables"

        private GenericViewDTO<OutboundOrder> outboundOrderWaveViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundDetail> outboundDetailViewDTO = new GenericViewDTO<OutboundDetail>();

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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnCancelOrder = e.Row.FindControl("btnCancel") as ImageButton;

                    if (btnCancelOrder != null && (outboundOrderWaveViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Pending ||
                        outboundOrderWaveViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Shipped ||
                        outboundOrderWaveViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Closed ||
                        outboundOrderWaveViewDTO.Entities[e.Row.DataItemIndex].LatestOutboundTrack.Type.Id == (int)TrackOutboundTypeName.Cancel))
                    {
                        btnCancelOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel_dis.png";
                        btnCancelOrder.Enabled = false;
                    }
                    else
                    {
                        btnCancelOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel.png";
                        btnCancelOrder.Enabled = true;
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    } 
                }
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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

                    LoadOutboundOrder(index);
                }
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        protected void grdMgrWave_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgrWave.PageSize * grdMgrWave.PageIndex + grdMgrWave.SelectedIndex;
                    currentIndex = grdMgrWave.SelectedIndex;

                    LoadOutboundOrderDetail(index);
                }
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        protected void grdMgrWave_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgrWave.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgrWave.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgrWave.ClientID + "');");
                    e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(grdMgrWave, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                //    LoadOutboundOrder(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdMgrWave, detailTitle);
                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            GenericViewDTO<OutboundOrder> outboundOrderWaveAuxViewDTO = new GenericViewDTO<OutboundOrder>();
            ContextViewDTO contextAux = new ContextViewDTO();
            string detailTitle;

            try
            {
                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgr.AllowPaging = false;
                    PopulateGrid();

                    contextAux = context;
                    grdMgrAux = grdMgr;

                    contextAux.MainFilter = this.Master.ucMainFilter.MainFilter;
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.OutboundType)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(outboundOrderWaveViewDTO.Entities[index].Warehouse.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(outboundOrderWaveViewDTO.Entities[index].Owner.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.OutboundType)].FilterValues.Add(new FilterItem(outboundOrderWaveViewDTO.Entities[index].OutboundType.Id.ToString()));
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Add(new FilterItem(outboundOrderWaveViewDTO.Entities[index].Number));
                    //context.MainFilter[index].FilterValues.Clear();
                    //context.MainFilter[index].FilterValues.Add(new FilterItem(((int)OutboundTypeName.OrdenPickingWave).ToString()));

                    outboundOrderWaveAuxViewDTO = iDispatchingMGR.FindAllOutboundOrder(context);
                    grdMgrAux.DataSource = outboundOrderWaveAuxViewDTO.Entities;
                    grdMgrAux.DataBind();


                    LoadOutboundOrder(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                    base.ExportToExcel(grdMgrAux, grdMgrWave, detailTitle);
                    grdMgr.AllowPaging = true;
                }
                

            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
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
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "CancelOrder")
                {
                    CancelOrder(index);
                }
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //        currentIndex = -1;
        //        divDetail.Visible = false;
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
        //    }
        //}

        //protected void imgbtnSearchVendor_Click(object sender, ImageClickEventArgs e)
        //{
        //    pnlPanelPoUp.Visible = true;
        //    mpeModalPopUpVendor.Show();
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "OutboundOrderWaveConsult";

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
                if (ValidateSession(WMSTekSessions.OutboundOrderWave.OutboundOrderWaveList))
                {
                    outboundOrderWaveViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderWave.OutboundOrderWaveList];
                    isValidViewDTO = true;
                }

                if (ValidateSession(WMSTekSessions.OutboundOrderWave.OutboundOrderList))
                {
                    outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderWave.OutboundOrderList];
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
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.trackOutboundTypeVisible = true;
            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabDatesVisible = true;
            this.Master.ucMainFilter.expirationDateVisible = true;
            this.Master.ucMainFilter.expectedDateVisible = true;
            this.Master.ucMainFilter.tabDispatchingVisible = true;
            this.Master.ucMainFilter.tabDispatchingHeaderText = this.lblAdvancedFilter.Text;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.codeLabel = this.lblDocumentNumber.Text;
            this.Master.ucMainFilter.DocumentNumberLabel = this.lblOutboundOrder.Text;
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

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // Carga lista de OutboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            int index = Convert.ToInt16(EntityFilterName.OutboundType);

            //Order
            context.MainFilter[index].FilterValues.Clear();
            context.MainFilter[index].FilterValues.Add(new FilterItem(((int)OutboundTypeName.OrdenPickingWave).ToString()));
                        
            TextBox txtDocumentNumber = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDocumentNumber");
            TextBox txtOrder = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");

            context.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Clear();

            if (!string.IsNullOrEmpty(txtOrder.Text))
                context.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Add(new FilterItem(txtOrder.Text.Trim()));

            context.MainFilter[Convert.ToInt16(EntityFilterName.Code)].FilterValues.Clear();

            if (!string.IsNullOrEmpty(txtDocumentNumber.Text))
            {
                Dictionary<string, string> subQueryParams = new Dictionary<string, string>();
                subQueryParams.Add("SubQueryCode", "ExistsDocumentInWave");
                subQueryParams.Add("outboundNumber", txtDocumentNumber.Text.Trim());

                outboundOrderWaveViewDTO = iDispatchingMGR.FindAllOutboundOrder(context, subQueryParams);
            }
            else
            {
                outboundOrderWaveViewDTO = iDispatchingMGR.FindAllOutboundOrder(context, null);
            }
            //outboundOrderWaveViewDTO = iDispatchingMGR.FindAllOutboundOrder(context);

            if (!outboundOrderWaveViewDTO.hasError() && outboundOrderWaveViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundOrderWave.OutboundOrderWaveList, outboundOrderWaveViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(outboundOrderWaveViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!outboundOrderWaveViewDTO.hasConfigurationError() && outboundOrderWaveViewDTO.Configuration != null && outboundOrderWaveViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderWaveViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderWaveViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(outboundOrderWaveViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        /// <summary>
        /// Retorna el detalle de cada doc de entrada
        /// </summary>
        /// <param name="index"></param>
        protected void LoadOutboundOrder(int index)
        {
            grdMgrWave.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                int id = outboundOrderWaveViewDTO.Entities[index].Id;
                int idWhs = outboundOrderWaveViewDTO.Entities[index].Warehouse.Id;
                int idOwn = outboundOrderWaveViewDTO.Entities[index].Owner.Id;
                
                outboundOrderViewDTO = iDispatchingMGR.GetOutboundOrder_ByIdWave(id, idOwn, idWhs, context);
                this.lblNroDoc.Text = outboundOrderWaveViewDTO.Entities[index].Number;

                if (outboundOrderViewDTO != null && outboundOrderViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdMgrWave, outboundOrderViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdMgrWave.DataSource = outboundOrderViewDTO.Entities;
                    grdMgrWave.DataBind();

                    CallJsGridViewDetail();

                    Session.Add(WMSTekSessions.OutboundOrderWave.OutboundOrderList, outboundOrderViewDTO);
                }

                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        /// <summary>
        /// Retorna el detalle de cada doc de entrada
        /// </summary>
        /// <param name="index"></param>
        protected void LoadOutboundOrderDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;

            if (index != -1)
            {
                divCloseOrder.Visible = true;
                mpCloseOrder.Show();

                int id = outboundOrderViewDTO.Entities[index].Id;

                outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, id);
                this.lblCloseOrden.Text = this.lblTitleDetailOrder.Text + outboundOrderViewDTO.Entities[index].Number;

                if (outboundDetailViewDTO != null && outboundDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!outboundDetailViewDTO.hasConfigurationError() && outboundDetailViewDTO.Configuration != null && outboundDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, outboundDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = outboundDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                //divDetail.Visible = true;
            }
            //else
            //{
            //    divDetail.Visible = false;
            //}
        }

        private void CancelOrder(int index)
        {
            outboundOrderWaveViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderWave.OutboundOrderWaveList];

            var waveSelected = outboundOrderWaveViewDTO.Entities[index];

            var cancelWaveViewDTO = iDispatchingMGR.CancelWaveOrder(waveSelected, context);

            if (cancelWaveViewDTO.hasError())
            {
                if (!string.IsNullOrEmpty(cancelWaveViewDTO.Errors.OriginalMessage) && cancelWaveViewDTO.Errors.OriginalMessage.ToLower().Contains("ola tiene track"))
                {
                    UpdateSession();
                    ucStatus.ShowMessage("Elemento modificado exitosamente.");
                }
                else
                    this.Master.ucError.ShowError(cancelWaveViewDTO.Errors);
            }
            else
            {
                UpdateSession();
                ucStatus.ShowMessage(cancelWaveViewDTO.MessageStatus.Message);
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('OutboundOrder_GetByIdWave', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdMgrWave');", true);
        }

        #endregion

        protected void btnCloseOrder_Click(object sender, EventArgs e)
        {
            try
            {
                CloseOrder();
            }
            catch (Exception ex)
            {
                outboundOrderWaveViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderWaveViewDTO.Errors);
            }
        }

        protected void CloseOrder()
        {

        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdMgrWave_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}
