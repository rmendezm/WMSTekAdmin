<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="B2BAdministration.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.B2BAdministration" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" language="javascript">
        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            //clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('B2BAdministration', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function setDivsAfter() {
            var heightDivBottom = $("#__hsMasterDetailRD").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetail > .container > .row:first-child").height();
            var heightStatusBarPanel = $(".statusBarPanel").height();
            var extraSpace = 100;
            var divDetailFilter = $("#divFilterDetail").height();

            var totalHeight = heightDivBottom - heightLabelsBottom - heightStatusBarPanel - extraSpace - divDetailFilter;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");

            var heightDivTop = $("#hsMasterDetail_LeftP_Content").height();
            var heightDivPrinter = $("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_divPrintLabel").height();
            var totalHeightTop = heightDivTop - heightDivPrinter;
            $("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr").parent().css("max-height", totalHeightTop + "px");
        }

        function ShowProgress2() {

            if ($("#ctl00_MainContent_txtKeyField").val() != "" && $("#ctl00_MainContent_txtValueField").val() != "") {
                loadingAjaxStart();
            }
        }

        function ShowProgress() {
            document.getElementById('<% Response.Write(uprEditNew.ClientID); %>').style.display = "inline";
        }

        function ShowMessage(title, message) {

            var position = (document.body.clientWidth - 400) / 2 + "px";
            document.getElementById("divFondoPopup").style.display = 'block';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
            document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

            document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
            document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

            return false;
        }

        function HideMessage() {
            document.getElementById("divFondoPopup").style.display = 'none';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

            return false;
        }
    </script>

    <style>
        .btn-detail{
            margin-left: 5px;
            margin-bottom: 5px;
        }      
        
        .lblFilterDetail{
            font-size: 11px !important;
        }

        #divFilterDetail{
            padding-bottom: 4px;
        }
    </style>

    <link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="40">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        
                                        <div ID="divPrintLabel" runat="server" class="divPrintLabel" visible="false">
               
                                            <div class="divCtrsFloatLeft">
                                                <div id="divCodStatus" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblCopies" runat="server" Text="Nº de Copias" />
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:TextBox ID="txtQtycopies" runat="server" Width="30"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvQtycopies" runat="server" ControlToValidate="txtQtycopies" ValidationGroup="valPrint" Text=" * " ErrorMessage="Nº de Copias es requerido." />
                                                        <asp:RangeValidator ID="rvQtycopies" runat="server" ErrorMessage="El valor debe estar entre 1 y 100." MaximumValue="100" MinimumValue="1" Text=" *" ControlToValidate="txtQtycopies" ValidationGroup="valPrint" Type="Integer"></asp:RangeValidator>
                                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbeQtycopies" runat="server" TargetControlID="txtQtycopies" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
                                                    </div>
                                                </div> 
                        
                                                <div id="div2" runat="server" class="divControls">
                                                    <div class="fieldRight">
                                                        <asp:Label ID="lblPrinter" runat="server" Text="Impresora" />
                                                    </div>
                                                    <div class="fieldLeft">
                                                        <asp:UpdatePanel ID="updPrinter" runat="server">
                                                            <ContentTemplate>
                                                                <asp:DropDownList ID="ddlPrinters" runat="server" AutoPostBack="true"/>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </div>           
                                            </div>
                    
                                            <div class="divValidationSummary">
                                                <asp:Label ID="lblNotPrinter" runat="server" CssClass="modalValidation" Text="No existen impresoras asociadas al usuario." ></asp:Label> 
                                                <asp:ValidationSummary ID="valPrint" ValidationGroup="valPrint" runat="server" CssClass="modalValidation" />                                         
                                            </div>   
                    
                                        </div>

                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">

                                                <Columns>

                                                    <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="WhsName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="N° Ola" AccessibleHeaderText="OutboundOrderNumber" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOutboundOrder" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Tipo Doc" AccessibleHeaderText="OutboundTypeName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblTypeDoc" runat="server" text='<%# Eval ( "OutboundOrder.OutboundType.Code" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Doc Referencia" AccessibleHeaderText="ReferenceNumber" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOutboundRefNumber" runat="server" text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>
                                                    
                                                    <asp:templatefield HeaderText="Traza" AccessibleHeaderText="DispatchType" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblDispatchType" runat="server" text='<%# Eval ( "DispatchType" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                     <asp:templatefield HeaderText="Pedidos" AccessibleHeaderText="QtyOrders" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblQtyOrders" runat="server" text='<%# Eval ( "OutboundOrder.QtyOrdersFromWave" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Fecha Emisión" AccessibleHeaderText="EmissionDate" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblEmissionDate" runat="server" text='<%# (((DateTime)Eval ( "OutboundOrder.EmissionDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "OutboundOrder.EmissionDate", "{0:d}" )) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Cliente" AccessibleHeaderText="Customer" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblCustomerName" runat="server" text='<%# Eval ( "Customer.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                </Columns>

                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>                   
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />             
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

                </Content>
            </TopPanel>
            <BottomPanel HeightMin="120">
                <Content>                
                    <asp:UpdatePanel ID="upGridDetail" runat="server"  UpdateMode="Conditional">
                        <ContentTemplate>     
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">

                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-3">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Documento Ola: " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>  
                                        </div>
                                        <div class="col-md-9">
                                            <asp:Button ID="btnLpnPallet" OnClick="btnLpnPallet_Click" runat="server" Text="LPN Pallet" CssClass="pull-right btn-detail" visible="false" />
                                            <asp:Button ID="btnLpnBundle" OnClick="btnLpnBundle_Click" runat="server" Text="LPN Bulto" CssClass="pull-right btn-detail" />
                                        </div>
                                    </div>
                                    <div class="row" id="divFilterDetail">
                                        <div class="col-md-12">
                                            <asp:Label ID="lblFilterDetailByLpn" runat="server" Text="LPN: " CssClass="lblFilterDetail"/>  
                                            <asp:TextBox ID="txtFilterDetailByLpn" runat="server" CssClass="txtFilter" Width="120px"></asp:TextBox>

                                            <asp:Label ID="lblFilterDetailByItem" runat="server" Text="Item: " CssClass="lblFilterDetail"/>  
                                            <asp:TextBox ID="txtFilterDetailByItem" runat="server" CssClass="txtFilter" Width="120px"></asp:TextBox>

                                            <asp:Button ID="btnFilterDetail" runat="server" Text="Buscar en Detalle" OnClick="btnFilterDetail_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="True" 
                                                AutoGenerateColumns="False"
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                OnDataBound="grdDetail_DataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="true"
                                                OnPageIndexChanging="grdDetail_PageIndexChanging">  

                                                    <Columns>
                                                        <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectOutboundOrder">
	                                                        <HeaderTemplate>
		                                                        <input type="checkbox" onclick="toggleCheckBoxes('<%= grdDetail.ClientID %>', 'chkSelectDispatchDetail', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                                        </HeaderTemplate>
	                                                        <itemtemplate>
	                                                           <asp:CheckBox ID="chkSelectDispatchDetail" runat="server"/>
	                                                        </itemtemplate>
                                                        </asp:templatefield>

                                                        <asp:TemplateField HeaderText="ID" AccessibleHeaderText="IdDispatchDetail" ItemStyle-CssClass="text" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblIdDispatchDetail" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                         <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdCode" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLpnCode" runat="server" text='<%# Eval ( "LPN.IdCode" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="LPN Contenedor" AccessibleHeaderText="IdCodeContainer" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLpnCodeContainer" runat="server" text='<%# Eval ( "IdLpnCodeContainer" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="CodeName" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ShortName" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblItemName" runat="server" text='<%# Eval ( "Item.ShortName" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Factor de Conversión" AccessibleHeaderText="ConversionFactor" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblConversionFactor" runat="server" text='<%# Eval ( "ItemUom.ConversionFactor" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="Qty" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblQty" runat="server" text='<%# Eval ( "Qty" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="N° Pedido" AccessibleHeaderText="OutboundNumber" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "OutboundDetail.OutboundOrder.Number" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Factura" AccessibleHeaderText="ReferenceDocNumber" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblReferenceDocNumber" runat="server" text='<%# Eval ( "OutboundDetail.OutboundOrder.ReferenceDoc.ReferenceDocNumber" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Código Barra" AccessibleHeaderText="BarCode" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblBarCode" runat="server" text='<%# Eval ( "Item.ItemCustomer.BarCode" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="N° Sello" AccessibleHeaderText="SealNumber" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSealNumber" runat="server" text='<%# Eval ( "SealNumber" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>

                                                    <PagerSettings NextPageText="Siguiente" PreviousPageText="Anterior" Mode="NumericFirstLast" PageButtonCount="4"  FirstPageText="Primero" LastPageText="Último"/>
                                                    <pagerstyle CssClass="pagination-ys" />

                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>                
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />                        
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                        </Triggers>
                    </asp:UpdatePanel>     
                    
                    <asp:UpdateProgress ID="uprGridDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGridDetail" />

                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>    
    </div>

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Owner --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpASN" runat="server" TargetControlID="btnDummy"
                    BehaviorID="BImodalPopUpASN" PopupControlID="pnlOwner" BackgroundCssClass="modalBackground"
                    PopupDragHandleControlID="OwnerCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlOwner" runat="server" CssClass="modalBox" Style="display: none;">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Generar ASN" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            <div id="divCita" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblNroCita" runat="server" Text="Nro. Cita" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtNroCita" runat="server" Width="50px" MaxLength="10"
                                        ToolTip="Ingrese Nro. Cita" ValidationGroup="ASN" AutoPostBack="false" />
                                    <asp:HiddenField ID="hidNroCita" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvNroCita" ControlToValidate="txtNroCita" ValidationGroup="ASN"
                                        runat="server" ErrorMessage="Nro. Cita es requerido" Text=" * " />
                                </div>
                            </div>
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDateAsn" runat="server" Text="Fecha" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtDateAsn" runat="server" Width="70px" ValidationGroup="ASN"
                                        ToolTip="Ingrese fecha." />
                                    <asp:RequiredFieldValidator ID="rfvDateAsn" runat="server" ControlToValidate="txtDateAsn"
                                        ValidationGroup="ASN" Text=" * " ErrorMessage="Fecha es requerido" />
                                    <asp:RangeValidator ID="rvDateAsn" runat="server" ControlToValidate="txtDateAsn"
                                        ErrorMessage="Fecha debe estar entre 01-01-2000 y 31-12-2040" Text=" * " MinimumValue="01/01/2000"
                                        MaximumValue="31/12/2040" ValidationGroup="ASN" Type="Date" />
                                    <ajaxToolkit:CalendarExtender ID="calDateAsn" CssClass="CalMaster" runat="server"
                                        Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtDateAsn" PopupButtonID="txtDateAsn"
                                        Format="dd-MM-yyyy">
                                    </ajaxToolkit:CalendarExtender>
                                </div>
                            </div>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHourAsn" runat="server" Text="Hora" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtHourAsn" runat="server" Width="50px" MaxLength="5"
                                        ToolTip="Ingrese hora formato 24hrs." ValidationGroup="ASN" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvHourAsn" ControlToValidate="txtHourAsn" ValidationGroup="ASN"
                                        runat="server" ErrorMessage="Hora es requerido" Text=" * " />
                                    <asp:RegularExpressionValidator ID="revHourAsn" runat="server" ControlToValidate="txtHourAsn"
                                        ErrorMessage="Hora no es valida ej: 23:30" Display="Dynamic" ValidationExpression="^[0-2]?[0-9]:[0-5][0-9]$"
                                        ValidationGroup="ASN" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouseASN" runat="server" Text="Almacén" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtWarehouseASN" runat="server" Width="50px"
                                        MaxLength="10" ToolTip="Ingrese Almacén." ValidationGroup="ASN" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouseASN" ControlToValidate="txtWarehouseASN"
                                        ValidationGroup="ASN" runat="server" ErrorMessage="Almacén es requerido" Text=" * " />
                                </div>
                            </div>
                        </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="ASN" CssClass="modalValidation"
                                ShowMessageBox="false" />
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnGenerateASN" runat="server" Text="Aceptar" OnClick="btnGenerateASN_Click"
                                OnClientClick="ShowProgress();" CausesValidation="true" ValidationGroup="ASN" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCerrar_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up ASN --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGenerateASN" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnPrint" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>
               
    </div> 

     <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblDocName" runat="server" Text="Nº Ola" Visible="false" />    
    <asp:Label id="lblCustomer" runat="server" Text="Nombre Cliente" Visible="false" />    
    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />    
    <asp:Label id="lblAsnButtonTooltip" runat="server" Text="Descargar ASN" Visible="false" />    
    <asp:Label id="lblFilterRefDoc" runat="server" Text="Orden de Compra" Visible="false" />    
    <asp:Label id="lblNoDetailSelected" runat="server" Text="Debe seleccionar al menos un registro" Visible="false" />   
    <asp:Label id="lblTitleErrorCustomerSelected" runat="server" Text="Error con cliente" Visible="false" />  
    <asp:Label id="lblMessageErrorCustomerSelected" runat="server" Text="El cliente seleccionado no esta configurado como B2B en el archivo constantes" Visible="false" />  
    <asp:Label ID="lblErrorXmlConstMissing" runat="server" Text="No existe el tipo de bulto o pallet en constantes" Visible="false" />
    <asp:Label id="lblDoesntExistItemCustomerMatch" runat="server" Text="No existe registro en Item " Visible="false" /> 
    <asp:Label id="lblMissXsdFile" runat="server" Text="No se encontro ruta del archivo .xsd" Visible="false" /> 
    <asp:Label id="lblNoDispatchDetailAsn" runat="server" Text="No se encontraron registros del detalle para crear ASN" Visible="false" />   
    <asp:Label ID="lblTitle" runat="server" Text="Generación Archivos ASN" Visible="false"/> 
    <asp:Label id="lblErrorCustomerNotFound" runat="server" Text="Cliente no encontrado en el sistema" Visible="false" /> 
    <asp:Label id="lblMissingBill" runat="server" Text="No se puede generar ASN ya que registros seleccionados no tienen factura" Visible="false" /> 
    <asp:Label ID="lblRangeQtyCopy" runat="server" Text="Nº de Copias el valor debe estar entre " Visible="false"/>  
    <asp:Label ID="lblValidateDispatchDetailWithSealNumber" runat="server" Text="Los lpn seleccionados deben tener N° sello creado" Visible="false"/>  
    <asp:Label ID="lblTaskLabelNotFound" runat="server" Text="No hay tarea de impresión para asignar campo" Visible="false"/> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
