<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatusBarContent.ascx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.StatusBarContent" %>

<div class="statusBarPanel">
    <div id="divStatus" class="statusBarPanelLeft">
        <asp:Label ID="lblStatusBar" runat="server" Visible="true"></asp:Label>
    </div>
    <div class="statusBarRecordInfo">
        <%-- TODO: Implementar en Fase 3
            <div id="divPageSize" class="statusBarPager" runat="server">    
            Items por Pág. 
            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSizeSelectedIndexChanged" SkinID="ddlFilter" Width="45px">
                <asp:ListItem Text="10" Value="10" />
                <asp:ListItem Text="20" Value="20" />
                <asp:ListItem Text="50" Value="50" />
                <asp:ListItem Text="100" Value="100" />
                <asp:ListItem Text="Todos" Value="0" />
            </asp:DropDownList> 
        </div>
        --%>
        <div id="divPager" class="statusBarPager" runat="server" visible="false">
            <asp:ImageButton ID="btnFirst" runat="server" ToolTip="Primero" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_first.png" 
                onmouseover="this.src='../../WebResources/Images/Buttons/Pager/icon_first_on.png';"
                onmouseout="this.src='../../WebResources/Images/Buttons/Pager/icon_first.png';"
                onclick="btnFirst_Click"/>
            
            <asp:ImageButton ID="btnPrevious" runat="server" ToolTip="Anterior" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_previous.png" onclick="btnPrevious_Click"
                 onmouseover="this.src='../../WebResources/Images/Buttons/Pager/icon_previous_on.png';"
                onmouseout="this.src='../../WebResources/Images/Buttons/Pager/icon_previous.png';"   />   
                         
            Pág. 
            <asp:DropDownList ID="ddlPages" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPagesSelectedIndexChanged" SkinID="ddlFilter" /> 
            de 
            <asp:Label ID="lblPageCount" runat="server" Text="" />
            
            <asp:ImageButton ID="btnNext" runat="server" ToolTip="Siguiente" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_next.png" onclick="btnNext_Click"
                onmouseover="this.src='../../WebResources/Images/Buttons/Pager/icon_next_on.png';"
                onmouseout="this.src='../../WebResources/Images/Buttons/Pager/icon_next.png';"   />   
            
            <asp:ImageButton ID="btnLast" runat="server" ToolTip="último" ImageUrl="~/WebResources/Images/Buttons/Pager/icon_last.png" 
                onmouseover="this.src='../../WebResources/Images/Buttons/Pager/icon_last_on.png';"
                onmouseout="this.src='../../WebResources/Images/Buttons/Pager/icon_last.png';"            
                onclick="btnLast_Click"  />
                
            
                
        </div>
        <div id="divRecordCount" class="statusBarPanelLeft">
            <asp:Label ID="lblRecordCount" runat="server" Text="" Visible="true" />
            <asp:Label ID="lblTotalRecordCount" runat="server" Text="" Visible="true" Font-Bold="true"/>
        </div>
    </div>        
</div>
<asp:Label ID="lblItem" runat="server" Text="Elementos " Visible="false" />
<asp:Label ID="lblDe" runat="server" Text=" de " Visible="false" />
