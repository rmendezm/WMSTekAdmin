<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="GroupItem2Mgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.GroupItem2Mgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("GrpItem2_FindAll_Filters", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("GrpItem2_FindAll_Filters", "ctl00_MainContent_grdMgr");
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
                                OnRowDataBound="grdMgr_RowDataBound"
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True"
                                EnableViewState="False" 
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />   
                                    <asp:TemplateField HeaderText="Código Sector" AccessibleHeaderText="GrpItem1Code">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrpItem1Code" runat="server" Text='<%# Eval ( "GrpItem1.Code" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre Sector" AccessibleHeaderText="GrpItem1Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrpItem1Name" runat="server" Text='<%# Eval ( "GrpItem1.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                     
                                     <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval  ( "GrpItem1.Owner.Name" ) %>' ></asp:Label>
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
    
    <%-- Pop up Editar/Nuevo GroupItem2 --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- Modal Update Progress --%>
                  
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <%-- Pop up Editar/Nuevo GroupItem2 --%>
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp"  runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlGroupItem2" BackgroundCssClass="modalBackground" PopupDragHandleControlID="GroupItem2Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlGroupItem2" runat="server"  CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="GroupItem2Caption" runat="server" CssClass="modalHeader">
                         <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Rubro" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Rubro" />
                            <%-- <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />--%>
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            <%-- Dueño --%> 
                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlOwner" runat="server" Width="120px" AutoPostBack="true" OnSelectedIndexChanged="ddlOwner_SelectedIndexChanged"/>
                                    <asp:RequiredFieldValidator ID="rfvIdOwner" runat="server" ValidationGroup="EditNew"
                                        Text=" * " ControlToValidate="ddlOwner"  Display="Dynamic" InitialValue="-1"  
                                        ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>

                            <%-- GrpItem1 --%>       
                            <div id="divGrpItem1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblGrpItem1" runat="server" Text="Sector" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlGrpItem1" runat="server"  ></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvGrpItem1" runat="server" ControlToValidate="ddlGrpItem1"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Sector es requerido" />
                                </div>
                            </div>
                            <%-- Code --%>       
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="100" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revCode" runat="server" ControlToValidate="txtCode"
	                                     ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                
                                </div>
                            </div>        
                                                   
                            <%-- Nombre --%>       
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="30" Width="100" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
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
            <%-- Pop up Editar/Nuevo GroupItem2 --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Rubro?" Visible="false" />
    <asp:Label ID="lblFilterCodeLabel" runat="server" Text="Patente" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
 
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>