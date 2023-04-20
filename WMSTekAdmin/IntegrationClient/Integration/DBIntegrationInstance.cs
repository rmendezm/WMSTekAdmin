using Binaria.WMSTek.DataAccess.Entities;
using Binaria.WMSTek.DataAccess.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binaria.WMSTek.IntegrationClient.Integration
{
    public class DBIntegrationInstance
    {
        static private DBIntegrationInstance instance = null;
        static Dictionary<string, int> sequenceTables;
        private DBIntegrationInstance()
        {
        }

        public static DBIntegrationInstance getInstance()
        {
            if (instance == null)
            {
                instance = new DBIntegrationInstance();
            }
            return instance;
        }

        public void LoadIds(List<Instance> instances)
        {
            var DBManager = new DBManager();
            var con = DBConfiguration.getInstance();
            var integrationPool = con.GetCurrentPool();
            sequenceTables = new Dictionary<string, int>();

            if (integrationPool == null)
                throw new Exception("Integration pool not found in db config");

            var configQuery = DBManager.GetQuery(integrationPool.Code, "GetSecuencialByTableName");
            if (configQuery == null)
                throw new Exception("Query queryCode GetSecuencialByTableName object not found in db config");

            foreach (var instance in instances)
            {
                var sql = configQuery.SqlQuery;

                sql = sql.Replace("ColumnName", instance.ColumnName);
                sql = sql.Replace("TableName", instance.TableName);

                var cnx = new DBConnection("WMSTek_INTERFAZ");
                var callQuery = cnx.createSqlStmt(sql, configQuery.QueryParamters, configQuery.QueryParamters, integrationPool.TimeOut, configQuery);

                var set = callQuery.ExecuteQuery(configQuery.Code);

                if (set.Reader.Read())
                {
                    var sequence = set.getInt(instance.ColumnName, -1);

                    if (sequence != -1)
                    {
                        sequence++;
                        sequenceTables.Add(instance.TableName, sequence);
                    }
                }
                cnx.releaseDBConnection();
            }
        }

        public int GetIdEntityByClassName(string className)
        {
            var con = DBConfiguration.getInstance();
            var integrationPool = con.GetCurrentPool();
            int sequence = 0;

            if (integrationPool == null)
                throw new Exception("Integration pool not found in db config");

            var instances = DBIntegrationConfig.getInstance().GetInstances();
            var instanceByClassName = instances.Where(i => i.ClassName.Equals(className)).FirstOrDefault();

            if (instanceByClassName != null)
                sequence = GetIdEntityByTableName(instanceByClassName.TableName);

            return sequence;
        }
        private int GetIdEntityByTableName(string tableName)
        {
            int sequence = 0;

            lock (sequenceTables)
            {
                var sequenceByTable = sequenceTables.Where(st => st.Key.Equals(tableName)).FirstOrDefault();
                if (sequenceByTable.Key != null)
                {
                    var lastValue = sequenceByTable.Value;
                    sequence = lastValue;
                    lastValue++;
                    sequenceTables[tableName] = lastValue;
                }
            }

            return sequence;
        }
    }
}
