<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelBulkConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelBulkConsult" %>
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

    
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
                <div ID="divPrintLabel" runat="server" class="divPrintLabel" visible="false">
               
                    <div class="divCtrsFloatLeft">
                        <div id="divCodStatus" runat="server" class="divControls">
                            <div class="fieldRight">
                                <asp:Label ID="lblCopies" runat="server" Text="Nº de Copias" /></div>
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
                                <asp:DropDownList ID="ddlPrinters" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPrinters_Change" />
                                <asp:DropDownList ID="ddlLabelSize" runat="server" AutoPostBack="true" />
                                <asp:RequiredFieldValidator ID="rfvLabelSize" runat="server" ControlToValidate="ddlLabelSize" InitialValue="" ValidationGroup="valPrint" Text=" * " ErrorMessage="Tipo Etiqueta es requerido" />
                            </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </div>           
                    </div>
                    
                    <div class="divCtrsFloatLeft">
                        <asp:ValidationSummary ID="valPrint" ValidationGroup="valPrint" runat="server" CssClass="modalValidation"/>
                        <asp:Label ID="lblNotPrinter" runat="server" CssClass="modalValidation" Text="No existen impresoras asociadas al usuario." ></asp:Label>                              
                    </div>   
                    
                </div>   
                            
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">   
                            <%-- Grilla Principal --%>     
                            <div id="divGrid" runat="server" class="divGrid" onresize="resizeDivPrincipal();" >                
                                <asp:GridView ID="grdMgr" runat="server" AutoGenerateColumns="False"
                                        EnableViewState="False"
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

                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Customer.Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Customer.Owner.TradeName" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                            
                                    <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
              
                                        <asp:TemplateField HeaderText="Cód. Cliente" AccessibleHeaderText="CustomerCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerCode" runat="server" Text='<%# Bind("Customer.Code") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>  

                                        <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Bind("Customer.Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>  

                                        <asp:TemplateField HeaderText="Dirección Entrega" AccessibleHeaderText="Address1Delv">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDeliveryAddress1" runat="server" Text='<%# Bind("Customer.Address1Delv") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>      

                                        <asp:TemplateField HeaderText="Ciudad Entrega" AccessibleHeaderText="CityName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCityName" runat="server" Text='<%# Bind("Customer.CityDelv.Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>   

                                        <asp:TemplateField HeaderText="Teléfono Entrega" AccessibleHeaderText="DeliveryPhone">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDeliveryPhone" runat="server" Text='<%# Bind("Customer.PhoneDelv") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>  

                                        <asp:TemplateField HeaderText="Nº Doc. Salida" SortExpression="OutboundNumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutboundNumber" runat="server" Text='<%# Bind("OutboundNumber") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>   
                            
                                        <asp:TemplateField HeaderText="Lpn" SortExpression="IdLpnCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Bind("IdLpnCode") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>   

                                        <asp:TemplateField HeaderText="Peso Total" SortExpression="WeightTotal">
                                            <ItemTemplate>
                                                <asp:Label ID="lblWeightTotal" runat="server" Text='<%# Bind("WeightTotal") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>   
                                                                                    
                                         <asp:TemplateField HeaderText="LPN Mixto" SortExpression="CountItems">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkCountItems" runat="server" Checked='<%# (int)(Eval ( "CountItems" )) > 1 ? true : false  %>'  Enabled="false" />
                                                </center>
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
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnPrint" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>       
            
    
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    

    <%-- Mensajes de advertencia y auxiliares --%>
    <asp:Label ID="lblFilterCodeLabel" runat="server" Text="Lpn" Visible="false" />
    <asp:Label ID="lblFilterOutboundNumber" runat="server" Text="Documento Salida" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Impresión de Etiquetas" Visible="false"/>
    <asp:Label id="lblNoRowsSelected" runat="server" Text="Debe seleccionar al menos un elemento para imprimir." Visible="false" />    
    <asp:Label ID="lblRangeQtyCopy" runat="server" Text="Nº de Copias el valor debe estar entre " Visible="false"/>
    <asp:Label ID="lblErrorLpn" runat="server" Text="No es posible generar impresión para el lpn  " Visible="false"/>
       
    <%-- FIN Mensajes de advertencia y auxiliares --%>

</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
