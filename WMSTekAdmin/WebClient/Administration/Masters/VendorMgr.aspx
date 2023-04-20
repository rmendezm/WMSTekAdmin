<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="VendorMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.VendorMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("Vendor_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("Vendor_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

     <style>
        #ctl00_MainContent_divModalFields select  {
            width: 130px !important;
        }
    </style>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <%-- Grilla Principal --%>
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" OnRowEditing="grdMgr_RowEditing" OnPageIndexChanging="grdMgr_PageIndexChanging"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AllowPaging="True" EnableViewState="false"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:BoundField DataField="Address1" HeaderText="Dirección 1" AccessibleHeaderText="Address1" />

                                    <asp:BoundField  DataField="Address2" HeaderText="Dirección 2" AccessibleHeaderText="Address2" />
                                    
                                    <asp:TemplateField HeaderText="País" AccessibleHeaderText="Country">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCountry" runat="server" Text='<%# Eval ( "Country.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Región" AccessibleHeaderText="State">
                                        <ItemTemplate>
                                            <asp:Label ID="lblState" runat="server" Text='<%# Eval ( "State.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comuna" AccessibleHeaderText="City">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCity" runat="server" Text='<%# Eval ( "City.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Phone" HeaderText="Teléfono" AccessibleHeaderText="Phone" />
                                    <asp:BoundField DataField="Fax" HeaderText="Fax" AccessibleHeaderText="Fax" />
                                    <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />
                                    <asp:TemplateField HeaderText="Inspección" AccessibleHeaderText="HasInspection">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkHasInspection" runat="server" Checked='<%# Eval ( "HasInspection" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
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
                     <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                  </Triggers>
                </asp:UpdatePanel>  
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
    <%-- FIN Grilla Principal --%>
       
     <%-- Pop up Editar/Nuevo Proveedor --%>             
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="VendorCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlVendor" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="VendorCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">                    
                           <asp:Label ID="lblNew" runat="server" Text="Nuevo Proveedor" />
                           <asp:Label ID="lblEdit" runat="server" Text="Editar Proveedor" />
                           <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                            
                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlOwner" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />                                    
                                </div>
                            </div>
                                       
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />     
                                </div>
                            </div>
                                                                                
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" Width="250" style="width: 250px !important"/>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div id="divAddress1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblAddress1" runat="server" Text="Dirección 1" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtAddress1" runat="server" MaxLength="60" Width="250" style="width: 250px !important"/>
                                    <%--<asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Dirección 1 es requerido">
                                    </asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div id="divAddress2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblAddress2" runat="server" Text="Dirección 2" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtAddress2" runat="server" MaxLength="60" Width="150" style="width: 250px !important"/>
                                    <%--<asp:RequiredFieldValidator ID="rfvAddress2" runat="server" ControlToValidate="txtAddress2"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Dirección 2 es requerido">
                                    </asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div id="divCountry" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCountry" runat="server" Text="País"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" />
                                    <%--<asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="País es requerido" InitialValue="-1">
                                    </asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div id="divState" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblState" runat="server" Text="Región"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlState" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlState_SelectedIndexChanged" />
                                    <%--<asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Región es requerida" InitialValue="-1">
                                    </asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div id="divCity" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCity" runat="server" Text="Comuna"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlCity" runat="server" />
                                    <%--<asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="ddlCity"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Ciudad es requerida" InitialValue="-1">
                                    </asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div id="divPhone" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPhone" runat="server" Text="Teléfono" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="20" Width="150" />
                                    <%--<asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Teléfono es requerido">
                                    </asp:RequiredFieldValidator>--%>
                                    <asp:RegularExpressionValidator ID="revtxtPhone" runat="server" ControlToValidate="txtPhone"
                                        ErrorMessage="Teléfono permite ingresar solo números" 
	                                    ValidationExpression="[0-99999999999]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>   
                                </div>
                            </div>
                            <div id="divFax" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFax" runat="server" Text="Fax" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFax" runat="server" MaxLength="20" Width="150" />
                                   <%-- <asp:RequiredFieldValidator ID="rfvFax" runat="server" ControlToValidate="txtFax"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Fax es requerido">
                                    </asp:RequiredFieldValidator>--%>
                                    <asp:RegularExpressionValidator ID="revtxtFax" runat="server" ControlToValidate="txtFax"
                                        ErrorMessage="Fax permite ingresar solo números" 
	                                    ValidationExpression="[0-99999999999]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>   
                                </div>
                            </div>
                            <div id="divEmail" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblEmail" runat="server" Text="E-mail" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="150" style="width: 250px !important"/>
                                    <asp:RegularExpressionValidator ID="revMail" runat="server" ControlToValidate="txtEmail"
                                        ErrorMessage="Email Inválido" ValidationGroup="EditNew" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*
                                    </asp:RegularExpressionValidator>
                                    <%--<asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="E-mail es requerido">
                                    </asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div id="divHasInspection" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHasInspection" runat="server" Text="Inspección" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkHasInspection" runat="server" /></div>
                            </div>
                        </div>
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>   
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
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

    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Pop up Editar/Nuevo Proveedor --%>

    <%-- Carga masiva de Proveedor --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Cliente --%>
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
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Proveedores" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Proveedor.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div9" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label4" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwnerLoad" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwnerLoad" runat="server" ControlToValidate="ddlOwnerLoad"
                                        InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile2" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile2"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile2">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir2_Click" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div10" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Proveedor?" Visible="false" />
    <asp:Label ID="lblMessajeDeleteError" runat="server" Text="No es posible eliminar el proveedor ya que posee documentos de entrada asociados." Visible="false" />
    <asp:Label ID="lblTitleDeleteError" runat="server" Text="No es posible eliminar el Proveedor" Visible="false" />
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Proveedores" Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen proveedores en el archivo." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblGln" runat="server" Text="GLN" Visible="false" />
   <%-- <asp:Label ID="lblGlnIsNotNumeric" runat="server" Text="Código GLN debe ser Numérico." Visible="false" />
    <asp:Label ID="lblGlnLengthInvalid" runat="server" Text="Largo del Código GLN debe ser 13." Visible="false" />
    <asp:Label ID="lblGlnCheckDigit" runat="server" Text="Dígito Verificador del Código GLN no es Válido." Visible="false" />--%>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
