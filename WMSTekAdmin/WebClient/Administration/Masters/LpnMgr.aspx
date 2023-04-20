<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="LpnMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.LpnMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 

    $(document).ready(function () {
        initializeGridDragAndDrop("Lpn_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("Lpn_FindAll", "ctl00_MainContent_grdMgr");
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
                                DataKeyNames="IdCode" 
                                runat="server" 
                                OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnPageIndexChanging="grdMgr_PageIndexChanging"
                                AllowPaging="True" 
                                EnableViewState="false" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="IdCode" HeaderText="Código" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnerName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.TradeName" ) %>'></asp:Label>
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. Tipo" AccessibleHeaderText="LpnTypeCode">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLpnTypeCode" runat="server" Text='<%# Eval ( "LPNType.Code" ) %>'></asp:Label>
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo Lpn" AccessibleHeaderText="LpnTypeName">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLpnTypeName" runat="server" Text='<%# Eval ( "LPNType.Name" ) %>'></asp:Label>
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:templatefield headertext="Fifo" accessibleHeaderText="Fifo">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblFifo" runat="server" text='<%# ((DateTime) Eval ("Fifo") > DateTime.MinValue)? Eval("Fifo", "{0:d}"):"" %>' />
                                                </div>
                                            </center>    
                                    </itemtemplate>
                                    </asp:templatefield>           
                                                                            
                                    <asp:BoundField DataField="WeightEmpty" HeaderText="Peso en vacío" AccessibleHeaderText="WeightEmpty" />
                                    <asp:BoundField DataField="WeightTotal" HeaderText="Peso Total" AccessibleHeaderText="WeightTotal" />
                                    <asp:TemplateField HeaderText="Cerrado" AccessibleHeaderText="IsClosed">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkIsClosed" runat="server" Checked='<%# Eval ( "IsClosed" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="SealNumber" HeaderText="Número Sello" AccessibleHeaderText="SealNumber" />

                                    <asp:TemplateField HeaderText="Lpn Contenedor" AccessibleHeaderText="LpnParent">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLpnParent" runat="server" Text='<%# Eval ( "LpnParent" ) %>'></asp:Label>
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Es contenedor" AccessibleHeaderText="IsParent">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkIsParent" runat="server" Checked='<%# Eval ( "IsParent" ) %>' Enabled="false" />
                                            </center>
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
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop"
        CssClass="updateProgress" TargetControlID="uprGrid" />
    <%-- FIN Modal Update Progress --%>
    
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Lpn --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlLpn" BackgroundCssClass="modalBackground" PopupDragHandleControlID="LpnCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlLpn" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="LpnCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo LPN" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar LPN" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                        
                            <%--STATUS --%>
                            <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" /></div>
                            </div>
                                    
                            <%--OWNER --%>
                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlOwner" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOwner_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                                                                                
                            <%--CODE --%>
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtCode" runat="server" ControlToValidate="txtCode"
	                                     ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>

                            <%--TYPE LPN --%>
                            <div id="divLpnType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLpnType" runat="server" Text="Tipo" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlLpnType" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvLpnType" runat="server" ControlToValidate="ddlLpnType"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo es requerido" />
                                </div>
                            </div>

                            <%--WeightEmpty --%>
                            <div id="divWeightEmpty" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWeightEmpty" runat="server" Text="Peso en Vacio" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWeightEmpty" runat="server" MaxLength="15" Width="150" />
                                    <asp:Label ID='lblTypeUnitOfMass' runat="server" class="TypeUnit"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvWeightEmpty" runat="server" ControlToValidate="txtWeightEmpty"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Peso en Vacio es requerido" />
                                    <asp:RangeValidator ID="rvWeightEmpty" runat="server" ControlToValidate="txtWeightEmpty"
                                        ErrorMessage="Peso en Vacio no contiene un número válido" MaximumValue="9999999999"
                                        MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>                                      
                                </div>
                            </div>
                            <%--WeightTotal --%>
                            <div id="divWeightTotal" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWeightTotal" runat="server" Text="Peso Total" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWeightTotal" runat="server" MaxLength="15" Width="150" />
                                    <asp:Label ID='lblTypeUnitOfMass2' runat="server" class="TypeUnit"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvWeightTotal" runat="server" ControlToValidate="txtWeightTotal"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Peso Total es requerido" />
                                    <asp:RangeValidator ID="rvWeightTotal" runat="server" ControlToValidate="txtWeightTotal"
                                        ErrorMessage="Peso Total no contiene un número válido" MaximumValue="9999999999"
                                        MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>                                        
                                </div>
                            </div>
                            <%--IsClosed --%>
                            <div id="divIsClosed" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIsClosed" runat="server" Text="Cerrado" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkIsClosed" runat="server" />
                                </div>
                            </div>
                            <%--SealNumber --%>
                            <div id="divSealNumber" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblSealNumber" runat="server" Text="Nº Sello" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtSealNumber" runat="server" MaxLength="20" Width="150" />
                                    <%--<asp:RequiredFieldValidator ID="rfvSealNumber" runat="server" ControlToValidate="txtSealNumber"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nº Sello es requerido" />--%>
                                    <asp:RegularExpressionValidator ID="revtxtSealNumber" runat="server" ControlToValidate="txtSealNumber"
	                                     ErrorMessage="Nº Sello permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * "/>
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
            <%-- Pop up Editar/Nuevo Lpn --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este LPN?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
