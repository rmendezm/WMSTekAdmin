using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dashboard;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using Binaria.WMSTek.WebClient.Base;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class OutboundDashboard : BasePage
    {
        private GenericViewDTO<KpiProductivity> taskKpiProductivityViewDTO = new GenericViewDTO<KpiProductivity>();
        private GenericViewDTO<KpiProductivityForZone> kpiProductivityForZoneViewDTO = new GenericViewDTO<KpiProductivityForZone>();
        private GenericViewDTO<KpiFillRate> kpiFillRateViewDTO = new GenericViewDTO<KpiFillRate>();
        private GenericViewDTO<KpiLeadTime> kpiLeadTimeViewDTO = new GenericViewDTO<KpiLeadTime>();
        private GenericViewDTO<KpiInfoDashboard> kpiInfoDashboardViewDTO = new GenericViewDTO<KpiInfoDashboard>();

        private int idWhs { get { return this.Master.ucMainFilter.idWhs; } }
        private int idOwn { get { return this.Master.ucMainFilter.idOwn; } }

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
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                //if (base.webMode == WebMode.Normal)
                //{
                //    LoadPopulateList();
                //    LoadCharts();
                //}

                //Modifica el Ancho del Div de los Graficos
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivCharts();", true);
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(btnKpiExportToExcel);
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }


        public void Initialize()
        {
            InitializeFilter(!Page.IsPostBack, false);

            if (!Page.IsPostBack)
            {
                //hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
                this.hsMasterDetail.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .22);
                this.HorizontalSplitter1.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .32);

                this.Splitter1.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .36);
                this.Splitter2.LeftPanel.WidthDefault = Convert.ToInt16(Convert.ToInt16(Session["screenX"]) * .36);

                context.MainFilter = this.Master.ucMainFilter.MainFilter;

                LoadPopulateList();
                LoadCharts();
            }

        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(BtnSearchClick);
        }

        protected void BtnSearchClick(object sender, EventArgs e)
        {
            try
            {
                this.ddlUserCharts4.Items.Clear();
                base.LoadWorkZoneByWhsAndTypeZone(this.ddlKpiZone, Convert.ToInt32(TypeWorkZone.Almacenamiento), idWhs, true, this.Master.AllRowsText);
                base.LoadUsersByInRole(this.ddlUserCharts4, context.SessionInfo.IdRole, true, this.Master.AllRowsText);
                base.LoadCustomer(this.ddlCustomerCharts5, idOwn, true, this.Master.AllRowsText);
                LoadTypeCustomer(this.ddlLeadTimeCustomer, idOwn, true, this.Master.AllRowsText);

                LoadCharts();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }



        private void LoadPopulateList()
        {
            ddlTypeLeadTime.DataSource = Enum.GetNames(typeof(SeriesChartType));
            ddlTypeLeadTime.DataBind();
            ddlTypeFillRate.DataSource = Enum.GetNames(typeof(SeriesChartType));
            ddlTypeFillRate.DataBind();
            ddlTypeKpiPicking.DataSource = Enum.GetNames(typeof(SeriesChartType));
            ddlTypeKpiPicking.DataBind();
            ddlTypeKpiZone.DataSource = Enum.GetNames(typeof(SeriesChartType));
            ddlTypeKpiZone.DataBind();
            ddlKpiZoneUnit.DataSource = Enum.GetNames(typeof(TypeUnitKPI));
            ddlKpiZoneUnit.DataBind();

            //Selecciona el tipo de grafico Column por defecto
            ddlTypeLeadTime.SelectedValue = SeriesChartType.Column.ToString();
            ddlTypeFillRate.SelectedValue = SeriesChartType.Column.ToString();
            ddlTypeKpiPicking.SelectedValue = SeriesChartType.Column.ToString();
            ddlTypeKpiZone.SelectedValue = SeriesChartType.Column.ToString();
            ddlKpiZoneUnit.SelectedValue = TypeUnitKPI.Volumen.ToString();

            base.LoadWorkZoneByWhsAndTypeZone(this.ddlKpiZone,Convert.ToInt32(TypeWorkZone.Almacenamiento), idWhs, true, this.Master.AllRowsText);
            //base.LoadUsersByInRole(this.ddlUserCharts4, context.SessionInfo.IdRole, true, this.Master.AllRowsText);
            base.LoadUsersByNotInRole(this.ddlUserCharts4, context.SessionInfo.IdRole, true, this.Master.AllRowsText);
            base.LoadCustomer(this.ddlCustomerCharts5, idOwn, true, this.Master.AllRowsText);

            LoadTypeCustomer(this.ddlLeadTimeCustomer, idOwn, true, this.Master.AllRowsText);
            LoadUnidPicking();
        }

        private void LoadCharts()
        {
            // Valida variable de sesion del Usuario Loggeado
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                LoadDataChart_KpiZone();
                LoadChart_KpiPicking();
                LoadChart_KpiFillRate();
                LoadChart_KpiLeadTime();
                LoadKpiInfo();
            }
        }

        #region INICIO GRAFICOS

        protected void TimerKpiLeadTime_Tick(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadChart_KpiLeadTime();
                }
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void TimerKpiZona_Tick(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadDataChart_KpiZone();
                }
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void TimerKpiPicking_Tick(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadChart_KpiPicking();
                }
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void TimerKpiFillRate_Tick(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadChart_KpiFillRate();
                }
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void TimerInfoKpi_Tick(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadKpiInfo();
                }

            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void chkAutoRefreshIndoKpi_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    if (chkAutoRefreshIndoKpi.Checked)
                    {
                        TimerInfoKpi.Enabled = true;
                    }
                    else
                    {
                        TimerInfoKpi.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void btnRefreshInfoKpi_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    LoadKpiInfo();
                }
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }

        }

        public void LoadChart_KpiZone(List<KpiProductivityForZone> lstForZone)
        {

            Random random = new Random();

            //Informacion del Grafico
            this.ChartKpiZona.ChartAreas[0].AxisX.Title = "Zona";
            this.ChartKpiZona.ChartAreas[0].AxisY.Title = "Porcentaje";

            //Angulo en el que se mostrara el contenido del Eje X
            this.ChartKpiZona.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            this.ChartKpiZona.Series["Series1"].YValueType = ChartValueType.Double;


            foreach (var item in lstForZone)
            {
                int currentIndex = this.ChartKpiZona.Series["Series1"].Points.AddXY(item.WorkZoneName, item.PercentageZone);
                this.ChartKpiZona.Series["Series1"].Points[currentIndex].ToolTip = item.WorkZoneName.Trim() + " / " + item.PercentageZone.ToString() + "%";
                this.ChartKpiZona.Series["Series1"].Points[currentIndex].PostBackValue = item.IdWorkZone.ToString();
                this.ChartKpiZona.Series["Series1"].Points[currentIndex].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            }
            this.ChartKpiZona.DataBind();

            string typeChart = ddlTypeKpiZone.SelectedValue;
            SeriesChartType type = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), typeChart);

            this.ChartKpiZona.Series["Series1"].ChartType = type;
            this.ChartKpiZona.Series["Series1"].CustomProperties = "MinPixelPointWidth=15, DrawSideBySide=False, DrawingStyle=Cylinder";

        }

        private void LoadDataChart_KpiZone()
        {
            string typeCharts = ddlKpiZoneUnit.SelectedValue;
            TypeUnitKPI typeUnit = (TypeUnitKPI)Enum.Parse(typeof(TypeUnitKPI), typeCharts);

            if (ddlKpiZone.SelectedValue.Trim() == "-1")
            {
                if (ValidateSession(WMSTekSessions.Global.MainFilter))
                {
                    context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
                }
                else
                {
                    context.MainFilter = new List<EntityFilter>();
                }
                context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].FilterValues.Clear();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.Global.MainFilter))
                {
                    context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
                }
                else
                {
                    context.MainFilter = new List<EntityFilter>();
                }
                context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].AddFilterItem(new FilterItem("", ddlKpiZone.SelectedValue.Trim()));
            }

            switch (typeUnit)
            {
                case TypeUnitKPI.Cantidad:
                    kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneQty(idWhs, context);
                    break;
                case TypeUnitKPI.Peso:
                    kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneWeight(idWhs, context);
                    break;
                case TypeUnitKPI.Volumen:
                    kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneVolumen(idWhs, context);
                    break;
            }

            //Agrupa los valores de la zonas encontradas
            IEnumerable<KpiProductivityForZone> lstForZone = from i in kpiProductivityForZoneViewDTO.Entities
                                                             group i by i.WorkZoneName into g
                                                             select new KpiProductivityForZone()
                                                             {
                                                                 PercentageZone = decimal.Round((g.Sum(a => a.PercentageZone) / g.Count()), 0),
                                                                 WorkZoneName = g.First().WorkZoneName,
                                                                 IdWorkZone = g.First().IdWorkZone
                                                             };

            LoadChart_KpiZone(lstForZone.ToList());
            Session.Add(WMSTekSessions.Shared.ListGridViewKPI, lstForZone.ToList());
            //LoadChart_KpiZone(lstForZone.ToList<KpiProductivityForZone>());

        }

        public void LoadChart_KpiPicking()
        {
            string user = this.ddlUserCharts4.SelectedValue.Trim();
            string typeUnid = this.ddlKpiPickingUnid.SelectedValue.Trim();
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
            Random random = new Random();

            if (user == "-1")
            {
                if (ValidateSession(WMSTekSessions.Global.MainFilter))
                {
                    context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
                }
                else
                {
                    context.MainFilter = new List<EntityFilter>();
                }

                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.Global.MainFilter))
                {
                    context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
                }
                else
                {
                    context.MainFilter = new List<EntityFilter>();
                }
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].AddFilterItem(new FilterItem("", user));
            }

            taskKpiProductivityViewDTO = iDashboardMGR.GetKpiProductivityByUserWhs(idWhs, context);


            if (typeUnid.ToUpper() == "CANTIDAD")
            {
                //Suma las Cantidades
                var lst = from i in taskKpiProductivityViewDTO.Entities.ToList()
                          group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                          select new
                          {
                              Qty = g.Sum(a => a.Qty),
                              UserWms = g.First().UserWms,
                              Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                              Year = g.First().DateCreated.Year.ToString()
                          };

                foreach (var item in lst)
                {
                    int currentIndex = this.ChartKpiPicking.Series["Series1"].Points.AddXY(item.Month, item.Qty);
                    this.ChartKpiPicking.Series["Series1"].Points[currentIndex].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    this.ChartKpiPicking.Series["Series1"].Points[currentIndex].ToolTip = item.Month + "-" + item.Year + " / " + item.Qty.ToString();
                }
                this.ChartKpiPicking.DataBind();
            }


            if (typeUnid.ToUpper() == "LINEAS")
            {
                //Cuenta la Cantidad de Lineas
                var lstLine = from i in taskKpiProductivityViewDTO.Entities.ToList()
                              group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                              select new
                              {
                                  Lines = g.Count(),
                                  UserWms = g.First().UserWms,
                                  Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                                  Year = g.First().DateCreated.Year
                              };

                foreach (var item in lstLine)
                {
                    int currentIndex = this.ChartKpiPicking.Series["Series1"].Points.AddXY(item.Month, item.Lines);
                    this.ChartKpiPicking.Series["Series1"].Points[currentIndex].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    this.ChartKpiPicking.Series["Series1"].Points[currentIndex].ToolTip = item.Month + "-" + item.Year + " / " + item.Lines.ToString();
                }
                this.ChartKpiPicking.DataBind();
            }


            if (typeUnid.ToUpper() == "LPN")
            {
                //Cuenta la Cantidad de LPNS
                var lstLine = from i in taskKpiProductivityViewDTO.Entities.ToList()
                              group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                              select new
                              {
                                  ContLPNs = g.Select(l => l.IdLpnCode).Distinct().Count(),
                                  UserWms = g.First().UserWms,
                                  Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                                  Year = g.First().DateCreated.Year
                              };

                foreach (var item in lstLine)
                {
                    int currentIndex = this.ChartKpiPicking.Series["Series1"].Points.AddXY(item.Month, item.ContLPNs);
                    this.ChartKpiPicking.Series["Series1"].Points[currentIndex].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    this.ChartKpiPicking.Series["Series1"].Points[currentIndex].ToolTip = item.Month + "-" + item.Year + " / " + item.ContLPNs.ToString();
                }
                this.ChartKpiPicking.DataBind();
            }

            //Informacion del Grafico
            this.ChartKpiPicking.ChartAreas[0].AxisX.Title = "Fecha Creación";
            this.ChartKpiPicking.ChartAreas[0].AxisY.Title = typeUnid;

            //Angulo en el que se mostrara el contenido del Eje X
            this.ChartKpiPicking.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            this.ChartKpiPicking.Series["Series1"].YValueType = ChartValueType.Double;

            string typeChart = ddlTypeKpiPicking.SelectedValue;
            SeriesChartType type = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), typeChart);

            //Tipo de Grafico
            this.ChartKpiPicking.Series["Series1"].ChartType = type;
            this.ChartKpiPicking.Series["Series1"].CustomProperties = "MinPixelPointWidth=15, DrawSideBySide=False, DrawingStyle=Cylinder";
        }

        public void LoadChart_KpiFillRate()
        {
            string customer = this.ddlCustomerCharts5.SelectedValue;

            if (ValidateSession(WMSTekSessions.Global.MainFilter))
            {
                context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
            }
            else if (context.MainFilter == null)
            {
                context.MainFilter = new List<EntityFilter>();
                var arrEnum = Enum.GetValues(typeof(EntityFilterName));

                foreach (var item in arrEnum)
                {
                    context.MainFilter.Add(new EntityFilter(item.ToString(), new FilterItem()));
                }
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();

            if (customer != "-1")
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].AddFilterItem(new FilterItem("", customer));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].AddFilterItem(new FilterItem("", idOwn.ToString()));

            kpiFillRateViewDTO = iDashboardMGR.GetKpiFillRate(idWhs, context);


            //Agrupa los valores del FillRate 
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
            var lst = from i in kpiFillRateViewDTO.Entities.ToList()
                      group i by new { i.DateCreated.Month, i.DateCreated.Year } into g
                      select new
                      {
                          PercentSatisfaction = decimal.Round((g.Sum(i => i.PercentSatisfaction) / g.Count()), 0),
                          DateCreated = formatoFecha.GetMonthName(g.First().DateCreated.Month)
                      };

            //Informacion del Grafico
            this.ChartKpiFillRate.ChartAreas[0].AxisX.Title = "Fecha Creación";
            this.ChartKpiFillRate.ChartAreas[0].AxisY.Title = "Nivel Satisfacción %";

            //Angulo en el que se mostrara el contenido del Eje X
            this.ChartKpiFillRate.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

            this.ChartKpiFillRate.Series["Series1"].YValueType = ChartValueType.Double;
            this.ChartKpiFillRate.ChartAreas[0].AxisY.Interval = 10;
            this.ChartKpiFillRate.ChartAreas[0].AxisY.Maximum = 100;

            Random random = new Random();
            foreach (var item in lst)
            {
                int currentIndex = this.ChartKpiFillRate.Series["Series1"].Points.AddXY(item.DateCreated, item.PercentSatisfaction);
                this.ChartKpiFillRate.Series["Series1"].Points[currentIndex].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                this.ChartKpiFillRate.Series["Series1"].Points[currentIndex].ToolTip = item.DateCreated + " / " + item.PercentSatisfaction.ToString() + "%";
            }
            this.ChartKpiFillRate.DataBind();


            string typeChart = ddlTypeFillRate.SelectedValue;
            SeriesChartType type = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), typeChart);
            //Tipo de Grafico
            this.ChartKpiFillRate.Series["Series1"].ChartType = type;
            this.ChartKpiFillRate.Series["Series1"].CustomProperties = "MinPixelPointWidth=5, DrawSideBySide=False, DrawingStyle=Cylinder";
        }

        public void LoadChart_KpiLeadTime()
        {
            string typoLead = this.ddlLeadTime.SelectedValue;
            string typeCustomer = (this.ddlLeadTimeCustomer.SelectedValue == "Otros" ? null : this.ddlLeadTimeCustomer.SelectedValue);
            string typeTitulo = string.Empty;

            if (ValidateSession(WMSTekSessions.Global.MainFilter))
            {
                context.MainFilter = (List<EntityFilter>)Session[WMSTekSessions.Global.MainFilter];
            }
            else if (context.MainFilter == null)
            {
                context.MainFilter = new List<EntityFilter>();
                var arrEnum = Enum.GetValues(typeof(EntityFilterName));

                foreach (var item in arrEnum)
                {
                    context.MainFilter.Add(new EntityFilter(item.ToString(), new FilterItem()));
                }
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].FilterValues.Clear();
            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();

            if (typeCustomer != "-1")
            {
                context.MainFilter[Convert.ToInt16(EntityFilterName.Customer)].AddFilterItem(new FilterItem("", typeCustomer));
            }

            context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].AddFilterItem(new FilterItem("", idOwn.ToString()));

            if (typoLead == "D")
            {
                typeTitulo = "Delta Dias";
                kpiLeadTimeViewDTO = iDashboardMGR.GetKpiLeadTime(idWhs, true, context);
            }
            else
            {
                typeTitulo = "Delta Horas";
                kpiLeadTimeViewDTO = iDashboardMGR.GetKpiLeadTime(idWhs, false, context);
            }

            //Agrupa los valores del LeadTime 
            DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
            var lst = from a in kpiLeadTimeViewDTO.Entities.ToList()
                      group a by new { a.SpecialField1, a.DateCreated.Month, a.DateCreated.Year } into g
                      select new
                      {
                          Delta = decimal.Round((g.Sum(i => i.Delta) / g.Count()), 0),
                          Month = formatoFecha.GetMonthName(g.First().DateCreated.Month),
                          CustomerName = g.First().CustomerName,
                          TypeCustomer = g.First().SpecialField1
                      };


            //Informacion del Grafico
            this.ChartKpiLeadTime.ChartAreas[0].AxisX.Title = "Fecha Creación";
            this.ChartKpiLeadTime.ChartAreas[0].AxisY.Title = typeTitulo;

            //this.Chart1.Series["Series1"].XValueType = ChartValueType.DateTime;
            //this.Chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd-MM-yyyy";
            //Angulo en el que se mostrara el contenido del Eje X
            this.ChartKpiLeadTime.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

            this.ChartKpiLeadTime.Series["Series1"].YValueType = ChartValueType.Double;

            Random random = new Random();
            foreach (var item in lst)
            {
                int currentIndex = this.ChartKpiLeadTime.Series["Series1"].Points.AddXY(item.Month, item.Delta);
                this.ChartKpiLeadTime.Series["Series1"].Points[currentIndex].Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                this.ChartKpiLeadTime.Series["Series1"].Points[currentIndex].ToolTip = item.Month.Trim() + " / " + item.Delta.ToString() + (typoLead == "D" ? "dias" : "horas");
            }
            this.ChartKpiLeadTime.DataBind();

            string typeChart = ddlTypeLeadTime.SelectedValue;
            SeriesChartType type = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), typeChart);

            //Tipo de Grafico
            this.ChartKpiLeadTime.Series["Series1"].ChartType = type;
            this.ChartKpiLeadTime.Series["Series1"].CustomProperties = "MinPixelPointWidth=5, DrawSideBySide=False, DrawingStyle=Cylinder";
        }

        public void LoadKpiInfo()
        {
            double KpiInfoTotal = 0;
            double KpiReleased = 0;
            double KpiPicking = 0;
            double KpiPacking = 0;
            double KpiAnden = 0;
            double KpiDispatch = 0;

            Int32 type = 7;
            var lstTypeLoc = GetConst("DiffDaysDashboard");

            if (lstTypeLoc.Count > 0)
                type = Convert.ToInt32(lstTypeLoc[0]);

            DateTime EmissionDateStart = DateTime.Now.AddDays(-type);
            DateTime EmissionDateEnd = DateTime.Now.AddDays(type);

            //DateTime EmissionDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
            //DateTime EmissionDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            //TODO cambio forma de llamar a kpi
            kpiInfoDashboardViewDTO = null;  //iDashboardMGR.GetKpiInfoDashboard(idWhs, idOwn, EmissionDateStart, EmissionDateEnd, context);

            if (kpiInfoDashboardViewDTO.Entities != null && kpiInfoDashboardViewDTO.Entities.Count() > 0)
            {
                foreach (var item in kpiInfoDashboardViewDTO.Entities)
                {
                    switch (item.Type.Trim().ToLower())
                    {
                        case "pedidostotales":
                            KpiInfoTotal = item.Total;
                            break;

                        case "pedidosliberados":
                            KpiReleased = item.Total;
                            break;

                        case "picking":
                            KpiPicking = item.Total;
                            break;

                        case "packing":
                            KpiPacking = item.Total;
                            break;

                        case "anden":
                            KpiAnden = item.Total;
                            break;

                        case "despacho":
                            KpiDispatch = item.Total;
                            break;
                    }
                }
            }

            this.lblKpiInfoTotal.Text = KpiInfoTotal.ToString();
            this.lblKpiReleased.Text = (KpiInfoTotal == 0 ? (KpiReleased == 0 ? "0" : "100") : Math.Round((KpiReleased / KpiInfoTotal) * 100, 0).ToString());
            this.lblKpiReleasedDesc.Text = KpiReleased + "/" + KpiInfoTotal;
            this.lblKpiPicking.Text = (KpiReleased == 0 ? (KpiPicking == 0 ? "0" : "100") : Math.Round((KpiPicking / KpiReleased) * 100, 0).ToString());
            this.lblKpiPickingDesc.Text = KpiPicking + "/" + KpiReleased;
            this.lblKpiPacking.Text = KpiPacking.ToString();
            this.lblKpiAnden.Text = (KpiPacking == 0 ? (KpiAnden == 0 ? "0" : "100") : Math.Round((KpiAnden / KpiPacking) * 100, 0).ToString());
            this.lblKpiAndenDesc.Text = KpiAnden + "/" + KpiPacking;
            this.lblKpiDispatch.Text = (KpiAnden == 0 ? (KpiDispatch == 0 ? "0" : "100") : Math.Round((KpiDispatch / KpiAnden) * 100, 0).ToString());
            this.lblKpiDispatchDesc.Text = KpiDispatch + "/" + KpiAnden;

        }

        protected void ddlLeadTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart_KpiLeadTime();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlKpiZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataChart_KpiZone();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlUserCharts4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart_KpiPicking();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlCustomerCharts5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart_KpiFillRate();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlTypeKpiZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataChart_KpiZone();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlTypeKpiPicking_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart_KpiPicking();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlTypeFillRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart_KpiFillRate();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }
        protected void ddlTypeLeadTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadChart_KpiLeadTime();
            }
            catch (Exception ex)
            {
                this.Master.ucError.ShowError(baseControl.handleException(ex, context));
            }
        }

        protected void ChartKpiZona_Click(object sender, System.Web.UI.WebControls.ImageMapEventArgs e)
        {
            string IdWorZone = e.PostBackValue;
            //Carga nuevamente el grafico
            LoadDataChart_KpiZone();

            //if ( ValidateSession(WMSTekSessions.Shared.ListGridViewKPI) && !string.IsNullOrEmpty(IdWorZone))
            if (kpiProductivityForZoneViewDTO.Entities != null && kpiProductivityForZoneViewDTO.Entities.Count > 0)
            {
                //List<KpiProductivityForZone> lstZone = (List<KpiProductivityForZone>)Session[WMSTekSessions.Shared.ListGridViewKPI];

                //LoadChart_KpiZone(kpiProductivityForZoneViewDTOAux.Entities);
                //LoadGridObject(new KpiProductivityForZone(), grdGridKpiZone, kpiProductivityForZoneViewDTO.Entities.ToArray());
                Session.Remove(WMSTekSessions.Shared.ListGridViewKPI);

                context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].FilterValues.Clear();
                context.MainFilter[Convert.ToInt16(EntityFilterName.WorkZone)].AddFilterItem(new FilterItem("", IdWorZone.Trim()));

                kpiProductivityForZoneViewDTO = iDashboardMGR.GetKpiZoneDetail(idWhs, context);

                // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
                //if (!kpiProductivityForZoneViewDTO.hasConfigurationError())
                //    base.ConfigureGridByProperties(grdGridKpiZone, kpiProductivityForZoneViewDTO.Configuration);

                grdGridKpiZone.EmptyDataText = this.Master.EmptyGridText;
                grdGridKpiZone.DataSource = kpiProductivityForZoneViewDTO.Entities;
                grdGridKpiZone.DataBind();

                Session.Add(WMSTekSessions.Shared.ListGridViewKPI, kpiProductivityForZoneViewDTO);

                lblTitlePopUp.Text = "Detalle " + this.lblTitleKpiZona.Text;
                divKpiPopUp.Visible = true;

                divGidKpiFillRate.Visible = false;
                divGridKpiLeadTime.Visible = false;
                divGridPicking.Visible = false;
                divGridKpiZone.Visible = true;

                modalPopUp.Show();
            }
        }

        protected void ChartKpiPicking_Click(object sender, System.Web.UI.WebControls.ImageMapEventArgs e)
        {
            string a = e.PostBackValue;
            string user = this.ddlUserCharts4.SelectedValue.Trim();

            //Carga nuevamente el grafico
            LoadChart_KpiPicking();

            if (taskKpiProductivityViewDTO.Entities != null && taskKpiProductivityViewDTO.Entities.Count > 0)
            {

                context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Clear();

                if (user != "-1")
                {
                    context.MainFilter[Convert.ToInt16(EntityFilterName.Name)].AddFilterItem(new FilterItem("", user));
                }

                taskKpiProductivityViewDTO = iDashboardMGR.GetKpiProductivityByUserWhsDetail(idWhs, context);

                //LoadGridObject(new KpiProductivity(), grdGridPicking, taskKpiProductivityViewDTO.Entities.ToArray());

                //if (!taskKpiProductivityViewDTO.hasConfigurationError())
                //    base.ConfigureGridByProperties(grdGridPicking, taskKpiProductivityViewDTO.Configuration);

                grdGridPicking.EmptyDataText = this.Master.EmptyGridText;
                grdGridPicking.DataSource = taskKpiProductivityViewDTO.Entities;
                grdGridPicking.DataBind();

                Session.Add(WMSTekSessions.Shared.ListGridViewKPI, taskKpiProductivityViewDTO);

                lblTitlePopUp.Text = "Detalle " + this.lblTitelKpiPicking.Text;
                divKpiPopUp.Visible = true;

                divGidKpiFillRate.Visible = false;
                divGridKpiLeadTime.Visible = false;
                divGridPicking.Visible = true;
                divGridKpiZone.Visible = false;

                modalPopUp.Show();
            }
        }

        protected void ChartKpiLeadTime_Click(object sender, System.Web.UI.WebControls.ImageMapEventArgs e)
        {
            string a = e.PostBackValue;
            //Carga nuevamente el grafico
            LoadChart_KpiLeadTime();

            if (kpiLeadTimeViewDTO.Entities != null && kpiLeadTimeViewDTO.Entities.Count > 0)
            {
                //LoadGridObject(new KpiLeadTime(), grdMgr, kpiLeadTimeViewDTO.Entities.ToArray());

                //if (!kpiLeadTimeViewDTO.hasConfigurationError())
                //    base.ConfigureGridByProperties(grdGridKpiLeadTime, kpiLeadTimeViewDTO.Configuration);

                grdGridKpiLeadTime.EmptyDataText = this.Master.EmptyGridText;
                grdGridKpiLeadTime.DataSource = kpiLeadTimeViewDTO.Entities;
                grdGridKpiLeadTime.DataBind();

                Session.Add(WMSTekSessions.Shared.ListGridViewKPI, grdGridKpiLeadTime);

                lblTitlePopUp.Text = "Detalle " + this.lblTitleKpiLeadTime.Text + " (" + this.ddlLeadTime.SelectedItem.Text + ")";
                divKpiPopUp.Visible = true;

                divGidKpiFillRate.Visible = false;
                divGridKpiLeadTime.Visible = true;
                divGridPicking.Visible = false;
                divGridKpiZone.Visible = false;

                modalPopUp.Show();
            }
        }

        protected void ChartKpiFillRate_Click(object sender, System.Web.UI.WebControls.ImageMapEventArgs e)
        {
            string a = e.PostBackValue;
            //Carga nuevamente el grafico
            LoadChart_KpiFillRate();

            if (kpiFillRateViewDTO.Entities != null && kpiFillRateViewDTO.Entities.Count > 0)
            {
                //LoadGridObject(new KpiFillRate(), grdMgr, kpiFillRateViewDTO.Entities.ToArray());

                //if (!kpiFillRateViewDTO.hasConfigurationError())
                //    base.ConfigureGridByProperties(grdGidKpiFillRate, kpiFillRateViewDTO.Configuration);

                grdGidKpiFillRate.EmptyDataText = this.Master.EmptyGridText;
                grdGidKpiFillRate.DataSource = kpiFillRateViewDTO.Entities;
                grdGidKpiFillRate.DataBind();

                Session.Add(WMSTekSessions.Shared.ListGridViewKPI, kpiFillRateViewDTO);

                lblTitlePopUp.Text = "Detalle " + this.lblKpiFillRate.Text;
                divKpiPopUp.Visible = true;

                divGidKpiFillRate.Visible = true;
                divGridKpiLeadTime.Visible = false;
                divGridPicking.Visible = false;
                divGridKpiZone.Visible = false;

                modalPopUp.Show();
            }
        }

        private void LoadGridObject(object typeObject, GridView grid, Object[] lstObject)
        {
            Type type = typeObject.GetType();
            PropertyInfo[] propertyInfo = type.GetProperties();
            for (int i = 0; i < propertyInfo.Count(); i++)
            {
                BoundField columna = new BoundField();
                columna.HeaderText = propertyInfo[i].Name;
                columna.DataField = propertyInfo[i].Name;
                grid.Columns.Add(columna);
            }

            grid.DataSource = lstObject;
            grid.DataBind();
        }


        protected void btnKpiExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.Shared.ListGridViewKPI))
                {
                    if (divGidKpiFillRate.Visible)
                    {
                        kpiFillRateViewDTO = (GenericViewDTO<KpiFillRate>)Session[WMSTekSessions.Shared.ListGridViewKPI];

                        grdGidKpiFillRate.DataSource = kpiFillRateViewDTO.Entities;
                        grdGidKpiFillRate.DataBind();

                        base.ExportToExcel(this.grdGidKpiFillRate, lblTitlePopUp.Text, null, null);
                    }

                    if (divGridKpiLeadTime.Visible)
                    {
                        kpiLeadTimeViewDTO = (GenericViewDTO<KpiLeadTime>)Session[WMSTekSessions.Shared.ListGridViewKPI];

                        grdGridKpiLeadTime.DataSource = kpiLeadTimeViewDTO.Entities;
                        grdGridKpiLeadTime.DataBind();

                        base.ExportToExcel(this.grdGridKpiLeadTime, lblTitlePopUp.Text, null, null);
                    }

                    if (divGridPicking.Visible)
                    {
                        taskKpiProductivityViewDTO = (GenericViewDTO<KpiProductivity>)Session[WMSTekSessions.Shared.ListGridViewKPI];

                        grdGridPicking.DataSource = taskKpiProductivityViewDTO.Entities;
                        grdGridPicking.DataBind();

                        base.ExportToExcel(this.grdGridPicking, lblTitlePopUp.Text, new GridView(), null);
                    }

                    if (divGridKpiZone.Visible)
                    {
                        //grdGridKpiZone.AllowPaging = false;
                        kpiProductivityForZoneViewDTO = (GenericViewDTO<KpiProductivityForZone>)Session[WMSTekSessions.Shared.ListGridViewKPI];

                        grdGridKpiZone.DataSource = kpiProductivityForZoneViewDTO.Entities;
                        grdGridKpiZone.DataBind();

                        base.ExportToExcel(this.grdGridKpiZone, lblTitlePopUp.Text, null, null);
                        //grdGridKpiZone.AllowPaging = true;
                    }

                }

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
            //objControl.DataTextField = "Name";
            //objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        private void LoadUnidPicking()
        {
            this.ddlKpiPickingUnid.Items.Insert(0, new ListItem("Cantidad", "Cantidad"));
            this.ddlKpiPickingUnid.Items.Insert(1, new ListItem("Lineas", "Lineas"));
            this.ddlKpiPickingUnid.Items.Insert(2, new ListItem("LPN", "LPN"));

            this.ddlKpiPickingUnid.Items[0].Selected = true;
        }


        #endregion FIN GRAFICOS
    }
}
