using System;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils;
using System.Web;
using System.Web.UI;

namespace Binaria.WMSTek.WebClient.Account
{
    public partial class Logout : BasePage// : System.Web.UI.Page
    {
        /// <summary>
        /// Avisa al usuario que la sesion ha expirado y redirecciona a la pagina de inicio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Master.ucDialog.ShowAlert(lblTitle.Text, lblLogout.Text, "logout");                
            }
            ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDivImgBack();", true); 
        }

        protected override void Page_Init(object sender, EventArgs e)
        {
            this.Master.ucDialog.BtnOkClick += new EventHandler(btnDialogOk_Click);
            //this.Master.ucDialog.ImgCloseClick += new EventHandler(btnDialogOk_Click);

            this.Master.ucTaskBar.Visible = false;
            this.Master.ucMainFilter.Visible = false;
        }

        /// <summary>
        /// Respuesta desde el mensaje de alerta
        /// </summary>
        protected void btnDialogOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "Success", "RedirectPage();", true);   
          
        }

        protected void btnDialogSession_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/Account/Session.aspx");           
        }
    }
}
