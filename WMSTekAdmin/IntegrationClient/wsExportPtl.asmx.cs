using Binaria.WMSTek.DataAccess.Integration;
using Binaria.WMSTek.DataAccess.Warehousing;
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
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.IntegrationClient
{
    /// <summary>
    /// Descripción breve de wmExportPtl
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]

    public class wsExportPtl : wsBase
    {
        /// <summary>
        /// wsExportTaskDetailPTL
        /// </summary>
        /// <param name="nroOla">Es el número que identifica la Ola y que fue generado por el liberador de Olas de Wmstek</param>
        /// <param name="idWhs">Es el id de la bodega en la cual se procesan las Olas</param>
        /// <param name="ptlTypeCode">Es el código de tipo de PTL que está extrayendo los datos (Pallet, caja, e-Commerce, Carro) 
        /// (’PTLPAL’,’PTLCAJ’,‘PTLECO’,’PTLCAR’)</param>
        /// <param name="taskTypeCode">Es el código de tipo de tarea que fue generado por el Wmstek</param>
        /// <param name="IdLpn">Es el id del LPN que pertenece a la Ola</param>
        /// <returns></returns>
        [WebMethod]
        public TaskDetailPtlIfzFunctional wsExportTaskDetailPTL(int nroOla, int? idWhs, string ptlTypeCode, string taskTypeCode, string IdLpn)
        {
            TaskDetailPtlIfzFunctional taskDetailPtlReturn = new TaskDetailPtlIfzFunctional();
            TaskDetailPtlIfzDAO taskDetailPtlIfzDAO = new TaskDetailPtlIfzDAO();
            ItemUomDAO itemUomDAO = new ItemUomDAO();
            //const string STATE_EXPORTED = "T";
            //const string CURRENT_USER = "WS";

            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    TaskDetailPtlIfz tdParam = new TaskDetailPtlIfz();
                    tdParam.NroOla = nroOla;                    
                    tdParam.PtlTypeCode = ptlTypeCode;

                    if (idWhs != null && idWhs > 0)
                        tdParam.IdWhs = (int)idWhs;

                    if(!string.IsNullOrEmpty(taskTypeCode))
                        tdParam.TaskTypeCode = taskTypeCode;

                    if (!string.IsNullOrEmpty(IdLpn))
                        tdParam.IdLpnTargetUsed = IdLpn;

                    GenericViewDTO<TaskDetailPtlIfz> taskDetailPtlIfzViewDTO = taskDetailPtlIfzDAO.GetByAnyParameter(tdParam);

                    if (taskDetailPtlIfzViewDTO.hasError())
                    {
                        statusMessage = "OLA NO EXISTE WMS";
                        throw new Exception(statusMessage);
                    }
                    else
                    {
                        //Thread.Sleep(500);

                        if (taskDetailPtlIfzViewDTO.Entities != null && taskDetailPtlIfzViewDTO.Entities.Count > 0)
                        {     
                            int nroTicket = GetNroTicket();

                            //Valida que Exista un ticket valido 
                            if (nroTicket > 0)
                            {
                                taskDetailPtlReturn.NroTicket = nroTicket;
                                List<TaskDetailPtlIfz> lstTaskDetailPtlIfz = new List<TaskDetailPtlIfz>();

                                foreach (TaskDetailPtlIfz td in taskDetailPtlIfzViewDTO.Entities)
                                {
                                    td.ListItemUomPtlIfz = new List<ItemUomPtlIfz>();

                                    ItemUom itemUomParam = new ItemUom();
                                    itemUomParam.Item = new Item(td.IdItem);

                                    var itemUomViewDTO = itemUomDAO.GetByAnyParameter(itemUomParam);

                                    //GenericViewDTO<Item> GetItemByCode(ContextViewDTO context, string itemCode, bool filter);

                                    //Se agrega lista de presentaciones para los items
                                    foreach (ItemUom iu in itemUomViewDTO.Entities)
                                    {
                                        ItemUomPtlIfz itemUomPtl = new ItemUomPtlIfz();
                                        itemUomPtl.Id = iu.Id;
                                        itemUomPtl.IdItem = iu.Item.Id;
                                        itemUomPtl.UomCode = iu.Code;
                                        itemUomPtl.ConversionFactor = iu.ConversionFactor;
                                        itemUomPtl.BarCode = iu.BarCode;
                                        itemUomPtl.IdUomType = iu.UomType.Id;
                                        itemUomPtl.Length = iu.Length;
                                        itemUomPtl.Width = iu.Width;
                                        itemUomPtl.Height = iu.Height;
                                        itemUomPtl.Volume = iu.Volume;
                                        itemUomPtl.Weight = iu.Weight;
                                        itemUomPtl.Status = iu.Status;
                                        itemUomPtl.UomQty = iu.UomQty;
                                        itemUomPtl.UnitQty = iu.UnitQty;
                                        itemUomPtl.MaxWeightUpon = iu.MaxWeightUpon;
                                        itemUomPtl.PutawayZone = iu.PutawayZone;
                                        itemUomPtl.PickArea = iu.PickArea;
                                        itemUomPtl.SpecialField1 = iu.SpecialField1;
                                        itemUomPtl.SpecialField2 = iu.SpecialField2;
                                        itemUomPtl.SpecialField3 = iu.SpecialField3;
                                        itemUomPtl.SpecialField4 = iu.SpecialField4;
                                        itemUomPtl.UomName = iu.Name;
                                        itemUomPtl.ItemCode = iu.Item.Code;
                                        itemUomPtl.BigTicket = iu.BigTicket;

                                        td.ListItemUomPtlIfz.Add(itemUomPtl);
                                    }
                                            

                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(TaskDetailPtlIfz).Name.Trim();
                                    ws.ListXml = SerializeObject(td, true);
                                    ws.IdObjeto = td.IdTaskDetailPtl;
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
                                        lstTaskDetailPtlIfz.Add(td);
                                    }
                                }

                                taskDetailPtlReturn.ListTaskDetailPtlIfz = lstTaskDetailPtlIfz;
                            }
                        }
                        else
                        {
                            statusMessage = "ERROR NO EXISTEN DATOS";
                            throw new Exception(statusMessage);
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

            return taskDetailPtlReturn;
        }


        /// <summary>
        /// wsExportOutboundOrderAndDetailPTL
        /// </summary>
        /// <param name="nroOla">Es el número que identifica la Ola y que fue generado por el liberador de Olas de Wmstek</param>
        /// <param name="idWhs">Es el id de la bodega en la cual se procesan las Olas</param>
        /// <param name="ptlTypeCode">Es el código de tipo de PTL que está extrayendo los datos (Pallet, caja, e-Commerce, Carro) 
        /// (’PTLPAL’,’PTLCAJ’,‘PTLECO’,’PTLCAR’)</param>
        [WebMethod]
        public OutboundOrderPtlIfzFunctional wsExportOutboundOrderAndDetailPTL(int nroOla, int? idWhs, string ptlTypeCode)
        {
            OutboundOrderPtlIfzFunctional outboundOrderPtlReturn = new OutboundOrderPtlIfzFunctional();
            OutboundOrderPtlIfzDAO outboundOrderPtlIfzDAO = new OutboundOrderPtlIfzDAO();
            OutboundDetailPtlIfzDAO outboundDetailPtlIfzDAO = new OutboundDetailPtlIfzDAO();
            ItemDAO itemDAO = new ItemDAO();
            //const string STATE_EXPORTED = "T";
            //const string CURRENT_USER = "WS";

            try
            {

                Initialize();

                if (initMsg == "OK")
                {
                    if (idWhs == null)
                        idWhs = -1;

                    GenericViewDTO<OutboundOrderPtlIfz> outboundOrderPtlIfzViewDTO = outboundOrderPtlIfzDAO.GetByFilters(nroOla, (int)idWhs, ptlTypeCode, null);

                    if (outboundOrderPtlIfzViewDTO.hasError())
                    {
                        statusMessage = "OLA NO EXISTE WMS";
                        throw new Exception(statusMessage);
                    }
                    else
                    {
                        //Thread.Sleep(500);

                        if (outboundOrderPtlIfzViewDTO.Entities != null && outboundOrderPtlIfzViewDTO.Entities.Count > 0)
                        {
                            int nroTicket = GetNroTicket();

                            //Valida que Exista un ticket valido 
                            if (nroTicket > 0)
                            {
                                outboundOrderPtlReturn.NroTicket = nroTicket;
                                List<OutboundOrderPtlIfz> lstOutboundOrderPtlIfz = new List<OutboundOrderPtlIfz>();

                                foreach (OutboundOrderPtlIfz ord in outboundOrderPtlIfzViewDTO.Entities)
                                {
                                    GenericViewDTO<OutboundDetailPtlIfz> ordDetPtlIfzViewDTO = outboundDetailPtlIfzDAO.GetTaskDetailPtlIfzByNroOlaAndIdOrder(ord.NroOla, ord.IdOutboundOrder);
                                    ord.OutboundOrderPtlsIfz = ordDetPtlIfzViewDTO.Entities;
                                    
                                    foreach (OutboundDetailPtlIfz detalle in ord.OutboundOrderPtlsIfz)
                                    {
                                        var itemViewDTO = itemDAO.GetById(detalle.IdItem);
                                        try
                                        {
                                            detalle.ItemCode = itemViewDTO.Entities[0].Code;
                                        }
                                        catch (Exception)
                                        {
                                            statusMessage = "ItemId: " + detalle.IdItem.ToString() + " NO encontrado.";
                                            throw new Exception(statusMessage);
                                        }                                        
                                    }

                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(OutboundOrderPtlIfz).Name.Trim();
                                    ws.ListXml = SerializeObject(ord,true);
                                    ws.IdObjeto = ord.IdOutboundOrderPtl;
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
                                        lstOutboundOrderPtlIfz.Add(ord);
                                    }
                                }

                                outboundOrderPtlReturn.ListOutboundOrderPtlfz = lstOutboundOrderPtlIfz;
                            }
                        }
                        else
                        {
                            statusMessage = "ERROR NO EXISTEN DATOS";
                            throw new Exception(statusMessage);
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

            return outboundOrderPtlReturn;
        }

        [WebMethod]
        public WaveDispatchIfzFunctional wsExportWaveDispatchPTL(int nroOla)
        {
            WaveDispatchIfzFunctional waveDispatchIfzReturn = new WaveDispatchIfzFunctional();
            WaveDispatchIfzDAO waveDipatchIfzDAO = new WaveDispatchIfzDAO();
            //const string STATE_EXPORTED = "T";
            //const string CURRENT_USER = "WS";

            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    GenericViewDTO<WaveDispatchIfz> waveDispatchIfzViewDTO = waveDipatchIfzDAO.GetByNroOla(nroOla);

                    if (waveDispatchIfzViewDTO.hasError())
                    {
                        statusMessage = "OLA NO EXISTE WMS";
                        throw new Exception(statusMessage);
                    }
                    else
                    {
                        //Thread.Sleep(500);

                        if (waveDispatchIfzViewDTO.Entities != null && waveDispatchIfzViewDTO.Entities.Count > 0)
                        {
                            int nroTicket = GetNroTicket();

                            //Valida que Exista un ticket valido 
                            if (nroTicket > 0)
                            {
                                waveDispatchIfzReturn.NroTicket = nroTicket;
                                List<WaveDispatchIfz> lstWaveDispatchIfz = new List<WaveDispatchIfz>();

                                foreach (WaveDispatchIfz td in waveDispatchIfzViewDTO.Entities)
                                {
                                    WServiceMessageIfz ws = new WServiceMessageIfz();
                                    ws.Ticket = nroTicket;
                                    ws.Objeto = typeof(WaveDispatchIfz).Name.Trim();
                                    ws.ListXml = SerializeObject(td, true);
                                    ws.IdObjeto = td.IdWaveDispatch;
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
                                        lstWaveDispatchIfz.Add(td);
                                    }
                                }

                                waveDispatchIfzReturn.ListWaveDispatchIfz = lstWaveDispatchIfz;
                            }
                        }
                        else
                        {
                            statusMessage = "ERROR NO EXISTEN DATOS";
                            throw new Exception(statusMessage);
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

            return waveDispatchIfzReturn;
        }

        [WebMethod]
        public CfgUserWmsIfzFunctional ExportUsers(int QtyUsersToRet)
        {
            CfgUserWmsIfzFunctional usersReturn = new CfgUserWmsIfzFunctional();
            CfgUserWmsIfzDAO cfgUserWmsIfzDAO = new CfgUserWmsIfzDAO();

            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    GenericViewDTO<CfgUserWmsIfz> cfgUserWmsIfzViewDTO = cfgUserWmsIfzDAO.GetUnProcessedWithPagination(QtyUsersToRet);

                    if (cfgUserWmsIfzViewDTO.hasError())
                    {
                        if (cfgUserWmsIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = cfgUserWmsIfzViewDTO.Errors.ClassFullName + "::" + cfgUserWmsIfzViewDTO.Errors.Method + " - " + cfgUserWmsIfzViewDTO.Errors.Title + " - " + cfgUserWmsIfzViewDTO.Errors.Message + " - " + cfgUserWmsIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {
                        if (cfgUserWmsIfzViewDTO.Entities != null && cfgUserWmsIfzViewDTO.Entities.Count > 0)
                        {
                            Thread.Sleep(500);

                            int nroTicket = GetNroTicket();

                            if (nroTicket > 0)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(CfgUserWmsIfz).Name.Trim();
                                ws.ListXml = SerializeObject(cfgUserWmsIfzViewDTO.Entities.ToList(), true);
                                ws.Transferido = false;
                                ws.Procesado = false;
                                ws.DateCreated = DateTime.Now;

                                wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                if (wServiceMessageIfzViewDTO.hasError())
                                {
                                    if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                    statusProcess = "ERROR";
                                    clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                    statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    throw new Exception(statusMessage);
                                }
                                else
                                {
                                    usersReturn.NroTicket = nroTicket;
                                    usersReturn.ListCfgUserWmsIfz = cfgUserWmsIfzViewDTO.Entities.ToList();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return usersReturn;
        }

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
                                case ("CfgUserWmsIfz"):
                                    CfgUserWmsIfzDAO cfgUserWmsIfzDAO = new CfgUserWmsIfzDAO();
                                    GenericViewDTO<CfgUserWmsIfz> cfgUserWmsIfzDTO = new GenericViewDTO<CfgUserWmsIfz>();
                                    cfgUserWmsIfzDTO = cfgUserWmsIfzDAO.UpdateConfirm(nroTicket, "T");
                                    break;

                                case ("TaskDetailPtlIfz"):
                                    TaskDetailPtlIfzDAO taskDetailPtlIfzDAO = new TaskDetailPtlIfzDAO();
                                    GenericViewDTO<TaskDetailPtlIfz> taskDetailPtlIfzDTO = new GenericViewDTO<TaskDetailPtlIfz>();
                                    taskDetailPtlIfzDTO = taskDetailPtlIfzDAO.UpdateConfirm(nroTicket, "T");
                                    break;

                                case ("WaveDispatchIfz"):
                                    WaveDispatchIfzDAO waveDispatchIfzDAO = new WaveDispatchIfzDAO();
                                    GenericViewDTO<WaveDispatchIfz> waveDispatchIfzDTO = new GenericViewDTO<WaveDispatchIfz>();
                                    waveDispatchIfzDTO = waveDispatchIfzDAO.UpdateConfirm(nroTicket, "T");
                                    break;

                                case ("OutboundOrderPtlIfz"):
                                    OutboundOrderPtlIfzDAO outboundOrderPtlIfzDAO = new OutboundOrderPtlIfzDAO();
                                    GenericViewDTO<OutboundOrderPtlIfz> outboundOrderPtlIfzDTO = new GenericViewDTO<OutboundOrderPtlIfz>();
                                    outboundOrderPtlIfzDTO = outboundOrderPtlIfzDAO.UpdateConfirm(nroTicket, "T");
                                    break;

                            }

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
                statusMessage = LogException(ex, " ExportPtl");
            }
            finally
            {
                // Registra el movimiento realizado
                SetClientMessage(" TICKET: NO REGISTRADO", "wsExportPtl");
            }

            return clientResponse;
        }

        public TaskDetailIfzFunctional ExportTaskDetail(int QtyTaskDetailToRet)
        {    // EL Valor de QtyTaskDetailToRet se esta usando para indicar el numero de la OLA que se debe exportar al PTL
            TaskDetailIfzFunctional taskDetailReturn = new TaskDetailIfzFunctional();
            TaskDetailIfzDAO taskDetailIfzDAO = new TaskDetailIfzDAO();
            const string STATE_EXPORTED = "T";
            const string CURRENT_USER = "WS";

            try
            {
                Initialize();

                if (initMsg == "OK")
                {
                    GenericViewDTO<TaskDetailIfz> taskDetailIfzViewDTO = taskDetailIfzDAO.GetUnProcessedByLastIdTask(QtyTaskDetailToRet);

                    if (taskDetailIfzViewDTO.hasError())
                    {
                        if (taskDetailIfzViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                        {
                            statusMessage = taskDetailIfzViewDTO.Errors.ClassFullName + "::" + taskDetailIfzViewDTO.Errors.Method + " - " + taskDetailIfzViewDTO.Errors.Title + " - " + taskDetailIfzViewDTO.Errors.Message + " - " + taskDetailIfzViewDTO.Errors.OriginalMessage;
                            throw new Exception(statusMessage);
                        }
                    }
                    else
                    {
                        if (taskDetailIfzViewDTO.Entities != null && taskDetailIfzViewDTO.Entities.Count > 0)
                        {
                            Thread.Sleep(500);

                            int nroTicket = GetNroTicket();

                            if (nroTicket > 0)
                            {
                                WServiceMessageIfz ws = new WServiceMessageIfz();
                                ws.Ticket = nroTicket;
                                ws.Objeto = typeof(TaskDetailIfz).Name.Trim();
                                ws.ListXml = SerializeObject(taskDetailIfzViewDTO.Entities.ToList(), true);
                                ws.Transferido = false;
                                ws.Procesado = false;
                                ws.DateCreated = DateTime.Now;

                                wServiceMessageIfzViewDTO = theWServiceMessageIfzDAO.Insert(ws);

                                if (wServiceMessageIfzViewDTO.hasError())
                                {
                                    if (wServiceMessageIfzViewDTO.Errors.Code == WMSTekError.Generic.NullReference) genericError = true;
                                    statusProcess = "ERROR";
                                    clientResponse = wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message;
                                    statusMessage = wServiceMessageIfzViewDTO.Errors.ClassFullName + "::" + wServiceMessageIfzViewDTO.Errors.Method + " - " + wServiceMessageIfzViewDTO.Errors.Title + " - " + wServiceMessageIfzViewDTO.Errors.Message + " - " + wServiceMessageIfzViewDTO.Errors.OriginalMessage;
                                    throw new Exception(statusMessage);
                                }
                                else
                                {
                                    var taskDetailParam = new TaskDetailIfz()
                                    {
                                        IdTask = taskDetailIfzViewDTO.Entities.First().IdTask,
                                        StateInterface = STATE_EXPORTED,
                                        DateModified = DateTime.Now,
                                        UserModified = CURRENT_USER
                                    };

                                    var updateStateTaskDetailViewDTO = taskDetailIfzDAO.UpdateTaskDetailStateAsExportedByIdTask(taskDetailParam);

                                    if (updateStateTaskDetailViewDTO.hasError())
                                    {
                                        statusProcess = "ERROR";
                                        clientResponse = updateStateTaskDetailViewDTO.Errors.Title + " - " + updateStateTaskDetailViewDTO.Errors.Message;
                                        statusMessage = updateStateTaskDetailViewDTO.Errors.ClassFullName + "::" + updateStateTaskDetailViewDTO.Errors.Method + " - " + updateStateTaskDetailViewDTO.Errors.Title + " - " + updateStateTaskDetailViewDTO.Errors.Message + " - " + updateStateTaskDetailViewDTO.Errors.OriginalMessage;
                                        throw new Exception(statusMessage);
                                    }
                                    else
                                    {
                                        taskDetailReturn.NroTicket = nroTicket;
                                        taskDetailReturn.ListTaskDetailIfz = taskDetailIfzViewDTO.Entities.ToList();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                genericError = true;
                statusProcess = "ERROR";
                statusMessage = LogException(ex, " Export");
                throw new Exception(statusMessage);
            }

            return taskDetailReturn;
        }
    }
}
