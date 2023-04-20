using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.Reception;
using Binaria.WMSTek.Framework.Base;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class VendorMgr : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<Vendor> vendorViewDTO = new GenericViewDTO<Vendor>();
        private bool isValidViewDTO = false;
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

        public int IdCountry
        {
            get { return (int)(ViewState["IdCountry"] ?? -1); }
            set { ViewState["IdCountry"] = value; }
        }

        public int IdState
        {
            get { return (int)(ViewState["IdState"] ?? -1); }
            set { ViewState["IdState"] = value; }
        }

        public int IdCity
        {
            get { return (int)(ViewState["IdCity"] ?? -1); }
            set { ViewState["IdCity"] = value; }
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

                        if (isValidViewDTO)
                        {
                            base.FindAllPlaces();
                            PopulateLists();
                        }
                    }

                    if (ValidateSession(WMSTekSessions.VendorMgr.VendorList))
                    {
                        vendorViewDTO = (GenericViewDTO<Vendor>)Session[WMSTekSessions.VendorMgr.VendorList];
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
        //        vendorViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(vendorViewDTO.Errors);
        //    }
        //}

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el pais... cambiara el estado y la ciudad
                isNew = true;
                base.ConfigureDDlState(this.ddlState, isNew, IdState, Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el estado, solo cambia la ciudad.
                isNew = true;
                base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);

                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
            }
        }
        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;

                if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "Dirección 2" || e.Row.Cells[i].Text == "Direcci&#243;n 2")
                        {
                            e.Row.Cells[i].Text = this.lblGln.Text;
                        }
                    }
                }
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            this.ddlOwnerLoad.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

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
                    int idOwn = int.Parse(this.ddlOwnerLoad.SelectedValue);

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

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja1$]", connectionString);
                    DataSet myDataSet = new DataSet();
                    DataTable dataTable;

                    try
                    {
                        dataAdapter.Fill(myDataSet);
                        dataTable = myDataSet.Tables["Table"];
                    }
                    catch
                    {
                        throw new InvalidOperationException(this.lblExcelComponentDoesntExist.Text);
                    }

                    var lstExcel = from r in dataTable.AsEnumerable()
                                   select new
                                   {
                                       VendorCode = r.Field<object>("VendorCode"),
                                       VendorName = r.Field<object>("VendorName"),
                                       Address1 = r.Field<object>("Address1"),
                                       Address2 = r.Field<object>("Address2"),
                                       CountryName = r.Field<object>("CountryName"),
                                       StateName = r.Field<object>("StateName"),
                                       CityName = r.Field<object>("CityName"),
                                       Phone = r.Field<object>("Phone"),
                                       Fax = r.Field<object>("Fax"),
                                       Email = r.Field<object>("Email"),
                                       HasInspection = r.Field<object>("HasInspection"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")
                                   };

                    vendorViewDTO = new GenericViewDTO<Vendor>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            Vendor newVendor = new Vendor();

                            newVendor.Owner = new Owner(idOwn);


                            if (!ValidateIsNotNull(item.VendorCode))
                            {
                                errorUp = "VendorCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newVendor.Code = item.VendorCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.VendorName))
                            {
                                errorUp = "VendorCode " + item.VendorCode + " - VendorName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                newVendor.Name = item.VendorName.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.Address1))
                                newVendor.Address1 = item.Address1.ToString().Trim();

                            if (ValidateIsNotNull(item.Address2))
                                newVendor.Address2 = item.Address2.ToString().Trim();

                            if (ValidateIsNotNull(item.CityName))
                            {
                                newVendor.City = new City() { Name = item.CityName.ToString().Trim() };
                            }
                            else
                            {
                                newVendor.City = new City();
                            }

                            if (ValidateIsNotNull(item.StateName))
                            {
                                newVendor.State = new State() { Name = item.StateName.ToString().Trim() };
                            }
                            else
                            {
                                newVendor.State = new State();
                            }

                            if (ValidateIsNotNull(item.CountryName))
                            {
                                newVendor.Country = new Country() { Name = item.CountryName.ToString().Trim() };
                            }
                            else
                            {
                                newVendor.Country = new Country();
                            }

                            if (ValidateIsNotNull(item.Phone))
                                newVendor.Phone = item.Phone.ToString().Trim();

                            if (ValidateIsNotNull(item.Fax))
                                newVendor.Fax = item.Fax.ToString().Trim();

                            if (ValidateIsNotNull(item.Email))
                                newVendor.Email = item.Email.ToString().Trim();


                            if (ValidateIsNotNull(item.HasInspection))
                            {
                                bool HasInspection;

                                if (item.HasInspection.ToString().Trim() == "0")
                                {
                                    newVendor.HasInspection = false;
                                }
                                else if (item.HasInspection.ToString().Trim() == "1")
                                {
                                    newVendor.HasInspection = true;
                                }
                                else
                                {
                                    if (bool.TryParse(item.HasInspection.ToString().Trim(), out HasInspection))
                                    {
                                        newVendor.HasInspection = HasInspection;
                                    }
                                    else
                                    {
                                        errorUp = "VendorCode " + item.VendorCode + " - HasInspection " + this.lblFieldInvalid.Text;
                                        break;
                                    }
                                }
                            }

                            if (ValidateIsNotNull(item.SpecialField1))
                                newVendor.SpecialField1 = item.SpecialField1.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField2))
                                newVendor.SpecialField2 = item.SpecialField2.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField3))
                                newVendor.SpecialField3 = item.SpecialField3.ToString().Trim();

                            if (ValidateIsNotNull(item.SpecialField4))
                                newVendor.SpecialField4 = item.SpecialField4.ToString().Trim();

                            vendorViewDTO.Entities.Add(newVendor);
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
                        if (vendorViewDTO.Entities.Count > 0)
                        {
                            vendorViewDTO = iWarehousingMGR.MaintainVendorMassive(vendorViewDTO, context);

                            if (vendorViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, vendorViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(vendorViewDTO.MessageStatus.Message);
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
            catch (InvalidOperationException ex)
            {
                ShowAlertLocal(this.lblTitle.Text, ex.Message);
                divFondoPopupProgress.Visible = false;
                divLoad.Visible = true;
                modalPopUpLoad.Show();
            }
            catch (Exception ex)
            {
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, vendorViewDTO.Errors.Message);
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

            context.MainFilter = this.Master.ucMainFilter.MainFilter;
        }

        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
                vendorViewDTO.ClearError();
            }

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            vendorViewDTO = iWarehousingMGR.FindAllVendor(context);

            if (!vendorViewDTO.hasError() && vendorViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.VendorMgr.VendorList, vendorViewDTO);
                Session.Remove(WMSTekSessions.Shared.VendorList);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(vendorViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.ConfigureDDlCountry(this.ddlCountry, isNew, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlState, isNew, IdState, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCity, isNew, IdCity, IdState, IdCountry, this.Master.EmptyRowText);
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);;
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla
            if (!vendorViewDTO.hasConfigurationError() && vendorViewDTO.Configuration != null && vendorViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, vendorViewDTO.Configuration);

            grdMgr.DataSource = vendorViewDTO.Entities;
            grdMgr.DataBind();

            
            ucStatus.ShowRecordInfo(vendorViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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

            //Si el parametro GS1 esta activado se activa nuevo filtro de busqueda por GLN
            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
            {
                this.Master.ucMainFilter.codeNumericVisible = true;
                this.Master.ucMainFilter.codeNumericLabel = this.lblGln.Text;
            }

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
            this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnAddVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnAddToolTip = this.lblAddLoadToolTip.Text;
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
                IdCountry = vendorViewDTO.Entities[index].Country.Id;
                IdState = vendorViewDTO.Entities[index].State.Id;
                IdCity = vendorViewDTO.Entities[index].City.Id;
                PopulateLists();
                isNew = false;

                //Recupera los datos de la entidad a editar
                hidEditId.Value = vendorViewDTO.Entities[index].Id.ToString();

                txtName.Text = vendorViewDTO.Entities[index].Name;
                txtCode.Text = vendorViewDTO.Entities[index].Code;
                txtAddress1.Text = vendorViewDTO.Entities[index].Address1;
                txtAddress2.Text = vendorViewDTO.Entities[index].Address2;
                txtEmail.Text = vendorViewDTO.Entities[index].Email;

                ddlCountry.SelectedValue = (IdCountry > 0) ? vendorViewDTO.Entities[index].Country.Id.ToString() : "-1";
                ddlState.SelectedValue = (IdState > 0) ? vendorViewDTO.Entities[index].State.Id.ToString() : "-1";
                ddlCity.SelectedValue = (IdCity > 0) ? vendorViewDTO.Entities[index].City.Id.ToString() : "-1";

                txtPhone.Text = vendorViewDTO.Entities[index].Phone;
                txtFax.Text = vendorViewDTO.Entities[index].Fax;
                txtEmail.Text = vendorViewDTO.Entities[index].Email;
                ddlOwner.SelectedValue = (vendorViewDTO.Entities[index].Owner.Id).ToString();
                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Proveedor
            if (mode == CRUD.Create)
            {
                PopulateLists();
                IdCountry = -1;
                IdState = -1;
                IdCity = -1;
                isNew = true;

                hidEditId.Value = "0";

                // Selecciona owner seleccionado en el Filtro
                
                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;

                txtName.Text = string.Empty;
                txtCode.Text = string.Empty;
                txtAddress1.Text = string.Empty;
                txtAddress2.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtFax.Text = string.Empty;
                txtEmail.Text = string.Empty;
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
            {
                this.lblAddress2.Text = this.lblGln.Text;
            }

            if (vendorViewDTO != null && vendorViewDTO.Configuration != null && vendorViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(vendorViewDTO.Configuration, true);
                else
                    base.ConfigureModal(vendorViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {            
            Vendor vendor = new Vendor(Convert.ToInt32(hidEditId.Value));

            vendor.Code = txtCode.Text.Trim();
            vendor.Name = txtName.Text.Trim();
            vendor.Address1 = txtAddress1.Text.Trim();

            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))) && !string.IsNullOrEmpty(txtAddress2.Text.Trim()))
            {
                string resultGLN = ValidateCodeGLN(this.txtAddress2.Text.Trim());

                if (string.IsNullOrEmpty(resultGLN))
                {
                    vendor.Address2 = txtAddress2.Text.Trim();
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    //CustomValidator val = new CustomValidator();
                    val.ErrorMessage = resultGLN;
                    val.ControlToValidate = "ctl00$MainContent$txtAddress2";
                    val.IsValid = false;
                    val.ValidationGroup = "EditNew";
                    this.Page.Validators.Add(val);

                    //rfvBarCode.IsValid = false;
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                    return;
                }
            }
            else
            {
                vendor.Address2 = txtAddress2.Text.Trim();
            }

            vendor.Country = new Country(Convert.ToInt32(ddlCountry.SelectedValue));
            vendor.State = new State(Convert.ToInt32(ddlState.SelectedValue));
            vendor.City = new City(Convert.ToInt32(ddlCity.SelectedValue));
            vendor.Phone = txtPhone.Text.Trim();
            vendor.Fax = txtFax.Text.Trim();
            vendor.Email = txtEmail.Text.Trim();
            vendor.HasInspection = this.chkHasInspection.Checked;
            vendor.Owner = new Owner(Convert.ToInt32(ddlOwner.SelectedValue));

           if(hidEditId.Value == "0")
                vendorViewDTO = iWarehousingMGR.MaintainVendor(CRUD.Create, vendor, context);
           else
                vendorViewDTO = iWarehousingMGR.MaintainVendor(CRUD.Update, vendor, context);

           divEditNew.Visible = false;
           modalPopUp.Hide();

           if (vendorViewDTO.hasError())
           {
               UpdateSession(true);
               divEditNew.Visible = true;
               modalPopUp.Show();
           }
           else
           {
               crud = true;
               ucStatus.ShowMessage(vendorViewDTO.MessageStatus.Message);

               UpdateSession(false);
           }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            //Valida que no exista un Documento Asociado a un Proveedor
            int idVendor = vendorViewDTO.Entities[index].Id;
            GenericViewDTO<InboundOrder> orderViewDTO = iReceptionMGR.GetInboundByVendor(idVendor, context);
            if (orderViewDTO.hasError())
            {
                this.Master.ucError.ShowError(orderViewDTO.Errors);
            }
            else
            {
                if (orderViewDTO.Entities != null && orderViewDTO.Entities.Count > 0)
                {
                    orderViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Vendor.InvalidDelete.Inbound, context));
                    //orderViewDTO.Errors.Level = ErrorLevel.Warning;
                    //orderViewDTO.Errors.Title = this.lblTitleDeleteError.Text;
                    //orderViewDTO.Errors.Message = this.lblMessajeDeleteError.Text;
                    this.Master.ucError.ShowError(orderViewDTO.Errors);
                }
                else
                {

                    vendorViewDTO = iWarehousingMGR.MaintainVendor(CRUD.Delete, vendorViewDTO.Entities[index], context);

                    if (vendorViewDTO.hasError())
                        UpdateSession(true);
                    else
                    {
                        crud = true;
                        ucStatus.ShowMessage(vendorViewDTO.MessageStatus.Message);

                        UpdateSession(false);
                    }
                }
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
                vendorViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(vendorViewDTO.Errors);
            }
        }                

        #endregion
    }
}