using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Funtional.Integration;
using Binaria.WMSTek.Framework.Entities.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.IntegrationClient.Integration;
using System.Web.Services.Protocols;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Summary description for wsImportIfz
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsImportIfz : wsBase
    {
        public AuthWS Login;
        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo ItemIfz en el Sistema.
        /// </summary>
        /// <param name="itemIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportItem(ItemIfzFuntional itemIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportItem";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (itemIfzFun != null && itemIfzFun.ListItemIfz != null && itemIfzFun.ListItemIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(itemIfzFun.ListItemIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (ItemIfz item in itemIfzFun.ListItemIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(ItemIfz).Name.Trim();
                                ws.ListXml = SerializeObject(item, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }                           
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [ItemchIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo ItemUomIfz en el Sistema.
        /// </summary>
        /// <param name="itemUomIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportItemUom(ItemUomIfzFuntional itemUomIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportItemUom";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (itemUomIfzFun != null && itemUomIfzFun.ListItemUomIfz != null && itemUomIfzFun.ListItemUomIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(itemUomIfzFun.ListItemUomIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (ItemUomIfz itemUom in itemUomIfzFun.ListItemUomIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(ItemUomIfz).Name.Trim();
                                ws.ListXml = SerializeObject(itemUom, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [ItemUomIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo UomTypeIfz en el Sistema.
        /// </summary>
        /// <param name="uomTypeIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportUomType(UomTypeIfzFuntional uomTypeIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportUomType";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (uomTypeIfzFun != null && uomTypeIfzFun.ListUomTypeIfz != null && uomTypeIfzFun.ListUomTypeIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(uomTypeIfzFun.ListUomTypeIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (UomTypeIfz uomType in uomTypeIfzFun.ListUomTypeIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(UomTypeIfz).Name.Trim();
                                ws.ListXml = SerializeObject(uomType, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [UomTypeIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo GrpItem1Ifz en el Sistema.
        /// </summary>
        /// <param name="grpItem1IfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportGrpItem1(GrpItem1IfzFuntional grpItem1IfzFun)
        {
            try
            {
                Initialize();
                method = "ImportGrpItem1";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (grpItem1IfzFun != null && grpItem1IfzFun.ListGrpItem1Ifz != null && grpItem1IfzFun.ListGrpItem1Ifz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(grpItem1IfzFun.ListGrpItem1Ifz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (GroupItem1Ifz grpItem1 in grpItem1IfzFun.ListGrpItem1Ifz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(GroupItem1Ifz).Name.Trim();
                                ws.ListXml = SerializeObject(grpItem1, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [GrpItem1Ifz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo GrpItem2Ifz en el Sistema.
        /// </summary>
        /// <param name="grpItem2IfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportGrpItem2(GrpItem2IfzFuntional grpItem2IfzFun)
        {
            try
            {
                Initialize();
                method = "ImportGrpItem2";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (grpItem2IfzFun != null && grpItem2IfzFun.ListGrpItem2Ifz != null && grpItem2IfzFun.ListGrpItem2Ifz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(grpItem2IfzFun.ListGrpItem2Ifz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (GroupItem2Ifz grpItem2 in grpItem2IfzFun.ListGrpItem2Ifz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(GroupItem2Ifz).Name.Trim();
                                ws.ListXml = SerializeObject(grpItem2, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [GrpItem2Ifz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo GrpItem3Ifz en el Sistema.
        /// </summary>
        /// <param name="grpItem3IfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportGrpItem3(GrpItem3IfzFuntional grpItem3IfzFun)
        {
            try
            {
                Initialize();
                method = "ImportGrpItem3";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (grpItem3IfzFun != null && grpItem3IfzFun.ListGrpItem3Ifz != null && grpItem3IfzFun.ListGrpItem3Ifz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(grpItem3IfzFun.ListGrpItem3Ifz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (GroupItem3Ifz grpItem3 in grpItem3IfzFun.ListGrpItem3Ifz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(GroupItem3Ifz).Name.Trim();
                                ws.ListXml = SerializeObject(grpItem3, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [GrpItem3Ifz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo GrpItem4Ifz en el Sistema.
        /// </summary>
        /// <param name="grpItem4IfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportGrpItem4(GrpItem4IfzFuntional grpItem4IfzFun)
        {
            try
            {
                Initialize();
                method = "ImportGrpItem4";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (grpItem4IfzFun != null && grpItem4IfzFun.ListGrpItem4Ifz != null && grpItem4IfzFun.ListGrpItem4Ifz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(grpItem4IfzFun.ListGrpItem4Ifz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (GroupItem4Ifz grpItem4 in grpItem4IfzFun.ListGrpItem4Ifz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(GroupItem4Ifz).Name.Trim();
                                ws.ListXml = SerializeObject(grpItem4, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [GrpItem4Ifz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo VendorIfz en el Sistema.
        /// </summary>
        /// <param name="vendorIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportVendor(VendorIfzFuntional vendorIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportVendor";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (vendorIfzFun != null && vendorIfzFun.ListVendorIfz != null && vendorIfzFun.ListVendorIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(vendorIfzFun.ListVendorIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (VendorIfz vendor in vendorIfzFun.ListVendorIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(VendorIfz).Name.Trim();
                                ws.ListXml = SerializeObject(vendor, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [VendorIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo CustomerIfz en el Sistema.
        /// </summary>
        /// <param name="customerIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportCustomer(CustomerIfzFuntional customerIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportCustomer";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (customerIfzFun != null && customerIfzFun.ListCustomerIfz != null && customerIfzFun.ListCustomerIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(customerIfzFun.ListCustomerIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (CustomerIfz customer in customerIfzFun.ListCustomerIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(CustomerIfz).Name.Trim();
                                ws.ListXml = SerializeObject(customer, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [CustomerIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo ItemCustomerIfz en el Sistema.
        /// </summary>
        /// <param name="itemCustomerIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportItemCustomer(ItemCustomerIfzFuntional itemCustomerIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportItemCustomer";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (itemCustomerIfzFun != null && itemCustomerIfzFun.ListItemCustomerIfz != null && itemCustomerIfzFun.ListItemCustomerIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(itemCustomerIfzFun.ListItemCustomerIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (ItemCustomerIfz itemCustomer in itemCustomerIfzFun.ListItemCustomerIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(ItemCustomerIfz).Name.Trim();
                                ws.ListXml = SerializeObject(itemCustomer, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [ItemCustomerIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo BranchIfz en el Sistema.
        /// </summary>
        /// <param name="branchIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportBranch(BranchIfzFuntional branchIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportBranch";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (branchIfzFun != null && branchIfzFun.ListBranchIfz != null && branchIfzFun.ListBranchIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(branchIfzFun.ListBranchIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (BranchIfz branch in branchIfzFun.ListBranchIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(BranchIfz).Name.Trim();
                                ws.ListXml = SerializeObject(branch, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [BranchIf] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo ItemVendorIfz en el Sistema.
        /// </summary>
        /// <param name="itemVendorIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportItemVendor(ItemVendorIfzFuntional itemVendorIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportItemVendor";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (itemVendorIfzFun != null && itemVendorIfzFun.ListItemVendorIfz != null && itemVendorIfzFun.ListItemVendorIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(itemVendorIfzFun.ListItemVendorIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (ItemVendorIfz itemVendor in itemVendorIfzFun.ListItemVendorIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(ItemVendorIfz).Name.Trim();
                                ws.ListXml = SerializeObject(itemVendor, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [ItemVendorIf] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo ReferenceDocIfz en el Sistema.
        /// </summary>
        /// <param name="referenceDocIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportReferenceDoc(ReferenceDocIfzFuntional referenceDocIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportReferenceDoc";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (referenceDocIfzFun != null && referenceDocIfzFun.ListReferenceDocIfz != null && referenceDocIfzFun.ListReferenceDocIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(referenceDocIfzFun.ListReferenceDocIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (ReferenceDocIfz referenceDoc in referenceDocIfzFun.ListReferenceDocIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(ReferenceDocIfz).Name.Trim();
                                ws.ListXml = SerializeObject(referenceDoc, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [ReferenceDocIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo KitIfz en el Sistema.
        /// </summary>
        /// <param name="KitIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportKit(KitIfzFuntional kitIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportKit";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (kitIfzFun != null && kitIfzFun.ListKitIfz != null && kitIfzFun.ListKitIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(kitIfzFun.ListKitIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (KitIfz kit in kitIfzFun.ListKitIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(KitIfz).Name.Trim();
                                ws.ListXml = SerializeObject(kit, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [KitIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo InboundOrderIfz en el Sistema.
        /// </summary>
        /// <param name="inboundOrderIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportInboundOrder(InboundOrderIfzFuntional inboundOrderIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportInboundOrder";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (inboundOrderIfzFun != null && inboundOrderIfzFun.ListInboundOrderIfz != null && inboundOrderIfzFun.ListInboundOrderIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(inboundOrderIfzFun.ListInboundOrderIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (InboundOrderIfz inboundOrder in inboundOrderIfzFun.ListInboundOrderIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(InboundOrderIfz).Name.Trim();
                                ws.ListXml = SerializeObject(inboundOrder, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [InboundOrderIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
            }

            return clientResponse;
        }

        /// <summary>
        /// Metodo Utilizado para Registrar Objetos del Tipo OutboundOrderIfz en el Sistema.
        /// </summary>
        /// <param name="outboundOrderIfzFun"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("Login")]
        public String ImportOutboundOrder(OutboundOrderIfzFuntional outboundOrderIfzFun)
        {
            try
            {
                Initialize();
                method = "ImportOutboundOrder";

                if (initMsg == "OK")
                {
                    Thread.Sleep(500);

                    int nroTicket = GetNroTicket();

                    //Valida que Exista un ticket valido 
                    if (nroTicket > 0)
                    {
                        //Valida que existan datos a insertar
                        if (outboundOrderIfzFun != null && outboundOrderIfzFun.ListOutboundOrderIfz != null && outboundOrderIfzFun.ListOutboundOrderIfz.Count > 0)
                        {
                            var validateAuth = ValidateAuth(outboundOrderIfzFun.ListOutboundOrderIfz.Select(o => o.OwnCode).ToList());

                            if (!string.IsNullOrEmpty(validateAuth))
                                return validateAuth;

                            foreach (OutboundOrderIfz outboundOrder in outboundOrderIfzFun.ListOutboundOrderIfz)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(OutboundOrderIfz).Name.Trim();
                                ws.ListXml = SerializeObject(outboundOrder, false);
                                ws.Transferido = true;
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
                                    statusProcess = "OK";
                                    clientResponse = "TICKET: " + nroTicket;
                                }
                            }
                        }
                        else
                        {
                            statusProcess = "ERROR";
                            clientResponse = "OBJETO [OutboundOrderIfz] NO VALIDO";
                        }
                    }
                    else
                    {
                        statusProcess = "ERROR";
                        clientResponse = "TICKET NO VALIDO";
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ERROR";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                //RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");

                SetClientMessage(" TRANSACCION NO REALIZADA", "wsImport");
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
                        statusProcess = "OK";
                        statusMessage = "Ticket NO Registrado.";

                        ticket.InnerText = nroTicket.ToString();
                        theMessageNode.AppendChild(ticket);

                        XmlElement message = theXmlDoc.CreateElement("message");
                        message.InnerText = statusMessage;

                        theMessageNode.AppendChild(message);
                    }
                }
                else
                {
                    //Cantidad de Registro por Ticket
                    int cantRegTicket = wServiceMessageIfzViewDTO.Entities.Count();                    
                    int cantRegProcesado = wServiceMessageIfzViewDTO.Entities.Where(w => w.Procesado == true).Count();
                    int cantRegNoProcesado = wServiceMessageIfzViewDTO.Entities.Where(w => w.Procesado == false).Count();
                    int cantRegProcesadoErr = wServiceMessageIfzViewDTO.Entities.Where(w => !string.IsNullOrEmpty(w.Error)).Count();

                    if (wServiceMessageIfzViewDTO.Entities != null && wServiceMessageIfzViewDTO.Entities.Count > 0)
                    {         

                        if (cantRegTicket == cantRegNoProcesado)
                        {
                            //genericError = false;
                            statusProcess = "OK";
                            statusMessage = "Ticket Fue Transferido.";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);

                            XmlElement message = theXmlDoc.CreateElement("message");
                            message.InnerText = statusMessage;

                            theMessageNode.AppendChild(message);
                        }
                        else if (cantRegProcesado > 0 && cantRegProcesado < cantRegTicket)
                        {
                            statusProcess = "OK";
                            statusMessage = "Ticket En Proceso.";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);
                            XmlElement message = theXmlDoc.CreateElement("message");
                            message.InnerText = statusMessage;

                            theMessageNode.AppendChild(message);
                        }
                        else if (cantRegProcesado > 0 && cantRegProcesado == cantRegTicket && cantRegProcesadoErr == 0)
                        {
                            statusProcess = "OK";
                            statusMessage = "Ticket Fue Procesado.";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);

                            XmlElement message = theXmlDoc.CreateElement("message");
                            message.InnerText = statusMessage;

                            theMessageNode.AppendChild(message);
                        }
                        else if (cantRegProcesado > 0 && cantRegProcesado == cantRegTicket && cantRegProcesadoErr > 0)
                        {
                            statusProcess = "OK";
                            statusMessage = "Ticket Procesado Con Error: ";

                            ticket.InnerText = nroTicket.ToString();
                            theMessageNode.AppendChild(ticket);

                            var wsErrors = wServiceMessageIfzViewDTO.Entities.Where(w => !string.IsNullOrEmpty(w.Error));
                            foreach (var wsMess in wsErrors)
                            {
                                if (wsMess.Error.IndexOf("</error>") > -1)
                                {
                                    while (wsMess.Error.IndexOf("</error>") > -1)
                                    {
                                        var message = wsMess.Error.Substring(0, wsMess.Error.IndexOf("</error>") + 8);
                                        wsMess.Error = wsMess.Error.Remove(0, wsMess.Error.IndexOf("</error>") + 8);

                                        var errorNode = XmlFormatHelper.ErrorMsgToXml(message);
                                        var importErrorNode = theMessageNode.OwnerDocument.ImportNode(errorNode, true);
                                        theMessageNode.AppendChild(importErrorNode);
                                    }                                    
                                }
                                else
                                {
                                    var errorNode = XmlFormatHelper.ErrorMsgToXml(wsMess.Error);
                                    var importErrorNode = theMessageNode.OwnerDocument.ImportNode(errorNode, true);
                                    theMessageNode.AppendChild(importErrorNode);
                                }
                            }                                
                        }
                    }
                    else
                    {
                        //genericError = false;
                        statusProcess = "OK";
                        statusMessage = "Ticket No Valido.";

                        ticket.InnerText = nroTicket.ToString();
                        theMessageNode.AppendChild(ticket);

                        XmlElement message = theXmlDoc.CreateElement("message");
                        message.InnerText = statusMessage;

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
        private string ValidateAuth(List<string> ownCodes)
        {
            var errorMsg = string.Empty;

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
                        errorMsg = ERR_MSG_AUTH;
                }
                else
                    errorMsg = ERR_MSG_VAL_AUTH;

                if (string.IsNullOrEmpty(errorMsg))
                {
                    var isValid = ValidateOwnerByUser(ownCodes, userCredentials);

                    if (!isValid)
                        errorMsg = ERR_INVALID_OWNER_LIST;
                }
            }

            return errorMsg;
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
