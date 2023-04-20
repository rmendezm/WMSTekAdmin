using Binaria.WMSTek.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binaria.WMSTek.IntegrationClient.Integration
{
    public class IntegrationConfigManager
    {
        static private IntegrationConfigManager instance = null;
        LogManager log = LogManager.getInstance();

        private IntegrationConfigManager()
        {
        }

        public static IntegrationConfigManager getInstance()
        {
            if (instance == null)
            {
                instance = new IntegrationConfigManager();
            }
            return instance;
        }

        public void Execute()
        {
            try
            {
                var instances = DBIntegrationConfig.getInstance().GetInstances();

                if (instances.Count > 0)
                {
                    DBIntegrationInstance.getInstance().LoadIds(instances);
                }
                else
                    log.warningMessage(this.GetType().FullName, "No instances found");
            }
            catch (Exception ex)
            {
                log.exceptionMessage(this.GetType().FullName, ex.Message);
            }
        }
    }
}
