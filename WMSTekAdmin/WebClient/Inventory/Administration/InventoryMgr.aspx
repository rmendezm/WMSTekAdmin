<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="InventoryMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Inventory.InventoryMgr" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);

    $(document).ready(function () {
        initializeGridDragAndDrop("Inventory_FindAll", "ctl00_MainContent_grdMgr", "InventoryMgr");

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
        initializeGridDragAndDrop("Inventory_FindAll", "ctl00_MainContent_grdMgr", "InventoryMgr");
    }

    function confirmUploadFile() {
        if (confirm("¿Confirma la carga de archivo de inventario manual?")) {
            showProgress();
            return true;
        } else {
            return false;
        }
    }

    function ShowMessage(title, message) {

        var position = (document.body.clientWidth - 400) / 2 + "px";
        document.getElementById("divFondoPopup").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.display = 'block';
        document.getElementById("ctl00_MainContent_divMensaje").style.marginLeft = position;

        document.getElementById("ctl00_MainContent_lblDialogTitle").innerHTML = title;
        document.getElementById("ctl00_MainContent_divDialogMessage").innerHTML = message;

        return false;
    }
</script>

     <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <%-- Grilla Principal --%>
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivs();" >
                        <asp:GridView ID="grdMgr" 
                                 runat="server"    
                                 DataKeyNames="Id"
                                 OnRowDeleting="grdMgr_RowDeleting" 
                                 OnRowEditing="grdMgr_RowEditing"
                                 OnRowDataBound="grdMgr_RowDataBound"    
                                 OnRowCreated="grdMgr_RowCreated"             
                                 AllowPaging="True"          
                                 EnableViewState="false"
                                 AllowSorting="false" 
                                 onrowcommand="grdMgr_RowCommand"
                                 AutoGenerateColumns="false"
                                 CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                 EnableTheming="false">
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" accessibleHeaderText="Id"/>
                        
                                    <asp:templatefield HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblWarehouseCode" runat="server" text='<%# Eval ( "Warehouse.Code" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>
                                         
                                    <asp:templatefield HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblWarehouse" runat="server" text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>      
                         
                                    <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerCode" runat="server" text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblOwnerTradeName" runat="server" text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                         
                                    <asp:BoundField DataField="Number" HeaderText="Nº Inv."  AccessibleHeaderText="Number" ItemStyle-HorizontalAlign="Center"/>
                
                                    <asp:templatefield headertext="Creado" accessibleHeaderText="CreateDate">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblCreateDate" runat="server" text='<%# ((DateTime) Eval ("CreateDate") > DateTime.MinValue)? Eval("CreateDate", "{0:d}"):"" %>' />
                                                </div>
                                            </center>    
                                        </itemtemplate>
                                    </asp:templatefield>           
                        
                                    <asp:templatefield headertext="Inicio" accessibleHeaderText="StartDate" SortExpression="StartDate">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblStartDate" runat="server" text='<%# ((DateTime) Eval ("StartDate") > DateTime.MinValue)? Eval("StartDate", "{0:dd-MM-yyyy HH:mm}"):"" %>' />
                                                 </div>
                                            </center>    
                                    </itemtemplate>
                                    </asp:templatefield>    
                          
                                    <asp:templatefield headertext="Término" accessibleHeaderText="EndDate" SortExpression="EndDate">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:label ID="lblEndDate" runat="server" text='<%# ((DateTime) Eval ("EndDate") > DateTime.MinValue)? Eval("EndDate", "{0:dd-MM-yyyy HH:mm}"):"" %>' />
                                                </div>
                                            </center>    
                                    </itemtemplate>
                                    </asp:templatefield>             
                        
                                    <asp:templatefield headertext="Centro Compl." AccessibleHeaderText="IsFullWhs">
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkIsFullWhs" runat="server" checked='<%# Eval ( "IsFullWhs" ) %>' Enabled="false"/>
                                                </div>
                                            </center>                        
                                        </itemtemplate>
                                    </asp:templatefield>
                                
                                     <asp:templatefield headertext="Traza" accessibleHeaderText="TrackInventoryType">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblTrackInventoryType" runat="server" text='<%# Eval ( "TrackInventoryType.Name" ) %>' />
                                           </div>
                                        </itemtemplate>
                                     </asp:templatefield>  

                                     <asp:templatefield headertext="Creador" accessibleHeaderText="UserCreated">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblUserCreated" runat="server" text='<%# Eval ( "UserCreate.UserName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>  
                       
                                     <asp:templatefield headertext="Aprobador" accessibleHeaderText="UserApproval">
                                        <itemtemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:label ID="lblUserApproval" runat="server" text='<%# Eval ( "UserApproval.UserName" ) %>' />
                                            </div>
                                        </itemtemplate>
                                     </asp:templatefield>                  
                        
                                     <asp:BoundField DataField="CountQty" HeaderText="Cant. Conteos" accessibleHeaderText="CountQty" ItemStyle-HorizontalAlign="Center"/>
                         
                                    <asp:templatefield headertext="Activo" accessibleHeaderText="Status" SortExpression="Status">
                                        <ItemStyle Wrap="false" />
                                        <itemtemplate>
                                            <center>
                                                <div style="word-wrap: break-word;">
                                                    <asp:CheckBox ID="chkStatus" runat="server" checked='<%# Eval ( "Status" ) %>' Enabled="false"/>
                                               </div>
                                            </center>    
                                    </itemtemplate>
                                    </asp:templatefield>                   
                    
                                    <asp:BoundField DataField="Description" HeaderText="Descripción"  AccessibleHeaderText="Description"/>
                    
                                   <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                        <ItemTemplate> 
                                            <div style="width:170px">
                                                <center>
	                                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png" CausesValidation="false" CommandName="Edit" ToolTip="Editar Inventario"/>
                                                    <asp:ImageButton ID="btnAddLocation" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" CausesValidation="false" CommandName="Add" ToolTip="Agregar Ubicación" CommandArgument='<%# Container.DataItemIndex %>' />	                                    
	                                                <asp:ImageButton ID="btnDelete" runat="server"  ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png" CausesValidation="false" CommandName="Delete" ToolTip="Eliminar Inventario"/>
	                                                <asp:ImageButton ID="btnNextTrack" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_start_inventory.png" CausesValidation="false" CommandName="NextTrack"/>
	                                                <asp:ImageButton ID="btnClose" runat="server"  ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_close.png" CausesValidation="false" CommandName="Close" ToolTip="Cerrar Inventario sin Aplicar"/>
                                                    <asp:ImageButton ID="btnManualInventory" runat="server"  ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process.png" CausesValidation="false" CommandName="Manual" ToolTip="Acciones inventario manual" CommandArgument='<%# Container.DataItemIndex %>' />
	                                            </center>
                                            </div>	                        
                                        </ItemTemplate>
                                    </asp:TemplateField>                                
                                        
                             </Columns>
                        </asp:GridView>
                        </div>
                        <%-- FIN Grilla Principal --%>    
                    </div>
                </div>
            </div>
              
            <%-- Pop up Editar/Nuevo Inventario --%>
            <div id="divEditNew" runat="server" visible="false">
	            <asp:Button ID="btnDummy" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	            <ajaxToolKit:ModalPopupExtender 
	                ID="modalPopUp" runat="server" TargetControlID="btnDummy" 
	                PopupControlID="pnl"  
	                BackgroundCssClass="modalBackground" 
	                PopupDragHandleControlID="Caption" Drag="true" >
	            </ajaxToolKit:ModalPopupExtender>
            	
	            <asp:Panel ID="pnl" runat="server" CssClass="modalBox">
	                <%-- Encabezado --%>			
		            <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Inventario" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Inventario" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
	                </asp:Panel>
                    <%-- Fin Encabezado --%>    
                	 <div class="modalControls">   
                	 	<asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
	                    <asp:HiddenField ID="hidTrackInventoryTypeId" runat="server" />
	                    <div class="divCtrsFloatLeft">
	                        <%-- Track --%>  
	                        <div id="divTrackInventoryType" runat="server" class="divControls">
	                            <div class="fieldRight"><asp:Label ID="lblTrackInventoryType2" runat="server" Text="Traza" /></div>
                                <div class="fieldLeft"><b><asp:Label ID="lblTrackInventoryType" runat="server" /></b></div>
                            </div>
                            
	                        <%-- Create Date --%>  
                            <div id="divCreateDate" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblCreateDate2" runat="server" Text="Creado" /></div>
                                <div class="fieldLeft"><b><asp:Label ID="lblCreateDate" runat="server" /></b></div>
                            </div>            	    
                            
                            <br />
                	    
	                        <%-- Warehouse --%>   
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist." /></div>
                                <div class="fieldLeft">
                                <asp:DropDownList ID="ddlWarehouse" runat="server" />
                                <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse" InitialValue="-1" ValidationGroup="EditNew" 
                                        Text=" * " ErrorMessage="Centro Dist. es requerido" />
                                </div>
                            </div>	    
                	        
                	        <%-- Full Whs --%>  
		                    <div id="divIsFullWhs" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblIsFullWhs" runat="server" Text="Centro Compl."/></div>
                                <div class="fieldLeft"><asp:CheckBox ID="chkIsFullWhs" runat="server" AutoPostBack="True" OnCheckedChanged="chkIsFullWhs_CheckedChanged"/></div>
                            </div>	
                	        
	                        <%-- Owner --%>   	    
                            <div id="divOwner" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblOwner" runat="server" Text="Dueño"></asp:Label></div>
                                <div class="fieldLeft"><asp:DropDownList ID="ddlOwner" runat="server" Width="120px"></asp:DropDownList>
                                <asp:requiredfieldvalidator ID="rfvOwner" runat="server"  ValidationGroup="EditNew" Text=" * " ErrorMessage="Dueño es requerido" controltovalidate="ddlOwner" display="dynamic" InitialValue="-1"/></div>
                            </div>
                                	    
	                        <%-- Start Date --%>  
                            <div id="divStartDate" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblStartDate" runat="server" Text="Inicio" /></div>
                                
                                <%-- Date --%>  
                                <div class="fieldLeft">
	                            <table cellpadding="0" cellspacing="0">  
    	                            <tr>
	                                    <td rowspan="2">              
                                            <asp:TextBox ID="txtStartDate" runat="server" Width="70px" />
                                    
                                            <ajaxToolkit:CalendarExtender ID="calStartDate" 
                                 CssClass="CalMaster"
                                    runat="server" 
                                    Enabled="true" 
                                    FirstDayOfWeek="Sunday" 
                                    TargetControlID="txtStartDate"
                                    PopupButtonID="txtStartDate" Format="dd-MM-yyyy" />
                                            <asp:requiredfieldvalidator ID="rfvStartDate" runat="server"  ValidationGroup="EditNew" Text=" * " controltovalidate="txtStartDate" display="Static" ErrorMessage="Inicio es requerido"/>&nbsp;&nbsp;
                                            <asp:RangeValidator ID="rvStartDate" runat="server" ControlToValidate="txtStartDate" ErrorMessage="Inicio debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />
                                            <%-- Hours --%>  
                                            <asp:TextBox ID="txtStartDateHours" runat="server" Width="20" MaxLength="2" Visible="false"/>
                                            
                                            <%--<ajaxToolkit:NumericUpDownExtender ID="udStartDateHours" runat="server"
                                                TargetControlID="txtStartDateHours" 
                                                Width="50"
                                                TargetButtonDownID="btnLessStartDateHours" 
                                                TargetButtonUpID="btnMoreStartDateHours"                     
                                                Minimum = "00"
                                                Maximum = "23" />	--%>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="btnMoreStartDateHours" Visible ="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" />
                                        </td>                        
                                        <td rowspan="2">
                                            <asp:RangeValidator Type="Integer" ID="rangeStartDateHours" ControlToValidate="txtStartDateHours" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Hora Inicio debe estar entre 0 y 23" MinimumValue="0" MaximumValue="23" />&nbsp;&nbsp;
                                            <%-- Minutes --%>  
                                            <asp:TextBox ID="txtStartDateMinutes" runat="server" Width="20px" MaxLength="2" Visible="false" />

                                            <%--<ajaxToolkit:NumericUpDownExtender Enabled="false" EnableViewState="false" ID="udStartDateMinutes" runat="server"
                                                TargetControlID="txtStartDateMinutes" 
                                                Width="50"
                                                TargetButtonDownID="btnLessStartDateMinutes" 
                                                TargetButtonUpID="btnMoreStartDateMinutes"                        
                                                RefValues="" 
                                                Step="10"                    
                                                Minimum = "00"
                                                Maximum = "59" />--%>	                            
                                        </td>                        
                                        <td>
                                            <asp:ImageButton ID="btnMoreStartDateMinutes" Visible = "false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" />
                                        </td>   
                                        <td rowspan="2" >
                                            <asp:RangeValidator Type="Integer" ID="rangeStartDateMinutes" ControlToValidate="txtStartDateMinutes" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Minutos de Inicio debe estar entre 0 y 59" MinimumValue="0" MaximumValue="59" />
                                        </td>                        
                                     </tr>
                                     <tr >
                                        <td >
                                            <asp:ImageButton ID="btnLessStartDateHours" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
                                        </td>                        
                                        <td >
                                            <asp:ImageButton ID="btnLessStartDateMinutes" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
                                        </td>                        
                                     </tr> 
                                  </table>
                                </div>
                            </div>
                            
	                        <%-- End Date --%>  
                            <div id="divEndDate" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblEndDate" runat="server" Text="Término"></asp:Label></div>
                                
                                <%-- Date --%> 
                                <div class="fieldLeft">
                                <table cellpadding="0" cellspacing="0">  
    	                            <tr >
	                                    <td rowspan="2">              
                                            <asp:TextBox ID="txtEndDate" runat="server" Width="70px"  />
                                    
                                            <ajaxToolkit:CalendarExtender ID="calEndDate" 
                                 CssClass="CalMaster"
                                    runat="server" 
                                    Enabled="true" 
                                    FirstDayOfWeek="Sunday" 
                                    TargetControlID="txtEndDate"
                                    PopupButtonID="txtEndDate" Format="dd-MM-yyyy" />
                                            <asp:requiredfieldvalidator ID="rfvEndDate" runat="server"  ValidationGroup="EditNew" Text=" * " controltovalidate="txtEndDate" display="Static" ErrorMessage="Término es requerido"/>&nbsp;&nbsp;
                                            <asp:RangeValidator ID="rvEndDate" runat="server" ControlToValidate="txtEndDate" ErrorMessage="Término debe estar entre 01/01/2000 y 31/12/2040" Text=" * " MaximumValue="31/12/2040" MinimumValue="01/01/2000" ValidationGroup="EditNew" Type="Date" />                                        
                                            <%-- Hours --%>  
                                            <asp:TextBox ID="txtEndDateHours" runat="server" Width="20" MaxLength="2" Visible="false"/>
                                            
                                            <%--<ajaxToolkit:NumericUpDownExtender ID="udEndDateHours" runat="server"
                                                TargetControlID="txtEndDateHours" 
                                                Width="50"
                                                TargetButtonDownID="btnLessEndDateHours" 
                                                TargetButtonUpID="btnMoreEndDateHours"                     
                                                Minimum = "00"
                                                Maximum = "23" />	--%>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="btnMoreEndDateHours" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" />
                                        </td>                        
                                        <td rowspan="2">
                                            <asp:RangeValidator Type="Integer" ID="rangeEndDateHours" ControlToValidate="txtEndDateHours" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Hora Inicio debe estar entre 0 y 23" MinimumValue="0" MaximumValue="23" />&nbsp;&nbsp;
                                            <%-- Minutes --%>  
                                            <asp:TextBox ID="txtEndDateMinutes" Visible="false" runat="server" Width="20px" MaxLength="2" />

                                            <%--<ajaxToolkit:NumericUpDownExtender ID="udEndDateMinutes" runat="server"
                                                TargetControlID="txtEndDateMinutes" 
                                                Width="50"
                                                TargetButtonDownID="btnLessEndDateMinutes" 
                                                TargetButtonUpID="btnMoreEndDateMinutes"                        
                                                RefValues="" 
                                                Step="10"                    
                                                Minimum = "00"
                                                Maximum = "59" />--%>	                            
                                        </td>                        
                                        <td>
                                            <asp:ImageButton ID="btnMoreEndDateMinutes" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" />
                                        </td>   
                                        <td rowspan="2" >
                                            <asp:RangeValidator Type="Integer" ID="rangeEndDateMinutes" ControlToValidate="txtEndDateMinutes" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Minutos de Inicio debe estar entre 0 y 59" MinimumValue="0" MaximumValue="59" />
                                        </td>                        
                                     </tr>
                                     <tr >
                                        <td >
                                            <asp:ImageButton ID="btnLessEndDateHours" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
                                        </td>                        
                                        <td >
                                            <asp:ImageButton ID="btnLessEndDateMinutes" Visible="false" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
                                        </td>                        
                                     </tr> 
                                  </table>                
                                </div>
                            </div>   
                	    
	                            
                	    
	                        <%-- User Approval --%>  
	                        <div id="divUserApproval" runat="server" class="divControls">
	                            <div class="fieldRight"><asp:Label ID="lblUserApproval" runat="server" Text="Aprobador"></asp:Label></div>
                                <div class="fieldLeft"><asp:DropDownList ID="ddlUserApproval" runat="server" TabIndex="9"></asp:DropDownList>
                                <asp:requiredfieldvalidator ID="rfvUserApproval" runat="server"  ValidationGroup="EditNew" Text=" * " ErrorMessage="Aprobador es requerido" controltovalidate="ddlUserApproval" display="dynamic" InitialValue="-1" /> </div>                                            
                           </div>	 	    
                	    
	                        <%-- Count Qty --%>  
	                        <div id="divCountQty" runat="server" class="divControls">
	                            <div class="fieldRight"><asp:Label ID="lblCountQty" runat="server" Text="Cant. Conteos" /></div>
	                            <div class="fieldLeft">
	                            <table cellpadding="0" cellspacing="0">
                                    <tr >
                                        <td rowspan="2" ><asp:TextBox ID="txtCountQty" runat="server" Width="15"/></td>
                                        <td><asp:ImageButton ID="btnMoreQty" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_more.png" CausesValidation="false" /></td>                        
                                     </tr>
                                     <tr >
                                        <td ><asp:ImageButton ID="btnLessQty" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_less.png" CausesValidation="false" />
            	                            <asp:RangeValidator Type="Integer" ID="rangeCountQty" ControlToValidate="txtCountQty" ValidationGroup="EditNew" runat="server" Text=" * " ErrorMessage="Cant. de Conteos debe estar entre 1 y 5" MinimumValue="1" MaximumValue="5" />
                                            <asp:requiredfieldvalidator ID="rfvCountQty" runat="server"  ValidationGroup="EditNew" Text=" * " controltovalidate="txtCountQty" display="dynamic" ErrorMessage="Cant. de Conteos es requerido"/>	            
                                        </td>                        
                                     </tr>                
                                </table>
                                </div>
                                
                                <ajaxToolkit:NumericUpDownExtender ID="udCountQty" runat="server"
                                    TargetControlID="txtCountQty" 
                                    Width="50"
                                    RefValues="" 
                                    ServiceDownMethod=""
                                    ServiceUpMethod=""
                                    TargetButtonDownID="btnLessQty" 
                                    TargetButtonUpID="btnMoreQty" 
                                    Minimum = "1"
                                    Maximum = "5" />	
                            </div>            

	                        <%-- Status --%>  
		                    <div id="divStatus" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblStatus" runat="server" Text="Activo"/></div>
                                <div class="fieldLeft"><asp:CheckBox ID="chkStatus" runat="server" /></div>
                            </div>

	                        <%-- Description --%>  
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblDescription" runat="server" Text="Descripción" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtDescription" runat="server" MaxLength="60"/>
                                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" /></div>
                            </div>
                         </div>	
                         			
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew"   
                                ShowMessageBox="false" CssClass="modalValidation"/>
                        </div>
                        	    
			            <%--Mensaje de advertencia--%>
                        <div id="divWarning" runat="server" Visible="false" class="modalValidation">                    
                            <div><asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Error en el ingreso de datos:" /></div>                  
                            <div><asp:Label id="lblErrorCode" runat="server" ForeColor="Red" /></div>
                        </div>                           
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true"
                                ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>
                    </div>       
	            </asp:Panel>
	        </div>
            <%-- FIN Pop up Editar/Nuevo Inventario --%>
            
            <%-- Pop up Cargar Ubicaciones --%>
            <div id="divLocations" runat="server" visible="false">
	            <asp:Button ID="btnDummy2" runat="Server" Style="display: none" /> 	<!-- Boton 'dummy' para propiedad TargetControlID -->
	            <ajaxToolKit:ModalPopupExtender 
	                ID="modalPopUp2" runat="server" TargetControlID="btnDummy2" 
	                PopupControlID="pnlLocation"  
	                BackgroundCssClass="modalBackground" 
	                PopupDragHandleControlID="pnlCaption2" Drag="true" >
	            </ajaxToolKit:ModalPopupExtender>
            	
	            <asp:Panel ID="pnlLocation" runat="server" CssClass="modalBox">
	                <%-- Encabezado --%>			
		            <asp:Panel ID="pnlCaption2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblTitleLocation" runat="server" Text="Agregar / Quitar Ubicación Inventario Nº " />
                            <asp:Label ID="lblNroInventory" runat="server" Text="" />
                            <asp:ImageButton ID="btnImgClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
	                </asp:Panel>
	                
	               <div id="divCtrs1" style="height: 520px; width: 820px; " class="modalControls" >
                          	
                       <div class="modalBoxContent">    
                        <asp:HiddenField ID="hfIndex" runat="server" Value="-1" />     
                                        
                        <div class="divItemDetails" style="width: 95%;height: 120px;">                       
                            <div id="Div1" runat="server" visible="true" style="background-color: #F4F4F7" >
                                <asp:Label ID="lblGridDetail" runat="server" Text="Filtros de busqueda para las ubicaciones" />
                            </div>
                            <br />
                            
                            <%-- Location Type --%>
                            <div id="div4" class="divCtrsFloatLeft" style="width: 110px">
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblHangar" runat="server" Text="Hangar" /><br />
                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlHangar" runat="server" Width="100px" Height="22px" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlHangar_SelectedIndexChanged"/>
                                </div>
                                <div class="mainFilterPanelTabItem" >
                                    <asp:Label ID="lblLocationType" runat="server" Text="Tipo" /> <br />
                                    <asp:DropDownList SkinID="ddlFilter" ID="ddlLocationType" runat="server" Width="100px" Height="22px"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlHangar_SelectedIndexChanged"/>
                                </div>

                            </div>
                            <div id="divLocationEqual" runat="server" class="divCtrsFloatLeft" style="width: 100px">
                                <%-- Location --%>
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblLocation" runat="server" Text="Ubicación" /><br />
                                    <asp:TextBox ID="txtLocation" runat="server" Width="70px" />
                                </div>
                            </div>
                            <div id="divLocationRange" runat="server" visible="true" class="divCtrsFloatLeft" style="width: 100px" >
                                <%-- Range Location --%>
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblLocationFrom" runat="server" Text="Ubic. Desde" /><br />
                                    <asp:TextBox ID="txtLocationFrom" runat="server" Width="70px" />
                                </div>
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblLocationTo" runat="server" Text="Ubic. Hasta" /><br />
                                    <asp:TextBox ID="txtLocationTo" runat="server" Width="70px" />
                                </div>
                            </div>
                                                     
                            <%-- Fila / Row / Hilera--%>
                            <div id="divFiltroRow" class="divCtrsFloatLeft" style="width: 100px">
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblFrom" runat="server" Text="Fila Desde"/><br />
                                    <asp:DropDownList ID="ddlRowFrom" runat="server" Width="70px" Height="22px" />
                                    <%--<asp:RequiredFieldValidator ID="rfvRowFrom" runat="server" ControlToValidate="ddlRowFrom"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fila desde es requerido" /> --%>                                       
                               </div>
                               <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblRowTo" runat="server" Text="Fila Hasta"/><br />
                                    <asp:DropDownList ID="ddlRowTo" runat="server" Width="70px" Height="22px" />
                                  <%--  <asp:RequiredFieldValidator ID="rfvRowTo" runat="server" ControlToValidate="ddlRowTo"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fila hasta es requerido" />  --%>                                      
                                </div>   
                                
                            </div>           
                                
                            <%--Column--%>
                            <div id="divFiltroColumns" class="divCtrsFloatLeft" style="width: 100px">
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblColumn" runat="server" Text="Columna Desde"/><br />                         
                                    <asp:DropDownList ID="ddlColumnFrom" runat="server" Width="70px"  Height="22px" TabIndex="1"/>
                                    <%--<asp:RequiredFieldValidator ID="rfvColumnFrom" runat="server" ControlToValidate="ddlColumnFrom"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna Desde es requerida" />  --%>                                  
                               </div>
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblColumnTo" runat="server" Text="Columna Hasta"/><br />                                   
                       
                                    <asp:DropDownList ID="ddlColumnTo" runat="server" Width="70px" Height="22px" TabIndex="1"/>
                                   <%-- <asp:RequiredFieldValidator ID="rfvColumnTo" runat="server" ControlToValidate="ddlColumnTo"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna Hasta es requerida" /> --%>                                           
                               </div>                                           
                            </div>
                            
                            <%--Nivel--%>
                            <div id="divFiltroLevel" class="divCtrsFloatLeft" style="width: 100px">
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblLevelFrom" runat="server" Text="Nivel Desde"/><br />
                                    <asp:DropDownList ID="ddlLevelFrom" runat="server" Width="70px" Height="22px"  />
                                   <%-- <asp:RequiredFieldValidator ID="rfvLevelFrom" runat="server" ControlToValidate="ddlLevelFrom"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel Desde es requerido" />    --%>                                    
                                </div>
                                <div class="mainFilterPanelTabItem">
                                    <asp:Label ID="lblLevelTo" runat="server" Text="Nivel Hasta"/><br />                           
                                    <asp:DropDownList ID="ddlLevelTo" runat="server" Width="70px" Height="22px"  />
                                   <%-- <asp:RequiredFieldValidator ID="rfvLevelTo" runat="server" ControlToValidate="ddlLevelTo"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel Hasta es requerido" />   --%>                                       
                                </div>                                 
                            </div>  

                            <%--Mensaje de advertencia--%>  
                            <div id="divErrorLocation" runat="server" class="divControls">
                                <asp:Label ID="lblErrorLocation" ForeColor="Red" runat="server"></asp:Label>   
                            </div>                                  
                        </div>      
                                             
                        <div id="div2" runat="server" class="modalActions" style="width:780px;">
                            <asp:Button ID="btnSearchLocation" runat="server" Text="Buscar" CausesValidation="true"
                                ValidationGroup="EditNew"  Width="67px" onclick="btnSearchLocation_Click" />
                                                                 
                            <asp:Button ID="btnCancelLocation" runat="server" Text="Cancelar" Width="67px"  />
                        </div>  
                           
                        <%--Contadores--%>
                        <div class="modalBoxContent" style="text-align: center;">
                            <asp:Label ID="lblLocationNoAssigned" runat="server" Text="Total Sin Asociar: " Font-Size="X-Small" />
                            <asp:Label ID="lblCountNoAssigned" runat="server" Text="0" Font-Size="X-Small" Font-Bold="true" />
                            &nbsp;&nbsp;
                            <asp:Label ID="lblLocationAssigned" runat="server" Text="Total Asociadas: " Font-Size="X-Small" />
                            <asp:Label ID="lblCountAssigned" runat="server" Text="0" Font-Size="X-Small" Font-Bold="true" />
                        </div>                      
                    </div>  
                       <div class="modalBoxContent"  style="text-align: center;">
                                <div id="divlists" style="vertical-align: top;">
                                    <table>
                                        <tr>
                                            <td  style=" text-align:center;">
                                                <div class="fieldRight" style="width: 300px">
                                                    <fieldset style="width:280px; text-align:center;">
                                                    <legend>Ubicaciones Sin Asociar</legend>
                                                        </br>
                                                        <asp:ListBox ID="lstLocNoAsociated" runat="server" Width="250" Height="220" 
                                                            SelectionMode="Multiple" Font-Bold="true" BackColor="#ffffcc" >
                                                        </asp:ListBox>
                                                    </fieldset>
                                                </div>
                                            </td>
                                           
                                            <td style=" text-align:center;">
                                                <div class="fieldRight" style="vertical-align: top; width: 50px;">
                                                    <asp:Button ID="btnAgregarTodas" runat="server" Text=">>" Width="25px" ToolTip="Agregar Todas"
                                                        OnClick="btnAgregarTodas_Click" />
                                                    <asp:Button ID="btnAgregar" runat="server" Text=">" Width="25px" ToolTip="Agregar Seleccionadas"
                                                        OnClick="btnAgregarSelec_Click" />
                                                    <asp:Button ID="btnQuitar" runat="server" Text="<" Width="25px" ToolTip="Quitar Seleccionadas"
                                                        OnClick="btnQuitarSelec_Click" />
                                                     <asp:Button ID="btnQuitarTodas" runat="server" Text="<<" Width="25px" ToolTip="Quitar Todas"
                                                        OnClick="btnQuitarTodas_Click" style=" text-align:center;" />
                                                </div>
                                            </td>
                                            <td  style=" text-align:center;">
                                                <div class="fieldRight" style="width: 200px">
                                                    <fieldset style="width:280px; text-align:center;">
                                                    <legend>Ubicaciones Asociadas</legend>
                                                        </br>
                                                        <asp:ListBox ID="lstLocAsociated" runat="server" Width="250" Height="220" 
                                                            SelectionMode="Multiple" Font-Bold="true" BackColor="#ffffcc" >
                                                        </asp:ListBox>
                                                    </fieldset>
                                                </div>
                                            </td>
                                        </tr>
                                    </table> 
                                    
                                </div>
                            </div>
                          <%--<div>

                             <%--<asp:GridView ID="grdLocations" runat="server"
                                         OnRowCreated="grdLocations_RowCreated"             
                                         AllowPaging="false" 
                                         DataKeyNames="IdLoc"
                                         onrowcommand="grdLocations_RowCommand" 
                                         AutoGenerateColumns="False" 
                                         onpageindexchanging="grdLocations_PageIndexChanging" 
                                         PageSize="9000000"
                                         PagerSettings-Mode="NextPrevious" 
                                         Visible="true" onrowdatabound="grdLocations_RowDataBound" >
                                     <Columns>
                                           <asp:BoundField DataField="IdLoc" HeaderText="ID" accessibleHeaderText="IdLoc"/>
                                        
                                            <asp:templatefield HeaderText="IdCode" AccessibleHeaderText="Code">
                                                <itemtemplate>
                                                   <asp:label ID="lblIdCode" runat="server" text='<%# Eval ( "Location.IdCode" ) %>' />
                                                </itemtemplate>
                                             </asp:templatefield>
                                             
                                            <asp:templatefield HeaderText="Pasillo" AccessibleHeaderText="Aisle">
                                                <itemtemplate>
                                                   <asp:label ID="lblAisle" runat="server" text='<%# Eval ( "Location.Aisle" ) %>' />
                                                </itemtemplate>
                                             </asp:templatefield>      
                                             
                                            <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="Column">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblColumn" runat="server" text='<%# Eval ( "Location.Column" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="Level">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblLevel" runat="server" text='<%# Eval ( "Location.Level" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>            
                                            
                                            <asp:TemplateField HeaderText="Hilera" AccessibleHeaderText="Row">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRow" runat="server" text='<%# Eval ( "Location.Row" ) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>     
                                                                             
                                            <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Acciones">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkAddLocation" runat="server"  checked='<%# Eval ( "InInventory" ) %>' />      
                                                </ItemTemplate>
                                            </asp:TemplateField>     
                                                                                                                                        
                                        </Columns>
                                </asp:GridView>
                        </div>   --%>                                 
                  </div>
             </asp:Panel>
	        </div>
	        
            <%-- FIN Pop Cargar Ubicaciones --%>            
                   
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearch" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />      
        
      </Triggers>
    </asp:UpdatePanel>  

    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
    <%-- FIN Modal Update Progress --%>  
                   
                   
    <%-- Pop up Editar/Nueva Ubicación --%>        
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <div id="divUbicationNEW" runat="server" visible="false">        
                <asp:Button ID="btnDummyNEW" runat="Server" Style="display: none" />
                <asp:Button ID="btnClosedIframe" runat="server" Style="display: none" />

                <ajaxToolkit:ModalPopupExtender ID="modalPopUpNEW" runat="server" TargetControlID="btnDummyNEW"
                    PopupControlID="pnlLocationNEW" BackgroundCssClass="modalBackground" PopupDragHandleControlID="LocationCaption"
                    Drag="true" CancelControlID="btnClosedIframe">
                </ajaxToolkit:ModalPopupExtender>    
                               
                <asp:Panel ID="pnlLocationNEW" runat="server" CssClass="modalBox">
                <%-- Encabezado --%>
                    <asp:Panel ID="LocationCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddLocation" runat="server" Text="Agregar Ubicación" />
                            <asp:ImageButton ID="ImageButton1" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>    
                    
                    <%-- Cuerpo --%>  
                    <div class="modalControls">  
                        <iframe id="iframeLocation" src="" runat="server" style=" height:500px; width:820px"></iframe>
                    </div>
                    <%-- FIn Cuerpo --%>  
                </asp:Panel>   
                   
            </div>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
        <%-- <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />--%>
      </Triggers>
    </asp:UpdatePanel>  

    <asp:UpdateProgress ID="uprEditNew" runat="server" AssociatedUpdatePanelID="upEditNew" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>               
    <%--> FIN <-- Pop up Editar/Nueva Ubicación --> FIN <--%>       
    
    <!-- begin manual inventory options -->
    <asp:UpdatePanel ID="upManualInventory" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <div id="divManualInventory" runat="server" visible="false">  

                <asp:Button ID="btnDummyManualInventory" runat="Server" Style="display: none" />

                <ajaxToolkit:ModalPopupExtender ID="modalPopUpManualInventory" runat="server" TargetControlID="btnDummyManualInventory"
                    PopupControlID="pnlManualInventory" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlManualInventoryCaption"
                    Drag="true" CancelControlID="btnCloseManualInventory">
                </ajaxToolkit:ModalPopupExtender>    

                <asp:Panel ID="pnlManualInventory" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>			
		            <asp:Panel ID="pnlManualInventoryCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label1" runat="server" Text="Inventario Manual" />
                            <asp:ImageButton ID="btnCloseManualInventory" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
	                </asp:Panel>
                    <%-- Fin Encabezado --%>  

                    <%-- Cuerpo --%>  
                    <div class="modalControls"> 
                        <div class="divCtrsFloatLeft">

                            <asp:HiddenField ID="hidInventoryId" runat="server" />

                            <%-- Download --%>  
	                        <div id="divDownloadFile" runat="server" class="divControls">
	                            <div class="fieldRight">
                                    <asp:Label ID="lblDownloadFile" runat="server" Text="Descargar Template" />
	                            </div>
                                <div class="fieldLeft">
                                    <asp:HyperLink ID="btnDownloadFile" runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Inventario.xlsx" ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" ToolTip="Descargar template" />
                                </div>
                            </div>
                            <%-- End Download --%>  

                            <%-- Upload --%>  
	                        <div id="divUploadFile" runat="server" class="divControls">
	                            <div class="fieldRight">
                                    <asp:Label ID="lblUploadFile" runat="server" Text="Seleccione Archivo" />
	                            </div>
                                <div class="fieldLeft">
                                      <asp:FileUpload ID="uploadFile" runat="server" Width="400px" ValidationGroup="Load"/>  
                                    
                                      <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo" ValidationGroup="Load" ControlToValidate="uploadFile"></asp:RequiredFieldValidator>
                                      <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load" ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*" ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" ControlToValidate="uploadFile"></asp:RegularExpressionValidator>                        
                                </div>
                            </div>
                            <%-- End Upload --%>  

                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir" runat="server" Text="Cargar Archivo" ValidationGroup="Load" OnClientClick="return confirmUploadFile();" onclick="btnSubir_Click" />
                                </div>
                            </div>

                        </div>

                        <div style="clear: both"></div>

                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load" ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div3" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>

                    </div>
                    <%-- Fin Cuerpo --%>  

                </asp:Panel>

            </div>
        </ContentTemplate>  
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir" />
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="uprManualInventory" runat="server" AssociatedUpdatePanelID="upManualInventory" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgressManualInventory" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>         
    
    <webUc:UpdateProgressOverlayExtender ID="upoeManualInventory" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprManualInventory" />    

    <div id="divFondoPopupProgress" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;" runat="server">
        <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoadNew" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
    </div>

    <div id="divFondoPopup" style="display: none; position: fixed; top: 0; left: 0;
        z-index: 400000; width: 100%; height: 100%; background-color: Gray; Filter: Alpha(Opacity=40);
        opacity: 0.5; text-align: center; vertical-align: middle;">
    </div>
    <div id="divMensaje" class="modalBox" style="z-index: 400001; display: none; position: absolute; 
        width: 400px;  top: 200px; margin-top: 0;"  runat="server">
        
        <div id="divDialogTitleMessage" runat="server" class="modalHeader">
			<div class="divCaption">
			    <asp:Label ID="lblDialogTitle" runat="server" />
            </div>
	    </div>
	    <div id="divPanelMessage" class="divDialogPanel" runat="server">
        
            <div class="divDialogMessage">
                <asp:Image id="Image1" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_info_small.png" />
            </div>
            <div id="divDialogMessage" runat="server" class="divDialogMessage" />        
            <div id="divAlert" runat="server" visible="true" class="divDialogButtons">
                <asp:Button ID="btnMessageInfo" runat="server" Text="Aceptar"  OnClientClick="return HideMessage();" />
            </div>    
        </div>
               
    </div>  
    <!-- end manual inventory options -->
                   
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Inventario?" Visible="false" />
    <asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />  
    <asp:Label id="lblStartDateError" runat="server" Text=" - Fecha de Inicio debe ser mayor a Fecha Actual." Visible="false" />  
    <asp:Label id="lblEndDateError" runat="server" Text=" - Fecha de Término debe ser mayor a Fecha de Inicio." Visible="false" />  
    <asp:Label id="lblApplyInventoryWarning" Visible="false" runat="server" 
        Text="¿Desea aplicar el Inventario? \n\n Esta acción modificará el Stock.\n Se recomienda realizar previamente copia de seguridad de los datos del Centro de Distribución. \n Comuníquese con su soporte TI, para esta operación"  />  
    <asp:Label id="lblCloseInventoryWarning" Visible="false" runat="server" Text="¿Desea cerrar este Inventario?" />  
    <asp:Label id="lblInventoryOk" Visible="false" runat="server" Text="El Inventario ha sido aplicado con éxito." />  
    <asp:Label ID="lblUserApplyInventoryAlert" Visible="false" runat="server" Text="Usuario no tiene permiso para aplicar inventario" />
    <asp:Label ID="lblCountedLocationAlert" Visible="false" runat="server" Text="Existen ubicaciones pendientes por contar" />
    <asp:Label ID="lblNroInventoryFilter" runat="server" Text="Nº Inventario" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Adm. Inventario" Visible="false" />
    <asp:Label id="lblErrorRow" runat="server" Text="- La fila hasta no puede ser menor que la fila desde" Visible="false" />  
    <asp:Label id="lblErrorColunm" runat="server" Text="- La columna Hasta no puede ser menor que la columna desde" Visible="false" />  
    <asp:Label id="lblErrorLevel" runat="server" Text="- El nivel Hasta no puede ser menor que el nivel desde" Visible="false" /> 
    <asp:Label id="lblRangoLocation" runat="server" Text="- Se debe ingresar Ubicación Desde y Hasta" Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblValidateRepeatedLpns" runat="server" Text="LPN [IDCODE] esta repetido en distintas ubicaciones" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     

</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>

