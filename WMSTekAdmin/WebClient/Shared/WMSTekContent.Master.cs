using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Shared
{
    public partial class WMSTekContent : System.Web.UI.MasterPage
    {
        public string ClosePageFunction;

        public string EmptyGridText
        {
            get { return this.lblEmptyGridText.Text; } 
        }

        public string NoDetailsText
        {
            get { return this.lblNoDetails.Text; }
        }

        public string EmptyRowText
        {
            get { return this.lblEmptyRow.Text; }
        }

        public string AllRowsText
        {
            get { return this.lblAllRows.Text; }
        }

        public string YesText
        {
            get { return this.lblYes.Text; }
        }

        public string NoText
        {
            get { return this.lblNo.Text; }
        }

        public string AllText
        {
            get { return this.lblAll.Text; }
        }

        public string TotalText
        {
            get { return this.lblTotal.Text; }
        }

        public string AsociadasText
        {
            get { return this.lblAsociadas.Text; }
        }

        public string SinAsociarText
        {
            get { return this.lblSinAsociar.Text; }
        }
                
        public ScriptManager SmMasterContent
        {
            get { return this.smMasterContent; }
        }

        public ErrorContent ucError
        {
            get { return this.ucContentError; }
        }

        public DialogBoxContent ucDialog
        {
            get { return this.ucContentDialog; }
        }

        public TaskBarContent ucTaskBar
        {
            get { return this.ucTaskBarContent; }
        }

        public MainFilterContent ucMainFilter
        {
            get { return this.ucMainFilterContent; }
        }

              

        public UpdatePanel upStatusBar
        {
            get { return this.upStatusBarContent; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadUserData();
            string script = "<script type=text/javascript src=" + ResolveUrl("~/WebResources/Javascript/Utils.js") + "></script>";
            Page.RegisterStartupScript("utils", script);
            //ClosePageFunction = Page.ClientScript.GetPostBackEventReference(this, "MyCustomArgument");
        }

        private void LoadUserData()
        {
            var basePage = new BasePage();
            if (basePage.ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {

                User objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                this.lblUserName.Text = "Hola " + objUser.FirstName + " " + objUser.LastName;
                //this.lblUserName2.Text = objUser.FirstName + " " + objUser.LastName;
                rpRoles.DataSource = objUser.Roles.Where(role => role.RoleModule.IdModule == 1).ToList();
                rpRoles.DataBind();

                // Carga lista de Warehouses del Usuario en sesión y selecciona el Warehouse por defecto
                basePage.LoadUserWarehouses(this.ddlWarehouse, lblEmptyRow.Text, "-1", false);
                basePage.SelectDefaultWarehouse(ddlWarehouse);

                // Carga lista de Owners del Usuario en sesión y selecciona el Owner por defecto
                basePage.LoadUserOwners(this.ddlOwner, lblEmptyRow.Text, "-1", false, string.Empty, false);
                basePage.SelectDefaultOwner(ddlOwner);

                // Opción 'Mantener valores del Filtro entre consultas'
                //int index = Convert.ToInt16(CfgParameterName.KeepFilterValues);
                ////this.chkKeepFilter.Checked = Convert.ToBoolean(Convert.ToInt16(GetCfgParameter(Framework.Utils.Enums.CfgParameterName.KeepFilterValues.ToString())));
                ////context.SessionInfo.FilterKeep = this.chkKeepFilter.Checked;

                //LoadPopulateList();
                //LoadCharts();

                ScriptManager.RegisterStartupScript(Page, GetType(), "GetMenuByUser", "GetMenuByUser(" + objUser.Id + ");", true);
            }
        }

        /// <summary>
        /// Cambia Warehouse a utilizar en la sesión (no cambia Warehouse por defecto configurado para el usuario)
        /// </summary>
        protected void ddlWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            var context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];

            int idWhs = int.Parse((string)Session[WMSTekSessions.OptionMenuSelected.SelectedIdWhs]); 

            if (context.SessionInfo.User.Warehouses != null)
            {
                foreach (Warehouse warehouse in context.SessionInfo.User.Warehouses)
                {
                    if (warehouse.Id == idWhs)
                    {
                        context.SessionInfo.Warehouse = warehouse;
                        var basePage = new BasePage();
                        basePage.SelectDefaultWarehouse(ddlWarehouse);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Cambia Owner a utilizar en la sesión (no cambia Owner por defecto configurado para el usuario)
        /// </summary>
        protected void ddlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            var context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];

            int idOwn = int.Parse((string)Session[WMSTekSessions.OptionMenuSelected.SelectedIdOwn]);

            if (context.SessionInfo.User.Owners != null)
            {
                foreach (Owner owner in context.SessionInfo.User.Owners)
                {
                    if (owner.Id == idOwn)
                    {
                        context.SessionInfo.Owner = owner;
                        var basePage = new BasePage();
                        basePage.SelectDefaultOwner(ddlOwner);
                        break;
                    }
                }
            }
        }


        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    ucDialog.BtnOkClick += new EventHandler(btnDialogOkNew_Click);

        //}

        //protected void btnDialogOkNew_Click(object sender, EventArgs e)
        //{
        //    //Response.Redirect("~/DetectScreen.aspx");
        //    Response.Redirect("~/DetectScreen.aspx");
        //    //Response.End();
        //}

        //protected void smMasterContent_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        //{
        //    if (e.Exception.Data["ExtraInfo"] != null)
        //    {
        //        smMasterContent.AsyncPostBackErrorMessage =
        //            e.Exception.Message +
        //            e.Exception.Data["ExtraInfo"].ToString();
        //    }
        //    else
        //    {
        //        smMasterContent.AsyncPostBackErrorMessage = e.Exception.ToString();
        //        //"An unspecified error occurred.";
        //    }
        //}
    }
}
