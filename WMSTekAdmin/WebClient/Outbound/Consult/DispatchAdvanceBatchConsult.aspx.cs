using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
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
    public partial class DispatchAdvanceBatchConsult : BasePage
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

                if (base.webMode == WebMode.Normal) 
                    Initialize();
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
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                currentIndex = grdMgr.SelectedIndex;
                currentPage = grdMgr.PageIndex;

                LoadDetail(index);
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
                dispatchAdvanceViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchAdvanceViewDTO.Errors);
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

                    var dispatchSelected = (DispatchAdvanced)Session[WMSTekSessions.OutboundConsult.DispatchBatchSelected];
                    detailTitle = "Batch " + dispatchSelected.Outbound.Id.ToString();
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
                if (ValidateSession(WMSTekSessions.OutboundConsult.DispatchBatchList))
                {
                    dispatchAdvanceViewDTO = (GenericViewDTO<DispatchAdvanced>)Session[WMSTekSessions.OutboundConsult.DispatchBatchList];
                    isValidViewDTO = true;
                }
            }
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            Master.ucMainFilter.codeNumericVisible = true;
            Master.ucMainFilter.codeNumericLabel = lblBatchNbr.Text;
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

            dispatchAdvanceViewDTO = iDispatchingMGR.FindAllDispatchAdvancedBatch(context);

            if (!dispatchAdvanceViewDTO.hasError() && dispatchAdvanceViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundConsult.DispatchBatchList, dispatchAdvanceViewDTO);
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

            if (!dispatchAdvanceViewDTO.hasConfigurationError() && dispatchAdvanceViewDTO.Configuration != null && dispatchAdvanceViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchAdvanceViewDTO.Configuration);

            grdMgr.DataSource = dispatchAdvanceViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(dispatchAdvanceViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
                var dispatchSelected = dispatchAdvanceViewDTO.Entities[index];
                var id = dispatchSelected.Id;

                this.lblNroDoc.Text = dispatchSelected.Outbound.Id.ToString();

                dispatchDetailViewDTO = iDispatchingMGR.GetDetailByIdDispatchAdvancedBatch(id, context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.OutboundConsult.DispatchBatchSelected, dispatchSelected);
                Session.Add(WMSTekSessions.OutboundConsult.DispatchBatchDetail, dispatchDetailViewDTO);

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
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('DispatchAdvanced_GetByBatchId', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }
        #endregion
    }
}