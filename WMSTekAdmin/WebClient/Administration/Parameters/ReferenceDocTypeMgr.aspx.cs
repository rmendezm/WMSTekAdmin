using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Reception;

namespace Binaria.WMSTek.WebClient.Administration.Parameters
{
    public partial class ReferenceDocTypeMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<ReferenceDocType> referenceDocTypeViewDTO = new GenericViewDTO<ReferenceDocType>();
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
                        UpdateSession(false);
                    }

                    if (ValidateSession(WMSTekSessions.ReferenceDocMgr.ReferenceDocList))
                    {
                        referenceDocTypeViewDTO = (GenericViewDTO<ReferenceDocType>)Session[WMSTekSessions.ReferenceDocMgr.ReferenceDocList];
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
        //        referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
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
                referenceDocTypeViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        /// <summary>
        /// Este metodo se ha sobrescrito para poder tener los nombres de los tooltip traducidos
        /// ya que al tenerlo en labels, esto si se puede traducir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    base.grdMgr_RowDataBound(sender, e);

        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        //Carga los textos del tool tip

        //        ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;
        //        ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;

        //        if (btnEdit != null)
        //        {
        //            btnEdit.ToolTip = this.lblToolTipEdit.Text;
        //        }
        //        if (btnDelete != null)
        //        {
        //            btnDelete.ToolTip = this.lblToolTipDelete.Text;
        //        }
        //    }
        //}

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
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
                referenceDocTypeViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            referenceDocTypeViewDTO = iWarehousingMGR.FindAllReferenceDocType(context);

            if (!referenceDocTypeViewDTO.hasError() && referenceDocTypeViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ReferenceDocMgr.ReferenceDocList, referenceDocTypeViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(referenceDocTypeViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(referenceDocTypeViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!referenceDocTypeViewDTO.hasConfigurationError() && referenceDocTypeViewDTO.Configuration != null && referenceDocTypeViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, referenceDocTypeViewDTO.Configuration);

            grdMgr.DataSource = referenceDocTypeViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(referenceDocTypeViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }
        

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.nameVisible = true;
            this.Master.ucMainFilter.codeVisible = true;

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
                hidEditId.Value = referenceDocTypeViewDTO.Entities[index].Id.ToString();


                txtName.Text = referenceDocTypeViewDTO.Entities[index].Name;
                txtCode.Text = referenceDocTypeViewDTO.Entities[index].Code;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nueva Proveedor
            if (mode == CRUD.Create)
            {
                hidEditId.Value = "0";
                txtName.Text = string.Empty;
                txtCode.Text = string.Empty;
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (referenceDocTypeViewDTO.Configuration != null && referenceDocTypeViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(referenceDocTypeViewDTO.Configuration, true);
                else
                    base.ConfigureModal(referenceDocTypeViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            
            ReferenceDocType referenceDocType = new ReferenceDocType();

            referenceDocType.Id = Convert.ToInt32(hidEditId.Value);

            referenceDocType.Name = txtName.Text.Trim();
            referenceDocType.Code = txtCode.Text.Trim();

            if (hidEditId.Value == "0")
                referenceDocTypeViewDTO = iWarehousingMGR.MaintainReferenceDocType(CRUD.Create, referenceDocType, context);
            else
                referenceDocTypeViewDTO = iWarehousingMGR.MaintainReferenceDocType(CRUD.Update, referenceDocType, context);

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (referenceDocTypeViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(referenceDocTypeViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            referenceDocTypeViewDTO = iWarehousingMGR.MaintainReferenceDocType(CRUD.Delete, referenceDocTypeViewDTO.Entities[index], context);

            if (referenceDocTypeViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(referenceDocTypeViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }
        #endregion
    }
}
