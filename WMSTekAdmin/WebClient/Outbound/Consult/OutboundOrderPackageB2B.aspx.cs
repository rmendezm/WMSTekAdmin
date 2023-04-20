using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class OutboundOrderPackageB2B : BasePage
    {
        
        #region "Declaración de Variables"

        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundDetail> outboundDetailViewDTO = new GenericViewDTO<OutboundDetail>();
        private GenericViewDTO<OutboundOrder> selectedOrdersViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<ItemLocation> itemLocationViewDTO = new GenericViewDTO<ItemLocation>();

        private bool isValidViewDTO = false;
        private List<bool> checkedOrdersCurrentView = new List<bool>();
        private List<bool> selectedOrdersCurrentView = new List<bool>();
        private bool existSelected = false;

        private int currentWhs
        {
            get
            {
                if (ValidateViewState("currentWhs"))
                    return (int)ViewState["currentWhs"];
                else
                    return -1;
            }

            set { ViewState["currentWhs"] = value; }
        }


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

        private class PrePrecubing
        {
            public OutboundOrder OutboundOrder { get; set; }
            public OutboundDetail outboundDetail { get; set; }
            public string locCodePickBox { get; set; }
            public string locCodePickDet { get; set; }
            public decimal ConversionFactor { get; set; }
            public int QtyBox { get; set; }
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    base.Page_Load(sender, e);
                    SaveCheckedRows();
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                        //SaveCheckedRows();
                    }
                    ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.btnExportToExcelPrecubing);
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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

                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }


        protected void grdPrecubing_OnDataBound(object sender, EventArgs e)
        {
            try
            {
                List<PrePrecubing> lstPrecubing = (List<PrePrecubing>)Session["ListPrecubing"];
                List<PrePrecubing> lstPrecubingPreviousRow = (List<PrePrecubing>)Session["ListPrecubing"];

                //for (int i = grdPrecubing.Rows.Count - 1; i > 0; i--)
                //{
                //    int pageInd = grdPrecubing.PageSize * grdPrecubing.PageIndex;

                //    GridViewRow row = grdPrecubing.Rows[i];
                //    GridViewRow previousRow = grdPrecubing.Rows[i - 1];

                //    if (lstPrecubing[i + pageInd].OutboundOrder.Id == lstPrecubingPreviousRow[(i + pageInd) - 1].OutboundOrder.Id)
                //    {

                //        //if (previousRow.Cells[9].RowSpan == 0)
                //        //{
                //        //    if (row.Cells[9].RowSpan == 0)
                //        //    {
                //        //        previousRow.Cells[9].RowSpan += 2;
                //        //    }
                //        //    else
                //        //    {
                //        //        previousRow.Cells[9].RowSpan = row.Cells[9].RowSpan + 1;
                //        //    }
                //        //    row.Cells[9].Visible = false;
                //        //}

                //        //if (previousRow.Cells[12].RowSpan == 0)
                //        //{
                //        //    if (row.Cells[12].RowSpan == 0)
                //        //    {
                //        //        previousRow.Cells[12].RowSpan += 2;
                //        //    }
                //        //    else
                //        //    {
                //        //        previousRow.Cells[12].RowSpan = row.Cells[12].RowSpan + 1;
                //        //    }
                //        //    row.Cells[12].Visible = false;
                //        //}

                //        //if (previousRow.Cells[13].RowSpan == 0)
                //        //{
                //        //    if (row.Cells[13].RowSpan == 0)
                //        //    {
                //        //        previousRow.Cells[13].RowSpan += 2;
                //        //    }
                //        //    else
                //        //    {
                //        //        previousRow.Cells[13].RowSpan = row.Cells[13].RowSpan + 1;
                //        //    }
                //        //    row.Cells[13].Visible = false;
                //        //}

                //    }
                //}

            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void grdPrecubing_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdPrecubing.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdPrecubing.ClientID + "')");

                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdPrecubing.ClientID + "');");
                    // e.Row.Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdPrecubing, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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

                    LoadOutboundOrderDetail(index);
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                //ReloadData();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    LoadOutboundOrderDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                }
                else
                {
                    detailTitle = null;
                }

                base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnExportToExcelPrecubing_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    base.ExportToExcel(this.grdPrecubing, this.lblTitle.Text, null, null);                 
                }

            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void btnReprocess_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (existSelected)
                    {
                        OutboundOrder selectedOrder = new OutboundOrder();

                        for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                        {
                            if (checkedOrdersCurrentView[i] && !selectedOrdersCurrentView[i])
                            {
                                // Recupera la Orden seleccionada
                                selectedOrder = outboundOrderViewDTO.Entities[i];

                                // Agrega la Orden seleccionada a la lista de Ordenes seleccionadas
                                selectedOrdersViewDTO.Entities.Add(selectedOrder);
                            }
                        }

                        List<PrePrecubing> lstPrecubing = GeneratePrecubing(selectedOrdersViewDTO.Entities);
                        Session["ListPrecubing"] = lstPrecubing;

                        grdPrecubing.DataSource = lstPrecubing;
                        grdPrecubing.DataBind();

                        lblSumQtyBoxes.Text = lstPrecubing.Sum(b => b.QtyBox).ToString();

                        CallJsGridView();

                        divPrecubing.Visible = true;
                        mpPrecubing.Show();

                    }
                    else
                    {
                        isValidViewDTO = false;
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblNoOrdersSelected.Text, string.Empty);
                    }       

                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdPrecubing_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
        //        outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
        //    }
        //}

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "OutboundOrderPrecubing";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
               // UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.OutboundConsult.OutboundOrderListPrecubing))
                {
                    outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundConsult.OutboundOrderListPrecubing];
                    isValidViewDTO = true;
                }
            }

            InitializeCheckedRows();
            InitializeSelectedRows();
            SaveSelectedRows();
            PopulateGrid();
            //PopulateSelectedGrid();
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
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.OutboundTypeCode = GetConst("OutboundOrderTypeForDispatchOR").ConvertAll(s => s.ToUpper()).ToArray();
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblFilterReferenceNumber.Text;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;
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
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
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

            outboundOrderViewDTO = iDispatchingMGR.FindAllOutboundOrderPackageB2B(context);

            if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.OutboundOrderListPrecubing, outboundOrderViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }

            InitializeSelectedRows();
            SaveSelectedRows();
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(outboundOrderViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
                currentIndex = -1;
                currentPage = 0;
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
                int id = outboundOrderViewDTO.Entities[index].Id;

                outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, id);
                this.lblNroDoc.Text = outboundOrderViewDTO.Entities[index].Number;

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

                divDetail.Visible = true;
                divDetailTitle.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
                divDetailTitle.Visible = false;
            }
        }

        /// <summary>
        /// Limpia la lista de checkboxes seleccionados
        /// </summary>
        protected void InitializeCheckedRows()
        {
            checkedOrdersCurrentView.Clear();

            if (ValidateSession(WMSTekSessions.OutboundConsult.OutboundOrderListPrecubing))
                outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundConsult.OutboundOrderListPrecubing];

            if (outboundOrderViewDTO.Entities != null && checkedOrdersCurrentView != null)
            {
                for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                {
                    checkedOrdersCurrentView.Add(false);
                }
            }
        }

        /// <summary>
        /// Salva en un array los checkboxes seleccionados
        /// </summary>
        protected void SaveCheckedRows()
        {
            // Recorre la lista de Ordenes, y obtiene el ID de las seleccionadas
            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                if (checkedOrdersCurrentView.Count > (grdMgr.PageIndex * grdMgr.PageSize) + i)
                {
                    GridViewRow row = grdMgr.Rows[i];
                    checkedOrdersCurrentView[(grdMgr.PageIndex * grdMgr.PageSize) + i] = ((CheckBox)row.FindControl("chkSelectOrder")).Checked;

                    // Valida que se haya seleccionado al menos una orden
                    if (checkedOrdersCurrentView[(grdMgr.PageIndex * grdMgr.PageSize) + i])
                    {
                        existSelected = true;
                    }
                }
            }

        }

        /// <summary>
        /// Limpia la lista de Ordenes seleccionados
        /// </summary>
        protected void InitializeSelectedRows()
        {
            selectedOrdersCurrentView.Clear();

            if (outboundOrderViewDTO.Entities != null && selectedOrdersCurrentView != null)
            {
                for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                {
                    selectedOrdersCurrentView.Add(false);
                }
            }
        }

        /// <summary>
        /// Salva en un array las Ordenes ya seleccionadas
        /// </summary>
        protected void SaveSelectedRows()
        {
            if (selectedOrdersViewDTO.Entities.Count > 0)
            {
                for (int i = 0; i < outboundOrderViewDTO.Entities.Count; i++)
                {
                    foreach (OutboundOrder selectedOrder in selectedOrdersViewDTO.Entities)
                    {
                        if (selectedOrder.Id == outboundOrderViewDTO.Entities[i].Id)
                        {
                            selectedOrdersCurrentView[i] = true;
                            break;
                        }
                    }
                }

                // Recupera el Whs actual
                currentWhs = selectedOrdersViewDTO.Entities[0].Warehouse.Id;
            }
        }

        private List<PrePrecubing> GeneratePrecubing(List<OutboundOrder> orders)
        {
            List<PrePrecubing> result = new List<PrePrecubing>();
            try
            {

                foreach (OutboundOrder ord in orders)
                {
                    UomType uomTypeParam = new UomType();
                    uomTypeParam.Owner = new Owner(ord.Owner.Id);
                    uomTypeParam.Name = ord.Customer.CustomerB2B.UomTypeLpnCode;

                    GenericViewDTO<UomType> uomTypeVewDTO = iWarehousingMGR.GetUomType_ByOwnAndName(context, uomTypeParam);

                    if (uomTypeVewDTO.Entities != null && uomTypeVewDTO.Entities.Count > 0)
                    {
                        //Rescata el detalle de la orden
                        outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, ord.Id);

                        if (outboundDetailViewDTO != null && outboundDetailViewDTO.Entities.Count > 0)
                        {
                            int idWhs = ord.Warehouse.Id;
                            int idOwn = ord.Owner.Id;
                            int idUomType = uomTypeVewDTO.Entities[0].Id;

                            foreach (OutboundDetail det in outboundDetailViewDTO.Entities)
                            {
                                PrePrecubing precubingDet = new PrePrecubing();

                                decimal converFactor = 0;
                                int cantBox = 0;
                                int item = det.Item.Id;

                                GenericViewDTO<ItemUom> itemUom = iWarehousingMGR.GetByItemAndOwnerAndUomType(det.Item.Id, idOwn, idUomType, context);

                                if (itemUom != null && itemUom.Entities.Count > 0)
                                {
                                    converFactor = itemUom.Entities[0].ConversionFactor;
                                    cantBox = (int)(det.ItemQty / converFactor);
                                    
                                    precubingDet.OutboundOrder = ord;
                                    precubingDet.outboundDetail = det;
                                    precubingDet.QtyBox = cantBox;
                                    precubingDet.ConversionFactor = converFactor;

                                    result.Add(precubingDet);
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(this.lblItemNotUom.Text);
                                }
                            }
                        }
                        else
                        {
                            throw new System.InvalidOperationException(this.lblNotExistsDetailOrders.Text);
                        }
                    }
                }              

            }
            catch (InvalidOperationException ex)
            {
                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, ex.Message, "");
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }

            return result;
        }


        private GenericViewDTO<OutboundOrder> GeneratePrecubing(GenericViewDTO<OutboundOrder> orders)
        {            
            try
            {
                List<PrePrecubing> result = new List<PrePrecubing>();
                foreach (OutboundOrder ord in orders.Entities)
                {
                    //Rescata el detalle de la orden
                    outboundDetailViewDTO = iDispatchingMGR.GetDetailByIdOutbound(context, ord.Id);

                    if (outboundDetailViewDTO != null && outboundDetailViewDTO.Entities.Count > 0)
                    {
                        int idWhs = ord.Warehouse.Id;
                        int idOwn = ord.Owner.Id;

                        UomType uomTypeParam = new UomType();
                        uomTypeParam.Owner = new Owner(idOwn);
                        uomTypeParam.Name = ord.Customer.CustomerB2B.UomTypeLpnCode;

                        GenericViewDTO<UomType> uomTypeVewDTO = iWarehousingMGR.GetUomType_ByOwnAndName(context, uomTypeParam);

                        if (uomTypeVewDTO.Entities != null && uomTypeVewDTO.Entities.Count > 0)
                        {
                            foreach (OutboundDetail det in outboundDetailViewDTO.Entities)
                            {
                                PrePrecubing precubingDet = new PrePrecubing();

                                decimal converFactor = 0;
                                int cantBox = 0;
                                int item = det.Item.Id;
                                int idUomType = uomTypeVewDTO.Entities[0].Id;

                                GenericViewDTO<ItemUom> itemUom = iWarehousingMGR.GetByItemAndOwnerAndUomType(det.Item.Id, idOwn, idUomType, context);

                                if (itemUom != null && itemUom.Entities.Count > 0)
                                {
                                    converFactor = itemUom.Entities[0].ConversionFactor;
                                    cantBox = (int)(det.ItemQty / converFactor);
                                   
                                    precubingDet.OutboundOrder = ord;
                                    precubingDet.outboundDetail = det;
                                    precubingDet.QtyBox = cantBox;
                                    result.Add(precubingDet);
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(this.lblItemNotUom.Text);
                                }
                            }
                        }                       
                    }
                    else
                    {
                        throw new System.InvalidOperationException(this.lblNotExistsDetailOrders.Text);
                    }                    

                }

            }
            catch (InvalidOperationException ex)
            {
                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, ex.Message, "");
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }

            return orders;
        }

        protected void grdPrecubing_DataBound(object sender, EventArgs e)
        {
            Boolean HasData = false;
            foreach (GridViewRow row in ((GridView)sender).Rows)
            {
                HasData = row.RowType.Equals(DataControlRowType.DataRow);
            }
            if (HasData.Equals(true))
            {
                ((GridView)sender).FooterRow.Cells[0].ColumnSpan = 1;
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "griPrecubing", "initializeGridWithNoDragAndDrop(true);", true);
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('OutboundOrderDetail_ById', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }


        //public static ExpandoObject Expando(this IEnumerable<KeyValuePair<string, object>> dictionary)
        //{
        //    var expando = new ExpandoObject();
        //    var expandoDic = (IDictionary<string, object>)expando;
        //    foreach (var item in dictionary)
        //    {
        //        expandoDic.Add(item);
        //    }
        //    return expando;
        //}

        #endregion


    }
}
