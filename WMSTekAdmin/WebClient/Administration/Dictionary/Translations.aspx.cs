using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Globalization;
using System.Drawing.Text;
using System.Drawing;
using System.Web.UI.WebControls;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Entities.I18N;
using Binaria.WMSTek.Framework.Entities.Profile;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;



namespace Binaria.WMSTek.WebClient.Administration.Dictionary
{
    public partial class Translations : BasePage
    {
        #region "Declaración de Variables"
        private GenericViewDTO<Language> languageViewDTO = new GenericViewDTO<Language>();
        private GenericViewDTO<Binaria.WMSTek.Framework.Entities.I18N.Dictionary> dictionaryViewDTO = new GenericViewDTO<Binaria.WMSTek.Framework.Entities.I18N.Dictionary>();
        private GenericViewDTO<Translate> translatesViewDTO = new GenericViewDTO<Translate>();
        private GenericViewDTO<Translate> translateSelectedViewDTO = new GenericViewDTO<Translate>();
        private GenericViewDTO<Module> moduleViewDTO = new GenericViewDTO<Module>();
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

        //Propiedad para saber si hay algún item seleccionado
        private bool existSelectedItems
        {
            get { return (bool)(ViewState["existSelectedItems"] ?? false); }
            set { ViewState["existSelectedItems"] = value; }
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

                    //if (!Page.IsPostBack)
                    //{
                    //    // Carga inicial del ViewDTO
                    //    UpdateSession(false);
                    //}

                    if (ValidateSession(WMSTekSessions.TranslationMgr.TranslationList))
                    {
                        translatesViewDTO = (GenericViewDTO<Translate>)Session[WMSTekSessions.TranslationMgr.TranslationList];
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    //Si es un ViewDTO valido, carga la grilla y las listas
                    if (isValidViewDTO)
                    {
                        //Es necesario cargar la grilla SIEMPRE para evitar problemas con el orden dinamico de las columnas
                       PopulateGrid();
                    }
                }

                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (base.webMode == WebMode.Normal)
                {
                    base.Page_Load(sender, e);
                }
            }
            catch (Exception ex)
            {
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
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
        //        translateViewDTO.Errors = baseControl.handleException(ex, context);
        //        this.Master.ucError.ShowError(translateViewDTO.Errors);
        //    }
        //}

        /// <summary>
        /// Persiste los cambios en la entidad (nueva o editada)
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                grdMgr.Enabled = true;
                SaveChanges();
            }
            catch (Exception ex)
            {
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva entidad
        /// </summary>
        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ImageButton btnDelete = e.Row.FindControl("btnDelete") as ImageButton;
                    ImageButton btnEdit = e.Row.FindControl("btnEdit") as ImageButton;

                    if (btnDelete != null)
                    {
                        btnDelete.OnClientClick = "if(confirm('" + lblConfirmDelete.Text + "')==false){return false;}";
                    }

                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                base.grdMgr_RowDataBound(sender, e);
                string oldValue = string.Empty;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    TextBox txtTranslate = (TextBox)e.Row.FindControl("txtTranslate") as TextBox;
                    Label lblDiccionary = (Label)e.Row.FindControl("lblIdDiccionary") as Label;
                    Label lblLanguague = (Label)e.Row.FindControl("lblIdLanguage") as Label;
                    
                    if (txtTranslate != null && lblDiccionary != null && lblLanguague != null)
                    {
                        //si esta seleccionado obtiene el id de items

                        if (txtTranslate.Text != string.Empty)//Permite Validar si la traduccion viene vacia
                        {
                            foreach (Translate translate in translatesViewDTO.Entities)
                            {
                                if (translate.Language.Id.Equals(Convert.ToInt32(lblLanguague.Text)) &&
                                    translate.Dictionary.Id.Equals(Convert.ToInt32(lblDiccionary.Text)))
                                {
                                    oldValue = translate.Dictionary.TextValue;
                                }
                            }
                            if (oldValue != string.Empty)
                            {
                                string newValue = txtTranslate.Text;

                                //Defino el font
                                Font font = new Font("Arial", 10, FontStyle.Regular);
                                Size textSizeOldValue = System.Windows.Forms.TextRenderer.MeasureText(oldValue, font);
                                Size textSizeNewValue = System.Windows.Forms.TextRenderer.MeasureText(newValue, font);

                                //verifica si el ancho de la traduccion nueva en pixeles es mayor 20% que el ancho de la traduccion antigua
                                if (textSizeNewValue.Width > (textSizeOldValue.Width + (textSizeOldValue.Width * 0.2)))
                                {
                                    System.Web.UI.WebControls.Image imgWarning = (System.Web.UI.WebControls.Image)e.Row.FindControl("imgWarning") as System.Web.UI.WebControls.Image;
                                    RequiredFieldValidator reqParameterValue1 = new RequiredFieldValidator();
                                    reqParameterValue1.ID = "reqParameterValue";
                                    reqParameterValue1.ControlToValidate = "txtTranslate";
                                    reqParameterValue1.ErrorMessage = txtTranslate.Text + " is very long";
                                    reqParameterValue1.SetFocusOnError = true;
                                    reqParameterValue1.ValidationGroup = "EditNew";
                                    reqParameterValue1.Text = " req ";
                                    reqParameterValue1.Display = ValidatorDisplay.None;
                                    imgWarning.Visible = true;
                                    imgWarning.ToolTip = lblError2.Text;
                                    TableCell reqCell1 = new TableCell();
                                    reqCell1.Controls.Add(reqParameterValue1);

                                    e.Row.Cells.Add(reqCell1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                translatesViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
            }
        }
        #endregion

        #region "Métodos"

        /// <summary>
        /// inicializa
        /// </summary>
        protected void Initialize()
        {
            context.SessionInfo.IdPage = "Translations";
            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            //InitializeCheckedRows();
            //InitializeSelectedRows();
            //SaveCheckedRows();
        }

        /// <summary>
        /// Carga en sesion la lista de la Entidad
        /// </summary>
        /// <param name="showError">Determina si se mostrara el error producido en una operacion anterior</param>
        private void UpdateSession(bool showError)
        {
            //Muestra primero el error generado en la operacion anterior
            if (showError)
            {
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
                translatesViewDTO.ClearError();
            }
            context.MainFilter = this.Master.ucMainFilter.MainFilter;


            DropDownList ddlTranslate = (DropDownList)this.Master.ucMainFilter.FindControl("ddlTranslate");
            int IdLanguaje = Convert.ToInt32(ddlTranslate.SelectedValue);

            translatesViewDTO = iLanguageMGR.GetDictionaryTranslate(context, IdLanguaje);

            if (!translatesViewDTO.hasError() && translatesViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.TranslationMgr.TranslationList, translatesViewDTO);
                isValidViewDTO = true;

                if (!crud)
                    ucStatus.ShowMessage(translatesViewDTO.MessageStatus.Message);
            }
            else
            {
                isValidViewDTO = false;
                this.Master.ucError.ShowError(translatesViewDTO.Errors);
            }

        }

        /// <summary>
        /// Configuracion inicial del Filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            //Texto Base
            this.Master.ucMainFilter.codeVisible = true;
            //Traduccion
            this.Master.ucMainFilter.nameVisible = true;
            //Lenguaje
            this.Master.ucMainFilter.translateVisible = true;
            //Modulo
            this.Master.ucMainFilter.moduleVisible = true;
            //Tipo de objeto
            this.Master.ucMainFilter.typeObjectVisible = true;
            //Propiedad
            this.Master.ucMainFilter.propertyVisible = true;
            //ObjetoContenedor
            this.Master.ucMainFilter.objectContainerVisible = true;

            //Setea el nombre de los cuadros de textos (code y name por texto base y Traducción)
            this.Master.ucMainFilter.codeLabel = lblTextBase.Text;
            this.Master.ucMainFilter.nameLabel = lblTranslate.Text;


            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.btnRefreshVisible = true;

            this.Master.ucTaskBar.BtnSaveClick += new EventHandler(btnSave_Click);
            this.Master.ucTaskBar.btnSaveVisible = true;

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

        /// <summary>
        /// Carga la grilla
        /// </summary>
        private void PopulateGrid()
        {
            grdMgr.DataSource = null;
            grdMgr.PageIndex = currentPage;

            // Configura ORDEN de las columnas de la grilla

            if (!translatesViewDTO.hasConfigurationError() && translatesViewDTO.Configuration != null && translatesViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, translatesViewDTO.Configuration);

            grdMgr.DataSource = translatesViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(translatesViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);

        }

       /// <summary>
       /// Refresh
       /// </summary>
        protected void ReloadData()
        {
            //Como no es Actualizacion, setea la variable para que muestre "Listo"
            crud = false;
            UpdateSession(false);

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }

        /// <summary>
        /// Persiste los cambios realizados en la entidad
        /// </summary>
        /// <param name="index"></param>
        protected void SaveChanges()
        {
            int count = 0;
            int count2 = 0;
            bool Equal = true;
            GenericViewDTO<Translate> translateVirtualViewDTO = new GenericViewDTO<Translate>();
            DropDownList ddlTranslate = (DropDownList)this.Master.ucMainFilter.FindControl("ddlTranslate");
            int IdLanguaje = Convert.ToInt32(ddlTranslate.SelectedValue);

            foreach (GridViewRow row in grdMgr.Rows)
            {
                if (translatesViewDTO.Entities.Count >= count2 + 1)
                {
                    Translate cfgTranslate = new Translate();
                    cfgTranslate.Dictionary = new Binaria.WMSTek.Framework.Entities.I18N.Dictionary();
                    cfgTranslate.Language = new Language();

                    //Obtiene el valor modificado
                    cfgTranslate.TextValue = ((TextBox)row.FindControl("txtTranslate")).Text;

                    //Se setea el contador, ya que es distinto el index de la grilla al index del DTO
                    int RegistrosPorPagina = grdMgr.Rows.Count;
                    int PaginaActual = currentPage + 1;
                    int TotalRegHastaPagActual = RegistrosPorPagina * PaginaActual;
                    int PosAlRegModificado = TotalRegHastaPagActual - (RegistrosPorPagina - count);
                    count2 = PosAlRegModificado;
                    //Si hay modificaciones se updatea

                    if (cfgTranslate.TextValue != string.Empty)
                    {
                        if (cfgTranslate.TextValue != translatesViewDTO.Entities[PosAlRegModificado].TextValue)
                        {
                            cfgTranslate.Language.Id = IdLanguaje;
                            cfgTranslate.Dictionary.Id = translatesViewDTO.Entities[PosAlRegModificado].Dictionary.Id;
                            cfgTranslate.CodStatus = true;
                            Equal = false;
                            translateVirtualViewDTO.Entities.Add(cfgTranslate);
                        }
                    }
                }
                count++;
            }
            if (!Equal)
            {
                if (translateVirtualViewDTO.Entities.Count > 0)
                {
                    //Se envia a verificar si se inserta o se updatea
                    translatesViewDTO = iLanguageMGR.MaintainTranslation(translateVirtualViewDTO, context);
                }
            }
            else
            {
                //Muestra mensaje que nada ha sido modificado
                ucStatus.ShowMessage(translatesViewDTO.MessageStatus.Message = this.lblError1.Text);
            }

            if (translatesViewDTO.hasError())
            {
                UpdateSession(true);
                //Muestra mensaje en la barra de status
                ucStatus.ShowMessage(translatesViewDTO.MessageStatus.Message);
            }
            else
            {
                crud = true;
                //Muestra mensaje en la barra de status
                ucStatus.ShowMessage(translatesViewDTO.MessageStatus.Message);

                //Actualiza
                UpdateSession(false);
            }
        }
        #endregion
    }
}
