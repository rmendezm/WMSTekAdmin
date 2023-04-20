using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Profile;

namespace Binaria.WMSTek.WebClient.Pruebas
{
    public partial class XML : System.Web.UI.Page
    {
        public BaseControl baseControl;
        protected ContextViewDTO context;


        private GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        private GenericViewDTO<InboundDetail> inboundDetailViewDTO = new GenericViewDTO<InboundDetail>();
        private IReceptionMGR iReceptionMGR;
        private XmlDocument xmlDoc = new XmlDocument();
        private XmlDocument xlsDoc = new XmlDocument();
        private static string validation = "0K";
        private XmlSchemaSet schemas = new XmlSchemaSet();
        private InboundOrder inOrd;

        protected void Page_Init(object sender, EventArgs e)
        {
            var objectInstances = (InstanceFactory)Session[WMSTekSessions.Global.ObjectInstances];
            iReceptionMGR = (IReceptionMGR)objectInstances.getObject("receptionMGR");
            context = new ContextViewDTO();
            context.LanguageCode = "es";
            context.PathClassRemoting = MiscUtils.ReadSetting("remotingFile", string.Empty);
            context.SessionInfo = new SessionInfo();
            context.SessionInfo.User = new User();
            context.SessionInfo.User.UserName = "webService";
        }

        protected void btnSendInbound_Click(object sender, EventArgs e)
        {
            String sourceUserName = "WebServ";
            String sourceAppName = "PruebaIfz";
            String sourceHostName = "HostPrueba";
            String idTicketTransfer = null;
            Char typeMovto = 'I';
            int? recordRequestQty = null;
            int? recordProcessQty = null;
            String rangeRequest = null;
            XmlDocument clientXML = new XmlDocument();
            String wsMessage = string.Empty;

            clientXML.Load(Request.PhysicalApplicationPath + "Pruebas\\XML\\InboundOrder.xml");
            //clientXML.Load(Request.PhysicalApplicationPath + "Pruebas\\XML\\Inbound.xml");

            wsMessage = InsertInbound
                        (
                            sourceUserName,
                            sourceAppName,
                            sourceHostName,
                            idTicketTransfer,
                            typeMovto,
                            recordRequestQty,
                            recordProcessQty,
                            rangeRequest,
                            clientXML.InnerXml.ToString()
                       );

            lblWsMessage.Text = wsMessage;

        }

        public String InsertInbound(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            int lineNumber = 1;

            try
            {
                // Carga el documento en un objeto XML
                validation = LoadXML(document);

                if (validation == "OK")
                {
                    // Valida estructura del XML
                    validation = ValidateXML("inbound");

                    if (validation == "OK")
                    {
                        // Crea la orden desde el XML
                        validation = CreateInboundOrder();

                        if (validation == "OK")
                        {
                            // Completa las propiedades de la orden
                            inOrd.LatestInboundTrack = new InboundTrack();
                            inOrd.LatestInboundTrack.Type = new TrackInboundType();
                            inOrd.LatestInboundTrack.Type.Id = (int)TrackInboundTypeName.Anunciado;
                            inOrd.LatestInboundTrack.InboundOrder = new InboundOrder();
                            inOrd.LatestInboundTrack.DateTrack = DateTime.Now;

                            foreach (InboundDetail detail in inOrd.InboundDetails)
                            {
                                detail.InboundOrder = new InboundOrder();
                                detail.LineNumber = lineNumber;
                                lineNumber++;
                            }

                            // Inserta la orden y sus detalles
                            inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrder(CRUD.Create, inOrd, context);

                            if (inboundOrderViewDTO.hasError())
                            {
                                validation = inboundOrderViewDTO.Errors.Message + " - " + inboundOrderViewDTO.Errors.OriginalMessage;
                            }
                            else
                            {
                                validation = "OK";
                            }
                        }
                    }
                }

                return validation;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return ex.Message + " - " + ex.InnerException.Message;
                else
                    return ex.Message;
            }
        }

        private string LoadXML(string document)
        {
            try
            {
                xmlDoc.LoadXml(document);

                return "OK";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return ex.Message + " - " + ex.InnerException.Message;
                else
                    return ex.Message;
            }  
        }

        /// <summary>
        /// Valida la estructura del XML enviado por el cliente. 
        /// </summary>
        private string ValidateXML(string objectType)
        {
            switch (objectType)
            {
                case "inbound":
                    xlsDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "InboundOrder.xsd");
                    //xlsDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "Inbound.xsd");
                    return MiscUtils.ValidateXml(xmlDoc.InnerXml, xlsDoc.InnerXml);
                default:
                    return "Invalid objectType";
            }
        }

        private string CreateInboundOrder()
        {
            try
            {
                inOrd = DeserializeObject<InboundOrder>(xmlDoc.InnerXml);

                return "OK";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return ex.Message + " - " + ex.InnerException.Message;
                else
                    return ex.Message;
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
    }
}
