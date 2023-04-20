using System;
using System.IO;
using System.Text;
using System.Web.Services;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.AdminApp.Integration;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Integration;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using System.Collections.Generic;
using Binaria.WMSTek.DataAccess.Integration;
using Binaria.WMSTek.DataAccess.Manager;
using Binaria.WMSTek.IntegrationClient.Integration;
using System.Linq;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Summary description for wsBase
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsBase : WebService
    {
        protected BaseControl baseControl;

        protected ContextViewDTO context;
        protected IReceptionMGR iReceptionMGR;
        protected IDispatchingMGR iDispatchingMGR;
        protected IWarehousingMGR iWarehousingMGR;
        protected IIntegrationMGR iIntegrationMGR;
        protected ILayoutMGR iLayoutMGR;
        protected ILabelMGR iLabelMGR;
        protected IProfileMGR iProfileMGR;

        protected LogManager theLog;

        protected XmlDocument xmlInput = new XmlDocument();
        protected XmlDocument xsdDoc = new XmlDocument();

        protected static string initMsg = "OK";
        protected static string statusProcess = "OK";
        protected static string statusMessage = string.Empty;
        protected static string clientResponse = string.Empty;
        protected string method = string.Empty;
        protected bool genericError = false;
        private XmlSchemaSet schemas = new XmlSchemaSet();

        //protected GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        //protected GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        protected GenericViewDTO<MovementWeb> movementWebViewDTO = new GenericViewDTO<MovementWeb>();
        protected GenericViewDTO<MovementIfz> movementIfzViewDTO;
        protected GenericViewDTO<WServiceMessageIfz> wServiceMessageIfzViewDTO;
        protected MovementIfz movementIfz;

        protected BranchIfzDAO theBranchIfzDAO;
        protected WServiceMessageIfzDAO theWServiceMessageIfzDAO;
        protected ReceiptIfzDAO theReceiptIfzDAO;
        protected PutawayIfzDAO thePutawayIfzDAO;
        protected ReceiptDetailIfzDAO theReceiptDetailIfzDAO;
        protected DispatchIfzDAO theDispatchIfzDAO;
        protected DispatchDetailIfzDAO theDispatchDetailIfzDAO;
        protected MovementAdjustIfzDAO theMovementAdjustIfzDAO;
        protected BillingLogIfzDAO theBillingLogIfzDAO;
        protected SerialTrackIfzDAO theSerialTrackIfzDAO;

        protected string ERR_MSG_AUTH = "Error en autenticacion";
        protected string ERR_MSG_VAL_AUTH = "Error al ejecutar validacion de autenticacion";
        protected string ERR_INVALID_OWNER = "Error, owner no válido según usuario";
        protected string ERR_INVALID_OWNER_LIST = "Error, al menos un owner no válido según usuario";
        protected string ERR_USER_HAS_NOT_OWNERS = "Error, usuario no tiene ningun owner configurado";

        private static IntegrationConfigManager integrationConfigManager = null;
        public wsBase()
        {
            InitializeLog();
            InitializeBaseControl();

            if (integrationConfigManager == null)
            {
                integrationConfigManager = IntegrationConfigManager.getInstance();
                integrationConfigManager.Execute();
            }

            theBranchIfzDAO = new BranchIfzDAO();
            theWServiceMessageIfzDAO = new WServiceMessageIfzDAO();
            theReceiptIfzDAO = new ReceiptIfzDAO();
            thePutawayIfzDAO = new PutawayIfzDAO();
            theReceiptDetailIfzDAO = new ReceiptDetailIfzDAO();
            theDispatchIfzDAO = new DispatchIfzDAO();
            theDispatchDetailIfzDAO = new DispatchDetailIfzDAO();
            theMovementAdjustIfzDAO = new MovementAdjustIfzDAO();
            theBillingLogIfzDAO = new BillingLogIfzDAO();
            theSerialTrackIfzDAO = new SerialTrackIfzDAO();
        }

        protected void Initialize()
        {
            initMsg = "OK";
            InitializeLog();
            statusMessage = string.Empty;

            if (initMsg == "OK")
            {
                InitializeBaseControl();

                if (initMsg == "OK")
                {
                    InitializeInstances();

                    if (initMsg == "OK") InitializeContext();
                }
            }
        }

        /// <summary>
        /// Inicializa Logmanager
        /// </summary>
        private void InitializeLog()
        {
            int logLevel, logType;
            string logPath;

            logLevel = int.Parse(MiscUtils.ReadSetting("logLevel", "20"));
            logType = int.Parse(MiscUtils.ReadSetting("logType", "1"));
            logPath = Context.Request.PhysicalApplicationPath + MiscUtils.ReadSetting("logPath", "");

            try
            {
                theLog = LogManager.getInstance();

                if (!theLog.Initialized) theLog.initialize(logLevel, logType, logPath);

                initMsg = "OK";
            }
            catch (Exception ex)
            {
                genericError = true;
                if (ex.InnerException != null)
                    initMsg = ex.Message + " - " + ex.InnerException.Message;
                else
                    initMsg = ex.Message;
            }
        }

        /// <summary>
        /// Inicializa BaseControl
        /// </summary>
        protected void InitializeBaseControl()
        {
            try
            {
                context = new ContextViewDTO();
                //baseControl = BaseControl.getInstance(MiscUtils.ReadSetting("webClientPath", Context.Request.PhysicalApplicationPath), context);
                theLog.warningMessage("Context.Request.PhysicalApplicationPath ", Context.Request.PhysicalApplicationPath);

                baseControl = BaseControl.getInstance(Context.Request.PhysicalApplicationPath);
                
                initMsg = "OK";
            }
            catch (Exception ex)
            {
                genericError = true;
                initMsg = LogException(ex, " InitializeBaseControl");
            }
        }

        /// <summary>
        /// Inicializa Instancias
        /// </summary>
        protected void InitializeInstances()
        {
            try
            {
                var objectInstances = InstanceFactory.getInstance(MiscUtils.ReadSetting("webClientPath", Context.Request.PhysicalApplicationPath));
                iProfileMGR = (IProfileMGR)objectInstances.getObject("profileMGR");

                iReceptionMGR = (IReceptionMGR)objectInstances.getObject("receptionMGR");
                iDispatchingMGR = (IDispatchingMGR)objectInstances.getObject("dispatchingMGR");
                iWarehousingMGR = (IWarehousingMGR)objectInstances.getObject("warehousingMGR");
                iIntegrationMGR = (IntegrationMGR)objectInstances.getObject("integrationMGR");
                iLayoutMGR = (LayoutMGR)objectInstances.getObject("layoutMGR");
                iLabelMGR = (ILabelMGR)objectInstances.getObject("labelMGR");

                if (iReceptionMGR == null || iIntegrationMGR == null)
                    initMsg = "InitializeInstances() error.";
                else
                    initMsg = "OK";

            }
            catch (Exception ex)
            {
                genericError = true;
                initMsg = LogException(ex, " InitializeInstances");
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

                context.SessionInfo = new SessionInfo();
                context.SessionInfo.User = new User();
                context.SessionInfo.User.UserName = "webService";

                context.MainFilter = new List<EntityFilter>();

                foreach (var item in Enum.GetNames(typeof(EntityFilterName)))
                {
                    EntityFilter entity = new EntityFilter(item.ToString(), new FilterItem());
                    context.MainFilter.Add(entity);
                }
                foreach (EntityFilter entityFilter in context.MainFilter)
                {
                    entityFilter.FilterValues.Clear();
                }

                initMsg = "OK";
            }
            catch (Exception ex)
            {
                genericError = true;
                initMsg = LogException(ex, " InitializeBaseControl");
            }
        }

        protected void LoadXML(string document)
        {
            try
            {
                xmlInput.LoadXml(document);

                statusProcess = "OK";
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException) genericError = true;
                statusProcess = "ER";
                statusMessage = LogException(ex, " LoadXML");
            }
        }

        protected string LogException(Exception ex, string method)
        {
            if (ex.InnerException != null)
            {
                theLog.exceptionMessage(this.GetType().FullName + method, "Exception: " + ex.Message + " Inner Exception: " + ex.InnerException.Message + " StackTrace: " + ex.StackTrace);
                return this.GetType().FullName + method + ": " + ex.Message + " - " + ex.InnerException.Message;
            }
            else
            {
                theLog.exceptionMessage(this.GetType().FullName + method, "Exception: " + ex.Message + " StackTrace: " + ex.StackTrace);
                return this.GetType().FullName + method + ": " + ex.Message;
            }
        }

        /// <summary>
        /// Valida la estructura del XML enviado por el cliente. 
        /// </summary>
        protected void ValidateXML()
        {
            xsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + method + ".xsd");

            statusMessage = MiscUtils.ValidateXml(xmlInput.InnerXml, xsdDoc.InnerXml);

            if (statusMessage != "OK")
            {
                statusProcess = "ER";
                clientResponse = statusMessage;
            }
            else
            {
                statusProcess = "OK";
                statusMessage = string.Empty;
            }
        }


        /// <summary>
        /// Registra el movimiento realizado en la tabla MovementIfz
        /// </summary>
        protected void RegisterMovement(string sourceUserName, string sourceAppName, string sourceHostName, char typeMovto, string webServiceName)
        {
            movementIfz = new MovementIfz();
            movementIfz.SourceUserName = sourceUserName;
            movementIfz.SourceAppName = sourceAppName;
            movementIfz.SourceHostName = sourceHostName;
            movementIfz.IdTicketTransfer = null;
            movementIfz.Dateprocess = DateTime.Now;
            movementIfz.WebServiceName = webServiceName;
            movementIfz.TypeMovto = typeMovto.ToString();
            movementIfz.RecordRequestQty = -1;
            movementIfz.RecordProcessQty = -1;
            movementIfz.RangeRequest = null;
            movementIfz.StatusProcess = statusProcess;
            movementIfz.StatusMessage = statusMessage;

            if (iIntegrationMGR != null)
            {
                movementIfzViewDTO = iIntegrationMGR.RegisterMovement(CRUD.Create, movementIfz, context);

                if (movementIfzViewDTO.hasError())
                {
                    genericError = true;
                    theLog.errorMessage(DateTime.Now.ToString(), this.GetType().ToString(), movementIfzViewDTO.Errors.OriginalMessage);
                    SetClientMessage(" FECHA: " + DateTime.Now.ToString(), webServiceName);
                }
                else
                    SetClientMessage(" ID: " + movementIfzViewDTO.Entities[0].Id.ToString(), webServiceName);
            }
            else
            {
                genericError = true;
                SetClientMessage(" ID: NO REGISTRADO", webServiceName);
            }
        }

        /// <summary>
        /// Setea el mensaje a devolver al cliente
        /// </summary>
        /// <param name="id"></param>
        protected void SetClientMessage(String id, String webServiceName)
        {
            if (statusProcess == "OK")
            {
                // Según el tipo de webService, se devuelve un mensaje de status o un XML al cliente
                //   wsImport --> "OK" / "ER"
                //   wsExport --> xmlOut / "ER"
                if (webServiceName == "wsImport")
                    clientResponse = "OK;" + clientResponse;
                else
                 clientResponse = statusMessage;
            }
            else
            {
                if (genericError)
                {
                    if (baseControl != null)
                        clientResponse = "ERROR " + id + " [" + method + "] " + baseControl.handleError(new ErrorDTO(WMSTekError.Generic.GenericError, context.LanguageCode)).Title;
                    else
                        clientResponse = "ERROR " + id + " [" + method + "] ";
                }
                else
                    clientResponse = "ERROR " + id + " [" + method + "] " + clientResponse;
            }
        }

        /// <summary>
        /// Crea una instancia de tipo T a partir de un string XML
        /// </summary>
        public static T DeserializeObject<T>(string xml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }

        /// <summary>
        /// Crea un string XML a partir de una instancia de tipo T
        /// </summary>
        public static string SerializeObject<T>(T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));

            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            xs.Serialize(xmlTextWriter, obj);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            return UTF8ByteArrayToString(memoryStream.ToArray());
        }

        public static string SerializeObject<T>(T obj, bool xmlDeclaration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter str = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(str, new XmlWriterSettings { OmitXmlDeclaration = !xmlDeclaration }))
            {
                serializer.Serialize(writer, obj);
            }
            string messageToLog = str.ToString();
            return messageToLog;
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public int GetNroTicket()
        {
            int NroTicket = -1;

            try
            {

                Initialize();

                if (initMsg == "OK")
                {
                    NroTicket = DBIntegrationInstance.getInstance().GetIdEntityByClassName("Binaria.WMSTek.Framework.Entities.Integration.WServiceMessageIfz");
                    statusProcess = "OK";

                }
                else
                {
                    genericError = true;
                    statusProcess = "ER";
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }

            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ER";
                statusMessage = LogException(ex, " Import");
                throw new Exception("Error", ex);
            }

            return NroTicket;
        }
        protected bool ValidateOwnerByUser(List<string> ownCodes, UserCredentials userCredentials)
        {
            bool isValid = false;

            if (ownCodes != null && ownCodes.Count > 0)
            {
                ownCodes = ownCodes.Distinct().ToList();

                var ownersByUser = iProfileMGR.OwnersByUser(userCredentials.userName, userCredentials.password, context);

                if (!ownersByUser.hasError() && ownersByUser.Entities.Count > 0)
                {
                    var configuresOwnersByUser = ownersByUser.Entities.Select(o => o.Code).Distinct().ToList();

                    bool contained = !ownCodes.Except(configuresOwnersByUser).Any();

                    if (contained)
                        isValid = true;
                }
            }

            return isValid;
        }
        protected bool HasToValidateAuth()
        {
            bool hasToValidate = true;

            var isCompany3PL = iProfileMGR.IsCompany3PL();

            if (!isCompany3PL)
            {
                var hasToAuthWS = iProfileMGR.HasToAuthWS();

                if (!hasToAuthWS)
                    hasToValidate = false;
            }

            return hasToValidate;
        }
        public class UserCredentials
        {
            public string userName { get; set; }
            public string password { get; set; }
        }
    }
}
