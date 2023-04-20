using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Billing.Consult
{
    public partial class BillingDownload : System.Web.UI.Page
    {
        public string currentNameFile
        {
            get
            {
                if (ViewState["fileName"] != null && ViewState["fileName"].ToString() != string.Empty)
                    return (string)ViewState["fileName"];
                else
                    return "";
            }
            set { ViewState["fileName"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            if (Request.QueryString["FileName"] != string.Empty)
            {
                currentNameFile = Request.QueryString["FileName"].Trim();
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                MemoryStream memoryStream = (MemoryStream)Session[WMSTekSessions.Shared.BillingDownloadFile];

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=\"" + currentNameFile + "\"");
                memoryStream.WriteTo(Response.OutputStream);
                Response.End();
            }
            catch (System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }         
          
        }
    }
}