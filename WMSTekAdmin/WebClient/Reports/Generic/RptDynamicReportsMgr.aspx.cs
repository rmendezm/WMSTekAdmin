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
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.WebClient.Shared;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.WebClient.Reports.Utils;

namespace Binaria.WMSTek.WebClient.Reports
{
    public partial class RptDynamicReportsMgr : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Reception.Receipt> receiptViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Reception.Receipt>();
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Dispatching.Dispatch> dispatchViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Dispatching.Dispatch>();


        public FilterReport ucMainFilterReport
        {
            get { return this.ucFilterReport; }
        }

        public int idInboundOrder
        {
            get
            {
                if (ValidateViewState("IdInboundOrder"))
                    return (int)ViewState["IdInboundOrder"];
                else
                    return 0;
            }

            set { ViewState["IdInboundOrder"] = value; }
        }

        public int idDispatch
        {
            get
            {
                if (ValidateViewState("IdDispatch"))
                    return (int)ViewState["IdDispatch"];
                else
                    return 0;
            }

            set { ViewState["IdDispatch"] = value; }
        }
        

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
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                CallJsGridViewHeader();
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }            
        }

        protected void Initialize()
        {
            InitializeFilter(!Page.IsPostBack, false);
        }
        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.Visible = false;
            this.ucMainFilterReport.Visible = true;
            this.ucMainFilterReport.searchVisible = false;
            this.ucMainFilterReport.typeReportVisible = true;
            this.ucMainFilterReport.Initialize(init, refresh);

            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
            this.ucMainFilterReport.DdlTypeReportSelectedIndexChanged += new EventHandler(ddlTypeReport_SelectedIndexChanged);

            loadControlsTypeReports(init);
        }

        void ddlTypeReport_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                loadControlsTypeReports(Page.IsPostBack);
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
          
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadReport();
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        private void LoadReport()
        {
            var server = MiscUtils.ReadSetting("reportingServiceAddress", string.Empty);
            var fixedUrl = "/Pages/ReportViewer.aspx?";

            var typeReport = this.ucMainFilterReport.typeReports.Trim();
            var reportUrl = MiscUtils.ReadSetting(typeReport, string.Empty);

            var embeded = "&rs:embed=true";

            var rSStringConectionLinkedServerBD = MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty);
            var rssCnx = "&StringConectionLinkedServerBD=" + rSStringConectionLinkedServerBD;

            var finalUrl = server + fixedUrl + reportUrl + embeded + rssCnx;

            var script = "loadReportFromSSRS('" + finalUrl + "');";
            ScriptManager.RegisterStartupScript(Page, GetType(), "loadReportFromSSRS", script, true);
        }

        private void LoadReportOld()
        {
            string typeReport = this.ucMainFilterReport.typeReports.Trim();
            bool showReport = false;
            this.divWarning.Visible = true;

            if (typeReport == "ReportTotalReceiptPath" || typeReport == "ReportReceiptPath" ||
                typeReport == "ReportDispatchPath" || typeReport == "ReportHistoricItemsReceiptPath" ||
                typeReport == "ReportHistoricItemsRepositPath" || typeReport == "ReportHistoricItemsPickeadosPath" ||
                typeReport == "ReportHistoricItemsPackedPath" || typeReport == "ReportHistoricItemsCyclicCountPath" ||
                typeReport == "ReportHistoricItemsDicpatchPath" || typeReport == "ReportHistoricItemsStoredPath")
            {
                showReport = valiDataShowReport(typeReport);
            }
            else
            {
                showReport = true;
            }

            if (showReport)
            {
                // Le indicamos la carpeta y el informe sin la extensión de este.
                this.rptViewDynamic.ServerReport.ReportPath = MiscUtils.ReadSetting(typeReport, string.Empty);

                // Y el servidor donde está alojado el informe.
                this.rptViewDynamic.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

                //Asigna Credenciales
                //this.rptViewDynamic.ServerReport.ReportServerCredentials = new CustomReportCredentials("User", "Password", "Dom");

                ////Credenciales                    
                int total = this.rptViewDynamic.ServerReport.GetDataSources().Count;
                DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                ReportDataSourceInfoCollection datasources = this.rptViewDynamic.ServerReport.GetDataSources();
                for (int j = 0; j < 0; j++)
                {
                    permisos[j].Name = datasources[total].Name;
                    permisos[j].UserId = "";
                    permisos[j].Password = "";
                }
                this.rptViewDynamic.ServerReport.SetDataSourceCredentials(permisos);

                //Llena los parametros necesarios para los reportes
                loadReportParameter(typeReport);

                //DandoFormato a los margenes del informe
                this.rptViewDynamic.Attributes.Add("style", "margin-bottom: 50px;");
                this.rptViewDynamic.ShowPrintButton = false;
                this.rptViewDynamic.ShowFindControls = false;
                this.rptViewDynamic.ShowRefreshButton = false;

                // Añado el/los parámetro/s al ReportViewer.
                this.rptViewDynamic.ServerReport.SetParameters(reportParameter);
                this.rptViewDynamic.DataBind();
                this.divWarning.Visible = false;
                this.divReport.Visible = true;
                this.rptViewDynamic.ServerReport.Refresh();
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivReport();", true);
            }
        }

        private void loadReportParameter(string typeReport)
        {
            // Creo una colección de parámetros de tipo ReportParameter 
            // para añadirlos al control ReportViewer.
            reportParameter = new List<ReportParameter>();
            reportParameter.Clear();

            //Capturo los valores de los filtros
            DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            TextBox txtTruck = (TextBox)this.ucMainFilterReport.FindControl("txtFilterCode");
            TextBox txtFilterDate = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDate");
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwnerUser");
            DropDownList ddlCtgItem = (DropDownList)this.ucMainFilterReport.FindControl("ddlCategoryItem");
            DropDownList ddlGrp1 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm1");
            DropDownList ddlGrp2 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm2");
            DropDownList ddlGrp3 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm3");
            DropDownList ddlGrp4 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm4");
            DropDownList ddlWorkZone = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWorkZone");
            TextBox txtLocation = (TextBox)this.ucMainFilterReport.FindControl("txtFilterLocation");
            TextBox txtDocNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDocumentNumber");
            TextBox txtIdReceipt = (TextBox)this.ucMainFilterReport.FindControl("txtFilterCode");
            TextBox txtDocReference = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDescription");
            TextBox txtYear = (TextBox)this.ucMainFilterReport.FindControl("txtFilterYear");
            TextBox txtDateFrom = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDateFrom");
            TextBox txtDateTo = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDateTo");
            TextBox txtItem = (TextBox)this.ucMainFilterReport.FindControl("txtFilterItem");

            string idWhs = ddlWhs.SelectedValue.ToString();
            string idOwn = ddlOwn.SelectedValue.ToString();
            string idCtgItem = ddlCtgItem.SelectedValue.ToString();
            string idGrp1 = ddlGrp1.SelectedValue.ToString();
            string idGrp2 = ddlGrp2.SelectedValue.ToString();
            string idGrp3 = ddlGrp3.SelectedValue.ToString();
            string idGrp4 = ddlGrp4.SelectedValue.ToString();
            string idTruckCode = txtTruck.Text.Trim();
            string idWorkZone = ddlWorkZone.SelectedValue.ToString();
            string idLocCode = txtLocation.Text.Trim();
            string docNumber = txtDocNumber.Text.Trim();
            string idReceipt = txtIdReceipt.Text.Trim();
            string docReference = txtDocReference.Text.Trim();
            string year = txtYear.Text.Trim();
            string nomWhs = ddlWhs.SelectedItem.ToString();
            string nomOwn = ddlOwn.SelectedItem.ToString();
            string idItem = txtItem.Text.Trim();

            if (typeReport == "ReportStockByItemPath")
            {
                // Añado los parámetros necesarios.
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("IdCtgItem", idCtgItem, false));
                reportParameter.Add(new ReportParameter("IdGrpItem1", idGrp1, false));
                reportParameter.Add(new ReportParameter("IdGrpItem2", idGrp2, false));
                reportParameter.Add(new ReportParameter("IdGrpItem3", idGrp3, false));
                reportParameter.Add(new ReportParameter("IdGrpItem4", idGrp4, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportStockByLocPath")
            {
                // Añado los parámetros necesarios.
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("IdWorkZone", idWorkZone, false));
                reportParameter.Add(new ReportParameter("IdLocCode", idLocCode, false));
                reportParameter.Add(new ReportParameter("IdGrpItem1", idGrp1, false));
                reportParameter.Add(new ReportParameter("IdGrpItem2", idGrp2, false));
                reportParameter.Add(new ReportParameter("IdGrpItem3", idGrp3, false));
                reportParameter.Add(new ReportParameter("IdGrpItem4", idGrp4, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportConsolidateTruckPath")
            {

                DateTime dt1 = Convert.ToDateTime(txtFilterDate.Text);
                DateTime dt = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second, dt1.Millisecond);

                string date1 = String.Format("{0:MM/dd/yyyy}", dt);
                string DateFormat1From = date1;//este queda por defecto con hora 00:00:00 
                DateTime DateFormat2To = Convert.ToDateTime(dt).AddDays(1);
                //DateFormat2To.AddDays(1);
                string date2 = String.Format("{0:MM/dd/yyyy}", DateFormat2To);

                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdTruckCode", idTruckCode, false));
                reportParameter.Add(new ReportParameter("TrackOutboundDate1", DateFormat1From, false));
                reportParameter.Add(new ReportParameter("TrackOutboundDate2", date2, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportTotalReceiptPath")
            {
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("InboundNumber", docNumber, false));
                reportParameter.Add(new ReportParameter("IdInboundOrder", idInboundOrder.ToString(), false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportReceiptPath")
            {
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("InboundNumber", docNumber, false));
                reportParameter.Add(new ReportParameter("IdReceipt", idReceipt, false));
                reportParameter.Add(new ReportParameter("ReferenceDoc", docReference, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
                reportParameter.Add(new ReportParameter("IdInboundOrder", idInboundOrder.ToString(), false));
            }

            if (typeReport == "ReportDispatchPath")
            {
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("OutboundNumber", docNumber, false));
                reportParameter.Add(new ReportParameter("IdDispatch", idDispatch.ToString(), false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportHistoricItemsReceiptPath")
            {
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }
            
            if (typeReport == "ReportHistoricItemsRepositPath")
            {
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportHistoricItemsPickeadosPath")
            {
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }
            
            if (typeReport == "ReportHistoricItemsPackedPath")
            {
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportHistoricItemsCyclicCountPath")
            {
                // Añado los parámetros necesarios.
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportHistoricItemsDicpatchPath")
            {
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }
            
            if (typeReport == "ReportHistoricItemsStoredPath")
            {
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                reportParameter.Add(new ReportParameter("Year", year, false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

            if (typeReport == "ReportABCDItemsPath")
            {
                DateTime df = Convert.ToDateTime(txtDateFrom.Text.Trim());
                DateTime dt = Convert.ToDateTime(txtDateTo.Text.Trim());
                string dateFrom = String.Format("{0:MM/dd/yyyy}", df);
                string dateTo = String.Format("{0:MM/dd/yyyy}", dt);

                //Rescata los dias habiles existente entre las dos fechas
                TimeSpan ts = Convert.ToDateTime(txtDateTo.Text.Trim()) - Convert.ToDateTime(txtDateFrom.Text.Trim());
                int businessDays = this.GetBusinessDays(Convert.ToDateTime(txtDateFrom.Text.Trim()), Convert.ToDateTime(txtDateTo.Text.Trim()));
                int days = ts.Days;
                int holidaysDays = 0;

                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("IdItem", (string.IsNullOrEmpty(idItem.Trim())? "-1" : idItem.Trim()), false));
                reportParameter.Add(new ReportParameter("DateFrom", dateFrom, false));
                reportParameter.Add(new ReportParameter("DateTo", dateTo, false));
                reportParameter.Add(new ReportParameter("Days", days.ToString(), false));
                reportParameter.Add(new ReportParameter("BusinessDays", businessDays.ToString(), false));
                reportParameter.Add(new ReportParameter("HolidaysDays", holidaysDays.ToString(), false));
                reportParameter.Add(new ReportParameter("NomWhs", nomWhs, false));
                reportParameter.Add(new ReportParameter("NomOwn", nomOwn, false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
            }

        }

        private bool valiDataShowReport(string typeReport)
        {
            bool result = false;
            //Capturo los valores de los filtros
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwnerUser");
            TextBox txtDocNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDocumentNumber");
            TextBox txtIdReceipt = (TextBox)this.ucMainFilterReport.FindControl("txtFilterCode");
            TextBox txtDocReference = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDescription");
            TextBox txtYear = (TextBox)this.ucMainFilterReport.FindControl("txtFilterYear");
            DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");

            string idWhs = ddlWhs.SelectedValue.ToString();
            string idOwn = ddlOwn.SelectedValue.ToString();
            string docNumber = txtDocNumber.Text.Trim();
            string idReceipt = txtIdReceipt.Text.Trim();
            string docReference = txtDocReference.Text.Trim();
            string year = txtYear.Text.Trim();

            Binaria.WMSTek.Framework.Entities.Reception.Receipt receipt = new Binaria.WMSTek.Framework.Entities.Reception.Receipt();
            Binaria.WMSTek.Framework.Entities.Dispatching.OutboundOrder outbound = new Binaria.WMSTek.Framework.Entities.Dispatching.OutboundOrder();

            if (typeReport == "ReportTotalReceiptPath")
            {
                receipt.InboundOrder = new InboundOrder();
                receipt.InboundOrder.Owner = new Owner();
                receipt.InboundOrder.Owner.Id = Convert.ToInt32(idOwn);
                receipt.InboundOrder.Number = docNumber;

                receiptViewDTO = iReceptionMGR.GetTotalReceiptReportConsult(receipt, context);

                if (receiptViewDTO.Entities.Count > 0)
                {
                    idInboundOrder = receiptViewDTO.Entities[0].InboundOrder.Id;
                    result = true;
                }
                else
                {
                    this.reportViewDTO.MessageStatus = new MessageStatusDTO();
                    this.reportViewDTO.MessageStatus.Message = this.lblErrorReceipt.Text;
                    this.lblTitleWarning.Text = this.lblReceipt.Text.Trim();
                    this.divWarning.Visible = true;
                    this.divReport.Visible = false;
                }
            }

            if (typeReport == "ReportReceiptPath")
            {
                if (idReceipt == string.Empty) idReceipt = "-1";

                if (!MiscUtils.IsNumeric(idReceipt))
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitleInfo.Text, this.lblErrorReceiptFormat.Text, string.Empty);
                }
                else
                {
                    receipt.InboundOrder = new InboundOrder();
                    receipt.InboundOrder.Owner = new Owner();
                    receipt.InboundOrder.Owner.Id = Convert.ToInt32(idOwn);
                    receipt.InboundOrder.Number = docNumber;
                    receipt.Id = Convert.ToInt32(idReceipt);
                    receipt.ReferenceDoc = docReference;

                    // carga todas las recepciones 
                    context.MainFilter = this.Master.ucMainFilter.MainFilter;

                    //Consulta
                    receiptViewDTO = iReceptionMGR.GetReceiptHeaderReportConsult(context);

                    if (receiptViewDTO.Entities.Count > 0)
                    {
                        idInboundOrder = receiptViewDTO.Entities[0].InboundOrder.Id;
                        result = true;
                    }
                    else
                    {
                        this.reportViewDTO.MessageStatus = new MessageStatusDTO();
                        this.reportViewDTO.MessageStatus.Message = this.lblErrorReceipt.Text;
                        this.lblTitleWarning.Text = this.lblReceipt.Text.Trim();
                        this.divWarning.Visible = true;
                        this.divReport.Visible = false;
                    }
                }
            }

            if (typeReport == "ReportDispatchPath")
            {
                outbound.Owner = new Binaria.WMSTek.Framework.Entities.Warehousing.Owner();
                outbound.Owner.Id = Convert.ToInt32(idOwn);
                outbound.Number = docNumber;

                // carga todas las recepciones 
                context.MainFilter = this.Master.ucMainFilter.MainFilter;

                //consulta 
                dispatchViewDTO = iDispatchingMGR.GetOutboundDispatchHeader(context);

                if (dispatchViewDTO.Entities.Count > 0)
                {
                    //Obtengo el despacho para pasarlo como parametro al subreporte
                    idDispatch = dispatchViewDTO.Entities[0].Id;
                    result = true;
                }
                else
                {
                    this.reportViewDTO.MessageStatus = new MessageStatusDTO();
                    this.reportViewDTO.MessageStatus.Message = this.lblErrorDispatch.Text;
                    this.lblTitleWarning.Text = this.lblDispatch.Text.Trim();
                    this.divWarning.Visible = true;
                    this.divReport.Visible = false;
                }
            }

            if (typeReport == "ReportHistoricItemsReceiptPath" || typeReport == "ReportHistoricItemsRepositPath" ||
                typeReport == "ReportHistoricItemsPickeadosPath" || typeReport == "ReportHistoricItemsPackedPath" ||
                typeReport == "ReportHistoricItemsCyclicCountPath" || typeReport == "ReportHistoricItemsDicpatchPath" ||
                typeReport == "ReportHistoricItemsStoredPath")
            {
                //valida que no sea "blanco"
                if (year == string.Empty)
                {
                    year = "-1";
                }

                if (int.Parse(year) < 2012 || int.Parse(year) > 2030)
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitleInfo.Text, this.lblErrorReceptionYear.Text, string.Empty);
                    divReport.Visible = false;
                }
                else
                    //valida que sea numerico o muestra mensaje error
                    if (!MiscUtils.IsNumeric(year))
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitleInfo.Text, this.lblErrorFormatYear.Text, string.Empty);
                        divReport.Visible = false;
                    }
                    else
                    {
                        result = true;
                    }
            }

            return result;
        }

        private void loadControlsTypeReports(bool postBack)
        {
            string typeReport = this.ucMainFilterReport.typeReports.Trim();

            this.Master.ucMainFilter.Visible = false;
            this.ucMainFilterReport.Visible = true;
            this.ucMainFilterReport.searchVisible = true;

            this.ucMainFilterReport.warehouseVisible = false;
            this.ucMainFilterReport.codeVisible = false;
            this.ucMainFilterReport.dateVisible = false;
            this.ucMainFilterReport.ownerVisible = false;
            this.ucMainFilterReport.ownerUserVisible = false;
            this.ucMainFilterReport.categoryItemVisible = false;
            this.ucMainFilterReport.divBscGroupItem = false;
            this.ucMainFilterReport.documentVisible = false;
            this.ucMainFilterReport.descriptionVisible = false;
            this.ucMainFilterReport.locationVisible = false;
            this.ucMainFilterReport.workZoneVisible = false;
            this.ucMainFilterReport.dateFromVisible = false;
            this.ucMainFilterReport.dateToVisible = false;
            this.ucMainFilterReport.itemVisible = false;

            switch (typeReport)
            {
                case "ReportStockByItemPath":

                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.categoryItemVisible = true;
                    this.ucMainFilterReport.divBscGroupItem = true;
                    break;

                case "ReportStockByLocPath":
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.workZoneVisible = true;
                    this.ucMainFilterReport.locationVisible = true;
                    this.ucMainFilterReport.divBscGroupItem = true;
                    break;

                case "ReportTotalReceiptPath":
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.documentVisible = true;
                    break;

                case "ReportReceiptPath":
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.documentVisible = true;
                    this.ucMainFilterReport.codeVisible = true;
                    this.ucMainFilterReport.descriptionVisible = true;
                    this.ucMainFilterReport.descriptionLabel = this.lblDescription.Text;
                    this.ucMainFilterReport.codeLabel = this.lblCodeVirtual.Text;
                    break;

                case "ReportDispatchPath":
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.documentVisible = true;
                    break;

                case "ReportConsolidateTruckPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.codeVisible = true;
                    this.ucMainFilterReport.codeLabel = lblCodeTruck.Text;
                    this.ucMainFilterReport.dateVisible = true;
                    this.ucMainFilterReport.dateLabel = lblDate.Text;
                    break;

                case "ReportHistoricItemsReceiptPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportHistoricItemsRepositPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportHistoricItemsPickeadosPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportHistoricItemsPackedPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportHistoricItemsCyclicCountPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportHistoricItemsDicpatchPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportHistoricItemsStoredPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.YearVisible = true;
                    break;

                case "ReportABCDItemsPath":
                    this.ucMainFilterReport.warehouseVisible = true;
                    this.ucMainFilterReport.ownerUserVisible = true;
                    //this.ucMainFilterReport.ownerNotIncludeAll = true;
                    this.ucMainFilterReport.dateFromVisible = true;
                    this.ucMainFilterReport.dateToVisible = true;
                    this.ucMainFilterReport.itemVisible = true;
                    break;
            }

            this.ucMainFilterReport.ClearControls();
            this.ucMainFilterReport.Initialize(postBack, true);
            this.ucMainFilterReport.typeReports = typeReport;
            this.rptViewDynamic.ServerReport.Refresh();
            //this.rptViewDynamic.LocalReport.Refresh();
            this.rptViewDynamic.Reset();
            divWarning.Visible = false;

            CallJsGridViewHeader();
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}


