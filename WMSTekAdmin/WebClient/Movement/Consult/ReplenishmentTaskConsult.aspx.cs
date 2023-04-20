using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Tasks;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Movement
{
    public partial class ReplenishmentTaskConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<TaskConsult> taskRepConsultViewDTO = new GenericViewDTO<TaskConsult>();
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
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
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

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Actualiza la vista actual, cargando nuevamente las variables de sesion desde base de datos. 
        /// </summary>
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
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
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
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
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
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
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
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
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
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
                taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        // TODO: Implementar en Fase 3
        //protected void ucStatus_pageSizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        grdMgr.PageSize = ucStatus.PageSize;
        //        PopulateGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        taskRepConsultViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
        //    }
        //}
        #endregion

        #region "Métodos"


        protected void Initialize()
        {
            context.SessionInfo.IdPage = "ReplenishmentConsult";

            InitializeFilter(!Page.IsPostBack, false);
            InitializeTaskBar();
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.MovementConsult.ReplenishmentList))
                {
                    taskRepConsultViewDTO = (GenericViewDTO<TaskConsult>)Session[WMSTekSessions.MovementConsult.ReplenishmentList];
                    isValidViewDTO = true;
                }
            }
        }
        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.itemVisible = true;
            this.Master.ucMainFilter.statusVisible = true;
            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;
            this.Master.ucMainFilter.statusLabel = this.lblFilterStatus.Text;

            this.Master.ucMainFilter.trackTaskTypeCode = new string[] { }; 
            this.Master.ucMainFilter.trackTaskTypeVisible = true;
            this.Master.ucMainFilter.trackTaskTypeNotIncludeAll = false;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.TaskReplenishmentAfter;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.TaskReplenishmentBefore;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Estado
        /// </summary>
        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            // ucStatus.pageSizeChanged += new EventHandler(ucStatus_pageSizeChanged);
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
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // Carga consulta de Tareas de Reposición
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            taskRepConsultViewDTO = iTasksMGR.FindAllReplenishmentConsult(context);

            if (!taskRepConsultViewDTO.hasError() && taskRepConsultViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.MovementConsult.ReplenishmentList, taskRepConsultViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(taskRepConsultViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(taskRepConsultViewDTO.Errors);
            }
        }
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!taskRepConsultViewDTO.hasConfigurationError() && taskRepConsultViewDTO.Configuration != null && taskRepConsultViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, taskRepConsultViewDTO.Configuration);

            grdMgr.DataSource = taskRepConsultViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(taskRepConsultViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            crud = false;
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        #endregion
    }
}