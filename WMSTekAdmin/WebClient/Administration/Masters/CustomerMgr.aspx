<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Shared/WMSTekContent.Master" CodeBehind="CustomerMgr.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Masters.CustomerMgr" %>

<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
<script src="<%= Page.ResolveClientUrl("~/WebResources/Javascript/UtilMassive.js")%>"></script>
<script type="text/javascript" language="javascript">
    window.onresize = SetDivs;

    $(document).ready(function () {
        initializeGridDragAndDrop("Customer_FindAll", "ctl00_MainContent_grdMgr", "CustomerMgr");

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
        initializeGridDragAndDrop("Customer_FindAll", "ctl00_MainContent_grdMgr", "CustomerMgr");
    }
</script>

<div class="container">
    <div class="row">
        <div class="col-md-12">
            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <%-- Grilla Principal --%>
                    <div id="divGrid" runat="server" visible="true" class="divGrid" onresize="SetDivs();" >
                        <asp:GridView ID="grdMgr" DataKeyNames="Id" runat="server" OnRowCreated="grdMgr_RowCreated"
                            OnRowDeleting="grdMgr_RowDeleting" OnRowEditing="grdMgr_RowEditing" OnPageIndexChanging="grdMgr_PageIndexChanging"
                            OnRowDataBound="grdMgr_RowDataBound"
                            AllowPaging="True" EnableViewState="False" 
                            AutoGenerateColumns="False"
                            CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                            EnableTheming="false">
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" InsertVisible="True" ReadOnly="True" AccessibleHeaderText="Id" />
                                <asp:BoundField DataField="Name" HeaderText="Nombre" AccessibleHeaderText="Name" />
                                <asp:BoundField DataField="Code" HeaderText="Código" AccessibleHeaderText="Code" />
                                <asp:TemplateField HeaderText="Dueño" AccessibleHeaderText="OwnName">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblTradeName" runat="server" Text='<%# Eval  ( "Owner.Name" ) %>' ></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Address1Fact" HeaderText="Dir. Fac." AccessibleHeaderText="Address1Fact" />
                                <asp:BoundField DataField="Address2Fact" HeaderText="Dir. Fac. Opc." AccessibleHeaderText="Address2Fact" />
                                <asp:TemplateField HeaderText="País Fac." AccessibleHeaderText="CountryNameFact">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCountryFact" runat="server" Text='<%# Eval ( "CountryFact.Name" ) %>' />
                                       </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Región Fac." AccessibleHeaderText="StateNameFact">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">                                
                                            <asp:Label ID="lblStateFact" runat="server" Text='<%# Eval ( "StateFact.Name" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Comuna Fac." AccessibleHeaderText="CityNameFact">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCityFact" runat="server" Text='<%# Eval ( "CityFact.Name" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="PhoneFact" HeaderText="Teléfono Fac." AccessibleHeaderText="PhoneFact" />
                                <asp:BoundField DataField="FaxFact" HeaderText="Fax Fac." AccessibleHeaderText="FaxFact" />
                                <asp:BoundField DataField="Address1Delv" HeaderText="Dir. Entrega" AccessibleHeaderText="Address1Delv" />
                                <asp:BoundField DataField="Address2Delv" HeaderText="Dir. Entrega Opc." AccessibleHeaderText="Address2Delv" />
                                <asp:TemplateField HeaderText="País Entrega" AccessibleHeaderText="CountryNameDelv">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCountryDelv" runat="server" Text='<%# Eval ( "CountryDelv.Name" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Localidad Entrega" AccessibleHeaderText="StateNameDelv">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblStateDelv" runat="server" Text='<%# Eval ( "StateDelv.Name" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Comuna Entrega" AccessibleHeaderText="CityNameDelv">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblCityDelv" runat="server" Text='<%# Eval ( "CityDelv.Name" ) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="PhoneDelv" HeaderText="Teléfono Entrega" AccessibleHeaderText="PhoneDelv" />
                                <asp:BoundField DataField="FaxDelv" HeaderText="Fax Entrega" AccessibleHeaderText="FaxDelv" />
                                <asp:BoundField DataField="Email" HeaderText="E-mail" AccessibleHeaderText="Email" />

                                <asp:TemplateField HeaderText="Prioridad" AccessibleHeaderText="Priority">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblPriority" runat="server" Text='<%# ((int) Eval ("Priority") > -1) ? Eval("Priority") : "" %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tiempo Esperado" AccessibleHeaderText="TimeExpected">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblTimeExpected" runat="server" Text='<%# ((int) Eval ("TimeExpected") > -1) ? Eval("TimeExpected") : "" %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dias Vencimiento" AccessibleHeaderText="ExpirationDays">
                                    <ItemTemplate>
                                        <div style="word-wrap: break-word;">
                                            <asp:Label ID="lblExpirationDays" runat="server" Text='<%#  ((int) Eval ("ExpirationDays") > -1) ? Eval("ExpirationDays") : "" %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="SpecialField1" HeaderText="Tipo" AccessibleHeaderText="SpecialField1" />
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
    
    <asp:UpdatePanel ID="upEditNew" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Customer --%>
            <div id="divEditNew" runat="server" visible="false">
                <asp:Button ID="btnDummy" runat="Server" Style="display: none" />
                <!-- Boton 'dummy' para propiedad TargetControlID -->
                <ajaxToolkit:ModalPopupExtender ID="modalPopUp" runat="server" TargetControlID="btnDummy"
                    PopupControlID="pnlCustomer" BackgroundCssClass="modalBackground" PopupDragHandleControlID="CustomerCaption"
                    Drag="true">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="pnlCustomer" runat="server" CssClass="modalBox">
                    <%-- Encabezado --%>
                    <asp:Panel ID="CustomerCaption" runat="server" CssClass="modalHeader">
                            <div class="divCaption">
                                <asp:Label ID="lblNew" runat="server" Text="Nuevo Cliente" />
                                <asp:Label ID="lblEdit" runat="server" Text="Editar Cliente" />
                                <asp:ImageButton ID="btnClose" runat="server" ToolTip="Cerrar" CssClass="closeButton" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                            </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <asp:HiddenField ID="hidEditId" runat="server" Value="-1" />
                        <asp:HiddenField ID="hidEditIdB2B" runat="server" Value="-1" />
                        <ajaxToolkit:TabContainer runat="server" ID="tabCustomer" ActiveTabIndex="0">
                            <ajaxToolkit:TabPanel runat="server" ID="tabGeneral">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <div id="divOwner" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblOwner" runat="server" Text="Dueño" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlOwner" runat="server" Width="120px"/>
                                                <asp:RequiredFieldValidator ID="rfvIdOwner" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlOwner"  Display="Dynamic" InitialValue="-1" 
                                                    ErrorMessage="Dueño es requerido" /></div>
                                        </div>
                                               
                                           
                                        <div id="divCode" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCode" runat="server" Text="Código" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtCode" runat="server" MaxLength="20" Width="150px" />
                                                <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="txtCode"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Código es requerido" />                                                    
                                            </div>
                                        </div>
                                                                                                                    
                                        <div id="divName" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblName" runat="server" Text="Nombre" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="150px" />
                                                <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Nombre es requerido"></asp:RequiredFieldValidator>                                                 
                                            </div>
                                        </div>

                                        <div id="divEmail" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblEmail" runat="server" Text="E-mail" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtEmail" runat="server" MaxLength="90" Width="150px" />
                                                <asp:RegularExpressionValidator ID="revMail" runat="server" ControlToValidate="txtEmail"
                                                    ErrorMessage="Email Inválido" Text="*
                                                "  ValidationGroup="EditNew" 
                                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="E-mail es requerido"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div id="divTimeExpected" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblTimeExpected" runat="server" Text="Tiempo Esperado" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtTimeExpected" runat="server" MaxLength="5" Width="150px" />
                                                <asp:Label ID="lblFormatTime" runat="server" Text="(hrs)"></asp:Label>
                                                <asp:RequiredFieldValidator ID="rfvTimeExpected" runat="server" ControlToValidate="txtTimeExpected"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Tiempo Esperado es requerido" Enabled="false"></asp:RequiredFieldValidator>
                                                 <asp:RangeValidator ID="rvTimeExpected" runat="server" 
                                                    ControlToValidate="txtTimeExpected" Text="*" 
                                                        ErrorMessage="Tiempo Esperado no contiene un número válido"
                                                        MaximumValue="32767" MinimumValue="0" ValidationGroup="EditNew" 
                                                    Type="Integer"></asp:RangeValidator>
                                            </div>
                                        </div>
                                        <div id="divExpiration" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblExpiration" runat="server" Text="Días Vencimiento" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtExpiration" Width="100px" runat="server" MaxLength="5" />
                                                <asp:RequiredFieldValidator ID="rfvExpiration" runat="server" ControlToValidate="txtExpiration"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Días Vencimiento es requerido" Enabled="false" />
                                                 <asp:RangeValidator ID="rvExpiration" runat="server" Text="*" 
                                                    ControlToValidate="txtExpiration" ErrorMessage="Días Vencimiento no contiene un número válido"
                                                        MaximumValue="32767" MinimumValue="0" ValidationGroup="EditNew" 
                                                    Type="Integer"></asp:RangeValidator>
                                                    
                                            </div>
                                        </div>
                                        <div id="divPriority" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblPriority" runat="server" Text="Prioridad" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtPriority" runat="server" MaxLength="5" Width="50px" />
                                                <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ControlToValidate="txtPriority"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Prioridad es requerido" Enabled="false"></asp:RequiredFieldValidator>
                                                 <asp:RangeValidator ID="rvPriority" runat="server" Text="*" 
                                                    ControlToValidate="txtPriority" ErrorMessage="Prioridad no contiene un número válido"
                                                        MaximumValue="32767" MinimumValue="0" ValidationGroup="EditNew" 
                                                    Type="Integer"></asp:RangeValidator>
                                            </div>
                                        </div>
                                        
                                        <div id="divSpecialField1" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblSpecialField1" runat="server" Text="Tipo" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtSpecialField1" runat="server" MaxLength="50" 
                                                    Width="150px" />
                                               <%-- <asp:RequiredFieldValidator ID="rfvSpecialField1" runat="server" ControlToValidate="txtSpecialField1"
                                                    ValidationGroup="EditNew" Text=" * " ErrorMessage="Tipo es requerido"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revSpecialField1" runat="server" 
                                                    ControlToValidate="txtSpecialField1"
                                                    ErrorMessage="Tipo permite ingresar solo caracteres alfanuméricos" 
                                                    ValidationExpression="[a-zA-Z 0-9999999ñÑçÇáÁäÄàÀâÂéÉëËèÈêÊíÍïÏìÌîÎóÓöÖòÒôÔúÚüÜùÙûÛ_.-]*"
                                                    ValidationGroup="EditNew" Text=" * "></asp:RegularExpressionValidator> --%>                                                 
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divValidationSummary">
                                        <asp:ValidationSummary ID="rfvSummary" runat="server" ValidationGroup="EditNew" 
                                            ShowMessageBox="True" CssClass="modalValidation"/>
                                    </div>    
                                    <div style="clear: both"></div>                                     
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="TabDelivery">
                                <ContentTemplate>
                                    <div class="divCtrsFloatLeft">
                                        <%--DeliveryAddress1--%>
                                        <div id="divDeliveryAddress1" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryAddress1" Text="Dir. Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryAddress1" runat="server" MaxLength="60"/>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryAddress1" ValidationGroup="EditNew" ControlToValidate="txtDeliveryAddress1"
                                                    runat="server" ErrorMessage="Dir. Entrega es requerido" Text=" * "  Enabled="false"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <%--DeliveryAddress2--%>
                                        <div id="divDeliveryAddress2" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryAddress2" Text="Dir. Entrega Opc." runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryAddress2" runat="server" MaxLength="60"/>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryAddress2" ValidationGroup="EditNew" ControlToValidate="txtDeliveryAddress2"
                                                    runat="server" ErrorMessage="Dir. Entrega Opc. es requerido" Text=" * " Enabled="false"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <%--CountryDelivery--%>
                                        <div id="divCountryDelivery" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCountryDelivery" Text="País Entrega" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCountryDelivery" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCountryDelivery_SelectedIndexChanged"
                                                    Width="130px" />
                                                <asp:RequiredFieldValidator ID="rfvCountryDelivery" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCountryDelivery" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="País Entrega es requerido" Enabled="false" />
                                            </div>
                                        </div>
                                        <%--StateDelivery--%>
                                        <div id="divStateDelivery" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblStateDelivery" Text="Localidad Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlStateDelivery" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStateDelivery_SelectedIndexChanged"
                                                    Width="130px" />
                                                <asp:RequiredFieldValidator ID="rfvStateDelivery" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlStateDelivery" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Región Entrega es requerido" Enabled="false" />
                                            </div>
                                        </div>
                                        <%--CityDelivery--%>
                                        <div id="divCityDelivery" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCityDelivery" Text="Ciudad Entrega" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCityDelivery" runat="server" Width="130px" />
                                                <asp:RequiredFieldValidator ID="rfvCityDelivery" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCityDelivery" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Ciudad Entrega es requerido" Enabled="false"/>
                                            </div>
                                        </div>
                                        <%-- DeliveryPhone --%>
                                        <div id="divDeliveryPhone" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblDeliveryPhone" Text="Tel. Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtDeliveryPhone" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryPhone" Text=" * " ValidationGroup="EditNew" ControlToValidate="txtDeliveryPhone"
                                                    runat="server" ErrorMessage="Tel. Entrega es requerido" Enabled="false"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revtxtDeliveryPhone" runat="server" 
                                                ControlToValidate="txtDeliveryPhone" ErrorMessage="Tel. Entrega permite ingresar solo números" 
                                                ValidationExpression="[0-99999999999]*"
                                                ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator> 
                                            </div>
                                        </div>
                                        <%-- FaxPhone --%>
                                        <div id="divFaxPhoneDev" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFaxPhoneDev" Text="Fax Entrega" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFaxPhoneDev" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFaxPhoneDev" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtFaxPhoneDev" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Fax Entrega es requerido" Enabled="false"/>
                                                <asp:RegularExpressionValidator ID="revtxtFaxPhoneDev" runat="server" 
                                                ControlToValidate="txtFaxPhoneDev" ErrorMessage="Fax Entrega permite ingresar solo números" 
                                                ValidationExpression="[0-99999999999]*"
                                                ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator> 
                                            </div>
                                        </div>                                       
                                    </div>
                                    <div style="clear: both"></div> 
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                            <ajaxToolkit:TabPanel runat="server" ID="TabSales">
                                <ContentTemplate>
                                    <div id="Div2" class="divCtrsFloatLeft">
                                        <%--FactAddress1 / direccion de Factura 1--%>
                                        <div id="divFactAddress1" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactAddress1" Text="Dir. Factura" runat="server" />
                                            </div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactAddress1" runat="server" Width="150px" MaxLength="60"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFactAddress1" ValidationGroup="EditNew" ControlToValidate="txtFactAddress1"
                                                    runat="server" ErrorMessage="Dir. Factura es requerido" Text=" * " ></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <%--FactAddress2 / direccion de Factura 2--%>
                                        <div id="divFactAddress2" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactAddress2" Text="Dir. Factura Opc." runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactAddress2" runat="server" Width="150px" MaxLength="60"></asp:TextBox>
                                             <asp:RequiredFieldValidator ID="rfvFactAddress2" ValidationGroup="EditNew" ControlToValidate="txtFactAddress2"
                                                    runat="server" Text=" * " ErrorMessage="Dir. Factura Opc. es requerido" Enabled="false" ></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <%--CountryFact--%>
                                        <div id="divCountryFact" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCountryFact" Text="País Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCountryFact" runat="server" Width="130px" OnSelectedIndexChanged="ddlCountryFact_SelectedIndexChanged"
                                                    AutoPostBack="True" />
                                               <asp:RequiredFieldValidator ID="rfvCountryFact" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCountryFact" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="País Factura es requerido" />
                                            </div>
                                        </div>
                                        <%--StateFact--%>
                                        <div id="divStateFact" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblStateFact" Text="Localidad Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlStateFact" runat="server" Width="130px" OnSelectedIndexChanged="ddlStateFact_SelectedIndexChanged"
                                                    AutoPostBack="True" />
                                                <asp:RequiredFieldValidator ID="rfvStateFact" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlStateFact" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Región Factura es requerido" />
                                            </div>
                                        </div>
                                        <%--CityFact--%>
                                        <div id="divCityFact" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblCityFact" Text="Ciudad Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:DropDownList ID="ddlCityFact" runat="server" Width="130px" />
                                                <asp:RequiredFieldValidator ID="rfvCityFact" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="ddlCityFact" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Ciudad Factura es requerido" />
                                            </div>
                                        </div>
                                        <%-- FactPhone --%>
                                        <div id="divFactPhone" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFactPhone" Text="Tel. Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFactPhone" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFactPhone" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtFactPhone" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Tel. Factura es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtFactPhone" runat="server" 
                                                ControlToValidate="txtFactPhone" ErrorMessage="Tel. Factura permite ingresar solo números" 
                                                ValidationExpression="[0-99999999999]*"
                                                ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator> 
                                            </div>
                                        </div>
                                        <%-- FaxPhone --%>
                                        <div id="divFaxPhoneFac" runat="server" class="divControls">
                                            <div class="fieldRight">
                                                <asp:Label ID="lblFaxPhoneFac" Text="Fax Factura" runat="server" /></div>
                                            <div class="fieldLeft">
                                                <asp:TextBox ID="txtFaxPhoneFac" runat="server" Width="100px" MaxLength="20"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFaxPhoneFac" runat="server" ValidationGroup="EditNew"
                                                    Text=" * " ControlToValidate="txtFaxPhoneFac" Display="dynamic" InitialValue="-1"
                                                    ErrorMessage="Fax Factura es requerido" />
                                                <asp:RegularExpressionValidator ID="revtxtFaxPhoneFac" runat="server" 
                                                ControlToValidate="txtFaxPhoneFac" ErrorMessage="Fax Factura permite ingresar solo números"                                                  ValidationExpression="[0-99999999999]*"
                                                ValidationGroup="EditNew" Text=" * ">
                                                </asp:RegularExpressionValidator> 
                                            </div>
                                        </div>                                    
                                    </div>
                                    <div style="clear: both"></div> 
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                        </ajaxToolkit:TabContainer>
                        <div id="divActions" runat="server" class="modalActions">
                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Aceptar" CausesValidation="true" ValidationGroup="EditNew" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancelar" />
                        </div>                              
                    </div>
                </asp:Panel>
             </div>
            <%-- Pop up Editar/Nuevo Customer --%>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grdMgr" EventName="RowCommand" />
            <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnNew" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <%-- Modal Update Progress --%>
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
    <%-- FIN Modal Update Progress --%>

    <%-- Carga masiva de Clientes --%>
    <asp:UpdatePanel ID="upLoad" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- Pop up Editar/Nuevo Cliente --%>
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
                            <asp:Label ID="Label3" runat="server" Text="Carga Masiva de Clientes" />
                            <asp:HyperLink runat="server" NavigateUrl="~/WebResources/XlsTemplateFile/Carga%20Masiva%20Clientes.xlsx" 
                            ImageUrl="~/WebResources/Images/Buttons/Download XLS.png" CssClass="downloadFile"
                            ToolTip="Descargar archivo de ejemplo" />
                            <asp:ImageButton ID="ImageButton1" runat="server" CssClass="closeButton" ToolTip="Cerrar" ImageAlign="Top" ImageUrl="~/WebResources/Images/Buttons/icon_close.png" />
                        </div>
                    </asp:Panel>
                    <%-- Fin Encabezado --%>
                    <div class="modalControls">
                        <div class="divCtrsFloatLeft">
                            <div id="div9" runat="server" class="divControls">
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
                        <div id="div10" runat="server" class="modalActions">
                            <asp:Button ID="btnCancelLoad" runat="server" Text="Cancelar" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <%-- FIN Pop up Editar/Nuevo --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSubir2" />
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
    <asp:Label ID="lblConfirmDelete" runat="server" Text="¿Desea eliminar este Cliente?" Visible="false" />
    <asp:Label ID="lbltabGeneral" runat="server" Text="Datos Generales" Visible="false" />    
    <asp:Label ID="lblTabDelivery" runat="server" Text="Entrega" Visible="false" />            
    <asp:Label ID="lblTabSales" runat="server" Text="Facturación" Visible="false" />    
    <asp:Label ID="lblTabB2B" runat="server" Text="B2B" Visible="false" />   
    <asp:Label ID="lblAddLoadToolTip" runat="server" Text="Carga Masiva" Visible="false" />
    <asp:Label ID="lblTitle" runat="server" Text="Mantenedor de Clientes" Visible="false" />
    <asp:Label ID="lblFormatFileNotValid" runat="server" Text="Formato del archivo no es válido." Visible="false" />
    <asp:Label ID="lblFieldNotNull" runat="server" Text="No puede ser nulo." Visible="false" />
    <asp:Label ID="lblMessajeLoadOK" runat="server" Text="La carga se realizo de forma existosa." Visible="false" />
    <asp:Label ID="lblNotItemsFile" runat="server" Text="No existen clientes en el archivo." Visible="false" />
    <asp:Label ID="lblFieldInvalid" runat="server" Text="Formato del campo no es válido." Visible="false" />
    <asp:Label ID="lblExcelComponentDoesntExist" runat="server" Text="No se encontro componente para leer Excel." Visible="false" />

    <asp:Label ID="lblGln" runat="server" Text="GLN" Visible="false" />
    <%--<asp:Label ID="lblGlnIsNotNumeric" runat="server" Text="Código GLN debe ser Numérico." Visible="false" />
    <asp:Label ID="lblGlnLengthInvalid" runat="server" Text="Largo del Código GLN debe ser 13." Visible="false" />
    <asp:Label ID="lblGlnCheckDigit" runat="server" Text="Dígito Verificador del Código GLN no es Válido." Visible="false" />--%>

    <asp:Label ID="lblGlnMessageRequired" runat="server" Text="GLN es requerido" Visible="false" />
    <asp:Label ID="lblFactAddress2MessageRequired" runat="server" Text="Dir. Factura Opc. es requerido" Visible="false" />
    <asp:Label ID="lblGlnMessageRegularExpression" runat="server" Text="GLN permite ingresar solo caracteres numéricos" Visible="false" />
    <asp:Label ID="lblFactAddress2MessageRegularExpression" runat="server" Text="Dir. Factura Opc. permite ingresar solo caracteres alfanuméricos" Visible="false" />

    <asp:Label ID="lblSearchWarning" runat="server" Text="Debe ingresar código o nombre en filtro de búsqueda" Visible="false" />
    <asp:Label ID="lblMaxLength" runat="server" Text="Campo @field como máximo debe tener @maxLenght caracteres en cliente @customerCode" Visible="false" />
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>
</asp:Content>
<asp:Content ID="StatusContent" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
