using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.DTO;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Stocks.Consult
{
    public partial class ReserveStockLog : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<ReserveStock> reserveStockViewDTO = new GenericViewDTO<ReserveStock>();
        private GenericViewDTO<Framework.Entities.Warehousing.ReserveStockLog> reserveStockLogViewDTO = new GenericViewDTO<Framework.Entities.Warehousing.ReserveStockLog>();
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    if (isValidViewDTO && Page.IsPostBack)
                    {
                        PopulateGrid();
                        PopulateGridDetail();
                    }
                }
            
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndexToLoadDetail = index;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadDetail(index);
                }
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                    LoadDetail(index);

                    var reserveStock = (ReserveStock)Session[WMSTekSessions.ReserveStockLog.Selected];
                    detailTitle = "Cliente " + reserveStock.NameCustomer;
                    base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                }

                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
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
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void grdDetail_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                PopulateGrid();

                var detail = (GenericViewDTO<Framework.Entities.Warehousing.ReserveStockLog>)Session[WMSTekSessions.ReserveStockLog.Detail];

                grdDetail.DataSource = detail.Entities;
                grdDetail.DataBind();
                grdDetail.PageIndex = e.NewPageIndex;
                grdDetail.DataBind();

                CallJsGridViewDetail();
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }
        #endregion

        #region "Metodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReserveStockLog";

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
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;

            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.itemVisible = true;

            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblCustomer.Text;

            this.Master.ucMainFilter.divEnableReserveStockOnZeroVisible = true;

            //this.Master.ucMainFilter.dateFromVisible = true;
            //this.Master.ucMainFilter.dateToVisible = true;

            //this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            //this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            //this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

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
                grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByDetailGridPage.ToString()));
                grdDetail.EmptyDataText = this.Master.EmptyGridText;
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }

        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                reserveStockViewDTO = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.ReserveStockLog.List];

                var reserveStockSelected = reserveStockViewDTO.Entities[index];

                reserveStockLogViewDTO = iWarehousingMGR.FindAllReserveStockLog(reserveStockSelected.IdOwn, reserveStockSelected.Idwhs, reserveStockSelected.IdItem, reserveStockSelected.IdCustomer, context);

                if (reserveStockLogViewDTO != null && reserveStockLogViewDTO.Entities.Count > 0)
                {
                    if (!reserveStockLogViewDTO.hasConfigurationError() && reserveStockLogViewDTO.Configuration != null && reserveStockLogViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, reserveStockLogViewDTO.Configuration);

                    grdDetail.DataSource = reserveStockLogViewDTO.Entities;
                    grdDetail.DataBind();

                    lblReserveStockSelected.Text = "Cliente: " + reserveStockSelected.NameCustomer + " - Item: " + reserveStockSelected.ShortNameItem;
                    lblReserveStockSelected2.Text = "Bodega: " + reserveStockSelected.Warehouse.Name + " - Dueño: " + reserveStockSelected.Owner.Name;

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.ReserveStockLog.Selected, reserveStockViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.ReserveStockLog.Detail, reserveStockLogViewDTO);
                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
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

            var filterItem = context.MainFilter.Where(f => f.Name == "Item").FirstOrDefault();

            if (filterItem != null && filterItem.FilterValues.Count > 0)
            {
                var itemCode = filterItem.FilterValues.FirstOrDefault().Value;

                if (!string.IsNullOrEmpty(itemCode))
                {
                    var filterOwner = context.MainFilter.Where(f => f.Name == "Owner").FirstOrDefault();
                    var idOwn = int.Parse(filterOwner.FilterValues.First().Value);

                    var itemViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, itemCode, idOwn, true);

                    if (!itemViewDTO.hasError() && itemViewDTO.Entities.Count > 0)
                    {
                        filterItem.FilterValues.First().Value = itemViewDTO.Entities.First().Id.ToString();
                    }
                }
            }

            var filterCustomer = context.MainFilter.Where(f => f.Name == "Name").FirstOrDefault();

            if (filterCustomer != null && filterCustomer.FilterValues.Count > 0)
            {
                var indexCustomer = Convert.ToInt16(EntityFilterName.Customer);

                context.MainFilter[indexCustomer].FilterValues = new List<Framework.Entities.Base.FilterItem>();
                context.MainFilter[indexCustomer].FilterValues.Add(new Framework.Entities.Base.FilterItem(filterCustomer.FilterValues.First().Value.Trim()));
            }

            var enableDeleteReserveStockLog = false;

            var filterEnableReserveStockOnZero = new List<bool>();
            var chkdivEnableReserveStockOnZero = (CheckBox)this.Master.ucMainFilter.FindControl("chkdivEnableReserveStockOnZero");

            if (chkdivEnableReserveStockOnZero != null && chkdivEnableReserveStockOnZero.Checked)
                enableDeleteReserveStockLog = true;

            filterEnableReserveStockOnZero.Add(enableDeleteReserveStockLog);

            ClearFilter("EnableReserveStockOnZero");
            CreateFilterByList("EnableReserveStockOnZero", filterEnableReserveStockOnZero);

            reserveStockViewDTO = iWarehousingMGR.ReserveStockLogHeader(context);

            if (!reserveStockViewDTO.hasError() && reserveStockViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ReserveStockLog.List, reserveStockViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(reserveStockViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            reserveStockViewDTO = (GenericViewDTO<ReserveStock>)Session[WMSTekSessions.ReserveStockLog.List];

            if (reserveStockViewDTO != null)
            {
                if (!reserveStockViewDTO.hasConfigurationError() && reserveStockViewDTO.Configuration != null && reserveStockViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdMgr, reserveStockViewDTO.Configuration);

                grdMgr.DataSource = reserveStockViewDTO.Entities;
                grdMgr.DataBind();

                ucStatus.ShowRecordInfo(reserveStockViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

                CallJsGridViewHeader();
            }
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                reserveStockLogViewDTO = (GenericViewDTO<Framework.Entities.Warehousing.ReserveStockLog>)Session[WMSTekSessions.ReserveStockLog.Detail];

                if (!reserveStockLogViewDTO.hasConfigurationError() && reserveStockLogViewDTO.Configuration != null && reserveStockLogViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, reserveStockLogViewDTO.Configuration);

                grdDetail.DataSource = reserveStockLogViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                reserveStockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reserveStockViewDTO.Errors);
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('ReserveStockLogDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, true);", true);
        }

        #endregion
    }
}