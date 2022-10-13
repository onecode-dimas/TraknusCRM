using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_prospectpartheader
    {
        #region Constants
        private const string _classname = "BL_tss_prospectpartheader";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_tss_rating _DL_tss_rating = new DL_tss_rating();
        #endregion

        #region Properties
        private bool _tss_priceamount = false;
        private Money _tss_priceamount_value;
        public decimal tss_priceamount
        {
            get { return _tss_priceamount ? _tss_priceamount_value.Value : 0; }
            set { _tss_priceamount = true; _tss_priceamount_value = new Money(value); }
        }

        private bool _tss_totalamount = false;
        private Money _tss_totalamount_value;
        public decimal tss_totalamount
        {
            get { return _tss_totalamount ? _tss_totalamount_value.Value : 0; }
            set { _tss_totalamount = true; _tss_totalamount_value = new Money(value); }
        }

        //two option
        private bool _tss_underminimumprice = false;
        private bool _tss_underminimumprice_value;
        public bool tss_underminimumprice
        {
            get { return _tss_underminimumprice ? _tss_underminimumprice_value : false; }
            set { _tss_underminimumprice = true; _tss_underminimumprice_value = value; }
        }
        #endregion

        #region Publics
        //Update Total Price (Prospect Part) from Prospect Part Lines
        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {
                bool flagMinimumPrice = false;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_priceamount"))
                    {
                        //tss_totalamount = entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        Guid prospectID = entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").Id;
                        string prospectName = entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").LogicalName;


                        QueryExpression qProspectPartLines = new QueryExpression("tss_prospectpartlines")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_prospectpartheader",ConditionOperator.Equal,prospectID)
                                }
                            }
                        };
                        var ecProspectPartLines = organizationService.RetrieveMultiple(qProspectPartLines);
                        foreach (var item in ecProspectPartLines.Entities)
                        {
                            if (item.GetAttributeValue<bool>("tss_underminimumprice")) flagMinimumPrice = true;
                            tss_priceamount = item.GetAttributeValue<Money>("tss_priceamount").Value;
                            tss_totalamount += tss_priceamount;
                        }

                        Entity pros = new Entity(prospectName);
                        pros.Id = prospectID;
                        tss_underminimumprice = flagMinimumPrice;
                        if (_tss_underminimumprice) pros.Attributes["tss_underminimumprice"] = _tss_underminimumprice_value;
                        pros.Attributes["tss_totalamount"] = _tss_totalamount_value;
                        organizationService.Update(pros);
                    }
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

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity postImg)
        {
            try
            {
                bool flagMinimumPrice = false;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    if (entity.Attributes.Contains("tss_priceamount"))
                    {
                        //tss_totalamount = entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        Guid prospectID = entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").Id;
                        string prospectName = entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").LogicalName;

                        QueryExpression qProspectPartLines = new QueryExpression("tss_prospectpartlines")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_prospectpartheader",ConditionOperator.Equal,prospectID)
                                }
                            }
                        };
                        var ecProspectPartLines = organizationService.RetrieveMultiple(qProspectPartLines);

                        flagMinimumPrice = postImg.GetAttributeValue<bool>("tss_underminimumprice");
                        foreach (var item in ecProspectPartLines.Entities)
                        {
                            if (item.GetAttributeValue<bool>("tss_underminimumprice") && !flagMinimumPrice) flagMinimumPrice = true;
                            tss_priceamount = item.GetAttributeValue<Money>("tss_priceamount").Value;
                            tss_totalamount += tss_priceamount;
                        }

                        Entity pros = new Entity(prospectName);
                        pros.Id = prospectID;
                        tss_underminimumprice = flagMinimumPrice;
                        //throw new InvalidPluginExecutionException("UMP: " + tss_underminimumprice);
                        if (_tss_underminimumprice) pros.Attributes["tss_underminimumprice"] = _tss_underminimumprice_value;
                        pros.Attributes["tss_totalamount"] = _tss_totalamount_value;
                        organizationService.Update(pros);
                        
                    }
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

        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity preImg)
        {
            try
            {
                int countIMP = 0;
                bool flagMinimumPrice = true;
                Entity entity = organizationService.Retrieve(pluginExceptionContext.PrimaryEntityName, pluginExceptionContext.PrimaryEntityId, new ColumnSet(true));
                if (entity.LogicalName == pluginExceptionContext.PrimaryEntityName)
                {
                    //check pipeline status on header
                    if (entity.Attributes.Contains("tss_prospectpartheader"))
                    {
                        var context = new OrganizationServiceContext(organizationService);
                        var header = (from c in context.CreateQuery("tss_prospectpartheader")
                                      where c.GetAttributeValue<Guid>("tss_prospectpartheaderid") == entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").Id
                                      select c).ToList();
                        if (header.Count > 0)
                        {
                            if (header[0].Attributes.Contains("tss_pipelinephase"))
                            {
                                if (header[0].GetAttributeValue<OptionSetValue>("tss_pipelinephase").Value != 865920000)
                                {
                                    throw new InvalidPluginExecutionException("Can't delete prospect line, pipeline status is not inquiry!");
                                }
                            }
                        }
                    }

                    if (entity.Attributes.Contains("tss_priceamount"))
                    {
                        //tss_totalamount = entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        Guid prospectID = entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").Id;
                        string prospectName = entity.GetAttributeValue<EntityReference>("tss_prospectpartheader").LogicalName;
                        bool UMP = preImg.GetAttributeValue<bool>("tss_underminimumprice");
                        //throw new InvalidPluginExecutionException("UMP: " + UMP);
                        QueryExpression qProspectPartLines = new QueryExpression("tss_prospectpartlines")
                        {
                            ColumnSet = new ColumnSet(true),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
                                {
                                    new ConditionExpression("tss_prospectpartheader",ConditionOperator.Equal,prospectID)
                                }
                            }
                        };
                        var ecProspectPartLines = organizationService.RetrieveMultiple(qProspectPartLines);
                        foreach (var item in ecProspectPartLines.Entities)
                        {
                            if (item.GetAttributeValue<bool>("tss_underminimumprice")) countIMP++;
                            tss_priceamount = item.GetAttributeValue<Money>("tss_priceamount").Value;
                            tss_totalamount += tss_priceamount;
                        }
                        tss_totalamount -= entity.GetAttributeValue<Money>("tss_priceamount").Value;
                        if (countIMP == 1 && UMP) flagMinimumPrice = false;
                        if (countIMP < 1) flagMinimumPrice = false;
                        Entity pros = new Entity(prospectName);
                        pros.Id = prospectID;
                        tss_underminimumprice = flagMinimumPrice;
                        pros.Attributes["tss_underminimumprice"] = _tss_underminimumprice_value;
                        pros.Attributes["tss_totalamount"] = _tss_totalamount_value;
                        organizationService.Update(pros);
                    }
                }
                else
                {
                    throw new Exception("Wrong entity " + entity.LogicalName + " on Prospect Part Lines !");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PostOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnCreate_UpdateProspectRating(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            try
            {
                #region Variables
                Entity entity = organizationService.Retrieve(pluginExecutionContext.PrimaryEntityName, pluginExecutionContext.PrimaryEntityId, new ColumnSet(true));
                Guid ProspectRatingId = Guid.Empty;

                DateTime EstimationCloseDate = DateTime.MinValue;
                #endregion
                
                if (entity.LogicalName == _DL_tss_prospectpartheader.EntityName && entity.Attributes.Contains("tss_statusreason"))
                {
                    //Estimation Close Date
                    if (entity.Attributes.Contains("tss_estimationclosedate"))
                    {
                        LocalTimeFromUtcTimeRequest convert = new LocalTimeFromUtcTimeRequest
                        {
                            UtcTime = entity.GetAttributeValue<DateTime>("tss_estimationclosedate"),
                            TimeZoneCode = 205 // Timezone of user
                        };
                        LocalTimeFromUtcTimeResponse response = (LocalTimeFromUtcTimeResponse)organizationService.Execute(convert);
                        EstimationCloseDate = response.LocalTime.Date;
                    }
                    else
                        throw new InvalidPluginExecutionException("Can not found Estimation Close Date !");

                    if (entity.GetAttributeValue<OptionSetValue>("tss_statusreason").Value == Configuration.ProspectStatusReason_Open)
                    {
                        int Compare = DateTime.Compare(EstimationCloseDate, DateTime.Now.Date);

                        if (Compare < 0)
                        {
                            ProspectRatingId = GetProspectRating(organizationService, true, 0, 0, 0, false);
                        }
                        else
                        {
                            var dateSpan = CalculateDate.DateTimeSpan.CompareDates(EstimationCloseDate.Date, DateTime.Now.Date);

                            int TotalMonths = dateSpan.Months;
                            int TotalDays = dateSpan.Days;

                            if (TotalMonths > 0)
                            {
                                if (TotalMonths > 0 && TotalDays > 0)
                                {
                                    TotalMonths += 1;  //inverse +1 in TotalMonth

                                    ProspectRatingId = GetProspectRating(organizationService, false, Configuration.RatingPeriod_Month, TotalMonths, TotalMonths, false);
                                }
                                else
                                {
                                    ProspectRatingId = GetProspectRating(organizationService, false, Configuration.RatingPeriod_Month, TotalMonths, TotalMonths, false);
                                }
                            }
                            else
                            {
                                ProspectRatingId = GetProspectRating(organizationService, false, Configuration.RatingPeriod_Month, TotalMonths, TotalMonths, true);
                            }
                        }
                    }

                    Entity ProspectPart = new Entity(_DL_tss_prospectpartheader.EntityName);
                    ProspectPart.Id = entity.Id;
                    if (ProspectRatingId != Guid.Empty)
                        ProspectPart.Attributes["tss_rating"] = new EntityReference(_DL_tss_rating.EntityName, ProspectRatingId);
                    organizationService.Update(ProspectPart);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_UpdateProspectRating : " + ex.Message.ToString());
            }
        }

        public Guid GetProspectRating(IOrganizationService organizationservice, bool IsOverdue, int Period, int StartPeriod, int EndPeriod, bool IsHot)
        {
            try
            {
                QueryExpression queryExpression = new QueryExpression(_DL_tss_rating.EntityName);
                queryExpression.ColumnSet = new ColumnSet(true);

                if (IsOverdue == true)
                {
                    queryExpression.Criteria.AddCondition("tss_isoverdue", ConditionOperator.Equal, IsOverdue);
                }
                else if (IsHot == true)
                {
                    queryExpression.Criteria.AddCondition("tss_rating", ConditionOperator.Equal, "Hot");
                }
                else
                {
                    queryExpression.Criteria.AddCondition("tss_period", ConditionOperator.Equal, Period);
                    queryExpression.Criteria.AddCondition("tss_startperiod", ConditionOperator.LessEqual, StartPeriod);
                    queryExpression.Criteria.AddCondition("tss_endperiod", ConditionOperator.GreaterEqual, EndPeriod);
                }

                EntityCollection ec = _DL_tss_rating.Select(organizationservice, queryExpression);
                if (ec.Entities.Count > 0)
                    return ec.Entities[0].Id;
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".GetProspectRating : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
