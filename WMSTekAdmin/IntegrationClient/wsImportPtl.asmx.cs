using Binaria.WMSTek.DataAccess.Base;
using Binaria.WMSTek.DataAccess.Integration;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Funtional.Integration;
using Binaria.WMSTek.Framework.Entities.Integration;
using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Descripción breve de wsImportPtl
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class wsImportPtl : wsBase
    {
        private TransactionManager dBTransaction = TransactionManager.getInstance();

        /// <summary>
        /// ImportTaskDetailPtlResult
        /// </summary>
        /// <param name="taskDetPtlResIfzFunc"></param>
        /// <returns></returns>
        [WebMethod]
        public String ImportTaskDetailPtlResult(TaskDetailPtlResultIfzFunctional taskDetPtlResIfzFunc)
        {
            try
            {
                Initialize();
                method = "ImportTaskDetailPtlResult";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    if (nroTicket > 0)
                    {
                        if (taskDetPtlResIfzFunc.ListTaskDetailPtlResultIfz != null && taskDetPtlResIfzFunc.ListTaskDetailPtlResultIfz.Count > 0)
                        {
                            var transaction = dBTransaction.Open("WMSTek_INTERFAZ");
                            context.CurrentTransaction = transaction;

                            var wServiceMessageIfzDAO = new WServiceMessageIfzDAO(context.CurrentTransaction);
     
                            foreach (var td in taskDetPtlResIfzFunc.ListTaskDetailPtlResultIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(TaskDetailPtlResultIfz).Name.Trim();
                                ws.ListXml = SerializeObject(td, false);
                                ws.Transferido = false;
                                ws.DateCreated = DateTime.Now;

                                wServiceMessageIfzViewDTO = wServiceMessageIfzDAO.Insert(ws);

                                if (wServiceMessageIfzViewDTO.hasError())
                                {
                                    if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                    statusProcess = "ERROR";
                                    clientResponse = "Error al insertar en WServiceMessage " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                    statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    break;
                                }
                                else
                                {
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }

                            if (statusProcess == "OK")
                                context.CompleteTransaction();
                            else
                                context.AbortTransaction();
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [TaskDetailPtlResultIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " ImportTaskDetailPtlResult");
            }

            return clientResponse;
        }


        [WebMethod]
        public String ImportTaskPtl(TaskPtlIfzFunctional taskPtlIfzFunc)
        {
            try
            {
                Initialize();
                method = "ImportTaskPtl";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    if (nroTicket > 0)
                    {
                        if (taskPtlIfzFunc.ListTaskPtlIfz != null && taskPtlIfzFunc.ListTaskPtlIfz.Count > 0)
                        {
                            WServiceMessageIfz ws = new WServiceMessageIfz();
                            ws.Ticket = nroTicket;
                            ws.Objeto = typeof(TaskPtlIfz).Name.Trim();
                            ws.ListXml = SerializeObject(taskPtlIfzFunc.ListTaskPtlIfz, false);
                            ws.Transferido = true;
                            ws.DateCreated = DateTime.Now;

                            wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                            if (wServiceMessageIfzViewDTO.hasError())
                            {
                                if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                statusProcess = "ERROR";
                                clientResponse = "Error al insertar en WServiceMessage " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                            }
                            else
                            {
                                statusProcess = "OK";
                                clientResponse = "TICKET: " + nroTicket;
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [TaskPtlIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " ImportPtl");
            }

            return clientResponse;
        }

        [WebMethod]
        public string ConfirmNroTicketImport(int nroTicket)
        {
            XmlDocument theXmlDoc = new XmlDocument();
            XmlElement theRootNode;
            XmlElement theMessageNode;
            XmlElement theErrors;

            try
            {
                Initialize();
                WServiceMessageIfz ws = new WServiceMessageIfz();
                ws.Ticket = nroTicket;

                theRootNode = theXmlDoc.CreateElement("root");
                theXmlDoc.AppendChild(theRootNode);

                theMessageNode = theXmlDoc.CreateElement("messages");
                theRootNode.AppendChild(theMessageNode);
                XmlElement ticket = theXmlDoc.CreateElement("ticket");

                wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.GetByAnyParameter(ws);

                if (wServiceMessageIfzViewDTO.hasError())
                {
                    if (wServiceMessageIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                    {
                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                        throw new Exception(statusMessage);
                    }
                    else
                    {
                        //genericError = false;
                        statusProcess = "0";
                        statusMessage = "Ticket NO Registrado.";

                        ticket.InnerText = nroTicket.ToString();
                        theMessageNode.AppendChild(ticket);

                        XmlElement message = theXmlDoc.CreateElement("message");
                        message.InnerText = statusProcess + " " + statusMessage;

                        theMessageNode.AppendChild(message);
                    }
                }
                else
                {
                    //Cantidad de Registro por Ticket
                    int cantRegTicket = wServiceMessageIfzViewDTO.Entities.Count();
                    //int cantRegTransferido = wServiceMessageIfzViewDTO.Entities.Where(w => w.Transferido == true).Count();
                    int cantRegProcesado = wServiceMessageIfzViewDTO.Entities.Where(w => w.Procesado == true).Count();
                    int cantRegNoProcesado = wServiceMessageIfzViewDTO.Entities.Where(w => w.Procesado == false).Count();
                    int cantRegProcesadoErr = wServiceMessageIfzViewDTO.Entities.Where(w => !string.IsNullOrEmpty(w.Error)).Count();

                    if (wServiceMessageIfzViewDTO.Entities != null && wServiceMessageIfzViewDTO.Entities.Count > 0)
                    {

                        if (cantRegTicket == cantRegNoProcesado)
                        {
                            //genericError = false;
                            statusProcess = "Code: 0";
                            statusMessage = "Ticket Fue Transferido.";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);

                            XmlElement message = theXmlDoc.CreateElement("message");
                            message.InnerText = statusProcess + " " + statusMessage;

                            theMessageNode.AppendChild(message);
                        }
                        else if (cantRegProcesado > 0 && cantRegProcesado < cantRegTicket)
                        {
                            statusProcess = "Code: 0";
                            statusMessage = "Ticket En Proceso.";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);
                            XmlElement message = theXmlDoc.CreateElement("message");
                            message.InnerText = statusProcess + " " + statusMessage;

                            theMessageNode.AppendChild(message);
                        }
                        else if (cantRegProcesado > 0 && cantRegProcesado == cantRegTicket && cantRegProcesadoErr == 0)
                        {
                            statusProcess = "Code: 0";
                            statusMessage = "Ticket Fue Procesado.";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);

                            XmlElement message = theXmlDoc.CreateElement("message");
                            message.InnerText = statusMessage;

                            theMessageNode.AppendChild(message);
                        }
                        else if (cantRegProcesado > 0 && cantRegProcesado == cantRegTicket && cantRegProcesadoErr > 0)
                        {
                            statusProcess = "Code: 1";
                            statusMessage = "Ticket Procesado Con Error: ";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);

                            //theErrors = theXmlDoc.CreateElement("errors");
                            //theMessageNode.AppendChild(theErrors);

                            var wsErrors = wServiceMessageIfzViewDTO.Entities.Where(w => !string.IsNullOrEmpty(w.Error));
                            foreach (var wsMess in wsErrors)
                            {
                                var errorNode = XmlFormatHelper.ErrorMsgToXml(wsMess.Error);
                                var importErrorNode = theMessageNode.OwnerDocument.ImportNode(errorNode, true);
                                theMessageNode.AppendChild(importErrorNode);
                            }
                        }

                        wServiceMessageIfzViewDTO.Entities[0].DateConfirmTicket = DateTime.Now;

                        var updateServiceMessageDTO = theWServiceMessageIfzDAO.UpdateTransferConfirm(wServiceMessageIfzViewDTO.Entities[0]);

                        if (updateServiceMessageDTO.hasError())
                        {
                            if (updateServiceMessageDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                            {
                                statusMessage = updateServiceMessageDTO.Errors.ClassFullName + "::" + updateServiceMessageDTO.Errors.Method + " - " + updateServiceMessageDTO.Errors.Title + " - " + updateServiceMessageDTO.Errors.Message + " - " + updateServiceMessageDTO.Errors.OriginalMessage;
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                    else
                    {
                        //genericError = false;
                        statusProcess = "Code: 0";
                        statusMessage = "Ticket No Valido.";

                        ticket.InnerText = nroTicket.ToString();
                        theMessageNode.AppendChild(ticket);

                        XmlElement message = theXmlDoc.CreateElement("message");
                        message.InnerText = statusProcess + " " + statusMessage;

                        theMessageNode.AppendChild(message);
                    }
                }
            }
            catch (Exception ex)
            {
                genericError = false;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            //finally
            //{
            //    // Registra el movimiento realizado
            //    SetClientMessage(" TICKET: NO REGISTRADO", "wsExport");
            //}

            //return clientResponse;
            return theXmlDoc.InnerXml;
        }

        private XmlElement ErrorMsgNode(string errorMsg)
        {
            var theXmlDoc = new XmlDocument();
            var errors = theXmlDoc.CreateElement("errors");

            var error = theXmlDoc.CreateElement("error");
            error.InnerText = errorMsg;
            errors.AppendChild(error);

            return errors;
        }
       


        private XmlElement MessageNode(string errorMsg)
        {
            var theXmlDoc = new XmlDocument();
            //var errors = theXmlDoc.CreateElement("errors");

            var error = theXmlDoc.CreateElement("message");
            error.InnerText = errorMsg;
            //errors.AppendChild(error);

            return error;
        }

    }
}
