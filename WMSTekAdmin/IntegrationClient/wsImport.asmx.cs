using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Services;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Integration;
using Binaria.WMSTek.Framework.Base;
using System.Xml;
using Binaria.WMSTek.Framework.Utils;
using System.Data;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Summary description for wsImport
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsImport : wsBase
    {
        private GenericViewDTO<InboundDetail> inboundDetailViewDTO = new GenericViewDTO<InboundDetail>();
        private GenericViewDTO<InboundOrder> inboundOrderViewDTO = new GenericViewDTO<InboundOrder>();
        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<OutboundDetail> outboundDetailViewDTO = new GenericViewDTO<OutboundDetail>();
        private GenericViewDTO<Item> itemViewDTO = new GenericViewDTO<Item>();
        private GenericViewDTO<GrpItem1> grpItem1ViewDTO = new GenericViewDTO<GrpItem1>();
        private GenericViewDTO<GrpItem2> grpItem2ViewDTO = new GenericViewDTO<GrpItem2>();
        private GenericViewDTO<GrpItem3> grpItem3ViewDTO = new GenericViewDTO<GrpItem3>();
        private GenericViewDTO<ItemUom> itemUomViewDTO = new GenericViewDTO<ItemUom>();
        private GenericViewDTO<Warehouse> whsViewDTO = new GenericViewDTO<Warehouse>();
        private GenericViewDTO<Owner> ownViewDTO = new GenericViewDTO<Owner >();
        private GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();
        private GenericViewDTO<CategoryItem> ctgItemViewDTO = new GenericViewDTO<CategoryItem>();
        private GenericViewDTO<OutboundType> outboundTypeDTO = new GenericViewDTO<OutboundType>();
        private GenericViewDTO<Carrier> carrierViewDTO = new GenericViewDTO<Carrier>();
        private GenericViewDTO<InboundType> inboundTypeDTO = new GenericViewDTO<InboundType>();
        private GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();
        private InboundOrder inboundOrder;
        private OutboundOrder outboundOrder;
        private Item item;
        private GrpItem1 grpItem1;
        private GrpItem2 grpItem2;
        private GrpItem3 grpItem3;
        private ItemUom itemUom;
        private Vendor vendor;
        private Customer customer;


        #region "InboundOrder"
        [WebMethod]
        public String ImportInbound(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportInbound";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaInboundImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();            
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportInbound" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
        #endregion

        #region "OutboundOrder"
        [WebMethod]
        public String ImportOutbound(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportOutbound";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaOutboundImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportOutbound" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region "ImportItem"
        [WebMethod]
        public String ImportItem(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportItem";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaItemImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportItem" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "ImportVendor"
        [WebMethod]
        public String ImportVendor(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportVendor";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaVendorImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportVendor" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "ImportCustomer"
        [WebMethod]
        public String ImportCustomer(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportCustomer";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaCustomerImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportCustomer" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "ImportItemUom"
        [WebMethod]
        public String ImportItemUom(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
       {
            clientResponse = string.Empty;
            method = "ImportItemUom";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaItemUomImport()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportItemUom" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        [WebMethod]
        public String GetXmlSchemaGrpItem1Import()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportGrpItem1" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [WebMethod]
        public String ImportGrpItem1(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportGrpItem1";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaGrpItem2Import()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportGrpItem2" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [WebMethod]
        public String ImportGrpItem2(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportGrpItem2";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }

        [WebMethod]
        public String GetXmlSchemaGrpItem3Import()
        {
            XmlDocument theXsdDoc = new XmlDocument();
            try
            {
                theXsdDoc.Load(MiscUtils.ReadSetting("XSDPath", "") + "ImportGrpItem3" + ".xsd");
                return theXsdDoc.InnerXml;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [WebMethod]
        public String ImportGrpItem3(String sourceUserName, String sourceAppName, String sourceHostName, String idTicketTransfer, Char typeMovto, int? recordRequestQty, int? recordProcessQty, String rangeRequest, String document)
        {
            clientResponse = string.Empty;
            method = "ImportGrpItem3";

            Import(sourceUserName, sourceAppName, sourceHostName, document, typeMovto);

            return clientResponse;
        }


        #region "Generic"
        private void Import(String sourceUserName, String sourceAppName, String sourceHostName, String document, Char typeMovto)
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
                            CreateObject();

                            if (statusProcess == "OK")
                            {
                                ValidateObject();

                                if (statusProcess == "OK")
                                {                                    
                                    ImportObject();
                                }
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
                statusMessage = LogException(ex, " Import");
            }
            finally
            {
                // Registra el movimiento realizado
                RegisterMovement(sourceUserName, sourceAppName, sourceHostName, typeMovto, "wsImport");
            }
        }

        private void ValidateObject()
        {
            try
            {
                switch (method)
                {
                    case "ImportInbound":
                        #region Inbound
                        if (inboundOrder.Warehouse.Code != null)
                        {
                            whsViewDTO = iLayoutMGR.GetWarehouseByCode(inboundOrder.Warehouse.Code, context);

                            if (whsViewDTO.Entities.Count > 0)
                            {
                                inboundOrder.Warehouse = whsViewDTO.Entities[0];                                
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "WAREHOUSE CODE NOT FOUND: " + inboundOrder.Warehouse.Code;
                                break;
                            }
                        }

                        if (inboundOrder.Owner.Code != null)
                        {
                            ownViewDTO = iWarehousingMGR.GetOwnerByCode(context, inboundOrder.Owner.Code);

                            if (ownViewDTO.Entities.Count > 0)
                            {
                                inboundOrder.Owner = ownViewDTO.Entities[0];                                
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "OWNER CODE NOT FOUND: " + inboundOrder.Owner.Code;
                                break;
                            }
                        }

                        if (inboundOrder.OutboundOrder.Number != null)
                        {
                            OutboundOrder theOutOrder =  new OutboundOrder();
                            theOutOrder.Number = inboundOrder.OutboundOrder.Number;
                            theOutOrder.Owner = inboundOrder.Owner;
                            outboundOrderViewDTO = iDispatchingMGR.GetOutboundByNumberAndOwner(context, theOutOrder);

                            if (outboundOrderViewDTO.Entities.Count > 0)
                            {
                                inboundOrder.OutboundOrder = outboundOrderViewDTO.Entities[0];
                            }
                        }

                        if (inboundOrder.InboundType.Code != null)
                        {
                            inboundTypeDTO = iReceptionMGR.GetInboundTypeByCode(context, inboundOrder.InboundType.Code);

                            if (inboundTypeDTO.Entities.Count > 0)
                            {
                                inboundOrder.InboundType = inboundTypeDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "INBOUNDTYPE CODE NOT FOUND: " + inboundOrder.InboundType.Code;
                                break;
                            }
                        }

                        if (inboundOrder.Vendor.Code != null)
                        {
                            vendorViewDTO = iWarehousingMGR.GetVendorByCodeAndOwner(context, inboundOrder.Vendor.Code, inboundOrder.Owner.Id);

                            if (vendorViewDTO.Entities.Count > 0)
                            {
                                inboundOrder.Vendor = vendorViewDTO.Entities[0];                                
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "VENDOR CODE NOT FOUND: " + inboundOrder.Vendor.Code;
                                break;
                            }
                        }

                        foreach (InboundDetail theDetail in inboundOrder.InboundDetails)
                        {
                            if (theDetail.Item.Code != null)
                            {
                                itemViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, theDetail.Item.Code, inboundOrder.Owner.Id,false);

                                if (itemViewDTO.Entities.Count > 0)
                                {
                                    theDetail.Item = itemViewDTO.Entities[0];                                    
                                }
                                else
                                {
                                    statusProcess = "NOK";
                                    statusMessage = "ITEM CODE NOT FOUND: " + theDetail.Item.Code;
                                    break;
                                }
                            }

                            if (theDetail.CategoryItem.Code != null)
                            {
                                ctgItemViewDTO = iWarehousingMGR.GetCategoryItemByCodeAndOwner(theDetail.CategoryItem.Code, inboundOrder.Owner.Id, context);

                                if (ctgItemViewDTO.Entities.Count > 0)
                                {
                                    theDetail.CategoryItem = ctgItemViewDTO.Entities[0];                                    
                                }
                                else
                                {
                                    statusProcess = "NOK";
                                    statusMessage = "CATEGORY ITEM CODE NOT FOUND: " + theDetail.CategoryItem.Code;
                                    break;
                                }
                            }
                        }
                        statusProcess = "OK";
                        break;
                        #endregion
                    case "ImportItem":
                        #region Item
                        if (item.Owner.Code != null)
                        {
                            ownViewDTO = iWarehousingMGR.GetOwnerByCode(context, item.Owner.Code);

                            if (ownViewDTO.Entities.Count > 0)
                            {
                                item.Owner = ownViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "OWNER CODE NOT FOUND: " + item.Owner.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "OWNER NOT FOUND IN XML DOCUMENT";
                            break;
                        }

                        if (item.GrpItem1.Code != null)
                        {
                            grpItem1ViewDTO = iWarehousingMGR.GetByCodeGrpItem1(context, item.GrpItem1.Code);

                            if (grpItem1ViewDTO.Entities.Count > 0)
                            {
                                item.GrpItem1 = grpItem1ViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "GROUPITEM1 CODE NOT FOUND: " + item.GrpItem1.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "GROUPITEM1 NOT FOUND IN XML DOCUMENT";
                            break;
                        }

                        if (item.GrpItem2.Code != null)
                        {
                            grpItem2ViewDTO = iWarehousingMGR.GetByCodeGrpItem2(context, item.GrpItem2.Code, item.GrpItem1.Id);

                            if (grpItem2ViewDTO.Entities.Count > 0)
                            {
                                item.GrpItem2 = grpItem2ViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "GROUPITEM2 CODE NOT FOUND: " + item.GrpItem2.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "GROUPITEM2 NOT FOUND IN XML DOCUMENT";
                            break;
                        }

                        if (item.GrpItem3.Code != null)
                        {
                            grpItem3ViewDTO = iWarehousingMGR.GetByCodeGrpItem3(context, item.GrpItem3.Code, item.GrpItem2.Id);

                            if (grpItem3ViewDTO.Entities.Count > 0)
                            {
                                item.GrpItem3 = grpItem3ViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "GROUPITEM3 CODE NOT FOUND: " + item.GrpItem3.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "GROUPITEM3 NOT FOUND IN XML DOCUMENT";
                            break;
                        }
                        statusProcess = "OK";
                        break;
                        #endregion
                    case "ImportOutbound":
                        #region Outbound
                        if (outboundOrder.Warehouse.Code != null)
                        {
                            whsViewDTO = iLayoutMGR.GetWarehouseByCode(outboundOrder.Warehouse.Code, context);

                            if (whsViewDTO.Entities.Count >0)
                            {
                                outboundOrder.Warehouse = whsViewDTO.Entities[0];
                            }
                            else
                            {                                  
                                statusProcess = "NOK";
                                statusMessage = "WAREHOUSE CODE NOT FOUND: " + outboundOrder.Warehouse.Code;
                                break;
                            }
                        }

                        if (outboundOrder.Owner.Code != null)
                        {
                            ownViewDTO = iWarehousingMGR.GetOwnerByCode(context, outboundOrder.Owner.Code);

                            if (ownViewDTO.Entities.Count >0)
                            {
                                outboundOrder.Owner = ownViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "OWNER CODE NOT FOUND: " + outboundOrder.Owner.Code;
                                break;
                            }
                        }

                        if (outboundOrder.OutboundType.Code != null)
                        {
                            outboundTypeDTO = iDispatchingMGR.GetByCodeOutboundType(context, outboundOrder.OutboundType.Code);

                            if (outboundTypeDTO.Entities.Count >0)
                            {
                                outboundOrder.OutboundType = outboundTypeDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "OUTBOUNDTYPE CODE NOT FOUND: " + outboundOrder.OutboundType.Code;
                                break;
                            }
                        }


                        if (outboundOrder.Carrier.Code != null)
                        {
                            carrierViewDTO = iWarehousingMGR.GetCarrierByCode(context, outboundOrder.Carrier.Code);

                            if (carrierViewDTO.Entities.Count > 0)
                            {
                                outboundOrder.Carrier = carrierViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "CARRIER CODE NOT FOUND: " + outboundOrder.OutboundType.Code;
                                break;
                            }
                        }

                        foreach (OutboundDetail theOutboundDetail in outboundOrder.OutboundDetails)
                        {
                            if (theOutboundDetail.Item.Code != null)
                            {
                                itemViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, theOutboundDetail.Item.Code, outboundOrder.Owner.Id, false);

                                if (itemViewDTO.Entities.Count >0)
                                {
                                    theOutboundDetail.Item = itemViewDTO.Entities[0];
                                }
                                else
                                {
                                    statusProcess = "NOK";
                                    statusMessage = "ITEM CODE NOT FOUND: " + theOutboundDetail.Item.Code;
                                    break;
                                }
                            }
                            if (theOutboundDetail.CategoryItem.Code != null)
                            {
                                ctgItemViewDTO = iWarehousingMGR.GetCategoryItemByCodeAndOwner(theOutboundDetail.CategoryItem.Code, outboundOrder.Owner.Id, context);

                                if (ctgItemViewDTO.Entities.Count >0)
                                {
                                    theOutboundDetail.CategoryItem = ctgItemViewDTO.Entities[0];
                                }
                                else
                                {
                                    statusProcess = "NOK";
                                    statusMessage = "CATEGORYITEM CODE NOT FOUND: " + theOutboundDetail.CategoryItem.Code;
                                    break;
                                }
                            }
                        }

                        statusProcess = "OK";
                        break;
                        #endregion
                    case "ImportGrpItem1":

                        if (grpItem1.Code != null)
                        {
                            grpItem1ViewDTO = iWarehousingMGR.GetByCodeGrpItem1(context, grpItem1.Code);

                            if (grpItem1ViewDTO.Entities.Count > 0)
                            {
                                grpItem1 = grpItem1ViewDTO.Entities[0];
                            }
                            else
                            {
                                clientResponse = "ERROR NOT FOUND GRPITEM1 CODE:" + grpItem1.Code;
                                statusMessage = "NOK";
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "CODE GROUPITEM1 NOT FOUND  XML DOCUMENT";
                            break;
                        }
                        statusProcess = "OK";
                        break;
                    case "ImportGrpItem2":

                        if (grpItem2.Code != null)
                        {
                            grpItem1ViewDTO = iWarehousingMGR.GetByCodeGrpItem1(context, grpItem2.GrpItem1.Code);

                            if (grpItem1ViewDTO.Entities.Count > 0)
                            {
                                grpItem2.GrpItem1 = grpItem1ViewDTO.Entities[0];

                                //grpItem2ViewDTO = iWarehousingMGR.GetByCodeGrpItem2(context, grpItem2.Code, grpItem2.GrpItem1.Id);

                                //if (grpItem2ViewDTO.Entities.Count > 0)
                                //{
                                //    grpItem2 = grpItem2ViewDTO.Entities[0];
                                //}
                                //else
                                //{
                                //    statusProcess = "NOK";
                                //    statusMessage = "CODE GROUPITEM2 NOT FOUND: " + grpItem2.Code;
                                //    break;
                                //}
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "CODE GROUPITEM1 NOT FOUND: " + grpItem2.GrpItem1.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "CODE GROUPITEM2 NOT FOUND  XML DOCUMENT";
                            break;
                        }
                        statusProcess = "OK";
                        break;
                    case "ImportGrpItem3":
                        #region GrpItem3
                        if (grpItem3.Code != null)
                        {
                            grpItem1ViewDTO = iWarehousingMGR.GetByCodeGrpItem1(context, grpItem3.GrpItem1.Code);
                            if (grpItem1ViewDTO.Entities.Count > 0)
                            {
                                grpItem3.GrpItem1 = grpItem1ViewDTO.Entities[0];
                                grpItem2ViewDTO = iWarehousingMGR.GetByCodeGrpItem2(context, grpItem3.GrpItem2.Code, grpItem3.GrpItem1.Id);

                                if (grpItem2ViewDTO.Entities.Count > 0)
                                {
                                    grpItem3.GrpItem2 = grpItem2ViewDTO.Entities[0];
                                    //grpItem3ViewDTO = iWarehousingMGR.GetByCodeGrpItem3(context, grpItem3.Code, grpItem3.GrpItem2.Id);

                                    //if (grpItem2ViewDTO.Entities.Count > 0)
                                    //{
                                    //    grpItem3 = grpItem3ViewDTO.Entities[0];
                                    //}
                                    //else
                                    //{
                                    //    statusProcess = "NOK";
                                    //    statusMessage = "CODE GROUPITEM3 NOT FOUND: " + grpItem3.Code;
                                    //    break;
                                    //}
                                }
                                else
                                {
                                    statusProcess = "NOK";
                                    statusMessage = "CODE GROUPITE21 NOT FOUND: " + grpItem3.GrpItem2.Code;
                                    break;
                                }
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "CODE GROUPITEM1 NOT FOUND: " + grpItem3.GrpItem1.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "CODE GROUPITEM3 NOT FOUND  XML DOCUMENT";
                            break;
                        }
                        statusProcess = "OK";
                        break;
                        #endregion

                    case "ImportVendor":

                        if (vendor.Owner.Code != null)
                        {
                            ownViewDTO = iWarehousingMGR.GetOwnerByCode(context, vendor.Owner.Code);

                            if (ownViewDTO.Entities.Count > 0)
                            {
                                vendor.Owner = ownViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "CODE OWNER NOT FOUND: " + vendor.Owner.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "CODE OWNER NOT FOUND  XML DOCUMENT";
                            break;
                        }
                        statusProcess = "OK";
                        break;
                    case "ImportCustomer":

                        if (customer.Owner.Code != null)
                        {
                            ownViewDTO = iWarehousingMGR.GetOwnerByCode(context, customer.Owner.Code);

                            if (ownViewDTO.Entities.Count > 0)
                            {
                                customer.Owner = ownViewDTO.Entities[0];
                            }
                            else
                            {
                                statusProcess = "NOK";
                                statusMessage = "CODE OWNER NOT FOUND: " + customer.Owner.Code;
                                break;
                            }
                        }
                        else
                        {
                            statusProcess = "NOK";
                            statusMessage = "CODE OWNER NOT FOUND  XML DOCUMENT";
                            break;
                        }
                        statusProcess = "OK";
                        break;
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

        private void CreateObject()
        {
            try
            {
                switch (method)
                {
                    case "ImportInbound":
                        inboundOrder = DeserializeObject<InboundOrder>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportItem":
                        item = DeserializeObject<Item>(xmlInput.DocumentElement.InnerXml);                            
                        statusProcess = "OK";
                        break;
                    case "ImportOutbound":
                        outboundOrder = DeserializeObject<OutboundOrder>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportItemUom":
                        itemUom = DeserializeObject<ItemUom>(xmlInput.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportGrpItem1":
                        grpItem1 = DeserializeObject<GrpItem1>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportGrpItem2":
                        grpItem2 = DeserializeObject<GrpItem2>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportGrpItem3":
                        grpItem3 = DeserializeObject<GrpItem3>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportVendor":
                        vendor = DeserializeObject<Vendor>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
                    case "ImportCustomer":
                        customer = DeserializeObject<Customer>(xmlInput.DocumentElement.InnerXml);
                        statusProcess = "OK";
                        break;
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

        private void ImportObject()
        {
            int lineNumber = 1;

            switch (method)
            {
                case "ImportInbound":

                    inboundOrderViewDTO = iReceptionMGR.GetInboundByNumberAndOwner(inboundOrder.Number, inboundOrder.Owner.Id, context);

                    if (inboundOrderViewDTO.Entities.Count == 0)
                    {
                        // Completa las propiedades de la Orden
                        inboundOrder.LatestInboundTrack = new InboundTrack();
                        inboundOrder.LatestInboundTrack.Type = new TrackInboundType();
                        inboundOrder.LatestInboundTrack.Type.Id = (int)TrackInboundTypeName.Anunciado;
                        inboundOrder.LatestInboundTrack.InboundOrder = new InboundOrder();
                        inboundOrder.LatestInboundTrack.DateTrack = DateTime.Now;

                        if (inboundOrder.EmissionDate == DateTime.MinValue) inboundOrder.EmissionDate = DateTime.Now;

                        // Completa las propiedades del detalle de la Orden
                        foreach (InboundDetail detail in inboundOrder.InboundDetails)
                        {
                            detail.InboundOrder = new InboundOrder();
                            detail.LineNumber = lineNumber;

                            if (detail.LineCode == null || detail.LineCode == string.Empty)
                                detail.LineCode = lineNumber.ToString();

                            lineNumber++;
                        }

                        // Importa la orden y sus detalles
                        inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrder(CRUD.Create, inboundOrder, context);
                    }
                    else
                    {
                        inboundOrder.Id = inboundOrderViewDTO.Entities[0].Id;
                        inboundOrderViewDTO = iReceptionMGR.MaintainInboundOrder(CRUD.Update, inboundOrder, context);
                    }


                    if (inboundOrderViewDTO.hasError())
                    {
                        if (inboundOrderViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = inboundOrderViewDTO.Errors.Title + " - " + inboundOrderViewDTO.Errors.Message;
                        statusMessage = inboundOrderViewDTO.Errors.ClassFullName + "::" + inboundOrderViewDTO.Errors.Method + " - " + inboundOrderViewDTO.Errors.Title + " - " + inboundOrderViewDTO.Errors.Message + " - " + inboundOrderViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }
                    break;

                case "ImportItem":
                    // Importa el Item
                    itemViewDTO = iWarehousingMGR.GetItemByCodeAndOwner(context, item.Code, item.Owner.Id, false);

                    if (itemViewDTO.Entities.Count > 0)
                    {
                        item.Id = itemViewDTO.Entities[0].Id;
                        itemViewDTO = iWarehousingMGR.MaintainItem(CRUD.Update, item, context);
                    }
                    else
                    {   
                        itemViewDTO = iWarehousingMGR.MaintainItem(CRUD.Create, item, context);
                    }                    

                    if (itemViewDTO.hasError())
                    {
                        if (itemViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = itemViewDTO.Errors.Title + " - " + itemViewDTO.Errors.Message;
                        statusMessage = itemViewDTO.Errors.ClassFullName + "::" + itemViewDTO.Errors.Method + " - " + itemViewDTO.Errors.Title + " - " + itemViewDTO.Errors.Message + " - " + itemViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }  
                    break;

                case "ImportOutbound":

                    outboundOrderViewDTO = iDispatchingMGR.GetOutboundByNumberAndOwner(context, outboundOrder);

                    if (outboundOrderViewDTO.Entities.Count == 0)
                    {
                        // Completa las propiedades de la Orden
                        outboundOrder.LatestOutboundTrack = new OutboundTrack();
                        outboundOrder.LatestOutboundTrack.DateTrack = DateTime.Now;
                        outboundOrder.LatestOutboundTrack.Type = new TrackOutboundType();
                        outboundOrder.LatestOutboundTrack.Type.Id = (int)TrackOutboundTypeName.Pending;
                        outboundOrder.LatestOutboundTrack.OutboundOrder = new OutboundOrder();

                        if (outboundOrder.EmissionDate == DateTime.MinValue) outboundOrder.EmissionDate = DateTime.Now;

                        // Completa las propiedades del detalle de la Orden
                        foreach (OutboundDetail detail in outboundOrder.OutboundDetails)
                        {
                            detail.OutboundOrder = new OutboundOrder();
                            detail.LineNumber = lineNumber;

                            if (detail.LineCode == null || detail.LineCode == string.Empty)
                                detail.LineCode = lineNumber.ToString();

                            lineNumber++;
                        }

                        //TODO:SOLUCION PARCHE 

                        outboundOrder.StateDelivery = new State(-1);
                        outboundOrder.StateFact = new State(-1);
                        outboundOrder.CountryDelivery = new Country(-1);
                        outboundOrder.CountryFact = new Country(-1);
                        outboundOrder.CityDelivery = new City(-1);
                        outboundOrder.CityFact = new City(-1);


                        // Importa la orden y sus detalles
                        outboundOrderViewDTO = iDispatchingMGR.MaintainOutboundOrder(CRUD.Create, outboundOrder, context);
                    }
                    else
                    {
                        outboundOrder.Id = outboundOrderViewDTO.Entities[0].Id;
                        outboundOrderViewDTO = iDispatchingMGR.MaintainOutboundOrder(CRUD.Update, outboundOrder, context);
                    }

                    if (outboundOrderViewDTO.hasError())
                    {
                        if (outboundOrderViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = outboundOrderViewDTO.Errors.Title + " - " + outboundOrderViewDTO.Errors.Message;
                        statusMessage = outboundOrderViewDTO.Errors.ClassFullName + "::" + outboundOrderViewDTO.Errors.Method + " - " + outboundOrderViewDTO.Errors.Title + " - " + outboundOrderViewDTO.Errors.Message + " - " + outboundOrderViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }
                    break;

                case "ImportGrpItem1":
                    // Importa el Item
                    grpItem1ViewDTO = iWarehousingMGR.GetByCodeGrpItem1(context, grpItem1.Code);

                    if (grpItem1ViewDTO.Entities.Count > 0)
                    {
                        grpItem1.Id = grpItem1ViewDTO.Entities[0].Id;
                        grpItem1ViewDTO = iWarehousingMGR.MaintainGrpItem1(CRUD.Update, grpItem1, context);
                    }
                    else
                    {
                        grpItem1ViewDTO = iWarehousingMGR.MaintainGrpItem1(CRUD.Create, grpItem1, context);
                    }

                    if (grpItem1ViewDTO.hasError())
                    {
                        if (grpItem1ViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = grpItem1ViewDTO.Errors.Title + " - " + grpItem1ViewDTO.Errors.Message;
                        statusMessage = grpItem1ViewDTO.Errors.ClassFullName + "::" + grpItem1ViewDTO.Errors.Method + " - " + grpItem1ViewDTO.Errors.Title + " - " + grpItem1ViewDTO.Errors.Message + " - " + grpItem1ViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }
                    break;

                case "ImportGrpItem2":

                    grpItem2ViewDTO = iWarehousingMGR.GetByCodeGrpItem2(context, grpItem2.Code, grpItem2.GrpItem1.Id);

                    if (grpItem2ViewDTO.Entities.Count > 0)
                    {
                        grpItem2.Id = grpItem2ViewDTO.Entities[0].Id;
                        grpItem2ViewDTO = iWarehousingMGR.MaintainGrpItem2(CRUD.Update, grpItem2, context);
                    }
                    else
                    {
                        grpItem2ViewDTO = iWarehousingMGR.MaintainGrpItem2(CRUD.Create, grpItem2, context);
                    }

                    if (grpItem2ViewDTO.hasError())
                    {
                        if (grpItem2ViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = grpItem2ViewDTO.Errors.Title + " - " + grpItem2ViewDTO.Errors.Message;
                        statusMessage = grpItem2ViewDTO.Errors.ClassFullName + "::" + grpItem2ViewDTO.Errors.Method + " - " + grpItem2ViewDTO.Errors.Title + " - " + grpItem2ViewDTO.Errors.Message + " - " + grpItem2ViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }
                    break;
                case "ImportGrpItem3":

                    grpItem3ViewDTO = iWarehousingMGR.GetByCodeGrpItem3(context, grpItem3.Code, grpItem3.GrpItem2.Id);

                    if (grpItem3ViewDTO.Entities.Count > 0)
                    {
                        grpItem3.Id = grpItem3ViewDTO.Entities[0].Id;
                        grpItem3ViewDTO = iWarehousingMGR.MaintainGrpItem3(CRUD.Update, grpItem3, context);
                    }
                    else
                    {
                        grpItem3ViewDTO = iWarehousingMGR.MaintainGrpItem3(CRUD.Create, grpItem3, context);
                    }

                    if (grpItem3ViewDTO.hasError())
                    {
                        if (grpItem3ViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = grpItem3ViewDTO.Errors.Title + " - " + grpItem3ViewDTO.Errors.Message;
                        statusMessage = grpItem3ViewDTO.Errors.ClassFullName + "::" + grpItem3ViewDTO.Errors.Method + " - " + grpItem3ViewDTO.Errors.Title + " - " + grpItem3ViewDTO.Errors.Message + " - " + grpItem3ViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }
                    break;
                case "ImportVendor":

                    vendorViewDTO = iWarehousingMGR.GetVendorByCodeAndOwner(context,vendor.Code,vendor.Owner.Id);

                    vendor.State = new State(-1);
                    vendor.Country = new Country(-1);
                    vendor.City = new City(-1);

                    if (vendorViewDTO.Entities.Count > 0)
                    {
                        vendor.Id = vendorViewDTO.Entities[0].Id;
                        vendorViewDTO = iWarehousingMGR.MaintainVendor(CRUD.Update, vendor, context);
                    }
                    else
                    {   
                        vendorViewDTO = iWarehousingMGR.MaintainVendor(CRUD.Create, vendor, context);
                    }

                    if (vendorViewDTO.hasError())
                    {
                        if (vendorViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = vendorViewDTO.Errors.Title + " - " + vendorViewDTO.Errors.Message;
                        statusMessage = vendorViewDTO.Errors.ClassFullName + "::" + vendorViewDTO.Errors.Method + " - " + vendorViewDTO.Errors.Title + " - " + vendorViewDTO.Errors.Message + " - " + vendorViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
                        statusProcess = "OK";
                    }

                    break;
                case "ImportCustomer":

                    customerViewDTO = iWarehousingMGR.GetCustomerByCodeAndOwn(context, customer.Code, customer.Owner.Id);

                    customer.StateDelv = new State(-1);
                    customer.StateFact = new State(-1);
                    customer.CountryFact = new Country(-1);
                    customer.CountryDelv = new Country(-1);
                    customer.CityFact = new City(-1);
                    customer.CityDelv = new City(-1);
                        

                    if (customerViewDTO.Entities.Count > 0)
                    {
                        customer.Id = customerViewDTO.Entities[0].Id;
                        customerViewDTO = iWarehousingMGR.MaintainCustomer(CRUD.Update, customer, context);
                    }
                    else
                    {
                        customerViewDTO = iWarehousingMGR.MaintainCustomer(CRUD.Create, customer, context);
                    }

                    if (vendorViewDTO.hasError())
                    {
                        if (customerViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                        statusProcess = "ER";
                        clientResponse = customerViewDTO.Errors.Title + " - " + customerViewDTO.Errors.Message;
                        statusMessage = customerViewDTO.Errors.ClassFullName + "::" + customerViewDTO.Errors.Method + " - " + customerViewDTO.Errors.Title + " - " + customerViewDTO.Errors.Message + " - " + customerViewDTO.Errors.OriginalMessage;
                    }
                    else
                    {
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

        #endregion
    }
}
