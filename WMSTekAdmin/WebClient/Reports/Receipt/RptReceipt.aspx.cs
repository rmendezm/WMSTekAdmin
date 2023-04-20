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


namespace Binaria.WMSTek.WebClient.Reports.Receipt
{
    public partial class RptReceipt : BasePage
    {
        GenericViewDTO<Binaria.WMSTek.Framework.Entities.Reception.Receipt> receiptViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Reception.Receipt>();
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
                    this.ucMainFilterReport.emptyRowLabelText = "Seleccione";
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
            this.ucMainFilterReport.ownerVisible = true;
            this.ucMainFilterReport.documentVisible = true;
            this.ucMainFilterReport.codeVisible = true;
            this.ucMainFilterReport.descriptionVisible = true;
            this.ucMainFilterReport.descriptionLabel = this.lblDescription.Text;
            this.ucMainFilterReport.codeLabel = this.lblCodeVirtual.Text;
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
            string idReceipt = string.Empty;
            string docReference = string.Empty;
            string IdInboundOrder = string.Empty;

            //Capturo los valores de los filtros
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            TextBox txtDocNumber = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDocumentNumber");
            TextBox txtIdReceipt = (TextBox)this.ucMainFilterReport.FindControl("txtFilterCode");
            TextBox txtDocReference = (TextBox)this.ucMainFilterReport.FindControl("txtFilterDescription");

            //lblCode.Text = this.lblCodeVirtual.Text;
            idOwn = ddlOwn.SelectedValue.ToString();
            docNumber = txtDocNumber.Text.Trim();
            idReceipt = txtIdReceipt.Text.Trim();
            //valida qie no sea "blanco"
            if (idReceipt == string.Empty) idReceipt = "-1";
            //valida que sea numerico o muestra mensaje error
            if (!MiscUtils.IsNumeric(idReceipt))
                this.lblError2.Visible = true;
            else
            {
                this.lblError2.Visible = false;
                
                docReference = txtDocReference.Text.Trim() == string.Empty? "-1": txtDocReference.Text.Trim();

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
                    //Obtengo el id de doc de entrada
                    IdInboundOrder = receiptViewDTO.Entities[0].InboundOrder.Id.ToString();

                    // Le indicamos la carpeta y el informe sin la extensión de este.
                    this.rptViewReceipt.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportReceiptPath", string.Empty);

                    // Y el servidor donde está alojado el informe.
                    this.rptViewReceipt.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

                    //Credenciales
                    int total = rptViewReceipt.ServerReport.GetDataSources().Count;
                    DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                    ReportDataSourceInfoCollection datasources = rptViewReceipt.ServerReport.GetDataSources();
                    for (int j = 0; j < total; j++)
                    {
                        permisos[j].Name = datasources[j].Name;
                        permisos[j].UserId = "";
                        permisos[j].Password = "";
                    }
                    rptViewReceipt.ServerReport.SetDataSourceCredentials(permisos);

                    // Creo una colección de parámetros de tipo ReportParameter 
                    // para añadirlos al control ReportViewer.
                    reportParameter = new List<ReportParameter>();
                    reportParameter.Clear();

                    // Añado los parámetros necesarios.
                    reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
                    reportParameter.Add(new ReportParameter("InboundNumber", docNumber, false));
                    reportParameter.Add(new ReportParameter("IdReceipt", idReceipt, false));
                    reportParameter.Add(new ReportParameter("ReferenceDoc", docReference, false));
                    reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));
                    reportParameter.Add(new ReportParameter("IdInboundOrder", IdInboundOrder, false));
                    
                    //DandoFormato a los margenes del informe
                    rptViewReceipt.Attributes.Add("style", "margin-bottom: 50px;");

                    // Añado el/los parámetro/s al ReportViewer.
                    this.rptViewReceipt.ServerReport.SetParameters(reportParameter);
                    rptViewReceipt.ShowPrintButton = false;
                    rptViewReceipt.DataBind();

                    divReport.Visible = true;

                    rptViewReceipt.ServerReport.Refresh();

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
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}
