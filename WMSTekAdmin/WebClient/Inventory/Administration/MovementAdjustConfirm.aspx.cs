using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;

namespace Binaria.WMSTek.WebClient.Inventory.Administration
{
    public partial class MovementAdjustConfirm : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<MovementAdjust> MovementAdjustViewDTO = new GenericViewDTO<MovementAdjust>();
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    CheckBox chkAdjustConfirm = e.Row.FindControl("chkAdjustConfirm") as CheckBox;

                    if (chkAdjustConfirm != null && MovementAdjustViewDTO.Entities[e.Row.DataItemIndex].StateInterface == "A")
                    {
                        chkAdjustConfirm.Enabled = true;
                    }
                    else if (chkAdjustConfirm != null)
                    {
                        chkAdjustConfirm.Enabled = false;
                    }

                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    //// Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    // Permite mostrar el detalle de la fila seleccionada al hacer click sobre la misma

                    // Asinga el atributo 'onclick' a todas las columnas de la grilla, excepto a la que contiene los checkboxes
                    // IMPORTANTE: no cambiar de lugar la columna [0] que contiene los checkboxes
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (i == GetColumnIndexByName(e.Row, "Actions") || i == GetColumnIndexByName(e.Row, "Checkbox"))
                        {
                            continue;
                        }

                        e.Row.Cells[i].Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdMgr, "Select$" + e.Row.RowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = false;

                foreach (GridViewRow row in grdMgr.Rows)
                {
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)row.Cells[0].FindControl("chkAdjustConfirm");

                    if (chk.Checked == true)
                    {
                        isValid = true;
                    }
                }

                if (isValid)
                {
                    this.ShowConfirm(this.lblConfirmAdjustHeader.Text, lblConfirmAdjust.Text);
                }
                else
                {
                    this.Master.ucDialog.ShowAlert(this.lblConfirmAdjustHeader.Text, this.lblNotSelectedAdjust.Text, "confirm");
                }
            }
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }

        }

        protected void btnSaveConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateSession(WMSTekSessions.InventoryConsult.MovementAdjustList))
                {
                    MovementAdjustViewDTO = (GenericViewDTO<MovementAdjust>)Session[WMSTekSessions.InventoryConsult.MovementAdjustList];

                    Session.Add(WMSTekSessions.InventoryConsult.MovementAdjustList, MovementAdjustViewDTO);

                    btnOk_Click(new object(), new EventArgs());
                }

            }
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {

                if (ValidateSession(WMSTekSessions.InventoryConsult.MovementAdjustList))
                {
                    int index = 0;
                    bool isValid = true;
                    GenericViewDTO<MovementAdjust> MovementAdjustTempViewDTO = new GenericViewDTO<MovementAdjust>();

                    MovementAdjustViewDTO = (GenericViewDTO<MovementAdjust>)Session[WMSTekSessions.InventoryConsult.MovementAdjustList];

                    for (int i = 0; i < grdMgr.Rows.Count; i++)
                    {
                        GridViewRow row = grdMgr.Rows[i];

                        if (((System.Web.UI.WebControls.CheckBox)row.FindControl("chkAdjustConfirm")).Checked)
                        {
                            index = (grdMgr.PageIndex * grdMgr.PageSize) + i;

                            MovementAdjustViewDTO.Entities[index].StateInterface = "C";
                            MovementAdjustTempViewDTO = iWarehousingMGR.MaintainMovementAdjust(CRUD.Update, MovementAdjustViewDTO.Entities[index], context);

                            if (MovementAdjustTempViewDTO.hasError())
                                isValid = false;
                        }
                    }
                    if(isValid)
                    {
                        crud = true;
                        ucStatus.ShowMessage(MovementAdjustTempViewDTO.MessageStatus.Message);

                        UpdateSession();
                    }
                    else
                    {
                        UpdateSession();
                    }
                }
            }
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                modalPopUpDialog.Hide();
            }
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
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
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a eliminar
                int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                DeleteRow(deleteIndex);
            }
            catch (Exception ex)
            {
                MovementAdjustViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "MovementAdjustConfirm";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //UpdateSession();
                
            }
            else
            {
                if (ValidateSession(WMSTekSessions.InventoryConsult.MovementAdjustList))
                {
                    MovementAdjustViewDTO = (GenericViewDTO<MovementAdjust>)Session[WMSTekSessions.InventoryConsult.MovementAdjustList];
                    isValidViewDTO = true;
                }

                if (isValidViewDTO)
                {
                    //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                    PopulateGrid();
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

            // Configura textos a mostar
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

            // Configura parámetros de fechas
            this.Master.ucMainFilter.DateBefore = CfgParameterName.AdjustmentConsultDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.AdjustmentConsultDaysAfter;

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
            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnConfirm_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnSaveVisible = true;
            this.Master.ucTaskBar.btnSaveToolTip = this.lblBtnSaveToolTip.Text;
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
            // Carga consulta de Tareas de Ajuste
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            MovementAdjustViewDTO = iWarehousingMGR.GetToConfirmMovementAdjust(context);

            if (!MovementAdjustViewDTO.hasError() && MovementAdjustViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.InventoryConsult.MovementAdjustList, MovementAdjustViewDTO);
                isValidViewDTO = true;

                ucStatus.ShowMessage(MovementAdjustViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(MovementAdjustViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            grdMgr.DataSource = MovementAdjustViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(MovementAdjustViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                divGrid.Visible = true;
                this.Master.ucError.ClearError();
            }
        }

        private void ShowConfirm(string title, string message)
        {
            this.divConfirm.Visible = true;

            this.lblDialogTitle.Text = title;
            this.divDialogMessage.InnerHtml = message;

            //this.Visible = true;
            modalPopUpDialog.Show();
        }

        private void DeleteRow(int index)
        {
            MovementAdjustViewDTO = iWarehousingMGR.MaintainMovementAdjust(CRUD.Delete, MovementAdjustViewDTO.Entities[index], context);

            if (MovementAdjustViewDTO.hasError())
            {
                UpdateSession();
                ucStatus.ShowMessage(MovementAdjustViewDTO.Errors.Message);
            }
            else
            {
                //Muestra mensaje de status
                crud = true;
                ucStatus.ShowMessage(MovementAdjustViewDTO.MessageStatus.Message);

                //Actualiza grid
                UpdateSession();
            }
        }

        #endregion
    }
}