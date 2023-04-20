<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="WorkZoneMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.LogisticsResources.WorkZoneMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="MainContent">
<script language="javascript" type="text/javascript">
    function resizeDivPrincipal() {
        //debugger;
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMain").style.height = h;
        document.getElementById("ctl00_MainContent_divMain").style.width = w;
    }

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(resizeDivPrincipal);

    $(document).ready(function () {
        initializeGridDragAndDrop("hangar_FindAll", "ctl00_MainContent_hsVertical_leftPanel_ctl01_grdMgr");

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
        initializeGridDragAndDrop("hangar_FindAll", "ctl00_MainContent_hsVertical_leftPanel_ctl01_grdMgr");
    }
    
</script>

    <style>
        #hsVertical_LeftP_Content {
            max-height: 500px !important;
        }

        #__hsVerticalRD {
            max-height: 550px !important;
            overflow: auto !important;
        }
    </style>
    
    <div id="divMain" runat="server" style="width: 100%; height: 100%; margin: 0px; margin-bottom: 80px">
        <spl:Splitter LiveResize="false" CookieDays="0" ID="hsVertical" runat="server" StyleFolder="~/WebResources/Styles/Obout/default">
            <LeftPanel ID="leftPanel" WidthDefault="500" WidthMin="100">
                <Content>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12">
                                <%-- Grilla Principal --%>
                                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divGrid" runat="server" visible="true" class="divGrid" style=" height:100%" onresize="SetDivs();">
                                            <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id"                                     
                                                OnRowDataBound="grdMgr_RowDataBound"
                                                OnRowCreated="grdMgr_RowCreated" 
                                                OnRowDeleting="grdMgr_RowDeleting" 
                                                OnRowEditing="grdMgr_RowEditing"
                                                OnPageIndexChanging="grdMgr_PageIndexChanging" 
                                                OnSelectedIndexChanged="grdMgr_SelectedIndexChanged"
                                                AllowPaging="True" EnableViewState="false"
                                                AutoGenerateColumns="False"
                                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                                EnableTheming="false">
                                                <Columns>
                                                    <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True"
                                                        AccessibleHeaderText="Id" />
                                                    <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                                    <asp:TemplateField HeaderText="Tipo" AccessibleHeaderText="TypeZone">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTypeZone" runat="server" Text='<%# Enum.GetName(typeof(Binaria.WMSTek.Framework.Utils.Enums.TypeWorkZone),Convert.ToInt32(Eval("TypeZone"))) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                                    <asp:TemplateField HeaderText="Cód. CD." AccessibleHeaderText="WarehouseCode">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblWarehouseCode" runat="server" Text='<%# Eval ( "Warehouse.Code" ) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Centro Dist." AccessibleHeaderText="Warehouse">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblWarehouse" runat="server" Text='<%# Eval ( "Warehouse.ShortName" ) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="CodStatus">
                                                        <ItemTemplate>
                                                            <center>
                                                                <asp:CheckBox ID="chkCodStatus" runat="server" Checked='<%# Eval ( "CodStatus" ) %>'
                                                                    Enabled="false" />
                                                            </center>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ShowHeader="False" HeaderText="Acciones" AccessibleHeaderText="Actions">
                                                        <ItemTemplate>
                                                            <center>
                                                                <div style="width: 60px">
                                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                                        CausesValidation="false"   CommandName="Edit" />
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
                                        <%--<asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />--%>
                                    </Triggers>
                                </asp:UpdatePanel>
                                <%-- FIN Grilla Principal --%>
                            </div>
                        </div>
                    </div>     
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </LeftPanel>
            <RightPanel ID="rightPanel" WidthMin="50">
                <Content>
                    <%-- Panel de Ubicaciones asociadas --%>
                    <asp:UpdatePanel ID="upGridLocation1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <%-- Filtro de Ubicaciones --%>
                            <div class="modalBoxContent" style="width: auto">
                                <div class="divItemDetails" style="width: auto">
                                    <div id="Div12" runat="server" visible="true" style="background-color: #F4F4F7">
                                        <asp:Label ID="lblGridDetail" runat="server" Text="Filtros de busqueda para las ubicaciones" />
                                    </div>
                                    <div class="divLeftFilterLoc" style="width:650px; height:140px;">
                                        <%-- Hangar --%>
                                        <div id="div3" class="divCtrsFloatLeft">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblHangar1" runat="server" Text="Bodega" /><br />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlHangar1" SkinID="ddlFilter" AutoPostBack="true" runat="server"
                                                    Width="110px" OnSelectedIndexChanged="ddlHangar1_SelectedIndexChanged" />
                                                <asp:RequiredFieldValidator ID="rfvHangar" runat="server" ControlToValidate="ddlHangar1"
                                                    InitialValue="-1" ValidationGroup="SearchLoc" Text=" * " ErrorMessage="Hangar es Requerido" />
                                            </div>
                                        </div>
                                        <%-- Location Type --%>
                                        <div id="div4" class="divCtrsFloatLeft">
                                            <div class="fieldRight" style="width:60px;">
                                                <asp:Label ID="lblLocationType1" runat="server" Text="Tipo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList SkinID="ddlFilter" ID="ddlLocationType1" runat="server" Width="110px" 
                                                 AutoPostBack="true" OnSelectedIndexChanged="ddlHangar1_SelectedIndexChanged"/>
                                            </div>
                                        </div>
                                        <%-- IdLocCode --%>
                                        <div id="div5" class="divCtrsFloatLeft">
                                            <div class="fieldRight" style="width:70px;">
                                                <asp:Label ID="lblIdLocCode1" runat="server" Text="Ubicación" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox SkinID="txtFilter" ID="txtIdLocCode1" runat="server" Width="90px" />
                                                <br />
                                            </div>
                                        </div>
                                        
                                        <%--Column--%>
                                        <div id="divFiltroColumns" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblColumn" runat="server" Text="Columna Desde" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlColumnFrom" runat="server" Width="70px" TabIndex="1" />
                                                <asp:RequiredFieldValidator ID="rfvColumnFrom" runat="server" ControlToValidate="ddlColumnFrom"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna Desde es requerida" />
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblColumnTo" runat="server" Text="Columna Hasta" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlColumnTo" runat="server" Width="70px" TabIndex="1" />
                                                <asp:RequiredFieldValidator ID="rfvColumnTo" runat="server" ControlToValidate="ddlColumnTo"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Columna Hasta es requerida" />
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblAisle" runat="server" Text="Pasillo" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtAisle" runat="server" Width="50px" />
                                            </div>
                                        </div>
                                        
                                        <%--Nivel--%>
                                        <div id="divFiltroLevel" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLevelFrom" runat="server" Text="Nivel Desde" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlLevelFrom" runat="server" Width="70px" />
                                                <asp:RequiredFieldValidator ID="rfvLevelFrom" runat="server" ControlToValidate="ddlLevelFrom"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel Desde es requerido" />
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblLevelTo" runat="server" Text="Nivel Hasta" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlLevelTo" runat="server" Width="70px" />
                                                <asp:RequiredFieldValidator ID="rfvLevelTo" runat="server" ControlToValidate="ddlLevelTo"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Nivel Hasta es requerido" />
                                            </div>
                                           
                                        </div>
                                        
                                        <%-- Fila / Row / Hilera--%>
                                        <div id="divFiltroRow" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFrom" runat="server" Text="Fila Desde" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlRowFrom" runat="server" Width="70px" />
                                                <asp:RequiredFieldValidator ID="rfvRowFrom" runat="server" ControlToValidate="ddlRowFrom"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fila desde es requerido" />
                                                <br />
                                            </div>
                                            <div class="fieldRight">
                                                <asp:Label ID="lblRowTo" runat="server" Text="Fila Hasta" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlRowTo" runat="server" Width="70px" />
                                                <asp:RequiredFieldValidator ID="rfvRowTo" runat="server" ControlToValidate="ddlRowTo"
                                                    InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Fila hasta es requerido" />
                                            </div>
                                            <asp:Label ID="lblError" ForeColor="Red" runat="server"></asp:Label>
                                        </div>
                                        <br />
                                        <%-- Check Boxes--%>
                                        <div id="div6" class="divControls">
                                            <div class="fieldLeft">
                                                <br />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:CheckBox ID="chkAssociated" Checked="false" runat="server" Text="Buscar Asociadas" /> 
                                                <asp:CheckBox ID="chkNoAssociated" Checked="true" runat="server" Text="Buscar No Asociadas" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="div1" runat="server" class="modalActions">
                                    <asp:Button ID="btnSearchLoc" runat="server" Text="Buscar" CausesValidation="true"
                                        OnClick="btnSearchLoc_Click" ValidationGroup="SearchLoc" />
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SearchLoc"
                                        CssClass="modalValidation" />
                                </div>
                            </div>
                            <%--Contadores--%>
                            <div>
                                <asp:Label ID="lblLocationCount1" runat="server" Text="Total Asociadas: " Font-Size="X-Small" />
                                &nbsp;&nbsp;
                                <asp:Label ID="lblLocationCount2" runat="server" Text="Total Sin Asociar: " Font-Size="X-Small" />
                            </div>
                            <%-- FIN Filtro de Ubicaciones --%>
                            <hr />
                            <div id="div2" runat="server" visible="true">
                                <%--<div class="divCenter"> --%> 
                                    <asp:Label ID="lblWorkZoneName" runat="server" Text="" Font-Size="Small" />                                  
                                    <%--<asp:Label ID="lblLocationGridTitle1" runat="server" Text=" - Ubicaciones Asociadas"
                                        Font-Size="Small" />--%>
                                <%--</div>--%>
                                <%--<div class="divCenter">
                                    <asp:Label ID="lblLocationGridTitle2" runat="server" Text="Ubicaciones sin Asociar"
                                        Font-Size="Small" />
                                </div>--%>
                            </div>
                            <%--<br />--%>
                            <%--<br />--%>
                            <div id="divLocation2" runat="server" visible="true">
                                <div id="divlists" style="vertical-align: top;">
                                    <table>
                                        <tr>
                                            <td  style=" text-align:center;">
                                                <div class="fieldRight" style="width: 180px">
                                                    <fieldset style="width:180px; text-align:center;">
                                                    <legend>Ubicaciones Asociadas</legend>
                                                        <asp:ListBox ID="lstLocAsociated" runat="server" Width="150" Height="200" SelectionMode="Multiple">
                                                        </asp:ListBox>
                                                    </fieldset>
                                                </div>
                                            </td>
                                           
                                            <td style=" text-align:center;">
                                                <div class="fieldRight" style="vertical-align: top; width: 50px;">
                                                    <asp:Button ID="btnQuitarTodas" runat="server" Text=">>" Width="25px" ToolTip="Quitar Todas"
                                                        OnClick="btnQuitarTodas_Click" style=" text-align:center;" />
                                                    <asp:Button ID="btnQuitar" runat="server" Text=">" Width="25px" ToolTip="Quitar Seleccionadas"
                                                        OnClick="btnQuitarSelec_Click" />
                                                    <asp:Button ID="btnAgregar" runat="server" Text="<" Width="25px" ToolTip="Agregar Seleccionadas"
                                                        OnClick="btnAgregarSelec_Click" />
                                                    <asp:Button ID="btnAgregarTodas" runat="server" Text="<<" Width="25px" ToolTip="Agregar Todas"
                                                        OnClick="btnAgregarTodas_Click" />
                                                </div>
                                            </td>
                                            <td  style=" text-align:center;">
                                                <div class="fieldRight" style="width: 180px">
                                                    <fieldset style="width:180px; text-align:center;">
                                                    <legend>Ubicaciones sin Asociar</legend>
                                                        <asp:ListBox ID="lstLocNoAsociated" runat="server" Width="150" Height="200" SelectionMode="Multiple">
                                                        </asp:ListBox>
                                                    </fieldset>
                                                </div>
                                            </td>
                                        </tr>
                                    </table> 
                                    
                                </div>
                            </div>
                            </div>
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
                            <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$grdMgr" EventName="RowCommand" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$grdMgr" EventName="PageIndexChanging" />
                            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$grdMgr" EventName="SelectedIndexChanged" />
               
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="uprLocations" runat="server" AssociatedUpdatePanelID="upGridLocation1"
                        DisplayAfter="20" DynamicLayout="true">
                        <ProgressTemplate>
                            <div class="divProgress">
                                <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server"
                        ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLocations" />
                    <%-- FIN Panel de Ubicaciones asociadas --%>
                </Content>
                <Footer Height="67">
                    <div style="color: White">
                        No Borrar - Evita que BottomPanel se solape con StatusBar</div>
                </Footer>
            </RightPanel>
        </spl:Splitter>
    </div>
    <%-- Pop up Editar/Nueva Zona --%>
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpeworkZone" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlworkZone" BackgroundCssClass="modalBackground" PopupDragHandleControlID="workZoneCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlworkZone" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="workZoneCaption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Zona" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Zona" />
                            <asp:ImageButton ID="btnClose" runat="server" CssClass="closeButton" ImageAlign="Top" ToolTip="Cerrar"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidEditIndex" runat="server" Value="-1" />
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                            <div id="divCodStatus" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblCodStatus" runat="server" Text="Activo" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkCodStatus" runat="server" />
                                </div>
                            </div>
                            <div id="divWarehouse" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblWarehouse" runat="server" Text="Centro Dist."></asp:Label>
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlWarehouse" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvWarehouse" runat="server" ControlToValidate="ddlWarehouse"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Centro Dist. es requerido" />
                                </div>
                            </div>
                            <div id="divType" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblTypeWorkZone" runat="server" Text="Tipo"></asp:Label>
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlTypeWorkZone" runat="server" />
                                    <asp:RequiredFieldValidator ID="rfvTypeWorkZone" runat="server" ControlToValidate="ddlTypeWorkZone"
                                        InitialValue="-1" ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo Zona es requerido" />
                                </div>
                            </div>
                            <div id="divName" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblName" runat="server" Text="Nombre" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="20" Width="150px" />
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtName" runat="server" ControlToValidate="txtName"
	                                     ErrorMessage="Nombre permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z0-9 ñáéíóú]*" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divDescription" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="90" Width="150px" />
                                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Descripción es requerido" />
                                    <asp:RegularExpressionValidator ID="revtxtDescription" runat="server" ControlToValidate="txtDescription"
	                                     ErrorMessage="Descripción permite ingresar solo caracteres alfanuméricos" 
	                                     ValidationExpression="[a-zA-Z0-9 ñáéíóú]*" ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>
                        <%--Mensaje de advertencia y botones de accion--%>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" CssClass="modalValidation" />
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsVertical$leftPanel$ctl01$grdMgr"
                EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
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
    <%-- FIN Pop up Editar/Nueva Zona --%>
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Zona?"
        Visible="false" />
    <asp:Label ID="lblErrorNoZoneSelected" runat="server" Text="Debe seleccionar una Zona de la Grilla"
        Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
