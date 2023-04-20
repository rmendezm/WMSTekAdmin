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
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities;

namespace Binaria.WMSTek.WebClient.Reports.ReceiptReport
{
    public partial class RptTotalReceipt : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Reception.Receipt> receiptViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Reception.Receipt>();
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
            divWarning.Visible = false;
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
            this.ucMainFilterReport.documentVisible = true;
            this.ucMainFilterReport.warehouseVisible = true;
            this.ucMainFilterReport.inboundTypeVisible = true;
            this.ucMainFilterReport.reqFilterInboundTypeEnabled = true;

            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void LoadReport()
        {
            Binaria.WMSTek.Framework.Entities.Reception.Receipt receipt = new Binaria.WMSTek.Framework.Entities.Reception.Receipt();
            receipt.InboundOrder = new InboundOrder();
            receipt.InboundOrder.Owner = new Binaria.WMSTek.Framework.Entities.Warehousing.Owner();
            string idOwn = string.Empty;
            string docNumber = string.Empty;
            int idWhs = -1;
            int idInboundType = -1;

            //Capturo los valores de los filtros
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            TextBox txtDocNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDocumentNumber");
            DropDownList ddlFilterWarehouse = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            DropDownList ddlFilterInboundType = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterInboundType");

            idOwn = ddlOwn.SelectedValue.ToString();
            docNumber = txtDocNumber.Text.Trim();

            if (ddlFilterWarehouse != null && ddlFilterWarehouse.SelectedValue != null)
                idWhs = int.Parse(ddlFilterWarehouse.SelectedValue);

            if (ddlFilterInboundType != null && ddlFilterInboundType.SelectedValue != null)
                idInboundType = int.Parse(ddlFilterInboundType.SelectedValue);

            receipt.InboundOrder.Owner.Id = Convert.ToInt32(idOwn);
            receipt.InboundOrder.Number = docNumber;
            receipt.InboundOrder.Warehouse = new Framework.Entities.Layout.Warehouse() { Id = idWhs };
            receipt.InboundOrder.InboundType = new InboundType() { Id = idInboundType };

            receiptViewDTO = iReceptionMGR.GetTotalReceiptReportConsult(receipt, context);

            if (receiptViewDTO.Entities.Count > 0)
            {

                int IdInboundOrder = receiptViewDTO.Entities[0].InboundOrder.Id;

                // Le indicamos la carpeta y el informe sin la extensión de este.
                this.rptViewTotalReceipt.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportTotalReceiptPath", string.Empty);

                // Y el servidor donde está alojado el informe.
                this.rptViewTotalReceipt.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

                //Credenciales
                int total = rptViewTotalReceipt.ServerReport.GetDataSources().Count;
                DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                ReportDataSourceInfoCollection datasources = rptViewTotalReceipt.ServerReport.GetDataSources();
                for (int j = 0; j < total; j++)
                {
                    permisos[j].Name = datasources[j].Name;
                    permisos[j].UserId = "";
                    permisos[j].Password = "";
                }
                rptViewTotalReceipt.ServerReport.SetDataSourceCredentials(permisos);

                // Creo una colección de parámetros de tipo ReportParameter 
                // para añadirlos al control ReportViewer.
                reportParameter = new List<ReportParameter>();
                reportParameter.Clear();

                // Añado los parámetros necesarios.
                reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                reportParameter.Add(new ReportParameter("InboundNumber", docNumber, false));
                reportParameter.Add(new ReportParameter("IdInboundOrder", IdInboundOrder.ToString(), false));
                reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

                //DandoFormato a los margenes del informe
                rptViewTotalReceipt.Attributes.Add("style", "margin-bottom: 50px;");

                // Añado el/los parámetro/s al ReportViewer.
                this.rptViewTotalReceipt.ServerReport.SetParameters(reportParameter);

                rptViewTotalReceipt.DataBind();

                divReport.Visible = true;
                rptViewTotalReceipt.ShowPrintButton = false;
                rptViewTotalReceipt.ServerReport.Refresh();

                CallJsGridViewHeader();
            }
            else
            {
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
