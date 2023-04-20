<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" 
    CodeBehind="LabelPackingListB2BReprint.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Administration.Devices.LabelPackingListB2BReprint" %>

<%@ MasterType virtualpath="~/Shared/WMSTekContent.Master" %>
<%@ Register TagPrefix="webUc" TagName="ucStatus" Src="~/Shared/StatusBarContent.ascx" %>
<%@ Register TagPrefix="spl" Namespace="OboutInc.Splitter2" Assembly="obout_Splitter2_Net" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            initializeGridDragAndDrop("LabelPackingListB2BReprint_FindAll", "ctl00_MainContent_grdMgr");
            //initializeGridWithNoDragAndDrop();

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
            initializeGridDragAndDrop("LabelPackingListB2BReprint_FindAll", "ctl00_MainContent_grdMgr");
            //initializeGridWithNoDragAndDrop();
        }
    </script>

    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div id="divGrid" runat="server" visible="true" class="divGrid" > 
                            <asp:GridView ID="grdMgr" 
                                    DataKeyNames="Id" 
                                    runat="server" 
                                    OnRowCreated="grdMgr_RowCreated"
                                    AllowPaging="True" 
                                    OnRowDataBound="grdMgr_RowDataBound"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-bordered table-hover table-curved table-dragAndDrop table-condensed" 
                                    EnableTheming="false">
                                <Columns>

                                    <asp:templatefield HeaderText="Seleccionar" AccessibleHeaderText="chkSelectLabel">
	                                    <HeaderTemplate>
		                                    <input type="checkbox" onclick="toggleCheckBoxes('<%= grdMgr.ClientID %>', 'chkSelectLabel', this.checked)" id="chkAll" title="Seleccionar todos" />
	                                    </HeaderTemplate>
	                                    <itemtemplate>
	                                        <asp:CheckBox ID="chkSelectLabel" runat="server"/>
	                                    </itemtemplate>
                                    </asp:templatefield>

                                    <asp:TemplateField HeaderText="ID" AccessibleHeaderText="ID" Visible="false">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblIdTaskLabel" runat="server" Text='<%# Eval ( "Id" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Código Etiqueta" AccessibleHeaderText="Name">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLabelCode" runat="server" Text='<%# Eval ( "LabelTemplate.Code" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Fecha Creación" AccessibleHeaderText="DateCreated">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblDateCreated" runat="server" Text='<%# Eval ( "DateCreated", "{0:d}"  ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Nombre Cliente" AccessibleHeaderText="CustomerName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval ( "Customer.Name" ) %>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Tipo Etiqueta" AccessibleHeaderText="LabelTemplateName">
                                        <ItemTemplate>
                                            <div style="word-wrap: break-word;">
                                                <asp:Label ID="lblLabelTemplateName" runat="server" Text='<%# Eval ( "LabelTemplate.Name" ) %>'></asp:Label>
                                            </div>
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
                         <asp:AsyncPostBackTrigger ControlID="ctl00$ucTaskBarContent$btnPrint" EventName="Click" />
                  </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <asp:UpdateProgress ID="uprGrid" runat="server" AssociatedUpdatePanelID="upGrid" DisplayAfter="20" DynamicLayout="true">
        <ProgressTemplate>
            <div class="divProgress">
                <asp:ImageButton ID="imgProgress" runat="server" ImageUrl="~/WebResources/Images/Buttons/icon_progress_old.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <webUc:UpdateProgressOverlayExtender ID="muprGrid" runat="server" ControlToOverlayID="divTop" CssClass="updateProgress" TargetControlID="uprGrid" />


    <asp:Label ID="lblDocName" runat="server" Text="N° Doc" Visible="false" />
    <asp:Label ID="lblFilterDate" runat="server" Text="Fecha" Visible="false" />
    <asp:Label ID="lblFilterCustomer" runat="server" Text="Nombre Cliente" Visible="false" />
    <asp:Label ID="lblFilterLpn" runat="server" Text="LPN" Visible="false" />
    <asp:Label ID="lblFilterItem" runat="server" Text="Código Item" Visible="false" />
    <asp:Label ID="lblValidateExistsFilterLpn" runat="server" Text="Debe ingresar el Código LPN" Visible="false" />
    <asp:Label ID="lblFilterLabelCode" runat="server" Text="Código Etiqueta" Visible="false" />
    <asp:Label id="lblNoSelected" runat="server" Text="Debe seleccionar al menos un registro" Visible="false" />  
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="StatusContent" runat="server">
    <%-- Barra de Estado --%>
    <webUc:ucStatus ID="ucStatus" runat="server" />
</asp:Content>
