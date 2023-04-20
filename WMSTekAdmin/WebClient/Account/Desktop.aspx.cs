using System;
using System.Web;
using System.Web.UI;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dashboard;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Tasks;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class Desktop : BasePage
    {
        private GenericViewDTO<ProductivitySummary> toCollectViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> collectingViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> collectedViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<Monitor> monitorViewDTO = new GenericViewDTO<Monitor>();
        private GenericViewDTO<ProductivitySummary> taskPendingCycleCountViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskPendingAdjustViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskPendingReplenishViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskCompletedCycleCountViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskCompletedAdjustViewDTO = new GenericViewDTO<ProductivitySummary>();
        private GenericViewDTO<ProductivitySummary> taskCompletedReplenishViewDTO = new GenericViewDTO<ProductivitySummary>();

        private GenericViewDTO<KpiProductivity> taskKpiProductivityViewDTO = new GenericViewDTO<KpiProductivity>();
        private GenericViewDTO<KpiProductivityForZone> kpiProductivityForZoneViewDTO = new GenericViewDTO<KpiProductivityForZone>();
        private GenericViewDTO<KpiFillRate> kpiFillRateViewDTO = new GenericViewDTO<KpiFillRate>();
        private GenericViewDTO<KpiLeadTime> kpiLeadTimeViewDTO = new GenericViewDTO<KpiLeadTime>();
        private GenericViewDTO<KpiInfoDashboard> kpiInfoDashboardViewDTO = new GenericViewDTO<KpiInfoDashboard>();

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
        private int currentIndex
        {
            get
            {
                if (ValidateViewState("indexOrder"))
                    return (int)ViewState["indexOrder"];
                else
                    return -1;
            }

            set { ViewState["indexOrder"] = value; }
        }
        private string currentKpi
        {
            get
            {
                if (ValidateViewState("currentKpi"))
                    return (string)ViewState["currentKpi"];
                else
                    return string.Empty;
            }

            set { ViewState["currentKpi"] = value; }
        }

        protected override void Page_Init(object sender, EventArgs e)
        {
            //this.Master.ucDialog.BtnOkClick += new EventHandler(btnDialogOk_Click);
            base.Page_Init(sender, e);

            InitializeFilter(!Page.IsPostBack, false);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            //// 25-03-2015
            ////Linea Agregada para el funcionamiento en Distintos Navegadores
            if (Request.UserAgent.IndexOf("AppleWebKit") > 0)
            {
                Request.Browser.Adapters.Clear();
            }

            int idOwn = 0;
            int idWhs = 0;

            var ddlOwner = (DropDownList)this.Master.FindControl("ddlOwner");

            if (ddlOwner != null && !string.IsNullOrEmpty(ddlOwner.SelectedValue))
                idOwn = int.Parse(ddlOwner.SelectedValue);

            if (idOwn == 0)
                idOwn = context.SessionInfo.Owner.Id;

            var ddlWarehouse = (DropDownList)this.Master.FindControl("ddlWarehouse");

            if (ddlWarehouse != null && !string.IsNullOrEmpty(ddlWarehouse.SelectedValue))
                idWhs = int.Parse(ddlWarehouse.SelectedValue);

            if (idWhs == 0)
                idWhs = context.SessionInfo.Warehouse.Id;

            //base.LoadCustomer(this.ddlCustomerCharts5, idOwn, true, this.Master.AllRowsText);
           // LoadTypeCustomer(this.ddlLeadTimeCustomer, idOwn, true, this.Master.AllRowsText);
            base.LoadUsersByInRole(this.ddlUserCharts4, context.SessionInfo.IdRole, true, this.Master.AllRowsText);
            LoadUnidPicking();
            LoadZoneUnit();
            base.LoadWorkZoneByWhsAndTypeZone(this.ddlKpiZone, Convert.ToInt32(TypeWorkZone.Almacenamiento), idWhs, true, this.Master.AllRowsText);
        }

        /// <summary>
        /// Mantiene entre consultas los valores seleccionados del Filtro Avanzado
        /// </summary>
        protected void chkKeepFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKeepFilter.Checked)
                context.SessionInfo.FilterKeep = true;
            else
                context.SessionInfo.FilterKeep = false  ;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
        }

        private void LoadTypeCustomer(DropDownList objControl, int idOwner, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();

            if (ValidateSession(WMSTekSessions.Shared.CustomerList))
                customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];
            else
            {
                customerViewDTO = iWarehousingMGR.FindCustomerByOwner(context, idOwner);
                Session.Add(WMSTekSessions.Shared.CustomerList, customerViewDTO);
            }

            //Si el Campo SpecialField1 viene null lo reemplazamos por el texto 'Otros'
            for (int i = 0; i < customerViewDTO.Entities.Count(); i++)
            {
                customerViewDTO.Entities[i].SpecialField1 = (string.IsNullOrEmpty(customerViewDTO.Entities[i].SpecialField1) ? "Otros" : customerViewDTO.Entities[i].SpecialField1);
            }


            var varLst = customerViewDTO.Entities.Select(s => s.SpecialField1).Distinct();

            objControl.DataSource = varLst;
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        private void LoadUnidPicking()
        {
            ddlKpiPickingUnid.Items.Clear();
            this.ddlKpiPickingUnid.Items.Insert(0, new ListItem("Cantidad", "Cantidad"));
            this.ddlKpiPickingUnid.Items.Insert(1, new ListItem("Lineas", "Lineas"));
            this.ddlKpiPickingUnid.Items.Insert(2, new ListItem("LPN", "LPN"));

            this.ddlKpiPickingUnid.Items[0].Selected = true;
        }
        private void LoadZoneUnit()
        {
            ddlKpiZoneUnit.DataSource = Enum.GetNames(typeof(TypeUnitKPI));
            ddlKpiZoneUnit.DataBind();
            ddlKpiZoneUnit.SelectedValue = TypeUnitKPI.Volumen.ToString();
        }

        public void LoadKpiInfo()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            
            //context.MainFilter[6].FilterValues[0].Value = context.SessionInfo.Owner.Id.ToString();

            var filterOwner = context.MainFilter.Where(f => f.Name == "Owner").FirstOrDefault();
            filterOwner.FilterValues[0].Value = context.SessionInfo.Owner.Id.ToString();


            var filterWarehouse = context.MainFilter.Where(f => f.Name == "Warehouse").FirstOrDefault();
            filterWarehouse.FilterValues[0].Value = context.SessionInfo.Warehouse.Id.ToString();
            
            CreateDateRangeFilter();

            kpiInfoDashboardViewDTO = iDashboardMGR.GetKpiInfoDashboard(context);

            if (!kpiInfoDashboardViewDTO.hasError())
                CalculateTotals(kpiInfoDashboardViewDTO.Entities);
            else 
                Master.ucError.ShowError(kpiInfoDashboardViewDTO.Errors);
        }
        private void KpiVisibilityConfig()
        {
            var totalKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "TotalKpiVisibility").FirstOrDefault();

            if (totalKpiVisibility != null && totalKpiVisibility.ParameterValue == "0")
                divKpiTotal.Attributes.Add("class", "col-md-3 tile_stats_count hideElement");

            var dispatchKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "DispatchKpiVisibility").FirstOrDefault();

            if (dispatchKpiVisibility != null && dispatchKpiVisibility.ParameterValue == "0")
                divKpiDispatch.Attributes.Add("class", "col-md-3 tile_stats_count hideElement");

            var pendingKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "PendingKpiVisibility").FirstOrDefault();

            if (pendingKpiVisibility != null && pendingKpiVisibility.ParameterValue == "0")
                divKpiPending.Attributes.Add("class", "col-md-3 tile_stats_count hideElement");

            var releasedKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "ReleasedKpiVisibility").FirstOrDefault();

            if (releasedKpiVisibility != null && releasedKpiVisibility.ParameterValue == "0")
                divKpiReleased.Attributes.Add("class", "col-md-3 tile_stats_count hideElement");

            var pickingKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "PickingKpiVisibility").FirstOrDefault();

            if (pickingKpiVisibility != null && pickingKpiVisibility.ParameterValue == "0")
                divKpiPicking.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var wavePickingKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "WavePickingKpiVisibility").FirstOrDefault();

            if (wavePickingKpiVisibility != null && wavePickingKpiVisibility.ParameterValue == "0")
                divKpiWavePicking.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var packingKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "PackingKpiVisibility").FirstOrDefault();

            if (packingKpiVisibility != null && packingKpiVisibility.ParameterValue == "0")
                divKpiPacking.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var sortingKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "SortingKpiVisibility").FirstOrDefault();

            if (sortingKpiVisibility != null && sortingKpiVisibility.ParameterValue == "0")
                divKpiSorting.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var routedKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "RoutedKpiVisibility").FirstOrDefault();

            if (routedKpiVisibility != null && routedKpiVisibility.ParameterValue == "0")
                divKpiRouted.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var putKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "PutKpiVisibility").FirstOrDefault();

            if (putKpiVisibility != null && putKpiVisibility.ParameterValue == "0")
                divKpiPut.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var replanishmentKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "ReplanishmentKpiVisibility").FirstOrDefault();

            if (replanishmentKpiVisibility != null && replanishmentKpiVisibility.ParameterValue == "0")
                divKpiReplanishment.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var adjustKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "AdjustKpiVisibility").FirstOrDefault();

            if (adjustKpiVisibility != null && adjustKpiVisibility.ParameterValue == "0")
                divKpiAdjust.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");

            var lpnGuidedMovementKpiVisibility = context.CfgParameters.Where(p => p.ParameterCode == "LpnGuidedMovementKpiVisibility").FirstOrDefault();

            if (lpnGuidedMovementKpiVisibility != null && lpnGuidedMovementKpiVisibility.ParameterValue == "0")
                divKpiLpnGuidedMovement.Attributes.Add("class", "col-md-4 tile_stats_count hideElement");
        }
        private void CalculateTotals(List<KpiInfoDashboard> data)
        {
            if (data.Count == 0)
            {
                lblLastTimeUpdatedControlPanel.Text = string.Empty;
                divInfoKpiChartContent.Visible = false;
                udpInfoKpi.Update();
                this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = this.Master.EmptyGridText });
                return;
            }

            var totalOrders = CalculateTotalByType(KpiInfoDashboard.TOTAL, data);
            lblKpiTotal.Text = FormatNumber(totalOrders);

            CalculateTotalPending(data, totalOrders);
            CalculateTotalReleased(data);
            CalculateTotalDispatched(data, totalOrders);

            CalculateTotalPicking(data);
            CalculateTotalPickingWave(data);
            CalculateTotalPacking(data);
            CalculateTotalReplanishment(data);
            CalculateTotalPut(data);
            CalculateTotalSorting(data);
            CalculateTotalAdjust(data);
            CalculateGuidedMovement(data);
            CalculateRoute(data);

            divInfoKpiChartContent.Visible = true;
            lblLastTimeUpdatedControlPanel.Text = "Ultima vez actualizado: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        private void CalculateTotalPending(List<KpiInfoDashboard> data, double totalOrders)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.PENDING, data);
            var porcentage = CalculatePorcentage(total, totalOrders);

            lblKpiPending.Text = porcentage.ToString() + "%";
            lblKpiPending.ForeColor = ColorLabelPending(porcentage, total);
            lblKpiPendingDesc.Text = KpiMessage(total);
        }

        private void CalculateTotalReleased(List<KpiInfoDashboard> data)
        {
            var totalPending = CalculateTotalByType(KpiInfoDashboard.PENDING, data);
            var totalReleased = CalculateTotalByType(KpiInfoDashboard.RELEASE, data);
			var sumPendig = totalPending + totalReleased;
			
            var porcentage = CalculatePorcentage(totalReleased, sumPendig);

            lblKpiReleased.Text = porcentage.ToString() + "%";
            lblKpiReleased.ForeColor = ColorLabelCompleted(porcentage, totalReleased);
            lblKpiReleasedDesc.Text = KpiMessage(totalReleased);
        }

        private void CalculateTotalDispatched(List<KpiInfoDashboard> data, double totalOrders)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.DISPATCH, data);
            var porcentage = CalculatePorcentage(total, totalOrders);

            lblKpiDispatch.Text = porcentage.ToString() + "%";
            lblKpiDispatch.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiDispatchDesc.Text = KpiMessage(total);
        }

        private void CalculateTotalPicking(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.PICKING, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.PICKING, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.PICKING, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiPicking.Text = porcentage.ToString() + "%";
            lblKpiPicking.ToolTip = ToolTipPorcentage(true);
            lblKpiPicking.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiPickingDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateTotalPickingWave(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.PICKING_WAVE, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.PICKING_WAVE, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.PICKING_WAVE, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiPickingWave.Text = porcentage.ToString() + "%";
            lblKpiPickingWave.ToolTip = ToolTipPorcentage(true);
            lblKpiPickingWave.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiPickingWaveDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateTotalPacking(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.PACKING, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.PACKING, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.PACKING, false, data);
            var porcentage = CalculatePorcentage(completed, total);
           
            lblKpiPacking.Text = porcentage.ToString() + "%";
            lblKpiPacking.ToolTip = ToolTipPorcentage(true);
            lblKpiPacking.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiPackingDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateTotalReplanishment(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.REPLANISHMENT, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.REPLANISHMENT, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.REPLANISHMENT, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiReplanishment.Text = porcentage.ToString() + "%";
            lblKpiReplanishment.ToolTip = ToolTipPorcentage(true);
            lblKpiReplanishment.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiReplanishmentDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateTotalPut(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.PUT, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.PUT, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.PUT, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiPut.Text = porcentage.ToString() + "%";
            lblKpiPut.ToolTip = ToolTipPorcentage(true);
            lblKpiPut.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiPutDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateTotalSorting(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.SORTING, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.SORTING, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.SORTING, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiSorting.Text = porcentage.ToString() + "%";
            lblKpiSorting.ToolTip = ToolTipPorcentage(true);
            lblKpiSorting.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiSortingDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateTotalAdjust(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.ADJUST, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.ADJUST, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.ADJUST, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiAdjust.Text = porcentage.ToString() + "%";
            lblKpiAdjust.ToolTip = ToolTipPorcentage(true);
            lblKpiAdjust.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiAdjustDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateGuidedMovement(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.GUIDED_MOVEMENT, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.GUIDED_MOVEMENT, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.GUIDED_MOVEMENT, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiGuidedMovement.Text = porcentage.ToString() + "%";
            lblKpiGuidedMovement.ToolTip = ToolTipPorcentage(true);
            lblKpiGuidedMovement.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiGuidedMovementDesc.Text = KpiMessage(completed, incompleted);
        }

        private void CalculateRoute(List<KpiInfoDashboard> data)
        {
            var total = CalculateTotalByType(KpiInfoDashboard.ROUTE, data);
            var completed = CalculateCompleteTotalByType(KpiInfoDashboard.ROUTE, true, data);
            var incompleted = CalculateCompleteTotalByType(KpiInfoDashboard.ROUTE, false, data);
            var porcentage = CalculatePorcentage(completed, total);

            lblKpiRoute.Text = porcentage.ToString() + "%";
            lblKpiRoute.ToolTip = ToolTipPorcentage(true);
            lblKpiRoute.ForeColor = ColorLabelCompleted(porcentage, total);
            lblKpiRouteDesc.Text = KpiMessage(completed, incompleted);
        }

        private double CalculateTotalByType(string trackType, List<KpiInfoDashboard> data)
        {
            return data.Where(d => d.Type.ToLower() == trackType).Select(d => d.Total).Sum();
        }

        private double CalculateCompleteTotalByType(string trackType, bool isComplete, List<KpiInfoDashboard> data)
        {
            return data.Where(d => d.Type.ToLower() == trackType && d.IsComplete == isComplete).Select(d => d.Total).Sum();
        }

        private string KpiMessage(double total)
        {
            return FormatNumber(total) + " en total";
        }

        private string KpiMessage(double completed, double incompleted)
        {
            return FormatNumber(completed) + " completados /" + FormatNumber(incompleted) + " pendientes";
        }

        private string ToolTipPorcentage(bool isCompleted)
        {
            if (isCompleted)
                return "Completados";
            else
                return "Pendientes";
        }

        private Color ColorLabelCompleted(double porcentaje, double total)
        {
            var color = Color.Green;

            if (porcentaje < 25 && total > 0)
            {
                color = Color.Red;
            }

            return color;
        }

        private Color ColorLabelPending(double porcentaje, double total)
        {
            var color = Color.Green;

            if (porcentaje > 75 && total > 0)
            {
                color = Color.Red;
            }

            return color;
        }

        private double CalculatePorcentage(double part, double total)
        {
            double porcentage = 0;

            if (total != 0)
                porcentage = Math.Round((part / total) * 100, 2);

            return porcentage;
        }

        private void CreateDateRangeFilter()
        {
            string NAME_FILTER = "DateRange";

            if (context.MainFilter.Exists(filter => filter.Name == NAME_FILTER))
                context.MainFilter.Where(f => f.Name == NAME_FILTER).First().FilterValues = new List<FilterItem>();
 
            CreateDateFilter(txtStartDateFilterPanelControl.Text.Trim(), true, NAME_FILTER, context);
            CreateDateFilter(txtEndDateFilterPanelControl.Text.Trim(), false, NAME_FILTER, context);
        }

        private void CreateDateFilter(string date, bool isStartDate, string nameFilter, ContextViewDTO context)
        {
            var filterValue = string.Empty;

            if (!string.IsNullOrEmpty(date))
            {
                DateTime dateFilter = StringToDateTime(date);
                filterValue = isStartDate ? dateFilter.ToString("MM/dd/yyyy") : dateFilter.ToString("MM/dd/yyyy") + " 23:59:59";
            }

            var filterDateRange = context.MainFilter.Where(f => f.Name == nameFilter).FirstOrDefault();
            filterDateRange.FilterValues.Add(new FilterItem()
            {
                Name = nameFilter,
                Value = filterValue
            });
        }

        private DateTime StringToDateTime(string strDate)
        {
            DateTime finalDate = DateTime.MinValue;
            bool isValidDate = DateTime.TryParseExact(strDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out finalDate);

            if (isValidDate)
            {
                return finalDate;
            }
            else
            {
                throw new FormatException("Fecha Inválida");
            }
        }

        private string FormatNumber(double number)
        {
            return number.ToString("N0", CultureInfo.CreateSpecificCulture("es-CL"));
        }

        protected void btnUpdchartPanelControl_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiInfo();
                KpiVisibilityConfig();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void timerRefreshControlPanel_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadKpiInfo();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void chkEnableTimerControlPanel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEnableTimerControlPanel.Checked)
                {
                    LoadKpiInfo();
                    udpInfoKpi.Update();
                }

                timerRefreshControlPanel.Enabled = chkEnableTimerControlPanel.Checked;
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        #region "Orders PopUp"
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                kpiInfoDashboardViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kpiInfoDashboardViewDTO.Errors);
            }
        }
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }
        public void LoadKpiData(GenericViewDTO<OutboundOrder> outboundOrderViewDTO)
        {
            divKpiOrders.Visible = true;
            upKpiOrders.Update();
            mpKpiOrders.Show();

            currentIndex = -1;
            currentPage = 0;

            grdMgr.PageIndex = currentPage;

            if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.OutboundOrderKpi.List, outboundOrderViewDTO);
                PopulateGrid();
            }
            else
            {
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        private void PopulateGrid()
        {
            InitializeGrid();

            int pageNumber;

            grdMgr.EmptyDataText = this.Master.EmptyGridText;

            var outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderKpi.List];

            if (outboundOrderViewDTO == null)
                return;

            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();
            grdMgr.SelectedIndex = currentIndex;

            CallJsGridViewHeader();

            lblKpiOrdersCount.Text = this.Master.TotalText + outboundOrderViewDTO.Entities.Count.ToString();

            if (grdMgr.PageCount > 1)
            {
                divPageGrdMgr.Visible = true;
                ddlPages.Items.Clear();
                for (int i = 0; i < grdMgr.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPage) lstItem.Selected = true;

                    ddlPages.Items.Add(lstItem);
                }
                this.lblPageCount.Text = grdMgr.PageCount.ToString();
            }
            else
            {
                divPageGrdMgr.Visible = false;
            }
        }
        protected void btnFirstgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            ddlPages.SelectedIndex = 0;
            ddlPagesSelectedIndexChanged(sender, e);
            mpKpiOrders.Show();

            btnFirstgrdMgr.Enabled = false;
            btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
            btnPrevgrdMgr.Enabled = false;
            btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";

            btnNextgrdMgr.Enabled = true;
            btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
            btnLastgrdMgr.Enabled = true;
            btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
        }
        protected void btnPrevgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            var outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderKpi.List];

            if (outboundOrderViewDTO == null)
                return;

            if (currentPage > 0)
            {
                currentPage = currentPage - 1;
                grdMgr.PageIndex = currentPage;

                PopulateGrid();

                if (currentPage > 0)
                {
                    btnNextgrdMgr.Enabled = true;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgr.Enabled = true;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnPrevgrdMgr.Enabled = false;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstgrdMgr.Enabled = false;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";

                    btnNextgrdMgr.Enabled = true;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgr.Enabled = true;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
            }
            else
            {
                btnPrevgrdMgr.Enabled = false;
                btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                btnFirstgrdMgr.Enabled = false;
                btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
            }

            mpKpiOrders.Show();
        }
        protected void ddlPagesSelectedIndexChanged(object sender, EventArgs e)
        {
            var outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderKpi.List];

            if (outboundOrderViewDTO == null)
                return;

            currentPage = ddlPages.SelectedIndex;
            grdMgr.PageIndex = currentPage;

            PopulateGrid();

            if (currentPage == grdMgr.PageCount)
            {
                btnNextgrdMgr.Enabled = false;
                btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastgrdMgr.Enabled = false;
                btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
            }
            else
            {
                if (currentPage == 0)
                {
                    btnPrevgrdMgr.Enabled = false;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstgrdMgr.Enabled = false;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                }
                else
                {
                    btnNextgrdMgr.Enabled = true;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgr.Enabled = true;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevgrdMgr.Enabled = true;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgr.Enabled = true;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }

            mpKpiOrders.Show();
        }
        protected void btnNextgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            var outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderKpi.List];

            if (outboundOrderViewDTO == null)
                return;

            if (currentPage < grdMgr.PageCount - 1)
            {
                currentPage = currentPage + 1;
                grdMgr.PageIndex = currentPage;

                PopulateGrid();

                if (currentPage < grdMgr.PageCount - 1)
                {
                    btnPrevgrdMgr.Enabled = true;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgr.Enabled = true;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
                else
                {
                    btnNextgrdMgr.Enabled = false;
                    btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                    btnLastgrdMgr.Enabled = false;
                    btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";

                    btnPrevgrdMgr.Enabled = true;
                    btnPrevgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgr.Enabled = true;
                    btnFirstgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }

            }
            else
            {
                btnNextgrdMgr.Enabled = false;
                btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastgrdMgr.Enabled = false;
                btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
            }

            mpKpiOrders.Show();
        }
        protected void btnLastgrdMgr_Click(object sender, ImageClickEventArgs e)
        {
            ddlPages.SelectedIndex = grdMgr.PageCount - 1;
            ddlPagesSelectedIndexChanged(sender, e);
            mpKpiOrders.Show();

            btnNextgrdMgr.Enabled = false;
            btnNextgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
            btnLastgrdMgr.Enabled = false;
            btnLastgrdMgr.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
        }
        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('OutboundOrder_FindAll', 'ctl00_MainContent_grdMgr', 'OutboundOrderMgr');", true);
        }
        #endregion

        #region "Tasks PopUp"
        protected void grdMgrTasks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void grdMgrTasks_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                kpiInfoDashboardViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(kpiInfoDashboardViewDTO.Errors);
            }
        }
        private void InitializeGridTasks()
        {
            grdMgrTasks.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgrTasks.EmptyDataText = this.Master.EmptyGridText;
        }
        public void LoadKpiTasksData(GenericViewDTO<TaskConsult> tasksViewDTO)
        {
            divKpiTasks.Visible = true;
            upKpiTasks.Update();
            mpKpiTasks.Show();

            currentIndex = -1;
            currentPage = 0;

            grdMgrTasks.PageIndex = currentPage;

            if (!tasksViewDTO.hasError() && tasksViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TasksKpi.List, tasksViewDTO);
                PopulateGridTasks();
            }
            else
            {
                this.Master.ucError.ShowError(tasksViewDTO.Errors);
            }
        }
        private void PopulateGridTasks()
        {
            InitializeGridTasks();

            int pageNumber;

            grdMgrTasks.EmptyDataText = this.Master.EmptyGridText;

            var tasksViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksKpi.List];

            if (tasksViewDTO == null)
                return;

            grdMgrTasks.DataSource = tasksViewDTO.Entities;
            grdMgrTasks.DataBind();
            grdMgrTasks.SelectedIndex = currentIndex;

            CallJsGridViewHeaderTasks();

            lblKpiTasksCount.Text = this.Master.TotalText + tasksViewDTO.Entities.Count.ToString();

            if (grdMgrTasks.PageCount > 1)
            {
                divPageGrdMgrTasks.Visible = true;
                ddlPagesTasks.Items.Clear();
                for (int i = 0; i < grdMgrTasks.PageCount; i++)
                {
                    pageNumber = i + 1;
                    ListItem lstItem = new ListItem(pageNumber.ToString());

                    if (i == currentPage) lstItem.Selected = true;

                    ddlPagesTasks.Items.Add(lstItem);
                }
                this.lblPageTasksCount.Text = grdMgrTasks.PageCount.ToString();
            }
            else
            {
                divPageGrdMgrTasks.Visible = false;
            }
        }
        protected void btnFirstgrdMgrTasks_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesTasks.SelectedIndex = 0;
            ddlPagesTasksSelectedIndexChanged(sender, e);
            mpKpiTasks.Show();

            btnFirstgrdMgrTasks.Enabled = false;
            btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
            btnPrevgrdMgrTasks.Enabled = false;
            btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";

            btnNextgrdMgrTasks.Enabled = true;
            btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
            btnLastgrdMgrTasks.Enabled = true;
            btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
        }
        protected void btnPrevgrdMgrTasks_Click(object sender, ImageClickEventArgs e)
        {
            var tasksViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksKpi.List];

            if (tasksViewDTO == null)
                return;

            if (currentPage > 0)
            {
                currentPage = currentPage - 1;
                grdMgrTasks.PageIndex = currentPage;

                PopulateGridTasks();

                if (currentPage > 0)
                {
                    btnNextgrdMgrTasks.Enabled = true;
                    btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgrTasks.Enabled = true;
                    btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
                else
                {
                    btnPrevgrdMgrTasks.Enabled = false;
                    btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstgrdMgrTasks.Enabled = false;
                    btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";

                    //ACA
                    btnNextgrdMgrTasks.Enabled = true;
                    btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgrTasks.Enabled = true;
                    btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                }
            }
            else
            {
                btnPrevgrdMgrTasks.Enabled = false;
                btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                btnFirstgrdMgrTasks.Enabled = false;
                btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
            }

            mpKpiTasks.Show();
        }
        protected void ddlPagesTasksSelectedIndexChanged(object sender, EventArgs e)
        {
            var tasksViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksKpi.List];

            if (tasksViewDTO == null)
                return;

            currentPage = ddlPagesTasks.SelectedIndex;
            grdMgrTasks.PageIndex = currentPage;

            PopulateGridTasks();

            if (currentPage == grdMgrTasks.PageCount)
            {
                btnNextgrdMgrTasks.Enabled = false;
                btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastgrdMgrTasks.Enabled = false;
                btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
            }
            else
            {
                if (currentPage == 0)
                {
                    btnPrevgrdMgrTasks.Enabled = false;
                    btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous_dis.png";
                    btnFirstgrdMgrTasks.Enabled = false;
                    btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first_dis.png";
                }
                else
                {
                    btnNextgrdMgrTasks.Enabled = true;
                    btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next.png";
                    btnLastgrdMgrTasks.Enabled = true;
                    btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last.png";
                    btnPrevgrdMgrTasks.Enabled = true;
                    btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgrTasks.Enabled = true;
                    btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }

            mpKpiTasks.Show();
        }
        protected void btnNextgrdMgrTasks_Click(object sender, ImageClickEventArgs e)
        {
            var tasksViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksKpi.List];

            if (tasksViewDTO == null)
                return;

            if (currentPage < grdMgrTasks.PageCount - 1)
            {
                currentPage = currentPage + 1;
                grdMgrTasks.PageIndex = currentPage;

                PopulateGridTasks();

                if (currentPage < grdMgrTasks.PageCount - 1)
                {
                    btnPrevgrdMgrTasks.Enabled = true;
                    btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgrTasks.Enabled = true;
                    btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
                else
                {
                    btnNextgrdMgrTasks.Enabled = false;
                    btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                    btnLastgrdMgrTasks.Enabled = false;
                    btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";

                    //ACA
                    btnPrevgrdMgrTasks.Enabled = true;
                    btnPrevgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_previous.png";
                    btnFirstgrdMgrTasks.Enabled = true;
                    btnFirstgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_first.png";
                }
            }
            else
            {
                btnNextgrdMgrTasks.Enabled = false;
                btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
                btnLastgrdMgrTasks.Enabled = false;
                btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
            }

            mpKpiTasks.Show();
        }
        protected void btnLastgrdMgrTasks_Click(object sender, ImageClickEventArgs e)
        {
            ddlPagesTasks.SelectedIndex = grdMgrTasks.PageCount - 1;
            ddlPagesTasksSelectedIndexChanged(sender, e);
            mpKpiTasks.Show();

            btnNextgrdMgrTasks.Enabled = false;
            btnNextgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_next_dis.png";
            btnLastgrdMgrTasks.Enabled = false;
            btnLastgrdMgrTasks.ImageUrl = "~/WebResources/Images/Buttons/Pager/icon_last_dis.png";
        }
        private void CallJsGridViewHeaderTasks()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeaderTasks", "initializeGridDragAndDrop('TaskMgr_FindAll', 'ctl00_MainContent_grdMgrTasks');", true);
        }
        #endregion

        #region "Util KPI"
        private void CreateDateRangeFilterKpi(ContextViewDTO context)
        {
            if (!string.IsNullOrEmpty(txtStartDateFilterPanelControl.Text) || !string.IsNullOrEmpty(txtEndDateFilterPanelControl.Text))
            {
                string NAME_FILTER = "DateRange";

                if (context.MainFilter.Exists(filter => filter.Name == NAME_FILTER))
                    context.MainFilter.Where(f => f.Name == NAME_FILTER).First().FilterValues = new List<FilterItem>();

                CreateDateFilter(txtStartDateFilterPanelControl.Text.Trim(), true, NAME_FILTER, context);
                CreateDateFilter(txtEndDateFilterPanelControl.Text.Trim(), false, NAME_FILTER, context);
            }
        }
        private int GetOwner()
        {
            int idOwn = 0;

            var ddlOwner = (DropDownList)this.Master.FindControl("ddlOwner");

            if (ddlOwner != null && !string.IsNullOrEmpty(ddlOwner.SelectedValue))
                idOwn = int.Parse(ddlOwner.SelectedValue);

            if (idOwn == 0)
                idOwn = context.SessionInfo.Owner.Id;

            return idOwn;
        }
        private int GetWarehouse()
        {
            int idWhs = 0;

            var ddlWarehouse = (DropDownList)this.Master.FindControl("ddlWarehouse");

            if (ddlWarehouse != null && !string.IsNullOrEmpty(ddlWarehouse.SelectedValue))
                idWhs = int.Parse(ddlWarehouse.SelectedValue);

            if (idWhs == 0)
                idWhs = context.SessionInfo.Warehouse.Id;

            return idWhs;
        }
        private GenericViewDTO<OutboundOrder> OnlyOrdersData(int idTrackOutboundType = 0, string distinctOutType = null)
        {
            var newContext = NewContext();

            if (idTrackOutboundType > 0)
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.TrackOutboundType)].FilterValues.Add(new FilterItem(idTrackOutboundType.ToString()));

            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(GetOwner().ToString()));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(GetWarehouse().ToString()));

            CreateDateRangeFilterKpi(newContext);

            var outboundOrderViewDTO = iDispatchingMGR.FindAllOutboundOrder(newContext);

            if (outboundOrderViewDTO.Entities.Count > 0 && !string.IsNullOrEmpty(distinctOutType))
                outboundOrderViewDTO.Entities.RemoveAll(o => o.OutboundType.Code == distinctOutType);

            return outboundOrderViewDTO;
        }
        private GenericViewDTO<OutboundOrder> OrdersJoinTaskData(string taskTypeCodeList, int idTrackOutboundType = 0)
        {
            var newContext = NewContext();

            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(GetOwner().ToString()));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(GetWarehouse().ToString()));

            if (idTrackOutboundType > 0)
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.TrackOutboundType)].FilterValues.Add(new FilterItem(idTrackOutboundType.ToString()));

            CreateDateRangeFilterKpi(newContext);

            var outboundOrderViewDTO = iDispatchingMGR.FindAllOutboundOrderJoinTask(taskTypeCodeList, newContext);

            return outboundOrderViewDTO;
        }
        private GenericViewDTO<TaskConsult> TasksData(List<string> taskTypes)
        {
            var newContext = NewContext();

            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(GetOwner().ToString()));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(GetWarehouse().ToString()));

            var taskTypeFilter = newContext.MainFilter[Convert.ToInt16(EntityFilterName.TaskType)];

            foreach (var taskType in taskTypes)
            {
                taskTypeFilter.AddFilterItem(new FilterItem(taskType));
            }

            CreateDateRangeFilterKpi(newContext);

            var tasksViewDTO = iTasksMGR.FindAllTaskMgrKpi(newContext);

            return tasksViewDTO;
        }
        #endregion

        #region "Orders KPI events"
        protected void btnTotalData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiData(OnlyOrdersData(0, "PIKWV"));
                currentKpi = "KPI-Total";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnDispatchData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Shipped;
                LoadKpiData(OnlyOrdersData(idTrackOutboundType));
                currentKpi = "KPI-Despachos";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnPendingData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Pending;
                LoadKpiData(OnlyOrdersData(idTrackOutboundType));
                currentKpi = "KPI-Pendientes";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnReleasedData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Released;
                LoadKpiData(OnlyOrdersData(idTrackOutboundType));
                currentKpi = "KPI-Liberados";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnPickingData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Picking;
                LoadKpiData(OrdersJoinTaskData("PIKOR,PIKBT,PIKPS,PIKIT,PIKVA,PKLPN", idTrackOutboundType));
                currentKpi = "KPI-Picking";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnPickingWaveData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Picking;
                LoadKpiData(OrdersJoinTaskData("PIKWV", idTrackOutboundType));
                currentKpi = "KPI-PickingOla";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnPackingData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Packing;
                LoadKpiData(OrdersJoinTaskData("PAKOR", idTrackOutboundType));
                currentKpi = "KPI-Packing";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnSortingData_Click(object sender, EventArgs e)
        {
            try
            {
                int idTrackOutboundType = (int)TrackOutboundTypeName.Sorting;
                LoadKpiData(OrdersJoinTaskData("SORT", idTrackOutboundType));
                currentKpi = "KPI-Distribucion";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        #endregion

        #region "Task KPI events"
        protected void btnRoutedData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiData(OrdersJoinTaskData("RUTEO"));
                currentKpi = "KPI-Ruteo";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnPutData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiTasksData(TasksData(new List<string>() { "PUT" }));
                currentKpi = "KPI-Almacenamiento";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnReplanishmentData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiTasksData(TasksData(new List<string>() { "REPL" }));
                currentKpi = "KPI-Reposicion";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnAdjustData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiTasksData(TasksData(new List<string>() { "CCNT", "CCLOC", "AJU" }));
                currentKpi = "KPI-Ajustes";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnGuidedMovementData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadKpiTasksData(TasksData(new List<string>() { "MDLPN" }));
                currentKpi = "KPI-MovimientoDirigidoLPN";
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        #endregion

        #region "Excel"
        protected void btnExcelGridOrders_Click(object sender, EventArgs e)
        {
            try
            {
                var outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.OutboundOrderKpi.List];

                if (outboundOrderViewDTO == null)
                    return;

                grdMgr.AllowPaging = false;

                grdMgr.DataSource = outboundOrderViewDTO.Entities;
                grdMgr.DataBind();

                currentPageTitle = currentKpi;
                base.ExportToExcel(grdMgr, null, null);

                grdMgr.AllowPaging = true;

                mpKpiOrders.Show();
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void btnExcelGridTasks_Click(object sender, EventArgs e)
        {
            try
            {
                var tasksViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TasksKpi.List];

                if (tasksViewDTO == null)
                    return;

                grdMgrTasks.AllowPaging = false;

                grdMgrTasks.DataSource = tasksViewDTO.Entities;
                grdMgrTasks.DataBind();

                base.ExportToExcel(grdMgrTasks, null, null);

                grdMgrTasks.AllowPaging = true;

                mpKpiTasks.Show();
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        #endregion

        protected void btnPollo_Click(object sender, EventArgs e)
        {
            string u = "";
        }
    }
}
