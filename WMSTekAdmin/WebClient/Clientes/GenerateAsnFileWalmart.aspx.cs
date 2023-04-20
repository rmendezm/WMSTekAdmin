using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using ClosedXML.Excel;
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

namespace Binaria.WMSTek.WebClient.Clientes
{
    public partial class GenerateAsnFileWalmart : BasePage
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

        private string customerCodeWalmart
        {
            get { return GetConst("CodeCustomerWalmart")[0]; }
        }
        #endregion

        #region "Eventos"
        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal) Initialize();
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
                var selecDispactch = new DispatchSpecial();

                if (ValidateSession(WMSTekSessions.GenerateAsnFileWalmart.SelectedDispatchASN))
                {
                    var listIdDispatch = (List<int>)Session[WMSTekSessions.GenerateAsnFileWalmart.ListIdDipatches];

                    ClearFilter("listDispatchDetail");
                    CreateFilterByList("listDispatch", listIdDispatch);

                    dispatchDetailViewDTO = iDispatchingMGR.GetByIdDispatchASNWalmart("Walmart", context);

                    if (!dispatchDetailViewDTO.hasError())
                    {
                        selecDispactch = (DispatchSpecial)Session[WMSTekSessions.GenerateAsnFileWalmart.SelectedDispatchASN];
                        isValidViewDTO = true;

                        if (dispatchDetailViewDTO.Entities != null && dispatchDetailViewDTO.Entities.Count > 0)
                        {

                            if (selecDispactch.Customer.CustomerB2B == null || selecDispactch.Customer.CustomerB2B.TemplateASNFile == null ||
                                selecDispactch.Customer.CustomerB2B.TemplateASNFile == "")
                            {
                                ShowAlert(this.lblTitle.Text, this.lblErrorTemplate.Text);
                            }
                            else
                            {
                                string file = getPathAsn(selecDispactch.Customer.CustomerB2B.TemplateASNFile);
                                XmlDocument xsd = new XmlDocument();
                                xsd.Load(file);

                                DataSet theData = new DataSet();
                                StringReader xmlSR = new StringReader(xsd.InnerXml);
                                theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                                DataTable tableDet = theData.Tables["RECORD"];
                                foreach (DispatchDetail disp in dispatchDetailViewDTO.Entities)
                                {
                                    if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                                    {
                                        throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                                    }

                                    DataRow rowDet = tableDet.NewRow();
                                    rowDet["OC"] = selecDispactch.OutboundOrder.ReferenceNumber;
                                    rowDet["COD_EAN"] = disp.Item.ItemCustomer.BarCode;
                                    rowDet["LOTE"] = (int)disp.ItemUom.ConversionFactor;
                                    rowDet["CANTIDAD"] = disp.Qty;
                                    rowDet["SKU"] = disp.Item.ItemCustomer.ItemCodeCustomer;

                                    tableDet.Rows.Add(rowDet);
                                }

                                dsToExcel(theData);
                            }

                            modalPopUpASN.Hide();
                            upEditNew.Update();
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

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtHourAsn.Text = "";
                this.txtDateAsn.Text = "";
                this.txtWarehouseASN.Text = "";

                ReloadData();
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

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Metodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "GenerateAsnFileWalmart";

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
                if (ValidateSession(WMSTekSessions.GenerateAsnFileWalmart.DispatchList))
                {
                    dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileWalmart.DispatchList];
                    isValidViewDTO = true;
                }
            }
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            //Tipos de Tareas
            this.Master.ucMainFilter.listTrackOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listTrackOutboundType = GetConst("OutboundOrderReadyForDispatchTrackOutboundType");
            this.Master.ucMainFilter.listOutboundType = new System.Collections.Generic.List<String>();
            this.Master.ucMainFilter.listOutboundType = GetConst("OutboundOrderReadyForDispatchOutboundType");
            this.Master.ucMainFilter.OutboundTypeCode = new string[] { };

            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.outboundTypeVisible = false;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = this.lblReferenceDoc.Text;
            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = this.lblReferenceNumbDoc.Text;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.DispatchAdvanceDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.DispatchAdvanceDaysAfter;

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

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;

            this.Master.ucTaskBar.btnDownloadVisible = true;
            this.Master.ucTaskBar.BtnDownloadClick += new EventHandler(btnSave_Click);
        }

        protected void OpenPopUpASN(List<int> listIdDipatch)
        {
            try
            {
                bool flagError = false;
                var listSpecialDispatch = new List<DispatchSpecial>();

                foreach (var idDispatch in listIdDipatch)
                {
                    var dispatchSpecialDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileWalmart.DispatchList];

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

                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Add(new FilterItem(selecDispactch.OutboundOrder.Number));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.OutboundType)].FilterValues.Add(new FilterItem(selecDispactch.OutboundOrder.OutboundType.Id.ToString()));
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
                    Session.Add(WMSTekSessions.GenerateAsnFileWalmart.SelectedDispatchASN, listSpecialDispatch.FirstOrDefault());
                    Session.Add(WMSTekSessions.GenerateAsnFileWalmart.ListIdDipatches, listIdDipatch);

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

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            DropDownList ddlOwn = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");
            customerViewDTO = iWarehousingMGR.GetCustomerByCodeAndOwn(context, customerCodeWalmart, int.Parse(ddlOwn.SelectedValue));

            if (customerViewDTO != null && customerViewDTO.Entities.Count > 0)
            {
                dispatchSpecialViewDTO = iDispatchingMGR.GetDispatchSpecialHeaderWalmart(context, customerViewDTO.Entities.FirstOrDefault().Id);

                if (!dispatchSpecialViewDTO.hasError() && dispatchSpecialViewDTO.Entities != null)
                {
                    Session.Add(WMSTekSessions.GenerateAsnFileWalmart.DispatchList, dispatchSpecialViewDTO);
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

            if (!dispatchSpecialViewDTO.hasConfigurationError() && dispatchSpecialViewDTO.Configuration != null && dispatchSpecialViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, dispatchSpecialViewDTO.Configuration);

            grdMgr.DataSource = dispatchSpecialViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(dispatchSpecialViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.GenerateAsnFileWalmart.DispatchDetailList];

                if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();

            }
        }

        protected void LoadDetail(int index)
        {
            grdDetail.EmptyDataText = this.Master.NoDetailsText;
            txtHourAsn.Text = "";
            txtDateAsn.Text = "";
            this.txtWarehouseASN.Text = "";

            if (index != -1)
            {
                int id = dispatchSpecialViewDTO.Entities[index].Id;

                ClearFilter("listDispatchDetail");
                CreateFilterByList("listDispatch", new List<int>() { id });

                dispatchDetailViewDTO = iDispatchingMGR.GetByIdDispatchASNWalmart("Walmart", context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.GenerateAsnFileWalmart.SelectedDispatchASN, dispatchSpecialViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.GenerateAsnFileWalmart.DispatchDetailList, dispatchDetailViewDTO);

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
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        private void dsToExcel(DataSet ds)
        {
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["OC"].ToString();
                worksheet.Cell(i, 2).Value = dr["COD_EAN"].ToString();
                worksheet.Cell(i, 2).DataType = XLCellValues.Text;
                worksheet.Cell(i, 3).Value = dr["LOTE"].ToString();               
                worksheet.Cell(i, 4).Value = dr["CANTIDAD"].ToString();
                worksheet.Cell(i, 5).Value = dr["SKU"].ToString();
                worksheet.Cell(i, 5).DataType = XLCellValues.Text;

                i++;
            }

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "WalmartASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            Response.End();
        }

        private string getPathAsn(string templateASNFile)
        {
            string selected = string.Empty;

            if (!string.IsNullOrEmpty(templateASNFile))
            {
                var filexsdTemp = templateASNFile.Split('\\');

                if (filexsdTemp.Length > 0)
                {
                    var filexsd = filexsdTemp[filexsdTemp.Length - 1];
                    selected = Request.PhysicalApplicationPath + "WebResources\\B2BTemplateASN\\" + filexsd;
                }
            }

            return selected;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('Walmart', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail');", true);
        }

        #endregion
    }
}