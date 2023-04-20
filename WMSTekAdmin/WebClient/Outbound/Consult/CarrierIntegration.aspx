<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="CarrierIntegration.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.CarrierIntegration" %>

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
            initializeGridDragAndDrop('CarrierIntegration', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
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

                                                    <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectOutboundOrder">
	                                                    <HeaderTemplate>
		                                                    <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOutboundOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                                    </HeaderTemplate>
	                                                    <itemtemplate>
	                                                        <asp:CheckBox ID="chkSelectOutboundOrder" runat="server"/>
	                                                    </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:TemplateField HeaderText="ID" AccessibleHeaderText="IdDispatch" ItemStyle-CssClass="text" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblIdDispatch" runat="server" text='<%# Eval ( "Id" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:templatefield HeaderText="Centro" AccessibleHeaderText="WhsName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Dueño" AccessibleHeaderText="OwnName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Orden de Compra" AccessibleHeaderText="OutboundOrderNumber" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblOutboundOrder" runat="server" text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                                        </itemtemplate>
                                                    </asp:templatefield>

                                                    <asp:templatefield HeaderText="Transportista" AccessibleHeaderText="CarrierName" >
                                                        <itemtemplate>
                                                            <asp:label ID="lblCarrierName" runat="server" text='<%# Eval ( "OutboundOrder.Carrier.Name" ) %>' />
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
                                        <div class="col-md-12">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Documento : " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/> 
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
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false"
                                                AllowPaging="false">  

                                                <Columns>

                                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblLpnCode" runat="server" text='<%# Eval ( "LPN.IdCode" ) %>'></asp:Label>
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

                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="Qty" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQty" runat="server" text='<%# Eval ( "Qty" ) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>

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
            </BottomPanel>
        </spl:HorizontalSplitter>
    </div>

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
    <asp:Label id="lblDocName" runat="server" Text="Nº Documento" Visible="false" />    
    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />    
    <asp:Label id="lblFilterRefDoc" runat="server" Text="Orden de Compra" Visible="false" /> 
    <asp:Label id="lblCarrier" runat="server" Text="Nombre Transportista" Visible="false" />  
    <asp:Label ID="lblRangeQtyCopy" runat="server" Text="Nº de Copias el valor debe estar entre " Visible="false"/> 
    <asp:Label id="lblValidateCarrier" runat="server" Text="Debe ingresar un carrier" Visible="false" />  
    <asp:Label id="lblProcessButtonTooltip" runat="server" Text="Procesar" Visible="false" />
    <asp:Label id="lblNoDispatchSelected" runat="server" Text="Debe seleccionar al menos un registro" Visible="false" />  
    <asp:Label ID="lblTitle" runat="server" Text="Generación Archivo" Visible="false"/> 
    <asp:Label ID="lblCarrierNotFound" runat="server" Text="Carrier no soportado" Visible="false"/> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
