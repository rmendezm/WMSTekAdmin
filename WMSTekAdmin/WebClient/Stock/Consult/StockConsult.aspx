<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.StockWebConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

</script>

    <div class="container">
        <div class="row">
            <div>
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
                                AutoGenerateColumns="False"
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
                        
                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo" SortExpression="Fifo">
                                        <itemtemplate>
                                               <asp:label ID="lblFifo" runat="server" 
                                                   text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                        </itemtemplate>
                                    </asp:templatefield>                                                       
                                                                
                                    <asp:templatefield headertext="Cód. Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
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
                                                               
                                   <asp:templatefield headertext="Cant." accessibleHeaderText="Qty" SortExpression="Qty">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQty" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield headertext="Volumen" accessibleHeaderText="TotalVolumen" SortExpression="TotalVolumen">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblTotalVolumen" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("TotalVolumen") == -1)?" ":Eval ("TotalVolumen")) %>' />
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
                        
                                    <asp:TemplateField HeaderText="Bloqueo Stock" AccessibleHeaderText="HoldCode" SortExpression="HoldCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblHoldCode" runat="server" Text='<%# Eval("Hold") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                         
                         
                                     <asp:templatefield headertext="Lote" accessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                        </itemtemplate>
                                        <ItemStyle CssClass="text" />
                                    </asp:templatefield>            
                    
                     
                                    <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <asp:label ID="lblFabricationDate" runat="server"  
                                                text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                            </center>    
                                        </itemtemplate>
                                    </asp:templatefield>    
                    
                                      <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <itemtemplate>
                                               <asp:label ID="lblExpiration" runat="server"  text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 
                    
                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" SortExpression="IdLpnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLpn" runat="server" Text='<%# Eval("Lpn.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                    
                                    <asp:BoundField DataField="IdLpnCodeContainer" HeaderText="LPN Contenedor" AccessibleHeaderText="IdLpnCodeContainer" />
                                    <asp:BoundField DataField="LpnTypeCodeContainer" HeaderText="Tipo LPN Contenedor" AccessibleHeaderText="LpnTypeCodeContainer" />
                                        
                                    <asp:TemplateField HeaderText="Ubicacion" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Location.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                        

                                    <asp:TemplateField HeaderText="Presentación por Defecto" AccessibleHeaderText="DefaultUomName">
	                                        <ItemTemplate>
		                                        <asp:Label ID="lblUomName" runat="server" Text='<%# Eval ("Item.SpecialField4") %>'></asp:Label>
	                                    </ItemTemplate>
                                    </asp:TemplateField> 

                                    <asp:templatefield headertext="Nro Sello" accessibleHeaderText="SealNumber" SortExpression="SealNumber" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblSealNumber" runat="server" text='<%# Eval ("Lpn.SealNumber") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>

                                      <asp:templatefield headertext="Envasado" accessibleHeaderText="GrpClass7" SortExpression="GrpClass7" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblGrpClass7" runat="server" text='<%#  string.IsNullOrEmpty((string)Eval ("GrpClass7")) ? "" : (Convert.ToDateTime(Eval("GrpClass7"))).ToShortDateString() %>' />
                                        </itemtemplate>
                                    </asp:templatefield>

                                     <asp:templatefield headertext="Nro Parte" accessibleHeaderText="GrpClass8" SortExpression="GrpClass8" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblGrpClass8" runat="server" text='<%# Eval ("GrpClass8") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>

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