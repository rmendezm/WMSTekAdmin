using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using Binaria.WMSTek.Framework.Entities.Configuration;
using Binaria.WMSTek.Framework.Entities.Warehousing;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class AskInfoProcessMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<AskInfoProcess> askInfoProcessViewDTO = new GenericViewDTO<AskInfoProcess>();
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

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal) Initialize();
            }
            catch (Exception ex)
            {
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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

                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
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
                askInfoProcessViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"
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
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnNewVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.WmsProcessTypeVisible = true;
            this.Master.ucMainFilter.IdModule = 2;

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
            askInfoProcessViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(askInfoProcessViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void DeleteRow(int index)
        {
            askInfoProcessViewDTO = (GenericViewDTO<AskInfoProcess>)Session[WMSTekSessions.AskInfoProcessMgr.AskInfoList];

            askInfoProcessViewDTO = iWarehousingMGR.MaintainAskInfo(CRUD.Delete, askInfoProcessViewDTO.Entities[index], context); 

            if (askInfoProcessViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(askInfoProcessViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
                askInfoProcessViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            var askInfoParam = new AskInfoProcess();

            var wmsProcessFilt = context.MainFilter.Where(filter => filter.Name == "WmsProcessType").ToList();
            if (wmsProcessFilt != null && wmsProcessFilt.Count > 0 && wmsProcessFilt.First().FilterValues.Count > 0)
            {
                askInfoParam.WmsProcess = new WmsProcess();
                askInfoParam.WmsProcess.Code = wmsProcessFilt.First().FilterValues.First().Value;
            }

            askInfoProcessViewDTO = iWarehousingMGR.GetAskInfoProcessByAnyParameter(askInfoParam, context);

            if (!askInfoProcessViewDTO.hasError() && askInfoProcessViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.AskInfoProcessMgr.AskInfoList, askInfoProcessViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(askInfoProcessViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(askInfoProcessViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            askInfoProcessViewDTO = (GenericViewDTO<AskInfoProcess>)Session[WMSTekSessions.AskInfoProcessMgr.AskInfoList];

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!askInfoProcessViewDTO.hasConfigurationError() && askInfoProcessViewDTO.Configuration != null && askInfoProcessViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, askInfoProcessViewDTO.Configuration);

            grdMgr.DataSource = askInfoProcessViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(askInfoProcessViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

        protected void ShowModal(int index, CRUD mode)
        {
            populateList();

            if (mode == CRUD.Update)
            {
                askInfoProcessViewDTO = (GenericViewDTO<AskInfoProcess>)Session[WMSTekSessions.AskInfoProcessMgr.AskInfoList];

                //Recupera los datos de la entidad a editar
                hidEditId.Value = "1";
                                
                this.txtParameterAsk.Text = askInfoProcessViewDTO.Entities[index].ParameterAsk;                
                this.txtSequence.Text = askInfoProcessViewDTO.Entities[index].Sequence.ToString();
                this.ddlObject.SelectedValue = askInfoProcessViewDTO.Entities[index].Object;
                this.ddlProperty.SelectedValue = askInfoProcessViewDTO.Entities[index].Property;
                this.ddlWmsProcessType.SelectedValue = askInfoProcessViewDTO.Entities[index].WmsProcess.Code;

                this.ddlObject.Enabled = false;
                this.ddlProperty.Enabled = false;
                this.txtSequence.Enabled = false;
                this.ddlWmsProcessType.Enabled = false;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }
            else if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores
                hidEditId.Value = "0";

                this.ddlObject.Enabled = true;
                this.ddlProperty.Enabled = true;
                this.txtSequence.Enabled = true;
                this.ddlWmsProcessType.Enabled = true;
                                
                this.txtParameterAsk.Text = string.Empty;
                this.txtSequence.Text = string.Empty;
                this.ddlProperty.SelectedValue = "-1";
                this.ddlObject.SelectedValue = "-1";
                this.ddlWmsProcessType.SelectedValue = "-1";

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
            CallJsGridView();
        }

        protected void SaveChanges()
        {
            AskInfoProcess askInfo = new AskInfoProcess();
            askInfo.Object = this.ddlObject.SelectedValue;
            askInfo.ParameterAsk = this.txtParameterAsk.Text.Trim();
            askInfo.Property = this.ddlProperty.SelectedValue;
            askInfo.Sequence = int.Parse(this.txtSequence.Text.Trim());
            askInfo.WmsProcess = new WmsProcess();
            askInfo.WmsProcess.Code = this.ddlWmsProcessType.SelectedValue;

            if (hidEditId.Value == "0")
            {
                askInfoProcessViewDTO = iWarehousingMGR.MaintainAskInfo(CRUD.Create, askInfo, context);
            }
            else
            {
                askInfoProcessViewDTO = iWarehousingMGR.MaintainAskInfo(CRUD.Update, askInfo, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (askInfoProcessViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(askInfoProcessViewDTO.MessageStatus.Message);
                //Actualiza grilla
                UpdateSession(false);
            }
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }

        private void populateList()
        {
            base.LoadWmsProcess(this.ddlWmsProcessType, 2, true, this.Master.EmptyRowText);


            int i = 0;
            this.ddlProperty.Items.Insert(i, new ListItem(this.Master.EmptyRowText, "-1"));
            foreach (var prop in Enum.GetValues(typeof(eAskInfoProperty)))
            {
                i++;
                this.ddlProperty.Items.Insert(i, new ListItem(prop.ToString(), prop.ToString()));
            }
            this.ddlProperty.Items[0].Selected = true;

            i = 0;
            this.ddlObject.Items.Insert(i, new ListItem(this.Master.EmptyRowText, "-1"));
            foreach (var obj in Enum.GetValues(typeof(eAskInfoObject)))
            {
                i++;
                this.ddlObject.Items.Insert(i, new ListItem(obj.ToString(), obj.ToString()));                
            }
            this.ddlObject.Items[0].Selected = true;

        }

        #endregion
    }
}