using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web;
using System.Collections;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_kauio
    {
        #region Constants
        private const string _classname = "BL_tss_kauio";
        private const string _entityname = "tss_kauio";
        #endregion

        #region Depedencies
        private BL_trs_runningnumber _BL_trs_runningnumber = new BL_trs_runningnumber();
        private BL_tss_population _BL_tss_population = new BL_tss_population();
        private DL_population _DL_population = new DL_population();
        #endregion

        DateTime? nulldatetime = null;
        decimal? nulldecimal = null;

        #region CREATE
        public void Form_OnCreate_PreOperation(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                //Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];

                if (_entity.LogicalName == _entityname)
                {
                    //_entity = _organizationservice.Retrieve(_entityname, _entity.Id, new ColumnSet(true));
                    Entity keyAccount = _organizationservice.Retrieve("tss_keyaccount", _entity.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));

                    if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                    {
                        throw new InvalidPluginExecutionException("KA UIO can only be created when the status Key Account is OPEN.");
                    }

                    //throw new InvalidPluginExecutionException("HELLO WORLD ! WOOHOO !");

                    Guid _serialnumber = _entity.Attributes.Contains("tss_serialnumber") ? _entity.GetAttributeValue<EntityReference>("tss_serialnumber").Id : new Guid();

                    if (_serialnumber != null && _serialnumber != new Guid())
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.Equal, _serialnumber);

                        QueryExpression _queryexpression = new QueryExpression("tss_kauio");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddFilter(_filterexpression);

                        EntityCollection _kauiocollection = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _kauio in _kauiocollection.Entities)
                        {
                            Entity _keyaccount = _organizationservice.Retrieve("tss_keyaccount", _kauio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));
                            int _status = _keyaccount.GetAttributeValue<OptionSetValue>("tss_status").Value;

                            if (keyAccount.Id == _keyaccount.Id)
                            {
                                throw new InvalidPluginExecutionException("Serial number already exist is this Key Account !");
                            }
                            else if (keyAccount.Id != _keyaccount.Id && (_status == 865920000 || _status == 865920001 || _status == 865920003))
                            {
                                throw new InvalidPluginExecutionException("Serial number is used in Key Account '" + _keyaccount.GetAttributeValue<string>("tss_kamsid") + "' !");
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Serial Number NOT found in population !");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    Guid _serialnumber = new Guid();
                    if (entity.Attributes.Contains("tss_serialnumber"))
                    {
                        _serialnumber = entity.GetAttributeValue<EntityReference>("tss_serialnumber").Id;
                    }
                    else
                        throw new InvalidPluginExecutionException("Serial Number NOT found in population !");

                    Entity _account = entity.Attributes.Contains("tss_customer") ? organizationService.Retrieve("account", entity.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true)) : null;

                    if (_account == null)
                        throw new InvalidPluginExecutionException("Customer NOT found !");

                    Entity oPopulation = new Entity("new_population");
                    oPopulation.Id = _serialnumber;

                    if (_account.Attributes.Contains("new_businesssector"))
                    {
                        Entity _businesssector = organizationService.Retrieve("new_businesssector", _account.GetAttributeValue<EntityReference>("new_businesssector").Id, new ColumnSet(true));

                        if (_businesssector.Attributes.Contains("tss_estworkinghour"))
                            oPopulation["tss_estworkinghour"] = _businesssector.GetAttributeValue<int>("tss_estworkinghour");
                    }

                    if (entity.Attributes.Contains("tss_currenthourmeter"))
                        oPopulation["tss_mscurrenthourmeter"] = entity.GetAttributeValue<decimal>("tss_currenthourmeter");

                    if (entity.Attributes.Contains("tss_currenthourmeterdate"))
                    {
                        if (entity.GetAttributeValue<DateTime>("tss_currenthourmeterdate") != DateTime.MinValue)
                            oPopulation["tss_mscurrenthourmeterdate"] = entity.GetAttributeValue<DateTime>("tss_currenthourmeterdate").ToLocalTime().Date;
                    }

                    if (entity.Attributes.Contains("tss_lasthourmeter"))
                        oPopulation["tss_mslasthourmeter"] = entity.GetAttributeValue<decimal>("tss_lasthourmeter");

                    if (entity.Attributes.Contains("tss_lasthourmeterdate"))
                    {
                        if (entity.GetAttributeValue<DateTime>("tss_lasthourmeterdate") != DateTime.MinValue)
                            oPopulation["tss_mslasthourmeterdate"] = entity.GetAttributeValue<DateTime>("tss_lasthourmeterdate").ToLocalTime().Date;
                    }

                    organizationService.Update(oPopulation);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PostOperation : " + ex.Message.ToString());
            }
        }
        #endregion

        #region UPDATE
        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    Entity keyAccount = organizationService.Retrieve("tss_keyaccount", entity.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));

                    if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                    {
                        throw new InvalidPluginExecutionException("KA UIO can only be updated when the status Key Account is OPEN.");
                    }

                    Guid _serialnumber = entity.GetAttributeValue<EntityReference>("tss_serialnumber").Id;

                    FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                    _filterexpression.AddCondition("tss_serialnumber", ConditionOperator.Equal, _serialnumber);

                    QueryExpression _queryexpression = new QueryExpression("tss_kauio");
                    _queryexpression.ColumnSet = new ColumnSet(true);
                    _queryexpression.Criteria.AddFilter(_filterexpression);

                    EntityCollection _kauiocollection = organizationService.RetrieveMultiple(_queryexpression);

                    foreach (var _kauio in _kauiocollection.Entities)
                    {
                        Entity _keyaccount = organizationService.Retrieve("tss_keyaccount", _kauio.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));
                        int _status = _keyaccount.GetAttributeValue<OptionSetValue>("tss_status").Value;

                        if (keyAccount.Id != _keyaccount.Id && (_status == 865920000 || _status == 865920001 || _status == 865920003))
                        {
                            throw new InvalidPluginExecutionException("Serial number is used in Key Account '" + _keyaccount.GetAttributeValue<string>("tss_kamsid") + "' !");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    Guid _serialnumber = entity.GetAttributeValue<EntityReference>("tss_serialnumber").Id;

                    Entity oPopulation = new Entity("new_population");
                    oPopulation.Id = _serialnumber;

                    //oPopulation["tss_mscurrenthourmeter"] = entity.Attributes.Contains("tss_currenthourmeter") ? entity.GetAttributeValue<decimal>("tss_currenthourmeter") : nulldecimal;
                    //oPopulation["tss_mscurrenthourmeterdate"] = entity.GetAttributeValue<DateTime>("tss_currenthourmeterdate") == DateTime.MinValue ? nulldatetime : entity.GetAttributeValue<DateTime>("tss_currenthourmeterdate").ToLocalTime().Date;
                    //oPopulation["tss_mslasthourmeter"] = entity.Attributes.Contains("tss_lasthourmeter") ? entity.GetAttributeValue<decimal>("tss_lasthourmeter") : nulldecimal;
                    //oPopulation["tss_mslasthourmeterdate"] = entity.GetAttributeValue<DateTime>("tss_lasthourmeterdate") == DateTime.MinValue ? nulldatetime : entity.GetAttributeValue<DateTime>("tss_lasthourmeterdate").ToLocalTime().Date;

                    if (entity.Attributes.Contains("tss_currenthourmeter"))
                        oPopulation["tss_mscurrenthourmeter"] = entity.GetAttributeValue<decimal>("tss_currenthourmeter");

                    if (entity.GetAttributeValue<DateTime>("tss_currenthourmeterdate") != DateTime.MinValue)
                        oPopulation["tss_mscurrenthourmeterdate"] = entity.GetAttributeValue<DateTime>("tss_currenthourmeterdate").ToLocalTime().Date;

                    if (entity.Attributes.Contains("tss_lasthourmeter"))
                        oPopulation["tss_mslasthourmeter"] = entity.GetAttributeValue<decimal>("tss_lasthourmeter");

                    if (entity.GetAttributeValue<DateTime>("tss_lasthourmeterdate") != DateTime.MinValue)
                        oPopulation["tss_mslasthourmeterdate"] = entity.GetAttributeValue<DateTime>("tss_lasthourmeterdate").ToLocalTime().Date;

                    if (entity.Attributes.Contains("tss_estworkinghour"))
                        oPopulation["tss_estworkinghour"] = entity.GetAttributeValue<int>("tss_estworkinghour");

                    organizationService.Update(oPopulation);

                    //updateEstWorkHourPopulation(organizationService, entity);

                    //string _errordescription = isValid(organizationService, _serialnumber);
                    //if (_errordescription != "")
                    //    throw new InvalidPluginExecutionException(_errordescription);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PostOperation : " + ex.Message.ToString());
            }
        }
        #endregion

        #region UPDATE
        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));

                if (entity.LogicalName == _entityname)
                {
                    Entity keyAccount = organizationService.Retrieve("tss_keyaccount", entity.GetAttributeValue<EntityReference>("tss_keyaccountid").Id, new ColumnSet(true));

                    if (keyAccount.GetAttributeValue<OptionSetValue>("tss_status").Value != 865920000) //OPEN
                    {
                        throw new InvalidPluginExecutionException("KA UIO can only be deleted when the status Key Account is OPEN.");
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PreOperation : " + ex.Message.ToString());
            }
        }
        #endregion

        public void updateEstWorkHourPopulation(IOrganizationService organizationService, Entity entity)
        {
            FilterExpression fPopulation = new FilterExpression(LogicalOperator.And);
            fPopulation.AddCondition("new_serialnumber", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("tss_serialnumber").Id);
            //fPopulation.AddCondition("tss_populationstatus", ConditionOperator.Equal, false);

            QueryExpression qPopulation = new QueryExpression("new_population");
            qPopulation.Criteria.AddFilter(fPopulation);
            qPopulation.ColumnSet = new ColumnSet(true);

            EntityCollection EC_Population = _DL_population.Select(organizationService, qPopulation);

            foreach (Entity ent in EC_Population.Entities)
            {
                if (entity.Attributes.Contains("tss_estworkinghour"))
                {
                    ent["tss_estworkinghour"] = entity["tss_estworkinghour"];

                    organizationService.Update(ent);
                }
            }

        }

        public string isValid(IOrganizationService organizationService, Guid _serialnumber)
        {
            Entity _population = organizationService.Retrieve("new_population", _serialnumber, new ColumnSet(true));
            List<string> _errorcollection = new List<string>();
            string _errormessage = "";
            bool _mscurrenthourmeter = _population.Attributes.Contains("tss_mscurrenthourmeter");
            bool _mscurrenthourmeterdate = _population.Attributes.Contains("tss_mscurrenthourmeterdate");
            bool _mslasthourmeter = _population.Attributes.Contains("tss_mslasthourmeter");
            bool _mslasthourmeterdate = _population.Attributes.Contains("tss_mslasthourmeterdate");
            bool _deliverydate = _population.Attributes.Contains("new_deliverydate");

            if (
                (_mscurrenthourmeter && _mscurrenthourmeterdate && _mslasthourmeter && _mslasthourmeterdate) ||
                (_mscurrenthourmeter && _mscurrenthourmeterdate && _deliverydate) ||
                (!_mscurrenthourmeter && !_mscurrenthourmeterdate && !_mslasthourmeter && !_mslasthourmeterdate && _deliverydate)
                )
                _errormessage = "";
            else
            {
                if (_mscurrenthourmeter)
                    _errorcollection.Add("Current Hour Meter");

                if (!_mscurrenthourmeterdate)
                    _errorcollection.Add("Current Hour Meter (Date)");

                if (!_mslasthourmeter)
                    _errorcollection.Add("Last Hour Meter");

                if (!_mslasthourmeterdate)
                    _errorcollection.Add("Last Hour Meter (Date)");

                if (!_deliverydate)
                    _errorcollection.Add("Delivery Date");

                if (_errorcollection.Count() > 0)
                {
                    foreach (var item in _errorcollection)
                    {
                        _errormessage += item + ", ";
                    }

                    _errormessage = _errormessage.Substring(0, _errormessage.Length - 1) + " still not define !";
                }
                else
                    _errormessage = "Current Hour Meter, Current Hour Meter (Date), Last Hour Meter , Last Hour Meter (Date), Delivery Date or Overhaul Date still not define";
            }

            return _errormessage;
        }
    }
}
