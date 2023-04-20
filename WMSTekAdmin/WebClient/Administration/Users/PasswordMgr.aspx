<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="PasswordMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Users.PasswordMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        initializeGridDragAndDrop("User_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("User_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <style>
        .divGrid {
            overflow:hidden !important;
        }
    </style>

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
                                OnRowEditing="grdMgr_RowEditing" 
                                OnRowDataBound="grdMgr_RowDataBound" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True" 
                                EnableViewState="False" 
                                OnRowDeleting="grdMgr_RowDeleting" 
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        SortExpression="Id" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="UserName" HeaderText="Usuario" AccessibleHeaderText="UserName" />
                                    <asp:BoundField DataField="FirstName" HeaderText="Nombre" AccessibleHeaderText="FirstName" />
                                    <asp:BoundField DataField="LastName" HeaderText="Apellido" AccessibleHeaderText="LastName" />
                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <div style="width: 60px">
                                                <center>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" />
                                                    <asp:ImageButton ID="btnReset" runat="server" CausesValidation="false" CommandName="Delete"
                                                        ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_reset_key.png" />
                                                </center>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>    

                        </div>
                        <%-- Grilla Principal --%>
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
            <%-- Pop up Editar/Nuevo Usuario --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlUser" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <%-- Encabezado --%>
                <asp:Panel ID="pnlUser" runat="server" CssClass="modalBox">
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Contraseña" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidIsBaseUser" runat="server" Value="false" />
                        <div class="divCtrsFloatLeft">
                            <div id="divUserName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblUserName" runat="server" Text="Usuario" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtUserName" runat="server" MaxLength="30" Width="150" Enabled="false" />
                                </div>
                            </div>
                            <div id="divFirstName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFirstName" runat="server" Text="Nombre" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFirstName" runat="server" MaxLength="30" Width="150" TabIndex="2" Enabled="false" />
                                </div>
                            </div>
                            <div id="divLastName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLastName" runat="server" Text="Apellido" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLastName" runat="server" MaxLength="30" Width="150" TabIndex="3" Enabled="false" />
                                </div>
                            </div>
                            <div id="divPassword" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPassword" runat="server" Text="Nueva Contraseña" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPassword" runat="server" MaxLength="30" Width="150" TabIndex="1"
                                        TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nueva Contraseña es requerido" />
                                </div>
                            </div>
                            <div id="divPasswordRepeat" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblRepeat" runat="server" Text="Repita Nueva Contraseña" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPasswordRepeat" runat="server" MaxLength="30" Width="150" TabIndex="2"
                                        TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="rfvPasswordRepeat" runat="server" ControlToValidate="txtPasswordRepeat"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Repita Nueva Contraseña es requerido" />
                                    <asp:CompareValidator ID="cvRepeatPassword" runat="server" ControlToCompare="txtPassword"
                                        ControlToValidate="txtPasswordRepeat" ErrorMessage="Las Contraseñas ingresadas no coinciden"
                                        ValidationGroup="EditNew" Text=" * " />
                                </div>
                            </div>
                        </div>
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"    ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                        
                    </div>
                </asp:Panel>
            </div>    
            <%-- FIN Pop up Editar Password --%>       </ContentTemplate>
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
    <asp:Label ID="lblFilterCode" runat="server" Text="Usuario" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblFilterDescription" runat="server" Text="Apellido" Visible="false" />    
    <asp:Label ID="lblToolTipReset" runat="server" Text="Resetea la contraseña" Visible="false" />    
    <asp:Label ID="lblToolTipEditar" runat="server" Text="Editar Contraseña" Visible="false" />
    <asp:Label ID="lblPoliciyPasswordTitle" runat="server" Text="Error Política Contraseñas" Visible="false"/>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
                
                
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
