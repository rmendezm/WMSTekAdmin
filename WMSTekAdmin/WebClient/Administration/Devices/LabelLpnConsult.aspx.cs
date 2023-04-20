using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Layout;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Label;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.WebClient.Base;
using System.Xml;
using System.Configuration;
using Binaria.WMSTek.Framework.Entities.Configuration;
using System.Linq;

namespace Binaria.WMSTek.WebClient.Administration.Devices
{
    public partial class LabelLpnConsult : BasePage
    {
        #region "Declaración de Variables"

        private GenericViewDTO<LabelLpn> labelLpnViewDTO = new GenericViewDTO<LabelLpn>();
        private GenericViewDTO<CfgParameter> cfgParameterViewDTO = new GenericViewDTO<CfgParameter>();
        private GenericViewDTO<TaskLabel> taskLabelViewDTO;
        private GenericViewDTO<LabelTemplate> labelTemplateViewDTO;
        private GenericViewDTO<LPNType> lpnTypeViewDTO;
        
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
                        LoadControls();
                    }
                }
            }
            catch (Exception ex)
            {
                labelLpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLpnViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {                
                ////Realiza un renderizado de la pagina
                ScriptManager.RegisterStartupScript(Page, GetType(), "FormateaAnchoDiv", "SetDivs();", true);
            }
            catch (Exception ex)
            {
                labelLpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLpnViewDTO.Errors);
            }
        }

        protected void ddlPrinters_Change(object sender, EventArgs e)
        {
            try
            {
                ReloadLabelSize();
            }
            catch (Exception ex)
            {
                labelLpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLpnViewDTO.Errors);
            }
        }

        protected void ddlTypeLpn_Change(object sender, EventArgs e)
        {
            try
            {
                ReloadLabelStart();
            }
            catch (Exception ex)
            {
                labelLpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLpnViewDTO.Errors);
            }
        }
        

        /// <summary>
        /// Limpia el filtro y carga los datos desde base de datos
        /// </summary>
        /// 
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                chkReprint.Checked = false;
                txtLabelStart.Enabled = false;
                rfvLabelStart.Enabled = false;
                txtLabelFinish.Text = string.Empty;
                txtQtyLabel.Text = string.Empty;
                InitializeFilter(false, true);

                lpnTypeViewDTO = iWarehousingMGR.FindLpnTypeById(Convert.ToInt32(ddlTypeLpn.SelectedValue.ToString()), context);

                if (!lpnTypeViewDTO.hasError() && lpnTypeViewDTO.Entities.Count > 0)
                    txtLabelStart.Text = lpnTypeViewDTO.Entities.First().NextAvailableNumber.ToString();

                ucStatus.ShowMessage(baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Ready, context)).Message);
            }
            catch (Exception ex)
            {
                labelLpnViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(labelLpnViewDTO.Errors);
            }
        }

        /// <summary>
        /// Inserta TaskLabel
        /// </summary>
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            bool existError = false;
            Int32 labelStart = Convert.ToInt32(txtLabelStart.Text.Trim());
            Int32 labelFinish = 0;
            int qtyCopies = 1;

            if (txtLabelFinish.Text.Trim() != string.Empty)
                labelFinish = Convert.ToInt32(txtLabelFinish.Text.Trim());

            if (txtQtycopies.Text.Trim() != string.Empty)
                qtyCopies = Convert.ToInt32(txtQtycopies.Text.Trim());

            taskLabelViewDTO = new GenericViewDTO<TaskLabel>();
            List<TaskLabel> taskLabels = new List<TaskLabel>();

            Int32 contLabel = labelStart;

            lpnTypeViewDTO = new GenericViewDTO<LPNType>();
            lpnTypeViewDTO = iWarehousingMGR.FindLpnTypeById(Convert.ToInt32(ddlTypeLpn.SelectedValue.ToString()), context);

            int NextAvailableNumber = lpnTypeViewDTO.Entities[0].NextAvailableNumber;

            if (!chkReprint.Checked && contLabel != NextAvailableNumber)
            {
                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblValidateLabelStart.Text, string.Empty);
                existError = true;
            }

            if (!existError && chkReprint.Checked)
            {
                if (contLabel > NextAvailableNumber)
                {
                    //Etiqueta de Inicio No Puede ser Mayor que la cantidad que se encuentra asociada a la BD
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorLabelStart.Text + NextAvailableNumber.ToString(), string.Empty);
                    existError = true;
                }
                else if (labelFinish > NextAvailableNumber)
                {
                    //Etiqueta de Fin No Puede ser Mayor que la cantidad que se encuentra asociada a la BD
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorLabelFinish.Text + NextAvailableNumber.ToString(), string.Empty);
                    existError = true;
                }

            }


            //labelTemplateViewDTO = iLabelMGR.GetLabelByCode("LPN", context);

            //if (labelTemplateViewDTO.hasError())
            //{
            //    this.Master.ucError.ShowError(labelTemplateViewDTO.Errors);
            //}
            //else
            if (!existError)
            {
                if (Convert.ToInt32(ddlLabelSize.SelectedValue) > 0)
                {
                    while (contLabel < labelFinish)
                    {
                        String lpn = contLabel.ToString();
                        string numberSufix = base.GetConst("LPNNumberSufix").First();
                        lpnTypeViewDTO = iWarehousingMGR.FindLpnTypeById(Convert.ToInt32(ddlTypeLpn.SelectedValue.ToString()), context);


                        string codeSufix = lpnTypeViewDTO.Entities[0].Code;

                        lpn = lpn.PadLeft(int.Parse(numberSufix), '0');
                        lpn = codeSufix + lpn;
                        //lpn = context.CfgParameters[Convert.ToInt32(CfgParameterName.PrefixLpnBarcode)].ParameterValue + lpn;

                        string xmlParam = iLabelMGR.GetForLPNSecuence(lpn);

                        TaskLabel taskLabel = new TaskLabel();

                        int Valor = Convert.ToInt32(ddlLabelSize.SelectedValue);
                        ////if (!labelTemplateViewDTO.hasError())
                        ////    Valor = Convert.ToInt32(labelTemplateViewDTO.Entities[0].Id.ToString());

                        taskLabel.LabelTemplate = new LabelTemplate();
                        taskLabel.LabelTemplate.Id = Valor;

                        taskLabel.Printer = new Printer(Convert.ToInt32(ddlPrinters.SelectedValue.ToString()));
                        taskLabel.User = new Binaria.WMSTek.Framework.Entities.Profile.User(context.SessionInfo.User.Id);

                        taskLabel.DelayPrinted = 0;
                        taskLabel.IsPrinted = false;
                        taskLabel.ParamString = xmlParam;

                        taskLabels.Add(taskLabel);

                        contLabel++;
                    }

                    taskLabelViewDTO.Entities = taskLabels;

                    if (taskLabelViewDTO.Entities.Count > 0)
                    {
                        // Crea el registro a imprimir en TaskLabel
                        taskLabelViewDTO = iLabelMGR.MaintainTaskLabel(CRUD.Create, taskLabelViewDTO.Entities, context, qtyCopies);

                        if (!taskLabelViewDTO.hasError() && !chkReprint.Checked)
                        {
                            crud = true;
                            ucStatus.ShowMessage(taskLabelViewDTO.MessageStatus.Message);
                            txtQtycopies.Text = string.Empty;

                            lpnTypeViewDTO = new GenericViewDTO<LPNType>();
                            lpnTypeViewDTO = iWarehousingMGR.FindLpnTypeById(Convert.ToInt32(ddlTypeLpn.SelectedValue.ToString()), context);

                            lpnTypeViewDTO.Entities[0].NextAvailableNumber = contLabel;
                            lpnTypeViewDTO = iWarehousingMGR.MaintainLpnType(CRUD.Update, lpnTypeViewDTO.Entities[0], context);
                            ////UPDATE de LpnNumberSequence
                            //CfgParameter CfgParameter = new CfgParameter();

                            //CfgParameter.ParameterCode = context.CfgParameters[Convert.ToInt32(CfgParameterName.LpnNumberSequence)].ParameterCode;
                            //CfgParameter.ParameterValue = contLabel.ToString();
                            //CfgParameter.DateModified = DateTime.Now;
                            //CfgParameter.UserModified = context.SessionInfo.User.UserName;

                            //cfgParameterViewDTO = iConfigurationMGR.UpdateCfgParameter(CfgParameter, context);

                            if (lpnTypeViewDTO.hasError())
                            {
                                taskLabelViewDTO.Errors = lpnTypeViewDTO.Errors;
                            }
                            //else
                            //{
                            //    context.CfgParameters[Convert.ToInt32(CfgParameterName.LpnNumberSequence)].ParameterValue = contLabel.ToString();
                            //}
                        }

                        if (taskLabelViewDTO.hasError())
                        {
                            this.Master.ucError.ShowError(taskLabelViewDTO.Errors);
                        }

                        LoadControls();
                    }
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtQtyLabel.Text = string.Empty;
            txtQtycopies.Text = "1";
            txtLabelFinish.Text = string.Empty;
            ucStatus.ClearStatus();
        }

        protected void imgBtnAddItem_Click(object sender, ImageClickEventArgs e)
        {
            Int32 Num = Convert.ToInt32(txtLabelStart.Text.Trim()) + Convert.ToInt32(txtQtyLabel.Text.Trim());
            txtLabelFinish.Text = Num.ToString();
        }

        protected void chkReprint_CheckedChanged(object sender, EventArgs e)
        {
            if (chkReprint.Checked)
            {
                txtLabelStart.Text = string.Empty;
                txtQtyLabel.Text = string.Empty;
                txtQtycopies.Text = "1";
                txtLabelFinish.Text = string.Empty;
                txtLabelStart.Enabled = true;
                rfvLabelStart.Enabled = true;
                txtLabelStart.Focus();
            }
            else
            {
                lpnTypeViewDTO = new GenericViewDTO<LPNType>();
                lpnTypeViewDTO = iWarehousingMGR.FindLpnTypeById(int.Parse(ddlTypeLpn.SelectedValue.Trim()), context);

                if (lpnTypeViewDTO.Entities.Count > 0)
                    txtLabelStart.Text = lpnTypeViewDTO.Entities[0].NextAvailableNumber.ToString();
                else
                    txtLabelStart.Text = string.Empty;

                txtQtyLabel.Text = string.Empty;
                txtQtycopies.Text = "1";
                txtLabelFinish.Text = string.Empty;
                txtLabelStart.Enabled = false;
                rfvLabelStart.Enabled = false;
                txtQtyLabel.Focus();
            }
        }

            #endregion

        #region "Métodos"

        public void Initialize()
        {
            context.SessionInfo.IdPage = "LabelLpnConsult";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            ucStatus.ShowMessage(baseControl.handleMessageStatus(new MessageStatusDTO(BaseMessageStatus.Generic.Ready, context)).Message);
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            this.Master.ucMainFilter.searchVisible = false;
            this.Master.ucMainFilter.Initialize(init, refresh);
        }

        /// <summary>
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            Master.ucTaskBar.btnRefreshVisible = true;

            Master.ucTaskBar.BtnPrintClick += new EventHandler(btnPrint_Click);
            Master.ucTaskBar.btnPrintVisible = true;
        }

        protected void LoadControls()
        {
            lblNotPrinter.Visible = false;
            this.txtQtycopies.Text = "1";
            string nroCopys = GetCfgParameter(CfgParameterName.MaxPrintedCopy.ToString());
            this.rvQtycopies.MaximumValue = nroCopys;
            this.rvQtycopies.ErrorMessage = (this.lblRangeQtyCopy.Text + "1 y " + nroCopys + ".");
            
            txtQtyLabel.Text = string.Empty;
            txtLabelFinish.Text = string.Empty;
            //txtLabelStart.Text = context.CfgParameters[Convert.ToInt32(CfgParameterName.LpnNumberSequence)].ParameterValue;
            txtLabelStart.Text = "0";

            txtLabelStart.Enabled = false;
            rfvLabelStart.Enabled = false;
            chkReprint.Checked = false;

            // Carga lista de impresoras asociadas al usuario
            base.LoadUserPrinters(this.ddlPrinters);
            base.LoadLpnType(ddlTypeLpn, false, null);

            ReloadLabelSize();
            ReloadLabelStart();

            // Selecciona impresora por defecto
            base.SelectDefaultPrinter(this.ddlPrinters);

            if (ddlPrinters.Items.Count == 0)
            {
                lblNotPrinter.Visible = true;
                txtQtyLabel.Enabled = false;
                txtQtycopies.Enabled = false;
                ddlPrinters.Enabled = false;
                Master.ucTaskBar.btnPrintEnabled = false;
            }
        }

        protected void ReloadLabelSize()
        {
            if (!String.IsNullOrEmpty(ddlPrinters.SelectedValue.Trim()))
                base.LoadLabelSize(this.ddlLabelSize, Convert.ToInt32(ddlPrinters.SelectedValue.ToString()), "LPN");
        }

        protected void ReloadLabelStart()
        {
            Int32 nextAvailableNumber = 0;

            if (!String.IsNullOrEmpty(ddlTypeLpn.SelectedValue.Trim()))
            {
                lpnTypeViewDTO = new GenericViewDTO<LPNType>();
                lpnTypeViewDTO = iWarehousingMGR.FindAllLpnType(context);//(ddlTypeLpn.SelectedValue.Trim(), context);

                foreach(LPNType typeLPN in lpnTypeViewDTO.Entities)
                {
                    if(typeLPN.Id.ToString() == ddlTypeLpn.SelectedValue)
                    {
                        nextAvailableNumber = typeLPN.NextAvailableNumber;
                    }
                    
                }

                txtLabelStart.Text = nextAvailableNumber.ToString();
                txtQtyLabel.Text = string.Empty;
                txtLabelFinish.Text = string.Empty;
            }
            else
            {
                txtLabelStart.Text = "0";
            }
        }
        #endregion
    }
}
