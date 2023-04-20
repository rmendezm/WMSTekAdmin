<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ReasonMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Parameters.ReasonMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("Reason_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Reason_FindAll", "ctl00_MainContent_grdMgr");
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" OnRowEditing="grdMgr_RowEditing" OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True" EnableViewState="false" 
                                onrowdatabound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                        
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="TypeInOut">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTypeInOut" runat="server" Text='<%# Eval ( "TypeInOut" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Bloquea" AccessibleHeaderText="CanBlock">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkCanBlock" runat="server" Checked='<%# Eval ( "CanBlock" ) %>'
                                                    Enabled="false" />
                                            </center>                                   
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Genera Conteo Cíclico" AccessibleHeaderText="CanCount">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkCanCount" runat="server" Checked='<%# Eval ( "CanCount" ) %>'
                                                    Enabled="false" />
                                            </center>                                
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>'
                                                    Enabled="false" />
                                            </center>                                
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Informe ERP" AccessibleHeaderText="InfoERP">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkInfoERP" runat="server" Checked='<%# Eval ( "InfoERP" ) %>'
                                                    Enabled="false" />
                                            </center>                                
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" ToolTip="Editar"/>
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" ToolTip="Eliminar" />
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
            <%-- Pop up Editar/Nuevo Proveedor --%>
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="VendorCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlVendor" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="VendorCaption" runat="server" CssClass="modalHeader" >
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Razón" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Razón" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                             <%--Status--%>                        
                            <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" /></div>
                            </div>    

                           <%-- Code --%>
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="5" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtCode" runat="server" ControlToValidate="txtCode"
	                                     ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                            
                                 </div>
                            </div>
                                                                             
                            <%-- Name --%>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="60" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                          
                                    
                                </div>
                            </div>

                            <%-- TypeInOut (manejar como lista 123)--%>
                            <div id="divTypeInOut" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTypeInOut" runat="server" Text="Tipo" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTypeInOut" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvTypeInOut" runat="server" ControlToValidate="ddlTypeInOut"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo es requerido" InitialValue="-1"/></div>
                            </div>
                            
                             <%--CanBlock  (Bloquea Ubicacion)--%>
                              <div id="divCanBlock" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCanBlock" runat="server" Text="Puede Bloquear"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkCanBlock" runat="server" />
                                </div>
                            </div>
                             <%--CanCount  (Genera conteo cíclico)--%>
                              <div id="divCanCount" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCanCount" runat="server" Text="Genera Conteo Cíclico"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkCanCount" runat="server" />
                                </div>
                            </div>
                            <%--InfoERP  (Informa al ERP)--%>
                            <div id="divInfoERP" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblInfoERP" runat="server" Text="Informa al ERP"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkInfoERP" runat="server" />
                                </div>
                            </div>

                        </div>
                       <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"    ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                           
                    </div>    
                </asp:Panel>
            </div>
            <%-- Pop up Editar/Nuevo Razon --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Razón?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>

