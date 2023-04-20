using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Tasks;
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
    public partial class OutboundOrderBatchConsult : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<TaskConsult> outboundOrderBatchViewDTO = new GenericViewDTO<TaskConsult>();
        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();

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

                if (base.webMode == WebMode.Normal) 
                    Initialize();
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                        ImageButton btnCancelOrder = e.Row.FindControl("btnCancel") as ImageButton;

                        if (btnCancelOrder != null && (outboundOrderBatchViewDTO.Entities[e.Row.DataItemIndex].TrackTaskType.Id != (int)TrackTaskTypeName.Liberada))
                        {
                            btnCancelOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel_dis.png";
                            btnCancelOrder.Enabled = false;
                        }
                        else
                        {
                            btnCancelOrder.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_cancel.png";
                            btnCancelOrder.Enabled = true;
                        }

                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
            }
        }
        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                currentIndex = grdMgr.SelectedIndex;

                LoadOutboundOrders(index);
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
            }
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
            }
        }
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            string detailTitle = null;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    LoadOutboundOrders(index);

                    var batchSelected = (TaskConsult)Session[WMSTekSessions.OutboundOrderBatch.Selected];
                    detailTitle = "Batch " + batchSelected.Task.Id.ToString();
                    base.ExportToExcel(grdMgr, grdMgrBatch, detailTitle);
                }

                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
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
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
            }
        }
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void grdMgrBatch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void grdMgrBatch_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    //e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    //e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                outboundOrderBatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"
        protected void Initialize()
        {
            context.SessionInfo.IdPage = "OutboundOrderBatchConsult";

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
                if (ValidateSession(WMSTekSessions.OutboundOrderBatch.List))
                {
                    outboundOrderBatchViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.OutboundOrderBatch.List];
                    isValidViewDTO = true;
                }

                if (ValidateSession(WMSTekSessions.OutboundOrderBatch.Detail))
                {
                    outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderBatch.Detail];
                }
            }
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            Master.ucMainFilter.codeNumericVisible = true;
            Master.ucMainFilter.codeNumericLabel = lblBatchNbr.Text;

            //this.Master.ucMainFilter.codeVisible = true;
            //this.Master.ucMainFilter.trackOutboundTypeVisible = true;
            //this.Master.ucMainFilter.advancedFilterVisible = true;
            //this.Master.ucMainFilter.tabDatesVisible = true;
            //this.Master.ucMainFilter.expirationDateVisible = true;
            //this.Master.ucMainFilter.expectedDateVisible = true;
            //this.Master.ucMainFilter.tabDispatchingVisible = true;
            //this.Master.ucMainFilter.tabDispatchingHeaderText = this.lblAdvancedFilter.Text;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            //this.Master.ucMainFilter.codeLabel = this.lblDocumentNumber.Text;
            //this.Master.ucMainFilter.DocumentNumberLabel = this.lblOutboundOrder.Text;

            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
        }
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }
        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            
            int index = Convert.ToInt16(EntityFilterName.TaskType);
            context.MainFilter[index].FilterValues.Clear();
            context.MainFilter[index].FilterValues.Add(new FilterItem("PIKBT"));

            outboundOrderBatchViewDTO = iTasksMGR.FindAllTaskMgrKpi(context);

            if (!outboundOrderBatchViewDTO.hasError() && outboundOrderBatchViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundOrderBatch.List, outboundOrderBatchViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(outboundOrderBatchViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(outboundOrderBatchViewDTO.Errors);
            }
        }
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            if (!outboundOrderBatchViewDTO.hasConfigurationError() && outboundOrderBatchViewDTO.Configuration != null && outboundOrderBatchViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderBatchViewDTO.Configuration);

            grdMgr.DataSource = outboundOrderBatchViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(outboundOrderBatchViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }
        protected void ReloadData()
        {
            UpdateSession();

            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }
        protected void LoadOutboundOrders(int index)
        {
            if (index != -1)
            {
                var batchSelected = outboundOrderBatchViewDTO.Entities[index];

                this.lblNroDoc.Text = batchSelected.Task.Id.ToString();

                var subQueryParams = new Dictionary<string, string>();
                subQueryParams.Add("SubQueryCode", "ExistsDocumentInBatch");
                subQueryParams.Add("idTask", batchSelected.Task.Id.ToString());

                var newContext = NewContext();
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(batchSelected.Task.Owner.Id.ToString()));
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(batchSelected.Task.Warehouse.Id.ToString()));

                outboundOrderViewDTO = iDispatchingMGR.FindAllOutboundOrder(newContext, subQueryParams);

                if (outboundOrderViewDTO != null && outboundOrderViewDTO.Entities.Count > 0)
                {
                    if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdMgrBatch, outboundOrderViewDTO.Configuration);

                    grdMgrBatch.DataSource = outboundOrderViewDTO.Entities;
                    grdMgrBatch.DataBind();

                    CallJsGridViewDetail();

                    Session.Add(WMSTekSessions.OutboundOrderBatch.Selected, batchSelected);
                    Session.Add(WMSTekSessions.OutboundOrderBatch.Detail, outboundOrderViewDTO);
                }

                divDetail.Visible = true;
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
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('OutboundOrder_GetByIdBatch', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdMgrBatch');", true);
        }

        private void CancelOrder(int index)
        {
            outboundOrderBatchViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.OutboundOrderBatch.List];

            var batchSelected = outboundOrderBatchViewDTO.Entities[index];

            var cancelBatchViewDTO = iDispatchingMGR.CancelBatchOrder(batchSelected, context);

            if (cancelBatchViewDTO.hasError())
            {
                if (!string.IsNullOrEmpty(cancelBatchViewDTO.Errors.OriginalMessage) && cancelBatchViewDTO.Errors.OriginalMessage.ToLower().Contains("ola tiene track"))
                {
                    UpdateSession();
                    ucStatus.ShowMessage("Elemento modificado exitosamente.");
                }
                else
                    this.Master.ucError.ShowError(cancelBatchViewDTO.Errors);
            }
            else
            {
                UpdateSession();
                ucStatus.ShowMessage(cancelBatchViewDTO.MessageStatus.Message);
            }
        }
        #endregion
    }
}