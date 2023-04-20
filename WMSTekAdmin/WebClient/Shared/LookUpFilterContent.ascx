<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LookUpFilterContent.ascx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.LookUpFilterContent" %>

    <div class="mgrFilterPanelLookUp">
        <div class="mgrFilterPanelFloat">
            <asp:RadioButtonList ID="rblSearchCriteria" runat="server">
                <asp:ListItem Selected="True"></asp:ListItem>
                <asp:ListItem></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="mgrFilterPanelFloat">
            <asp:TextBox ID="txtSearchValue" runat="server" Width="120px"></asp:TextBox>
            <asp:ImageButton ID="btnSearch" ToolTip="Buscar" runat="server" onclick="btnSearch_Click" ImageUrl="~/WebResources/Images/Buttons/TaskBar/icon_search.png" />	        
        </div>            
	 </div>


