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

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class CountryMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Country> countryViewDTO = new GenericViewDTO<Country>();
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            divLoad.Visible = true;
            modalPopUpLoad.Show();
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
                        theLog.exceptionMessage("Binaria.WMSTek.WebClient.Administration.Masters.CountryMgr", "EXCEPTION AL CARGAR ARCHIVO " + ex.ToString());
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       CountryName = r.Field<object>("CountryName")
                                   };

                    countryViewDTO = new GenericViewDTO<Country>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            if (!IsValidExcelRow(item))
                                continue;

                            var newCountry = new Country();

                            if (!ValidateIsNotNull(item.CountryName))
                            {
                                errorUp = "CountryName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newCountry.Name = item.CountryName.ToString().Trim().ToUpper();
                            }

                            countryViewDTO.Entities.Add(newCountry);
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
                            if (countryViewDTO.Entities.Count > 0)
                            {
                                countryViewDTO = iLayoutMGR.MaintainCountryMassive(countryViewDTO, context);

                                if (countryViewDTO.hasError())
                                {
                                    ShowAlertLocal(this.lblTitle.Text, countryViewDTO.Errors.Message);
                                    divFondoPopupProgress.Visible = false;
                                    divLoad.Visible = true;
                                    modalPopUpLoad.Show();
                                }
                                else
                                {
                                    ucStatus.ShowMessage(countryViewDTO.MessageStatus.Message);
                                    ShowAlertLocal(this.lblTitle.Text, this.lblMessajeLoadOK.Text);

                                    divFondoPopupProgress.Visible = false;
                                    divLoad.Visible = false;
                                    modalPopUpLoad.Hide();
                                }
                            }
                            else
                            {
                                ShowAlertLocal(this.lblTitle.Text, this.lblNotCountriesInFile.Text);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, countryViewDTO.Errors.Message);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
                countryViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(countryViewDTO.Errors);
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
            Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnAddVisible = true;
        }

        private void InitializeFilter(bool init, bool refresh)
        {
            Master.ucMainFilter.divCountryFilterVisible = true;

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
            countryViewDTO.MessageStatus = baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Search, context));
            ucStatus.ShowMessage(countryViewDTO.MessageStatus.Message);
        }

        private void InitializeGrid()
        {
            grdMgr.PageSize = Convert.ToInt16(GetCfgParameter(CfgParameterName.MaxRecordsByGridPage.ToString()));
            grdMgr.EmptyDataText = this.Master.EmptyGridText;
        }

        private void DeleteRow(int index)
        {
            countryViewDTO = (GenericViewDTO<Country>)Session[WMSTekSessions.CountryMgr.CountryList];

            countryViewDTO = iLayoutMGR.MaintainCountry(CRUD.Delete, countryViewDTO.Entities[index], context); 

            if (countryViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(countryViewDTO.MessageStatus.Message);
                UpdateSession(false);
            }
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(countryViewDTO.Errors);
                countryViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            var countryParam = new Country();

            //var nameCountry = context.MainFilter.Where(filter => filter.Name == "Name").ToList();
            //if (nameCountry != null && nameCountry.Count > 0 && nameCountry.First().FilterValues.Count > 0)
            //{
            //    countryParam.Name = nameCountry.First().FilterValues.First().Value;
            //}

            var idCountryFilter = context.MainFilter.Where(filter => filter.Name == Master.ucMainFilter.ID_COUNTRY).ToList();
            if (idCountryFilter != null && idCountryFilter.Count > 0 && idCountryFilter.First().FilterValues.Count > 0)
            {
                countryParam.Id = int.Parse(idCountryFilter.First().FilterValues.First().Value);
            }

            countryViewDTO = iLayoutMGR.FindAllCountry(countryParam, context);

            if (!countryViewDTO.hasError() && countryViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CountryMgr.CountryList, countryViewDTO);
                isValidViewDTO = true;

                //Muestra Mensaje en barra de status
                if (!crud)
                    ucStatus.ShowMessage(countryViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(countryViewDTO.Errors);
            }

            upGrid.Update();
            CallJsGridView();
        }

        private void PopulateGrid()
        {
            countryViewDTO = (GenericViewDTO<Country>)Session[WMSTekSessions.CountryMgr.CountryList];

            if (countryViewDTO == null)
                return;

            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            //if (!countryViewDTO.hasConfigurationError() && countryViewDTO.Configuration != null && countryViewDTO.Configuration.Count > 0)
            //    base.ConfigureGridOrder(grdMgr, countryViewDTO.Configuration);

            grdMgr.DataSource = countryViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(countryViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
                countryViewDTO = (GenericViewDTO<Country>)Session[WMSTekSessions.CountryMgr.CountryList];

                //Recupera los datos de la entidad a editar
                hidEditId.Value = countryViewDTO.Entities[index].Id.ToString();
                txtName.Text = countryViewDTO.Entities[index].Name;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }
            else if (mode == CRUD.Create)
            {
                //si es nuevo Seteo valores
                hidEditId.Value = "0";

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
            var country = new Country();
            country.Name = txtName.Text.Trim();

            if (hidEditId.Value == "0")
            {
                countryViewDTO = iLayoutMGR.MaintainCountry(CRUD.Create, country, context);
            }
            else
            {
                country.Id = int.Parse(hidEditId.Value);
                countryViewDTO = iLayoutMGR.MaintainCountry(CRUD.Update, country, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (countryViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(countryViewDTO.MessageStatus.Message);
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