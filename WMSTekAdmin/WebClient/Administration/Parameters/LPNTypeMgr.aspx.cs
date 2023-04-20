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
using System.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Parameters
{
    public partial class LPNTypeMgr : BasePage
    {
        #region "Declaracion de variables"

        private GenericViewDTO<LPNType> lpnViewDTO;
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
                        UpdateSession(false);

                        PopulateLists();
                    }

                    if (ValidateSession(WMSTekSessions.LpnTypeMgr.LpnTypeList))
                    {
                        lpnViewDTO = (GenericViewDTO<LPNType>)Session[WMSTekSessions.LpnTypeMgr.LpnTypeList];
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

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        #endregion

        #region "Métodos"

        public void Initialize()
        {
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();
            InitializeTypeUnitOfMeasure_OfMass();
        }

        private void InitializeTypeUnitOfMeasure_OfMass()
        {
            String type = string.Empty;
            String typeMass = string.Empty;

            var lstTypeLoc = GetConst("TypeOfUnitOfMeasure");
            var lstTypeMass = GetConst("TypeOfUnitOfMass");

            if (lstTypeLoc.Count == 0)
                type = "(mts)";
            else
                type = "(" + lstTypeLoc[0].Trim() + ")";

            if (lstTypeMass.Count == 0)
                typeMass = "(k)";
            else
                typeMass = "(" + lstTypeMass[0].Trim() + ")";


            this.lblTypeUnitMeasure2.Text = type;
            this.lblTypeUnitMeasure3.Text = type;
            this.lblTypeUnitMeasure4.Text = type;

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

            lpnViewDTO = iWarehousingMGR.FindAllLpnType(context);

            if (!lpnViewDTO.hasError() && lpnViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.LpnTypeMgr.LpnTypeList, lpnViewDTO);
                Session.Remove(WMSTekSessions.Shared.LpnTypeList);
                isValidViewDTO = true;

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
            this.Master.ucMainFilter.SaveOnIndexChanged = true;
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
            // Editar Tipo LPN
            if (mode == CRUD.Update)
            {
                this.lblError.Text = string.Empty;
                //Recupera los datos de la entidad a editar
                hidEditId.Value = lpnViewDTO.Entities[index].Id.ToString();

                txtCode.Text = lpnViewDTO.Entities[index].Code;
                ddlOwner.SelectedValue = lpnViewDTO.Entities[index].Owner.Id.ToString();
                txtName.Text = lpnViewDTO.Entities[index].Name;
                txtTare.Text = lpnViewDTO.Entities[index].Tare.ToString();
                txtVolume.Text = lpnViewDTO.Entities[index].Volume.ToString();
                txtLength.Text = lpnViewDTO.Entities[index].Length.ToString();
                txtWidth.Text = lpnViewDTO.Entities[index].Width.ToString();
                txtHeight.Text = lpnViewDTO.Entities[index].Height.ToString();
                txtNextAvailableNumber.Text = lpnViewDTO.Entities[index].NextAvailableNumber.ToString();
                txtWeightCapacity.Text = lpnViewDTO.Entities[index].WeightCapacity.ToString();
                txtVolumeCapacity.Text = lpnViewDTO.Entities[index].VolumeCapacity.ToString();
                this.chkStatus.Checked = lpnViewDTO.Entities[index].Status;
                this.chkPTLLabel.Checked = lpnViewDTO.Entities[index].PTLLabel;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Tipo LPN
            if (mode == CRUD.Create)
            {
                // Selecciona owner seleccionado en el Filtro
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                hidEditId.Value = "0";
                txtCode.Text = string.Empty;
                txtName.Text = string.Empty;
                txtTare.Text = string.Empty;
                txtVolume.Text = string.Empty;
                txtLength.Text = string.Empty;
                txtWidth.Text = string.Empty;
                txtHeight.Text = string.Empty;
                txtNextAvailableNumber.Text = string.Empty;
                txtWeightCapacity.Text = string.Empty;
                txtVolumeCapacity.Text = string.Empty;
                this.chkStatus.Checked = true;
                this.chkPTLLabel.Checked = false;

                //De la pagina
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (lpnViewDTO.Configuration != null && lpnViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(lpnViewDTO.Configuration, true);
                else
                    base.ConfigureModal(lpnViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            divShowErrors.Visible = false;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            //Agrega los datos del LPn
            LPNType lpnType = new LPNType();
            lpnType.Owner = new Owner();
            bool EsValido = false;

            //Recupera la lista de LpnTypeList
            if (ValidateSession(WMSTekSessions.LpnTypeMgr.LpnTypeList))
            {
                lpnViewDTO = (GenericViewDTO<LPNType>)Session[WMSTekSessions.LpnTypeMgr.LpnTypeList];
                isValidViewDTO = true;
            }
            if (hidEditId.Value == "0")
            {
                if (lpnViewDTO.Entities.Count == 0)
                    EsValido = true;

                // Valida el codigo repetido (Solo si es Tipo nuevo)
                foreach (LPNType objLpnType in lpnViewDTO.Entities)
                {
                    if (objLpnType.Code.Trim().ToUpper() == txtCode.Text.Trim().ToUpper() && objLpnType.Owner.Id.ToString() == ddlOwner.SelectedValue.Trim())
                    {
                        this.lblError.Text = this.lblErrorCodeExist.Text;
                        EsValido = false;
                        lpnViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.LPNType.Exists, context));
                        break;
                    }
                    else
                    {
                        EsValido = true;
                    }
                }

            }
            else
            {
                EsValido = true;
            }

            //Esta nueva validacion realiza la busque del codigo en la BD
            if (EsValido)
            {
                GenericViewDTO<LPNType> viewDtoLpn = iWarehousingMGR.FindCodeLpnType(txtCode.Text.Trim(), int.Parse(ddlOwner.SelectedValue), context);
                
                if (viewDtoLpn.hasError())
                {
                    ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);
                    EsValido = false;
                }
                else
                {
                    if (hidEditId.Value == "0" && viewDtoLpn.Entities != null && viewDtoLpn.Entities.Count > 0)
                    {
                        this.lblError.Text = this.lblErrorCodeExist.Text;                        
                        EsValido = false;
                        lpnViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.LPNType.Exists, context));
                    }

                    if (hidEditId.Value != "0" && viewDtoLpn.Entities != null && viewDtoLpn.Entities.Count > 0 && viewDtoLpn.Entities[0].Id != Convert.ToInt32(hidEditId.Value))
                    {
                        this.lblError.Text = this.lblErrorCodeExist.Text;
                        EsValido = false;
                        lpnViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.LPNType.Exists, context));
                    }
                }
            }

            //Valida si es PTL y que exista solo una vez por Owner
            if (EsValido)
            {
                int cantPTL = lpnViewDTO.Entities.Where(s => s.Code.Trim().ToUpper() != txtCode.Text.Trim().ToUpper() && s.Owner.Id.ToString().Equals(ddlOwner.SelectedValue) && s.PTLLabel == true).Count();

                if (cantPTL > 0 && this.chkPTLLabel.Checked)
                {
                    //this.lblError.Text = "rrrrrrrrrrrrr";
                    EsValido = false;
                    lpnViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.LPNType.PTLExists, context));
                }
            }


            if (EsValido)
            {
                divShowErrors.Visible = false;
                this.lblError.Text = string.Empty;
                lpnType.Id = Convert.ToInt32(hidEditId.Value);
                lpnType.Code = txtCode.Text;
                lpnType.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
                lpnType.Name = txtName.Text.Trim();
                if (!string.IsNullOrEmpty(this.txtTare.Text)) lpnType.Tare = Convert.ToDecimal(txtTare.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtVolume.Text)) lpnType.Volume = Convert.ToDecimal(txtVolume.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtLength.Text)) lpnType.Length = Convert.ToDecimal(txtLength.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtWidth.Text)) lpnType.Width = Convert.ToDecimal(txtWidth.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtHeight.Text)) lpnType.Height = Convert.ToDecimal(txtHeight.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtNextAvailableNumber.Text)) lpnType.NextAvailableNumber = Convert.ToInt32(txtNextAvailableNumber.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtWeightCapacity.Text)) lpnType.WeightCapacity = Convert.ToDecimal(txtWeightCapacity.Text.Trim());
                if (!string.IsNullOrEmpty(this.txtVolumeCapacity.Text)) lpnType.VolumeCapacity = Convert.ToDecimal(txtVolumeCapacity.Text.Trim());
                lpnType.Status = this.chkStatus.Checked;
                lpnType.PTLLabel = this.chkPTLLabel.Checked;

                if (hidEditId.Value == "0")
                {
                    lpnViewDTO = iWarehousingMGR.MaintainLpnType(CRUD.Create, lpnType, context);
                }
                else
                {
                    lpnType.Id = Convert.ToInt32(hidEditId.Value);
                    lpnViewDTO = iWarehousingMGR.MaintainLpnType(CRUD.Update, lpnType, context);
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
                    crud = true;
                    ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);

                    UpdateSession(false);
                }
            }
            else
            {
                if (lpnViewDTO.hasError())
                {
                    UpdateSession(true);
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                }
                else
                {
                    modalPopUp.Show();
                    divShowErrors.Visible = true;
                    this.lblError.Visible = true;
                }
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            lpnViewDTO = iWarehousingMGR.MaintainLpnType(CRUD.Delete, lpnViewDTO.Entities[index], context);

            if (lpnViewDTO.hasError())
                UpdateSession(true);
            else
            {
                crud = true;
                ucStatus.ShowMessage(lpnViewDTO.MessageStatus.Message);

                UpdateSession(false);
            }
        }
        #endregion
 
    }
}
