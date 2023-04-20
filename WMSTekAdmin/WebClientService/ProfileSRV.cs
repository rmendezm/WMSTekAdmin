using System.Collections.Generic;
using Binaria.WMSTek.AdminApp.Manager;
using Binaria.WMSTek.Framework.Utils.Enums;
using Binaria.WMSTek.Framework.Utils;

namespace Binaria.WMSTek.WebClient.Service
{
    public class ProfileSRV
    {
        /*
        //private ProfileMGR profileMGR = new ProfileMGR();
        private IProfileMGR iProfileMGR = (IProfileMGR)InstanceFactory.getInstance().getObject("profileMGR");

        public UserViewDTO LoadUser(string login, string password)
        {
            return iProfileMGR.LoadUser(login, password, contextDTO);
        }

        public UserViewDTO LoadAllUser()
        {
            return iProfileMGR.LoadAllUser();
        }

        public UserViewDTO MaintainUser(CRUD action, int id, UserViewDTO userViewDTO)
        {
            return iProfileMGR.MaintainUser(action, id, userViewDTO, contextDTO);
        }

        */
        /*
        public static bool AuthenticateUser(string login, string password)
        {
            //bool isValid = false;

            // TODO: hacer llamada x Remoting  -- quitar referencia a dll AdminApp 
            
                    String filename = "WebClientService.config";
                    RemotingConfiguration.Configure(filename);
                    //HttpChannel channel = new HttpChannel();
                    //ChannelServices.RegisterChannel(channel);

                    //IProfileSRV remoteProfileAPP = (IProfileSRV) Activator.GetObject(typeof(IProfileSRV), "http://localhost:1234/ProfileSRV.soap");

                    //isValid = remoteProfileAPP.AuthenticateUser(userName, password);

            
            if (isValid)
            {
                // carga instancia del Usuario (con sus Datos Y Roles?)
                //ObjUser objUser;  // De tipo [MarshallByValue]
                IDTO objUser;

                //objUser = ServicioRemoto.Profile.LoadUser(userName, password);  // Load user es una llamada [MarshalByRefObject]

                objUser = ProfileMGR.LoadUser(login, password);

                //Session["ActivoUser"] = "1";
                
                
                // TODO: cargar objUser en SESSION / o en CACHE ?

                //LoadRol(objUser.idUser);

                Navigation.LoadDefault();
            }

            else
                Navigation.LoadLogin(false);
        }

        public static IDTO LoadUser(string login, string password)
        {
            return ProfileMGR.LoadUser(login, password);
        }

        public static void LoadRol(int idUser)
        {
            List<ObjRol> lstRol;
            XML objMenu;  // TODO: objMenu es XML o definir un tipo propio?

            lstRol = ServicioRemoto.Profile.LoadRol(idUser); // TODO: Rol es un arreglo de roles / List<> ?
                                                             // se carga el o los Roles con su Arbol de Permisos asociado

            // TODO: cargar en CACHE los Arbol de Permisos de los Roles asociados al Usuario
            //       - verificar si ya existen en cache
            //       -  si ya existen en cache --> no hacer nada
            //       -  si no existen en cache --> cargar en cache
            //       - NOTA: el Arbol de Permisos será usado para filtrar y armar las páginas de usuario, no el menú


            objMenu = ServicioRemoto.Profile.LoadMenu(idUser);

            // TODO: cargar menú en control de usuario MenuControl / o en MasterPage
            //       - objMenu retorna en formato XML
            //       - el XML es transformado con un XSLT y cargado en un control de tipo Menu
        }
         */ 
    }
}
