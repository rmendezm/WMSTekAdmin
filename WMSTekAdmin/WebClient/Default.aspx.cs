using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient
{
    public partial class Default : System.Web.UI.Page
    {
        #region "Declaración de Variables"

        private ContextViewDTO context;
        private GenericViewDTO<EntityFilter> mainFilterViewDTO;
        GenericViewDTO<Auxiliary> auxViewDTO;
        private bool preLoginOk = true;
        private bool loginOk = true;
        private BaseControl baseControl;
        private LogManager theLog;
        private IProfileMGR iProfileMGR;
        private ILayoutMGR iLayoutMGR;

        #endregion

        #region "Eventos"

        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Remove("UpdateDictionary");

            // Detecta resolución de pantalla del cliente
            if (Session["screenX"] == null)
            {
                Response.Redirect("DetectScreen.aspx");
            }

            // SECUENCIA DE INICIO
            // -------------------

            // Antes de Login:
            //  - Log
            //  - Base Control
            //     - Error Messages (errorAdmin.config)
            //     - Status Bar Messages (messageStatusAdmin.config)
            //  - Instances (instanceAdmin.config)
            //  - Inicializar Context
            //  - Queries Pool (databaseAdmin.config)
            //  - Operation Service (...)

            // Después de Login:
            //  - Configurar Context
            //  - Main Filter

            try
            {
                if (!Page.IsPostBack)
                {
                    Initialize();
                }
                else
                {
                    context = (ContextViewDTO)Session[WMSTekSessions.Global.Context];
                    baseControl = BaseControl.getInstance(Request.PhysicalApplicationPath, context);
                    var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];
                    iProfileMGR = (IProfileMGR)objectInstances.getObject("profileMGR");
                    iLayoutMGR = (ILayoutMGR)objectInstances.getObject("layoutMGR");

                    InitializeContext();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Login Exception", ex);
            }
        }

        protected void btnAuthenticateUser_Click(object sender, EventArgs e)
        {
            try
            {
                string login = this.inputUser.Value.Trim();
                string password = MiscUtils.Encrypt(this.inputPassword.Value.Trim());

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    loginOk = false;
                    divMessageError.Visible = true;
                    this.lblErroLogin.Text = "Ingrese Usuario y/o Contraseña";
                }
                else
                {
                    GenericViewDTO<User> userViewDTO = iProfileMGR.GetUser(login, password, context);

                    if (!userViewDTO.hasError())
                    {
                        if (userViewDTO.Entities != null && userViewDTO.Entities.Count > 0)
                        {
                            if (userViewDTO.Entities[0].Warehouses == null || userViewDTO.Entities[0].Warehouses.Count < 1)
                            {
                                if (login.ToUpper() != "BASE")
                                {
                                    loginOk = false;
                                    this.lblErroLogin.Text = "Usuario Sin Centros de Distribución Asociadas";
                                    divMessageError.Visible = true;
                                }
                                else
                                {
                                    Session[WMSTekSessions.Global.LoggedInUser] = (object)userViewDTO.Entities[0];

                                    this.ConfigureContext(userViewDTO.Entities[0]);
                                    this.ConfigureMainFilter();

                                    if (loginOk)
                                    {
                                        SetCookiesForSession(login);
                                        Response.Redirect("~/Account/Desktop.aspx", false);
                                    }
                                }
                            }
                            else
                            {
                                Session[WMSTekSessions.Global.LoggedInUser] = (object)userViewDTO.Entities[0];

                                this.ConfigureContext(userViewDTO.Entities[0]);
                                this.ConfigureMainFilter();

                                if (loginOk)
                                {
                                    SetCookiesForSession(login);
                                    Response.Redirect("~/Account/Desktop.aspx", false);
                                }
                            }
                        }
                        else
                        {
                            this.inputUser.Focus();
                            loginOk = false;
                            this.lblErroLogin.Text = "Usuario y/o Contraseña Incorrecto";
                            divMessageError.Visible = true;
                        }
                    }
                    else
                    {
                        loginOk = false;
                        divMessageError.Visible = true;
                        this.lblErroLogin.Text = userViewDTO.Errors.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("LoginPrincipal_Authenticate Exception", ex);
            }
        }
        private void SetCookiesForSession(string userName)
        {
            Session[WMSTekSessions.Global.LoggedIn] = userName;
            string guid = Guid.NewGuid().ToString();
            Session[WMSTekSessions.Global.AuthToken] = guid;
            Response.Cookies.Add(new HttpCookie(WMSTekSessions.Global.AuthToken, guid));
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        #endregion

        #region "Métodos"
        protected void Initialize()
        {
            try
            {
                InitializeLog();
                if (preLoginOk) InitializeBaseControl();
                if (preLoginOk) InitializeInstances();
                if (preLoginOk) InitializeContext();
                if (preLoginOk) InitializeQueriesPool();

                theLog.debugMessage("WebClient.Initialize", "Inicializacion Config : " + preLoginOk);
                theLog.debugMessage("WebClient.Initialize", "Inicializacion Instancias : " + preLoginOk);
                theLog.debugMessage("WebClient.Initialize", "Inicializacion Conexion a BD : " + preLoginOk);

                if (preLoginOk)
                {
                    btnReload.Visible = false;
                    LoginPrincipal.Visible = true;
                    divMessageError.Visible = false;
                }
                else
                {
                    LoginPrincipal.Visible = false;
                    divMessageError.Visible = true;
                    btnReload.Visible = true;
                }
            }
            catch (Exception ex)
            {
                preLoginOk = false;
                throw new Exception("Initialize Exception", ex);
            }
        }


        /// <summary>
        /// Inicializa Logmanager
        /// </summary>
        protected void InitializeLog()
        {
            int logLevel, logType;
            string logPath;

            logLevel = int.Parse(MiscUtils.ReadSetting("logLevel", "20"));
            logType = int.Parse(MiscUtils.ReadSetting("logType", "1"));
            logPath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("logPath", "");

            try
            {
                theLog = LogManager.getInstance();
                theLog.initialize(logLevel, logType, logPath);
            }
            catch (Exception ex)
            {
                preLoginOk = false;
                throw new Exception("Initialize Log Exception", ex);
            }
        }

        /// <summary>
        /// Inicializa BaseControl
        /// </summary>
        protected void InitializeBaseControl()
        {
            try
            {
                theLog.debugMessage("Binaria.WMSTek.WebClient.Default", " Request.PhysicalApplicationPath" + Request.PhysicalApplicationPath);
                baseControl = BaseControl.getInstance(Request.PhysicalApplicationPath, context);
            }
            catch (Exception ex)
            {
                preLoginOk = false;
                throw new Exception("Initialize BaseControl Exception", ex);
            }
        }

        /// <summary>
        /// Inicializa Instancias
        /// </summary>
        protected void InitializeInstances()
        {
            ErrorDTO error;
            try
            {
                var objectInstances = InstanceFactory.getInstance(Request.PhysicalApplicationPath);
                Session[WMSTekSessions.Global.ObjectInstances] = objectInstances;

                iProfileMGR = (IProfileMGR)objectInstances.getObject("profileMGR");

                if (iProfileMGR == null)
                {
                    preLoginOk = false;
                    error = baseControl.handleError(new ErrorDTO(WMSTekError.Web.Init.InstanceError, MiscUtils.ReadSetting("languageCode", "es")));

                    this.lblErroLogin.Text = error.Message;
                }
            }
            catch (Exception ex)
            {
                preLoginOk = false;
                throw new Exception("Initialize Instances Exception", ex);
            }
        }

        private void InitializeContext()
        {
            context = new ContextViewDTO();

            try
            {
                // Lenguage base de la implementacion. Puede ser distinto al lenguage base, que es siempre español ("es")
                context.LanguageCode = MiscUtils.ReadSetting("languageCode", "es");

                //Busca la ruta de la clase remota en el web.config
                context.PathClassRemoting = MiscUtils.ReadSetting("remotingFile", string.Empty);
            }
            catch (Exception ex)
            {
                preLoginOk = false;
                throw new Exception("Initialize Context Exception", ex);
            }
        }

        /// <summary>
        /// Inicializa set de queries
        /// </summary>
        private void InitializeQueriesPool()
        {
            try
            {
                auxViewDTO = iProfileMGR.TestConnection(context);

                String nameAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                String beta = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion;
                lblVersion.Text += nameAssembly;
                lblVersion.Text += beta;

                try
                {
                    lblVersionRF.Text += baseControl.GetVersionRF(context.PathClassRemoting);
                }
                catch (Exception ex)
                {
                    lblVersionRF.Text += "Sin Conexión";
                }

                if (auxViewDTO.hasError())
                {

                    if (auxViewDTO.Errors.Code == "DataBase.SqlError")
                    {
                        preLoginOk = false;
                        this.lblErroLogin.Text = auxViewDTO.Errors.Message;
                    }
                    else
                    {
                        HttpRuntime.UnloadAppDomain();
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Default", " HttpRuntime.UnloadAppDomain() True ");
                    }
                }
            }
            catch (Exception ex)
            {
                preLoginOk = false;
                theLog.exceptionMessage("Binaria.WMSTek.WebClient.Default", "  Error APP POOL ");
                HttpRuntime.UnloadAppDomain();
                
                throw new Exception("Initialize Queries Pool Exception", ex);
            }
        }

        private bool ValidateSession(string sessionName)
        {
            if (Session[sessionName] != null && Session[sessionName].ToString() != string.Empty)
                return true;
            else
                return false;
        }



        /// <summary>
        /// Carga los detalles del Context para el usuario logueado
        /// </summary>
        /// <param name="loggedInUser"></param>
        private void ConfigureContext(User loggedInUser)
        {
            try
            {
                // TODO: parametrizar y arreglar roles
                // TODO: avisar si el usuario no tiene asignada Warehouses
                context.SessionInfo = new SessionInfo();

                context.SessionInfo.IdModule = Convert.ToInt16(Framework.Utils.Enums.EnumModule.Web);
                context.SessionInfo.IdRole = loggedInUser.Roles[0].Id;
                context.SessionInfo.User = loggedInUser;

                // Rescata la bodega por defecto del Usuario loggeado
                if (loggedInUser.Warehouses != null)
                {
                    foreach (Warehouse warehouse in loggedInUser.Warehouses)
                    {
                        if (warehouse.IsDefault)
                        {
                            context.SessionInfo.Warehouse = warehouse;
                            break;
                        }
                    }
                }

                // Rescata el dueño por defecto del Usuario loggeado
                if (loggedInUser.Owners != null)
                {
                    foreach (Owner owner in loggedInUser.Owners)
                    {
                        if (owner.IsDefault)
                        {
                            context.SessionInfo.Owner = owner;
                            break;
                        }
                    }
                }

                // Rescata la impresora por defecto del Usuario loggeado
                if (loggedInUser.Printers != null)
                {
                    foreach (Printer printer in loggedInUser.Printers)
                    {
                        if (printer.IsDefault)
                        {
                            context.SessionInfo.Printer = printer;
                            break;
                        }
                    }
                }

                // Carga datos de la Compañia
                context.SessionInfo.Company = iLayoutMGR.FindAllCompany(context).Entities[0];

                // Carga Parametros de Configuracion (a nivel de User -> Warehouse -> Company)
                context = iProfileMGR.LoadContextDetails(context);

                if (!context.hasError())
                {
                    Session[WMSTekSessions.Global.Context] = (object)context;
                }
                else
                {
                    loginOk = false;
                    throw new Exception(context.Errors.OriginalMessage);
                }
            }
            catch (Exception ex)
            {
                loginOk = false;
                throw new Exception("Configure Context Exception", ex);
            }
        }

        /// <summary>
        /// Carga el estado inicial del Filtro Principal 
        /// </summary>
        public void ConfigureMainFilter()
        {
            try
            {
                mainFilterViewDTO = new GenericViewDTO<EntityFilter>();

                // TODO: crear estructura para que se pueda cargar (y salvar) el estado inicial del filtro (por Usuario?)
                mainFilterViewDTO = iProfileMGR.LoadMainFilter(context);

                if (!mainFilterViewDTO.hasError())
                {
                    Session[WMSTekSessions.Global.MainFilter] = (object)mainFilterViewDTO.Entities;
                }
                else
                {
                    loginOk = false;
                    throw new Exception(mainFilterViewDTO.Errors.OriginalMessage);
                }
            }
            catch (Exception ex)
            {
                loginOk = false;
                throw new Exception("Configure MainFilter Exception", ex);
            }
        }

        #endregion


        #region Page Pre-Init: force uplevel browser setting
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (BrowserCompatibility.IsUplevel)
            {
                Page.ClientTarget = "uplevel";
            }
        }
        #endregion

    }


    public static class BrowserCompatibility
    {
        #region IsUplevel Browser property
        private enum UpLevel { chrome, firefox, safari }

        public static bool IsUplevel
        {
            get
            {
                bool ret = false;
                string _browser;

                try
                {

                    if (HttpContext.Current == null) return ret;
                    _browser = HttpContext.Current.Request.UserAgent.ToLower();

                    foreach (UpLevel br in Enum.GetValues(typeof(UpLevel)))
                    { if (_browser.Contains(br.ToString())) { ret = true; break; } }

                    return ret;
                }
                catch { return ret; }
            }
        }
        #endregion
    }
}
