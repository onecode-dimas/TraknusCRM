using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using TrakNusRapidService.DataLayer;
using Microsoft.Xrm.Sdk.Query;

using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Plugins.BusinessLayer
{
    class BL_trs_funtionallocation
    {
        #region Constants
        private const string _classname = "BL_trs_funtionallocation";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_trs_functionallocation _DL_trs_functionallocation = new DL_trs_functionallocation();
        private DL_new_population _DL_new_population = new DL_new_population();
        private FSAP _fSAP = new FSAP();
        #endregion

        #region Privates
        private void SendtoMobile(IOrganizationService organizationService, Guid id)
        {
            FMobile _fmobile = new FMobile(organizationService);
            _fmobile.SendFunctionalLocation(organizationService, id);
        }
        #endregion

        #region Publics
        #endregion

        #region Events
        #region Forms
        public void Form_OnCreate(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                Guid customer = Guid.Empty;
                string FunLocCode = string.Empty;
                string FunLocName = string.Empty;
                string CustomerName = string.Empty;
                string Area = string.Empty;
                string Address = string.Empty;
                string longitude = string.Empty;
                string latitude = string.Empty;
                string Location = string.Empty;

                Entity entity = new Entity("trs_functionallocation");
                Entity previous = new Entity("trs_functionallocation");

                if (pluginExceptionContext.MessageName == "Create" || pluginExceptionContext.MessageName == "Update")
                {
                    entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                }

                if (pluginExceptionContext.MessageName == "Update" || pluginExceptionContext.MessageName == "Delete")
                {
                    previous = (Entity)pluginExceptionContext.PreEntityImages["PreImage"];
                }

                if (entity.LogicalName == _DL_trs_functionallocation.EntityName)
                {
                    if (entity.Attributes.Contains("trs_functionalcode") && entity.Attributes["trs_functionalcode"] != null)
                    {
                        FunLocCode = entity.Attributes["trs_functionalcode"].ToString();
                    }
                    else if (previous.Attributes.Contains("trs_functionalcode") && previous.Attributes["trs_functionalcode"] != null)
                    {
                        FunLocCode = previous.Attributes["trs_functionalcode"].ToString();
                    }

                    if (entity.Attributes.Contains("trs_name") && entity.Attributes["trs_name"] != null)
                    {
                        FunLocName = entity.Attributes["trs_name"].ToString();
                    }
                    else if (previous.Attributes.Contains("trs_name") && previous.Attributes["trs_name"] != null)
                    {
                        FunLocName = previous.Attributes["trs_name"].ToString();
                    }

                    if (entity.Attributes.Contains("trs_customer") && entity.Attributes["trs_customer"] != null)
                    {
                        customer = ((EntityReference)entity.Attributes["trs_customer"]).Id;
                    }
                    else if (previous.Attributes.Contains("trs_customer") && previous.Attributes["trs_customer"] != null)
                    {
                        customer = ((EntityReference)previous.Attributes["trs_customer"]).Id;
                    }

                    QueryExpression qe = new QueryExpression();
                    qe.EntityName = "account";
                    qe.ColumnSet.AllColumns = true;

                    ConditionExpression con1 = new ConditionExpression();
                    con1.AttributeName = "accountid";
                    con1.Operator = ConditionOperator.Equal;
                    con1.Values.Add(customer);

                    FilterExpression Mainfilter = new FilterExpression();
                    Mainfilter.FilterOperator = LogicalOperator.And;
                    Mainfilter.Conditions.Add(con1);

                    qe.Criteria.AddFilter(Mainfilter);
                    EntityCollection results = organizationService.RetrieveMultiple(qe);

                    if (results.Entities.Count > 0)
                    {
                        foreach (var item in results.Entities)
                        {
                            CustomerName = item.Attributes["name"].ToString();
                        }
                    }

                    if (entity.Attributes.Contains("trs_functionaladdress") && entity.Attributes["trs_functionaladdress"] != null)
                    {
                        Address = entity.Attributes["trs_functionaladdress"].ToString();
                    }
                    else if (previous.Attributes.Contains("trs_functionaladdress") && previous.Attributes["trs_functionaladdress"] != null)
                    {
                        Address = previous.Attributes["trs_functionaladdress"].ToString();
                    }

                    if (entity.Attributes.Contains("trs_area") && entity.Attributes["trs_area"] != null)
                    {
                        Area = entity.Attributes["trs_area"].ToString();
                    }
                    else if (previous.Attributes.Contains("trs_area") && previous.Attributes["trs_area"] != null)
                    {
                        Area = previous.Attributes["trs_area"].ToString();
                    }

                    if (entity.Attributes.Contains("trs_functionallongitude") && entity.Attributes["trs_functionallongitude"] != null)
                    {
                        longitude = entity.Attributes["trs_functionallongitude"].ToString();
                    }
                    else if (previous.Attributes.Contains("trs_functionallongitude") && previous.Attributes["trs_functionallongitude"] != null)
                    {
                        longitude = previous.Attributes["trs_functionallongitude"].ToString();
                    }

                    if (entity.Attributes.Contains("trs_functionallatitude") && entity.Attributes["trs_functionallatitude"] != null)
                    {
                        latitude = entity.Attributes["trs_functionallatitude"].ToString();
                    }
                    else if (previous.Attributes.Contains("trs_functionallatitude") && previous.Attributes["trs_functionallatitude"] != null)
                    {
                        latitude = previous.Attributes["trs_functionallatitude"].ToString();
                    }
                    
                    Location = longitude + ", " + latitude;

                    if ((entity.Attributes.Contains("trs_functionallongitude") && entity.Attributes.Contains("trs_functionallatitude")) &&
                        (entity.Attributes["trs_functionallongitude"] != null || entity.Attributes["trs_functionallatitude"] != null))
                    {
                        UpdatePopulationByFunctionallocationCode(entity.Id, organizationService, Convert.ToDecimal(longitude), Convert.ToDecimal (latitude));
                    }

                    //Send to Mobile
                    SendtoMobile(organizationService, entity.Id);

                    //Send to SAP (08/12/2014)
                    if (_fSAP.SynchronizetoSAP(organizationService))
                    {
                        string path = @"\\" + _fSAP.GetSAPSharingPath(organizationService) + @"\02_TRSTOSAP\";
                        if (System.IO.Directory.Exists(path))
                        {
                            string text = FunLocCode + "|" + FunLocName + "|" + CustomerName + "|" + Area + "|" + Address + "|" + Location;
                            string filename = "FL_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + FunLocCode + ".txt";
                            System.IO.File.WriteAllText(path + filename, text);
                        }
                        else
                        {
                            throw new Exception("Directory not found: " + path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate : " + ex.Message.ToString());
            }
        }
        #endregion

        #region Fields
        private void UpdatePopulationByFunctionallocationCode(Guid _funcLocId, IOrganizationService organizationService, decimal _longitude, decimal _latitude)
        {
            try
            {
                QueryExpression qe = new QueryExpression();
                qe.EntityName = "new_population";
                qe.ColumnSet.AddColumns("new_populationid");

                ConditionExpression con1 = new ConditionExpression();
                con1.AttributeName = "trs_functionallocation";
                con1.Operator = ConditionOperator.Equal;
                con1.Values.Add(_funcLocId);

                FilterExpression Mainfilter = new FilterExpression();
                Mainfilter.FilterOperator = LogicalOperator.And;
                Mainfilter.Conditions.Add(con1);

                qe.Criteria.AddFilter(Mainfilter);

                
                EntityCollection eCollection = _DL_new_population.Select(organizationService, qe);
                foreach (Entity _entity in eCollection.Entities)
                {
                    _DL_new_population.trs_functionallongitude = _longitude;
                    _DL_new_population.trs_functionallatitude = _latitude;
                    _DL_new_population.Update(organizationService, _entity.GetAttributeValue<Guid>("new_populationid"));
                }

            }
            catch (Exception ex)
            {
                
                throw new InvalidPluginExecutionException(_classname + ".UpdatePopulationByFunctionallocationCode : " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion

        public void UpdateFunctionalLocationRunningNumber(IOrganizationService organizationService,
            IPluginExecutionContext pluginExecutionContext)
        {

            try
            {
                //create organization service context
                var _DL_functionallocation = new DL_trs_functionallocation();

                OrganizationServiceContext context = new OrganizationServiceContext(organizationService);
                //get current functional location
                var entity = pluginExecutionContext.InputParameters["Target"] as Entity;
                
                if (entity == null) throw new Exception("Entity target is null");
                
                var currentEntity = (from currentity in context.CreateQuery("trs_functionallocation")
                    where
                        currentity.GetAttributeValue<Guid>("trs_functionallocationid") ==
                        entity.GetAttributeValue<Guid>("trs_functionallocationid")
                    select currentity).First();
                entity = currentEntity;

                var customerGuid = entity.GetAttributeValue<EntityReference>("trs_customer").Id;

                int lastRunningNumber = 0;
                //get running number 
                try
                {
                    var funlocrunningnumber = (from flRunningNumber in context.CreateQuery("trs_funclocrunningnumber")
                                               where flRunningNumber.GetAttributeValue<EntityReference>("trs_customer").Id == customerGuid
                                               select flRunningNumber).First();
                    //if found
                    lastRunningNumber = funlocrunningnumber.GetAttributeValue<int>("trs_lastnumber");
                    funlocrunningnumber.Attributes["trs_lastnumber"] = lastRunningNumber + 1;
                    context.UpdateObject(funlocrunningnumber);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    //there is not one that having that customerGuid
                    //create new one
                    var newRunningNumber = new Entity("trs_funclocrunningnumber");
                    newRunningNumber.Attributes["trs_customer"] = new EntityReference("account",customerGuid);
                    newRunningNumber.Attributes["trs_lastnumber"] = 01;
                    context.AddObject(newRunningNumber);
                    context.SaveChanges();
                    lastRunningNumber = 0;
                }

                //Get current customer
                var customerEntity = (from customer in context.CreateQuery("account")
                    where customer.GetAttributeValue<Guid>("accountid") == customerGuid
                    select customer).First();

                var customerName = customerEntity.GetAttributeValue<String>("name");
                var customerCode = customerEntity.GetAttributeValue<String>("accountnumber");
                //query to fun loc
                var currentRunningNumberString = (1 + lastRunningNumber).ToString().PadLeft(3, '0');
                
                _DL_functionallocation.trs_name = String.Format("{0} - {1}",customerName,currentRunningNumberString);
                _DL_functionallocation.trs_functionalcode = String.Format("{0}-{1}", customerCode, currentRunningNumberString);
                _DL_functionallocation.Update(organizationService,entity.GetAttributeValue<Guid>("trs_functionallocationid"));

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateFunctionalLocationRunningNumber : " + ex.ToString() + "Stack Trace :" + ex.StackTrace);
            }
        }

    }
}
