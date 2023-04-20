<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    Culture="es-ES" UICulture="es-ES" CodeBehind="OutboundOrderPackageB2B.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundOrderPackageB2B" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1"  ContentPlaceHolderID="MainContent"  runat="server"  >

<script type="text/javascript" language='Javascript'>
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

        removeFooterHeader();
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('OutboundOrder_FindAllPrecubing', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');

        removeFooterHeader();
    }

    function removeFooterHeader() {
        removeFooter("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr");
    }
    
</script> 

    <div id="divPrincipal" style="margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter   CookieDays="0"  ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel"  >
            <Header Height="30">
                <asp:UpdatePanel ID="upSimulacion" runat="server" UpdateMode="Always">
                    <ContentTemplate>
				        <div class="divGridTitleDispatch">
                            <div class="divCenter">
                                <asp:Label ID="lblSelectedOrders" runat="server" Text="Asignar Pedidos" />
                            </div>                    
                            <asp:ImageButton ID="btnReprocess" runat="server" onclick="btnReprocess_Click" Enabled="true" 
                            ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process.png" ToolTip="Simular Pedidos"/>                     
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
			</Header>
            <Content>
                <div style="width:100%;height:100%;"  >
                    <%-- Panel Grilla Principal --%>
                    <asp:UpdatePanel ID="upGrid"  runat="server" UpdateMode="Always">
                        <ContentTemplate >  
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">
                                        <%--<div runat="server" class="divGrid" onresize="SetDivs();">--%>
                                        <asp:GridView ID="grdMgr" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True" 
                                            EnableViewState="False"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                        <Columns>
                                             <%-- IMPORTANTE: no cambiar esta columna de lugar --%>
                                            <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions" >
                                                <HeaderTemplate>
                                                    <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
                                                </HeaderTemplate>                
                                                <ItemTemplate> 
                                                    <center>
                                                    <div style="width:20px">
                                                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                                                    </div>	                        
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                                
                                            <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                            </asp:BoundField>
                                            <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" >                                                <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                </div>
                                                </itemtemplate>
                                             </asp:templatefield>
                                            <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                <itemtemplate>
                                                   <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                   </div>
                                                </itemtemplate>
                                             </asp:templatefield>
                                            <asp:templatefield HeaderText="C&#243;d. CD. Destino" AccessibleHeaderText="WarehouseTargetCode"  >
                                                <itemtemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblWarehouseTargeteCode" runat="server" text='<%# Eval ( "WarehouseTarget.Code" ) %>' />
                                                    </div>
                                                </itemtemplate>
                                             </asp:templatefield>
                                            <asp:templatefield HeaderText="Centro Dist. Destino" AccessibleHeaderText="WarehouseTarget">
                                                <itemtemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblWarehouseTarget" runat="server" text='<%# Eval ( "WarehouseTarget.ShortName" ) %>' />
                                                   </div>
                                                </itemtemplate>
                                             </asp:templatefield>                 
                                            <asp:TemplateField HeaderText="C&#243;d. Due&#241;o" AccessibleHeaderText="OwnerCode" >
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Due&#241;o" AccessibleHeaderText="Owner">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="N&#176; Doc." DataField="Number" AccessibleHeaderText="OutboundNumber" />
                                            <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo Doc.">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundType.Code" ) %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="OutboundTypeName">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundType.Name" ) %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "Status" ) %>'
                                                         Enabled="false"/>
                                                    </center>    
                                            </itemtemplate>
                                            </asp:templatefield>
                                            <asp:BoundField DataField="ReferenceNumber" HeaderText="N&#176; Ref." AccessibleHeaderText="ReferenceNumber" >
                                            </asp:BoundField>
                                            <asp:BoundField DataField="LoadCode" HeaderText="C&#243;d. Carga"   AccessibleHeaderText="LoadCode" />
                                            <asp:BoundField DataField="LoadSeq" HeaderText="Sec. Carga" AccessibleHeaderText="LoadSeq" />
                                            <asp:BoundField DataField="Priority" HeaderText="Prioridad" AccessibleHeaderText="Priority" />
                                            <asp:templatefield headertext="Liberaci&#243;n Autom." accessibleHeaderText="InmediateProcess">
                                                <ItemStyle Wrap="false" />
                                                <itemtemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkInmediateProcess" runat="server" checked='<%# Eval ( "InmediateProcess" ) %>' Enabled="false"/>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:templatefield>
                                            <asp:TemplateField HeaderText="Esperada" AccessibleHeaderText="ExpectedDate" SortExpression="ExpectedDate">
                                                <ItemStyle Wrap="false" />                                
                                                    <ItemTemplate>                                        
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblExpectedDate" runat="server" Text='<%# ((DateTime) Eval ("ExpectedDate") > DateTime.MinValue)? Eval("ExpectedDate", "{0:d}"):"" %>' />
                                                               </div>
                                                            </center>                                         
                                                    </ItemTemplate>                                
                                            </asp:TemplateField>  
                                            <asp:TemplateField HeaderText="Emisi&#243;n" AccessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
                                                <ItemStyle Wrap="false" />                                
                                                    <ItemTemplate>                                         
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:Label ID="lblEmissionDate" runat="server" Text='<%# ((DateTime) Eval ("EmissionDate") > DateTime.MinValue)? Eval("EmissionDate", "{0:d}"):"" %>' />
                                                                 </div>
                                                            </center>                                                               
                                                    </ItemTemplate>                                
                                            </asp:TemplateField>     
                                            <asp:TemplateField HeaderText="Salida" AccessibleHeaderText="ShipmentDate" SortExpression="ShipmentDate">
                                                <ItemStyle Wrap="false" />                                
                                                    <ItemTemplate>                                        
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblShipmentDate" runat="server" Text='<%# ((DateTime) Eval ("ShipmentDate") > DateTime.MinValue)? Eval("ShipmentDate", "{0:d}"):"" %>' />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>                                
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                                <ItemStyle Wrap="false" />                                
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                           </div>
                                                        </center>
                                                    </ItemTemplate>                                  
                                            </asp:TemplateField>  
                                            <asp:TemplateField HeaderText="Cancelaci&#243;n" AccessibleHeaderText="CancelDate" SortExpression="CancelDate">
                                                <ItemStyle Wrap="false" />                                
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCancelDate" runat="server" Text='<%# ((DateTime) Eval ("CancelDate") > DateTime.MinValue)? Eval("CancelDate", "{0:d}"):"" %>' />
                                                             </div>
                                                        </center>
                                                    </ItemTemplate>                                
                                            </asp:TemplateField>  
                                            <asp:BoundField DataField="CancelUser" HeaderText="Usuario Cancelación" AccessibleHeaderText="CancelUser"/>
                                            <asp:TemplateField HeaderText="Traza" accessibleHeaderText="OutboundTrack" SortExpression="OutboundTrack">
                                               <ItemStyle Wrap="false" />                               
                                                    <itemtemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:label ID="lblOutboundTrack" runat="server" text='<%# Eval ( "LatestOutboundTrack.Type.Name" ) %>' />
                                                        </div>
                                                    </itemtemplate>                               
                                             </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                             <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "CustomerCode" ) %>'></asp:Label>
                                                         </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField> 
                                            <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                             <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "CustomerName" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>    
                                            <asp:BoundField DataField="DeliveryAddress1" HeaderText="Direcci&#243;n Entrega" AccessibleHeaderText="DeliveryAddress1"/>
                                            <asp:BoundField DataField="DeliveryAddress2" HeaderText="Direcci&#243;n Entrega Opc." AccessibleHeaderText="DeliveryAddress2"/>
                                            <asp:TemplateField HeaderText="Pa&#237;s Entrega" AccessibleHeaderText="CountryDelivery" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblCountryDeliveryName" runat="server" text='<%# Eval ( "CountryDelivery.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Regi&#243;n Entrega" AccessibleHeaderText="StateDelivery" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                         <asp:Label ID="lblStateDeliveryName" runat="server" text='<%# Eval ( "StateDelivery.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>         
                                            <asp:TemplateField HeaderText="Ciudad Entrega" AccessibleHeaderText="CityDelivery" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                             <asp:Label ID="lblCityDeliveryName" runat="server" text='<%# Eval ( "CityDelivery.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>                         
                                            <asp:BoundField DataField="DeliveryPhone" HeaderText="Tel. Entrega" AccessibleHeaderText="DeliveryPhone"/>
                                            <asp:BoundField DataField="DeliveryEmail" HeaderText="E-mail Entrega" AccessibleHeaderText="DeliveryEmail"/>
                                            <asp:TemplateField HeaderText="Preparar Completo" AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                                <itemtemplate>
                                                    <center>
                                                        <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "FullShipment" ) %>' Enabled="false" />
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Transportista" AccessibleHeaderText="CarrierCode" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                             <asp:Label ID="lblCarrierCode" runat="server" text='<%# Eval ( "Carrier.Code" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>  
                                            <asp:BoundField DataField="RouteCode" HeaderText="Ruta" AccessibleHeaderText="RouteCode"/>
                                            <asp:BoundField DataField="Plate" HeaderText="Patente" AccessibleHeaderText="Plate"/>
                                            <asp:BoundField DataField="Invoice" HeaderText="N&#176; Factura" AccessibleHeaderText="Invoice" />
                                            <asp:BoundField DataField="FactAddress1" HeaderText="Direcci&#243;n Factura" AccessibleHeaderText="FactAddress1"/>
                                            <asp:BoundField DataField="FactAddress2" AccessibleHeaderText="FactAddress2" HeaderText="Direcci&#243;n Factura Opc." />
                                            <asp:TemplateField HeaderText="Pa&#237;s Factura" AccessibleHeaderText="CountryFact" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblCountryFactName" runat="server" text='<%# Eval ( "CountryFact.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Regi&#243;n Factura" AccessibleHeaderText="StateFact" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblStateFactName" runat="server" text='<%# Eval ( "StateFact.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>         
                                            <asp:TemplateField HeaderText="Ciudad Factura" AccessibleHeaderText="CityFact" >
                                                <itemtemplate>
                                                    <center>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblCityFactName" runat="server" text='<%# Eval ( "CityFact.Name" ) %>'></asp:Label>
                                                        </div>
                                                    </center>    
                                                </itemtemplate>
                                            </asp:TemplateField>   
                                            <asp:BoundField DataField="FactPhone" HeaderText="Tel. Factura" AccessibleHeaderText="FactPhone"/>
                                            <asp:BoundField DataField="FactEmail" HeaderText="E-mail Factura" AccessibleHeaderText="FactEmail"/>
                                            <asp:BoundField DataField="SpecialField4" HeaderText="Total Bultos" AccessibleHeaderText="SpecialField4"/>
                                         </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                            <%--</div>--%>
                       </ContentTemplate>
                       <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />             
                         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl00$btnReprocess" EventName="Click" />
                      </Triggers>
                    </asp:UpdatePanel>  
                    
                    <%-- FIN Panel Grilla Principal --%>
               </div> 
            </Content>
        </TopPanel>
        <BottomPanel HeightMin="50">
            <Header Height="20">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                    <ContentTemplate>   
                        <div id="divDetailTitle" runat="server" class="divGridDetailTitle" visible="false">
                            <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                        </div>
                    </ContentTemplate>
                    <Triggers>  
                    <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />                   
                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl00$btnReprocess" EventName="Click" /> 
                    </Triggers>
                </asp:UpdatePanel>  
            </Header>
            <Content>            
                <div style="width:100%;height:100%;" >
                    <%-- Panel Grilla Detalle --%>
                    <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Always">
                        <ContentTemplate>   
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">   
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                              
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="False" 
                                                AutoGenerateColumns="False"                                     
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                           
                                                <Columns>
                                                    <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" AccessibleHeaderText="Id"/>
                                                    <asp:BoundField DataField="LineNumber" HeaderText="N&#176; L&#237;nea" AccessibleHeaderText="LineNumber" 
                                                        ItemStyle-HorizontalAlign="Center" >
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="LineCode" HeaderText="C&#243;d. L&#237;nea"  AccessibleHeaderText="LineCode"  ItemStyle-CssClass="text"
                                                        ItemStyle-HorizontalAlign="Center" >
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode" ItemStyle-CssClass="text">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>                                           
                                                    <asp:TemplateField HeaderText="Descripci&#243;n" AccessibleHeaderText="Item">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "Item.Description" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Categor&#237;a" AccessibleHeaderText="CtgItem">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCtgItem2" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                        
                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemQty" )) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"/>--%>
                                                    <asp:templatefield headertext="Activo" AccessibleHeaderText="DetailStatus">
                                                        <ItemTemplate>
                                                             <asp:CheckBox ID="chkDetailStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                                        </ItemTemplate>
                                                    </asp:templatefield>
                                                    <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text"/>
                                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                                                </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield>              
                                                    <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                                                </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield> 
                                                    <asp:templatefield headertext="Elaboraci&#243;n" accessibleHeaderText="FabricationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                                </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield>                                                                         
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
                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl00$btnReprocess" EventName="Click" /> 
                      </Triggers>
                    </asp:UpdatePanel>  
                    <asp:UpdateProgress ID="uprDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress0" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprDetail" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprDetail" />
                    <%-- FIN Panel Grilla Detalle --%>
                </div>
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
            
        </BottomPanel>
    </spl:HorizontalSplitter>
    </div>  
    
    <%-- Panel Info Precubing --%>
    <asp:UpdatePanel ID="udpSimulatePrecubing" runat="server" >
    <ContentTemplate>
        <div id="divPrecubing" runat="server" visible="false" class="divItemDetails" >
            <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
            <!-- Boton 'dummy' para propiedad TargetControlID -->
            <ajaxToolkit:ModalPopupExtender ID="mpPrecubing" runat="server" TargetControlID="btnDummy2"
                PopupControlID="pnlPrecubing" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                Drag="true">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlPrecubing" runat="server" CssClass="modalBox" Width="830px" >
                <%-- Encabezado --%>
                <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">                    
                    <div class="divCaption">    
                        <asp:Label ID="lblClose" runat="server" Text="Detalle Simulaci&#243;n B2B" />
                    </div>
                </asp:Panel>
                <%-- Fin Encabezado --%>
                
                <div class="modalControls">
                    <div class="divCtrsFloatLeft">                       
                        <div class="divLookupGrid" style=" width:800px">

                            <div>
                                <asp:Label ID="lblQtyBoxes" Text="Total Cantidad Cajas: " runat="server" />
                                <asp:Label ID="lblSumQtyBoxes" runat="server" />
                            </div>

                            <asp:GridView ID="grdPrecubing" runat="server" AllowPaging="false" ShowFooter="false"
                                OnRowCreated = "grdPrecubing_RowCreated"  
                                AutoGenerateColumns="false"
                                OnRowDataBound="grdPrecubing_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                   <%-- <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id">
                                    <ItemStyle Wrap="False"></ItemStyle>
                                    </asp:BoundField>--%>
                                    <%--<asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" >
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                               <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "OutboundOrder.Warehouse.Code" ) %>' />
                                            </div>
                                        </itemtemplate>
                                    </asp:templatefield>--%>
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <itemtemplate>
                                           <div style="word-wrap: break-word;">
                                            <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "OutboundOrder.Warehouse.ShortName" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                                      
                                   <%-- <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" >
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Due&#241;o" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "OutboundOrder.Owner.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="N&#176; Doc." AccessibleHeaderText="OutboundNumber">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Eval ( "OutboundOrder.Number" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   <%-- <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo Doc.">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundOrder.OutboundType.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Tipo Doc." AccessibleHeaderText="OutboundTypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "OutboundOrder.OutboundType.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                    
                                    <asp:TemplateField HeaderText="N&#176; Ref." AccessibleHeaderText="ReferenceNumber">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Eval ( "OutboundOrder.ReferenceNumber" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="N&#176; Linea" AccessibleHeaderText="LineNumber">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLineNumber" runat="server" Text='<%# Eval ( "OutboundDetail.LineNumber" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="C&#243;d. Item" AccessibleHeaderText="CodeItem">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCodeItem" runat="server" Text='<%# Eval ( "OutboundDetail.Item.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word; text-align:right">
                                                <asp:Label ID="lblItemQty" runat="server" Text='<%# Eval ( "OutboundDetail.ItemQty" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:BoundField HeaderText="Fact. Conversi&#243;n" DataField="ConversionFactor" AccessibleHeaderText="ConversionFactor" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign ="Right" />                                    
                                    <asp:BoundField HeaderText="N&#176; Cajas" DataField="QtyBox" AccessibleHeaderText="QtyBox" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign ="Right" />
                                </Columns>
                                <FooterStyle CssClass="Footer"  BorderColor="#D9E6FD" BorderStyle="Solid"   />
                             </asp:GridView>
                        </div>
                    </div>
                    <div style="clear:both" />                       
                    <div id="Div1" runat="server" class="modalActions">                        
                        <asp:Button ID="btnClosePrecubing" runat="server" Text="Cerrar" ToolTip="Cerrar Simulaci&#243;n" />
                        <asp:Button ID="btnExportToExcelPrecubing" runat="server" Text="Excel" 
                        onclick="btnExportToExcelPrecubing_Click" ToolTip="Exportar a Excel" />
                    </div>   
                </div>                     
            </asp:Panel>
        </div>
        </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl00$btnReprocess" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>  
    <asp:UpdateProgress ID="uprSimulatePrecubing" runat="server" AssociatedUpdatePanelID="udpSimulatePrecubing" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress0" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server" ControlToOverlayID="divTop" 
    CssClass="updateProgress" TargetControlID="uprSimulatePrecubing" />
        <%-- FIN Panel Cerrar Auditoria --%>
    
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Emisi&#243;n" Visible="false" />   
    <asp:Label ID="lblTitle" runat="server" Text="Simulaci&#243;n B2B" Visible="false"/>
    <asp:Label id="lblNoOrdersSelected" runat="server" Text="Debe seleccionar al menos una Orden." Visible="false" />   	
    <asp:Label id="lblItemNotUom" runat="server" Text="Existen items sin unidades de medida creadas." Visible="false" />  
    <asp:Label id="lblLpnTypeNotExists" runat="server" Text="No existen LPNs asociados." Visible="false" />  
    <asp:Label id="lblNotExistsDetailOrders" runat="server" Text="Existen ordenes sin detalle." Visible="false" /> 
    <asp:Label ID="lblDescription" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label ID="lblName" runat="server" Text="Cod. Cliente" Visible="false" /> 
    <asp:Label id="lblFilterReferenceNumber" runat="server" Text="Ord. Compra" Visible="false" /> 
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>
