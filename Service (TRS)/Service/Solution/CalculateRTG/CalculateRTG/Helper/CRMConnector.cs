using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Crm.Sdk.Messages;


namespace CalculateRTG.Helper
{
    public class CRMConnector
    {
        #region Constants
        private const string _classname = "CRMConnector";
        #endregion

        #region Properties
        private string _connectionName = "CRMCS";
        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; }
        }

        private string _connectionString = null;
        public string ConnectionString
        {
            get { return _connectionString; }
        }
        #endregion

        #region Private
        private static Boolean IsValidConnectionString(String connectionString)
        {
            if (connectionString.ToLower().Contains("url=") ||
                connectionString.ToLower().Contains("server=") ||
                connectionString.ToLower().Contains("serviceuri="))
                return true;

            return false;
        }
        #endregion

        #region Connectors
        public String GetConnectionString()
        {
            try
            {
                string connectionString = string.Empty;

                for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                {
                    if (ConfigurationManager.ConnectionStrings[i].Name == _connectionName)
                    {
                        connectionString = ConfigurationManager.ConnectionStrings[i].ConnectionString;
                        break;
                    }
                }

                if (connectionString == string.Empty)
                    throw new Exception("Can not found Connection String.");
                else
                    if (IsValidConnectionString(connectionString))
                        return connectionString;
                    else
                        throw new Exception("Invalid Connection String. Please change it to : " + ConnectionStringExample());
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetConnectionString : " + ex.Message.ToString());
            }
        }

        public OrganizationService Connect()
        {

            _connectionString = "Url = http://192.168.0.70:5555/TraktorNusantara; Domain = traknus; Username = admin.crm; Password = pass@word2";
            CrmConnection crmConnection = CrmConnection.Parse(_connectionString);
            try
            {
                return new OrganizationService(crmConnection);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Connect : " + ex.Message.ToString());
            }
        }

        public Guid GetLoggedUserId(OrganizationService organizationService)
        {
            return ((WhoAmIResponse)organizationService.Execute(new WhoAmIRequest())).UserId;
        }

        public string ConnectionStringExample()
        {
            return "Url=myCrmUrl;Domain=myDomain;Username=myUsername;Password=myPassword;";
        }
        #endregion
    }
}
