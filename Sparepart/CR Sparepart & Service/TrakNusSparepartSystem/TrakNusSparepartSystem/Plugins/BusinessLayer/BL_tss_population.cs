using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using TrakNusSparepartSystem.DataLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_population
    {
        #region Constants
        private const string _classname = "BL_tss_population";

        #endregion

        #region Depedencies
        private DL_tss_kauio _DL_tss_kauio = new DL_tss_kauio();
        #endregion

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            { 
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if(entity.GetAttributeValue<int>("tss_estworkinghour") > 24)
                {
                    throw new Exception("Estimated Working Hour exceeds 24 hours, please recheck your input. ");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Unexpected error on .Form_OnCreate_PostOperation : " + ex.Message);
            }
        }

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    string NPEquipmentNumber = entity.GetAttributeValue<String>("trs_npequipmentnumber");
                    string NPEngineNumber = entity.GetAttributeValue<String>("trs_npenginenumber");
                    string NPChasisNumber = entity.GetAttributeValue<String>("trs_npchasisnumber");
                    string NPModel = entity.GetAttributeValue<String>("trs_npmodel");
                    string NPProductName = entity.GetAttributeValue<String>("trs_npproductname");
                    string NPSerialNumber = entity.GetAttributeValue<String>("trs_npserialnumber");
                    string _serialnumber = entity.Attributes.Contains("new_serialnumber") ? entity.GetAttributeValue<string>("new_serialnumber") : string.Empty;

                    if (!string.IsNullOrEmpty(NPEquipmentNumber) && !string.IsNullOrEmpty(NPEngineNumber) && !string.IsNullOrEmpty(NPChasisNumber) && !string.IsNullOrEmpty(NPModel) && !string.IsNullOrEmpty(NPProductName) && !string.IsNullOrEmpty(NPSerialNumber))
                    {
                        //Set to Non-UIO
                        Entity Population = new Entity("new_population");
                        Population.Id = entity.Id;
                        Population["tss_populationstatus"] = false;
                        organizationService.Update(Population);
                    }
                    else
                    {
                        //Set to UIO
                        Entity Population = new Entity("new_population");
                        Population.Id = entity.Id;
                        Population["tss_populationstatus"] = true;
                        organizationService.Update(Population);

                        //updateKAUIODeliveryDate(organizationService, Population);
                    }

                    if (!string.IsNullOrEmpty(_serialnumber))
                        updateKAUIODeliveryDate(organizationService, entity);
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Prospect Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (entity.GetAttributeValue<int>("tss_estworkinghour") > 24)
                {
                    throw new Exception("Estimated Working Hour exceeds 24 hours, please recheck your input. ");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error on .Form_OnUpdate_PostOperation : " + ex.Message);
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    string NPEquipmentNumber = entity.GetAttributeValue<String>("trs_npequipmentnumber");
                    string NPEngineNumber = entity.GetAttributeValue<String>("trs_npenginenumber");
                    string NPChasisNumber = entity.GetAttributeValue<String>("trs_npchasisnumber");
                    string NPModel = entity.GetAttributeValue<String>("trs_npmodel");
                    string NPProductName = entity.GetAttributeValue<String>("trs_npproductname");
                    string NPSerialNumber = entity.GetAttributeValue<String>("trs_npserialnumber");
                    string _serialnumber = entity.Attributes.Contains("new_serialnumber") ? entity.GetAttributeValue<string>("new_serialnumber") : string.Empty;

                    if (!string.IsNullOrEmpty(NPEquipmentNumber) && !string.IsNullOrEmpty(NPEngineNumber) && !string.IsNullOrEmpty(NPChasisNumber) && !string.IsNullOrEmpty(NPModel) && !string.IsNullOrEmpty(NPProductName) && !string.IsNullOrEmpty(NPSerialNumber))
                    {
                        //Set to Non-UIO
                        Entity Population = new Entity("new_population");
                        Population.Id = entity.Id;
                        Population["tss_populationstatus"] = false;
                        organizationService.Update(Population);
                    }else
                    {
                        //Set to UIO
                        Entity Population = new Entity("new_population");
                        Population.Id = entity.Id;
                        Population["tss_populationstatus"] = true;
                        organizationService.Update(Population);

                        //updateKAUIODeliveryDate(organizationService, Population);
                    }

                    if (!string.IsNullOrEmpty(_serialnumber))
                        updateKAUIODeliveryDate(organizationService, entity);
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Prospect Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation: " + ex.Message.ToString());
            }
        }

        //public void updateKAUIODeliveryDate(IOrganizationService organizationService, Entity entity)
        //{
        //    #region update DeliveryDate on KAUIO

        //    FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
        //    //fTCMS.AddCondition("tss_marketsizeid", ConditionOperator.Equal, listMarketSize.Entities[0].GetAttributeValue<Guid>("tss_mastermarketsizeid"));
        //    fKAUIO.AddCondition("tss_serialnumber", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("new_serialnumber").Id);

        //    QueryExpression qKAUIO = new QueryExpression("tss_kauio");
        //    qKAUIO.Criteria.AddFilter(fKAUIO);
        //    qKAUIO.ColumnSet = new ColumnSet(true);

        //    EntityCollection EC_KAUIO = _DL_tss_kauio.Select(organizationService, qKAUIO);

        //    foreach (Entity ent in EC_KAUIO.Entities)
        //    {
        //        if (entity["new_deliverydate"] != null && entity["new_deliverydate"].ToString() != "")
        //        {
        //            ent["tss_deliverydate"] = entity["new_deliverydate"];
        //            organizationService.Update(ent);
        //        }
        //    }

        //    #endregion
        //}

        public void updateKAUIODeliveryDate(IOrganizationService organizationService, Entity _population)
        {
            #region GET KAUIO
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.Equal, _population.Id);

            QueryExpression _queryexpression = new QueryExpression("tss_kauio");
            _queryexpression.Criteria.AddFilter(_filterexpression);
            _queryexpression.ColumnSet = new ColumnSet(true);

            EntityCollection _kauiocollection = _DL_tss_kauio.Select(organizationService, _queryexpression);
            #endregion

            #region UPDATE DELIVERY DATE
            foreach (var item in _kauiocollection.Entities)
            {
                Entity _kauio = organizationService.Retrieve("tss_kauio", item.Id, new ColumnSet(true));
                Entity _keyaccount = organizationService.Retrieve("tss_keyaccount", item.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true)); // _keyaccountid = 
                int _status = _keyaccount.GetAttributeValue<OptionSetValue>("tss_status").Value;

                if (_status == 865920000) // STATUS : OPEN
                {
                    _kauio["tss_deliverydate"] = _population["new_deliverydate"];

                    organizationService.Update(_kauio);
                }
            }
            #endregion
        }
    }
}
