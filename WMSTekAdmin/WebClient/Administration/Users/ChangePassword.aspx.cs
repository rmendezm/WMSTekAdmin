using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Administration.Users
{
    public partial class ChangePassword :BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();

                Control pagecontrol;
                pagecontrol = new Control();

                pagecontrol = (Control)sender;
                base.Page_Init(sender, e);
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                    if (MiscUtils.Encrypt(this.txtPasswordActual.Text) == objUser.Password)
                    {
                        SaveChanges(objUser);
                    }
                    else
                    {
                        this.Master.ucDialog.ShowAlert(lblTitle.Text, lblError.Text, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                userViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];
            }

            if (!Page.IsPostBack)
            {
                this.Master.ucError.ClearError();
            }
            
            Master.ucMainFilter.Visible = false;
            Master.ucTaskBar.Visible = false;
        }

        protected void SaveChanges(User user)
        {
            var passPolicyErrors = PasswordPolicy.IsValid(txtPasswordNew.Text);

            if (passPolicyErrors != null && passPolicyErrors.Count > 0)
            {
                var errorMsg = string.Join("<br>", passPolicyErrors.ToArray());

                this.Master.ucDialog.ShowAlert(lblPoliciyPasswordTitle.Text, errorMsg, string.Empty);
                return;
            }

            //Se encripta la pass
            user.Password = MiscUtils.Encrypt(txtPasswordNew.Text);

            // Editar Password
            userViewDTO = iProfileMGR.UpdatePassword(user, context);

            if (userViewDTO.hasError())
            {
                this.Master.ucError.ShowError(userViewDTO.Errors);
            }
            else
            {
                this.Master.ucDialog.ShowAlert(lblTitle.Text, lblExito.Text, string.Empty);
            }
        }

        #endregion
    }
}
