
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" 
CodeBehind="ReleaseOrderForLpn.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.ReleaseOrderForLpn" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
<%--<%@ Register TagPrefix="webUc" Assembly="Flan.Controls" Namespace="Flan.Controls"  %>--%>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">

//    window.onresize = resizeDiv;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);

    function resizeDiv() {
      
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
//debugger;
//        var divPrin = document.getElementById("hsMasterDetail_LeftP_Content").style.height;
//        
//        if (divPrin > document.body.clientHeight)
            
        
    }
   

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

    function index() {
        var indexNew = document.getElementById("ctl00_MainContent_pnlLocRelease").style.zIndex;
        document.getElementById("ctl00_MainContent_mpeLocRelease_backgroundElement").style.zIndex = indexNew - 1;
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
        initializeGridDragAndDrop('OutboundOrder_GetByTrackFilter', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr', 'ReleaseOrderForLpn');
    }

    $(document).ready(function () {
        initializeGridWithNoDragAndDrop(true);
        Sys.Application.add_init(appl_init);
    });

    function appl_init() {
        var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
        pgRegMgr.add_beginRequest(beforeAsyncPostBack);
        pgRegMgr.add_endRequest(afterAsyncPostBack);
    }

    function beforeAsyncPostBack() {

    }

    function afterAsyncPostBack() {
        initializeGridWithNoDragAndDrop(true);
    }
   
</script>

<style type="text/css">
    .legendCaption
    {
    	font-family: Verdana, Helvetica, Sans-Serif;
        font-weight:  bold;
        font-size: 11px;
        padding: 0px;
    }
    
   /*.mpeLocRelease
    {
         z-index: 100004;	
    }*/

   #ctl00_MainContent_pnlReleaseDispatch{
       z-index: 10000001 !important;
   }
</style>

<div id="divPrincipal">
    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">  
        <%-- Ordenes Pendientes --%>
        <TopPanel ID="topPanel" HeightMin="50">
            <Content> 
                 <div class="container">
                    <div class="row">
                        <div class="col-md-12">   
                            <asp:UpdatePanel ID="upPendingOrders" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>                     
                            
                                    <%-- Grilla Principal --%>   
                                    <asp:GridView ID="grdMgr" runat="server" 
                                        DataKeyNames="Id"
                                        OnRowCreated="grdMgr_RowCreated"
                                        OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                        OnRowCommand ="grdMgr_RowCommand"
                                        AllowPaging="true" 
                                        EnableViewState="False"
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdMgr_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false">
                                        <Columns>
                           
                                       <%-- IMPORTANTE: no cambiar esta columna de lugar --%>
                                        <asp:TemplateField  HeaderText="Acción" AccessibleHeaderText="Actions" >
                                            <%--<HeaderTemplate>
                                    
                                            </HeaderTemplate> --%>               
                                            <ItemTemplate> 
                                                <center>
                                                <div style="width:20px">
                                                    <asp:ImageButton ID="btnReleasePopUp" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_release.png" 
                                                    ToolTip="Liberar" CommandName="ReleaseLPN" CommandArgument="<%# Container.DataItemIndex %>"/>	
                                                </div>	                        
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
       
                                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id" />

                                        <asp:templatefield headertext="En Otra Sim." accessibleHeaderText="InOtherSimulation" SortExpression="InOtherSimulation">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkInSimulation" runat="server" checked='<%# Eval ( "InOtherSimulation" ) %>' Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>
                            
                                        <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                                             
                                        <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                               </div>                                                        
                                            </itemtemplate>
                                         </asp:templatefield>
                             
                                        <asp:templatefield HeaderText="Cód. CD. Destino" AccessibleHeaderText="WarehouseTargetCode">
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

                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Owner.Name" ) %>'/>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField HeaderText="Nº Doc." ItemStyle-Wrap="false" DataField="Number" 
                                            AccessibleHeaderText="OutboundNumber" >
                                        </asp:BoundField>
                           
                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "OutboundType.Code" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeName" HeaderText="Tipo Doc.">
                                            <ItemTemplate>
                                                <div style="woerd-wrap: break-word;">
                                                    <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ("OutboundType.Name") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                                </center>    
                                        </itemtemplate>
                                        </asp:templatefield>
                            
                                        <asp:BoundField DataField="ReferenceNumber" HeaderText="Nº Ref." AccessibleHeaderText="ReferenceNumber">
                                        </asp:BoundField>
                            
                                        <asp:BoundField DataField="LoadCode" HeaderText="Cód. Carga" 
                                            ItemStyle-Wrap="false"  AccessibleHeaderText="LoadCode">

                                        </asp:BoundField>
                                        <asp:BoundField DataField="LoadSeq" HeaderText="Sec. Carga" 
                                            AccessibleHeaderText="LoadSeq" ItemStyle-Wrap="false">

                                        </asp:BoundField>
                                        <asp:BoundField DataField="Priority" HeaderText="Prioridad" 
                                            AccessibleHeaderText="Priority" ItemStyle-Wrap="false">
                                        </asp:BoundField>
                            
                                        <asp:templatefield headertext="Liberación Autom." accessibleHeaderText="InmediateProcess">
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
                                        <asp:TemplateField HeaderText="Emisión" AccessibleHeaderText="EmissionDate" SortExpression="EmissionDate">
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
                                        <asp:TemplateField HeaderText="Cancelación" AccessibleHeaderText="CancelDate" SortExpression="CancelDate">
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
                            
                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "CustomerCode" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>


                                        </asp:TemplateField>                
                                        <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "CustomerName" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>                                              
                            
                                        <asp:BoundField DataField="DeliveryAddress1" HeaderText="Dirección Entrega" AccessibleHeaderText="DeliveryAddress1"/>
                                        <asp:BoundField DataField="DeliveryAddress2" HeaderText="Dirección Entrega Opc." AccessibleHeaderText="DeliveryAddress2"/>
                            
                                        <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryDelivery" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCountryDeliveryName" runat="server" text='<%# Eval ( "CountryDelivery.Name" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>

                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateDelivery" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblStateDeliveryName" runat="server" text='<%# Eval ( "StateDelivery.Name" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>

                                        </asp:TemplateField>         
                            
                                        <asp:TemplateField HeaderText="Comuna Entrega" AccessibleHeaderText="CityDelivery" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCityDeliveryName" runat="server" text='<%# Eval ( "CityDelivery.Name" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>

                                        </asp:TemplateField>                         
                            
                                        <asp:BoundField DataField="DeliveryPhone" HeaderText="Teléfono" AccessibleHeaderText="DeliveryPhone"/>
                                        <asp:BoundField DataField="DeliveryEmail" HeaderText="E-mail" AccessibleHeaderText="DeliveryEmail"/>
                            
                                        <asp:TemplateField HeaderText="Compl." AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "FullShipment" ) %>' Enabled="false" />
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Transportista" AccessibleHeaderText="CarrierCode" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCarrierCode" runat="server" text='<%# Eval ( "Carrier.Code" ) %>'/>
                                                     </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>  
                            
                                        <asp:BoundField DataField="RouteCode" HeaderText="Ruta" AccessibleHeaderText="RouteCode"/>
                                        <asp:BoundField DataField="Plate" HeaderText="Patente" AccessibleHeaderText="Plate"/>
                                        <asp:BoundField DataField="Invoice" HeaderText="Nº Factura" AccessibleHeaderText="Invoice"/>
                                        <asp:BoundField DataField="FactAddress1" HeaderText="Dirección Factura" AccessibleHeaderText="FactAddress1"/>
                                        <asp:BoundField DataField="FactAddress2" AccessibleHeaderText="FactAddress2" HeaderText="Dirección Factura Opc." />
                                                                                
                                       <asp:TemplateField HeaderText="País Factura" AccessibleHeaderText="CountryFact" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCountryFactName" runat="server" text='<%# Eval ( "CountryFact.Name" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>

                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Región Factura" AccessibleHeaderText="StateFact" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblStateFactName" runat="server" text='<%# Eval ( "StateFact.Name" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>

                                        </asp:TemplateField>         
                            
                                        <asp:TemplateField HeaderText="Comuna Factura" AccessibleHeaderText="CityFact" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCityFactName" runat="server" text='<%# Eval ( "CityFact.Name" ) %>'/>
                                                    </div>
                                                </center>    
                                            </itemtemplate>

                                        </asp:TemplateField>   
                                                            
                                        <asp:BoundField DataField="FactPhone" HeaderText="Tel. Factura" AccessibleHeaderText="FactPhone"/>
                                        <asp:BoundField DataField="FactEmail" HeaderText="E-mail Factura" AccessibleHeaderText="FactEmail"/>
                                     
                                        <asp:BoundField DataField="SpecialField1" HeaderText="Campo. Esp. 1" AccessibleHeaderText="SpecialField1"
                                            SortExpression="SpecialField1" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SpecialField2" HeaderText="Campo. Esp. 2" AccessibleHeaderText="SpecialField2"
                                            SortExpression="SpecialField2" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SpecialField3" HeaderText="Campo. Esp. 3" AccessibleHeaderText="SpecialField3"
                                            SortExpression="SpecialField3" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SpecialField4" HeaderText="Campo. Esp. 4" AccessibleHeaderText="SpecialField4"
                                            SortExpression="SpecialField4" ItemStyle-Wrap="false"  ItemStyle-CssClass="text">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundField>
                                     </Columns>
                                    </asp:GridView>
                                    <%-- Fin Grilla Principal --%> 
                        
                                </ContentTemplate>
                                <Triggers>
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />  
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnReleaseLpn" EventName="Click" />
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
            </Content>
        </TopPanel>
        
        <BottomPanel HeightMin="100">
            <Content>
                 <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:UpdatePanel ID="upSelectedOrders" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>                                
                        
                                    <%-- Detalle --%>
                                    <div id="divDetail" runat="server" visible="false" class="divGridDetail" style="width:100%">
                                        <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                            <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
                                            <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                        </div>
            	            
                                      <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                            DataKeyNames="Id" 
                                            EnableViewState="False" 
                                            AllowPaging="False"
                                            OnRowCreated="grdDetail_RowCreated" 
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdDetail_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                       
                                            <Columns>
                                                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" AccessibleHeaderText="Id"/>
                                 
                                                <asp:BoundField DataField="LineNumber" HeaderText="Nº Línea" AccessibleHeaderText="LineNumber" 
                                                    ItemStyle-HorizontalAlign="Center" >
                                            
                                                </asp:BoundField>
                                                <asp:BoundField DataField="LineCode" HeaderText="Cód. Línea"  AccessibleHeaderText="LineCode"
                                                    ItemStyle-HorizontalAlign="Center" >
                                                </asp:BoundField>
                                    
                                               <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'/>
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
                                   
                                                <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Item">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "Item.Description" ) %>'/>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgItem">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblCtgItem2" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'/>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                
                                                <asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"/>
                                    
                                                <asp:BoundField DataField="ItemStock" HeaderText="Stock" AccessibleHeaderText="ItemStock"/>
                                   
                                                <asp:templatefield headertext="Activo" AccessibleHeaderText="DetailStatus">
                                                    <ItemTemplate>
                                                         <asp:CheckBox ID="chkDetailStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                                    </ItemTemplate>
                                                </asp:templatefield>

                                                <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber"/>
                                    
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
                                    
                                                <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
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
                                    <%-- FIN Detalle --%>
                        
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
                                    <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnReleaseLpn" EventName="Click" />
                               </Triggers>      
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </BottomPanel>
        
    </spl:HorizontalSplitter>  
</div>
   
    <%-- PopUp Liberar Pedidos --%>
     <asp:UpdatePanel ID="upRelease" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="divReleaseDispatch" runat="server" visible="false">
	            <asp:Button ID="btnDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolKit:ModalPopupExtender 
                    ID="mpReleaseDispatch" runat="server" TargetControlID="btnDummy" 
                    PopupControlID="pnlReleaseDispatch"  
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="Caption" Drag="true" >
                </ajaxToolKit:ModalPopupExtender>
            	
                <asp:Panel ID="pnlReleaseDispatch" Width="1200px" Height="580px" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>			
	                <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
	                    <div class="divCaption">
		                    <asp:Label ID="lblEdit" runat="server" Text="Liberar por LPN"/>
                            <asp:ImageButton ID="btnClose" runat="server" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar"/>
                        </div>			        
                    </asp:Panel>
                    <%-- Fin Encabezado --%>    
	                <div id="divPrincipalLpn" runat="server" class="modalControls" >
	                    <div runat="server" style="float:left">
	                        <fieldset style="Height:465px; width:560px">
	                        <legend  class="legendCaption">Datos Pedido</legend>
	                            <asp:HiddenField ID="hidIdWhs" runat="server" Value="-1" />
                                <asp:HiddenField ID="hidIdOwn" runat="server" Value="-1" />
                                <asp:HiddenField ID="hidCustomerCode" runat="server" Value="-1" />
	                            
                                <div id="div2"  class="divControls" style="display: none;" >
                                    <div style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblIdOutboundOrder" runat="server" Text="Id" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtIdOutboundOrder" runat="server" Width="100px"  Enabled="false"/>  
                                     </div>
                                </div>
                                <div style="position: relative;">   
                                    <div  style="position: absolute; top:0px; right: 10px;">
                                        <asp:Label ID="Label2" runat="server" style=" font-size:10px; " Text="Nivel Satisfacción" />   
                                    </div>                              
                                    <div style="position: absolute; top:20px; right: 25px;">                                          
                                        <asp:TextBox ID="txtPrecent" runat="server" Width="50px" ReadOnly="true" BackColor="#FFFFFF" style="text-align:center; color:#FF3300" Text="0%"/>  
                                    </div>                                    
                                </div>                                   
                               
                                <div id="div3"  class="divControls" >
                                    <div  style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblOutboundNumber" runat="server" Text="Nº Doc." />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtOutboundNumber" runat="server" Width="150px" BackColor="#FFFFCC" ReadOnly="true"/>                                    
                                    </div>
                                </div>
	                                
                                <div id="div4"  class="divControls" >
                                    <div style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblNameWarehouse" runat="server" Text="Centro Dist." /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtNameWarehouse" runat="server" Width="150px" BackColor="#FFFFCC" ReadOnly="true"/>                                       
                                    </div>
                                </div>
                                                                    
                                <div id="divOwner"  class="divControls" >
                                    <div style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblOwner" runat="server" Text="Dueño" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtNameOwner" runat="server" Width="150px" BackColor="#FFFFCC" ReadOnly="true"/>                                       
                                    </div>
                                </div>
                                                                
                                <div id="divIdInboundType"  class="divControls" >
                                    <div style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblOutboundType" runat="server" Text="Tipo Doc." /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtOutboundType" runat="server" Width="150px" BackColor="#FFFFCC" ReadOnly="true"/>
                                    </div>
                                </div>                                
                                   
                                <div id="divVendor"  class="divControls">
                                    <div style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblCustomer" runat="server" Text="Cliente" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtCustomer" runat="server" Width="150px" BackColor="#FFFFCC" ReadOnly="true"/>
                                    </div>
                                </div>

                                <div id="divEmissionDate"  class="divControls" >
                                    <div style=" width:70px" class="fieldRight">
                                        <asp:Label ID="lblEmissionDate" runat="server" Text="Emisión" /></div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtEmissionDate" runat="server" BackColor="#FFFFCC" Width="150px" ReadOnly="true"/>                                 
                                   </div>
                                </div>
	                                
	                            <div runat="server" class="divGridDetail" style=" height:270px; width:550px; border: 1px solid #CCCCCC; overflow-x:scroll ; overflow-y: scroll; margin-left:1px;">
	                                <asp:GridView ID="grdDetailPopUp" runat="server" SkinID="grdDetail"
                                        DataKeyNames="Id"  Visible="true"
                                        EnableViewState="False" 
                                        AllowPaging="False"
                                        OnRowCreated="grdDetailPopUp_RowCreated" 
                                        OnRowDataBound="grdDetailPopUp_RowDataBound"
                                        AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                        EnableTheming="false">
                                   
                                        <Columns>
                                            <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" AccessibleHeaderText="Id"/>
                                             
                                            <asp:BoundField DataField="LineNumber" HeaderText="Nº Línea" AccessibleHeaderText="LineNumber" 
                                                ItemStyle-HorizontalAlign="Center" >                                                        
                                            </asp:BoundField>
                                                
                                            <%--<asp:BoundField DataField="LineCode" HeaderText="Cód. Línea"  AccessibleHeaderText="LineCode"
                                                ItemStyle-HorizontalAlign="Center" >
                                            </asp:BoundField>--%>
                                                
                                            <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemCode">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblItemCode" runat="server" text='<%# Eval ( "Item.Code" ) %>'/>
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
                                               
                                            <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Item">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "Item.Description" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                                
                                            <%--<asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgItem">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCtgItem2" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            
                                            <asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty"/>
                                                
                                            <asp:BoundField DataField="ItemStock" HeaderText="Stock" AccessibleHeaderText="ItemStock"/>
                                               
                                            <asp:templatefield headertext="Activo" AccessibleHeaderText="DetailStatus">
                                                <ItemTemplate>
                                                        <asp:CheckBox ID="chkDetailStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                                </ItemTemplate>
                                            </asp:templatefield>

                                            <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber"/>
                                                
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
                                                
                                            <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
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
	                        </fieldset>
	                    </div>
	                    
	                    <div runat="server" style="float:right">
	                        <fieldset style="Height:465px; width:600px">
	                        <legend  class="legendCaption">Asignar Bultos</legend>
    	                        <asp:UpdatePanel ID="udpLpn" runat="server" UpdateMode="Always">
                                <ContentTemplate>
    	                        <div id="div1"  class="divControls" >
                                    <div class="fieldRight">
                                        <asp:Label ID="lblLpnCode" runat="server" Text="Lpn" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtLpnCode" runat="server" ></asp:TextBox>  
                                        <asp:ImageButton ID="btnSearchLpn" runat="server" ToolTip="Buscar" OnClick="btnSearchLpn_Click"
                                        ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"  Width="20px" Height="20px"
                                        onmouseover="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search_on.png';"
                                        onmouseout="this.src='../../WebResources/Images/Buttons/TaskBar/icon_search.png';" />                               
                                    </div>
                                </div>
                                
                                <div id="Div5" runat="server" class="divGridDetail" style=" height:195px; width:590px; border: 1px solid #CCCCCC; overflow-x:scroll ; overflow-y: scroll; margin-left:1px;">
                                    <asp:GridView ID="grdLpn" runat="server" SkinID="grdDetail"
                                        DataKeyNames="Id"  Visible="true"
                                        EnableViewState="False"                                          
                                        AllowPaging="false"
                                        OnRowCommand = "grdLpn_RowCommand"
                                        OnSelectedIndexChanged ="grdLpn_SelectedIndexChanged"
                                        OnRowCreated="grdLpn_RowCreated" 
                                        OnRowDataBound="grdLpn_RowDataBound"
                                        AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                        EnableTheming="false">
                                        <Columns>
                                            
                                            <%-- IMPORTANTE: no cambiar esta columna de lugar --%>
                                            <asp:TemplateField ShowHeader="true" AccessibleHeaderText="Actions" >
                                                <HeaderTemplate>                                                        
                                                </HeaderTemplate>                
                                                <ItemTemplate> 
                                                    <center>
                                                    <div style="width:20px">
                                                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                                                        <%--<input type="checkbox" ID="chkSelectOrder" CommandArgument="Pollo" />--%>
                                                        <%--onclick="ValidateLpn('<%# Eval ( "Lpn.IdCode" ) %>');"--%>
                                                    </div>	                        
                                                    </center>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                            <asp:TemplateField HeaderText="Lpn" AccessibleHeaderText="IdLpnCode">
                                                <ItemTemplate>
                                                    <div style=" width:100px; word-wrap: break-word;">
                                                        <asp:Label ID="lblIdLpnCode" runat="server" text='<%# Eval ( "Lpn.IdCode" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                           
                                            <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode">
                                                <ItemTemplate>
                                                    <div style=" width:100px; word-wrap: break-word;">
                                                        <asp:Label ID="lblLocCode" runat="server" text='<%# Eval ( "Location.IdCode" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                            <asp:TemplateField HeaderText="Tipo Ubicación" AccessibleHeaderText="LocTypeName">
                                                <ItemTemplate>
                                                    <div style=" width:100px; word-wrap: break-word;">
                                                        <asp:Label ID="lblLocTypeName" runat="server" text='<%# Eval ( "Location.Type.LocTypeName" ) %>'/>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                                                                                                                           
                                        </Columns>
                                    </asp:GridView>
                                </div>
    	                        
    	                        <div id="div6" runat="server" class="divGridDetailScroll" style=" height:185px; width:590px; border: 1px solid #CCCCCC; overflow-x:scroll ; overflow-y: scroll; margin-left:1px;">
                                    <asp:GridView ID="grdLpnDetail" runat="server" 
                                        DataKeyNames="Id" EnableViewState="false"                                         
                                        OnRowCreated="grdLpnDetail_RowCreated" SkinID="grdDetail"
                                        OnRowDataBound="grdLpnDetail_RowDataBound"
                                        AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
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
                                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval ("Status") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                                <ItemStyle Wrap="false" />
                                                <ItemTemplate>
                                                    <right>
                                                        <asp:Label ID="lblQty" runat="server" 
                                                        text='<%# ((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty") %>' />
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
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSearchLpn" EventName="Click" /> 
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$grdLpn" EventName="RowCommand" /> 
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$grdLpn" EventName="SelectedIndexChanged" />   
                                                                    
                                </Triggers>   
     	                        </asp:UpdatePanel>
     	                        
     	                         <asp:UpdateProgress ID="uprLpn" runat="server" AssociatedUpdatePanelID="udpLpn" DisplayAfter="20" DynamicLayout="true">
                                    <ProgressTemplate>
                                        <div class="divProgress">
                                            <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprLpn" runat="server" ControlToOverlayID="divPrincipalLpn"
                                 CssClass="updateProgress" TargetControlID="uprLpn" />
	                        </fieldset>
	                    </div>
             
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnRelease" runat="server"  Text="Liberar" CausesValidation="true" OnClick="btnRelease_Click"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" OnClick="btnCancel_Click" />
                        </div>  
                                          
                    </div>
                </asp:Panel>
	        </div>
	        
	        
        </ContentTemplate>
        <Triggers>
             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSearchLpn" EventName="Click" /> 
             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnRelease" EventName="Click" /> 
             <asp:AsyncPostBackTrigger ControlID="ctl00$ucContentDialog$btnClose" EventName="Click" />       
             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$grdLpn" EventName="SelectedIndexChanged" />  
             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnReleaseLpn" EventName="Click" />                                
        </Triggers>   
     </asp:UpdatePanel>       
           
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="divLocRelease" runat="server" visible="true">
	            <asp:Button ID="btnSalir" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolKit:ModalPopupExtender 
                    ID="mpeLocRelease" runat="server" TargetControlID="btnSalir" 
                    PopupControlID="pnlLocRelease"  
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="Caption" Drag="true" >
                </ajaxToolKit:ModalPopupExtender>
            	
                <asp:Panel ID="pnlLocRelease" Width="500px" Height="220px" runat="server" CssClass="modalBox">
           
                <%-- Encabezado --%>			
	                <asp:Panel ID="Panel2" runat="server" CssClass="modalHeader">
	                    <div class="divCaption">
		                    <asp:Label ID="Label1" runat="server" Text="Liberar por LPN"/>
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar"/>
                        </div>			        
                    </asp:Panel>
                    <%-- Fin Encabezado --%>    
	                <div id="div8" runat="server" class="modalControls" >
	                    <div id="divLocStageDispatch" runat="server" class="divControls">      
                            <div class="fieldRight">
                                <asp:Label ID="lblLocStageDispatch" runat="server" Text="Ubicación de Embalaje" />
                            </div> 
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlLocStageDispatch" runat="server" Width="100" />
                            </div>
                        </div>
                        <div id="divLocDock" runat="server" class="divControls">      
                            <div class="fieldRight">
                                <asp:Label ID="lblLocDock" runat="server" Text="Ubicación de Andén" />
                            </div> 
                            <div class="fieldLeft">
                                <asp:DropDownList ID="ddlLocDock" runat="server" Width="100" />
                            </div>
                        </div>
                        <div id="divErrorMessage" visible="false" runat="server" style="text-align:center; 
                            font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal;font-size: 11px; color:Red;"> 
                            <br />
                            <ul>
                                <li>
                                    <asp:Label ID="lblErrorMessage" runat="server"></asp:Label> 
                                </li>                     
                           </ul>
                        </div>
                        <div id="div7" runat="server" class="modalActions">
                            <asp:Button ID="btnReleaseLpn" runat="server"  Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" OnClick="btnReleaseLpn_Click" />
                            <asp:Button ID="btnCancelLpn" runat="server" Text="Cancelar" OnClick="btnCancelLpn_Click" />
                        </div>    
	                </div>
	                
	                </asp:Panel>
	        </div>
        </ContentTemplate>
        <Triggers>     
             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnRelease" EventName="Click" />     
             <asp:AsyncPostBackTrigger ControlID="ctl00$ucContentDialog$btnClose" EventName="Click" />   
             <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnReleaseLpn" EventName="Click" />  
        </Triggers>   
     </asp:UpdatePanel> 
     

     
     
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
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />  
    <asp:Label ID="lblNoLpnSelected" runat="server" Text="Debe seleccionar al menos un LPN." Visible="false" />    	
    <asp:Label ID="lblMaxOrdeSelected" runat="server" Text="Debe seleccionar solo una orden por liberación." Visible="false" />    
    <asp:Label ID="lblNoWhsSelected" runat="server" Text="Las Ordenes seleccionadas deben pertenecer al mismo Centro de Distribución." Visible="false" />    	
    <asp:Label ID="lblNoOwnerSelected" runat="server" Text="Las Ordenes seleccionadas deben pertenecer al mismo Dueño." Visible="false" />    
    <asp:Label ID="lblTitle" runat="server" Text="Liberar Pedido por LPN" Visible="false"/>
    <asp:Label ID="lblMsgErrorUbic" runat="server" Text="Debe seleccionar al menos una ubicación." Visible="false" />  
    <asp:Label ID="lblErrorLpnQty" runat="server" Text="Lpn supera la cantidad para los ítems solicitados ." Visible="false" />  
<%-- FIN Mensajes de Confirmacion y Auxiliares --%>  

</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>
</asp:Content>