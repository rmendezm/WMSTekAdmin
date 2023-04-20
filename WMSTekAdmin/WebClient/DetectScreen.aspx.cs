using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient
{
    public partial class DetectScreen : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["action"] != null)
            {
                Session["screenX"] = Request.QueryString["screenX"].ToString();
                Session["screenY"] = Request.QueryString["screenY"].ToString();
                Response.Redirect("Default.aspx");                      
            }
            else
            {
                InitializeSession(); 
            }
        }

        protected void InitializeSession()
        {
            this.Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Application["MessageSession"] = "";
            RemoveCookiesForSession();
        }
        protected void RemoveCookiesForSession()
        {
            if (Request.Cookies[WMSTekSessions.Global.NETSessionId] != null)
            {
                Response.Cookies[WMSTekSessions.Global.NETSessionId].Value = string.Empty;
                Response.Cookies[WMSTekSessions.Global.NETSessionId].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies[WMSTekSessions.Global.AuthToken] != null)
            {
                Response.Cookies[WMSTekSessions.Global.AuthToken].Value = string.Empty;
                Response.Cookies[WMSTekSessions.Global.AuthToken].Expires = DateTime.Now.AddMonths(-20);
            }
        }
    }
}
