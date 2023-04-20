using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils;
using System.Text;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class GenericError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblTitle.Text = MiscUtils.ReadSetting("defaultErrorTitle", "Se ha producido un error.");
            lblMessage.Text = MiscUtils.ReadSetting("defaultErrorMessage", "El sistema detectó una excepción no identificada.");

            /*
            String[] solutions = MiscUtils.ReadSetting("defaultErrorSolutions", "Volver al inicio.").Split('|');

            if (solutions != null && solutions[0] != string.Empty)
            {
                StringBuilder sb = new StringBuilder();

                foreach (string solution in solutions)
                {
                    sb.Append("<li>");
                    sb.Append(solution);
                    sb.Append("</li>");
                }

                divErrorSolution.InnerHtml = sb.ToString();
            }
            */

            if (Session["Error"] != null)
            {
                Exception ex = (Exception)Session["Error"];

                lblExMessage.Text = ex.GetBaseException().Message;
                lblExSource.Text = ex.GetBaseException().Source;
                lblExStackTrace.Text = ex.GetBaseException().StackTrace;
            }
            else
                lblExMessage.Text = lblNoInfo.Text;
        }
    }
}
