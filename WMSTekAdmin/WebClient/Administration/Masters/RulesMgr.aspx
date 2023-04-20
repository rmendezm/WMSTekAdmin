<%@ Page Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="RulesMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.RulesMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Interface" Assembly="obout_Interface" %>
<%@ Register TagPrefix="obout" Namespace="Obout.Grid" Assembly="obout_Grid_NET" %>
<%@ Register TagPrefix="webUc" TagName="ucLookUpFilter" Src="~/Shared/LookUpFilterContent.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .divItemDetails{
            width: 90% !important;
            max-height: 230px;
            overflow-y: auto;
        }
    </style>
    
<script type="text/javascript">
    function ReOrderGridView() {
        $("[id*=grdDetail]").sortable({
            items: 'tr:not(tr:first-child)',
            cursor: 'pointer',
            axis: 'y',
            dropOnEmpty: false,
            start: function(e, ui) {
                ui.item.addClass("selected");
            },
            stop: function(e, ui) {
                ui.item.removeClass("selected");

                //Recorre la grilla y ordena la secuencia
                var cont = 1;
                var lstReglas = "";
                $("#ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail tr").not(':first').each(function() {
                    $(this).find("td").eq(5).text(cont);
                    lstReglas = lstReglas + $(this).find("td").eq(0).text() + "-" + cont + ";";
                    cont++;
                });
        
                $('#ctl00_MainContent_hidListaGrupos').val('');
                $('#ctl00_MainContent_hidListaGrupos').val(lstReglas);
                //alert($('#ctl00_MainContent_hidListaGrupos').val());
                
            },
            receive: function(e, ui) {

                $(this).find("tbody").append(ui.item);
            }
        });
    }


    window.onresize = resizeDivPrincipal;
    function resizeDivPrincipal() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divCustomRule").style.height = h;
        document.getElementById("ctl00_MainContent_divCustomRule").style.width = w;
    }

    $(function () {
        var gridHeader = 'ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdMgr';
        if ($("#" + gridHeader).length > 0) {
            initializeGridDragAndDrop('CustomRule_FindAll', gridHeader);
        }

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
        initializeGridWithNoDragAndDrop(true);
    }

    function clearFilterDetail(gridDetail) {
        if ($("#" + gridDetail).length == 0) {
            if ($("div.container").length == 2) {
                $("div.container:last div.row-height-filter").remove();
            }
        }
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_ctl00_ctl01_grdDetail';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('GroupRuleByIdCustomRule', gridDetail);
    }
</script>
    
    <style>
        .divLookupGrid {
            overflow: hidden !important;
            width: 100% !important;
        }
	
        .froze-header-grid {
            max-height: 280px !important;
        }

        #ctl00_MainContent_pnlLookupRule {
            width: 80% !important;
            left: 10% !important;
        }
	
    </style>
    

<div runat="server" id="divCustomRule" style=" width:100%; height:100%; margin: 0px; margin-bottom: 80px">

    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server"
        StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        
        <TopPanel ID="topPanel"  HeightMin="50" HeightDefault="500" >
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <%-- Grilla Principal --%>
                                    <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                                        <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" 
                                            OnRowCreated="grdMgr_RowCreated"
                                            OnRowDeleting="grdMgr_RowDeleting" 
                                            OnRowEditing="grdMgr_RowEditing" 
                                            OnRowDataBound="grdMgr_RowDataBound"
                                            OnSelectedIndexChanged="grdMgr_SelectedIndexChanged" 
                                            AllowPaging="True" EnableViewState="false"
                                            AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                            EnableTheming="false">                    
                                            <Columns>
                                
                                                <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" Visible="false" />
                                                <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                                <asp:TemplateField HeaderText="Proceso" AccessibleHeaderText="Process">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWmsProcessName" runat="server" Text='<%# Eval ( "WmsProcess.Name" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status" SortExpression="Status">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# ((int) Eval("Status") == 1)? true : false %>'
                                                                Enabled="false" />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                    
                                                <asp:TemplateField HeaderText="Por Defecto" AccessibleHeaderText="DefaultRule" SortExpression="DefaultRule">
                                                    <ItemStyle Wrap="false" />
                                                    <ItemTemplate>
                                                        <center>
                                                            <asp:CheckBox ID="chkCodDefaultRule" runat="server" Checked='<%# ((int) Eval("DefaultRule") == 1)? true : false %>'
                                                                Enabled="false" />
                                                        </center>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Centro Distr." AccessibleHeaderText="Warehouse">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IdWhs" AccessibleHeaderText="IdWhs" Visible="false">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word;">
                                                            <asp:Label ID="lblIdWarehouse" runat="server" Text='<%# Eval ( "Warehouse.Id" ) %>' />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="UserCreated" HeaderText="Creado Por" AccessibleHeaderText="UserCreated" />
                                                <asp:BoundField DataField="DateCreated" HeaderText="Fecha Creación" AccessibleHeaderText="DateCreated" />
                                                <asp:BoundField DataField="UserModified" HeaderText="Modificado Por" AccessibleHeaderText="UserModified" />
                                                <asp:BoundField DataField="DateModified" HeaderText="Fecha Modificación" AccessibleHeaderText="DateModified" />
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
                                            <EmptyDataTemplate>
                                                <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                            </EmptyDataTemplate>
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
                <%-- FIN Modal Update Progress --%>
            </Content>
        </TopPanel>
        
        <BottomPanel HeightMin="50">
            <Content>
                 <asp:UpdatePanel ID="upDetail" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>  
                    
                        <div id="divGroupRuleDetail" visible="false" runat="server" class="divItemDetails">    
                        
                            <div id="divDescripRule" runat="server" visible="true" class="divGridTitle">
                                <asp:Label ID="lblGridDetail" runat="server" Text="" />
                            </div>
                            
                            <%--Codigo--%>
                            <div class="mainFilterPanelItem">
                                <asp:Label ID="lblCode" runat="server" Text="Código" /><br />
                                <asp:TextBox ID="txtCode" runat="server" Width="180px" />
                            <%--</div>--%>
                            <%--Boton Buscar--%>
                            <%--<div class="mainFilterPanelItem">--%>
                                <asp:ImageButton ID="imgbtnSearchRule" runat="server" Height="18px" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png"
                                    OnClick="imgBtnSearchRule_Click" Width="18px" ValidationGroup="searchRule" ToolTip="Buscar Regla" />
                            </div>
                            <%--Descripcion--%>
                            <div class="mainFilterPanelItem">
                                <asp:Label ID="lblDescription" runat="server" Text="Nombre" /><br />
                                <asp:TextBox ID="txtDescription" runat="server" Width="200px" MaxLength="30" Enabled="False" ReadOnly="True" />
                                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                    ValidationGroup="AddRule" Text=" * " ErrorMessage="Nombre es requerido." />
                            </div>
                            
                            <%--Boton Agregar Item--%>
                            <div class="mainFilterPanelItem">
                                <br />
                                <asp:ImageButton ID="imgBtnAddRule" runat="server" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_add_item.png"
                                    OnClick="imgBtnAddRule_Click" ValidationGroup="AddRule" ToolTip="Agregar Regla" />
                                &nbsp;&nbsp;&nbsp;   
                                 
                                <asp:ImageButton runat="server" ID="btnUpdateSequence"  ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_tick_inventory.png"
                                ToolTip="Guardar Secuencia" OnClick="btnUpdateSequence_Click" />
                                <br />
             
                            </div>                                                        
                            
                            <%--Panel de error--%>
                            <asp:Panel ID="pnlError" runat="server" Visible="false">
                                <asp:Label ID="lblMesage" runat="server" ForeColor="Red" Text="" />
                            </asp:Panel>
                            
                            <%--Panel de resumen de validacion--%>
                            <div class="mainFilterPanelItem">
                                <asp:ValidationSummary ID="valAddRule" runat="server" ValidationGroup="AddRule" />
                                <asp:ValidationSummary ID="valSearchRule" runat="server" ValidationGroup="searchRule" />
                                <asp:Label ID="Label2" Visible="false" runat="server" Text="Debe Seleccionar" ForeColor="Red" />
                            </div>
                        
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-12">                                 
                                        <%-- Grilla Detalle --%>
                                        <div id="divDetail" runat="server" visible="true" class="divGrid" onresize="SetDivs();"  >
                                            <asp:GridView ID="grdDetail"  runat="server" AllowPaging="True" EnableViewState="False" 
                                                DataKeyNames="IdCustomRule"
                                                OnRowDeleting="grdDetail_RowDeleting" 
                                                OnRowCreated="grdDetail_RowCreated"
                                                OnRowDataBound="grdDetail_RowDataBound"
                                                OnRowCommand="grdDetail_RowCommand"
                                                AutoGenerateColumns="False"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">                    
                                                <Columns>                                
                                                    <%--<asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" Visible="false" />--%>
                                                    <asp:TemplateField  HeaderText="ID" InsertVisible="True"  AccessibleHeaderText="IdRule" Visible="true" >
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblIdRule" runat="server" Text='<%# Eval ( "Rule.Id" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>  
                                                    <asp:TemplateField  HeaderText="Código" InsertVisible="True"  AccessibleHeaderText="Id" Visible="true" >
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblRuleCode" runat="server" Text='<%# Eval ( "Rule.Code" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>  
                                                    <asp:TemplateField  HeaderText="Nombre" InsertVisible="True"  AccessibleHeaderText="Id" Visible="true" >
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblRuleName" runat="server" Text='<%# Eval ( "Rule.Name" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Proceso" AccessibleHeaderText="Process">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word;">
                                                                <asp:Label ID="lblWmsProcessName" runat="server" Text='<%# Eval ( "WmsProcess.Name" ) %>' />
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Priorizar" AccessibleHeaderText="Prioritize">
                                                        <ItemTemplate> 
                                                            <center>
                                                            <div style="width:60px;margin:0px; padding: 0px;">
	                                                            <asp:ImageButton ID="btnUp" runat="server" 
	                                                                ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_up.png" 
	                                                                onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_up_on.png';"
                                                                    onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_up.png';"
	                                                                CausesValidation="false" CommandName="Up" ToolTip="Mover Arriba"/>
	                                                            <asp:ImageButton ID="btnDown" runat="server" 
	                                                                ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_down.png" 
	                                                                onmouseover="this.src='../../WebResources/Images/Buttons/GridActions/icon_down_on.png';"
                                                                    onmouseout="this.src='../../WebResources/Images/Buttons/GridActions/icon_down.png';"
	                                                                CausesValidation="false" CommandName="Down" ToolTip="Mover Abajo"/>
                                                            </div>	                        
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>  
                                                    <asp:TemplateField HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                    <ItemTemplate>
                                                        <center>
                                                            <div style="width: 60px">                                                    
                                                                <asp:ImageButton ID="btnDeleteRuleGroup" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                                    CausesValidation="false" CommandName="Delete" />
                                                            </div>
                                                        </center>
                                                    </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:BoundField DataField="SequenceExecution" HeaderText="Secuencia"  ItemStyle-HorizontalAlign="Center"
                                                     AccessibleHeaderText="SequenceExecution" />
                                        
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <asp:Label ID="lblEmptyGridDetail" runat="server" Text="No se han encontrado registros." />
                                                </EmptyDataTemplate>
                                            </asp:GridView>    
                                         </div>
                                    </div>
                                </div>  
                            </div>
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
                         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />
                         <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$ctl00$ctl01$imgbtnSearchRule" EventName="Click" />--%>
                         <asp:AsyncPostBackTrigger ControlID="imgBtnAddRule" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$grdSearchRules" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="btnUpdateSequence" EventName="Click" />
                        
                    </Triggers>
                    
                </asp:UpdatePanel>
                
                <%-- Modal Update Progress --%>
                <asp:UpdateProgress ID="udpDetail" runat="server" AssociatedUpdatePanelID="upDetail"
                    DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <webUc:UpdateProgressOverlayExtender ID="muprGridDetail" runat="server" ControlToOverlayID="divTop"
                    CssClass="updateProgress" TargetControlID="udpDetail" />
                <%-- FIN Modal Update Progress --%>
            </Content>
            
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
             
        </BottomPanel>
    </spl:HorizontalSplitter>
    
</div>    
    
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>             
            <%-- Pop up Editar/Nuevo Rol --%>
            <div id="divEditNew" runat="server" visible="false">            
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnl" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnl" runat="server" CssClass="modalBox">
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nuevo Grupo de Reglas" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Grupo de Reglas" />
                            <asp:Label ID="lblView" runat="server" Text="Detalles del Grupo"/>
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>                    
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                                                
                        <div class="divCtrsFloatLeft">
                            <div id="divCodStatus" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblCodStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft"><asp:CheckBox ID="chkCodStatus" runat="server" /></div>
                            </div>
                            <div id="divDefaultRule" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblDefaultRule" runat="server" Text="Por Defecto" /></div>
                                <div class="fieldLeft"><asp:CheckBox ID="chkDefaultRule" runat="server" /></div>
                            </div>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="150" />
                                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" /></div>
                            </div>
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblProcess" runat="server" Text="Proceso" /></div>
                                <div class="fieldLeft"><asp:DropDownList ID="ddlProcess" runat="server"  Width="150" />
                                <asp:RequiredFieldValidator ID="rfvProcess" runat="server" ControlToValidate="ddlProcess"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Proceso es requerido" />
                                </div>
                            </div>
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblWarehouse" runat="server" Text="Centro Distr." /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWarehouse" runat="server"  Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro Distribución es requerido" />
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
            <%-- FIN Pop up Editar/Nuevo Rol --%>
       </ContentTemplate>
       <Triggers>
         <%--<asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />--%>
         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdMgr" EventName="RowCommand" />
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
    
    
    <%-- Add Rule--%>
    <asp:UpdatePanel ID="udpAddRule" runat="server" UpdateMode="Always" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div id="divLookupRules" runat="server" visible="true" >
                <asp:Button ID="btnDummy1" runat="Server" Style="display: none" />
                
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpLookupRule" runat="server" TargetControlID="btnDummy1"
                    PopupControlID="pnlLookupRule" BackgroundCssClass="modalBackground" PopupDragHandleControlID="pnlLookupRule"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                
                <asp:Panel ID="pnlLookupRule" runat="server" CssClass="modalBox" style=" width:910px">
                    <asp:Panel ID="pnlHeadBar2" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblAddRule" runat="server" Text="Nuevo" />
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageAlign="Top" CssClass="closeButton" 
                            ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidItemId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidListaGrupos" runat="server" Value="" />
                        <webUc:ucLookUpFilter ID="ucFilterItem" runat="server" />
                        
                        <div class="divCtrsFloatLeft">
                            <div class="divLookupGrid" style=" width:880px">
                                <asp:GridView ID="grdSearchRules" runat="server" DataKeyNames="Id" 
                                    OnRowCreated="grdSearchRules_RowCreated"
                                    OnRowCommand="grdSearchRules_RowCommand"                                     
                                    AutoGenerateColumns="False"
                                    EnableTheming="False"
                                    OnRowDataBound="grdSearchRules_RowDataBound"
                                    CssClass="table table-bordered table-hover table-curved table-NoDragAndDrop">
                                    <Columns>
                                        <asp:BoundField AccessibleHeaderText="Id" DataField="Id" HeaderText="ID" InsertVisible="True"
                                            SortExpression="Id" />
                                        <asp:TemplateField AccessibleHeaderText="CodeRule" HeaderText="C&oacute;d.">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblCodeRegla" runat="server" Text='<%# Eval ("Code") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="Description" HeaderText="Nombre">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblNameRegla" runat="server" Text='<%# Eval ("Name") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="NameProcess"  ControlStyle-Width="50px" HeaderText="Proceso">
                                            <ItemTemplate>
                                                <div style="word-wrap: break-word;">
                                                    <asp:Label ID="lblNameProcess" runat="server" Text='<%# Eval ("NameProcess") %>' />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="imgBtnAsigRule" runat="server" Height="20px" 
                                                    ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_add.png" Width="20px" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="lblEmptyGridDetail01" runat="server" Text="No se han encontrado registros." />
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                        <div style="clear: both" />
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
<%--            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$bottomPanel$divItemDetail$ctl01$grdDetail"
                EventName="RowCommand" />--%>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- FIN Lookup Items --%>
    
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Grupo de Reglas?" Visible="false" />
    <asp:Label ID="lblConfirmDeleteGroupRule" runat="server" Text="¿Desea eliminar esta Regla?" Visible="false" />
    <asp:Label ID="lblDetailsHead" runat="server" Text="Detalle del Grupo: " Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblFilterCode" runat="server" Text="C&oacute;digo" Visible="false" />
    <asp:Label ID="lblNewDetail" runat="server" Text="Nuevo Detalle - Seleccione Regla" Visible="false" />      
    <asp:Label ID="lblErrUpdateSequence" runat="server" Text="* No se han realizado cambios en la secuencia." Visible="false" />   
    <asp:Label ID="lblErrRuleAsig" runat="server" ForeColor="Red" Text="* La regla ya se encuentra ingresada." />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>

    
    
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>