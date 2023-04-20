<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="ReferenceDocMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Outbound.Administration.ReferenceDocMgr" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("ReferenceDoc_FindAll", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();

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
            initializeGridDragAndDrop("ReferenceDoc_FindAll", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }

        function showProgress() {
            if (document.getElementById('ctl00_MainContent_uploadFile').value.length > 0) {
                setTimeout(function () {
                    var modal = $('<div />');
                    modal.addClass("modalLoading");
                    $('body').append(modal);
                    var loading = $(".loading");

                    var top = Math.max($(window).height() / 3.5, 0);
                    var left = Math.max($(window).width() / 2.6, 0);
                    loading.css({ top: top, left: left });
                    loading.show();
                }, 30);
                return true;

            } else {
                return false;
            }
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
    </script>

    <link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

     <div class="container">
        <div class="row">
            <div class="col-md-12">
                <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <asp:GridView ID="grdMgr" 
                            DataKeyNames="Id" 
                            runat="server" 
                            OnRowCreated="grdMgr_RowCreated"
                            OnRowDeleting="grdMgr_RowDeleting" 
                            OnRowEditing="grdMgr_RowEditing" 
                            OnPageIndexChanging="grdMgr_PageIndexChanging"
                            AllowPaging="True" 
                            OnRowDataBound="grdMgr_RowDataBound"
                            AutoGenerateColumns="false"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                            <Columns>

                                <asp:TemplateField HeaderText="N° Doc. referencia" AccessibleHeaderText="ReferenceDocNumber">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblReferenceDocNumber" runat="server" Text='<%# Eval ( "ReferenceDocNumber" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo Doc. referencia" AccessibleHeaderText="ReferenceDocTypeName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblReferenceDocTypeName" runat="server" Text='<%# Eval ( "ReferenceDocType.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Orden" AccessibleHeaderText="OutboundOrder">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOutboundOrder" runat="server" Text='<%# Eval ( "Order.Number" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo Orden" AccessibleHeaderText="OutboundOrderType">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOutboundOrderType" runat="server" Text='<%# Eval ( "OutboundType.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                 <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblOwner" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                 <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                 <asp:TemplateField HeaderText="Fecha Factura" AccessibleHeaderText="InvoiceDate">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblInvoiceDate" runat="server" text='<%# ((DateTime) Eval ("InvoiceDate") > DateTime.MinValue)? Eval("InvoiceDate", "{0:d}"):"" %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                    <ItemTemplate>
                                        <center>
                                            <div style="width: 60px">
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
    </div>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
             <%-- Pop up Editar/Nuevo Lpn --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlReferenceDoc" BackgroundCssClass="modalBackground" PopupDragHandleControlID="ReferenceDocCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlReferenceDoc" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="ReferenceDocCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Doc referencia" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Doc referencia" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />

                        <div class="divCtrsFloatLeft">

                                <div id="divReferenceDocNumber" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblReferenceDocNumber" runat="server" Text="N° Doc Referencia" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtReferenceDocNumber" runat="server" MaxLength="20" Width="150" />
                                        <asp:RequiredFieldValidator ID="rfvtxtReferenceDocNumber" runat="server" ControlToValidate="txtReferenceDocNumber"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="N° doc es requerido" />
                                        <asp:RegularExpressionValidator ID="revtxtReferenceDocNumber" runat="server" ControlToValidate="txtReferenceDocNumber"
	                                         ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                         ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                         ValidationGroup="EditNew" Text=" * ">
                                        </asp:RegularExpressionValidator>     
                                    </div>
                                </div>

                                <div id="divReferenceDocType" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="Label4" runat="server" Text="Tipo Doc Referencia" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlReferenceDocType" runat="server" Width="120px" />
                                        <asp:RequiredFieldValidator ID="rfvReferenceDocType" runat="server" ValidationGroup="EditNew" Text=" * " ControlToValidate="ddlReferenceDocType" Display="Dynamic" InitialValue="-1" ErrorMessage="Tipo Doc Referencia es requerido" />
                                    </div>
                                </div>

                                <div id="divInvoiceDate" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblInvoiceDate" runat="server" Text="Fecha Factura" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox SkinID="txtFilter" ID="txtInvoiceDate" runat="server" Width="80px" ValidationGroup="EditNew" ToolTip="Ingrese fecha." />
                                        <asp:RequiredFieldValidator ID="rfvDateAsn" runat="server" ControlToValidate="txtInvoiceDate" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fecha es requerida"  />
                                
                                        <asp:RangeValidator ID="rvInvoiceDate" runat="server" ControlToValidate="txtInvoiceDate"
                                            ErrorMessage="Fecha debe estar entre 01-01-2000 y 31-12-2040" Text=" * " MinimumValue="01/01/2000"
                                            MaximumValue="31/12/2040" ValidationGroup="EditNew" Type="Date" />
                                        <ajaxToolkit:CalendarExtender ID="calInvoiceDate" CssClass="CalMaster" runat="server" 
                                            Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtInvoiceDate" PopupButtonID="txtInvoiceDate"
                                            Format="dd/MM/yyyy">
                                        </ajaxToolkit:CalendarExtender>
                                    </div>
                                 </div>


                                <div id="divOwn" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="Label1" runat="server" Text="Dueño" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlOwner" runat="server" Width="120px" />
                                        <asp:RequiredFieldValidator ID="rfvIdOwner" runat="server" ValidationGroup="EditNew" Text=" * " ControlToValidate="ddlOwner" Display="Dynamic" InitialValue="-1" ErrorMessage="Dueño es requerido" />
                                    </div>
                                </div>

                                <div id="divWhs" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="Label2" runat="server" Text="Centro Dist." />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlWhs" runat="server" Width="120px" />
                                        <asp:RequiredFieldValidator ID="rfvIdWhs" runat="server" ValidationGroup="EditNew" Text=" * " ControlToValidate="ddlWhs" Display="Dynamic" InitialValue="-1" ErrorMessage="Bodega es requerida" />
                                    </div>
                                </div>

                                <div id="divOutboundOrder" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="Label3" runat="server" Text="Orden" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:TextBox ID="txtOutboundOrder" runat="server" MaxLength="20" Width="150" />
                                        <asp:RequiredFieldValidator ID="rfvtxtOutboundOrder" runat="server" ControlToValidate="txtOutboundOrder"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Orden es requerida" />
                                       <%-- <asp:RegularExpressionValidator ID="revtxtOutboundOrder" runat="server" ControlToValidate="txtOutboundOrder"
	                                         ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                         ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                         ValidationGroup="EditNew" Text=" * ">
                                        </asp:RegularExpressionValidator> --%>    
                                    </div>
                                </div>

                                <div id="divOutboundType" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="Label5" runat="server" Text="Tipo Orden" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:DropDownList ID="ddlDocType" runat="server" Width="120px" />
                                        <asp:RequiredFieldValidator ID="rqDocType" runat="server" ValidationGroup="EditNew" Text=" * " ControlToValidate="ddlDocType" Display="Dynamic" InitialValue="-1" ErrorMessage="Tipo Doc es requerido" />
                                    </div>
                                </div>
                        </div>

                        <div style="clear: both"></div>
			            <div class="divValidationSummary" >
				            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false" CssClass="modalValidation"/>
			            </div>
			            <div id="divActions" runat="server" class="modalActions">
				            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
				            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
			            </div>          

                    </div>  
                </asp:Panel>
             </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <%-- Carga masiva de documentos --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" 
                    TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" 
                    BackgroundCssClass="modalBackground" 
                    PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label6" runat="server" Text="Carga Masiva de Documentos" />
                             <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Docs%20Ref.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />                                
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">                       
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label7" runat="server" Text="Seleccione Archivo" />
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
                                    <asp:Label ID="Label8" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnLoadFile" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                     onclick="btnLoadFile_Click" OnClientClick="showProgress();" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div8" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Load --%>            

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnLoadFile"/>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />            
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad">
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressGrid" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
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

    <%-- Mensaje--%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este documento de referencia?" Visible="false" />
    <asp:Label id="lblFilterDate" runat="server" Text="Factura" Visible="false" /> 
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es valído." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Docs. Referencia" Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblInvalid" runat="server" Text="inválido" Visible="false" />
     <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
