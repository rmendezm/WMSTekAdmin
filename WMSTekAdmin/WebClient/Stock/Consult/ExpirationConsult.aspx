<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpirationConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master"  Inherits="Binaria.WMSTek.WebClient.Stocks.ExpirationConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("ExpirationConsult_FindAll", "ctl00_MainContent_grdMgr", "ExpirationConsult");

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
        initializeGridDragAndDrop("ExpirationConsult_FindAll", "ctl00_MainContent_grdMgr", "ExpirationConsult");
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
                            OnRowCreated="grdMgr_RowCreated"
                            EnableViewState="False" onrowdatabound="grdMgr_RowDataBound1"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                
                             <Columns>
                                <asp:templatefield HeaderText="Cód.Centro" AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
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
                     
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" ItemStyle-CssClass="text">
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
                                        
                                <asp:templatefield headertext="Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:templatefield headertext="Nombre" accessibleHeaderText="LongName" SortExpression="LongName">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblItemName" runat="server" text='<%# Eval ("Item.LongName") %>' />
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>

                                <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblItemDescription" runat="server" text='<%# Eval ("Item.Description") %>' />
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>
                     
                                <asp:templatefield headertext="Categoría" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblCategoryItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' />
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>
                    
                               <asp:templatefield headertext="Cantidad" accessibleHeaderText="ItemQty" SortExpression="ItemQty">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <right>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblQty" runat="server" 
                                                text='<%# ((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty") %>' />
                                            </div>
                                        </right>    
                                    </itemtemplate>
                                </asp:templatefield>
                    
                                <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblExpiration" runat="server"  text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>
                                                   
                                <asp:templatefield headertext="Días Venc." accessibleHeaderText="ExpirationDays" SortExpression="ExpirationDays">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblExpirationDays" runat="server"></asp:Label>
                                            </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>                     
                                                   
                                <asp:templatefield headertext="Fifo" accessibleHeaderText="FifoDate" SortExpression="FifoDate">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblFifoDate" runat="server"  
                                                text='<%# ((DateTime) Eval ("FifoDate") > DateTime.MinValue)? Eval("FifoDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>                        
                     
                                <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                    <ItemStyle Wrap="false" />
                                    <itemtemplate>
                                        <center>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblFabricationDate" runat="server"  
                                                text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                            </div>
                                        </center>    
                                    </itemtemplate>
                                </asp:templatefield>                        
                   
                                <asp:templatefield headertext="Ubicación" accessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                           <asp:label ID="lblIdLocCode" runat="server" text='<%# Eval ("Location.IdCode") %>' />
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>                   
                                       
                               <asp:templatefield headertext="Lote" accessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>                                                           

                               <asp:templatefield headertext="LPN" accessibleHeaderText="IdLpnCode" ItemStyle-CssClass="text">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdLpnCode" runat="server" text='<%# Eval ( "Lpn.Code" ) %>'></asp:Label>
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>
                    
                               <asp:templatefield headertext="Tipo LPN" accessibleHeaderText="LpnTypeCode">
                                    <itemtemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLpnTypeCode" runat="server" text='<%# Eval ( "Lpn.LPNType.Code" ) %>'></asp:Label>
                                        </div>
                                    </itemtemplate>
                                </asp:templatefield>                    
                    
                            <asp:templatefield headertext="Precio" accessibleHeaderText="Price" SortExpression="Price">
                                <itemtemplate>
                                    <center>
                                        <div style="word-wrap: break-word;">
                                            <asp:label ID="lblPrice" runat="server" text=' <%# ((decimal) Eval ("Price") == -1 )?" ": Eval ("Price") %>' />
                                        </div>
                                    </center>
                                </itemtemplate>
                            </asp:templatefield>
                
                           <asp:TemplateField HeaderText="Nº Doc" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber" ItemStyle-CssClass="text">
                                <itemtemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ( "InboundOrder.Number" ) %>' />
                                    </div>
                                </itemtemplate>
                                <ItemStyle Wrap="false"/>
                            </asp:TemplateField>
                
                            <%--<asp:TemplateField HeaderText="Nº Línea" AccessibleHeaderText="InboundLineNumber" ItemStyle-CssClass="text">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label ID="lblInboundLineNumber" runat="server" text='<%# ((int) Eval ("InboundLineNumber")  == -1 )?" ": Eval ("InboundLineNumber")%>'></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>       
                
                            <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("Item.GrpItem1.Name") %>' />
                                   </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                       <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("Item.GrpItem2.Name") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("Item.GrpItem3.Name") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("Item.GrpItem4.Name") %>' />
                                   </div>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                
                            <asp:TemplateField HeaderText="Sello" AccessibleHeaderText="SealNumber" ItemStyle-CssClass="text">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label runat="server" ID="lblSealNumber" Text='<%# Bind("Seal") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                
                            <asp:TemplateField HeaderText="Bloqueo Ubic" AccessibleHeaderText="ReasonCode" ItemStyle-CssClass="text">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label runat="server" ID="lblReasonCode" Text='<%# Bind("Location.Reason.Name") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                
                            <asp:TemplateField HeaderText="Bloqueo Stock" AccessibleHeaderText="HoldCode" ItemStyle-CssClass="text">
                                <ItemTemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:Label runat="server" ID="lblHoldCode" Text='<%# Bind("Hold") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>      
                                 
                             <asp:templatefield headertext="Presentación por Defecto" accessibleHeaderText="ItemUomName" SortExpression="ItemUomName">
                                <itemtemplate>
                                    <div style="word-wrap: break-word;">
                                        <asp:label ID="lblItemUomName" runat="server" text='<%# Eval ("Item.ItemUom.Name") %>' />
                                    </div>
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
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>