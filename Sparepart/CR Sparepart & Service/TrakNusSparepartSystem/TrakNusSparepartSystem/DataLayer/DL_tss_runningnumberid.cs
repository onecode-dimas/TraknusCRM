using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.DataLayer
{
    public class DL_tss_runningnumberid
    {
        #region Dependencies
        private DL_tss_runningnumber _DL_tss_runningnumber = new DL_tss_runningnumber();
        #endregion

        #region Properties
        private string _classname = "DL_tss_runningnumberid";

        private string _entityname = "tss_runningnumberid";
        public string EntityName
        {
            get { return _entityname; }
        }

        private string _displayname = "Running Number Id";
        public string DisplayName
        {
            get { return _displayname; }
        }

        private bool _tss_name = false;
        private string _tss_name_value;
        public string tss_name
        {
            get { return _tss_name ? _tss_name_value : null; }
            set { _tss_name = true; _tss_name_value = value; }
        }

        private bool _tss_runningnumber = false;
        private EntityReference _tss_runningnumber_value;
        public Guid tss_runningnumber
        {
            get { return _tss_runningnumber ? _tss_runningnumber_value.Id : Guid.Empty; }
            set { _tss_runningnumber = true; _tss_runningnumber_value = new EntityReference(_DL_tss_runningnumber.EntityName, value); }
        }

        private bool _tss_lastnumber = false;
        private Money _tss_lastnumber_value;
        public decimal tss_lastnumber
        {
            get { return _tss_lastnumber ? _tss_lastnumber_value.Value : 0; }
            set { _tss_lastnumber = true; _tss_lastnumber_value = new Money(value); }
        }
        #endregion

        public Entity Select(IOrganizationService organizationService, Guid id)
        {
            try
            {
                return organizationService.Retrieve(_entityname, id, new ColumnSet(true));
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public EntityCollection Select(IOrganizationService organizationService, QueryExpression queryExpression)
        {
            try
            {
                return organizationService.RetrieveMultiple(queryExpression);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Select : " + ex.Message.ToString());
            }
        }

        public void Insert(IOrganizationService organizationService)
        {
            try
            {
                if (_tss_name)
                {
                    Entity entity = new Entity(_entityname);
                    entity.Attributes["tss_name"] = _tss_name_value;
                    if (_tss_runningnumber) { entity.Attributes["tss_runningnumber"] = _tss_runningnumber_value; }
                    if (_tss_lastnumber) { entity.Attributes["tss_lastnumber"] = _tss_lastnumber_value; }
                    organizationService.Create(entity);
                }
                else
                {
                    throw new Exception("Primary Key is empty.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Insert : " + ex.Message.ToString());
            }
        }

        public void Update(IOrganizationService organizationService, Guid id)
        {
            try
            {
                Entity entity = new Entity(_entityname);
                entity.Id = id;
                if (_tss_name) { entity.Attributes["tss_name"] = _tss_name_value; }
                if (_tss_runningnumber) { entity.Attributes["tss_runnningnumber"] = _tss_runningnumber_value; }
                if (_tss_lastnumber) { entity.Attributes["tss_lastnumber"] = _tss_lastnumber_value; }
                organizationService.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".Update : " + ex.Message.ToString());
            }
        }

        decimal LastNumber;
        string Prefix, GuidEntity, EntityPartId;
        public string newRunningIdString(IOrganizationService organizationService, string EntityName, Guid EntityId, ITracingService trace)
        {
            try
            {
                trace.Trace("Reading Data");
                //retreieve  quotation part header
                var entityHeader = organizationService.Retrieve(EntityName, EntityId, new ColumnSet(true));

                //get Prefix from RunningNumber
                QueryExpression Query = new QueryExpression("trs_runningnumber");
                Query.ColumnSet = new ColumnSet(true);
                //Query.Criteria.AddCondition("trs_entityname", ConditionOperator.Equal, QuotationPartHeader.LogicalName);
                Query.Criteria.AddCondition("trs_entityname", ConditionOperator.Equal, EntityName);

                EntityCollection lineItems = organizationService.RetrieveMultiple(Query);
                if (lineItems.Entities.Count > 0)
                {
                    foreach (Entity sItem in lineItems.Entities)
                    {
                        Prefix = sItem.GetAttributeValue<string>("trs_prefix").ToString();   //get prefix
                        trace.Trace(Prefix);

                        //get  LastNumber from RunningNumberId
                        QueryExpression rquery = new QueryExpression("tss_runningnumberid");
                        rquery.ColumnSet = new ColumnSet(true);
                        rquery.Criteria.AddCondition("tss_runningnumber", ConditionOperator.Equal, sItem.Id);

                        EntityCollection Items = organizationService.RetrieveMultiple(rquery);
                        if (Items.Entities.Count > 0)
                        {
                            foreach (Entity Item in Items.Entities)
                            {
                                trace.Trace("getting last number");
                                LastNumber = Item.GetAttributeValue<decimal>("tss_lastnumber");
                                trace.Trace(LastNumber.ToString());

                                GuidEntity = entityHeader.Id.ToString().Substring(0, 6);  //get 6 digit of guid
                                //QuotationId = Prefix + "-" + (000000 + LastNumber + 1) + "-" + GuidEntity;
                                EntityPartId = Prefix + "-" + (Convert.ToInt32(LastNumber) + 1).ToString("00000#") + "-" + GuidEntity.ToUpper();

                                #region Update LastNumber in tss_runningnumberid
                                Entity NumberIdEntity = organizationService.Retrieve("tss_runningnumberid", Item.Id, new ColumnSet(true));
                                //Entity NumberIdEntity = new Entity("tss_runningnumberid");
                                NumberIdEntity.Attributes["tss_lastnumber"] = LastNumber + 1; //increment lastnumber by 1
                                organizationService.Update(NumberIdEntity);
                                #endregion
                            }
                        }
                    }
                }

                #region Check tss_quotationid existing or not
                //var QuotationPartHeader = organizationService.Retrieve(EntityName, EntityId, new ColumnSet(true));

                /*QueryExpression QueryHeader = new QueryExpression("tss_quotationpartheader");
                QueryHeader.ColumnSet = new ColumnSet(true);
                QueryHeader.Criteria.AddCondition("tss_quotationid", ConditionOperator.Contains, Prefix);
                EntityCollection QuotationPartHeader = organizationService.RetrieveMultiple(QueryHeader);
                foreach (var item in QuotationPartHeader.Entities)
                {
                    if (item.GetAttributeValue<string>("tss_quotationid")!=string.Empty)
                    {
                        GuidEntity = item.Id.ToString().Substring(0, 6);  //get 6 digit of guid
                        QuotationId = Prefix + "-" + (000000 + LastNumber + 1) + "-" + GuidEntity;
                    }
                    else
                    {
                        GuidEntity = item.Id.ToString().Substring(0, 6);  //get 6 digit of guid
                        QuotationId = Prefix + "-" + (000000 + LastNumber + 1) + "-" + GuidEntity;
                    }
                }*/

                /*if (QuotationPartHeader.GetAttributeValue<string>("tss_quotationid")))
                {
                    GuidEntity = QuotationPartHeader.Id.ToString().Substring(0, 6);  //get 6 digit of guid
                    QuotationId = Prefix + "-" + (000000 + LastNumber + 1) + "-" + GuidEntity;
                }
                else
                {
                    GuidEntity = QuotationPartHeader.Id.ToString().Substring(0, 6);  //get 6 digit of guid
                    QuotationId = Prefix + "-" + (000000 + LastNumber + 1) + "-" + GuidEntity;
                } */

                //GuidEntity = QuotationPartHeader.Id.ToString().Substring(0, 6);  //get 6 digit of guid
                //QuotationId = Prefix + "-" + (000000 + LastNumber + 1) + "-" + GuidEntity;

                #endregion

                return EntityPartId;
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".newRunningIdString : " + ex.Message.ToString());
            }
        }
    }
}
