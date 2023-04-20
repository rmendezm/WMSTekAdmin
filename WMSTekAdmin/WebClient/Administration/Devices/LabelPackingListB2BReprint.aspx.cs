using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Label;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class LabelPackingListB2BReprint : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<TaskLabel> taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                    }
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                        if (i == GetColumnIndexByName(e.Row, "chkSelectLabel"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
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
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var listSelected = GetAllRowsSelected();

                if (listSelected.Count > 0)
                {
                    ClearFilter("listSelected");
                    CreateFilterByList("listSelected", listSelected.Select(tl => tl.Id).ToList());

                    taskLabelViewDTO = iLabelMGR.UpdateTaskLabelsToReprint(context);

                    if (taskLabelViewDTO.hasError())
                    {
                        this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
                    }
                    else
                    {
                        UpdateSession();
                        ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
                    }
                }
                else
                {
                    this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblNoSelected.Text });
                }
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskLabelViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "LabelPackingListB2BReprint";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                
            }
        }

        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.BtnPrintClick += new EventHandler(btnPrint_Click);
            this.Master.ucTaskBar.btnPrintVisible = true;
            this.Master.ucTaskBar.btnPrintEnabled = false;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.DocumentNumberLabel = lblDocName.Text;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.nameLabel = lblFilterCustomer.Text;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = lblFilterLabelCode.Text; 
            this.Master.ucMainFilter.codeAltVisible = true;
            this.Master.ucMainFilter.codeLabelAlt = lblFilterItem.Text;
            this.Master.ucMainFilter.impressionTailVisible = true;
            this.Master.ucMainFilter.descriptionVisible = true;
            this.Master.ucMainFilter.descriptionLabel = lblFilterLpn.Text;

            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;

            this.Master.ucMainFilter.chkDateFromEnabled = false;
            this.Master.ucMainFilter.chkDateToEnabled = false;

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
        }

        private void UpdateSession()
        {
            RadioButtonList rblFind = (RadioButtonList)this.Master.ucMainFilter.FindControl("rblSearchCriteriaFind");
            eTypeLabelInPrintQueue selectedValue = (eTypeLabelInPrintQueue)(int.Parse(rblFind.SelectedValue));

            TextBox txtLpnCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterDescription");

            if (selectedValue != eTypeLabelInPrintQueue.Price && string.IsNullOrEmpty(txtLpnCode.Text))
            {
                this.Master.ucError.ShowError(new ErrorDTO() { Level = ErrorLevel.Warning, Title = "Advertencia", Message = lblValidateExistsFilterLpn.Text });
                return;
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            taskLabelViewDTO = iLabelMGR.GetTasksToReprint(selectedValue, "LabelPackingListB2BReprint_FindAll", context);

            if (!taskLabelViewDTO.hasError() && taskLabelViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LabelPackingListB2BReprint.TaskLabelList, taskLabelViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
            }
            else
            {
                if (taskLabelViewDTO.Errors.Code != WMSTekError.DataBase.NoRowsReturned)
                {
                    this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
                }
            }
        }

        protected void ReloadData()
        {
            UpdateSession();

            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;

                if (taskLabelViewDTO.Entities.Count > 0)
                {
                    //divPrintLabel.Visible = true;
                    //this.txtQtycopies.Text = "1";
                    Master.ucTaskBar.btnPrintEnabled = true;
                }
                else
                {
                    //divPrintLabel.Visible = false;
                    Master.ucTaskBar.btnPrintEnabled = false;
                }
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!taskLabelViewDTO.hasConfigurationError() && taskLabelViewDTO.Configuration != null && taskLabelViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, taskLabelViewDTO.Configuration);

            grdMgr.DataSource = taskLabelViewDTO.Entities;
            grdMgr.DataBind();

            CallJsGridViewHeader();

            ucStatus.ShowRecordInfo(taskLabelViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private List<TaskLabel> GetAllRowsSelected()
        {
            var listSelected = new List<TaskLabel>();
            taskLabelViewDTO = (GenericViewDTO<TaskLabel>)Session[WMSTekSessions.LabelPackingListB2BReprint.TaskLabelList];

            for (int i = 0; i < grdMgr.Rows.Count; i++)
            {
                var row = grdMgr.Rows[i];
                var chkSelect = (CheckBox)row.FindControl("chkSelectLabel");
                var lblIdTaskLabel = (Label)row.FindControl("lblIdTaskLabel");

                if (chkSelect.Checked)
                {
                    listSelected.Add(taskLabelViewDTO.Entities.Where(d => d.Id == int.Parse(lblIdTaskLabel.Text.Trim())).First());
                }
            }

            return listSelected;
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "initializeGridDragAndDrop('LabelPackingListB2BReprint_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }

        #endregion
    }
}