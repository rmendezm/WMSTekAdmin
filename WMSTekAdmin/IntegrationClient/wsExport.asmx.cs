using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Summary description for wsExport
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsExport : wsBase
    {
        private GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();

        #region "Receipt"
        [WebMethod]
        public String ExportReceipt(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ExportReceipt";

            Export(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }


        [WebMethod]
        public String GetXmlSchemaReceiptImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ExportReceipt" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Dispatch"
        [WebMethod]
        public String ExportDispatch(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ExportDispatch";

            Export(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaDispatchExport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ExportDispatch.xsd" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "MovementLog"
        [WebMethod]
        public String ExportMovementLog(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ExportMovementLog";

            Export(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }


        [WebMethod]
        public String GetXmlSchemaMovementLogExport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ExportMovementLog" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Generic"
        private void Export(String sourceUserName, String sourceAppName, String sourceHostName, String document, Char typeMovto)
        {
            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    // Carga el documento en un objeto XML
                    LoadXML(document);

                    if (statusProcess == "OK")
                    {
                        // Valida estructura del XML
                        ValidateXML();

                        if (statusProcess == "OK")
                        {
                            GetObject(document);

                            if (statusProcess == "OK")
                            {
                                CreateXML();
                            }
                        }
                    }
                }
                else
                {
                    genericError = true;
                    statusProcess = "ER";
                    statusMessage = initMsg;
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ER";
                statusMessage = LogException(ex, " Export");
            }
            finally
            {
                // Registra el movimiento realizado
                RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsExport");
            }
        }

        private void GetObject(String document)
        {
            switch (method)
            {
                case "ExportReceipt":
                    // Exporta Inbound con sus Recepciones
                    InboundOrder inbound = new InboundOrder();
                    inbound.Owner = new Binaria.WMSTek.Framework.Entities.Warehousing.Owner();

                    inbound.Number =                   xmlInput.ChildNodes[0].ChildNodes[0].InnerText;
                    inbound.Owner.Id = Convert.ToInt32(xmlInput.ChildNodes[0].ChildNodes[1].InnerText);
                    
                                       
                    // TODO: leer desde XML
                    inboundOrderViewDTO = iReceptionMGR.GetInboundByNumberAndOwner(inbound.Number, inbound.Owner.Id, context);

                    if (inboundOrderViewDTO.hasError())
                    {
                        if (inboundOrderViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = inboundOrderViewDTO.Errors.Title + " - " + inboundOrderViewDTO.Errors.Message;
                        statusMessage = inboundOrderViewDTO.Errors.ClassFullName + "::" + inboundOrderViewDTO.Errors.Method + " - " + inboundOrderViewDTO.Errors.Title + " - " + inboundOrderViewDTO.Errors.Message + " - " + inboundOrderViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        if (inboundOrderViewDTO.Entities.Count != 1)
                        {
                            ErrorDTO error;
                            statusProcess = "ER";

                            if (inboundOrderViewDTO.Entities.Count == 0)
                                error = baseControl.handleError(new ErrorDTO(WMSTekError.WebServices.Export.NoRecordsFound, context.LanguageCode));
                            else
                                error = baseControl.handleError(new ErrorDTO(WMSTekError.WebServices.Export.MultipleRecordsFound, context.LanguageCode));

                            clientResponse = error.Title + " - " + error.Message;
                            statusMessage = clientResponse;
                        }
                        else
                            statusProcess = "OK";
                    }
                    break;

                case "ExportDispatch":
                    // Exporta Inbound con sus Recepciones
                    OutboundOrder outbound = new OutboundOrder();
                    outbound.Owner = new Binaria.WMSTek.Framework.Entities.Warehousing.Owner();

                    outbound.Number = xmlInput.ChildNodes[0].ChildNodes[0].InnerText;
                    outbound.Owner.Id = Convert.ToInt32(xmlInput.ChildNodes[0].ChildNodes[1].InnerText);

                    outboundOrderViewDTO = iDispatchingMGR.ExtractDispatchByNumberAndOwner(context, outbound);

                    if (outboundOrderViewDTO.hasError())
                    {
                        if (outboundOrderViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = outboundOrderViewDTO.Errors.Title + " - " + outboundOrderViewDTO.Errors.Message;
                        statusMessage = outboundOrderViewDTO.Errors.ClassFullName + "::" + outboundOrderViewDTO.Errors.Method + " - " + outboundOrderViewDTO.Errors.Title + " - " + outboundOrderViewDTO.Errors.Message + " - " + outboundOrderViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        if (outboundOrderViewDTO.Entities.Count != 1)
                        {
                            ErrorDTO error;
                            statusProcess = "ER";

                            if (outboundOrderViewDTO.Entities.Count == 0)
                                error = baseControl.handleError(new ErrorDTO(WMSTekError.WebServices.Export.NoRecordsFound, context.LanguageCode));
                            else
                                error = baseControl.handleError(new ErrorDTO(WMSTekError.WebServices.Export.MultipleRecordsFound, context.LanguageCode));

                            clientResponse = error.Title + " - " + error.Message;
                            statusMessage = clientResponse;
                        }
                        else
                            statusProcess = "OK";
                    }
                    break;


                case "ExportMovementLog":
                    // Exporta Inbound con sus Recepciones
                    MovementWeb movementWeb = new MovementWeb();
                    movementWeb.Owner = new Binaria.WMSTek.Framework.Entities.Warehousing.Owner();
                    movementWeb.MovementType = new MovementType();
                    int qtyMovementToRet = 0;

                    if (xmlInput.ChildNodes[0].ChildNodes[1].InnerText == string.Empty)
                    {
                        movementWeb.Owner.Id = -1;
                    }
                    else
                    {
                        movementWeb.Owner.Id = Convert.ToInt32(xmlInput.ChildNodes[0].ChildNodes[1].InnerText);
                    }

                    if (xmlInput.ChildNodes[0].ChildNodes[0].InnerText == string.Empty)
                    {
                        movementWeb.MovementType.Id = -1;
                    }

                    else
                    {
                        movementWeb.MovementType.Id = Convert.ToInt32(xmlInput.ChildNodes[0].ChildNodes[0].InnerText);
                    }
                    movementWeb.Id =              Convert.ToInt32(xmlInput.ChildNodes[0].ChildNodes[2].InnerText);
                    qtyMovementToRet =            Convert.ToInt32(xmlInput.ChildNodes[0].ChildNodes[3].InnerText);

                    movementWebViewDTO = iWarehousingMGR.EstractMovementLog(context, movementWeb, qtyMovementToRet);

                    if (movementWebViewDTO.hasError())
                    {
                        if (movementWebViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = movementWebViewDTO.Errors.Title + " - " + movementWebViewDTO.Errors.Message;
                        statusMessage = movementWebViewDTO.Errors.ClassFullName + "::" + movementWebViewDTO.Errors.Method + " - " + movementWebViewDTO.Errors.Title + " - " + movementWebViewDTO.Errors.Message + " - " + movementWebViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        if (movementWebViewDTO.Entities.Count < 1)
                        {
                            ErrorDTO error = null;
                            statusProcess = "ER";

                            if (movementWebViewDTO.Entities.Count == 0)
                                error = baseControl.handleError(new ErrorDTO(WMSTekError.WebServices.Export.NoRecordsFound, context.LanguageCode));

                            clientResponse = error.Title + " - " + error.Message;
                            statusMessage = clientResponse;
                        }
                        else
                            statusProcess = "OK";
                    }
                    break;




                default:
                    genericError = true;
                    statusProcess = "ER";
                    statusMessage = "Invalid objectType";
                    break;
            }

        }

        private void CreateXML()
        {
            XmlDocument xmlOutput = new XmlDocument();
            

            try
            {
                switch (method)
                {
                    case "ExportReceipt":
                        InboundOrder inbound = inboundOrderViewDTO.Entities[0];
                        XmlNode receiptNode = xmlOutput.CreateNode(XmlNodeType.Element, "Receipt", null);

                        // Inbound Data
                        // ------------
                        // Id Inbound
                        XmlNode idInboundOrder = xmlOutput.CreateNode(XmlNodeType.Element, "IdInboundOrder", null);
                        idInboundOrder.InnerXml = inbound.Id.ToString();
                        receiptNode.AppendChild(idInboundOrder);

                        // Id Whs
                        XmlNode idWhs = xmlOutput.CreateNode(XmlNodeType.Element, "IdWhs", null);
                        idWhs.InnerXml = inbound.Warehouse.Id.ToString();
                        receiptNode.AppendChild(idWhs);

                        // Id Own
                        XmlNode idOwn = xmlOutput.CreateNode(XmlNodeType.Element, "IdOwn", null);
                        idOwn.InnerXml = inbound.Owner.Id.ToString();
                        receiptNode.AppendChild(idOwn);

                        // Id Own Code
                        XmlNode ownCode = xmlOutput.CreateNode(XmlNodeType.Element, "OwnCode", null);
                        ownCode.InnerXml = inbound.Owner.Code ?? string.Empty; 
                        receiptNode.AppendChild(ownCode);

                        // Inbound Number
                        XmlNode inboundNumber = xmlOutput.CreateNode(XmlNodeType.Element, "InboundNumber", null);
                        inboundNumber.InnerXml = inbound.Number ?? string.Empty;
                        receiptNode.AppendChild(inboundNumber);

                        // Status
                        XmlNode status = xmlOutput.CreateNode(XmlNodeType.Element, "Status", null);
                        status.InnerXml = inbound.Status.ToString();
                        receiptNode.AppendChild(status);

                        // Id TrackInboundType
                        XmlNode idTrackInboundType = xmlOutput.CreateNode(XmlNodeType.Element, "IdTrackInboundType", null);
                        idTrackInboundType.InnerXml = inbound.LatestInboundTrack.Type.Id.ToString();
                        receiptNode.AppendChild(idTrackInboundType);

                        // Id TrackInboundType
                        XmlNode nameTrackInboundType = xmlOutput.CreateNode(XmlNodeType.Element, "NameTrackInboundType", null);
                        nameTrackInboundType.InnerXml = inbound.LatestInboundTrack.Type.Name ?? string.Empty;
                        receiptNode.AppendChild(nameTrackInboundType);

                        // Date Track
                        XmlNode dateTrack = xmlOutput.CreateNode(XmlNodeType.Element, "DateTrack", null);
                        if (inbound.LatestInboundTrack.DateTrack > DateTime.MinValue)
                            dateTrack.InnerXml = inbound.LatestInboundTrack.DateTrack.ToUniversalTime().ToShortDateString();
                        else
                            dateTrack.InnerXml = string.Empty;
                        receiptNode.AppendChild(dateTrack);


                        // Receipt Data
                        // ------------
                        if (inbound.Receipts.Count > 0)
                        {
                            XmlNode receiptDetails = xmlOutput.CreateNode(XmlNodeType.Element, "ReceiptDetails", null);

                            foreach (Receipt receipt in inbound.Receipts)
                            {
                                XmlNode receiptDetail = xmlOutput.CreateNode(XmlNodeType.Element, "ReceiptDetail", null);

                                // Id Inbound
                                XmlNode idInboundOrderDetail = xmlOutput.CreateNode(XmlNodeType.Element, "IdInboundOrder", null);
                                idInboundOrderDetail.InnerXml = inbound.Id.ToString();
                                receiptDetail.AppendChild(idInboundOrderDetail);

                                // Receipt Date
                                XmlNode receiptDate = xmlOutput.CreateNode(XmlNodeType.Element, "ReceiptDate", null);
                                if (receipt.ReceiptDate > DateTime.MinValue)
                                    receiptDate.InnerXml = receipt.ReceiptDate.ToUniversalTime().ToShortDateString();
                                else
                                    receiptDate.InnerXml = string.Empty;
                                receiptDetail.AppendChild(receiptDate);

                                // Status
                                XmlNode statusDetail = xmlOutput.CreateNode(XmlNodeType.Element, "Status", null);
                                statusDetail.InnerXml = receipt.Status.ToString();
                                receiptDetail.AppendChild(statusDetail);

                                // Reference Doc
                                XmlNode referenceDoc = xmlOutput.CreateNode(XmlNodeType.Element, "ReferenceDoc", null);
                                referenceDoc.InnerXml = receipt.ReferenceDoc ?? string.Empty;
                                receiptDetail.AppendChild(referenceDoc);

                                // Id Reference Doc Type
                                XmlNode idReferenceDocType = xmlOutput.CreateNode(XmlNodeType.Element, "IdReferenceDocType", null);
                                idReferenceDocType.InnerXml = receipt.IdReferenceDocType.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(idReferenceDocType);

                                // Reference Doc Type Name
                                XmlNode referenceDocTypeName = xmlOutput.CreateNode(XmlNodeType.Element, "ReferenceDocTypeName", null);
                                referenceDocTypeName.InnerXml = receipt.ReferenceDocTypeName ?? string.Empty;
                                receiptDetail.AppendChild(referenceDocTypeName);

                                // Carrier
                                XmlNode carrier = xmlOutput.CreateNode(XmlNodeType.Element, "Carrier", null);
                                carrier.InnerXml = receipt.Carrier.Id.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(carrier);

                                // Carrier Code
                                XmlNode carrierCode = xmlOutput.CreateNode(XmlNodeType.Element, "CarrierCode", null);
                                carrierCode.InnerXml = receipt.Carrier.Code ?? string.Empty;
                                receiptDetail.AppendChild(carrierCode);

                                // Id Truck Code
                                XmlNode idTruckCode = xmlOutput.CreateNode(XmlNodeType.Element, "IdTruckCode", null);
                                idTruckCode.InnerXml = receipt.Truck.IdCode ?? string.Empty;
                                receiptDetail.AppendChild(idTruckCode);

                                // Driver Code
                                XmlNode driverCode = xmlOutput.CreateNode(XmlNodeType.Element, "DriverCode", null);
                                driverCode.InnerXml = receipt.Driver.Id.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(driverCode);

                                // Receipt details Data
                                // --------------------
                                // Id Receipt Detail
                                XmlNode idReceiptDetail = xmlOutput.CreateNode(XmlNodeType.Element, "IdReceiptDetail", null);
                                idReceiptDetail.InnerXml = receipt.ReceiptDetails[0].Id.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(idReceiptDetail);

                                // Id Item
                                XmlNode idItem = xmlOutput.CreateNode(XmlNodeType.Element, "IdItem", null);
                                idItem.InnerXml = receipt.ReceiptDetails[0].Item.Id.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(idItem);

                                // Item Code
                                XmlNode itemCode = xmlOutput.CreateNode(XmlNodeType.Element, "ItemCode", null);
                                itemCode.InnerXml = receipt.ReceiptDetails[0].Item.Code ?? string.Empty;
                                receiptDetail.AppendChild(itemCode);

                                // Id Category Item
                                XmlNode idCtgItem = xmlOutput.CreateNode(XmlNodeType.Element, "IdCtgItem", null);
                                idCtgItem.InnerXml = receipt.ReceiptDetails[0].CategoryItem.Id.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(idCtgItem);

                                // Category Name
                                XmlNode ctgName = xmlOutput.CreateNode(XmlNodeType.Element, "CtgName", null);
                                ctgName.InnerXml = receipt.ReceiptDetails[0].CategoryItem.Name ?? string.Empty;
                                receiptDetail.AppendChild(ctgName);

                                // Item Qty
                                XmlNode itemQty = xmlOutput.CreateNode(XmlNodeType.Element, "ItemQty", null);
                                itemQty.InnerXml = receipt.ReceiptDetails[0].Qty.ToString() ?? string.Empty;
                                receiptDetail.AppendChild(itemQty);

                                // Fifo Date
                                XmlNode fifoDate = xmlOutput.CreateNode(XmlNodeType.Element, "FifoDate", null);
                                if (receipt.ReceiptDetails[0].FifoDate > DateTime.MinValue)
                                    fifoDate.InnerXml = receipt.ReceiptDetails[0].FifoDate.ToUniversalTime().ToShortDateString();
                                else
                                    fifoDate.InnerXml = string.Empty;
                                receiptDetail.AppendChild(fifoDate);

                                // Expiration Date
                                XmlNode expirationDate = xmlOutput.CreateNode(XmlNodeType.Element, "ExpirationDate", null);
                                if (receipt.ReceiptDetails[0].ExpirationDate > DateTime.MinValue)
                                    expirationDate.InnerXml = receipt.ReceiptDetails[0].ExpirationDate.ToUniversalTime().ToShortDateString();
                                else
                                    expirationDate.InnerXml = string.Empty;
                                receiptDetail.AppendChild(expirationDate);

                                // Fabrication Date
                                XmlNode fabricationDate = xmlOutput.CreateNode(XmlNodeType.Element, "FabricationDate", null);
                                if (receipt.ReceiptDetails[0].FabricationDate > DateTime.MinValue)
                                    fabricationDate.InnerXml = receipt.ReceiptDetails[0].FabricationDate.ToUniversalTime().ToShortDateString();
                                else
                                    fabricationDate.InnerXml = string.Empty;
                                receiptDetail.AppendChild(fabricationDate);

                                // Lot Number
                                XmlNode lotNumber = xmlOutput.CreateNode(XmlNodeType.Element, "LotNumber", null);
                                lotNumber.InnerXml = receipt.ReceiptDetails[0].LotNumber ?? string.Empty;
                                receiptDetail.AppendChild(lotNumber);

                                // Id Lpn Code
                                XmlNode idLpnCode = xmlOutput.CreateNode(XmlNodeType.Element, "IdLpnCode", null);
                                idLpnCode.InnerXml = receipt.ReceiptDetails[0].LPN.IdCode ?? string.Empty;
                                receiptDetail.AppendChild(idLpnCode);

                                receiptDetails.AppendChild(receiptDetail);
                            }

                            receiptNode.AppendChild(receiptDetails);
                        }

                        xmlOutput.AppendChild(receiptNode);

                        clientResponse = xmlOutput.InnerXml;
                        statusProcess = "OK";
                        break;


                    case "ExportDispatch":

                        OutboundOrder outbound = outboundOrderViewDTO.Entities[0];
                        XmlNode dispatchNode = xmlOutput.CreateNode(XmlNodeType.Element, "Dispatch", null);

                        // Outbound Data
                        // ------------
                        // Id Outbound
                        XmlNode idOutboundOrder = xmlOutput.CreateNode(XmlNodeType.Element, "IdOutboundOrder", null);
                        idOutboundOrder.InnerXml = outbound.Id.ToString() ?? string.Empty;
                        dispatchNode.AppendChild(idOutboundOrder);

                        // Id Whs
                        XmlNode idWhs1 = xmlOutput.CreateNode(XmlNodeType.Element, "IdWhs", null);
                        idWhs1.InnerXml = outbound.Warehouse.Id.ToString() ?? string.Empty;
                        dispatchNode.AppendChild(idWhs1);

                        // Id Own
                        XmlNode idOwn1 = xmlOutput.CreateNode(XmlNodeType.Element, "IdOwn", null);
                        idOwn1.InnerXml = outbound.Owner.Id.ToString() ?? string.Empty;
                        dispatchNode.AppendChild(idOwn1);

                        // Id Own Code
                        XmlNode ownCode1 = xmlOutput.CreateNode(XmlNodeType.Element, "OwnCode", null);
                        ownCode1.InnerXml = outbound.Owner.Code ?? string.Empty;
                        dispatchNode.AppendChild(ownCode1);

                        // Outbound Number
                        XmlNode outboundNumber = xmlOutput.CreateNode(XmlNodeType.Element, "OutboundNumber", null);
                        outboundNumber.InnerXml = outbound.Number ?? string.Empty;
                        dispatchNode.AppendChild(outboundNumber);

                        // Status
                        XmlNode status1 = xmlOutput.CreateNode(XmlNodeType.Element, "Status", null);
                        status1.InnerXml = outbound.Status.ToString();
                        dispatchNode.AppendChild(status1);

                        // Id TrackInboundType
                        XmlNode idTrackOutboundType = xmlOutput.CreateNode(XmlNodeType.Element, "IdTrackOutboundType", null);
                        idTrackOutboundType.InnerXml = outbound.LatestOutboundTrack.Type.Id.ToString() ?? string.Empty;
                        dispatchNode.AppendChild(idTrackOutboundType);

                        // Name TrackInbound Type
                        XmlNode nameTrackOutboundType = xmlOutput.CreateNode(XmlNodeType.Element, "NameTrackOutboundType", null);
                        nameTrackOutboundType.InnerXml = outbound.LatestOutboundTrack.Type.Name ?? string.Empty;
                        dispatchNode.AppendChild(nameTrackOutboundType);

                        // Date Track
                        XmlNode dateTrack1 = xmlOutput.CreateNode(XmlNodeType.Element, "DateTrack", null);
                        if (outbound.DateTrack > DateTime.MinValue)
                            dateTrack1.InnerXml = outbound.DateTrack.ToUniversalTime().ToShortDateString();
                        else
                            dateTrack1.InnerXml = string.Empty;
                        dispatchNode.AppendChild(dateTrack1);


                        // Dispatch Data
                        // ------------
                        if (outbound.Dispatch.Count > 0)
                        {
                            XmlNode dispatchDetails = xmlOutput.CreateNode(XmlNodeType.Element, "DispatchDetails", null);

                            foreach (Dispatch dispatch in outbound.Dispatch)
                            {
                                XmlNode dispatchtDetail = xmlOutput.CreateNode(XmlNodeType.Element, "DispatchDetail", null);

                                // Id Outbound
                                XmlNode idOutboundOrderDetail = xmlOutput.CreateNode(XmlNodeType.Element, "IdOutboundOrder", null);
                                idOutboundOrderDetail.InnerXml = outbound.Id.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(idOutboundOrderDetail);

                                // Id Track Outbound 
                                XmlNode idTrackOutbound = xmlOutput.CreateNode(XmlNodeType.Element, "IdTrackOutbound", null);
                                idTrackOutbound.InnerXml = outbound.Dispatch[0].IdTrackOutbound.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(idTrackOutbound);

                                // Track Outbound Date
                                XmlNode trackOutboundDate = xmlOutput.CreateNode(XmlNodeType.Element, "TrackOutboundDate", null);
                                if (outbound.Dispatch[0].TrackOutboundDate > DateTime.MinValue)
                                    trackOutboundDate.InnerXml = outbound.Dispatch[0].TrackOutboundDate.ToUniversalTime().ToShortDateString();
                                else
                                    trackOutboundDate.InnerXml = string.Empty;
                                dispatchtDetail.AppendChild(trackOutboundDate);

                                // Status
                                XmlNode statusDetail = xmlOutput.CreateNode(XmlNodeType.Element, "Status", null);
                                statusDetail.InnerXml = dispatch.Status.ToString();
                                dispatchtDetail.AppendChild(statusDetail);

                                // Carrier
                                XmlNode idCarrier = xmlOutput.CreateNode(XmlNodeType.Element, "IdCarrier", null);
                                idCarrier.InnerXml = dispatch.Carrier.Id.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(idCarrier);

                                // Carrier Code
                                XmlNode carrierCode = xmlOutput.CreateNode(XmlNodeType.Element, "CarrierCode", null);
                                carrierCode.InnerXml = dispatch.Carrier.Code ?? string.Empty;
                                dispatchtDetail.AppendChild(carrierCode);

                                // Id Truck Code
                                XmlNode idTruckCode = xmlOutput.CreateNode(XmlNodeType.Element, "IdTruckCode", null);
                                idTruckCode.InnerXml = dispatch.Truck.IdCode ?? string.Empty;
                                dispatchtDetail.AppendChild(idTruckCode);

                                // Driver Code
                                XmlNode driverCode = xmlOutput.CreateNode(XmlNodeType.Element, "DriverCode", null);
                                driverCode.InnerXml = dispatch.Driver.Code ?? string.Empty;
                                dispatchtDetail.AppendChild(driverCode);

                                // Dispatch details Data
                                // --------------------
                                // Id Dispatch Detail
                                XmlNode idDispatchDetail = xmlOutput.CreateNode(XmlNodeType.Element, "IdDispatchDetail", null);
                                idDispatchDetail.InnerXml = dispatch.DispatchDetails[0].Id.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(idDispatchDetail);

                                // Id WmsProcessCode
                                XmlNode idWmsProcessCode = xmlOutput.CreateNode(XmlNodeType.Element, "IdWmsProcessCode", null);
                                idWmsProcessCode.InnerXml = dispatch.DispatchDetails[0].ProcessCode ?? string.Empty;
                                dispatchtDetail.AppendChild(idWmsProcessCode);

                                // Id Item
                                XmlNode idItem = xmlOutput.CreateNode(XmlNodeType.Element, "IdItem", null);
                                idItem.InnerXml = dispatch.DispatchDetails[0].Item.Id.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(idItem);

                                // Item Code
                                XmlNode itemCode = xmlOutput.CreateNode(XmlNodeType.Element, "ItemCode", null);
                                itemCode.InnerXml = dispatch.DispatchDetails[0].Item.Code ?? string.Empty;
                                dispatchtDetail.AppendChild(itemCode);

                                // Id Category Item
                                XmlNode idCtgItem = xmlOutput.CreateNode(XmlNodeType.Element, "IdCtgItem", null);
                                idCtgItem.InnerXml = dispatch.DispatchDetails[0].CategoryItem.Id.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(idCtgItem);

                                // Category Name
                                XmlNode ctgName = xmlOutput.CreateNode(XmlNodeType.Element, "CtgName", null);
                                ctgName.InnerXml = dispatch.DispatchDetails[0].CategoryItem.Name ?? string.Empty;
                                dispatchtDetail.AppendChild(ctgName);

                                // Item Qty
                                XmlNode itemQty = xmlOutput.CreateNode(XmlNodeType.Element, "ItemQty", null);
                                itemQty.InnerXml = dispatch.DispatchDetails[0].Qty.ToString() ?? string.Empty;
                                dispatchtDetail.AppendChild(itemQty);

                                // Fifo Date
                                XmlNode fifoDate = xmlOutput.CreateNode(XmlNodeType.Element, "FifoDate", null);
                                if (dispatch.DispatchDetails[0].Fifo > DateTime.MinValue)
                                    fifoDate.InnerXml = dispatch.DispatchDetails[0].Fifo.ToUniversalTime().ToShortDateString();
                                else
                                    fifoDate.InnerXml = string.Empty;
                                dispatchtDetail.AppendChild(fifoDate);

                                // Expiration Date
                                XmlNode expirationDate = xmlOutput.CreateNode(XmlNodeType.Element, "ExpirationDate", null);
                                if (dispatch.DispatchDetails[0].Expiration > DateTime.MinValue)
                                    expirationDate.InnerXml = dispatch.DispatchDetails[0].Expiration.ToUniversalTime().ToShortDateString();
                                else
                                    expirationDate.InnerXml = string.Empty;
                                dispatchtDetail.AppendChild(expirationDate);

                                // Fabrication Date
                                XmlNode fabricationDate = xmlOutput.CreateNode(XmlNodeType.Element, "FabricationDate", null);
                                if (dispatch.DispatchDetails[0].Fabrication > DateTime.MinValue)
                                    fabricationDate.InnerXml = dispatch.DispatchDetails[0].Fabrication.ToUniversalTime().ToShortDateString();
                                else
                                    fabricationDate.InnerXml = string.Empty;
                                dispatchtDetail.AppendChild(fabricationDate);

                                // Lot Number
                                XmlNode lotNumber = xmlOutput.CreateNode(XmlNodeType.Element, "LotNumber", null);
                                lotNumber.InnerXml = dispatch.DispatchDetails[0].LotNumber ?? string.Empty;
                                dispatchtDetail.AppendChild(lotNumber);

                                // Id Lpn Code
                                XmlNode idLpnCode = xmlOutput.CreateNode(XmlNodeType.Element, "IdLpnCode", null);
                                idLpnCode.InnerXml = dispatch.DispatchDetails[0].Lpn.IdCode ?? string.Empty;
                                dispatchtDetail.AppendChild(idLpnCode);

                                dispatchDetails.AppendChild(dispatchtDetail);
                            }

                            dispatchNode.AppendChild(dispatchDetails);
                        }

                        xmlOutput.AppendChild(dispatchNode);

                        clientResponse = xmlOutput.InnerXml;
                        statusProcess = "OK";
                        break;




                    case "ExportMovementLog":

                        if (movementWebViewDTO.Entities.Count > 0)
                        {
                            XmlNode movementsNode = xmlOutput.CreateNode(XmlNodeType.Element, "MovementsLog", null);

                            foreach (MovementWeb movementWeb in movementWebViewDTO.Entities)
                            {
                                XmlNode movementNode = xmlOutput.CreateNode(XmlNodeType.Element, "MovementLog", null);
                                // Movement Data
                                // ------------
                                // Id Movement
                                XmlNode idMovementWeb = xmlOutput.CreateNode(XmlNodeType.Element, "IdMovement", null);
                                idMovementWeb.InnerXml = movementWeb.Id.ToString() ?? string.Empty;
                                movementNode.AppendChild(idMovementWeb);

                                // Id Movement Type
                                XmlNode idMovementType = xmlOutput.CreateNode(XmlNodeType.Element, "IdMovementType", null);
                                idMovementType.InnerXml = movementWeb.MovementType.Id.ToString() ?? string.Empty;
                                movementNode.AppendChild(idMovementType);

                                // StartTime
                                XmlNode startTime = xmlOutput.CreateNode(XmlNodeType.Element, "StartTime", null);
                                startTime.InnerXml = movementWeb.StartTime.ToString();                               
                                movementNode.AppendChild(startTime);

                                //EndTime
                                XmlNode endTime = xmlOutput.CreateNode(XmlNodeType.Element, "EndTime", null);
                                endTime.InnerXml = movementWeb.StartTime.ToUniversalTime().ToShortDateString();
                                movementNode.AppendChild(endTime);

                                //UserName
                                XmlNode userName = xmlOutput.CreateNode(XmlNodeType.Element, "UserName", null);
                                userName.InnerXml = movementWeb.UserName ?? string.Empty;
                                movementNode.AppendChild(userName);

                                // Id Whs
                                XmlNode idWhs2 = xmlOutput.CreateNode(XmlNodeType.Element, "WhsCode", null);
                                idWhs2.InnerXml = movementWeb.Warehouse.Code.ToString() ?? string.Empty;
                                movementNode.AppendChild(idWhs2);

                                // Document Number
                                XmlNode documentNumber = xmlOutput.CreateNode(XmlNodeType.Element, "DocumentNumber", null);
                                documentNumber.InnerXml = movementWeb.DocumentNumber ?? string.Empty;
                                movementNode.AppendChild(documentNumber);

                                // Id Document Type 
                                XmlNode idDocumentType = xmlOutput.CreateNode(XmlNodeType.Element, "DocumentType", null);
                                idDocumentType.InnerXml = movementWeb.DocumentType ?? string.Empty;
                                movementNode.AppendChild(idDocumentType);

                                // DocumentLineNumber
                                XmlNode documentLineNumber = xmlOutput.CreateNode(XmlNodeType.Element, "DocumentLineNumber", null);
                                documentLineNumber.InnerXml = movementWeb.DocumentLineNumber ?? string.Empty;
                                movementNode.AppendChild(documentLineNumber);

                                //ReferenceNumber
                                XmlNode referenceNumber = xmlOutput.CreateNode(XmlNodeType.Element, "ReferenceNumber", null);
                                referenceNumber.InnerXml = movementWeb.ReferenceNumber ?? string.Empty;
                                movementNode.AppendChild(referenceNumber);

                                // Id Own
                                XmlNode idOwn2 = xmlOutput.CreateNode(XmlNodeType.Element, "OwnCode", null);
                                idOwn2.InnerXml = movementWeb.Owner.Code ?? string.Empty;
                                movementNode.AppendChild(idOwn2);

                                //IdItem
                                XmlNode idItem3 = xmlOutput.CreateNode(XmlNodeType.Element, "ItemCode", null);
                                idItem3.InnerXml = movementWeb.Item.Code ?? string.Empty;
                                movementNode.AppendChild(idItem3);


                                //Item Code
                                //XmlNode itemCode3 = xmlOutput.CreateNode(XmlNodeType.Element, "ItemCode", null);
                                //itemCode3.InnerXml = movementWeb.Item.Code ?? string.Empty;
                                //movementNode.AppendChild(itemCode3);

                                //UomCode
                                XmlNode uomCode = xmlOutput.CreateNode(XmlNodeType.Element, "UomCode", null);
                                uomCode.InnerXml = movementWeb.ItemUom.Code ?? string.Empty;
                                movementNode.AppendChild(uomCode);

                                //// Name TrackInbound Type
                                //XmlNode idCtgItem3 = xmlOutput.CreateNode(XmlNodeType.Element, "IdCtgItem", null);
                                //idCtgItem3.InnerXml = movementWeb.CategoryItem.Id.ToString() ?? string.Empty;
                                //movementNode.AppendChild(idCtgItem3);

                                // CtgName
                                XmlNode ctgName3 = xmlOutput.CreateNode(XmlNodeType.Element, "CtgCode", null);
                                ctgName3.InnerXml = movementWeb.CategoryItem.Code ?? string.Empty;
                                movementNode.AppendChild(ctgName3);

                                // Lot Number
                                XmlNode lotNumber3 = xmlOutput.CreateNode(XmlNodeType.Element, "LotNumber", null);
                                lotNumber3.InnerXml = movementWeb.LotNumber ?? string.Empty;
                                movementNode.AppendChild(lotNumber3);

                                // Fifo Date
                                XmlNode fifoDate2 = xmlOutput.CreateNode(XmlNodeType.Element, "FifoDate", null);
                                fifoDate2.InnerXml = movementWeb.FifoDate.ToUniversalTime().ToShortDateString();
                                movementNode.AppendChild(fifoDate2);

                                //Expiration Date
                                XmlNode expirationDate2 = xmlOutput.CreateNode(XmlNodeType.Element, "ExpirationDate", null);
                                expirationDate2.InnerXml = movementWeb.ExpirationDate.ToUniversalTime().ToShortDateString();
                                movementNode.AppendChild(expirationDate2);

                                // Fabrication Date
                                XmlNode fabricationDate2 = xmlOutput.CreateNode(XmlNodeType.Element, "FabricationDate", null);
                                fabricationDate2.InnerXml = movementWeb.FabricationDate.ToUniversalTime().ToShortDateString();
                                movementNode.AppendChild(fabricationDate2);

                                // IdLpnCodeSource 
                                XmlNode idLpnCodeSource = xmlOutput.CreateNode(XmlNodeType.Element, "IdLpnCodeSource", null);
                                idLpnCodeSource.InnerXml = movementWeb.IdLpnCodeSource ?? string.Empty;
                                movementNode.AppendChild(idLpnCodeSource);

                                // IdLpnCodeTarget
                                XmlNode idLpnCodeTarget = xmlOutput.CreateNode(XmlNodeType.Element, "IdLpnCodeTarget", null);
                                idLpnCodeTarget.InnerXml = movementWeb.IdLpnCodeTarget ?? string.Empty;
                                movementNode.AppendChild(idLpnCodeTarget);

                                //LpnParentSource
                                XmlNode lpnParentSource = xmlOutput.CreateNode(XmlNodeType.Element, "LpnParentSource", null);
                                lpnParentSource.InnerXml = movementWeb.LpnParentSource ?? string.Empty;
                                movementNode.AppendChild(lpnParentSource);

                                //LpnParentTarget
                                XmlNode lpnParentTarget = xmlOutput.CreateNode(XmlNodeType.Element, "LpnParentTarget", null);
                                lpnParentTarget.InnerXml = movementWeb.LpnParentTarget ?? string.Empty;
                                movementNode.AppendChild(lpnParentTarget);

                                //IdLocCodeProposal
                                XmlNode idLocCodeProposal = xmlOutput.CreateNode(XmlNodeType.Element, "IdLocCodeProposal", null);
                                idLocCodeProposal.InnerXml = movementWeb.IdLocCodeProposal ?? string.Empty;
                                movementNode.AppendChild(idLocCodeProposal);

                                //IdLocCodeSource
                                XmlNode idLocCodeSource = xmlOutput.CreateNode(XmlNodeType.Element, "IdLocCodeSource", null);
                                idLocCodeSource.InnerXml = movementWeb.IdLocCodeSource ?? string.Empty;
                                movementNode.AppendChild(idLocCodeSource);

                                //IdLocCodeTarget
                                XmlNode idLocCodeTarget = xmlOutput.CreateNode(XmlNodeType.Element, "IdLocCodeTarget", null);
                                idLocCodeTarget.InnerXml = movementWeb.IdLocCodeTarget ?? string.Empty;
                                movementNode.AppendChild(idLocCodeTarget);

                                //ItemQtyMov
                                XmlNode itemQtyMov = xmlOutput.CreateNode(XmlNodeType.Element, "ItemQtyMov", null);
                                itemQtyMov.InnerXml = movementWeb.ItemQtyMov.ToString() ?? string.Empty;
                                movementNode.AppendChild(itemQtyMov);

                                //QtyBeforeSource
                                XmlNode qtyBeforeSource = xmlOutput.CreateNode(XmlNodeType.Element, "QtyBeforeSource", null);
                                qtyBeforeSource.InnerXml = movementWeb.QtyBeforeSource.ToString() ?? string.Empty;
                                movementNode.AppendChild(qtyBeforeSource);

                                //QtyBeforeTarget
                                XmlNode qtyBeforeTarget = xmlOutput.CreateNode(XmlNodeType.Element, "QtyBeforeTarget", null);
                                qtyBeforeTarget.InnerXml = movementWeb.QtyBeforeTarget.ToString() ?? string.Empty;
                                movementNode.AppendChild(qtyBeforeTarget);

                                //ReasonCode
                                XmlNode reasonCode = xmlOutput.CreateNode(XmlNodeType.Element, "ReasonCode", null);
                                reasonCode.InnerXml = movementWeb.ReasonCode ?? string.Empty;
                                movementNode.AppendChild(reasonCode);

                                //HoldCode
                                XmlNode holdCode = xmlOutput.CreateNode(XmlNodeType.Element, "HoldCode", null);
                                holdCode.InnerXml = movementWeb.HoldCode ?? string.Empty;
                                movementNode.AppendChild(holdCode);

                                //RoutingCode
                                XmlNode routingCode = xmlOutput.CreateNode(XmlNodeType.Element, "RoutingCode", null);
                                routingCode.InnerXml = movementWeb.RoutingCode ?? string.Empty;
                                movementNode.AppendChild(routingCode);

                                //SpecialField1
                                XmlNode specialField1 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField1", null);
                                specialField1.InnerXml = movementWeb.SpecialField1 ?? string.Empty;
                                movementNode.AppendChild(specialField1);

                                //SpecialField2
                                XmlNode specialField2 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField2", null);
                                specialField2.InnerXml = movementWeb.SpecialField2 ?? string.Empty;
                                movementNode.AppendChild(specialField2);

                                //SpecialField3
                                XmlNode specialField3 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField3", null);
                                specialField3.InnerXml = movementWeb.SpecialField3 ?? string.Empty;
                                movementNode.AppendChild(specialField3);

                                //SpecialField4
                                XmlNode specialField4 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField4", null);
                                specialField4.InnerXml = movementWeb.SpecialField4 ?? string.Empty;
                                movementNode.AppendChild(specialField4);

                                //SpecialField5
                                XmlNode specialField5 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField5", null);
                                specialField5.InnerXml = movementWeb.SpecialField5 ?? string.Empty;
                                movementNode.AppendChild(specialField5);

                                //SpecialField6
                                XmlNode specialField6 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField6", null);
                                specialField6.InnerXml = movementWeb.SpecialField6 ?? string.Empty;
                                movementNode.AppendChild(specialField6);

                                //SpecialField7
                                XmlNode specialField7 = xmlOutput.CreateNode(XmlNodeType.Element, "SpecialField7", null);
                                specialField7.InnerXml = movementWeb.SpecialField7 ?? string.Empty;
                                movementNode.AppendChild(specialField7);



                                movementsNode.AppendChild(movementNode);
                            }
                            xmlOutput.AppendChild(movementsNode);

                            clientResponse = xmlOutput.InnerXml;
                            statusProcess = "OK";
                            break;
                        }
                        else
                        {
                            statusProcess = "OK";
                            break;
                        }


                    default:
                        genericError = true;
                        statusProcess = "ER";
                        statusMessage = "Invalid objectType";
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException) genericError = true;
                statusProcess = "ER";
                statusMessage = LogException(ex, " Create " + method);

                if (ex.InnerException != null)
                    clientResponse = ex.Message + ": " + ex.InnerException.Message;
                else
                    clientResponse = ex.Message;
            }
        }
        
        #endregion
    }
}
