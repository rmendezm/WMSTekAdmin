<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="ItemRotation.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Movement.Consult.ItemRotation" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("MovLog_FindRotationItem", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();

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
            initializeGridDragAndDrop("MovLog_FindRotationItem", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }

    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" 
                                    DataKeyNames="Id" 
                                    runat="server" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    AllowPaging="True" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                <Columns>

                                    <asp:TemplateField HeaderText="Id Movto" AccessibleHeaderText="MovementId" SortExpression="Movement" Visible="false" >
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblMovementId" runat="server" Text='<%# Eval ( "Id" ) %>'></asp:Label>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Tipo" AccessibleHeaderText="MovementTypeId" SortExpression="MovementType" Visible="false">
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblMovementType" runat="server" Text='<%# Eval ( "MovementType.Id" ) %>'></asp:Label>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Movto." AccessibleHeaderText="MovementTypeName" SortExpression="MovementType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMovementTypeName" runat="server" Text='<%# Eval ( "MovementType.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:BoundField DataField="StartTime" HeaderText="Inicio" AccessibleHeaderText="StartTime"
                                        SortExpression="StartTime" ItemStyle-HorizontalAlign="Center" />

                                    <asp:BoundField DataField="EndTime" HeaderText="Fin" AccessibleHeaderText="EndTime"
                                        SortExpression="EndTime" ItemStyle-HorizontalAlign="Center" />

                                    <asp:BoundField DataField="UserName" HeaderText="Operador" AccessibleHeaderText="UserName"
                                        SortExpression="UserName" ItemStyle-HorizontalAlign="Center" />

                                    <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" ItemStyle-CssClass="text" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerId" runat="server" Text='<%# ((int) Eval ( "Owner.Id" )== -1)?" ":Eval ( "Owner.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="ItemId" SortExpression="ItemId" ItemStyle-CssClass="text" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemId" runat="server" Text='<%# ((int) Eval ("Item.Id")== -1)?" ":Eval ("Item.Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ("Item.Code") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Item" AccessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval ("Item.Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Id Categoria" AccessibleHeaderText="IdCtgItem" SortExpression="IdCtgItem" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# ((int) Eval ("CategoryItem.Id")== -1)?" ":Eval ("CategoryItem.Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCategoryItemName" runat="server" Text='<%# Eval ("CategoryItem.Name") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="LotNumber" HeaderText="Lote" AccessibleHeaderText="LotNumber" SortExpression="LotNumber" ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="text" />

                                    <asp:TemplateField HeaderText="Fecha Ingreso" AccessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Expira." AccessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Fabrica" AccessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="IdLpnCodeSource" HeaderText="LPN Origen" AccessibleHeaderText="IdLpnCodeSource" SortExpression="IdLpnCodeSource" ItemStyle-CssClass="text" />
                                
                                    <asp:BoundField DataField="IdLpnCodeTarget" HeaderText="LPN Destino" AccessibleHeaderText="IdLpnCodeTarget" SortExpression="IdLpnCodeTarget" ItemStyle-CssClass="text" />
                                    
                                    <asp:BoundField DataField="LpnParentSource" HeaderText="LPN Padre Origen" AccessibleHeaderText="LpnParentSource" SortExpression="LpnParentSource" ItemStyle-CssClass="text" />
                                    
                                    <asp:BoundField DataField="LpnParentTarget" HeaderText="LPN Padre Destino" AccessibleHeaderText="LpnParentTarget" SortExpression="LpnParentTarget" ItemStyle-CssClass="text" />
                                    
                                    <asp:BoundField DataField="IdLocCodeProposal" HeaderText="Ubic. Propuesta" AccessibleHeaderText="IdLocCodeProposal" SortExpression="IdLocCodeProposal" />
                                    
                                    <asp:BoundField DataField="IdLocCodeSource" HeaderText="Ubic. Origen" AccessibleHeaderText="IdLocCodeSource" SortExpression="IdLocCodeSource" ItemStyle-CssClass="text" />
                                    
                                    <asp:BoundField DataField="IdLocCodeTarget" HeaderText="Ubic. Destino" AccessibleHeaderText="IdLocCodeTarget" SortExpression="IdLocCodeTarget" ItemStyle-CssClass="text" />

                                    <asp:TemplateField HeaderText="Qty Movimiento" AccessibleHeaderText="ItemQtyMov" SortExpression="ItemQtyMov">
                                        <ItemStyle Wrap="false" />
                                        <ItemTemplate>
                                            <center>
                                                <asp:Label ID="lblItemQtyMov" runat="server" Text='<%# GetFormatedNumber(((decimal) Eval ("ItemQtyMov") == -1)?" ":Eval ("ItemQtyMov")) %>' />
                                            </center>
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

    <asp:Label ID="lblFilterDate" runat="server" Text="Inicio" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Destino" Visible="false" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
