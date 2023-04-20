using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Text;
using ClosedXML.Excel;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Devices;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class B2BAdministration : BasePage
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

                // Si no esta en modo Configuration, sigue el curso normal
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
                //Realiza un renderizado de la pagina
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
                var customer = ValidateCustomerFromOutboundOrder();
                if (customer.Key == null)
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = lblTitleErrorCustomerSelected.Text, Message = lblMessageErrorCustomerSelected.Text });
                }
                else
                {
                    OpenPopUpASN();
                } 
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnGenerateASN_Click(object sender, EventArgs e)
        {
            try
            {
                var customer = ValidateCustomerFromOutboundOrder();
                var listFinalDispatchDetail = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities;

                if (listFinalDispatchDetail.Count > 0)
                {
                    if (string.IsNullOrEmpty(hidNroCita.Value) || !hidNroCita.Value.Trim().Equals(txtNroCita.Text.Trim()))
                    {
                        var outboundOrderViewDTO = SaveAppointmentNumber(txtNroCita.Text.Trim());

                        if (outboundOrderViewDTO.hasError())
                        {
                            this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
                            return;
                        }
                    }

                    CreateAsn(customer.Key, customer.Value, listFinalDispatchDetail.Select(dispatchDetail => dispatchDetail.Id).ToList());

                }
                
                isValidViewDTO = true;
            }
            catch (System.Threading.ThreadAbortException th)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                if (currentIndexToLoadDetail != -1)
                {
                    ReloadData();
                    LoadDetail(currentIndexToLoadDetail);
                }
                else
                {
                    ReloadData();
                }

                ShowAlert(this.lblTitle.Text, ex.Message);
            }
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            divEditNew.Visible = false;
            modalPopUpASN.Hide();
            upEditNew.Update();
            isValidViewDTO = true;
        }

        protected void btnLpnBundle_Click(object sender, EventArgs e)
        {
            try
            {
                var listDetailsSelected = GetAllRowsSelected();

                if (listDetailsSelected.Count > 0)
                {
                    string lPNTypePacking = GetConst("LPNTypePacking")[0];
                    string LPNNumberSufix = GetConst("LPNNumberSufix")[0];

                    var dispatchDetailViewDTO = iDispatchingMGR.SaveLabelBundlesB2B(listDetailsSelected, lPNTypePacking, LPNNumberSufix, context);

                    if (!dispatchDetailViewDTO.hasError())
                    {
                        if (currentIndexToLoadDetail != -1)
                        {
                            ReloadData();
                            LoadDetail(currentIndexToLoadDetail);
                        }
                        else
                        {
                            ReloadData();
                        }
                        
                        ucStatus.ShowMessage(dispatchDetailViewDTO.MessageStatus.Message);
                    }
                    else
                    {
                        this.Master.ucError.ShowError(dispatchDetailViewDTO.Errors);
                    }
                }
                else
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblNoDetailSelected.Text });
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnLpnPallet_Click(object sender, EventArgs e)
        {
            try
            {
                var listDetailsSelected = GetAllRowsSelected();

                if (listDetailsSelected.Count > 0)
                {
                    var dispatchDetailViewDTO = iDispatchingMGR.SaveLabelPalletB2B(listDetailsSelected, context);

                    if (!dispatchDetailViewDTO.hasError())
                    {
                        if (currentIndexToLoadDetail != -1)
                        {
                            ReloadData();
                            LoadDetail(currentIndexToLoadDetail);
                        }
                        else
                        {
                            ReloadData();
                        }

                        ucStatus.ShowMessage(dispatchDetailViewDTO.MessageStatus.Message);  
                    }
                    else
                    {
                        if (currentIndexToLoadDetail != -1)
                        {
                            ReloadData();
                            LoadDetail(currentIndexToLoadDetail);
                        }
                        else
                        {
                            ReloadData();
                        }

                        this.Master.ucError.ShowError(dispatchDetailViewDTO.Errors);
                    }
                }
                else
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblNoDetailSelected.Text });
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

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
                    ReloadData();
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

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
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

        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void grdDetail_DataBound(object sender, EventArgs e)
        {
            try
            {
                for (int i = grdDetail.Rows.Count - 1; i > 0; i--)
                {
                    int pageInd = grdDetail.PageSize * grdDetail.PageIndex;

                    var distpachDetailRow = dispatchDetailViewDTO.Entities[i + pageInd];
                    var dispatchDetailpreviousRow = dispatchDetailViewDTO.Entities[(i + pageInd) - 1];

                    GridViewRow row = grdDetail.Rows[i];
                    GridViewRow previousRow = grdDetail.Rows[i - 1];

                    int positionColumnCheckbox = 0;

                    if (!string.IsNullOrEmpty(distpachDetailRow.Lpn.IdCode) && !string.IsNullOrEmpty(dispatchDetailpreviousRow.Lpn.IdCode))
                    {
                        if (distpachDetailRow.Lpn.IdCode == dispatchDetailpreviousRow.Lpn.IdCode)
                        {
                            int positionColumnLpn = 2;
                            //Lpn
                            if (previousRow.Cells[positionColumnLpn].RowSpan == 0)
                            {
                                if (row.Cells[positionColumnLpn].RowSpan == 0)
                                {
                                    previousRow.Cells[positionColumnCheckbox].RowSpan += 2;
                                    previousRow.Cells[positionColumnLpn].RowSpan += 2;
                                }
                                else
                                {
                                    previousRow.Cells[positionColumnCheckbox].RowSpan = row.Cells[positionColumnCheckbox].RowSpan + 1;
                                    previousRow.Cells[positionColumnLpn].RowSpan = row.Cells[positionColumnLpn].RowSpan + 1;
                                }
                                row.Cells[positionColumnCheckbox].Visible = false;
                                row.Cells[positionColumnLpn].Visible = false;
                            }
                        }
                    } 

                    if (!string.IsNullOrEmpty(distpachDetailRow.IdLpnCodeContainer) && !string.IsNullOrEmpty(dispatchDetailpreviousRow.IdLpnCodeContainer))
                    {
                        if (distpachDetailRow.IdLpnCodeContainer == dispatchDetailpreviousRow.IdLpnCodeContainer)
                        {
                            int positionColumnLpnContainer = 3;
                            //Lpn Contenedor
                            if (previousRow.Cells[positionColumnLpnContainer].RowSpan == 0)
                            {
                                if (row.Cells[positionColumnLpnContainer].RowSpan == 0)
                                {
                                    previousRow.Cells[positionColumnCheckbox].RowSpan += 2;
                                    previousRow.Cells[positionColumnLpnContainer].RowSpan += 2;
                                }
                                else
                                {
                                    previousRow.Cells[positionColumnCheckbox].RowSpan = row.Cells[positionColumnCheckbox].RowSpan + 1;
                                    previousRow.Cells[positionColumnLpnContainer].RowSpan = row.Cells[positionColumnLpnContainer].RowSpan + 1;
                                }
                                row.Cells[positionColumnCheckbox].Visible = false;
                                row.Cells[positionColumnLpnContainer].Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "loadingAjaxStart", "loadingAjaxStart();", true);

                var listDetailsSelected = GetAllRowsSelectedPrint();

                if (listDetailsSelected.Count > 0)
                {
                    int rowsWithoutSealNumber = listDetailsSelected.Where(dd => string.IsNullOrEmpty(dd.SealNumber)).Count();

                    if (rowsWithoutSealNumber == 0)
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
                        var printLabelViewDTO = iDispatchingMGR.PrintLabel(listDetailsSelected, printer, qtyCopies, context);

                        if (!printLabelViewDTO.hasError())
                        {
                            if (currentIndexToLoadDetail != -1)
                            {
                                ReloadData();
                                LoadDetail(currentIndexToLoadDetail);
                            }
                            else
                            {
                                ReloadData();
                            }

                            ucStatus.ShowMessage(printLabelViewDTO.MessageStatus.Message);
                        }
                        else
                        {
                            this.Master.ucError.ShowError(printLabelViewDTO.Errors);
                        }

                        LoadControls();
                    }
                    else
                    {
                        this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblValidateDispatchDetailWithSealNumber.Text });
                    }
                }
                else
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblNoDetailSelected.Text });
                }

                ScriptManager.RegisterStartupScript(Page, GetType(), "loadingAjaxStop", "loadingAjaxStop();", true);
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void grdDetail_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                var data = FilterDetail();

                PopulateGrid();

                grdDetail.DataSource = data;
                grdDetail.DataBind();
                grdDetail.PageIndex = e.NewPageIndex;
                grdDetail.DataBind();

                CallJsGridViewDetail();

                isValidViewDTO = false;
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        protected void btnFilterDetail_Click(object sender, EventArgs e)
        {
            try
            {
                var data = FilterDetail();

                PopulateGrid();

                grdDetail.DataSource = data;
                grdDetail.PageIndex = 0;
                grdDetail.DataBind();

                CallJsGridViewDetail();

                isValidViewDTO = false;
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        private List<DispatchDetail> FilterDetail()
        {
            dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail];

            var list = dispatchDetailViewDTO.Entities;

            if (list != null && list.Count > 0)
            {
                if (!string.IsNullOrEmpty(txtFilterDetailByLpn.Text))
                {
                    list = list.Where(dd => dd.Lpn.Code.ToLower().Contains(txtFilterDetailByLpn.Text.Trim().ToLower())).ToList();

                    if(list.Count ==  0)
                    {
                        list = dispatchDetailViewDTO.Entities.Where(dd => dd.IdLpnCodeContainer != null).ToList();
                        list = list.Where(dd => dd.IdLpnCodeContainer.ToLower().Contains(txtFilterDetailByLpn.Text.Trim().ToLower())).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(txtFilterDetailByItem.Text))
                {
                    list = list.Where(dd => dd.Item.ShortName.ToLower().Contains(txtFilterDetailByItem.Text.Trim().ToLower())).ToList();
                }
            }

            return list;
        }

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "B2BAdministration";

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

            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.btnExcelVisible = true;

            this.Master.ucTaskBar.btnDownloadVisible = true;
            this.Master.ucTaskBar.BtnDownloadClick += new EventHandler(btnSave_Click);
            this.Master.ucTaskBar.btnDownloadToolTip = lblAsnButtonTooltip.Text;

            Master.ucTaskBar.BtnPrintClick += new EventHandler(btnPrint_Click);
            Master.ucTaskBar.btnPrintVisible = true;
            Master.ucTaskBar.btnPrintEnabled = false;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.DocumentNumberLabel = lblDocName.Text;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblCustomer.Text;
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
                grdDetail.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByDetailGridPage.ToString()));
                grdDetail.EmptyDataText = this.Master.EmptyGridText;
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
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

            ucStatus.ShowRecordInfo(dispatchSpecialViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
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

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
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

                //upGrid.Update();
            }
        }

        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            dispatchSpecialViewDTO = iDispatchingMGR.GetDispatchSpecialHeaderForB2BAdministration(context);

            if (!dispatchSpecialViewDTO.hasError() && dispatchSpecialViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.B2BAdministration.OrdersList, dispatchSpecialViewDTO);
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
                dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.B2BAdministration.OrdersList];
                
                int idOutboundOrder = dispatchSpecialViewDTO.Entities[index].IdOutboundOrder;

                dispatchDetailViewDTO = iDispatchingMGR.GetDispatchSpecialDetailForB2BAdministration(idOutboundOrder, context);

                if (dispatchDetailViewDTO != null && dispatchDetailViewDTO.Entities.Count > 0)
                {
                    // Configura ORDEN de las columnas de la grilla
                    if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                    // Detalle de Documentos de Entrada
                    grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                    grdDetail.DataBind();

                    lblNroDoc.Text = dispatchSpecialViewDTO.Entities[index].OutboundOrder.Number;

                    CallJsGridViewDetail();
                }

                Session.Add(WMSTekSessions.B2BAdministration.OrderSelected, dispatchSpecialViewDTO.Entities[index]);
                Session.Add(WMSTekSessions.B2BAdministration.OrderDetail, dispatchDetailViewDTO);
                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }

        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail];

                // Configura ORDEN de las columnas de la grilla
                if (!dispatchDetailViewDTO.hasConfigurationError() && dispatchDetailViewDTO.Configuration != null && dispatchDetailViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, dispatchDetailViewDTO.Configuration);

                // Detalle de Documentos de Entrada
                grdDetail.DataSource = dispatchDetailViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }

        private List<DispatchDetail> GetAllRowsSelected()
        {
            var listDispatchDetailSelected = new List<DispatchDetail>();
            dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail];

            for (int i = 0; i < grdDetail.Rows.Count; i++)
            {
                var row = grdDetail.Rows[i];
                var chkSelectDispatchDetail = (CheckBox)row.FindControl("chkSelectDispatchDetail");
                var lblIdDispatchDetail = (Label)row.FindControl("lblIdDispatchDetail");

                if (chkSelectDispatchDetail.Checked)
                {
                    if (row.Cells[0].RowSpan > 0)
                    {
                        for (int j = 0; j < row.Cells[0].RowSpan; j++)
                        {
                            var rowAux = grdDetail.Rows[j];
                            var lblIdDispatchDetailAux = (Label)rowAux.FindControl("lblIdDispatchDetail");
                            var dispatchDet = dispatchDetailViewDTO.Entities.Where(d => d.Id == int.Parse(lblIdDispatchDetailAux.Text.Trim())).First();

                            var dispatchDetAux = new DispatchDetail()
                            {
                                Dispatch = dispatchDet.Dispatch,
                                Warehouse = dispatchDet.Warehouse,
                                Item = dispatchDet.Item,
                                OutboundDetail = dispatchDet.OutboundDetail,
                                Lpn = dispatchDet.Lpn,
                                IdLpnCodeContainer = dispatchDet.IdLpnCodeContainer
                            };

                            if (listDispatchDetailSelected != null && !listDispatchDetailSelected.Exists(s => s.Dispatch.Id == dispatchDetAux.Dispatch.Id &&
                             s.Warehouse.Id == dispatchDetAux.Warehouse.Id && s.Lpn.IdCode == dispatchDetAux.Lpn.IdCode && 
                             s.IdLpnCodeContainer == dispatchDetAux.IdLpnCodeContainer))
                            {
                                listDispatchDetailSelected.Add(dispatchDetAux);
                            }                          
                        }
                    }
                    else
                    {
                        listDispatchDetailSelected.Add(dispatchDetailViewDTO.Entities.Where(d => d.Id == int.Parse(lblIdDispatchDetail.Text.Trim())).First());
                    }
                }
            }

            return listDispatchDetailSelected;
        }

        private List<DispatchDetail> GetAllRowsSelectedPrint()
        {
            var listDispatchDetailSelected = new List<DispatchDetail>();
            dispatchDetailViewDTO = (GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail];

            for (int i = 0; i < grdDetail.Rows.Count; i++)
            {
                var row = grdDetail.Rows[i];
                var chkSelectDispatchDetail = (CheckBox)row.FindControl("chkSelectDispatchDetail");
                var lblIdDispatchDetail = (Label)row.FindControl("lblIdDispatchDetail");

                if (chkSelectDispatchDetail.Checked)
                {
                    if (row.Cells[0].RowSpan > 0)
                    {
                        for (int j = 0; j < row.Cells[0].RowSpan; j++)
                        {
                            var rowAux = grdDetail.Rows[j];
                            var lblIdDispatchDetailAux = (Label)rowAux.FindControl("lblIdDispatchDetail");
                            var dispatchDet = dispatchDetailViewDTO.Entities.Where(d => d.Id == int.Parse(lblIdDispatchDetailAux.Text.Trim())).First();

                            var dispatchDetAux = new DispatchDetail()
                            {
                                Dispatch = dispatchDet.Dispatch,
                                Warehouse = dispatchDet.Warehouse,
                                Item = dispatchDet.Item,
                                OutboundDetail = dispatchDet.OutboundDetail,
                                Lpn = dispatchDet.Lpn,
                                IdLpnCodeContainer = dispatchDet.IdLpnCodeContainer,
                                SealNumber = dispatchDet.SealNumber
                            };

                            if (listDispatchDetailSelected != null && !listDispatchDetailSelected.Exists(s => s.Dispatch.Id == dispatchDetAux.Dispatch.Id &&
                             s.Warehouse.Id == dispatchDetAux.Warehouse.Id && s.IdLpnCodeContainer == dispatchDetAux.IdLpnCodeContainer))
                            {
                                listDispatchDetailSelected.Add(dispatchDetAux);
                            }
                        }
                    }
                    else
                    {
                        listDispatchDetailSelected.Add(dispatchDetailViewDTO.Entities.Where(d => d.Id == int.Parse(lblIdDispatchDetail.Text.Trim())).First());
                    }
                }
            }

            return listDispatchDetailSelected;
        }
        protected void OpenPopUpASN()
        {
            try
            {
                txtNroCita.Text = string.Empty;

                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                var outboundOrderParam = new OutboundOrder()
                {
                    Id = selectDispatch.OutboundDetail.OutboundOrder.Id,
                    Warehouse = new Warehouse(Master.ucMainFilter.idWhs),
                    Owner = new Owner(Master.ucMainFilter.idOwn)
                };
                var outboundOrderViewDTO = iDispatchingMGR.GetAppointmentNumber(outboundOrderParam, context);

                if (!outboundOrderViewDTO.hasError())
                {
                    if (outboundOrderViewDTO.Entities.Count > 0 && !string.IsNullOrEmpty(outboundOrderViewDTO.Entities.First().Invoice))
                    {
                        txtNroCita.Text = outboundOrderViewDTO.Entities.First().Invoice;
                        hidNroCita.Value = outboundOrderViewDTO.Entities.First().Invoice;
                    }
                }

                this.txtDateAsn.Text = "";
                this.txtHourAsn.Text = "";
                this.txtWarehouseASN.Text = "";
                txtNroCita.Text = string.Empty;
                
                divEditNew.Visible = true;
                modalPopUpASN.Show();
                upEditNew.Update();
                isValidViewDTO = false;

                
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }
        }

        private GenericViewDTO<OutboundOrder> SaveAppointmentNumber(string appointmentNumber)
        {
            var outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();

            try
            {
                var selectDispatch = (DispatchSpecial)Session[WMSTekSessions.B2BAdministration.OrderSelected];
                var outboundOrderParam = new OutboundOrder()
                {
                    Invoice = appointmentNumber,
                    Owner = new Owner(Master.ucMainFilter.idOwn),
                    Warehouse = new Warehouse(Master.ucMainFilter.idWhs),
                    Id = selectDispatch.OutboundOrder.Id
                };

                outboundOrderViewDTO = iDispatchingMGR.SaveAppointmentNumber(outboundOrderParam, context);
            }
            catch (Exception ex)
            {
                dispatchSpecialViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(dispatchSpecialViewDTO.Errors);
            }

            return outboundOrderViewDTO;
        }

        private void CreateAsn(string customerCode, string customerName, List<int> listIdDispatchDetail)
        {
            try
            {
                switch (customerName)
                {
                    case CustomerB2B.SODIMAC:
                        CreateAsnSodimac(listIdDispatchDetail);
                        break;
                    case CustomerB2B.ABCDIN:
                        CreateAsnABCDin(listIdDispatchDetail);
                        break;
                    case CustomerB2B.CORONA:
                        CreateAsnCorona(listIdDispatchDetail);
                        break;
                    case CustomerB2B.EASY:
                        CreateAsnEasy(listIdDispatchDetail);
                        break;
                    case CustomerB2B.FALABELLA:
                        CreateAsnFalabella(listIdDispatchDetail);
                        break;
                    case CustomerB2B.HITES:
                        CreateAsnHites(listIdDispatchDetail);
                        break;
                    case CustomerB2B.LAPOLAR:
                        CreateAsnLaPolar(listIdDispatchDetail);
                        break;
                    case CustomerB2B.RIPLEY:
                        CreateAsnRipley(listIdDispatchDetail);
                        break;
                    case CustomerB2B.PARIS:
                        CreateAsnParis(listIdDispatchDetail);
                        break;
                    case CustomerB2B.JOHNSON:
                        CreateAsnJohnson(listIdDispatchDetail);
                        break;
                    case CustomerB2B.TOTTUS:
                        CreateAsnTottus(listIdDispatchDetail);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }

        private void CreateAsnABCDin(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailABCDinDTO = iDispatchingMGR.GetByIdDispatchASNABCDin("ABCDin", context);
                if (!dispatchDetailABCDinDTO.hasError())
                {
                    if (dispatchDetailABCDinDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableDet = theData.Tables["RECORD"];

                    foreach (DispatchDetail disp in dispatchDetailABCDinDTO.Entities)
                    {
                        if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                        }

                        if (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                        {
                            throw new Exception(lblMissingBill.Text);
                        }

                        DataRow rowDet = tableDet.NewRow();
                        rowDet["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                        rowDet["SUCURSAL_ORIGEN"] = txtWarehouseASN.Text.Trim();
                        rowDet["SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                        rowDet["NRO_BULTO"] = disp.SealNumber == null ? "" : disp.SealNumber;
                        rowDet["SKU"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                        rowDet["CANTIDAD"] = disp.Qty;
                        rowDet["NRO_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                        rowDet["TIPO_DOC"] = "EFAC";
                        tableDet.Rows.Add(rowDet);
                    }

                    dsToExcelABCDin(theData);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailABCDinDTO.Errors);
                }
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

        private void CreateAsnCorona(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailCoronaDTO = iDispatchingMGR.GetByIdDispatchASNCorona("Corona", context);
                if (!dispatchDetailCoronaDTO.hasError())
                {
                    if (dispatchDetailCoronaDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableDet = theData.Tables["RECORD"];

                    foreach (DispatchDetail disp in dispatchDetailCoronaDTO.Entities)
                    {
                        if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                        }

                        if (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                        {
                            throw new Exception(lblMissingBill.Text);
                        }

                        DataRow rowDet = tableDet.NewRow();
                        rowDet["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                        rowDet["SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                        rowDet["NRO_BULTO"] = disp.SealNumber == null ? "" : disp.SealNumber;
                        rowDet["SKU"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                        rowDet["CANTIDAD"] = disp.Qty;
                        rowDet["TIPO_DOC"] = "FE";
                        rowDet["NRO_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                        rowDet["FECHA_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.InvoiceDate.ToString("dd/MM/yyyy");
                        tableDet.Rows.Add(rowDet);
                    }

                    dsToExcelCorona(theData);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailCoronaDTO.Errors);
                }
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

        private void CreateAsnEasy(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailEasyDTO = iDispatchingMGR.GetByIdDispatchASNEasy("Easy", context);
                if (!dispatchDetailEasyDTO.hasError())
                {
                    if (dispatchDetailEasyDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    //Carga el archivo xsd del cliente
                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableDet = theData.Tables["RECORD"];

                    //Valida si existe algun item sin la configuracion de ItemCustomer
                    if (dispatchDetailEasyDTO.Entities.Exists(r => string.IsNullOrEmpty(r.Item.ItemCustomer.ItemCodeCustomer)))
                    {
                        string idDispatch = dispatchDetailEasyDTO.Entities.First(r => string.IsNullOrEmpty(r.Item.ItemCustomer.ItemCodeCustomer)).Id.ToString();
                        throw new Exception(lblDoesntExistItemCustomerMatch.Text + idDispatch);
                    }

                    var lstDispatchDetDist = (from p in dispatchDetailEasyDTO.Entities
                                          select new
                                          {
                                              idDispatch = p.Dispatch.Id,
                                              sku = p.Item.ItemCustomer.ItemCodeCustomer,
                                              countLpn = p.SpecialField4
                                          }).ToList().Distinct();

                    var lstDispatchDet = from p in lstDispatchDetDist
                                         group p by p.sku into g
                                         select new
                                         {
                                             sku = g.Key,
                                             countLpn = g.Sum(x => int.Parse(x.countLpn)).ToString()
                                         };


                    foreach (var disp in lstDispatchDet.Distinct())
                    {
                        DataRow rowDet = tableDet.NewRow();
                        rowDet["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                        rowDet["SKU"] = disp.sku;
                        rowDet["CANTIDAD"] = int.Parse(disp.countLpn);
                        tableDet.Rows.Add(rowDet);
                    }

                    //foreach (DispatchDetail disp in dispatchDetailEasyDTO.Entities)
                    //{
                    //    if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                    //    {
                    //        throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                    //    }

                    //    DataRow rowDet = tableDet.NewRow();
                    //    rowDet["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                    //    rowDet["SKU"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                    //    rowDet["CANTIDAD"] = int.Parse(disp.SpecialField4);
                    //    tableDet.Rows.Add(rowDet);
                    //}

                    dsToExcelEasy(theData);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailEasyDTO.Errors);
                }
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

        private void CreateAsnFalabella(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailFalabellaDTO = iDispatchingMGR.GetByIdDispatchASNFalabella("Falabella", context);
                if (!dispatchDetailFalabellaDTO.hasError())
                {
                    if (dispatchDetailFalabellaDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    //Carga el archivo xsd del cliente
                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    if (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                    {
                        throw new Exception(lblMissingBill.Text);
                    }

                    int nroFactura = (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber) ? 0 : Int32.Parse(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber));
                    DataTable tableInbound = theData.Tables["PIR"];
                    DataRow rowInbound = tableInbound.NewRow();
                    rowInbound["num"] = selectDispatch.Dispatch.Id;
                    rowInbound["NRO_OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                    rowInbound["FECHA_DESPACHO"] = this.txtDateAsn.Text.Trim();
                    rowInbound["HORA_DESPACHO"] = this.txtHourAsn.Text.Trim();
                    rowInbound["TOTAL_BULTOS"] = dispatchDetailViewDTO.Entities.Select(s => s.Lpn.IdCode).Distinct().Count().ToString();
                    rowInbound["TOTAL_TOTES"] = "0";
                    rowInbound["TOTAL_COLGADOS"] = "0";
                    rowInbound["NRO_SERIE_FACT"] = "";
                    rowInbound["NRO_FACTURA"] = nroFactura;
                    rowInbound["ALMACEN"] = this.txtWarehouseASN.Text.Trim();
                    rowInbound["GUIAS_DESPACHO"] = "";
                    rowInbound["PIR_ID"] = 1;
                    tableInbound.Rows.Add(rowInbound);

                    DataTable tableMatch = theData.Tables["PRODUCTO"];
                    DataRow rowMatch = tableMatch.NewRow();
                    rowMatch["PIR_Id"] = 1;
                    rowMatch["PRODUCTO_Id"] = 1;
                    tableMatch.Rows.Add(rowMatch);

                    foreach (var disp in dispatchDetailFalabellaDTO.Entities)
                    {
                        DataTable tableItem = theData.Tables["PRODUCTO_ROW"];
                        DataRow rowItem = tableItem.NewRow();
                        rowItem["num"] = disp.Id;

                        if (string.IsNullOrEmpty(disp.Item.ItemCustomer.BarCode))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Item.Code);
                        }

                        rowItem["UPC"] = string.IsNullOrEmpty(disp.Item.ItemCustomer.BarCode) ? "" : disp.Item.ItemCustomer.BarCode;
                        rowItem["DESCRIPCION_LARGA"] = disp.Item.LongName;
                        rowItem["NRO_LOCAL"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                        rowItem["LOCAL"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Name) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Name;
                        rowItem["CANTIDAD"] = disp.Qty;
                        rowItem["TIPO_EMBALAJE"] = disp.Lpn.LPNType.Code;
                        rowItem["NRO_BULTO"] = string.IsNullOrEmpty(disp.SealNumber) ? "0" : disp.SealNumber;
                        rowItem["PRODUCTO_Id"] = 1;
                        tableItem.Rows.Add(rowItem);
                    }

                    modalPopUpASN.Hide();
                    upEditNew.Update();

                    string attachment = "attachment; filename=ePIR-" + nroFactura + "_" + context.SessionInfo.User.UserName + "_" + DateTime.Now.ToString("yyyMMddhhmmss") + ".xml";
                    var isoEncoding = Encoding.GetEncoding("ISO-8859-1");
                    Response.Clear();
                    Response.ContentType = "text/xml";
                    Response.Charset = "ISO-8859-1";
                    Response.ContentEncoding = isoEncoding;
                    //XmlWriter xmlWriter = new System.Xml.XmlTextWriter(this.Response.OutputStream, isoEncoding);

                    // create xml data document with xml declaration
                    XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument(theData);
                    xmlDoc.DataSet.EnforceConstraints = false;
                    XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                    xmlDoc.PrependChild(xmlDec);

                    Response.AddHeader("content-disposition", attachment);
                    Response.Write(xmlDoc.InnerXml);

                    Response.End();
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailFalabellaDTO.Errors);
                }
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

        private void CreateAsnHites(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailHitesDTO = iDispatchingMGR.GetByIdDispatchASNHites("Hites", context);
                if (!dispatchDetailHitesDTO.hasError())
                {
                    if (dispatchDetailHitesDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableDet = theData.Tables["RECORD"];

                    foreach (DispatchDetail disp in dispatchDetailHitesDTO.Entities)
                    {
                        if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                        }

                        if (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                        {
                            throw new Exception(lblMissingBill.Text);
                        }

                        DataRow rowDet = tableDet.NewRow();
                        rowDet["RUT_PROVEEDOR"] = context.SessionInfo.Company.Code;
                        rowDet["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                        rowDet["SUCURSAL_ORIGEN"] = txtWarehouseASN.Text.Trim();
                        rowDet["SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                        rowDet["NRO_BULTO"] = disp.SealNumber == null ? "" : disp.SealNumber;
                        rowDet["SKU_CLIENTE"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                        rowDet["SKU_COMPANIA"] = disp.Item.Code;
                        rowDet["CANTIDAD"] = disp.Qty;
                        rowDet["NRO_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                        rowDet["TIPO_DOC"] = "FAC";
                        tableDet.Rows.Add(rowDet);
                    }

                    dsToExcelHites(theData);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailHitesDTO.Errors);
                }
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

        private void CreateAsnLaPolar(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailLaPolarDTO = iDispatchingMGR.GetByIdDispatchASNLaPolar("LaPolar", context);
                if (!dispatchDetailLaPolarDTO.hasError())
                {
                    if (dispatchDetailLaPolarDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }
                    
                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableDet = theData.Tables["RECORD"];

                    foreach (DispatchDetail disp in dispatchDetailLaPolarDTO.Entities)
                    {
                        if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Item.Code.ToString());
                        }

                        DataRow rowDet = tableDet.NewRow();
                        rowDet["NRO_BULTO"] = disp.SealNumber == null ? "" : disp.SealNumber;
                        rowDet["TIPO_EMBALAJE"] = "CAJAS";
                        rowDet["COD_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                        rowDet["DESC_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Name) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Name;
                        rowDet["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                        rowDet["TIPO_OC"] = selectDispatch.OutboundDetail.OutboundOrder.OutboundType.Name;
                        rowDet["SKU_CLIENTE"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                        rowDet["DESC_ARTICULO"] = disp.Item.Description;
                        rowDet["CANTIDAD"] = disp.Qty;
                        tableDet.Rows.Add(rowDet);
                    }

                    dsToExcelLaPolar(theData);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailLaPolarDTO.Errors);
                }
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

        private void CreateAsnRipley(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailRipleyDTO = iDispatchingMGR.GetByIdDispatchASNRipley("Ripley", context);
                if (!dispatchDetailRipleyDTO.hasError())
                {
                    if (dispatchDetailRipleyDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    if (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                    {
                        throw new Exception(lblMissingBill.Text);
                    }

                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableInbound = theData.Tables["CABECERA"];
                    DataRow rowInbound = tableInbound.NewRow();
                    rowInbound["RUT_PROVEEDOR"] = context.SessionInfo.Company.Code;
                    rowInbound["DESC_DOC_DESPACHO"] = "";
                    rowInbound["NRO_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                    rowInbound["DOC_ES_ELECTRONICO"] = "FACTURA_ELECTRONICA";
                    rowInbound["OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;

                    tableInbound.Rows.Add(rowInbound);

                    DataTable tableDet = theData.Tables["DETALLE"];
                    foreach (DispatchDetail disp in dispatchDetailRipleyDTO.Entities)
                    {
                        if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                        }

                        DataRow rowDet = tableDet.NewRow();
                        rowDet["NRO_BULTO"] = string.IsNullOrEmpty(disp.SealNumber) ? "" : disp.SealNumber;
                        rowDet["FEC_BULTO"] = txtDateAsn.Text.Trim();
                        rowDet["SKU_CLIENTE"] = disp.Item.ItemCustomer.ItemCodeCustomer.Trim();
                        rowDet["SKU_COMPANIA"] = disp.Item.Code;
                        rowDet["CANTIDAD"] = disp.Qty;

                        tableDet.Rows.Add(rowDet);
                    }

                    dsToXmlRipley(theData);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailRipleyDTO.Errors);
                }
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

        private void CreateAsnSodimac(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailSodimacDTO = iDispatchingMGR.GetByIdDispatchSodimac("Sodimac", context);
                if (!dispatchDetailSodimacDTO.hasError())
                {
                    if (dispatchDetailSodimacDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    //Se llena con los ReferenceDocNumber asociados al despacho

                    List<string> lstrefDocNumber = dispatchDetailSodimacDTO.Entities.Select(dd => dd.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber).Distinct().ToList(); 

                    //Carga el archivo xsd del cliente
                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);

                    if (string.IsNullOrEmpty(file))
                    {
                        throw new Exception(lblMissXsdFile.Text);
                    }

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

                    DataTable tableInbound = theData.Tables["Cabecera"];
                    DataRow rowInbound = tableInbound.NewRow();
                    rowInbound["NRO"] = 1;
                    rowInbound["NRO_CITA"] = this.txtNroCita.Text;
                    rowInbound["NRO_OC"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                    rowInbound["FECHA_DESPACHO_PACTADA"] = formatDate(this.txtDateAsn.Text.Trim().ToString());
                    rowInbound["HORA_DESPACHO_PACTADA"] = this.txtHourAsn.Text.Trim() + ":00";
                    rowInbound["TOTAL_BULTOS"] = dispatchDetailSodimacDTO.Entities.Where(dispatchDetail => bundleCodes.Contains(dispatchDetail.Lpn.LPNType.Code)).Select(s => s.Lpn.IdCode).Distinct().Count().ToString();
                    rowInbound["TOTAL_TOTES"] = 0;
                    rowInbound["TOTAL_PALLETS"] = dispatchDetailSodimacDTO.Entities.Where(dispatchDetail => palletCodes.Contains(dispatchDetail.Lpn.LPNType.Code)).Select(s => s.Lpn.IdCode).Distinct().Count().ToString();
                    rowInbound["GUIA"] = (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber) ? 0 : int.Parse(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber));
                    rowInbound["BODEGA"] = this.txtWarehouseASN.Text.Trim();
                    rowInbound["CABECERA_Id"] = 1;
                    tableInbound.Rows.Add(rowInbound);

                    DataTable tableDet = theData.Tables["DETALLE"];
                    foreach (var disp in dispatchDetailSodimacDTO.Entities)
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
                        if (string.IsNullOrEmpty(docRef))
                        {
                            throw new Exception(lblMissingBill.Text);
                        }

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

                    modalPopUpASN.Hide();
                    upEditNew.Update();

                    Response.End();
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailSodimacDTO.Errors);
                }
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

        private void CreateAsnTottus(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailTottusDTO = iDispatchingMGR.GetByIdDispatchASNTottus("Tottus", context);
                if (!dispatchDetailTottusDTO.hasError())
                {
                    if (dispatchDetailTottusDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    string separator = "|";
                    //Carga el archivo xsd del cliente
                    string file = getPathAsn(selectDispatch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
                    XmlDocument xsd = new XmlDocument();
                    xsd.Load(file);

                    DataSet theData = new DataSet();
                    StringReader xmlSR = new StringReader(xsd.InnerXml);
                    theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                    DataTable tableInbound = theData.Tables["Cabecera"];
                    DataRow rowInbound = tableInbound.NewRow();
                    rowInbound["NRO_LINEA"] = "1";
                    rowInbound["OC"] = separator + selectDispatch.OutboundDetail.OutboundOrder.ReferenceNumber;
                    rowInbound["FEC_DESPACHO"] = txtDateAsn.Text.Trim();
                    rowInbound["HORA_DESPACHO"] = txtHourAsn.Text.Trim();
                    rowInbound["CANT_LPN"] = dispatchDetailTottusDTO.Entities.Select(dd => dd.Lpn.IdCode).Distinct().Count(); 
                    rowInbound["COLGADOS"] = "0";
                    rowInbound["BIGTICKET"] = "0";
                    rowInbound["PALLET_CONTENEDOR"] = "0" + separator + "0";
                    rowInbound["ALMACEN_DESPACHO"] = txtWarehouseASN.Text.Trim();

                    tableInbound.Rows.Add(rowInbound);

                    DataTable tableDet = theData.Tables["DETALLE"];

                    if (string.IsNullOrEmpty(selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                    {
                        throw new Exception(lblMissingBill.Text);
                    }

                    var distinctKindsOfItems = dispatchDetailTottusDTO.Entities.GroupBy(dd => dd.Item.Id).Select(group => group.First().Item).ToList();

                    foreach (Item item in distinctKindsOfItems)
                    {
                        if (string.IsNullOrEmpty(item.ItemCustomer.BarCode))
                        {
                            throw new Exception(lblDoesntExistItemCustomerMatch.Text + item.Code);
                        }

                        DataRow rowDet = tableDet.NewRow();
                        rowDet["NRO_LINEA"] = "2";
                        rowDet["COD_EAN"] = string.IsNullOrEmpty(item.ItemCustomer.BarCode) ? "" : item.ItemCustomer.BarCode;

                        var firstDispatchDetailByItem = dispatchDetailTottusDTO.Entities.Where(dd => dd.Item.Id == item.Id).First();
                        var branchCode = firstDispatchDetailByItem.OutboundDetail.OutboundOrder.Branch.Code;
                        var branchName = firstDispatchDetailByItem.OutboundDetail.OutboundOrder.Branch.Name;
                        var typeLpn = firstDispatchDetailByItem.Lpn.LPNType.Code;

                        rowDet["COD_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(branchCode) ? "" : branchCode;
                        rowDet["DESC_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(branchName) ? "" : branchName;
                        rowDet["CANTIDAD"] = dispatchDetailTottusDTO.Entities.Where(dd => dd.Item.Id == item.Id).Sum(dd => dd.Qty);
                        rowDet["TIPO_EMBALAJE"] = typeLpn;
                        rowDet["NRO_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;

                        tableDet.Rows.Add(rowDet);
                    }

                    DataTable tableFooter = theData.Tables["FOOTER"];
                    DataRow rowFooter = tableFooter.NewRow();
                    rowFooter["NRO_LINEA"] = "3";
                    rowFooter["NRO_DOC_DESPACHO"] = selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                    tableFooter.Rows.Add(rowFooter);

                    StringBuilder str = new StringBuilder();
                    foreach (DataTable t in theData.Tables)
                    {
                        foreach (DataRow row in t.Rows)
                        {
                            string[] myStringArray = convertArrayString(row);
                            //Elimina el Id el CABECERA_Id
                            myStringArray = myStringArray.Where((source, index) => index != (myStringArray.Count() - 1)).ToArray();

                            if (row[0].ToString() == "2")
                            {
                                myStringArray[myStringArray.ToList().Count() - 1] = separator + myStringArray[myStringArray.ToList().Count() - 1] + separator + separator + separator;
                            }
                            str.AppendLine(string.Join(separator, myStringArray));
                        }
                    }

                    modalPopUpASN.Hide();
                    upEditNew.Update();

                    string attachment = "attachment; filename=epir-" + selectDispatch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber + "-" + DateTime.Now.ToString("ddMMyy-hhmm") + ".txt";
                    Response.Clear();
                    Response.ContentType = "text/plain";
                    Response.AddHeader("content-disposition", attachment);

                    using (StreamWriter writer = new StreamWriter(Response.OutputStream))
                    {
                        writer.WriteLine(str);
                    }

                    Response.End();
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailTottusDTO.Errors);
                }
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

        private void CreateAsnParis(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailParisDTO = iDispatchingMGR.GetByIdDispatchASNCencosud("Cencosud", context);

                if (!dispatchDetailParisDTO.hasError())
                {
                    if (dispatchDetailParisDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    CreateAsnCencosud(selectDispatch, dispatchDetailParisDTO, CustomerB2B.PARIS);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailParisDTO.Errors);
                }
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

        private void CreateAsnJohnson(List<int> listIdDispatchDetail)
        {
            try
            {
                var selectDispatch = ((GenericViewDTO<DispatchDetail>)Session[WMSTekSessions.B2BAdministration.OrderDetail]).Entities.First();

                ClearFilter("listDispatch");
                CreateFilterByList("listDispatchDetail", listIdDispatchDetail);

                var dispatchDetailJohnsonDTO = iDispatchingMGR.GetByIdDispatchASNParis("Paris", context);
                if (!dispatchDetailJohnsonDTO.hasError())
                {
                    if (dispatchDetailJohnsonDTO.Entities.Count == 0)
                    {
                        ucStatus.ShowMessage(lblNoDispatchDetailAsn.Text);
                        return;
                    }

                    CreateAsnParisAndJohnson(selectDispatch, dispatchDetailJohnsonDTO, CustomerB2B.JOHNSON);
                }
                else
                {
                    this.Master.ucError.ShowError(dispatchDetailJohnsonDTO.Errors);
                }
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

        private void CreateAsnParisAndJohnson(DispatchDetail selectDispactch, GenericViewDTO<DispatchDetail> dispatchDetail, string customerName)
        {
            //Carga el archivo xsd del cliente
            string file = getPathAsn(selectDispactch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
            XmlDocument xsd = new XmlDocument();
            xsd.Load(file);

            DataSet theData = new DataSet();
            StringReader xmlSR = new StringReader(xsd.InnerXml);
            theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

            DataTable tableDet = theData.Tables["RECORD"];
            foreach (DispatchDetail disp in dispatchDetail.Entities)
            {
                if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                {
                    throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                }

                if (string.IsNullOrEmpty(selectDispactch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                {
                    throw new Exception(lblMissingBill.Text);
                }

                DataRow rowDet = tableDet.NewRow();
                rowDet["TIPO_EMBALAJE"] = "CAJAS";
                rowDet["NRO_BULTO"] = string.IsNullOrEmpty(disp.SealNumber) ? "" : disp.SealNumber;
                rowDet["CAMPO_FIJO"] = "";
                rowDet["DESC_ARTICULO"] = disp.Item.Description;
                rowDet["COD_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Code) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Code;
                rowDet["COD_DEPTO_COMPRA"] = string.IsNullOrEmpty(disp.Item.ItemCustomer.DepartmentItem) ? "" : disp.Item.ItemCustomer.DepartmentItem;
                rowDet["DESC_DEPTO_COMPRA"] = string.IsNullOrEmpty(disp.Item.ItemCustomer.DepartmentDescription) ? "" : disp.Item.ItemCustomer.DepartmentDescription;
                rowDet["NOMB_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Name) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Name;
                rowDet["SKU_CLIENTE"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                rowDet["SKU_COMPANIA"] = disp.Item.Code;
                rowDet["OC"] = selectDispactch.OutboundDetail.OutboundOrder.ReferenceNumber;
                rowDet["CANTIDAD"] = disp.Qty;
                rowDet["NRO_DOC_DESPACHO"] = string.IsNullOrEmpty(selectDispactch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber) ? "" : selectDispactch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                tableDet.Rows.Add(rowDet);
            }

            dsToExcelParisAndJohnson(theData, customerName);
        }
        private void CreateAsnCencosud(DispatchDetail selectDispactch, GenericViewDTO<DispatchDetail> dispatchDetail, string customerName)
        {
            string file = getPathAsn(selectDispactch.OutboundDetail.OutboundOrder.Customer.CustomerB2B.TemplateASNFile);
            XmlDocument xsd = new XmlDocument();
            xsd.Load(file);

            DataSet theData = new DataSet();
            StringReader xmlSR = new StringReader(xsd.InnerXml);
            theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

            DataTable tableDet = theData.Tables["RECORD"];
            foreach (DispatchDetail disp in dispatchDetail.Entities)
            {
                if (string.IsNullOrEmpty(disp.Item.ItemCustomer.ItemCodeCustomer))
                {
                    throw new Exception(lblDoesntExistItemCustomerMatch.Text + disp.Id.ToString());
                }

                if (string.IsNullOrEmpty(selectDispactch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber))
                {
                    throw new Exception(lblMissingBill.Text);
                }

                DataRow rowDet = tableDet.NewRow();

                rowDet["OC"] = selectDispactch.OutboundDetail.OutboundOrder.ReferenceNumber;
                rowDet["NRO_DOC_DESPACHO"] = string.IsNullOrEmpty(selectDispactch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber) ? "" : selectDispactch.OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber;
                rowDet["HU"] = "";
                rowDet["NOMB_SUCURSAL_DESTINO"] = string.IsNullOrEmpty(disp.OutboundDetail.OutboundOrder.Branch.Name) ? "" : disp.OutboundDetail.OutboundOrder.Branch.Name;
                rowDet["SKU_CLIENTE"] = disp.Item.ItemCustomer.ItemCodeCustomer;
                rowDet["CANTIDAD"] = disp.Qty;
                rowDet["LOTE"] = string.IsNullOrEmpty(disp.LotNumber) ? "" : disp.LotNumber;
                rowDet["FECHA_VENCIMIENTO"] = disp.Expiration == DateTime.MinValue ? "" : disp.Expiration.ToString("dd/MM/yyyy");
                rowDet["COSTO"] = Convert.ToInt32(disp.Price);

                tableDet.Rows.Add(rowDet);
            }

            dsToExcelCencosud(theData, "Cencosud");
        }
        private void dsToExcelABCDin(DataSet ds)
        {
            // Create the workbook
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["OC"].ToString();
                worksheet.Cell(i, 2).Value = dr["SUCURSAL_ORIGEN"].ToString();
                worksheet.Cell(i, 3).Value = dr["SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 4).Value = dr["NRO_BULTO"].ToString();
                worksheet.Cell(i, 4).DataType = XLCellValues.Text;
                worksheet.Cell(i, 5).Value = dr["SKU"].ToString();
                worksheet.Cell(i, 5).DataType = XLCellValues.Text;
                worksheet.Cell(i, 6).Value = dr["CANTIDAD"].ToString();
                worksheet.Cell(i, 7).Value = dr["NRO_DOC_DESPACHO"].ToString();
                worksheet.Cell(i, 8).Value = dr["TIPO_DOC"].ToString();

                i++;
            }


            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "ABCDinASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            Response.End();
        }

        private void dsToExcelCorona(DataSet ds)
        {
            // Create the workbook
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["OC"].ToString();
                worksheet.Cell(i, 2).Value = dr["SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 3).Value = dr["NRO_BULTO"].ToString();
                worksheet.Cell(i, 3).DataType = XLCellValues.Text;
                worksheet.Cell(i, 4).Value = dr["SKU"].ToString();
                worksheet.Cell(i, 5).Value = dr["CANTIDAD"].ToString();
                worksheet.Cell(i, 6).Value = dr["TIPO_DOC"].ToString();
                worksheet.Cell(i, 7).Value = dr["NRO_DOC_DESPACHO"].ToString();
                worksheet.Cell(i, 8).Value = dr["FECHA_DOC_DESPACHO"].ToString();

                i++;
            }

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "CoronaASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            Response.End();
        }

        private void dsToExcelEasy(DataSet ds)
        {
            // Create the workbook
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["OC"].ToString();
                worksheet.Cell(i, 2).Value = dr["SKU"].ToString();
                worksheet.Cell(i, 3).Value = dr["CANTIDAD"].ToString();

                i++;
            }

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "EasyASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            modalPopUpASN.Hide();
            upEditNew.Update();

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            Response.End();
        }

        private void dsToExcelHites(DataSet ds)
        {
            // Create the workbook
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["RUT_PROVEEDOR"].ToString();
                worksheet.Cell(i, 2).Value = dr["OC"].ToString();
                worksheet.Cell(i, 3).Value = dr["SUCURSAL_ORIGEN"].ToString();
                worksheet.Cell(i, 4).Value = dr["SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 5).Value = dr["NRO_BULTO"].ToString();
                worksheet.Cell(i, 5).DataType = XLCellValues.Text;
                worksheet.Cell(i, 6).Value = dr["SKU_CLIENTE"].ToString();
                worksheet.Cell(i, 6).DataType = XLCellValues.Text;
                worksheet.Cell(i, 7).Value = dr["SKU_COMPANIA"].ToString();
                worksheet.Cell(i, 8).Value = dr["CANTIDAD"].ToString();
                worksheet.Cell(i, 9).Value = dr["NRO_DOC_DESPACHO"].ToString();
                worksheet.Cell(i, 10).Value = dr["TIPO_DOC"].ToString();

                i++;
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string customerCodeHites = ListB2BCustomer().Where(x => x.Value == "HITES").First().Value;

            string fileName = "ASN_" + customerCodeHites.Substring(0, customerCodeHites.Length - 2) + "_" + DateTime.Now.ToString("dd_MM_yyyy");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xls");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            Response.End();
        }

        private void dsToExcelLaPolar(DataSet ds)
        {
            // Create the workbook
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["NRO_BULTO"].ToString();
                worksheet.Cell(i, 1).DataType = XLCellValues.Text;
                worksheet.Cell(i, 2).Value = dr["TIPO_EMBALAJE"].ToString();
                worksheet.Cell(i, 3).Value = dr["COD_SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 4).Value = dr["DESC_SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 5).Value = dr["OC"].ToString();
                worksheet.Cell(i, 6).Value = dr["TIPO_OC"].ToString();
                worksheet.Cell(i, 7).Value = dr["SKU_CLIENTE"].ToString();
                worksheet.Cell(i, 7).DataType = XLCellValues.Text;
                worksheet.Cell(i, 8).Value = dr["DESC_ARTICULO"].ToString();
                worksheet.Cell(i, 9).Value = dr["CANTIDAD"].ToString();

                i++;
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "EasyLaPolar-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            Response.End();
        }

        private void dsToXmlRipley(DataSet ds)
        {

            XElement root = new XElement("DOC_BULTOS_DEM_ODI");
            root.Add(new XElement("RUT", ds.Tables["CABECERA"].Rows[0]["RUT_PROVEEDOR"].ToString()));

            XElement invoiceGuide = new XElement("FACTURA_GUIA");
            root.Add(invoiceGuide);

            foreach (DataTable table in ds.Tables)
            {
                if (table.TableName == "CABECERA")
                {
                    foreach (DataRow row in table.Rows)
                    {
                        invoiceGuide.Add(new XElement("NUM_FAC_GUI", row["NRO_DOC_DESPACHO"].ToString()));
                        invoiceGuide.Add(new XElement("FACTURA_O_GUIA", row["DOC_ES_ELECTRONICO"].ToString()));
                        invoiceGuide.Add(new XElement("NUM_ODI", row["OC"].ToString()));
                    }
                }
                else if (table.TableName == "DETALLE")
                {
                    foreach (DataRow row in table.Rows)
                    {
                        XElement bundle = new XElement("BULTO_DEM_ODI",
                            new XElement("NUM_BULTO", row["NRO_BULTO"].ToString()),
                            new XElement("FECHA_BULTO", row["FEC_BULTO"].ToString())
                        );

                        XElement detailBundle = new XElement("DETALLE_BULTO_DEM_ODI",
                            new XElement("NUM_PROD_RIPLEY", row["SKU_CLIENTE"].ToString()),
                            new XElement("PROV_CASEPACK", row["SKU_COMPANIA"].ToString().Trim()),
                            new XElement("CANTIDAD", row["CANTIDAD"].ToString())
                        );

                        bundle.Add(detailBundle);
                        invoiceGuide.Add(bundle);
                    }
                }
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            string fileName = "RipleyASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xml");

            using (StreamWriter writer = new StreamWriter(Response.OutputStream))
            {
                string headerXml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\" standalone=\"no\"?>" + Environment.NewLine + "<!DOCTYPE DOC_BULTOS_DEM_ODI SYSTEM \"dtd/odi_asignada.dtd\" >" + Environment.NewLine;
                string finalXml = headerXml + root.ToString();
                writer.WriteLine(finalXml);
            }

            Response.End();
        }

        private void dsToExcelParisAndJohnson(DataSet ds, string customerName)
        {
            customerName = customerName.ToLower();
            string customerFinalName = char.ToUpper(customerName[0]) + customerName.Substring(1);
            // Create the workbook
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["TIPO_EMBALAJE"].ToString();
                worksheet.Cell(i, 2).Value = dr["NRO_BULTO"].ToString();
                worksheet.Cell(i, 3).Value = dr["CAMPO_FIJO"].ToString();
                worksheet.Cell(i, 4).Value = dr["DESC_ARTICULO"].ToString();
                worksheet.Cell(i, 5).Value = dr["COD_SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 6).Value = dr["COD_DEPTO_COMPRA"].ToString();
                worksheet.Cell(i, 7).Value = dr["DESC_DEPTO_COMPRA"].ToString();
                worksheet.Cell(i, 8).Value = dr["NOMB_SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 9).Value = dr["SKU_CLIENTE"].ToString();
                worksheet.Cell(i, 10).Value = dr["SKU_COMPANIA"].ToString();
                worksheet.Cell(i, 11).Value = dr["OC"].ToString();
                worksheet.Cell(i, 12).Value = dr["CANTIDAD"].ToString();
                worksheet.Cell(i, 13).Value = dr["NRO_DOC_DESPACHO"].ToString();

                i++;
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = customerFinalName + "ASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            Response.End();
        }
        private void dsToExcelCencosud(DataSet ds, string customerName)
        {
            customerName = customerName.ToLower();
            string customerFinalName = char.ToUpper(customerName[0]) + customerName.Substring(1);

            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ASN");

            int i = 1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                worksheet.Cell(i, 1).Value = dr["OC"].ToString();
                worksheet.Cell(i, 2).Value = dr["NRO_DOC_DESPACHO"].ToString();
                worksheet.Cell(i, 3).Value = dr["HU"].ToString();
                worksheet.Cell(i, 4).Value = dr["NOMB_SUCURSAL_DESTINO"].ToString();
                worksheet.Cell(i, 5).Value = dr["SKU_CLIENTE"].ToString();
                worksheet.Cell(i, 6).Value = dr["CANTIDAD"].ToString();
                worksheet.Cell(i, 7).Value = dr["LOTE"].ToString();
                worksheet.Cell(i, 8).Value = dr["FECHA_VENCIMIENTO"].ToString();
                worksheet.Cell(i, 9).Value = dr["COSTO"].ToString();

                i++;
            }

            modalPopUpASN.Hide();
            upEditNew.Update();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = customerFinalName + "ASN-" + DateTime.Now.ToString("ddMMyy-hhmm");
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");

            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream, false);
                memoryStream.WriteTo(Response.OutputStream);
            }

            Response.End();
        }
        private Dictionary<string, string> ListB2BCustomer()
        {
            const string KEY = "List_B2BCustomer";

            if (Session[KEY] == null)
            {
                var listB2BCustomer = new Dictionary<string, string>();
                listB2BCustomer.Add(GetConst("CodeCustomerASNABCDin")[0], CustomerB2B.ABCDIN);
                listB2BCustomer.Add(GetConst("CodeCustomerCorona")[0], CustomerB2B.CORONA);
                listB2BCustomer.Add(GetConst("CodeCustomerEasy")[0], CustomerB2B.EASY);
                listB2BCustomer.Add(GetConst("CodeCustomerASNFalabella")[0], CustomerB2B.FALABELLA);
                listB2BCustomer.Add(GetConst("CodeCustomerHites")[0], CustomerB2B.HITES);
                listB2BCustomer.Add(GetConst("CodeCustomerLaPolar")[0], CustomerB2B.LAPOLAR);
                listB2BCustomer.Add(GetConst("CodeCustomerParis")[0], CustomerB2B.PARIS);
                listB2BCustomer.Add(GetConst("CodeCustomerJohnson")[0], CustomerB2B.JOHNSON);
                listB2BCustomer.Add(GetConst("CodeCustomerRipley")[0], CustomerB2B.RIPLEY);
                listB2BCustomer.Add(GetConst("CodeCustomerASNSodimac")[0], CustomerB2B.SODIMAC);
                listB2BCustomer.Add(GetConst("CodeCustomerTottus")[0], CustomerB2B.TOTTUS);

                Session[KEY] = listB2BCustomer;
            }

            return (Dictionary<string, string>)Session[KEY];
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

        private string formatDate(string originalDate)
        {
            string day = originalDate.Substring(0, 2);
            string month = originalDate.Substring(3, 2);
            string year = originalDate.Substring(6, 4);

            return year + "-" + month + "-" + day;
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

        private KeyValuePair<string, string> ValidateCustomerFromOutboundOrder()
        {
            var selectDispatch = (DispatchSpecial)Session[WMSTekSessions.B2BAdministration.OrderSelected];

            var customerSelected = ListB2BCustomer().Where(custB2B => custB2B.Key.Equals(selectDispatch.OutboundOrder.CustomerCode)).FirstOrDefault();

            return customerSelected;
        }

        public void ShowAlert(string title, string message)
        {
            Encoding encod = Encoding.ASCII;
            string script = "ShowMessage('" + title + "','" + message.Replace("'", "") + "');";
            script = script.Replace("\\", Convert.ToChar(47).ToString());
            //string script = "setTimeout(ShowMessage('" + title + "','" + message.Replace("'","") + "'),2000);";
            ScriptManager.RegisterStartupScript(Page, GetType(), "Menssage", script, true);
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('B2BAdministrationDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, true);", true);  
        }
        #endregion       
    }
}