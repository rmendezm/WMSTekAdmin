using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Binaria.WMSTek.WebClient
{
    /// <summary>
    /// Descripción breve de DownloadDocument
    /// </summary>
    public class DownloadDocument : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string fileName = context.Request.QueryString["filename"];
                string attachment = "attachment; filename=" + fileName;
                byte[] data = (byte[])context.Session["DataRpt"];

                if (data != null)
                {
                    context.Response.AddHeader("content-disposition", attachment);
                    context.Response.ContentType = contentType(Path.GetExtension(fileName));
                    context.Response.BinaryWrite(data);
                    context.Response.Flush();
                    context.Response.End();

                    context.Session["DataRpt"] = null;
                }
            }
            catch (Exception ex)
            {
                LogManager.getInstance().exceptionMessage("Binaria.WMSTek.WebClient.DownloadDocument", "EXCEPTION al descargar archivo en handler " + ex.ToString());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string contentType(string extension)
        {
            string ext = extension.Replace(".", "").ToLower();

            if (ext.Equals("pdf"))
            {
                return "application/pdf";
            }
            else
            {
                return "text/html";
            }
        }
    }
}