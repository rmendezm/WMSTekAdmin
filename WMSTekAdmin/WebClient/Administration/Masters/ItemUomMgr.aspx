<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="ItemUomMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.ItemUomMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>

<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>
<script type="text/javascript" language="javascript">
    window.onresize = resizeDivPrincipal; 
    
    function resizeDivPrincipal() {
        var h = document.body.clientHeight + "px";
        var w = document.body.clientWidth + "px";
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.height = h;
        document.getElementById("ctl00_MainContent_divMainPrincipal").style.width = w;
    }

    function clearFilterDetail(gridDetail) {
        if ($("#" + gridDetail).length == 0) {
            if ($("div.container").length == 2) { 
                $("div.container:last div.row-height-filter").remove();
            }
        }
    }

    function initializeGridDragAndDropCustom() {
        var gridDetail = 'ctl00_MainContent_hsMasterDetail_bottomPanel_ctl01_grdMgr';
        clearFilterDetail(gridDetail);
        initializeGridDragAndDrop('ItemUom_GetItemUomByIdItem', gridDetail);
    }

    function setDivsAfter() {
        var heightDivBottom = $("#__hsMasterDetailLD").height();
        var heightLabelsBottom = $(".divGridTitle").height();
        var extraSpace = 45;

        var totalHeight = heightDivBottom - heightLabelsBottom - extraSpace;
        $("#ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdItem").parent().css("max-height", totalHeight + "px");
    }
</script>

  <div runat="server" id="divMainPrincipal" style="width:100%;height:100%;margin:0px;margin-bottom:80px">
    <spl:HorizontalSplitter LiveResize="false" CookieDays="0" ID="hsMasterDetail" runat="server" StyleFolder="~/WebResources/Styles/Obout/default" OnSplitterResize="SetDivs();">
        <TopPanel ID="topPanel" HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Principal --%>
                            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                                <ContentTemplate>
                                    <%-- Grilla de Items --%>
                                    <asp:GridView ID="grdItem" DataKeyNames="Id" runat="server" 
                                    AllowPaging="True" EnableViewState="False" 
                                    OnSelectedIndexChanged="grdItem_SelectedIndexChanged"
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    OnRowCreated="grdItem_RowCreated" 
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="Id" AccessibleHeaderText="Id" />
                                        <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                        <asp:TemplateField HeaderText="Cód. Dueño" AccessibleHeaderText="OwnerCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerCode" runat="server" Text='<%# Eval ( "Owner.Code" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="Owner">
                                            <ItemTemplate>
                                                <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Description" HeaderText="Descripción" AccessibleHeaderText="Description" />
                                        <asp:TemplateField HeaderText="Sector" AccessibleHeaderText="GrpItem1">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGroupItem1" Text='<%# Bind("GrpItem1.Name") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rubro" AccessibleHeaderText="GrpItem2">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGroupItem2" Text='<%# Bind("GrpItem2.Name") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Familia" AccessibleHeaderText="GrpItem3">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGroupItem3" Text='<%# Bind("GrpItem3.Name") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Subfamilia" AccessibleHeaderText="GrpItem4">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGroupItem4" Text='<%# Bind("GrpItem4.Name") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="LongName" HeaderText="Nombre Largo" AccessibleHeaderText="LongName" />
                                        <asp:BoundField DataField="ShortName" HeaderText="Nombre corto" AccessibleHeaderText="ShortName" />
                                        <asp:TemplateField HeaderText="Activo" AccessibleHeaderText="Status">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkStatus" runat="server" Checked='<%# Eval ( "Status" ) %>' Enabled="false" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="lblEmptyGrid" runat="server" Text="No se han encontrado registros." />
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                <%-- FIN Grilla de Items --%>
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

                <asp:UpdateProgress ID="uprItems" runat="server" AssociatedUpdatePanelID="upGrid" 
                DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>                        
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <webUc:UpdateProgressOverlayExtender ID="upoeItems" runat="server" ControlToOverlayID="topPanel" CssClass="updateProgress" TargetControlID="uprItems" />       
                <%-- FIN Panel Grilla Principal --%>    
            </Content>
        </TopPanel>
        <BottomPanel ID="bottomPanel" HeightMin="50">
            <Content>
                <div class="container">
                    <div class="row">
                        <div class="col-md-12">
                            <%-- Panel Grilla Detalle --%>
                            <asp:UpdatePanel ID="upGridUom" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <%-- Uoms --%>
                                    <div id="divUom" runat="server" visible="false" class="divGridDetailScroll">
                                        <div class="divGridDetailTitle">
                                            <asp:Label ID="lblUomGridTitle" runat="server" Text="Presentaciones: " />
                                            <asp:Label ID="lblItemName" runat="server" Text="" />
                                        </div>
                                        <%-- Grilla UOM --%>
                            
                                        <asp:Label ID="lblEmpty" runat="server" Text="Debe seleccionar un item" ForeColor="Red" />
                                        <asp:GridView ID="grdMgr" runat="server" DataKeyNames="Id" OnRowCreated="grdMgr_RowCreated"
                                        AllowPaging="False" EnableViewState="False" 
                                        OnRowDeleting="grdMgr_RowDeleting" OnRowEditing="grdMgr_RowEditing"
                                        OnRowDataBound="grdMgr_RowDataBound"
                                        AutoGenerateColumns="false"
                                        CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                        EnableTheming="false">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="ID" AccessibleHeaderText="Id" />
                                        <asp:TemplateField HeaderText="Acciones">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_edit.png"
                                                        CausesValidation="false" CommandName="Edit" ToolTip="Editar" />
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/WebResources/Images/Buttons/GridActions/icon_delete.png"
                                                        CausesValidation="false" CommandName="Delete" ToolTip="Eliminar" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Código Uom" AccessibleHeaderText="UomCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblCodeUom" runat="server" Text='<%# Eval("Code") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Factor de conversión" AccessibleHeaderText="ConversionFactor">
                                            <ItemTemplate>
                                                <asp:Label ID="lblConversionFactor" runat="server" Text='<%# GetFormatedNumber(Eval("ConversionFactor")) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Código de barra"  AccessibleHeaderText="BarCode">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBarCode" runat="server" Text='<%# Eval("BarCode") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nombre"  AccessibleHeaderText="UomName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Peso" AccessibleHeaderText="Weight">
                                            <ItemTemplate>
                                                <asp:Label ID="lblWeight" runat="server" Text='<%# (((decimal)Eval("Weight")== -1) ? " ":Eval("Weight"))%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Volumen" AccessibleHeaderText="Volume">
                                            <ItemTemplate>
                                                <asp:Label ID="lblVolume" runat="server" Text='<%# (((decimal)Eval("Volume")== -1) ? " ":Eval("Volume"))%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largo"  AccessibleHeaderText="Height">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLength" runat="server" Text='<%# (((decimal)Eval("Length")== -1) ? " ":Eval("Length"))%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ancho"  AccessibleHeaderText="Width">
                                            <ItemTemplate>
                                                <asp:Label ID="lblWidth" runat="server" Text='<%# (((decimal)Eval("Width")== -1) ? " ":Eval("Width"))%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Alto" AccessibleHeaderText="Height">
                                            <ItemTemplate>
                                                <asp:Label ID="lblHeight" runat="server" Text='<%# (((decimal)Eval("Height")== -1) ? " ":Eval("Height"))%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max. de presentaciones por base pallet" AccessibleHeaderText="LayoutUomQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUomQty" runat="server" Text='<%# ((int)Eval("UomQty") == -1) ? " " : Eval("UomQty")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max. cantidad apilada" AccessibleHeaderText="LayoutUnitQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUnitQty" runat="server" Text='<%# ((int)Eval("UnitQty") == -1) ? " " : Eval("UnitQty")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max. peso apilado" AccessibleHeaderText="LayoutMaxWeightUpon">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMaxWeightUpon" runat="server" Text='<%# ((((decimal)Eval("MaxWeightUpon")== -1) ? " ":Eval("MaxWeightUpon")))%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                         <asp:TemplateField HeaderText="Big Ticket" AccessibleHeaderText="BigTicket">
                                            <ItemTemplate>
                                                <center>
                                                    <asp:CheckBox ID="chkBigTicket" runat="server" Checked='<%# Eval ( "BigTicket" ) %>' Enabled="false" />
                                                </center>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <asp:Label ID="lblEmptyGrid" runat="server" Text="Este Item no tiene presentaciones asociadas." />
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                        <%-- FIN Grilla UOM --%>
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
                                     <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$topPanel$ctl01$grdItem" EventName="RowCommand" />
                                     <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                                     <%--<asp:AsyncPostBackTrigger ControlID="ctl00_MainContent_hsMasterDetail_topPanel_ctl01_grdItem" EventName="RowCommand" />--%>
                                </Triggers>
                            </asp:UpdatePanel>  
                        </div>
                    </div>
                 </div>    
                    
                 <asp:UpdateProgress ID="uprDetail" runat="server" AssociatedUpdatePanelID="upGridUom" 
                 DisplayAfter="20" DynamicLayout="true">
                    <ProgressTemplate>                        
                        <div class="divProgress">
                            <asp:ImageButton ID="imgProgress2" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />	        
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                 <webUc:UpdateProgressOverlayExtender ID="UpdateProgressOverlayExtender1" runat="server" 
                 ControlToOverlayID="divUom" CssClass="updateProgress" TargetControlID="uprDetail" />     
                <%-- FIN Panel Grilla Detalle --%>
            </Content>
            <Footer Height="67">
                <div style="color:White">No Borrar - Evita que BottomPanel se solape con StatusBar</div>
            </Footer>
        </BottomPanel>
    </spl:HorizontalSplitter>
  </div>    
    
    <%-- Panel Nuevo/Editar ItemUom --%>
     <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <%-- Pop up Editar/Nueva Presentación --%>
            <div id="divEditNew" runat="server" visible="false">    
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnl" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnl" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblNew" runat="server" Text="Nueva Presentación" />
                            <asp:Label ID="lblEdit" runat="server" Text="Editar Presentación" />
                            <asp:ImageButton ID="btnClose" runat="server" ImageAlign="Top" ToolTip="Cerrar" CssClass="closeButton" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <div class="divCtrsFloatLeft">
                            <div id="divStatusUom" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblStatus" runat="server" Text="Activo" /></div>
                                <div class="fieldLeft"><asp:CheckBox ID="chkStatus" runat="server" Checked="false" /></div>
                            </div>                        
                            <div id="divUomCode" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblUomCode" runat="server" Text="Código" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtUomCode" runat="server" MaxLength="20" Width="140" />
                                    <asp:RequiredFieldValidator ID="rfvUomCode" runat="server" ControlToValidate="txtUomCode"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />
                                    <asp:RegularExpressionValidator ID="revUomCode" runat="server" ControlToValidate="txtUomCode"
                                         ErrorMessage="Código permite ingresar solo caracteres alfanuméricos" 
                                         ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ-]*"
                                         ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div id="divConversionFactor" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblConversionFactor" runat="server" Text="Factor de Conversión" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtConversionFactor" runat="server" MaxLength="8" Width="150" />
                                <asp:RequiredFieldValidator ID="rfvConversionFactor" runat="server" ControlToValidate="txtConversionFactor"
                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Factor de Conversión es requerido">
                                </asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvConversionFactor" runat="server" ControlToValidate="txtConversionFactor"
                                    ErrorMessage="Factor de Conversión no contiene un número válido" MaximumValue="99999999"
                                    MinimumValue="1" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator></div>
                            </div>
                            <div id="divBarCode" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblBarCode" runat="server" Text="Código de Barra" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtBarCode" runat="server" MaxLength="30" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvBarCode" runat="server" ControlToValidate="txtBarCode"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Código de Barra es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revtxtBarCode" runat="server" ControlToValidate="txtBarCode"
                                         ErrorMessage="Código de Barra permite ingresar solo caracteres alfanuméricos" 
                                         ValidationExpression="[a-zA-Z 0-9999999]*"
                                         ValidationGroup="EditNew" Text=" * ">
                                    </asp:RegularExpressionValidator>
                                </div>
                            </div>
                            
                            <div id="divUomName" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblUomName" runat="server" Text="Unidad de Medida" /></div>
                                <div class="fieldLeft"><asp:DropDownList ID="ddlUomName" runat="server" MaxLength="20" Width="150" />
                                    <asp:RequiredFieldValidator ID="rfvUomName" runat="server" ControlToValidate="ddlUomName"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Unidad de Medida es requerido">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                                                    
                            <div id="divLength" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblLength" runat="server" Text="Largo" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLength" runat="server" MaxLength="13" Width="150" />
                                    <asp:Label ID="lblTypeUnitMeasure" runat="server"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvLength" runat="server" ControlToValidate="txtLength"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Largo es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="RVLength" runat="server" ControlToValidate="txtLength" ErrorMessage="Largo no contiene un número válido"
                                        MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divWidth" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblWidth" runat="server" Text="Ancho" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWidth" runat="server" MaxLength="13" Width="150" />
                                    <asp:Label ID="lblTypeUnitMeasure2" runat="server"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvWidth" runat="server" ControlToValidate="txtWidth"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Ancho es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvWidth" runat="server" ControlToValidate="txtWidth" ErrorMessage="Ancho no contiene un número válido"
                                        MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*
                                    </asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divHeight" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblHeight" runat="server" Text="Alto" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtHeight" runat="server" MaxLength="13" Width="150" />
                                    <asp:Label ID="lblTypeUnitMeasure3" runat="server"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvHeight" runat="server" ControlToValidate="txtHeight"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Alto es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvHeight" runat="server" ControlToValidate="txtHeight" ErrorMessage="Alto no contiene un número válido"
                                        MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divVolume" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblVolume" runat="server" Text="Volumen" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtVolume" runat="server" MaxLength="13" Width="150" />                                    
                                    <asp:Label ID="lblTypeUnitMeasure4" runat="server">(m³)</asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvVolume" runat="server" ControlToValidate="txtVolume"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Volumen es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvVolume" runat="server" ControlToValidate="txtVolume" ErrorMessage="Volumen no contiene un número válido"
                                        MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divWeight" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblWeight" runat="server" Text="Peso" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtWeight" runat="server" MaxLength="13" />
                                    <asp:Label ID="lblTypeUnitOfMass2" runat="server"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvWeight" runat="server" ControlToValidate="txtWeight"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Peso es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvWeight" runat="server" ControlToValidate="txtWeight" ErrorMessage="Peso no contiene un número válido"
                                        MaximumValue="99999999" MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divLayoutUomQty" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblLayoutUomQty" runat="server" Text="Max. de presentaciones por base pallet" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtLayoutUomQty" runat="server" MaxLength="9" Width="100" />
                                    <asp:RequiredFieldValidator ID="rfvLayoutUomQty" runat="server" ControlToValidate="txtLayoutUomQty"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Max. de presentaciones por base pallet es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvLayoutUomQty" runat="server" ControlToValidate="txtLayoutUomQty"
                                        ErrorMessage="Max. de presentaciones por base pallet no contiene un número válido" MaximumValue="2147483640"
                                        MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divLayoutUnitQty" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblLayoutUnitQty" runat="server" Text="Max. Cantidad apilada" /></div>
                                <div class="fieldLeft"><asp:TextBox ID="txtLayoutUnitQty" runat="server" MaxLength="9" Width="100" />
                                    <asp:RequiredFieldValidator ID="rfvLayoutUnitQty" runat="server" ControlToValidate="txtLayoutUnitQty"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Max. Cantidad apilada es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvLayoutUnitQty" runat="server" ControlToValidate="txtLayoutUnitQty"
                                        ErrorMessage="Max. Cantidad apilada no contiene un número válido" MaximumValue="2147483640"
                                        MinimumValue="0" ValidationGroup="EditNew" Type="Integer">*</asp:RangeValidator>
                                </div>
                            </div>
                            <div id="divLayoutMaxWeightUpon" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblLayoutMaxWeightUpon" runat="server" Text="Max. Peso apilado" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtLayoutMaxWeightUpon" runat="server" MaxLength="13" Width="150" />
                                    <asp:Label ID="lblTypeUnitOfMass" runat="server"></asp:Label>
                                    <asp:RequiredFieldValidator ID="rfvLayoutMaxWeightUpon" runat="server" ControlToValidate="txtLayoutMaxWeightUpon"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Max. Peso apilado es requerido">
                                    </asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvLayoutMaxWeightUpon" runat="server" ControlToValidate="txtLayoutMaxWeightUpon"
                                        ErrorMessage="Max. Peso apilado no contiene un número válido" MaximumValue="99999999"
                                        MinimumValue="0" ValidationGroup="EditNew" Type="Double">*</asp:RangeValidator>
                                </div>
                            </div>

                            <div id="divBigTicket" runat="server" class="divControls">
                                <div class="fieldRight"><asp:Label ID="lblBigTicket" runat="server" Text="Big Ticket" /></div>
                                <div class="fieldLeft">
                                    <asp:CheckBox ID="chkBigTicket" runat="server" />
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
            <%-- FIN Pop up Editar/Nuevo Presentación --%>
       </ContentTemplate>
       <Triggers>
         <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$hsMasterDetail$bottomPanel$ctl01$grdMgr" EventName="RowCommand" />
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
    <%-- FIN Panel Nuevo/Editar ItemUom --%>

    <%-- Carga masiva de items --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Item --%>
            <div id="divLoad" runat="server" visible="false">
                <asp:Button ID="btnDummyLoad" runat="Server"  Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUpLoad" runat="server" TargetControlID="btnDummyLoad"
                    PopupControlID="panelLoad" BackgroundCssClass="modalBackground" PopupDragHandleControlID="panelCaptionLoad"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelLoad" runat="server" CssClass="modalBox" >
                    <%-- Encabezado --%>
                    <asp:Panel ID="panelCaptionLoad" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Presentaciones" />
                             <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Presentacion.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div2" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label4" runat="server" Text="Dueño" />
                                </div>
                                <div class="fieldLeft">
                                    <asp:DropDownList runat="server" ID="ddlOwnerLoad" AutoPostBack="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvOwnerLoad" runat="server" ControlToValidate="ddlOwnerLoad"
                                        InitialValue="-1" ValidationGroup="Load" Text=" * " ErrorMessage="Dueño es requerido" />
                                </div>
                            </div>
                        
                            <div class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="Label5" runat="server" Text="Seleccione Archivo" />
                                </div>
                                <div class="fieldLeft">                      
                                    <asp:FileUpload ID="uploadFile2" runat="server" Width="400px" ValidationGroup="Load"/>                                    
                                    <asp:RequiredFieldValidator ID="rfvUpload" runat="server" Text="*" ErrorMessage="Debe seleccionar un archivo"
                                    ValidationGroup="Load" ControlToValidate="uploadFile2"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator id="revUpload" runat="server"  ValidationGroup="Load"
                                    ErrorMessage="Cargar solo archivos .xls ó .xlsx"  Text="*"
                                    ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.xls|.xlsx)$" 
                                    ControlToValidate="uploadFile2">
                                    </asp:RegularExpressionValidator>                             
                                    
                                </div>  
                            </div>
                            <div class="divControls" >
                                <div class="fieldRight">
                                    <asp:Label ID="Label6" runat="server" Text="" />
                                </div>
                                <div class="fieldLeft"style=" width:400px; text-align:right;" >  
                                    <asp:Button ID="btnSubir2" runat="server" Text="Cargar Archivo" ValidationGroup="Load" 
                                    OnClientClick="showProgress()" onclick="btnSubir2_Click" />
                                </div>
                            </div>
                           <%-- <div style="clear: both">
                            </div>--%>
                        
                        </div>
                        <div style="clear: both">
                            </div>
                        <div class="divValidationSummary">
                            <asp:ValidationSummary ID="vsLoad" runat="server" ValidationGroup="Load"
                                ShowMessageBox="false" CssClass="modalValidation" />
                        </div>                            
                        <div id="div1" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
            <%--<asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />--%>
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="uprLoad" runat="server" AssociatedUpdatePanelID="upLoad" DisplayAfter="20" 
     DynamicLayout="true" >
        <ProgressTemplate>
            <div class="divProgress" style="z-index: 2147483647 !important;">
                <asp:ImageButton ID="imgProgressLoad" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprLoad" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprLoad" />
    
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
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar esta Presentación?" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Nombre Largo" Visible="false" />
    <asp:Label ID="lblFilterBarCode" runat="server" Text="Cód. de Barra" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Presentación" Visible="false" />
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen items en el archivo." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotFileLoad" runat="server" Text="Seleccione el archivo que desea cargar." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />

    <asp:Label ID="lblGtin" runat="server" Text="GTIN" Visible="false" />
    <asp:Label ID="lblGtinIsNotNumeric" runat="server" Text="GTIN debe ser Numérico." Visible="false" />
    <asp:Label ID="lblGtinLengthInvalid" runat="server" Text="Largo del GTIN no es válido." Visible="false" />
    <asp:Label ID="lblGtinCheckDigitGTIN8" runat="server" Text="Dígito Verificador del GTIN-8 no es Válido." Visible="false" />
    <asp:Label ID="lblGtinCheckDigitGTIN12" runat="server" Text="Dígito Verificador del GTIN-12 no es Válido." Visible="false" />
    <asp:Label ID="lblGtinCheckDigitGTIN13" runat="server" Text="Dígito Verificador del GTIN-13 no es Válido." Visible="false" />
    <asp:Label ID="lblGtinCheckDigitGTIN14" runat="server" Text="Dígito Verificador del GTIN-14 no es Válido." Visible="false" />

    <asp:Label ID="lblGtinMessageRequired" runat="server" Text="GTIN es requerido" Visible="false" />
    <asp:Label ID="lblBarCodeMessageRequired" runat="server" Text="Código de Barra es requerido" Visible="false" />
    <asp:Label ID="lblGtinMessageRegularExpression" runat="server" Text="GTIN permite ingresar solo caracteres numéricos" Visible="false" />
    <asp:Label ID="lblBarCodeMessageRegularExpression" runat="server" Text="Código de Barra permite ingresar solo caracteres alfanuméricos" Visible="false" />
   <asp:Label ID="lblErrorConversionFactorGTIN" runat="server" Text="Factor de Conversión debe ser mayor que 1 para GTIN-14" Visible="false" />
        
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>

<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
