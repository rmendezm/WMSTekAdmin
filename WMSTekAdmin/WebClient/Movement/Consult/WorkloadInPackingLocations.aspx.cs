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

namespace Binaria.WMSTek.WebClient.Movement.Consult
{
    public partial class WorkloadInPackingLocations : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskStatistics> taskStatisticsViewDTO = new GenericViewDTO<TaskStatistics>();
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
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
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
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
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
                }
            }
            catch (Exception ex)
            {
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
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
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
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
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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
                taskStatisticsViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
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

            if (Page.IsPostBack)
            {
                if (ValidateSession(WMSTekSessions.TaskStatistics.List))
                {
                    taskStatisticsViewDTO = (GenericViewDTO<TaskStatistics>)Session[WMSTekSessions.TaskStatistics.List];
                    isValidViewDTO = true;
                }
            }
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblFilterLocation.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.DispatchAdvanceDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.DispatchAdvanceDaysAfter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            //ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
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

            taskStatisticsViewDTO = iTasksMGR.FindAllTaskStatistics(context);

            if (!taskStatisticsViewDTO.hasError() && taskStatisticsViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TaskStatistics.List, taskStatisticsViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(taskStatisticsViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(taskStatisticsViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            taskStatisticsViewDTO = (GenericViewDTO<TaskStatistics>)Session[WMSTekSessions.TaskStatistics.List];

            if (taskStatisticsViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!taskStatisticsViewDTO.hasConfigurationError() && taskStatisticsViewDTO.Configuration != null && taskStatisticsViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskStatisticsViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = taskStatisticsViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(taskStatisticsViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        #endregion
    }
}