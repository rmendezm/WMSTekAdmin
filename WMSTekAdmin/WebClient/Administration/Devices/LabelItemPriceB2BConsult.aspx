<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelItemPriceB2BConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelItemPriceB2BConsult" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content runat="server" ID="content1" ContentPlaceHolderID="MainContent">

<script type="text/javascript">

</script>         
    
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div ID="divPrintLabel" runat="server" class="divPrintLabel" visible="false">
               
                <div class="divCtrsFloatLeft">
                    <div id="divCodStatus" runat="server" class="divControls">
                        <div class="fieldRight">
                            <asp:Label ID="lblCopies" runat="server" Text="Nº de Copias" />
                        </div>
                        <div class="fieldLeft">
                            <asp:TextBox ID="txtQtycopies" runat="server" Width="30"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvQtycopies" runat="server" ControlToValidate="txtQtycopies" ValidationGroup="valPrint" Text=" * " ErrorMessage="Nº de Copias es requerido." />
                            <asp:RangeValidator ID="rvQtycopies" runat="server" ErrorMessage="El valor debe estar entre 1 y 100." MaximumValue="100" MinimumValue="1" Text=" *" ControlToValidate="txtQtycopies" ValidationGroup="valPrint" Type="Integer"></asp:RangeValidator>
                            <ajaxToolkit:FilteredTextBoxExtender ID="ftbeQtycopies" runat="server" TargetControlID="txtQtycopies" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
                        </div>
                    </div> 
                        
                <div id="div1" runat="server" class="divControls">
                    <div class="fieldRight">
                        <asp:Label ID="lblPrinter" runat="server" Text="Impresora" />
                    </div>
                    <div class="fieldLeft">
                        <asp:UpdatePanel ID="updPrinter" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="ddlPrinters" runat="server" AutoPostBack="true"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>           
            </div>
                    
            <div class="divValidationSummary">
                <asp:Label ID="lblNotPrinter" runat="server" CssClass="modalValidation" Text="No existen impresoras asociadas al usuario." ></asp:Label> 
                <asp:ValidationSummary ID="valPrint" ValidationGroup="valPrint" runat="server" CssClass="modalValidation" /> 
                                                                      
            </div>   
                    
        </div>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">   
                        <div id="divGrid" runat="server" class="divGrid" >
                                <%-- Grilla Principal --%>
                                <asp:GridView ID="grdMgr" runat="server" AutoGenerateColumns="False" AllowPaging="True" EnableViewState="False" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false"> 
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="Seleccions">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSeleccion', this.checked)" id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>                              
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 20px">
                                                        <asp:CheckBox ID="chkSeleccion" runat="server" ToolTip="Selección Imprimir"/>  
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField DataField="Id" HeaderText="ID" AccessibleHeaderText="Id" />--%>
                        
                                        <asp:TemplateField HeaderText="IdCustomer" AccessibleHeaderText="IdCustomer">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdCustomer" runat="server" Text='<%# Bind("ItemCustomer.Customer.Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="IdOwner" AccessibleHeaderText="IdOwner">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdOwner" runat="server" Text='<%# Bind("ItemCustomer.Owner.Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "ItemCustomer.Owner.Code" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "ItemCustomer.Owner.TradeName" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Cod.Cliente" AccessibleHeaderText="CustomerCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "ItemCustomer.Customer.Code" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "ItemCustomer.Customer.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# Bind("ItemCustomer.Item.Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód. Item Cliente" SortExpression="ItemCodeCustomer">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCodeCustomer" runat="server" Text='<%# Bind("ItemCustomer.ItemCodeCustomer") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nombre" SortExpression="LongItemName" AccessibleHeaderText="LongItemName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Bind("ItemCustomer.LongItemName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód. Item" SortExpression="ItemCode" AccessibleHeaderText="ItemCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCustomer.Item.Code") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                        
                                        <asp:TemplateField HeaderText="Descripción" SortExpression="Description" AccessibleHeaderText="Description">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("ItemCustomer.Item.Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                      
                                        <asp:TemplateField HeaderText="Precio" SortExpression="Price" AccessibleHeaderText="Price">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPrice" runat="server" Text='<%# Bind("ItemCustomer.Price") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                        
                                        <asp:TemplateField HeaderText="N° Documento" SortExpression="OutboundNumber" AccessibleHeaderText="OutboundNumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Bind("OutboundNumber") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ord. Compra" SortExpression="ReferenceNumber" AccessibleHeaderText="ReferenceNumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lblReferenceNumber" runat="server" Text='<%# Bind("ReferenceNumber") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cantidad" SortExpression="RealQty" AccessibleHeaderText="RealQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRealQty" runat="server" Text='<%# Bind("RealQty") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Etiqueta" SortExpression="LabelCodePrice" AccessibleHeaderText="LabelCodePrice">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLabelCodePrice" runat="server" Text='<%# Bind("LabelCodePrice") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>                        
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                <%-- FIN Grilla Principal --%>    
                            </div> 
                        </div>
                    </div>
                </div>   
        </ContentTemplate>
    </asp:UpdatePanel>   
            
    
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    


    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label ID="lblTitle" runat="server" Text="Impresión de Etiquetas" Visible="false"/>
    <asp:Label id="lblNoRowsSelected" runat="server" Text="Debe seleccionar al menos un elemento para imprimir." Visible="false" />
    <asp:Label ID="lblRangeQtyCopy" runat="server" Text="Nº de Copias el valor debe estar entre " Visible="false"/>    
    <asp:Label id="lblFilterReferenceNumber" runat="server" Text="Ord. Compra" Visible="false" /> 
    <asp:Label id="lblErrorLabelPrice" runat="server" Text="Cliente [CUSTOMERCODE] sin configuración de etiqueta de precio." Visible="false" /> 
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
