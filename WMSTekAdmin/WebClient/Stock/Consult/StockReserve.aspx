<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="StockReserve.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Stocks.StockReserve" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">       
<script type="text/javascript" language="javascript">

    function resizeDiv() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("divPrincipal").style.height = h;
        document.getElementById("divPrincipal").style.width = w;
    }

    window.onresize = resizeDiv;
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDiv);

    $(document).ready(function () {
        initializeGridDragAndDrop("ReserveStock_FindAll", "ctl00_MainContent_hsVertical_leftPanel_ctl01_grdItems");
        initializeGridWithNoDragAndDrop();
        removeFooter("#ctl00_MainContent_grdSearchItems");

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
        initializeGridDragAndDrop("ReserveStock_FindAll", "ctl00_MainContent_hsVertical_leftPanel_ctl01_grdItems");
        initializeGridWithNoDragAndDrop();
        removeFooter("#ctl00_MainContent_grdSearchItems");
    }

    function setDivsAfter() {
        //Width
        var maxWidth = $("#hsVertical_LeftP_Content").width();
        maxWidth = maxWidth - 35;
        $("#ctl00_MainContent_hsVertical_leftPanel_ctl01_divUsers").css({ "overflow-y": "hidden", "overflow-x": "auto", "max-width": maxWidth + "px" });


        //Height
        var maxHeight = $("#hsVertical_LeftP_Content").height();
        var filterHeight = $(".row-height-filter").height() || 0;
        var labelHeight = $("#ctl00_MainContent_hsVertical_leftPanel_ctl01_Div1").height();
        var totalHeight = maxHeight - filterHeight - labelHeight - 95;

        $("#ctl00_MainContent_hsVertical_leftPanel_ctl01_divUsers .froze-header-grid").css("height", totalHeight + "px");

        var totalHeightDivLeft = maxHeight - filterHeight - labelHeight - 105;
        $("#ctl00_MainContent_hsVertical_leftPanel_ctl01_divUsers").css("height", totalHeightDivLeft + "px");        
    }

    function ShowProgress() {
        document.getElementById('<% Response.Write(uprEditNew.ClientID); %>').style.display = "inline";
    }

    function QtyValidation(sender, args) {
      
        var valueToValidate = document.getElementById("<%=txtQtyEdit.ClientID %>").value;

        console.log("valueToValidate " + valueToValidate);

        if (valueToValidate == '') {
            sender.innerHTML = "Cantidad es requerida";
            args.IsValid = false;     
        } else if (isNaN(valueToValidate)) {
            sender.innerHTML = "Cantidad debe es numerica";
            args.IsValid = false;     
        } else if (parseInt(valueToValidate) <= 0) {
            sender.innerHTML = "Cantidad debe ser mayor a 0";
            args.IsValid = false;     
        } else {
            args.IsValid = true;   
        }
    }

</script>

    <style>
        #ctl00_MainContent_pnlLookupItem{
            z-index: 10000001 !important;
            max-height: 500px !important;
        }

        #divReserveItems {
            height: 370px;
	        width: 100%;
        }
    </style>
        
    <div id="divPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">  
    
       <spl:Splitter  LiveResize="false" CookieDays="0" ID="hsVertical" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" CSSLeftPanel="splitterLeftPanel" OnSplitterResize="setDivsAfter()">
         <LeftPanel ID="leftPanel" WidthDefault="520" >
                <Content>
                    <%-- Muestra Los Item  --%>
                    <asp:UpdatePanel ID="upItem" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>                                                      
                           
                            <%-- FIN Opciones de Menu --%>
                            
                            <%-- Usuarios Asignados --%>
                            <div id="divUsers" runat="server" visible="true" class="divFloatLeft">
                                <div id="Div1" runat="server" class="divFloatLeftHeaderNarrow">
                                    <asp:Label ID="lblTitleItem" Text="Lista de Items" runat="server" />
                                </div>

                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div ID="divGridItem">

                                             <asp:GridView ID="grdItems" runat="server"  
                                                DataKeyNames="IdItem" 
                                                SkinID="grdDetail"
                                                ShowFooter="false"                                                                     
                                                ShowHeader="true" 
                                                AutoGenerateColumns="False"                                     
                                                PageSize="50" 
                                                AllowPaging="True"   
                                                OnRowDataBound="grdItems_RowDataBound"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                            <EmptyDataRowStyle Wrap="False" />
                                            <Columns>
                                                <asp:BoundField DataField="IdItem" HeaderText="Id Item" 
                                                    accessibleHeaderText="IdItem" SortExpression="IdItem" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                                </asp:BoundField>                               
                                                <asp:BoundField DataField="ItemCode" HeaderText="Cod Item" 
                                                    accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                                </asp:BoundField> 
                                                <asp:BoundField DataField="ShortNameItem" HeaderText="Nombre Item" 
                                                    accessibleHeaderText="ShortNameItem" SortExpression="ShortNameItem" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Left">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Reserve" HeaderText="Reservado" DataFormatString="{0:F0}"
                                                    accessibleHeaderText="Reserve" SortExpression="Reserve" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Available" HeaderText="Disponible" DataFormatString="{0:F0}"
                                                    accessibleHeaderText="Available" SortExpression="Available" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                                </asp:BoundField>                                    
                                    
                                            </Columns>
                                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                            <EditRowStyle BackColor="#999999" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>                           
                           
                                   
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%-- FIN Usuarios Asignados --%>
                            
                        </ContentTemplate>  
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" /> 
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprItem" runat="server" AssociatedUpdatePanelID="upItem"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprItem" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprItem" />
                    <%-- FIN Configuración de Roles --%>    
                </Content>
               <%-- <Footer Height="67">
                  <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>   --%>      
             </LeftPanel>
             
             <RightPanel ID="rightPanel"  WidthMin="100">
                <Content> 
                    <%-- Asignación de Usuarios a Roles --%>
                    <asp:UpdatePanel ID="upUser" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                    
                    <%-- Lista de Usuarios --%>
                    
                    <%-- Roles Asignados --%>
                    <div id="divRoles" runat="server" visible="true" class="divFloatLeft" >
                        <div id="Div3" runat="server" class="divFloatLeftHeaderNarrow">
                            <asp:Label ID="lblItemsAsignados" Text="Items con Stock Reservado" runat="server" />
                        </div>
                        
                        <table>
                            <tr>
                                <td>
                                    <div class="mainFilterPanelItem">
                                        <asp:Label ID="lblCode" runat="server" Text="Cliente" /><br />
                                        <asp:DropDownList ID="ddlCustomer" runat="server" Width="120px" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" /> 
                                    </div>
                                    
                                    <div class="mainFilterPanelItem">
                                        <asp:Label ID="Label1" runat="server" Text="Item" /><br />
                                        <asp:TextBox ID="txtCode" runat="server" Width="100px" Enabled ="False" />
                                        <asp:ImageButton ID="imgBtnSearchItem2" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" OnClick="imgBtnSearchItem2_Click" Width="18px"  />
                                        <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="txtCode" ValidationGroup="AddItem" Text=" * " ErrorMessage="Item es requerido." />
                                    </div>
                                    <div class="mainFilterPanelItem">    
                                        <asp:Label ID="lblDescription" runat="server" Text="Descripción" /><br />
                                        <asp:TextBox ID="txtDescription" runat="server" Width="240px" MaxLength="30" Enabled="False" ReadOnly="True" />
                                        <%--<asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ValidationGroup="AddItem" Text="  " ErrorMessage="Descripción es requerido." />--%>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="mainFilterPanelItem">    
                                        <asp:Label ID="lblCantidadDisponible" runat="server" Text="Cant. Disponible" /><br />
                                        <asp:TextBox ID="txtQtyAvailable" runat="server" Width="80px" Text="0" Style="text-align:right" MaxLength="8" Enabled="False" ReadOnly="True"/>   
                                        <asp:RangeValidator ID="rvQtyAvailable" runat="server" ControlToValidate="txtQtyAvailable" ErrorMessage="Cantidad a reservar debe ser > 0" 
                                          Text=" * " MinimumValue="0" ValidationGroup="AddItem" Type="Double" Enabled="false" />                          
                                    </div>
                                    
                                    <div class="mainFilterPanelItem" >    
                                        <asp:Label ID="lblCantidad" runat="server" Text="Cant. Reservar" /><br />
                                        <asp:TextBox ID="txtQty" runat="server" Width="80px" Text="0" MaxLength="8" Style="text-align:right"  onFocus = 'ClearQty()'/>
                                        
                                        <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="txtQty" ValidationGroup="AddItem" Text=" * " ErrorMessage="Cantidad Reservar es requerido." />
                                        
                                        <asp:RangeValidator ID="rvQty" runat="server" ControlToValidate="txtQty" ErrorMessage="Cantidad a reservar debe ser > 0 y <= cant. disponible." 
                                        Text=" * " MinimumValue="0,1"  MaximumValue="9999999" ValidationGroup="AddItem" Type="Double" Enabled="false" />    
                                        
                                        <%--<asp:CompareValidator id="cvQty" Display="dynamic" ControlToValidate="txtQty" ControlToCompare="txtQtyAvailable" ForeColor="red" 
                                        Type="Double" EnableClientScript="false" Text="Validation Failed!" runat="server" Operator="GreaterThanEqual" ValidationGroup="AddItem"  />--%>
                                    </div>
                                    
                                    <div class="mainFilterPanelItem">
                                       <br />
                                        <asp:ImageButton ID="imgBtnAddItem" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_add_item.png"  ToolTip="Agregar Item"
                                        OnClick="imgBtnAddItem_Click" ValidationGroup="AddItem" />
                                    </div> 
                                    <asp:Panel ID="pnlError" runat="server" Visible="false">
                                        <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="El item ya fue ingresado" />
                                    </asp:Panel>
                                    <div class="mainFilterPanelItem">
                                        <asp:ValidationSummary ID="valAddItem" runat="server" ValidationGroup="AddItem" ShowMessageBox="false" CssClass="modalValidation"/>                            
                                    </div>  
                                </td>
                            </tr>
                        </table>       
                            
                        <div ID="divReserveItems">
                            <div id="divDetailTitle" runat="server" class="divGridDetailTitle">
                                <asp:Label ID="lblGridDetail" runat="server" Text="Items Reservados: " />
                            </div>                    
                            <asp:GridView ID="grdItemsReserve" 
                                runat="server" 
                                DataKeyNames="IdItem,IdCustomer,IdOwn,Idwhs" 
                                ShowFooter="false"                                     
                                AllowPaging="false" 
                                ShowHeader="true"                                
                                SkinID="grdDetail"
                                 EmptyDataText="No se han encontrado registros."
                                OnRowDeleting="grdItemsReserve_RowDeleting"
                                OnRowEditing="grdItemsReserve_RowEditing"
                                OnRowCreated="grdItemsReserve_RowCreated" 
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                OnRowDataBound="grdItemsReserve_RowDataBound"
                                EnableTheming="false">
                                <EmptyDataRowStyle Wrap="False" />
                                    <Columns>
                                        <asp:BoundField DataField="IdCustomer" HeaderText="Id Cliente"  Visible="false"
                                                accessibleHeaderText="IdCustomer" SortExpression="IdCustomer" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                            <asp:BoundField DataField="IdOwn" HeaderText="Id Owner"  Visible="false"
                                                accessibleHeaderText="IdOwn" SortExpression="IdOwn" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Idwhs" HeaderText="Id Whs"  Visible="false"
                                                accessibleHeaderText="Idwhs" SortExpression="Idwhs" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>                         
                                            <asp:BoundField DataField="NameCustomer" HeaderText="Cliente" 
                                                accessibleHeaderText="NameCustomer" SortExpression="NameCustomer" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>                                        
                                            <asp:BoundField DataField="IdItem" HeaderText="Id Item" 
                                                accessibleHeaderText="IdItem" SortExpression="IdItem" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ItemCode" HeaderText="Item" 
                                                accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ShortNameItem" HeaderText="Nombre Item" 
                                                accessibleHeaderText="ShortNameItem" SortExpression="ShortNameItem" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Reserve" HeaderText="Reservado" DataFormatString="{0:F0}"
                                                accessibleHeaderText="Reserve" SortExpression="Reserve" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Acciones"> 
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete" ToolTip="Eliminar"
                                                       CommandArgument='<%# Eval("IdItem") + ";" +Eval("IdCustomer") %>' ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" />

                                                    <asp:ImageButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit" ToolTip="Actualizar"
                                                       CommandArgument='<%# Eval("IdItem") + ";" +Eval("IdCustomer") %>' ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                </Columns>
                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                <EditRowStyle BackColor="#999999" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>                            
                                                        
                        </div>
                        
                    </div>
                    <%-- FIN Roles Asignados --%>    
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" /> 
                            <asp:AsyncPostBackTrigger ControlID="grdSearchItems" EventName="RowCommand" />
                        </Triggers>
                    </asp:UpdatePanel>    
                    <asp:UpdateProgress ID="uprUser" runat="server" AssociatedUpdatePanelID="upUser"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="muprUser" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprUser" />                                                          
                    <%-- FIN Asignación de Usuarios a Roles --%>
                 </Content>               
              </RightPanel>
             </spl:Splitter>
             
             
        <asp:UpdatePanel ID="upSelectItem" runat="server" UpdateMode="Conditional">
        <ContentTemplate> 
        <%-- Lookup Items --%>
        <div id="divLookupItem" runat="server" visible="false">
        <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
        <!-- Boton 'dummy' para propiedad TargetControlID -->
        <ajaxToolkit:ModalPopupExtender ID="mpLookupItem" runat="server" TargetControlID="btnDummy"
            PopupControlID="pnlLookupItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupItem" Drag="true">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="pnlLookupItem" runat="server" CssClass="modalBox">

            <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                <div class="divCaption">
                    <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item"/>
                    <asp:ImageButton ID="ImageButton2" runat="server" ToolTip="Cerrar" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                </div>   
                <div id="divPageGrdSearchItems" runat="server">
                    <div style="font-family: Verdana, Helvetica, Sans-Serif; font-weight: normal; font-size: 11px">
                        <asp:ImageButton ID="btnFirstGrdSearchItems" runat="server" OnClick="btnFirstGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first_dis.png" />
                        <asp:ImageButton ID="btnPrevGrdSearchItems" runat="server" OnClick="btnPrevGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next_dis.png" />
                        Pág. 
                        <asp:DropDownList ID="ddlPagesSearchItems" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSearchItemsSelectedIndexChanged" SkinID="ddlFilter" /> 
                        de 
                        <asp:Label ID="lblPageCountSearchItems" runat="server" Text="" />
                        <asp:ImageButton ID="btnNextGrdSearchItems" runat="server" OnClick="btnNextGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" />
                        <asp:ImageButton ID="btnLastGrdSearchItems" runat="server" OnClick="btnLastGrdSearchItems_Click" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" />
                    </div>
                </div>                 
            </asp:Panel>
            
            <div class="modalControls">
                <asp:HiddenField ID="hidItemId" runat="server" Value="-1" />
                <webUc:ucLookUpFilter ID="ucFilterItem" runat="server" />                        
                <div class="divCtrsFloatLeft">
                    <%--<div class="divLookupGrid">--%>
                        <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id" 
                            OnRowCommand="grdSearchItems_RowCommand" AllowPaging="true"
                            AutoGenerateColumns="False"
                            CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                            OnRowDataBound="grdItemsReserve_RowDataBound"
                            EnableTheming="false">
                            <Columns>
                                <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                    SortExpression="Id" />
                                <asp:TemplateField AccessibleHeaderText="ItemCode" HeaderText="Cód.">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCode0" runat="server" Text='<%# Eval ("Code") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="Description" HeaderText="Item">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemName0" runat="server" Text='<%# Eval ("Description") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <center>
                                            <asp:ImageButton ID="imgBtnAddItem" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                Width="20px" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                   <%-- </div>--%>
                </div>
                <div style="clear:both" />
            </div>
            
        </asp:Panel>
        </div>
        <%-- FIN Lookup Items --%> 
        
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" /> 
            <asp:AsyncPostBackTrigger ControlID="grdSearchItems" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$rightPanel$ctl01$imgBtnSearchItem2" EventName="Click" />
                                   
        </Triggers>
        </asp:UpdatePanel>    
        <asp:UpdateProgress ID="uprSelectItem" runat="server" AssociatedUpdatePanelID="upSelectItem" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
        <div class="divProgress">
            <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
        </div>
        </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprSelectItem" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprSelectItem" />      
                    
        <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
            <ContentTemplate> 
                <div id="divEditNew" runat="server" visible="false">    
                    <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />

                    <ajaxToolkit:ModalPopupExtender ID="modalPopEditReserve" runat="server" 
                        TargetControlID="btnDummy2" 
                        BehaviorID="BImodalPopUpEdit"                                  
                        PopupControlID="pnlEdit" 
                        BackgroundCssClass="modalBackground" 
                        PopupDragHandleControlID="EditCaption" 
                        Drag="true">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Panel ID="pnlEdit" runat="server" CssClass="modalBox" Style="display: none;">

                        <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                            <div class="divCaption">
                                <asp:Label ID="lblNew" runat="server" Text="Editar Reserva" />
                            </div>
                        </asp:Panel>

                        <div class="modalControls">
                            <div class="divCtrsFloatLeft">

                                <div id="divItem" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblItemEditTitle" runat="server" Text="Item" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:Label ID="lblItemEdit" runat="server" />
                                    </div>
                                </div>

                                <div id="divQty" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblQtyEdit" runat="server" Text="Cantidad" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox SkinID="txtFilter" ID="txtQtyEdit" runat="server" Width="50px"  MaxLength="10" ToolTip="Ingrese Cantidad" ValidationGroup="EditReserve" AutoPostBack="false" />
                                        <asp:RequiredFieldValidator ID="rfvQtyEdit" runat="server" ControlToValidate="txtQtyEdit" ValidationGroup="EditReserve" Text=" * " ErrorMessage="Cantidad es requerida"  />
                                        <asp:RangeValidator ID="rvQtyEdit" runat="server" ControlToValidate="txtQtyEdit" ErrorMessage="Cantidad a reservar debe ser > 0 y <= cant. disponible."  Text=" * " MinimumValue="0,1"  MaximumValue="9999999" ValidationGroup="EditReserve" Type="Double" Enabled="false" /> 
                                        <asp:CustomValidator runat="server" Display="Dynamic" ID="customValQtyEdit" ClientValidationFunction="QtyValidation" Text=" * "  ErrorMessage="" ControlToValidate="txtQtyEdit" ValidationGroup="EditReserve"> </asp:CustomValidator>  
                                    </div>
                                </div>

                            </div>
                            <div class="divValidationSummary" >                            
                                <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditReserve" CssClass="modalValidation" ShowMessageBox="false" />
                            </div>
                            <div id="divActions" runat="server" class="modalActions">
                                <asp:Button ID="btnUpdateReserve" runat="server" Text="Aceptar" OnClick="btnUpdateReserve_Click" OnClientClick="ShowProgress();" CausesValidation="true" ValidationGroup="EditReserve" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancelar"  OnClick="btnCerrar_Click"  />
                            </div>
                        </div>

                    </asp:Panel>
                </div>
            </ContentTemplate>
            <Triggers>
                <%-- <asp:AsyncPostBackTrigger ControlID="grdItemsReserve" EventName="RowCommand" />--%>
          </Triggers>
        </asp:UpdatePanel>

        <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
            <ProgressTemplate>                        
                <div class="divProgress">
                    <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />   
                    

    </div>    
    
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Item?" Visible="false" />
    <asp:Label id="lblFilterDate" runat="server" Text="Recep." Visible="false" />    	
    <asp:Label ID="lblFilterCode" runat="server" Text="Código" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblItemAsignado" runat="server" Text="Item se encuentra reservado." Visible="false" />
    <asp:Label ID="lblUnableToReserve" runat="server" Text="No hay cantidad disponible en item seleccionado" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    
         
    <script type="text/javascript" language="javascript">
        //Limpia la variable de Cantidad
        function ClearQty() {
            document.getElementById('<%=txtQty.ClientID%>').value = '';
        }
    </script>
    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
    <%-- FIN Barra de Estado --%>
</asp:Content>
