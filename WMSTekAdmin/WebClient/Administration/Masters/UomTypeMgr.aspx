<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="UomTypeMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.UomTypeMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

<script type="text/javascript">
    window.onresize = SetDivs;

        $(document).ready(function () {
            initializeGridDragAndDrop("UomType_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("UomType_FindAll", "ctl00_MainContent_grdMgr");
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
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID  Unid medida" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="IdUomType" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre unidad de medida" AccessibleHeaderText="UomName" />
                                    <asp:TemplateField HeaderText="ID Dueño" AccessibleHeaderText="IdOwn">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdOwn" runat="server" Text='<%# Eval ("Owner.Id") %>'/>
                                             </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:Label ID="lblOwnName" runat="server" Text='<%# Eval ("Owner.Name") %>'/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                    <asp:TemplateField HeaderText="Maneja Decimales" AccessibleHeaderText="HandlesDecimal">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkHandleDecimalGrid" runat="server" Checked='<%# Eval ( "HandlesDecimal" ) %>' Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tiene peso variable" AccessibleHeaderText="IsVariableWeight">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkIsVariableWeightGrid" runat="server" Checked='<%# Eval ( "IsVariableWeight" ) %>' Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="% Tolerancia" AccessibleHeaderText="OverPickingAllowed">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">                                
                                                <asp:Label ID="lblOverPickingAllowedGrid" runat="server" Text='<%#  ((int)Eval("OverPickingAllowed") == -1) ? "" : Eval("OverPickingAllowed")  %>'/>
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
            <%-- Pop up Editar/Nuevo UomType --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlUomType" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UomTypeCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlUomType" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="UomTypeCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Unidad de Medida" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Unidad de Medida" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                                                   
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Unidad de medida" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <asp:DropDownList ID="ddlOwner" runat="server" Width="150px" TabIndex="14" />
                                <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                            </div> 
                            
                            <div id="divHandleDecimals" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHandleDecimal" runat="server" Text="Maneja decimales" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkHandleDecimal" runat="server" />
                                </div>
                            </div>

                            <div id="divIsVariableWeight" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIsVariableWeight" runat="server" Text="Tiene peso variable" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkIsVariableWeight" runat="server" AutoPostBack="false"  />
                                </div>
                            </div>

                            <div id="divOverPickingAllowed" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOverPickingAllowed" runat="server" Text="% Tolerancia" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtOverPickingAllowed" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="reqOverPickingAllowed" runat="server" Enabled="false" ControlToValidate="txtOverPickingAllowed" ValidationGroup="EditNew" Text=" * " ErrorMessage="% Tolerancia es requerido">*</asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvOverPickingAllowed" runat="server" Enabled="false" ControlToValidate="txtOverPickingAllowed" ErrorMessage="% Tolerancia no contiene un número válido" MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                </div>
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
            <%-- Pop up Editar/Nuevo UomType --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Unidad de Medida?" Visible="false" />
    <asp:Label ID="lblNameFilter" runat="server" Text="Nombre Unidad" Visible="false" />

    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>