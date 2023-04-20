<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelItemConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelItemConsult" %>
        
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
                        
                                    <asp:TemplateField HeaderText="Id" AccessibleHeaderText="Id">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdUom" runat="server" Text='<%# Bind("Id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                                
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Item.Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Item.Owner.TradeName" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                        
                                    <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdItem" runat="server" Text='<%# Bind("Item.Id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Cód. Item" SortExpression="ItemCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("Item.Code") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Descripción" SortExpression="Description">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Item.Description") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                      
                                    <%--<asp:TemplateField HeaderText="Presentación" SortExpression="Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUomName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Unid. Medida" SortExpression="UomName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUomTypeUomName" runat="server" Text='<%# Bind("UomType.Name") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Cód. de Barras" SortExpression="Barcode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBarcode" runat="server" Text='<%# Bind("Barcode") %>'></asp:Label>
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
