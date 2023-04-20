using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DTO = Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Stock.Consult
{
    public partial class StockAge : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<DTO.Stock> stockViewDTO = new GenericViewDTO<DTO.Stock>();
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                UpdateSession();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                UpdateSession();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                UpdateSession();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                UpdateSession();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                UpdateSession();
                PopulateGrid();
            }
            catch (Exception ex)
            {
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
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
                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }


        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                UpdateSession();
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

                stockViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }

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
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.StockConsult.StockList))
                {
                    stockViewDTO = (GenericViewDTO<DTO.Stock>)Session[WMSTekSessions.StockConsult.StockAgeList];
                    isValidViewDTO = true;
                }
            }
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            //this.Master.ucMainFilter.locationFilterVisible = true;
            //this.Master.ucMainFilter.dateVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblLpnFilter.Text;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblLocationFilter.Text;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }
        private void InitializeTaskBar()
        {

            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            //Master.ucMainFilter.FilterOwnerAutoPostBack = true;
            //this.Master.ucMainFilter.ddlOwnerIndexChanged += new EventHandler(ddlOwnerIndexChanged);

            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnRefreshVisible = true;
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

            stockViewDTO = iWarehousingMGR.FindAllStockAge(context);

            if (!stockViewDTO.hasError() && stockViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StockConsult.StockAgeList, stockViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(stockViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(stockViewDTO.Errors);
            }
        }
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            if (!stockViewDTO.hasConfigurationError() && stockViewDTO.Configuration != null && stockViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, stockViewDTO.Configuration);

            grdMgr.DataSource = stockViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(stockViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }
        protected void ReloadData()
        {
            UpdateSession();

            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('Stock_FindAllStockAge', 'ctl00_MainContent_grdMgr');", true);
        }
        #endregion
    }
}