<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master"
    CodeBehind="GenerateTaskCycleCount.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Inventory.Administration.GenerateTaskCycleCount" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="webUc" Assembly="Flan.Controls" Namespace="Flan.Controls" %>
<%@ Register TagPrefix="obout" Namespace="OboutInc.Calendar2" Assembly="obout_Calendar2_NET" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs; 
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(SetDivs);
</script>

    <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="divGridTitleDispatch">
                <div class="divCenter">
                    <asp:Label ID="lblPendingOrders" runat="server" Text="Generar Tareas de conteo" />
                </div>
                <asp:ImageButton ID="btnReprocess" runat="server" OnClick="btnReprocess_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_process_dis.png"
                    ToolTip="Generar Tareas de conteo" />
            </div>
            <div class="container">
                <div class="row">
                    <div class="col-md-12">
                        <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();">
                            <%-- Grilla Principal --%>
                            <asp:GridView ID="grdMgr" runat="server" AllowPaging="True" AllowSorting="False"
                                OnRowCreated="grdMgr_RowCreated" EnableViewState="false" OnRowDataBound="grdMgr_RowDataBound"
                                AutoGenerateColumns="false"
                                CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                EnableTheming="false">
                                <Columns>
                                    <asp:TemplateField ShowHeader="False" AccessibleHeaderText="Actions">
                                        <HeaderTemplate>
                                            <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectOrder', this.checked)" id="chkAll" title="Seleccionar todos" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <center>
                                                <div style="width: 20px">
                                                    <asp:CheckBox ID="chkSelectOrder" runat="server" />
                                                </div>
                                            </center>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdCode" HeaderText="Ubicación" InsertVisible="True" ReadOnly="True"
                                        AccessibleHeaderText="IdLocCode" />
                                    <%--<asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />--%>
                                    <asp:TemplateField HeaderText="Hilera" AccessibleHeaderText="RowLoc">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowLoc" runat="server" Text='<%# Eval ( "Row" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Columna" AccessibleHeaderText="ColumnLoc">
                                        <ItemTemplate>
                                            <asp:Label ID="lblColumnLoc" runat="server" Text='<%# Eval ( "Column" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nivel" AccessibleHeaderText="LevelLoc">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLevelLoc" runat="server" Text='<%# Eval ( "Level" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="StockId" AccessibleHeaderText="StockId">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStockId" runat="server" Text='<%# Eval ( "Stock.Id" ) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Dueño" AccessibleHeaderText="IdOwn">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdOwner" runat="server" Text='<%# Eval ( "Owner.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOwnerTradeName" runat="server" Text='<%# Eval ( "Owner.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id Item" AccessibleHeaderText="IdItem">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdItem" runat="server" Text='<%# Eval ( "Stock.Item.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Codigo Item" AccessibleHeaderText="ItemCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval ( "Stock.Item.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre" AccessibleHeaderText="LongItemName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLongItemName" runat="server" Text='<%# Eval ( "Stock.Item.LongName" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Id CtgItem" AccessibleHeaderText="IdCtgItem">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdCtgItem" runat="server" Text='<%# Eval ( "Stock.CategoryItem.Id" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Categoría" AccessibleHeaderText="CtgName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCtgName" runat="server" Text='<%# Eval ( "Stock.CategoryItem.Name" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LPN" AccessibleHeaderText="IdLpnCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblIdLpnCode" runat="server" Text='<%# Eval ( "Stock.Lpn.Code" ) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cantidad" AccessibleHeaderText="ItemQty">
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemQty" runat="server" Text='<%# GetFormatedNumber(((decimal) Eval ("Stock.Qty") == -1)?" ":Eval ("Stock.Qty")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pendiente Entrada" AccessibleHeaderText="CountTaskSource">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCountTaskSource" runat="server" Text='<%# GetFormatedNumber(((int) Eval ("CountTaskSource") == -1)?"0":Eval ("CountTaskSource")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pendiente Salida" AccessibleHeaderText="CountTaskTarget">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCountTaskTarget" runat="server" Text='<%# GetFormatedNumber(((int) Eval ("CountTaskTarget") == -1)?"0":Eval ("CountTaskTarget")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Advertencia" AccessibleHeaderText="Message">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtValida" runat="server" Visible="false"></asp:TextBox>
                                            <asp:Image ID="imgWarning" Width="18px" Height="18px" runat="server" ImageUrl="~/WebResources/Images/Buttons/AlarmMessage/icon_warning_small.png"
                                                Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <%-- FIN Grilla Principal --%>
                        </div>
                    </div>
                </div>
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
            <asp:AsyncPostBackTrigger ControlID="ctl00$MainContent$ctl01$btnReprocess" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnGenerateTask" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- PopUp Liberar Pedidos --%>
    <asp:UpdatePanel ID="upRelease" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="divReleaseDispatch" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="mpReleaseDispatch" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlReleaseDispatch" BackgroundCssClass="modalBackground" PopupDragHandleControlID="Caption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlReleaseDispatch" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="Caption" runat="server" CssClass="modalHeader">
                        <div class="divCaption">
                            <asp:Label ID="lblEdit" runat="server" Text="Generar Tareas" />
                            <asp:ImageButton ID="btnClose" runat="server" CssClass="closeButton" ImageAlign="Top"
                                ImageUrl="~/WebResources/Images/Buttons/icon_close.png" ToolTip="Cerrar" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div id="divModalFields" runat="server" class="divCtrsFloatLeft">
                            <div id="divPickingTitle" runat="server" visible="false" class="divControls">
                                <u>Generar Tareas</u></div>
                            <div id="divPriority" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblPriority" runat="server" Text="Prioridad" /></div>
                                <div class="fieldLeft">
                                    <asp:TextBox ID="txtPriority" runat="server" MaxLength="3" Width="20" Text="10" />
                                    <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ControlToValidate="txtPriority"
                                        ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido" />
                                    <asp:RangeValidator ID="rvPriority" runat="server" ControlToValidate="txtPriority"
                                        ErrorMessage="Prioridad no contiene un número válido [1-10]" Text=" * " MaximumValue="10"
                                        MinimumValue="1" ValidationGroup="EditNew" Type="Integer" />
                                </div>
                            </div>
                            <div id="divOperator" runat="server" class="divControls">
                                <div class="fieldRight">
                                    <asp:Label ID="lblOperator" runat="server" Text="Operador Asignado" /></div>
                                <div class="fieldLeft">
                                    <asp:DropDownList ID="ddlOperator" runat="server" Width="100" />
                                </div>
                            </div>
                            <div id="divDate" runat="server" class="divControls" visible="true">
                                <div class="fieldRight">
                                    <asp:Label ID="lblDate" runat="server" Text="Fecha de ejecución" /><br />
                                </div>
                                <div class="fieldLeft">
                                    <obout:Calendar ID="calDate" runat="server" ShortDayNames="Lu, Ma, Mi, Ju, Vi, Sa, Do"
                                        CultureName="es-ES" TextBoxId="txtDate" DatePickerMode="true" ShowYearSelector="true"
                                        YearSelectorType="DropDownList" YearMinScroll="2005" YearMaxScroll="2015" ShowMonthSelector="true"
                                        MonthSelectorType="DropDownList" ScrollBy="1" Columns="1" TitleText="" DatePickerImagePath="..\..\WebResources\Images\Buttons\Filter\cal_date_picker.gif"
                                        DatePickerSynchronize="true" TextArrowLeft="<" TextArrowRight=">" CSSDatePickerButton="calendarDatePickerButton" />
                                    <asp:TextBox ID="txtDate" SkinID="txtFilter" runat="server" Width="66px" Enabled="true" />
                                    <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender7" runat="server" TargetControlID="txtDate"
                                        Mask="99/99/9999" AutoComplete="true" MessageValidatorTip="true" MaskType="Date"
                                        InputDirection="LeftToRight">
                                    </ajaxToolkit:MaskedEditExtender>
                                    <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Fecha Invalida"
                                        Type="Date" MinimumValue="01-01-1901" MaximumValue="01-01-2090" Display="Dynamic"
                                        ControlToValidate="txtDate">
                                    </asp:RangeValidator>
                                </div>
                            </div>
                        </div>
                        <div>
                            <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" ShowMessageBox="false"
                                CssClass="modalValidation" />
                        </div>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnGenerateTask" runat="server" OnClick="btnGenerateTask_Click" Text="Aceptar"
                                CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <%-- Modal Update Progress --%>
    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upRelease"
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
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>
    <asp:Label ID="lblFilterDate" runat="server" Text="Fin" Visible="false" />
    <asp:Label ID="lblFilterName" runat="server" Text="Destino" Visible="false" />
    <asp:Label ID="lblItemCode" runat="server" Text="Código Item" Visible="false" />
    <asp:Label ID="lblItemName" runat="server" Text="Nombre" Visible="false" />
    <asp:Label ID="lblErrorValidate" runat="server" Text="La ubicación tiene tareas pendientes" Visible="false" />
    <asp:Label ID="lblGenerateTask" runat="server" Text="La ó las tareas fueron generadas correctamente." Visible="false" />
       
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
