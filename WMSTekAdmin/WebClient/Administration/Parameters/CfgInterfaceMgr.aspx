<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="CfgInterfaceMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Parameters.CfgInterfaceMgr" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("CfgInterface_FindAll", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();

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
            initializeGridDragAndDrop("CfgInterface_FindAll", "ctl00_MainContent_grdMgr");
            initializeGridWithNoDragAndDrop();
        }
    </script>

    <link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" >  
                            <asp:GridView ID="grdMgr" 
                                    DataKeyNames="Id" 
                                    runat="server" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    OnRowDeleting="grdMgr_RowDeleting" 
                                    OnRowEditing="grdMgr_RowEditing" 
                                    OnPageIndexChanging="grdMgr_PageIndexChanging"
                                    AllowPaging="True" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Código" AccessibleHeaderText="InterfaceCode">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblInterfaceCode" runat="server" Text='<%# Eval ( "InterfaceCode" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="InterfaceName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblInterfaceName" runat="server" Text='<%# Eval ( "InterfaceName" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Descripción" AccessibleHeaderText="Description">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval ( "Description" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                          <asp:TemplateField HeaderText="Status" AccessibleHeaderText="Status">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval ( "Status" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Intervalo" AccessibleHeaderText="FrequenceInterval">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblFrequenceInterval" runat="server" Text='<%# Eval ( "FrequenceInterval" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Fecha Inicio" AccessibleHeaderText="StartDate">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblStartDate" runat="server" Text='<%# (((DateTime)Eval ( "StartDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "StartDate", "{0:d}" )) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Fecha Fin" AccessibleHeaderText="EndDate">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblEndDate" runat="server" Text='<%# (((DateTime)Eval ( "EndDate" )).ToShortDateString() == "01/01/0001" ? "" : Eval ( "EndDate", "{0:d}" )) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                       <asp:TemplateField HeaderText="Paginación" AccessibleHeaderText="Pagination">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblPagination" runat="server" Text='<%# Eval ( "Pagination" ) %>'></asp:Label>
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
                    </ContentTemplate>
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
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

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />

    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
         <ContentTemplate>
             <div id="divEditNew" runat="server" visible="false">
                 <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                 <ajaxToolkit:ModalPopupExtender 
                     ID="modalPopUp" 
                     runat="server" 
                     TargetControlID="btnDummy"
                     PopupControlID="pnlInterface" 
                     BackgroundCssClass="modalBackground" 
                     PopupDragHandleControlID="InterfaceCaption"
                     Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlInterface" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="InterfaceCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Registro" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Registro" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>

                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />

                        <div class="divCtrsFloatLeft">

                            <%-- CODE --%>
                            <div id="divCode" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtCode" runat="server" MaxLength="50" Width="250" />
                                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode" ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" /> 
                                </div>
                            </div>
                             <%-- end CODE --%>

                            <%-- NAME --%>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblNameEdit" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="60" Width="250" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>
                             <%-- end NAME --%>

                            <%-- DESCRIPTION --%>
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="60" Width="250" />
                                    <asp:RegularExpressionValidator ID="revtxtDescription" runat="server" ControlToValidate="txtDescription"
	                                     ErrorMessage="Descripción permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>
                            <%-- end DESCRIPTION --%>

                            <%-- STATUS --%>
                            <div id="div1" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStatus" runat="server" Text="Habilitado" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkStatus" runat="server" /> 
                                </div>
                            </div>
                            <%-- end STATUS --%>

                            <%-- START DATE --%>
                            <div id="divStartDate" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStartDate" runat="server" Text="Fecha Inicio" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtStartDate" runat="server" Width="70px" 
                                    ValidationGroup="EditNew" ToolTip="Ingrese fecha." />
                                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Fecha es requerido"  />
                                
                                    <asp:RangeValidator ID="rvStartDate" runat="server" ControlToValidate="txtStartDate"
                                        ErrorMessage="Fecha debe estar entre 01-01-2000 y 31-12-2040" Text=" * " MinimumValue="01/01/2000"
                                        MaximumValue="31/12/2040" ValidationGroup="EditNew" Type="Date" />
                                    <ajaxToolkit:CalendarExtender ID="calStartDate" CssClass="CalMaster" runat="server" 
                                        Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtStartDate" PopupButtonID="txtStartDate"
                                        Format="dd/MM/yyyy">
                                    </ajaxToolkit:CalendarExtender>
                                </div>
                            </div>

                            <div id="divStartHour" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblStartHour" runat="server" Text="Hora Inicio" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtStartHour" runat="server" Width="50px"  MaxLength="5"
                                    ToolTip="Ingrese hora formato 24hrs." ValidationGroup="EditNew" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvStartHour" ControlToValidate="txtStartHour" ValidationGroup="EditNew"
                                    runat="server" ErrorMessage="Hora es requerido" Text=" * " />
                                    
                                    <asp:RegularExpressionValidator ID="revStartHour" runat="server" ControlToValidate="txtStartHour"
                                    ErrorMessage="Hora no es valida ej: 23:30" Display="Dynamic" 
                                    ValidationExpression="^[0-2]?[0-9]:[0-5][0-9]$" ValidationGroup="EditNew" Text=" * ">
                                   </asp:RegularExpressionValidator>
                                </div>
                            </div>  
                            <%-- end START DATE --%>

                            <%-- END DATE --%>
                            <div id="divEndDate" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblEndDate" runat="server" Text="Fecha Fin" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtEndDate" runat="server" Width="70px" 
                                    ValidationGroup="EditNew" ToolTip="Ingrese fecha." AutoPostBack="true" OnTextChanged="txtEndDate_TextChanged" />
                                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Fecha es requerido" Enabled="false"  />
                                
                                    <asp:RangeValidator ID="rvEndDate" runat="server" ControlToValidate="txtEndDate"
                                        ErrorMessage="Fecha debe estar entre 01-01-2000 y 31-12-2040" Text=" * " MinimumValue="01/01/2000"
                                        MaximumValue="31/12/2040" ValidationGroup="EditNew" Type="Date" Enabled="false" />
                                    <ajaxToolkit:CalendarExtender ID="calEndDate" CssClass="CalMaster" runat="server" 
                                        Enabled="True" FirstDayOfWeek="Sunday" TargetControlID="txtEndDate" PopupButtonID="txtEndDate"
                                        Format="dd/MM/yyyy">
                                    </ajaxToolkit:CalendarExtender>
                                </div>
                            </div>

                            <div id="divEndHour" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblEndHour" runat="server" Text="Hora Fin" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox SkinID="txtFilter" ID="txtEndHour" runat="server" Width="50px"  MaxLength="5"
                                    ToolTip="Ingrese hora formato 24hrs." ValidationGroup="EditNew" AutoPostBack="false" />
                                    <asp:RequiredFieldValidator ID="rfvEndHour" ControlToValidate="txtEndHour" ValidationGroup="EditNew"
                                    runat="server" ErrorMessage="Hora es requerido" Text=" * " Enabled="false" />
                                    
                                    <asp:RegularExpressionValidator ID="revEndHour" runat="server" ControlToValidate="txtEndHour"
                                    ErrorMessage="Hora no es valida ej: 23:30" Display="Dynamic" 
                                    ValidationExpression="^[0-2]?[0-9]:[0-5][0-9]$" ValidationGroup="EditNew" Text=" * " Enabled="false">
                                   </asp:RegularExpressionValidator>
                                </div>
                            </div>  
                            <%-- end END DATE --%>

                            <%-- Interval --%>
                            <div id="divFrequenceInterval" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFrequenceInterval" runat="server" Text="Frecuencia Intervalo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtFrequenceInterval" runat="server" MaxLength="10" Width="150" />    
                                    <asp:RegularExpressionValidator ID="revtxtFrequenceInterval" runat="server" ControlToValidate="txtFrequenceInterval"
	                                     ErrorMessage="Intervalo debe ser ser un número positivo" 
	                                     ValidationExpression="^[0-9]\d*$"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>
                            <%-- end Interval --%>

                            <%-- pagination --%>
                            <div id="divPagination" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPagination" runat="server" Text="Paginación" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPagination" runat="server" MaxLength="10" Width="150" />    
                                    <asp:RegularExpressionValidator ID="revtxtPagination" runat="server" ControlToValidate="txtPagination"
	                                     ErrorMessage="Paginación tiene que ser un número mayor a 0" 
	                                     ValidationExpression="^[1-9]\d*$"
	                                     ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>
                            </div>
                            <%-- end pagination --%>

                        </div>

                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>          

                    </div>
                </asp:Panel>
             </div>
         </ContentTemplate>
         <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
     </asp:UpdatePanel>

     <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />

    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este registro?" Visible="false" />
    <asp:Label ID="lblSearchByDescription" runat="server" Text="Descripción" Visible="false" /> 
    <asp:Label ID="lblSearchByName" runat="server" Text="Nombre" Visible="false" /> 
    <asp:Label ID="lblErrorDateFormat" runat="server" Text="Error en formato fecha" Visible="false" /> 
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
      <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
