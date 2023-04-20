<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="LabelItemLotConsult.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelItemLotConsult" %>

<%@ Register Assembly="obout_Calendar2_Net" Namespace="OboutInc.Calendar2" TagPrefix="obout" %>
        
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

     $(document).ready(function () {
         initializeGridDragAndDrop("ItemUomLot_FindAll", "ctl00_MainContent_grdMgr");

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
         initializeGridDragAndDrop("ItemUomLot_FindAll", "ctl00_MainContent_grdMgr");
     }
</script>
   
   
    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
        <ContentTemplate>
          
                <div ID="divPrintLabel" runat="server" class="divPrintLabel" visible="false">
                    <div class="divCtrsFloatLeft">
                        <div id="div1" runat="server" class="divControls">
                            <div class="fieldRight">
                                <asp:Label ID="lblPrinter" runat="server" Text="Impresora" />
                            </div>
                            <div class="fieldLeft">
                            <asp:UpdatePanel ID="updPrinter" runat="server">
                            <ContentTemplate>
                                <asp:DropDownList ID="ddlPrinters" runat="server" OnSelectedIndexChanged="ddlPrinters_Change" AutoPostBack="true"/>
                                <asp:DropDownList ID="ddlLabelSize" runat="server" AutoPostBack="true" />
                                <asp:RequiredFieldValidator ID="rfvLabelSize" runat="server" ControlToValidate="ddlLabelSize" InitialValue="" ValidationGroup="valPrint" Text=" * " ErrorMessage="Tipo Etiqueta es requerido" />
                            </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </div>           
                    </div>
                    <div class="divCtrsFloatLeft">
                        <%--<asp:ValidationSummary ID="valPrint" ValidationGroup="valPrint" runat="server" CssClass="modalValidation"/>--%>
                        <asp:Label ID="lblNotPrinter" runat="server" CssClass="modalValidation" Text="No existen impresoras asociadas al usuario." ></asp:Label>                              
                    </div> 
                </div>
                
                <%-- Grilla Principal --%>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div id="divGrid" runat="server"  >                
                        <asp:GridView ID="grdMgr" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                                OnRowEditing="grdMgr_RowEditing1" EnableViewState="False"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false"> 
                            <Columns>
                         
                                <asp:TemplateField HeaderText="Seleccions">
                                    <ItemTemplate>                                
                                        <asp:CheckBox ID="chkSeleccion"  Checked='<%# Eval ( "Selected" ) %>'  AutoPostBack="True" runat="server" OnCheckedChanged="chkSeleccion_CheckedChanged" ToolTip="Selección Imprimir"/>  
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Uom" AccessibleHeaderText="IdUom">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdUom" runat="server" Text='<%# Eval ( "IdUom" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdItem" runat="server" Text='<%# Eval ( "IdItem" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField> 
                                <asp:TemplateField HeaderText="UOM Code" AccessibleHeaderText="UomCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblUomCode" runat="server" Text='<%# Eval("UomCode") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Conversion Factor" AccessibleHeaderText="ConversionFactor" SortExpression="ConversionFactor">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("ConversionFactor") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Codigo de Barra" AccessibleHeaderText="BarCode" SortExpression="BarCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("BarCode") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Nombre UOM" AccessibleHeaderText="UomName" SortExpression="UomName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblUomName" runat="server" Text='<%# Eval("UomName") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Largo" AccessibleHeaderText="Length" SortExpression="Length">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLength1" runat="server" Text='<%# Eval("Length") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ancho" AccessibleHeaderText="Width" SortExpression="Width">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblWidth" runat="server" Text='<%# Eval("Width") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Alto" AccessibleHeaderText="Height" SortExpression="Height">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblHeight" runat="server" Text='<%# Eval("Height") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="Volume" SortExpression="Volume">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblVolume" runat="server" Text='<%# Eval("Volume") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="Weight" SortExpression="Weight">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblWeight" runat="server" Text='<%# Eval("Weight") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="Status" SortExpression="Status">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="LayoutUomQty" AccessibleHeaderText="LayoutUomQty" SortExpression="LayoutUomQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLayoutUomQtye" runat="server" Text='<%# Eval("LayoutUomQty") %>'></asp:Label>
                                            </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="LayoutUnitQty" AccessibleHeaderText="LayoutUnitQty" SortExpression="LayoutUomQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLayoutUnitQty" runat="server" Text='<%# Eval("LayoutUnitQty") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="LayoutMaxWeightUpon" AccessibleHeaderText="LayoutMaxWeightUpon" SortExpression="LayoutMaxWeightUpon">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">                                
                                            <asp:Label ID="lblLayoutMaxWeightUpon" runat="server" Text='<%# Eval("LayoutMaxWeightUpon") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Zona Alm." AccessibleHeaderText="PutawayZone" SortExpression="PutawayZone">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">                                
                                            <asp:Label ID="lblPutawayZone" runat="server" Text='<%# Eval("PutawayZone") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Area de Picking" AccessibleHeaderText="PickArea"  SortExpression="PickArea">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblPickArea" runat="server" Text='<%# Eval("PickArea") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="IdItem" AccessibleHeaderText="ItemIdItem" SortExpression="ItemIdItem">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemIdItem" runat="server" Text='<%# Eval("ItemIdItem") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cód Item" AccessibleHeaderText="ItemCode" SortExpression="ItemCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemCode1" runat="server" Text='<%# Eval("ItemCode") %>'></asp:Label>
                                            </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Descripcion" AccessibleHeaderText="Description" SortExpression="Description">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblDescriptione" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Own" AccessibleHeaderText="IdOwn" SortExpression="IdOwn">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval("IdOwn") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Nombre Comercial" AccessibleHeaderText="TradeName" SortExpression="TradeName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblTradeName" runat="server" Text='<%# Eval("TradeName") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnCode" SortExpression="OwnCode">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnCodee" runat="server" Text='<%# Eval("OwnCode") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName" SortExpression="OwnName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval("OwnName") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Nombre Corto Item" AccessibleHeaderText="ShortItemName" SortExpression="ShortItemName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblShortItemName" runat="server" Text='<%# Eval("ShortItemName") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="ItemStatus" SortExpression="ItemStatus">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemStatus" runat="server" Text='<%# Eval("ItemStatus") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Controla Serie Ent." AccessibleHeaderText="CtrlSerialInbound" SortExpression="CtrlSerialInbound">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCtrlSerialInbound" runat="server" Text='<%# Eval("CtrlSerialInbound") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Controla Seria Int." AccessibleHeaderText="CtrlSerialInternal" SortExpression="CtrlSerialInternal">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCtrlSerialInternal" runat="server" Text='<%# Eval("CtrlSerialInternal") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Controla Serie Sal." AccessibleHeaderText="CtrlSerialOutbound" SortExpression="CtrlSerialOutbound">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCtrlSerialOutbound" runat="server" Text='<%# Eval("CtrlSerialOutbound") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Controla Lote Ent." AccessibleHeaderText="LotControlInbound" SortExpression="LotControlInbound">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLotControlInbound" runat="server" Text='<%# Eval("LotControlInbound") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Controla Lote Int." AccessibleHeaderText="LotControlInternal" SortExpression="LotControlInternal">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLotControlInternal" runat="server" Text='<%# Eval("LotControlInternal") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Controla Lote Sal." AccessibleHeaderText="LotControlOutbound" SortExpression="LotControlOutbound">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblLotControlOutbound" runat="server" Text='<%# Eval("LotControlOutbound") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="ItemWeight" SortExpression="ItemWeight">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemWeight" runat="server" Text='<%# Eval("ItemWeight") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="ItemVolume" SortExpression="ItemVolume">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemVolume" runat="server" Text='<%# Eval("ItemVolume") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Largo" AccessibleHeaderText="ItemLength" SortExpression="ItemLength">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemLengthe" runat="server" Text='<%# Eval("ItemLength") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ancho" AccessibleHeaderText="ItemWidth" SortExpression="ItemWidth">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemWidth" runat="server" Text='<%# Eval("ItemWidth") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Altos" AccessibleHeaderText="ItemHeight" SortExpression="ItemHeight">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblItemHeight" runat="server" Text='<%# Eval("ItemHeight") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="NestedVolume" AccessibleHeaderText="NestedVolume" SortExpression="NestedVolume">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblNestedVolume" runat="server" Text='<%# Eval("NestedVolume") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id Zona Alm." AccessibleHeaderText="IdPutawayZone" SortExpression="IdPutawayZone">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblIdPutawayZone" runat="server" Text='<%# Eval("IdPutawayZone") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Punto ReOrden" AccessibleHeaderText="ReOrderPoint" SortExpression="ReOrderPoint">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblReOrderPoint" runat="server" Text='<%# Eval("ReOrderPoint") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cantidad ReOrden" AccessibleHeaderText="ReOrderQty" SortExpression="ReOrderQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblReOrderQty" runat="server" Text='<%# Eval("ReOrderQty") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cantidad Pallet" AccessibleHeaderText="PalletQty" SortExpression="ReOrderQty">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblPalletQty" runat="server" Text='<%# Eval("PalletQty") %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Lote"  AccessibleHeaderText="LoteNumero" SortExpression="ReOrderQty">
                                    <ItemTemplate>
                                            <asp:TextBox ID="txtLote" Visible='<%# Eval ( "UseLot" ) %>' Text='<%# Eval("LotNumber") %>' Width="80" MaxLength="20" runat="server" Enabled='<%# Eval ( "Selected" ) %>'  ></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvLotRequired" Enabled='<%# Eval ( "Selected" ) %>' runat="server" ControlToValidate="txtLote" ValidationGroup="valPrint" Text=" * " ErrorMessage="Nº de Copias es requerido." />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vencimiento" AccessibleHeaderText="VencimientoFecha" SortExpression="ReOrderQty">
                                    <ItemTemplate>
                                            <asp:TextBox ID="txtVencimiento" Visible='<%# Eval ( "UseLot" ) %>' Text='<%# DisplayDate(Eval("ExpirationDatePrint")) %>' Width="70" runat="server" Enabled="False"></asp:TextBox>
                                            <asp:ImageButton ID="imbVencimiento" runat="server" ImageUrl="~/WebResources/Images/Buttons/Filter/cal_date_picker.gif" Enabled='<%# Eval ( "Selected" ) %>'  />
                                            <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtVencimiento" Format="dd/MM/yyyy" PopupButtonID="imbVencimiento">
                                            </ajaxToolkit:CalendarExtender>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Copias" AccessibleHeaderText="CopiasNumero"  SortExpression="ReOrderQty">
                                    <ItemTemplate>
                                    <asp:TextBox ID="txtCopias" Text='<%#((Int32) Eval("copNumber")>0)? Eval("copNumber", "{0:0}"):"" %>' runat="server" MaxLength="3" Width="20" Enabled='<%# Eval ( "Selected" ) %>' ></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvQtycopies" Enabled='<%# Eval ( "Selected" ) %>' runat="server" ControlToValidate="txtCopias" ValidationGroup="valPrint" Text=" * " ErrorMessage="Nº de Copias es requerido." />
                                        <asp:RangeValidator ID="rvQtycopies" runat="server" ErrorMessage="El valor debe estar entre 1 y 100." MaximumValue="100" MinimumValue="1" Text=" *" ControlToValidate="txtCopias" ValidationGroup="valPrint" Enabled='<%# Eval ( "Selected" ) %>' Type="Integer"></asp:RangeValidator>
                                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbeQtycopies" runat="server" TargetControlID="txtCopias" FilterType="Numbers"></ajaxToolkit:FilteredTextBoxExtender>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Agregar" AccessibleHeaderText="Agregar" >
                                    <ItemTemplate>
                                        <center>
                                            <div style="width: 60px">
                                                <asp:ImageButton ID="btnEdit" Enabled='<%# Eval ( "Selected" ) %>' Visible='<%# Eval ( "Selected" ) %>' runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add_on.png"
                                                    CausesValidation="false" CommandName="Edit" ToolTip="Agregar"/>
                                            </div>
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
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
