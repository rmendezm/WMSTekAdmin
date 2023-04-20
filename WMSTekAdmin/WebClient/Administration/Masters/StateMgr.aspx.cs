using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class StateMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<State> stateViewDTO = new GenericViewDTO<State>();
        private bool isValidViewDTO = true;
        private bool isNew = false;

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
                if (base.webMode == WebMode.Normal)
                {
                    Initialize();

                    if (!Page.IsPostBack)
                    {
                        // Carga inicial del ViewDTO
                        //UpdateSession(false);

                        if (isValidViewDTO)
                        {
                            ////base.FindAllPlaces();
                            PopulateLists();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            divLoad.Visible = true;
            modalPopUpLoad.Show();
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }

        protected void btnSubir2_Click(object sender, EventArgs e)
        {
            string pathAux = "";

            try
            {
                string errorUp = "";

                if (uploadFile2.HasFile)
                {
                    string savePath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("UpLoadItemFilePath", "");
                    savePath += uploadFile2.FileName;
                    pathAux = savePath;

                    try
                    {
                        uploadFile2.SaveAs(savePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(ex.Message);
                    }

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                                "Data Source=" + savePath + ";" +
                                                "Extended Properties=Excel 8.0;";

                    OleDbDataAdapter dataAdapter = null;
                    DataSet myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            con.Open();
                            DataTable dt = con.GetSchema("Tables");
                            string firstSheet = dt.Rows[0]["TABLE_NAME"].ToString();

                            if (string.IsNullOrEmpty(firstSheet))
                            {
                                throw new Exception("No se encontro el nombre de la primera hoja del Excel");
                            }
                            else
                            {
                                dataAdapter = new OleDbDataAdapter("SELECT * FROM [" + firstSheet + "]", con);
                                dataAdapter.Fill(myDataSet);
                                dataTable = myDataSet.Tables["Table"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.StateMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       CountryName = r.Field<object>("CountryName"),
                                       StateName = r.Field<object>("StateName")
                                   };

                    stateViewDTO = new GenericViewDTO<State>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            var newState = new State();

                            if (!ValidateIsNotNull(item.StateName))
                            {
                                errorUp = "StateName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newState.Name = item.StateName.ToString().Trim().ToUpper();
                            }

                            if (!ValidateIsNotNull(item.CountryName))
                            {
                                errorUp = "StateName " + item.StateName + " - CountryName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newState.country = new Country();
                                newState.country.Name = item.CountryName.ToString().Trim().ToUpper();
                            }

                            stateViewDTO.Entities.Add(newState);
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
                            if (stateViewDTO.Entities.Count > 0)
                            {
                                stateViewDTO = iLayoutMGR.MaintainStateMassive(stateViewDTO, context);

                                if (stateViewDTO.hasError())
                                {
                                    ShowAlertLocal(this.lblTitle.Text, stateViewDTO.Errors.Message);
                                    divFondoPopupProgress.Visible = false;
                                    divLoad.Visible = true;
                                    modalPopUpLoad.Show();
                                }
                                else
                                {
                                    ucStatus.ShowMessage(stateViewDTO.MessageStatus.Message);
                                    ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                    divFondoPopupProgress.Visible = false;
                                    divLoad.Visible = false;
                                    modalPopUpLoad.Hide();
                                }
                            }
                            else
                            {
                                ShowAlertLocal(this.lblTitle.Text, this.lblNotStatesInFile.Text);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(this.lblFormatFileNotValid.Text);
                    }
                }
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, stateViewDTO.Errors.Message);
            }
            finally
            {
                //Pregunta si existe el archivo y lo elimina
                if (File.Exists(pathAux))
                {
                    File.Delete(pathAux);
                }
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
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
                stateViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(stateViewDTO.Errors);
            }
        }

        #endregion

        #region Metodos

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
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnAddVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            Master.ucMainFilter.divCountryFilterVisible = true;
            Master.ucMainFilter.divStateFilterVisible = true;

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);

            //this.Master.ucMainFilter.nameVisible = true;
        }

        private void InitializeStatusBar()
        {
            ucStatus.pageChanged += new EventHandler(ucStatus_pageChanged);
            ucStatus.pageFirst += new EventHandler(ucStatus_pageFirst);
            ucStatus.pagePrevious += new EventHandler(ucStatus_pagePrevious);
            ucStatus.pageNext += new EventHandler(ucStatus_pageNext);
            ucStatus.pageLast += new EventHandler(ucStatus_pageLast);

            // Mensaje a mostrar en los mantenedores que no cargan inicialmente la grilla
            stateViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(stateViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void DeleteRow(int index)
        {
            stateViewDTO = (GenericViewDTO<State>)Session[WMSTekSessions.StateMgr.StateList];

            stateViewDTO = iLayoutMGR.MaintainState(CRUD.Delete, stateViewDTO.Entities[index], context);

            if (stateViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(stateViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(stateViewDTO.Errors);
                stateViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            var stateParam = new State();

            //var nameStateFilter = context.MainFilter.Where(filter => filter.Name == "Name").ToList();
            //if (nameStateFilter != null && nameStateFilter.Count > 0 && nameStateFilter.First().FilterValues.Count > 0)
            //{
            //    stateParam.Name = nameStateFilter.First().FilterValues.First().Value;
            //}

            var idCountryFilter = context.MainFilter.Where(filter => filter.Name == Master.ucMainFilter.ID_COUNTRY).ToList();
            if (idCountryFilter != null && idCountryFilter.Count > 0 && idCountryFilter.First().FilterValues.Count > 0)
            {
                stateParam.IdCountry = int.Parse(idCountryFilter.First().FilterValues.First().Value);
            }

            var idStateFilter = context.MainFilter.Where(filter => filter.Name == Master.ucMainFilter.ID_STATE).ToList();
            if (idStateFilter != null && idStateFilter.Count > 0 && idStateFilter.First().FilterValues.Count > 0)
            {
                stateParam.Id = int.Parse(idStateFilter.First().FilterValues.First().Value);
            }

            stateViewDTO = iLayoutMGR.FindAllState(stateParam, context);

            if (!stateViewDTO.hasError() && stateViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.StateMgr.StateList, stateViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(stateViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(stateViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            stateViewDTO = (GenericViewDTO<State>)Session[WMSTekSessions.StateMgr.StateList];

            if (stateViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;

            grdMgr.DataSource = stateViewDTO.Entities;
            grdMgr.DataBind();
           
            ucStatus.ShowRecordInfo(stateViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
            if (mode == CRUD.Update)
            {
                stateViewDTO = (GenericViewDTO<State>)Session[WMSTekSessions.StateMgr.StateList];

                PopulateLists();
                isNew = false;

                //Recupera los datos de la entidad a editar
                hidEditId.Value = stateViewDTO.Entities[index].Id.ToString();
                txtName.Text = stateViewDTO.Entities[index].Name;
                ddlCountry.SelectedValue = (stateViewDTO.Entities[index].IdCountry).ToString();

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }
            else if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores
                hidEditId.Value = "0";

                PopulateLists();
                isNew = true;

                txtName.Text = string.Empty;

                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
            CallJsGridView();
        }

        protected void SaveChanges()
        {
            var state = new State();
            state.Name = txtName.Text.Trim();
            state.country = new Country();
            state.country.Id = int.Parse(ddlCountry.SelectedValue);

            if (hidEditId.Value == "0")
            {
                stateViewDTO = iLayoutMGR.MaintainState(CRUD.Create, state, context);
            }
            else
            {
                state.Id = int.Parse(hidEditId.Value);
                stateViewDTO = iLayoutMGR.MaintainState(CRUD.Update, state, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (stateViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(stateViewDTO.MessageStatus.Message);
                //Actualiza grilla
                UpdateSession(false);
            }
        }

        private void PopulateLists()
        {
            base.ConfigureDDlCountry(this.ddlCountry, isNew, -1, this.Master.EmptyRowText);
        }

        private void CallJsGridView()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "grid", "afterAsyncPostBack();", true);
        }

        #endregion
    }
}