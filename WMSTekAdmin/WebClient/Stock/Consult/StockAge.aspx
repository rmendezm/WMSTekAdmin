<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="StockAge.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Stock.Consult.StockAge" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUcLook" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        window.onresize = SetDivs;
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(SetDivs);
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>          
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
        
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

                                    <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Location.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>   

                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" SortExpression="IdLpnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLpn" runat="server" Text='<%# Eval("Lpn.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LpnType" SortExpression="LpnType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTypeLpn" runat="server" Text='<%# Eval("Lpn.LPNType.Code") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="IdLpnCodeContainer" HeaderText="LPN Contenedor" AccessibleHeaderText="IdLpnCodeContainer" />
                                    <asp:BoundField DataField="LpnTypeCodeContainer" HeaderText="Tipo LPN Contenedor" AccessibleHeaderText="LpnTypeCodeContainer" />

                                     <asp:templatefield headertext="Cód. Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
            
                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ShortName" SortExpression="ShortName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.ShortName") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                             

                                    <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                        <itemtemplate>
                                               <asp:label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.Description") %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 

                                    <asp:templatefield headertext="Cant." accessibleHeaderText="Qty" SortExpression="Qty">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                     <asp:templatefield headertext="Fecha de Movimiento a la Ubicación" accessibleHeaderText="DateCreated" SortExpression="DateCreated">
                                        <itemtemplate>
                                            <asp:label ID="lblDateCreated" runat="server" text='<%# ((DateTime) Eval ("DateCreated") > DateTime.MinValue) ? Eval("DateCreated", "{0:d}"): "" %>' />
                                        </itemtemplate>
                                    </asp:templatefield>  

                                    <asp:templatefield headertext="Días de Permanencia" accessibleHeaderText="DaysLeft" SortExpression="DaysLeft">
                                        <itemtemplate>
                                            <asp:label ID="lblDateCreated" runat="server" text='<%# GetDaysLeft((DateTime)Eval("DateCreated")) %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 

                                    <asp:templatefield headertext="Lote" accessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                        </itemtemplate>
                                    </asp:templatefield>   
                                     
                                    <asp:templatefield headertext="Categoría" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                        <itemtemplate>
                                            <asp:label ID="lblCategoryItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>      
                                     
                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo" SortExpression="Fifo">
                                        <itemtemplate>
                                            <asp:label ID="lblFifo" runat="server"  text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                        </itemtemplate>
                                    </asp:templatefield>  
                                     
                                    <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <asp:label ID="lblFabricationDate" runat="server" text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                        </itemtemplate>
                                    </asp:templatefield>    
                    
                                      <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <itemtemplate>
                                            <asp:label ID="lblExpiration" runat="server"  text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 

                                    <asp:templatefield headertext="Nº Doc. Entrada" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber">
                                        <itemtemplate>
                                               <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ("InboundOrder.Number") %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 

                                    <asp:TemplateField HeaderText="Tipo Doc. Entrada" AccessibleHeaderText="InboundTypeName" SortExpression="InboundTypeName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInboundTypeName" runat="server" Text='<%# Eval ("InboundOrder.InboundType.Name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
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

    <asp:Label ID="lblLpnFilter" runat="server" Text="LPN" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Fecha de Movimiento" Visible="false" />
    <asp:Label ID="lblLocationFilter" runat="server" Text="Ubicación" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>
