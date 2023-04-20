<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockLocationConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.StockLocationWebConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

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

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
	                         <%-- Grilla Principal --%>         
                             <asp:GridView ID="grdMgr" 
                                runat="server" 
                                AllowPaging="True" 
                                AllowSorting="False" 
                                OnRowCreated="grdMgr_RowCreated"
                                EnableViewState="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                    
                                 <Columns>
                                    <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                           <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>

                                     <asp:templatefield HeaderText="Bodega" AccessibleHeaderText="HngCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                           <asp:label ID="lblHngCode" runat="server" text='<%# Eval ( "Location.Hangar.Name" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>
                                         
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <itemtemplate>
                                           <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>

                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%--<asp:templatefield headertext="Zona Trabajo" accessibleHeaderText="WorkZone" SortExpression="WorkZoneName">
                                        <itemtemplate>
                                               <asp:label ID="lblWorkZoneName" runat="server" text='<%# Eval ("WorkZone.Name") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>--%>

                                    <asp:templatefield headertext="Tipo Ubic." accessibleHeaderText="LocationType" SortExpression="LocationType">
                                        <itemtemplate>
                                               <asp:label ID="lblLocationType" runat="server" text='<%# Eval ("Location.Type.LocTypeCode") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
                                            
                                    <asp:templatefield headertext="Ubicación" accessibleHeaderText="LocationCode" SortExpression="LocationCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblLocationCode" runat="server" text='<%# Eval ("Location.IdCode") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:templatefield headertext="Categoría" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                        <itemtemplate>
                                               <asp:label ID="lblCategoryItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 
                        
                                    <asp:templatefield headertext="Nº Doc. Entrada" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber">
                                        <itemtemplate>
                                               <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ("InboundOrder.Number") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>   
                        
                                    <asp:templatefield headertext="Nº Doc. Salida" accessibleHeaderText="OutboundNumber" SortExpression="OutboundNumber">
                                        <itemtemplate>
                                               <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ("OutboundOrder.Number") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>   
                         
                                    <asp:TemplateField HeaderText="Tipo Doc.Salida" AccessibleHeaderText="OutboundTypeName" SortExpression="OutboundTypeName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ("OutboundOrder.OutboundType.Name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                             
                                    <asp:templatefield headertext="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                             

                                    <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                        <itemtemplate>
                                               <asp:label ID="lblItemName" runat="server" text='<%# Eval ("Item.Description") %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 
                        
                                    <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber" SortExpression="LotNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ("Lot") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>        
                        
                                     <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo" SortExpression="Fifo">
                                        <itemtemplate>
                                               <asp:label ID="lblFifo" runat="server" text='<%# ((DateTime) Eval("FifoDate") == DateTime.MinValue) ? " " : Eval("FifoDate", "{0:d}") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>                                            
                        
                                    <asp:TemplateField HeaderText="Elaboración" AccessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval("FabricationDate") == DateTime.MinValue) ? " " : Eval("FabricationDate", "{0:d}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval("ExpirationDate") == DateTime.MinValue) ? " " : Eval("ExpirationDate", "{0:d}") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                                               
                                   <asp:templatefield headertext="Cant." accessibleHeaderText="Qty" SortExpression="Qty">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQty" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                     <asp:templatefield headertext="Peso" accessibleHeaderText="TotalWeight" SortExpression="TotalWeight">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblTotalWeight" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("TotalWeight") == -1)?" ":Eval ("TotalWeight")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                        
                                    <asp:TemplateField HeaderText="Bloqueo Ubic" AccessibleHeaderText="HoldLocation" SortExpression="HoldLocation" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHoldLocation" runat="server" Text='<%# Eval ("Location.Reason.Name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:templatefield headertext="Bloqueo Stock" accessibleHeaderText="Hold" SortExpression="Hold" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblHold" runat="server" text='<%# Eval ("Hold") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>    
                        
                                    <asp:templatefield headertext="LPN" accessibleHeaderText="LPN" SortExpression="LPN" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblLPN" runat="server" text='<%# Eval ("Lpn.IdCode") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>     
                                     
                                    <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="DefaultUomName">
	                                    <ItemTemplate>
		                                    <asp:Label ID="lblUomName" runat="server" Text='<%# Eval ("Item.SpecialField4") %>'></asp:Label>
	                                    </ItemTemplate>
                                    </asp:TemplateField> 

                                 </Columns>
                         
                              </asp:GridView>
                            <%-- FIN Grilla Principal --%>             
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
                  </Triggers>
                </asp:UpdatePanel>  
            </div>
        </div>
    </div>

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>                   

    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Fifo" Visible="false" />   
    <asp:Label id="LabelmessageFind" runat="server" Text="Debe ingresar por lo menos un criterio de busqueda" Visible="false" />   
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>