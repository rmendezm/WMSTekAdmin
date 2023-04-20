using System;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Utils;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Reports.Stock
{
    public partial class RptStateStockConsult : BasePage
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
            this.ucMainFilterReport.ownerVisible = true;
            this.ucMainFilterReport.warehouseVisible = true;
            this.ucMainFilterReport.itemVisible = true;

            this.ucMainFilterReport.rfvWarehouseEnabled = false;
            this.ucMainFilterReport.rfvOwnerEnabled = false;

            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);

            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            ddlOwn.AutoPostBack = false;
        }

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

        private void LoadReport()
        {
            // Le indicamos la carpeta y el informe sin la extensión de este.
            this.rptViewStateStockConsult.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportStateStockConsult", string.Empty);

            // Y el servidor donde está alojado el informe.
            this.rptViewStateStockConsult.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

            //Credenciales
            int total = rptViewStateStockConsult.ServerReport.GetDataSources().Count;
            DataSourceCredentials[] permisos = new DataSourceCredentials[total];

            ReportDataSourceInfoCollection datasources = rptViewStateStockConsult.ServerReport.GetDataSources();
            for (int j = 0; j < total; j++)
            {
                permisos[j].Name = datasources[j].Name;
                permisos[j].UserId = "";
                permisos[j].Password = "";
            }

            rptViewStateStockConsult.ServerReport.SetDataSourceCredentials(permisos);


            //Capturo los valores de los filtros
            DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            TextBox txtItem = (TextBox)this.ucMainFilterReport.FindControl("txtFilterItem");

            string idWhs = ddlWhs.SelectedValue.ToString();
            string idOwn = ddlOwn.SelectedValue.ToString();
            string itemCode = txtItem.Text.Trim();

            // Creo una colección de parámetros de tipo ReportParameter 
            // para añadirlos al control ReportViewer.
            reportParameter = new List<ReportParameter>();
            reportParameter.Clear();

            // Añado los parámetros necesarios.
            reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
            reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
            reportParameter.Add(new ReportParameter("ItemCode", itemCode, false));
            reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

            //DandoFormato a los margenes del informe
            rptViewStateStockConsult.Attributes.Add("style", "margin-bottom: 50px;");
            rptViewStateStockConsult.ShowPrintButton = true;
            // Añado el/los parámetro/s al ReportViewer.
            this.rptViewStateStockConsult.ServerReport.SetParameters(reportParameter);

            rptViewStateStockConsult.DataBind();

            divReport.Visible = true;

            rptViewStateStockConsult.ServerReport.Refresh();

            CallJsGridViewHeader();
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}