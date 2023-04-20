using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Data;

namespace Binaria.WMSTek.WebClient.Outbound.Consult
{
    public partial class IssuesOnOrder : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<OutboundOrder> outboundOrderViewDTO = new GenericViewDTO<OutboundOrder>();
        private GenericViewDTO<Issue> issueViewDTO = new GenericViewDTO<Issue>();
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
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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

                Page.Form.Attributes.Add("enctype", "multipart/form-data");
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "resizeDiv();", true);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdMgr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;
                currentIndexToLoadDetail = index;
                currentIndex = grdMgr.SelectedIndex;

                LoadDetail(index);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void btnOpenPopUp_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int deleteIndex = grdDetail.PageSize * grdDetail.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdDetail_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int editIndex = grdDetail.PageSize * grdDetail.PageIndex + e.NewEditIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void lkUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                OpenUploadImagePopUp();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void btnSaveUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (uploadImage.HasFile)
                    UploadFile(uploadImage.FileName);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal("Error", lblErrorUploadingImage.Text);
            }
        }
        protected void btnCancelUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                OpenIssuePopUp();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void lkGetImages_Click(object sender, EventArgs e)
        {
            try
            {
                var issueSelected = (Issue)Session[WMSTekSessions.IssuesOnOrder.DetailSelected];

                if (issueSelected != null)
                {
                    var issueImageViewDTO = iDispatchingMGR.GetIssueImageByIssue(issueSelected.Id, context);

                    if (!issueImageViewDTO.hasError())
                    {
                        if (issueImageViewDTO.Entities.Count > 0)
                        {
                            Session.Add(WMSTekSessions.IssuesOnOrder.Images, issueImageViewDTO.Entities);
                            OpenGetImagePopUp();
                        }
                        else
                        {
                            Session.Remove(WMSTekSessions.IssuesOnOrder.Images);
                            ucStatus.ShowMessage(lblNoImagesFound.Text);
                        }

                        grdImages.DataSource = issueImageViewDTO.Entities;
                        grdImages.DataBind();
                        upGetImages.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdImages_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int deleteIndex = grdImages.PageSize * grdImages.PageIndex + e.RowIndex;
                DeleteImage(deleteIndex);
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void grdImages_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var btnDeleteImage = e.Row.FindControl("btnDeleteImage") as ImageButton;

                    if (btnDeleteImage != null)
                        btnDeleteImage.OnClientClick = "if(confirm('" + lblConfirmDeleteImage.Text + "')==false){return false;}";
                }
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        protected void btnCancelGetImages_Click(object sender, EventArgs e)
        {
            try
            {
                OpenIssuePopUp();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }

        protected void btnSubir_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile.HasFile)
                {
                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(ex.Message);
                    }

                    DataTable dataTable;
                    try
                    {
                        dataTable = ConvertXlsToDataTable(savePath, 1);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }


                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       whsCode = r.Field<object>("whsCode"),
                                       ownCode = r.Field<object>("ownCode"),
                                       documentNumber = r.Field<object>("documentNumber"),
                                       documentType = r.Field<object>("documentType"),
                                       issue = r.Field<object>("issue"),
                                   };

                    var issuesToCreate = new GenericViewDTO<Issue>();

                    try
                    {
                        foreach (var issueToRegister in lstExcel)
                        {
                            Issue newIssue = new Issue();

                            if (!ValidateIsNotNull(issueToRegister.whsCode))
                            {
                                errorUp = "Warehouse " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newIssue.Warehouse = new Warehouse();
                                newIssue.Warehouse.Code = issueToRegister.whsCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(issueToRegister.ownCode))
                            {
                                errorUp = "Owner " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newIssue.Owner = new Owner();
                                newIssue.Owner.Code = issueToRegister.ownCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(issueToRegister.documentNumber))
                            {
                                errorUp = "DocumentNumber " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newIssue.DocumentNumber = issueToRegister.documentNumber.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(issueToRegister.documentType))
                            {
                                errorUp = "DocumentType " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newIssue.DocumentTypeCode = issueToRegister.documentType.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(issueToRegister.issue))
                            {
                                errorUp = "Issue " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newIssue.MessageIssue = issueToRegister.issue.ToString().Trim();
                            }

                            issuesToCreate.Entities.Add(newIssue);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (issuesToCreate.Entities.Count > 0)
                        {
                            if (issuesToCreate.Entities.Count > 200)
                            {
                                ShowAlertLocal(this.lblTitle.Text, this.lblTooManyRows.Text);
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                var issuesViewDTO = iDispatchingMGR.MaintainMassiveIssues(issuesToCreate, context);

                                if (issuesViewDTO.hasError())
                                {
                                    //UpdateSession(true);
                                    ShowAlertLocal(this.lblTitle.Text, issuesViewDTO.Errors.Message);
                                    divLoad.Visible = true;
                                    modalPopUpLoad.Show();
                                }
                                else
                                {
                                    ucStatus.ShowMessage(issuesViewDTO.MessageStatus.Message);
                                    ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);
                                    divLoad.Visible = false;
                                    modalPopUpLoad.Hide();

                                    UpdateSession();
                                    if (currentIndexToLoadDetail >= 0)
                                        LoadDetail(currentIndexToLoadDetail);
                                }
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }
                }
                else
                {
                    ShowAlertLocal(this.lblTitle.Text, this.lblNotFileLoad.Text);
                    divLoad.Visible = true;
                    modalPopUpLoad.Show();
                }
            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, outboundOrderViewDTO.Errors.Message);
            }
            finally
            {
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }

                //UpdateSession(false);
            }
        }
        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            try
            {
                OpenLoadPopUp();
            }
            catch (Exception ex)
            {
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"
        protected void Initialize()
        {
            context.SessionInfo.IdPage = "IssuesOnOrder";

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
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);
            Master.ucTaskBar.btnAddVisible = true;
        }
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.documentVisible = true;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

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
            grdImages.EmptyDataText = this.Master.EmptyGridText;
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
                outboundOrderViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
            }
        }
        private void UpdateSession()
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            outboundOrderViewDTO = iDispatchingMGR.OrdersAndIssues(context);

            if (!outboundOrderViewDTO.hasError() && outboundOrderViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.IssuesOnOrder.List, outboundOrderViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(outboundOrderViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucError.ShowError(outboundOrderViewDTO.Errors);
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
        protected void LoadDetail(int index)
        {
            if (index != -1)
            {
                outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.IssuesOnOrder.List];

                var orderSelected = outboundOrderViewDTO.Entities[index];

                issueViewDTO = iDispatchingMGR.IssueGetByOrderId(orderSelected.Id, orderSelected.OutboundType.Code, context);

                if (issueViewDTO != null && issueViewDTO.Entities.Count > 0)
                {
                    if (!issueViewDTO.hasConfigurationError() && issueViewDTO.Configuration != null && issueViewDTO.Configuration.Count > 0)
                        base.ConfigureGridOrder(grdDetail, issueViewDTO.Configuration);

                    grdDetail.DataSource = issueViewDTO.Entities;
                    grdDetail.DataBind();
                }

                CallJsGridViewDetail();
                lblNroDoc.Text = orderSelected.Number;

                Session.Add(WMSTekSessions.IssuesOnOrder.Selected, orderSelected);
                Session.Add(WMSTekSessions.IssuesOnOrder.Detail, issueViewDTO);
                divDetail.Visible = true;
            }
            else
            {
                divDetail.Visible = false;
            }
        }
        private void PopulateGrid()
        {
            outboundOrderViewDTO = (GenericViewDTO<OutboundOrder>)Session[WMSTekSessions.IssuesOnOrder.List];

            if (outboundOrderViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            if (!outboundOrderViewDTO.hasConfigurationError() && outboundOrderViewDTO.Configuration != null && outboundOrderViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, outboundOrderViewDTO.Configuration);

            grdMgr.DataSource = outboundOrderViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(outboundOrderViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

            CallJsGridViewHeader();
            upGrid.Update();
        }
        private void PopulateGridDetail()
        {
            if (divDetail.Visible)
            {
                issueViewDTO = (GenericViewDTO<Issue>)Session[WMSTekSessions.IssuesOnOrder.Detail];

                if (!issueViewDTO.hasConfigurationError() && issueViewDTO.Configuration != null && issueViewDTO.Configuration.Count > 0)
                    base.ConfigureGridOrder(grdDetail, issueViewDTO.Configuration);

                grdDetail.DataSource = issueViewDTO.Entities;
                grdDetail.DataBind();

                CallJsGridViewDetail();
                upGridDetail.Update();
            }
        }
        private void DeleteRow(int index)
        {
            issueViewDTO = (GenericViewDTO<Issue>)Session[WMSTekSessions.IssuesOnOrder.Detail];
            var issueSelected = issueViewDTO.Entities[index];

            var issueImageViewDTO = iDispatchingMGR.GetIssueImageByIssue(issueSelected.Id, context);

            if (!issueImageViewDTO.hasError())
            {
                if (issueImageViewDTO.Entities.Count > 0)
                    DeleteImages(issueImageViewDTO.Entities);

                issueViewDTO = iDispatchingMGR.MaintainIssue(CRUD.Delete, issueSelected, context);

                if (issueViewDTO.hasError())
                {
                    this.Master.ucError.ShowError(issueViewDTO.Errors);
                    //UpdateSession();
                    LoadDetail(currentIndexToLoadDetail);
                }
                else
                {
                    crud = true;
                    ucStatus.ShowMessage(issueViewDTO.MessageStatus.Message);
                    //UpdateSession();
                    LoadDetail(currentIndexToLoadDetail);
                }
            }
            else
            {
                this.Master.ucError.ShowError(issueImageViewDTO.Errors);
                LoadDetail(currentIndexToLoadDetail);
            }
        }
        private void DeleteImages(List<IssueImage> images)
        {
            if (images != null && images.Count > 0)
            {
                var imagesDirectory = Path.GetDirectoryName(Server.MapPath(images.First().ImageIssueUrl));

                foreach (var image in images)
                {
                    var imagePath = Server.MapPath(image.ImageIssueUrl);

                    if (File.Exists(imagePath))
                        File.Delete(imagePath);
                }

                //Directory.Delete(imagesDirectory, true);
            }
        }
        protected void ShowModal(int index, CRUD mode)
        {
            if (mode == CRUD.Update)
            {
                issueViewDTO = (GenericViewDTO<Issue>)Session[WMSTekSessions.IssuesOnOrder.Detail];

                var issueSelected = issueViewDTO.Entities[index];
                Session.Add(WMSTekSessions.IssuesOnOrder.DetailSelected, issueSelected);

                txtMessageIssue.Text = issueSelected.MessageIssue;
                hidEditId.Value = issueSelected.Id.ToString();

                lblNew.Visible = false;
                lblEdit.Visible = true;
                divUploadImages.Visible = true;
                ShowOrHideGetImages(issueSelected.Id);
            }
            else if (mode == CRUD.Create)
            {
                lblNew.Visible = true;
                lblEdit.Visible = false;
                divUploadImages.Visible = false;
                divGetImages.Visible = false;

                ClearPopUpForm();
            }

            var orderSelected = (OutboundOrder)Session[WMSTekSessions.IssuesOnOrder.Selected];
            headerTitle.Text = "Orden " + orderSelected.Number;

            OpenIssuePopUp();
        }
        private void ShowOrHideGetImages(long idIssue)
        {
            var hasImages = false;

            var issueImageViewDTO = iDispatchingMGR.GetIssueImageByIssue(idIssue, context);
            
            if (!issueImageViewDTO.hasError() && issueImageViewDTO.Entities.Count > 0)
                hasImages = true;

            divGetImages.Visible = hasImages;
            //upEditNew.Update();
        }
        protected void SaveChanges()
        {
            var orderSelected = (OutboundOrder)Session[WMSTekSessions.IssuesOnOrder.Selected];

            var issue = new Issue();
            issue.Owner = new Owner(orderSelected.Owner.Id);
            issue.Warehouse = new Warehouse(orderSelected.Warehouse.Id);
            issue.IdDocumentBound = orderSelected.Id;
            
            issue.DocumentTypeCode = orderSelected.OutboundType.Code;

            var message = txtMessageIssue.Text.Trim();

            int messageMaxLenght = 500;
            if (message.Length > messageMaxLenght)
                issue.MessageIssue = message.Substring(0, messageMaxLenght);
            else
                issue.MessageIssue = message;

            if (hidEditId.Value == "0")
            {
                issue.DateIssue = DateTime.Now;

                issueViewDTO = iDispatchingMGR.MaintainIssue(CRUD.Create, issue, context);
            }
            else
            {
                issue.Id = int.Parse(hidEditId.Value);
                issueViewDTO = iDispatchingMGR.MaintainIssue(CRUD.Update, issue, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (issueViewDTO.hasError())
            {
                this.Master.ucError.ShowError(issueViewDTO.Errors);
                //UpdateSession();
                LoadDetail(currentIndexToLoadDetail);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(issueViewDTO.MessageStatus.Message);
                //UpdateSession();
                LoadDetail(currentIndexToLoadDetail);
            }
        }
        private void UploadFile(string fileName)
        {
            bool hasError = false;
            double maxFileSize = 5.0;
            double fileSizeMB = (uploadImage.PostedFile.ContentLength / 1024) / 1024.0;

            if (fileSizeMB > maxFileSize)
            {
                var error = lblValidateSizeImage.Text + maxFileSize.ToString() + " MB";
                ucStatus.ShowError(error); 
            }
            else
            {
                var orderSelected = (OutboundOrder)Session[WMSTekSessions.IssuesOnOrder.Selected];
                var issueSelected = (Issue)Session[WMSTekSessions.IssuesOnOrder.DetailSelected];

                if (orderSelected != null && issueSelected != null)
                {
                    var upLoadImagesOnOrderPath = MiscUtils.ReadSetting("UpLoadImagesOnOrderPath", "");
                    var rootPath = Request.PhysicalApplicationPath + @"WebResources\" + upLoadImagesOnOrderPath + @"\";
                    var folderPath = DeleteForbiddenCharactersToDirectoryName(orderSelected.Owner.Code + orderSelected.Warehouse.Code + orderSelected.OutboundType.Code + orderSelected.Number);
                    var finalFolderPath = Path.Combine(rootPath, folderPath);

                    var imageName = Path.GetFileNameWithoutExtension(fileName);
                    var imageExt = Path.GetExtension(fileName);

                    int maxCharactersInFilename = 50;

                    if (imageName.Length > maxCharactersInFilename)
                        imageName = imageName.Substring(0, maxCharactersInFilename);

                    var finalFileName = imageName + "-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + imageExt;

                    var finalPath = Path.Combine(finalFolderPath, finalFileName);

                    if (!Directory.Exists(finalFolderPath))
                    {
                        var securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                        Directory.CreateDirectory(finalFolderPath, securityRules);
                    }

                    uploadImage.SaveAs(finalPath);

                    var virtualPath = "~/WebResources/" + upLoadImagesOnOrderPath + "/" + folderPath + "/" + finalFileName;

                    var newImage = new IssueImage()
                    {
                        Issue = new Issue() { Id = issueSelected.Id },
                        ImageIssueUrl = virtualPath
                    };
                    var issueImageViewDTO = iDispatchingMGR.MaintainIssueImage(CRUD.Create, newImage, context);

                    if (!issueImageViewDTO.hasError())
                        ucStatus.ShowMessage(lblFileUploadSuccessfully.Text);
                    else
                    {
                        hasError = true;
                        ucStatus.ShowError(issueImageViewDTO.Errors.Message);
                    }
                }
            }

            Initialize();

            UpdateSession();

            CloseUploadImagePopUp();
            OpenIssuePopUp();
            divGetImages.Visible = !hasError;

            //UpdateSession();
            //upGrid.Update();
            //LoadDetail(currentIndexToLoadDetail);
        }

        private string DeleteForbiddenCharactersToDirectoryName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
        private void DeleteImage(int index)
        {
            var issueSelected = (Issue)Session[WMSTekSessions.IssuesOnOrder.DetailSelected];
            var images = (List<IssueImage>)Session[WMSTekSessions.IssuesOnOrder.Images];

            if (issueSelected != null && images != null && images.Count > 0)
            {
                var imageSelected = images[index];

                var filePath = Server.MapPath(imageSelected.ImageIssueUrl);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                var deleteImageViewDTO = iDispatchingMGR.MaintainIssueImage(CRUD.Delete, imageSelected, context);

                if (deleteImageViewDTO.hasError())
                {
                    ucStatus.ShowError(deleteImageViewDTO.Errors.Message);
                }
                else
                {
                    var issueImageViewDTO = iDispatchingMGR.GetIssueImageByIssue(issueSelected.Id, context);

                    if (!issueImageViewDTO.hasError())
                    {
                        if (issueImageViewDTO.Entities.Count == 0)
                        {
                            Session.Remove(WMSTekSessions.IssuesOnOrder.Images);

                            ucStatus.ShowMessage(lblNoImagesFound.Text);
                            OpenIssuePopUp();
                            divGetImages.Visible = false;
                        }
                        else
                        {
                            Session.Add(WMSTekSessions.IssuesOnOrder.Images, issueImageViewDTO.Entities);
                            OpenGetImagePopUp();
                        }
                    }
                }
            }
        }
        private void OpenGetImagePopUp()
        {
            divGetImagesPopUp.Visible = true;
            upGetImages.Update();
            GetImagesPopUp.Show();
        }
        private void OpenIssuePopUp()
        {
            divEditNew.Visible = true;
            upEditNew.Update();
            modalPopUp.Show();
        }
        private void OpenUploadImagePopUp()
        {
            divUploadImagePopUp.Visible = true;
            upUploadImage.Update();
            UploadImagePopUp.Show();
        }
        private void CloseUploadImagePopUp()
        {
            divUploadImagePopUp.Visible = false;
            upUploadImage.Update();
            UploadImagePopUp.Hide();
        }
        private void ClearPopUpForm()
        {
            txtMessageIssue.Text = string.Empty;
            hidEditId.Value = "0";
        }
        private void OpenLoadPopUp()
        {
            divLoad.Visible = true;
            upLoad.Update();
            modalPopUpLoad.Show();
        }
        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDropCustom();", true);
        }

        private void CallJsGridViewDetail()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridDetail", "initializeGridDragAndDrop('IssuesOnOrderDetail', 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail', undefined, false);", true);
        }

        #endregion
    }
}