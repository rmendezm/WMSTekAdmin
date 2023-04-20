using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class LpnMgr : BasePage
    {
        #region "Declaracion de variables"

        private GenericViewDTO<LPN> lpnViewDTO = new GenericViewDTO<LPN>();
        private bool isValidViewDTO = false;

        //Propiedad para controlar el nro de pagina activa en la grilla
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

                        PopulateLists();
                    }

                    if (ValidateSession(WMSTekSessions.LpnMgr.LpnList))
                    {
                        lpnViewDTO = (GenericViewDTO<LPN>)Session[WMSTekSessions.LpnMgr.LpnList];
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                CallJsGridView();
            }
            catch (Exception ex)
            {
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                CallJsGridView();
            }
            catch (Exception ex)
            {
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
            }
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
        //        lpnViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
            }
        }


        protected void ddlOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadLpnTypeActive(this.ddlLpnType, int.Parse(this.ddlOwner.SelectedValue), true, this.Master.EmptyRowText);
                this.modalPopUp.Show();

                //GenericViewDTO<LPNType> lpnTypeViewDTO = new GenericViewDTO<LPNType>();

                //ContextViewDTO newContext = new ContextViewDTO();
                //newContext = context;
                //newContext.MainFilter = this.Master.ucMainFilter.MainFilter; ;
                //newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();
                //newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(this.ddlOwner.SelectedIndex, this.ddlOwner.SelectedValue));

                //lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(context);
                //Session.Add(WMSTekSessions.Shared.LpnTypeList, lpnTypeViewDTO);
                ////}

                //this.ddlLpnType.DataSource = lpnTypeViewDTO.Entities;
                //this.ddlLpnType.DataTextField = "Name";
                //this.ddlLpnType.DataValueField = "Id";
                //this.ddlLpnType.DataBind();

                //this.ddlLpnType.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
                //this.ddlLpnType.Items[0].Selected = true;
                

            }
            catch (Exception ex)
            {
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
                lpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
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
            InitializeTypeUnitOfMass();
        }

        private void InitializeTypeUnitOfMass()
        {
            String typeMass = string.Empty;

            var lstTypeMass = GetConst("TypeOfUnitOfMass");

            if (lstTypeMass.Count == 0)
                typeMass = "(k)";
            else
                typeMass = "(" + lstTypeMass[0].Trim() + ")";

            this.lblTypeUnitOfMass.Text = typeMass;
            this.lblTypeUnitOfMass2.Text = typeMass;
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
                lpnViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            lpnViewDTO = iWarehousingMGR.FindAllLPN(context);

            if (!lpnViewDTO.hasError() && lpnViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LpnMgr.LpnList, lpnViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(lpnViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!lpnViewDTO.hasConfigurationError() && lpnViewDTO.Configuration != null && lpnViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, lpnViewDTO.Configuration);

            grdMgr.DataSource = lpnViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(lpnViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
            //base.LoadLpnType(this.ddlLpnType, true, this.Master.EmptyRowText);
            
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;
            this.Master.ucMainFilter.lpnTypeVisible = true;
            this.Master.ucMainFilter.SaveOnIndexChanged = true;
            
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

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            lpnViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);
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
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;

            //Actualiza grilla
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
                LoadLpnTypeActive(this.ddlLpnType, lpnViewDTO.Entities[index].Owner.Id, true, this.Master.EmptyRowText);

                //Recupera los datos de la entidad a editar
                hidEditId.Value = lpnViewDTO.Entities[index].IdCode.ToString();

                txtCode.Text = lpnViewDTO.Entities[index].Code;
                txtCode.Enabled = false;
                ddlOwner.SelectedValue = lpnViewDTO.Entities[index].Owner.Id.ToString();
                ddlLpnType.SelectedValue = lpnViewDTO.Entities[index].LPNType.Id.ToString();
                txtWeightEmpty.Text = lpnViewDTO.Entities[index].WeightEmpty.ToString();
                txtWeightTotal.Text = lpnViewDTO.Entities[index].WeightTotal.ToString();
                this.chkStatus.Checked = lpnViewDTO.Entities[index].Status;
                
                this.chkIsClosed.Checked = lpnViewDTO.Entities[index].IsClosed;
                this.txtSealNumber.Text = lpnViewDTO.Entities[index].SealNumber;
                //this.chkIsClosed.Checked = false;
                //this.txtSealNumber.Text = string.Empty;
                  

                lblNew.Visible = false;
                lblEdit.Visible = true;            
            }

            // Nuevo Cliente
            if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores
                hidEditId.Value = "0";

                // Selecciona owner seleccionado en el Filtro
                this.ddlOwner.SelectedValue = Master.ucMainFilter.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
                //this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
            
                txtCode.Text = string.Empty;
                txtCode.Enabled = true;
                txtWeightEmpty.Text = string.Empty;
                txtWeightTotal.Text = string.Empty;
                this.chkStatus.Checked = true;
                this.chkIsClosed.Checked = false;
                this.txtSealNumber.Text = string.Empty;

                LoadLpnTypeActive(this.ddlLpnType, int.Parse(this.ddlOwner.SelectedValue), true, this.Master.EmptyRowText);
                ddlLpnType.SelectedValue = "-1";
                //De la pagina
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (lpnViewDTO != null && lpnViewDTO.Configuration != null && lpnViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(lpnViewDTO.Configuration, true);
                else
                    base.ConfigureModal(lpnViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            //Agrega los datos del LPN
            LPN lpn = new LPN();
            lpn.Owner = new Owner();
            lpn.LPNType = new LPNType();

            lpn.IdCode = txtCode.Text.Trim();
            lpn.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
            lpn.LPNType.Id = Convert.ToInt32(this.ddlLpnType.SelectedValue);
            lpn.Status = this.chkStatus.Checked;
            lpn.Fifo = DateTime.Now;
            if (!String.IsNullOrEmpty(txtWeightEmpty.Text)) lpn.WeightEmpty = Convert.ToDecimal(txtWeightEmpty.Text.Trim());
            if (!String.IsNullOrEmpty(txtWeightTotal.Text)) lpn.WeightTotal = Convert.ToDecimal(txtWeightTotal.Text.Trim());
            lpn.IsClosed = this.chkIsClosed.Checked;
            lpn.SealNumber = this.txtSealNumber.Text.Trim();

            if (hidEditId.Value == "0")
            {
                lpnViewDTO = iWarehousingMGR.MaintainLPN(CRUD.Create, lpn, context);
            }
            else
            {
                var lpnSelected = lpnViewDTO.Entities.Where(l => l.Code == txtCode.Text.Trim()).FirstOrDefault();

                if (lpnSelected != null)
                {
                    lpn.IsParent = lpnSelected.IsParent;
                }

                lpn.IdCode = hidEditId.Value;
                lpnViewDTO = iWarehousingMGR.MaintainLPN(CRUD.Update, lpn, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (lpnViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {   
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);
                //Actualiza grilla
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            lpnViewDTO = iWarehousingMGR.MaintainLPN(CRUD.Delete, lpnViewDTO.Entities[index], context);

            if (lpnViewDTO.hasError())
                UpdateSession(true);
            else
            {   
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "initializeGridDragAndDrop('Lpn_FindAll', 'ctl00_MainContent_grdMgr');", true);
        }
        #endregion
    }
}
