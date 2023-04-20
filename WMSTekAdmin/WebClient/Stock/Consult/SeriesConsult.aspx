<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SeriesConsult.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Stocks.SeriesConsult" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUcLook" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("Serial_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Serial_FindAll", "ctl00_MainContent_grdMgr");
    }

    function HideMessage() {
        document.getElementById("divFondoPopup").style.display = 'none';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'none';

        return false;
    }

    function ShowMessage(title, message) {
        var position = (document.body.clientWidth - 400) / 2 + "px";
        document.getElementById("divFondoPopup").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

        document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
        document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

        return false;
    }

    function showProgress() {
        if (document.getElementById('ctl00_MainContent_uploadFile').value.length > 0) {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modalLoading");
                $('body').append(modal);

                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 3.5, 0);
                var left = Math.max($(window).width() / 2.6, 0);
                loading.css({ top: top, left: left });
            }, 10);
            return true;

        } else {
            return false;
        }
    }

    function InboundOrderValidation(sender, args) {

        <%--var valueToValidate = document.getElementById("<%=txtInboundOrder.ClientID %>").value;--%>
        var valueToValidate = "";

        if (valueToValidate != '' && isNaN(valueToValidate)) {
            sender.innerHTML = "ID doc. entrada debe es numerica";
            args.IsValid = false;
        } else if (valueToValidate != '' && parseInt(valueToValidate) <= 0) {
            sender.innerHTML = "ID doc. entrada debe ser mayor a 0";
            args.IsValid = false;
        } else {
            args.IsValid = true;
        }
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
                                AllowSorting="False" 
                                OnRowCreated="grdMgr_RowCreated"
                                EnableViewState="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                OnRowDeleting="grdMgr_RowDeleting"
                                OnRowEditing="grdMgr_RowEditing"
                                OnRowCommand="grdMgr_RowCommand"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                    
                                 <Columns>
                                    <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                            <div style="word-wrap: break-word;">                                
                                        </itemtemplate>
                                     </asp:templatefield>
                                         
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                         
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:Label ID="lblOwner" runat="server" text='<%# Eval ( "Owner.TradeName" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:templatefield headertext="Serial" accessibleHeaderText="SerialNumber" SortExpression="SerialNumber">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblSerialNumber" runat="server" text='<%# Eval ("SerialNumber") %>' />
                                           </div>
                                        </itemtemplate>
                                    </asp:templatefield>         
                        
                                    <asp:templatefield headertext="Nº Doc Entrada" accessibleHeaderText="InboundNumber" SortExpression="InboundNumber">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblInboundNumber" runat="server" text='<%# Eval ("InboundOrder.Number") %>' />
                                            </div>
                                        </itemtemplate>
                                    </asp:templatefield>             
                        
                                    <asp:templatefield headertext="N° Doc Salida" accessibleHeaderText="OutboundNumber" SortExpression="Fifo">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblOutboundNumber" runat="server" text='<%# Eval ("OutboundOrder.Number") %>' />
                                            </div>
                                        </itemtemplate>
                                    </asp:templatefield>                                                       
                                                                
                                    <asp:templatefield headertext="Cód. Item" accessibleHeaderText="ItemCode" SortExpression="ItemCode" ItemStyle-CssClass="text">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                            </div>
                                        </itemtemplate>
                                    </asp:templatefield>
            
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="ShortName" SortExpression="ShortName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:Label ID="lblShortItemName" runat="server" Text='<%# Eval ("Item.ShortName") %>' />
                                             </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>                             

                                    <asp:templatefield headertext="Descripción" accessibleHeaderText="ItemDescription" SortExpression="ItemDescription">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblLongItemName" runat="server" text='<%# Eval ("Item.LongName") %>' />
                                           </div>
                                        </itemtemplate>
                                    </asp:templatefield>   
                                    

                                    <asp:templatefield headertext="LPN" accessibleHeaderText="lpnCode" SortExpression="lpnCode">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblLpnCode" runat="server" text='<%# Eval ("Stock.Lpn.Code") %>' />
                                           </div>
                                        </itemtemplate>
                                    </asp:templatefield>   

                                     <asp:templatefield headertext="Ubicación" accessibleHeaderText="IdLocCode" SortExpression="IdLocCode" Visible="false">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblLocationCode" runat="server" text='<%# Eval ("Stock.Location.Code") %>' />
                                           </div>
                                        </itemtemplate>
                                    </asp:templatefield>
                                     
                                    <asp:templatefield headertext="Qty" accessibleHeaderText="Qty" SortExpression="Qty" Visible="false">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">                                
                                               <asp:label ID="lblQty" runat="server" text='<%# GetFormatedNumber(((decimal) Eval ("Stock.Qty")== -1) ? " " : Eval("Stock.Qty"))%>' />
                                           </div>
                                        </itemtemplate>
                                    </asp:templatefield>
                                                               
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnCreate" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                        CausesValidation="false" CommandName="Create" CommandArgument="<%# Container.DataItemIndex %>" />
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                         
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
    

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlSerial" BackgroundCssClass="modalBackground" PopupDragHandleControlID="EditCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlSerial" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="EditCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Serial" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Serial" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidStockId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIdOwn" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIdWhs" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">

                             <%-- Serial Number --%>
                            <div id="divSerialNumber" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSerialNumber" runat="server" Text="N° Serial" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSerialNumber" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtSerialNumber"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="N° Serial es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtSerialNumber"
	                                     ErrorMessage="N° Serial permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>

                            <%-- ItemCode --%>
                            <div id="divItemCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblItemCode" runat="server" Text="Cód. Item" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtItemCode" runat="server" Enabled="false"  />
                                    <%--<asp:ImageButton ID="imgbtnSearchItem" runat="server" 
                                        Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                        OnClick="imgBtnSearchItem_Click" 
                                        ToolTip="Buscar Item" Width="18px"  />--%>
                                    <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="txtItemCode" ValidationGroup="EditNew" Text=" * " ErrorMessage="Código Item es requerido" />
                                </div>         
                            </div>  
                            
                            <div id="divItemDesc" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblItemNameTitle" runat="server" Text="Descripcion Item" Visible="false" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:Label ID="lblItemName" runat="server" />
                                </div>
                            </div>

                            <%-- InboundOrder --%>
                            <%--<div id="divInboundOrder" runat="server" class="divControls">
	                            <div class="fieldRight">
		                            <asp:Label ID="lblInboundOrder" runat="server" Text="Orden Entrada" /></div>
	                            <div class="fieldLeft">
		                            <asp:TextBox ID="txtInboundOrder" runat="server" MaxLength="20" Width="150" />
                                    <asp:CustomValidator runat="server" Display="Dynamic" ID="customValInboundOrder" ClientValidationFunction="InboundOrderValidation" Text=" * "  ErrorMessage="" ControlToValidate="txtInboundOrder" ValidationGroup="EditNew"> </asp:CustomValidator>  
	                            </div>
                            </div>--%>

                        </div>

                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>          

                    </div>
                </asp:Panel>
            </div>

            <%-- Lookup Items --%>
            <div id="divLookupItem" runat="server" visible="false">
                <asp:Button ID="Button1" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupItem" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlLookupItem" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupItem"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLookupItem" runat="server" CssClass="modalBox">
                    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddItem" runat="server" Text="Buscar Item" />
                            <asp:ImageButton ID="imgBtnCloseItemSearch" runat="server" ImageAlign="Top" CssClass="closeButton"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
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
                        <webUcLook:ucLookUpFilter ID="ucFilterItem" runat="server" />
                        <div class="divCtrsFloatLeft">
                            <div class="divLookupGrid">
                                <asp:GridView ID="grdSearchItems" runat="server" DataKeyNames="Id" OnRowCommand="grdSearchItems_RowCommand" AllowPaging="true"
                                    onrowdatabound="grdSearchItems_RowDataBound"
                                    AutoGenerateColumns="False"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
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
                                                    <asp:ImageButton ID="imgBtnAddItem" ToolTip="Agregar Proveedor" runat="server" Height="20px" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png"
                                                        Width="20px" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div style="clear: both" />
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Lookup Items --%>


        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <%-- Carga masiva de items --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Series" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Serial.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                       
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" OnClientClick="showProgress()" onclick="btnSubir_Click" />
                                </div>
                            </div>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" 
     DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />
    
    <div id="divFondoPopupProgress" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;" runat="server">
        <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadNew" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
    </div>

    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>
               
    </div>    

    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label id="lblFilterDate" runat="server" Text="Fifo" Visible="false" />   
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta serie?" Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Serieales" Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>    
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>      
</asp:Content>
