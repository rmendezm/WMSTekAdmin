<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ReleaseLpnWithoutDocument.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.ReleaseLpnWithoutDocument" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        function resizeDiv() {
            //debugger;
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("divPrincipal").style.height = h;
            document.getElementById("divPrincipal").style.width = w;
        }
        window.onresize = resizeDiv; 

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
            initializeGridDragAndDrop('ReleaseLpnWithoutDocument_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
        }
    </script>

    <div id="divPrincipal" style=" width:100%; height:100%; margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter LiveResize="false"   CookieDays="0"  ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel" HeightMin="50">
            <Content>
                 <%--<div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">--%>
                <%-- Panel Grilla Principal --%>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <asp:GridView ID="grdMgr" runat="server" AllowPaging="True"  
                                            OnRowCreated="grdMgr_RowCreated" 
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            EnableViewState="False"
                                            AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkReleaseConfirm', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkReleaseConfirm" runat="server" Visible="true" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLPN" runat="server" Text='<%# Eval ( "LPN.Code" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. Tipo Lpn" AccessibleHeaderText="LpnTypeCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnTypeCode" runat="server" Text='<%# Eval ( "LPN.LPNType.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LpnTypeName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnTypeName" runat="server" Text='<%# Eval ( "LPN.LPNType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. CD" AccessibleHeaderText="WhsCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="ShortWhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "LPN.Owner.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "LPN.Owner.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "LPN.Owner.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Documento" AccessibleHeaderText="InboundNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInboundNumber" runat="server" Text='<%# Eval ( "InboundOrder.Number" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo Documento" AccessibleHeaderText="InboundTypeName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInboundTypeName" runat="server" Text='<%# Eval ( "InboundOrder.InboundType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="NameTrackInboundType">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNameTrackInboundType" runat="server" Text='<%# Eval ( "TrackInboundType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdInboundOrder" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdInboundOrder" runat="server" Text='<%# Eval ( "InboundOrder.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Stock.Location.IdCode" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tot ítems" AccessibleHeaderText="TotItems" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotItems" runat="server" Text='<%# GetFormatedNumber(((Decimal) Eval ( "Stock.Qty" )==-1)?"":Eval ( "Stock.Qty" ))%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("LPN.Fifo") > DateTime.MinValue)? Eval("LPN.Fifo", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="WeightTotal" SortExpression="WeightTotal">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblWeightTotal" runat="server" Text='<%# GetFormatedNumber(((Decimal) Eval ( "LPN.WeightTotal" )==-1)?"":Eval ( "LPN.WeightTotal" ))%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="VolumeTotal" SortExpression="VolumeTotal">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVolumeTotal" runat="server" Text='<%# GetFormatedNumber(((Decimal) Eval ( "LPN.VolumeTotal" )==-1)?"":Eval ( "LPN.VolumeTotal" ))%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cerrado" AccessibleHeaderText="IsClosed" SortExpression="IsClosed">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsClosed" runat="server" Checked='<%# Eval ("LPN.IsClosed") %>'
                                                            Enabled="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cod. Proveedor" AccessibleHeaderText="VendorCode" SortExpression="VendorCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVendorCode" runat="server" Text='<%# Eval ("InboundOrder.Vendor.Code") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Proveedor" AccessibleHeaderText="VendorName" SortExpression="VendorName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval ("InboundOrder.Vendor.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Lpn Contenedor" AccessibleHeaderText="LpnParent" SortExpression="LpnParent" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnParent" runat="server" Text='<%# Eval ("LPN.LpnParent") %>' />
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
                                 <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                                 <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnRelease" EventName="Click" /> 
                              </Triggers>
                            </asp:UpdatePanel>  
                        </div>
                    </div>
                </div>
                <%-- FIN Panel Grilla Principal --%>
               <%--</div>--%>

                <%-- Panel Cerrar Auditoria --%>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">       
        <ContentTemplate>   
            <div id="divMessaje" runat="server" visible="false" class="divItemDetails" >
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpMessaje" runat="server" TargetControlID="btnDummy2"
                    PopupControlID="pnlMessaje" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlMessaje" runat="server" CssClass="modalBox" Width="430px" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblTitleMessaje" runat="server" Text="Motivo Confirmación" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    
                    
                                        
                </asp:Panel>
            </div>
            
            <div id="divConfirmPrin" runat="server">
                <asp:Button ID="btnDialogDummy" runat="Server" Style="display: none" /> 
                <ajaxToolKit:ModalPopupExtender 
	                ID="modalPopUpDialog" runat="server" TargetControlID="btnDialogDummy" 
	                PopupControlID="pnlDialog"  
	                BackgroundCssClass="modalBackground" 
	                PopupDragHandleControlID="Caption" Drag="true" >
	            </ajaxToolKit:ModalPopupExtender>
	            <asp:Panel ID="pnlDialog" runat="server" CssClass="modalBox" Width="400px">    	
		            <%-- Encabezado --%>    			
		            <asp:Panel ID="DialogHeader" runat="server" CssClass="modalHeader">
			            <div class="divCaption">
			                <asp:Label ID="lblDialogTitle" runat="server" />			    
                        </div>
	                </asp:Panel>
            	    
                    <div id="divDialogPanel" class="divDialogPanel" runat="server">
                        <div class="divDialogMessage">
                            <asp:Image id="imgDialogIcon" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
                        </div>
                        <div id="divDialogMessage" runat="server" class="divDialogMessage">                          
                        </div>
                        <%--<div id="divConfirm" runat="server" class="divDialogButtons">
                            <asp:Button ID="btnOk" runat="server" Text="   Sí   " OnClick="btnOk_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="   No   " OnClick="btnCancel_Click" />
                        </div> --%>
                    </div>                     
                 </asp:Panel>
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
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" /> 
        </Triggers>
        </asp:UpdatePanel>
        <%-- FIN Panel Cerrar Auditoria --%>


            </Content>
        </TopPanel>
        <BottomPanel HeightMin="50">
            <Content>
                <%--<div onresize="SetDivs();">--%>
                <%-- Panel Grilla Detalle --%>
                
                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">  
                    <ContentTemplate>                                                
                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Docto: " />
                                <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                            </div>
                            
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="false"
                                            OnRowCreated="grdDetail_RowCreated" SkinID="grdDetail"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdDetail_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Cod. Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName" SortExpression="LongItemName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description" SortExpression="Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgName" SortExpression="CtgName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status" AccessibleHeaderText="Status" SortExpression="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# ((Boolean) Eval ("Status") == true)? "Activo":"Inactivo" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="TotalWeight" SortExpression="TotalWeight">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblTotalWeight" runat="server" text='<%# GetFormatedNumber(((Decimal) Eval ( "TotalWeight" )==-1)?"":Eval ( "TotalWeight" ))%>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="TotalVolumen" SortExpression="TotalVolumen">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblTotalVolumen" runat="server" text='<%# GetFormatedNumber(((Decimal) Eval ( "TotalVolumen" )==-1)?"":Eval ( "TotalVolumen" ))%>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate"
                                                    SortExpression="ExpirationDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblExpiration" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fifo" AccessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="FabricationDate"
                                                    SortExpression="FabricationDate">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval ("Location.IdCode") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bulto" AccessibleHeaderText="IdLpnCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LpnTypeCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Precio" AccessibleHeaderText="Price" SortExpression="Price">
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblPrice" runat="server" Text=' <%# ((decimal) Eval ("Price") == -1 )?" ": Eval ("Price") %>' />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Recepción" AccessibleHeaderText="IdReceipt" SortExpression="IdReceipt" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:Label ID="lblIdReceipt" runat="server" Text='<%# ((int) Eval ("Receipt.Id") == -1 )?" ": Eval ("Receipt.Id") %>'></asp:Label>
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Doc. Entrada" AccessibleHeaderText="InboundNumber" ItemStyle-CssClass="text"
                                                    SortExpression="InboundNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInboundNumber" runat="server" Text='<%# Eval ( "InboundOrder.Number" ) %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="false" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Línea" AccessibleHeaderText="InboundLineNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInboundLineNumber" runat="server" Text='<%# ((int) Eval ("InboundLineNumber")  == -1 )?" ": Eval ("InboundLineNumber")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Línea" AccessibleHeaderText="OutboundLineNumber">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundLineNumber" runat="server" Text='<%# ((int) Eval ("OutboundLineNumber")  == -1 )?" ": Eval ("OutboundLineNumber")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("Item.GrpItem1.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("Item.GrpItem2.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("Item.GrpItem3.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4Name">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("Item.GrpItem4.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sello" AccessibleHeaderText="SealNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="Label4" Text='<%# Bind("Seal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Razón" AccessibleHeaderText="ReasonCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblReasonCode" Text='<%# Bind("Reason") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bloqueo" AccessibleHeaderText="HoldCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblHoldCode" Text='<%# Bind("Hold") %>' />
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
                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel> 
                        
                <%-- FIN Panel Grilla Detalle --%>
                <%--</div>--%>
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </BottomPanel>
    </spl:HorizontalSplitter>
</div>  

    <%-- PopUp Liberar Pedidos --%>
     <asp:UpdatePanel ID="upRelease" runat="server" UpdateMode="Always" >
        <ContentTemplate>
            <div id="divReleaseDispatch" runat="server" visible="false">
	            <asp:Button ID="btnDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	                <ajaxToolKit:ModalPopupExtender 
	                    ID="mpReleaseDispatch" runat="server" TargetControlID="btnDummy" 
	                    PopupControlID="pnlReleaseDispatch"  
	                    BackgroundCssClass="modalBackground" 
	                    PopupDragHandleControlID="Caption" Drag="true" >
	                </ajaxToolKit:ModalPopupExtender>
                	
	                <asp:Panel ID="pnlReleaseDispatch" runat="server" CssClass="modalBox">
	                    <%-- Encabezado --%>			
		                <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
		                    <div class="divCaption">
			                    <asp:Label ID="lblEdit" runat="server" Text="Liberar Pedidos"/>
                            </div>			        
	                    </asp:Panel>
                        <%-- Fin Encabezado --%>  
                         
		                <div class="modalControls">
		                    <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
		                        <div id="divPickingTitle" runat="server" visible="false" class="divControls"><u>Picking</u></div>
                                <div id="divUserNbr" runat="server" visible="false" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblUserNbr" runat="server" Text="Nº de Operarios"/>
                                    </div>                    
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtUserNbr" runat="server" MaxLength="3" Width="10" Text="1"/>
                                        <asp:RequiredFieldValidator ID="rfvUserNbr" runat="server" ControlToValidate="txtUserNbr" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nº de Operarios es requerido"/>
                                        <asp:RangeValidator ID="rvUserNbr" runat="server" ControlToValidate="txtUserNbr" ErrorMessage="Nº de Operarios no contiene un número válido [1-100]" Text=" * " MaximumValue="100" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />
                                    </div>                                  
                                </div>
                                <div id="divPriority" runat="server" class="divControls">                               
                                    <div class="fieldRight">
                                        <asp:Label ID="lblPriority" runat="server" Text="Prioridad"/>
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtPriority" runat="server" MaxLength="3" Width="20" Text="10"/>
                                        <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ControlToValidate="txtPriority" ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido"/>
                                        <asp:RangeValidator ID="rvPriority" runat="server" ControlToValidate="txtPriority" ErrorMessage="Prioridad no contiene un número válido [1-10]" Text=" * " MaximumValue="10" MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />                                
                                     </div>
                                </div>
                                <div id="divLocDock" runat="server" class="divControls">      
		                            <div class="fieldRight">
		                                <asp:Label ID="lblLocDock" runat="server" Text="Ubicación de Andén" />
		                            </div> 
		                            <div class="fieldLeft">
		                                <asp:DropDownList ID="ddlLocDock" runat="server" Width="100" />
                                        <asp:RequiredFieldValidator ID="rfvLocDock" runat="server"
                                        ControlToValidate="ddlLocDock" InitialValue="-1" ValidationGroup="EditNew" 
                                        Text=" * " ErrorMessage="Ubicación de Andén es requerido" />
                                    </div>
                                </div>
                           </div>
                         
                            <div> 
                                <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                            </div>
                            <div id="divActions" runat="server" class="modalActions">
                                <asp:Button ID="btnRelease" runat="server" OnClick="btnRelease_Click" Text="Aceptar" CausesValidation="true"
                                    ValidationGroup="EditNew" />
                                <asp:Button ID="btnCancelRelease" runat="server" Text="Cancelar" OnClick="btnCancelRelease_Click" CausesValidation="true"/>
                            </div>                    
                         </div>
	                </asp:Panel>
	        </div>
        </ContentTemplate>
     </asp:UpdatePanel>
     
    <asp:UpdateProgress ID="uprRelease" runat="server" AssociatedUpdatePanelID="upRelease">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress3" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprRelease" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprRelease" />
     
     <%-- FIN PopUp Liberar Despachos --%>






    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Fec Entrega" Visible="false" />  
    <asp:Label id="lblCodeLpn" runat="server" Text="LPN" Visible="false" />  
    <asp:Label id="lblConfirmClose" runat="server" Text="¿Desea confirmar Liberacion?" Visible="false" /> 	
    <asp:Label ID="lblConfirmReleaseHeader" runat="server" Text="Confirmar Liberacion" Visible ="false"  />
    <asp:Label ID="lblBtnSaveToolTip" runat="server" Text="Confirmar Liberación" Visible ="false"  />
    <asp:Label ID="lblNotSelectedRelease" runat="server" Text="No existen registros seleccionados" Visible ="false"  />
    <asp:Label ID="lblMixedOwner" runat="server" Text="No es posible mezclar Owner" Visible="false" />
    <asp:Label ID="lblMixedWarehouse" runat="server" Text="No es posible mezclar Centros" Visible="false" />
    <asp:Label ID="lblTaskLpn" runat="server" Text="Existe Lpn con tarea activa" Visible="false" />
    <asp:Label ID="lblConfirmRelease" runat="server" Text="¿Desea Liberar Seleccionados?" Visible ="false"  />
    <asp:Label ID="lblConfirmedRelease" runat="server" Text="Liberación Satisfactoria: " Visible ="false"  />
    <asp:Label ID="lblTabLote" runat="server" Text="Lote" Visible ="false"  />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>

