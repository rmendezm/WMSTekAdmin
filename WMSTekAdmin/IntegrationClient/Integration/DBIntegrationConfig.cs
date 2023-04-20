using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;
using System.Xml;
using Binaria.WMSTek.DataAccess.Entities;
using System.Xml.Linq;

namespace Binaria.WMSTek.IntegrationClient.Integration
{
    public class DBIntegrationConfig
    {
        static private DBIntegrationConfig instance = null;
        static List<Instance> instances;
        LogManager log = LogManager.getInstance();
        const string instancesFile = "interfaz-instances.config";
        private DBIntegrationConfig()
        {
            GetInstances();
        }

        public static DBIntegrationConfig getInstance()
        {
            if (instance == null)
            {
                instance = new DBIntegrationConfig();
            }
            return instance;
        }

        public List<Instance> GetInstances()
        {
            try
            {
                if (instances == null)
                {
                    var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");
                    var file = Path.Combine(folder, instancesFile);
                    if (File.Exists(file))
                        instances = ReadXml(file);
                }

                return instances;
            }
            catch (Exception ex)
            {
                log.exceptionMessage(this.GetType().FullName, ex.Message);
                throw;
            }
        }

        private List<Instance> ReadXml(string configFile)
        {
            var doc = XDocument.Load(configFile);

            var instances = doc.Descendants("instances").Elements().Select(d =>
                             new Instance()
                             {
                                  TableName = d.Element("table_name").Value,
                                  ColumnName = d.Element("column_name").Value,
                                  ClassName = d.Element("class_name").Value
                             }).ToList();

            return instances;
        }
    }
}
