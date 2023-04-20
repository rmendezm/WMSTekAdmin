<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="LPNTypeMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Parameters.LPNTypeMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("LpnType_FindAll", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("LpnType_FindAll", "ctl00_MainContent_grdMgr");
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
                                AutoGenerateColumns="false"
                                OnRowDataBound="grdMgr_RowDataBound"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                               <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Code" />
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnerTradeName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.TradeName" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Name" />
                                    <asp:BoundField DataField="Tare" HeaderText="Tara" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Tare" />
                                    <asp:BoundField DataField="Volume" HeaderText="Volumen" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Volume" />
                                    <asp:BoundField DataField="Length" HeaderText="Largo" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Length" />
                                    <asp:BoundField DataField="Width" HeaderText="Alto" AccessibleHeaderText="Width" />
                                    <asp:BoundField DataField="NextAvailableNumber" HeaderText="Prox. Disp." InsertVisible="True"
                                        ReadOnly="True" AccessibleHeaderText="NextAvailableNumber" />
                                    <asp:BoundField DataField="WeightCapacity" HeaderText="Capacidad Peso" InsertVisible="True"
                                        ReadOnly="True" AccessibleHeaderText="WeightCapacity" />
                                    <asp:BoundField DataField="VolumeCapacity" HeaderText="Capacidad Vol." InsertVisible="True"
                                        ReadOnly="True" AccessibleHeaderText="VolumeCapacity" />
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   <asp:TemplateField HeaderText="PTL" AccessibleHeaderText="PTLLabel">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkPTLLabel" runat="server" Checked='<%# Eval ( "PTLLabel" ) %>' Enabled="false" />
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
                                                        CausesValidation="false" CommandName="Delete" ToolTip="Eliminar"/>
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
    <%-- Encabezado --%>
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
    <%-- Fin Encabezado --%>
    
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
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Tipo de LPN" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Tipo de LPN" />
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
                                    <asp:DropDownList ID="ddlOwner" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                                                                             
                            <%--CODE --%>
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="10" Width="150" />
                                    
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtCode" runat="server" ControlToValidate="txtCode"
	                                     ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                            
                                 </div>
                            </div>

                            <%--NAME --%>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                            
                                </div>
                            </div>
                            <%--TARE --%>
                            <div id="divTare" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTare" runat="server" Text="Tara" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTare" runat="server" MaxLength="13" Width="150" />
                                    <asp:Label ID='lblTypeUnitOfMass' runat="server" class="TypeUnit"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvTare" runat="server" ControlToValidate="txtTare"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Tara es requerido" />
                                    <asp:RangeValidator ID="rvTare" runat="server" ControlToValidate="txtTare" ErrorMessage="Tara no contiene un número válido"
                                        MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <%--VOLUME --%>
                             <div id="divVolume" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblVolume" runat="server" Text="Volumen" />
                                </div>
                                <asp:TextBox ID="txtVolume" runat="server" MaxLength="13" />
                                <asp:Label ID='lblTypeUnitMeasure' runat="server" class="TypeUnit">(m³)</asp:Label>                                
                                <asp:RequiredFieldValidator ID="rfvVolume" runat="server" ControlToValidate="txtVolume"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Volumen es requerido" />
                                <asp:RangeValidator ID="rvVolume" runat="server" ControlToValidate="txtVolume" ErrorMessage="Volumen no contiene un número válido"
                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                             </div>
                            <%--LENGTH --%>
                            <div id="divLength" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLength" runat="server" Text="Largo" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLength" runat="server" MaxLength="13" />
                                    <asp:Label ID='lblTypeUnitMeasure2' runat="server" class="TypeUnit"></asp:Label>     
                                    <asp:RequiredFieldValidator ID="rfvLength" runat="server" ControlToValidate="txtLength" ValidationGroup="EditNew" Text=" * " ErrorMessage="Largo es requerido"/>
                                    <asp:RangeValidator ID="rvLength" runat="server" ControlToValidate="txtLength" ErrorMessage="Largo no contiene un número válido"
                                        MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <%--WIDTH --%>
                            <div id="divWidth" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWidth" runat="server" Text="Ancho" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWidth" runat="server" MaxLength="13" />
                                    <asp:Label ID='lblTypeUnitMeasure3' runat="server" class="TypeUnit"></asp:Label>  
                                    <asp:RequiredFieldValidator ID="rfvWidth" runat="server" ControlToValidate="txtWidth" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ancho es requerido" />
                                    <asp:RangeValidator ID="rvWidth" runat="server" ControlToValidate="txtWidth" ErrorMessage="Ancho no contiene un número válido"
                                        MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <%--HEIGTH --%>
                            <div id="divHeight" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblHeight" runat="server" Text="Alto" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtHeight" runat="server" MaxLength="13" />
                                    <asp:Label ID='lblTypeUnitMeasure4' runat="server" class="TypeUnit"></asp:Label>  
                                    <asp:RequiredFieldValidator ID="rfvHeight" runat="server" ControlToValidate="txtHeight" ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto es requerido"/>
                                    <asp:RangeValidator ID="rvHeight" runat="server" ControlToValidate="txtHeight" ErrorMessage="Alto no contiene un número válido"
                                        MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <%--NextAvailableNumber --%>
                            <div  class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblNextAvailableNumber" runat="server" Text="Prox. Num. Disp." />
                                </div>
                                <asp:TextBox ID="txtNextAvailableNumber" runat="server" MaxLength="8" />
                                <asp:RequiredFieldValidator ID="rfvNextAvailableNumber" runat="server" ControlToValidate="txtNextAvailableNumber"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Prox. Num. Disp. es requerido" />
                                <asp:RangeValidator ID="rvNextAvailableNumber" runat="server" ControlToValidate="txtNextAvailableNumber" ErrorMessage="Prox. Num. Disp. no contiene un número válido"
                                    MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                            
                                <%--WeightCapacity --%>
                                <div  class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblWeightCap" runat="server" Text="Capacidad Peso" />
                                    </div>
                                    <asp:TextBox ID="txtWeightCapacity" runat="server" MaxLength="13" />
                                    <asp:Label ID='lblTypeUnitOfMass2' runat="server" class="TypeUnit"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvWeightCapacity" runat="server" ControlToValidate="txtWeightCapacity"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Capacidad Peso es requerido" />
                                    <asp:RangeValidator ID="rvWeightCapacity" runat="server" ControlToValidate="txtWeightCapacity" ErrorMessage="Capacidad Peso no contiene un número válido"
                                        MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>                        
                                
                            <%--VolumeCapacity --%>
                            <div  class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblVolumeCap" runat="server" Text="Capacidad Volumen" />
                                </div>
                                <asp:TextBox ID="txtVolumeCapacity" runat="server" MaxLength="13" />
                                <asp:Label ID='lblTypeUnitMeasure5' runat="server" class="TypeUnit">(m³)</asp:Label>
                                <asp:RequiredFieldValidator ID="rfvVolumeCapacity" runat="server" ControlToValidate="txtVolumeCapacity"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Capacidad Volumen es requerido" />
                                <asp:RangeValidator ID="rvVolumeCapacity" runat="server" ControlToValidate="txtVolumeCapacity" ErrorMessage="Capacidad Volumen no contiene un número válido"
                                    MaximumValue="99999999,9999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                            </div>

                             <%--PTLLabel --%>
                            <div id="divPTLLabel" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPTLLabel" runat="server" Text="PTL" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkPTLLabel" runat="server" /></div>
                            </div>   

                            </div>
                            
                            <div id="div1" class="divControls" runat="server">
                            </div>
                            
                            <div id="divShowErrors" visible="false" class="divControls" runat="server">
                                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                                <br />
                                <br />
                            </div>
                        </div>
                        
                       <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"    ShowMessageBox="false" CssClass="modalValidation"/>
                            
                            <br />
                        </div>
                        
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
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
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
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
    <%-- Barra de Estado --%>
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Tipo de LPN?" Visible="false" />
    <asp:Label ID="lblErrorCodeExist" runat="server" Visible="false" Text="* El código ingresado ya existe, por favor ingrese un nuevo código" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
