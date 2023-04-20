using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Net;
using System.Security.Principal;

namespace Binaria.WMSTek.WebClient.Reports.Utils
{
    public class CustomReportCredentials : IReportServerCredentials
    {
        // local variable for network credential.
        private string _UserName;
        private string _PassWord;
        private string _DomainName;
        private WindowsIdentity _ImpersonationUser;

        public CustomReportCredentials(string UserName, string PassWord, string DomainName)
        {
            _UserName = UserName;
            _PassWord = PassWord;
            _DomainName = DomainName;
            // _ImpersonationUser = ImpersonationUser;
        }
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null; // not use ImpersonationUser
            }
        }
        public ICredentials NetworkCredentials
        {
            get
            {

                // use NetworkCredentials
                return new NetworkCredential(_UserName, _PassWord, _DomainName);
            }
        }
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {

            // not use FormsCredentials unless you have implements a custom autentication.
            authCookie = null;
            user = password = authority = null;
            return false;
        }

    }
}
