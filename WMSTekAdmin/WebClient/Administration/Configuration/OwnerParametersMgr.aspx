<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="OwnerParametersMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Configuration.OwnerParametersMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("CfgOwnerParameter_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("CfgOwnerParameter_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            
                            <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True" EnableViewState="False" OnRowDataBound="grdMgr_RowDataBound"
                                OnRowDeleting="grdMgr_RowDeleting" AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />

                                    <asp:TemplateField HeaderText="Bodega" AccessibleHeaderText="Warehouse">
                                        <ItemTemplate>
                                            <asp:Label ID="lblWarehouseName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Parámetro" AccessibleHeaderText="Code">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCode" runat="server" Text='<%# Eval ( "ParameterCode" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                        
                                    <asp:TemplateField HeaderText="Valor Actual" AccessibleHeaderText="ParameterValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblParameterValue" runat="server" Text='<%# Eval ( "ParameterValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="Type">
                                        <ItemTemplate>
                                            <asp:Label ID="lblType" runat="server" Text='<%# Eval ( "Type" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor Min." AccessibleHeaderText="MinValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMinValue" runat="server" Text='<%# Eval ( "MinValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor Max." AccessibleHeaderText="MaxValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMaxValue" runat="server" Text='<%# Eval ( "MaxValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor por defecto" AccessibleHeaderText="DefaultValue">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDefaultValue" runat="server" Text='<%# Eval ( "DefaultValue" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ámbito" AccessibleHeaderText="Scope">
                                        <ItemTemplate>
                                            <asp:Label ID="lblScope" runat="server" Text='<%# Eval ( "Scope" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Módulo" AccessibleHeaderText="Module">
                                        <ItemTemplate>
                                            <asp:Label ID="lblModuleName" runat="server" Text='<%# Eval ( "Module.Name" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Editable" AccessibleHeaderText="AllowEdit">
                                        <ItemTemplate>
                                            <center>
                                                <asp:CheckBox ID="chkAllowEdit" runat="server" Checked='<%# Eval ( "AllowEdit" ) %>'
                                                    Enabled="false" />
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                    <ItemTemplate>
                                        <center>
                                            <div style="width: 60px">
                                                <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                    CausesValidation="false" CommandName="Delete" />
                                            </div>
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                
                            <div id="divWarning" class="divWarning" runat="server" visible="false">
                                <asp:Label ID="lblError" Visible="false" runat="server" Text="Error en el ingreso de datos para el Parametro: " />ç
                                <asp:Label ID="lblErrorTitle" Visible="false" runat="server" Text="Dato no válido" />
                    
                            </div>             
                        </div>
                        <%-- FIN Grilla Principal --%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <%-- Barra de Estado --%>
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
    <%--ParameterValue  --%>

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar Parametro --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlParameters" BackgroundCssClass="modalBackground" PopupDragHandleControlID="ParametersCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlParameters" Width="1000px" runat="server" CssClass="modalBox">                                  
                                        
                    <asp:Panel ID="ParametersCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Configuración Parámetros" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>

                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft" style="width:360px">  
                            <div id="div1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label2" runat="server" Text="Parámetros" Font-Bold="true"/>
                                    <asp:ListBox ID="lstBoxParameters" runat="server" Width="350" Height="320" OnSelectedIndexChanged="lstBoxParameters_SelectedIndexChanged" 
                                            SelectionMode="Single" style="overflow-x:auto;"  Font-Bold="true" BackColor="#ffffe6"  AutoPostBack="true" >
                                        </asp:ListBox>  
                                    <asp:RequiredFieldValidator ID="rfvBoxParameters" runat="server" ControlToValidate="lstBoxParameters"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Parámetro es requerido" />
                                </div>
                            </div>
                       </div>
                        <div class="divCtrsFloatLeft" style="width:260px">
                            <div id="div2" runat="server" class="divControls ">
                                <div class="fieldRight">
                                    <asp:Label ID="Label1" runat="server" Text="Dueños" Font-Bold="true" />
                                    <asp:ListBox ID="lstBoxOwners" runat="server" Width="250" Height="320" 
                                        SelectionMode="Multiple" style="overflow-x:auto;"  Font-Bold="true"  BackColor="#ffffe6" >                                    
                                     </asp:ListBox>
                                    <asp:RequiredFieldValidator ID="rfvBoxOwners" runat="server" ControlToValidate="lstBoxOwners"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                        </div>
                        <div class="divCtrsFloatLeft">
                            <div id="div3" runat="server" class="divControls">                               
                                    <br />                               
                            </div>
                              <%--Parameter Code --%>
                            <div id="divParameterCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParameterCode" runat="server" Text="Parámetro" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtParameterCode" runat="server" MaxLength="20" Width="240" Enabled="false"/> 
                                </div>
                             </div>       
                            <%--Description --%>
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="20" Width="240" Enabled="false" /> 
                                </div>
                            </div>
                                                                                
                            <%--Type --%>
                            <div id="divType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblType" runat="server" Text="Tipo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtType" runat="server" MaxLength="20" Width="100" Enabled="false" /> 
                                </div>
                            </div>

                            <%--Alow Edit --%>
                            <div id="divAllowEdit" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblAllowEdit" runat="server" Text="Editable" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkAllowEdit" runat="server" Enabled="false" />
                                </div>
                            </div>
                             <%--MinValue --%>
                            <div id="divMinValue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMinValue" runat="server" Text="Valor Min." /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtMinValue" runat="server" MaxLength="20" Width="220" Enabled="false"/> 
                                </div>
                             </div>       
                             <%--Parameter Code --%>
                            <div id="divMaxValue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMaxValue" runat="server" Text="Valor Max." /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtMaxValue" runat="server" MaxLength="20" Width="220" Enabled="false"/> 
                                </div>
                             </div>       

                            <%--Default Value --%>
                            <div id="divDefaultValue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDefaultValue" runat="server" Text="Valor por Defecto" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDefaultValue" runat="server"  Width="220" Enabled="false" />                                  
                                </div>
                            </div>

                            <%--Parameter Value --%>
                            <div id="divParamValue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParamValue" runat="server" Text="Valor" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtParamValue" runat="server" MaxLength="1000" Width="220" /> 
                                    <asp:RequiredFieldValidator ID="reqParameterValue" runat="server" ControlToValidate="txtParamValue"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Valor es requerido" />
                                    <asp:RangeValidator ID="ranParameterValue" runat="server" ControlToValidate="txtParamValue" 
                                        MaximumValue ="1000" Type="String" ErrorMessage="..." Text=" * "/>
                                </div>
                            </div>
                           
                        </div>

                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server"  Text="Guardar" CausesValidation="true"
                                ValidationGroup="EditNew" OnClick="btnSave_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                                                
                    </div>
                   
                  
                </asp:Panel>
            </div>
            <%-- Pop up Editar/Nuevo Parametro --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucContentError$btnCloseError" EventName="Click" />
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta configuración?" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Parámetro" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Configuración Parametros" Visible="false"/>
    <asp:Label ID="lblReset" runat="server" Text="Debe reiniciar la aplicación para activar los cambios realizados. <br><br> ¿Desea reiniciar ahora?" Visible="false"/> 
    <asp:Label ID="lblPoliciyPasswordTitle" runat="server" Text="Error Política Contraseñas Parámetro DefaultPassword" Visible="false"/>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>        
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
