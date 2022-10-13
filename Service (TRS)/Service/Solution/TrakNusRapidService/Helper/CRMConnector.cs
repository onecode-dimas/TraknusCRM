/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 5 May 2014
 * Description  : Class for connect to CRM
 * Notes        : Default Connection Name = "CRMCS"
 * Compatibility: CRM 2011, CRM 2013
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Crm.Sdk.Messages;

using Microsoft.Xrm.Sdk;

namespace TrakNusRapidService.Helper
{
    public class CRMConnector
    {
        #region Constants
        private const string _classname = "CRMConnector";
        #endregion

        #region Properties
        private string _connectionName;
        public string ConnectionName
        {
            get { return _connectionName; }
            set { _connectionName = value; }
        }

        private string _connectionString = null;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        private LogCreator _logger = null;
        private bool _log = false;
        public bool Log
        {
            get { return _log; }
        }
        
        private string _err_value;
        #endregion

        #region Private
        private void Initialize(bool log = false, object logger = null)
        {
            try
            {
                if (log && logger == null)
                    throw new Exception("Please defined the logger object !");
                else if (log)
                {
                    _log = log;
                    _logger = (LogCreator)logger;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Initialize : " + ex.Message);
            }
        }

        private String GetConnectionString()
        {
            try
            {
                if (_log)
                    _logger.Write("Getting Connection String...\n");
                if (ConfigurationManager.ConnectionStrings.Count > 0 && _log)
                    _logger.Write("List of Available Connection : \n");

                string connectionString = string.Empty;
                for (int i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                {
                    if (_log)
                        _logger.Write("\t* " + ConfigurationManager.ConnectionStrings[i].Name + "\n");
                    if (ConfigurationManager.ConnectionStrings[i].Name == _connectionName)
                    {
                        connectionString = ConfigurationManager.ConnectionStrings[i].ConnectionString;
                        break;
                    }
                }

                if (connectionString == string.Empty)
                {
                    _err_value = "Can not found Connection String with name : " + _connectionName + "\n";
                    if (_log)
                        _logger.Write(_err_value);
                    throw new Exception(_err_value);
                }
                else
                    if (IsValidConnectionString(connectionString))
                        return connectionString;
                    else
                    {
                        _err_value = "Invalid Connection String. Please change it to : " + ConnectionStringExample();
                        if (_log)
                            _logger.Write(_err_value);
                        throw new Exception(_err_value);
                    }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".GetConnectionString : " + ex.Message.ToString());
            }
        }

        private static Boolean IsValidConnectionString(String connectionString)
        {
            if (connectionString.Replace(" ", "").ToLower().Contains("url=") ||
                connectionString.Replace(" ", "").ToLower().Contains("server=") ||
                connectionString.Replace(" ", "").ToLower().Contains("serviceuri="))
                return true;

            return false;
        }
        #endregion

        #region Constructors
        public CRMConnector()
        {
        }

        public CRMConnector(bool log = false, object logger = null)
        {
            try
            {
                Initialize(log, logger);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CRMConnector : " + ex.Message);
            }
        }

        public CRMConnector(bool log = false, object logger = null, string connectionName = "CRMCS")
        {
            try
            {
                Initialize(log, logger);
                _connectionName = connectionName;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".CRMConnector : " + ex.Message);
            }
        }
        #endregion

        #region Connectors
        public OrganizationService Connect()
        {
            try
            {
                if (_log)
                    _logger.Write("Connection Process Start...\n");
                if (_connectionString == null)
                    _connectionString = GetConnectionString();
                    //_connectionString = "Url = http://192.168.0.168/TraktorNusantaraDev; Domain = traknus; Username = admin.crm; Password = pass@word2";
                    //else
                    //    _connectionString = "Url = http://192.168.0.70:5555/TraktorNusantara; Domain = traknus; Username = admin.crm; Password = pass@word2";
                    //_connectionString = "Url = http://116.254.101.97:5555/TraktorNusantara; Domain = TecDevCloud; Username = admincrm; Password = pass@word1";
                if (_log)
                    _logger.Write("Establish Connection...\n");
                CrmConnection crmConnection = CrmConnection.Parse(_connectionString);
                if (_log)
                    _logger.Write("Connection Process Done...\n");
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

        public string GetCrmUriString()
        {
            return "http://192.168.0.168/traktornusantaradev/main.aspx";
        }

        public string ConnectionStringExample()
        {
            return "Url=myCrmUrl;Domain=myDomain;Username=myUsername;Password=myPassword;";
        }
        #endregion
    }
}
