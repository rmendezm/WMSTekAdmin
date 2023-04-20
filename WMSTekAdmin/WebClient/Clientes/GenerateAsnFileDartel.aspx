<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true"
    CodeBehind="GenerateAsnFileDartel.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Clientes.GenerateAsnFileDartel" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        function resizeDiv() {
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;

            removeFooter("ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr");
        }
        window.onresize = resizeDiv;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(resizeDiv);


        function HideMessage() {
            document.getElementById("divFondoPopup").style.display = 'none';
            document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

            return false;
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

        function abrirASN() {
            //document.getElementById("ctl00_MainContent_divEditNew").style.display = 'none';
            $find("modalPopUpASN").show();
            return false;
        }

        function cerrarASN() {
            //document.getElementById("ctl00_MainContent_divEditNew").style.display = 'none';
            $find("BImodalPopUpASN").hide();
            return false;
        }



        function ShowProgress() {
            document.getElementById('<% Response.Write(uprEditNew.ClientID); %>').style.display = "inline";
        }

        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('GetDispatchSpecialHeaderASNDartel', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }

        function setDivsAfter() {
            var heightDivBottom = $("#hsMasterDetail_RightP_Content").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
            var extraSpace = 15;

            var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;

            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
        }
    </script><link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" /><div id="divPrincipal" style="width: 100%; height: 100%; margin: 0px; margin-bottom: 80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server"
            StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Panel Grilla Principal --%>
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



                                        <asp:GridView ID="grdMgr" runat="server" OnRowCreated="grdMgr_RowCreated" OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                            AllowPaging="True" 
                                            AutoGenerateColumns="False"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectOutboundOrder">
	                                                <HeaderTemplate>
		                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOutboundOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                                </HeaderTemplate>
	                                                <itemtemplate>
	                                                   <asp:CheckBox ID="chkSelectOutboundOrder" runat="server"/>
	                                                </itemtemplate>
                                                 </asp:templatefield>

                                                <asp:TemplateField HeaderText="ID Despacho" AccessibleHeaderText="IdDispacth" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdDispacth" runat="server" Text='<%# Eval ( "Id" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ID Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "IdWhs" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ID Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "OutboundOrder.Owner.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="OutboundNumber" HeaderText="Nro Documento">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="ReferenceNumber" HeaderText="Orden de Compra">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="ReferenceDoc" HeaderText="Documento Ref.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReferenceDocument" runat="server" Text='<%# Eval ( "ReferenceDoc.ReferenceDocNumber" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField AccessibleHeaderText="CustomerCode" HeaderText="Codigo Cliente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "Customer.Code" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="CustomerName" HeaderText="Cliente">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="DateCreated" HeaderText="Fecha Creación">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDateCreated" runat="server" Text='<%# ((DateTime) Eval ("DateCreated") > DateTime.MinValue)? Eval("DateCreated", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="UserCreated" HeaderText="User">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUserCreated" runat="server" Text='<%# Eval ("UserCreated") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

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
                    <%-- FIN Panel Grilla Principal --%>
                    <asp:UpdateProgress ID="uprSelectedOrders" runat="server" AssociatedUpdatePanelID="upGrid"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrders" runat="server" ControlToOverlayID="divTop"
                        CssClass="updateProgress" TargetControlID="uprSelectedOrders" />
                </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>
                    <%-- Panel Grilla Detalle --%>
                    <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                  <div class="container">
                                    <div class="row">
                                        <div class="col-md-9">
                                                <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                    <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
                                                    <asp:Label ID="lblNroDoc" runat="server" Text="" />
                                                    &nbsp;&nbsp
                                                    <asp:Label ID="lblNroDocRef" runat="server" Text="Doc Referencia: " />
                                                    <asp:Label ID="lblNroDocRefDet" runat="server" Text="" />
                                                    &nbsp;&nbsp;&nbsp;&nbsp;  
	                        
                                                </div>
                                        </div>
                                         <div class="col-md-3">
                                                   <asp:Button ID="btnLpnBundle" OnClick="btnLpnBundle_Click" runat="server" Text="LPN Bulto" CssClass="pull-right btn-detail" />
                                         </div>
                                   </div>
                               </div>

                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12"> 
                                            <div id="divGrid" runat="server" class="textLeft">
                                                <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail" DataKeyNames="Id"
                                                    EnableViewState="True" AutoGenerateColumns="False" OnRowCreated="grdDetail_RowCreated"
                                                    OnRowDataBound="grdDetail_RowDataBound"
                                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                    EnableTheming="false"
                                                 >

                                                    <Columns>
                                                        <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectOutboundOrder">
	                                                        <HeaderTemplate>
		                                                        <input type="checkbox" onclick="toggleCheckBoxes('<%= grdDetail.ClientID %>', 'chkSelectDispatchDetail', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                                        </HeaderTemplate>
	                                                        <itemtemplate>
	                                                           <asp:CheckBox ID="chkSelectDispatchDetail" runat="server"/>
	                                                        </itemtemplate>
                                                        </asp:templatefield>
                                                        <asp:TemplateField HeaderText="ID" AccessibleHeaderText="IdDispatchDetail" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblIdDispatchDetail" runat="server" Text='<%# Eval ( "Id" ) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Nro Linea" AccessibleHeaderText="LineNumber" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLineNumber" runat="server" Text='<%# Eval ( "LineNumber" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ID Item" AccessibleHeaderText="IdItem" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# Eval ( "Item.Id" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Solicitado" AccessibleHeaderText="ItemQty">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval ( "Qty" )== -1)?"":Eval ( "Qty" )) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Cod. Barra" AccessibleHeaderText="BarCode" SortExpression="BarCode">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblBarCode" runat="server" Text='<%# Eval ("ItemUom.BarCode") %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblPrice" runat="server" Text='<%# ((decimal)Eval ( "Price" )== -1)?"":Eval ( "Price" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "LotNumber" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="F. Fifo" AccessibleHeaderText="Fifo" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblFifo" runat="server" Text='<%# (((DateTime)Eval ( "Fifo" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Fifo", "{0:d}" )) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="F. Expiración" AccessibleHeaderText="Expiration" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblExpiration" runat="server" Text='<%# (((DateTime)Eval ( "Expiration" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Expiration", "{0:d}" )) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="F. Fabricación" AccessibleHeaderText="Fabrication"
                                                            ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblFabrication" runat="server" Text='<%# (((DateTime)Eval ( "Fabrication" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "Fabrication", "{0:d}" )) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Lpn" AccessibleHeaderText="Lpn" ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLpn" runat="server" Text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Cod. Tipo Lpn" AccessibleHeaderText="LpnTypeCode"
                                                            ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLpnTypeCode" runat="server" Text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Nom. Tipo Lpn" AccessibleHeaderText="LpnTypeName"
                                                            ItemStyle-CssClass="text">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLpnTypeName" runat="server" Text='<%# Eval ( "Lpn.LPNType.Name" ) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <%--<asp:PostBackTrigger ControlID="btnGenerateASN" /> --%>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr"
                                EventName="RowCommand" />
                            <%-- <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$btnOpenPopUpASN" EventName="Click" />--%>
                        </Triggers>
                    </asp:UpdatePanel>
                    <%-- FIN Panel Grilla Detalle --%>
                    <asp:UpdateProgress ID="uprSelectedOrdersDetail" runat="server" AssociatedUpdatePanelID="upGridDetail"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrdersDetail" runat="server"
                        ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectedOrdersDetail" />
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar</div>
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
                    <div class="modalControls">  <asp:Label ID="Label2" runat="server" Text="¿Desea generar un ASN?" />
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                          
 

                        </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="ASN" CssClass="modalValidation"
                                ShowMessageBox="false" />
                        </div>
                        <div id="divActions" runat="server" class="modalActions" onload="btnCerrar_Click">
                            <asp:Button ID="btnGenerateASN" runat="server" Text="Aceptar" OnClick="btnGenerateASN_ClickLegacy" 
                                OnClientClick="ShowProgress();" CausesValidation="true" ValidationGroup="ASN" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCerrar_Click" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo Owner --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGenerateASN" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnPrint" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>


    <asp:UpdatePanel ID="upEditNewSend" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Send ASN --%>
            <div id="divEditNewSend" runat="server" visible="false">
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpASNSend" runat="server" TargetControlID="btnDummy2"
                    BehaviorID="BImodalPopUpASNSend" PopupControlID="pnlOwner2" BackgroundCssClass="modalBackground"
                    PopupDragHandleControlID="OwnerCaption2" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlOwner2" runat="server" CssClass="modalBox" Style="display: none;">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label1" runat="server" Text="Enviar ASN" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">  <asp:Label ID="Label3" runat="server" Text="¿Desea enviar el ASN?" />
                        <asp:HiddenField ID="hidEditIdSend" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                          
 

                        </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ASN" CssClass="modalValidation"
                                ShowMessageBox="false" />
                        </div>
                        <div id="div3" runat="server" class="modalActions" onload="btnCerrar_Click">
                            <asp:Button ID="Button2" runat="server" Text="Aceptar" OnClick="btnGenerateASN_ClickLegacySend" 
                                OnClientClick="ShowProgress();" CausesValidation="true" ValidationGroup="ASN" />
                            <asp:Button ID="Button3" runat="server" Text="Cancelar" OnClick="btnCerrar_ClickSend" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo Owner --%>
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
    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0; z-index: 400000;
        width: 100%; height: 100%; background-color: Gray; filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute;
        width: 400px; top: 200px; margin-top: 0;" runat="server">
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
            <div class="divCaption">
                <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
        </div>
        <div id="divPanelMessage" class="divDialogPanel" runat="server">
            <div class="divDialogMessage">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar" OnClientClick="return HideMessage();" />
            </div>
        </div>
    </div>
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />   
    <asp:Label ID="lblDescription" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label ID="lblName" runat="server" Text="Cod. Cliente" Visible="false" />
    <asp:Label ID="lblReferenceDoc" runat="server" Text="Doc. Referencia" Visible="false" />
    <asp:Label ID="lblReferenceNumbDoc" runat="server" Text="Orden Compra" Visible="false" /> 
    <asp:Label ID="lblErrorReferenceDoc" runat="server" Text="Doc. Referencia" Visible="false" />
    <asp:Label id="lblNoDetailSelected" runat="server" Text="Debe seleccionar al menos un registro" Visible="false" />   
    <asp:Label ID="lblTitle" runat="server" Text="Generación Archivos ASN" Visible="false" />
    <asp:Label ID="lblErrorDetail" runat="server" Text="No existe detalle para crear el archivo." Visible="false" />
    <asp:Label ID="lblErrorTemplate" runat="server" Text="El cliente no posee un template para generar el archivo." Visible="false" />
    <asp:Label ID="lblErrorExistTaskOrder" runat="server" Text="No se puede generar ASN por que existen tareas pendientes." Visible="false" />
    <asp:Label ID="lblErrorNotItemUom" runat="server" Text="Para el ítem [ITEM], NO existe una presentación de Cliente creada." Visible="false" />
    <asp:Label ID="lblErrorMoreThanOneItemUom" runat="server" Text="Para el ítem [ITEM], existe más de una presentación de UNIDAD creada." Visible="false" />
    <asp:Label ID="lblToolTipASN" runat="server" Text="Generar ASN" Visible="false" />
    <asp:Label ID="lblErrorXmlConstMissing" runat="server" Text="No existe el tipo de bulto o pallet en constantes" Visible="false" />
    <asp:Label id="lblErrorCustomerNotFound" runat="server" Text="Cliente no encontrado en el sistema" Visible="false" /> 
    <asp:Label ID="lblRangeQtyCopy" runat="server" Text="Nº de Copias el valor debe estar entre " Visible="false"/>  
    <asp:Label ID="lblValidateDispatchDetailWithSealNumber" runat="server" Text="Los lpn seleccionados deben tener N° sello creado" Visible="false"/>  
    <asp:Label id="lblSelectedOutboundOrderWithDifferentReferenceDocNumber" runat="server" Text="Se seleccionaron documentos de salida de mas de una orden de compra" Visible="false" /> 
    <asp:Label id="lblValidateAtLeastOneOutboundOrderSelected" runat="server" Text="Debe seleccionar al menos un documento de salida" Visible="false" /> 
    <asp:Label id="lblDoesntExistItemCustomerMatch" runat="server" Text="No existe registro en Item " Visible="false" />  
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
