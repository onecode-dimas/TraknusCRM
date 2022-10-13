using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;
using TrakNusSparepartSystem.Helper;
using CrmEarlyBound;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_KeyAccount
    {
        #region Depedencies

        private DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        private DL_tss_kauio _DL_tss_kauio = new DL_tss_kauio();
        private DL_tss_kagroupuiocommodity _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        private DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();

        RetrieveHelper _retrievehelper = new RetrieveHelper();

        DateTime? nulldatetime = null;
        int? nullint = null;
        decimal? nulldecimal = null;

        #endregion

        public void reviseKeyAccount(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            Entity en_KeyAccount = _DL_tss_keyaccount.Select(organizationService, context.PrimaryEntityId);

            //FilterExpression fSetup = new FilterExpression(LogicalOperator.And);
            //fSetup.AddCondition("tss_isactive", ConditionOperator.Equal, true);

            //QueryExpression qSetup = new QueryExpression("tss_matrixmarketsizeperiod");
            //qSetup.ColumnSet = new ColumnSet(true);
            //qSetup.Criteria.AddFilter(fSetup);
            //EntityCollection setups = organizationService.RetrieveMultiple(qSetup);

            #region update key account active period and status
            //en_KeyAccount["tss_activeenddate"] = en_KeyAccount.GetAttributeValue<DateTime>("tss_activeenddate").ToLocalTime().AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms")).AddDays(-1);
            en_KeyAccount["tss_status"] = new OptionSetValue(865920004);
            en_KeyAccount["tss_reason"] = new OptionSetValue(865920000);
            organizationService.Update(en_KeyAccount);
            #endregion

            #region create new KA, KA UIO and KA UIO Group Commodity

            Guid newKAID;
            Entity new_en_KeyAccount = new Entity(_DL_tss_keyaccount.EntityName);
            //DateTime newActiveStartDate = new_en_KeyAccount.GetAttributeValue<DateTime>("tss_activeenddate").AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));

            GetActivePeriodDate _getactiveperioddate = new GetActivePeriodDate(organizationService);
            _getactiveperioddate.Process();

            DateTime? _startdatemarketsize = _getactiveperioddate.StartDateMarketSize;
            DateTime? _enddatemarketsize = _getactiveperioddate.EndDateMarketSize;

            #region new KA
            //new_en_KeyAccount["tss_msperiodstart"] = en_KeyAccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
            //new_en_KeyAccount["tss_msperiodend"] = en_KeyAccount.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime();
            //new_en_KeyAccount["tss_activestartdate"] = en_KeyAccount.GetAttributeValue<DateTime>("tss_msperiodstart").ToLocalTime().AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
            //new_en_KeyAccount["tss_activeenddate"] = en_KeyAccount.GetAttributeValue<DateTime>("tss_msperiodend").ToLocalTime();

            new_en_KeyAccount["tss_msperiodstart"] = _startdatemarketsize;
            new_en_KeyAccount["tss_msperiodend"] = _enddatemarketsize;
            new_en_KeyAccount["tss_activestartdate"] = _startdatemarketsize;
            new_en_KeyAccount["tss_activeenddate"] = _enddatemarketsize;

            //new_en_KeyAccount["tss_activeenddate"] = en_KeyAccount.GetAttributeValue<DateTime>("tss_msperiodend").AddMonths(setups.Entities[0].GetAttributeValue<int>("tss_evaluationms"));
            new_en_KeyAccount["tss_calculatetoms"] = en_KeyAccount.GetAttributeValue<bool>("tss_calculatetoms");
            new_en_KeyAccount["tss_customer"] = en_KeyAccount.GetAttributeValue<EntityReference>("tss_customer");
            new_en_KeyAccount["tss_funlock"] = en_KeyAccount.GetAttributeValue<EntityReference>("tss_funlock");
            new_en_KeyAccount["tss_pss"] = en_KeyAccount.GetAttributeValue<EntityReference>("tss_pss");
            new_en_KeyAccount["tss_reason"] = null; // en_KeyAccount.GetAttributeValue<OptionSetValue>("tss_reason");
            new_en_KeyAccount["tss_revision"] = en_KeyAccount.GetAttributeValue<int>("tss_revision") + 1;
            new_en_KeyAccount["tss_status"] = new OptionSetValue(865920000);
            new_en_KeyAccount["tss_reason"] = new OptionSetValue(865920005);
            new_en_KeyAccount["tss_version"] = new OptionSetValue(865920001); // en_KeyAccount.GetAttributeValue<OptionSetValue>("tss_version");
            new_en_KeyAccount["tss_isevaluation"] = false;

            newKAID = organizationService.Create(new_en_KeyAccount);

            Entity en_currKA = _DL_tss_keyaccount.Select(organizationService, newKAID);
            en_currKA["tss_kamsid"] = en_KeyAccount.GetAttributeValue<string>("tss_kamsid");
            organizationService.Update(en_currKA);

            #endregion

            #region find prev KA UIO 
            FilterExpression fKAUIO = new FilterExpression(LogicalOperator.And);
            fKAUIO.AddCondition("tss_keyaccountid", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qKAUIO = new QueryExpression(_DL_tss_kauio.EntityName);
            qKAUIO.ColumnSet = new ColumnSet(true);
            qKAUIO.Criteria.AddFilter(fKAUIO);
            EntityCollection ENC_KAUIO = _DL_tss_kauio.Select(organizationService, qKAUIO);
            #endregion

            #region Create New KA UIO
            //if (ENC_KAUIO.Entities.Count > 0)
            //{
            //    Entity new_en_KAUIO = new Entity(_DL_tss_kauio.EntityName);
            //    new_en_KAUIO["tss_calculatestatus"] = ENC_KAUIO[0].GetAttributeValue<Boolean>("tss_calculatestatus");
            //    new_en_KAUIO["tss_calculatetoms"] = ENC_KAUIO[0].GetAttributeValue<Boolean>("tss_calculatetoms");
            //    new_en_KAUIO["tss_customer"] = ENC_KAUIO[0].GetAttributeValue<EntityReference>("tss_customer");
            //    new_en_KAUIO["tss_keyaccountid"] = newKAID;
            //    new_en_KAUIO["tss_name"] = ENC_KAUIO[0].GetAttributeValue<string>("tss_name");
            //    new_en_KAUIO["tss_pss"] = ENC_KAUIO[0].GetAttributeValue<EntityReference>("tss_pss");
            //    new_en_KAUIO["tss_reason"] = ENC_KAUIO[0].GetAttributeValue<OptionSetValue>("tss_reason");
            //    new_en_KAUIO["tss_serialnumber"] = ENC_KAUIO[0].GetAttributeValue<EntityReference>("tss_serialnumber");
            //    organizationService.Create(new_en_KAUIO);
            //}

            for (int i = 0; i < ENC_KAUIO.Entities.Count; i++)
            {
                Entity new_en_KAUIO = new Entity(_DL_tss_kauio.EntityName);

                new_en_KAUIO["tss_calculatestatus"] = null; // ENC_KAUIO[i].GetAttributeValue<Boolean>("tss_calculatestatus");
                new_en_KAUIO["tss_calculatetoms"] = ENC_KAUIO[i].GetAttributeValue<Boolean>("tss_calculatetoms");
                new_en_KAUIO["tss_customer"] = ENC_KAUIO[i].GetAttributeValue<EntityReference>("tss_customer");
                new_en_KAUIO["tss_keyaccountid"] = new EntityReference("tss_keyaccount", newKAID);
                new_en_KAUIO["tss_name"] = ENC_KAUIO[i].GetAttributeValue<string>("tss_name");
                new_en_KAUIO["tss_pss"] = ENC_KAUIO[i].GetAttributeValue<EntityReference>("tss_pss");
                new_en_KAUIO["tss_reason"] = ENC_KAUIO[i].GetAttributeValue<OptionSetValue>("tss_reason");
                new_en_KAUIO["tss_serialnumber"] = ENC_KAUIO[i].GetAttributeValue<EntityReference>("tss_serialnumber");

                new_en_KAUIO["tss_currenthourmeter"] = ENC_KAUIO[i].Attributes.Contains("tss_currenthourmeter") ? ENC_KAUIO[i].GetAttributeValue<decimal>("tss_currenthourmeter") : nulldecimal;
                new_en_KAUIO["tss_currenthourmeterdate"] = ENC_KAUIO[i].Attributes.Contains("tss_currenthourmeterdate") ? ENC_KAUIO[i].GetAttributeValue<DateTime>("tss_currenthourmeterdate").ToLocalTime() : nulldatetime;
                new_en_KAUIO["tss_deliverydate"] = ENC_KAUIO[i].Attributes.Contains("tss_deliverydate") ? ENC_KAUIO[i].GetAttributeValue<DateTime>("tss_deliverydate").ToLocalTime() : nulldatetime;
                new_en_KAUIO["tss_estworkinghour"] = ENC_KAUIO[i].Attributes.Contains("tss_estworkinghour") ? ENC_KAUIO[i].GetAttributeValue<int>("tss_estworkinghour") : nullint;
                new_en_KAUIO["tss_lasthourmeter"] = ENC_KAUIO[i].Attributes.Contains("tss_lasthourmeter") ? ENC_KAUIO[i].GetAttributeValue<decimal>("tss_lasthourmeter") : nulldecimal;
                new_en_KAUIO["tss_lasthourmeterdate"] = ENC_KAUIO[i].Attributes.Contains("tss_lasthourmeterdate") ? ENC_KAUIO[i].GetAttributeValue<DateTime>("tss_lasthourmeterdate").ToLocalTime() : nulldatetime;

                organizationService.Create(new_en_KAUIO);
            }
            #endregion

            #region find prev KA Group UIO Commodity
            FilterExpression fKAUIOGC = new FilterExpression(LogicalOperator.And);
            fKAUIOGC.AddCondition("tss_keyaccountid", ConditionOperator.Equal, context.PrimaryEntityId);

            QueryExpression qKAUIOGC = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
            qKAUIOGC.ColumnSet = new ColumnSet(true);
            qKAUIOGC.Criteria.AddFilter(fKAUIOGC);
            EntityCollection ENC_KAUIOGC = _DL_tss_kagroupuiocommodity.Select(organizationService, qKAUIOGC);

            #endregion

            #region Create New Group KA UIO
            //if (ENC_KAUIOGC.Entities.Count > 0)
            //{
            //    Entity new_en_KAUIOGC = new Entity(_DL_tss_kagroupuiocommodity.EntityName);
            //    new_en_KAUIOGC["tss_calculatestatus"] = ENC_KAUIOGC[0].GetAttributeValue<Boolean>("tss_calculatestatus");
            //    new_en_KAUIOGC["tss_calculatetoms"] = ENC_KAUIOGC[0].GetAttributeValue<Boolean>("tss_calculatetoms");
            //    new_en_KAUIOGC["tss_customer"] = ENC_KAUIOGC[0].GetAttributeValue<EntityReference>("tss_customer");
            //    new_en_KAUIOGC["tss_keyaccountid"] = newKAID;
            //    new_en_KAUIOGC["tss_name"] = ENC_KAUIOGC[0].GetAttributeValue<string>("tss_name");
            //    new_en_KAUIOGC["tss_pss"] = ENC_KAUIOGC[0].GetAttributeValue<EntityReference>("tss_pss");
            //    new_en_KAUIOGC["tss_reason"] = ENC_KAUIOGC[0].GetAttributeValue<OptionSetValue>("tss_reason");
            //    new_en_KAUIOGC["tss_groupuiocommodity"] = ENC_KAUIOGC[0].GetAttributeValue<EntityReference>("tss_groupuiocommodity");
            //    organizationService.Create(new_en_KAUIOGC);
            //}

            for (int i = 0; i < ENC_KAUIOGC.Entities.Count; i++)
            {
                Entity new_en_KAUIOGC = new Entity(_DL_tss_kagroupuiocommodity.EntityName);

                new_en_KAUIOGC["tss_calculatestatus"] = null; // ENC_KAUIOGC[i].GetAttributeValue<Boolean>("tss_calculatestatus");
                new_en_KAUIOGC["tss_calculatetoms"] = ENC_KAUIOGC[i].GetAttributeValue<Boolean>("tss_calculatetoms");
                new_en_KAUIOGC["tss_customer"] = ENC_KAUIOGC[i].GetAttributeValue<EntityReference>("tss_customer");
                new_en_KAUIOGC["tss_keyaccountid"] = new EntityReference("tss_keyaccount", newKAID);
                new_en_KAUIOGC["tss_name"] = ENC_KAUIOGC[i].GetAttributeValue<string>("tss_name");
                new_en_KAUIOGC["tss_pss"] = ENC_KAUIOGC[i].GetAttributeValue<EntityReference>("tss_pss");
                new_en_KAUIOGC["tss_reason"] = ENC_KAUIOGC[i].GetAttributeValue<OptionSetValue>("tss_reason");
                new_en_KAUIOGC["tss_groupuiocommodity"] = ENC_KAUIOGC[i].GetAttributeValue<EntityReference>("tss_groupuiocommodity");

                organizationService.Create(new_en_KAUIOGC);
            }
            #endregion

            //Get User
            Entity EN_User = organizationService.Retrieve("systemuser", en_KeyAccount.GetAttributeValue<EntityReference>("tss_pss").Id, new ColumnSet(true));
            EN_User["tss_marketsizeconfirmed"] = false;
            EN_User["tss_salestargetconfirmed"] = false;
            organizationService.Update(EN_User);

            #endregion
        }

        public void ReviseKeyAccount(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context, EntityCollection entityToRevise)
        {
            foreach (var o in entityToRevise.Entities)
            {
                o.Attributes["tss_status"] = new OptionSetValue(865920000);
                o.Attributes["tss_reason"] = new OptionSetValue(865920000);
                o.Attributes["tss_version"] = new OptionSetValue(865920001); // REVISION
                o.Attributes["tss_revision"] = (o.Attributes.Contains("tss_revision") ? o.GetAttributeValue<int>("tss_revision") : 0) + 1;

                organizationService.Update(o);

                #region KA UIO dan KA Group UIO
                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                _filterexpression.AddCondition("tss_keyaccountid", ConditionOperator.Equal, o.Id);

                QueryExpression _queryexpression = new QueryExpression(_DL_tss_kauio.EntityName);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddFilter(_filterexpression);
                List<Entity> _kauiocollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                foreach (var item in _kauiocollection)
                {
                    tss_kauio _kauio = new tss_kauio();

                    _kauio.Id = item.Id;
                    _kauio.tss_CalculateStatus = false;

                    organizationService.Update(_kauio);
                }

                _queryexpression = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
                _queryexpression.ColumnSet = new ColumnSet(true);
                _queryexpression.Criteria.AddFilter(_filterexpression);
                List<Entity> _kagroupuiocollection = _retrievehelper.RetrieveMultiple(organizationService, _queryexpression);

                foreach (var item in _kagroupuiocollection)
                {
                    tss_kagroupuiocommodity _kagroupuiocommodity = new tss_kagroupuiocommodity();

                    _kagroupuiocommodity.Id = item.Id;
                    _kagroupuiocommodity.tss_CalculateStatus = false;

                    organizationService.Update(_kagroupuiocommodity);
                }
                #endregion


            }
        }
    }
}
