using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Utility;
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
    public partial class CheckOrdersWaveInQueue : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<TaskQueue> taskQueueViewDTO = new GenericViewDTO<TaskQueue>();
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

        private String docSimUrl
        {
            get
            {
                if (Request.QueryString["WAVE"] != null && Request.QueryString["WAVE"] != String.Empty)
                    return Request.QueryString["WAVE"];
                else
                    return String.Empty;
            }
        }
        private int whsSimUrl
        {
            get
            {
                if (Request.QueryString["WHS"] != null && Request.QueryString["WHS"] != String.Empty)
                    return int.Parse(Request.QueryString["WHS"]);
                else
                    return -1;
            }
        }
        private String ownSimUrl
        {
            get
            {
                if (Request.QueryString["OWN"] != null && Request.QueryString["OWN"] != String.Empty)
                    return Request.QueryString["OWN"];
                else
                    return String.Empty;
            }
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

                    if (!IsPostBack)
                    {
                        if (!string.IsNullOrEmpty(docSimUrl))
                        {
                            TextBox txtNumber = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCodeNumeric");
                            txtNumber.Text = docSimUrl;
                        }
                        if (whsSimUrl > 0)
                        {
                            DropDownList ddlWarehouse = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
                            ddlWarehouse.SelectedValue = whsSimUrl.ToString();
                        }
                        if (!string.IsNullOrEmpty(ownSimUrl))
                        {
                            DropDownList ddlOwner = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
                            ddlOwner.SelectedValue = ownSimUrl.ToString();
                        }

                        ReloadData();
                    }
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
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
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var btnCancelTaskQueue = e.Row.FindControl("btnCancel") as ImageButton;

                    if (btnCancelTaskQueue != null)
                    {
                        btnCancelTaskQueue.OnClientClick = "if(confirm('" + lblConfirmCancel.Text + "')==false){return false;}";

                        if (taskQueueViewDTO.Entities.Count > 0 && taskQueueViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Waiting)
                        {
                            btnCancelTaskQueue.Visible = true;
                        }
                        else
                        {
                            btnCancelTaskQueue.Visible = false;
                        }
                    }

                    var btnRetryTaskQueue = e.Row.FindControl("btnEdit") as ImageButton;

                    if (btnRetryTaskQueue != null)
                    {
                        btnRetryTaskQueue.OnClientClick = "if(confirm('" + lblRetryCancel.Text + "')==false){return false;}";

                        if (taskQueueViewDTO.Entities.Count > 0 && (taskQueueViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Error || taskQueueViewDTO.Entities[e.Row.DataItemIndex].TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Cancel))
                        {
                            btnRetryTaskQueue.Visible = true;
                        }
                        else
                        {
                            btnRetryTaskQueue.Visible = false;
                        }
                    }

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");

                    }
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "CloseOrder")
                {
                    //CloseOrder(index);
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
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
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
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
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                    e.Row.TableSection = TableRowSection.TableHeader;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var taskQueue = (TaskQueue)e.Row.DataItem;

                    string cssClass = null;

                    switch ((eTrackTaskQueue)taskQueue.TrackTaskQueue.IdTrackTaskQueue)
                    {
                        case eTrackTaskQueue.InProcess:
                            cssClass = "blueAlert";
                            break;
                        case eTrackTaskQueue.Successful:
                            cssClass = "greenAlert";
                            break;
                        case eTrackTaskQueue.Error:
                            cssClass = "redAlert";
                            break;
                        case eTrackTaskQueue.Cancel:
                            cssClass = "yellowAlert";
                            break;
                        default:
                            break;
                    }

                    if (!string.IsNullOrEmpty(cssClass))
                        e.Row.CssClass = cssClass;
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int retryIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                taskQueueViewDTO = (GenericViewDTO<TaskQueue>)Session[WMSTekSessions.CheckOrdersWaveInQueue.OrdersList];
                var taskQueueToRetry = taskQueueViewDTO.Entities[retryIndex];

                taskQueueToRetry.TrackTaskQueue.IdTrackTaskQueue = (int)eTrackTaskQueue.Waiting;
                var retryTaskQueueViewDTO = iDispatchingMGR.UpdateTaskQueue(taskQueueToRetry, context);

                if (retryTaskQueueViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(retryTaskQueueViewDTO.Errors);
                }
                else
                {
                    UpdateSession();
                    ucStatus.ShowMessage(retryTaskQueueViewDTO.MessageStatus.Message);
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int cancelIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;
                taskQueueViewDTO = (GenericViewDTO<TaskQueue>)Session[WMSTekSessions.CheckOrdersWaveInQueue.OrdersList];
                var taskQueueToCancel = taskQueueViewDTO.Entities[cancelIndex];

                taskQueueToCancel.TrackTaskQueue.IdTrackTaskQueue = (int)eTrackTaskQueue.Cancel;
                var cancelTaskQueueViewDTO =  iDispatchingMGR.UpdateTaskQueue(taskQueueToCancel, context);

                if (cancelTaskQueueViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(cancelTaskQueueViewDTO.Errors);
                }
                else
                {
                    UpdateSession();
                    ucStatus.ShowMessage(cancelTaskQueueViewDTO.MessageStatus.Message);
                }
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
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
            context.SessionInfo.IdPage = "CheckOrdersWaveInQueue";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            Master.ucMainFilter.divTaskQueueFilterVisible = true;

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = false;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = false;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.codeNumericVisible = true;
            this.Master.ucMainFilter.codeNumericLabel = lblDocName.Text;

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

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            if (!taskQueueViewDTO.hasConfigurationError() && taskQueueViewDTO.Configuration != null && taskQueueViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskQueueViewDTO.Configuration);

            grdMgr.DataSource = taskQueueViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(taskQueueViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;

                upGrid.Update();
            }
        }

        private void UpdateSession()
        {
            this.Master.ucMainFilter.ClearFilterObject();
            this.Master.ucMainFilter.LoadControlValuesToFilterObject();

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            ClearFilter("IdTypeTask");
            CreateFilterByList("IdTypeTask", new List<int>() { (int)eTypeTaskQueue.ReleaseWave });

            taskQueueViewDTO = iDispatchingMGR.GetOrdersWaveInQueue(context);

            if (!taskQueueViewDTO.hasError() && taskQueueViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CheckOrdersWaveInQueue.OrdersList, taskQueueViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(taskQueueViewDTO.MessageStatus.Message);
            }
            else
            {
                if (taskQueueViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                {
                    this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
                }
                else
                {
                    Session.Remove(WMSTekSessions.CheckOrdersWaveInQueue.OrdersList);
                }
            }
        }

        #endregion
    }
}