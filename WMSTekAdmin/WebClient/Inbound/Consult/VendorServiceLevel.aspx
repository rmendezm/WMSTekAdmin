<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="VendorServiceLevel.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inbound.Consult.VendorServiceLevel" %>

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

                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();">
                        <asp:GridView ID="grdMgr" runat="server"                                 
                                OnRowCreated="grdMgr_RowCreated"
                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                EnableViewState="false"                
                                AllowPaging="True" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                            <Columns>
                                <%--  <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" accessibleHeaderText="Id" Visible="false"/>--%>
                                
                                <asp:TemplateField HeaderText="Cód. CD." accessibleHeaderText="whsCode" SortExpression="whsCode"  ItemStyle-CssClass="text" Visible="true">
                                    <itemtemplate>
                                        <asp:label ID="lblWhsCode" runat="server" text='<%# Eval ( "WhsCode" ) %>' />
                                    </itemtemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Centro Dist." accessibleHeaderText="whsName" SortExpression="whsName"  ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <asp:label ID="lblWhsName" runat="server" text='<%# Eval ( "WhsName" ) %>' />
                                    </itemtemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text" Visible="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnCode" runat="server" text='<%# Eval ( "OwnCode" ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOwnName" runat="server" text='<%# Eval ("OwnName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>                
                                
                                <asp:templatefield headertext="Cód. Proveedor" accessibleHeaderText="VendorCode" SortExpression="VendorCode" ItemStyle-CssClass="text" Visible="true">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <asp:label ID="lblVendorCode" runat="server" text='<%# Eval("VendorCode")%>' />
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>
                                
                                <asp:TemplateField HeaderText="Proveedor" accessibleHeaderText="VendorName" SortExpression="VendorName" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                    <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval("VendorName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Wrap="false"/>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Cant.Esperada" AccessibleHeaderText="StockExpected" SortExpression="StockExpected" ItemStyle-HorizontalAlign="Right" >
                                    <ItemTemplate>
                                        <asp:Label ID="lblStockExpected" runat="server" Text='<%# GetFormatedNumber(Eval ("StockExpected")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cant.Recibida" AccessibleHeaderText="StockReceived" SortExpression="StockReceived" ItemStyle-HorizontalAlign="Right" >
                                    <ItemTemplate>
                                        <asp:Label ID="lblStockReceived" runat="server" Text='<%# GetFormatedNumber(Eval ("StockReceived")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="% x Cant." AccessibleHeaderText="StockPercent" SortExpression="StockPercent" ItemStyle-HorizontalAlign="Right" >
                                    <ItemTemplate>
                                        <asp:Label ID="lblStockPercent" runat="server" Text='<%# GetFormatedNumber(Eval ("StockPercent")) %>'  />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Líneas Esperadas" AccessibleHeaderText="LineExpected" SortExpression="LineExpected" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLineExpected" runat="server" Text='<%# Eval ("LineExpected") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Líneas Recibidas" AccessibleHeaderText="LineReceived" SortExpression="LineReceived" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLineReceived" runat="server" Text='<%# Eval ("LineReceived") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="% x Línea" AccessibleHeaderText="LinePercent" SortExpression="LinePercent" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLinePercent" runat="server" Text='<%# GetFormatedNumber(Eval("LinePercent")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cant.Esperada(Fecha)" AccessibleHeaderText="DateExpected" SortExpression="DateExpected" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateExpected" runat="server" Text='<%# GetFormatedNumber(Eval ("DateExpected")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cant.Recibida(Fecha)" AccessibleHeaderText="DateReceived" SortExpression="DateReceived" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDateReceived" runat="server" Text='<%# GetFormatedNumber(Eval ("DateReceived")) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="% x Fecha" AccessibleHeaderText="DatePercent" SortExpression="DatePercent" ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDatePercent" runat="server" Text='<%# GetFormatedNumber(Eval ("DatePercent")) %>' />
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
                <%-- FIN Panel Grilla Principal --%>
            </div>
        </div>
    </div>  

 
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Fecha" Visible="false" />    	
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
<%-- Barra de Estado --%>        
<webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>