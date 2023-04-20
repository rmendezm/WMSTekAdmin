using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;


namespace Binaria.WMSTek.WebClient
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new AssemblyResourceProvider());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session != null)
            {
                Exception ex = Server.GetLastError();
                HttpContext.Current.Session["Error"] = ex;//.ToString();
                Response.Redirect("~/Shared/GenericError.aspx", false);
                Server.ClearError();
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}