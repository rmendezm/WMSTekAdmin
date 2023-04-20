
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="InventoryLocation.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Inventory.InventoryLocation" %>
<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script language="javascript" type="text/javascript">

    //Cierra la ventana de las ubicaciones del inventario
    function HideScreemLocation() {
        var a = parent.document.getElementById('ctl00$MainContent$btnClosedIframe');
        a.click();
    }

    $(document).ready(function () {
        initializeGridDragAndDrop("GetEmptyLocations", "ctl00_MainContent_grdMgr");

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
        initializeGridDragAndDrop("GetEmptyLocations", "ctl00_MainContent_grdMgr");
    }
    
</script>

     <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <%-- Panel Agregar Ubicación--%>			
            <div id="divModal" runat="server" visible="true">
                    <%-- Encabezado --%>			
			        <asp:Panel ID="OutboundCaption" runat="server" CssClass="modalHeader">
			          <div class="divCaption">
				        <asp:Label ID="lblEdit" runat="server" Text="Agregar / Quitar Ubicación Inventario Nº " Width="770px"/>
                      </div>	        
		            </asp:Panel>
                    <%-- Fin Encabezado --%>    
                    
                    <div class="modalBoxContent">                      
                        <div class="divItemDetails">                       
                            <div id="Div1" runat="server" visible="true" style="background-color: #F4F4F7" >
                                <asp:Label ID="lblGridDetail" runat="server" Text="Filtros de busqueda para las ubicaciones" />
                            </div>
                            <br />
                                                     
                            <%-- Fila / Row / Hilera--%>
                            <div id="divFiltroRow" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblFrom" runat="server" Text="Fila Desde"/>
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlRowFrom" runat="server" Width="70px" />
                                    <asp:RequiredFieldValidator ID="rfvRowFrom" runat="server" ControlToValidate="ddlRowFrom"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fila desde es requerido" />                                        
                               </div>
                               <div class="fieldRight">
                                    <asp:Label ID="lblRowTo" runat="server" Text="Fila Hasta"/>
                                </div>                                  
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlRowTo" runat="server" Width="70px" />
                                    <asp:RequiredFieldValidator ID="rfvRowTo" runat="server" ControlToValidate="ddlRowTo"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fila hasta es requerido" />                                        
                                </div>   
                                
                            </div>           
                                
                            <%--Column--%>
                            <div id="divFiltroColumns" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblColumn" runat="server" Text="Columna Desde"/>
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlColumnFrom" runat="server" Width="70px" TabIndex="1"/>
                                    <asp:RequiredFieldValidator ID="rfvColumnFrom" runat="server" ControlToValidate="ddlColumnFrom"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna Desde es requerida" />                                    
                               </div>
                                <div class="fieldRight">
                                    <asp:Label ID="lblColumnTo" runat="server" Text="Columna Hasta"/>                                    
                                </div>                               
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlColumnTo" runat="server" Width="70px" TabIndex="1"/>
                                    <asp:RequiredFieldValidator ID="rfvColumnTo" runat="server" ControlToValidate="ddlColumnTo"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna Hasta es requerida" />                                            
                               </div>                                           
                            </div>
                            
                            <%--Nivel--%>
                            <div id="divFiltroLevel" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblLevelFrom" runat="server" Text="Nivel Desde"/></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlLevelFrom" runat="server" Width="70px"  />
                                    <asp:RequiredFieldValidator ID="rfvLevelFrom" runat="server" ControlToValidate="ddlLevelFrom"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel Desde es requerido" />                                        
                                </div>
                                <div class="fieldRight">
                                    <asp:Label ID="lblLevelTo" runat="server" Text="Nivel Hasta"/>
                                </div>                                   
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlLevelTo" runat="server" Width="70px"  />
                                    <asp:RequiredFieldValidator ID="rfvLevelTo" runat="server" ControlToValidate="ddlLevelTo"
                                      InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel Hasta es requerido" />                                          
                                </div>                                 
                            </div>  
                            
                            <asp:Label ID="lblError" ForeColor="Red" runat="server"></asp:Label>                                            
                        </div>      
                        
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" Text="Guardar" CausesValidation="true"
                                ValidationGroup="EditNew" onclick="btnSave_Click" Width="67px" />
                                
                            <asp:Button ID="btnDelete" runat="server" Text="Eliminar" CausesValidation="true"
                                ValidationGroup="EditNew" Width="67px" onclick="btnDelete_Click" />
                                                                
                            <asp:Button ID="btnCancel" runat="server" Text="Volver" Width="67px" 
                                onclick="btnCancel_Click" />
                        </div>                        
                    </div>       
           </div>
           <%-- FIN Panel Nuevo/Editar Documento --%>			
            
            
            <%-- Grilla Principal --%>

             <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div id="divGrid" runat="server" class="divGrid" onresize="SetDivsLocation();" style="overflow: auto">
                               <asp:GridView ID="grdMgr" 
                                         runat="server"
                                         DataKeyNames="IdLoc"
                                         OnRowCreated="grdMgr_RowCreated"        
                                         AllowPaging="True"               
                                         EnableViewState="False" 
                                         AutoGenerateColumns="False"
                                         Visible ="true" PageSize="20" 
                                         OnRowDataBound="grdMgr_RowDataBound"
                                         CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                         EnableTheming="false">
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
                                                                 
                                            <asp:TemplateField HeaderText="Estado" AccessibleHeaderText="Estado" Visible="false">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkAddLocation" runat="server"  checked='<%# Eval ( "InInventory" ) %>' AutoPostBack="True" />      
                                                </ItemTemplate>
                                            </asp:TemplateField>     
                             
                                        </Columns>
                                </asp:GridView>          
                        </div>
                    </div>
                </div>
            </div>
            <%-- FIN Grilla Principal --%>                
            
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnRefresh" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$btnSearchAux" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnFirst" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnPrevious" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnNext" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$btnLast" EventName="Click" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$StatusContent$ucStatus$ddlPages" EventName="SelectedIndexChanged" />
         <asp:AsyncPostBackTrigger ControlID="ctl00$ucMainFilterContent$ddlFilterHangar" EventName="SelectedIndexChanged" />
               
      </Triggers>
    </asp:UpdatePanel>  

    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>                        
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
                
    <webUc:UpdateProgressOverlayExtender ID="modalUpdateProgress" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />    
     <%-- Barra de Estado --%>  
                   
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label id="lblFilterDate" runat="server" Text="Creación" Visible="false" />  
    
    <asp:Label id="lblErrorRow" runat="server" Text="La fila hasta no puede ser menor que la fila desde" Visible="false" />  
    <asp:Label id="lblErrorColunm" runat="server" Text="La columna Hasta no puede ser menor que la columna desde" Visible="false" />  
    <asp:Label id="lblErrorLevel" runat="server" Text="El nivel Hasta no puede ser menor que el nivel desde" Visible="false" />  
    <asp:Label id="lblErrorNoRowsAdd" runat="server" Text="No se han encontrado ubicaciones, asegurese que no esten agregadas en algun inventario" Visible="false" />  
    <asp:Label id="lblErrorNoRowsDelete" runat="server" Text="No se han encontrado ubicaciones, , asegurese que no esten eliminadas de  este inventario" Visible="false" />  

    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>     
</asp:Content>

<asp:Content ID="StatusContent"  ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>        
    <webUc:ucStatus id="ucStatus" runat="server"/>      
</asp:Content>