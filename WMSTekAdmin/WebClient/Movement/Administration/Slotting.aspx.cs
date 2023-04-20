using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Movement.Administration
{
    public partial class Slotting : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskConsult> tasksConsultViewDTO = new GenericViewDTO<TaskConsult>();
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
        #endregion

        #region Events

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
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

                VisibilityBtnProcess();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
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
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
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
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
                        {
                            continue;
                        }

                        // Agrega atributos para cambiar el color de la fila seleccionada
                        e.Row.Cells[i].Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                        e.Row.Cells[i].Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    }
                }
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateTasks();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
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
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                PopulateGrid();
                CallJsGridView();
            }
            catch (Exception ex)
            {
                tasksConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        private void GenerateTasks()
        {
            tasksConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.Slotting.List];

            if (tasksConsultViewDTO != null && tasksConsultViewDTO.Entities.Count > 0)
            {
                var resultViewDTO = iWarehousingMGR.SlottingTransaction(tasksConsultViewDTO.Entities, context);

                if (resultViewDTO.hasError())
                    this.Master.ucError.ShowError(resultViewDTO.Errors);
                else
                {
                    tasksConsultViewDTO.Entities = new List<TaskConsult>();
                    ucStatus.ShowMessage(resultViewDTO.MessageStatus.Message);
                    isValidViewDTO = true;
                }

                CallJsGridView();
            }
            else
            {
                ucStatus.ShowWarning(lblWarningMsg.Text);
            }
        }

        protected void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            //Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;

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

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            tasksConsultViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(tasksConsultViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
                tasksConsultViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            var validateSlottingNotFinished = iTasksMGR.ValidateSlottingNotFinished(this.Master.ucMainFilter.idWhs, context);

            if (!validateSlottingNotFinished.hasError())
            {
                if (validateSlottingNotFinished.Entities.Count > 0)
                {
                    ucStatus.ShowWarning(lblValidateSlottingNotFinished.Text);
                }
                else
                {
                    tasksConsultViewDTO = iWarehousingMGR.Slotting(this.Master.ucMainFilter.idWhs, context);

                    if (!tasksConsultViewDTO.hasError() && tasksConsultViewDTO.Entities != null)
                    {
                        Session.Add(WMSTekSessions.Slotting.List, tasksConsultViewDTO);
                        isValidViewDTO = true;

                        //Muestra Mensaje en barra de status
                        if (!crud)
                            ucStatus.ShowMessage(tasksConsultViewDTO.MessageStatus.Message);
                    }
                    else
                    {
                        isValidViewDTO = false;
                        this.Master.ucError.ShowError(tasksConsultViewDTO.Errors);
                    }
                }
            }
            else
            {
                this.Master.ucError.ShowError(validateSlottingNotFinished.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            tasksConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.Slotting.List];

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!tasksConsultViewDTO.hasConfigurationError() && tasksConsultViewDTO.Configuration != null && tasksConsultViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, tasksConsultViewDTO.Configuration);

            grdMgr.DataSource = tasksConsultViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(tasksConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void VisibilityBtnProcess()
        {
            tasksConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.Slotting.List];

            if (tasksConsultViewDTO != null && tasksConsultViewDTO.Entities.Count > 0)
                btnProcess.Visible = true;
            else
                btnProcess.Visible = false;
        }

        protected void ReloadData()
        {
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentIndex = -1;
                currentPage = 0;
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }

        #endregion
    }
}