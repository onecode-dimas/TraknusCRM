using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;
using CalculateRTG.DataLayer;

namespace CalculateRTG.BusinessLayer
{
    class BL_serviceappointment
    {
        #region Depedencies
        private DL_serviceappointment _DL_serviceappointment = new DL_serviceappointment();
        private Helper.LogCreator log = new Helper.LogCreator();
        #endregion

        public void Execute(OrganizationService organizationService, Guid userId)
        {
            Helper.EntityExtractor entityExtractor = new Helper.EntityExtractor();
            log.UserID = userId;
            log.Write("starting reminder process ...");
            EntityCollection eWo = GetWoModifiedToday(organizationService);
            if (eWo.Entities.Count > 0)
            {
                //harus tahu commercial header
                Guid woId = eWo.Entities[0].GetAttributeValue<Guid>("activityid");
                            
                BusinessLayer.BL_serviceappointment wo1 = new BusinessLayer.BL_serviceappointment();
                EntityCollection eServiceApp1 = wo1.getAllServiceAppointment(organizationService, woId);
                if (eServiceApp1.Entities.Count > 0)
                {                    
                    string leaderId = ((EntityReference)eServiceApp1.Entities[0].Attributes["trs_mechanicleader"]).Id.ToString();
                    
                    BL_task bt = new BL_task();
                    EntityCollection ecolTask = bt.CheckingTaskRecord(organizationService, woId);
                    foreach (Entity eTask in ecolTask.Entities)
                    {
                        Guid taskId = eTask.GetAttributeValue<Guid>("activityid");
                        decimal totRTG = eTask.GetAttributeValue<decimal>("trs_totalrtg");

                        EntityCollection eAp1 = getResourcesActivityParty(organizationService, woId);
                        Int16 countMechanic = Convert.ToInt16(eAp1.Entities.Count());
                        foreach (Entity partyList1 in eAp1.Entities)
                        {
                            Guid mechanicId = ((EntityReference)partyList1.Attributes["partyid"]).Id;
                            string mId = mechanicId.ToString();

                            //For Leader
                            double leaderPoint = Convert.ToDouble(totRTG) * 0.1;
                            double memberPoint = Convert.ToDouble(totRTG) - leaderPoint;

                            decimal leadFormula = Convert.ToDecimal(leaderPoint) + (Convert.ToDecimal(memberPoint) / countMechanic);
                            decimal memberFormula = Convert.ToDecimal(memberPoint) / countMechanic;

                            BL_trs_mechanictimehourdetails mhd = new BL_trs_mechanictimehourdetails();
                            if (leaderId.Equals(mId, StringComparison.Ordinal))
                            {
                                mhd.Form_OnCreate(organizationService, mechanicId, woId, totRTG, leadFormula, "");
                                //Leader
                            }
                            else
                            {
                                mhd.Form_OnCreate(organizationService, mechanicId, woId, totRTG, memberFormula, "");
                                //"Member"
                            }
                        }
                    }
            
                }
            }
        }

        #region Old Code
        //public void ExecuteTesting(OrganizationService organizationService, Guid userId)
        //{
        //    Helper.EntityExtractor entityExtractor = new Helper.EntityExtractor();
        //    log.UserID = userId;
        //    log.Write("starting reminder process ...");

        //    Guid woId = new Guid("2B7E3CDC-A364-E411-A206-C4346BAC57E3");

        //    BusinessLayer.BL_serviceappointment wo1 = new BusinessLayer.BL_serviceappointment();
        //    EntityCollection eServiceApp1 = wo1.getAllServiceAppointment(organizationService, woId);
        //    if (eServiceApp1.Entities.Count > 0)
        //    {
        //        string leaderId = ((EntityReference)eServiceApp1.Entities[0].Attributes["trs_mechanicleader"]).Id.ToString();
        //        BL_task bt = new BL_task();
        //        EntityCollection ecolTask = bt.CheckingTaskRecord(organizationService, woId);
        //        foreach (Entity eTask in ecolTask.Entities)
        //        {
        //            Guid taskId = eTask.GetAttributeValue<Guid>("activityid");
        //            decimal totRTG = eTask.GetAttributeValue<decimal>("trs_totalrtg");

        //            EntityCollection eAp1 = getResourcesActivityParty(organizationService, woId);
        //            Int16 countMechanic = Convert.ToInt16(eAp1.Entities.Count());
        //            foreach (Entity partyList1 in eAp1.Entities)
        //            {
        //                Guid mechanicId = ((EntityReference)partyList1.Attributes["partyid"]).Id;
        //                string mId = mechanicId.ToString();

        //                //For Leader
        //                double leaderPoint = Convert.ToDouble(totRTG) * 0.1;
        //                double memberPoint = Convert.ToDouble(totRTG) - leaderPoint;

        //                decimal leadFormula = Convert.ToDecimal(leaderPoint) + (Convert.ToDecimal(memberPoint) / countMechanic);
        //                decimal memberFormula = Convert.ToDecimal(memberPoint) / countMechanic;

        //                BL_trs_mechanictimehourdetails mhd = new BL_trs_mechanictimehourdetails();
                        

        //                if (leaderId.Equals(mId, StringComparison.Ordinal))
        //                {
        //                    mhd.Form_OnCreate(organizationService, mechanicId, woId, totRTG, leadFormula, "");
        //                    //Leader
        //                }
        //                else
        //                {
        //                    mhd.Form_OnCreate(organizationService, mechanicId, woId, totRTG, memberFormula, "");
        //                    //"Member"
        //                }
        //            }
        //        }
        //    }
                             
        //}
        #endregion

        private EntityCollection getResourcesActivityParty(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression("activityparty");
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression conId = new ConditionExpression();
                conId.AttributeName = "activityid";
                conId.Operator = ConditionOperator.Equal;
                conId.Values.Add(Id.ToString());

                ConditionExpression typeMask = new ConditionExpression();
                typeMask.AttributeName = "participationtypemask";
                typeMask.Operator = ConditionOperator.Equal;
                typeMask.Values.Add("10");
                //Activity Party Types = 10 (Specifies a resource/ServiceAppointment.Resources)
                ////http://msdn.microsoft.com/en-us/library/gg328549.aspx

                qe.Criteria.AddCondition(conId);
                qe.Criteria.AddCondition(typeMask);

                return organizationService.RetrieveMultiple(qe);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private EntityCollection GetWoModifiedToday(OrganizationService organizationService)
        {
            DataLayer.DL_serviceappointment sap= new DataLayer.DL_serviceappointment();

            QueryExpression queryExpression = new QueryExpression(sap.EntityName);
            queryExpression.ColumnSet.AddColumns("activityid", "modifiedon");

            FilterExpression filterExpression = new FilterExpression(LogicalOperator.And);
            filterExpression.AddCondition("modifiedon", ConditionOperator.Equal, DateTime.Now.Date);
            filterExpression.AddCondition("statuscode", ConditionOperator.Equal, 167630002);
            queryExpression.Criteria.AddFilter(filterExpression);

            return sap.Select(organizationService, queryExpression);

        }

        public EntityCollection getAllServiceAppointment(IOrganizationService organizationService, Guid Id)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_DL_serviceappointment.EntityName);
                qe.ColumnSet = new ColumnSet(true);

                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "activityid";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(Id.ToString());

                qe.Criteria.AddCondition(ce);

                return _DL_serviceappointment.Select(organizationService, qe);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
