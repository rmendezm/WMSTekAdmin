using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Tasks;
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
    public partial class ActivateReplacementOnDemand : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<TaskConsult> taskViewDTO = new GenericViewDTO<TaskConsult>();
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

        #region "Eventos"
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
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
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
                    }
                }
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
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
                        //No haga postback en ciertas columnas
                        if (i == GetColumnIndexByName(e.Row, "Actions"))
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
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
                currentIndex = -1;
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }

        protected void btnActivate_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var listTasksSelected = new List<TaskConsult>();
                taskViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TaskMgr.TaskListToActivate];

                for (int i = 0; i < grdMgr.Rows.Count; i++)
                {
                    var row = grdMgr.Rows[i];
                    var chkSelectTask = (CheckBox)row.FindControl("chkSelectTask");

                    if (chkSelectTask != null && chkSelectTask.Checked)
                    {
                        var lblIdTask = (Label)row.FindControl("lblIdTask");
                        listTasksSelected.Add(taskViewDTO.Entities.Where(t => t.Id == int.Parse(lblIdTask.Text.Trim())).First());
                    }
                }

                if (listTasksSelected.Count == 0)
                {
                    isValidViewDTO = false;
                    taskViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Task.GenerateTask.NotTaskSelected, context)); 
                    this.Master.ucError.ShowError(taskViewDTO.Errors);
                }
                else
                {
                    taskViewDTO = iTasksMGR.ActivateTasks(listTasksSelected, context);

                    if (taskViewDTO.hasError())
                    {
                        isValidViewDTO = false;
                        taskViewDTO.Errors = baseControl.handleError(new ErrorDTO(taskViewDTO.Errors.Code, context));
                        this.Master.ucError.ShowError(taskViewDTO.Errors);
                    }
                    else
                    {
                        ReloadData();
                        ucStatus.ShowMessage(taskViewDTO.MessageStatus.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
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
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
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
                taskViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }
        }
        #endregion

        #region "Metodos"

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

            Master.ucTaskBar.btnRefreshVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.warehouseVisible = true;
            //this.Master.ucMainFilter.warehouseNotIncludeAll = true;//Centro Obligatorio
            this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.ownerNotIncludeAll = false;//Owner NO Obligatorio
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;

            this.Master.ucMainFilter.codeLabel = lblFilterCode.Text;
            this.Master.ucMainFilter.descriptionLabel = lblFilterDescription.Text;

            this.Master.ucMainFilter.advancedFilterVisible = true;
            this.Master.ucMainFilter.tabLocationVisible = true;
            this.Master.ucMainFilter.locationFilterVisible = true;

            this.Master.ucMainFilter.LocationRangeVisible = false;
            this.Master.ucMainFilter.LocationRowColumnEqualVisible = false;
            this.Master.ucMainFilter.LocationEqualVisible = false;
            this.Master.ucMainFilter.LocationAisleEqualVisible = false;
            this.Master.ucMainFilter.LocationLevelEqualVisible = false;

            this.Master.ucMainFilter.InitializeFilterLocWhs();

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
            taskViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(taskViewDTO.MessageStatus.Message);
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
                this.Master.ucError.ShowError(taskViewDTO.Errors);
                taskViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            taskViewDTO = iTasksMGR.FindAllTasksToActivate(context);

            if (!taskViewDTO.hasError() && taskViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TaskMgr.TaskListToActivate, taskViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(taskViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(taskViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            taskViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.TaskMgr.TaskListToActivate];

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!countryViewDTO.hasConfigurationError() && countryViewDTO.Configuration != null && countryViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, countryViewDTO.Configuration);

            grdMgr.DataSource = taskViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(taskViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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