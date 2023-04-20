using Binaria.WMSTek.DataAccess.Integration;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Funtional.Integration;
using Binaria.WMSTek.Framework.Entities.Integration;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.IntegrationClient.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Summary description for wsExportIfz
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsExportIfz : wsBase
    {
        public AuthWS Login;
        /// <summary>
        /// Metodo Utilizado para Obtener Objetos del Tipo ReceiptIfz en el Sistema.
        /// </summary>
        /// <param name="QtyReceiptToRet">Cantidad de registros que se desea retornar</param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public ReceiptIfzFuntional ExportReceipt(int QtyReceiptToRet)
        {
            ReceiptIfzFuntional receiptReturn = new ReceiptIfzFuntional();
            List<ReceiptIfz> ListReceiptIfz = new List<ReceiptIfz>();
            try
            {

                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth();

                    GenericViewDTO<ReceiptIfz> receiptIfzViewDTO = theReceiptIfzDAO.GetUnProcessedWithPagination(QtyReceiptToRet, context.MainFilter);

                    if (receiptIfzViewDTO.hasError())
                    {
                        if (receiptIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = receiptIfzViewDTO.Errors.ClassFullName + "::" + receiptIfzViewDTO.Errors.Method + " - " + receiptIfzViewDTO.Errors.Title + " - " + receiptIfzViewDTO.Errors.Message + " - " + receiptIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {
                        int nroTicket = GetNroTicket();

                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)
                        {
                            receiptReturn.NroTicket = nroTicket;
                            List<ReceiptIfz> lstReceiptIfz = new List<ReceiptIfz>();                           
                            
                            Thread.Sleep(500);

                            if (receiptIfzViewDTO.Entities != null && receiptIfzViewDTO.Entities.Count > 0)
                            {
                                foreach (ReceiptIfz rp in receiptIfzViewDTO.Entities)
                                {
                                    GenericViewDTO<ReceiptDetailIfz> recepDetIfzViewDTO = theReceiptDetailIfzDAO.GetReceiptDetailIfzByIdReceipt(rp.Id);

                                    rp.ReceiptDetailsIfz = recepDetIfzViewDTO.Entities;

                                    rp.ReceiptDetailsIfz.ForEach(rd => rd.IdLpnCode = MiscUtils.TrimNonAscii(rd.IdLpnCode));

                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(ReceiptIfz).Name.Trim();
                                    ws.ListXml = SerializeObject(rp, true);
                                    ws.IdObjeto = rp.Id;
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    }
                                    else
                                    {
                                        ListReceiptIfz.Add(rp);
                                    }
                                }
                                receiptReturn.ListReceiptIfz = ListReceiptIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return receiptReturn;
        }

        [WebMethod]
        [SoapHeader("Login")]
        public ReceiptIfzFuntional ExportReceiptsForNoReceiptedItems(int QtyReceiptToRet, string ownCode)
        {
            ReceiptIfzFuntional receiptReturn = new ReceiptIfzFuntional();
            List<ReceiptIfz> ListReceiptIfz = new List<ReceiptIfz>();
            try
            {

                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth(ownCode);

                    GenericViewDTO<ReceiptIfz> receiptIfzViewDTO = theReceiptIfzDAO.GetUnProcessedWithPaginationForNoReceiptedItems(QtyReceiptToRet, context.MainFilter);

                    if (receiptIfzViewDTO.hasError())
                    {
                        if (receiptIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = receiptIfzViewDTO.Errors.ClassFullName + "::" + receiptIfzViewDTO.Errors.Method + " - " + receiptIfzViewDTO.Errors.Title + " - " + receiptIfzViewDTO.Errors.Message + " - " + receiptIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {
                        int nroTicket = GetNroTicket();

                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)
                        {
                            receiptReturn.NroTicket = nroTicket;
                            List<ReceiptIfz> lstReceiptIfz = new List<ReceiptIfz>();

                            Thread.Sleep(500);

                            if (receiptIfzViewDTO.Entities != null && receiptIfzViewDTO.Entities.Count > 0)
                            {
                                foreach (ReceiptIfz rp in receiptIfzViewDTO.Entities)
                                {
                                    GenericViewDTO<ReceiptDetailIfz> recepDetIfzViewDTO = theReceiptDetailIfzDAO.GetReceiptDetailIfzByIdReceipt(rp.Id);

                                    rp.ReceiptDetailsIfz = recepDetIfzViewDTO.Entities;

                                    rp.ReceiptDetailsIfz.ForEach(rd => rd.IdLpnCode = MiscUtils.TrimNonAscii(rd.IdLpnCode));

                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(ReceiptIfz).Name.Trim();
                                    ws.ListXml = SerializeObject(rp, true);
                                    ws.IdObjeto = rp.Id;
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    }
                                    else
                                    {
                                        ListReceiptIfz.Add(rp);
                                    }
                                }
                                receiptReturn.ListReceiptIfz = ListReceiptIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return receiptReturn;
        }

        /// <summary>
        /// Metodo Utilizado para Obtener Objetos del Tipo DispatchIfz en el Sistema.
        /// </summary>
        /// <param name="QtyDispatchToRet">Cantidad de registros que se desea retornar</param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public DispatchIfzFuntional ExportDispatch(int QtyDispatchToRet)
        {
            DispatchIfzFuntional dispatchReturn = new DispatchIfzFuntional();

            List<DispatchIfz> ListDispatchIfz = new List<DispatchIfz>();
            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth();

                    GenericViewDTO<DispatchIfz> dispatchIfzViewDTO = theDispatchIfzDAO.GetUnProcessedWithPagination(QtyDispatchToRet, context.MainFilter);

                    if (dispatchIfzViewDTO.hasError())
                    {
                        if (dispatchIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = dispatchIfzViewDTO.Errors.ClassFullName + "::" + dispatchIfzViewDTO.Errors.Method + " - " + dispatchIfzViewDTO.Errors.Title + " - " + dispatchIfzViewDTO.Errors.Message + " - " + dispatchIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {
                        
                        //Rescata nuevo nro de ticket
                        int nroTicket = GetNroTicket();
                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)
                        {
                            dispatchReturn.NroTicket = nroTicket;

                            Thread.Sleep(500);

                            //Valida que existan datos a insertar
                            if (dispatchIfzViewDTO.Entities != null && dispatchIfzViewDTO.Entities.Count > 0)
                            {
                                foreach (DispatchIfz dp in dispatchIfzViewDTO.Entities)
                                {
                                    GenericViewDTO<DispatchDetailIfz> dispDetIfzViewDTO = theDispatchDetailIfzDAO.GetDispatchDetailIfzByIdDispatch(dp.Id);

                                    dp.DispatchDetailsIfz = dispDetIfzViewDTO.Entities;

                                    dp.DispatchDetailsIfz.ForEach(dd => dd.IdLpnCode = MiscUtils.TrimNonAscii(dd.IdLpnCode));

                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(DispatchIfz).Name.Trim();
                                    ws.ListXml = SerializeObject<DispatchIfz>(dp, true);
                                    ws.IdObjeto = dp.Id;
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                        //throw new Exception(statusMessage);
                                    }
                                    else
                                    {
                                        ListDispatchIfz.Add(dp);
                                    }
                                }
                                dispatchReturn.ListDispatchIfz = ListDispatchIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }                    
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return dispatchReturn;
        }

        /// <summary>
        /// Metodo Utilizado para Obtener Objetos del Tipo MovementAdjust en el Sistema.
        /// </summary>
        /// <param name="QtyMovementAdjustToRet">Cantidad de registros que se desea retornar</param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public MovementAdjustIfzFuntional ExportMovementAdjust(int QtyMovementAdjustToRet)
        {
            MovementAdjustIfzFuntional movementAdjustReturn = new MovementAdjustIfzFuntional();
            List<MovementAdjustIfz> ListMovementAdjustIfz = new List<MovementAdjustIfz>();
            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth();

                    GenericViewDTO<MovementAdjustIfz> movAdjustIfzViewDTO = theMovementAdjustIfzDAO.GetUnProcessedWithPagination(QtyMovementAdjustToRet, context.MainFilter);

                    if (movAdjustIfzViewDTO.hasError())
                    {
                        if (movAdjustIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = movAdjustIfzViewDTO.Errors.ClassFullName + "::" + movAdjustIfzViewDTO.Errors.Method + " - " + movAdjustIfzViewDTO.Errors.Title + " - " + movAdjustIfzViewDTO.Errors.Message + " - " + movAdjustIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {

                        //Rescata nuevo nro de ticket
                        int nroTicket = GetNroTicket();

                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)                            
                        {
                            movementAdjustReturn.NroTicket = nroTicket;
                            Thread.Sleep(500);

                            //Valida que existan datos a insertar
                            if (movAdjustIfzViewDTO.Entities != null && movAdjustIfzViewDTO.Entities.Count > 0)
                            {
                                movAdjustIfzViewDTO.Entities.ForEach(ma => ma.IdLpnCode = MiscUtils.TrimNonAscii(ma.IdLpnCode));

                                foreach (MovementAdjustIfz  mv in movAdjustIfzViewDTO.Entities)
                                {
                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(MovementAdjustIfz).Name.Trim();
                                    ws.IdObjeto = mv.Id;
                                    ws.ListXml = SerializeObject<MovementAdjustIfz>(mv, true);
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    }
                                    else
                                    {
                                        ListMovementAdjustIfz.Add(mv);
                                        
                                    }
                                }
                                movementAdjustReturn.ListMovementAdjustIfz = ListMovementAdjustIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }

            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return movementAdjustReturn;
        }

        /// <summary>
        /// Metodo Utilizado para Obtener Objetos del Tipo BillingLog en el Sistema.
        /// </summary>
        /// <param name="QtyBillingLogToRet">Cantidad de registros que se desea retornar</param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public BillingLogIfzFuntional ExportBillingLog(int QtyBillingLogToRet)
        {
            BillingLogIfzFuntional billingLogReturn = new BillingLogIfzFuntional();
            List<BillingLogIfz> ListBillingLogIfz = new List<BillingLogIfz>();
            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth();

                    GenericViewDTO<BillingLogIfz> billingLogIfzViewDTO = theBillingLogIfzDAO.GetUnProcessedWithPagination(QtyBillingLogToRet, context.MainFilter);

                    if (billingLogIfzViewDTO.hasError())
                    {
                        if (billingLogIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = billingLogIfzViewDTO.Errors.ClassFullName + "::" + billingLogIfzViewDTO.Errors.Method + " - " + billingLogIfzViewDTO.Errors.Title + " - " + billingLogIfzViewDTO.Errors.Message + " - " + billingLogIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {

                        
                        //Rescata nuevo nro de ticket
                        int nroTicket = GetNroTicket();

                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)
                        {
                            billingLogReturn.NroTicket = nroTicket;
                            Thread.Sleep(500);

                            if (billingLogIfzViewDTO.Entities != null && billingLogIfzViewDTO.Entities.Count > 0)
                            {
                                foreach (BillingLogIfz bl in billingLogIfzViewDTO.Entities)
                                {
                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(BillingLogIfz).Name.Trim();
                                    ws.IdObjeto = bl.Id;
                                    ws.ListXml = SerializeObject<BillingLogIfz>(bl, true);
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    }
                                    else
                                    {
                                        ListBillingLogIfz.Add(bl);
                                    }
                                }
                                billingLogReturn.ListBillingLogIfz = ListBillingLogIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return billingLogReturn;
        }

        /// <summary>
        /// Metodo Utilizado para Obtener Objetos del Tipo Putaway en el Sistema.
        /// </summary>
        /// <param name="QtyPutawayToRet">Cantidad de registros que se desea retornar</param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public PutawayIfzFuntional ExportPutaway(int QtyPutawayToRet)
        {
            PutawayIfzFuntional putawayReturn = new PutawayIfzFuntional();
            List<PutawayIfz> ListPutawayIfz = new List<PutawayIfz>();
            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth();

                    GenericViewDTO<PutawayIfz> putawayIfzViewDTO = thePutawayIfzDAO.GetUnProcessedWithPagination(QtyPutawayToRet, context.MainFilter);

                    if (putawayIfzViewDTO.hasError())
                    {
                        if (putawayIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = putawayIfzViewDTO.Errors.ClassFullName + "::" + putawayIfzViewDTO.Errors.Method + " - " + putawayIfzViewDTO.Errors.Title + " - " + putawayIfzViewDTO.Errors.Message + " - " + putawayIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {
                        //Rescata nuevo nro de ticket
                        int nroTicket = GetNroTicket();

                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)
                        {
                            putawayReturn.NroTicket = nroTicket;
                            Thread.Sleep(500);

                            if (putawayIfzViewDTO.Entities != null && putawayIfzViewDTO.Entities.Count > 0)
                            {
                                foreach (PutawayIfz put in putawayIfzViewDTO.Entities)
                                {
                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(PutawayIfz).Name.Trim();
                                    ws.IdObjeto = put.Id;
                                    ws.ListXml = SerializeObject<PutawayIfz>(put, true);
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    }
                                    else
                                    {
                                        ListPutawayIfz.Add(put);
                                    }
                                }
                                putawayReturn.ListPutawayIfz = ListPutawayIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return putawayReturn;
        }

        /// <summary>
        /// Metodo Utilizado para Obtener Objetos del Tipo DispatchIfz en el Sistema.
        /// </summary>
        /// <param name="QtyDispatchToRet">Cantidad de registros que se desea retornar</param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public SerialTrackIfzFuntional ExportSerialTrack(int QtyDispatchToRet)
        {
            SerialTrackIfzFuntional serialTrackReturn = new SerialTrackIfzFuntional();

            List<SerialTrackIfz> ListSerialTrackIfz = new List<SerialTrackIfz>();
            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    ValidateAuth();

                    GenericViewDTO<SerialTrackIfz> serialTrackIfzViewDTO = theSerialTrackIfzDAO.GetUnProcessedWithPagination(QtyDispatchToRet, context.MainFilter);

                    if (serialTrackIfzViewDTO.hasError())
                    {
                        if (serialTrackIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = serialTrackIfzViewDTO.Errors.ClassFullName + "::" + serialTrackIfzViewDTO.Errors.Method + " - " + serialTrackIfzViewDTO.Errors.Title + " - " + serialTrackIfzViewDTO.Errors.Message + " - " + serialTrackIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {

                        //Rescata nuevo nro de ticket
                        int nroTicket = GetNroTicket();
                        //Valida que Exista un ticket valido 
                        if (nroTicket > 0)
                        {
                            serialTrackReturn.NroTicket = nroTicket;

                            Thread.Sleep(500);

                            //Valida que existan datos a insertar
                            if (serialTrackIfzViewDTO.Entities != null && serialTrackIfzViewDTO.Entities.Count > 0)
                            {
                                foreach (SerialTrackIfz dp in serialTrackIfzViewDTO.Entities)
                                {
                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(SerialTrackIfz).Name.Trim();
                                    ws.ListXml = SerializeObject<SerialTrackIfz>(dp, true);
                                    ws.IdObjeto = dp.Id;
                                    ws.Transferido = false;
                                    ws.Procesado = false;
                                    ws.DateCreated = DateTime.Now;

                                    //Inserta Registro en la BD
                                    wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                    if (wServiceMessageIfzViewDTO.hasError())
                                    {
                                        if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                        statusProcess = "ERROR";
                                        clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                        statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                        //throw new Exception(statusMessage);
                                    }
                                    else
                                    {
                                        ListSerialTrackIfz.Add(dp);
                                    }
                                }
                                serialTrackReturn.ListSerialTrackIfz = ListSerialTrackIfz;
                            }
                            else
                            {
                                statusMessage = "ERROR NO EXISTEN DATOS";
                                throw new Exception(statusMessage);
                            }
                        }
                    }
                }
                else
                {
                    statusMessage = initMsg;
                    throw new Exception(statusMessage);
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return serialTrackReturn;
        }

        /// <summary>
        /// Metodo Utilizado para Confirmar numero de ticket.
        /// </summary>
        /// <param>OK o ERROR</param>
        /// <returns></returns>
        [WebMethod]
        public String ConfirmNroTicketExport(int nroTicket)
        {
            try
            {
                Initialize();
                WServiceMessageIfz ws = new WServiceMessageIfz();
                ws.Ticket = nroTicket;

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
                        statusProcess = "OK";
                        statusMessage = "Ticket NO Registrado.";
                    }
                }
                else
                {

                    if (wServiceMessageIfzViewDTO.Entities != null && wServiceMessageIfzViewDTO.Entities.Count > 0)
                    {
                        if (wServiceMessageIfzViewDTO.Entities[0].Transferido)
                        {
                            //genericError = false;
                            statusProcess = "OK";
                            statusMessage = "Ticket Ya Fue Transferido.";
                        }
                        else
                        {
                            wServiceMessageIfzViewDTO.Entities[0].Transferido = true;
                            wServiceMessageIfzViewDTO.Entities[0].DateConfirmTicket = DateTime.Now;

                            switch (wServiceMessageIfzViewDTO.Entities[0].Objeto)
                            {
                                case ("DispatchIfz"):
                                    DispatchIfzDAO dispatchIfzDAO = new DispatchIfzDAO();
                                    GenericViewDTO<DispatchIfz> dispatchIfzDTO = new GenericViewDTO<DispatchIfz>();
                                    dispatchIfzDTO = dispatchIfzDAO.UpdateConfirm(nroTicket, "T");
                                break;

                                case ("ReceiptIfz"):
                                    ReceiptIfzDAO receiptIfzDAO = new ReceiptIfzDAO();
                                    GenericViewDTO<ReceiptIfz> receiptIfzDTO = new GenericViewDTO<ReceiptIfz>();
                                    receiptIfzDTO= receiptIfzDAO.UpdateConfirm(nroTicket, "T");
                                break;

                                case ("MovementAdjustIfz"):
                                    MovementIfzDAO movementIfzDAO = new MovementIfzDAO();
                                    GenericViewDTO<MovementIfz> movementIfzDTO = new GenericViewDTO<MovementIfz>();
                                    movementIfzDTO = movementIfzDAO.UpdateConfirm(nroTicket, "T");
                                break;

                                case ("BillingLogIfz"):
                                    BillingLogIfzDAO billingLogIfzDAO = new BillingLogIfzDAO();
                                    GenericViewDTO<BillingLogIfz> billingLogIfzDTO = new GenericViewDTO<BillingLogIfz>();
                                    billingLogIfzDTO = billingLogIfzDAO.UpdateConfirm(nroTicket, "T");
                                break;
                                case ("SerialTrackIfz"):
                                    SerialTrackIfzDAO serialTrackIfzDAO = new SerialTrackIfzDAO();
                                    GenericViewDTO<SerialTrackIfz> serialTrackIfDTO = new GenericViewDTO<SerialTrackIfz>();
                                    serialTrackIfDTO = serialTrackIfzDAO.UpdateConfirm(nroTicket, "T");
                                break;

                                case ("PutawayIfz"):
                                    PutawayIfzDAO putawayIfzDAO = new PutawayIfzDAO();
                                    GenericViewDTO<PutawayIfz> putawayIfzDTO = new GenericViewDTO<PutawayIfz>();
                                    putawayIfzDTO = putawayIfzDAO.UpdateConfirm(nroTicket, "T");
                                    break;

                            }

                            //update de dispatch en tabla de mensajes en la bd de interfaz 

                            //update de los mensajes correspondientes al numero de ticket en la bd de interfaz 
                            wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.UpdateTransferConfirm(wServiceMessageIfzViewDTO.Entities[0]);

                            
                            if (wServiceMessageIfzViewDTO.hasError())
                            {
                                if (wServiceMessageIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                                {
                                    statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    throw new Exception(statusMessage);
                                }
                            }
                            else
                            {

                                statusProcess = "OK";
                                statusMessage = "Ticket Transferido Correctamente.";
                            }
                        }
                    }
                    else
                    {
                        //genericError = false;
                        statusProcess = "OK";
                        statusMessage = "Ticket No Valido";
                    }
                }
            }
            catch (Exception ex)
            {
                genericError = false;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
            }
            finally
            {
                // Registra el movimiento realizado
                SetClientMessage(" TICKET: NO REGISTRADO", "wsExport");
            }

            return clientResponse;
        }
        private void ValidateAuth(string ownCodeToFilter = null)
        {
            var hasToValidateAuth = HasToValidateAuth();

            if (hasToValidateAuth)
            {
                var userCredentials = new UserCredentials();

                if (Login != null)
                {
                    if (Login.IsValid())
                    {
                        userCredentials.userName = Login.userName;
                        userCredentials.password = Login.password;
                    }
                    else
                        throw new Exception(ERR_MSG_AUTH);
                }
                else
                    throw new Exception(ERR_MSG_VAL_AUTH);

                if (string.IsNullOrEmpty(ownCodeToFilter))
                    AddFilterOwnerByOwnersPerUser(userCredentials);
                else
                    CreateFilterByOwn(userCredentials, ownCodeToFilter);
            }
        }

        private void AddFilterOwnerByOwnersPerUser(UserCredentials userCredentials)
        {
            var ownersByUser = iProfileMGR.OwnersByUser(userCredentials.userName, userCredentials.password, context);

            if (!ownersByUser.hasError() && ownersByUser.Entities.Count > 0)
            {
                var onwerFilter = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)];

                foreach (var ownerByUser in ownersByUser.Entities)
                {
                    onwerFilter.FilterValues.Add(new FilterItem(ownerByUser.Code));
                }
            }
            else
                throw new Exception(ERR_USER_HAS_NOT_OWNERS);
        }
        private void CreateFilterByOwn(UserCredentials userCredentials, string ownCode)
        {
            var ownersByUser = iProfileMGR.OwnersByUser(userCredentials.userName, userCredentials.password, context);

            if (!ownersByUser.hasError() && ownersByUser.Entities.Count > 0)
            {
                var getOwner = ownersByUser.Entities.Where(o => o.Code == ownCode).FirstOrDefault();

                if (getOwner != null)
                {
                    var onwerFilter = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)];
                    onwerFilter.FilterValues.Add(new FilterItem(ownCode));
                }
                else
                    throw new Exception(ERR_INVALID_OWNER);
            }
            else
                throw new Exception(ERR_USER_HAS_NOT_OWNERS);
        }
    }
}
