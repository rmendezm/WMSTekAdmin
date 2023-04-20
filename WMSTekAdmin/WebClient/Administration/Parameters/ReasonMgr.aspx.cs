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
    public partial class ReasonMgr :BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Reason> reasonViewDTO = new GenericViewDTO<Reason>();
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

                        if (isValidViewDTO)
                        {
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.ReasonMgr.ReasonList))
                    {
                        reasonViewDTO = (GenericViewDTO<Reason>)Session[WMSTekSessions.ReasonMgr.ReasonList];
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
        //        reasonViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(reasonViewDTO.Errors);
        //    }
        //}

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                var lstTypeReason = GetConst("TypeReasonsNotModified");

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;
                    ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;

                    if (btnDelete != null) btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";

                    // Deshabilita la opcion de Editar y Eliminar de la Razon
                    if (btnDelete != null && lstTypeReason.Contains(reasonViewDTO.Entities[e.Row.DataItemIndex].Code.ToUpper()))
                    {
                        btnDelete.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_delete_dis.png";
                        btnDelete.Enabled = false;
                    }


                    if (btnEdit != null && lstTypeReason.Contains(reasonViewDTO.Entities[e.Row.DataItemIndex].Code.ToUpper()))
                    {
                        btnEdit.ImageUrl = "~/WebResources/Images/Buttons/GridActions/icon_edit_dis.png";
                        btnEdit.Enabled = false;
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                                switch (Convert.ToInt16(lblTypeInOut.Text))
                                {
                                    case 0:
                                        lblTypeInOut.Text = TypeInOut.System.ToString();
                                        break;

                                    case 1:
                                        lblTypeInOut.Text = TypeInOut.Inbound.ToString();
                                        break;

                                    case 2:
                                        lblTypeInOut.Text = TypeInOut.Outbound.ToString();
                                        break;

                                    case 3:
                                        lblTypeInOut.Text = TypeInOut.Both.ToString();
                                        break;

                                    case 4:
                                        lblTypeInOut.Text = TypeInOut.QualityControl.ToString();
                                        break;
                                    case 5:
                                        lblTypeInOut.Text = TypeInOut.Adjust.ToString();
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                reasonViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
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
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
                reasonViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            reasonViewDTO = iWarehousingMGR.FindAllReason(context);

            if (!reasonViewDTO.hasError() && reasonViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ReasonMgr.ReasonList, reasonViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(reasonViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(reasonViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.LoadTypeInOut(this.ddlTypeInOut, this.Master.EmptyRowText);
        }

        private void PopulateGrid()
        {
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!reasonViewDTO.hasConfigurationError() && reasonViewDTO.Configuration != null && reasonViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, reasonViewDTO.Configuration);

            grdMgr.DataSource = reasonViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(reasonViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            //Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.statusVisible = true;

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

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
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
                hidEditId.Value = reasonViewDTO.Entities[index].Id.ToString();

                PopulateLists();
                txtCode.Text = reasonViewDTO.Entities[index].Code;
                txtName.Text = reasonViewDTO.Entities[index].Name;
                chkCanBlock.Checked = reasonViewDTO.Entities[index].CanBlock;
                chkCanCount.Checked = reasonViewDTO.Entities[index].CanCount;
                chkInfoERP.Checked = reasonViewDTO.Entities[index].InfoERP;
                ddlTypeInOut.SelectedValue = reasonViewDTO.Entities[index].TypeInOut.ToString();
                chkStatus.Checked = reasonViewDTO.Entities[index].Status;
              
                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Proveedor
            if (mode == CRUD.Create)
            {
                PopulateLists();

                hidEditId.Value = "0";

                chkStatus.Checked = true;
                txtName.Text = string.Empty;
                txtCode.Text = string.Empty;
                txtName.Text = string.Empty;
                ddlTypeInOut.SelectedValue = "-1";

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (reasonViewDTO.Configuration != null && reasonViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(reasonViewDTO.Configuration, true);
                else
                    base.ConfigureModal(reasonViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            
            Reason reason = new Reason();

            reason.Id = Convert.ToInt32(hidEditId.Value);
            reason.Name = txtName.Text.Trim();
            reason.Code = txtCode.Text.Trim();
            reason.CanBlock = chkCanBlock.Checked;
            reason.CanCount = chkCanCount.Checked;
            reason.InfoERP = chkInfoERP.Checked;
            reason.TypeInOut = Convert.ToInt32(ddlTypeInOut.SelectedValue);
            reason.Status = chkStatus.Checked;

            if (hidEditId.Value == "0")
                reasonViewDTO = iWarehousingMGR.MaintainReason(CRUD.Create, reason, context);
            else
                reasonViewDTO = iWarehousingMGR.MaintainReason(CRUD.Update, reason, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (reasonViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(reasonViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            reasonViewDTO = iWarehousingMGR.MaintainReason(CRUD.Delete, reasonViewDTO.Entities[index], context);

            if (reasonViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(reasonViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }
        #endregion

    }
}
