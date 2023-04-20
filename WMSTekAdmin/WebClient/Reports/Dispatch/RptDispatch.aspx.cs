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

namespace Binaria.WMSTek.WebClient.Reports.Dispatch
{
    public partial class RptDispatch : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Dispatching.Dispatch> dispatchViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Dispatching.Dispatch>();
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

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.Visible = false;
            this.ucMainFilterReport.Visible = true;
            this.ucMainFilterReport.ownerVisible = true;
            this.ucMainFilterReport.documentVisible = true;
            
            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Carga la ruta, los parametros y carga el reporte
        /// </summary>
        private void LoadReport()
        {

            Binaria.WMSTek.Framework.Entities.Dispatching.OutboundOrder outbound = new Binaria.WMSTek.Framework.Entities.Dispatching.OutboundOrder();
            outbound.Owner = new Binaria.WMSTek.Framework.Entities.Warehousing.Owner();
            string idOwn = string.Empty;
            string docNumber = string.Empty;
            int IdDispatch = -1;

            //Capturo los valores de los filtros
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            TextBox txtDocNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDocumentNumber");

            idOwn = ddlOwn.SelectedValue.ToString();
            docNumber = txtDocNumber.Text.Trim();

            outbound.Owner.Id = Convert.ToInt32(idOwn);
            outbound.Number = docNumber;

            // carga todas las recepciones 
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            //consulta 
            dispatchViewDTO = iDispatchingMGR.GetOutboundDispatchHeader(context);

            if (dispatchViewDTO.Entities.Count > 0)
            {
                //Obtengo el despacho para pasarlo como parametro al subreporte
                IdDispatch = dispatchViewDTO.Entities[0].Id;

                // Le indicamos la carpeta y el informe sin la extensión de este.
                this.rptViewDispatch.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportDispatchPath", string.Empty);

                // Y el servidor donde está alojado el informe.
                this.rptViewDispatch.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

                //Credenciales
                int total = rptViewDispatch.ServerReport.GetDataSources().Count;
                DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                ReportDataSourceInfoCollection datasources = rptViewDispatch.ServerReport.GetDataSources();
                for (int j = 0; j < total; j++)
                {
                    permisos[j].Name = datasources[j].Name;
                    permisos[j].UserId = "";
                    permisos[j].Password = "";
                }
                rptViewDispatch.ServerReport.SetDataSourceCredentials(permisos);

                // Creo una colección de parámetros de tipo ReportParameter 
                // para añadirlos al control ReportViewer.
                reportParameter = new List<ReportParameter>();
                reportParameter.Clear();

                // Añado los parámetros necesarios.
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("OutboundNumber", docNumber, false));
                reportParameter.Add(new ReportParameter("IdDispatch", IdDispatch.ToString(), false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

                //DandoFormato a los margenes del informe
                rptViewDispatch.Attributes.Add("style", "margin-bottom: 50px;");

                rptViewDispatch.ShowPrintButton = false;

                // Añado el/los parámetro/s al ReportViewer.
                this.rptViewDispatch.ServerReport.SetParameters(reportParameter);

                rptViewDispatch.DataBind();

                divReport.Visible = true;

                rptViewDispatch.ServerReport.Refresh();

                CallJsGridViewHeader();
            }
            else
            {
                reportViewDTO.MessageStatus = new MessageStatusDTO();
                reportViewDTO.MessageStatus.Message = "Recepción no existe";
                divWarning.Visible = true;
                divReport.Visible = false;
            }
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}
