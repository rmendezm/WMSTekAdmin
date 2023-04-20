using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;

namespace Binaria.WMSTek.WebClient.Base
{
    public class BaseUserControl : System.Web.UI.UserControl
    {
        protected ILanguageMGR iLanguageMGR;
        protected IProfileMGR iProfileMGR;
        public ILayoutMGR iLayoutMGR;
        public IDispatchingMGR iDispatchingMGR;
        public IReceptionMGR iReceptionMGR;
        public IWarehousingMGR iWarehousingMGR;
        public IConfigurationMGR iConfigurationMGR;
        public IRulesMGR iRulesMGR;

        public BaseControl baseControl;

        protected User objUser;
        protected ContextViewDTO context;
        protected GenericViewDTO<Translate> translateViewDTO;
        GenericViewDTO<Auxiliary> userControlViewDTO = new GenericViewDTO<Auxiliary>();
        protected WebMode webMode;

        protected virtual void Page_Init(object sender, EventArgs e)
        {
            context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
            if(context != null)
            { 
                baseControl = BaseControl.getInstance(Request.PhysicalApplicationPath, context);
                var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];

                iLanguageMGR = (ILanguageMGR)objectInstances.getObject("languageMGR");
                iProfileMGR = (IProfileMGR)objectInstances.getObject("profileMGR");
                iLayoutMGR = (ILayoutMGR)objectInstances.getObject("layoutMGR");
                iDispatchingMGR = (IDispatchingMGR)objectInstances.getObject("dispatchingMGR");
                iReceptionMGR = (IReceptionMGR)objectInstances.getObject("receptionMGR");
                iWarehousingMGR = (IWarehousingMGR)objectInstances.getObject("warehousingMGR");
                iConfigurationMGR = (IConfigurationMGR)objectInstances.getObject("configurationMGR");

                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.LoggedInUser))
                    objUser = (User)Session[WMSTekSessions.Global.LoggedInUser];

                if (((BasePage)Page).ValidateSession(WMSTekSessions.Global.Context))
                    context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];

                ConfigurePage();

                if (webMode == WebMode.Normal && objUser != null)
                {
                    TranslatePage(this);
                }
            }
        }

        /// <summary>
        /// Configura pagina segun el rol de usuario
        /// </summary>
        protected void ConfigurePage()
        {
            // Permissions
            // TODO:
            // - Cargar lista de objetos de la pagina, con propiedades de acces segun rol (view, edit, delete)
            // - Recorrer objetos de la pagina, y setear propiedades Enabled / Visible segun rol

            // Dictionary
            if (((BasePage)Page).ValidateSession("UpdateDictionary"))
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
        /// Entra aqui cuando quiere configurar la pagina dependiendo del rol del usuario
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
                // lblErrorMessage.Text = dictionaryViewDTO.Errors.Message;
                // divErrorMessage.Visible = true;
            }
        }
        //cambiar este por el antiguo
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
                // - Carga una lista de [Object | Translation] para el idioma del usuario
                translateViewDTO = iLanguageMGR.GetTranslationByContent(content.ToString(), objUser.Language.Id, context);

                // - Recorrer objetos de la pagina, y setear propiedades Text = Translation para los Objetos que se encuentren en la lista
                if (translateViewDTO.Entities != null && translateViewDTO.Entities.Count > 0)
                {
                    LoopTranslate(this, translateViewDTO);
                }
            }
        }
        /// <summary>
        /// entra cuando me logueo
        /// </summary>
        /// <param name="control"></param>
        /// <param name="translateViewDTO"></param>
        static public void LoopTranslate(Control control, GenericViewDTO<Translate> translateViewDTO)
        {
            //    TODO: modificar para soportar cualquier control con texto 
            //    TODO:idModule deberia estar en una variable de sesion de contexto.
            //    TODO: manejo de excepciones

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
    }
}