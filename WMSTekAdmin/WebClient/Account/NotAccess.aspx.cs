using System;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Account
{
    public partial class NotAccess : BasePage
    {
        /// <summary>
        /// Avisa al usuario que no tiene acceso a la pagina solicitada, y redirecciona a la pagina de inicio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Page_Load(object sender, EventArgs e)
        {
            modalPopUp.Show();
        }
    }
}
