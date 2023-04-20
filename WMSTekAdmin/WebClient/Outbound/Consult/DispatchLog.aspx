<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchLog.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Outbound.Consult.DispatchLog" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
	            <%-- Grilla Principal --%>         
                <asp:GridView ID="grdMgr" 
                        runat="server"
                        AllowPaging="True" 
                        AllowSorting="False" 
                        OnRowCreated="grdMgr_RowCreated"
                        EnableViewState="false" >
                        
                         <Columns>
                             <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" SortExpression="Id"/>
                             
                             <asp:TemplateField HeaderText="Tipo Mov." AccessibleHeaderText="MovementType" SortExpression="MovementType">
                                 <itemtemplate>
                                    <center>
                                     <asp:Label ID="lblMovementTypeId" runat="server" Text='<%# Eval ( "MovementType.Id" ) %>' ></asp:Label>
                                     </center>
                                 </itemtemplate>
                             </asp:TemplateField>

                             <asp:TemplateField HeaderText="Desc. Tipo Mov." AccessibleHeaderText="MovementName" SortExpression="MovementName">
                                 <itemtemplate>
                                     <asp:Label ID="lblMovementType" runat="server" Text='<%# Eval ( "MovementType.Name" ) %>' ></asp:Label>
                                 </itemtemplate>
                             </asp:TemplateField>
                             
                             <asp:BoundField DataField="StartTime" HeaderText="Inicio" AccessibleHeaderText="StartTime" SortExpression="StartTime" ItemStyle-HorizontalAlign="Center"/>
                             <asp:BoundField DataField="EndTime" HeaderText="Fin" AccessibleHeaderText="EndTime" SortExpression="EndTime" ItemStyle-HorizontalAlign="Center"/>                     
                             <asp:BoundField DataField="UserName" HeaderText="Usuario" AccessibleHeaderText="UserName" SortExpression="UserName" ItemStyle-HorizontalAlign="Center"/>                     
                             
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
                                                  
                             <asp:BoundField DataField="DocumentType" HeaderText="Tipo Doc." AccessibleHeaderText="DocumentType" SortExpression="DocumentType" ItemStyle-HorizontalAlign="Center"/>
                             <asp:BoundField DataField="DocumentNumber" HeaderText="Nº Doc." AccessibleHeaderText="DocumentNumber" SortExpression="DocumentNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="DocumentLineNumber" HeaderText="Nº Línea" AccessibleHeaderText="DocumentLineNumber" SortExpression="DocumentLineNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="ReferenceNumber" HeaderText="Nº Ref." AccessibleHeaderText="ReferenceNumber" SortExpression="DocumentLineNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text"/>                     
                                                  
                            <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                <ItemTemplate>
                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                <ItemTemplate>
                                    <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.TradeName" ) %>'></asp:Label>
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
                                    <asp:label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.Description") %>' />
                                </itemtemplate>
                            </asp:templatefield>

                            <asp:templatefield headertext="Categoría Item" accessibleHeaderText="CategoryItem" SortExpression="CategoryItemName">
                                <itemtemplate>
                                       <asp:label ID="lblItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' />
                                </itemtemplate>
                            </asp:templatefield>   
                                                  
                             <asp:BoundField DataField="LotNumber" HeaderText="Nº Lote" AccessibleHeaderText="LotNumber" SortExpression="LotNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text"/>
                             
                            <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                <ItemStyle Wrap="false" />
                                <itemtemplate>
                                    <center>
                                        <asp:label ID="lblFifoDate" runat="server"  
                                        text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                    </center>    
                            </itemtemplate>
                            </asp:templatefield>    
                            
                            <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                <ItemStyle Wrap="false" />
                                <itemtemplate>
                                    <center>
                                        <asp:label ID="lblExpirationDate" runat="server"  
                                        text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                    </center>    
                            </itemtemplate>
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
                            
                             <asp:BoundField DataField="IdLpnCodeSource" HeaderText="LPN Origen" AccessibleHeaderText="IdLpnCodeSource" SortExpression="IdLpnCodeSource"  ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="IdLpnCodeTarget" HeaderText="LPN Destino" AccessibleHeaderText="IdLpnCodeTarget" SortExpression="IdLpnCodeTarget"  ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="IdLocCodeProposal" HeaderText="Propuesta" AccessibleHeaderText="IdLocCodeProposal" SortExpression="IdLocCodeProposal"  ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="IdLocCodeSource" HeaderText="Origen" AccessibleHeaderText="IdLocCodeSource" SortExpression="IdLocCodeSource"  ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="IdLocCodeTarget" HeaderText="Destino" AccessibleHeaderText="IdLocCodeTarget" SortExpression="IdLocCodeTarget"  ItemStyle-CssClass="text"/>

                            <asp:templatefield headertext="Cant. Mov." accessibleHeaderText="ItemQtyMov" SortExpression="ItemQtyMov">
                                <ItemStyle Wrap="false" />
                                <itemtemplate>
                                    <center>
                                        <asp:label ID="lblItemQtyMov" runat="server" 
                                        text='<%# ((decimal) Eval ("ItemQtyMov") == -1)?" ":Eval ("ItemQtyMov") %>' />
                                    </center>    
                            </itemtemplate>
                            </asp:templatefield>

                            <asp:templatefield headertext="Cant. Ant. Origen" accessibleHeaderText="QtyBeforeSource" SortExpression="QtyBeforeSource">
                                <ItemStyle Wrap="false" />
                                <itemtemplate>
                                    <center>
                                        <asp:label ID="lblQtyBeforeSource" runat="server" 
                                        text='<%# ((decimal) Eval ("QtyBeforeSource") == -1)?" ":Eval ("QtyBeforeSource") %>' />
                                    </center>    
                            </itemtemplate>
                            </asp:templatefield>                    

                            <asp:templatefield headertext="Cant. Ant. Dest" accessibleHeaderText="QtyBeforeTarget" SortExpression="QtyBeforeTarget">
                                <ItemStyle Wrap="false" />
                                <itemtemplate>
                                    <center>
                                        <asp:label ID="lblQtyBeforeTarget" runat="server" 
                                        text='<%# ((decimal) Eval ("QtyBeforeTarget") == -1)?" ":Eval ("QtyBeforeTarget") %>' />
                                    </center>    
                            </itemtemplate>
                            </asp:templatefield>    

                             <asp:BoundField DataField="ReasonCode" HeaderText="Razón" AccessibleHeaderText="ReasonCode" SortExpression="ReasonCode"  ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="HoldCode" HeaderText="Bloqueo" AccessibleHeaderText="HoldCode" SortExpression="HoldCode"  ItemStyle-CssClass="text"/>
                             <asp:BoundField DataField="RoutingCode" HeaderText="Ruta" AccessibleHeaderText="RoutingCode" SortExpression="RoutingCode"  ItemStyle-CssClass="text"/>
                             
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

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Fin" Visible="false" />    	
    <asp:Label ID="lblFilterName" runat="server" Text="Destino" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>