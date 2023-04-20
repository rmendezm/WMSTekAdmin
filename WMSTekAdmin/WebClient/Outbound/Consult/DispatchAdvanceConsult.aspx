<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="DispatchAdvanceConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.DispatchAdvanceWebConsult" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language='Javascript'>
    function resizeDiv() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
    }
    window.onresize = resizeDiv;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);

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
        initializeGridDragAndDrop('DispatchAdvanced_FindAll', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
    }
</script> 

   <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel" HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>  
                                     <asp:GridView ID="grdMgr" runat="server" 
                                        DataKeyNames="Id" 
                                        OnRowCreated="grdMgr_RowCreated"
                                        OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                        EnableViewState="False"                
                                        AllowPaging="True" 
                                        AutoGenerateColumns="false"
                                        OnRowDataBound="grdMgr_RowDataBound"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false">
                        
                                    <Columns>
                       
                                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id"> </asp:BoundField>
                            
                                        <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Outbound.Warehouse.Code" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                                             
                                        <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                            <itemtemplate>
                                               <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Outbound.Warehouse.ShortName" ) %>' />
                                            </itemtemplate>
                                         </asp:templatefield>
                
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Outbound.Owner.Code" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Outbound.Owner.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                
                                        <asp:TemplateField HeaderText="Nº Doc." AccessibleHeaderText="OutboundNumber" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutboundNumber" runat="server" text='<%# Eval ( "Outbound.Number" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                           
                                        <asp:TemplateField AccessibleHeaderText="OutboundTypeCode" HeaderText="Tipo Doc.">
                                            <ItemTemplate>
                                                <asp:label ID="lblOutboundType" runat="server" text='<%# Eval ( "Outbound.OutboundType.Code" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Nombre Tipo Doc." AccessibleHeaderText="OutboundTypeName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ( "Outbound.OutboundType.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Ola" AccessibleHeaderText="WaveId">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWaveId" runat="server" Text='<%# ((int) Eval ("WaveId") > 0 )? Eval("WaveId"): "" %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Traza" AccessibleHeaderText="OutboundTrackName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblOutboundTrackName" runat="server" text='<%# Eval ( "Outbound.LatestOutboundTrack.Type.Name" ) %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "Outbound.Status" ) %>'
                                                     Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>
                            
                                        <asp:TemplateField AccessibleHeaderText="ReferenceNumber" HeaderText="Nº Ref." ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <asp:label ID="lblReferenceNumber" runat="server" text='<%# Eval ( "Outbound.ReferenceNumber" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                         <asp:TemplateField AccessibleHeaderText="Priority" HeaderText="Prioridad">
                                            <ItemTemplate>
                                                <asp:label ID="lblPriority" runat="server" text='<%# Eval ( "Outbound.Priority" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                           
                                        <asp:templatefield headertext="Liberación Autom." accessibleHeaderText="InmediateProcess">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkInmediateProcess" runat="server" checked='<%# Eval ( "Outbound.InmediateProcess" ) %>' Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>
                            
                                        <%--Inicio Aqui van todas las columnas calculadas en sql--%>
                                        <asp:TemplateField HeaderText="Solicitado" accessibleHeaderText="QtySolicitado" SortExpression="QtySolicitado">
                                          <ItemTemplate>
                                            <asp:Label ID="lblQtySolicitado" runat="server" Text='<%#GetFormatedNumber( Eval("QtySolicitado")) %>'></asp:Label>
                                          </ItemTemplate>
                                          <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Solicitado" accessibleHeaderText="QtySolicitado" DataField="QtySolicitado" />--%>
                            
                                        <asp:TemplateField HeaderText="Liberado" AccessibleHeaderText="QtyRelease" SortExpression="QtyRelease">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyRelease" runat="server" Text='<%#GetFormatedNumber( Eval("QtyRelease")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Liberado %" AccessibleHeaderText="PctRelease" SortExpression="PctRelease">
	                                        <ItemTemplate>
		                                        <center>
			                                        <div style="word-wrap: break-word;">
				                                        <asp:Label ID="lblPctRelease" runat="server" Text='<%# Eval ("PctRelease") %>' />
			                                        </div>
		                                        </center>
	                                        </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Picking Uni" AccessibleHeaderText="QtyPicking" SortExpression="QtyPicking">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyPicking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPicking")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Picking Uni" accessibleHeaderText="QtyPicking" DataField="QtyPicking" />--%>
                                        <asp:BoundField HeaderText="Picking %" accessibleHeaderText="PctPicking" DataField="PctPicking" />
                            
                                        <asp:TemplateField HeaderText="Packing Uni" AccessibleHeaderText="QtyPacking" SortExpression="QtyPacking">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyPacking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPacking")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Packing Uni" accessibleHeaderText="QtyPacking" DataField="QtyPacking" />--%>
                                        <asp:BoundField HeaderText="Packing %" accessibleHeaderText="PctPacking" DataField="PctPacking" />
                            
                                        <asp:TemplateField HeaderText="Loading Uni" AccessibleHeaderText="QtyLoading" SortExpression="QtyLoading">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyLoading" runat="server" Text='<%#GetFormatedNumber( Eval("QtyLoading")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Loading Uni" accessibleHeaderText="QtyLoading" DataField="QtyLoading" />--%>
                                        <asp:BoundField HeaderText="Loading %" accessibleHeaderText="PctLoading" DataField="PctLoading" />
                            
                                        <%--<asp:TemplateField HeaderText="Shipping Uni" AccessibleHeaderText="QtyShipping" SortExpression="QtyShipping">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyShipping" runat="server" Text='<%#GetFormatedNumber( Eval("QtyShipping")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>--%>

                                        <asp:TemplateField HeaderText="Routing Uni" AccessibleHeaderText="QtyRouting" SortExpression="QtyRouting">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyRouting" runat="server" Text='<%#GetFormatedNumber( Eval("QtyRouting")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:BoundField HeaderText="Routing %" accessibleHeaderText="PctRouting" DataField="PctRouting" />
                            
                                        <asp:TemplateField HeaderText="Shipping Uni" AccessibleHeaderText="QtyShipping" SortExpression="QtyShipping">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyShipping" runat="server" Text='<%#GetFormatedNumber( Eval("QtyShipping")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <%--<asp:BoundField HeaderText="Shipping Uni" accessibleHeaderText="QtyShipping" DataField="QtyShipping" />--%>
                                        <asp:BoundField HeaderText="Shipping %" accessibleHeaderText="PctShipping" DataField="PctShipping" />
                                         <%--FIN Aqui van todas las columnas calculadas en sql--%>
                           
                                        <asp:templatefield headertext="Emisión" accessibleHeaderText="EmissionDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblEmissionDate" runat="server" text='<%# ((DateTime) Eval ("Outbound.EmissionDate") > DateTime.MinValue)? Eval("Outbound.EmissionDate"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>  
                                                        
                                        <asp:templatefield headertext="Esperada" accessibleHeaderText="ExpectedDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblExpectedDate" runat="server" text='<%# ((DateTime) Eval ("Outbound.ExpectedDate") > DateTime.MinValue)? Eval("Outbound.ExpectedDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>   
                                                        
                                        <asp:templatefield headertext="Salida" accessibleHeaderText="ShipmentDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblShipmentDate" runat="server" text='<%# ((DateTime) Eval ("Outbound.ShipmentDate") > DateTime.MinValue)? Eval("Outbound.ShipmentDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>   
                                                        
                                        <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("Outbound.ExpirationDate") > DateTime.MinValue)? Eval("Outbound.ExpirationDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>           
                                                    
                                        <asp:TemplateField HeaderText="Usuario Cancelac." AccessibleHeaderText="CancelUser" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblCancelUser" runat="server" text='<%# Eval ( "Outbound.CancelUser" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                            
                                        <asp:templatefield headertext="Cancelación" accessibleHeaderText="CancelDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblCancelDate" runat="server" text='<%# ((DateTime) Eval ("Outbound.CancelDate") > DateTime.MinValue)? Eval("Outbound.CancelDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>  
                                            
                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerCode" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblCustomerCode" runat="server" text='<%# Eval ( "Outbound.CustomerCode" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>   
                                         
                                        <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblCustomerName" runat="server" text='<%# Eval ( "Outbound.CustomerName" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField> 
                                        
                                        <asp:TemplateField HeaderText="Sucursal" AccessibleHeaderText="Branch" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblbranchName" runat="server" text='<%# Eval ( "Outbound.Branch.Name" ) %>'/>
                                                </div>  
                                            </itemtemplate>
                                        </asp:TemplateField>  
                           
                                        <asp:TemplateField HeaderText="Dirección Entrega" AccessibleHeaderText="DeliveryAddress1">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblDeliveryAddress1" runat="server" text='<%# Eval ( "Outbound.DeliveryAddress1" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>                    
                                        <asp:TemplateField HeaderText="Dirección Entrega Opc." AccessibleHeaderText="DeliveryAddress2">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblDeliveryAddress2" runat="server" text='<%# Eval ( "Outbound.DeliveryAddress2" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryDelivery" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblCountryDeliveryName" runat="server" text='<%# Eval ( "Outbound.CountryDelivery.Name" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateDelivery" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblStateDeliveryName" runat="server" text='<%# Eval ( "Outbound.StateDelivery.Name" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>         
                            
                                        <asp:TemplateField HeaderText="Ciudad Entrega" AccessibleHeaderText="CityDelivery" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblCityDeliveryName" runat="server" text='<%# Eval ( "Outbound.CityDelivery.Name" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>                        

                                        <asp:TemplateField HeaderText="Tel. Entrega" AccessibleHeaderText="DeliveryPhone" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblDeliveryPhone" runat="server" text='<%# Eval ( "Outbound.DeliveryPhone" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>  
                                            
                                        <asp:TemplateField HeaderText="E-mail Entrega" AccessibleHeaderText="DeliveryEmail" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblDeliveryEmail" runat="server" text='<%# Eval ( "Outbound.DeliveryEmail" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>  
                            
                                        <asp:TemplateField HeaderText="Preparar Completo" AccessibleHeaderText="FullShipment" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkFullShipment" runat="server" checked='<%# Eval ( "Outbound.FullShipment" ) %>' Enabled="false" />
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Transportista" AccessibleHeaderText="CarrierCode" ItemStyle-Wrap="false">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblCarrierCode" runat="server" text='<%# Eval ( "Outbound.Carrier.Code" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>  
                           
                                        <asp:TemplateField HeaderText="Ruta" AccessibleHeaderText="RouteCode" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblRouteCode" runat="server" text='<%# Eval ( "Outbound.RouteCode" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField> 

                                        <asp:TemplateField HeaderText="Fecha Despacho" AccessibleHeaderText="ShippedDate" ItemStyle-Wrap="false">
                                            <ItemTemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblShippedDate" runat="server" text='<%# (((DateTime)Eval ( "ShippedDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "ShippedDate", "{0:d}" )) %>'></asp:Label>
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Líneas" AccessibleHeaderText="SpecialField1" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblSpecialField1" runat="server" text='<%# Eval ( "Outbound.SpecialField1" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cantidad Total" AccessibleHeaderText="SpecialField2" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblSpecialField2" runat="server" text='<%# Eval ( "Outbound.SpecialField2" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Confirmación Packing" AccessibleHeaderText="SpecialField3" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblSpecialField3" runat="server" text='<%# Eval ( "Outbound.SpecialField3" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Campo Especial 4" AccessibleHeaderText="SpecialField4" ItemStyle-Wrap="false" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                     <asp:Label ID="lblSpecialField4" runat="server" text='<%# Eval ( "Outbound.SpecialField4" ) %>'></asp:Label>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>

                                        <asp:templatefield headertext="Picking" AccessibleHeaderText="IsOrderPicked" SortExpression="IsOrderPicked">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkIsOrderPicked" runat="server" checked='<%# Eval ( "IsOrderPicked" ) %>' Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>

                                        <asp:templatefield headertext="Packing" AccessibleHeaderText="IsOrderPacked" SortExpression="IsOrderPacked">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkIsOrderPacked" runat="server" checked='<%# Eval ( "IsOrderPacked" ) %>' Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>

                                        <asp:templatefield headertext="Ruteo" AccessibleHeaderText="IsOrderRouted" SortExpression="IsOrderRouted">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkIsOrderRouted" runat="server" checked='<%# Eval ( "IsOrderRouted" ) %>' Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>

                                        <asp:templatefield headertext="Carga" AccessibleHeaderText="IsOrderLoaded" SortExpression="IsOrderLoaded">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkIsOrderLoaded" runat="server" checked='<%# Eval ( "IsOrderLoaded" ) %>' Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>

                                        <asp:templatefield headertext="Tiene Pick en Pausa" AccessibleHeaderText="HasPausedPicking" SortExpression="HasPausedPicking">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkHasPausedPicking" runat="server" checked='<%# Eval ( "Outbound.HasPausedPicking" ) %>' Enabled="false"/>
                                                </center>    
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
                            <%-- FIN Panel Grilla Principal --%>
                        </div>
                    </div>
                </div>
            </Content>
        </TopPanel>
        <BottomPanel HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Detalle --%>
                            <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>  
                                    <%-- Grilla Detalle --%>
                                    <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                      <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
	                                    <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
	                                    <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                      </div>
                                      <asp:GridView ID="grdDetail" runat="server" DataKeyNames="Id" EnableViewState="false" OnRowCreated="grdDetail_RowCreated" SkinID="grdDetail"
                                            AutoGenerateColumns="false"
                                            OnRowDataBound="grdDetail_RowDataBound"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">
                                    <Columns>
                        
                                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id"> </asp:BoundField>
                                          
                                       <asp:TemplateField HeaderText="Secuencia" accessibleHeaderText="LineNumber" SortExpression="LineNumber">
                                          <ItemTemplate>
                                                <asp:Label ID="lblIdLineNumber" runat="server" Text='<%# Eval("OutboundDetail.LineNumber") %>'></asp:Label>
                                          </ItemTemplate>
                                           <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                       <asp:TemplateField HeaderText="Nº Línea" accessibleHeaderText="LineCode" SortExpression="LineCode">
                                          <ItemTemplate>
                                                <asp:Label ID="lblIdLineCode" runat="server" Text='<%# Eval("OutboundDetail.LineCode") %>'></asp:Label>
                                          </ItemTemplate>
                                           <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                          <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("OutboundDetail.Item.Code") %>'></asp:Label>
                                          </ItemTemplate>
                                          <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("OutboundDetail.Item.LongName") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>                               
                            
                                        <asp:TemplateField HeaderText="Descripción"  accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                          <ItemTemplate>
                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("OutboundDetail.Item.Description") %>'></asp:Label>
                                          </ItemTemplate>
                                          <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                            
                                            
                                        <asp:TemplateField HeaderText="Categoría" accessibleHeaderText="CategoryItem" SortExpression="CategoryItem">
                                          <ItemTemplate>
                                            <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval("OutboundDetail.CategoryItem.Name") %>'></asp:Label>
                                          </ItemTemplate>
                                          <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Cant. Solicitada" accessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                          <ItemTemplate>
                                            <asp:Label ID="lblItemQty" runat="server" Text='<%#GetFormatedNumber( Eval("OutboundDetail.ItemQty")) %>'></asp:Label>
                                          </ItemTemplate>
                                          <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                            
                                        <%--Inicio Aqui van todas las columnas calculadas en sql --%>
                                        <asp:TemplateField HeaderText="Liberado" AccessibleHeaderText="QtyRelease" SortExpression="QtyRelease">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyRelease" runat="server" Text='<%#GetFormatedNumber(Eval("QtyRelease")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false" />
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Liberado %" AccessibleHeaderText="PctRelease" SortExpression="PctRelease">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPctRelease" runat="server" Text='<%#GetFormatedNumber(Eval("PctRelease")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false" />
                                        </asp:TemplateField>
                                       

                                        <asp:TemplateField HeaderText="Picking Uni" AccessibleHeaderText="QtyPicking" SortExpression="QtyPicking">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyPicking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPicking")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Picking Uni" accessibleHeaderText="QtyPicking" DataField="QtyPicking" />--%>
                                        <asp:BoundField HeaderText="Picking %" AccessibleHeaderText="PctPicking" DataField="PctPicking" />
                            
                                        <asp:TemplateField HeaderText="Packing Uni" AccessibleHeaderText="QtyPacking" SortExpression="QtyPacking">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyPacking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPacking")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Packing Uni" accessibleHeaderText="QtyPacking" DataField="QtyPacking" />--%>
                                        <asp:BoundField HeaderText="Packing %" AccessibleHeaderText="PctPacking" DataField="PctPacking" />

                                        <asp:TemplateField HeaderText="Routing Uni" AccessibleHeaderText="QtyRouting" SortExpression="QtyRouting">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyRouting" runat="server" Text='<%#GetFormatedNumber( Eval("QtyRouting")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>

                                        <asp:BoundField HeaderText="Routing %" AccessibleHeaderText="PctRouting" DataField="PctRouting" />
                            
                            
                                        <asp:TemplateField HeaderText="Loading Uni" AccessibleHeaderText="QtyLoading" SortExpression="QtyLoading">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyLoading" runat="server" Text='<%#GetFormatedNumber( Eval("QtyLoading")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField HeaderText="Loading Uni" accessibleHeaderText="QtyLoading" DataField="QtyLoading" />--%>
                                        <asp:BoundField HeaderText="Loading %" AccessibleHeaderText="PctLoading" DataField="PctLoading" />
                            
                                        <asp:TemplateField HeaderText="Shipping Uni" AccessibleHeaderText="QtyShipping" SortExpression="QtyShipping">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQtyShipping" runat="server" Text='<%#GetFormatedNumber( Eval("QtyShipping")) %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="false"/>
                                        </asp:TemplateField>
                                        <%-- <asp:BoundField HeaderText="Shipping Uni" accessibleHeaderText="QtyShipping" DataField="QtyShipping" />--%>
                                        <asp:BoundField HeaderText="Shipping %" AccessibleHeaderText="PctShipping" DataField="PctShipping" />
                                         <%--FIN Aqui van todas las columnas calculadas en sql--%>
                           
                                        <asp:templatefield headertext="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkCodStatus" runat="server" checked='<%# Eval ( "OutboundDetail.Status" ) %>'
                                                     Enabled="false"/>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>

                                       <asp:templatefield headertext="Lote" AccessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "OutboundDetail.LotNumber" ) %>'></asp:Label>
                                            </itemtemplate>
                                        </asp:templatefield>
                            
                                        <asp:templatefield headertext="Fifo" AccessibleHeaderText="FifoDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblFifoDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.FifoDate") > DateTime.MinValue)? Eval("OutboundDetail.FifoDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield> 
                            
                                        <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblExpirationDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.ExpirationDate") > DateTime.MinValue)? Eval("OutboundDetail.ExpirationDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield> 
                                                      
                                        <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("OutboundDetail.FabricationDate") > DateTime.MinValue)? Eval("OutboundDetail.FabricationDate", "{0:d}"):"" %>' />
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield> 

                                        <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="ItemUomName" SortExpression="ItemUomName">
	                                        <ItemTemplate>
		                                        <center>
			                                        <div style="word-wrap: break-word;">
				                                        <asp:Label ID="lblItemUomName" runat="server" Text='<%# Eval ("OutboundDetail.Item.ItemUom.Name") %>' />
			                                        </div>
		                                        </center>
	                                        </ItemTemplate>
                                        </asp:TemplateField>

                                     </Columns>
                                    </asp:GridView>
                                    </div>           
                                    <%-- FIN Grilla Detalle --%>
                    
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
                            <%-- FIN Panel Grilla Detalle --%>
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

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Emisión" Visible="false" />   
    <asp:Label id="lblFilterReferenceNumber" runat="server" Text="Ord. Compra" Visible="false" /> 
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>
