using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class GroupItem2Mgr : BasePage
    {

        #region "Declaración de Variables"

        private GenericViewDTO<GrpItem2> grpItem2ViewDTO = new GenericViewDTO<GrpItem2>();
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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        //UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.GroupItem2Mgr.GroupItem2List))
                    {
                        grpItem2ViewDTO = (GenericViewDTO<GrpItem2>)Session[WMSTekSessions.GroupItem2Mgr.GroupItem2List];
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
            }
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
        //        grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
        //    }
        //}

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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
            }
        }

        protected void grdMgr_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Valida variable de sesion del Usuario Loggeado
                if (ValidateSession(WMSTekSessions.Global.LoggedInUser))
                {
                    // Calcula la posicion en el ViewDTO de la fila a eliminar
                    int deleteIndex = grdMgr.PageSize * grdMgr.PageIndex + e.RowIndex;

                    DeleteRow(deleteIndex);
                }
            }
            catch (Exception ex)
            {
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            //string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();

                // Capturo la posicion de la fila activa
                //int index = grdMgr.PageSize * grdMgr.PageIndex + grdMgr.SelectedIndex;

                //if (index != -1)
                //{
                //    LoadOutboundOrderDetail(index);
                //    detailTitle = lblGridDetail.Text + lblNroDoc.Text + string.Empty;
                //}
                //else
                //{
                //    detailTitle = null;
                //}

                //base.ExportToExcel(grdMgr, grdDetail, detailTitle);

                base.ExportToExcel(grdMgr, null, null, true);
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
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
            PopulateList();
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
                grpItem2ViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            grpItem2ViewDTO = iWarehousingMGR.FindAllGrpItem2(context);

            if (!grpItem2ViewDTO.hasError() && grpItem2ViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.GroupItem2Mgr.GroupItem2List, grpItem2ViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(grpItem2ViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
            }
        }

        private void PopulateList()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
            base.LoadGroupItem1(this.ddlGrpItem1, true ,this.Master.EmptyRowText,-1);
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!grpItem2ViewDTO.hasConfigurationError() && grpItem2ViewDTO.Configuration != null && grpItem2ViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, grpItem2ViewDTO.Configuration);

            grdMgr.DataSource = grpItem2ViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(grpItem2ViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            //this.Master.ucMainFilter.codeLabel = this.lblFilterCodeLabel.Text;

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
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;
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
            // Editar Groupitem2
            if (mode == CRUD.Update)
            {
                //Recupera los datos de la entidad a editar
                hidEditId.Value = grpItem2ViewDTO.Entities[index].Id.ToString();
                this.ddlOwner.SelectedValue = (grpItem2ViewDTO.Entities[index].GrpItem1.Owner.Id).ToString();

                base.LoadGroupItem1(this.ddlGrpItem1, true, this.Master.EmptyRowText, grpItem2ViewDTO.Entities[index].GrpItem1.Owner.Id);

                this.ddlGrpItem1.SelectedValue = grpItem2ViewDTO.Entities[index].GrpItem1.Id.ToString();
                this.txtCode.Text = grpItem2ViewDTO.Entities[index].Code;
                this.txtName.Text = grpItem2ViewDTO.Entities[index].Name;

                this.lblNew.Visible = false;
                this.lblEdit.Visible = true;
            }

            // Nueva Groupitem2
            if (mode == CRUD.Create)
            {
                if (context.MainFilter == null)
                    context.MainFilter = this.Master.ucMainFilter.MainFilter;
                hidEditId.Value = "0";
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
                base.LoadGroupItem1(this.ddlGrpItem1, true, this.Master.EmptyRowText, int.Parse(this.ddlOwner.SelectedValue));
                this.ddlGrpItem1.SelectedValue = "-1";
                this.lblNew.Visible = true;
                this.lblEdit.Visible = false;

                this.txtCode.Text = string.Empty;
                this.txtName.Text = string.Empty;
            }

            if (grpItem2ViewDTO.Configuration != null && grpItem2ViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(grpItem2ViewDTO.Configuration, true);
                else
                    base.ConfigureModal(grpItem2ViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            GrpItem2 grpItem2 = new GrpItem2();
            grpItem2.Id = int.Parse(this.hidEditId.Value.Trim());
            grpItem2.Code = txtCode.Text.Trim();
            grpItem2.Name = txtName.Text.Trim();
            grpItem2.GrpItem1 = new GrpItem1(int.Parse(this.ddlGrpItem1.SelectedValue));

            if (hidEditId.Value == "0")
                grpItem2ViewDTO = iWarehousingMGR.MaintainGrpItem2(CRUD.Create, grpItem2, context);
            else
                grpItem2ViewDTO = iWarehousingMGR.MaintainGrpItem2(CRUD.Update, grpItem2, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (grpItem2ViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(grpItem2ViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            grpItem2ViewDTO = iWarehousingMGR.MaintainGrpItem2(CRUD.Delete, grpItem2ViewDTO.Entities[index], context);

            if (grpItem2ViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(grpItem2ViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        protected void ddlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el owner... cambiará el sector
                //isNew = true;
                //IdStateDelivery = 0;
                base.LoadGroupItem1(this.ddlGrpItem1, true, this.Master.EmptyRowText, Convert.ToInt32(this.ddlOwner.SelectedValue));
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                grpItem2ViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(grpItem2ViewDTO.Errors);
            }
        }

        #endregion
       
    }
}
