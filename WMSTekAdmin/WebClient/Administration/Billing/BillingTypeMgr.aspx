<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="BillingTypeMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Billing.BillingTypeMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
    
        $(document).ready(function () {
            initializeGridDragAndDrop("BillingType_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("BillingType_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" AutoGenerateColumns="False" AllowPaging="True" 
                                OnRowCreated="grdMgr_RowCreated" 
                                OnRowDeleting="grdMgr_RowDeleting" 
                                OnRowEditing="grdMgr_RowEditing"
                                OnPageIndexChanging="grdMgr_PageIndexChanging" 
                                OnRowDataBound="grdMgr_RowDataBound"
                                EnableViewState="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                        
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Forma Cobro" AccessibleHeaderText="ModeName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblModeName" runat="server" Text='<%# Eval ("BillingMode.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Paso Cobro" AccessibleHeaderText="StepName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblStepName" runat="server" Text='<%# Eval ("BillingStep.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo LPN" AccessibleHeaderText="LPNTypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblLPNTypeName" runat="server" Text='<%# Eval ("LPNType.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Vas" AccessibleHeaderText="VasName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblVasName" runat="server" Text='<%# Eval ("RecipeVas.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tiempo Cobro" AccessibleHeaderText="TimeType">
                                        <ItemTemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblTimeType" runat="server" Text='<%# (string)Eval ( "TimeType" ) == "C" ? "CadaVez" : ((string)Eval ( "TimeType" ) == "D" ? "Diario" : ((string)Eval ( "TimeType" ) == "A" ? "Adicional" : "Fijo")) %>'/>
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Zona" AccessibleHeaderText="WorkZoneName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblWorkZoneName" runat="server" Text='<%# Eval ("WorkZone.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Documento" AccessibleHeaderText="OutboundTypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblOutboundTypeName" runat="server" Text='<%# Eval ("OutboundType.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 60px">
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CommandName="Edit" ToolTip="Editar" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CommandName="Delete" ToolTip="Eliminar" />
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
            <%-- Pop up Editar/Nuevo BillingContract --%>
            <div id="divEditNew" runat="server" visible="false">    
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlBillingType" BackgroundCssClass="modalBackground" PopupDragHandleControlID="BillingTypeCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlBillingType" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Tipo Cobro" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Tipo Cobro" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">

                            <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Activo" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" />
                                </div>
                            </div>

                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="5" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                </div>
                            </div>
                                       
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="60" />
                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" />
                                </div>
                            </div>

                            <div id="divTimeType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTimeType" runat="server" Text="Tiempo Cobro" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlTimeType" AutoPostBack="true" OnSelectedIndexChanged="ddlTimeType_SelectedIndexChanged"/>
                                    <asp:RequiredFieldValidator ID="rfvTimeType" runat="server" ControlToValidate="ddlTimeType" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tiempo Cobro es requerido" />
                                </div>
                            </div>
                            
                            <div id="divMode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMode" runat="server" Text="Forma Cobro" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlMode" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvMode" runat="server" ControlToValidate="ddlMode" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Forma Cobro es requerido" />
                                </div>
                            </div>

                            <div id="divStep" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStep" runat="server" Text="Etapa Cobro" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlStep" AutoPostBack="true" OnSelectedIndexChanged="ddlStep_OnSelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvStep" runat="server" ControlToValidate="ddlStep" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Etapa Cobro es requerido" />
                                </div>
                            </div>

                            <div id="divLPNType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLPNType" runat="server" Text="Tipo LPN" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlLPNType" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvLPNType" runat="server" ControlToValidate="ddlLPNType" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo LPN es requerido" />
                                </div>
                            </div>

                            <div id="divVAS" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblVAS" runat="server" Text="VAS" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlVAS" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvVAS" runat="server" ControlToValidate="ddlVAS" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="VAS es requerido" />
                                </div>
                            </div>

                            <div id="divWorkZone" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWorkZone" runat="server" Text="Zona" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlWorkZone" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvWorkZone" runat="server" ControlToValidate="ddlWorkZone" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Zona es requerido" />
                                </div>
                            </div>

                            <div id="divOutboundType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOutboundType" runat="server" Text="Tipo Documento" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOutboundType" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOutboundType" runat="server" ControlToValidate="ddlOutboundType" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Documento es requerido" />
                                </div>
                            </div>

                        </div>    
                        <div class="divValidationSummary" >
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
            <%-- FIN Pop up Editar/Nuevo  --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Tipo de Cobro?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
        
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>