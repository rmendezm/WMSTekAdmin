using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Reflection;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Display;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Label;
using Binaria.WMSTek.Framework.Entities.Billing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Obout.Grid;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using Binaria.WMSTek.Framework.Entities.Rules;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Data;
using ClosedXML.Excel;
using System.Web.Caching;
using System.Xml.Linq;
using Binaria.WMSTek.Framework.Entities.Utility;
using System.Text.RegularExpressions;
using Binaria.WMSTek.Framework.Entities.Ptl;
using DocumentFormat.OpenXml.Vml.Office;

namespace Binaria.WMSTek.WebClient.Base
{
    public class BasePage : System.Web.UI.Page
    {
        #region "Declaración de Variables"
        public ILanguageMGR iLanguageMGR;
        public IProfileMGR iProfileMGR;
        public IDeviceMGR iDeviceMGR;
        public ILabelMGR iLabelMGR;
        public ILayoutMGR iLayoutMGR;
        public IReceptionMGR iReceptionMGR;
        public IWarehousingMGR iWarehousingMGR;
        public IDispatchingMGR iDispatchingMGR;
        public IInventoryMGR iInventoryMGR;
        public ITasksMGR iTasksMGR;
        public IConfigurationMGR iConfigurationMGR;
        public IRulesMGR iRulesMGR;
        public IDashboardMGR iDashboardMGR;
        public IBillingMGR iBillingMGR;
        public IFuntionalMGR iFuntionalMGR;
        public IIntegrationMGR iIntegrationMGR;
        public ICfgTablesMGR cfgTablesMGR;
        public IUtilityMGR utilityMGR;

        public LogManager theLog = LogManager.getInstance();
        public BaseControl baseControl;
        MiscUtils util;
        protected User objUser;
        protected ContextViewDTO context;
        protected GenericViewDTO<Translate> translateViewDTO;
        protected GenericViewDTO<Warehouse> warehouseViewDTO;
        protected WebMode webMode;
        protected bool crud = false;
        protected string currentPageName = string.Empty;
        protected string currentPageTitle = string.Empty;

        public GenericViewDTO<Country> Country
        {
            get { return (GenericViewDTO<Country>)(ViewState["Country"] ?? null); }
            set { ViewState["Country"] = value; }
        }

        public GenericViewDTO<State> State
        {
            get { return (GenericViewDTO<State>)(ViewState["State"] ?? null); }
            set { ViewState["State"] = value; }
        }

        public GenericViewDTO<City> City
        {
            get { return (GenericViewDTO<City>)(ViewState["City"] ?? null); }
            set { ViewState["City"] = value; }
        }

        public GenericViewDTO<TrackTaskQueue> TrackTaskQueue
        {
            get { return (GenericViewDTO<TrackTaskQueue>)(ViewState["TrackTaskQueue"] ?? null); }
            set { ViewState["TrackTaskQueue"] = value; }
        }

        public GenericViewDTO<GrpItem1> GrpItem1
        {
            get { return (GenericViewDTO<GrpItem1>)(Session["GrpItem1"] ?? null); }
            set { Session["GrpItem1"] = value; }
        }

        public GenericViewDTO<GrpItem2> GrpItem2
        {
            get { return (GenericViewDTO<GrpItem2>)(Session["GrpItem2"] ?? null); }
            set { Session["GrpItem2"] = value; }
        }

        public GenericViewDTO<GrpItem3> GrpItem3
        {
            get { return (GenericViewDTO<GrpItem3>)(Session["GrpItem3"] ?? null); }
            set { Session["GrpItem3"] = value; }
        }

        public GenericViewDTO<GrpItem4> GrpItem4
        {
            get { return (GenericViewDTO<GrpItem4>)(Session["GrpItem4"] ?? null); }
            set { Session["GrpItem4"] = value; }
        }

        public int IdOwner
        {
            get { return (int)(Session["IdOwner"] ?? -1); }
            set { Session["IdOwner"] = value; }
        }

        #endregion

        #region "Eventos"

        protected virtual void Page_Init(object sender, EventArgs e)
        {

            if (Session[WMSTekSessions.Global.LoggedIn] == null || 
                Session[WMSTekSessions.Global.AuthToken] == null || 
                Request.Cookies[WMSTekSessions.Global.AuthToken] == null ||
                !Session[WMSTekSessions.Global.AuthToken].ToString().Equals(Request.Cookies[WMSTekSessions.Global.AuthToken].Value))
            {
                this.Logout();
            }
            // Valida variable de sesion del Usuario Loggeado
            else if (!ValidateSession(WMSTekSessions.Global.LoggedInUser))
            {
                this.Logout();
            }
            else
            {
                context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
                baseControl = BaseControl.getInstance(Request.PhysicalApplicationPath, context);
                var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];

                iLanguageMGR = (ILanguageMGR)objectInstances.getObject("languageMGR");
                iProfileMGR = (IProfileMGR)objectInstances.getObject("profileMGR");
                iDeviceMGR = (IDeviceMGR)objectInstances.getObject("deviceMGR");
                iLabelMGR = (ILabelMGR)objectInstances.getObject("labelMGR");
                iLayoutMGR = (ILayoutMGR)objectInstances.getObject("layoutMGR");
                iReceptionMGR = (IReceptionMGR)objectInstances.getObject("receptionMGR");
                iWarehousingMGR = (IWarehousingMGR)objectInstances.getObject("warehousingMGR");
                iDispatchingMGR = (IDispatchingMGR)objectInstances.getObject("dispatchingMGR");
                iInventoryMGR = (IInventoryMGR)objectInstances.getObject("inventoryMGR");
                iTasksMGR = (ITasksMGR)objectInstances.getObject("tasksMGR");
                iConfigurationMGR = (IConfigurationMGR)objectInstances.getObject("configurationMGR");
                iRulesMGR = (IRulesMGR)objectInstances.getObject("rulesMGR");
                iDashboardMGR = (IDashboardMGR)objectInstances.getObject("dashboardMGR");
                iBillingMGR = (IBillingMGR)objectInstances.getObject("billingMGR");
                iFuntionalMGR = (IFuntionalMGR)objectInstances.getObject("funtionalMGR");
                iIntegrationMGR = (IIntegrationMGR)objectInstances.getObject("integrationMGR");
                cfgTablesMGR = (ICfgTablesMGR)objectInstances.getObject("cfgTablesMGR");
                utilityMGR = (IUtilityMGR)objectInstances.getObject("utilityMGR");

                objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                // Setea variable de sesion del Contexto del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.Context))
                    context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];

                webMode = WebMode.Normal;
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            // Si no esta en modo Configuration, sigue el curso normal
            if (webMode == WebMode.Normal) ValidateAccess();
        }

        #endregion

        #region "Configuración"

        /// <summary>
        /// Configura pagina segun el rol de usuario
        /// </summary>
        protected void ConfigurePage()
        {
            // Dictionary
            if (ValidateSession("UpdateDictionary"))
            {
                UpdateDictionary(this);
                webMode = WebMode.Configuration;
            }
            else
            {
                webMode = WebMode.Normal;
            }
        }

        /// <summary>
        /// Actualiza el diccionario base para los objetos de la pagina. 
        /// Se utiliza cuando el flag 'UpdateDictionary' esta activo
        /// </summary>
        protected void UpdateDictionary(Control content)
        {
            GenericViewDTO<Dictionary> dictionaryViewDTO = new GenericViewDTO<Dictionary>();

            List<Dictionary> dictionaryList = new List<Dictionary>();

            LoopUpdateDictionary(this, dictionaryList, content.ToString());

            // Si hay elementos a traducir, envia a grabar GenericViewDTO<Dictionary>
            if (dictionaryList.Count > 0)
            {
                dictionaryViewDTO = iLanguageMGR.MaintainDictionary(dictionaryList, context);
            }

            if (dictionaryViewDTO.hasError())
            {
                // TODO: mostar y loggear error
            }
        }

        static public void LoopUpdateDictionary(Control control, List<Dictionary> dictionaryList, string contentID)
        {

            foreach (Control c in control.Controls)
            {
                // TODO: modificar para soportar cualquier control con texto 
                // TODO: ver parametros idObjectKey y idModule a pasar al Dictionary
                //        - idModule deberia estar en una variable de sesion de contexto?
                // TODO: manejo de excepciones

                if (c is Label)
                {
                    Label ctrl = (Label)c;

                    if (!String.IsNullOrEmpty(ctrl.Text) && ctrl.Text != " * ")
                        dictionaryList.Add(new Dictionary(0, ctrl.ID, 1, ctrl.Parent.ID, contentID, ctrl.GetType().ToString(), "Text", ctrl.Text, Convert.ToBoolean(CodStatus.Enabled)));
                }

                if (c is Button)
                {
                    Button ctrl = (Button)c;

                    if (!String.IsNullOrEmpty(ctrl.Text))
                        dictionaryList.Add(new Dictionary(0, ctrl.ID, 1, ctrl.Parent.ID, contentID, ctrl.GetType().ToString(), "Text", ctrl.Text, Convert.ToBoolean(CodStatus.Enabled)));
                }
                if (c is ImageButton)
                {
                    ImageButton ctrl = (ImageButton)c;
                    if (!String.IsNullOrEmpty(ctrl.ToolTip))
                        dictionaryList.Add(new Dictionary(0, ctrl.ID, 1, ctrl.Parent.ID, contentID, ctrl.GetType().ToString(), "ToolTip", ctrl.ToolTip, Convert.ToBoolean(CodStatus.Enabled)));
                }

                if (c is LinkButton)
                {
                    LinkButton ctrl = (LinkButton)c;

                    if (!String.IsNullOrEmpty(ctrl.Text))
                        dictionaryList.Add(new Dictionary(0, ctrl.ID, 1, ctrl.Parent.ID, contentID, ctrl.GetType().ToString(), "Text", ctrl.Text, Convert.ToBoolean(CodStatus.Enabled)));
                }

                if (c is GridView)
                {
                    GridView ctrl = (GridView)c;

                    for (int i = 0; i < ctrl.Columns.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(ctrl.Columns[i].HeaderText))
                            dictionaryList.Add(new Dictionary(0, ctrl.Columns[i].HeaderText, 1, ctrl.ID, contentID, "GridColumn", "HeaderText", ctrl.Columns[i].HeaderText, Convert.ToBoolean(CodStatus.Enabled)));
                    }
                }
                if (c is RequiredFieldValidator)
                {
                    RequiredFieldValidator ctrlRequiredFieldValidator = (RequiredFieldValidator)c;
                    if (!String.IsNullOrEmpty(ctrlRequiredFieldValidator.ErrorMessage))
                        dictionaryList.Add(new Dictionary(0, ctrlRequiredFieldValidator.ID, 1, ctrlRequiredFieldValidator.Parent.ID, contentID, ctrlRequiredFieldValidator.GetType().ToString(), "ErrorMessage", ctrlRequiredFieldValidator.ErrorMessage, Convert.ToBoolean(CodStatus.Enabled)));

                }
                if (c is RegularExpressionValidator)
                {
                    RegularExpressionValidator ctrlRequiredFieldValidator = (RegularExpressionValidator)c;
                    if (!String.IsNullOrEmpty(ctrlRequiredFieldValidator.ErrorMessage))
                        dictionaryList.Add(new Dictionary(0, ctrlRequiredFieldValidator.ID, 1, ctrlRequiredFieldValidator.Parent.ID, contentID, ctrlRequiredFieldValidator.GetType().ToString(), "ErrorMessage", ctrlRequiredFieldValidator.ErrorMessage, Convert.ToBoolean(CodStatus.Enabled)));

                }
                if (c is RangeValidator)
                {
                    RangeValidator ctrlRangeValidator = (RangeValidator)c;
                    if (!String.IsNullOrEmpty(ctrlRangeValidator.ErrorMessage))
                        dictionaryList.Add(new Dictionary(0, ctrlRangeValidator.ID, 1, ctrlRangeValidator.Parent.ID, contentID, ctrlRangeValidator.GetType().ToString(), "ErrorMessage", ctrlRangeValidator.ErrorMessage, Convert.ToBoolean(CodStatus.Enabled)));

                }
                if (c is ValidationSummary)
                {
                    ValidationSummary ctrlValidationSummary = (ValidationSummary)c;
                    if (!String.IsNullOrEmpty(ctrlValidationSummary.HeaderText))
                        dictionaryList.Add(new Dictionary(0, ctrlValidationSummary.ID, 1, ctrlValidationSummary.Parent.ID, contentID, ctrlValidationSummary.GetType().ToString(), "HeaderText", ctrlValidationSummary.HeaderText, Convert.ToBoolean(CodStatus.Enabled)));
                }
                if (c is CompareValidator)
                {
                    CompareValidator ctrlCompareValidator = (CompareValidator)c;
                    if (!String.IsNullOrEmpty(ctrlCompareValidator.ErrorMessage))
                        dictionaryList.Add(new Dictionary(0, ctrlCompareValidator.ID, 1, ctrlCompareValidator.Parent.ID, contentID, ctrlCompareValidator.GetType().ToString(), "ErrorMessage", ctrlCompareValidator.ErrorMessage, Convert.ToBoolean(CodStatus.Enabled)));
                }
                if (c is CustomValidator)
                {
                    CustomValidator ctrlCustomValidator = (CustomValidator)c;
                    if (!String.IsNullOrEmpty(ctrlCustomValidator.ErrorMessage))
                        dictionaryList.Add(new Dictionary(0, ctrlCustomValidator.ID, 1, ctrlCustomValidator.Parent.ID, contentID, ctrlCustomValidator.GetType().ToString(), "ErrorMessage", ctrlCustomValidator.ErrorMessage, Convert.ToBoolean(CodStatus.Enabled)));
                }

                if (c.HasControls())
                {
                    LoopUpdateDictionary(c, dictionaryList, contentID);
                }
            }

        }

        /// Configura la pagina segun el idioma de usuario
        /// </summary>
        protected void TranslatePage(Control content)
        {
            // (Si Usuario.Idioma == idiomaBase no se efectua ninguna accion ya que todos los textos estan en español por defecto
            if (objUser.Language.Id != Constants.BASELANGUAGE)
            {
                // Carga una lista de [Object | Translate] para el idioma del usuario
                translateViewDTO = iLanguageMGR.GetTranslationByContent(content.ToString(), objUser.Language.Id, context);

                // Recorre objetos de la pagina, y setea propiedades Text = Translation para los Objetos que se encuentren en la lista
                if (translateViewDTO.Entities != null && translateViewDTO.Entities.Count > 0)
                {
                    LoopTranslate(this, translateViewDTO);
                }
            }
        }

        static public void LoopTranslate(Control control, GenericViewDTO<Translate> translateViewDTO)
        {
            foreach (Control c in control.Controls)
            {
                if (c is Label)
                {
                    Label ctrl = (Label)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.Label" && translationDTO.Dictionary.IdObjectKey == ctrl.ID)
                        {
                            ctrl.Text = translationDTO.TextValue;
                        }
                    }
                }

                if (c is Button)
                {
                    Button ctrl = (Button)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.Button" && translationDTO.Dictionary.IdObjectKey == ctrl.ID)
                        {
                            ctrl.Text = translationDTO.TextValue;
                        }
                    }
                }
                if (c is ImageButton)
                {
                    ImageButton ctrl = (ImageButton)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.ImageButton" && translationDTO.Dictionary.IdObjectKey == ctrl.ID)
                        {
                            ctrl.ToolTip = translationDTO.TextValue;
                        }
                    }
                }

                if (c is LinkButton)
                {
                    LinkButton ctrl = (LinkButton)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.LinkButton" && translationDTO.Dictionary.IdObjectKey == ctrl.ID)
                        {
                            ctrl.Text = translationDTO.TextValue;
                        }
                    }
                }

                if (c is GridView)
                {
                    GridView ctrl = (GridView)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "GridColumn")
                        {
                            for (int i = 0; i < ctrl.Columns.Count; i++)
                            {
                                if (ctrl.Columns[i].HeaderText == translationDTO.Dictionary.IdObjectKey)
                                {
                                    ctrl.Columns[i].HeaderText = translationDTO.TextValue;
                                }
                            }
                        }
                    }
                }

                if (c is CheckBox)
                {
                    CheckBox ctrlCheckBox = (CheckBox)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.CheckBox" && translationDTO.Dictionary.IdObjectKey == ctrlCheckBox.ID)
                        {
                            ctrlCheckBox.Text = translationDTO.TextValue;
                        }
                    }
                }
                if (c is RequiredFieldValidator)
                {
                    RequiredFieldValidator ctrlRequiredFieldValidator = (RequiredFieldValidator)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.RequiredFieldValidator" && translationDTO.Dictionary.IdObjectKey == ctrlRequiredFieldValidator.ID)
                        {
                            ctrlRequiredFieldValidator.ErrorMessage = translationDTO.TextValue;
                        }
                    }

                }
                if (c is RegularExpressionValidator)
                {
                    RegularExpressionValidator ctrlRegularExpressionValidator = (RegularExpressionValidator)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.RegularExpressionValidator" && translationDTO.Dictionary.IdObjectKey == ctrlRegularExpressionValidator.ID)
                        {
                            ctrlRegularExpressionValidator.ErrorMessage = translationDTO.TextValue;
                        }
                    }
                }
                if (c is RangeValidator)
                {
                    RangeValidator ctrlRangeValidator = (RangeValidator)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.RangeValidator" && translationDTO.Dictionary.IdObjectKey == ctrlRangeValidator.ID)
                        {
                            ctrlRangeValidator.ErrorMessage = translationDTO.TextValue;
                        }
                    }
                }
                if (c is ValidationSummary)
                {
                    ValidationSummary ctrlValidationSummary = (ValidationSummary)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.ValidationSummary" && translationDTO.Dictionary.IdObjectKey == ctrlValidationSummary.ID)
                        {
                            ctrlValidationSummary.HeaderText = translationDTO.TextValue;
                        }
                    }
                }
                if (c is CompareValidator)
                {
                    CompareValidator ctrlCompareValidator = (CompareValidator)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.CompareValidator" && translationDTO.Dictionary.IdObjectKey == ctrlCompareValidator.ID)
                        {
                            ctrlCompareValidator.ErrorMessage = translationDTO.TextValue;
                        }
                    }
                }
                if (c is CustomValidator)
                {
                    CustomValidator ctrlCustomValidator = (CustomValidator)c;

                    foreach (Translate translationDTO in translateViewDTO.Entities)
                    {
                        if (translationDTO.Dictionary.ObjectType == "System.Web.UI.WebControls.CustomValidator" && translationDTO.Dictionary.IdObjectKey == ctrlCustomValidator.ID)
                        {
                            ctrlCustomValidator.ErrorMessage = translationDTO.TextValue;
                        }
                    }
                }

                if (c.HasControls())
                {
                    LoopTranslate(c, translateViewDTO);
                }
            }
        }

        /// <summary>
        /// Valida que el usuario tenga acceso a la pagina que esta invocando
        /// </summary>
        protected void ValidateAccess()
        {
            XmlDocument xmlMenu = new XmlDocument();
            bool isValidAccess = false;

            currentPageName = GetCurrentPageName();

            // Página de errores
            if (currentPageName == "InventoryLocation.aspx")
            {
                isValidAccess = true;
            }
            else
            {
                // Página de errores
                if (currentPageName == "GenericError.aspx")
                {
                    isValidAccess = true;
                }
                else
                {
                    XmlNodeList menuNodes = xmlMenu.GetElementsByTagName("aspxPage");

                    foreach (Binaria.WMSTek.Framework.Entities.Profile.MenuItem menuItem in objUser.Menu)
                    {
                        string aspxPage = menuItem.AspxPage;

                        // Obtiene el nombre de la pagina, sin la ruta de acceso
                        if (aspxPage != null && aspxPage != string.Empty)
                            aspxPage = aspxPage.Substring(aspxPage.LastIndexOf("/") + 1);

                        if (aspxPage == currentPageName)
                        {
                            isValidAccess = true;
                            currentPageTitle = menuItem.TextValue;
                            break;
                        }
                    }
                }
            }

            if (!isValidAccess)
                Response.Redirect("~/Account/NotAccess.aspx");
        }

        public string GetCurrentPageName()
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            string sRet = oInfo.Name;

            if (oInfo.Name != "InventoryLocation.aspx" && oInfo.Name != "RptDynamicReportsMgr.aspx" &&
                oInfo.Name != "CheckOrdersWaveInQueue.aspx" && oInfo.Name != "CheckOrdersInQueue.aspx" &&
                oInfo.Name != "CheckBatchesInQueue.aspx")
            {
                // Permite diferenciar llamadas a la misma página, pero con diferentes parámetros (ReceiptConsult por ej.)
                if (Request.QueryString != null && Request.QueryString.ToString() != String.Empty)
                {
                    sRet = sRet + "?" + Request.QueryString.ToString();
                }
            }

            return sRet;
        }

        public bool ValidateSession(string sessionName)
        {
            if (Session[sessionName] != null && Session[sessionName].ToString() != string.Empty)
                return true;
            else
                return false;
        }

        public bool ValidateViewState(string viewStateName)
        {
            if (ViewState[viewStateName] != null && ViewState[viewStateName].ToString() != string.Empty)
                return true;
            else
                return false;
        }

        #endregion

        #region LISTAS DDL (DropDownList)

        protected void LoadDisplayType(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<DisplayType> displayTypeViewDTO = new GenericViewDTO<DisplayType>();

            // Lista de Tipos de Display
            displayTypeViewDTO = iDeviceMGR.FindAllDisplayType(context);

            objControl.DataSource = displayTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (showEmptyRow)
            {
                // TODO: ver por que no inserta este valor
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadLanguage(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Language> languageViewDTO = new GenericViewDTO<Language>();

            languageViewDTO = iLanguageMGR.FindAllLanguage(context);
            objControl.DataSource = languageViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }
        /// <summary>
        /// Se ocupa en Configurations/Translates.aspx.cs
        /// No se debe agregar el item "todos"
        /// </summary>
        public void LoadLanguageDefined(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Language> languageViewDTO = new GenericViewDTO<Language>();

            languageViewDTO = iLanguageMGR.GetLanguageDefined(context);
            objControl.DataSource = languageViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();
        }
        /// <summary>
        /// Se ocupa en Configurations/Translates.aspx.cs
        /// </summary>
        public void LoadModule(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.Profile.Module> moduleViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Profile.Module>();

            moduleViewDTO = iProfileMGR.FindAllModule(context);
            objControl.DataSource = moduleViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }

        public void LoadModuleAssigned(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Binaria.WMSTek.Framework.Entities.Profile.Module> moduleViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.Profile.Module>();
            GenericViewDTO<RoleModule> roleModuleViewDTO = new GenericViewDTO<RoleModule>();

            roleModuleViewDTO = iProfileMGR.FindAllRoleModule(context);
            moduleViewDTO = iProfileMGR.FindAllModule(context);

            var distModule = roleModuleViewDTO.Entities.Select(s => s.Module.Id).Distinct();
            moduleViewDTO.Entities = (from a in moduleViewDTO.Entities
                                      join b in distModule on a.Id equals b
                                      select a).ToList();

            objControl.DataSource = moduleViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }
        /// <summary>
        /// Se ocupa en Configurations/Translates.aspx.cs
        /// </summary>
        public void LoadTypeObject(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Dictionary> dicionaryViewDTO = new GenericViewDTO<Dictionary>();
            GenericViewDTO<Dictionary> dicionaryViewDTOCtr = new GenericViewDTO<Dictionary>();
            dicionaryViewDTO = iLanguageMGR.ObjectGetType(context);
            Dictionary objDictionary;

            foreach (Dictionary dic in dicionaryViewDTO.Entities)
            {
                objDictionary = new Dictionary();
                char[] delimiterChars = { '.' };
                string[] objecttype1 = dic.ObjectType.Split(delimiterChars);
                int count = objecttype1.Length;
                string texto = objecttype1[count - 1];
                objDictionary.ObjectType = texto;
                objDictionary.TextValue = dic.ObjectType;
                dicionaryViewDTOCtr.Entities.Add(objDictionary);
            }

            objControl.DataSource = dicionaryViewDTOCtr.Entities;
            objControl.DataTextField = "ObjectType";
            objControl.DataValueField = "TextValue";
            objControl.DataBind();

            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }

        public void LoadContainer(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Dictionary> dicionaryViewDTO = new GenericViewDTO<Dictionary>();

            dicionaryViewDTO = iLanguageMGR.ContainerGetObject(context);

            objControl.DataSource = dicionaryViewDTO.Entities;
            objControl.DataTextField = "IdControlContainer";
            objControl.DataValueField = "IdControlContainer";
            objControl.DataBind();

            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }

        public void LoadProperty(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Dictionary> dicionaryViewDTO = new GenericViewDTO<Dictionary>();

            dicionaryViewDTO = iLanguageMGR.GetProperty(context);
            objControl.DataSource = dicionaryViewDTO.Entities;
            objControl.DataTextField = "TextProperty";
            //objControl.DataValueField = "Id";
            objControl.DataBind();

            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }

        protected void LoadForeman(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> foremanViewDTO = new GenericViewDTO<User>();
            var newContext = NewContext();
            if (context.SessionInfo.User.UserName.ToUpper().Equals("BASE"))
            {
                foremanViewDTO = iProfileMGR.FindAllUserBase(newContext);
            }
            else
            {
                foremanViewDTO = iProfileMGR.FindAllUser(newContext);
            }

            objControl.DataSource = foremanViewDTO.Entities;

            objControl.Items.Clear();

            foreach (User foreman in foremanViewDTO.Entities)
            {
                objControl.Items.Add(new ListItem((foreman.FirstName + " " + foreman.LastName), foreman.Id.ToString()));
            }
            if (showEmptyRow)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }
        }
        protected void LoadUsersByNotInRole(DropDownList objControl, int rolId, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetUsersByNotInRole(rolId, context);

            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                if (user.CodStatus != false)
                {
                    objControl.Items.Add(new ListItem((user.FirstName + " " + user.LastName), user.Id.ToString()));
                }
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void LoadUsersByInRole(DropDownList objControl, int rolId, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetByRoleAndIdWhs(rolId, context);

            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                if (user.CodStatus != false)
                {
                    objControl.Items.Add(new ListItem((user.FirstName + " " + user.LastName), user.UserName));
                }
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void LoadUsersByNotInRoleAndIdWhs(DropDownList objControl, int rolId, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetUsersByNotInRoleAndIdWhs(rolId, context);

            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                if (user.CodStatus != false)
                {
                    objControl.Items.Add(new ListItem((user.FirstName + " " + user.LastName), user.Id.ToString()));
                }
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void loadRolesByNotInUser(DropDownList objControl, int userId, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<Role> roleViewDTO = new GenericViewDTO<Role>();

            roleViewDTO = iProfileMGR.GetRolesByNotInUser(userId, context);

            objControl.DataSource = roleViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void LoadUsersInWorkZone(DropDownList objControl, int idWorkZone, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetUsersByWorkzone(idWorkZone, context);

            foreach (User user in userViewDTO.Entities)
                objControl.Items.Add(new ListItem((user.UserName + " - " + user.FirstName + " " + user.LastName), user.UserName.ToString()));

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        protected void LoadUsersByCodStatus(DropDownList objControl, CodStatus codStatus, bool loadRoles, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetUsersByCodStatus(codStatus, loadRoles, context);

            objControl.Items.Clear();
            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                objControl.Items.Add(new ListItem((user.FirstName + " " + user.LastName), user.Id.ToString()));
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void GetUsersByCodStatusIdWhs(DropDownList objControl, CodStatus codStatus, bool loadRoles, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetUsersByCodStatusIdWhs(codStatus, loadRoles, context);

            objControl.Items.Clear();
            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                objControl.Items.Add(new ListItem((user.FirstName + " " + user.LastName), user.Id.ToString()));
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void LoadUsersOperator(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetOperatorByWhs(context);

            objControl.Items.Clear();
            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                objControl.Items.Add(new ListItem((user.UserName), user.UserName));
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        protected void LoadUsersOperatorWihtOutDefaultWhs(DropDownList objControl, bool showEmptyRow, string emptyRowText)
        {
            GenericViewDTO<User> userViewDTO = new GenericViewDTO<User>();

            userViewDTO = iProfileMGR.GetOperatorWihtOutDefaultWhsByWhs(context);

            objControl.Items.Clear();
            objControl.DataSource = userViewDTO.Entities;

            foreach (User user in userViewDTO.Entities)
            {
                objControl.Items.Add(new ListItem((user.UserName), user.UserName));
            }

            if (showEmptyRow) objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
        }

        public void FindAllTrackTaskQueue()
        {
            var trackTaskQueueViewDTO = new GenericViewDTO<TrackTaskQueue>();
            var trackAux = new GenericViewDTO<TrackTaskQueue>();
            trackAux.Entities = new List<TrackTaskQueue>();

            trackTaskQueueViewDTO = utilityMGR.FindAllTrackTaskQueue(context);

            if (trackTaskQueueViewDTO.Entities != null && trackTaskQueueViewDTO.Entities.Count > 0)
            {   
                foreach (var track in trackTaskQueueViewDTO.Entities)
                {
                    if (track.IdTrackTaskQueue != (int)eTrackTaskQueue.Release)
                    {
                        trackAux.Entities.Add(track);
                    }
                }
            }

            TrackTaskQueue = trackAux;
        }

        public void FindAllTrackTaskQueueSimulation()
        {
            var trackTaskQueueViewDTO = new GenericViewDTO<TrackTaskQueue>();
            var trackAux = new GenericViewDTO<TrackTaskQueue>();
            trackAux.Entities = new List<TrackTaskQueue>();

            trackTaskQueueViewDTO = utilityMGR.FindAllTrackTaskQueue(context);

            if (trackTaskQueueViewDTO.Entities != null && trackTaskQueueViewDTO.Entities.Count > 0)
            {
                foreach (var track in trackTaskQueueViewDTO.Entities)
                {
                    if (track.IdTrackTaskQueue != (int)eTrackTaskQueue.FirstStepSuccessful && track.IdTrackTaskQueue != (int)eTrackTaskQueue.SecondStepSuccessful)
                    {
                        trackAux.Entities.Add(track);
                    }
                }
            }

            TrackTaskQueue = trackAux;
        }

        public void FindAllPlaces()
        {
            GenericViewDTO<Country> countryViewDTO = new GenericViewDTO<Country>();
            GenericViewDTO<State> stateViewDTO = new GenericViewDTO<State>();
            GenericViewDTO<City> cityViewDTO = new GenericViewDTO<City>();

            countryViewDTO = iLayoutMGR.FindAllCountry(context);
            stateViewDTO = iLayoutMGR.FindAllState(context);
            cityViewDTO = iLayoutMGR.FindAllCity(context);

            Country = countryViewDTO;
            State = stateViewDTO;
            City = cityViewDTO;
        }

        public void ConfigureDDlCountry(DropDownList objControl3, bool isNew, int idcountry, string emptyRowText)
        {
            if (Country == null || State == null || City == null)
                FindAllPlaces();

            objControl3.SelectedValue = null;
            objControl3.DataSource = Country.Entities;
            objControl3.DataTextField = "Name";
            objControl3.DataValueField = "Id";
            objControl3.DataBind();

            if (isNew)
            {
                objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl3.Items[0].Selected = true;
            }
            else
            {
                if (idcountry != -1)
                {
                    objControl3.SelectedValue = idcountry.ToString();
                    objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                }
                else
                {
                    objControl3.SelectedValue = null;
                    objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl3.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDlState(DropDownList objControl1, bool isNew, int idstate, int idcountry, string emptyRowText)
        {
            if (Country == null || State == null || City == null)
                FindAllPlaces();
            if (!(Country.Entities.Count > 0) || !(State.Entities.Count > 0) || !(City.Entities.Count > 0))
                FindAllPlaces();

            GenericViewDTO<State> StateDTO = new GenericViewDTO<State>();
            StateDTO.Entities = new List<State>();

            //No ha seleccionado ni paìs ni estado
            if (idstate == -1 && idcountry == -1)
            {
                if (isNew)
                {
                    objControl1.SelectedValue = null;
                    objControl1.DataSource = StateDTO.Entities;
                    objControl1.DataTextField = "Name";
                    objControl1.DataValueField = "Id";
                    objControl1.DataBind();

                    objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl1.Items[0].Selected = true;
                }

                if (objControl1.Items.Count < 1)
                {
                    objControl1.SelectedValue = null;
                    objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl1.Items[0].Selected = true;
                }
                else
                {
                    objControl1.SelectedValue = null;
                    objControl1.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los states del view state
                foreach (State state in State.Entities)
                {
                    //si pertenece al pais se agrega
                    if (state.IdCountry == idcountry)
                    {
                        StateDTO.Entities.Add(state);
                    }
                }
                if (StateDTO.Entities.Count > 0)
                {
                    objControl1.SelectedValue = null;
                    objControl1.DataSource = StateDTO.Entities;
                    objControl1.DataTextField = "Name";
                    objControl1.DataValueField = "Id";
                    objControl1.DataBind();

                    if (isNew)
                    {
                        objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl1.Items[0].Selected = true;
                    }
                    else
                    {
                        if (idstate != -1)
                            objControl1.SelectedValue = idstate.ToString();

                        objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    }
                }
            }
        }

        public void ConfigureDDlCity(DropDownList objControl2, bool isNew, int idcity, int idstate, int idcountry, string emptyRowText)
        {
            //Si esta todo nulo carga lugares nuevamente 
            if (Country == null || State == null || City == null)
            {
                FindAllPlaces();
            }
            if (!(Country.Entities.Count > 0) || !(State.Entities.Count > 0) || !(City.Entities.Count > 0))
                FindAllPlaces();

            GenericViewDTO<City> CityDTO = new GenericViewDTO<City>();
            CityDTO.Entities = new List<City>();

            //si no esta seleccionado el state entonces limpia la lista
            if (idcity == -1 && idstate == -1)
            {
                if (objControl2.Items.Count > 0)
                {
                    objControl2.Items.Clear();
                }
                //inserta un item vacio 
                objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                //selecciona item -1
                objControl2.Items[0].Selected = true;
            }
            else
            {
                //recorre los states del view state
                foreach (City city in City.Entities)
                {
                    //si pertenece al país y al estado se agrega el item
                    if ((city.IdCountry == idcountry) && (city.IdState == idstate))
                    {
                        CityDTO.Entities.Add(city);
                    }
                }
                if (CityDTO.Entities.Count > 0)
                {
                    objControl2.SelectedValue = null;
                    objControl2.DataSource = CityDTO.Entities;
                    objControl2.DataTextField = "Name";
                    objControl2.DataValueField = "Id";
                    objControl2.DataBind();

                    if (isNew)
                    {
                        objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl2.Items[0].Selected = true;
                    }
                    else
                    {
                        if (idcity != -1)
                        {
                            objControl2.SelectedValue = null;
                            objControl2.SelectedValue = idcity.ToString();
                            objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        }
                        else
                        {
                            objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        }
                    }
                }
                else
                {
                    if (objControl2.Items.Count > 0)
                    {
                        objControl2.Items.Clear();
                    }
                    //inserta un item vacio 
                    objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    //selecciona item -1
                    objControl2.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDlTrackTaskQueue(DropDownList dropDownList, bool isNew, int idTrackTaskQueue, string emptyRowText, bool isSimulation)
        {
            //   if (TrackTaskQueue == null)
            if (isSimulation)
            {
                FindAllTrackTaskQueueSimulation();
            }
            else
            {
                FindAllTrackTaskQueue();
            }

            dropDownList.SelectedValue = null;
            dropDownList.DataSource = TrackTaskQueue.Entities;
            dropDownList.DataTextField = "NameTrackTaskQueue";
            dropDownList.DataValueField = "IdTrackTaskQueue";
            dropDownList.DataBind();

            if (isNew)
            {
                dropDownList.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                dropDownList.Items[0].Selected = true;
            }
            else
            {
                if (idTrackTaskQueue != -1)
                {
                    dropDownList.SelectedValue = idTrackTaskQueue.ToString();
                }
                else
                {
                    dropDownList.SelectedValue = null;
                    dropDownList.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    dropDownList.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureLstViewTrackTaskQueue(ListBox dropDownList, bool isNew, int idTrackTaskQueue, string emptyRowText, bool isSimulation)
        {
            //   if (TrackTaskQueue == null)
            if (isSimulation)
            {
                FindAllTrackTaskQueueSimulation();
            }
            else
            {
                FindAllTrackTaskQueue();
            }

            dropDownList.SelectedValue = null;
            dropDownList.DataSource = TrackTaskQueue.Entities;
            dropDownList.DataTextField = "NameTrackTaskQueue";
            dropDownList.DataValueField = "IdTrackTaskQueue";
            dropDownList.DataBind();
            dropDownList.Items[1].Selected = true;

            if (isNew)
            {
                dropDownList.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                dropDownList.Items[0].Selected = true;
            }
            //else
            //{
            //    if (idTrackTaskQueue != -1)
            //    {
            //        dropDownList.SelectedValue = idTrackTaskQueue.ToString();
            //    }
            //    else
            //    {
            //        dropDownList.SelectedValue = null;
            //        dropDownList.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            //        dropDownList.Items[0].Selected = true;
            //    }
            //}
        }


        public void LoadStagingLocations(DropDownList objControl, int idWhs, string emptyRowText)
        {
            // TODO: cargar desde view state
            GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();

            locationViewDTO = iLayoutMGR.GetStagingLocations(idWhs, context);

            objControl.Items.Clear();

            foreach (Location location in locationViewDTO.Entities)
                objControl.Items.Add(new ListItem(location.Code, location.IdCode));

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadForkLiftLocationsInWorkZone(DropDownList objControl, int idWorkZone, int idWhs, string emptyRowText)
        {
            GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();

            locationViewDTO = iLayoutMGR.GetForkLiftLocationsInWorkZone(idWhs, idWorkZone, context);

            foreach (Location location in locationViewDTO.Entities)
                objControl.Items.Add(new ListItem(location.Code, location.IdCode));

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadLocationsByWhsAndType(DropDownList objControl, int idWhs, string type, string emptyRowText, bool isNew)
        {
            GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();

            locationViewDTO = iLayoutMGR.GetLocationsByWhsAndType(idWhs, type, context);

            objControl.Items.Clear();

            foreach (Location location in locationViewDTO.Entities)
                objControl.Items.Add(new ListItem(location.Code, location.IdCode));

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadLocationsByWhsAndListType(DropDownList objControl, int idWhs, List<string> lstType, string emptyRowText, bool isNew)
        {
            GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();
            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = context.MainFilter;

            foreach (var filter in newContext.MainFilter)
            {
                filter.FilterValues.Clear();
            }

            //newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(idWhs.ToString()));

            foreach (var item in lstType)
            {
                newContext.MainFilter[Convert.ToInt16(EntityFilterName.LocationType)].FilterValues.Add(new FilterItem(item));
            }

            locationViewDTO = iLayoutMGR.FindAllLocation(newContext);



            objControl.Items.Clear();

            foreach (Location location in locationViewDTO.Entities)
                objControl.Items.Add(new ListItem(location.Code, location.IdCode));

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// La diferencia que tiene este metodo con el "LoadWarehouses" es que en este se deshabilita los filtros para que 
        /// traiga todos los centros, por que al no quitar los filtros entraba en conflicto con 
        /// el filtro nombre Item que se encuentra en el Mantenedor de maximos y mininos por ubicacion
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="isNew">Indica si tiene que cargar el item "todos"</param>
        /// <param name="emptyRowText">Lleva lo que se le pone a la fila vacia (ItemName)</param>
        /// <param name="emptyRowValue">Lleva lo que se le pone a la fila vacia (ItemName)</param>
        public void LoadWarehousesLessFilter(DropDownList objControl, bool isNew, string emptyRowText, string emptyRowValue)
        {
            warehouseViewDTO = new GenericViewDTO<Warehouse>();

            // TODO: actualizar este manejo por sesion en las listas mas utilizadas
            if (ValidateSession(WMSTekSessions.Shared.WarehouseList))
            {
                warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.Shared.WarehouseList];

                if (warehouseViewDTO.Entities.Count == 0)
                {
                    context.MainFilter = null;
                    warehouseViewDTO = iLayoutMGR.FindAllWarehouse(context);
                    Session.Add(WMSTekSessions.Shared.WarehouseList, warehouseViewDTO);
                }
            }
            else
            {
                context.MainFilter = null;
                warehouseViewDTO = iLayoutMGR.FindAllWarehouse(context);
                Session.Add(WMSTekSessions.Shared.WarehouseList, warehouseViewDTO);
            }

            //Limpia el control
            objControl.Items.Clear();

            objControl.DataSource = warehouseViewDTO.Entities;
            objControl.DataTextField = "ShortName";
            objControl.DataValueField = "Id";
            objControl.DataBind();
            objControl.Items.Insert(0, new ListItem(emptyRowText, emptyRowValue));

            if (isNew)
            {
                // Agrega la opción 'Seleccione...' y la selecciona
                objControl.Items[0].Selected = true;
            }
        }
        public void LoadWarehouses(DropDownList objControl, bool isNew, string emptyRowText, string emptyRowValue)
        {
            warehouseViewDTO = new GenericViewDTO<Warehouse>();

            // TODO: actualizar este manejo por sesion en las listas mas utilizadas
            if (ValidateSession(WMSTekSessions.Shared.WarehouseList))
            {
                warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.Shared.WarehouseList];

                if (warehouseViewDTO.Entities.Count == 0)
                {
                    warehouseViewDTO = iLayoutMGR.FindAllWarehouse(context);
                    Session.Add(WMSTekSessions.Shared.WarehouseList, warehouseViewDTO);
                }
            }
            else
            {
                warehouseViewDTO = iLayoutMGR.FindAllWarehouse(context);
                Session.Add(WMSTekSessions.Shared.WarehouseList, warehouseViewDTO);
            }

            //Limpia el control
            objControl.Items.Clear();

            objControl.DataSource = warehouseViewDTO.Entities;
            objControl.DataTextField = "ShortName";
            objControl.DataValueField = "Id";
            objControl.DataBind();
            objControl.Items.Insert(0, new ListItem(emptyRowText, emptyRowValue));

            if (isNew)
            {
                // Agrega la opción 'Seleccione...' y la selecciona
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWarehousesNew(DropDownList objControl, bool isNew, string emptyRowText, string emptyRowValue)
        {
            warehouseViewDTO = new GenericViewDTO<Warehouse>();
            GenericViewDTO<Warehouse> warehouseViewDTONew = new GenericViewDTO<Warehouse>();

            // TODO: actualizar este manejo por sesion en las listas mas utilizadas
            //if (ValidateSession(WMSTekSessions.Shared.WarehouseList))
            //{
            //    warehouseViewDTO = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.Shared.WarehouseList];
            //    warehouseViewDTONew = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

            //    if (warehouseViewDTO.Entities.Count == 0)
            //    {
            //        warehouseViewDTO = iLayoutMGR.FindAllWarehouse(context);
            //        if (warehouseViewDTONew.Entities.Count > 0)
            //        {
            //            foreach (var item in warehouseViewDTONew.Entities)
            //            {
            //                if (warehouseViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
            //                {
            //                    warehouseViewDTO.Entities.Remove(warehouseViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
            //                }
            //            }
            //        }

            //        Session.Add(WMSTekSessions.Shared.WarehouseList, warehouseViewDTO);
            //    }
            //}
            //else
            //{
            var newContext = NewContext();
            warehouseViewDTO = iLayoutMGR.FindAllWarehouse(newContext);
            warehouseViewDTONew = (GenericViewDTO<Warehouse>)Session[WMSTekSessions.UserMgr.WarehouseList];

            if (warehouseViewDTONew != null && warehouseViewDTONew.Entities.Count > 0)
            {
                foreach (var item in warehouseViewDTONew.Entities)
                {
                    if (warehouseViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
                    {
                        warehouseViewDTO.Entities.Remove(warehouseViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
                    }
                }
            }
            Session.Add(WMSTekSessions.Shared.WarehouseList, warehouseViewDTO);
            //}

            //Limpia el control
            objControl.Items.Clear();

            objControl.DataSource = warehouseViewDTO.Entities;
            objControl.DataTextField = "ShortName";
            objControl.DataValueField = "Id";
            objControl.DataBind();
            objControl.Items.Insert(0, new ListItem(emptyRowText, emptyRowValue));

            if (isNew)
            {
                // Agrega la opción 'Seleccione...' y la selecciona
                objControl.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Trae todos los rowloc de Locations
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="emptyRowText"></param>
        /// <param name="emptyRowValue"></param>
        /// <param name="isNew"></param>
        /// <param name="idWarehouse"></param>
        public void LoadLocationRows(DropDownList objControl, string emptyRowText, string emptyRowValue, bool isNew, int idWarehouse)
        {
            GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();

            if (ValidateSession(WMSTekSessions.Shared.RowLocList))
                locationViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Shared.RowLocList];
            else
            {
                locationViewDTO = iLayoutMGR.GetRowLocations(idWarehouse, context);
                Session.Add(WMSTekSessions.Shared.RowLocList, locationViewDTO);
            }

            objControl.DataSource = locationViewDTO.Entities;
            objControl.DataTextField = "Row";
            objControl.DataValueField = "Row";
            objControl.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
            //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadLocation(DropDownList objControl, string emptyRowText, string emptyRowValue, bool isNew, int idWarehouse, int rowLoc)
        {
            GenericViewDTO<Location> locationViewDTO = new GenericViewDTO<Location>();

            if (ValidateSession(WMSTekSessions.Shared.LocationList))
                locationViewDTO = (GenericViewDTO<Location>)Session[WMSTekSessions.Shared.LocationList];
            else
            {
                locationViewDTO = iLayoutMGR.GetLocationByWhsAndRow(idWarehouse, rowLoc, context);
                Session.Add(WMSTekSessions.Shared.LocationList, locationViewDTO);
            }

            objControl.DataSource = locationViewDTO.Entities;
            objControl.DataTextField = "Code";
            objControl.DataValueField = "IdCode";
            objControl.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
            //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Trae las Columns de las ubicaciones filtradas por Hangar y tipo de ubicacion
        /// Se ocupa en el mantenedor de WorkZones
        /// </summary>
        public void LoadColumnLocationByHngAndLocType(DropDownList objControlFrom, DropDownList objControlTo, string emptyRowText, string emptyRowValue, bool isNew, int idWhs, int idHng, string locTypeCode)
        {
            GenericViewDTO<Location> locationColumnViewDTO = new GenericViewDTO<Location>();

            locationColumnViewDTO = iLayoutMGR.GetColumnByWhsHngAndLocType(context, idHng, locTypeCode);
            Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);

            objControlFrom.DataSource = locationColumnViewDTO.Entities;
            objControlFrom.DataTextField = "Column";
            objControlFrom.DataValueField = "Column";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationColumnViewDTO.Entities;
            objControlTo.DataTextField = "Column";
            objControlTo.DataValueField = "Column";
            objControlTo.DataBind();

            if (isNew)
            {
                if (locationColumnViewDTO.Entities.Count > 0)
                {
                    objControlTo.Items[locationColumnViewDTO.Entities.Count - 1].Selected = true;
                    objControlFrom.Items[0].Selected = true;
                }
            }
        }
        /// <summary>
        /// Trae todas los levels de las ubicaciones filtradas por Hangar y tipo de ubicacion
        /// Se ocupa en el mantenedor de WorkZones
        /// </summary>
        public void LoadLevelLocationByHngAndLocType(DropDownList objControlFrom, DropDownList objControlTo, string emptyRowText, string emptyRowValue, bool isNew, int idWhs, int idHng, string locTypeCode)
        {
            GenericViewDTO<Location> locationLevelViewDTO = new GenericViewDTO<Location>();

            locationLevelViewDTO = iLayoutMGR.GetLevelByWhsHngAndLocType(context, idHng, locTypeCode);
            Session.Add(WMSTekSessions.LocationMgr.LocationLevelList, locationLevelViewDTO);

            objControlFrom.DataSource = locationLevelViewDTO.Entities;
            objControlFrom.DataTextField = "Level";
            objControlFrom.DataValueField = "Level";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationLevelViewDTO.Entities;
            objControlTo.DataTextField = "Level";
            objControlTo.DataValueField = "Level";
            objControlTo.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
            //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                if (isNew)
                {
                    if (locationLevelViewDTO.Entities.Count > 0)
                    {
                        objControlTo.Items[locationLevelViewDTO.Entities.Count - 1].Selected = true;
                        objControlFrom.Items[0].Selected = true;
                    }
                }
            }
        }
        /// <summary>
        /// Trae todas las filas de las ubicaciones filtradas por Hangar y tipo de ubicacion,
        /// Se ocupa en el mantenedor de WorkZones
        /// </summary>
        public void LoadRowLocationByHngAndLocType(DropDownList objControlFrom, DropDownList objControlTo, string emptyRowText, string emptyRowValue, bool isNew, int idWhs, int idHng, string locTypeCode)
        {
            GenericViewDTO<Location> locationRowViewDTO = new GenericViewDTO<Location>();

            locationRowViewDTO = iLayoutMGR.GetRowByWhsHngAndLocType(context, idHng, locTypeCode);
            Session.Add(WMSTekSessions.LocationMgr.LocationRowList, locationRowViewDTO);

            objControlFrom.DataSource = locationRowViewDTO.Entities;
            objControlFrom.DataTextField = "Row";
            objControlFrom.DataValueField = "Row";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationRowViewDTO.Entities;
            objControlTo.DataTextField = "Row";
            objControlTo.DataValueField = "Row";
            objControlTo.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
            //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                if (locationRowViewDTO.Entities.Count > 0)
                {
                    objControlTo.Items[locationRowViewDTO.Entities.Count - 1].Selected = true;
                    objControlFrom.Items[0].Selected = true;
                }
            }
        }



        public void LoadListRowLoc()
        {
            GenericViewDTO<LocationConsult> locationRowViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListRow))
                locationRowViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListRow];
            else
            {
                locationRowViewDTO = iLayoutMGR.GetAllRowLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListRow, locationRowViewDTO);
            }
        }
        public void LoadListColumnLoc()
        {
            GenericViewDTO<LocationConsult> locationColumnViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListColumn))
                locationColumnViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListColumn];
            else
            {
                locationColumnViewDTO = iLayoutMGR.GetAllColumnsLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);
            }
        }
        public void LoadListLevelLoc()
        {
            GenericViewDTO<LocationConsult> locationLevelViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListLevel))
                locationLevelViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListLevel];
            else
            {
                locationLevelViewDTO = iLayoutMGR.GetAllLevelLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListLevel, locationLevelViewDTO);
            }
        }

        public void LoadListRowLocWhs()
        {
            GenericViewDTO<LocationConsult> locationRowViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListRow))
                locationRowViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListRow];
            else
            {
                locationRowViewDTO = iLayoutMGR.GetAllRowWhsHangarLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListRow, locationRowViewDTO);
            }
        }
        public void LoadListColumnLocWhs()
        {
            GenericViewDTO<LocationConsult> locationColumnViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListColumn))
                locationColumnViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListColumn];
            else
            {
                locationColumnViewDTO = iLayoutMGR.GetAllColumnsWhsHangarLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);
            }
        }
        public void LoadListLevelLocWhs()
        {
            GenericViewDTO<LocationConsult> locationLevelViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListLevel))
                locationLevelViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListLevel];
            else
            {
                locationLevelViewDTO = iLayoutMGR.GetAllLevelWhsHangarLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListLevel, locationLevelViewDTO);
            }
        }

        /// <summary>
        /// Trae todas los Columns de las ubicaciones
        /// Se ocupa en el Inventory Location 
        /// </summary>
        public void LoadColumnLoc(DropDownList objControlFrom, DropDownList objControlTo, bool isNew)
        {
            GenericViewDTO<LocationConsult> locationColumnViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListColumn))

                try
                {
                    locationColumnViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListColumn];
                }
                catch
                {
                    locationColumnViewDTO = iLayoutMGR.GetAllColumnsLocations(context);
                    Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);
                }
            else
            {
                locationColumnViewDTO = iLayoutMGR.GetAllColumnsLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);
            }

            objControlFrom.DataSource = locationColumnViewDTO.Entities;
            objControlFrom.DataTextField = "Column";
            objControlFrom.DataValueField = "Column";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationColumnViewDTO.Entities;
            objControlTo.DataTextField = "Column";
            objControlTo.DataValueField = "Column";
            objControlTo.DataBind();

            if (isNew)
            {
                if (locationColumnViewDTO.Entities.Count > 0)
                {
                    // Agrega la opción 'Seleccione...' 
                    //objControlTo.Items[locationColumnViewDTO.Entities.Count - 1].Selected = true;


                    objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                    objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                    objControlFrom.Items[0].Selected = true;
                    objControlTo.Items[0].Selected = true;
                }
            }
        }
        /// <summary>
        /// Trae todas los levels de las ubicaciones
        /// 
        /// </summary>
        public void LoadLevelLoc(DropDownList objControlFrom, DropDownList objControlTo, bool isNew)
        {
            GenericViewDTO<LocationConsult> locationLevelViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListLevel))
                locationLevelViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListLevel];
            else
            {
                locationLevelViewDTO = iLayoutMGR.GetAllLevelLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListLevel, locationLevelViewDTO);
            }

            objControlFrom.DataSource = locationLevelViewDTO.Entities;
            objControlFrom.DataTextField = "Level";
            objControlFrom.DataValueField = "Level";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationLevelViewDTO.Entities;
            objControlTo.DataTextField = "Level";
            objControlTo.DataValueField = "Level";
            objControlTo.DataBind();

            if (isNew)
            {
                if (locationLevelViewDTO.Entities.Count > 0)
                {
                    //objControlTo.Items[locationLevelViewDTO.Entities.Count - 1].Selected = true;
                    //objControlFrom.Items[0].Selected = true;

                    objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                    objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                    objControlFrom.Items[0].Selected = true;
                    objControlTo.Items[0].Selected = true;
                }
            }
        }
        /// <summary>
        /// Trae todas las filas de las ubicaciones, 
        /// 
        /// </summary>
        public void LoadRowLoc(DropDownList objControlFrom, DropDownList objControlTo, bool isNew)
        {
            GenericViewDTO<LocationConsult> locationRowViewDTO = new GenericViewDTO<LocationConsult>();

            if (ValidateSession(WMSTekSessions.LocationRange.LocationListRow))
                locationRowViewDTO = (GenericViewDTO<LocationConsult>)Session[WMSTekSessions.LocationRange.LocationListRow];
            else
            {
                //TODO: Esto se debe reparar, esta muy lento...deberia recibir algun parametro para la busqueda
                //y cargar menos veces las querys
                locationRowViewDTO = iLayoutMGR.GetAllRowLocations(context);
                Session.Add(WMSTekSessions.LocationRange.LocationListRow, locationRowViewDTO);
            }

            objControlFrom.DataSource = locationRowViewDTO.Entities;
            objControlFrom.DataTextField = "Row";
            objControlFrom.DataValueField = "Row";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationRowViewDTO.Entities;
            objControlTo.DataTextField = "Row";
            objControlTo.DataValueField = "Row";
            objControlTo.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
            //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                if (locationRowViewDTO.Entities.Count > 0)
                {
                    //objControlTo.Items[locationRowViewDTO.Entities.Count - 1].Selected = true;
                    //objControlFrom.Items[0].Selected = true;

                    objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                    objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                    objControlFrom.Items[0].Selected = true;
                    objControlTo.Items[0].Selected = true;
                }
            }
        }

        public void LoadRowLocWihtEntities(DropDownList objControlFrom, DropDownList objControlTo, bool isNew, ContextViewDTO contextoNew)
        {
            GenericViewDTO<LocationConsult> locationRowViewDTO = new GenericViewDTO<LocationConsult>();

            //TODO: Esto se debe reparar, esta muy lento...deberia recibir algun parametro para la busqueda
            //y cargar menos veces las querys
            locationRowViewDTO = iLayoutMGR.GetAllRowWhsHangarLocations(contextoNew);
            Session.Add(WMSTekSessions.LocationRange.LocationListRow, locationRowViewDTO);

            objControlFrom.DataSource = locationRowViewDTO.Entities;
            objControlFrom.DataTextField = "Row";
            objControlFrom.DataValueField = "Row";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationRowViewDTO.Entities;
            objControlTo.DataTextField = "Row";
            objControlTo.DataValueField = "Row";
            objControlTo.DataBind();

            if (isNew)
            {
                objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                objControlFrom.Items[0].Selected = true;
                objControlTo.Items[0].Selected = true;
            }
        }
        public void LoadColumnLocWihtEntities(DropDownList objControlFrom, DropDownList objControlTo, bool isNew, ContextViewDTO contextoNew)
        {
            GenericViewDTO<LocationConsult> locationColumnViewDTO = new GenericViewDTO<LocationConsult>();

            locationColumnViewDTO = iLayoutMGR.GetAllColumnsWhsHangarLocations(contextoNew);
            Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);

            objControlFrom.DataSource = locationColumnViewDTO.Entities;
            objControlFrom.DataTextField = "Column";
            objControlFrom.DataValueField = "Column";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationColumnViewDTO.Entities;
            objControlTo.DataTextField = "Column";
            objControlTo.DataValueField = "Column";
            objControlTo.DataBind();

            if (isNew)
            {
                objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                objControlFrom.Items[0].Selected = true;
                objControlTo.Items[0].Selected = true;
            }
        }
        public void LoadLevelLocWihtEntities(DropDownList objControlFrom, DropDownList objControlTo, bool isNew, ContextViewDTO contextoNew)
        {
            GenericViewDTO<LocationConsult> locationLevelViewDTO = new GenericViewDTO<LocationConsult>();

            locationLevelViewDTO = iLayoutMGR.GetAllLevelWhsHangarLocations(contextoNew);
            Session.Add(WMSTekSessions.LocationRange.LocationListLevel, locationLevelViewDTO);

            objControlFrom.DataSource = locationLevelViewDTO.Entities;
            objControlFrom.DataTextField = "Level";
            objControlFrom.DataValueField = "Level";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationLevelViewDTO.Entities;
            objControlTo.DataTextField = "Level";
            objControlTo.DataValueField = "Level";
            objControlTo.DataBind();

            if (isNew)
            {
                objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                objControlFrom.Items[0].Selected = true;
                objControlTo.Items[0].Selected = true;
            }
        }

        public void LoadRowLocWihtEntitiesFilter(DropDownList objControlFrom, DropDownList objControlTo, bool isNew, ContextViewDTO contextoNew)
        {
            GenericViewDTO<LocationConsult> locationRowViewDTO = new GenericViewDTO<LocationConsult>();

            //TODO: Esto se debe reparar, esta muy lento...deberia recibir algun parametro para la busqueda
            //y cargar menos veces las querys
            locationRowViewDTO = iLayoutMGR.GetAllRowWhsHangarLocType(contextoNew);
            Session.Add(WMSTekSessions.LocationRange.LocationListRow, locationRowViewDTO);

            objControlFrom.DataSource = locationRowViewDTO.Entities;
            objControlFrom.DataTextField = "Row";
            objControlFrom.DataValueField = "Row";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationRowViewDTO.Entities;
            objControlTo.DataTextField = "Row";
            objControlTo.DataValueField = "Row";
            objControlTo.DataBind();

            if (isNew)
            {
                objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                objControlFrom.Items[0].Selected = true;
                objControlTo.Items[0].Selected = true;
            }
        }
        public void LoadColumnLocWihtEntitiesFilter(DropDownList objControlFrom, DropDownList objControlTo, bool isNew, ContextViewDTO contextoNew)
        {
            GenericViewDTO<LocationConsult> locationColumnViewDTO = new GenericViewDTO<LocationConsult>();

            locationColumnViewDTO = iLayoutMGR.GetAllColumnsWhsHangarLocType(contextoNew);
            Session.Add(WMSTekSessions.LocationRange.LocationListColumn, locationColumnViewDTO);

            objControlFrom.DataSource = locationColumnViewDTO.Entities;
            objControlFrom.DataTextField = "Column";
            objControlFrom.DataValueField = "Column";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationColumnViewDTO.Entities;
            objControlTo.DataTextField = "Column";
            objControlTo.DataValueField = "Column";
            objControlTo.DataBind();

            if (isNew)
            {
                objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                objControlFrom.Items[0].Selected = true;
                objControlTo.Items[0].Selected = true;
            }
        }
        public void LoadLevelLocWihtEntitiesFilter(DropDownList objControlFrom, DropDownList objControlTo, bool isNew, ContextViewDTO contextoNew)
        {
            GenericViewDTO<LocationConsult> locationLevelViewDTO = new GenericViewDTO<LocationConsult>();

            locationLevelViewDTO = iLayoutMGR.GetAllLevelWhsHangarLocType(contextoNew);
            Session.Add(WMSTekSessions.LocationRange.LocationListLevel, locationLevelViewDTO);

            objControlFrom.DataSource = locationLevelViewDTO.Entities;
            objControlFrom.DataTextField = "Level";
            objControlFrom.DataValueField = "Level";
            objControlFrom.DataBind();

            objControlTo.DataSource = locationLevelViewDTO.Entities;
            objControlTo.DataTextField = "Level";
            objControlTo.DataValueField = "Level";
            objControlTo.DataBind();

            if (isNew)
            {
                objControlFrom.Items.Insert(0, new ListItem("Sel..", "-1"));
                objControlTo.Items.Insert(0, new ListItem("Sel..", "-1"));

                objControlFrom.Items[0].Selected = true;
                objControlTo.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Carga en un ddl la lista de Tipos de Reporte Creados en el sistema
        /// </summary>
        public void LoadTypeReports(DropDownList objControl, string emptyRowText, string emptyRowValue, bool isNew)
        {

            List<string> lstReports = GetConst("ListTypeReports");

            objControl.Items.Clear();

            for (int i = 0; i < lstReports.Count; i++)
            {
                objControl.Items.Insert(i, new ListItem(lstReports[i].Split(',')[1].ToString(), lstReports[i].Split(',')[0].ToString()));
            }

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, emptyRowValue));
                objControl.Items[0].Selected = true;
            }

        }

        /// <summary>
        /// Carga en un ddl la lista de Warehouses asignadas al Usuario conectado
        /// </summary>
        public void LoadUserWarehouses(DropDownList objControl, string emptyRowText, string emptyRowValue, bool isNew)
        {
            Session[WMSTekSessions.OptionMenuSelected.SelectedIdWhs] = objControl.SelectedValue;

            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];

            if (context.SessionInfo.User.Warehouses != null)
            {
                objControl.Enabled = true;
                objControl.DataSource = context.SessionInfo.User.Warehouses;
                objControl.DataTextField = "ShortName";
                objControl.DataValueField = "Id";
                objControl.DataBind();

                if (isNew)
                {
                    // Agrega la opción 'Seleccione...' 
                    objControl.Items.Insert(0, new ListItem(emptyRowText, emptyRowValue));
                }

                foreach (ListItem item in objControl.Items)
                {
                    if (item.Value != "-1")
                    {
                        item.Attributes.Add("title", item.Text);
                    }
                }
            }
            else
            {
                objControl.Enabled = false;
            }
        }

        /// <summary>
        /// Carga en un ddl la lista de Owners asignados al Usuario conectado
        /// </summary>
        public void LoadUserOwners(DropDownList objControl, string emptyRowText, string emptyRowValue, bool isNew, string nullRowText, bool includeNulls)
        {
            Session[WMSTekSessions.OptionMenuSelected.SelectedIdOwn] = objControl.SelectedValue;

            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            if (context.SessionInfo.User.Owners != null)
            {
                objControl.Enabled = true;
                objControl.DataSource = context.SessionInfo.User.Owners;
                objControl.DataTextField = "Name";
                objControl.DataValueField = "Id";
                objControl.DataBind();

                if (includeNulls)
                {
                    // Agrega la opción '(Sin Dueño)' 
                    objControl.Items.Insert(0, new ListItem(nullRowText, "-2"));
                }

                if (isNew)
                {
                    // Agrega la opción '(Todos)' 
                    objControl.Items.Insert(0, new ListItem(emptyRowText, emptyRowValue));
                }

                foreach (ListItem item in objControl.Items)
                {
                    if (item.Value != "-1")
                    {
                        item.Attributes.Add("title", item.Text);
                    }
                }
            }
            else
            {
                objControl.Enabled = false;
            }
        }


        /// <summary>
        /// Carga lista de impresoras asociadas al usuario en sesión
        /// </summary>
        public void LoadUserPrinters(DropDownList objControl)
        {
            if (context.SessionInfo.User.Printers != null)
            {
                objControl.Enabled = true;
                objControl.DataSource = context.SessionInfo.User.Printers;
                objControl.DataTextField = "Name";
                objControl.DataValueField = "Id";
                objControl.DataBind();
            }
            else
            {
                objControl.Enabled = false;
            }
        }

        public void LoadLabel(DropDownList objControl, string sWhere, string emptyRowText, bool isNew)
        {
            GenericViewDTO<LabelTemplate> theLabelDTO = new GenericViewDTO<LabelTemplate>();
            theLabelDTO = iLabelMGR.GetByAnyParameter(new LabelTemplate(), sWhere, context);

            objControl.DataSource = theLabelDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadLabelId(DropDownList objControl, string sWhere, string emptyRowText, bool isNew)
        {
            GenericViewDTO<LabelTemplate> theLabelDTO = new GenericViewDTO<LabelTemplate>();
            theLabelDTO = iLabelMGR.GetByAnyParameter(new LabelTemplate(), sWhere, context);

            objControl.DataSource = theLabelDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadLabelSize(DropDownList objControl, Int32 idPrinter, String labelCodeType)
        {
            GenericViewDTO<LabelTemplate> theLabelDTO = new GenericViewDTO<LabelTemplate>();


            if (context.SessionInfo.User.Printers != null)
            {
                GenericViewDTO<Printer> thePrinterDTO = new GenericViewDTO<Printer>();
                Printer thePrinter = new Printer();
                thePrinter.Id = idPrinter;
                thePrinterDTO = iDeviceMGR.GetPrinterByAnyParameter(thePrinter, context);

                if (!thePrinterDTO.hasError())
                {
                    String theWhere = "AND L.LabelCode LIKE '" + labelCodeType + "%'";
                    theWhere += " AND L.IdPrinterType = " + thePrinterDTO.Entities[0].PrinterType.Id.ToString();
                    theLabelDTO = iLabelMGR.GetByAnyParameter(null, theWhere, context);

                    if (!theLabelDTO.hasError())
                    {
                        objControl.Enabled = true;
                        objControl.DataSource = theLabelDTO.Entities;
                        objControl.DataTextField = "Name";
                        objControl.DataValueField = "Id";
                        objControl.DataBind();
                    }
                    else
                    {
                        objControl.Enabled = false;
                        objControl.DataSource = null;
                        objControl.DataTextField = "Name";
                        objControl.DataValueField = "Id";
                        objControl.DataBind();
                    }
                }
            }
            else
            {
                objControl.Enabled = false;
            }
        }

        public void LoadBillingTransaction(DropDownList objControl, BillingTransaction billingTransactionParam, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingTransaction> theTransactionDTO = new GenericViewDTO<BillingTransaction>();
            theTransactionDTO = iBillingMGR.BillingTransactionGetByAnyParameter(billingTransactionParam, context);

            objControl.DataSource = theTransactionDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadBillingType(DropDownList objControl, BillingType billingType, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingType> theTypeDTO = new GenericViewDTO<BillingType>();
            theTypeDTO = iBillingMGR.BillingTypeGetByAnyParameter(billingType, context);

            objControl.DataSource = theTypeDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadBillingMode(DropDownList objControl, BillingMode billingModeParam, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingMode> theModeDTO = new GenericViewDTO<BillingMode>();
            theModeDTO = iBillingMGR.BillingModeGetByAnyParameter(billingModeParam, context);

            objControl.DataSource = theModeDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadBillingStep(DropDownList objControl, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingStep> theStepDTO = new GenericViewDTO<BillingStep>();
            theStepDTO = iBillingMGR.BillingStepGetByAnyParameter(new BillingStep(), context);

            objControl.DataSource = theStepDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadBillingContract(DropDownList objControl, int idOwn, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingContract> theContractDTO = new GenericViewDTO<BillingContract>();
            BillingContract theBillingContract = new BillingContract();
            theBillingContract.Owner.Id = idOwn;
            theContractDTO = iBillingMGR.BillingContractGetByAnyParameter(theBillingContract, context);

            objControl.DataSource = theContractDTO.Entities;
            objControl.DataTextField = "Description";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadBillingMoney(DropDownList objControl, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingMoney> theMoneyDTO = new GenericViewDTO<BillingMoney>();
            theMoneyDTO = iBillingMGR.BillingMoneyGetByAnyParameter(new BillingMoney(), context);

            objControl.DataSource = theMoneyDTO.Entities;
            objControl.DataTextField = "Description";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }

        public void LoadBillingTimeType(DropDownList objControl, string emptyRowText, bool isNew)
        {
            GenericViewDTO<BillingTimeType> theTimeTypeDTO = new GenericViewDTO<BillingTimeType>();
            theTimeTypeDTO = iBillingMGR.BillingTimeTypeGetByAnyParameter(new BillingTimeType(), context);

            objControl.DataSource = theTimeTypeDTO.Entities;
            objControl.DataTextField = "Description";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // Agrega la opción '(Todos)' 
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            }

        }
        public void LoadCarriers(DropDownList objControl, string emptyRowText, bool isNew)
        {
            var newContext = NewContext();
            var carriereDTO = iWarehousingMGR.FindAllCarrier(newContext);

            objControl.DataSource = carriereDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Selecciona Centro de Distribución por defecto del Usuario loggeado
        /// </summary>
        /// <param name="objControl"></param>
        public void SelectDefaultWarehouse(DropDownList objControl)
        {
            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            ListItem item = new ListItem();

            if (context.SessionInfo.Warehouse != null)
            {
                item.Value = context.SessionInfo.Warehouse.Id.ToString();
                item.Text = context.SessionInfo.Warehouse.ShortName;
            }

            // Valida que el item a seleccionar esté en la lista, y lo selecciona
            objControl.ClearSelection();

            if (objControl.Items.Contains(item))
                objControl.SelectedValue = item.Value;
            else
                objControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Selecciona Owner por defecto del Usuario loggeado
        /// </summary>
        /// <param name="objControl"></param>
        public void SelectDefaultOwner(DropDownList objControl)
        {
            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            ListItem item = new ListItem();

            if (context.SessionInfo.Owner != null)
            {
                item.Value = context.SessionInfo.Owner.Id.ToString();
                item.Text = context.SessionInfo.Owner.Name;
            }

            // Valida que el item a seleccionar esté en la lista, y lo selecciona
            objControl.ClearSelection();

            if (objControl.Items.Contains(item))
                objControl.SelectedValue = item.Value;
            else
                objControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Selecciona Printer por defecto del Usuario loggeado
        /// </summary>
        /// <param name="objControl"></param>
        public void SelectDefaultPrinter(DropDownList objControl)
        {
            ListItem item = new ListItem();

            if (context.SessionInfo.Printer != null)
            {
                item.Value = context.SessionInfo.Printer.Id.ToString();
                item.Text = context.SessionInfo.Printer.Name;
            }

            // Valida que el item a seleccionar esté en la lista, y lo selecciona
            if (objControl.Items.Contains(item))
                objControl.SelectedValue = item.Value;
            else
                objControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Selecciona idioma del Usuario loggeado
        /// </summary>
        /// <param name="objControl"></param>
        public void SelectDefaultLanguague(DropDownList objControl)
        {
            ListItem item = new ListItem();
            item.Value = context.SessionInfo.User.Language.Id.ToString();
            item.Text = context.SessionInfo.User.Language.Name;

            // Valida que el item a seleccionar esté en la lista, y lo selecciona
            if (objControl.Items.Contains(item))
                objControl.SelectedValue = item.Value;
            else
                objControl.SelectedIndex = 0;
        }

        public void SelectDefaultinboundType(DropDownList objControl, int value)
        {
            try
            {
                ListItem item = new ListItem();
                item.Value = value.ToString();

                objControl.SelectedValue = item.Value;
            }
            catch
            {
                objControl.SelectedValue = "0";
            }

        }

        public void LoadOwner(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Owner> ownerViewDTO = new GenericViewDTO<Owner>();

            User objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];
            ownerViewDTO.Entities = objUser.Owners;

            objControl.DataSource = ownerViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
                      //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadOwnerNew(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Owner> ownerViewDTO = new GenericViewDTO<Owner>();
            GenericViewDTO<Owner> ownerViewDTONew = new GenericViewDTO<Owner>();

            if (ValidateSession(WMSTekSessions.Shared.OwnerList))
            {
                ownerViewDTO = (GenericViewDTO<Owner>)Session[WMSTekSessions.Shared.OwnerList];
                ownerViewDTONew = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];

                if (ValidateSession(WMSTekSessions.UserMgr.OwnerList))
                    if (ownerViewDTONew.Entities.Count > 0)
                    {
                        foreach (var item in ownerViewDTONew.Entities)
                        {
                            if (ownerViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
                            {
                                ownerViewDTO.Entities.Remove(ownerViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
                            }
                        }
                    }

            }
            else
            {
                var newContext = NewContext();
                ownerViewDTO = iWarehousingMGR.FindAllOwner(newContext);

                if (ValidateSession(WMSTekSessions.UserMgr.OwnerList))
                {
                    ownerViewDTONew = (GenericViewDTO<Owner>)Session[WMSTekSessions.UserMgr.OwnerList];

                    if (ownerViewDTONew.Entities.Count > 0)
                    {
                        foreach (var item in ownerViewDTONew.Entities)
                        {
                            if (ownerViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
                            {
                                ownerViewDTO.Entities.Remove(ownerViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
                            }
                        }
                    }
                }

                Session.Add(WMSTekSessions.Shared.OwnerList, ownerViewDTO);
            }
            objControl.Items.Clear();
            objControl.DataSource = ownerViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)//TODO:La llamadas al Load deben ser 2, una en el Update y otra en el Create
            //se debe setear el isNew (guiarse por el mantenedor de LPN funciona bien).
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }
        public void LoadVendorNew(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();
            GenericViewDTO<Vendor> vendorViewDTONew = new GenericViewDTO<Vendor>();

            if (ValidateSession(WMSTekSessions.Shared.VendorList))
            {
                vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.Shared.VendorList];
                vendorViewDTONew = (GenericViewDTO<Vendor>)Session[WMSTekSessions.UserMgr.VendorList];

                if (ValidateSession(WMSTekSessions.UserMgr.VendorList))
                {
                    if (vendorViewDTONew.Entities.Count > 0)
                    {
                        foreach (var item in vendorViewDTONew.Entities)
                        {
                            if (vendorViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
                            {
                                vendorViewDTO.Entities.Remove(vendorViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
                            }
                        }
                    }
                }
            }
            else
            {
                var newContext = NewContext();
                vendorViewDTO = iWarehousingMGR.FindAllVendor(newContext);

                if (ValidateSession(WMSTekSessions.UserMgr.VendorList))
                {
                    vendorViewDTONew = (GenericViewDTO<Vendor>)Session[WMSTekSessions.UserMgr.VendorList];

                    if (vendorViewDTONew.Entities.Count > 0)
                    {
                        foreach (var item in vendorViewDTONew.Entities)
                        {
                            if (vendorViewDTO.Entities.Exists(w => w.Id.Equals(item.Id)))
                            {
                                vendorViewDTO.Entities.Remove(vendorViewDTO.Entities.Find(w => w.Id.Equals(item.Id)));
                            }
                        }
                    }
                }

                Session.Add(WMSTekSessions.Shared.VendorList, vendorViewDTO);
            }
            objControl.Items.Clear();
            objControl.DataSource = vendorViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }
        public void LoadLocationType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<LocationType> typeLocationViewDTO = new GenericViewDTO<LocationType>();

            if (ValidateSession(WMSTekSessions.Shared.LocationTypeList))
                typeLocationViewDTO = (GenericViewDTO<LocationType>)Session[WMSTekSessions.Shared.LocationTypeList];
            else
            {
                typeLocationViewDTO = iLayoutMGR.FindAllLocationType(context);
                Session.Add(WMSTekSessions.Shared.LocationTypeList, typeLocationViewDTO);
            }

            objControl.DataSource = typeLocationViewDTO.Entities;
            objControl.DataTextField = "LocTypeName";
            objControl.DataValueField = "LocTypeCode";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadLocationType2(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<LocationType> typeLocationViewDTO = new GenericViewDTO<LocationType>();

            if (ValidateSession(WMSTekSessions.Shared.LocationTypeList))
                typeLocationViewDTO = (GenericViewDTO<LocationType>)Session[WMSTekSessions.Shared.LocationTypeList];
            else
            {
                typeLocationViewDTO = iLayoutMGR.FindAllLocationType(context);
                Session.Add(WMSTekSessions.Shared.LocationTypeList, typeLocationViewDTO);
            }

            objControl.DataSource = typeLocationViewDTO.Entities;
            objControl.DataTextField = "LocTypeName";
            objControl.DataValueField = "LocTypeCode";
            objControl.DataBind();
        }

        public void LoadLocationTypeFilter(DropDownList objControl, bool isNew, string emptyRowText, List<string> locType)
        {
            GenericViewDTO<LocationType> typeLocationViewDTO = new GenericViewDTO<LocationType>();

            if (ValidateSession(WMSTekSessions.Shared.LocationTypeZoneList))
                typeLocationViewDTO = (GenericViewDTO<LocationType>)Session[WMSTekSessions.Shared.LocationTypeZoneList];
            else
            {
                typeLocationViewDTO = iLayoutMGR.FindAllLocationType(context);

                var s = from a in typeLocationViewDTO.Entities
                        join b in locType.ToArray() on a.LocTypeCode equals b
                        select a;
                typeLocationViewDTO.Entities = s.ToList();

                Session.Add(WMSTekSessions.Shared.LocationTypeZoneList, typeLocationViewDTO);
            }

            objControl.DataSource = typeLocationViewDTO.Entities;
            objControl.DataTextField = "LocTypeName";
            objControl.DataValueField = "LocTypeCode";
            objControl.DataBind();
        }

        public void LoadVAS(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<RecipeVas> recipeVASViewDTO = new GenericViewDTO<RecipeVas>();

            if (ValidateSession(WMSTekSessions.Shared.RecipeVasList))
                recipeVASViewDTO = (GenericViewDTO<RecipeVas>)Session[WMSTekSessions.Shared.RecipeVasList];
            else
            {
                recipeVASViewDTO = iWarehousingMGR.FindAllRecipeVas(context);
                Session.Add(WMSTekSessions.Shared.RecipeVasList, recipeVASViewDTO);
            }

            objControl.DataSource = recipeVASViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadVendor(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();

            if (ValidateSession(WMSTekSessions.Shared.VendorList))
                vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.Shared.VendorList];
            else
            {
                vendorViewDTO = iWarehousingMGR.FindAllVendor(context);
                Session.Add(WMSTekSessions.Shared.VendorList, vendorViewDTO);
            }

            objControl.DataSource = vendorViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            //if (isNew)
            //{
            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
            //}
        }

        public void LoadCustomer(DropDownList objControl, int idOwner, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();

            customerViewDTO = iWarehousingMGR.FindCustomerByOwner(context, idOwner);
            Session.Add(WMSTekSessions.Shared.CustomerList, customerViewDTO);

            objControl.DataSource = customerViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadCustomerId(DropDownList objControl, int idOwner, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();

            if (ValidateSession(WMSTekSessions.Shared.VendorList))
                customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.Shared.CustomerList];
            else
            {

                customerViewDTO = iWarehousingMGR.FindCustomerByOwner(context, idOwner);
                Session.Add(WMSTekSessions.Shared.CustomerList, customerViewDTO);
            }

            objControl.DataSource = customerViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWmsProcess(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WmsProcess> wmsProcessViewDTO = new GenericViewDTO<WmsProcess>();

            if (ValidateSession(WMSTekSessions.Shared.WmsProcessList))
                wmsProcessViewDTO = (GenericViewDTO<WmsProcess>)Session[WMSTekSessions.Shared.WmsProcessList];
            else
            {
                wmsProcessViewDTO = iRulesMGR.FindAllWmsProcess(context);
                Session.Add(WMSTekSessions.Shared.WmsProcessList, wmsProcessViewDTO);
            }

            objControl.DataSource = wmsProcessViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWmsProcessModule(DropDownList objControl, int idModulo, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WmsProcess> wmsProcessViewDTO = new GenericViewDTO<WmsProcess>();

            if (ValidateSession(WMSTekSessions.Shared.WmsProcessList))
                wmsProcessViewDTO = (GenericViewDTO<WmsProcess>)Session[WMSTekSessions.Shared.WmsProcessList];
            else
            {
                wmsProcessViewDTO = iRulesMGR.GetWmsProcessByIdModule(idModulo, context);
                Session.Add(WMSTekSessions.Shared.WmsProcessList, wmsProcessViewDTO);
            }

            objControl.DataSource = wmsProcessViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWmsProcess(DropDownList objControl, int idModulo, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WmsProcess> wmsProcessViewDTO = new GenericViewDTO<WmsProcess>();

            wmsProcessViewDTO = iRulesMGR.GetWmsProcessByIdModule(idModulo, context);

            objControl.DataSource = wmsProcessViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadGroupItem1(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<GrpItem1> grpItem1ViewDTO = new GenericViewDTO<GrpItem1>();

            if (ValidateSession(WMSTekSessions.Shared.GrpItem1List))
                grpItem1ViewDTO = (GenericViewDTO<GrpItem1>)Session[WMSTekSessions.Shared.GrpItem1List];
            else
            {
                grpItem1ViewDTO = iWarehousingMGR.FindAllGrpItem1(context);
                Session.Add(WMSTekSessions.Shared.GrpItem1List, grpItem1ViewDTO);
            }

            objControl.DataSource = grpItem1ViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadGroupItem1(DropDownList objControl, bool isNew, string emptyRowText, int idOwn)
        {
            GenericViewDTO<GrpItem1> grpItem1ViewDTO = new GenericViewDTO<GrpItem1>();

            GenericViewDTO<State> StateDTO = new GenericViewDTO<State>();
            StateDTO.Entities = new List<State>();

            if (idOwn > 0)
            {
                grpItem1ViewDTO = iWarehousingMGR.GetGrpItem1ByIdOwner(idOwn, context);
                //Session.Add(WMSTekSessions.Shared.GrpItem1List, grpItem1ViewDTO);
            }

            objControl.DataSource = grpItem1ViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadTransactionTypeBilling(DropDownList objControl, bool isNew, string emptyRowText)
        {
            objControl.Items.Clear();

            foreach (TransactionTypeBilling type in Enum.GetValues(typeof(TransactionTypeBilling)))
            {
                objControl.Items.Add(new ListItem(type.ToString(), EnumsDescription(type)));
            }
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }

        }

        public void LoadTransactionProcessBillingInterface(DropDownList objControl, bool isNew, string emptyRowText)
        {
            objControl.Items.Clear();

            foreach (TransactionProcessBillingInterface type in Enum.GetValues(typeof(TransactionProcessBillingInterface)))
            {
                objControl.Items.Add(new ListItem(type.ToString(), ((int)type).ToString()));
            }
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }

        }

        public void FindAllGroupItem(bool withItems, int idOwn)
        {
            // TODO: poner en CACHE / SESSION
            GenericViewDTO<GrpItem1> grpItem1ViewDTO = new GenericViewDTO<GrpItem1>();
            GenericViewDTO<GrpItem2> grpItem2ViewDTO = new GenericViewDTO<GrpItem2>();
            GenericViewDTO<GrpItem3> grpItem3ViewDTO = new GenericViewDTO<GrpItem3>();
            GenericViewDTO<GrpItem4> grpItem4ViewDTO = new GenericViewDTO<GrpItem4>();

            if (idOwn >= 0)
            {
                grpItem1ViewDTO = iWarehousingMGR.FindAllGrpItem1(context, withItems, idOwn);
                grpItem2ViewDTO = iWarehousingMGR.FindAllGrpItem2(context, withItems, idOwn);
                grpItem3ViewDTO = iWarehousingMGR.FindAllGrpItem3(context, withItems, idOwn);
                grpItem4ViewDTO = iWarehousingMGR.FindAllGrpItem4(context, withItems, idOwn);
            }

            GrpItem1 = grpItem1ViewDTO;
            GrpItem2 = grpItem2ViewDTO;
            GrpItem3 = grpItem3ViewDTO;
            GrpItem4 = grpItem4ViewDTO;
        }

        public void ConfigureDDLGrpItem1(DropDownList objControl1, bool isNew, int idgrpItem1, string emptyRowText, bool withItems, int idOwn)
        {
            if (GrpItem1 == null || GrpItem2 == null || GrpItem3 == null || GrpItem4 == null || IdOwner != idOwn)
            {
                IdOwner = idOwn;
                FindAllGroupItem(withItems, idOwn);
            }

            objControl1.Items.Clear();
            objControl1.SelectedValue = null;
            objControl1.DataSource = GrpItem1.Entities;
            objControl1.DataTextField = "Name";
            objControl1.DataValueField = "Id";
            objControl1.DataBind();

            if (isNew)
            {
                objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl1.Items[0].Selected = true;
            }
            else
            {
                if (idgrpItem1 != -1)
                {
                    objControl1.SelectedValue = idgrpItem1.ToString();
                }
                else
                {
                    objControl1.SelectedValue = null;
                    objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl1.Items[0].Selected = true;
                }
            }
        }


        public void ConfigureDDLGrpItem2(DropDownList objControl2, bool isNew, int idgrpItem1, int idgrpItem2, string emptyRowText, bool withItems, int idOwn)
        {
            if (GrpItem1 == null || GrpItem2 == null || GrpItem3 == null || GrpItem4 == null || IdOwner != idOwn)
            {
                IdOwner = idOwn;
                FindAllGroupItem(withItems, idOwn);
            }

            GenericViewDTO<GrpItem2> GrpItem2DTO = new GenericViewDTO<GrpItem2>();
            GrpItem2DTO.Entities = new List<GrpItem2>();

            //si no ha seleccionado nada, se carga el texto por defecto.
            if ((idgrpItem2 == -1) && (idgrpItem1 == -1))
            {
                if (objControl2.Items.Count < 1)
                {
                    objControl2.SelectedValue = null;
                    objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl2.Items[0].Selected = true;
                }
                else
                {
                    objControl2.SelectedValue = null;
                    objControl2.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los grpItem2s del view grpItem2
                foreach (GrpItem2 grpItem2 in GrpItem2.Entities)
                {
                    //si pertenece al grupo se agrega
                    if (grpItem2.GrpItem1.Id == idgrpItem1)
                    {
                        GrpItem2DTO.Entities.Add(grpItem2);
                    }
                }
                if (GrpItem2DTO.Entities.Count > 0)
                {
                    objControl2.SelectedValue = null;
                    objControl2.DataSource = GrpItem2DTO.Entities;
                    objControl2.DataTextField = "Name";
                    objControl2.DataValueField = "Id";
                    objControl2.DataBind();

                    if (isNew)
                    {
                        objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl2.Items[0].Selected = true;
                    }
                    else
                    {
                        //Buscar el id de item2 en la fuente de datos (items que estan en el control)
                        var items = from item in GrpItem2DTO.Entities
                                    where item.Id == idgrpItem2
                                    select item;

                        DropDownList group2Select = new DropDownList();

                        group2Select.DataSource = items;
                        group2Select.DataBind();

                        if (group2Select.Items.Count > 0)
                        {
                            if (idgrpItem2 != -1)
                            {
                                objControl2.SelectedValue = idgrpItem2.ToString();
                            }
                        }
                        else
                        {
                            objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                            objControl2.Items[0].Selected = true;
                        }
                    }
                }
                else
                {
                    objControl2.Items.Clear();
                    objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl2.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDLGrpItem3(DropDownList objControl3, bool isNew, int idgrpItem1, int idgrpItem2, int idgrpItem3, string emptyRowText, bool withItems, int idOwn)
        {
            if (GrpItem1 == null || GrpItem2 == null || GrpItem3 == null || GrpItem4 == null || IdOwner != idOwn)
            {
                IdOwner = idOwn;
                FindAllGroupItem(withItems, idOwn);
            }

            GenericViewDTO<GrpItem3> GrpItem3DTO = new GenericViewDTO<GrpItem3>();
            GrpItem3DTO.Entities = new List<GrpItem3>();

            //si no ha seleccionado nada, se carga el texto por defecto.
            if ((idgrpItem1 == -1) && (idgrpItem2 == -1) && (idgrpItem3 == -1))
            {
                //si el control tiene items
                if (objControl3.Items.Count < 1)
                {
                    objControl3.SelectedValue = null;
                    objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl3.Items[0].Selected = true;
                }
                else
                {
                    objControl3.SelectedValue = null;
                    objControl3.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los grpItem2s del view grpItem2
                foreach (GrpItem3 grpItem3 in GrpItem3.Entities)
                {
                    //si pertenece al grupo se agrega
                    if ((grpItem3.GrpItem1.Id == idgrpItem1) && (grpItem3.GrpItem2.Id == idgrpItem2))
                    {
                        GrpItem3DTO.Entities.Add(grpItem3);
                    }
                }
                if (GrpItem3DTO.Entities.Count > 0)
                {
                    objControl3.SelectedValue = null;
                    objControl3.DataSource = GrpItem3DTO.Entities;
                    objControl3.DataTextField = "Name";
                    objControl3.DataValueField = "Id";
                    objControl3.DataBind();

                    if (isNew)
                    {
                        objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl3.Items[0].Selected = true;
                    }
                    else
                    {
                        if (idgrpItem3 != -1)
                        {
                            objControl3.SelectedValue = idgrpItem3.ToString();
                        }
                    }
                }
                else
                {
                    objControl3.Items.Clear();
                    objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl3.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDLGrpItem4(DropDownList objControl4, bool isNew, int idgrpItem1, int idgrpItem2, int idgrpItem3, int idgrpItem4, string emptyRowText, bool withItems, int idOwn)
        {
            if (GrpItem1 == null || GrpItem2 == null || GrpItem3 == null || GrpItem4 == null || IdOwner != idOwn)
            {
                IdOwner = idOwn;
                FindAllGroupItem(withItems, idOwn);
            }

            GenericViewDTO<GrpItem4> GrpItem4DTO = new GenericViewDTO<GrpItem4>();
            GrpItem4DTO.Entities = new List<GrpItem4>();

            //si no ha seleccionado nada, se carga el texto por defecto.
            if ((idgrpItem4 == -1) && (idgrpItem3 == -1) && (idgrpItem2 == -1) && (idgrpItem1 == -1))
            {
                if (objControl4.Items.Count < 1)
                {
                    objControl4.SelectedValue = null;
                    objControl4.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl4.Items[0].Selected = true;
                }
                else
                {
                    objControl4.SelectedValue = null;
                    objControl4.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los grpItem2s del view grpItem2
                foreach (GrpItem4 grpItem4 in GrpItem4.Entities)
                {
                    //si pertenece al grupo se agrega
                    if ((grpItem4.GrpItem1.Id == idgrpItem1) && (grpItem4.GrpItem2.Id == idgrpItem2) && (grpItem4.GrpItem3.Id == idgrpItem3))
                    {
                        GrpItem4DTO.Entities.Add(grpItem4);
                    }
                }
                if (GrpItem4DTO.Entities.Count > 0)
                {
                    objControl4.SelectedValue = null;
                    objControl4.DataSource = GrpItem4DTO.Entities;
                    objControl4.DataTextField = "Name";
                    objControl4.DataValueField = "Id";
                    objControl4.DataBind();

                    if (isNew)
                    {
                        objControl4.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl4.Items[0].Selected = true;
                    }
                    else
                    {
                        if (idgrpItem4 != -1)
                        {
                            objControl4.SelectedValue = idgrpItem4.ToString();
                        }
                    }
                }
                else
                {
                    objControl4.Items.Clear();
                    objControl4.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl4.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDLGrpItem1New(DropDownList objControl1, bool isNew, int idgrpItem1, string emptyRowText, bool withItems, int idOwn)
        {
            GenericViewDTO<GrpItem1> grpItem1ViewDTO = new GenericViewDTO<GrpItem1>();
            grpItem1ViewDTO = iWarehousingMGR.FindAllGrpItem1(context, withItems, idOwn);

            GrpItem1 = grpItem1ViewDTO;

            objControl1.SelectedValue = null;
            objControl1.DataSource = GrpItem1.Entities;
            objControl1.DataTextField = "Name";
            objControl1.DataValueField = "Id";
            objControl1.DataBind();

            if (isNew)
            {
                objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl1.Items[0].Selected = true;
            }
            else
            {
                if (idgrpItem1 != -1)
                {
                    objControl1.SelectedValue = idgrpItem1.ToString();
                }
                else
                {
                    objControl1.SelectedValue = null;
                    objControl1.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl1.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDLGrpItem2New(DropDownList objControl2, bool isNew, int idgrpItem1, int idgrpItem2, string emptyRowText, bool withItems, int idOwn)
        {
            //FindAllGroupItem(false, idOwn);

            GenericViewDTO<GrpItem2> grpItem2ViewDTO = new GenericViewDTO<GrpItem2>();
            grpItem2ViewDTO = iWarehousingMGR.FindAllGrpItem2(context, withItems, idOwn);

            GrpItem2 = grpItem2ViewDTO;

            GenericViewDTO<GrpItem2> GrpItem2DTO = new GenericViewDTO<GrpItem2>();
            GrpItem2DTO.Entities = new List<GrpItem2>();

            //si no ha seleccionado nada, se carga el texto por defecto.
            if ((idgrpItem2 == -1) && (idgrpItem1 == -1))
            {
                if (objControl2.Items.Count < 1)
                {
                    objControl2.SelectedValue = null;
                    objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl2.Items[0].Selected = true;
                }
                else
                {
                    objControl2.SelectedValue = null;
                    objControl2.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los grpItem2s del view grpItem2
                foreach (GrpItem2 grpItem2 in GrpItem2.Entities)
                {
                    //si pertenece al grupo se agrega
                    if (grpItem2.GrpItem1.Id == idgrpItem1)
                    {
                        GrpItem2DTO.Entities.Add(grpItem2);
                    }
                }
                if (GrpItem2DTO.Entities.Count > 0)
                {
                    objControl2.SelectedValue = null;
                    objControl2.DataSource = GrpItem2DTO.Entities;
                    objControl2.DataTextField = "Name";
                    objControl2.DataValueField = "Id";
                    objControl2.DataBind();

                    if (isNew)
                    {
                        objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl2.Items[0].Selected = true;
                    }
                    else
                    {
                        //Buscar el id de item2 en la fuente de datos (items que estan en el control)
                        var items = from item in GrpItem2DTO.Entities
                                    where item.Id == idgrpItem2
                                    select item;

                        DropDownList group2Select = new DropDownList();

                        group2Select.DataSource = items;
                        group2Select.DataBind();

                        if (group2Select.Items.Count > 0)
                        {
                            if (idgrpItem2 != -1)
                            {
                                objControl2.SelectedValue = idgrpItem2.ToString();
                            }
                        }
                        else
                        {
                            objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                            objControl2.Items[0].Selected = true;
                        }
                    }
                }
                else
                {
                    objControl2.Items.Clear();
                    objControl2.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl2.Items[0].Selected = true;
                }
            }
        }


        public void ConfigureDDLGrpItem3New(DropDownList objControl3, bool isNew, int idgrpItem1, int idgrpItem2, int idgrpItem3, string emptyRowText, bool withItems, int idOwn)
        {
            GenericViewDTO<GrpItem3> grpItem3ViewDTO = new GenericViewDTO<GrpItem3>();
            grpItem3ViewDTO = iWarehousingMGR.FindAllGrpItem3(context, withItems, idOwn);

            GrpItem3 = grpItem3ViewDTO;

            GenericViewDTO<GrpItem3> GrpItem3DTO = new GenericViewDTO<GrpItem3>();
            GrpItem3DTO.Entities = new List<GrpItem3>();

            //si no ha seleccionado nada, se carga el texto por defecto.
            if ((idgrpItem1 == -1) && (idgrpItem2 == -1) && (idgrpItem3 == -1))
            {
                //si el control tiene items
                if (objControl3.Items.Count < 1)
                {
                    objControl3.SelectedValue = null;
                    objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl3.Items[0].Selected = true;
                }
                else
                {
                    objControl3.SelectedValue = null;
                    objControl3.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los grpItem2s del view grpItem2
                foreach (GrpItem3 grpItem3 in GrpItem3.Entities)
                {
                    //si pertenece al grupo se agrega
                    if ((grpItem3.GrpItem1.Id == idgrpItem1) && (grpItem3.GrpItem2.Id == idgrpItem2))
                    {
                        GrpItem3DTO.Entities.Add(grpItem3);
                    }
                }
                if (GrpItem3DTO.Entities.Count > 0)
                {
                    objControl3.SelectedValue = null;
                    objControl3.DataSource = GrpItem3DTO.Entities;
                    objControl3.DataTextField = "Name";
                    objControl3.DataValueField = "Id";
                    objControl3.DataBind();

                    if (isNew)
                    {
                        objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl3.Items[0].Selected = true;
                    }
                    else
                    {
                        if (idgrpItem3 != -1)
                        {
                            objControl3.SelectedValue = idgrpItem3.ToString();
                        }
                    }
                }
                else
                {
                    objControl3.Items.Clear();
                    objControl3.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl3.Items[0].Selected = true;
                }
            }
        }

        public void ConfigureDDLGrpItem4New(DropDownList objControl4, bool isNew, int idgrpItem1, int idgrpItem2, int idgrpItem3, int idgrpItem4, string emptyRowText, bool withItems, int idOwn)
        {
            GenericViewDTO<GrpItem4> grpItem4ViewDTO = new GenericViewDTO<GrpItem4>();
            grpItem4ViewDTO = iWarehousingMGR.FindAllGrpItem4(context, withItems, idOwn);

            GrpItem4 = grpItem4ViewDTO;

            GenericViewDTO<GrpItem4> GrpItem4DTO = new GenericViewDTO<GrpItem4>();
            GrpItem4DTO.Entities = new List<GrpItem4>();

            //si no ha seleccionado nada, se carga el texto por defecto.
            if ((idgrpItem4 == -1) && (idgrpItem3 == -1) && (idgrpItem2 == -1) && (idgrpItem1 == -1))
            {
                if (objControl4.Items.Count < 1)
                {
                    objControl4.SelectedValue = null;
                    objControl4.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl4.Items[0].Selected = true;
                }
                else
                {
                    objControl4.SelectedValue = null;
                    objControl4.Items[0].Selected = true;
                }
            }
            else
            {
                //recorre los grpItem2s del view grpItem2
                foreach (GrpItem4 grpItem4 in GrpItem4.Entities)
                {
                    //si pertenece al grupo se agrega
                    if ((grpItem4.GrpItem1.Id == idgrpItem1) && (grpItem4.GrpItem2.Id == idgrpItem2) && (grpItem4.GrpItem3.Id == idgrpItem3))
                    {
                        GrpItem4DTO.Entities.Add(grpItem4);
                    }
                }
                if (GrpItem4DTO.Entities.Count > 0)
                {
                    objControl4.SelectedValue = null;
                    objControl4.DataSource = GrpItem4DTO.Entities;
                    objControl4.DataTextField = "Name";
                    objControl4.DataValueField = "Id";
                    objControl4.DataBind();

                    if (isNew)
                    {
                        objControl4.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                        objControl4.Items[0].Selected = true;
                    }
                    else
                    {
                        if (idgrpItem4 != -1)
                        {
                            objControl4.SelectedValue = idgrpItem4.ToString();
                        }
                    }
                }
                else
                {
                    objControl4.Items.Clear();
                    objControl4.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl4.Items[0].Selected = true;
                }
            }
        }

        /// <summary>
        /// Carga lista de Hangares, según Warehouse
        /// </summary>
        public void LoadHangarActive(DropDownList objControl, int idWarehouse, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();
            hangarViewDTO.Entities = new List<Hangar>();

            if (ValidateSession(WMSTekSessions.Shared.HangarList))
                hangarViewDTO = (GenericViewDTO<Hangar>)Session[WMSTekSessions.Shared.HangarList];
            else
            {
                hangarViewDTO = iLayoutMGR.FindAllHangar(context, false);
                Session.Add(WMSTekSessions.Shared.HangarList, hangarViewDTO);
            }

            //Buscar el id de hangar que corresponda al warehouse que se seleccionó
            var hangars = from hangar in hangarViewDTO.Entities
                          where hangar.Warehouse.Id == idWarehouse
                          where hangar.Status == true
                          select hangar;

            // Carga control
            objControl.Items.Clear();
            objControl.DataSource = hangars;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Carga lista de Hangares, según Warehouse
        /// </summary>
        public void LoadHangar(DropDownList objControl, int idWarehouse, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Hangar> hangarViewDTO = new GenericViewDTO<Hangar>();
            hangarViewDTO.Entities = new List<Hangar>();

            if (ValidateSession(WMSTekSessions.Shared.HangarList))
                hangarViewDTO = (GenericViewDTO<Hangar>)Session[WMSTekSessions.Shared.HangarList];
            else
            {
                hangarViewDTO = iLayoutMGR.FindAllHangar(context, false);
                Session.Add(WMSTekSessions.Shared.HangarList, hangarViewDTO);
            }

            //Buscar el id de hangar que corresponda al warehouse que se seleccionó
            var hangars = from hangar in hangarViewDTO.Entities
                          where hangar.Warehouse.Id == idWarehouse
                          select hangar;

            // Carga control
            objControl.Items.Clear();
            objControl.DataSource = hangars;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }



        public void LoadPrinters(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Printer> printerViewDTO = new GenericViewDTO<Printer>();
            if (ValidateSession(WMSTekSessions.Shared.PrinterList))
                printerViewDTO = (GenericViewDTO<Printer>)Session[WMSTekSessions.Shared.PrinterList];
            else
            {
                var newContext = NewContext();
                printerViewDTO = iDeviceMGR.FindAllPrinter(newContext);
                Session.Add(WMSTekSessions.Shared.PrinterList, printerViewDTO);
            }

            objControl.DataSource = printerViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadPrintServers(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<PrintServer> printServerViewDTO = new GenericViewDTO<PrintServer>();

            if (ValidateSession(WMSTekSessions.Shared.PrintServerList))
                printServerViewDTO = (GenericViewDTO<PrintServer>)Session[WMSTekSessions.Shared.PrintServerList];
            else
            {
                printServerViewDTO = iDeviceMGR.FindAllPrintServer(context);
                Session.Add(WMSTekSessions.Shared.PrintServerList, printServerViewDTO);
            }

            objControl.DataSource = printServerViewDTO.Entities;
            objControl.DataTextField = "ServerName";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadPrinterTypes(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<PrinterType> printerTypeViewDTO = new GenericViewDTO<PrinterType>();

            if (ValidateSession(WMSTekSessions.Shared.PrinterTypeList))
                printerTypeViewDTO = (GenericViewDTO<PrinterType>)Session[WMSTekSessions.Shared.PrinterTypeList];
            else
            {
                printerTypeViewDTO = iDeviceMGR.FindAllPrinterType(context);
                Session.Add(WMSTekSessions.Shared.PrinterTypeList, printerTypeViewDTO);
            }

            objControl.DataSource = printerTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadPtlTypes(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<PtlType> ptlTypeViewDTO = new GenericViewDTO<PtlType>();

            if (ValidateSession(WMSTekSessions.Shared.PtlTypeList))
                ptlTypeViewDTO = (GenericViewDTO<PtlType>)Session[WMSTekSessions.Shared.PtlTypeList];
            else
            {
                ptlTypeViewDTO = iWarehousingMGR.FindAllPtlType(context);
                Session.Add(WMSTekSessions.Shared.PtlTypeList, ptlTypeViewDTO);
            }

            objControl.DataSource = ptlTypeViewDTO.Entities;
            objControl.DataTextField = "PtlTypeName";
            objControl.DataValueField = "PtlTypeCode";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true; 
            }
        }

        public void LoadReasonLessFilter(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Reason> reasonViewDTO = new GenericViewDTO<Reason>();
            ContextViewDTO contextReason = new ContextViewDTO();

            if (ValidateSession(WMSTekSessions.ReasonMgr.ReasonList))
                reasonViewDTO = (GenericViewDTO<Reason>)Session[WMSTekSessions.ReasonMgr.ReasonList];
            else
            {
                reasonViewDTO = iWarehousingMGR.FindAllReason(contextReason);
                Session.Add(WMSTekSessions.ReasonMgr.ReasonList, reasonViewDTO);
            }

            objControl.DataSource = reasonViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadReasonFilterByTypeInOut(DropDownList objControl, TypeInOut typeInOut, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Reason> reasonViewDTO = new GenericViewDTO<Reason>();

            reasonViewDTO = iWarehousingMGR.GetReasonByTypeInOut((int)typeInOut, context);

            objControl.DataSource = reasonViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadReasonFilterByTypeInOutNew(DropDownList objControl, TypeInOut typeInOut, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Reason> reasonViewDTO = new GenericViewDTO<Reason>();

            reasonViewDTO = iWarehousingMGR.GetReasonByTypeInOut((int)typeInOut, context);

            objControl.DataSource = reasonViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem("Disponible", "-1"));

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, ""));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWorkZone(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();

            if (ValidateSession(WMSTekSessions.Shared.WorkZoneList))
                workZoneViewDTO = (GenericViewDTO<WorkZone>)Session[WMSTekSessions.Shared.WorkZoneList];
            else
            {
                workZoneViewDTO = iLayoutMGR.FindAllworkZone(context);
                Session.Add(WMSTekSessions.Shared.WorkZoneList, workZoneViewDTO);
            }

            objControl.DataSource = workZoneViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWorkZoneByWhs(DropDownList objControl, int idWhs, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();

            workZoneViewDTO = iLayoutMGR.GetWorkZoneByWhs(idWhs, context);
            Session.Add(WMSTekSessions.Shared.WorkZoneList, workZoneViewDTO);

            objControl.DataSource = workZoneViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWorkZoneByWhsWhitOutSession(DropDownList objControl, int idWhs, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();

            workZoneViewDTO = iLayoutMGR.GetWorkZoneByWhs(idWhs, context);

            objControl.DataSource = workZoneViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWorkZoneByWhsAndTypeZone(DropDownList objControl, int idWhs, int typeWorkZone, bool isNew, string emptyRowText)
        {
            GenericViewDTO<WorkZone> workZoneViewDTO = new GenericViewDTO<WorkZone>();

            workZoneViewDTO = iLayoutMGR.GetWorkZoneByWhsAndTypeZone(idWhs, typeWorkZone, context);
            Session.Add(WMSTekSessions.Shared.WorkZoneList, workZoneViewDTO);

            objControl.DataSource = workZoneViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadWorkZoneByWorkZoneList(DropDownList objControl, bool isNew, GenericViewDTO<WorkZone> WorkZoneList, string emptyRowText)
        {
            objControl.DataSource = null;
            objControl.DataBind();

            if (WorkZoneList == null)
            {
                WorkZoneList = new GenericViewDTO<WorkZone>();
            }

            objControl.DataSource = WorkZoneList.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        /// <summary>
        /// Se ocupa para llenar un dropdown list con los tipos de Razones en el mantenedor de razones
        /// </summary>
        public void LoadTypeInOut(DropDownList objControl, string emptyRowText)
        {
            //if (objControl.Items.Count == 0)
            //{
            //    objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            //    objControl.Items.Insert(1, new ListItem(TypeInOut.System.ToString(), "0"));
            //    objControl.Items.Insert(2, new ListItem(TypeInOut.Inbound.ToString(), "1"));
            //    objControl.Items.Insert(3, new ListItem(TypeInOut.Outbound.ToString(), "2"));
            //    objControl.Items.Insert(4, new ListItem(TypeInOut.Both.ToString(), "3"));
            //    objControl.Items.Insert(5, new ListItem(TypeInOut.QualityControl.ToString(), "4"));
            //}

            if (objControl.Items.Count == 0)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));

                foreach (var item in Enum.GetValues(typeof(TypeInOut)))
                {
                    objControl.Items.Insert(((int)item + 1), new ListItem(item.ToString(), ((int)item).ToString()));
                }
            }

            objControl.Items[0].Selected = true;
        }

        public void LoadConst(DropDownList objControl, string module, bool isNew, string emptyRowText)
        {
            List<String> listObject = new List<string>();

            listObject = GetConst(module);

            objControl.Items.Clear();

            for (int i = 0; i < listObject.Count; i++)
            {

                if (listObject[i].Split(',').Count() > 1)
                {
                    objControl.Items.Insert(i, new ListItem(listObject[i].Split(',')[1].ToString(), listObject[i].Split(',')[0].ToString()));
                }
                else
                {
                    objControl.Items.Insert(i, new ListItem(listObject[i].ToString(), listObject[i].ToString()));
                }

            }

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }

        }


        public void LoadLpnType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();

            if (ValidateSession(WMSTekSessions.Shared.LpnTypeList))
                lpnTypeViewDTO = (GenericViewDTO<LPNType>)Session[WMSTekSessions.Shared.LpnTypeList];
            else
            {
                lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(context);
                Session.Add(WMSTekSessions.Shared.LpnTypeList, lpnTypeViewDTO);
            }

            objControl.DataSource = lpnTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadLpnTypeNew(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();

            if (ValidateSession(WMSTekSessions.Shared.LpnTypeList))
                lpnTypeViewDTO = (GenericViewDTO<LPNType>)Session[WMSTekSessions.Shared.LpnTypeList];
            else
            {
                lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(context);
                Session.Add(WMSTekSessions.Shared.LpnTypeList, lpnTypeViewDTO);
            }

            objControl.DataSource = lpnTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadLpnTypeActive(DropDownList objControl, int idOwner, bool isNew, string emptyRowText)
        {
            GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();
            lpnTypeViewDTO.Entities = new List<LPNType>();

            if (ValidateSession(WMSTekSessions.Shared.LpnTypeList))
                lpnTypeViewDTO = (GenericViewDTO<LPNType>)Session[WMSTekSessions.Shared.LpnTypeList];
            else
            {
                lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(context);
                Session.Add(WMSTekSessions.Shared.LpnTypeList, lpnTypeViewDTO);
            }

            //Buscar el id de hangar que corresponda al warehouse que se seleccionó
            var lpnTypes = from lpnType in lpnTypeViewDTO.Entities
                           where lpnType.Owner.Id == idOwner
                           where lpnType.Status == true
                           select lpnType;

            // Carga control
            objControl.Items.Clear();
            objControl.DataSource = lpnTypes;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadCategoryItemByOwner(DropDownList objControl, int idOwn, bool isNew, bool refresh, string emptyRowText)
        {
            GenericViewDTO<CategoryItem> categoryItemViewDTO = new GenericViewDTO<CategoryItem>();
            try
            {
                if (ValidateSession(WMSTekSessions.Shared.CategoryItemList) && !refresh)
                    categoryItemViewDTO = (GenericViewDTO<CategoryItem>)Session[WMSTekSessions.Shared.CategoryItemList];
                else
                {
                    categoryItemViewDTO = iWarehousingMGR.GetCategoryItemByOwner(idOwn, context);
                    Session.Add(WMSTekSessions.Shared.CategoryItemList, categoryItemViewDTO);
                }

                objControl.DataSource = categoryItemViewDTO.Entities;
                objControl.DataTextField = "Name";
                objControl.DataValueField = "Id";
                objControl.DataBind();

                if (isNew)
                {
                    objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                    objControl.Items[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void LoadTruckType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TruckType> truckTypeViewDTO = new GenericViewDTO<TruckType>();

            if (ValidateSession(WMSTekSessions.Shared.TruckTypeList))
                truckTypeViewDTO = (GenericViewDTO<TruckType>)Session[WMSTekSessions.Shared.TruckTypeList];
            else
            {
                truckTypeViewDTO = iWarehousingMGR.FindAllTruckType(context);
                Session.Add(WMSTekSessions.Shared.TruckTypeList, truckTypeViewDTO);
            }

            objControl.DataSource = truckTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadMovementType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<MovementType> movementTypeViewDTO = new GenericViewDTO<MovementType>();

            movementTypeViewDTO = iWarehousingMGR.FindAllMovementTypeMovement(context);

            objControl.DataSource = movementTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadTaskType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TaskType> taskTypeViewDTO = new GenericViewDTO<TaskType>();

            taskTypeViewDTO = iTasksMGR.FindAllTaskType(context);

            objControl.DataSource = taskTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadUomType(DropDownList objControl, int idOwn, bool isNew, string emptyRowText)
        {
            GenericViewDTO<UomType> uomTypeViewDTO = new GenericViewDTO<UomType>();
            var newContext = NewContext();
            uomTypeViewDTO = iWarehousingMGR.GetUomType_ByOwn(newContext, idOwn);

            objControl.DataSource = uomTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Carga lista de Vendor, segun Owner seleccionado
        /// </summary>
        public void LoadVendorByOwner(DropDownList objControl, int idOwner, string emptyRowText)
        {
            GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();

            vendorViewDTO = iWarehousingMGR.GetVendorByOwner(context, idOwner);

            objControl.DataSource = vendorViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadDriver(ListBox objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<Driver> driverViewDTO = new GenericViewDTO<Driver>();

            driverViewDTO = iWarehousingMGR.FindAllDriver(context);
            objControl.DataSource = driverViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }

        }
        public void LoadInboundType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindAllInboundType(context);
            objControl.DataSource = inboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadInboundTypeFilter(DropDownList objControl, bool isNew, string emptyRowText, string[] lista)
        {
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();
            GenericViewDTO<InboundType> inTypeViewDTO = new GenericViewDTO<InboundType>();

            // TODO: cargar en session
            inboundTypeViewDTO = iReceptionMGR.FindAllInboundType(context);

            if (lista.Count() != 0)
            {
                IEnumerable<InboundType> varType;

                varType = from a in inboundTypeViewDTO.Entities
                          where lista.Contains(a.Code.Trim())
                          select a;

                if (varType.Count() < 1)
                {
                    varType = from a in inboundTypeViewDTO.Entities
                              where lista.Contains(a.Id.ToString())
                              select a;
                }

                inboundTypeViewDTO = new GenericViewDTO<InboundType>();
                foreach (InboundType inType in varType)
                {
                    inboundTypeViewDTO.Entities.Add(inType);
                }
            }

            objControl.DataSource = inboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadTrackInbound(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TrackInboundType> trackInboundTypeViewDTO = new GenericViewDTO<TrackInboundType>();

            trackInboundTypeViewDTO = iReceptionMGR.FindAllTrackInboundType(context);
            objControl.DataSource = trackInboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        protected void LoadTrackCloseInbound(DropDownList objControl, Int32 orderId, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TrackInboundType> trackInboundTypeViewDTO = new GenericViewDTO<TrackInboundType>();

            trackInboundTypeViewDTO = iReceptionMGR.GetTrackInboundTypeByCloseStatus(context, orderId);
            objControl.DataSource = trackInboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        protected void LoadTrackInboundClose(DropDownList objControl, Int32 orderId, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TrackInboundType> trackInboundTypeViewDTO = new GenericViewDTO<TrackInboundType>();

            trackInboundTypeViewDTO = iReceptionMGR.GetTrackInboundTypeByStatusClose(context, orderId);
            objControl.DataSource = trackInboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadTrackInbound(DropDownList objControl, bool isNew, string emptyRowText, string[] listTrack)
        {
            GenericViewDTO<TrackInboundType> trackInboundTypeViewDTO = new GenericViewDTO<TrackInboundType>();
            GenericViewDTO<TrackInboundType> trackTypeViewDTO = new GenericViewDTO<TrackInboundType>();
            TrackInboundType inboundTrack;

            trackInboundTypeViewDTO = iReceptionMGR.FindAllTrackInboundType(context);

            IEnumerable<TrackInboundType> varTrack;

            varTrack = from a in trackInboundTypeViewDTO.Entities
                       where listTrack.Contains(a.Id.ToString())
                       select a;

            foreach (TrackInboundType item in varTrack)
            {
                inboundTrack = new TrackInboundType();
                inboundTrack = item;
                trackTypeViewDTO.Entities.Add(inboundTrack);
            }

            objControl.DataSource = trackTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadAllOutboundType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<OutboundType> outboundTypeViewDTO = new GenericViewDTO<OutboundType>();
            GenericViewDTO<OutboundType> outTypeViewDTO = new GenericViewDTO<OutboundType>();

            // TODO: cargar en session
            outboundTypeViewDTO = iDispatchingMGR.FindAllOutboundType(context);

            objControl.DataSource = outboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadAllOutboundTypeNew(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<OutboundType> outboundTypeViewDTO = new GenericViewDTO<OutboundType>();
            GenericViewDTO<OutboundType> outTypeViewDTO = new GenericViewDTO<OutboundType>();

            // TODO: cargar en session
            outboundTypeViewDTO = iDispatchingMGR.FindAllOutboundType(context);

            objControl.DataSource = outboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Code";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadOutboundType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<OutboundType> outboundTypeViewDTO = new GenericViewDTO<OutboundType>();
            GenericViewDTO<OutboundType> outTypeViewDTO = new GenericViewDTO<OutboundType>();
            OutboundType outboundType;
            // TODO: cargar en session
            outboundTypeViewDTO = iDispatchingMGR.FindAllOutboundType(context);


            //Quita la "Orden Picking Wave" ya que esta se genera en forma automatica, nunca manual
            foreach (OutboundType Oty in outboundTypeViewDTO.Entities)
            {
                if (Oty.Code != "PIKWV")
                {
                    outboundType = new OutboundType();
                    outboundType = Oty;
                    outTypeViewDTO.Entities.Add(outboundType);
                }
            }

            objControl.DataSource = outTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadOutboundTypeFilter(DropDownList objControl, bool isNew, string emptyRowText, string[] lista)
        {
            GenericViewDTO<OutboundType> outboundTypeViewDTO = new GenericViewDTO<OutboundType>();
            GenericViewDTO<OutboundType> outTypeViewDTO = new GenericViewDTO<OutboundType>();
            OutboundType outboundType;
            // TODO: cargar en session
            outboundTypeViewDTO = iDispatchingMGR.FindAllOutboundType(context);


            if (lista.Count() != 0)
            {

                IEnumerable<OutboundType> varType;

                varType = from a in outboundTypeViewDTO.Entities
                          where lista.Contains(a.Code.Trim())
                          select a;
                if (varType.Count() < 1)
                {
                    varType = from a in outboundTypeViewDTO.Entities
                              where lista.Contains(a.Id.ToString())
                              select a;
                }


                foreach (OutboundType item in varType)
                {
                    outboundType = new OutboundType();
                    outboundType = item;
                    outTypeViewDTO.Entities.Add(outboundType);
                }
            }
            else
            {
                //Quita la "Orden Picking Wave" ya que esta se genera en forma automatica, nunca manual
                foreach (OutboundType Oty in outboundTypeViewDTO.Entities)
                {
                    if (Oty.Code != "PIKWV")
                    {
                        outboundType = new OutboundType();
                        outboundType = Oty;
                        outTypeViewDTO.Entities.Add(outboundType);
                    }
                }
            }

            objControl.DataSource = outTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        public void LoadReferenceDocType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<ReferenceDocType> referenceDocTypeTypeViewDTO = new GenericViewDTO<ReferenceDocType>();

            referenceDocTypeTypeViewDTO = iWarehousingMGR.FindAllReferenceDocType(context);
            objControl.DataSource = referenceDocTypeTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                // TODO: parametrizar "seleccione...
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// Carga sólo los tipo de documentos nacional e internacional
        /// </summary>
        public void LoadInboundTypeNacInt(DropDownList objControl, bool isNew, string emptyRowText)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindNacIntInboundType(context);

            objControl.DataSource = inboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }
        public void LoadInboundTypeDev(DropDownList objControl, bool isNew, string emptyRowText)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindDevInboundType(context);

            objControl.DataSource = inboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }
        public void LoadInboundTypeProd(DropDownList objControl, bool isNew, string emptyRowText)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindProdInboundType(context);

            objControl.DataSource = inboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }
        public void LoadInboundTypeTrasp(DropDownList objControl, bool isNew, string emptyRowText)
        {
            // TODO: cargar desde session en BasePage
            GenericViewDTO<InboundType> inboundTypeViewDTO = new GenericViewDTO<InboundType>();

            inboundTypeViewDTO = iReceptionMGR.FindTraspInboundType(context);

            objControl.DataSource = inboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadScope(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<CfgParameter> cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();

            if (ValidateSession(WMSTekSessions.Shared.ScopeList))
                cfgParameterViewDTO = (GenericViewDTO<CfgParameter>)Session[WMSTekSessions.Shared.ScopeList];
            else
            {
                cfgParameterViewDTO = iConfigurationMGR.FindAllScopes(context);
                Session.Add(WMSTekSessions.Shared.ScopeList, cfgParameterViewDTO);
            }

            objControl.DataSource = cfgParameterViewDTO.Entities;
            objControl.DataTextField = "Scope";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadKardexType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<KardexType> cfgParameterViewDTO = new GenericViewDTO<KardexType>();

            if (ValidateSession(WMSTekSessions.Shared.KardexTypeList))
                cfgParameterViewDTO = (GenericViewDTO<KardexType>)Session[WMSTekSessions.Shared.KardexTypeList];
            else
            {
                cfgParameterViewDTO = iWarehousingMGR.FindAllKardexType(context);
                Session.Add(WMSTekSessions.Shared.KardexTypeList, cfgParameterViewDTO);
            }

            objControl.DataSource = cfgParameterViewDTO.Entities;
            objControl.DataTextField = "KardexCode";
            objControl.DataValueField = "KardexCode";
            objControl.DataBind();

            objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadTrackTaskType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TrackTaskType> trackTaskTypeViewDTO = new GenericViewDTO<TrackTaskType>();

            trackTaskTypeViewDTO = iTasksMGR.FindAllTrackTaskType(context);

            objControl.DataSource = trackTaskTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadTrackTaskTypeFilter(DropDownList objControl, bool isNew, string emptyRowText, string[] lista)
        {
            GenericViewDTO<TrackTaskType> trackTaskTypeViewDTO = new GenericViewDTO<TrackTaskType>();

            trackTaskTypeViewDTO = iTasksMGR.FindAllTrackTaskType(context);

            if (lista.Count() != 0)
            {
                IEnumerable<TrackTaskType> varTrack;

                varTrack = from a in trackTaskTypeViewDTO.Entities
                           where lista.Contains(a.Name.Trim())
                           select a;

                if (varTrack.Count() < 1)
                {
                    varTrack = from a in trackTaskTypeViewDTO.Entities
                               where lista.Contains(a.Id.ToString())
                               select a;
                }

                trackTaskTypeViewDTO = new GenericViewDTO<TrackTaskType>();

                foreach (TrackTaskType track in varTrack)
                {
                    trackTaskTypeViewDTO.Entities.Add(track);
                }
            }

            objControl.DataSource = trackTaskTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }

        public void LoadTrackOutboundType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<TrackOutboundType> trackOutboundTypeViewDTO = new GenericViewDTO<TrackOutboundType>();

            trackOutboundTypeViewDTO = iDispatchingMGR.FindAllTrackOutboundType(context);

            objControl.DataSource = trackOutboundTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }
        public void LoadDispatchType(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<DispatchType> dispatchTypeViewDTO = new GenericViewDTO<DispatchType>();

            dispatchTypeViewDTO = iDispatchingMGR.FindAllDispatchType(context);

            objControl.DataSource = dispatchTypeViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
            objControl.Items[0].Selected = true;
        }


        /// <summary>
        /// Carga lista de Grupo de Regas
        /// </summary>
        public void LoadCustomRules(DropDownList objControl, bool isNew, string emptyRowText)
        {
            GenericViewDTO<CustomRule> cusmtomRuleViewDTO = new GenericViewDTO<CustomRule>();
            cusmtomRuleViewDTO.Entities = new List<CustomRule>();

            if (ValidateSession(WMSTekSessions.Shared.CustomRuleList))
                cusmtomRuleViewDTO = (GenericViewDTO<CustomRule>)Session[WMSTekSessions.Shared.CustomRuleList];
            else
            {
                cusmtomRuleViewDTO = iRulesMGR.FindAllCustomRule(context);
                Session.Add(WMSTekSessions.Shared.CustomRuleList, cusmtomRuleViewDTO);
            }


            // Carga control
            objControl.Items.Clear();
            objControl.DataSource = cusmtomRuleViewDTO.Entities;
            objControl.DataTextField = "Name";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }


        public void LoadTypeWorkZone(DropDownList objControl, bool isNew, string emptyRowText)
        {
            objControl.Items.Clear();

            foreach (TypeWorkZone type in Enum.GetValues(typeof(TypeWorkZone)))
            {
                objControl.Items.Add(new ListItem(type.ToString(), ((int)type).ToString()));
            }
            objControl.DataBind();

            if (isNew)
            {
                objControl.ClearSelection();
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
        }
        #endregion

        #region GRIDVIEW

        /// <summary>
        /// Para grilla Obout
        /// </summary>
        public void RowDataBound(object sender, GridRowEventArgs e)
        {
            if (e.Row.RowType == GridRowType.DataRow)
            {
                string editIndex = e.Row.DataItemIndex.ToString();

                ImageButton btn = e.Row.FindControl("btnEdit") as ImageButton;
                if (btn != null) { btn.CommandArgument = editIndex; }
            }
        }

        /// <summary>
        /// Agrega el indice de cada fila a los botones de la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string editIndex = e.Row.DataItemIndex.ToString();

                ImageButton btn = e.Row.FindControl("btnEdit") as ImageButton;
                if (btn != null)
                {
                    btn.CommandArgument = editIndex;
                    Label lbltoolTipEdith = (Label)Master.FindControl("lblToolTipEdit");
                    btn.ToolTip = lbltoolTipEdith.Text;
                }
                btn = e.Row.FindControl("btnDelete") as ImageButton;
                if (btn != null)
                {
                    btn.CommandArgument = editIndex;
                    Label lbltoolTipDelete = (Label)Master.FindControl("lblToolTipDelete");
                    btn.ToolTip = lbltoolTipDelete.Text;
                }
                btn = e.Row.FindControl("btnSelect") as ImageButton;
                if (btn != null)
                {
                    btn.CommandArgument = editIndex;
                    Label lblToolTipSelect = (Label)Master.FindControl("lblToolTipSelect");
                    btn.ToolTip = lblToolTipSelect.Text;
                }
                btn = e.Row.FindControl("btnClose") as ImageButton;
                if (btn != null)
                {
                    btn.CommandArgument = editIndex;
                    Label lblToolTipClose = (Label)Master.FindControl("lblToolTipClose");
                    btn.ToolTip = lblToolTipClose.Text;
                }
                btn = e.Row.FindControl("btnNextTrack") as ImageButton;
                if (btn != null)
                {
                    btn.CommandArgument = editIndex;

                }

                btn = e.Row.FindControl("btnFilter") as ImageButton;
                if (btn != null)
                {
                    btn.CommandArgument = editIndex;
                    Label lblToolTipFilter = (Label)Master.FindControl("lblToolTipFilter");
                    btn.ToolTip = lblToolTipFilter.Text;
                }
            }
        }

        /// <summary>
        /// En una grilla, obtiene el indice de la columna, a partir de la propiedad AccessibleHeaderText
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="accessibleHeaderText"></param>
        /// <returns></returns>
        private int GetColumnIndex(GridView grd, string accessibleHeaderText)
        {
            for (int index = 0; index < grd.Columns.Count; index++)
            {
                if (String.Compare(grd.Columns[index].AccessibleHeaderText, accessibleHeaderText, true) == 0)
                {
                    return index;
                }
            }
            return -1;
        }

        protected int GetColumnIndex(DataControlFieldCollection grdColumns, string accessibleHeaderText)
        {
            for (int index = 0; index < grdColumns.Count; index++)
            {
                if (String.Compare(grdColumns[index].AccessibleHeaderText, accessibleHeaderText, true) == 0)
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Configura el ORDEN y visibilidad de las las columnas de la grilla
        /// </summary>
        /// <param name="configuration"></param>
        protected void ConfigureGridByProperties(GridView grd, List<PropertyDTO> Properties)
        {
            int i;
            DataControlField field;

            // Las Properties estan ordenadadas por la columna FieldOrder
            foreach (PropertyDTO property in Properties)
            {
                i = GetColumnIndex(grd.Columns, property.FieldName);

                if (i > -1 && i < grd.Columns.Count)
                {
                    field = grd.Columns[i];
                    grd.Columns.RemoveAt(i);
                    if (property.VisibleGrid)
                    {
                        if (property.GridWidth > 0)
                        {
                            field.ItemStyle.Width = property.GridWidth;
                            field.ItemStyle.Wrap = false;
                        }

                        grd.Columns.Add(field);
                    }
                }
            }
        }

        /// <summary>
        /// Configura el ORDEN de las columnas de la grilla
        /// </summary>
        /// <param name="configuration"></param>
        protected void ConfigureGridOrder(GridView grd, List<PropertyDTO> Properties)
        {
            int i;
            DataControlField field;

            // Las Properties estan ordenadadas por la columna FieldOrder
            foreach (PropertyDTO property in Properties)
            {
                i = GetColumnIndex(grd.Columns, property.FieldName);

                if (i > -1 && i < grd.Columns.Count)
                {
                    //if (!string.IsNullOrEmpty(property.GridMask))
                    //{
                    //    //Formatea los datos de acuerdo a la mascara ingresada
                    //    if (grd.Columns[i] is BoundField)
                    //    {
                    //        var fieldData = grd.Columns[i] as BoundField;
                    //        if (fieldData != null)
                    //            fieldData.DataFormatString = property.GridMask.Trim();
                    //    }else{
                    //        var templField = grd.Columns[i] as TemplateField;
                    //        if (templField != null)
                    //        {
                    //            object a = templField.ItemTemplate.GetType();                                
                    //        }
                    //    }
                    //}

                    field = grd.Columns[i];
                    grd.Columns.RemoveAt(i);
                    grd.Columns.Add(field);
                }
            }
        }

        /// <summary>
        /// Configura VISIBILIDAD de las columnas de la grilla
        /// </summary>
        /// <param name="configuration"></param>
        protected void ConfigureGridVisible(GridView grd, List<PropertyDTO> Properties)
        {
            int i = -1;

            foreach (PropertyDTO property in Properties)
            {
                // Configura propiedad 'Visible'
                i = GetColumnIndex(grd, property.FieldName);

                if (i > -1 && i < grd.Columns.Count)
                {
                    grd.Columns[i].Visible = property.VisibleGrid;
                }
            }
        }

        protected void ConfigureGridSort(GridView grd)
        {
            for (int i = 0; i < grd.Columns.Count; i++)
            {
                grd.Columns[i].SortExpression = grd.Columns[i].AccessibleHeaderText;
            }
        }

        static public void LoopConfigure(Control control, List<PropertyDTO> Properties, bool isNew)
        {
            // TODO: modificar para soportar cualquier control 
            // TODO: revisar logica - pasa muchas veces por el mismo control
            foreach (Control c in control.Controls)
            {
                foreach (PropertyDTO property in Properties)
                {
                    // Configura propiedades 'Visible', 'Required', 'Enabled' y 'Default Value'
                    string rfvName = "rfv" + property.FieldName;
                    string txtName = "txt" + property.FieldName;
                    string ddlName = "ddl" + property.FieldName;
                    string chkName = "chk" + property.FieldName;
                    string divName = "div" + property.FieldName;
                    string lblName = "lbl" + property.FieldName;

                    // Propiedad 'Visible'
                    if (control.FindControl(divName) != null)
                        control.FindControl(divName).Visible = property.VisibleEditNew;


                    // Propiedad 'Required'
                    if (control.FindControl(rfvName) != null)
                        control.FindControl(rfvName).Visible = property.Required;

                    // Propiedad 'Default Value' (solo para modo 'New')
                    if (isNew && property.DefaultValue != null)
                    {
                        if (control.FindControl(lblName) != null)
                            ((Label)control.FindControl(lblName)).Text = property.DefaultValue;

                        if (control.FindControl(txtName) != null)
                            ((TextBox)control.FindControl(txtName)).Text = property.DefaultValue;

                        if (control.FindControl(chkName) != null && property.DefaultValue != string.Empty)
                            ((CheckBox)control.FindControl(chkName)).Checked = Convert.ToBoolean(Convert.ToInt16(property.DefaultValue));

                        if (control.FindControl(ddlName) != null && property.DefaultValue != string.Empty)
                            ((DropDownList)control.FindControl(ddlName)).SelectedValue = property.DefaultValue;
                    }

                    // Propiedad 'Enabled' Y 'Width'
                    if (control.FindControl(txtName) != null)
                    {
                        ((TextBox)control.FindControl(txtName)).Enabled = property.Enabled;
                        ((TextBox)control.FindControl(txtName)).Width = property.FieldWidth;
                    }

                    if (control.FindControl(ddlName) != null)
                    {
                        ((DropDownList)control.FindControl(ddlName)).Enabled = property.Enabled;
                        ((DropDownList)control.FindControl(ddlName)).Width = property.FieldWidth;
                    }

                    if (control.FindControl(chkName) != null)
                    {
                        ((CheckBox)control.FindControl(chkName)).Enabled = property.Enabled;
                        ((CheckBox)control.FindControl(chkName)).Width = property.FieldWidth;
                    }
                }

                if (c.HasControls())
                {
                    LoopConfigure(c, Properties, isNew);
                }
            }
        }

        /// <summary>
        /// Recorre lista de propiedades del DTO y configura los controles en los modos 'New' y 'Edit'
        /// </summary>
        /// <param name="configuration"></param>
        protected void ConfigureModal(List<PropertyDTO> Properties, bool isNew)
        {
            LoopConfigure(this, Properties, isNew);
        }

        #endregion

        #region FUNCIONES VARIAS
        /// <summary>
        /// Permite elegir entre diferentes combinaciones para las propiedades 'FirstName' y 'LastName'
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string FormatName(string firstName, string lastName, string format)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return "";
            }
            else
            {
                switch (format)
                {
                    case Constants.Nombre_Apellido:  // Nombre Apellido
                        return firstName + " " + lastName;
                    case Constants.Apellido_Nombre:  // Apellido, Nombre
                        return lastName + ", " + firstName;
                    default:
                        return firstName + " " + lastName;
                }
            }
        }

        /// <summary>
        /// Abandona la sesion y redirecciona a la pagina de Log Out
        /// 
        /// </summary>
        protected void Logout()
        {
            this.Session.RemoveAll();
            Session.Abandon();
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            //Response.Redirect("~/DetectScreen.aspx", false);
            RemoveCookiesForSession();
            Response.Redirect("~/Account/Logout.aspx", false);
        }
        protected void RemoveCookiesForSession()
        {
            if (Request.Cookies[WMSTekSessions.Global.NETSessionId] != null)
            {
                Response.Cookies[WMSTekSessions.Global.NETSessionId].Value = string.Empty;
                Response.Cookies[WMSTekSessions.Global.NETSessionId].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies[WMSTekSessions.Global.AuthToken] != null)
            {
                Response.Cookies[WMSTekSessions.Global.AuthToken].Value = string.Empty;
                Response.Cookies[WMSTekSessions.Global.AuthToken].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        public bool ValidateNumber(List<String> validateList)
        {
            bool bandera = true;
            util = new MiscUtils();

            foreach (string item in validateList)
            {
                if (item != "")
                {
                    if (util.IsNumber(item)) bandera = true;
                    else
                    {
                        bandera = false;
                        break;
                    }
                }
            }
            return bandera;
        }
        /// <summary>
        /// Valida el valor dependiendo de tipo, máximo y mínimo.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public GenericViewDTO<CfgParameter> GenericValidate(string type, string value, string max, string min)
        {
            util = new MiscUtils();
            Decimal decMax = 0;
            Decimal decMin = 0;
            DateTime dateMax = DateTime.MinValue;
            DateTime dateMin = DateTime.MinValue;
            bool IsString = true;
            GenericViewDTO<CfgParameter> paramError = new GenericViewDTO<CfgParameter>();
            //TODO:Implementar culture, por los formatos de decimales

            //Primero se chequea que tipo de dato es el mín y el máx
            try
            {
                decMax = Convert.ToDecimal(max);
                decMin = Convert.ToDecimal(min);
                IsString = false;
            }
            catch
            {
                //se sabe que es string o datetime...continúa
                IsString = true;
            }
            if (IsString)
            {
                try
                {
                    //si lo convierte es fecha
                    dateMax = Convert.ToDateTime(max);
                    dateMin = Convert.ToDateTime(min);
                    IsString = false;
                }
                catch
                {
                    IsString = true;
                }
            }
            switch (type)
            {
                case "STRING":
                    //TODO: Agregar la validacion de los rangos en forma alfabética.
                    if (IsString)
                    {
                        //TODO:Valida suponiendo el rango  alfabetico??
                    }
                    else
                    {
                        //Valida el largo del string
                        if (value.Length > decMax || value.Length < decMin)
                        {
                            paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.Overflow);
                        }
                    }
                    break;
                case "INT":
                    //Valida que sea número
                    if (util.IsNumber(value))
                    {
                        Int64 intValue = Convert.ToInt64(value);
                        if (intValue > decMax || intValue < decMin)
                        {
                            paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.Overflow);
                        }
                    }
                    else
                    {
                        //Envia error "el formato no es correcto"
                        paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.IncorrectFormat);
                    }
                    break;
                case "DEC":
                    //Valida que sea numero DECIMAL
                    if (util.IsNumber(value))
                    {
                        decimal decValue = Convert.ToDecimal(value);
                        if (decValue > decMax || decValue < decMin)
                        {
                            paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.Overflow);
                        }
                        else
                        {
                            //TODO:Envia error "El valor ha sobrepasado el máximo o el mínimo permitido"
                        }
                    }
                    else
                    {
                        //TODO:Envia error "el tipo de dato debe ser numérico"
                        paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.IncorrectFormat);
                    }
                    break;

                case "DATETIME":
                    //Valida que sea numero
                    if (!IsString)
                    {
                        DateTime dateValue = Convert.ToDateTime(value);
                        if (dateValue > dateMax || dateValue < dateMin)
                        {
                            paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.Overflow);
                        }
                    }
                    else
                    {
                        paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.IncorrectFormat);
                    }
                    break;

                case "BOOL":
                    //Valida que sea numero
                    if (util.IsNumber(value))
                    {
                        try
                        {
                            bool boolValue = Convert.ToBoolean(Convert.ToInt16(value));
                        }
                        catch
                        {
                            paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.IncorrectFormat);
                            break;
                        }
                    }
                    else
                    {
                        paramError.Errors = new ErrorDTO(WMSTekError.BusinessAdmin.Parameter.IncorrectFormat);
                    }
                    break;

            }
            return paramError;
        }

        public void ExportToExcelOld(GridView grdMaster, GridView grdDetail, String detailTitle)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            string style = @"<style> .text { mso-number-format:\@; } </style> ";
            string fileName = currentPageTitle + "_" + DateTime.Now.ToShortDateString();

            fileName = fileName.Replace(" ", "_").Replace(">", "").Replace("<", "").Replace(":", "").Replace("%", "").Replace("/", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("°", "");

            Page page = new Page();
            HtmlForm form = new HtmlForm();
            Table table = new Table();
            TableRow titleRow = new TableRow();
            TableRow dateRow = new TableRow();
            TableHeaderCell titleCell = new TableHeaderCell();
            TableCell dateCell = new TableCell();

            PrepareGridViewForExport(grdMaster);

            // Title
            titleCell.Text = currentPageTitle;
            titleCell.HorizontalAlign = HorizontalAlign.Left;
            titleCell.Font.Size = 12;
            titleRow.Cells.Add(titleCell);
            table.Rows.Add(titleRow);

            // Export Date
            dateCell.Text = "Fecha Consulta: " + DateTime.Now.ToShortDateString();
            dateRow.Cells.Add(dateCell);
            table.Rows.Add(dateRow);

            // Empty row
            table.Rows.Add(new TableRow());

            form.Controls.Add(table);

            // Grilla master
            form.Controls.Add(grdMaster);

            // Grilla detalle
            if (detailTitle != null)
            {
                Table tableDetail = new Table();
                TableRow titleRowDetail = new TableRow();
                TableHeaderCell titleCellDetail = new TableHeaderCell();

                PrepareGridViewForExport(grdDetail);

                // Empty rows
                tableDetail.Rows.Add(new TableRow());
                tableDetail.Rows.Add(new TableRow());

                // Detail title
                titleCellDetail.Text = detailTitle;
                titleCellDetail.HorizontalAlign = HorizontalAlign.Left;
                titleCellDetail.Font.Size = 11;
                titleRowDetail.Cells.Add(titleCellDetail);
                tableDetail.Rows.Add(titleRowDetail);

                form.Controls.Add(tableDetail);
                form.Controls.Add(grdDetail);
            }

            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
            page.RenderControl(htw);

            Response.Clear();
            Response.Buffer = true;

            // TODO: averiguar que ContentType conviene
            //Response.ContentType = "application/vnd.ms-excel";
            Response.ContentType = "application/excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.Default;

            Response.Write(style);
            Response.Write(sb.ToString());

            Response.End();
        }

        public void ExportToExcelOld(GridView grdMaster, string masterTitle, GridView grdDetail, String detailTitle)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            string style = @"<style> .text { mso-number-format:\@; } </style> ";
            string fileName = masterTitle + "_" + DateTime.Now.ToShortDateString();

            fileName = fileName.Replace(" ", "_").Replace(">", "").Replace("<", "").Replace(":", "").Replace("%", "").Replace("/", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("°", "");

            Page page = new Page();
            HtmlForm form = new HtmlForm();
            Table table = new Table();
            TableRow titleRow = new TableRow();
            TableRow dateRow = new TableRow();
            TableHeaderCell titleCell = new TableHeaderCell();
            TableCell dateCell = new TableCell();

            PrepareGridViewForExport(grdMaster);

            // Title
            titleCell.Text = masterTitle;
            titleCell.HorizontalAlign = HorizontalAlign.Left;
            titleCell.Font.Size = 12;
            titleRow.Cells.Add(titleCell);
            table.Rows.Add(titleRow);

            // Export Date
            dateCell.Text = "Fecha Consulta: " + DateTime.Now.ToShortDateString();
            dateRow.Cells.Add(dateCell);
            table.Rows.Add(dateRow);

            // Empty row
            table.Rows.Add(new TableRow());

            form.Controls.Add(table);

            // Grilla master
            form.Controls.Add(grdMaster);

            // Grilla detalle
            if (detailTitle != null)
            {
                Table tableDetail = new Table();
                TableRow titleRowDetail = new TableRow();
                TableHeaderCell titleCellDetail = new TableHeaderCell();

                PrepareGridViewForExport(grdDetail);

                // Empty rows
                tableDetail.Rows.Add(new TableRow());
                tableDetail.Rows.Add(new TableRow());

                // Detail title
                titleCellDetail.Text = detailTitle;
                titleCellDetail.HorizontalAlign = HorizontalAlign.Left;
                titleCellDetail.Font.Size = 11;
                titleRowDetail.Cells.Add(titleCellDetail);
                tableDetail.Rows.Add(titleRowDetail);

                form.Controls.Add(tableDetail);
                form.Controls.Add(grdDetail);
            }

            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
            page.RenderControl(htw);

            Response.Clear();
            Response.Buffer = true;

            // TODO: averiguar que ContentType conviene
            //Response.ContentType = "application/vnd.ms-excel";
            Response.ContentType = "application/excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.Default;

            Response.Write(style);
            Response.Write(sb.ToString());

            Response.End();
        }

        public void ExportToExcel(GridView grdMaster, GridView grdDetail, String detailTitle, bool csvFormat = true)
        {
            var downloadFileWillBeCsv = DownloadFileWillBeCsv();

            if (downloadFileWillBeCsv)
                ExportToCsv(grdMaster, grdDetail, detailTitle);
            else
                ExportToExcel(grdMaster, "Hoja1", grdDetail, detailTitle);
        }

        public void ExportToExcel(GridView grdMaster, string masterTitle, GridView grdDetail, string detailTitle)
        {
            
            //reviso si viene un parametro con el detalle (grdDetail), si es asi extraigo su id y tipo, si no es asi es porque es una cabecera, por tanto recibo el estado directamente
            string tipoDoc = "";
            if (grdDetail != null && detailTitle != null)
            {
                tipoDoc = "_Detalle_" + detailTitle.Substring(detailTitle.LastIndexOf('_') + 1);

            }
            else
            {
                tipoDoc = detailTitle;
            }

            var workbook = CreateExcel(grdMaster, masterTitle, grdDetail, detailTitle);
            if (detailTitle != null)
            { 
                workbook = CreateExcel(grdMaster, masterTitle, grdDetail, detailTitle.Substring(0, detailTitle.IndexOf("_") + 1).Replace("_", ""));
            }

            if (workbook != null)
            {
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = currentPageTitle.Replace(" ", "_") + "_" + DateTime.Now.ToShortDateString() + tipoDoc;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xls");

                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream, false);
                    memoryStream.WriteTo(Response.OutputStream);
                }

                Response.End();
            }
        }

        private XLWorkbook CreateExcel(GridView grdMaster, string title, GridView grdDetail, string detailTitle)
        {
            XLWorkbook workbook = null;


            if (grdMaster != null && grdMaster.Rows.Count > 0)
            {
                workbook = new XLWorkbook();
                var worksheetMaster = workbook.Worksheets.Add(title);
                int indexStart = 5;

                FirstParagraphExcel(worksheetMaster);
                CreateHeaderExcel(grdMaster, worksheetMaster);
                int lastIndexRow = CreateBodyExcel(grdMaster, worksheetMaster, indexStart + 1, detailTitle);

                if (grdDetail != null && grdDetail.Rows.Count > 0)
                {
                    CreateHeaderExcel(grdDetail, worksheetMaster, lastIndexRow);
                    CreateBodyExcel(grdDetail, worksheetMaster, lastIndexRow + 1);
                }
            }

            return workbook;
        }

        private void FirstParagraphExcel(IXLWorksheet worksheet, int indexRow = 2)
        {
            worksheet.Cell(indexRow, 1).Value = currentPageTitle;
            worksheet.Cell(indexRow, 1).DataType = XLCellValues.Text;
            worksheet.Cell(indexRow, 1).Style.Font.Bold = true;

            worksheet.Cell(++indexRow, 1).Value = "Fecha Consulta: " + DateTime.Now.ToShortDateString();
            worksheet.Cell(++indexRow, 1).DataType = XLCellValues.Text;
        }

        private void CreateHeaderExcel(GridView grid, IXLWorksheet worksheet, int indexRow = 5)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                int indexColumn = 1;
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    if (grid.Columns[i].Visible)
                    {
                        String header = grid.Columns[i].HeaderText;

                        worksheet.Cell(indexRow, indexColumn).Value = header;
                        worksheet.Cell(indexRow, indexColumn).DataType = XLCellValues.Text;
                        worksheet.Cell(indexRow, indexColumn).Style.Font.Bold = true;
                        worksheet.Cell(indexRow, indexColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ++indexColumn;
                    }
                }
            }
        }

        private int CreateBodyExcel(GridView grid, IXLWorksheet worksheet, int indexRow = 2, string detailTitle = null)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                int indexColumn = 1;
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    if (!grid.Columns[i].Visible)
                        continue;

                    string value = string.Empty;
                    DateTime dateInExcel = DateTime.MinValue;
                    XLCellValues typeData = XLCellValues.Text;

                    if (row.Cells[i].Controls.Count > 0)
                    {
                        foreach (Control control in row.Cells[i].Controls)
                        {
                            Label label = control as Label;
                            if (label != null)
                            {
                                var date = isStringADateTime(label.Text.Trim());
                                if (date != null)
                                {
                                    typeData = XLCellValues.DateTime;
                                    dateInExcel = ((DateTime)date);
                                }
                                else
                                {
                                    value = label.Text.Trim();
                                }
                                break;
                            }

                            if (label == null)
                            {
                                HyperLink link = control as HyperLink;

                                if (link != null)
                                {
                                    var date = isStringADateTime(link.Text.Trim());
                                    if (date != null)
                                    {
                                        typeData = XLCellValues.DateTime;
                                        dateInExcel = ((DateTime)date);
                                    }
                                    else
                                    {
                                        value = link.Text.Trim();
                                    }
                                }
                            }

                            if (label == null)
                            {
                                CheckBox check = control as CheckBox;
                                if (check != null)
                                {
                                    if (check.Checked)
                                    {
                                        value = "SI";
                                    }
                                    else
                                    {
                                        value = "NO";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(row.Cells[i].Text))
                        {
                            if (!row.Cells[i].Text.Equals("&nbsp;"))
                            {
                                var date = isStringADateTime(value = row.Cells[i].Text);
                                if (date != null)
                                {
                                    typeData = XLCellValues.DateTime;
                                    dateInExcel = ((DateTime)date);
                                }
                                else
                                {
                                    value = value = row.Cells[i].Text;
                                }

                            }
                        }
                    }

                    if (value.Contains(","))
                        value = value.Replace(",", ".");

                    if (!string.IsNullOrEmpty(value) && GetLetterOccurence(value, '.') <= 1)
                    {
                        if (value.Length > 1 && value.Substring(0, 1) == "0" && !value.Contains('.'))
                        {
                            typeData = XLCellValues.Text;
                        }
                        else
                        {
                            int testInt;
                            if (int.TryParse(value, out testInt))
                                typeData = XLCellValues.Number;

                            double testDouble;
                            if (double.TryParse(value, out testDouble) && typeData != XLCellValues.Number)
                                typeData = XLCellValues.Number;
                        }
                    }

                    if (typeData == XLCellValues.Text)
                        value = "'" + value;

                    if (typeData == XLCellValues.DateTime)
                        worksheet.Cell(indexRow, indexColumn).Value = dateInExcel;
                    else
                        worksheet.Cell(indexRow, indexColumn).Value = value.Trim();

                    worksheet.Cell(indexRow, indexColumn).DataType = typeData;
                    worksheet.Cell(indexRow, indexColumn).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dateInExcel = DateTime.MinValue;
                    ++indexColumn;
                }

                indexRow++;
            }

            if (!string.IsNullOrEmpty(detailTitle))
            {
                worksheet.Cell(++indexRow, 1).Value = detailTitle;
                worksheet.Cell(++indexRow, 1).DataType = XLCellValues.Text;
                ++indexRow;
            }

            return indexRow;
        }

        private static int GetLetterOccurence(string inPutString, char letterToBeChecked)
        {
            char[] chars = inPutString.ToArray();

            var result = (from c in chars
                          where c.Equals(letterToBeChecked)
                          select c).Count();
            return result;
        }

        private DateTime? isStringADateTime(string strDate)
        {
            DateTime finalDate = DateTime.MinValue;
            DateTime finalDateAndTime = DateTime.MinValue;
            DateTime finalDateAndTime2 = DateTime.MinValue;
            DateTime finalDateAndTime3 = DateTime.MinValue;
            DateTime finalDateAndTime4 = DateTime.MinValue;



            bool isValidDate = DateTime.TryParseExact(strDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out finalDate);
            bool isValidDateAndTime = DateTime.TryParseExact(strDate, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out finalDateAndTime);
            bool isValidDateAndTime2 = DateTime.TryParseExact(strDate, "dd/MM/yyyy H:mm:ss", null, System.Globalization.DateTimeStyles.None, out finalDateAndTime2);

            bool isValidDateAndTime3 = DateTime.TryParseExact(strDate, "dd/MM/yyyy H:mm:ss", null, System.Globalization.DateTimeStyles.None, out finalDateAndTime3);
            bool isValidDateAndTime4 = DateTime.TryParseExact(strDate, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out finalDateAndTime4);

            if (isValidDate)
            {
                return finalDate;
            }
            else if (isValidDateAndTime)
            {
                return finalDateAndTime;
            }
            else if (isValidDateAndTime2)
            {
                return finalDateAndTime2;
            }
            else if (isValidDateAndTime3)
            {
                return finalDateAndTime3;
            }
            else if (isValidDateAndTime4)
            {
                return finalDateAndTime4;
            }
            else
            {
                return null;
            }
        }

        public void ExportToCsv(GridView grdMaster, GridView grdDetail, String detailTitle)
        {
            ExportToCsv(grdMaster, "Hoja1", grdDetail, detailTitle);
        }

        public void ExportToCsv(GridView grdMaster, string masterTitle, GridView grdDetail, string detailTitle)
        {
            string fileName = currentPageTitle.Replace(" ", "_") + "-" + DateTime.Now.ToShortDateString();

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".csv");
            Response.Charset = "utf-8";
            Response.ContentType = "application/text";
            Response.ContentEncoding = Encoding.UTF8;

            Response.Output.Write((char)65279 + CreateCsv(grdMaster, grdDetail, detailTitle));
            Response.Flush();
            Response.End();
        }

        private string CreateCsv(GridView grdMaster, GridView grdDetail, string detailTitle)
        {
            char separator = ';';
            var sb = new StringBuilder();

            CreateHeaderCsv(sb, separator, grdMaster);
            CreateBodyCsv(sb, separator, grdMaster);

            if (grdDetail != null && grdDetail.Rows.Count > 0)
            {
                CreateHeaderCsv(sb, separator, grdDetail);
                CreateBodyCsv(sb, separator, grdDetail);
            }

            return sb.ToString();
        }

        private void FirstParagraphCsv(StringBuilder sb, char separator)
        {
            if (sb != null)
            {
                sb.Append(currentPageTitle + separator);
                sb.Append("Fecha Consulta: " + DateTime.Now.ToShortDateString() + separator);

                sb.Append(newCsvLine());
            }
        }

        private void FirstParagraphDetailCsv(StringBuilder sb, char separator, string detailTitle)
        {
            if (sb != null)
            {
                sb.Append(detailTitle + separator);

                sb.Append(newCsvLine());
            }
        }

        private void CreateHeaderCsv(StringBuilder sb, char separator, GridView grid)
        {
            if (sb != null)
            {
                for (int k = 0; k < grid.Columns.Count; k++)
                {
                    if (!grid.Columns[k].Visible)
                        continue;

                    sb.Append(grid.Columns[k].HeaderText + separator);
                }

                sb.Append(newCsvLine());
            }
        }

        private void CreateBodyCsv(StringBuilder sb, char separator, GridView grid)
        {
            if (sb != null)
            {
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    for (int k = 0; k < grid.Columns.Count; k++)
                    {
                        if (!grid.Columns[k].Visible)
                            continue;

                        string value = string.Empty;

                        if (grid.Rows[i].Cells[k].Controls.Count > 0)
                        {
                            bool isGetValue = false;
                            foreach (Control control in grid.Rows[i].Cells[k].Controls)
                            {
                                Label label = control as Label;
                                if (label != null)
                                {
                                    value = label.Text;
                                    isGetValue = true;
                                    break;
                                }

                                CheckBox check = control as CheckBox;
                                if (check != null)
                                {
                                    if (check.Checked)
                                        value = "SI";
                                    else
                                        value = "NO";

                                    isGetValue = true;
                                    break;
                                }

                                HyperLink link = control as HyperLink;
                                if (link != null)
                                {
                                    value = link.Text;
                                    isGetValue = true;
                                    break;
                                }
                            }

                            if (!isGetValue)
                                value = "";
                        }
                        else
                        {
                            if (!grid.Rows[i].Cells[k].Text.Equals("&nbsp;"))
                                value = grid.Rows[i].Cells[k].Text;
                            else
                                value = "";
                        }

                        value = ReplaceUncommonCharacters(value.Trim());

                        sb.Append(value.Replace(",", ".") + separator);
                    }

                    sb.Append(newCsvLine());
                }
            }
        }

        private static string ReplaceUncommonCharacters(string value)
        {
            var newValue = value.Replace("&#174;", "®")
                                .Replace("&#160;", "");

            return newValue.Trim();
        }

        private static string newCsvLine()
        {
            return "\r\n";
        }

        /// <summary>
        /// Da formato de texto a todas las celdas de la grilla
        /// </summary>
        /// <param name="gv"></param>
        private void PrepareGridViewForExport(Control gv)
        {
            //LinkButton lb = new LinkButton();
            Literal l = new Literal();
            string name = String.Empty;

            for (int i = 0; i < gv.Controls.Count; i++)
            {
                if (gv.Controls[i].GetType() == typeof(LinkButton))
                {
                    l.Text = (gv.Controls[i] as LinkButton).Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(DropDownList))
                {
                    l.Text = (gv.Controls[i] as DropDownList).SelectedItem.Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(CheckBox))
                {
                    // TODO: traducir 
                    l.Text = (gv.Controls[i] as CheckBox).Checked ? "Sí" : "No";
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(Calendar))
                {
                    l.Text = (gv.Controls[i] as Calendar).SelectedDate.ToShortDateString();
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(ImageButton))
                {
                    l.Text = "";
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(Image))
                {
                    l.Text = "";
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                if (gv.Controls[i].HasControls())
                {
                    PrepareGridViewForExport(gv.Controls[i]);
                }
            }
        }
        /// <summary>
        /// Trae las constantes de un archivo XML
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public List<string> GetConst(string tagName)
        {
            string pathXml;
            XmlDocument xDoc = new XmlDocument();
            List<string> listConst = new List<string>();
            try
            {
                pathXml = baseControl.ConstantPath;
                xDoc.Load(pathXml);

                XmlNodeList Parametros = xDoc.ChildNodes;
                XmlNodeList listaRutaInstall = ((XmlElement)Parametros[1]).GetElementsByTagName(tagName);

                foreach (XmlElement nodo in listaRutaInstall)
                {
                    //XmlNodeList nPath = nodo.GetElementsByTagName("valor");
                    XmlNodeList nPath = nodo.GetElementsByTagName(nodo.FirstChild.Name);
                    foreach (XmlElement nodes in nPath)
                    {
                        listConst.Add(nodes.InnerText);
                    }
                }

                if (listConst.Count == 0)
                {
                    throw new Exception("Constante " + tagName + " no encontrada");
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return listConst;
        }


        public string GetCfgParameter(string parameterName)
        {
            string result = string.Empty;
            try
            {
                var param = context.CfgParameters.FirstOrDefault(w => w.ParameterCode.ToUpper().Equals((parameterName).ToUpper()));

                if (param != null)
                    result = param.ParameterValue.Trim();
                else
                    throw new Exception("Parámetro " + parameterName + " no encontrado");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result;
        }

        /// <summary>
        /// Ordena de los items del DropDownList de manera alfabetica
        /// </summary>
        /// <param name="ddlList"></param>
        public void AlphabeticalOrderDropDownList(DropDownList ddlList)
        {
            string[] array = new string[ddlList.Items.Count];

            List<ListItem> l = new List<ListItem>();
            foreach (ListItem li in ddlList.Items)
            {
                l.Add(li);
            }

            ddlList.DataSource = l.OrderBy(order => order.Text);
            ddlList.DataTextField = "Text";
            ddlList.DataValueField = "Value";
            ddlList.DataBind();

        }

        /// <summary>
        /// Entrega los dias habiles existentes entre las fechas ingresadas
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public int GetBusinessDays(DateTime dateFrom, DateTime dateTo)
        {
            int dayNro = 0;
            TimeSpan ts = dateTo - dateFrom;

            GenericViewDTO<NonWorkingDay> days = iWarehousingMGR.GetAllBetweenDate(dateFrom, dateTo, context);

            if (!days.hasError())
            {
                dayNro = days.Entities.Count();
            }

            dayNro = ts.Days - dayNro;

            if (dayNro < 0)
                dayNro = 0;

            return dayNro;
        }
        public object GetDynamicSortProperty(object item, string propName)
        {
            object objecto;


            objecto = item.GetType().GetProperty(propName).GetValue(item, null);

            if (objecto is Object)
            {
                return null;
            }
            else
            {
                return objecto;
            }
        }
        public string GetFormatNumberParam()
        {
            string format = GetCfgParameter(CfgParameterName.FormatDecimalNumByGridPage.ToString());
            return format;
        }
        /// <summary>
        /// Formatea el vlor numerico ingresado, de acuerdo al formato entregado por el parametro de Sistema
        /// </summary>
        /// <param name="value">Ingresar Valor Numerico</param>
        /// <returns></returns>
        public string GetFormatedNumber(object value)
        {
            //string format = context.CfgParameters[Convert.ToInt16(CfgParameterName.FormatDecimalNumByGridPage)].ParameterValue.Trim();
            string format = GetCfgParameter(CfgParameterName.FormatDecimalNumByGridPage.ToString()); ;
            string result = "";

            try
            {
                if (value != null)
                {
                    string typeName = value.GetType().Name.ToUpper();

                    if (!string.IsNullOrEmpty(format) && format != "1")
                    {
                        switch (typeName)
                        {
                            case "INT16":
                                result = ((Int16)value).ToString();
                                break;
                            case "INT32":
                                result = ((Int32)value).ToString();
                                break;
                            case "INT64":
                                result = ((Int64)value).ToString();
                                break;
                            case "DOUBLE":
                                result = ((Double)value).ToString(format);
                                break;
                            case "DECIMAL":
                                result = ((Decimal)value).ToString(format);
                                break;
                        }
                    }
                    else
                    {
                        switch (typeName)
                        {
                            case "INT16":
                                result = ((Int16)value).ToString();
                                break;
                            case "INT32":
                                result = ((Int32)value).ToString();
                                break;
                            case "INT64":
                                result = ((Int64)value).ToString();
                                break;
                            case "DOUBLE":
                                result = ((Double)value).ToString();
                                break;
                            case "DECIMAL":
                                result = ((Decimal)value).ToString();
                                break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public string EnumsDescription(TransactionTypeBilling value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        /// <summary>
        /// Carga datos de un archivo xml a un DataTable
        /// </summary>
        /// <param name="fileXML">ruta del archivo a cargar</param>
        /// <param name="nroWorkSheet">numero de hoja que se desea cargar</param>
        /// <returns></returns>
        public DataTable ConvertXlsToDataTable(string fileXML, int nroWorkSheet)
        {
            DataTable dt = new DataTable();

            using (XLWorkbook workBook = new XLWorkbook(fileXML))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(nroWorkSheet);

                //Loop through the Worksheet rows.
                bool firstRow = true;
                int columnMax = 0;

                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            if (cell.Value.ToString().Equals(String.Empty))
                            {
                                dt.Columns.Add("NULL");
                            }
                            else
                            {
                                dt.Columns.Add(cell.Value.ToString().Trim());
                            }
                            columnMax++;
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        int i = 0;
                        int count = 0;
                        // verifica si toda la fila esta vacia
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                count++;
                            }
                        }
                        //si toda la fila esta vacia salta la fila
                        if (count >= columnMax)
                            continue;

                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (i == 0) { dt.Rows.Add(); }

                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = "NULL";
                            }
                            else
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = row.Cell(j).Value.ToString();
                            }
                            i++;
                        }
                    }

                }
            }

            return dt;
        }

        public bool ValidateIsNotNull(object field)
        {
            bool result = true;

            if (field == null)
            {
                result = false;
            }
            else if (field.ToString() == "")
            {
                result = false;
            }
            else if (field.ToString() == "NULL")
            {
                result = false;
            }

            return result;
        }

        public void ShowAlertLocal(string title, string message)
        {
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        public int GetColumnIndexByName(GridViewRow row, string SearchColumnName)
        {
            int columnIndex = 0;
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.ContainingField.AccessibleHeaderText == SearchColumnName)
                {
                    break;
                }

                columnIndex++;
            }
            return columnIndex;
        }

        public int GetColumnIndexByName(GridView grid, string name)
        {
            foreach (DataControlField col in grid.Columns)
            {
                if (col.HeaderText.ToLower().Trim() == name.ToLower().Trim())
                {
                    return grid.Columns.IndexOf(col);
                }
            }

            return -1;
        }

        protected XElement readXml()
        {
            const string CACHE_KEY = "XML_PATH";

            if (Cache[CACHE_KEY] == null)
            {
                var xml = XElement.Load(baseControl.ConstantPath);
                Cache.Insert(CACHE_KEY, xml, new CacheDependency(baseControl.ConstantPath));
            }

            return (XElement)Cache[CACHE_KEY];
        }

        protected bool IsValidExcelRow(object myObject)
        {
            int cont = 0;

            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(object))
                {
                    var value = pi.GetValue(myObject, null);
                    if (value == null || value.ToString() == "NULL")
                    {
                        cont++;
                    }
                }
            }

            return cont != myObject.GetType().GetProperties().Length;
        }

        protected void ClearFilter(string nameFilter)
        {
            if (context.MainFilter == null)
                return;

            if (context.MainFilter.Exists(filter => filter.Name == nameFilter))
            {
                context.MainFilter.RemoveAll(filter => filter.Name == nameFilter);
            }
        }

        protected void CreateFilterByList<T>(string nameFilter, List<T> listItems)
        {
            ClearFilter(nameFilter);

            var listFilter = new List<FilterItem>();

            foreach (var item in listItems)
            {
                listFilter.Add(new FilterItem() { Name = nameFilter, Value = item.ToString() });
            }

            context.MainFilter.Add(new EntityFilter()
            {
                Name = nameFilter,
                FilterValues = listFilter
            });
        }

        protected void CreateFilterTypeLocationUsedForStockAvailable()
        {
            const string ID_TAG = "TypeLocations";

            var listLocations = GetConst("TypeLocationUsedForStockAvailable");

            ClearFilter(ID_TAG);
            CreateFilterByList(ID_TAG, listLocations);
        }

        public string ValidateCodeGLN(string gln)
        {
            try
            {
                Label lblIsNotNumeric = (Label)Master.FindControl("lblGlnIsNotNumeric");
                Label lblLengthInvalid = (Label)Master.FindControl("lblGlnLengthInvalid");
                Label lblCheckDigit = (Label)Master.FindControl("lblGlnCheckDigit");

                string result = string.Empty;
                int lengthGLN = gln.Trim().Length;
                Regex regIsNum = new Regex(@"^[0-9]+$");

                //Valida que el codigo de barra ingresado sea numerico
                if (!regIsNum.IsMatch(gln.Trim()))
                {
                    result = lblIsNotNumeric.Text;
                }
                else
                {
                    if (lengthGLN != 13)
                    {
                        result = lblLengthInvalid.Text;
                    }
                    else
                    {
                        //Rescata digito Verificador.
                        string digVerif = gln.Substring(lengthGLN - 1, 1);
                        gln = gln.Substring(0, lengthGLN - 1);

                        int factor = 3;
                        int sum = 0;

                        for (int index = gln.Length; index > 0; --index)
                        {
                            sum = sum + int.Parse(gln.Substring(index - 1, 1)) * factor;
                            factor = 4 - factor;
                        }

                        string resultDigit = ((1000 - sum) % 10).ToString();

                        if (resultDigit != digVerif)
                        {
                            result = lblCheckDigit.Text;
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string ShowMaxLenghtInString(string message, int maxCharacters)
        {
            if (message.Length > maxCharacters)
                return message.Substring(0, maxCharacters - 3) + "...";
            else
                return message;
        }
        public ContextViewDTO NewContext()
        {
            ContextViewDTO newContext = new ContextViewDTO();
            newContext.MainFilter = new List<EntityFilter>();

            foreach (var item in Enum.GetNames(typeof(EntityFilterName)))
            {
                EntityFilter entity = new EntityFilter(item.ToString(), new FilterItem());
                newContext.MainFilter.Add(entity);
            }
            foreach (EntityFilter entityFilter in newContext.MainFilter)
            {
                entityFilter.FilterValues.Clear();
            }

            newContext.SessionInfo = new SessionInfo();
            newContext.SessionInfo.IdModule = 0;
            newContext.SessionInfo.IdPage = "";
            newContext.SessionInfo.User = new User();

            return newContext;
        }
        public string GetDaysLeft(DateTime theDate)
        {
            if (theDate == DateTime.MinValue)
                return string.Empty;

            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - theDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "Hace un segundo" : "Hace " + ts.Seconds + " segundos";

            if (delta < 2 * MINUTE)
                return "Hace un minuto";

            if (delta < 45 * MINUTE)
                return "Hace " + ts.Minutes + " minutos";

            if (delta < 90 * MINUTE)
                return "Hace una hora";

            if (delta < 24 * HOUR)
                return "Hace " + ts.Hours + " horas";

            if (delta < 48 * HOUR)
                return "Ayer";

            if (delta < 30 * DAY)
                return "Hace " + (ts.Days + 1) + " días";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "Hace un mes" : "Hace " + months + " meses";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "Hace un año" : "Hace " + years + " years ago";
            }
        }
        public int GetLPNNumberSufix()
        {
            var listaSufix = GetConst("LPNNumberSufix");
            var countSufix = 0;

            if (listaSufix.Count > 0)
                countSufix = Convert.ToInt32(listaSufix[0]);

            return countSufix;
        }

        protected DataTable ConvertXlsToDataTableHeader(string fileXML)
        {
            DataTable dt = new DataTable();

            using (XLWorkbook workBook = new XLWorkbook(fileXML))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);

                //Loop through the Worksheet rows.
                int columnMax = 0;
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.

                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            if (cell.Value.ToString().Equals(String.Empty))
                            {
                                dt.Columns.Add("NULL");
                            }
                            else
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            columnMax++;
                        }
                        firstRow = false;
                    }
                    else
                    {
                        //Add rows to DataTable.
                        int i = 0;
                        int count = 0;
                        // verifica si toda la fila esta vacia
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                count++;
                            }
                        }
                        //si toda la fila esta vacia salta la fila
                        if (count >= columnMax)
                            continue;
                        for (int j = 1; j <= columnMax; j++)
                        {
                            if (i == 0) { dt.Rows.Add(); }

                            if (row.Cell(j).Value.ToString().Equals(String.Empty))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = "NULL";
                            }
                            else
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = row.Cell(j).Value.ToString();
                            }
                            i++;
                        }
                    }
                }
            }

            return dt;
        }
        private bool DownloadFileWillBeCsv()
        {
            bool willBeCsv = true;

            var value = context.CfgParameters.Where(param => param.ParameterCode == "DownloadFileWillBeCsv").FirstOrDefault();

            if (value != null)
            {
                if (value.ParameterValue == "0")
                    willBeCsv = false;
            }

            return willBeCsv;
        }

        protected bool isIntegratedToSimpliroute(int idOwn, int idWhs)
        {
            const string ID_SIMPLIROUTE = "IdEmpresaSimpliRoute";
            var newContext = NewContext();
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(idOwn.ToString()));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(idWhs.ToString()));
            newContext.MainFilter[Convert.ToInt16(EntityFilterName.Name)].FilterValues.Add(new FilterItem(ID_SIMPLIROUTE));

            var parameter = iConfigurationMGR.CfgOwnerParameterFindAll(newContext);

            if (parameter.Entities.Count == 0)
                return false;

            return parameter.Entities.First().ParameterValue == "0" ? false : true;
        }
        #endregion

        public void LoadLogicalWarehouses(DropDownList objControl, string emptyRowText, string emptyRowValue, bool isNew = false)
        {
            //Session[WMSTekSessions.OptionMenuSelected.SelectedIdWhs] = objControl.SelectedValue;

            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];

            var getLogicalWarehouses = iWarehousingMGR.GetAllLogicalWarehouses(context);

            if (isNew)
                getLogicalWarehouses.Entities = getLogicalWarehouses.Entities.Where(lw => lw.hasReasonsAssociated == true).ToList();

            objControl.Enabled = true;
            objControl.DataSource = getLogicalWarehouses.Entities;
            objControl.DataTextField = "WarehouseCode";
            objControl.DataValueField = "Id";
            objControl.DataBind();

            if (isNew)
            {
                objControl.Items.Insert(0, new ListItem(emptyRowText, "-1"));
                objControl.Items[0].Selected = true;
            }
            else
            {
                if (objControl.Items.Count > 0)
                    objControl.Items[0].Selected = true;
            }
        }
    }
}
