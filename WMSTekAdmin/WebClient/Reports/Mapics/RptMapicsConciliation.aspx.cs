using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Reports.Mapics
{
    public partial class RptMapicsConciliation : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();
        public FilterReport ucMainFilterReport
        {
            get { return this.ucFilterReport; }
        }
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    this.ucMainFilterReport.emptyRowLabelText = "Seleccione";
                    Initialize();
                    divReport.Visible = false;
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
                CallJsGridViewHeader();
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

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

        protected void Initialize()
        {
            divWarning.Visible = false;
            InitializeFilter(!Page.IsPostBack, false);
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.Visible = false;
            this.ucMainFilterReport.Visible = true;
            //this.ucMainFilterReport.warehouseVisible = true;
            this.ucMainFilterReport.dateVisible = true;
            this.ucMainFilterReport.itemVisible = true;
            //this.ucMainFilterReport.changeDefaultWhsAsData = true;

            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
        }
        private void LoadReport()
        {
            reportViewDTO.MessageStatus.Message = string.Empty;
            divWarning.Visible = false;
            divReport.Visible = true;

            // Le indicamos la carpeta y el informe sin la extensión de este.
            this.rptMapicsConciliation.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportMapicsConciliationPath", string.Empty);

            // Y el servidor donde está alojado el informe.
            this.rptMapicsConciliation.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

            //Credenciales
            int total = rptMapicsConciliation.ServerReport.GetDataSources().Count;
            DataSourceCredentials[] permisos = new DataSourceCredentials[total];

            ReportDataSourceInfoCollection datasources = rptMapicsConciliation.ServerReport.GetDataSources();
            for (int j = 0; j < total; j++)
            {
                permisos[j].Name = datasources[j].Name;
                permisos[j].UserId = "";
                permisos[j].Password = "";
            }
            rptMapicsConciliation.ServerReport.SetDataSourceCredentials(permisos);


            //Capturo los valores de los filtros
            //DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            var txtFilterDate = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDate");
            var txtFilterItem = (TextBox)this.ucMainFilterReport.FindControl("txtFilterItem");

            //var warehouseCode = ddlWhs.SelectedItem.Text.Trim();
            var executionTime = txtFilterDate.Text.Trim();
            var sku = txtFilterItem.Text.Trim();

            // Creo una colección de parámetros de tipo ReportParameter 
            // para añadirlos al control ReportViewer.
            reportParameter = new List<ReportParameter>();
            reportParameter.Clear();

            // Añado los parámetros necesarios.
            reportParameter.Add(new ReportParameter("executionTime", executionTime, false));
            reportParameter.Add(new ReportParameter("SKU", sku, false));
            reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

            //DandoFormato a los margenes del informe
            rptMapicsConciliation.Attributes.Add("style", "margin-bottom: 50px;");
            rptMapicsConciliation.ShowPrintButton = false;
            // Añado el/los parámetro/s al ReportViewer.
            this.rptMapicsConciliation.ServerReport.SetParameters(reportParameter);

            rptMapicsConciliation.DataBind();

            divReport.Visible = true;

            rptMapicsConciliation.ServerReport.Refresh();

            CallJsGridViewHeader();
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}