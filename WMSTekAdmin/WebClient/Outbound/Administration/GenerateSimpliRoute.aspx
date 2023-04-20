<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="GenerateSimpliRoute.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.GenerateSimpliRoute" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function clearFilterDetail(gridDetail) {
            if ($("#" + gridDetail).length == 0) {
                if ($("div.container").length == 2) {
                    //$("div.container:last div.row:first").remove();
                    $("div.container:last div.row-height-filter").remove();
                }
            }
        }

        function initializeGridDragAndDropCustom() {
            var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
            clearFilterDetail(gridDetail);
            initializeGridDragAndDrop('ReceiptDetail_ById', gridDetail);
        }

        function setDivsAfter() {
            var heightDivBottom = $("#__hsMasterDetailRD").height();
            var heightLabelsBottom = $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_divDetailTitle").height();
            var extraSpace = 160;

            var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
            $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail").parent().css("max-height", totalHeight + "px");
        }
    </script>

    <div id="divMainPrincipal" runat="server" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
        <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
            <TopPanel ID="topPanel" HeightMin="50" HeightDefault="500">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                    <ContentTemplate>
                                        <%-- Grilla Principal --%>
                                        <div id="divGrid" runat="server" visible="true" onresize="SetDivs();" >
                                            <asp:GridView ID="grdMgr" runat="server" AutoGenerateColumns = "false"
                                                AllowPaging="True" EnableViewState="false" 
                                                OnRowCreated="grdMgr_RowCreated" 
                                                OnRowDataBound="grdMgr_RowDataBound"
                                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                   <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectLpn">
	                                                    <HeaderTemplate>
		                                                    <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectLpn', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                                    </HeaderTemplate>
	                                                    <itemtemplate>
	                                                   <asp:CheckBox ID="chkSelectLpn" runat="server" onclick="validateCheckBoxCount()"/>
	                                                </itemtemplate>
                                                 </asp:templatefield>
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
                                                <asp:TemplateField HeaderText="Dirección Dest." AccessibleHeaderText="DeliveryAddress1">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDeliveryAddress1" runat="server" Text='<%# Eval ( "OutboundOrder.DeliveryAddress1" ) %>'></asp:Label>
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
                                                <asp:TemplateField HeaderText="Nº Documento" AccessibleHeaderText="OutboundNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Nº Referencia" AccessibleHeaderText="ReferenceNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tipo Salida" AccessibleHeaderText="OutboundTypeName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="NameTrackOutboundType">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNameTrackOutboundType" runat="server" Text='<%# Eval ( "TrackOutboundType.Name" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Id Documento" AccessibleHeaderText="IdOutboundOrder" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIdOutboundOrder" runat="server" Text='<%# Eval ( "OutboundOrder.Id" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ruta" AccessibleHeaderText="RouteCode" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRouteCode" runat="server" Text='<%# Eval ( "OutboundOrder.RouteCode" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%-- <asp:TemplateField HeaderText="Línea" AccessibleHeaderText="OutboundLineNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOutboundLineNumber" runat="server" Text='<%# Eval ( "Stock.OutboundLineNumber" ) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
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
                                                        <asp:Label ID="lblWeightTotal" runat="server" Text='<%# Eval ("LPN.WeightTotal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volúmen" AccessibleHeaderText="VolumeTotal" SortExpression="VolumeTotal">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVolumeTotal" runat="server" Text='<%# Eval ("LPN.VolumeTotal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cerrado" AccessibleHeaderText="IsClosed" SortExpression="IsClosed">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsClosed" runat="server" Checked='<%# Eval ("LPN.IsClosed") %>'
                                                            Enabled="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cod. Cliente" AccessibleHeaderText="CustomerCode" SortExpression="CustomerCode">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ("OutboundOrder.CustomerCode") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName" SortExpression="CustomerName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ("OutboundOrder.CustomerName") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fec. Emisión" AccessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("OutboundOrder.EmissionDate") > DateTime.MinValue)? Eval("OutboundOrder.EmissionDate", "{0:d}"):"" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch" ItemStyle-Wrap="false">
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblbranchName" runat="server" text='<%# Eval ( "OutboundOrder.Branch.Name" ) %>'/>
                                                        </div>  
                                                    </itemtemplate>
                                                </asp:TemplateField>          
                                                <asp:TemplateField HeaderText="Pais" AccessibleHeaderText="CountryName" SortExpression="CountryName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCountryName" runat="server" Text='<%# Eval ("OutboundOrder.CountryDelivery.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Región" AccessibleHeaderText="StateName" SortExpression="StateName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStateName" runat="server" Text='<%# Eval ("OutboundOrder.StateDelivery.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ciudad" AccessibleHeaderText="CityName" SortExpression="CityName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCityName" runat="server" Text='<%# Eval ("OutboundOrder.CityDelivery.Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sello" AccessibleHeaderText="SealNumber" SortExpression="SealNumber" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSealNumber" runat="server" Text='<%# Eval ("LPN.SealNumber") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Lpn Contenedor" AccessibleHeaderText="LpnParent" SortExpression="LpnParent" ItemStyle-CssClass="text">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLpnParent" runat="server" Text='<%# Eval ("LPN.LpnParent") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                        <%-- FIN Grilla Principal --%>
                            
                                                        
                                        <%-- FIN Panel Cerrar Documento --%>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />  
                                        <%--<asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" /> --%>
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" /> 
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" />   
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$imbDeleteReceipt" EventName="Click" /> 
                            
                                        <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> --%>            
                                    </Triggers>
                                </asp:UpdatePanel>
                                <%-- FIN Barra de Estado --%>
                            </div>
                        </div>
                    </div>  

                    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$upGrid"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop"
                    CssClass="updateProgress" TargetControlID="uprGrid" />
                    
                    <%-- FIN Modal Update Progress --%>
                 </Content>
            </TopPanel>
            <BottomPanel HeightMin="50">
                <Content>     
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">           
                                 <%--   grilla detalle inicio--%>
                                 <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">       
                                        <ContentTemplate>                        
                                            <div id="divDetail" runat="server" visible="true" class="divGridDetailScroll">    
                                                
                                              <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                                  <asp:ImageButton ID="imbDeleteReceipt" runat="server" Enabled="true" Height="24px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_cancel.png" OnClick="imbDeleteReceipt_Click" ToolTip="Eliminar detalle" Visible="false" />
	                                            <asp:Label ID="lblGridDetail" runat="server" visible="false" Text="Documento: " />
	                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                              </div>
	                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
	                                                DataKeyNames="Id" EnableViewState="false"
	                                                OnRowCreated="grdDetail_RowCreated" 
                                                    OnRowDataBound="grdDetail_RowDataBound"
                                                    AutoGenerateColumns="false"
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
                                                        <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ("Status") %>'
                                                            Enabled="false" />
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
                                                            <asp:Label ID="lblTotalWeight" runat="server" text='<%# ((decimal) Eval ("TotalWeight") == -1)?" ":Eval ("TotalWeight") %>' />
                                                        </right>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="TotalVolumen" SortExpression="TotalVolumen">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <right>
                                                            <asp:Label ID="lblTotalVolumen" runat="server" text='<%# ((decimal) Eval ("TotalVolumen") == -1)?" ":Eval ("TotalVolumen") %>' />
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
                                                            <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
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
                                                        <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ("Location.IdCode") %>' />
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
                                                        <asp:Label ID="lblLpnTypeCode" runat="server" Text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
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
                                                        <asp:Label runat="server" ID="lblSealNumber" Text='<%# Bind("Seal") %>' />
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
                       
                                           <%-- <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" /> --%>
                                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" />   
                                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> 
                                            <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />   --%>
                                            <asp:AsyncPostBackTrigger ControlID="imbDeleteReceipt" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                 <%--fin grilla detalle--%>
                            </div>
                        </div>  
                    </div>
                    <%--<asp:Button ID="Button1" runat="server" Text="nada" />--%>
                </Content>
                <Footer Height="67">
                    <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </BottomPanel>
        </spl:HorizontalSplitter>
        
        
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
                    
                    <div class="modalControls">
                        <div id="div3" style="width: 100%; margin-bottom: 6px;height: 100px;" >
                            <asp:HiddenField ID="hidIdReceiptMotive" runat="server" Value="-1" />
                            
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDoc" runat="server" Text="Recepción: " />
                                </div>
                                <div class="fieldLeft">
                                    <b><asp:Label ID="lblInboundOrder" runat="server" /></b>
                                </div>
                            </div>
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label1" runat="server" Text="Tipo Recepción: " />
                                </div>
                                <div class="fieldLeft">
                                    <b><asp:Label ID="lblReceiptType" runat="server" /></b>
                                </div>
                            </div>
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTypeRejection" runat="server" Text="Tipo Rechazo: " />
                                </div>
                                <div class="fieldLeft">
                                    <b> <asp:DropDownList ID="ddlTypeRejection" runat="server"></asp:DropDownList></b>
                                    <asp:RequiredFieldValidator ID="rfvTypeRejection" runat="server" InitialValue="-1"
                                        Text=" * " ErrorMessage="Tipo Rechazo es requerido" ControlToValidate="ddlTypeRejection" />
                                </div>
                            </div>
                            <div id="divMotiveRejection" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMotiveRejection" runat="server" Text="Motivo Rechazo: " />
                                </div>
                                <div class="fieldLeft">
                                    <b><asp:DropDownList ID="ddlMotiveRejection" runat="server"></asp:DropDownList></b>
                                    <asp:RequiredFieldValidator ID="rfvMotiveRejection" runat="server" InitialValue="-1"
                                         Text=" * " ErrorMessage="Motivo Rechazo es requerido" ControlToValidate="ddlMotiveRejection" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="divCtrsFloatLeft">       
                    </div>
                    <div style="clear:both" />                       
                    <div id="Div1" runat="server" class="modalActions">
                        <asp:Button ID="btnSaveConfirm" runat="server"  Text="Aceptar" CausesValidation="true" 
                          OnClick="btnSaveConfirm_Click" />
                    </div>                        
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
			              <%-- <asp:Label ID="lblDialogTitle" runat="server" />	 --%> 
                            Crear Ruta
                        </div>
	                </asp:Panel>
            	    
                    <div id="divDialogPanel" class="divDialogPanel" runat="server">
                        <div class="divDialogMessage">
                            <asp:Image id="imgDialogIcon" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
                        </div>
                        <div id="divDialogMessage" runat="server" class="divDialogMessage">       
                            ¿Desea Confirmar La Creacion De Ruta(s) SimpliRoute?
                        </div>
                        <div id="divConfirm" runat="server" class="divDialogButtons">
                            <asp:Button ID="btnOk" runat="server" Text="   Sí   " OnClick="btnOk_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="   No   " OnClick="btnCancel_Click" />
                        </div> 
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
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" />   
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> 
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />   
        </Triggers>
        </asp:UpdatePanel>
        <%-- FIN Panel Cerrar Auditoria --%>
        
        
    </div>  
    
    
     
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Documento?" Visible="false" />
    <asp:Label ID="lblDetailOrder" runat="server" Text="¿Ver detalle" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Fecha" Visible="false" />
    <asp:Label id="lblCodeLpn" runat="server" Text="LPN" Visible="false" />  
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Button ID="btnDetail" runat="server" Text="" OnClick="btnDetail_Click" />
    <asp:Label ID="lblConfirmCreateSimpliRoute" runat="server" Text="¿Desea Confirmar La Creacion De Ruta(s) SimpliRoute?" Visible ="false"  />
    <asp:Label ID="lblConfirmCreateSimpliRouteHeader" runat="server" Text="Crear Ruta" Visible ="false"  />
    <asp:Label ID="lblBtnSaveToolTip" runat="server" Text="Crear Ruta" Visible ="false"  />
    <asp:Label ID="lblNotSelectedRow" runat="server" Text="Debe Seleccionar Al Menos Una Fila" Visible ="false"  />
    <asp:HiddenField ID="hdIndexGrd" runat="server" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    
    <script type="text/javascript" language="javascript">
        function ViewDetail(grd) {
            var index = grd.parentElement.parentElement.parentElement.parentElement.rowIndex;
            var btnDetail = document.getElementById("ctl00$MainContent$btnDetail");
            document.getElementById('ctl00$MainContent$hdIndexGrd').value = index-1;
            
            btnDetail.click();
            return false;
        }
        function CheckAllEmp(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdMgr.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                GridVwHeaderChckbox.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }
        function resizeDivPrincipal() {
            //debugger;
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
        }
    </script>
    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
