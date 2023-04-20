using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.DataTransfer.Base;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class DialogBoxContent : System.Web.UI.UserControl
    {
        #region "Declaración de Variables"

        public event EventHandler BtnOkClick;
        public event EventHandler BtnCancelClick;
        public event EventHandler ImgCloseClick;
        
        public string Caller
        {
            get { return caller; }
            set { caller = value; }
        }

        private string caller
        {
            get 
            {
                if (ViewState["caller"] != null)
                    return ViewState["caller"].ToString();
                else
                    return string.Empty;
            }
            set { ViewState["caller"] = value; }
        }

        public bool linkPageVisible
        {
            get { return this.linkPage.Visible; }
            set { this.linkPage.Visible = value; }
        }

        public string linkNavigationUrl
        {
            get { return this.linkPage.NavigateUrl; }
            set { this.linkPage.NavigateUrl = value; }
        }

        public string linkText
        {
            get { return this.linkPage.Text; }
            set { this.linkPage.Text = value; }
        }

        #endregion

        #region "Eventos"

        protected void btnOk_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            modalPopUpDialog.Hide();

            // Dispara el evento 'BtnOkClick' que será capturado por las páginas que implementen este control
            OnBtnOkClick(e);
        }

        protected void imgClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            modalPopUpDialog.Hide();

            // Dispara el evento 'BtnSessionClick' que será capturado por las páginas que implementen este control
            OnImgClose_Click(e);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            modalPopUpDialog.Hide();

            // Dispara el evento 'BtnCancelClick' que será capturado por las páginas que implementen este control
            OnBtnCancelClick(e);
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            modalPopUpDialog.Hide();

            // Dispara el evento 'BtnOkClick' que será capturado por las páginas que implementen este control
            if (caller != string.Empty) OnBtnOkClick(e);
        }

        

        #endregion

        #region "Métodos"

        public void ClearDialog()
        {
            this.Visible = false;
        }

        public void ShowAlert(string title, string message, string caller)
        {
            this.caller = caller;

            this.divAlert.Visible = true;
            this.divConfirm.Visible = false;
            this.divDialogMessage2.Visible = false;
            this.imgClose.Visible = false;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;

            this.Visible = true;
            modalPopUpDialog.Show();
            this.btnClose.Focus();
        }

        public void ShowSession(string title, string message, string message2, string caller)
        {
            this.caller = caller;

            this.divAlert.Visible = false;
            this.divConfirm.Visible = false;
            this.divDialogMessage2.Visible = true;
            this.imgClose.Visible = false;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;
            this.divDialogMessage2.InnerHtml = message2;

            this.Visible = true;
            modalPopUpDialog.Show();
            
            this.btnClose.Focus();
        }

        public void ShowConfirm(string title, string message, string caller)
        {
            this.caller = caller;

            this.divAlert.Visible = false;
            this.divConfirm.Visible = true;
            this.divDialogMessage2.Visible = false;
            this.imgClose.Visible = false;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;

            this.Visible = true;
            modalPopUpDialog.Show();
        }

        protected void OnBtnOkClick(EventArgs e)
        {
            if (BtnOkClick != null)
            {
                BtnOkClick(this, e);
            }
        }

        protected void OnImgClose_Click(EventArgs e)
        {
            if (ImgCloseClick != null)
            {
                ImgCloseClick(this, e);
            }
        }

        protected void OnBtnCancelClick(EventArgs e)
        {
            if (BtnCancelClick != null)
            {
                BtnCancelClick(this, e);
            }
        }
        
        #endregion
    }
}


