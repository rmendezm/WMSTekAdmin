<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="RoleMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Users.RoleMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        initializeGridDragAndDrop("Role_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Role_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop();
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
                                runat="server" 
                                DataKeyNames="Id" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True" 
                                EnableViewState="false"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false"
                            >
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                    SortExpression="Id" AccessibleHeaderText="Id" />
                                <asp:BoundField DataField="IsBaseRole" Visible="false" />
                                <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                <asp:TemplateField HeaderText="Modulo" AccessibleHeaderText="NameModule">
                                    <ItemTemplate>
                                        <center>
                                            <asp:Label ID="lblNameModule" runat="server" Text='<%# Eval ( "RoleModule.Module.Name" ) %>' />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="CodStatus">
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "CodStatus" ) %>'
                                                Enabled="false" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                    <ItemTemplate>
                                        <div style="width: 60px">
                                            <center>
                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                    onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_edit_on.png';"
                                                    onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_edit.png';"
                                                    CausesValidation="false" CommandName="Edit" />
                                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                    onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_delete_on.png';"
                                                    onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_delete.png';"
                                                    CausesValidation="false" CommandName="Delete" />
                                            </center>
                                        </div>
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
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
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
            <%-- Pop up Editar/Nuevo Rol --%>
            <div id="divEditNew" runat="server" visible="false">            
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnl" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnl" runat="server" CssClass="modalBox">
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Rol" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Rol" />
                            <asp:Label ID="lblView" runat="server" Text="Detalles del Rol"/>
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>                    
                    </asp:Panel>
                    <div class="modalControls" >
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidEditIdRoleModule" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIsBaseRole" runat="server" Value="false" />
                        <div class="divCtrsFloatLeft">
                            <div id="divCodStatus" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblCodStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft"><asp:CheckBox ID="chkCodStatus" runat="server" /></div>
                            </div>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="100" />
                                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" /></div>
                            </div>
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtDescription" runat="server" MaxLength="200" Width="150" />
                                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" /></div>
                            </div>
                            
                            <div id="divRolModule" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblRolModule" runat="server" Text="Modulo" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlRolModule" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvRolModule" runat="server" ControlToValidate="ddlRolModule"
                                    ValidationGroup="EditNew" Text=" * "  ErrorMessage="Modulo es requerido" InitialValue="-1" />
                                </div>
                                
                            </div>
                        </div>
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                        
                     </div>   
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo Rol --%>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>  

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Modal Update Progress --%>

    <%-- Mensajes de Confirmacion y Auxiliares --%>
     <asp:Label ID="lblToolTipEditar" runat="server" Text="Editar" Visible="false" />    
    <asp:Label ID="lblToolTipEliminar" runat="server" Text="Eliminar" Visible="false" />     
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Rol?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
