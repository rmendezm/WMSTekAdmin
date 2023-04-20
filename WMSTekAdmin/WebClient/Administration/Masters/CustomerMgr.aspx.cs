using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;

namespace Binaria.WMSTek.WebClient.Administration.Masters
{
    public partial class CustomerMgr : BasePage
    {
        #region "Declaracion de variables"

        private GenericViewDTO<Customer> customerViewDTO;
        private bool isValidViewDTO = false;
        private bool isNew = false;
        

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

        public int currentIndex
        {
            get
            {
                if (ValidateViewState("index"))
                    return (int)ViewState["index"];
                else
                    return -1;
            }

            set { ViewState["page"] = value; }
        }

        public int IdCountryDelivery
        {
            get { return (int)(ViewState["IdCountryDelivery"] ?? -1); }
            set { ViewState["IdCountryDelivery"] = value; }
        }

        public int IdStateDelivery
        {
            get { return (int)(ViewState["IdStateDelivery"] ?? -1); }
            set { ViewState["IdStateDelivery"] = value; }
        }

        public int IdCityDelivery
        {
            get { return (int)(ViewState["IdCityDelivery"] ?? -1); }
            set { ViewState["IdCityDelivery"] = value; }
        }

        public int IdCountryFact
        {
            get { return (int)(ViewState["IdCountryFact"] ?? -1); }
            set { ViewState["IdCountryFact"] = value; }
        }

        public int IdStateFact
        {
            get { return (int)(ViewState["IdStateFact"] ?? -1); }
            set { ViewState["IdStateFact"] = value; }
        }

        public int IdCityFact
        {
            get { return (int)(ViewState["IdCityFact"] ?? -1); }
            set { ViewState["IdCityFact"] = value; }
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
                        if (isValidViewDTO)
                        {
                            base.FindAllPlaces();
                            PopulateLists();
                        }

                        this.tabGeneral.HeaderText = this.lbltabGeneral.Text;
                        this.TabDelivery.HeaderText = this.lblTabDelivery.Text;
                        this.TabSales.HeaderText = this.lblTabSales.Text;
                    }

                    if (ValidateSession(WMSTekSessions.CustomerMgr.CustomerList))
                    {
                        customerViewDTO = (GenericViewDTO<Customer>)Session[WMSTekSessions.CustomerMgr.CustomerList];
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                //this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                Page.Form.Attributes.Add("enctype", "multipart/form-data");
            }
            catch (Exception ex)
            {
                customerViewDTO.Errors = baseControl.handleException(ex, context);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
        //        customerViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(customerViewDTO.Errors);
        //    }
        //}

        protected void ddlCountryDelivery_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais... cambiará el estado y la ciudad
                isNew = true;
                IdStateDelivery = 0;
                IdCityDelivery = 0;
                base.ConfigureDDlState(this.ddlStateDelivery, isNew, IdStateDelivery, Convert.ToInt32(this.ddlCountryDelivery.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCityDelivery, isNew, IdCityDelivery, Convert.ToInt32(this.ddlStateDelivery.SelectedValue), Convert.ToInt32(this.ddlCountryDelivery.SelectedValue), this.Master.EmptyRowText);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
            }
        }

        protected void ddlStateDelivery_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                isNew = true;
                IdCityDelivery = 0;
                base.ConfigureDDlCity(this.ddlCityDelivery, isNew, IdCityDelivery, Convert.ToInt32(this.ddlStateDelivery.SelectedValue), Convert.ToInt32(this.ddlCountryDelivery.SelectedValue), this.Master.EmptyRowText);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
            }
        }

        protected void ddlCountryFact_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el pais... cambiará el estado y la ciudad
                isNew = true;
                IdStateFact = 0;
                IdCityFact = 0;
                base.ConfigureDDlState(this.ddlStateFact, isNew, IdStateFact, Convert.ToInt32(this.ddlCountryFact.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCityFact, isNew, IdCityFact, Convert.ToInt32(this.ddlStateFact.SelectedValue), Convert.ToInt32(this.ddlCountryFact.SelectedValue), this.Master.EmptyRowText);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
            }
        }

        protected void ddlStateFact_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Si cambia el estado, solo cambia la ciudad.
                isNew = true;
                IdCityFact = 0;
                base.ConfigureDDlCity(this.ddlCityFact, isNew, IdCityFact, Convert.ToInt32(this.ddlStateFact.SelectedValue), Convert.ToInt32(this.ddlCountryFact.SelectedValue), this.Master.EmptyRowText);
                modalPopUp.Show();
            }
            catch (Exception ex)
            {
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
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
                        if (e.Row.Cells[i].Text == "Dir. Fac. Opc.")
                        {
                            e.Row.Cells[i].Text = this.lblGln.Text;
                        }
                    }
                }
            }
        }

        protected void btnNewLoad_Click(object sender, EventArgs e)
        {
            base.LoadUserOwners(this.ddlOwnerLoad, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            //if (context.MainFilter == null)
            //{
            //    context.MainFilter = this.Master.ucMainFilter.MainFilter;
            //}

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
                                       CustomerCode = r.Field<object>("CustomerCode"),
                                       CustomerName = r.Field<object>("CustomerName"),
                                       Address1Fact = r.Field<object>("Address1Fact"),
                                       Address2Fact = r.Field<object>("Address2Fact"),
                                       CityNameFact = r.Field<object>("CityNameFact"),
                                       StateNameFact = r.Field<object>("StateNameFact"),
                                       CountryNameFact = r.Field<object>("CountryNameFact"),
                                       PhoneFact = r.Field<object>("PhoneFact"),
                                       FaxFact = r.Field<object>("FaxFact"),
                                       Address1Delv = r.Field<object>("Address1Delv"),
                                       Address2Delv = r.Field<object>("Address2Delv"),
                                       CityNameDelv = r.Field<object>("CityNameDelv"),
                                       StateNameDelv = r.Field<object>("StateNameDelv"),
                                       CountryNameDelv = r.Field<object>("CountryNameDelv"),
                                       PhoneDelv = r.Field<object>("PhoneDelv"),
                                       FaxDelv = r.Field<object>("FaxDelv"),
                                       Email = r.Field<object>("Email"),
                                       Priority = r.Field<object>("Priority"),
                                       TimeExpected = r.Field<object>("TimeExpected"),
                                       ExpirationDays = r.Field<object>("ExpirationDays"),
                                       SpecialField1 = r.Field<object>("SpecialField1"),
                                       SpecialField2 = r.Field<object>("SpecialField2"),
                                       SpecialField3 = r.Field<object>("SpecialField3"),
                                       SpecialField4 = r.Field<object>("SpecialField4")
                                   };

                    customerViewDTO = new GenericViewDTO<Customer>();

                    try
                    {
                        foreach (var item in lstExcel)
                        {
                            Customer newCustomer = new Customer();

                            newCustomer.Owner = new Owner(idOwn);

                            if (!ValidateIsNotNull(item.CustomerCode))
                            {
                                errorUp = "CustomerCode " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (item.CustomerCode.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "CustomerCode").Replace("@maxLenght", "20").Replace("@customerCode", item.CustomerCode.ToString());
                                    break;
                                }

                                newCustomer.Code = item.CustomerCode.ToString().Trim();
                            }

                            if (!ValidateIsNotNull(item.CustomerName))
                            {
                                errorUp = "CustomerCode " + newCustomer.Code + " - CustomerName " + this.lblFieldNotNull.Text;
                                break;
                            }
                            else
                            {
                                if (item.CustomerName.ToString().Length > 60)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "CustomerName").Replace("@maxLenght", "60").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.Name = item.CustomerName.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.Address1Fact))
                            {
                                if (item.Address1Fact.ToString().Length > 60)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "Address1Fact").Replace("@maxLenght", "60").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.Address1Fact = item.Address1Fact.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.Address2Fact))
                            {
                                if (item.Address1Fact.ToString().Length > 60)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "Address2Fact").Replace("@maxLenght", "60").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.Address2Fact = item.Address2Fact.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.CityNameFact))
                            {
                                if (item.CityNameFact.ToString().Length > 30)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "CityNameFact").Replace("@maxLenght", "30").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.CityFact = new City() { Name = item.CityNameFact.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.StateNameFact))
                            {
                                if (item.StateNameFact.ToString().Length > 30)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "StateNameFact").Replace("@maxLenght", "30").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.StateFact = new State() { Name = item.StateNameFact.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.CountryNameFact))
                            {
                                if (item.CountryNameFact.ToString().Length > 30)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "CountryNameFact").Replace("@maxLenght", "30").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.CountryFact = new Country() { Name = item.CountryNameFact.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.PhoneFact))
                            {
                                if (item.PhoneFact.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "PhoneFact").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.PhoneFact = item.PhoneFact.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.FaxFact))
                            {
                                if (item.FaxFact.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "FaxFact").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.FaxFact = item.FaxFact.ToString().Trim();
                            }


                            if (ValidateIsNotNull(item.Address1Delv))
                            {
                                if (item.Address1Delv.ToString().Length > 60)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "Address1Delv").Replace("@maxLenght", "60").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.Address1Delv = item.Address1Delv.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.Address2Delv))
                            {
                                if (item.Address2Delv.ToString().Length > 60)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "Address2Delv").Replace("@maxLenght", "60").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.Address2Delv = item.Address2Delv.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.CityNameDelv))
                            {
                                if (item.CityNameDelv.ToString().Length > 30)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "CityNameDelv").Replace("@maxLenght", "30").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.CityDelv = new City() { Name = item.CityNameDelv.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.StateNameDelv))
                            {
                                if (item.StateNameDelv.ToString().Length > 30)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "StateNameDelv").Replace("@maxLenght", "30").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.StateDelv = new State() { Name = item.StateNameDelv.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.CountryNameDelv))
                            {
                                if (item.CountryNameDelv.ToString().Length > 30)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "CountryNameDelv").Replace("@maxLenght", "30").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.CountryDelv = new Country() { Name = item.CountryNameDelv.ToString().Trim() };
                            }

                            if (ValidateIsNotNull(item.PhoneDelv))
                            {
                                if (item.PhoneDelv.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "PhoneDelv").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.PhoneDelv = item.PhoneDelv.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.FaxDelv))
                            {
                                if (item.FaxDelv.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "FaxDelv").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.FaxDelv = item.FaxDelv.ToString().Trim();
                            }


                            if (ValidateIsNotNull(item.Email))
                            {
                                if (item.Email.ToString().Length > 100)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "Email").Replace("@maxLenght", "100").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.Email = item.Email.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.Priority))
                            {
                                int Priority;
                                if (int.TryParse(item.Priority.ToString().Trim(), out Priority))
                                {
                                    newCustomer.Priority = Priority;
                                }
                                else
                                {
                                    errorUp = "CustomerCode " + newCustomer.Code + " - Priority " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.TimeExpected))
                            {
                                int TimeExpected;
                                if (int.TryParse(item.TimeExpected.ToString().Trim(), out TimeExpected))
                                {
                                    newCustomer.TimeExpected = TimeExpected;
                                }
                                else
                                {
                                    errorUp = "CustomerCode " + newCustomer.Code + " - TimeExpected " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.ExpirationDays))
                            {
                                int ExpirationDays;
                                if (int.TryParse(item.ExpirationDays.ToString().Trim(), out ExpirationDays))
                                {
                                    newCustomer.ExpirationDays = ExpirationDays;
                                }
                                else
                                {
                                    errorUp = "CustomerCode " + newCustomer.Code + " - ExpirationDays " + this.lblFieldInvalid.Text;
                                    break;
                                }
                            }

                            if (ValidateIsNotNull(item.SpecialField1))
                            {
                                if (item.SpecialField1.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "SpecialField1").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.SpecialField1 = item.SpecialField1.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.SpecialField2))
                            {
                                if (item.SpecialField2.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "SpecialField2").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.SpecialField2 = item.SpecialField2.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.SpecialField3))
                            {
                                if (item.SpecialField3.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "SpecialField3").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.SpecialField3 = item.SpecialField3.ToString().Trim();
                            }

                            if (ValidateIsNotNull(item.SpecialField4))
                            {
                                if (item.SpecialField4.ToString().Length > 20)
                                {
                                    errorUp = lblMaxLength.Text.Replace("@field", "SpecialField4").Replace("@maxLenght", "20").Replace("@customerCode", newCustomer.Code);
                                    break;
                                }

                                newCustomer.SpecialField4 = item.SpecialField4.ToString().Trim();
                            }

                            customerViewDTO.Entities.Add(newCustomer);
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
                        if (customerViewDTO.Entities.Count > 0)
                        {
                            customerViewDTO = iWarehousingMGR.MaintainCustomerMassive(customerViewDTO, context);

                            if (customerViewDTO.hasError())
                            {
                                ShowAlertLocal(this.lblTitle.Text, customerViewDTO.Errors.Message);
                                divFondoPopupProgress.Visible = false;
                                divLoad.Visible = true;
                                modalPopUpLoad.Show();
                            }
                            else
                            {
                                ucStatus.ShowMessage(customerViewDTO.MessageStatus.Message);
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                ShowAlertLocal(this.lblTitle.Text, customerViewDTO.Errors.Message);
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
                this.Master.ucError.ShowError(customerViewDTO.Errors);
                customerViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            customerViewDTO = iWarehousingMGR.FindAllCustomer(context);

            if (!customerViewDTO.hasError() && customerViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.CustomerMgr.CustomerList, customerViewDTO);
                isValidViewDTO = true;

                //Muestra el mensaje en la barra de estado
                if (!crud)
                    ucStatus.ShowMessage(customerViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(customerViewDTO.Errors);
            }
        }
        /// <summary>
        /// Carga las listas
        /// </summary>
        private void PopulateLists()
        {
            base.LoadUserOwners(this.ddlOwner, this.Master.EmptyRowText, "-1", true, string.Empty, false);

            base.ConfigureDDlCountry(this.ddlCountryDelivery, isNew, IdCountryDelivery, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlStateDelivery, isNew, IdStateDelivery, IdCountryDelivery, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCityDelivery, isNew, IdCityDelivery, IdStateDelivery, IdCountryDelivery, this.Master.EmptyRowText);

            base.ConfigureDDlCountry(this.ddlCountryFact, isNew, IdCountryFact, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlStateFact, isNew, IdStateFact, IdCountryFact, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCityFact, isNew, IdCityFact, IdStateFact, IdCountryFact, this.Master.EmptyRowText);
        }
        /// <summary>
        /// Carga la grilla
        /// </summary>
        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN y VISIBILIDAD de las columnas de la grilla
            if (!customerViewDTO.hasConfigurationError() && customerViewDTO.Configuration != null && customerViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, customerViewDTO.Configuration);

            grdMgr.DataSource = customerViewDTO.Entities;
            grdMgr.DataBind();


            ucStatus.ShowRecordInfo(customerViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
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
            Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            this.Master.ucTaskBar.BtnAddClick += new EventHandler(btnNewLoad_Click);

            Master.ucTaskBar.btnNewVisible = true;
            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnExcelVisible = true;
            this.Master.ucTaskBar.btnAddVisible = true;
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
            var txtFilterCode = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");
            var txtFilterName = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterName");

            if (txtFilterCode != null && txtFilterName != null)
            {
                if (string.IsNullOrEmpty(txtFilterCode.Text) && string.IsNullOrEmpty(txtFilterName.Text))
                {
                    ucStatus.ShowWarning(lblSearchWarning.Text);
                    return;
                }
            }

            //Setea crud por que solo es un select
            crud = false;
            //Actualiza la grilla
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
            currentIndex = index;
            tabCustomer.ActiveTabIndex = 0;

            // Editar Cliente
            if (mode == CRUD.Update)
            {
                IdCountryDelivery = customerViewDTO.Entities[index].CountryDelv.Id;
                IdStateDelivery = customerViewDTO.Entities[index].StateDelv.Id;
                IdCityDelivery = customerViewDTO.Entities[index].CityDelv.Id;

                IdCountryFact = customerViewDTO.Entities[index].CountryFact.Id;
                IdStateFact = customerViewDTO.Entities[index].StateFact.Id;
                IdCityFact = customerViewDTO.Entities[index].CityFact.Id;

                PopulateLists();

                isNew = false;

                //Recupera los datos de la entidad a editar
                hidEditId.Value = customerViewDTO.Entities[index].Id.ToString();
                hidEditIdB2B.Value = customerViewDTO.Entities[index].CustomerB2B.Id.ToString();

                txtName.Text = customerViewDTO.Entities[index].Name;
                txtCode.Text = customerViewDTO.Entities[index].Code;
                txtEmail.Text = customerViewDTO.Entities[index].Email;
                txtTimeExpected.Text = customerViewDTO.Entities[index].TimeExpected.ToString();
                txtExpiration.Text = customerViewDTO.Entities[index].ExpirationDays.ToString();
                if (customerViewDTO.Entities[index].Owner != null) this.ddlOwner.SelectedValue = (customerViewDTO.Entities[index].Owner.Id).ToString();

                txtPriority.Text = customerViewDTO.Entities[index].Priority.ToString();

                //Entrega
                txtDeliveryAddress1.Text = customerViewDTO.Entities[index].Address1Delv;
                txtDeliveryAddress2.Text = customerViewDTO.Entities[index].Address2Delv;
                txtDeliveryPhone.Text = customerViewDTO.Entities[index].PhoneDelv;
                txtFaxPhoneDev.Text = customerViewDTO.Entities[index].FaxDelv;
                if (customerViewDTO.Entities[index].CountryDelv != null) this.ddlCountryDelivery.SelectedValue = (customerViewDTO.Entities[index].CountryDelv.Id).ToString();
                if (customerViewDTO.Entities[index].StateDelv != null) this.ddlStateDelivery.SelectedValue = (customerViewDTO.Entities[index].StateDelv.Id).ToString();
                if (customerViewDTO.Entities[index].CityDelv != null) this.ddlCityDelivery.SelectedValue = (customerViewDTO.Entities[index].CityDelv.Id).ToString();

                //Facturacion
                txtFactAddress1.Text = customerViewDTO.Entities[index].Address1Fact;
                txtFactAddress2.Text = customerViewDTO.Entities[index].Address2Fact;
                txtFactPhone.Text = customerViewDTO.Entities[index].PhoneFact;
                txtFaxPhoneFac.Text = customerViewDTO.Entities[index].FaxFact;
                if (customerViewDTO.Entities[index].CountryFact != null) this.ddlCountryFact.SelectedValue = customerViewDTO.Entities[index].CountryFact.Id.ToString();
                if (customerViewDTO.Entities[index].StateFact != null) this.ddlStateFact.SelectedValue = customerViewDTO.Entities[index].StateFact.Id.ToString();
                if (customerViewDTO.Entities[index].CityFact != null) this.ddlCityFact.SelectedValue = customerViewDTO.Entities[index].CityFact.Id.ToString();
                
                this.txtSpecialField1.Text = customerViewDTO.Entities[index].SpecialField1;

                lblNew.Visible = false;
                lblEdit.Visible = true;
            }

            // Nuevo Cliente
            if (mode == CRUD.Create)
            {
                //General
                // Selecciona owner seleccionado en el Filtro
                IdCountryDelivery = -1;
                IdStateDelivery = -1;
                IdCityDelivery = -1;

                IdCountryFact = -1;
                IdStateFact = -1;
                IdCityFact = -1;

                isNew = true;
                PopulateLists();

                this.ddlOwner.SelectedValue = context.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues[0].Value;
                
                hidEditId.Value = "0";
                hidEditIdB2B.Value = "0";
                txtName.Text = string.Empty;
                txtCode.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtExpiration.Text = string.Empty;
                txtPriority.Text = string.Empty;
                txtTimeExpected.Text = string.Empty;

                isNew = true;

                //Entrega
                //IdCountryDelivery = -1;
                //IdStateDelivery = -1;
                //IdCityDelivery = -1;
                txtDeliveryAddress1.Text = string.Empty;
                txtDeliveryAddress2.Text = string.Empty;
                txtDeliveryPhone.Text = string.Empty;
                txtFaxPhoneDev.Text = string.Empty;
                //this.ddlCountryDelivery.SelectedValue = "-1";
                //this.ddlStateDelivery.SelectedValue = "-1";
                //this.ddlCityDelivery.SelectedValue = "-1";

                //Facturacion
                //IdCountryFact = -1;
                //IdStateFact = -1;
                //IdCityFact = -1;
                txtFactAddress1.Text = string.Empty;
                txtFactAddress2.Text = string.Empty;
                txtFactPhone.Text = string.Empty;
                txtFaxPhoneFac.Text = string.Empty;

                this.ddlCountryFact.SelectedValue = "-1";
                this.ddlStateFact.SelectedValue = "-1";
                this.ddlCityFact.SelectedValue = "-1";

                this.txtSpecialField1.Text = string.Empty;

                //De la pagina
                lblNew.Visible = true;
                lblEdit.Visible = false;
            }

            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))))
            {
                this.lblFactAddress2.Text = this.lblGln.Text;
                this.rfvFactAddress2.ErrorMessage = this.lblGlnMessageRequired.Text;
            }
            else
            {
                this.rfvFactAddress2.ErrorMessage = this.lblFactAddress2MessageRequired.Text;
            }

            if (customerViewDTO != null && customerViewDTO.Configuration != null && customerViewDTO.Configuration.Count > 0)
            {
                if (mode == CRUD.Create)
                    base.ConfigureModal(customerViewDTO.Configuration, true);
                else
                    base.ConfigureModal(customerViewDTO.Configuration, false);
            }

            divEditNew.Visible = true;
            modalPopUp.Show();
        }

        protected void SaveChanges()
        {
            Customer customer = new Customer();
            customer.Owner = new Owner();
            customer.CountryFact = new Country();
            customer.StateFact = new State();
            customer.CityFact = new City();
            customer.CountryDelv = new Country();
            customer.StateDelv = new State();
            customer.CityDelv = new City();
            customer.CustomerB2B = new CustomerB2B();

            customer.Code = txtCode.Text.Trim();
            customer.Name = txtName.Text.Trim();
            customer.Email = txtEmail.Text.Trim();
            if (!string.IsNullOrEmpty(this.txtPriority.Text))
                customer.Priority = Convert.ToInt32(txtPriority.Text.Trim());
            else
                customer.Priority = -1;
            customer.Owner.Id = Convert.ToInt32(this.ddlOwner.SelectedValue);
            if (!string.IsNullOrEmpty(this.txtExpiration.Text))
                customer.ExpirationDays = Convert.ToInt32(this.txtExpiration.Text.Trim());
            else
                customer.ExpirationDays = -1;
            if (!string.IsNullOrEmpty(this.txtTimeExpected.Text))
                customer.TimeExpected = Convert.ToInt32(this.txtTimeExpected.Text.Trim());
            else
                customer.TimeExpected = -1;
            //Delivery
            customer.Address1Delv = txtDeliveryAddress1.Text;
            if (!string.IsNullOrEmpty(this.txtDeliveryAddress2.Text))
                customer.Address2Delv = txtDeliveryAddress2.Text;
            customer.CityDelv = new City(Convert.ToInt32(this.ddlCityDelivery.SelectedValue));
            customer.StateDelv = new State(Convert.ToInt32(this.ddlStateDelivery.SelectedValue));
            customer.CountryDelv = new Country(Convert.ToInt32(this.ddlCountryDelivery.SelectedValue));
            if (!string.IsNullOrEmpty(this.txtDeliveryPhone.Text))
                customer.PhoneDelv = this.txtDeliveryPhone.Text.Trim();
            if (!string.IsNullOrEmpty(this.txtFaxPhoneDev.Text)) 
                customer.FaxDelv = this.txtFaxPhoneDev.Text.Trim();

            //Facturation
            customer.Address1Fact = txtFactAddress1.Text;

            if (Convert.ToBoolean(Convert.ToInt16(GetCfgParameter("AllowGS1Standard"))) && !string.IsNullOrEmpty(txtFactAddress2.Text.Trim()))
            {
                string resultGLN = ValidateCodeGLN(this.txtFactAddress2.Text.Trim());

                if (string.IsNullOrEmpty(resultGLN))
                {
                    if (!string.IsNullOrEmpty(this.txtFactAddress2.Text))
                        customer.Address2Fact = txtFactAddress2.Text;
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    //CustomValidator val = new CustomValidator();
                    val.ErrorMessage = resultGLN;
                    val.ControlToValidate = "ctl00$MainContent$txtFactAddress2";
                    val.IsValid = false;
                    val.ValidationGroup = "EditNew";                    
                    this.Page.Validators.Add(val);
        
                    divEditNew.Visible = true;
                    modalPopUp.Show();
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.txtFactAddress2.Text))
                    customer.Address2Fact = txtFactAddress2.Text;
            }

            customer.CityFact = new City(Convert.ToInt32(this.ddlCityFact.SelectedValue));
            customer.StateFact = new State(Convert.ToInt32(this.ddlStateFact.SelectedValue));
            customer.CountryFact = new Country(Convert.ToInt32(this.ddlCountryFact.SelectedValue));
            if (!string.IsNullOrEmpty(this.txtFactPhone.Text))
                customer.PhoneFact = this.txtFactPhone.Text.Trim();
            if (!string.IsNullOrEmpty(this.txtFaxPhoneFac.Text))
                customer.FaxFact = this.txtFaxPhoneFac.Text.Trim();

            customer.SpecialField1 = this.txtSpecialField1.Text.Trim();
                           

            if (hidEditIdB2B.Value != "0")
                customer.CustomerB2B.Id = Convert.ToInt32(hidEditIdB2B.Value);

            
            if (hidEditId.Value == "0")
            {
                customerViewDTO = iWarehousingMGR.MaintainCustomer(CRUD.Create, customer, context);
            }
            else
            {
                customer.Id = Convert.ToInt32(hidEditId.Value);
                customer.CustomerB2B.IdCustomer = Convert.ToInt32(hidEditId.Value);

                customerViewDTO = iWarehousingMGR.MaintainCustomer(CRUD.Update, customer, context);
            }

            divEditNew.Visible = false;
            modalPopUp.Hide();

            if (customerViewDTO.hasError())
            {
                UpdateSession(true);
                divEditNew.Visible = true;
                modalPopUp.Show();
            }
            else
            {
                //Muestra el mensaje en la barra de status
                crud = true;
                ucStatus.ShowMessage(customerViewDTO.MessageStatus.Message);
                //Actualiza la grilla
                UpdateSession(false);
            }
        }

        /// <summary>
        /// Elimina la entidad
        /// </summary>
        /// <param name="index">Posicion en el ViewDTO de la entidad a eliminar</param>
        private void DeleteRow(int index)
        {
            customerViewDTO = iWarehousingMGR.MaintainCustomer(CRUD.Delete, customerViewDTO.Entities[index], context);

            if (customerViewDTO.hasError())
                UpdateSession(true);
            else
            {
                //Muestra el mensaje en la barra de status
                ucStatus.ShowMessage(customerViewDTO.MessageStatus.Message);
                crud = true;
                //Actualiza la grilla
                UpdateSession(false);
            }
        }


        private void LoadTemplateAsn(DropDownList objControl)
        {
            string logPath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("B2BTemplateAsnPath", "");

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(logPath);

            objControl.Items.Clear();

            foreach (System.IO.FileInfo fi in dirInfo.GetFiles("*.*"))
            {
                objControl.Items.Insert(0, new ListItem(fi.Name,fi.FullName));
            }

            objControl.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
        }

        private void LoadTemplatePrice(DropDownList objControl)
        {
            string logPath = Request.PhysicalApplicationPath + MiscUtils.ReadSetting("B2BTemplateLabelPricePath", "");

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(logPath);
            
            objControl.Items.Clear();

            foreach (System.IO.FileInfo fi in dirInfo.GetFiles("*.*"))
            {
                objControl.Items.Insert(0, new ListItem(fi.Name, fi.FullName));
            }

            objControl.Items.Insert(0, new ListItem(this.Master.EmptyRowText, "-1"));
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
                customerViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(customerViewDTO.Errors);
            }
        }        

        #endregion
    }
}
