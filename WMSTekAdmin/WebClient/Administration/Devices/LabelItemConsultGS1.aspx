<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelItemConsultGS1.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelItemConsultGS1" %>
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

<style>        
    .froze-header-grid {
        max-height: 90%;
    }
</style>
        
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
               
                            <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >    

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

                                        <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text" Visible="false">
                                        <itemtemplate>
                                           <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>
                                         
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <itemtemplate>
                                           <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ubicacion" AccessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Location.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode" SortExpression="IdLpnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLpn" runat="server" Text='<%# Eval("Lpn.IdCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                        
                                    <asp:templatefield headertext="Categoría" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                        <itemtemplate>
                                               <asp:label ID="lblCategoryItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>         
                                                                
                                    <asp:templatefield headertext="Cód. Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                               <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>
            
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongName" SortExpression="LongName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemName" runat="server" Text='<%# Eval ("Item.LongName") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>        
                                                               
                                   <asp:templatefield headertext="Cant." accessibleHeaderText="Qty" SortExpression="Qty">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblQty" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("Qty") == -1)?" ":Eval ("Qty")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield headertext="Volumen" accessibleHeaderText="TotalVolumen" SortExpression="TotalVolumen">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblTotalVolumen" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("TotalVolumen") == -1)?" ":Eval ("TotalVolumen")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield headertext="Peso" accessibleHeaderText="TotalWeight" SortExpression="TotalWeight">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <right>
                                                <asp:label ID="lblTotalWeight" runat="server" 
                                                text='<%# GetFormatedNumber(((decimal) Eval ("TotalWeight") == -1)?" ":Eval ("TotalWeight")) %>' />
                                            </right>    
                                        </itemtemplate>
                                    </asp:templatefield>
                  
                         
                                     <asp:templatefield headertext="Lote" accessibleHeaderText="LotNumber" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <asp:Label ID="lblLotNumber" runat="server" text='<%# Eval ( "Lot" ) %>'></asp:Label>
                                        </itemtemplate>
                                        <ItemStyle CssClass="text" />
                                    </asp:templatefield>   
                                        
                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo" SortExpression="Fifo">
                                        <itemtemplate>
                                               <asp:label ID="lblFifo" runat="server" text='<%# Eval ("FifoDate") %>' />
                                        </itemtemplate>
                                    </asp:templatefield>   
                    
                                    <asp:templatefield headertext="Elaboración" accessibleHeaderText="FabricationDate" SortExpression="FabricationDate">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <asp:label ID="lblFabricationDate" runat="server"  
                                                text='<%# ((DateTime) Eval ("FabricationDate") > DateTime.MinValue)? Eval("FabricationDate", "{0:d}"):"" %>' />
                                            </center>    
                                        </itemtemplate>
                                    </asp:templatefield>    
                    
                                      <asp:templatefield headertext="Vencimiento" accessibleHeaderText="ExpirationDate" SortExpression="ExpirationDate">
                                        <itemtemplate>
                                               <asp:label ID="lblExpiration" runat="server"  text='<%# ((DateTime) Eval ("ExpirationDate") > DateTime.MinValue)? Eval("ExpirationDate", "{0:d}"):"" %>' />
                                        </itemtemplate>
                                    </asp:templatefield> 
                    
                                    
                            
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
    <asp:Label ID="lblErrorItem" runat="server" Text="No es posible generar impresión para el Item  " Visible="false"/>
    <asp:Label id="LabelmessageFind" runat="server" Text="Debe ingresar por lo menos un criterio de busqueda" Visible="false" />   
    <%-- FIN Mensajes de advertencia y auxiliares --%>

</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
