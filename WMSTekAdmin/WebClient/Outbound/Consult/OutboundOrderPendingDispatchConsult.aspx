<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"  
CodeBehind="OutboundOrderPendingDispatchConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.OutboundOrderPendingDispatchConsult" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
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
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('GetOutboundOrderPendingDispatch', 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr');
    }
    
</script> 

    <div id="divPrincipal" style="margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter   CookieDays="0"  ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel"  >
            <Header Height="1">
				<div style="width:100%;height:100%;" >			
				</div>
			</Header>
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
                            <asp:UpdatePanel ID="upGrid"  runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate >  
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
                                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id" SortExpression="Id">
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        </asp:BoundField>
                                        <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" >
                                            <itemtemplate>
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
                                        <asp:templatefield HeaderText="Cód. CD. Destino" AccessibleHeaderText="WarehouseTargetCode"  >
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
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" >
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Nº Doc." DataField="Number" AccessibleHeaderText="OutboundNumber" />
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
                                        <asp:BoundField DataField="ReferenceNumber" HeaderText="Nº Ref." AccessibleHeaderText="ReferenceNumber" >
                                        </asp:BoundField>
                                        <asp:BoundField DataField="LoadCode" HeaderText="Cód. Carga"   AccessibleHeaderText="LoadCode" />
                                        <asp:BoundField DataField="LoadSeq" HeaderText="Sec. Carga" AccessibleHeaderText="LoadSeq" />
                                        <asp:BoundField DataField="Priority" HeaderText="Prioridad" AccessibleHeaderText="Priority" />
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
                                        <asp:BoundField DataField="DeliveryAddress1" HeaderText="Dirección Entrega" AccessibleHeaderText="DeliveryAddress1"/>
                                        <asp:BoundField DataField="DeliveryAddress2" HeaderText="Dirección Entrega Opc." AccessibleHeaderText="DeliveryAddress2"/>
                                        <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryDelivery" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCountryDeliveryName" runat="server" text='<%# Eval ( "CountryDelivery.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Región Entrega" AccessibleHeaderText="StateDelivery" >
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
                                        <asp:BoundField DataField="Invoice" HeaderText="Nº Factura" AccessibleHeaderText="Invoice" />
                                        <asp:BoundField DataField="FactAddress1" HeaderText="Dirección Factura" AccessibleHeaderText="FactAddress1"/>
                                        <asp:BoundField DataField="FactAddress2" AccessibleHeaderText="FactAddress2" HeaderText="Dirección Factura Opc." />
                                        <asp:TemplateField HeaderText="País Factura" AccessibleHeaderText="CountryFact" >
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:Label ID="lblCountryFactName" runat="server" text='<%# Eval ( "CountryFact.Name" ) %>'></asp:Label>
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Región Factura" AccessibleHeaderText="StateFact" >
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
                            <div id="divSelected"  runat="server">
                                <%-- Panel Grilla Detalle --%>
                                <asp:UpdatePanel ID="upGridDetail" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>      
                                        <div id="divDetail" runat="server" visible="false" class="divGridDetailScroll">
                                          <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
	                                        <asp:Label ID="lblGridDetail" runat="server" Text="Detalle Doc: " />
	                                        <asp:Label ID="lblNroDoc" runat="server" Text=""/>
                                          </div>
                                            <asp:GridView ID="grdDetail" runat="server" SkinID="grdDetail"
                                                DataKeyNames="Id" 
                                                EnableViewState="False" 
                                                AutoGenerateColumns="False"
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound" 
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false" >
                           
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
                                        
                                                    <%--Inicio Aqui van todas las columnas calculadas en sql--%>
                                                    <asp:TemplateField HeaderText="Cant. Pickeadas" AccessibleHeaderText="QtyPicking" SortExpression="QtyPicking">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyPicking" runat="server" Text='<%#GetFormatedNumber( Eval("QtyPicking")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>
                                        
                                                    <asp:TemplateField HeaderText="Cant. Pendiente" AccessibleHeaderText="QtyPending" SortExpression="QtyPending">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQtyPending" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false"/>
                                                    </asp:TemplateField>
                                        
                                                    <asp:TemplateField AccessibleHeaderText="SpecialField1" HeaderText="Campo. Esp. 1">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField1" ) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="SpecialField2" HeaderText="Campo. Esp. 2">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField2" ) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="SpecialField3" HeaderText="Campo. Esp. 3">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField3" ) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField AccessibleHeaderText="SpecialField4" HeaderText="Campo. Esp. 4">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Eval ( "OutboundDetail.SpecialField4" ) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="True" AccessibleHeaderText="Id"/>
                                                    <asp:BoundField DataField="LineNumber" HeaderText="Nº Línea" AccessibleHeaderText="LineNumber" 
                                                        ItemStyle-HorizontalAlign="Center" >
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="LineCode" HeaderText="Cód. Línea"  AccessibleHeaderText="LineCode"  ItemStyle-CssClass="text"
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
                                                    <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Item">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItem2" runat="server" text='<%# Eval ( "Item.Description" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgItem">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblCtgItem2" runat="server" text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblItemQty" runat="server" text='<%# GetFormatedNumber(Eval ( "ItemQty" ) )%>'></asp:Label>
                                                             </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--<asp:BoundField DataField="ItemQty" HeaderText="Cantidad" AccessibleHeaderText="ItemQty" ItemStyle-HorizontalAlign="Right"/>
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
                                                    <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate">
                                                        <ItemStyle Wrap="false" />
                                                        <itemtemplate>
                                                            <center>
                                                                <div style="word-wrap: break-word;">
                                                                    <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                                                </div>
                                                            </center>    
                                                    </itemtemplate>
                                                    </asp:templatefield>                  --%>                                                       
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
                                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />
                                  </Triggers>
                                </asp:UpdatePanel>  
                                <%-- FIN Panel Grilla Detalle --%>
                    
                                <asp:UpdateProgress ID="uprSelectedOrdersDetail" runat="server" AssociatedUpdatePanelID="upGridDetail" DisplayAfter="10" DynamicLayout="true">
                                <ProgressTemplate>
                                    <div class="divProgress">
                                        <asp:ImageButton ID="imgProgress1" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
                                    </div>
                                </ProgressTemplate>
                                </asp:UpdateProgress>
                                <webUc:UpdateProgressOverlayExtender ID="muprSelectedOrdersDetail" runat="server" ControlToOverlayID="divSelected" 
                                CssClass="updateProgress" TargetControlID="uprSelectedOrdersDetail" />
                
                            </div>
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
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>