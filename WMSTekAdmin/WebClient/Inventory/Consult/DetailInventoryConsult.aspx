<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="DetailInventoryConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inventory.Consult.DetailInventoryConsult" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("GetInventoryDetail_SpecialAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("GetInventoryDetail_SpecialAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                    <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                    <asp:GridView ID="grdMgr" 
                            runat="server" DataKeyNames="Id" 
                            OnRowCreated="grdMgr_RowCreated" 
                            EnableViewState="false"
                            AllowSorting="False"
                            AllowPaging="true" 
                            OnRowDataBound="grdMgr_RowDataBound"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Id Detalle" AccessibleHeaderText="IdInventoryDetail">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdInventoryDetail" runat="server" Text='<%# Eval ( "id" )%>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>                        
                                <asp:TemplateField HeaderText="Id Inventario" AccessibleHeaderText="IdInventoryOrder" ItemStyle-CssClass="text" Visible="false">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdInventoryOrder" runat="server" Text='<%# Eval ( "InventoryOrder.id" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="N° Inventario" AccessibleHeaderText="InventoryNumber">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblNroInventario" runat="server" Text='<%# Eval ( "InventoryOrder.Number" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Centro" AccessibleHeaderText="IdWhs" ItemStyle-CssClass="text" Visible="false">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "InventoryOrder.Warehouse.Id" ) %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "InventoryOrder.Warehouse.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode" ItemStyle-CssClass="text">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Location.IdCode" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Es Vacia?" AccessibleHeaderText="IsEmptyLocation">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIsEmptyLocation" runat="server" Text='<%# ((bool)Eval ( "isEmptyLocation" )== true)?"SI":"NO" %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem" Visible="false">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdItem" runat="server" Text='<%# Eval ( "Item.Id" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Código Item" AccessibleHeaderText="ItemCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "Item.LongName" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn" Visible="false">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "InventoryOrder.Owner.Id" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ( "InventoryOrder.Owner.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval ( "lotNumber" ) %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Serie" AccessibleHeaderText="SerialNumber">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblSerialNumber" runat="server" Text='<%# Eval ( "serialNumber" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha Ingreso" AccessibleHeaderText="FifoDate">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime) Eval ("fifoDate") > DateTime.MinValue)? Eval("fifoDate", "{0:d}"):"" %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha Expira" AccessibleHeaderText="ExpirationDate">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime) Eval ("expirationDate") > DateTime.MinValue)? Eval("expirationDate", "{0:d}"):"" %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha Fabrica" AccessibleHeaderText="FabricationDate">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime) Eval ("fabricationDate") > DateTime.MinValue)? Eval("fabricationDate", "{0:d}"):"" %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cant. Sistema" AccessibleHeaderText="ActualQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblActualQty" runat="server" Text='<%# GetFormatedNumber(Eval ( "StockQty" )) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cant. Contada" AccessibleHeaderText="ItemQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(Eval ( "itemQty" )) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Categoria" AccessibleHeaderText="IdCtgItem" Visible="false">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# Eval ( "CategoryItem.Id" ) %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Categoria" AccessibleHeaderText="CtgName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "CategoryItem.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Operador" AccessibleHeaderText="UserInventory">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblUserInventory" runat="server" Text='<%# Eval ( "userInventory" ) %>'></asp:Label>
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reintentos" AccessibleHeaderText="RetryQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblRetryQty" runat="server" Text='<%# Eval ( "retryQty" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Eval ( "LPN.IdCode" ) %>'></asp:Label>
                                       </div>
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
        
 
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Emisión" Visible="false" />
    <asp:Label ID="lblInventoryCode" runat="server" Text="Nro. Inventario" Visible="false" />
    <asp:Label ID="lblFilterOther" runat="server" Text="Otros" Visible="false" />
    <asp:Label ID="lblLotNumber" runat="server" Text="Lote" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>
