<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Binaria.WMSTek.WebClient.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
    
    <title>WMSTek</title>

    <link href="~/WebResources/Images/Login/favicon-wk.png" rel="shortcut icon"/>  
    <link href="~/WebResources/Styles/bootstrap.3.3.7.min.css" rel="stylesheet" />
    <link href="~/WebResources/Styles/signin.css" rel="stylesheet" />
    <script src="WebResources/Script/jquery-3.0.0.min.js"> </script>
    <script src="WebResources/Script/jquery-migrate-3.0.0.min.js"> </script>

    <style>
        .footer {
            font-size: 12px;
        }
    </style>

    <script  type="text/javascript">
        jQuery(document).ready(function () {
            $.backstretch([
                "WebResources/Images/Login/1.jpg"
                , "WebResources/Images/Login/3.jpg"
            ], { duration: 8000, fade: 2000 });

            $('.login-form input[type="text"], .login-form input[type="password"], .login-form textarea').on('focus', function () {
                $(this).removeClass('input-error');
            });
        });

        var pathname = window.location.pathname;
        var serverDomain = '';

        if (getAllIndexes(pathname, '/').length > 1) {
            serverDomain = window.location.pathname.split('/')[1];
        }

        localStorage.removeItem("serverDomain");
        localStorage.setItem("serverDomain", serverDomain);

        function getAllIndexes(arr, val) {
            var indexes = [], i = -1;
            while ((i = arr.indexOf(val, i + 1)) != -1) {
                indexes.push(i);
            }
            return indexes;
        }
    </script>

</head>
<body >
    <form runat="server" >
        <asp:ScriptManager ID="smDefault" runat="server" EnablePartialRendering="true" EnableScriptGlobalization="True" />  

        <script src="WebResources/Script/bootstrap.min.js"></script>
        <script src="WebResources/Script/jquery.backstretch.min.js"></script>

        <div class="text-center div-signin">
            <br />
            <div class="form-group" style="height:70px">
                <img class="img-responsive center-block" src="WebResources/Images/Login/Logo_WMSTEK.png"/>   
                <%--<img style="position: absolute; margin: 10px 0px 0px 30px;" class="img-responsive center-block" src="WebResources/Images/Login/LogoBlancoMin.png"/>   --%> 
                <div class="h-30"></div>
            </div>  

            <div runat="server" id="LoginPrincipal" class="form-group">                          
               <%-- <h1 class="h3 mb-3 font-weight-normal">Login</h1>--%>

                <div class="form-group" style="text-align:left;">
                    <label for="inputUser"></label>
                    <input runat="server" type="text" id="inputUser" class="form-control" placeholder="Usuario" title="Ingrese Usuario" required autofocus />
                </div>
                <div class="form-group"  style="text-align:left;">
                    <label for="inputPassword" ></label>
                    <input runat="server" type="password" id="inputPassword" class="form-control" placeholder="Contraseña" title="Ingrese Contraseña" required />
                </div>

                <div class="form-group">  
                    <asp:Button CssClass="btn btn-md btn-primary btn-block" ID="btnAuthenticateUser" runat="server" Text="Ingresar" OnClick="btnAuthenticateUser_Click" />
                </div> 

            </div>

            <div id="divMessageError" runat="server" visible="false" class="alert alert-danger">
                <strong>Error!</strong>
                <asp:Label ID="lblErroLogin" runat="server" />
            </div>

            <asp:Button CssClass="btn btn-lg btn-primary btn-block" ID="btnReload" runat="server" Text="Recargar" Visible="false"  OnClick="btnReload_Click" />
            <br />
            <div class="footer">            
                <p class="mt-5 mb-xl-5 text-muted">
                    &copy; <%=DateTime.Now.Year%> Todos los Derechos Reservados. STG Chile
                    <br/>
                    Av. Américo Vespucio Sur 991. Las Condes - Santiago.
                    <br/>
                    Teléfonos: (+56 2) 2 392 5000 - (+56 2) 2 392 5018
                    <br /><br />
                    <samp> <strong> Versión WEB: <asp:Label runat="server" ID="lblVersion" Text="" /> Versión RF: <asp:Label runat="server" ID="lblVersionRF" Text="" /></strong></samp>
                </p>
            </div>
        </div>

    </form>
</body>
</html>