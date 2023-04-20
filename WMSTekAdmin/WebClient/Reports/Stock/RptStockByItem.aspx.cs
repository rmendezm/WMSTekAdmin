﻿using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.Reporting.WebForms;

using Binaria.WMSTek.Framework.Base;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Entities.Devices;
using Binaria.WMSTek.Framework.Entities.Display;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.WebClient.Shared;
using Binaria.WMSTek.Framework.Entities.Base;

namespace Binaria.WMSTek.WebClient.Reports.Stock
{
    public partial class RptStockByItem : BasePage
    {
        List<ReportParameter> reportParameter = null;
        GenericViewDTO<Auxiliary> reportViewDTO = new GenericViewDTO<Auxiliary>();
        public FilterReport ucMainFilterReport
        {
            get { return this.ucFilterReport; }
        }
        //Propiedad para obtener el id del owner seleccionado
        public int currentIdOwner
        {
            get
            {
                if (ValidateSession("idOwn"))
                    return (int)Session["idOwn"];
                else
                    return 4;//TODO: Despues cambiar este valor a -1
            }

            set { Session["idOwn"] = value; }
        }
        //Propiedad para obtener el id de groupItem1
        private int idGrpItem1
        {
            get { return (int)(ViewState["idGrpItem1"] ?? -1); }
            set { ViewState["idGrpItem1"] = value; }
        }
        //Propiedad para obtener el id de groupItem2
        private int idGrpItem2
        {
            get { return (int)(ViewState["idGrpItem2"] ?? -1); }
            set { ViewState["idGrpItem2"] = value; }
        }
        //Propiedad para obtener el id de groupItem3
        private int idGrpItem3
        {
            get { return (int)(ViewState["idGrpItem3"] ?? -1); }
            set { ViewState["idGrpItem3"] = value; }
        }
        //Propiedad para obtener el id de groupItem4
        private int idGrpItem4
        {
            get { return (int)(ViewState["idGrpItem4"] ?? -1); }
            set { ViewState["idGrpItem4"] = value; }
        }

        protected override void Page_Init(object sender, EventArgs e)
        {
            try
            {
                base.Page_Init(sender, e);

                // Si no esta en modo Configuration, sigue el curso normal
                if (base.webMode == WebMode.Normal)
                {
                    this.ucMainFilterReport.emptyRowLabelText = "Seleccione";
                    Initialize();
                    divReport.Visible = false;
                }
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            try
            {
                CallJsGridViewHeader();
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        /// <summary>
        /// Carga la grilla, filtrada por el criterio de busqueda ingresado
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
               LoadReport();
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        protected void Initialize()
        {
            InitializeFilter(!Page.IsPostBack, false);
        }
        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.Visible = false;
            this.ucMainFilterReport.Visible = true;
            this.ucMainFilterReport.ownerVisible = true;
            this.ucMainFilterReport.warehouseVisible = true;
            this.ucMainFilterReport.categoryItemVisible = true;
            this.ucMainFilterReport.divBscGroupItem = true;
            
            this.ucMainFilterReport.Initialize(init, refresh);
            this.ucMainFilterReport.BtnSearchClick += new EventHandler(btnSearch_Click);
        }

        private void LoadLists()
        {
            try
            {
                DropDownList ddlgrp1 = (DropDownList)base.FindControl("ddlGrpItem1");
                DropDownList ddlgrp2 = (DropDownList)base.FindControl("ddlGrpItem2");
                DropDownList ddlgrp3 = (DropDownList)base.FindControl("ddlGrpItem3");
                DropDownList ddlgrp4 = (DropDownList)base.FindControl("ddlGrpItem4");

                base.ConfigureDDLGrpItem1(ddlgrp1, true, idGrpItem1, this.Master.EmptyRowText, false, currentIdOwner);
                base.ConfigureDDLGrpItem2(ddlgrp2, true, idGrpItem1, idGrpItem2, this.Master.EmptyRowText, false, currentIdOwner);
                base.ConfigureDDLGrpItem3(ddlgrp3, true, idGrpItem1, idGrpItem2, idGrpItem3, this.Master.EmptyRowText, false, currentIdOwner);
                base.ConfigureDDLGrpItem4(ddlgrp4, true, idGrpItem1, idGrpItem2, idGrpItem3, idGrpItem4, this.Master.EmptyRowText, false, currentIdOwner);
            }
            catch (Exception ex)
            {
                reportViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(reportViewDTO.Errors);
            }
        }

        private void LoadReport()
        {
            // Le indicamos la carpeta y el informe sin la extensión de este.
            this.rptViewStockByItem.ServerReport.ReportPath = MiscUtils.ReadSetting("ReportStockByItemPath", string.Empty);

            // Y el servidor donde está alojado el informe.
            this.rptViewStockByItem.ServerReport.ReportServerUrl = new System.Uri(MiscUtils.ReadSetting("reportingServiceAddress", string.Empty));

            //Credenciales
            int total = rptViewStockByItem.ServerReport.GetDataSources().Count;
            DataSourceCredentials[] permisos = new DataSourceCredentials[total];

            ReportDataSourceInfoCollection datasources = rptViewStockByItem.ServerReport.GetDataSources();
            for (int j = 0; j < total; j++)
            {
                permisos[j].Name = datasources[j].Name;
                permisos[j].UserId = "";
                permisos[j].Password = "";
            }
            rptViewStockByItem.ServerReport.SetDataSourceCredentials(permisos);


            //Capturo los valores de los filtros
            DropDownList ddlWhs = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterWarehouse");
            DropDownList ddlOwn = (DropDownList)this.ucMainFilterReport.FindControl("ddlFilterOwner");
            DropDownList ddlCtgItem = (DropDownList)this.ucMainFilterReport.FindControl("ddlCategoryItem");
            DropDownList ddlGrp1 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm1");
            DropDownList ddlGrp2 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm2");
            DropDownList ddlGrp3 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm3");
            DropDownList ddlGrp4 = (DropDownList)this.ucMainFilterReport.FindControl("ddlBscGrpItm4");


            string idWhs = ddlWhs.SelectedValue.ToString();
            string idOwn = ddlOwn.SelectedValue.ToString();
            string idCtgItem = ddlCtgItem.SelectedValue.ToString();
            string idGrp1 = ddlGrp1.SelectedValue.ToString();
            string idGrp2 = ddlGrp2.SelectedValue.ToString();
            string idGrp3 = ddlGrp3.SelectedValue.ToString();
            string idGrp4 = ddlGrp4.SelectedValue.ToString();

            // Creo una colección de parámetros de tipo ReportParameter 
            // para añadirlos al control ReportViewer.
            reportParameter = new List<ReportParameter>();
            reportParameter.Clear();

            // Añado los parámetros necesarios.
            reportParameter.Add(new ReportParameter("IdWhs", idWhs, false));
            reportParameter.Add(new ReportParameter("IdOwn", idOwn, false));
            reportParameter.Add(new ReportParameter("IdCtgItem", idCtgItem, false));
            reportParameter.Add(new ReportParameter("IdGrpItem1", idGrp1, false));
            reportParameter.Add(new ReportParameter("IdGrpItem2", idGrp2, false));
            reportParameter.Add(new ReportParameter("IdGrpItem3", idGrp3, false));
            reportParameter.Add(new ReportParameter("IdGrpItem4", idGrp4, false));
            reportParameter.Add(new ReportParameter("StringConectionLinkedServerBD", MiscUtils.ReadSetting("RSStringConectionLinkedServerBD", string.Empty), false));

            //DandoFormato a los margenes del informe
            rptViewStockByItem.Attributes.Add("style", "margin-bottom: 50px;");
            rptViewStockByItem.ShowPrintButton = false;
            // Añado el/los parámetro/s al ReportViewer.
            this.rptViewStockByItem.ServerReport.SetParameters(reportParameter);

            rptViewStockByItem.DataBind();

            divReport.Visible = true;

            rptViewStockByItem.ServerReport.Refresh();

            CallJsGridViewHeader();
        }

        private void CallJsGridViewHeader()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "gridHeader", "resizeDiv();", true);
        }
    }
}
