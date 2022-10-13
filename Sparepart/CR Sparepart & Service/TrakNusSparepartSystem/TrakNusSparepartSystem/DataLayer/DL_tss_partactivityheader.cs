using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_partactivityheader
    {
        #region Dependencies
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        //private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        //private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        private DL_currency _DL_currency = new DL_currency();
        #endregion

        #region Propeties
        private string _classname = "DL_tss_partactivityheader";

        private string _entityname = "tss_partactivityheader";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Part Activity";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_planobject = false;
        private string _tss_planobject_value;
        public string tss_planobject
        {
            get { return _tss_planobject ? _tss_planobject_value : null; }
            set { _tss_planobject = true; _tss_planobject_value = value; }
        }

        private bool _tss_pss = false;
        private EntityReference _tss_pss_value;
        public Guid tss_pss
        {
            get { return _tss_pss ? _tss_pss_value.Id : Guid.Empty; }
            set { _tss_pss = true; _tss_pss_value = new EntityReference(_DL_systemuser.EntityName, value); }
        }

        private bool _tss_customer = false;
        private EntityReference _tss_customer_value;
        public Guid tss_customer
        {
            get { return _tss_customer ? _tss_customer_value.Id : Guid.Empty; }
            set { _tss_customer = true; _tss_customer_value = new EntityReference(_DL_account.EntityName, value); }
        }

        private bool _tss_location = false;
        private string _tss_location_value;
        public string tss_location
        {
            get { return _tss_location ? _tss_location_value : null; }
            set { _tss_location = true; _tss_location_value = value; }
        }

        private bool _tss_meetwith = false;
        private EntityReference _tss_meetwith_value;
        public Guid tss_meetwith
        {
            get { return _tss_meetwith ? _tss_meetwith_value.Id : Guid.Empty; }
            set { _tss_meetwith = true; _tss_meetwith_value = new EntityReference(_DL_contact.EntityName, value); }
        }

        private bool _tss_description = false;
        private string _tss_description_value;
        public string tss_description
        {
            get { return _tss_description ? _tss_description_value : null; }
            set { _tss_description = true; _tss_description_value = value; }
        }

        private bool _tss_plandate = false;
        private DateTime _tss_plandate_value;
        public DateTime tss_plandate
        {
            get { return _tss_plandate ? _tss_plandate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_plandate = true; _tss_plandate_value = value.ToLocalTime(); }
        }

        private bool _tss_planstarttime = false;
        private DateTime _tss_planstarttime_value;
        public DateTime tss_planstarttime
        {
            get { return _tss_planstarttime ? _tss_planstarttime_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_planstarttime = true; _tss_planstarttime_value = value.ToLocalTime(); }
        }

        private bool _tss_planendtime = false;
        private DateTime _tss_planendtime_value;
        public DateTime tss_planendtime
        {
            get { return _tss_planendtime ? _tss_planendtime_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_planendtime = true; _tss_planendtime_value = value.ToLocalTime(); }
        }

        private bool _tss_actualdate = false;
        private DateTime _tss_actualdate_value;
        public DateTime tss_actualdate
        {
            get { return _tss_actualdate ? _tss_actualdate_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_actualdate = true; _tss_actualdate_value = value.ToLocalTime(); }
        }

        private bool _tss_actualstarttime = false;
        private DateTime _tss_actualstarttime_value;
        public DateTime tss_actualstarttime
        {
            get { return _tss_actualstarttime ? _tss_actualstarttime_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_actualstarttime = true; _tss_actualstarttime_value = value.ToLocalTime(); }
        }

        private bool _tss_actualendtime = false;
        private DateTime _tss_actualendtime_value;
        public DateTime tss_actualendtime
        {
            get { return _tss_actualendtime ? _tss_actualendtime_value.ToLocalTime() : DateTime.MinValue; }
            set { _tss_actualendtime = true; _tss_actualendtime_value = value.ToLocalTime(); }
        }

        //OptionSet
        private bool _tss_status = false;
        private int _tss_status_value;
        public int tss_status
        {
            get { return _tss_status ? _tss_status_value : int.MinValue; }
            set { _tss_status = true; _tss_status_value = value; }
        }
        #endregion

        public int RetrieveMaxVisitOnSetup(IOrganizationService organizationService,ITracingService tracer)
        {
            int maxVisit = 0;
            QueryExpression Query = new QueryExpression("tss_sparepartsetup");  //component from quotation
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.AddCondition("tss_name", ConditionOperator.Equal, "TSS");
            Query.Criteria.AddCondition("tss_maxvisitperday", ConditionOperator.NotNull);

            EntityCollection SetupItems = organizationService.RetrieveMultiple(Query);
            if (SetupItems.Entities.Count > 0)
            {
                foreach (Entity Item in SetupItems.Entities)
                {                                         
                    maxVisit = Convert.ToInt32(Item.GetAttributeValue<string>("tss_maxvisitperday"));
                    tracer.Trace(maxVisit.ToString());
                }
            }

            return maxVisit;
        }

        public int RetrieveMaxbackDateOnSetup(IOrganizationService organizationService)
        {
            int maxBackDate = 0;
            QueryExpression Query = new QueryExpression("tss_sparepartsetup");  //component from quotation
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.AddCondition("tss_name", ConditionOperator.Equal, "TSS");
            Query.Criteria.AddCondition("tss_maxbackdated", ConditionOperator.NotNull);

            EntityCollection SetupItems = organizationService.RetrieveMultiple(Query);
            if (SetupItems.Entities.Count > 0)
            {
                foreach (Entity Item in SetupItems.Entities)
                {
                    maxBackDate = Convert.ToInt32(Item.GetAttributeValue<string>("tss_maxbackdated"));
                }
            }

            return maxBackDate;
        }

        public void CheckMaxActivityPerDays(IOrganizationService organizationService, string entityName, Guid id,ITracingService tracer)
        {                  
            var getActivityHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
            var Pss = getActivityHeader.GetAttributeValue<EntityReference>("ownerid").Id;
            //var Pss = getActivityHeader.GetAttributeValue<EntityReference>("systemuser");
            var CreatedOn = getActivityHeader.GetAttributeValue<DateTime>("createdon").Date;

            QueryExpression Query = new QueryExpression(entityName);  //component from quotation
            Query.ColumnSet = new ColumnSet(true);
            Query.Criteria.AddCondition("tss_plansubject", ConditionOperator.NotNull);
            Query.Criteria.AddCondition("tss_pss", ConditionOperator.Equal, Pss);
            Query.Criteria.AddCondition("createdon", ConditionOperator.Equal, CreatedOn);

            tracer.Trace(RetrieveMaxVisitOnSetup(organizationService, tracer).ToString());

            EntityCollection getPartActivityHeader = organizationService.RetrieveMultiple(Query);
            if (getPartActivityHeader.Entities.Count >= RetrieveMaxVisitOnSetup(organizationService,tracer))  //if pss more than spare part setup
            {
                tracer.Trace("while part activity more than equal maxvisit setup");
                throw new Exception("Part Activity more than spart part setup");

                //foreach (Entity Item in SetupItems.Entities)
                //{
                //    
                //} 
            }
            tracer.Trace("successfull");
        }

        public void CheckVisitDateBackDate(IOrganizationService organizationService, string entityName, Guid id, ITracingService tracer)
        {
            //var getActivityHeader = organizationService.Retrieve(entityName, id, new ColumnSet(true));
            //var Pss = getActivityHeader.GetAttributeValue<EntityReference>("ownerid").Id;
            //var Pss = getActivityHeader.GetAttributeValue<EntityReference>("systemuser");

            //DateTime dt = new DateTime(2000, 1, 1);
            //int prevMonth = dt.AddMonths(-1).Month;

            //string dateInString = "01.10.2009";
            //DateTime startDate = DateTime.Parse(dateInString);
            //DateTime expiryDate = startDate.AddDays(RetrieveMaxbackDateOnSetup(organizationService));

            //if (DateTime.Now > expiryDate)
            //{
            //    //... trial expired
            //}

            Entity entity = organizationService.Retrieve(entityName, id, new ColumnSet(true));
            var ActualDate = entity.GetAttributeValue<DateTime>("tss_actualdate");

            DateTime previousMonth = DateTime.Now;
            previousMonth = previousMonth.AddMonths(-1); //get previous month

            tracer.Trace(RetrieveMaxbackDateOnSetup(organizationService).ToString());

            var MaximumDate = previousMonth.Date.AddDays(RetrieveMaxbackDateOnSetup(organizationService));
            tracer.Trace(MaximumDate.ToString()); //check tracing

            if (ActualDate > MaximumDate)
            {
                throw new Exception("PSS only allow to input Actual Date backdated for previous month according Max Backdated on Setup");
            }
            tracer.Trace("sucscesfull");
        }
    }
}
