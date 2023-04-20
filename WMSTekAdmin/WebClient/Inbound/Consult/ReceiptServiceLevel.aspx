<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ReceiptServiceLevel.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.ReceiptServiceLevel" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("GetReceiptDetailByMainFilter", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("GetReceiptDetailByMainFilter", "ctl00_MainContent_grdMgr");
        }
    </script>

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
                                EnableViewState="false"                
                                AllowPaging="True" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" accessibleHeaderText="Id" Visible="false"/>
                            
                                <asp:TemplateField HeaderText="Nº Doc." accessibleHeaderText="InboundOrder" SortExpression="InboundOrder"  ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ("InboundOrder.Number") %>' />
                                    </itemtemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                            
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnCode" runat="server" text='<%# Eval ( "InboundOrder.Owner.Code" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnName" runat="server" text='<%# Eval ( "InboundOrder.Owner.TradeName" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                
                            
                                <asp:templatefield headertext="Recepción" accessibleHeaderText="ReceiptDate">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <asp:label ID="lblReceiptDate" runat="server" text='<%# ((DateTime) Eval ("Receipt.ReceiptDate") > DateTime.MinValue)? Eval("Receipt.ReceiptDate", "{0:d}"):"" %>' />
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>
                            
                                <asp:TemplateField HeaderText="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("Item.Code") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Descrip. Item" AccessibleHeaderText="Description" SortExpression="Description" >
                                    <ItemTemplate>
                                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>  
                                
                                <asp:TemplateField HeaderText="Camión" accessibleHeaderText="IdTruckCode" SortExpression="IdTruckCode">
                                    <ItemTemplate>
                                        <asp:label ID="lblIdTruckCode" runat="server" text='<%# Eval ( "Receipt.Truck.IdCode" ) %>' />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                           
                                <asp:templatefield HeaderText="Cant. Esperada" AccessibleHeaderText="Qty" >
                                    <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                    <itemtemplate>
                                        <asp:label  ID="lblQty" runat="server" text='<%# GetFormatedNumber(Eval("Qty")) %>' />
                                </itemtemplate>
                                </asp:templatefield>  
                                <asp:templatefield HeaderText="Cant. Recibida" AccessibleHeaderText="Received" >
                                    <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                    <itemtemplate>
                                        <asp:label  ID="lblReceived" runat="server" text='<%# GetFormatedNumber(Eval("Received")) %>' />
                                </itemtemplate>
                                </asp:templatefield>  
                           
                                <%-- <asp:BoundField DataField="Qty" HeaderText="Cant. Esperada" accessibleHeaderText="Qty" 
                                SortExpression="Qty" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign ="Right"/>
                            
                                <asp:BoundField DataField="Received" HeaderText="Cant. Recibida" accessibleHeaderText="Received" 
                                SortExpression="Received" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right"/>--%>
                           
                                <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WhsCode" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <asp:label ID="lblWhsCode" runat="server" text='<%# Eval ( "Receipt.Warehouse.Code" ) %>' />
                                    </itemtemplate>
                                    </asp:templatefield>
                             
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="WhsName" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <asp:label ID="lblWhsName" runat="server" text='<%# Eval ( "Receipt.Warehouse.Name" ) %>' />
                                    </itemtemplate>
                                    </asp:templatefield>
                                             
                                <asp:templatefield headertext="% Nivel de Servicios" accessibleHeaderText="ServiceLevel" >
                                    <ItemStyle Wrap="false" HorizontalAlign="Right" />
                                    <itemtemplate>
                                            <asp:label  ID="lblServiceLevel" runat="server" text='<%# (((Decimal) Eval("Received") / (Decimal) Eval("Qty") * 100)) %>' />
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

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Recep." Visible="false" />    	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
<%-- Barra de Estado --%>        
<webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>