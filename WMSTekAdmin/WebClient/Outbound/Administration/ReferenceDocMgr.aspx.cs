using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Layout;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace Binaria.WMSTek.WebClient.Outbound.Administration
{
    public partial class ReferenceDocMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<ReferenceDoc> referenceDocViewDTO = new GenericViewDTO<ReferenceDoc>();
        private bool isValidViewDTO = true;


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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
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
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
            }
        }
        protected void btnMassiveLoad_Click(object sender, EventArgs e)
        {
            try
            {
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
            }
        }
        protected void btnLoadFile_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile.HasFile)
                {
                    GenericViewDTO<ReferenceDoc> newOutboundOrder = new GenericViewDTO<ReferenceDoc>();

                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(ex.Message);
                    }

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                "Data Source=" + savePath + ";" +
                                                "Extended Properties=Excel 8.0;";

                    var dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", connectionString);
                    DataSet myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        dataAdapter.Fill(myDataSet);
                        dataTable = myDataSet.Tables["Table"];
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       ReferenceDocNumber = r.Field<object>("ReferenceDocNumber"),
                                       ReferenceDocType = r.Field<object>("ReferenceDocType"),
                                       InvoiceDate = r.Field<object>("InvoiceDate"),
                                       OwnCode = r.Field<object>("OwnCode"),
                                       WhsCode = r.Field<object>("WhsCode"),
                                       OutboundNumber = r.Field<object>("OutboundNumber"),
                                       OutboundType = r.Field<object>("OutboundType")
                                   };

                    referenceDocViewDTO = new GenericViewDTO<ReferenceDoc>();
                    referenceDocViewDTO.Entities = new List<ReferenceDoc>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            var referenceDoc = new ReferenceDoc();

                            if (!ValidateIsNotNull(item.ReferenceDocNumber))
                            {
                                errorUp = "ReferenceDocNumber " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                referenceDoc.ReferenceDocNumber = item.ReferenceDocNumber.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.ReferenceDocType))
                            {
                                errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber +  " - ReferenceDocType " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                referenceDoc.ReferenceDocType = new ReferenceDocType();
                                referenceDoc.ReferenceDocType.Code = item.ReferenceDocType.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.InvoiceDate))
                            {
                                errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber + " - InvoiceDate " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                DateTime finalDate;
                                var flag = DateTime.TryParse(item.InvoiceDate.ToString(), out finalDate);

                                if (flag)
                                {
                                    referenceDoc.InvoiceDate = finalDate;
                                }
                                else
                                {
                                    errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber + " - InvoiceDate " + this.lblInvalid.Text;
                                    break;
                                } 
                            }

                            if (!ValidateIsNotNull(item.OwnCode))
                            {
                                errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber + " - OwnCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                referenceDoc.Owner = new Owner();
                                referenceDoc.Owner.Code = item.OwnCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.WhsCode))
                            {
                                errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber + " - WhsCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                referenceDoc.Warehouse = new Warehouse();
                                referenceDoc.Warehouse.Code = item.WhsCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.OutboundNumber))
                            {
                                errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber + " - OutboundNumber " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                referenceDoc.Order = new OutboundOrder();
                                referenceDoc.Order.Number = item.OutboundNumber.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.OutboundType))
                            {
                                errorUp = "ReferenceDocNumber " + item.ReferenceDocNumber + " - OutboundType " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                referenceDoc.OutboundType = new OutboundType();
                                referenceDoc.OutboundType.Code = item.OutboundType.ToString().Trim();
                            }

                            referenceDocViewDTO.Entities.Add(referenceDoc);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }

                    if (errorUp != "")
                    {
                        ShowAlertLocal(this.lblTitle.Text, errorUp);
                        divFondoPopupProgress.Visible = false;
                        divLoad.Visible = true;
                        modalPopUpLoad.Show();
                    }
                    else
                    {
                        if (referenceDocViewDTO.Entities.Count > 0)
                        {
                            referenceDocViewDTO = iDispatchingMGR.MaintainReferenceDocMassive(referenceDocViewDTO, context);

                            if (referenceDocViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, referenceDocViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(referenceDocViewDTO.MessageStatus.Message);
                                ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = false;
                                modalPopUpLoad.Hide();
                            }
                        }
                        else
                        {
                            ShowAlertLocal(this.lblTitle.Text, this.lblNotItemsFile.Text);
                            divFondoPopupProgress.Visible = false;
                            divLoad.Visible = true;
                            modalPopUpLoad.Show();
                        }
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                referenceDocViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
            }
            finally
            {
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
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

            if (!Page.IsPostBack)
            {
                PopulateLists();
            }
        }

        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnNewClick += new EventHandler(btnNew_Click);
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnMassiveLoad_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnAddVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerVisible = true;

            this.Master.ucMainFilter.referenceDocTypeVisible = true;
            this.Master.ucMainFilter.documentVisible = true;
            this.Master.ucMainFilter.dateFromVisible = true;
            this.Master.ucMainFilter.dateToVisible = true;
            this.Master.ucMainFilter.DateBefore = CfgParameterName.OutboundDaysBefore;
            this.Master.ucMainFilter.DateAfter = CfgParameterName.OutboundDaysAfter;
            this.Master.ucMainFilter.dateLabel = lblFilterDate.Text;

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
            referenceDocViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(referenceDocViewDTO.MessageStatus.Message);
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
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
                referenceDocViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            referenceDocViewDTO = iDispatchingMGR.FindAllReferenceDoc(context);

            if (!referenceDocViewDTO.hasError() && referenceDocViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.ReferenceDocumentMgr.ReferenceDocList, referenceDocViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(referenceDocViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(referenceDocViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            referenceDocViewDTO = (GenericViewDTO<ReferenceDoc>)Session[WMSTekSessions.ReferenceDocumentMgr.ReferenceDocList];

            if (referenceDocViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!countryViewDTO.hasConfigurationError() && countryViewDTO.Configuration != null && countryViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, countryViewDTO.Configuration);

            grdMgr.DataSource = referenceDocViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(referenceDocViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
        private void DeleteRow(int index)
        {
            referenceDocViewDTO = (GenericViewDTO<ReferenceDoc>)Session[WMSTekSessions.ReferenceDocumentMgr.ReferenceDocList];

            referenceDocViewDTO = iDispatchingMGR.MaintainReferenceDoc(CRUD.Delete, referenceDocViewDTO.Entities[index], context);

            if (referenceDocViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(referenceDocViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }
        protected void ShowModal(int index, CRUD mode)
        {
            if (mode == CRUD.Update)
            {
                referenceDocViewDTO = (GenericViewDTO<ReferenceDoc>)Session[WMSTekSessions.ReferenceDocumentMgr.ReferenceDocList];

                //Recupera los datos de la entidad a editar
                hidEditId.Value = referenceDocViewDTO.Entities[index].Id.ToString();
                txtReferenceDocNumber.Text = referenceDocViewDTO.Entities[index].ReferenceDocNumber;
                ddlReferenceDocType.SelectedValue = referenceDocViewDTO.Entities[index].ReferenceDocType.Id.ToString();
                txtInvoiceDate.Text = referenceDocViewDTO.Entities[index].InvoiceDate.ToString("dd/MM/yyyy");
                ddlOwner.SelectedValue = referenceDocViewDTO.Entities[index].Owner.Id.ToString();
                ddlWhs.SelectedValue = referenceDocViewDTO.Entities[index].Warehouse.Id.ToString();
                txtOutboundOrder.Text = referenceDocViewDTO.Entities[index].Order.Number;
                ddlDocType.SelectedValue = referenceDocViewDTO.Entities[index].OutboundType.Id.ToString();

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }
            else if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores
                hidEditId.Value = "0";

                txtReferenceDocNumber.Text = string.Empty;
                ddlReferenceDocType.SelectedValue = "-1";
                txtInvoiceDate.Text = string.Empty;
                ddlOwner.SelectedValue = "-1";
                ddlWhs.SelectedValue = "-1";
                txtOutboundOrder.Text = string.Empty;
                ddlDocType.SelectedValue = "-1";

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
            CallJsGridView();
        }
        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);
            base.LoadUserWarehouses(this.ddlWhs, this.Master.EmptyRowText, "-1", true);
            base.LoadReferenceDocType(ddlReferenceDocType, true, this.Master.EmptyRowText);
            base.LoadOutboundType(ddlDocType, true, this.Master.EmptyRowText);
        }
        protected void SaveChanges()
        {
            var referenceDoc = new ReferenceDoc();
            referenceDoc.ReferenceDocNumber = txtReferenceDocNumber.Text.Trim();
            referenceDoc.ReferenceDocType = new ReferenceDocType();
            referenceDoc.ReferenceDocType.Id = int.Parse(ddlReferenceDocType.SelectedValue);
            referenceDoc.InvoiceDate = DateTime.ParseExact(txtInvoiceDate.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); 
            referenceDoc.Owner = new Owner();
            referenceDoc.Owner.Id = int.Parse(ddlOwner.SelectedValue);
            referenceDoc.Warehouse = new Warehouse();
            referenceDoc.Warehouse.Id = int.Parse(ddlWhs.SelectedValue);
            referenceDoc.Order = new OutboundOrder();
            referenceDoc.Order.Number = txtOutboundOrder.Text;
            referenceDoc.OutboundType = new OutboundType();
            referenceDoc.OutboundType.Id = int.Parse(ddlDocType.SelectedValue);

            if (hidEditId.Value == "0")
            {
                referenceDocViewDTO = iDispatchingMGR.MaintainReferenceDoc(CRUD.Create, referenceDoc, context);
            }
            else
            {
                referenceDoc.Id = int.Parse(hidEditId.Value);
                referenceDocViewDTO = iDispatchingMGR.MaintainReferenceDoc(CRUD.Update, referenceDoc, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (referenceDocViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(referenceDocViewDTO.MessageStatus.Message);
                //Actualiza grilla
                UpdateSession(false);
            }
        }
        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }
        #endregion
    }
}