using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;

namespace Binaria.WMSTek.WebClient.Reports
{
    public partial class RptGenericPrint : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.QueryString.Get("TypeReportEnum")))
                {
                    string p_TypeReport = Request.QueryString.Get("TypeReportEnum");
                    String[] type = Request.QueryString.AllKeys;

                    List<ReportParameter> reportParameter;

                    this.rptViewPrint.ServerReport.ReportPath = MiscUtils.ReadSetting(p_TypeReport, string.Empty);
                    this.rptViewPrint.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

                    //Credenciales
                    int total = rptViewPrint.ServerReport.GetDataSources().Count;
                    DataSourceCredentials[] permisos = new DataSourceCredentials[total];

                    ReportDataSourceInfoCollection datasources = rptViewPrint.ServerReport.GetDataSources();
                    for (int j = 0; j < total; j++)
                    {
                        permisos[j].Name = datasources[j].Name;
                        permisos[j].UserId = "";
                        permisos[j].Password = "";
                    }
                    rptViewPrint.ServerReport.SetDataSourceCredentials(permisos);

                    reportParameter = new List<ReportParameter>();
                    reportParameter.Clear();

                    foreach (String key in type)
                    {
                        if (!key.Equals("TypeReportEnum"))
                        {
                            String valorQuery = Request.QueryString.Get(key);
                            reportParameter.Add(new ReportParameter(key, valorQuery, false));
                        }
                    }

                    reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

                    rptViewPrint.Attributes.Add("style", "margin-bottom: 50px;");
                    rptViewPrint.ShowPrintButton = true;
                    // Añado el/los parámetro/s al ReportViewer.
                    this.rptViewPrint.ServerReport.SetParameters(reportParameter);

                    rptViewPrint.DataBind();

                    divReport.Visible = true;

                    rptViewPrint.ServerReport.Refresh();
                }
            }


        }
    }
}
