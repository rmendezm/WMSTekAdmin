using Binaria.WMSTek.Framework.DataTransfer.Base;
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
    public partial class OutboundOrdersPendingReplenishment : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<TaskConsult> taskConsultViewDTO = new GenericViewDTO<TaskConsult>();
        private GenericViewDTO<TaskDetail> taskDetailViewDTO = new GenericViewDTO<TaskDetail>();
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

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();
                }

            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                taskConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksPendingReplenishment.List];
                PopulateGrid();
                upGrid.Update();
                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
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
                    grdMgr.AllowPaging = false;

                    var taskSelected = (TaskConsult)Session[WMSTekSessions.TasksPendingReplenishment.Selected];
                    taskConsultViewDTO = new GenericViewDTO<TaskConsult>();
                    taskConsultViewDTO.Entities = new List<TaskConsult>();
                    taskConsultViewDTO.Entities.Add(taskSelected);

                    PopulateGrid();
                    upGrid.Update();

                    LoadDetail(index);

                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;

                    base.ExportToExcel(grdMgr, grdDetail, detailTitle);

                    taskConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksPendingReplenishment.List];

                    PopulateGrid();
                    upGrid.Update();

                    grdMgr.AllowPaging = true;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                taskConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "TasksPendingReplenishment";

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
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnExcelDetailClick += new EventHandler(btnExcelDetail_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnExcelDetailVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.DocumentNumberLabel = lblDocName.Text;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

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

            // Configura ORDEN de las columnas de la grilla
            if (!taskConsultViewDTO.hasConfigurationError() && taskConsultViewDTO.Configuration != null && taskConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskConsultViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = taskConsultViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(taskConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
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

                upGrid.Update();
            }
        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            taskConsultViewDTO = iTasksMGR.GetTasksPendingReplenishment(context);

            if (!taskConsultViewDTO.hasError() && taskConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TasksPendingReplenishment.List, taskConsultViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(taskConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucError.ShowError(taskConsultViewDTO.Errors);
            }
        }

        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                taskConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksPendingReplenishment.List];
                var taskSelected = taskConsultViewDTO.Entities[index].Task;

                int idOutboundOrder = taskSelected.IdDocumentBound;
                var typeCode = taskSelected.TypeCode;

                if (typeCode == "PIKBT")
                {
                    taskDetailViewDTO = iTasksMGR.GetTasksDetailPendingReplenishmentByBatchOutboundOrder(idOutboundOrder, context);
                }
                else
                {
                    taskDetailViewDTO = iTasksMGR.GetTasksDetailPendingReplenishmentByOutboundOrder(idOutboundOrder, context);
                }

                if (taskDetailViewDTO != null && taskDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!taskDetailViewDTO.hasConfigurationError() && taskDetailViewDTO.Configuration != null && taskDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, taskDetailViewDTO.Configuration);

                    grdDetail.DataSource = taskDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    lblNroDoc.Text = taskConsultViewDTO.Entities[index].OutboundOrder.Number;

                    CallJsGridViewDetail();
                }
                else
                {
                    lblNroDoc.Text = String.Empty;
                }

                Session.Add(WMSTekSessions.TasksPendingReplenishment.Selected, taskConsultViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.TasksPendingReplenishment.Detail, taskDetailViewDTO);
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
                taskDetailViewDTO = (GenericViewDTO<TaskDetail>)Session[WMSTekSessions.TasksPendingReplenishment.Detail];

                // Configura ORDEN de las columnas de la grilla
                if (!taskDetailViewDTO.hasConfigurationError() && taskDetailViewDTO.Configuration != null && taskDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, taskDetailViewDTO.Configuration);

                // Detalle de Documentos de Entrada
                grdDetail.DataSource = taskDetailViewDTO.Entities;
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
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('TaskMgr_GetTasksPendingReplenishmentDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, true);", true);
        }

        #endregion
    }
}