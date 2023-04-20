<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="BuildingReplacementTask.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.BuildingReplacementTask" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    //window.onresize = SetDivs;
    //var prm = Sys.WebForms.PageRequestManager.getInstance();
    //prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("BuildingReplacementTaskConsult", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("BuildingReplacementTaskConsult", "ctl00_MainContent_grdMgr");
        setHeightGrid();
    }

    function setHeightGrid() {
        var extraHeight = 20;
        var maxHeight = $("#ctl00_divTop").height() - $(".divGridTitleDispatch").height() - $(".row-height-filter").height() - extraHeight;
        $(".froze-header-grid").css("height", maxHeight + "px");
    }

    function setDivsAfter() {

        var extraHeight = 20;
        var heightDivContainerTable = $("div.froze-header-grid").height();

        var totalHeight = heightDivContainerTable - $(".divGridTitleDispatch").height();
        totalHeight = totalHeight - extraHeight;

        $("div.froze-header-grid").css("height", totalHeight + "px");

    }
</script>

    <style>
        .divGrid{
            overflow:visible !important;
        }
    </style>
     <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid">
                            <div class="divGridTitleDispatch">
                                <div>
                                   <asp:Label ID="lblPendingOrders" runat="server" Text="Generar tareas de reposición" />
                                   <asp:ImageButton ID="btnReprocess" runat="server" onclick="btnReprocess_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png" ToolTip="Generar Tareas de reposición"/> 
                                </div>
                            </div> 
                            <%-- Grilla Principal --%>
                            <%-- <div id="divGrid1" runat="server" class="divGrid">    --%>                           
                                <asp:GridView 
                                    ID="grdMgr" 
                                    runat="server" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    OOnPageIndexChanging="grdMgr_PageIndexChanging"
                                    OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    AllowPaging="True" 
                                    EnableViewState="False" 
                                    AutoGenerateColumns="False" 
                                    style="margin-right: 0px"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOrder', this.checked)"
                                                    id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 20px">
                                                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>                    
                                        <asp:TemplateField HeaderText="IdWhs" AccessibleHeaderText="IdWhs">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdWhs" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>     
                                               
                                        <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="IdOwn" AccessibleHeaderText="IdOwn">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ( "Owner.Id" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                 
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Item.Code" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                           
                                        <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# ((int)Eval("Item.Id") == -1) ? " ":Eval("Item.Id") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                  
                                        <asp:TemplateField HeaderText="Item" AccessibleHeaderText="LongItemName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "Item.LongName" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Fila" AccessibleHeaderText="RowLoc">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowLoc" runat="server" Text='<%# Eval ( "Location.Row" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="ColumnLoc">
                                            <ItemTemplate>
                                                <asp:Label ID="lblColumnLoc" runat="server" Text='<%# Eval ( "Location.Column" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="LevelLoc">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLevelLoc" runat="server" Text='<%# Eval ( "Location.Level" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>                        
                                        <asp:TemplateField HeaderText="Ubicación" AccessibleHeaderText="IdLocCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdLocCode" runat="server" Text='<%# Eval ( "Location.IdCode" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>  
                            
                                        <asp:TemplateField HeaderText="Mínimo" AccessibleHeaderText="ReOrderPoint">
                                            <ItemTemplate>
                                                <asp:Label ID="lblReOrderPoint" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("ReOrderPoint") == -1) ? " " : Eval("ReOrderPoint"))%>' />
                                                                                   
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Máximo" AccessibleHeaderText="ReOrderQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblReOrderQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("ReOrderQty") == -1) ? " " : Eval("ReOrderQty"))%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Pendiente" AccessibleHeaderText="RepoPending">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRepoPending" runat="server" Text='<%# ((decimal)Eval("RepoPending") == -1) ? " " : Eval("RepoPending")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                  
                                        <asp:TemplateField HeaderText="Cant. en pick" AccessibleHeaderText="ItemQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(((decimal)Eval("Stock.Qty") == -1) ? " " : Eval("Stock.Qty"))%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>   
                            
                                        <asp:TemplateField HeaderText="Cant. en Put" AccessibleHeaderText="PutawayQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPutawayQty" runat="server" Text='<%#GetFormatedNumber( ((decimal)Eval("PutawayQty") == -1) ? " " : Eval("PutawayQty")) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                            
                                        <%--<asp:TemplateField HeaderText="Id Categoria" AccessibleHeaderText="IdCtgItem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# ((int)Eval("Stock.CategoryItem.Id") == -1) ? " " : Eval("Stock.CategoryItem.Id")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>     
                            
                                         <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "Stock.CategoryItem.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>           
                            
                                        <asp:TemplateField HeaderText="Fec. Ingreso" AccessibleHeaderText="FifoDate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFifoDate" runat="server" Text='<%# ((DateTime)Eval("Stock.FifoDate") > DateTime.MinValue) ? Eval("Stock.FifoDate", "{0:d}"):"" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>         
                            
                                        <asp:TemplateField HeaderText="Expiración" AccessibleHeaderText="ExpirationDate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblExpirationDate" runat="server" Text='<%# ((DateTime)Eval("Stock.ExpirationDate") > DateTime.MinValue) ? Eval("Stock.ExpirationDate", "{0:d}"):"" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>       
                            
                                        <asp:TemplateField HeaderText="Fabricación" AccessibleHeaderText="FabricationDate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFabricationDate" runat="server" Text='<%# ((DateTime)Eval("Stock.FabricationDate") > DateTime.MinValue) ? Eval("Stock.FabricationDate", "{0:d}"):"" %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>    
                            
                                        <asp:TemplateField HeaderText="Lote" AccessibleHeaderText="LotNumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLotNumber" runat="server" Text='<%# Eval("Stock.Lot")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLpn" runat="server" Text='<%# Eval ( "Stock.Lpn.IdCode" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>  --%>                          
                            
                                        <asp:TemplateField HeaderText="Resultado de generación" AccessibleHeaderText="StatusGeneration">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatusGeneration" runat="server" Text='<%# Eval ( "TrackProcess" ) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>                        
                            
                                    </Columns>
                                </asp:GridView>
                            <%-- </div> --%>
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
                        <asp:AsyncPostBackTrigger ControlID="btnReprocess" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>  
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprGrid" />
    <%-- FIN Modal Update Progress --%>

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Usuario?"
        Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="Código Item" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Nombre Item" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
