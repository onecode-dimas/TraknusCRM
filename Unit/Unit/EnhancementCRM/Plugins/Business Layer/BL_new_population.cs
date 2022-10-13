using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using EnhancementCRM.HelperUnit.ZCRM_UPDATE_WARRANTY_DATE;
using System.ServiceModel;
using System.Reflection;
using EnhancementCRM.HelperUnit;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_new_population
    {
        #region Properties
        private string _classname = "BL_new_population";
        private string _entityname_population = "new_population";
        private const string ConfigurationName = "TRS";
        private const string ConfigurationEntityName = "trs_workflowconfiguration";
        private const string ConfigurationEntityPrimaryFieldName = "trs_generalconfig";
        private const string ConfigurationWebServiceEntityName = "ittn_webservicesconfiguration";
        private const int WebService_UpdateWOWarranty = 841150002;
        #endregion

        #region Depedencies
        Generator _Generator = new Generator();
        MWSLog _mwsLog = new MWSLog();
        #endregion

        #region Publics
        public void PostUpdate_Population_BASTSignDate(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Entity population = _organizationservice.Retrieve(_entityname_population, _entity.Id, new ColumnSet(true));

                if (population.Contains("trs_equipmentnumber") && population.Attributes["trs_equipmentnumber"] != null
                    && population.Contains("trs_bastsigndate") && population.Contains("trs_warrantyenddate"))
                {
                    #region Declare Parameters
                    String Token, EquipmentNo, BASTSignDate, WarrantyEndDate;
                    #endregion

                    EquipmentNo = population.GetAttributeValue<String>("trs_equipmentnumber");
                    BASTSignDate = population.GetAttributeValue<DateTime>("trs_bastsigndate").ToLocalTime().Date.ToString("yyyy-MM-dd");
                    WarrantyEndDate = population.GetAttributeValue<DateTime>("trs_warrantyenddate").ToLocalTime().Date.ToString("yyyy-MM-dd");

                    ZCRM_UPDATE_WARRANTY_DATE UpdatePopulation = new ZCRM_UPDATE_WARRANTY_DATE();
                    string WebServiceURL, UniqueKey, SAPUsername, SAPPassword;

                    #region Process
                    #region Get SAP WebService Configuration
                    _tracer.Trace("Start Getting SAP WebService Configuration");

                    QueryExpression queryConfiguration = new QueryExpression(ConfigurationEntityName);
                    queryConfiguration.ColumnSet = new ColumnSet(true);
                    queryConfiguration.Criteria.AddCondition(ConfigurationEntityPrimaryFieldName, ConditionOperator.Equal, ConfigurationName);
                    EntityCollection ECSAPConfiguration = _organizationservice.RetrieveMultiple(queryConfiguration);

                    if (ECSAPConfiguration.Entities.Count > 0)
                    {
                        Entity SAPConfiguration = ECSAPConfiguration.Entities[0];

                        QueryExpression WSCPO = new QueryExpression(ConfigurationWebServiceEntityName);
                        WSCPO.ColumnSet = new ColumnSet(true);
                        WSCPO.Criteria.AddCondition("ittn_workflowconfiguration", ConditionOperator.Equal, SAPConfiguration.Id);
                        WSCPO.Criteria.AddCondition("ittn_webservicefor", ConditionOperator.Equal, WebService_UpdateWOWarranty);
                        EntityCollection ECWS = _organizationservice.RetrieveMultiple(WSCPO);

                        if (ECWS.Entities.Count > 0)
                        {
                            Entity WSCPOS = ECWS.Entities[0];

                            WebServiceURL = WSCPOS.GetAttributeValue<string>("ittn_sapwebservice");
                            UniqueKey = WSCPOS.GetAttributeValue<string>("ittn_sapintegrationuniquekey");
                            SAPUsername = WSCPOS.GetAttributeValue<string>("ittn_sapwebserviceusername");
                            SAPPassword = WSCPOS.GetAttributeValue<string>("ittn_sapwebservicepassword");
                        }
                        else
                            throw new InvalidPluginExecutionException("Web Service for Update Work Order Warranty is null/empty!");
                    }
                    else
                        throw new InvalidPluginExecutionException("Cannot fount Workflow Configuration with name " + ConfigurationName + " !");
                    #endregion

                    #region Generate Token
                    _tracer.Trace("Start Generate Token");
                    Token = _Generator.Encrypt(EquipmentNo.Trim(), UniqueKey);
                    #endregion

                    #region Set Web Service Parameter Data
                    UpdatePopulation.CSRF_TOKEN = Token;
                    UpdatePopulation.I_EQUNR = EquipmentNo;
                    UpdatePopulation.I_GWLDT = BASTSignDate;
                    UpdatePopulation.I_GWLEN = WarrantyEndDate;
                    #endregion

                    #region Open Connection to SAP WebService
                    _tracer.Trace("Getting WebService Client");

                    EndpointAddress remoteAddress = new EndpointAddress(WebServiceURL);
                    BasicHttpBinding httpbinding = new BasicHttpBinding();
                    httpbinding.Name = "ZWEB_SERVICE_CRM";
                    httpbinding.MessageEncoding = WSMessageEncoding.Mtom;
                    httpbinding.TextEncoding = Encoding.UTF8;
                    httpbinding.SendTimeout = new TimeSpan(0, 10, 0);

                    _tracer.Trace("Creating Services Client");
                    ZWS_WARRANTY_DATE_V3Client client = new ZWS_WARRANTY_DATE_V3Client(httpbinding, remoteAddress);
                    client.ClientCredentials.UserName.UserName = SAPUsername;
                    client.ClientCredentials.UserName.Password = SAPPassword;

                    try
                    {
                        _tracer.Trace("Open Client WebService");

                        client.Open();

                        ZCRM_UPDATE_WARRANTY_DATEResponse response = client.ZCRM_UPDATE_WARRANTY_DATE(UpdatePopulation);

                        if (response != null)
                        {
                            DateTime SyncDate = DateTime.Parse(response.SYNC_DATE);
                            DateTime SyncTime = response.SYNC_TIME;
                            SyncDate = SyncDate.AddHours(SyncDate.Hour);
                            SyncDate = SyncDate.AddMinutes(SyncDate.Minute);
                            SyncDate = SyncDate.AddSeconds(SyncDate.Second);

                            if (response.RESULT != null)
                            {
                                String Result = response.RESULT;

                                if (string.Equals(Result, "success"))
                                {
                                    #region Success
                                    _mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Success to Update Equipment Warranty on SAP. CRM Equipment Number : "
                                        + EquipmentNo + ", BAST Sign Date : " + BASTSignDate + ", Warranty End Date : " + WarrantyEndDate + ", result : " + response.RESULT
                                        + _mwsLog.ColumnsSeparator, MWSLog.LogType.Information, MWSLog.Source.Outbound);
                                    #endregion
                                }
                                else
                                {
                                    throw new InvalidPluginExecutionException(string.Format("Result : {0}, Description : {1}", response.RESULT, response.DESCRIPTION));
                                }
                            }
                        }
                        else
                        {
                            _tracer.Trace("WebService Response is null/ empty!");
                            throw new InvalidPluginExecutionException("WebService Response is null/ empty!");
                        }
                    }
                    catch (Exception ex)
                    {
                        _mwsLog.Write(MethodBase.GetCurrentMethod().Name, "Failed to Update Equipment on SAP. CRM Equipment Number : "
                            + EquipmentNo + ", Details : " + ex.Message + _mwsLog.ColumnsSeparator, MWSLog.LogType.Error, MWSLog.Source.Outbound);

                        _tracer.Trace("Failed Time: " + DateTime.Now.ToString());

                        client.Abort();
                        client.Close();

                        throw new InvalidPluginExecutionException("Error Detail : " + ex.Message.ToString());
                    }
                    finally
                    {
                        client.Close();
                    }
                    #endregion
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostUpdate_Population_BASTSignDate: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
