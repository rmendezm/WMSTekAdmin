using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
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

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class WorkOrders : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Receipt> receiptViewDTO = new GenericViewDTO<Receipt>();
        private GenericViewDTO<ReceiptDetail> receiptDetailViewDTO = new GenericViewDTO<ReceiptDetail>();
        private bool isValidViewDTO = true;


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

        public int currentIndexToLoadDetail
        {
            get
            {
                if (ValidateViewState("currentIndexToLoadDetail"))
                    return (int)ViewState["currentIndexToLoadDetail"];
                else
                    return -1;
            }

            set { ViewState["currentIndexToLoadDetail"] = value; }
        }
        #endregion

        #region Events

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    if (isValidViewDTO)
                    {
                        PopulateGrid();
                        PopulateGridDetail();
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                currentIndexToLoadDetail = index;
                currentIndex = grdMgr.SelectedIndex;

                LoadDetail(index);
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            string detailTitle = null;

            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    var receiptSelected = (Receipt)Session[WMSTekSessions.WorkOrders.Selected];

                    grdMgr.AllowPaging = false;
                    grdMgr.DataSource = new List<Receipt>() { receiptSelected };
                    grdMgr.DataBind();

                    LoadDetail(index);

                    detailTitle = "Orden " + receiptSelected.InboundOrder.OutboundOrder.Number;
                    base.ExportToExcel(grdMgr, grdDetail, detailTitle);

                    PopulateGrid();
                    grdMgr.AllowPaging = true;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
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
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
            }
        }

        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);

            Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.documentVisible = true;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            receiptViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(receiptViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            receiptViewDTO = iReceptionMGR.WorkOrders(context);

            if (!receiptViewDTO.hasError() && receiptViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.WorkOrders.List, receiptViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(receiptViewDTO.MessageStatus.Message);
            }
            else
            {
                if (receiptViewDTO.hasError())
                    this.Master.ucError.ShowError(receiptViewDTO.Errors);
                else
                    grdMgr.EmptyDataText = this.Master.EmptyGridText;
            }

            CallJsGridViewHeader();
        }

        private void PopulateGrid()
        {
            receiptViewDTO = (GenericViewDTO<Receipt>)Session[WMSTekSessions.WorkOrders.List];

            if (receiptViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!receiptViewDTO.hasConfigurationError() && receiptViewDTO.Configuration != null && receiptViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, receiptViewDTO.Configuration);

            grdMgr.DataSource = receiptViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(receiptViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
                receiptViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(receiptViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                receiptViewDTO = (GenericViewDTO<Receipt>)Session[WMSTekSessions.WorkOrders.List];

                int idOutboundOrder = receiptViewDTO.Entities[index].InboundOrder.OutboundOrder.Id;
                int idReceipt = receiptViewDTO.Entities[index].Id;

                receiptDetailViewDTO = iReceptionMGR.WorkOrdersDetails(idOutboundOrder, idReceipt, context);

                if (receiptDetailViewDTO != null && receiptDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!receiptDetailViewDTO.hasConfigurationError() && receiptDetailViewDTO.Configuration != null && receiptDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, receiptDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = receiptDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    lblNroDoc.Text = receiptViewDTO.Entities[index].InboundOrder.OutboundOrder.Number;

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.WorkOrders.Selected, receiptViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.WorkOrders.Detail, receiptDetailViewDTO);
                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                receiptDetailViewDTO = (GenericViewDTO<ReceiptDetail>)Session[WMSTekSessions.WorkOrders.Detail];

                // Configura ORDEN de las columnas de la grilla
                if (!receiptDetailViewDTO.hasConfigurationError() && receiptDetailViewDTO.Configuration != null && receiptDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, receiptDetailViewDTO.Configuration);

                // Detalle de Documentos de Entrada
                grdDetail.DataSource = receiptDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('WorkOrderDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, true);", true);
        }

        #endregion
    }
}