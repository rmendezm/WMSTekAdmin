using System;
using System.Text;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils.Enums;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class ErrorContent : System.Web.UI.UserControl
    {
        public event EventHandler BtnCloseErrorClick;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OnBtnCloseErrorClick(EventArgs e)
        {
            if (BtnCloseErrorClick != null)
            {
                BtnCloseErrorClick(this, e);
            }
        } 

        public void ClearError()
        {
            this.Visible = false;
        }

        public void ClearValues()
        {
            lblTitle.Text = string.Empty;
            lblMessage.Text = string.Empty;
            divErrorSolution.InnerHtml = string.Empty;
            lblTime.Text = string.Empty;
            lblSeverity.Text = string.Empty;
            lblCodePrefix.Text = string.Empty;
            lblClassPrefix.Text = string.Empty;
            lblMethodPrefix.Text = string.Empty;
            lblOriginalMessage.Text = string.Empty;
        }

        public void ShowError(ErrorDTO error)
        {
            // Limpia mensaje anterior;
            ClearValues();

            // Información mostrada al usuario

            // Muestra un icono diferente segun el Error Level
            switch (error.Level)
            {
                case ErrorLevel.Info:
                    imgErrorLevel.ImageUrl = "~/WebResources/Images/Buttons/AlarmMessage/icon_info.png";
                    break;
                case ErrorLevel.Warning:
                    imgErrorLevel.ImageUrl = "~/WebResources/Images/Buttons/AlarmMessage/icon_warning.png";
                    break;
                case ErrorLevel.Error:
                    imgErrorLevel.ImageUrl = "~/WebResources/Images/Buttons/AlarmMessage/icon_error.png";
                    break;
                default:
                    imgErrorLevel.ImageUrl = "~/WebResources/Images/Buttons/AlarmMessage/icon_error.png";
                    break;
            }

            if (!String.IsNullOrEmpty(error.Title)) lblTitle.Text = error.Title;
            if (!String.IsNullOrEmpty(error.Message)) lblMessage.Text = error.Message;

            if (error.Solutions != null)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<ul>");
                foreach (string solution in error.Solutions)
                {
                    sb.Append("<li>");
                    sb.Append(solution);
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
                divErrorSolution.InnerHtml = sb.ToString();
            }

            // Información Adicional (al seleccionar 'Ver Detalles')
            if (!String.IsNullOrEmpty(error.Time)) lblTime.Text = lblTimePrefix.Text +  error.Time;
            if (!String.IsNullOrEmpty(error.Severity)) lblSeverity.Text = lblSeverityPrefix.Text + error.Severity;
            if (!String.IsNullOrEmpty(error.Code)) lblCode.Text = lblCodePrefix.Text + error.Code;
            if (!String.IsNullOrEmpty(error.ClassFullName)) lblClass.Text = lblClassPrefix.Text + error.ClassFullName;
            if (!String.IsNullOrEmpty(error.Method)) lblMethod.Text = lblMethodPrefix.Text + error.Method;
            if (!String.IsNullOrEmpty(error.OriginalMessage)) lblOriginalMessage.Text = lblOriginalMessagePrefix.Text + error.OriginalMessage;

            this.Visible = true;
            modalPopUpError.Show();
        }

        protected void btnCloseError_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            modalPopUpError.Hide();

            // Dispara el evento 'BtnOkClick' que será capturado por las páginas que implementen este control
            OnBtnCloseErrorClick(e);
        }
    }
}


