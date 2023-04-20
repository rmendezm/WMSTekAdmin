<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenuContent.ascx.cs" Inherits="Binaria.WMSTek.WebClient.Shared.MainMenuContent" %>

    <%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

    <%--No borrar el z-index de este div ya que permite que el menu siempre se vea visible por sobre las ventanas--%>
    <div id="menu" style="z-index: 10000">

           <asp:Menu ID="mnuMain" runat="server" 
            DynamicHorizontalOffset="0" 
            DynamicVerticalOffset="3" 
            StaticSubMenuIndent="2px" 
            Orientation="Horizontal" 
            Target="_blank"
           StaticPopOutImageUrl="~/WebResources/Images/Menu/ArrowDown.gif" CssClass="MenuBarHorizontal">
          <DataBindings>
            <asp:MenuItemBinding DataMember="MenuItem" TextField="Text" TargetField="Target" NavigateUrlField="NavigateUrl" SelectableField="Selectable"/>
          </DataBindings>

           <DynamicItemTemplate>
               <%# Eval("Text") %>
           </DynamicItemTemplate>
          
          <%--Encabezado --%>
          <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" CssClass="MenuBarHeader"/>           
          <StaticHoverStyle CssClass="MenuBarHover"/>
             
          <%--Items --%>
          <DynamicMenuStyle CssClass="MenuBarDynamic"/>
          <DynamicHoverStyle CssClass="MenuBarHover"/>          
          <DynamicMenuItemStyle HorizontalPadding="2px" VerticalPadding="2px" CssClass="MenuBarItem"/>
       </asp:Menu>

       <asp:XmlDataSource ID="xmlMenu" TransformFile="~/WebResources/XSLT/xsltMenu.xsl" 
            XPath="MenuItems/MenuItem" runat="server" EnableCaching="False" 
            EnableViewState="False"/> 
    </div>

