<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="DriverMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.DriverMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("Driver_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Driver_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" 
                                DataKeyNames="Id" 
                                runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                OnRowDataBound="grdMgr_RowDataBound"
                                AllowPaging="True" 
                                EnableViewState="false"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Phone" HeaderText="Teléfono" AccessibleHeaderText="Phone" />
                                    <asp:BoundField DataField="MovilePhone" HeaderText="Celular" AccessibleHeaderText="MovilePhone" />
                                    <asp:BoundField DataField="Fax" HeaderText="Fax" AccessibleHeaderText="Fax" />
                                    <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />
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
                        <%-- FIN Grilla Principal --%>
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
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />
    <%-- FIN Modal Update Progress --%>
    
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Driver --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlDriver" BackgroundCssClass="modalBackground" PopupDragHandleControlID="DriverCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlDriver" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="DriverCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Chofer" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Chofer" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                        
                            <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" /></div>
                            </div>  
                                                    
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                    <asp:RegularExpressionValidator ID="revCode" runat="server" ControlToValidate="txtCode"
	                                    ErrorMessage="Código permite solo letras de la A - Z o a - z" 
	                                    ValidationExpression="[a-zA-Z 0-9999999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                        
                                </div>
                            </div>
                                                    
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido">
                                    </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revName" runat="server" ControlToValidate="txtName"
	                                ErrorMessage="Nombre permite solo letras de la A - Z o a - z" 
	                                ValidationExpression="[a-zA-Z ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
	                                ValidationGroup="EditNew" Text=" * ">
                                </asp:RegularExpressionValidator>                                    
                                </div>
                            </div>
                      
                            <div id="divPhone" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPhone" runat="server" Text="Teléfono" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Teléfono es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtPhone"
	                                    ErrorMessage="Teléfono permite solo números" 
	                                    ValidationExpression="[0-9 ]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                    
                                </div>
                            </div>
                            <div id="divMovil" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMovil" runat="server" Text="Celular" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtMovilePhone" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvMovil" runat="server" ControlToValidate="txtMovilePhone"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Móvil es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revMovil" runat="server" ControlToValidate="txtMovilePhone"
                                        ErrorMessage="Celular permite solo números" 
                                        ValidationExpression="[0-9 ]*"
                                        ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                    
                                </div>
                            </div>                        
                            <div id="divFax" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFax" runat="server" Text="Fax" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFax" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvFax" runat="server" ControlToValidate="txtFax"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Fax es requerido" >
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revFax" runat="server" ControlToValidate="txtFax"
                                        ErrorMessage="Fax permite solo números" 
                                        ValidationExpression="[ 0-999999999-]*"
                                        ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                     
                                </div>
                            </div>
                            <div id="divEmail" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblEmail" runat="server" Text="E-mail" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="150" />
                                    <asp:RegularExpressionValidator ID="revMail" runat="server" ControlToValidate="txtEmail"
                                        ErrorMessage="E-mail Inválido" ValidationGroup="EditNew" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*
                                    </asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="E-mail es requerido">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="divValidationSummary">
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
            <%-- Pop up Editar/Nuevo Driver --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew"
        DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprEditNew" />
    <%-- FIN Modal Update Progress --%>
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Chofer?" Visible="false" />

    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>