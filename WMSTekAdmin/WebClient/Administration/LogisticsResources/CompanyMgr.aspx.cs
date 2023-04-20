using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using System.Data;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Base;
using System.Text.RegularExpressions;

namespace Binaria.WMSTek.WebClient.Administration.LogisticsResources
{
    public partial class CompanyMgr : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Company> companyViewDTO = new GenericViewDTO<Company>();
        private bool isValidViewDTO = false;
        

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
                InitializeTaskBar();
                InitializeFilter();
            }
            catch (Exception ex)
            {
                companyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!this.Page.IsPostBack)
                {
                    // Si no esta en modo Configuration, sigue el curso normal
                    if (base.webMode == WebMode.Normal) Initialize();
                }
            }
            catch (Exception ex)
            {
                companyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSession(false);
                PopulateData();   
            }
            catch (Exception ex)
            {
                companyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }         
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveChanges();
            }
            catch (Exception ex)
            {
                companyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el pais... cambiara el estado y la ciudad
                base.ConfigureDDlState(this.ddlState, true, -1, Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
                base.ConfigureDDlCity(this.ddlCity, true, -1, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                companyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //si cambia el estado, solo cambia la ciudad.
                base.ConfigureDDlCity(this.ddlCity, true, IdCity, Convert.ToInt32(this.ddlState.SelectedValue), Convert.ToInt32(this.ddlCountry.SelectedValue), this.Master.EmptyRowText);
            }
            catch (Exception ex)
            {
                companyViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        #endregion

        #region "Métodos"

        protected void Initialize()
        {
            if (ValidateSession(WMSTekSessions.CompanyMgr.Company))
            {
                companyViewDTO = (GenericViewDTO<Company>)Session[WMSTekSessions.CompanyMgr.Company];
                if (companyViewDTO != null)
                {
                    if (!companyViewDTO.hasError() && companyViewDTO.Entities.Count > 0)
                    {
                        isValidViewDTO = true;
                    }
                }
            }

            else
                UpdateSession(false);


            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                this.IdCity = companyViewDTO.Entities[0].City.Id;
                this.IdCountry = companyViewDTO.Entities[0].Country.Id;
                this.IdState = companyViewDTO.Entities[0].State.Id;
                PopulateData();
            }
            else
            {
                ErrorDTO error = new ErrorDTO("Debe haber al menos una compañia");
                companyViewDTO.Errors = baseControl.handleError(error);
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
            if (!Page.IsPostBack)
            {
                base.FindAllPlaces();
                PopulateLists();
            }
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.BtnSaveClick += new EventHandler(btnSave_Click);

            Master.ucTaskBar.btnRefreshVisible = true;
            Master.ucTaskBar.btnSaveVisible = true;
        }

        private void InitializeFilter()
        {
            Master.ucMainFilter.searchVisible = false;
        }

        /// <summary>
        /// Carga en sesion la lista de companys
        /// </summary>
        private void UpdateSession(bool showError)
        {
            // Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(companyViewDTO.Errors);
                companyViewDTO.ClearError();
            }

            // Carga lista de compañias, paises y estados.
            companyViewDTO = iLayoutMGR.FindAllCompany(context);

            //si tiene registros entonces se cargaran los ddls con el valor que tiene el objeto
            if (!companyViewDTO.hasError() && companyViewDTO.Entities != null && companyViewDTO.Entities.Count > 0)
            {
                //carga los codigos de pais, estado y ciudad para luego seleccionar en los ddls
                this.IdCity = companyViewDTO.Entities[0].City.Id;
                this.IdCountry = companyViewDTO.Entities[0].Country.Id;
                this.IdState = companyViewDTO.Entities[0].State.Id;

                Session.Add(WMSTekSessions.CompanyMgr.Company, companyViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(companyViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                if (companyViewDTO.hasError()) this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        private void PopulateData()
        {
            companyViewDTO = (GenericViewDTO<Company>)Session[WMSTekSessions.CompanyMgr.Company];

            if (companyViewDTO.Entities != null && companyViewDTO.Entities.Count == 1)
            {
                this.lblCompanyNameTitle.Text = companyViewDTO.Entities[0].Name;

                this.txtIdCompany.Text = companyViewDTO.Entities[0].Id.ToString();
                this.txtCompanyName.Text = companyViewDTO.Entities[0].Name;
                this.txtShortCompanyName.Text = companyViewDTO.Entities[0].ShortName;
                this.txtTradeName.Text = companyViewDTO.Entities[0].TradeName;
                this.txtCompanyCode.Text = companyViewDTO.Entities[0].Code;
                this.txtAddress1.Text = companyViewDTO.Entities[0].Address1;
                this.txtAddress2.Text = companyViewDTO.Entities[0].Address2;
                this.txtPhone1.Text = companyViewDTO.Entities[0].Phone1;
                this.txtPhone2.Text = companyViewDTO.Entities[0].Phone2;
                this.txtFax1.Text = companyViewDTO.Entities[0].Fax1;
                this.txtFax2.Text = companyViewDTO.Entities[0].Fax2;
                this.txtEmail.Text = companyViewDTO.Entities[0].Email;
                this.txtZipCode.Text = companyViewDTO.Entities[0].ZipCode;
                this.txtGLN.Text = companyViewDTO.Entities[0].GLN;

                // TODO: habilitar los combos de las reglas

                if (companyViewDTO.Configuration != null) base.ConfigureModal(companyViewDTO.Configuration, false);
            }
            else
            {
                companyViewDTO.Errors = baseControl.handleError(new ErrorDTO(WMSTekError.BusinessAdmin.Validation.Company.CompanyRequired, context));
                this.Master.ucError.ShowError(companyViewDTO.Errors);
            }
        }

        private void PopulateLists()
        {
            base.ConfigureDDlCountry(this.ddlCountry, false, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlState(this.ddlState, false, IdState, IdCountry, this.Master.EmptyRowText);
            base.ConfigureDDlCity(this.ddlCity, false, IdCity, IdState, IdCountry, this.Master.EmptyRowText);
        }

        private void CleanControls()
        {
            this.txtIdCompany.Text = string.Empty;
            this.txtCompanyName.Text = string.Empty;
            this.txtShortCompanyName.Text = string.Empty;
            this.txtTradeName.Text = string.Empty;
            this.txtCompanyCode.Text = string.Empty;
            this.txtAddress1.Text = string.Empty;
            this.txtAddress2.Text = string.Empty;
            this.txtPhone1.Text = string.Empty;
            this.txtPhone2.Text = string.Empty;
            this.txtFax1.Text = string.Empty;
            this.txtFax2.Text = string.Empty;
            this.txtEmail.Text = string.Empty;
            this.txtZipCode.Text = string.Empty;
            this.txtGLN.Text = string.Empty;
            this.ddlCity.ClearSelection(); 
            this.ddlCountry.ClearSelection();
            this.ddlState.ClearSelection();
        }

        protected void SaveChanges()
        {
            Company company = new Company();
            
            company.Id = Convert.ToInt32(this.txtIdCompany.Text);
            company.Code = this.txtCompanyCode.Text;
            company.Name = this.txtCompanyName.Text;
            company.ShortName = this.txtShortCompanyName.Text;
            company.TradeName = this.txtTradeName.Text;
            company.Address1 = this.txtAddress1.Text;
            company.Address2 = this.txtAddress2.Text;
            company.Country = new Country(Convert.ToInt32(this.ddlCountry.SelectedValue));
            company.State = new State(Convert.ToInt32(this.ddlState.SelectedValue));
            company.City = new City(Convert.ToInt32(this.ddlCity.SelectedValue));
            company.Phone1 = this.txtPhone1.Text;
            company.Phone2 = this.txtPhone2.Text;
            company.Fax1 = this.txtFax1.Text;
            company.Fax2 = this.txtFax2.Text;
            company.Email = this.txtEmail.Text;
            company.ZipCode = this.txtZipCode.Text;

            if(!string.IsNullOrEmpty(txtGLN.Text.Trim()))
            {
                string resultGLN = ValidateCodeGLN(this.txtGLN.Text.Trim());

                if (string.IsNullOrEmpty(resultGLN))
                {
                    company.GLN = txtGLN.Text;
                }
                else
                {
                    RequiredFieldValidator val = new RequiredFieldValidator();
                    val.ErrorMessage = resultGLN;
                    val.ControlToValidate = "ctl00$MainContent$txtGLN";
                    val.IsValid = false;
                    val.ValidationGroup = "EditNew";
                    this.Page.Validators.Add(val);
                    revtxtGLN.IsValid = false;
                    revtxtGLN.Visible = false;
                    revtxtGLN.Validate();
                    //revtxtGLN.ErrorMessage = "";

                    return;
                }

            }
                                                    
            companyViewDTO = iLayoutMGR.MaintainCompany(company, context);

            if (companyViewDTO.hasError())
            {
                UpdateSession(true);
            }
            else
            {
                crud = true;
                ucStatus.ShowMessage(companyViewDTO.MessageStatus.Message);
                ViewState.Remove("IdCountry");
                ViewState.Remove("IdState");
                ViewState.Remove("IdCity");
                Session.Remove(WMSTekSessions.CompanyMgr.Company);
                CleanControls();
                UpdateSession(false);
                PopulateData();
                PopulateLists();
            }

        }
                
        #endregion
    }
}