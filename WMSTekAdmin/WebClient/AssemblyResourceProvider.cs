using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Hosting;
using System.IO;
using System.Reflection;

namespace Binaria.WMSTek.WebClient
{
    public class AssemblyResourceProvider : System.Web.Hosting.VirtualPathProvider  
    {
        public AssemblyResourceProvider() {}
        private bool IsAppResourcePath(string virtualPath)
        {
            String checkPath =
               VirtualPathUtility.ToAppRelative(virtualPath);
            return checkPath.StartsWith("~/App_Resource/", StringComparison.InvariantCultureIgnoreCase);
        }
        public override bool FileExists(string virtualPath)
        {
            return (IsAppResourcePath(virtualPath) || base.FileExists(virtualPath));
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            if (IsAppResourcePath(virtualPath))
                return new AssemblyResourceVirtualFile(virtualPath);                
            else
                return base.GetFile(virtualPath);
        }
        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsAppResourcePath(virtualPath))
                return null;
            else 
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }

    class AssemblyResourceVirtualFile : VirtualFile
    {
        string path;
        public AssemblyResourceVirtualFile(string virtualPath) : base(virtualPath)
        {
            path = VirtualPathUtility.ToAppRelative(virtualPath); 
        }
        public override System.IO.Stream Open()
        {
            string[] parts = path.Split('/');
            string assemblyName = parts[2]; 
            string resourceName = parts[parts.Length -1];

            assemblyName = Path.Combine(HttpRuntime.BinDirectory, assemblyName);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(assemblyName);
            if (assembly != null)
            {

                System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
                return stream;
            }
            return null;
        }    
    }
    
}
