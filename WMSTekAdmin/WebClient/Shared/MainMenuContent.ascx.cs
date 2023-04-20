using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class MainMenuContent : BaseUserControl
    {
        protected override void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                LoadUserMenu();
        }

        private void LoadUserMenu()
        {
            if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                User objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                if (objUser.Menu != null)
                {
                    this.xmlMenu.Data = GetMenuXML(objUser.Menu);
                    this.xmlMenu.DataBind();
                    this.mnuMain.DataSource = this.xmlMenu;
                    this.mnuMain.DataBind();
                }
                else
                { 
                    // TODO: mostrar la pagina de errores por defecto diciendo que el sitio se encuentra 
                    //temporalmente fuera de linea, contactese con su supervisor.
                }
            }
        }

        /// <summary>
        /// Obtiene el dataset del menu de usuario, a partir de una List<MenuItem>
        /// </summary>
        /// <param name="menuViewDTO"></param>
        /// <returns></returns>
        private string GetMenuXML(List<Binaria.WMSTek.Framework.Entities.Profile.MenuItem> lstMenu)
        {
            //ErrorDTO errorDTO = new ErrorDTO();
            DataSet dsMenu = new DataSet();

            dsMenu.Tables.Add("Menu");
            dsMenu.Tables["Menu"].Columns.Add("IdMenu");
            dsMenu.Tables["Menu"].Columns.Add("IDParentMenu");
            dsMenu.Tables["Menu"].Columns.Add("Name");
            dsMenu.Tables["Menu"].Columns.Add("TextValue");
            dsMenu.Tables["Menu"].Columns.Add("ShortTitle");
            dsMenu.Tables["Menu"].Columns.Add("WinPath");
            dsMenu.Tables["Menu"].Columns.Add("AspxPage");
            dsMenu.Tables["Menu"].Columns.Add("MaxOpenedPages");
            dsMenu.Tables["Menu"].Columns.Add("MaxOpenedPagesMessage");

            foreach (Binaria.WMSTek.Framework.Entities.Profile.MenuItem menuItem in lstMenu)
            {
                DataRow row = dsMenu.Tables["Menu"].NewRow();

                row["IdMenu"] = menuItem.Id;
                row["IDParentMenu"] = menuItem.IDParentMenuItem;
                row["Name"] = menuItem.Name;
                row["TextValue"] = menuItem.TextValue;
                row["ShortTitle"] = menuItem.ShortTitle;
                row["WinPath"] = menuItem.WinPath;
                row["AspxPage"] = menuItem.AspxPage;
                row["MaxOpenedPages"] = menuItem.MaxOpenedPages;
                row["MaxOpenedPagesMessage"] = menuItem.MaxOpenedPagesMessage;

                dsMenu.Tables["Menu"].Rows.Add(row);
            }

            dsMenu.DataSetName = "Menus";
            DataRelation relation = new DataRelation("ParentChild",
                    dsMenu.Tables["Menu"].Columns["IdMenu"],
                    dsMenu.Tables["Menu"].Columns["IDParentMenu"], false);

            relation.Nested = true;
            dsMenu.Relations.Add(relation);

            return dsMenu.GetXml();
        }
    }
}