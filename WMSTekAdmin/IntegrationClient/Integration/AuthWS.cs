using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace Binaria.WMSTek.IntegrationClient.Integration
{
    public class AuthWS : SoapHeader
    {
        public string userName { get; set; }
        public string password { get; set; }

        public bool IsValid()
        {
            var iProfileMGR = (IProfileMGR)InstanceFactory.getInstance(MiscUtils.ReadSetting("webClientPath", "")).getObject("profileMGR");

            if (iProfileMGR == null)
                return false;

            if (string.IsNullOrEmpty(this.userName) || string.IsNullOrEmpty(this.password))
                return false;

            var context = new ContextViewDTO();
            var getUser = iProfileMGR.GetUserWS(this.userName, this.password, context);

            if (getUser.hasError() || getUser.Entities.Count == 0)
                return false;

            var rolesByUser = iProfileMGR.RolesByUser(getUser.Entities.First().Id, context);

            if (rolesByUser.hasError() || rolesByUser.Entities.Count == 0)
                return false;

            var validRole = rolesByUser.Entities.Where(r => r.Name == Constants.ROL_WS).FirstOrDefault();

            return validRole != null;
        }
    }
}