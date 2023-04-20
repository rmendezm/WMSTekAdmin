using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binaria.WMSTek.WebClient.Service
{
    public sealed class Navigation
    {
        public static void StartApp()
        { 
            /*
            // verificar si existe sesion de usuario activa

            if (sesion[usuario])
                LoadDefault();
            else
                LoadLogin(true);  // true --> indica que no muestra mensaje de error de autenticación
             */ 
        }

        public static void LoadPage(string webPage)
        { 
            // TODO: Redirect a webPage
        }

        public static void LoadLogin(bool isValid)
        {
            // TODO: Redirect a LoginPage(isValid)
        }

        public static void LoadDefault()
        {
            
            // TODO: Redirect a página inicial del usuario
        }
    }
}
