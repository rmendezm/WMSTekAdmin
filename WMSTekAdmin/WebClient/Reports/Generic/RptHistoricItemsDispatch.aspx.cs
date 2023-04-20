using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
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
using Binaria.WMSTek.Framework.Entities.DTO;


namespace Binaria.WMSTek.WebClient.Reports
{
    public partial class RptHistoricItemsDispatch : BasePage
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
                    return -1;
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
                    Initialize();
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
           
            this.ucMainFilterReport.warehouseVisible = true;
            this.ucMainFilterReport.ownerUserVisible = true;
            this.ucMainFilterReport.ownerNotIncludeAll = true;
            //this.ucMainFilterReport.ownerVisible = true;
            this.ucMainFilterReport.YearVisible = true;

            this.ucMainFilterReport.Initialize(init, refresh);
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwnerUser");
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void LoadReport()
        {
            string idWhs = string.Empty;
            string idOwn = string.Empty;
            string year = string.Empty;
            string NomWhs = string.Empty;
            string NomOwn = string.Empty;

            //Capturo los valores de los filtros
            DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwnerUser");
            TextBox txtYear = (TextBox)this.ucMainFilterReport.FindControl("txtFilterYear");

            idWhs = ddlWhs.SelectedValue.ToString();
            idOwn = ddlOwn.SelectedValue.ToString();
            year = txtYear.Text.Trim();

            NomWhs = ddlWhs.SelectedItem.ToString();
            NomOwn = ddlOwn.SelectedItem.ToString();

            //valida que no sea "blanco"
            if (year == string.Empty)
            {
                year = "-1";
            }

            if (int.Parse(year) < 2012 || int.Parse(year) > 2030)
            {
                this.lblError2.Visible = true;
                reportViewDTO.MessageStatus = new MessageStatusDTO();
                reportViewDTO.MessageStatus.Message = "Año debe estar entre 2012 y 2030";
                this.lblError2.Text = "Año debe estar entre 2012 y 2030";
                divReport.Visible = false;
            }
            else
                //valida que sea numerico o muestra mensaje error
                if (!MiscUtils.IsNumeric(year))
                {
                    this.lblError2.Visible = true;

                    reportViewDTO.MessageStatus = new MessageStatusDTO();
                    reportViewDTO.MessageStatus.Message = "Año debe ser Numérico";
                    this.lblError2.Text = "Año debe ser Numérico";
                    divReport.Visible = false;
                }
                else
                {
                    this.lblError2.Visible = false;

                    // Le indicamos la carpeta y el informe sin la extensión de este.
                    this.rptViewReceiptHistoric.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportHistoricItemsDicpatchPath", string.Empty);

                    // Y el servidor donde está alojado el informe.
                    this.rptViewReceiptHistoric.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

                    //Credenciales
                    int total = rptViewReceiptHistoric.ServerReport.GetDataSources().Count;
                    DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                    ReportDataSourceInfoCollection datasources = rptViewReceiptHistoric.ServerReport.GetDataSources();
                    for (int j = 0; j < total; j++)
                    {
                        permisos[j].Name = datasources[j].Name;
                        permisos[j].UserId = "";
                        permisos[j].Password = "";
                    }
                    rptViewReceiptHistoric.ServerReport.SetDataSourceCredentials(permisos);

                    // Creo una colección de parámetros de tipo ReportParameter 
                    // para añadirlos al control ReportViewer.
                    reportParameter = new List<ReportParameter>();
                    reportParameter.Clear();

                    // Añado los parámetros necesarios.
                    reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
                    reportParameter.Add(new ReportParameter("IdOwner", idOwn, false));
                    reportParameter.Add(new ReportParameter("Year", year, false));
                    reportParameter.Add(new ReportParameter("NomWhs", NomWhs, false));
                    reportParameter.Add(new ReportParameter("NomOwn", NomOwn, false));
                    reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
                    
                    //DandoFormato a los margenes del informe
                    rptViewReceiptHistoric.Attributes.Add("style", "margin-bottom: 50px;");

                    // Añado el/los parámetro/s al ReportViewer.
                    this.rptViewReceiptHistoric.ServerReport.SetParameters(reportParameter);
                    rptViewReceiptHistoric.ShowPrintButton = true;
                    rptViewReceiptHistoric.ShowFindControls = false;
                    rptViewReceiptHistoric.DataBind();

                    divReport.Visible = true;

                    rptViewReceiptHistoric.ServerReport.Refresh();

                    CallJsGridViewHeader();

                }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}