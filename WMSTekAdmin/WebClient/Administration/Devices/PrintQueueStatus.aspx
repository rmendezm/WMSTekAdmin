<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="PrintQueueStatus.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.PrintQueueStatus" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <script type="text/javascript" language="javascript">
        window.onresize = SetDivs; 
        var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(SetDivs);

            $(document).ready(function () {
                removeFooterTable();

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
                removeFooterTable();
            }
    </script>

    <script type="text/javascript">
        function ShowModalPopup() {
            var ModalPopup = '<%= modalEditMassiveTaskLabel.ClientID %>';
            $find(ModalPopup).show();
        }

        function removeFooterTable() {
            if ($("#ctl00_MainContent_grdMgr > tbody> tr:last-child table").length > 0) {
                $("#ctl00_MainContent_grdMgr > tbody> tr:last-child").remove();
            }
        }

    </script>

     <div class="container">
        <div class="row">
            <div class="col-md-12">    
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
	                         <%-- Grilla Principal --%>         
                             <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" 
                                OnRowDataBound="grdMgr_RowDataBound" 
                                OnRowEditing="grdMgr_RowEditing" 
                                OnRowCreated="grdMgr_RowCreated"
                                AllowPaging="True" 
                                EnableViewState="false"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                        
                                     <Columns>
                                         <asp:TemplateField AccessibleHeaderText="Seleccions">
                                            <HeaderTemplate>
                                                <div style="display:inline; background-color: white; padding:2px 2px 2px 2px;">
                                                    <a title="Editar Masivo"><img alt="" src="../../WebResources/Images/Buttons/GridActions/icon_edit.png" style="cursor:pointer;" onclick="ShowModalPopup();"/></a> 
                                                 </div><br />
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkAdjustConfirm', this.checked)" id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkAdjustConfirm" runat="server" Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                
                                        <asp:TemplateField HeaderText="ID" AccessibleHeaderText="IdTaskLabel" SortExpression="IdTaskLabel" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblId" runat="server" Text='<%# Eval ( "Id" ) %>' ></asp:Label>
                                                    </div> 
                                                </center>
                                            </itemtemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Código" AccessibleHeaderText="LabelCode" SortExpression="LabelCode" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCode" runat="server" Text='<%# Eval ( "LabelTemplate.Code" ) %>' ></asp:Label>
                                                    </div> 
                                                </center>
                                            </itemtemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Fecha Creación" AccessibleHeaderText="DateCreated" SortExpression="DateCreated" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblDateCreated" runat="server" Text='<%# Eval ( "DateCreated", "{0:d}" ) %>' ></asp:Label>
                                                    </div> 
                                                </center>
                                            </itemtemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Usuario" AccessibleHeaderText="UserName" SortExpression="UserName" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblUser" runat="server" Text='<%# Eval ( "User.FirstName" ) + " " + Eval ( "User.LastName" ) %>' ></asp:Label>
                                                    </div> 
                                                </center>
                                            </itemtemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Tipo Etiqueta" AccessibleHeaderText="LabelName" SortExpression="LabelName" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblLabelName" runat="server" Text='<%# Eval ( "LabelTemplate.Name" ) %>' ></asp:Label>
                                                    </div> 
                                                </center>
                                            </itemtemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Cliente" AccessibleHeaderText="CustomerName" SortExpression="CustomerName" ItemStyle-CssClass="text">
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>' ></asp:Label>
                                                    </div> 
                                                </center>
                                            </itemtemplate>
                                         </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 60px">
                                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                            CausesValidation="false" CommandName="Edit" />                                                   
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                             
                                     </Columns>
                                  </asp:GridView>
                             <%-- FIN Grilla Principal --%>             
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
                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" /> 
                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" />    
                  </Triggers>
                </asp:UpdatePanel>    
            </div>
        </div>
    </div>    
    
    <%-- Panel Cerrar Auditoria --%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">       
        <ContentTemplate>   
            <div id="divMessaje" runat="server" visible="false" class="divItemDetails" >
                <asp:Button ID="btnDummy2" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpMessaje" runat="server" TargetControlID="btnDummy2"
                    PopupControlID="pnlMessaje" BackgroundCssClass="modalBackground" PopupDragHandleControlID="UserCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlMessaje" runat="server" CssClass="modalBox" Width="430px" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="UserCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblTitleMessaje" runat="server" Text="Motivo Confirmación" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    
                    
                    <div class="divCtrsFloatLeft">       
                    </div>
                    <div style="clear:both" />                       
                    <div id="Div1" runat="server" class="modalActions">
                        <asp:Button ID="btnSaveConfirm" runat="server"  Text="Aceptar" CausesValidation="true" 
                          OnClick="btnSaveConfirm_Click" />
                    </div>                        
                </asp:Panel>
            </div>
            
            <div id="divConfirmPrin" runat="server">
                <asp:Button ID="btnDialogDummy" runat="Server" Style="display: none" /> 
                <ajaxToolKit:ModalPopupExtender 
	                ID="modalPopUpDialog" runat="server" TargetControlID="btnDialogDummy" 
	                PopupControlID="pnlDialog"  
	                BackgroundCssClass="modalBackground" 
	                PopupDragHandleControlID="Caption" Drag="true" >
	            </ajaxToolKit:ModalPopupExtender>
	            <asp:Panel ID="pnlDialog" runat="server" CssClass="modalBox" Width="400px">    	
		            <%-- Encabezado --%>    			
		            <asp:Panel ID="DialogHeader" runat="server" CssClass="modalHeader">
			            <div class="divCaption">
			                <asp:Label ID="lblDialogTitle" runat="server" />			    
                        </div>
	                </asp:Panel>
            	    
                    <div id="divDialogPanel" class="divDialogPanel" runat="server">
                        <div class="divDialogMessage">
                            <asp:Image id="imgDialogIcon" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
                        </div>
                        <div id="divDialogMessage" runat="server" class="divDialogMessage">                          
                        </div>
                        <div id="divConfirm" runat="server" class="divDialogButtons">
                            <asp:Button ID="btnOk" runat="server" Text="   Sí   " OnClick="btnOk_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="   No   " OnClick="btnCancel_Click" />
                        </div> 
                    </div>                     
                 </asp:Panel>
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
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnSave" EventName="Click" /> 
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnSaveConfirm" EventName="Click" />   
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnOk" EventName="Click" /> 
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$btnCancel" EventName="Click" />   
        </Triggers>
    </asp:UpdatePanel>
    <%-- FIN Panel Cerrar Auditoria --%>


    <%-- Pop up Editar Tarea Impresion --%>                  
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <div id="divEditNew" runat="server" visible="false">        
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlTaskLabel" BackgroundCssClass="modalBackground" PopupDragHandleControlID="TaskLabelCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlTaskLabel" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="TaskLabelCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEdit" runat="server" Text="Agregar Dato Etiqueta" />
                            <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">                    
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidParamString" runat="server" Value="" />
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                            
                            <div id="divCustomerName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCustomerName" runat="server" Text="Cliente"></asp:Label>
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtCustomerName" runat="server" Width="200px" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <div id="divTypeLabel" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTypeLabel" runat="server" Text="Tipo Etiqueta"></asp:Label>
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtTypeLabel" runat="server" Width="120px" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <div id="divDateCreated" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDateCreated" runat="server" Text="Fecha Creación"></asp:Label>
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtDateCreated" runat="server" Width="120px" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <%-- ParamName --%>                                                        
                            <div id="divParamName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParamName" runat="server" Text="Nombre" />
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:TextBox ID="txtParamName" runat="server" Width="120px" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvParamName" runat="server" ControlToValidate="txtParamName"
                                     ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revParamName" runat="server" ControlToValidate="txtParamName"
	                                    ErrorMessage="En el campo nombre debe ingresar solo letras de la A - Z o a - z ó números" 
	                                    ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                              
                                </div>
                            </div>
                            <%-- ParamValue --%>
                            <div id="divParamValue" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParamValue" runat="server" Text="Valor" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtParamValue" runat="server" Width="120px" MaxLength="100" />
                                    <asp:RequiredFieldValidator ID="rfvParamValue" runat="server" ControlToValidate="txtParamValue"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Valor es requerido" />
                                    <asp:RegularExpressionValidator ID="revParamValue" runat="server"  ControlToValidate="txtParamValue"            
                                            ErrorMessage="En el campo valor debe ingresar solo letras de la A - Z o a - z ó números" 
	                                    ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                    ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>                                                                             
                                </div>
                            </div>

                            <div id="divParamString" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDataLabel" runat="server" Text="Datos Etiqueta"></asp:Label>
                                </div>
                                <div class="fieldLeft"> 
                                    <asp:ListBox ID="lwParamString" runat="server" Width="280" Height="220" 
                                        SelectionMode="Multiple" Font-Bold="true" BackColor="#ffffcc" >
                                    </asp:ListBox>
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
                            <asp:Button ID="Button1" runat="server" Text="Cancelar" />
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
    <%-- FIN Pop up Editar Tarea Impresion --%>

    <%-- Pop up Editar Tarea Impresion Masiva --%>
    <asp:UpdatePanel ID="upEditMassiveTaskLabel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divEditMassiveTaskLabel" runat="server" visible="true">
                <asp:Button ID="btnEditMassiveTaskLabel" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalEditMassiveTaskLabel" runat="server" TargetControlID="btnEditMassiveTaskLabel"
                    PopupControlID="panelEditMassiveTaskLabel" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionEditMassiveTaskLabel"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelEditMassiveTaskLabel" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionEditMassiveTaskLabel" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEditMassiveTaskLabel" runat="server" Text="Edicion Masiva Tareas Etiqueta" />
                            <asp:ImageButton ID="ImageButton2" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">

                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParamNameMassive" runat="server" Text="Nombre" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtParamNameMassive" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvParamNameMassive" runat="server" ControlToValidate="txtParamNameMassive"
                                     ValidationGroup="EditNewMassive" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revParamNameMassive" runat="server" ControlToValidate="txtParamNameMassive"
	                                    ErrorMessage="En el campo nombre debe ingresar solo letras de la A - Z o a - z ó números" 
	                                    ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                    ValidationGroup="EditNewMassive" Text=" * ">
                                    </asp:RegularExpressionValidator>    
                                </div>
                            </div> 
                            
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblParamValueMassive" runat="server" Text="Valor" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:TextBox ID="txtParamValueMassive" runat="server" MaxLength="100" />
                                    <asp:RequiredFieldValidator ID="rfvParamValueMassive" runat="server" ControlToValidate="txtParamValueMassive"
                                        ValidationGroup="EditNewMassive" Text=" * " ErrorMessage="Valor es requerido" />
                                    <asp:RegularExpressionValidator ID="revParamValueMassive" runat="server"  ControlToValidate="txtParamValueMassive"            
                                            ErrorMessage="En el campo valor debe ingresar solo letras de la A - Z o a - z ó números" 
	                                    ValidationExpression="[a-zA-Z 0-9ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ]*"
	                                    ValidationGroup="EditNewMassive" Text=" * ">
                                    </asp:RegularExpressionValidator>     
                                </div>  
                            </div>
					  </div>
					  <div style="clear: both"></div>
					  <div class="divValidationSummary">
						  <asp:ValidationSummary ID="vsEditNewMassive" runat="server" ValidationGroup="EditNewMassive" ShowMessageBox="false" CssClass="modalValidation" />
					  </div>                            
					  <div id="div3" runat="server" class="modalActions">
						  <asp:Button ID="btnAcceptEditMassiveTaskLabel" runat="server" Text="Aceptar" ValidationGroup="EditNewMassive" OnClick="btnAcceptEditMassiveTaskLabel_Click" />
						  <asp:Button ID="btnCancelEditMassiveTaskLabel" runat="server" Text="Cancelar" />
					  </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprEditMassiveTaskLabel" runat="server" AssociatedUpdatePanelID="upEditMassiveTaskLabel" DisplayAfter="20" DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprEditMassiveTaskLabel" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprEditMassiveTaskLabel" />
    <%-- Fin Pop up Editar Tarea Impresion Masiva --%>

    <asp:Label ID="lblConfirmAdjustHeader" runat="server" Text="Confirmar impresión" Visible ="false"  />
    <asp:Label ID="lblNotSelectedAdjust" runat="server" Text="No existen tareas de impresión seleccionados" Visible ="false"  />
    <asp:Label ID="lblConfirmAdjust" runat="server" Text="¿Desea Confirmar que las tareas se impriman?" Visible ="false"  />
    <asp:Label ID="lblMissCustomer" runat="server" Text="Debe ingresar nombre de cliente" Visible ="false"  />
    <asp:Label ID="lblFilterCodeLabel" runat="server" Text="Lpn" Visible="false" />
    <asp:Label ID="lblFilterCustomerLabel" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label ID="lblValidateEditMassive" runat="server" Text="Debe seleccionar al menos una tarea" Visible="false" />
    <asp:Label ID="lblEditMassiveSuccess" runat="server" Text="Modificación masiva exitosa" Visible="false" />
    <asp:Label ID="LblRadiobuttonFalse" runat="server" Text="Precio" Visible="false" />
    <asp:Label ID="LblRadiobutton" runat="server" Text="Lpn" Visible="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
     <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>
</asp:Content>
