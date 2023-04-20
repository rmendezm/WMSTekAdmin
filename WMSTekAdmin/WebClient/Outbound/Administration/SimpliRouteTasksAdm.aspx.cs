using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.DataAccess.Layout;
using Binaria.WMSTek.DataAccess.Utility;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Utility;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class SimpliRouteTasksAdm : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskQueue> taskQueueViewDTO = new GenericViewDTO<TaskQueue>();
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
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

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
                //    LoadPackageDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}
                GridView grdMgrTemp = grdMgr;
                try
                {
                    grdMgrTemp.Columns.RemoveAt(5);
                }
                catch
                {

                }
                
                base.ExportToExcel(grdMgrTemp, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        /// 
        protected void btnExcelDetail_Click(object sender, EventArgs e)
        {
            GridView grdMgrAux = new GridView();
            ContextViewDTO contextAux = new ContextViewDTO();
            GenericViewDTO<PackageConsult> packageAuxViewDTO = new GenericViewDTO<PackageConsult>();
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    grdMgrAux = grdMgr;
                    contextAux.MainFilter = this.Master.ucMainFilter.MainFilter;
                    contextAux.SessionInfo = context.SessionInfo;
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Clear();
                    contextAux.MainFilter[Convert.ToInt16(EntityFilterName.LpnCode)].FilterValues.Add(new FilterItem(taskQueueViewDTO.Entities[index].IdLpnCode));
                    // packageAuxViewDTO = iDispatchingMGR.FindSimpliRoutePackage(contextAux);
                    //taskQueueViewDTO = iDispatchingMGR.GetOrdersInQueue(contextAux);
                    //grdMgrAux.DataSource = taskQueueViewDTO.Entities;
                    List<TaskQueue> listTemp = new List<TaskQueue>();
                    listTemp.Add(taskQueueViewDTO.Entities[index]);
                    grdMgrAux.DataSource = listTemp;
                    grdMgrAux.DataBind();
                    LoadDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;

                    // Validar si es csv o excel
                    bool willBeCsv = true;

                    var value = context.CfgParameters.Where(param => param.ParameterCode == "DownloadFileWillBeCsv").FirstOrDefault();

                    GridView grdMgrTemp = grdMgr;
                    try
                    {
                        grdMgrTemp.Columns.RemoveAt(5);
                    }
                    catch
                    {

                    }

                     
                    if (value != null)
                    {
                        if (value.ParameterValue == "0")
                            willBeCsv = false;
                    }
                    if (willBeCsv)
                    {
                        base.ExportToExcel(grdMgrTemp, grdDetail, detailTitle);
                    }
                    else
                    {
                        base.ExportToExcel(grdMgrTemp, grdDetail, detailTitle);
                    }

                }

                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
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
                        PopulateGridDetail();
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
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
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                currentIndex = grdMgr.SelectedIndex;

                LoadDetail(index);
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

                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
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
        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int retryIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;
                taskQueueViewDTO = (GenericViewDTO<TaskQueue>)Session[WMSTekSessions.SimpliRouteTasksAdm.List];
                var taskQueueToRetry = taskQueueViewDTO.Entities[retryIndex];

                int trackId = (int)eTrackTaskQueue.Waiting;
                /*
                if (string.IsNullOrEmpty(taskQueueToRetry.SpecialField1))
                {
                    trackId = (int)eTrackTaskQueue.InProcess;
                }
                else if (string.IsNullOrEmpty(taskQueueToRetry.SpecialField3))
                {
                    trackId = (int)eTrackTaskQueue.FirstStepSuccessful;
                }
                else if (string.IsNullOrEmpty(taskQueueToRetry.SpecialField4))
                {
                    trackId = (int)eTrackTaskQueue.SecondStepSuccessful;
                }
                */

                taskQueueToRetry.TrackTaskQueue.IdTrackTaskQueue = trackId;
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
                taskQueueViewDTO = (GenericViewDTO<TaskQueue>)Session[WMSTekSessions.SimpliRouteTasksAdm.List];
                var taskQueueToCancel = taskQueueViewDTO.Entities[cancelIndex];

                taskQueueToCancel.TrackTaskQueue.IdTrackTaskQueue = (int)eTrackTaskQueue.Cancel;
                var cancelTaskQueueViewDTO = iDispatchingMGR.UpdateTaskQueue(taskQueueToCancel, context);

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
        protected void btnApiDetail_Click(object sender, EventArgs e)
        {
            try
            {
                var taskQueueSelected = (TaskQueue)Session[WMSTekSessions.SimpliRouteTasksAdm.Selected];

                SimpliRouteVisitDAO simpliRouteVisitDAO = new SimpliRouteVisitDAO();
                SimpliRouteVisit theSimpliRouteVisit = new SimpliRouteVisit();

                theSimpliRouteVisit.IdLpnCode = taskQueueSelected.IdLpnCode;

                // Mantenedor CRUD
                /*
                var newSRV = new SimpliRouteVisit();
                newSRV.IdLpnCode = "lpn1";
                newSRV.IdSimpliRouteVisit = 12;
                newSRV.ListXml = "<xml><hola12></hola12></xml>";
                newSRV.Route = "rutita";
                var simpliRouteVisitViewDTOInsert = iDispatchingMGR.MaintainSimpliRouteVisit(CRUD.Create, newSRV, context);


                var modifySRV = new SimpliRouteVisit();
                modifySRV.IdLpnCode = "lpn1edited";
                //modifySRV.IdSimpliRouteVisit = 12;
                modifySRV.ListXml = "<xml><edited></edited></xml>";
                modifySRV.IdSimpliRouteVisit = 12;
                modifySRV.Route = "rutaEditada";
                modifySRV.IdTruckCode = "truckEd";
                modifySRV.DateCreated = DateTime.Now;
                var simpliRouteVisitViewDTOUpdate = iDispatchingMGR.MaintainSimpliRouteVisit(CRUD.Update, modifySRV, context);


                var deleteSRV = modifySRV;
                deleteSRV.IdSimpliRouteVisit = 1;
                var simpliRouteVisitViewDTODelete = iDispatchingMGR.MaintainSimpliRouteVisit(CRUD.Delete, deleteSRV, context);
                */

                var simpliRouteVisitViewDTO = iDispatchingMGR.SimpliRouteVisitGetByAnyParameter(theSimpliRouteVisit, context);
                if (simpliRouteVisitViewDTO.Entities.Count > 0)
                {

                    if (taskQueueSelected != null && simpliRouteVisitViewDTO.Entities.Count > 0)
                    {

                        txtIdSimpliRouteVisit.Text = simpliRouteVisitViewDTO.Entities[0].IdSimpliRouteVisit.ToString();
                        txtIdVisit.Text = simpliRouteVisitViewDTO.Entities[0].IdVisit.ToString();
                        txtIdRoute.Text = simpliRouteVisitViewDTO.Entities[0].Route;
                        txtIdLpnCode.Text = simpliRouteVisitViewDTO.Entities[0].IdLpnCode;
                        //txtIdTruckCode.Text = simpliRouteVisitViewDTO.Entities[0].IdTruckCode;

                        divManifest.Visible = !string.IsNullOrEmpty(taskQueueSelected.SpecialField4);
                        linkManfiest.NavigateUrl = string.IsNullOrEmpty(taskQueueSelected.SpecialField4) ? string.Empty : taskQueueSelected.SpecialField4;

                        //grdTracking.DataSource = 2;
                        grdTracking.DataBind();

                        OpenPopUp();

                    }


                }
                else
                {
                    ucStatus.ShowMessage("No se encontraron datos");
                }


            }
            catch (Exception ex)
            {
                taskQueueViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskQueueViewDTO.Errors);
            }
        }
        private void OpenPopUp()
        {
            CallJsGridViewDetail();
            divShowDetails.Visible = true;
            modalPopUpShowDetails.Show();
            upShowDetails.Update();
            //isValidViewDTO = false;
        }
        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            divShowDetails.Visible = false;
            modalPopUpShowDetails.Hide();
            upShowDetails.Update();
            isValidViewDTO = true;
        }
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
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
                    case eTrackTaskQueue.SecondStepSuccessful:
                        cssClass = "blueAlert";
                        break;
                    case eTrackTaskQueue.Successful:
                        cssClass = "greenAlert";
                        break;
                    case eTrackTaskQueue.FirstStepSuccessful:
                        cssClass = "blueAlert";
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
                divDetail.Visible = false;
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
                divDetail.Visible = false;
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
                divDetail.Visible = false;
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
                divDetail.Visible = false;
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
            context.SessionInfo.IdPage = "CourierTasksAdm";

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
                if (ValidateSession(WMSTekSessions.SimpliRouteTasksAdm.List))
                {
                    //taskQueueViewDTO = (GenericViewDTO<PackageConsult>)Session[WMSTekSessions.OutboundConsult.PackageList];
                    taskQueueViewDTO = (GenericViewDTO<TaskQueue>)Session[WMSTekSessions.SimpliRouteTasksAdm.List];
                    isValidViewDTO = true;
                }
                /*
                // Si es un ViewDTO valido, carga la grilla
                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
                    isValidViewDTO = false;

                    if (ValidateSession(WMSTekSessions.InboundConsult.ReceiptDetailList))
                    {
                        stockViewDTO = (GenericViewDTO<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>)Session[WMSTekSessions.InboundConsult.ReceiptDetailList];
                        isValidViewDTO = true;
                    }

                    if (isValidViewDTO)
                    {
                        PopulateGridDetail();
                    }
                }*/
            }
        }
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            Master.ucMainFilter.divTaskQueueFilterVisible2 = true;

            // Habilita criterios a usar
            //this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            //this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

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

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(taskQueueViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }
        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.SimpliRouteTasksAdm.Detail];

                // Configura ORDEN de las columnas de la grilla
                if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                // Detalle de Documentos de Entrada
                grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
            }
        }
        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                taskQueueViewDTO = (GenericViewDTO<TaskQueue>)Session[WMSTekSessions.SimpliRouteTasksAdm.List];

                var taskQueueSelected = taskQueueViewDTO.Entities[index];
                int idOutboundOrder = taskQueueSelected.IdDocumentBound;

                dispatchDetailViewDTO = iDispatchingMGR.GetLpnsForCourierByOutboundOrder(idOutboundOrder, context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.SimpliRouteTasksAdm.Selected, taskQueueSelected);
                Session.Add(WMSTekSessions.SimpliRouteTasksAdm.Detail, dispatchDetailViewDTO);

                this.lblNroDoc.Text = taskQueueSelected.NumberDocumentBound;


                switch ((eTrackTaskQueue)taskQueueSelected.TrackTaskQueue.IdTrackTaskQueue)
                {
                    case eTrackTaskQueue.InProcess:
                        this.lblStatusDescription.Text = "Petición En Proceso";
                        break;
                    case eTrackTaskQueue.Waiting:
                        this.lblStatusDescription.Text = "En Cola";
                        break;

                    case eTrackTaskQueue.Successful:
                        this.lblStatusDescription.Text = "Ruta Exitosamente Creada";
                        break;
                    case eTrackTaskQueue.SecondStepSuccessful:
                        this.lblStatusDescription.Text = "Ruta Recibida";
                        break;

                    case eTrackTaskQueue.FirstStepSuccessful:
                        this.lblStatusDescription.Text = "Petición Enviada A SimpliRoute";
                        break;
                    case eTrackTaskQueue.Error:
                        this.lblStatusDescription.Text = "Error Al Crear Ruta";
                        break;
                    case eTrackTaskQueue.Cancel:
                        this.lblStatusDescription.Text = "Creación De Ruta Cancelada";
                        break;

                    default:
                        break;
                }

                if (taskQueueSelected.TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.SecondStepSuccessful ||
                    taskQueueSelected.TrackTaskQueue.IdTrackTaskQueue == (int)eTrackTaskQueue.Successful)
                {
                    btnApiDetail.Enabled = true;
                }
                else
                {
                    btnApiDetail.Enabled = false;
                }

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

            // Si es un ViewDTO valido, carga la grilla
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

            ClearFilter("IdTypeTask");
            CreateFilterByList("IdTypeTask", new List<int>() { (int)eTypeTaskQueue.SimplirouteIntegration });
            int idTaskQueueFilterTemp = (int)this.Master.ucMainFilter.idTaskQueueFilter;
            if (idTaskQueueFilterTemp >= 0)
            {
                CreateFilterByList("IdTrackTaskQueue", new List<int>() { idTaskQueueFilterTemp });
            }
            else
            {
                ClearFilter("IdTrackTaskQueue");
            }



            taskQueueViewDTO = iDispatchingMGR.GetOrdersInQueue(context);

            if (!taskQueueViewDTO.hasError() && taskQueueViewDTO.Entities != null)
            {
                foreach (var ele in taskQueueViewDTO.Entities)
                {

                }
                Session.Add(WMSTekSessions.SimpliRouteTasksAdm.List, taskQueueViewDTO);
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
                    Session.Remove(WMSTekSessions.SimpliRouteTasksAdm.List);
                }
            }
        }
        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('ABCDin', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }
        #endregion
    }
}