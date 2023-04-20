using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class DispatchConfirm : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<DispatchSpecial> dispatchViewDTO = new GenericViewDTO<DispatchSpecial>();
        private GenericViewDTO<DispatchDetail> dispatchDetailViewDTO = new GenericViewDTO<DispatchDetail>();
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

        #region "Eventos"
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal)
                {
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var chkSelectDispatch = e.Row.FindControl("chkSelectDispatch") as CheckBox;

                    if (chkSelectDispatch != null)
                    {
                        if (dispatchViewDTO.Entities.Count > 0)
                        {
                            if (string.IsNullOrEmpty(dispatchViewDTO.Entities[e.Row.DataItemIndex].StateInterface))
                                chkSelectDispatch.Visible = true;
                            else
                                chkSelectDispatch.Visible = false;
                        }
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "chkSelectDispatch"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void imbDeleteReceipt_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.DispatchConfirm.Detail))
                {
                    var dispatchesDetailSelected = new List<DispatchDetail>();
                    dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.DispatchConfirm.Detail];

                    var dispatchSelected = (DispatchSpecial)Session[WMSTekSessions.DispatchConfirm.Selected];

                    for (int i = 0; i < grdDetail.Rows.Count; i++)
                    {
                        var row = grdDetail.Rows[i];

                        var chkSelectDispatchDetail = (CheckBox)row.FindControl("chkSelectDispatchDetail");

                        if (chkSelectDispatchDetail != null && chkSelectDispatchDetail.Checked)
                        {
                            var lblIdDispatchDetail = (Label)row.FindControl("lblIdDispatchDetail");

                            if (lblIdDispatchDetail != null)
                            {
                                int idDispatchDetail = int.Parse(lblIdDispatchDetail.Text.Trim());
                                var dispatchDetailSelected = dispatchDetailViewDTO.Entities.Where(dd => dd.Id == idDispatchDetail).FirstOrDefault();

                                if (dispatchDetailSelected != null)
                                {
                                    dispatchesDetailSelected.Add(dispatchDetailSelected);
                                }
                            }
                        }
                    }

                    if (dispatchesDetailSelected.Count > 0)
                    {
                        var dispatchesDetailToDelete = new List<DispatchDetail>();

                        foreach (var dispatchDetailSelected in dispatchesDetailSelected)
                        {
                            var dispatchDetailToDelete = dispatchDetailViewDTO.Entities.Where(dd => dd.Lpn.Code == dispatchDetailSelected.Lpn.Code).ToList();
                            dispatchesDetailToDelete.AddRange(dispatchDetailToDelete);
                        }

                        var cancelDispatchesDetail = iDispatchingMGR.CancelDispatchDetailByLpn(dispatchSelected, dispatchesDetailToDelete, context);

                        if (cancelDispatchesDetail.hasError())
                        {
                            this.Master.ucError.ShowError(cancelDispatchesDetail.Errors);
                        }
                        else
                        {
                            if (currentIndexToLoadDetail != -1)
                            {
                                ReloadData();
                                var reloadData = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.DispatchConfirm.List];

                                if (dispatchSelected != null && reloadData != null && reloadData.Entities != null && reloadData.Entities.Count > 0)
                                {
                                    var existsDispatch = reloadData.Entities.Where(d => d.Id == dispatchSelected.Id).FirstOrDefault();

                                    if (existsDispatch != null)
                                    {
                                        LoadDetail(currentIndexToLoadDetail);
                                    }
                                    else
                                    {
                                        divDetail.Visible = false;
                                    }
                                }
                                else
                                {
                                    divDetail.Visible = false;
                                }
                            }
                            else
                            {
                                ReloadData();
                            }

                            ucStatus.ShowMessage(cancelDispatchesDetail.MessageStatus.Message);
                        }
                    }
                    else
                        ucStatus.ShowWarning(lblNoLpnSelected.Text);
                }
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void imbDeleteReceiptByLine_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.DispatchConfirm.Detail))
                {
                    var dispatchesDetailSelected = new List<DispatchDetail>();
                    dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.DispatchConfirm.Detail];

                    var dispatchSelected = (DispatchSpecial)Session[WMSTekSessions.DispatchConfirm.Selected];

                    for (int i = 0; i < grdDetail.Rows.Count; i++)
                    {
                        var row = grdDetail.Rows[i];

                        var chkSelectDispatchDetail = (CheckBox)row.FindControl("chkSelectDispDetailByLine");

                        if (chkSelectDispatchDetail != null && chkSelectDispatchDetail.Checked)
                        {
                            var lblIdDispatchDetail = (Label)row.FindControl("lblIdDispatchDetail");

                            if (lblIdDispatchDetail != null)
                            {
                                int idDispatchDetail = int.Parse(lblIdDispatchDetail.Text.Trim());
                                var dispatchDetailSelected = dispatchDetailViewDTO.Entities.Where(dd => dd.Id == idDispatchDetail).FirstOrDefault();

                                if (dispatchDetailSelected != null)
                                {
                                    dispatchesDetailSelected.Add(dispatchDetailSelected);
                                }
                            }
                        }
                    }

                    if (dispatchesDetailSelected.Count > 0)
                    {
                        var cancelDispatchesDetail = iDispatchingMGR.CancelDispatchDetailByLine(dispatchSelected, dispatchesDetailSelected, context);

                        if (cancelDispatchesDetail.hasError())
                        {
                            this.Master.ucError.ShowError(cancelDispatchesDetail.Errors);
                        }
                        else
                        {
                            if (currentIndexToLoadDetail != -1)
                            {
                                ReloadData();
                                var reloadData = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.DispatchConfirm.List];

                                if (dispatchSelected != null && reloadData != null && reloadData.Entities != null && reloadData.Entities.Count > 0)
                                {
                                    var existsDispatch = reloadData.Entities.Where(d => d.Id == dispatchSelected.Id).FirstOrDefault();

                                    if (existsDispatch != null)
                                    {
                                        LoadDetail(currentIndexToLoadDetail);
                                    }
                                    else
                                    {
                                        divDetail.Visible = false;
                                    }
                                }
                                else
                                {
                                    divDetail.Visible = false;
                                }
                            }
                            else
                            {
                                ReloadData();
                            }

                            ucStatus.ShowMessage(cancelDispatchesDetail.MessageStatus.Message);
                        }
                    }
                    else
                        ucStatus.ShowWarning(lblNoLpnSelected.Text);
                }
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void grdDetail_DataBound(object sender, EventArgs e)
        {
            try
            {
                for (int i = grdDetail.Rows.Count - 1; i > 0; i--)
                {
                    int pageInd = grdDetail.PageSize * grdDetail.PageIndex;

                    var distpachDetailRow = dispatchDetailViewDTO.Entities[i + pageInd];
                    var dispatchDetailpreviousRow = dispatchDetailViewDTO.Entities[(i + pageInd) - 1];

                    GridViewRow row = grdDetail.Rows[i];
                    GridViewRow previousRow = grdDetail.Rows[i - 1];

                    int positionColumnCheckbox = 0;

                    if (!string.IsNullOrEmpty(distpachDetailRow.Lpn.IdCode) && !string.IsNullOrEmpty(dispatchDetailpreviousRow.Lpn.IdCode))
                    {
                        if (distpachDetailRow.Lpn.IdCode == dispatchDetailpreviousRow.Lpn.IdCode)
                        {
                            int positionColumnLpn = 2;
                            //Lpn
                            if (previousRow.Cells[positionColumnLpn].RowSpan == 0)
                            {
                                if (row.Cells[positionColumnLpn].RowSpan == 0)
                                {
                                    previousRow.Cells[positionColumnCheckbox].RowSpan += 2;
                                    previousRow.Cells[positionColumnLpn].RowSpan += 2;
                                }
                                else
                                {
                                    previousRow.Cells[positionColumnCheckbox].RowSpan = row.Cells[positionColumnCheckbox].RowSpan + 1;
                                    previousRow.Cells[positionColumnLpn].RowSpan = row.Cells[positionColumnLpn].RowSpan + 1;
                                }
                                row.Cells[positionColumnCheckbox].Visible = false;
                                row.Cells[positionColumnLpn].Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedDispatchesToUpdateInterface = new List<DispatchSpecial>();

                dispatchViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.DispatchConfirm.List];

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    var chkSelectDispatch = (CheckBox)row.FindControl("chkSelectDispatch");

                    if (chkSelectDispatch != null && chkSelectDispatch.Checked)
                    {
                        var lblIdDispatch = (Label)row.FindControl("lblIdDispatch");

                        if (lblIdDispatch != null)
                        {
                            var dispatchId = int.Parse(lblIdDispatch.Text.Trim());
                            var dispatchSelected = dispatchViewDTO.Entities.Where(d => d.Id == dispatchId).FirstOrDefault();

                            if (dispatchSelected != null)
                                selectedDispatchesToUpdateInterface.Add(dispatchSelected);
                        }
                    }    
                }

                if (selectedDispatchesToUpdateInterface.Count > 0)
                {
                    Session.Add(WMSTekSessions.DispatchConfirm.SeletedToUpdateInterface, selectedDispatchesToUpdateInterface);
                    this.ShowConfirm(this.lblConfirmDispatchHeader.Text, lblConfirmDispatch.Text);
                }
                else
                    this.Master.ucDialog.ShowAlert(this.lblConfirmDispatchHeader.Text, this.lblNotSelectedDispatch.Text, "confirm");
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.DispatchConfirm.SeletedToUpdateInterface))
                {
                    var dispatchesSelected = (List<DispatchSpecial>)Session[WMSTekSessions.DispatchConfirm.SeletedToUpdateInterface];

                    if (dispatchesSelected.Count > 0)
                    {
                        dispatchViewDTO = iDispatchingMGR.UpdateInterfaceDispatchById(dispatchesSelected, context);

                        if (!dispatchViewDTO.hasError())
                        {
                            UpdateSession();
                            ucStatus.ShowMessage(dispatchViewDTO.MessageStatus.Message);
                        }
                        else
                        {
                            this.Master.ucError.ShowError(dispatchViewDTO.Errors);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                modalPopUpDialog.Hide();
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
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
        #endregion

        #region "Métodos"
        protected void Initialize()
        {
            context.SessionInfo.IdPage = "DispatchConfirm";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeGridDetail();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
            }
        }
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirm_Click);
            this.Master.ucTaskBar.btnSaveVisible = true;
            this.Master.ucTaskBar.btnSaveToolTip = this.lblBtnSaveToolTip.Text;
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.outboundTypeVisible = true;
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = lblFilterReferenceNumber.Text;

            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblFilterLPN.Text;

            this.Master.ucMainFilter.dispatchTypeVisible = true;
            this.Master.ucMainFilter.dispatchTypeNotIncludeAll = false;
            this.Master.ucMainFilter.listDispatchType = new List<String>();

            var listDispatchTypeAllowedToDeleteLpn = GetConst("DispatchTypeAllowedToDeleteLPNWhenConfirmDispatch");

            if (listDispatchTypeAllowedToDeleteLpn.Count > 0)
                this.Master.ucMainFilter.listDispatchType = listDispatchTypeAllowedToDeleteLpn;
            else
                ucStatus.ShowWarning(lblNoConfiguredDispatchesType.Text);

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            this.Master.ucMainFilter.advancedFilterVisible = true;

            this.Master.ucMainFilter.tabDispatchingVisible = true;
            this.Master.ucMainFilter.tabDispatchingHeaderText = this.lblAdvancedFilter.Text;
            this.Master.ucMainFilter.divDispatchingPriorityVisible = true;

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
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }
        private void InitializeGridDetail()
        {
            try
            {
                //grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByDetailGridPage.ToString()));
                grdDetail.EmptyDataText = this.Master.EmptyGridText;
            }
            catch (Exception ex)
            {
                dispatchViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
        }
        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            dispatchViewDTO = iDispatchingMGR.GetDispatchSpecialHeaderForDispatchConfirm(context);

            if (!dispatchViewDTO.hasError() && dispatchViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.DispatchConfirm.List, dispatchViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(dispatchViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucError.ShowError(dispatchViewDTO.Errors);
            }
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
        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                dispatchViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.DispatchConfirm.List];

                var dispatchSelected = dispatchViewDTO.Entities[index];

                var subQueryParams = new Dictionary<string, string>();
                subQueryParams.Add("SubQueryCode", "OrderByLPN"); 

                dispatchDetailViewDTO = iDispatchingMGR.GetByAnyParameterDispatchDetail(new DispatchDetail() { Dispatch = new Dispatch() { Id = dispatchSelected.Id } }, subQueryParams, context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    imbDeleteReceipt.Visible = true;
                    imbDeleteReceiptByLine.Visible = true;

                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();
                }
                else
                {
                    imbDeleteReceipt.Visible = false;
                    imbDeleteReceiptByLine.Visible = false;
                }

                CallJsGridViewDetail();
                lblNroDoc.Text = dispatchSelected.OutboundOrder.Number;

                Session.Add(WMSTekSessions.DispatchConfirm.Selected, dispatchSelected);
                Session.Add(WMSTekSessions.DispatchConfirm.Detail, dispatchDetailViewDTO);
                divDetail.Visible = dispatchDetailViewDTO.Entities.Count > 0;
            }
            else
            {
                divDetail.Visible = false;
            }
        }
        private void PopulateGrid()
        {
            dispatchViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.DispatchConfirm.List];

            if (dispatchViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            if (!dispatchViewDTO.hasConfigurationError() && dispatchViewDTO.Configuration != null && dispatchViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchViewDTO.Configuration);

            grdMgr.DataSource = dispatchViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(dispatchViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
            upGrid.Update();
        }
        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.DispatchConfirm.Detail];

                if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }
        private void ShowConfirm(string title, string message)
        {
            this.divConfirm.Visible = true;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;

            modalPopUpDialog.Show();
        }
        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('DispatchConfirmDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, false);", true);
        }

        #endregion
    }
}