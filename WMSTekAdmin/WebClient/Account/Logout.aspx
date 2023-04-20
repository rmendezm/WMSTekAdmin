<%@ Page Language="C#"  MasterPageFile="~/Shared/WMSTekContent.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Account.Logout" %>
<%@ MasterType VirtualPath="~/Shared/WMSTekContent.Master" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server"  >
 <script type="text/javascript">
     window.onresize = resizeDivImgBack;

     function resizeDivImgBack() {
         var h = document.body.clientHeight + "px";
         var w = document.body.clientWidth + "px"

         if (document.getElementById("ctl00_MainContent_divImgBack") != null) {
             document.getElementById("ctl00_MainContent_divImgBack").style.height = h;
             document.getElementById("ctl00_MainContent_divImgBack").style.width = w;
         }
     }   

     $(function () {
        
     });
</script>  

    <style>
        #sidedrawer, #header{
            display: none;
        }
    </style>


<div id="divImgBack" runat="server" style="/*background-image:url(../WebResources/Images/Login/bg_body.jpg) ;*/  
    background-color: #F3FBFE;
    background-position: center center;
    background-repeat: no-repeat;
    background-attachment: fixed;  
    background-size: cover; 
    -moz-background-size: cover; 
    -webkit-background-size: cover; 
    -o-background-size: cover;">
    
    <%-- Mensajes de Confirmacion y Auxiliares --%>    
    <asp:Label ID="lblTitle" runat="server" Text="WMSTek" Visible="false"/>
    <asp:Label ID="lblLogout"  runat="server" Text="Su sesión ha expirado. " Visible="false"/><br />
    <asp:Label ID="lblLogout2" runat="server" Text="Cierre sesión para ingresar nuevamente. " Visible="false"/>
    <%-- FIN Mensajes de Confirmacion y Auxiliares --%>  
</div>		
</asp:Content>

