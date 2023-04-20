using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;
using System.IO;
using System.Xml;
using System.Data;
using System.Xml.Serialization;
using System.Text;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Reflection;
using System.Collections;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using System.Xml.Linq;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Clientes
{
    public partial class GenerateAsnFileSodimac : BasePage
    {
        #region "Declaración de Variables"


        private GenericViewDTO<DispatchSpecial> dispatchSpecialViewDTO = new GenericViewDTO<DispatchSpecial>();
        private GenericViewDTO<DispatchDetail> dispatchDetailViewDTO = new GenericViewDTO<DispatchDetail>();
        private GenericViewDTO<TaskConsult> taskViewDTO = new GenericViewDTO<TaskConsult>();
        private GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();
        private bool isValidViewDTO = false;


        // Propiedad para controlar el nro de pagina activa en la grilla
        public int currentPage
        {
            get
            {
                if (ValidateViewState("page"))
                    return (int)ViewState["page"];
                else
                    return 0;
            }

            set { ViewState["page"] = value; }
        }

        // Propiedad para controlar el indice activo en la grilla
        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return -1;
            }

            set { ViewState["index"] = value; }
        }

        private string customerCodeSodimac
        {
            get { return GetConst("CodeCustomerASNSodimac")[0]; }
        }

        #endregion

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "GenerateAsnFileSodimac";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .4);
            }
            else
            {
                if (ValidateSession(WMSTekSessions.GenerateAsnFileSodimac.DispatchList))
                {
                    dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileSodimac.DispatchList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de Tareas
            this.Master.ucMainFilter.listOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listOutboundType = GetConst("OutboundOrderReadyForDispatchOutboundType");
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = false;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.DispatchAdvanceDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.DispatchAdvanceDaysAfter;

            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblReferenceDoc.Text;
            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = this.lblReferenceNumbDoc.Text;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                divDetail.Visible = false;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        /// <summary>
        /// Configuracion inicial de la Grilla principal
        /// </summary>
        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {

            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.btnDownloadVisible = true;
            this.Master.ucTaskBar.BtnDownloadClick += new EventHandler(btnSave_Click);
        }


        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    // Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        // Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                        PopulateGrid();
                        PopulateGridDetail();
                    }
                }
                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);                
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void OpenPopUpASN(List<int> listIdDipatch)
        {

            try
            {
                bool flagError = false;
                var listSpecialDispatch = new List<DispatchSpecial>();

                foreach (var idDispatch in listIdDipatch)
                {
                    var dispatchSpecialDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileSodimac.DispatchList];

                    if (!dispatchSpecialDTO.hasError())
                    {
                        var selecDispactch = dispatchSpecialDTO.Entities.Where(x => x.Id == idDispatch).FirstOrDefault();

                        ContextViewDTO newContext = new ContextViewDTO();
                        newContext.MainFilter = new List<EntityFilter>();
                        var arrEnum = Enum.GetValues(typeof(EntityFilterName));
                        foreach (var item in arrEnum)
                        {
                            newContext.MainFilter.Add(new EntityFilter(item.ToString(), new List<FilterItem>()));
                        }

                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Clear();
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();

                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Add(new FilterItem(selecDispactch.OutboundOrder.Number));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(selecDispactch.IdWhs.ToString()));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(selecDispactch.OutboundOrder.Owner.Id.ToString()));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.TaskType)].FilterValues.Add(new FilterItem("PAKOR"));
                        newContext.MainFilter.Add(new EntityFilter("Completed", new FilterItem("0", "0")));

                        taskViewDTO = iTasksMGR.FindAllTaskMgr(newContext);

                        if (!taskViewDTO.hasError() && taskViewDTO.Entities != null)
                        {
                            if (taskViewDTO.Entities.Count > 0)
                            {
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorExistTaskOrder.Text, "");
                                flagError = true;
                                break;
                            }
                            else
                            {
                                listSpecialDispatch.Add(selecDispactch);
                            }
                        }
                        else
                        {
                            this.Master.ucError.ShowError(taskViewDTO.Errors);
                            flagError = true;
                            break;
                        }
                    }
                    else
                    {
                        this.Master.ucError.ShowError(dispatchSpecialDTO.Errors);
                        flagError = true;
                        break;
                    }
                }

                if (!flagError)
                {
                    Session.Add(WMSTekSessions.GenerateAsnFileSodimac.SelectedDispatchASN, listSpecialDispatch);
                    Session.Add(WMSTekSessions.GenerateAsnFileSodimac.ListIdDipatches, listIdDipatch);

                    this.txtDateAsn.Text = "";
                    this.txtHourAsn.Text = "";
                    this.txtWarehouseASN.Text = "";
                    divEditNew.Visible = true;
                    modalPopUpASN.Show();
                    upEditNew.Update();
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            divEditNew.Visible = false;
            modalPopUpASN.Hide();
            upEditNew.Update();
        }

        protected void btnGenerateASN_Click(object sender, EventArgs e)
        {
            try
            {
                List<DispatchSpecial> selecDispactch = new List<DispatchSpecial>();

                if (ValidateSession(WMSTekSessions.GenerateAsnFileSodimac.SelectedDispatchASN))
                {
                    var listIdDispatch = (List<int>)Session[WMSTekSessions.GenerateAsnFileSodimac.ListIdDipatches];

                    ClearFilter("listDispatchDetail");
                    CreateFilterByList("listDispatch", listIdDispatch);

                    dispatchDetailViewDTO = iDispatchingMGR.GetByIdDispatchSodimac("Sodimac", context);

                    if (!dispatchDetailViewDTO.hasError())
                    {
                        selecDispactch = (List<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileSodimac.SelectedDispatchASN];
                        isValidViewDTO = true;

                        if (dispatchDetailViewDTO.Entities != null && dispatchDetailViewDTO.Entities.Count > 0)
                        {
                            if (selecDispactch.FirstOrDefault().Customer.CustomerB2B == null || selecDispactch.FirstOrDefault().Customer.CustomerB2B.TemplateASNFile == null ||
                                selecDispactch.FirstOrDefault().Customer.CustomerB2B.TemplateASNFile == "")
                            {
                                ShowAlert(this.lblTitle.Text, this.lblErrorTemplate.Text);
                            }
                            else
                            {
                                //Se llena con los ReferenceDocNumber asociados al despacho
                                List<string> lstrefDocNumber = selecDispactch.Select(s => s.ReferenceDoc.ReferenceDocNumber).Distinct().ToList();

                                //Carga el archivo xsd del cliente
                                string file = selecDispactch.FirstOrDefault().Customer.CustomerB2B.TemplateASNFile;
                                XmlDocument xsd = new XmlDocument();
                                xsd.Load(file);

                                DataSet theData = new DataSet();
                                StringReader xmlSR = new StringReader(xsd.InnerXml);
                                theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                                XElement root = readXml();
                                IEnumerable<string> bundleCodes = null;
                                IEnumerable<string> palletCodes = null;

                                if (root.Element("BundleCode") != null && root.Element("PalletCode") != null)
                                {
                                    bundleCodes = from bundle in root.Element("BundleCode").Descendants("value")
                                                    select bundle.Value;

                                    palletCodes = from pallet in root.Element("PalletCode").Descendants("value")
                                                    select pallet.Value;
                                }
                                else
                                {
                                    throw new Exception(lblErrorXmlConstMissing.Text);
                                }

                                //Codigo para Mosaico
                                DataTable tableInbound = theData.Tables["Cabecera"];
                                DataRow rowInbound = tableInbound.NewRow();
                                rowInbound["NRO"] = 1;
                                rowInbound["NRO_CITA"] = this.txtNroCita.Text;
                                rowInbound["NRO_OC"] = selecDispactch.FirstOrDefault().OutboundOrder.ReferenceNumber;
                                rowInbound["FECHA_DESPACHO_PACTADA"] = formatDate(this.txtDateAsn.Text.Trim().ToString());
                                rowInbound["HORA_DESPACHO_PACTADA"] = this.txtHourAsn.Text.Trim() + ":00";
                                rowInbound["TOTAL_BULTOS"] = dispatchDetailViewDTO.Entities.Where(dispatchDetail => bundleCodes.Contains(dispatchDetail.Lpn.LPNType.Code)).Select(s => s.Lpn.IdCode).Distinct().Count().ToString();
                                rowInbound["TOTAL_TOTES"] = 0;
                                rowInbound["TOTAL_PALLETS"] = dispatchDetailViewDTO.Entities.Where(dispatchDetail => palletCodes.Contains(dispatchDetail.Lpn.LPNType.Code)).Select(s => s.Lpn.IdCode).Distinct().Count().ToString();
                                rowInbound["GUIA"] = (string.IsNullOrEmpty(selecDispactch.FirstOrDefault().ReferenceDoc.ReferenceDocNumber) ? 0 : Int32.Parse(selecDispactch.FirstOrDefault().ReferenceDoc.ReferenceDocNumber));
                                rowInbound["BODEGA"] = this.txtWarehouseASN.Text.Trim();
                                rowInbound["CABECERA_Id"] = 1;
                                tableInbound.Rows.Add(rowInbound);

                                DataTable tableDet = theData.Tables["DETALLE"];
                                foreach (var disp in dispatchDetailViewDTO.Entities)
                                {
                                    if (string.IsNullOrEmpty(disp.Item.ItemCustomer.BarCode))
                                    {
                                        throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Item.Code);
                                    }

                                    DataRow rowDet = tableDet.NewRow();
                                    rowDet["NRO"] = 2;
                                    rowDet["UPC_EAN"] = string.IsNullOrEmpty(disp.Item.ItemCustomer.BarCode) ? "" : disp.Item.ItemCustomer.BarCode; 
                                    rowDet["TIENDA_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                                    rowDet["DESC_TIENDA_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Name) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Name.Replace("Ñ", "N").Replace("ñ", "n");
                                    rowDet["UND_DESPACHAR"] = disp.Qty;

                                    string typeLpn = string.Empty;
                                    var isBundle = bundleCodes.Contains(disp.Lpn.LPNType.Code);
                                    if (isBundle)
                                    {
                                        typeLpn = "BU";
                                    }
                                    else
                                    {
                                        typeLpn = "PA";
                                    }

                                    rowDet["TIPO_EMBALAJE"] = typeLpn;
                                    rowDet["SERIE_ETIQUETA_EMBALAJE"] = (string.IsNullOrEmpty(disp.SealNumber) ? "0" : disp.SealNumber);
                                    rowDet["CABECERA_Id"] = 1;
                                    tableDet.Rows.Add(rowDet);
                                }

                                DataTable tableDet2 = theData.Tables["DETALLE2"];
                                int count = 0;
                                foreach (var docRef in lstrefDocNumber)
                                {
                                    DataRow rowDet = tableDet2.NewRow();
                                    rowDet["NRO"] = 3;
                                    rowDet["NRO_GUIA"] = docRef.Trim();
                                    rowDet["CABECERA_Id"] = 1;
                                    tableDet2.Rows.Add(rowDet);
                                    count++;

                                    if (count == 3)
                                        break;
                                }

                                StringBuilder str = new StringBuilder();
                                StringBuilder footer = new StringBuilder();

                                foreach (DataTable t in theData.Tables)
                                {
                                    int contRowTwo = 0;
                                    int contRowThree = 0;

                                    foreach (DataRow row in t.Rows)
                                    {
                                        string[] myStringArray = convertArrayString(row);
                                        //Elimina el Id el CABECERA_Id
                                        myStringArray = myStringArray.Where((source, index) => index != (myStringArray.Count() - 1)).ToArray();

                                        if (row[0].ToString() == "2")
                                        {
                                            contRowTwo++;
                                            myStringArray[myStringArray.ToList().Count() - 1] = "||" + myStringArray[myStringArray.ToList().Count() - 1];
                                        }

                                        if (row[0].ToString() == "3")
                                        {
                                            contRowThree++;

                                            if (contRowThree == t.Rows.Count)
                                            {
                                                footer.Append(string.Join("|", myStringArray));
                                            }
                                            else
                                            {
                                                footer.AppendLine(string.Join("|", myStringArray));
                                            }
                                        }
                                        else
                                        {
                                            if (row[0].ToString() == "2" && contRowTwo == t.Rows.Count)
                                            {
                                                str.Append(string.Join("|", myStringArray));
                                            }
                                            else
                                            {
                                                str.AppendLine(string.Join("|", myStringArray));
                                            }
                                        }
                                    }
                                }

                                string attachment = "attachment; filename=eDSP-" + DateTime.Now.ToString("ddMMyy-hhmm") + ".txt";
                                Response.Clear();
                                Response.ContentType = "text/plain";
                                Response.Charset = "windows-1252";
                                Response.ContentEncoding = Encoding.GetEncoding("Windows-1252");
                                Response.AddHeader("content-disposition", attachment);

                                using (StreamWriter writer = new StreamWriter(Response.OutputStream))
                                {
                                    writer.WriteLine(str);
                                    writer.Write(footer);
                                }

                                Response.End();

                                modalPopUpASN.Hide();
                                upEditNew.Update();
                            }
                        }
                    }
                    else
                    {
                        this.Master.ucError.ShowError(dispatchDetailViewDTO.Errors);
                    }   
                }
            }
            catch (System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                modalPopUpASN.Hide();
                upEditNew.Update();
                ShowAlert(this.lblTitle.Text, ex.Message);                
            }

        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var rowsSelected = GetAllRowsSelected();
                if (rowsSelected.Count > 0)
                {
                    var repeatedRowsByNumberRefDoc = rowsSelected.GroupBy(rnDoc => rnDoc.Value).ToList();
                    if (repeatedRowsByNumberRefDoc.Count() > 1)
                    {
                        ucStatus.ShowWarning(lblSelectedOutboundOrderWithDifferentReferenceDocNumber.Text);
                    }
                    else
                    {
                        ucStatus.ShowMessage(string.Empty);
                        OpenPopUpASN(rowsSelected.Select(rnDoc => rnDoc.Key).ToList());
                    }
                }
                else
                {
                    ucStatus.ShowMessage(lblValidateAtLeastOneOutboundOrderSelected.Text);
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }


        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                if (index != -1)
                {
                    LoadDetail(index);
                    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                }
                else
                {
                    detailTitle = null;
                }

                base.ExportToExcel(grdMgr, grdDetail, detailTitle);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }


        // Carga la grilla, filtrada por el criterio de busqueda ingresado
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtHourAsn.Text = "";
                this.txtDateAsn.Text = "";
                this.txtWarehouseASN.Text = "";
                TextBox txtNumReference = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");

                ReloadData();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // Carga lista de OutboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
            customerViewDTO = iWarehousingMGR.GetCustomerByCodeAndOwn(context, customerCodeSodimac, int.Parse(ddlOwn.SelectedValue));

            if (customerViewDTO != null && customerViewDTO.Entities.Count > 0)
            {
                dispatchSpecialViewDTO = iDispatchingMGR.GetDispatchSpecialHeaderSodimac(context, customerViewDTO.Entities.FirstOrDefault().Id);

                if (!dispatchSpecialViewDTO.hasError() && dispatchSpecialViewDTO.Entities != null)
                {


                    Session.Add(WMSTekSessions.GenerateAsnFileSodimac.DispatchList, dispatchSpecialViewDTO);
                    isValidViewDTO = true;

                    ucStatus.ShowMessage(dispatchSpecialViewDTO.MessageStatus.Message);

                }
                else
                {


                    isValidViewDTO = false;
                    this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);

                }
            }
            else
            {


                isValidViewDTO = false;
                dispatchSpecialViewDTO.Errors = baseControl.handleException(new Exception(lblErrorCustomerNotFound.Text), context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!dispatchSpecialViewDTO.hasConfigurationError() && dispatchSpecialViewDTO.Configuration != null && dispatchSpecialViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchSpecialViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = dispatchSpecialViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(dispatchSpecialViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.GenerateAsnFileSodimac.DispatchDetailList];

                // Configura ORDEN de las columnas de la grilla
                if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                // Detalle de Documentos de Entrada
                grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
            }
        }


        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "chkSelectOutboundOrder"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

                        // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "');");
                        e.Row.Cells[i].Attributes["onclick"] += ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);

                    }   
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    //capturo la posicion de la fila 
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndex = grdMgr.SelectedIndex;

                    LoadDetail(index);
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewDetailOnclick('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewDetailOnmouseover('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewDetailOnmouseout('" + e.Row.RowIndex + "', '" + grdDetail.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }


        /// <summary>
        /// Retorna el detalle de cada despacho
        /// </summary>
        /// <param name="index"></param>
        protected void LoadDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;
            txtHourAsn.Text = "";
            txtDateAsn.Text = "";
            this.txtWarehouseASN.Text = "";

            if (index != -1)
            {
                dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileSodimac.DispatchList];
                int id = dispatchSpecialViewDTO.Entities[index].Id;

                ClearFilter("listDispatchDetail");
                CreateFilterByList("listDispatch", new List<int>() { id });

                dispatchDetailViewDTO = iDispatchingMGR.GetByIdDispatchSodimac("Falabella",context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.GenerateAsnFileSodimac.DispatchDetailList, dispatchDetailViewDTO);

                this.lblNroDoc.Text = dispatchSpecialViewDTO.Entities[index].OutboundOrder.Number;
                this.lblNroDocRefDet.Text = dispatchSpecialViewDTO.Entities[index].ReferenceDoc.ReferenceDocNumber;
                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        public void ShowAlert(string title, string message)
        {
            Encoding encod = Encoding.ASCII;
            string script = "ShowMessage('" + title + "','" + message.Replace("'","") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            //string script = "setTimeout(ShowMessage('" + title + "','" + message.Replace("'","") + "'),2000);";
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }


       
        private string ConvertToCSV(DataSet objDataSet)
        {
            StringBuilder content = new StringBuilder();

            if (objDataSet.Tables.Count >= 1)
            {
                DataTable table = objDataSet.Tables[0];

                if (table.Rows.Count > 0)
                {
                    DataRow dr1 = (DataRow)table.Rows[0];
                    int intColumnCount = dr1.Table.Columns.Count;
                    int index = 1;

                    //add column names
                    foreach (DataColumn item in dr1.Table.Columns)
                    {
                        content.Append(String.Format("\"{0}\"", item.ColumnName));
                        if (index < intColumnCount)
                            content.Append(",");
                        else
                            content.Append("\r\n");
                        index++;
                    }

                    //add column data
                    foreach (DataRow currentRow in table.Rows)
                    {
                        string strRow = string.Empty;
                        for (int y = 0; y <= intColumnCount - 1; y++)
                        {
                            strRow += "\"" + currentRow[y].ToString() + "\"";

                            if (y < intColumnCount - 1 && y >= 0)
                                strRow += ",";
                        }
                        content.Append(strRow + "\r\n");
                    }
                }
            }

            return content.ToString();
        }

        private String[] convertArrayString(DataRow row)
        {
            string[] result = new string[row.ItemArray.Length];
            int cont = 0;
            foreach (object item in row.ItemArray)
            {
                result[cont] = item.ToString();
                cont++;
            }

            if (result.Count() > 0)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private string formatDate(string originalDate)
        {
            string day = originalDate.Substring(0, 2);
            string month = originalDate.Substring(3, 2);
            string year = originalDate.Substring(6, 4);

            return year + "-" + month + "-" + day;
        }

        private Dictionary<int, string> GetAllRowsSelected()
        {
            var listOutboundOrderSelected = new Dictionary<int, string>();

            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                var row = grdMgr.Rows[i];
                var chkSelectOutboundOrder = (CheckBox)row.FindControl("chkSelectOutboundOrder");
                var lblReferenceNumber = (Label)row.FindControl("lblReferenceNumber");
                var lblIdDispacth = (Label)row.FindControl("lblIdDispacth");

                if (chkSelectOutboundOrder.Checked)
                {
                    listOutboundOrderSelected.Add(int.Parse(lblIdDispacth.Text.Trim()), lblReferenceNumber.Text.Trim());
                }
            }

            return listOutboundOrderSelected;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('Falabella', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }
    }
}
