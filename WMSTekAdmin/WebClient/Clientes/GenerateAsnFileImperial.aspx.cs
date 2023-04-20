using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binaria.WMSTek.WebClient.Base;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Entities.Dispatching;
using Binaria.WMSTek.Framework.DataTransfer.Base;
using Binaria.WMSTek.Framework.Utils;
using Binaria.WMSTek.Framework.Entities.Funtional;
using System.Text;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data;
using Binaria.WMSTek.Framework.Entities.Warehousing;
using Binaria.WMSTek.Framework.Entities.Base;
using Binaria.WMSTek.Framework.Entities.Tasks;

namespace Binaria.WMSTek.WebClient.Clientes
{
    public partial class GenerateAsnFileImperial : BasePage
    {
        private GenericViewDTO<ImperialB2BFuntional> imperialB2BViewDTO = new GenericViewDTO<ImperialB2BFuntional>();
        private GenericViewDTO<ItemCustomer> itemCustomerViewDTO = new GenericViewDTO<ItemCustomer>();
        private GenericViewDTO<Customer> customerViewDTO = new GenericViewDTO<Customer>();
        private GenericViewDTO<TaskConsult> taskViewDTO = new GenericViewDTO<TaskConsult>();
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

        private string customerCodeImperial
        {
            get { return GetConst("CodeCustomerASNImperial")[0]; }
        }

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
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void Initialize()
        {
            context.SessionInfo.IdPage = "GenerateAsnFileImperial";

            InitializeTaskBar();
            InitializeFilter(!Page.IsPostBack, false);
            InitializeStatusBar();
            InitializeGrid();

            if (!Page.IsPostBack)
            {
                //UpdateSession();
            }
            else
            {
                if (ValidateSession(WMSTekSessions.FuntionalMgr.ImperialB2BList))
                {
                    imperialB2BViewDTO = (GenericViewDTO<ImperialB2BFuntional>)Session[WMSTekSessions.FuntionalMgr.ImperialB2BList];
                    isValidViewDTO = true;
                }
            }
        }

        /// <summary>
        /// Configuracion inicial del filtro de busqueda
        /// </summary>
        private void InitializeFilter(bool init, bool refresh)
        {
            // Habilita criterios a usar
            this.Master.ucMainFilter.warehouseNotIncludeAll = true;
            this.Master.ucMainFilter.warehouseVisible = true;
            this.Master.ucMainFilter.ownerNotIncludeAll = true;
            this.Master.ucMainFilter.ownerVisible = true;

            this.Master.ucMainFilter.codeVisible = true;
            this.Master.ucMainFilter.codeLabel = "Nro. Orden de Compra";

            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            this.Master.ucMainFilter.FilterWarehouseAutoPostBack = false;
            this.Master.ucMainFilter.FilterOwnerAutoPostBack = false;

            this.Master.ucMainFilter.Initialize(init, refresh);
            this.Master.ucMainFilter.BtnSearchClick += new EventHandler(btnSearch_Click);
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
        /// Configuracion inicial de la Barra de Tareas
        /// </summary>
        private void InitializeTaskBar()
        {
            this.Master.ucTaskBar.BtnRefreshClick += new EventHandler(btnRefresh_Click);
            this.Master.ucTaskBar.BtnPrintClick += new EventHandler(btnGenerateASN_Click);

            this.Master.ucTaskBar.btnRefreshVisible = true;
            this.Master.ucTaskBar.btnPrintVisible = true;
            this.Master.ucTaskBar.btnPrintEnabled = false;
            this.Master.ucTaskBar.btnPrintToolTip = this.lblToolTipASN.Text;

            //this.Master.ucTaskBar.BtnExcelClick += new EventHandler(btnExcel_Click);
            //this.Master.ucTaskBar.btnExcelVisible = true;
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

        protected void ucStatus_pageChanged(object sender, EventArgs e)
        {
            try
            {
                currentPage = ucStatus.SelectedPage;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void ucStatus_pageFirst(object sender, EventArgs e)
        {
            try
            {
                currentPage = 0;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void ucStatus_pagePrevious(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void ucStatus_pageNext(object sender, EventArgs e)
        {
            try
            {
                currentPage = currentPage + 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void ucStatus_pageLast(object sender, EventArgs e)
        {
            try
            {
                currentPage = grdMgr.PageCount - 1;
                currentIndex = -1;
                PopulateGrid();
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
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

                ImageButton s = (ImageButton)this.Master.ucTaskBar.FindControl("btnPrint");
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(s);
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                InitializeFilter(false, true);
                ReloadData();
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {            
            try
            {
                TextBox txtNumReference = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");

                if (string.IsNullOrEmpty(txtNumReference.Text.Trim()))
                {
                    this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorReferenceDoc.Text, "");
                }
                else
                {
                    ReloadData();
                }
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        /// <summary>
        /// Crea un Excel con los datos de toda la grilla
        /// </summary>
        protected void btnGenerateASN_Click(object sender, EventArgs e)
        {
            try
            {
                bool existsTask = false;

                if (ValidateSession(WMSTekSessions.FuntionalMgr.ImperialB2BList))
                {
                    //dispatchSpecialViewDTO = (GenericViewDTO<DispatchSpecial>)Session[WMSTekSessions.GenerateAsnFileSodimac.SelectedDispatchASN];
                    imperialB2BViewDTO = (GenericViewDTO<ImperialB2BFuntional>)Session[WMSTekSessions.FuntionalMgr.ImperialB2BList];


                    ContextViewDTO newContext = new ContextViewDTO();
                    newContext.MainFilter = new List<EntityFilter>();
                    var arrEnum = Enum.GetValues(typeof(EntityFilterName));
                    foreach (var item in arrEnum)
                    {
                        newContext.MainFilter.Add(new EntityFilter(item.ToString(), new List<FilterItem>()));
                    }

                    var lstOrders = from ord in imperialB2BViewDTO.Entities
                                    select new
                                    {
                                        outboundNumber = ord.OutboundNumber,
                                        idWhs = ord.IdWhs,
                                        idOwn = ord.IdOwn
                                    };

                    foreach (var selecImperial in lstOrders.Distinct())
                    {
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Clear();
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Clear();
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Clear();

                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.DocumentNbr)].FilterValues.Add(new FilterItem(selecImperial.outboundNumber));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Warehouse)].FilterValues.Add(new FilterItem(selecImperial.idWhs.ToString()));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.Owner)].FilterValues.Add(new FilterItem(selecImperial.idOwn.ToString()));
                        newContext.MainFilter[Convert.ToInt16(EntityFilterName.TaskType)].FilterValues.Add(new FilterItem("PAKOR"));
                        newContext.MainFilter.Add(new EntityFilter("Completed", new FilterItem("0", "0")));

                        taskViewDTO = iTasksMGR.FindAllTaskMgr(newContext);

                        if (!taskViewDTO.hasError() && taskViewDTO.Entities != null)
                        {
                            if (taskViewDTO.Entities.Count > 0)
                            {
                                existsTask = true;
                                break;
                            }
                            else
                            {
                                existsTask = false;
                            }
                        }
                        else
                        {
                            throw new Exception(taskViewDTO.Errors.Message);
                        }
                    }
                    

                    if (existsTask)
                    {
                        this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorExistTaskOrder.Text, "");
                    }
                    else
                    {
                        
                        if (imperialB2BViewDTO.Entities.Count > 0)
                        {
                            customerViewDTO = iWarehousingMGR.GetCustomerByCodeAndOwn(context, customerCodeImperial, imperialB2BViewDTO.Entities[0].IdOwn);

                            if (customerViewDTO.Entities[0].CustomerB2B == null || customerViewDTO.Entities[0].CustomerB2B.TemplateASNFile == null ||
                                customerViewDTO.Entities[0].CustomerB2B.TemplateASNFile == "")
                            {
                                this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorTemplate.Text, "");
                            }
                            else
                            {
                                string pathTemplate = customerViewDTO.Entities[0].CustomerB2B.TemplateASNFile;

                                //Carga el archivo xsd del cliente
                                XmlDocument xsd = new XmlDocument();
                                xsd.Load(pathTemplate);

                                DataSet theData = new DataSet();
                                StringReader xmlSR = new StringReader(xsd.InnerXml);
                                theData.ReadXml(xmlSR, XmlReadMode.ReadSchema);

                                foreach (var imp in imperialB2BViewDTO.Entities)
                                {
                                    DataTable tableItem = theData.Tables["record"];
                                    DataRow rowItem = tableItem.NewRow();
                                    rowItem["Nro_Orden_de_Compra"] = imp.OutboundNumber.ToString();
                                    rowItem["Nro_Guía_Proveedor"] = imp.NroGuia;
                                    rowItem["Tienda"] = imp.Store;
                                    rowItem["LPN"] = string.IsNullOrEmpty(imp.Lpn) ? "" : imp.Lpn.ToString();
                                    rowItem["Código_Producto_Imperial"] = imp.ItemCode;
                                    rowItem["Código_Producto_Proveedor"] = imp.ItemCodeCustomer;
                                    rowItem["EAN_13"] = imp.EAN13;
                                    rowItem["Unidad_Medida"] = imp.UOMCode;
                                    rowItem["Cantidad_Entrega"] = imp.ItemQty;
                                    rowItem["Lote"] = string.IsNullOrEmpty(imp.LoteNumber) ? "" : imp.LoteNumber;
                                    rowItem["Dirección_Despacho"] = string.IsNullOrEmpty(imp.DeliveryAddress) ? "" : imp.DeliveryAddress;
                                    tableItem.Rows.Add(rowItem);
                                }

                                StringBuilder str = new StringBuilder();
          
                                foreach (DataColumn col in theData.Tables[0].Columns)
                                {
                                    str.Append(col.ColumnName.Replace("_", " ") + "\t");
                                }
                                str.AppendLine();

                                foreach (DataRow row in theData.Tables[0].Rows)
                                {
                                    str.Append(row["Nro_Orden_de_Compra"].ToString() + "\t");
                                    str.Append(row["Nro_Guía_Proveedor"] + "\t");
                                    str.Append(row["Tienda"] + "\t");
                                    str.Append(row["LPN"].ToString() + "\t");
                                    str.Append(row["Código_Producto_Imperial"] + "\t");
                                    str.Append(row["Código_Producto_Proveedor"] + "\t");
                                    str.Append(row["EAN_13"].ToString() + "\t");
                                    str.Append(row["Unidad_Medida"] + "\t");
                                    str.Append(row["Cantidad_Entrega"] + "\t");
                                    str.Append(row["Lote"] + "\t");
                                    str.Append(row["Dirección_Despacho"] + "\t");
                                    str.AppendLine();
                                }                                                          

                                string attachment = "attachment; filename=Imperial-" + DateTime.Now.ToString("ddMMyy-hhmm") + ".xls";

                                Response.Clear();
                                Response.BufferOutput = true;
                                Response.Charset = "UTF-8";
                                Response.ContentEncoding = Encoding.Default;
                                //Response.ContentType = "application/vnd.ms-excel";
                                Response.ContentType = "application/excel";
                                Response.AddHeader("content-disposition", attachment);

                                //using (StreamWriter writer = new StreamWriter(Response.OutputStream))
                                //{
                                //    writer.WriteLine(str);
                                //}
      
                                Response.Write(str);

                                Response.Flush();
                                Response.End();
                            }
                        }
                        else
                        {
                            this.Master.ucDialog.ShowAlert(this.lblTitle.Text, this.lblErrorNotExistData.Text, "");
                        }
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
               server control at run time. */
        }

        protected void grdMgr_RowCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Agrega atributos para cambiar el color de la fila seleccionada
                    e.Row.Attributes.Add("onclick", "gridViewOnclick('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseover", "gridViewOnmouseover('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                    e.Row.Attributes.Add("onmouseout", "gridViewOnmouseout('" + e.Row.RowIndex + "', '" + grdMgr.ClientID + "')");
                }
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        private void PopulateGrid()
        {
            grdMgr.PageIndex = currentPage;
            grdMgr.SelectedIndex = currentIndex;

            // Configura ORDEN de las columnas de la grilla
            if (!imperialB2BViewDTO.hasConfigurationError() && imperialB2BViewDTO.Configuration != null && imperialB2BViewDTO.Configuration.Count > 0)
                base.ConfigureGridOrder(grdMgr, imperialB2BViewDTO.Configuration);

            // Encabezado de Recepciones
            grdMgr.DataSource = imperialB2BViewDTO.Entities;
            grdMgr.DataBind();

            ucStatus.ShowRecordInfo(imperialB2BViewDTO.Entities.Count, grdMgr.PageSize, grdMgr.PageCount, currentPage, grdMgr.AllowPaging);
        }

        /// <summary>
        /// Actualiza el ViewDTO y la variable de sesion asociada desde la base de datos
        /// </summary>
        private void UpdateSession()
        {
            // Carga lista de OutboundOrder
            context.MainFilter = this.Master.ucMainFilter.MainFilter;

            TextBox txtNumReference = (TextBox)this.Master.ucMainFilter.FindControl("txtFilterCode");
            DropDownList ddlWarehouse = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterWarehouse");
            DropDownList ddlOwner = (DropDownList)this.Master.ucMainFilter.FindControl("ddlFilterOwner");

            imperialB2BViewDTO = iFuntionalMGR.GetOrdersDispatch(customerCodeImperial, txtNumReference.Text.Trim(), int.Parse(ddlWarehouse.SelectedValue), int.Parse(ddlOwner.SelectedValue), context);

            if (!imperialB2BViewDTO.hasError() && imperialB2BViewDTO.Entities != null)
            {
                Session.Add(WMSTekSessions.FuntionalMgr.ImperialB2BList, imperialB2BViewDTO);
                isValidViewDTO = true;

                if (imperialB2BViewDTO.Entities.Count > 0)
                    this.Master.ucTaskBar.btnPrintEnabled = true;
                else
                    this.Master.ucTaskBar.btnPrintEnabled = false;

                ucStatus.ShowMessage(imperialB2BViewDTO.MessageStatus.Message);
            }
            else
            {
                this.Master.ucTaskBar.btnPrintEnabled = false;
                isValidViewDTO = false;
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }


        protected void ReloadData()
        {
            UpdateSession();

            // Si es un ViewDTO valido, carga la grilla
            if (isValidViewDTO)
            {
                divGrid.Visible = true;
                currentPage = 0;
                this.Master.ucError.ClearError();
            }
        }


        public void GenerateASN(GridView grdMaster)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            string style = @"<style> .text { mso-number-format:\@; } </style> ";
            string fileName = "Imperial" + "_" + DateTime.Now.ToShortDateString();

            fileName = fileName.Replace(" ", "_").Replace(">", "").Replace("<", "").Replace(":", "").Replace("%", "").Replace("/", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("°", "");

            Page page = new Page();
            HtmlForm form = new HtmlForm();
            Table table = new Table();
            //TableRow titleRow = new TableRow();
            //TableRow dateRow = new TableRow();
            //TableHeaderCell titleCell = new TableHeaderCell();
            //TableCell dateCell = new TableCell();

            PrepareGridViewForExport(grdMaster);

            // Grilla master
            form.Controls.Add(grdMaster);
            
            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
            page.RenderControl(htw);

            Response.Clear();
            Response.Buffer = true;

            // TODO: averiguar que ContentType conviene
            //Response.ContentType = "application/vnd.ms-excel";
            Response.ContentType = "application/excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.Default;

            Response.Write(style);
            Response.Write(sb.ToString());

            Response.End();
        }

        /// <summary>
        /// Da formato de texto a todas las celdas de la grilla
        /// </summary>
        /// <param name="gv"></param>
        private void PrepareGridViewForExport(Control gv)
        {
            LinkButton lb = new LinkButton();
            Literal l = new Literal();
            string name = String.Empty;

            for (int i = 0; i < gv.Controls.Count; i++)
            {
                if (gv.Controls[i].GetType() == typeof(LinkButton))
                {
                    l.Text = (gv.Controls[i] as LinkButton).Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(DropDownList))
                {
                    l.Text = (gv.Controls[i] as DropDownList).SelectedItem.Text;
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(CheckBox))
                {
                    // TODO: traducir 
                    l.Text = (gv.Controls[i] as CheckBox).Checked ? "Sí" : "No";
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                else if (gv.Controls[i].GetType() == typeof(Calendar))
                {
                    l.Text = (gv.Controls[i] as Calendar).SelectedDate.ToShortDateString();
                    gv.Controls.Remove(gv.Controls[i]);
                    gv.Controls.AddAt(i, l);
                }
                if (gv.Controls[i].HasControls())
                {
                    PrepareGridViewForExport(gv.Controls[i]);
                }
            }
        }



        protected void btnExcel_Click(object sender, EventArgs e)
        {
            string detailTitle;

            try
            {
                grdMgr.AllowPaging = false;
                PopulateGrid();
                                

                base.ExportToExcel(grdMgr, null, "");
                grdMgr.AllowPaging = true;
            }
            catch (System.Threading.ThreadAbortException)
            {
                //no hace nada
            }
            catch (Exception ex)
            {
                imperialB2BViewDTO.Errors = baseControl.handleException(ex, context);
                this.Master.ucError.ShowError(imperialB2BViewDTO.Errors);
            }
        }

        protected override void grdMgr_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                e.Row.TableSection = TableRowSection.TableHeader;
        }
    }
}
