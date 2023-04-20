using Binaria.WMSTek.Framework.DataTransfer.Base;
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

namespace Binaria.WMSTek.WebClient.Stock.Consult
{
    public partial class RecordsBySerial : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Serial> serialViewDTO = new GenericViewDTO<Serial>();
        private GenericViewDTO<SerialTrack> serialTrackViewDTO = new GenericViewDTO<SerialTrack>();
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
                    Initialize();
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                        if (i == GetColumnIndexByName(e.Row, "chkSelectOutboundOrder"))
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
            context.SessionInfo.IdPage = "RecordsBySerial";

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
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = "Serie";

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

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
        }
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
            grdDetail.EmptyDataText = this.Master.EmptyGridText;
        }
        private void InitializeGridDetail()
        {
            try
            {
                grdDetail.EmptyDataText = this.Master.EmptyGridText;
            }
            catch (Exception ex)
            {
                serialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(serialViewDTO.Errors);
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
        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            serialViewDTO = iWarehousingMGR.FindAllHeaders(context);

            if (!serialViewDTO.hasError() && serialViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.RecordsBySerial.List, serialViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(serialViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucError.ShowError(serialViewDTO.Errors);
            }
        }
        private void PopulateGrid()
        {
            serialViewDTO = (GenericViewDTO<Serial>)Session[WMSTekSessions.RecordsBySerial.List];

            if (serialViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            //if (!serialViewDTO.hasConfigurationError() && serialViewDTO.Configuration != null && serialViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, serialViewDTO.Configuration);

            grdMgr.DataSource = serialViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(serialViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
            upGrid.Update();
        }
        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                serialTrackViewDTO = (GenericViewDTO<SerialTrack>)Session[WMSTekSessions.RecordsBySerial.Detail];

                if (!serialTrackViewDTO.hasConfigurationError() && serialTrackViewDTO.Configuration != null && serialTrackViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, serialTrackViewDTO.Configuration);

                grdDetail.DataSource = serialTrackViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }
        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                serialViewDTO = (GenericViewDTO<Serial>)Session[WMSTekSessions.RecordsBySerial.List];

                var selectedSerial = serialViewDTO.Entities[index];

                serialTrackViewDTO = iWarehousingMGR.GetSerialTrackBySerial(selectedSerial.Id, context);

                if (serialTrackViewDTO != null && serialTrackViewDTO.Entities.Count > 0)
                {
                    if (!serialTrackViewDTO.hasConfigurationError() && serialTrackViewDTO.Configuration != null && serialTrackViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, serialTrackViewDTO.Configuration);

                    grdDetail.DataSource = serialTrackViewDTO.Entities;
                    grdDetail.DataBind();
                }

                CallJsGridViewDetail();
                lblSerial.Text = selectedSerial.SerialNumber;

                Session.Add(WMSTekSessions.RecordsBySerial.Selected, selectedSerial);
                Session.Add(WMSTekSessions.RecordsBySerial.Detail, serialTrackViewDTO);
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
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('SerialTrack_GetBySerial', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, false);", true);
        }
        #endregion
    }
}