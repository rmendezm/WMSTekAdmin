using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.Collections.Generic;


namespace Binaria.WMSTek.WebClient.Administration.Parameters
{
    public partial class HolidayMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<NonWorkingDay> NonWorkingDayViewDTO = new GenericViewDTO<NonWorkingDay>();
        private bool isValidViewDTO = false;
        MiscUtils util = new MiscUtils();

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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.NonWorkingDayMgr.NonWorkingDayList))
                    {
                        NonWorkingDayViewDTO = (GenericViewDTO<NonWorkingDay>)Session[WMSTekSessions.NonWorkingDayMgr.NonWorkingDayList];
                        isValidViewDTO = true;
                    }

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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadData();
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowModal(0, CRUD.Create);
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                base.grdMgr_RowDataBound(sender, e);
                //Ve el numero que viene y agrega el nombre del tyipo de razon

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //busca controles
                    Label lblTypeInOut = (Label)e.Row.FindControl("lblTypeInOut");
                    if (lblTypeInOut != null)
                    {
                        //me aseguro que no venga ninguno vacio
                        if (lblTypeInOut.Text != "")
                        {
                            if (util.IsNumber(lblTypeInOut.Text))
                            {
                             switch(Convert.ToInt16(lblTypeInOut.Text))
                             {
                                case 1:
                               
                                    lblTypeInOut.Text = TypeInOut.Inbound.ToString();
                                    break;
                                
                                 case 2:
                                
                                    lblTypeInOut.Text = TypeInOut.Outbound.ToString();
                                    break;

                                 case 3:
                                
                                    lblTypeInOut.Text = TypeInOut.Both.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
                
        }

        protected void grdMgr_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        protected void grdMgr_RowEditing1(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Calcula la posicion en el ViewDTO de la fila a editar
                int editIndex = grdMgr.PageSize * grdMgr.PageIndex + e.NewEditIndex;

                ShowModal(editIndex, CRUD.Update);
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
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
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        protected void grdMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                currentPage = e.NewPageIndex;
            }
            catch (Exception ex)
            {
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
                NonWorkingDayViewDTO.ClearError();
            }

            //this.Master.ucMainFilter.dateToVisible = true;
            
            context.MainFilter = this.Master.ucMainFilter.MainFilter;
            NonWorkingDayViewDTO = iWarehousingMGR.FindAllNonWorkingDay(context);
            //NonWorkingDayViewDTO = iWarehousingMGR.GetAllInFromYear(DateTime.Now,context);
            //this.Master.ucMainFilter.dateFromVisible = false;

            if (!NonWorkingDayViewDTO.hasError() && NonWorkingDayViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.NonWorkingDayMgr.NonWorkingDayList, NonWorkingDayViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(NonWorkingDayViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
            }
        }


        private void PopulateGrid()
        {
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!NonWorkingDayViewDTO.hasConfigurationError() && NonWorkingDayViewDTO.Configuration != null && NonWorkingDayViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, NonWorkingDayViewDTO.Configuration);

            grdMgr.DataSource = NonWorkingDayViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(NonWorkingDayViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Habilita criterios a usar
            this.Master.ucMainFilter.dateYearVisible = true;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
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

        protected void ReloadData()
        {
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Muestra ventana modal con los datos de la entidad a Editar o Crear
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a editar</param>
        /// <param name="mode">CRUD.Create o CRUD.Update</param>
        protected void ShowModal(int index, CRUD mode)
        {
            // Editar Proveedor
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = "0";
                txtCode.Text = NonWorkingDayViewDTO.Entities[index].DateNonWorking.ToString("dd-MM-yyyy");
                txtCode.Enabled = false;
                txtName.Text = NonWorkingDayViewDTO.Entities[index].Description;
                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Proveedor
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "1";
                txtCode.Text = string.Empty;
                txtCode.Enabled = true;
                txtName.Text = string.Empty;
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (NonWorkingDayViewDTO.Configuration != null && NonWorkingDayViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(NonWorkingDayViewDTO.Configuration, true);
                else
                    base.ConfigureModal(NonWorkingDayViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            NonWorkingDay nonWorkingDay = new NonWorkingDay();

            nonWorkingDay.DateNonWorking = DateTime.Parse(txtCode.Text);
            nonWorkingDay.Description = !String.IsNullOrEmpty(txtName.Text) ? txtName.Text : String.Empty;

            if (hidEditId.Value == "1")
                NonWorkingDayViewDTO = iWarehousingMGR.MaintainNonWorkingDay(CRUD.Create, nonWorkingDay, context);
            else
                NonWorkingDayViewDTO = iWarehousingMGR.MaintainNonWorkingDay(CRUD.Update, nonWorkingDay, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (NonWorkingDayViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(NonWorkingDayViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            NonWorkingDayViewDTO = iWarehousingMGR.MaintainNonWorkingDay(CRUD.Delete, NonWorkingDayViewDTO.Entities[index], context);

            if (NonWorkingDayViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(NonWorkingDayViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }
        #endregion

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                grdMgr.Columns[3].Visible = false;
                base.ExportToExcel(grdMgr, null, null);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                grdMgr.Columns[3].Visible = true;
                //no hace nada
            }
            catch (Exception ex)
            {
                grdMgr.Columns[3].Visible = true;
                NonWorkingDayViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(NonWorkingDayViewDTO.Errors);
                
            }
        }

    }
}
