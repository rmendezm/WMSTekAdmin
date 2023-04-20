<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="PrinterMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.PrinterMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("Printer_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);

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
        initializeGridDragAndDrop("Printer_FindAll", "ctl00_MainContent_grdMgr");
        initializeGridWithNoDragAndDrop(true);
    }
</script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <%-- Grilla Principal --%>
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                            <asp:GridView ID="grdMgr" 
                                    DataKeyNames="Id" 
                                    runat="server" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    OnRowDeleting="grdMgr_RowDeleting"  
                                    OnRowEditing="grdMgr_RowEditing" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    OnPageIndexChanging="grdMgr_PageIndexChanging"
                                    AllowPaging="True" 
                                    EnableViewState="false"
                                    AutoGenerateColumns="False"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="Id" />
                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="PrinterName" />
                                    <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                    <asp:TemplateField HeaderText="Cód. Centro" AccessibleHeaderText="WhsCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWhsCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Centro" AccessibleHeaderText="WhsName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblWhsName" runat="server" Text='<%# Eval ( "Warehouse.Name" ) %>' />
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="PrinterTypeName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPrinterType" runat="server" Text='<%# Eval ( "PrinterType.Name" ) %>' />
                                           </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                        
                                    <asp:TemplateField HeaderText="Servidor" AccessibleHeaderText="ServerName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblPrintServer" runat="server" Text='<%# Eval ( "PrintServer.ServerName" ) %>' />
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
                     <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
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
    <%-- FIN Grilla Principal --%>
          
    <%-- Pop up Editar/Nuevo Impresora --%>                  
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlVendor" BackgroundCssClass="modalBackground" PopupDragHandleControlID="VendorCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlVendor" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="VendorCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Impresora" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Impresora" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">    
                        <div runat="server" class="divCtrsFloatLeft" >                
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <ajaxToolkit:TabContainer runat="server" ID="tabPrinter" ActiveTabIndex="0" Height="350px" Width="100%">
                            <ajaxToolkit:TabPanel runat="server" ID="tabPrinterFeature" Width="100%">
                                <ContentTemplate>
                                    <%--Warehouse  --%>
                                    <div id="divWarehouse" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblWarehouse" runat="server" Text="Centro"></asp:Label></div>
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlWarehouse" runat="server" Width="120px" />
                                            <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                                InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro es requerido">
                                            </asp:RequiredFieldValidator>
                                        </div>
                                    </div>                        
                                    <%-- Name --%>
                                                        
                                    <div id="divName" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblName" runat="server" Text="Nombre" />
                                        </div>
                                        <div class="fieldLeft"> 
                                            <asp:TextBox ID="txtName" runat="server" Width="120px" MaxLength="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvNombrePrinter" runat="server" ControlToValidate="txtName"
                                             ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator>
                                            <%--<asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                                 ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator> --%>
                                           <%-- <asp:RegularExpressionValidator ID="revName" runat="server" ControlToValidate="txtName"
	                                            ErrorMessage="En el campo nombre debe ingresar solo letras de la A - Z o a - z ó números" 
	                                            ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ/\\.]*"
	                                            ValidationGroup="EditNew" Text=" * ">
                                            </asp:RegularExpressionValidator>            --%>                                                                  
                                        </div>
                                    </div>
                                    <%-- Descripción --%>
                                    <div id="divDescription" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                        <div class="fieldLeft">
                                            <asp:TextBox ID="txtDescription" runat="server" Width="120px" TextMode="MultiLine" />
                                            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" />
                                            <asp:RegularExpressionValidator ID="revTexbox3" runat="server"             
                                                   ErrorMessage="Campo Descripción debe ingresar hasta un maximo de 100 caracteres y debe ingresar solo letras de la A - Z o a - z ó números"            
                                                   ValidationExpression="^([\S\s]{0,100})$"   
                                                   ControlToValidate="txtDescription"      
	                                               ValidationGroup="EditNew" Text=" * "                                                 
                                                   Display="Dynamic">
                                            </asp:RegularExpressionValidator>                                                                             
                                        </div>
                                    </div>
                            
                                    <div id="divPrinterType" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblPrinterType" runat="server" Text="Tipo" />
                                        </div>
                                        <div class="fieldLeft"> 
                                            <asp:DropDownList runat="server" ID="ddlPrinterType"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvPrinterType" runat="server" ControlToValidate="ddlPrinterType"
                                            InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo de Impresora es requerido" />                                                              
                                        </div>
                                    </div>
                            
                                    <div id="divPrintServer" runat="server" class="divControls">
                                        <div class="fieldRight">
                                            <asp:Label ID="lblPrintServer" runat="server" Text="Servidor" />
                                        </div>
                                        <div class="fieldLeft"> 
                                            <asp:DropDownList runat="server" ID="ddlPrintServer"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvPrintServer" runat="server" ControlToValidate="ddlPrintServer"
                                            InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Servidor de Impresión es requerido" />                                                              
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel> 
                            <ajaxToolkit:TabPanel runat="server" ID="tabLabel" Width="100%">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                    <div class="divControls">
                                        <div class="fieldLeft">
                                            <asp:DropDownList ID="ddlLabel" runat="server" />
                                            <asp:Button ID="btnAddLabel" runat="server" Text="Asignar" OnClick="btnAddLabel_Click" />
                                        </div>   
                                        <div class="textLeft"> 
                                            <asp:Label ID="Label2" runat="server" Text="Etiquetas Asignadas:" />
                                            <br />
                                             <div style="overflow: auto; height: 300px; width: 250px;" >
                                                <asp:GridView ID="grdLabels" runat="server" ForeColor="#333333" OnRowDeleting="grdLabels_RowDeleting"
                                                DataKeyNames="IdLabel" OnRowCommand="grdLabels_RowCommand"
                                                    onrowdatabound="grdLabels_RowDataBound"
                                                    AutoGenerateColumns="False"
                                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop table-condensed" 
                                                    EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField DataField="IdLabel" ShowHeader="False" Visible="False" />
                                                    <asp:BoundField DataField="IdPrinter" ShowHeader="False" Visible="False" />
                                                    <asp:BoundField DataField="LabelName" HeaderText="Etiqueta" AccessibleHeaderText="LabelName" />
                                                    <asp:TemplateField HeaderText="Acción">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" ToolTip="Eliminar"/>
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                    <div style="clear: both"></div>   
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel> 
                        </ajaxToolkit:TabContainer>  
                        </div>
                        <div class="divValidationSummary" >
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                ShowMessageBox="false" CssClass="modalValidation"/>
                           <%-- <asp:ValidationSummary ID="vs" runat="server" ValidationGroup="EditNew" 
                                ShowMessageBox="false" CssClass="modalValidation"/>--%>
                        </div>                        
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
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

    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="muprEditNew" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditNew" />    
    <%-- FIN Pop up Editar/Nuevo impresora --%>
    
   <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Impresora?" Visible="false" />
    <asp:Label ID="lblLabelAsig" runat="server" Text="Etiqueta se encuentra asignada." Visible="false" />
    <asp:Label ID="lbltabPrinter" runat="server" Text="Datos Generales" Visible="false" />
    <asp:Label ID="lbltabLabel" runat="server" Text="Etiqueta" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>

