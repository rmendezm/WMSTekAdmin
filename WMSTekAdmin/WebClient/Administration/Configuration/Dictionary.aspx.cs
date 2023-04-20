using System;
using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Configuration
{
    public partial class Dictionary : System.Web.UI.Page
    {
        private ArrayList aspxList = new ArrayList();
        private ArrayList ascxList = new ArrayList();
        ContextViewDTO context = new ContextViewDTO();

        protected ILanguageMGR iLanguageMGR;

        protected void Page_Load(object sender, EventArgs e)
        {
            var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];

            iLanguageMGR = (ILanguageMGR)objectInstances.getObject("languageMGR");

            InitializeContext();
        }

        private void InitializeContext()
        {
            if (ValidateSession(WMSTekSessions.Global.Context))
                context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
        }

        public bool ValidateSession(string sessionName)
        {
            if (Session[sessionName] != null && Session[sessionName].ToString() != string.Empty)
                return true;
            else
                return false;
        }

        protected void DisableDictionary()
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.I18N.Dictionary> dictionaryViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.I18N.Dictionary>();

            dictionaryViewDTO = iLanguageMGR.DisableDictionary(context);

            if (dictionaryViewDTO.hasError())
            {
                // TODO: agregar panel para mostrar error
                /*
                lblErrorMessage.Text = dictionaryViewDTO.Errors.Message;
                divErrorMessage.Visible = true;
                 */
            }
        }

        protected void LoopUserControls()
        {
            try
            {
                foreach (string uc in ascxList)
                {
                    UpdateUcDictionary(uc);
                }
            }
            catch (Exception ex)
            {
                Label error = new Label();
                error.Text = ex.Message;
            }
        }

        protected void LoopPages()
        {
            foreach (string page in aspxList)
            {
                UpdatePageDictionary(page);
            }
        }

        public static void LoadPage(string pagePath)
        {
            // get the compiled type of referenced path
            Type type = BuildManager.GetCompiledType(pagePath);

            // if type is null, could not determine page type
            if (type == null)
                throw new ApplicationException("Page " + pagePath + " not found");

            // cast page object (could also cast an interface instance as well)
            // in this example, ASP220Page is a custom base page
            BasePage pageView = (BasePage)Activator.CreateInstance(type);

            // process the request with updated object
            ((IHttpHandler)pageView).ProcessRequest(HttpContext.Current);
        }

        protected void UpdatePageDictionary(string currentPage)
        {
            HtmlGenericControl myFrame = new HtmlGenericControl("iframe");
 
            myFrame.Attributes["src"] = currentPage;

            phFrames.Controls.Add(myFrame);
        }

        protected void UpdateUcDictionary(string currentUc)
        {
            UserControl ucCurrent = (UserControl)LoadControl(currentUc);

            try
            {

                string currentControlID = currentUc.Replace("/", "_").Remove(0, 2).Substring(0, currentUc.LastIndexOf('.') - 2);
                ucCurrent.ID = currentControlID;
                MainContentPanel.Controls.Add(ucCurrent);
                MainContentPanel.Controls.Remove(this.MainContentPanel.Controls[0]);
            }
            catch (Exception ex)
            {
                Label error1 = new Label();
                error1.Text = ex.Message;
            }
        }

        protected void DeleteUnusedDictionary()
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.I18N.Dictionary> dictionaryViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.I18N.Dictionary>();

            dictionaryViewDTO = iLanguageMGR.DeleteUnusedDictionary(context);

            if (dictionaryViewDTO.hasError())
            {
                // TODO: agregar panel para mostrar error
                /*
                lblErrorMessage.Text = dictionaryViewDTO.Errors.Message;
                divErrorMessage.Visible = true;
                 */
            }
        }

        protected void btnCreateDictionary_Click(object sender, EventArgs e)
        {
            try
            {
                Session["UpdateDictionary"] = "True";

                //Lista de Paginas aspx

                //ADMINISTRATION

                ////Configuration
                //aspxList.Add("../../Administration/Configuration/ItemParametersMgr.aspx");
                //aspxList.Add("../../Administration/Configuration/LocationParametersMgr.aspx");
                //aspxList.Add("../../Administration/Configuration/LpnParametersMgr.aspx");
                //aspxList.Add("../../Administration/Configuration/Map2DMgr.aspx");
                //aspxList.Add("../../Administration/Configuration/WebParametersMgr.aspx");

                ////Devices
                //aspxList.Add("../../Administration/Devices/LabelBulkConsult.aspx");
                //aspxList.Add("../../Administration/Devices/LabelItemConsult.aspx");
                //aspxList.Add("../../Administration/Devices/LabelLocationConsult.aspx");
                //aspxList.Add("../../Administration/Devices/LabelLpnConsult.aspx");
                //aspxList.Add("../../Administration/Devices/PrinterMgr.aspx");
                //aspxList.Add("../../Administration/Devices/TerminalMgr.aspx");

                ////Dictionary
                //aspxList.Add("../../Administration/Dictionary/Translations.aspx");

                ////LogisticResources
                //aspxList.Add("../../Administration/LogisticsResources/CompanyMgr.aspx");
                //aspxList.Add("../../Administration/LogisticsResources/HangarMgr.aspx");
                //aspxList.Add("../../Administration/LogisticsResources/LocationMgr.aspx");
                //aspxList.Add("../../Administration/LogisticsResources/WarehouseMgr.aspx");
                //aspxList.Add("../../Administration/LogisticsResources/WorkZoneMgr.aspx");

                ////Masters
                //aspxList.Add("../../Administration/Masters/TruckMgr.aspx");
                //aspxList.Add("../../Administration/Masters/CarrierMgr.aspx");
                //aspxList.Add("../../Administration/Masters/CustomerMgr.aspx");
                //aspxList.Add("../../Administration/Masters/DriverMgr.aspx");
                //aspxList.Add("../../Administration/Masters/ItemMgr.aspx");
                //aspxList.Add("../../Administration/Masters/ItemUomMgr.aspx");
                //aspxList.Add("../../Administration/Masters/KitsMgr.aspx");
                //aspxList.Add("../../Administration/Masters/LpnMgr.aspx");
                //aspxList.Add("../../Administration/Masters/OwnerMgr.aspx");
                //aspxList.Add("../../Administration/Masters/VendorMgr.aspx");

                ////Parameters
                //aspxList.Add("../../Administration/Parameters/CategoryItemMgr.aspx");
                //aspxList.Add("../../Administration/Parameters/LPNTypeMgr.aspx");
                //aspxList.Add("../../Administration/Parameters/ReasonMgr.aspx");
                //aspxList.Add("../../Administration/Parameters/ReferenceDocTypeMgr.aspx");
                //aspxList.Add("../../Administration/Parameters/TruckTypeMgr.aspx");

                ////Users
                //aspxList.Add("../../Administration/Users/ChangePassword.aspx");
                //aspxList.Add("../../Administration/Users/PasswordMgr.aspx");
                //aspxList.Add("../../Administration/Users/PermissionMgr.aspx");
                //aspxList.Add("../../Administration/Users/RoleMgr.aspx");
                //aspxList.Add("../../Administration/Users/UserMgr.aspx");

                ////Inbound
                ////Consult
                //aspxList.Add("../../Inbound/Consult/InboundOrderConsult.aspx");
                //aspxList.Add("../../Inbound/Consult/ReceiptConsult.aspx");
                //aspxList.Add("../../Inbound/Consult/ReceptionLog.aspx");

                ////Administration
                //aspxList.Add("../../Inbound/Administration/InboundOrderCloseMgr.aspx");
                //aspxList.Add("../../Inbound/Administration/InboundOrderMgr.aspx");

                ////Inventory
                ////Administration
                //aspxList.Add("../../Inventory/Administration/InventoryMgr.aspx");
                //aspxList.Add("../../Inventory/Administration/InventoryLocation.aspx");

                ////Consult
                //aspxList.Add("../../Inventory/Consult/AdjustmentConsult.aspx");
                //aspxList.Add("../../Inventory/Consult/CycleCountConsult.aspx");
                //aspxList.Add("../../Inventory/Consult/InventoryConsult.aspx");

                ////Movement
                ////Consult
                //aspxList.Add("../../Movement/Consult/ReplenishmentTaskConsult.aspx");

                ////OutBound
                ////Administration
                //aspxList.Add("../../Outbound/Administration/OutboundOrderMgr.aspx");
                //aspxList.Add("../../Outbound/Administration/ReleaseOrderMgr.aspx");

                ////Consult
                //aspxList.Add("../../Outbound/Consult/DispatchAdvanceConsult.aspx");
                //aspxList.Add("../../Outbound/Consult/DispatchLog.aspx");
                //aspxList.Add("../../Outbound/Consult/OutboundOrderConsult.aspx");
                //aspxList.Add("../../Outbound/Consult/PackagesConsult.aspx");
                aspxList.Add("../../Outbound/Administration/BuildingReplacementTask.aspx");


                //////Others
                //////Este grupo da error, creo que por referencia ciclica
                //aspxList.Add("../../Acount/Desktop.aspx");
                //aspxList.Add("../../Acount/Logout.aspx");
                //aspxList.Add("../../Acount/NotAccess.aspx");
                //aspxList.Add("../../DetectScreen.aspx");
                //aspxList.Add("../../Default.aspx");

                //////Stock
                //////Map
                //aspxList.Add("../../Stock/Map/Map2DConsult.aspx");

                //////Consult
                //aspxList.Add("../../Stock/Consult/ExpirationConsult.aspx");
                //aspxList.Add("../../Stock/Consult/MaxAndMinConsult.aspx");
                //aspxList.Add("../../Stock/Consult/PermanenceConsult.aspx");
                //aspxList.Add("../../Stock/Consult/StockConsult.aspx");
                //aspxList.Add("../../Stock/Consult/StockLocationConsult.aspx");



                ////Lo dejo al ultimo por que se crea un ciclo
                //aspxList.Add("../../Administration/Devices/TerminalMonitor.aspx");


                //////USER CONTROLS
                //ascxList.Add("../../Shared/DialogBoxContent.ascx");
                //aspxList.Add("../../Shared/DialogDelete.aspx");
                //ascxList.Add("../../Shared/ErrorContent.ascx");
                //ascxList.Add("../../Shared/FilterReport.ascx");
                //aspxList.Add("../../Shared/GenericError.aspx");
                //ascxList.Add("../../Shared/LookUpFilterContent.ascx");
                //ascxList.Add("../../Shared/MainMenuContent.ascx");
                //ascxList.Add("../../Shared/StatusBarContent.ascx");
                //ascxList.Add("../../Shared/FilterReport.ascx");
                //ascxList.Add("../../Shared/TaskBarContent.ascx");
                //aspxList.Add("../../Account/WMSTekContent.Master");
                //ascxList.Add("../../Account/LoginContent.ascx");




                ////Deshabilita todas las instancias de Dictionary
                ////DisableDictionary();

                //Actualiza los diccionarios asociados a cada Page
                LoopPages();

                //Actualiza los diccionarios asociados a cada User Control
                LoopUserControls();

                //Borra las instancias de Dictionary no utilizadas
                //DeleteUnusedDictionary();
            }
            catch(Exception ex)
            {
                Label error2 = new Label();
                error2.Text = ex.Message;
            }
        }
    }
}
