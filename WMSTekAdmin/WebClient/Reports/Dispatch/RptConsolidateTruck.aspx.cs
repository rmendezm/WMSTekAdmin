using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;

using Microsoft.Reporting.WebForms;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Display;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using Binaria.WMSTek.Framework.Entities.Base;


namespace Binaria.WMSTek.WebClient.Reports.Dispatch
{
    public partial class RptConsolidateTruck : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();
        public FilterReport ucMainFilterReport
        {
            get { return this.ucFilterReport; }
        }
        /// <summary>
        /// Propiedad para obtener el id del owner seleccionado
        /// </summary>
        public int currentIdOwner
        {
            get
            {
                if (ValidateSession("idOwn"))
                    return (int)Session["idOwn"];
                else
                    return -1;
            }

            set { Session["idOwn"] = value; }
        }

        /// <summary>
        /// Inicializa filtros si esta en modo normal
        /// </summary>
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    this.ucMainFilterReport.emptyRowLabelText = "Seleccione";
                    InitializeFilter(!Page.IsPostBack, false);
                }
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        //private Stream CreateStream(string name, string fileNameExtension,
        // Encoding encoding,
        //string mimeType, bool willSeek)
        //{
        //    Stream stream = new FileStream(name + "." + fileNameExtension,
        //        FileMode.Create);
        //    m_streams.Add(stream);
        //    return stream;
        //}
        //private void Export(LocalReport report)
        //{
        //    string deviceInfo =
        //      "<DeviceInfo>" +
        //      "  <OutputFormat>EMF</OutputFormat>" +
        //      "  <PageWidth>8.5in</PageWidth>" +
        //      "  <PageHeight>11in</PageHeight>" +
        //      "  <MarginTop>0.25in</MarginTop>" +
        //      "  <MarginLeft>0.25in</MarginLeft>" +
        //      "  <MarginRight>0.25in</MarginRight>" +
        //      "  <MarginBottom>0.25in</MarginBottom>" +
        //      "</DeviceInfo>";
        //    Warning[] warnings;
        //    m_streams = new List<Stream>();
        //    report.Render("Image", deviceInfo, CreateStream, out warnings);

        //    foreach (Stream stream in m_streams)
        //        stream.Position = 0;
        //}

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.Visible = false;
            this.ucMainFilterReport.Visible = true;
            this.ucMainFilterReport.warehouseVisible = true;
            this.ucMainFilterReport.codeVisible = true;
            this.ucMainFilterReport.codeLabel = lblCodeTruck.Text;
            this.ucMainFilterReport.dateVisible = true;
            this.ucMainFilterReport.dateLabel = lblDate.Text;
            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
            this.ucMainFilterReport.reqTxtFilterEnabled = true;
        }

        /// <summary>
        /// Carga la ruta, los parametros y carga el reporte
        /// </summary>
        private void LoadReport()
        {
            try
            {
                // Le indicamos la carpeta y el informe sin la extensión de este.
                this.rptViewConsolidate.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportConsolidateTruckPath", string.Empty);

                // Y el servidor donde está alojado el informe.
                this.rptViewConsolidate.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));
                this.rptViewConsolidate.ShowPrintButton = true;
                //Credenciales
                int total = rptViewConsolidate.ServerReport.GetDataSources().Count;
                DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                ReportDataSourceInfoCollection datasources = rptViewConsolidate.ServerReport.GetDataSources();
                for (int j = 0; j < total; j++)
                {
                    permisos[j].Name = datasources[j].Name;
                    permisos[j].UserId = "";
                    permisos[j].Password = "";
                }
                rptViewConsolidate.ServerReport.SetDataSourceCredentials(permisos);


                //Capturo los valores de los filtros
                DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
                TextBox txtTruck = (TextBox)this.ucMainFilterReport.FindControl("txtFilterCode");
                TextBox txtFilterDate = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDate");

                string idWhs = ddlWhs.SelectedValue.ToString();
                string idTruckCode = string.IsNullOrEmpty(txtTruck.Text.Trim()) ? "-1" : txtTruck.Text.Trim();

                DateTime dt1 = Convert.ToDateTime(txtFilterDate.Text);
                DateTime dt = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second, dt1.Millisecond);

                string date1 = String.Format("{0:dd/MM/yyyy}", dt);
                string DateFormat1From = date1;//este queda por defecto con hora 00:00:00 
                DateTime DateFormat2To = Convert.ToDateTime(dt).AddDays(1);
                //DateFormat2To.AddDays(1);
                string date2 = String.Format("{0:dd/MM/yyyy}", DateFormat2To);

                // Creo una colección de parámetros de tipo ReportParameter 
                // para añadirlos al control ReportViewer.
                reportParameter = new List<ReportParameter>();
                reportParameter.Clear();

                // Añado los parámetros necesarios.
                reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                reportParameter.Add(new ReportParameter("IdTruckCode", idTruckCode, false));
                reportParameter.Add(new ReportParameter("TrackOutboundDate1", dt1.ToString("dd/MM/yyyy"), false));
                reportParameter.Add(new ReportParameter("TrackOutboundDate2", dt1.AddDays(1).ToString("dd/MM/yyyy"), false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

                //DandoFormato a los margenes del informe
                //rptViewConsolidate.Attributes.Add("style", "margin-bottom:0px;");
                rptViewConsolidate.ShowPrintButton = true;

                // Añado el/los parámetro/s al ReportViewer.
                this.rptViewConsolidate.ServerReport.SetParameters(reportParameter);

                rptViewConsolidate.DataBind();

                divReport.Visible = true;

                rptViewConsolidate.ServerReport.Refresh();
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);                
        
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }
    }
}
