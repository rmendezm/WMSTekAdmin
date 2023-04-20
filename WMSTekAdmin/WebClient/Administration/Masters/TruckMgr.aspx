<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="TruckMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.TruckMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("Truck_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("Truck_FindAll", "ctl00_MainContent_grdMgr");
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
                                    <asp:BoundField DataField="IdCode" HeaderText="Patente" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="IdCode" />
                                    <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                        
                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:CheckBox ID="lblStatus" runat="server" Enabled="false" Checked='<%# Eval ( "Status" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="TruckType">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblTruckType" runat="server" Text='<%# Eval ( "Type.Name" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TruckMark" HeaderText="Marca" AccessibleHeaderText="TruckMark" />
                                    <asp:BoundField DataField="TruckModel" HeaderText="Modelo" AccessibleHeaderText="TruckModel" />
                                    <asp:BoundField DataField="FabricationYear" HeaderText="Año Fab." AccessibleHeaderText="FabricationYear" />
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
    
    <%-- Pop up Editar/Nuevo Camión --%>
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
            <%-- Pop up Editar/Nuevo Camión --%>
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp"  runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlTruck" BackgroundCssClass="modalBackground" PopupDragHandleControlID="TruckCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlTruck" runat="server"  CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="TruckCaption" runat="server" CssClass="modalHeader">
                         <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Camión" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Camión" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            <%-- Status --%>
                            <div id="divStatus" runat="server" class="divControls">
                                    <div class="fieldRight">
                                        <asp:Label ID="lblStatus" runat="server" Text="Activo" />
                                    </div>
                                    <div class="fieldLeft">
                                        <asp:CheckBox ID="chkStatus" runat="server" />
                                   </div>
                            </div>      
                            <%-- Code / Plate --%>       
                            <div id="divIdCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblIdCode" runat="server" Text="Placa" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtIdCode" runat="server" MaxLength="12" Width="100" />
                                    <asp:RequiredFieldValidator ID="rfvIdCode" runat="server" ControlToValidate="txtIdCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Placa es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revIdCode" runat="server" ControlToValidate="txtIdCode"
	                                     ErrorMessage="Placa permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ-]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                
                                </div>
                            </div>                               
                            <%-- Description --%>       
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="60" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revtxtDescription" runat="server" ControlToValidate="txtDescription"
	                                     ErrorMessage="Descripción permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ-]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
                                </div>
                            </div>
                            <%-- TruckType --%>
                            <div id="divTruckType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTruckType" runat="server" Text="Tipo"></asp:Label></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTruckType" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvTruckType" runat="server" ControlToValidate="ddlTruckType"
                                         InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo es requerido">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <%-- TruckMark --%>                        
                            <div id="divTruckMark" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTruckMark" runat="server" Text="Marca" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTruckMark" runat="server" MaxLength="30" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvTruckMark" runat="server" ControlToValidate="txtTruckMark"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Marca es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revTruckMark" runat="server" ControlToValidate="txtTruckMark"
	                                     ErrorMessage="Marca permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
                                </div>
                            </div> 
                            <%-- TruckModel --%>       
                            <div id="divTruckModel" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTruckModel" runat="server" Text="Modelo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtTruckModel" runat="server" MaxLength="49" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvTruckModel" runat="server" ControlToValidate="txtTruckModel"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Modelo es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revTruckModel" runat="server" ControlToValidate="txtTruckModel"
	                                     ErrorMessage="Modelo permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator> 
                                </div>
                            </div>
                            <%-- FabricationYear --%>   
                            <div id="divFabricationYear" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFabricationYear" runat="server" Text="Año Fabricación" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFabricationYear" runat="server" MaxLength="4" 
                                        Width="100" ToolTip="Ej: 2009" />
                                    <asp:RangeValidator ID="rvFabricationYear" runat="server" ControlToValidate="txtFabricationYear"
                                        ErrorMessage="Año Fabricación no contiene un número válido" 
                                        MaximumValue="2199"
                                        MinimumValue="1900" 
                                        ValidationGroup="EditNew" 
                                        Type="Integer" Text="*">
                                     </asp:RangeValidator>  
                                                                             
                                    <asp:RequiredFieldValidator ID="rfvFabricationYear" runat="server" ControlToValidate="txtFabricationYear"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Año Fabricación es requerido">
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
            <%-- Pop up Editar/Nuevo Camión --%>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Camión?" Visible="false" />
    <asp:Label ID="lblFilterCodeLabel" runat="server" Text="Patente" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
 
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
