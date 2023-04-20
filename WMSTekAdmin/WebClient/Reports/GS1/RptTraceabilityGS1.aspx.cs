using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Reports.GS1
{
    public partial class RptTraceabilityGS1 : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();

        public FilterReport ucMainFilterReport
        {
            get { return this.ucFilterReport; }
        }

        //Propiedad para obtener el id del owner seleccionado
        public int currentIdOwner
        {
            get
            {
                if (ValidateSession("idOwn"))
                    return (int)Session["idOwn"];
                else
                    return 4;//TODO: Despues cambiar este valor a -1
            }

            set { Session["idOwn"] = value; }
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

            this.ucMainFilterReport.nameVisible = true;
            this.ucMainFilterReport.itemVisible = true;
            this.ucMainFilterReport.lotnumberVisible = true;
            this.ucMainFilterReport.sealnumberVisible = true;

            this.ucMainFilterReport.fabricationDateVisible = true;
            this.ucMainFilterReport.chkFilterFabricationDateChecked = true;
            this.ucMainFilterReport.expirationDateVisible = true;
            this.ucMainFilterReport.chkFilterExpirationDateChecked = true;
            this.ucMainFilterReport.nameLabel = "GTIN";

            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
        }
                
        private void LoadReport()
        {
            // Le indicamos la carpeta y el informe sin la extensión de este.
            this.rptViewStockByItem.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportTraceabilityGS1Path", string.Empty);

            // Y el servidor donde está alojado el informe.
            this.rptViewStockByItem.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

            //Credenciales
            int total = rptViewStockByItem.ServerReport.GetDataSources().Count;
            DataSourceCredentials[] permisos = new DataSourceCredentials[total];

            ReportDataSourceInfoCollection datasources = rptViewStockByItem.ServerReport.GetDataSources();
            for (int j = 0; j < total; j++)
            {
                permisos[j].Name = datasources[j].Name;
                permisos[j].UserId = "";
                permisos[j].Password = "";
            }
            rptViewStockByItem.ServerReport.SetDataSourceCredentials(permisos);

            //Capturo los valores de los filtros
            DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            TextBox txtSealNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterSealNumber");
            TextBox txtLotNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterLotNumber");
            TextBox txtGtinNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterName");
            TextBox txtItemCode = (TextBox)this.ucMainFilterReport.FindControl("txtFilterItem");
            TextBox txtFilterFabricationDate = (TextBox)this.ucMainFilterReport.FindControl("txtFilterFabricationDate");
            TextBox txtFilterExpirationDate = (TextBox)this.ucMainFilterReport.FindControl("txtFilterExpirationDate");
            CheckBox chkFilterFabricationDate = (CheckBox)this.ucMainFilterReport.FindControl("chkFilterFabricationDate");
            CheckBox chkFilterExpirationDate = (CheckBox)this.ucMainFilterReport.FindControl("chkFilterExpirationDate");

            string idWhs = ddlWhs.SelectedValue.ToString();
            string idOwn = ddlOwn.SelectedValue.ToString();
            string sealNumber = string.IsNullOrEmpty(txtSealNumber.Text.Trim()) ? "-1" : txtSealNumber.Text.Trim();
            string lote = string.IsNullOrEmpty(txtLotNumber.Text.Trim()) ? "-1" : txtLotNumber.Text.Trim();
            string gtin = string.IsNullOrEmpty(txtGtinNumber.Text.Trim()) ? "-1" : txtGtinNumber.Text.Trim();
            string itemCode = string.IsNullOrEmpty(txtItemCode.Text.Trim()) ? "-1" : txtItemCode.Text.Trim();
            string fabricationDate = string.Empty;
            string expirationDate = string.Empty;

            if (chkFilterFabricationDate.Checked)
            {
                fabricationDate = txtFilterFabricationDate.Text.Trim();
            }

            if (chkFilterExpirationDate.Checked)
            {
                expirationDate = txtFilterExpirationDate.Text.Trim();
            }

            // Creo una colección de parámetros de tipo ReportParameter 
            // para añadirlos al control ReportViewer.
            reportParameter = new List<ReportParameter>();
            reportParameter.Clear();

            // Añado los parámetros necesarios.
            reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
            reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
            reportParameter.Add(new ReportParameter("Item", itemCode, false));
            reportParameter.Add(new ReportParameter("Gtin", gtin, false));
            reportParameter.Add(new ReportParameter("SealNumber", sealNumber, false));
            reportParameter.Add(new ReportParameter("LotNumber", lote, false));

            if(!string.IsNullOrEmpty(fabricationDate))
                reportParameter.Add(new ReportParameter("FabricationDate", fabricationDate, false));

            if (!string.IsNullOrEmpty(expirationDate))
                reportParameter.Add(new ReportParameter("ExpirationDate", expirationDate, false));

            reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

            //DandoFormato a los margenes del informe
            rptViewStockByItem.Attributes.Add("style", "margin-bottom: 50px;");
            rptViewStockByItem.ShowPrintButton = false;
            rptViewStockByItem.ShowParameterPrompts = false;
            rptViewStockByItem.ShowPageNavigationControls = true;
            rptViewStockByItem.ShowToolBar = true;
            rptViewStockByItem.ShowReportBody = true;
            // Añado el/los parámetro/s al ReportViewer.
            this.rptViewStockByItem.ServerReport.SetParameters(reportParameter);

            rptViewStockByItem.DataBind();

            divReport.Visible = true;

            rptViewStockByItem.ServerReport.Refresh();

            CallJsGridViewHeader();
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}
