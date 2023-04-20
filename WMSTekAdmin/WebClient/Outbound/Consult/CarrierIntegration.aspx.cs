using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class CarrierIntegration : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<DispatchSpecial> dispatchSpecialViewDTO = new GenericViewDTO<DispatchSpecial>();
        private GenericViewDTO<DispatchDetail> dispatchDetailViewDTO = new GenericViewDTO<DispatchDetail>();
        private bool isValidViewDTO = true;

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

        public int currentIndexToLoadDetail
        {
            get
            {
                if (ValidateViewState("currentIndexToLoadDetail"))
                    return (int)ViewState["currentIndexToLoadDetail"];
                else
                    return -1;
            }

            set { ViewState["currentIndexToLoadDetail"] = value; }
        }
        #endregion

        #region "Eventos"

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        LoadControls();
                    }
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                scriptManager.RegisterPostBackControl(this.Master.ucTaskBar.btnSaveTaskBar);
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    if (isValidViewDTO)
                    {
                        PopulateGrid();
                        PopulateGridDetail();
                    }
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
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

                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");

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
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                    currentIndexToLoadDetail = index;
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

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var listDispatchesSelected = GetAllRowsSelected();

                if (listDispatchesSelected.Count > 0)
                {
                    if (listDispatchesSelected.First().OutboundOrder.Carrier.CfgCarrier.IntegrationType == (int)eIntegrationType.File)
                    {
                        CreateFile(listDispatchesSelected.First().OutboundOrder.Carrier.Name.Trim().ToUpper(), listDispatchesSelected);
                    }
                    else if (listDispatchesSelected.First().OutboundOrder.Carrier.CfgCarrier.IntegrationType == (int)eIntegrationType.WebServices)
                    {
                        var newTaskQueue = new TaskQueue() {
                            IdTypeTask = (int)eTypeTaskQueue.CarrierIntegration,
                            TrackTaskQueue = new TrackTaskQueue() { IdTrackTaskQueue = (int)eTrackTaskQueue.Waiting },
                            DateCreated = DateTime.Now,
                            UserModified = context.SessionInfo.User.UserName
                        };

                        var utilityMgrDTO = utilityMGR.MaintainTaskQueue(CRUD.Create, newTaskQueue, context);

                        if (utilityMgrDTO.hasError())
                            throw new Exception(utilityMgrDTO.Errors.Message);
                        else
                            ucStatus.ShowMessage(utilityMgrDTO.MessageStatus.Message);
                    }
                }
                else
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblNoDispatchSelected.Text });
                }
            }
            catch (Exception ex)
            {
                //dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);

                ShowAlert(this.lblTitle.Text, ex.Message);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var dispatchesSelected = GetAllRowsSelected();

                if (dispatchesSelected.Count > 0)
                {
                    int qtyCopies = 1;

                    if (txtQtycopies.Text.Trim() != string.Empty)
                    {
                        int finalQty;
                        bool isValid = int.TryParse(txtQtycopies.Text.Trim(), out finalQty);

                        if (isValid)
                        {
                            qtyCopies = finalQty;
                        }
                    }

                    var printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue.ToString()));
                    var printLabelViewDTO = iDispatchingMGR.PrintLabelCarrier(dispatchesSelected, printer, qtyCopies, context);

                    if (!printLabelViewDTO.hasError())
                        ucStatus.ShowMessage(printLabelViewDTO.MessageStatus.Message);
                    else
                        this.Master.ucError.ShowError(printLabelViewDTO.Errors);

                    LoadControls();
                }
                else
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblNoDispatchSelected.Text });
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
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

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "CarrierIntegration";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeGridDetail();

            if (!Page.IsPostBack)
            {
                hsMasterDetail.TopPanel.HeightDefault = Convert.ToInt16(Convert.ToInt16(Session["screenY"]) * .35);
            }
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.btnSaveVisible = true;
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnSave_Click);
            this.Master.ucTaskBar.btnSaveToolTip = lblProcessButtonTooltip.Text;

            Master.ucTaskBar.BtnPrintClick += new EventHandler(btnPrint_Click);
            Master.ucTaskBar.btnPrintVisible = true;
            Master.ucTaskBar.btnPrintEnabled = false;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.DocumentNumberLabel = lblDocName.Text;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblCarrier.Text;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblFilterRefDoc.Text;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void InitializeGridDetail()
        {
            try
            {
                //grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByDetailGridPage.ToString()));
                grdDetail.EmptyDataText = this.Master.EmptyGridText;
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            TextBox txtCarrier = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterName");

            //if (string.IsNullOrEmpty(txtCarrier.Text))
            //{
            //    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblValidateCarrier.Text });
            //    return;
            //}

            dispatchSpecialViewDTO = iDispatchingMGR.GetDispatchSpecialHeaderForCarrierIntegration(context);

            if (!dispatchSpecialViewDTO.hasError() && dispatchSpecialViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CarrierIntegration.OrdersList, dispatchSpecialViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(dispatchSpecialViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.CarrierIntegration.OrdersList];

                int idDispatch = dispatchSpecialViewDTO.Entities[index].Id;

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatch", new List<int>() { idDispatch });

                dispatchDetailViewDTO = iDispatchingMGR.GetDispatchSpecialDetailForCarrierIntegration(context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    lblNroDoc.Text = dispatchSpecialViewDTO.Entities[index].OutboundOrder.Number;

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.CarrierIntegration.OrderSelected, dispatchSpecialViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.CarrierIntegration.OrderDetail, dispatchDetailViewDTO);
                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        protected void ReloadData()
        {
            UpdateSession();

            if (isValidViewDTO)
            {
                divDetail.Visible = false;
                currentIndex = -1;
                currentPage = 0;

                if (dispatchSpecialViewDTO.Entities.Count > 0)
                {
                    divPrintLabel.Visible = true;
                    this.txtQtycopies.Text = "1";
                    Master.ucTaskBar.btnPrintEnabled = true;
                }
                else
                {
                    divPrintLabel.Visible = false;
                    Master.ucTaskBar.btnPrintEnabled = false;
                }
            }
        }

        private void PopulateGrid()
        {
            dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.CarrierIntegration.OrdersList];

            if (dispatchSpecialViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            if (!dispatchSpecialViewDTO.hasConfigurationError() && dispatchSpecialViewDTO.Configuration != null && dispatchSpecialViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchSpecialViewDTO.Configuration);

            grdMgr.DataSource = dispatchSpecialViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(dispatchSpecialViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
            upGrid.Update();
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.CarrierIntegration.OrderDetail];

                if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }

        protected void LoadControls()
        {
            this.lblNotPrinter.Visible = false;
            this.txtQtycopies.Text = "1";

            string nroCopys = GetCfgParameter(CfgParameterName.MaxPrintedCopy.ToString());
            this.rvQtycopies.MaximumValue = nroCopys;
            this.rvQtycopies.ErrorMessage = (this.lblRangeQtyCopy.Text + "1 y " + nroCopys + ".");

            // Carga lista de impresoras asociadas al usuario
            base.LoadUserPrinters(this.ddlPrinters);

            // Selecciona impresora por defecto
            base.SelectDefaultPrinter(this.ddlPrinters);

            if (ddlPrinters.Items.Count == 0)
            {
                lblNotPrinter.Visible = true;
                txtQtycopies.Enabled = false;
                ddlPrinters.Enabled = false;
                Master.ucTaskBar.btnPrintEnabled = false;
            }
        }

        private List<DispatchSpecial> GetAllRowsSelected()
        {
            var listDispatchSelected = new List<DispatchSpecial>();
            dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.CarrierIntegration.OrdersList];

            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                var row = grdMgr.Rows[i];
                var chkSelectDispatchDetail = (CheckBox)row.FindControl("chkSelectOutboundOrder");
                var lblIdDispatch = (Label)row.FindControl("lblIdDispatch");

                if (chkSelectDispatchDetail.Checked)
                {
                    listDispatchSelected.Add(dispatchSpecialViewDTO.Entities.Where(d => d.Id == int.Parse(lblIdDispatch.Text.Trim())).First());
                }
            }

            return listDispatchSelected;
        }

        private void CreateFile(string carrierName, List<DispatchSpecial> listDispatchesSelected)
        {
            try
            {
                switch (carrierName)
                {
                    case CfgCarrier.TNT:
                        CreateFileTnt(listDispatchesSelected);
                        break;
                    default: throw new Exception(lblCarrierNotFound.Text);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateFileTnt(List<DispatchSpecial> listDispatchesSelected)
        {
            try
            {
                string carrierCode = listDispatchesSelected.First().OutboundOrder.Carrier.CfgCarrier.Code;

                string file = getPathFile(listDispatchesSelected.First().OutboundOrder.Carrier.CfgCarrier.FileTemplate);
                
                XmlDocument xsd = new XmlDocument();
                xsd.Load(file);

                DataSet theData = new DataSet();
                StringReader xmlSR = new StringReader(xsd.InnerXml);
                theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                DataTable tableHeader = theData.Tables["HEADER"];
                DataTable tableDetail = theData.Tables["DETAIL"];
                DataTable tableFooter = theData.Tables["FOOTER"];

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatch", listDispatchesSelected.Select(d => d.Id).ToList());

                dispatchDetailViewDTO = iDispatchingMGR.GetDispatchSpecialDetailForCarrierIntegration(context);

                if (dispatchDetailViewDTO.hasError() || dispatchDetailViewDTO.Entities.Count == 0)
                {
                    throw new Exception(dispatchDetailViewDTO.Errors.Message);
                }

                var cfgCarrierViewDTO = iDispatchingMGR.GetCfgCarrierByCarrierCode(carrierCode, context);

                if (cfgCarrierViewDTO.hasError() || cfgCarrierViewDTO.Entities.Count == 0)
                {
                    throw new Exception(cfgCarrierViewDTO.Errors.Message);
                }

                decimal correlativeOT = cfgCarrierViewDTO.Entities.First().CorrelativeOT;
                correlativeOT++;

                foreach (DispatchSpecial dispatch in listDispatchesSelected)
                {
                    DataRow rowHeader = tableHeader.NewRow();

                    //Documento
                    rowHeader["TIPO_REG"] = 1;
                    rowHeader["NUMERO_ENVIO"] = FormatStringTnt(correlativeOT.ToString(), 30); 
                    rowHeader["FECHA"] = DateTime.Now.ToString("DDMMYYYY"); 
                    rowHeader["REFERENCIA"] = FormatStringTnt(dispatch.OutboundOrder.ReferenceNumber, 15);  //duda

                    //Cliente TODO: es customer o warehouse?
                    rowHeader["CODREMI"] = FormatStringTnt(dispatch.OutboundOrder.Customer.Code, 9);
                    rowHeader["NOMRTE"] = FormatStringTnt(dispatch.OutboundOrder.Customer.Name, 40); 
                    rowHeader["DOMRTE"] = FormatStringTnt(dispatch.OutboundOrder.Customer.Address1Delv, 40); 
                    rowHeader["POBRTE"] = FormatStringTnt(string.Empty, 40); //falta poblacion
                    rowHeader["CP_ORI"] = FormatStringTnt(string.Empty, 7); //falta codigo postal
                    rowHeader["NIFREMI"] = FormatStringTnt(dispatch.OutboundOrder.Customer.Code, 20); 
                    rowHeader["TELREMI"] = FormatStringTnt(dispatch.OutboundOrder.Customer.PhoneDelv, 20);

                    //Destinatario
                    rowHeader["CODCONS"] = FormatStringTnt(string.Empty, 9); //falta codigo
                    rowHeader["NOMCONS"] = FormatStringTnt(string.Empty, 40); //falta nombre
                    rowHeader["DOMCONS"] = FormatStringTnt(dispatch.OutboundOrder.DeliveryAddress1, 40);
                    rowHeader["POBCONS"] = FormatStringTnt(dispatch.OutboundOrder.DeliveryAddress2, 40);
                    rowHeader["CP_REX"] = FormatStringTnt(string.Empty, 7); //falta codigo postal
                    rowHeader["NIFCONS"] = FormatStringTnt(string.Empty, 20); //falta rut
                    rowHeader["TELCONS"] = FormatStringTnt(dispatch.OutboundOrder.DeliveryPhone, 20);
                    rowHeader["CONTACTO"] = FormatStringTnt(dispatch.OutboundOrder.DeliveryEmail, 40);

                    var listDispatchDetail = dispatchDetailViewDTO.Entities.Where(dd => dd.Dispatch.Id == dispatch.Id).ToList();

                    var consolidatedStock = ConsolidatedStock(listDispatchDetail);

                    rowHeader["BULTOS"] = FormatStringTnt(listDispatchDetail.Select(dd => dd.Lpn.IdCode).Distinct().Count().ToString(), 6);
                    rowHeader["KILOS"] = FormatStringTnt(String.Format("{0:0.00}", consolidatedStock.TotalWeight), 6);
                    rowHeader["PVKILOS"] = FormatStringTnt(String.Format("{0:0}", consolidatedStock.TotalWeight), 6);
                    rowHeader["VOL"] = FormatStringTnt(String.Format("{0:0.00}", consolidatedStock.TotalVolumen), 6);
                    rowHeader["ML"] = FormatStringTnt("", 6);
                    rowHeader["SEMPOR"] = "P"; 
                    rowHeader["SEMPOR_R"] = "P"; 
                    rowHeader["SEMPOR_CR"] = "P"; 

                    //Opcionales
                    rowHeader["REEMBOLSO"] = FormatStringTnt("", 12);
                    rowHeader["VALSEMER"] = FormatStringTnt("", 12);
                    rowHeader["PORTEPAG"] = FormatStringTnt("", 12);
                    rowHeader["REEXPAG"] = FormatStringTnt("", 12);
                    rowHeader["REC_DESE"] = FormatStringTnt("", 12);
                    rowHeader["SEGURPAG"] = FormatStringTnt("", 12);
                    rowHeader["COMREE_P"] = FormatStringTnt("", 12);
                    rowHeader["IMPSEMERP"] = FormatStringTnt("", 12);
                    rowHeader["IVAPAG"] = FormatStringTnt("", 12);
                    rowHeader["TOTALP"] = FormatStringTnt("", 12);
                    rowHeader["PORTEDEB"] = FormatStringTnt("", 12);
                    rowHeader["REEXDEB"] = FormatStringTnt("", 12);
                    rowHeader["DESEMBOL"] = FormatStringTnt("", 12);
                    rowHeader["SEGURDEB"] = FormatStringTnt("", 12);
                    rowHeader["COMREE_D"] = FormatStringTnt("", 12);
                    rowHeader["IMPSEMERD"] = FormatStringTnt("", 12);
                    rowHeader["IVADEB"] = FormatStringTnt("", 12);
                    rowHeader["TOTALD"] = FormatStringTnt("", 12);
                    rowHeader["OBSER1"] = FormatStringTnt("", 100);
                    rowHeader["OBSER2"] = FormatStringTnt("", 100);
                    rowHeader["SERVICIO"] = FormatStringTnt("", 2);
                    rowHeader["OBHOJ1"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ2"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ3"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ4"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ5"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ6"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ7"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ8"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ9"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ10"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ11"] = FormatStringTnt("", 1);
                    rowHeader["OBHOJ12"] = FormatStringTnt("", 1);
                    rowHeader["FECHA_EA"] = FormatStringTnt("", 15);
                    rowHeader["ETIQUETADO"] = FormatStringTnt("", 1);
                    rowHeader["LOINEX"] = FormatStringTnt("", 300);
                    rowHeader["LIBRE1"] = FormatStringTnt("", 12);
                    rowHeader["LIBRE2"] = FormatStringTnt("", 12);
                    rowHeader["FINAL"] = FormatStringTnt("", 8);

                    tableHeader.Rows.Add(rowHeader);

                    foreach (var dispatchDetail in listDispatchDetail)
                    {
                        DataRow rowDetail = tableDetail.NewRow();

                        //Bultos
                        rowDetail["TIPO_REG"] = 2;
                        rowDetail["NUMERO_ENVIO"] = FormatStringTnt(correlativeOT.ToString(), 30); 
                        rowDetail["KEYBUL"] = FormatStringTnt(dispatchDetail.SealNumber, 40); 
                        rowDetail["BULCLI"] = FormatStringTnt(dispatchDetail.Lpn.Code, 40); 

                        //Opcionales
                        rowDetail["BULREF"] = FormatStringTnt("", 20);
                        rowDetail["DENOM"] = FormatStringTnt("", 40);
                        rowDetail["KILOS"] = FormatStringTnt("", 12);
                        rowDetail["LARGO"] = FormatStringTnt("", 6);
                        rowDetail["ANCHO"] = FormatStringTnt("", 6);
                        rowDetail["ALTURA"] = FormatStringTnt("", 6);
                        rowDetail["VOLUMEN"] = FormatStringTnt("", 6);
                        rowDetail["LIBRE1"] = FormatStringTnt("", 30);
                        rowDetail["LIBRE2"] = FormatStringTnt("", 12);
                        rowDetail["LIBRE3"] = FormatStringTnt("", 12);
                        rowDetail["LIBRE4"] = FormatStringTnt("", 15);
                        rowDetail["OBSERV"] = FormatStringTnt("", 40);
                        rowDetail["FINAL"] = "29154622";

                        tableDetail.Rows.Add(rowDetail);
                    }

                    DataRow rowFooter = tableFooter.NewRow();

                    rowFooter["TIPO_REG"] = "T";
                    rowFooter["NUMERO_ENVIO"] = FormatStringTnt(correlativeOT.ToString(), 30); 
                    rowFooter["TIPODOC"] = FormatStringTnt(ConvertRefDocTypeCodeToTntFormat(dispatch.ReferenceDoc.ReferenceDocType.Code), 10);
                    rowFooter["NUMERO"] = FormatStringTnt(dispatch.OutboundOrder.Number, 40); 
                    rowFooter["DESCRIPCION"] = FormatStringTnt(dispatch.OutboundOrder.OutboundType.Name, 40); 
                    rowFooter["SITUACION_O"] = "1";
                    rowFooter["SITUACION_D"] = "1";
                    rowFooter["GESTIONAR_DEV"] = "1";

                    tableFooter.Rows.Add(rowFooter);
                }

                var updateCfgCarrierViewDTO = iDispatchingMGR.UpdateCorrelativeOT(new CfgCarrier() { CorrelativeOT = correlativeOT, Code = carrierCode }, context);

                if (updateCfgCarrierViewDTO.hasError())
                {
                    throw new Exception(updateCfgCarrierViewDTO.Errors.Message);
                }

                DownloadTntFile(theData);
            }
            catch(System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DownloadTntFile(DataSet theData)
        {
            try
            {
                StringBuilder str = new StringBuilder();

                foreach (DataRow rh in theData.Tables[0].Rows)
                {
                    var arrayHeader = convertArrayString(rh);
                    str.AppendLine(string.Join("", arrayHeader));

                    var tableDetail = theData.Tables["DETAIL"];
                    var rowDetail = tableDetail.Select("NUMERO_ENVIO = '" + rh["NUMERO_ENVIO"] + "'");
                    foreach (var rd in rowDetail)
                    {
                        var arrayDetail = convertArrayString(rd);
                        str.AppendLine(string.Join("", arrayDetail));
                    }

                    var tableFooter = theData.Tables["FOOTER"];
                    var rowFooter = tableFooter.Select("NUMERO_ENVIO = '" + rh["NUMERO_ENVIO"] + "'");
                    foreach (var rf in rowFooter)
                    {
                        var arrayFooter = convertArrayString(rf);
                        str.AppendLine(string.Join("", arrayFooter));
                    }
                }

                string attachment = "attachment; filename=TNT-" + DateTime.Now.ToString("ddMMyy-hhmm") + ".txt";
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("content-disposition", attachment);

                using (StreamWriter writer = new StreamWriter(Response.OutputStream))
                {
                    writer.WriteLine(str);
                }

                Response.End();
            }
            catch (System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Binaria.WMSTek.Framework.Entities.Warehousing.Stock ConsolidatedStock(List<DispatchDetail> listDispatchDetail)
        {
            ContextViewDTO newContext = new ContextViewDTO()
            {
                MainFilter = new List<EntityFilter>()
            };

            newContext.MainFilter.Add(CreateFilter("Warehouse", Master.ucMainFilter.idWhs.ToString()));
            newContext.MainFilter.Add(CreateFilter("Owner", Master.ucMainFilter.idOwn.ToString()));

            var listDynamicFilter = new List<string>()
            {
                "Item", "LpnSource"
            };

            var listStock = new List<Binaria.WMSTek.Framework.Entities.Warehousing.Stock>();

            foreach (var dispatchDetail in listDispatchDetail)
            {
                foreach (var nameFilter in listDynamicFilter)
                {
                    if (newContext.MainFilter.Exists(filter => filter.Name == nameFilter))
                        newContext.MainFilter.RemoveAll(filter => filter.Name == nameFilter);
                }

                newContext.MainFilter.Add(CreateFilter(listDynamicFilter[0], dispatchDetail.Item.Id.ToString()));
                newContext.MainFilter.Add(CreateFilter(listDynamicFilter[1], dispatchDetail.Lpn.IdCode));

                var stockViewDTO = iWarehousingMGR.GetStockByFilters(newContext);

                if (!stockViewDTO.hasError() && stockViewDTO.Entities.Count > 0)
                {
                    listStock.AddRange(stockViewDTO.Entities);
                }
            }

            return new Binaria.WMSTek.Framework.Entities.Warehousing.Stock()
            {
                TotalVolumen = listStock.Sum(s => s.TotalVolumen),
                TotalWeight = listStock.Sum(s => s.TotalWeight)
            };
        }

        private EntityFilter CreateFilter(string name, string value)
        {
            return new EntityFilter()
            {
                Name = name,
                FilterValues = new List<FilterItem>()
                {
                    new FilterItem()
                    {
                        Name = name,
                        Value = value
                    }
                }
            };
        }

        private string FormatStringTnt(string str, int length, char delimiter = ' ')
        {
            if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
            }

            return str.PadRight(length, delimiter);
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

        private string getPathFile(string templateFile)
        {
            string selected = string.Empty;

            if (!string.IsNullOrEmpty(templateFile))
            {
                var filexsdTemp = templateFile.Split('\\');

                if (filexsdTemp.Length > 0)
                {
                    var filexsd = filexsdTemp[filexsdTemp.Length - 1];
                    selected = Request.PhysicalApplicationPath + "WebResources\\CarrierTemplate\\" + filexsd;
                }
            }

            return selected;
        }

        private string ConvertRefDocTypeCodeToTntFormat(string refDocTypeCode)
        {
            string finalRefDocTypeCode = string.Empty;

            switch (refDocTypeCode)
            {
                case "FAC":
                    finalRefDocTypeCode = "FACT";
                    break;
                case "GD":
                    finalRefDocTypeCode = "GD";
                    break;
                case "NCRED":
                    finalRefDocTypeCode = "NC";
                    break;
                case "NVTA":
                    finalRefDocTypeCode = "NV";
                    break;
                default:
                    finalRefDocTypeCode = "ND";
                    break;
            }

            return finalRefDocTypeCode;
        }

        public void ShowAlert(string title, string message)
        {
            Encoding encod = Encoding.ASCII;
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('CarrierIntegrationDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, false);", true);
        }

        #endregion
    }
}