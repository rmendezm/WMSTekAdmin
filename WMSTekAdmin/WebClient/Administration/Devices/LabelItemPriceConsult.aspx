<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelItemPriceConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelItemPriceConsult" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content runat="server" ID="content1" ContentPlaceHolderID="MainContent">

<script type="text/javascript" language="javascript">
    window.onresize = resizeDivPrincipal;

    function resizeDivPrincipal() {
        //debugger;
        //var h = (document.body.clientHeight - 135) + "px";
        //var w = document.body.clientWidth + "px";
        //document.getElementById("ctl00_MainContent_divGrid").style.height = h;
        //document.getElementById("ctl00_MainContent_divGrid").style.width = w;
    }
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
                                <asp:DropDownList ID="ddlPrinters" runat="server" OnSelectedIndexChanged="ddlPrinters_Change" AutoPostBack="true"/>
                                <asp:DropDownList ID="ddlLabelSize" runat="server" AutoPostBack="true"/>
                                <asp:RequiredFieldValidator ID="rfvLabelSize" runat="server" ControlToValidate="ddlLabelSize" 
                                InitialValue="" ValidationGroup="valPrint" Text=" * " ErrorMessage="Tipo Etiqueta es requerido" />
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
                                                <asp:Label ID="lblIdCustomer" runat="server" Text='<%# Bind("Customer.Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="IdOwner" AccessibleHeaderText="IdOwner">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdOwner" runat="server" Text='<%# Bind("Owner.Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.TradeName" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Cod.Cliente" AccessibleHeaderText="CustomerCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Eval ( "Customer.Code" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                                        <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdItem" runat="server" Text='<%# Bind("Item.Id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód. Item Cliente" SortExpression="ItemCodeCustomer">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCodeCustomer" runat="server" Text='<%# Bind("ItemCodeCustomer") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nombre" SortExpression="LongItemName" AccessibleHeaderText="LongItemName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLongItemName" runat="server" Text='<%# Bind("LongItemName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód. Item" SortExpression="ItemCode" AccessibleHeaderText="ItemCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("Item.Code") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                        
                                        <asp:TemplateField HeaderText="Descripción" SortExpression="Description" AccessibleHeaderText="Description">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Item.Description") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                      
                                        <asp:TemplateField HeaderText="Precio" SortExpression="Price" AccessibleHeaderText="Price">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPrice" runat="server" Text='<%# Bind("Price") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                        
                                        <asp:TemplateField HeaderText="SpecialField1" SortExpression="SpecialField1" AccessibleHeaderText="SpecialField1">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField1" runat="server" Text='<%# Bind("SpecialField1") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SpecialField2" SortExpression="SpecialField2" AccessibleHeaderText="SpecialField2">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField2" runat="server" Text='<%# Bind("SpecialField2") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SpecialField3" SortExpression="SpecialField3" AccessibleHeaderText="SpecialField3">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField3" runat="server" Text='<%# Bind("SpecialField3") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SpecialField4" SortExpression="SpecialField4" AccessibleHeaderText="SpecialField4">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialField4" runat="server" Text='<%# Bind("SpecialField4") %>'></asp:Label>
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
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
