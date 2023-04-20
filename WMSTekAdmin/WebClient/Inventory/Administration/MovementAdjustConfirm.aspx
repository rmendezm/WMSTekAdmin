<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MovementAdjustConfirm.aspx.cs" MasterPageFile="~/Shared/WMSTekContent.Master" Inherits="Binaria.WMSTek.WebClient.Inventory.Administration.MovementAdjustConfirm" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("MovementAdjust_GetToConfirm", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("MovementAdjust_GetToConfirm", "ctl00_MainContent_grdMgr");
    }
</script>
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                 <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
	                         <%-- Grilla Principal --%>         
                             <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" AutoGenerateColumns="false" 
                                OnRowDataBound="grdMgr_RowDataBound" AllowPaging="True"
                                EnableViewState="False" OnRowCreated="grdMgr_RowCreated"
                                OnRowDeleting="grdMgr_RowDeleting"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                        
                                     <Columns>
                                         <asp:TemplateField HeaderText="Checkbox" AccessibleHeaderText="Checkbox">
                                            <HeaderTemplate>
                                                <%--<asp:CheckBox ID="chkSelectAll" runat="server" onclick="CheckAllEmp(this);" />--%>
                                                <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkAdjustConfirm', this.checked)" id="chkAll" title="Seleccionar todos" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkAdjustConfirm" runat="server" Visible="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                            <ItemTemplate>
                                                <center>
                                                    <div style="width: 60px">
                                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                            CommandName="Delete" ToolTip="Eliminar" />
                                                    </div>
                                                </center>
                                            </ItemTemplate>
                                         </asp:TemplateField>

                                         <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" AccessibleHeaderText="Id"
                                            SortExpression="Id">
                                            <ItemStyle Wrap="False"></ItemStyle>
                                         </asp:BoundField>
                                                 
                                         <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode" SortExpression="WarehouseCode" ItemStyle-CssClass="text">
                                             <itemtemplate>
                                                <center>
                                                 <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' ></asp:Label>
                                                 </div> 
                                                 </center>
                                             </itemtemplate>
                                         </asp:TemplateField>
                             
                                         <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse" SortExpression="Warehouse">
                                             <itemtemplate>
                                                <center>
                                                 <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' ></asp:Label>
                                                </div>
                                                 </center>
                                             </itemtemplate>
                                         </asp:TemplateField>
                                                  
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode" ItemStyle-CssClass="text">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnerTradeName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                                </div>  
                                            </ItemTemplate>
                                        </asp:TemplateField>          
                            
                                        <asp:TemplateField HeaderText="Movimiento" AccessibleHeaderText="MovementName">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblMovementName" runat="server" text='<%# Eval ( "MovementType.Name" ) %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                                       

                                        <asp:templatefield headertext="Fecha" accessibleHeaderText="AdjustDate" SortExpression="AdjustDate">
                                            <ItemStyle Wrap="false" />
                                            <itemtemplate>
                                                <center>
                                                    <div style="word-wrap: break-word;">
                                                        <asp:label ID="lblAdjustDate" runat="server"  
                                                        text='<%# ((DateTime) Eval ("AdjustDate") > DateTime.MinValue)? Eval("AdjustDate", "{0:d}"):"" %>' />
                                                    </div>
                                                </center>    
                                            </itemtemplate>
                                        </asp:templatefield>  
                             
                                        <asp:templatefield headertext="Ubicación" accessibleHeaderText="IdLocCode" SortExpression="IdLocCode">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblIdLocCode" runat="server" text='<%# Eval ("IdLocCode") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>            
                            
                                        <asp:templatefield headertext="Lpn" accessibleHeaderText="IdLpnCode" SortExpression="IdLpnCode">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblIdLpnCode" runat="server" text='<%# Eval ("IdLpnCode") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>            
                            
                                        <asp:templatefield headertext="IdItem" accessibleHeaderText="IdItem" SortExpression="IdItem">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblIdItem" runat="server" text='<%# Eval ("Item.Id") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>            
                            
                                        <asp:templatefield headertext="Código" accessibleHeaderText="ItemCode" SortExpression="ItemCode">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblItemCode" runat="server" text='<%# Eval ("Item.Code") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>

                                        <asp:templatefield headertext="Descripción" accessibleHeaderText="Description" SortExpression="Description">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblDescription" runat="server" text='<%# Eval ("Item.Description") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>
                             
                                        <asp:templatefield headertext="Lote" accessibleHeaderText="LotNumber" SortExpression="LotNumber">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblLotNumber" runat="server" text='<%# Eval ("LotNumber") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>            

                                        <asp:templatefield headertext="Código Bloqueo" accessibleHeaderText="ReasonCode" SortExpression="ReasonCode">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblReasonCode" runat="server" text='<%# Eval ("ReasonCode") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>            

                                        <asp:templatefield headertext="Descripción Bloqueo" accessibleHeaderText="ReasonName" SortExpression="ReasonName">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblReasonName" runat="server" text='<%# Eval ("Reason.Name") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>       

                                        <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblItemQty" runat="server" 
                                                        text='<%# GetFormatedNumber(Eval("ItemQty"))%>'>
                                                    </asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField> 
                             
                                        <asp:templatefield headertext="Usuario" accessibleHeaderText="UserName" SortExpression="UserName">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblUserName" runat="server" text='<%# Eval ("UserName") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>   

                                       <asp:templatefield headertext="Categoria" accessibleHeaderText="CategoryItemName" SortExpression="CategoryItemName">
                                            <itemtemplate>
                                                <div style="word-wrap: break-word;">
                                                   <asp:label ID="lblCategoryItemName" runat="server" text='<%# Eval ("CategoryItem.Name") %>' />
                                                </div>
                                            </itemtemplate>
                                        </asp:templatefield>   
                             
                               
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
                     <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$grdMgr$ctl11$btnConfirm" EventName="Click" />--%>
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
     
    <%-- Mensajes de Confirmacion y Auxiliares --%>
	<asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />   
    <asp:Label id="lblConfirmClose" runat="server" Text="¿Desea confirmar este Ajuste?" Visible="false" /> 	
    <asp:Label ID="lblConfirmAdjustHeader" runat="server" Text="Confirmar Ajuste" Visible ="false"  />
    <asp:Label ID="lblBtnSaveToolTip" runat="server" Text="Confirmar Ajuste" Visible ="false"  />
    <asp:Label ID="lblNotSelectedAdjust" runat="server" Text="No existen ajustes seleccionados" Visible ="false"  />
    <asp:Label ID="lblConfirmAdjust" runat="server" Text="¿Desea Confirmar Ajustes Seleccionados?" Visible ="false"  />
    <asp:Label ID="lblConfirmedAdjust" runat="server" Text="Ajustes Confirmados: " Visible ="false"  />
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Ajuste?" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  
    
    <script type="text/javascript" language="javascript">
        function ViewDetail(grd) {
            var index = grd.parentElement.parentElement.parentElement.parentElement.rowIndex;
            var btnDetail = document.getElementById("ctl00$MainContent$btnDetail");
            document.getElementById('ctl00$MainContent$hdIndexGrd').value = index-1;
            
            btnDetail.click();
            return false;
        }
        function CheckAllEmp(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdMgr.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                GridVwHeaderChckbox.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }
        function resizeDivPrincipal() {
            //debugger;
            var h = document.body.clientHeight + "px";
            var w = document.body.clientWidth + "px";
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
            document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
        }
    </script>   
</asp:Content>
<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webuc:ucstatus id="ucStatus" runat="server"/>     
</asp:Content>