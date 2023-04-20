<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="BillingTypeContractMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Billing.BillingTypeContractMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
    
        $(document).ready(function () {
            initializeGridDragAndDrop("BillingTypeContract_FindAll", "ctl00_MainContent_grdMgr");

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
            initializeGridDragAndDrop("BillingTypeContract_FindAll", "ctl00_MainContent_grdMgr");
        }
    </script>

    <link href="<%= Page.ResolveClientUrl("~/WebResources/Styles/CalendarPopUp.css")%>" rel="stylesheet" type="text/css" />

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
                        
                                    <asp:TemplateField HeaderText="Owner" AccessibleHeaderText="OwnerName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblOwnerName" runat="server" Text='<%# Eval ("BillingContract.Owner.Name") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ("Warehouse.Name") %>' Width="170px"/>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Descripción Contrato" AccessibleHeaderText="ContractDescription">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblContractDescription" runat="server" Text='<%# Eval ("BillingContract.Description") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Transaccion" AccessibleHeaderText="TransactionName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblTransactionName" runat="server" Text='<%# Eval ("BillingTransaction.Name") %>' Width="170px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Cobro" AccessibleHeaderText="TypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblTypeName" runat="server" Text='<%# Eval ("BillingType.Name") %>' Width="200px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Moneda" AccessibleHeaderText="MoneyDescription">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblMoneyDescription" runat="server" Text='<%# Eval ("BillingMoney.Description") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Tiempo" AccessibleHeaderText="TimeTypeDescription">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblTimeTypeDescription" runat="server" Text='<%# Eval ("BillingTimeType.Description") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Valor" AccessibleHeaderText="Value">
                                        <ItemTemplate>
                                            <div style="word-wrap:break-word;">
                                                <asp:Label ID="lblValue" runat="server" Text='<%# Eval ("Value") %>' Width="120px" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:templatefield headertext="Inicio" accessibleHeaderText="StartTime">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblStartTime" runat="server" text='<%# ((String) Eval ("BillingTransaction.Type") != "N") ? ((String) Eval ("BillingTransaction.Type") == "C") ? Eval("StartTime", "{0:HH:mm}") : ((DateTime)Eval("StartTime")).ToShortDateString() == "01/01/0001" ? "" : Eval("StartTime", "{0:d}"):(((DateTime) Eval ("StartTime") > DateTime.MinValue)? ((DateTime)Eval("StartTime")).ToShortDateString() == "01/01/0001" ? "" : Eval("StartTime", "{0:HH:mm}"):"") %>' />
                                                </div>
                                            </center>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:templatefield headertext="Termino" accessibleHeaderText="EndTime">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblEndTime" runat="server" text='<%# ((String) Eval ("BillingTransaction.Type") != "N") ? ((String) Eval ("BillingTransaction.Type") == "C") ? Eval("EndTime", "{0:HH:mm}")  : ((DateTime)Eval("EndTime")).ToShortDateString() == "01/01/0001" ? "" : Eval("EndTime", "{0:d}"):(((DateTime) Eval ("EndTime") > DateTime.MinValue)? ((DateTime)Eval("EndTime")).ToShortDateString() == "01/01/0001" ? "" : Eval("EndTime", "{0:HH:mm}") : "") %>' />
                                                </div>
                                            </center>    
                                        </itemtemplate>
                                    </asp:templatefield>

                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <center>
                                                <div style="width:100px">
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
                    PopupControlID="pnlBillingTypeContract" BackgroundCssClass="modalBackground" PopupDragHandleControlID="BillingTypeContractCaption" Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="PnlBillingTypeContract" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Configuración de Cobro" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Configuración de Cobro" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">

                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOwner" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwner" AutoPostBack="true" OnSelectedIndexChanged="ddlOwner_SelectedIndexChanged" AppendDataBoundItems="true">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwner" runat="server" ControlToValidate="ddlOwner" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>

                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist." />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlWarehouse" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro Dist. es requerido" />
                                </div>
                            </div>

                            <div id="divContract" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblContract" runat="server" Text="Contrato" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlContract" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvContract" runat="server" ControlToValidate="ddlContract" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Contrato es requerido" />
                                </div>
                            </div>

                            <div id="divTimeType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTimeType" runat="server" Text="Tiempo" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlTimeType" AutoPostBack="true" OnSelectedIndexChanged="ddlTimeType_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTimeType" runat="server" ControlToValidate="ddlTimeType" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Tiempo es requerido" />
                                </div>
                            </div>

                            <div id="divTransaction" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTransaction" runat="server" Text="Transacción" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlTransaction" AutoPostBack="true" OnSelectedIndexChanged="ddlTransaction_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvTransaction" runat="server" ControlToValidate="ddlTransaction" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Transacción es requerido" />
                                </div>
                            </div>

                            <div id="divType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblType" runat="server" Text="Tipo Cobro" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlType" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="ddlType" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Cobro es requerido" />
                                </div>
                            </div>

                            <div id="divMoney" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblMoney" runat="server" Text="Moneda" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlMoney" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvMoney" runat="server" ControlToValidate="ddlMoney" InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Moneda es requerida" />
                                </div>
                            </div>

                            

                            <div id="divValue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblValue" runat="server" Text="Valor" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtValue" runat="server" MaxLength="12" />
                                    <asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Valor es requerido" />
                                    <asp:RangeValidator ID="rvValue" runat="server" ControlToValidate="txtValue" ValidationGroup="EditNew" Text=" * " ErrorMessage="Ingrese valores numericos" Type="Double" />
                                </div>
                            </div>

                            <%-- Start Date --%>  
                            <div id="divTimeDate" runat="server" class="divControls">
                            <div id="divStartDate" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblStartDate" runat="server" Text="Inicio"></asp:Label></div>
                                <div class="fieldLeft">
	                            <table cellpadding="0" cellspacing="0">  
    	                            <tr>
	                                    <td rowspan="2">              
                                            <asp:TextBox ID="txtStartDate" runat="server" Width="73px" />
                                    
                                            <ajaxToolkit:CalendarExtender ID="calStartDate" 
                                 CssClass="CalMaster"
                                    runat="server" 
                                    Enabled="true" 
                                    FirstDayOfWeek="Sunday" 
                                    TargetControlID="txtStartDate"
                                    PopupButtonID="txtStartDate" Format="dd/MM/yyyy" />
                                            <asp:requiredfieldvalidator ID="rfvStartDate" runat="server"  ValidationGroup="EditNew" Text=" * " controltovalidate="txtStartDate" display="Static" ErrorMessage="Inicio es requerido"/>&nbsp;&nbsp;
                                            <asp:RangeValidator ID="rvStartDate" runat="server" ControlToValidate="txtStartDate" ErrorMessage="Inicio debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />
                                        </td>
                                     </tr>
                                  </table>
                                </div>
                            </div>
                            
	                        <%-- End Date --%>  
                            <div id="divEndDate" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblEndDate" runat="server" Text="Fin"></asp:Label></div>
                                
                                <%-- Date --%> 
                                <div class="fieldLeft">
                                <table cellpadding="0" cellspacing="0">  
    	                            <tr >
	                                    <td rowspan="2">              
                                            <asp:TextBox ID="txtEndDate" runat="server" Width="73px"  />
                                    
                                            <ajaxToolkit:CalendarExtender ID="calEndDate" 
                                 CssClass="CalMaster"
                                    runat="server" 
                                    Enabled="true" 
                                    FirstDayOfWeek="Sunday" 
                                    TargetControlID="txtEndDate"
                                    PopupButtonID="txtEndDate" Format="dd/MM/yyyy" />
                                            <asp:requiredfieldvalidator ID="rfvEndDate" runat="server"  ValidationGroup="EditNew" Text=" * " controltovalidate="txtEndDate" display="Static" ErrorMessage="Término es requerido"/>&nbsp;&nbsp;
                                            <asp:RangeValidator ID="rvEndDate" runat="server" ControlToValidate="txtEndDate" ErrorMessage="Término debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />                                        
                                        </td>
                                     </tr>
                                  </table>                
                                </div>
                            </div>   

                            <%-- Start Time --%>  
                            <div id="divStartTime" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblStartTime" runat="server" Text="Inicio" /></div>
                                
                                <%-- Date --%>  
                                <div class="fieldLeft">
	                            <table cellpadding="0" cellspacing="0">  
    	                            <tr>
	                                    <td rowspan="2">              
                                            <%-- Hours --%>  
                                            <asp:TextBox ID="txtStartTimeHours" runat="server" Width="22" MaxLength="2" Visible="true"/>
                                            <asp:Label ID="lblSeparatorStartTime" runat="server">:</asp:Label>
                                            <%-- Minutes --%>  
                                            <asp:TextBox ID="txtStartTimeMinutes" runat="server" Width="22px" MaxLength="2" Visible="true" />
                                        </td>
                                        <td rowspan="2">
                                            <asp:RequiredFieldValidator ID="rfvStartTimeHours" runat="server" ControlToValidate="txtStartTimeHours"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Hora Inicio es requerido" />
                                            <asp:RangeValidator Type="Integer" ID="rangeStartTimeHours" ControlToValidate="txtStartTimeHours" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Hora debe estar entre 0 y 23" MinimumValue="0" MaximumValue="23" />&nbsp;&nbsp;
                                            
                                        </td>                        
                                        <td rowspan="2" >
                                            <asp:RequiredFieldValidator ID="rfvStartTimeMinutes" runat="server" ControlToValidate="txtStartTimeMinutes"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Minuto Inicio es requerido" />
                                            <asp:RangeValidator Type="Integer" ID="rangeStartTimeMinutes" ControlToValidate="txtStartTimeMinutes" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Minutos debe estar entre 0 y 59" MinimumValue="0" MaximumValue="59" />
                                        </td>                        
                                     </tr>
                                  </table>
                                </div>
                            </div>

                            <%-- End Time --%>  
                            <div id="divEndTime" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblEndTime" runat="server" Text="Fin" /></div>
                                
                                <%-- Date --%>  
                                <div class="fieldLeft">
	                            <table cellpadding="0" cellspacing="0">  
    	                            <tr>
	                                    <td rowspan="2">              
                                            <%-- Hours --%>  
                                            <asp:TextBox ID="txtEndTimeHours" runat="server" Width="22px" MaxLength="2" Visible="true"/>
                                            <asp:Label ID="lblSeparatorEndTime" runat="server">:</asp:Label>
                                            <%-- Minutes --%>  
                                            <asp:TextBox ID="txtEndTimeMinutes" runat="server" Width="22px" MaxLength="2" Visible="true" />
                                        </td>
                                        <td rowspan="2">
                                            <asp:RequiredFieldValidator ID="rfvEndTimeHours" runat="server" ControlToValidate="txtEndTimeHours"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Hora Fin es requerido" />
                                            <asp:RangeValidator Type="Integer" ID="rangeEndTimeHours" ControlToValidate="txtEndTimeHours" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Hora debe estar entre 0 y 23" MinimumValue="0" MaximumValue="23" />&nbsp;&nbsp;
                                            
                                        </td>                        
                                        <td rowspan="2" >
                                            <asp:RequiredFieldValidator ID="rfvEndTimeMinutes" runat="server" ControlToValidate="txtEndTimeMinutes"
                                            ValidationGroup="EditNew" Text=" * " ErrorMessage="Minuto Fin es requerido" />
                                            <asp:RangeValidator Type="Integer" ID="rangeEndTimeMinutes" ControlToValidate="txtEndTimeMinutes" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Minutos debe estar entre 0 y 59" MinimumValue="0" MaximumValue="59" />
                                        </td>                        
                                     </tr>
                                  </table>
                                </div>
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Tipo de Contrato?" Visible="false" />
    <asp:Label ID="lblName" runat="server" Text="Contrato" Visible="false" />
    <asp:Label ID="lblDescription" runat="server" Text="Transacción" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
        
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- FIN Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>