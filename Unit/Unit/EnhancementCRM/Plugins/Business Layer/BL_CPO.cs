using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Reflection;
using EnhancementCRM.HelperUnit;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_CPO
    {
        #region Constant
        private const string ConditionTypeCode_Incentive = "ZINS";
        #endregion

        #region Properties
        private string _classname = "BL_CPO";
        #endregion

        #region Publics
        public void PostUpdate_CPO_CPOIDSAP(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Entity CPO = _organizationservice.Retrieve(_entity.LogicalName, _entity.Id, new ColumnSet(true));

                if (CPO.Contains("new_cpoidsap") && CPO.Attributes["new_cpoidsap"] != null)
                {
                    QueryExpression qelines = new QueryExpression("salesorderdetail");
                    qelines.ColumnSet = new ColumnSet(true);
                    qelines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, CPO.Id);
                    EntityCollection CPOLines = _organizationservice.RetrieveMultiple(qelines);

                    if (CPOLines.Entities.Count() > 0)
                    {
                        foreach (var CPOLinesItem in CPOLines.Entities)
                        {
                            Entity Product = _organizationservice.Retrieve("product", CPOLinesItem.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));

                            #region Create Incentive
                            Entity Incentives = new Entity("new_incentive");
                            Incentives["new_cpo"] = new EntityReference(_entity.LogicalName, CPO.Id);
                            Incentives["new_name"] = CPO.GetAttributeValue<string>("ordernumber") + " ; " + CPO.GetAttributeValue<EntityReference>("ownerid").Name + " ; " + CPOLinesItem.GetAttributeValue<decimal>("new_itemnumber") + " ; " + Product.GetAttributeValue<String>("productnumber");
                            Incentives["new_cpoitemnumber"] = CPOLinesItem.GetAttributeValue<decimal>("new_itemnumber");

                            #region F1
                            if (Product.Contains("new_salesgroup"))
                            {
                                Entity SalesGroup = _organizationservice.Retrieve("new_salesgroup", Product.GetAttributeValue<EntityReference>("new_salesgroup").Id, new ColumnSet(true));

                                if (SalesGroup.Contains("new_salesgroupcode"))
                                    Incentives["new_productf1"] = SalesGroup.GetAttributeValue<String>("new_salesgroupcode");
                                if (SalesGroup.Contains("new_koefisien"))
                                    Incentives["new_koefisienf1"] = SalesGroup.GetAttributeValue<Decimal>("new_koefisien");
                                else
                                    Incentives["new_koefisienf1"] = new decimal(0);

                                #region F2
                                if (Product.Contains("new_materialgroup"))
                                {
                                    Entity MG = _organizationservice.Retrieve("new_materialgroup", Product.GetAttributeValue<EntityReference>("new_materialgroup").Id, new ColumnSet(true));

                                    if (MG.Contains("new_mgcode"))
                                    {
                                        Incentives["new_modelcategoryf2"] = MG.GetAttributeValue<String>("new_mgcode");

                                        QueryExpression qeF2 = new QueryExpression("new_f2incentiveparameter");
                                        qeF2.ColumnSet = new ColumnSet(true);
                                        qeF2.Criteria.AddCondition("new_name", ConditionOperator.Equal, MG.GetAttributeValue<String>("new_mgcode"));
                                        qeF2.Criteria.AddCondition("new_salesgroupproduct", ConditionOperator.Equal, Product.GetAttributeValue<EntityReference>("new_salesgroup").Id);
                                        EntityCollection F2 = _organizationservice.RetrieveMultiple(qeF2);

                                        if (F2.Entities.Count > 0)
                                        {
                                            Entity F2Parameter = F2.Entities[0];
                                                
                                            if (F2Parameter.Contains("new_koefisien"))
                                                Incentives["new_koefisienf2"] = F2Parameter.GetAttributeValue<Decimal>("new_koefisien");
                                            else
                                                Incentives["new_koefisienf2"] = new decimal(0);
                                        }
                                        else
                                            Incentives["new_koefisienf2"] = new decimal(0);
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region F3
                            Entity BC = _organizationservice.Retrieve("systemuser", CPO.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));

                            if (BC.Contains("new_grade"))
                            {
                                Entity F3Parameter = _organizationservice.Retrieve("new_f3incentiveparameter", BC.GetAttributeValue<EntityReference>("new_grade").Id, new ColumnSet(true));

                                if (F3Parameter.Contains("new_name"))
                                    Incentives["new_salesforcegradef3"] = F3Parameter.GetAttributeValue<String>("new_name");
                                if (F3Parameter.Contains("new_koefisien"))
                                    Incentives["new_koefisienf3"] = F3Parameter.GetAttributeValue<Decimal>("new_koefisien");
                                else
                                    Incentives["new_koefisienf3"] = new decimal(0);
                            }
                            #endregion

                            #region F6
                            Entity Customer = _organizationservice.Retrieve("account", CPO.GetAttributeValue<EntityReference>("customerid").Id, new ColumnSet(true));

                            if (Customer.Contains("new_customercategory"))
                            {
                                Entity F6Parameter = _organizationservice.Retrieve("new_f6incentiveparameter", Customer.GetAttributeValue<EntityReference>("new_customercategory").Id, new ColumnSet(true));

                                if (F6Parameter.Contains("new_name"))
                                    Incentives["new_custcategoryf6"] = F6Parameter.GetAttributeValue<String>("new_name");
                                if (F6Parameter.Contains("new_name"))
                                    Incentives["new_koefisienf6"] = F6Parameter.GetAttributeValue<Decimal>("new_koefisien");
                                else
                                    Incentives["new_koefisienf6"] = new decimal(0);
                            }
                            #endregion

                            #region F7
                            DateTime CreatedOn = CPO.GetAttributeValue<DateTime>("createdon");
                            string CreatedYear = CreatedOn.Year.ToString("YYYY");
                            int CreatedMonth = CreatedOn.Month;
                            decimal KoefisienF7 = 0;
                            int HistoricalSum = 0;

                            for (int i = 3; i == 0; i--)
                            {
                                DateTime CreatedDateMin = CPO.GetAttributeValue<DateTime>("createdon").AddMonths(-i);
                                string CreatedYearMin = CreatedDateMin.Year.ToString("YYYY");
                                int CreatedMonthMin = CreatedDateMin.Month;

                                QueryExpression _queryexpression = new QueryExpression("ittn_historicalf7");
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("ittn_bc", ConditionOperator.Equal, BC.Id);
                                _queryexpression.Criteria.AddCondition("ittn_year", ConditionOperator.Equal, CreatedYearMin);
                                _queryexpression.Criteria.AddCondition("ittn_month", ConditionOperator.Equal, CreatedMonthMin);

                                Entity _historicalf7 = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                if (_historicalf7 != null)
                                {
                                    KoefisienF7 += _historicalf7.GetAttributeValue<decimal>("ittn_f7");
                                    HistoricalSum += 1;
                                }
                            }

                            if (HistoricalSum > 0)
                                KoefisienF7 = KoefisienF7 / HistoricalSum;

                            //Incentives["new_datamanagementf7"] = ;
                            Incentives["new_koefisienf7"] = KoefisienF7;
                            #endregion

                            #region F8
                            decimal ExtendedAmount = CPOLinesItem.GetAttributeValue<Money>("ittn_totalextendedamount").Value;
                            decimal MinimumPrice = CPOLinesItem.Contains("new_minimumprice") ? CPOLinesItem.GetAttributeValue<Money>("new_minimumprice").Value : 0;
                            decimal KoefisienF8 = 0;
                            string OverUnderPrice = "";

                            if (MinimumPrice > 0)
                            {
                                decimal PercentagePerMinimumPrice = ExtendedAmount / MinimumPrice * 100;

                                QueryExpression _queryexpression = new QueryExpression("new_f8incentiveparameter");
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("new_start", ConditionOperator.LessEqual, PercentagePerMinimumPrice);
                                _queryexpression.Criteria.AddCondition("new_end", ConditionOperator.GreaterThan, PercentagePerMinimumPrice);

                                Entity _f8incentiveparameter = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                if (_f8incentiveparameter != null)
                                {
                                    OverUnderPrice = _f8incentiveparameter.GetAttributeValue<string>("new_name");
                                    KoefisienF8 = _f8incentiveparameter.GetAttributeValue<decimal>("new_koefisien");
                                }
                            }

                            Incentives["new_overunderpricef8"] = OverUnderPrice;
                            Incentives["new_koefisienf8"] = KoefisienF8;
                            #endregion

                            #region GROSS ( F9 )
                            decimal TaxAmount = (ExtendedAmount * 10 / 100);
                            decimal GrossSellingPrice = ExtendedAmount + TaxAmount;

                            Incentives["new_grosssellingprice"] = new Money(GrossSellingPrice);
                            #endregion

                            #region KOEFISIEN TOTAL
                            decimal DefaultKoefiesien = 0;
                            decimal KoefisienTotal =
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf1") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf1") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf2") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf2") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf3") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf3") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf4") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf4") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf5") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf5") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf6") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf6") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf7") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf7") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf8") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf8") : DefaultKoefiesien);

                            Incentives["new_koefisientotal"] = KoefisienTotal;
                            #endregion

                            #region KOEFISIEN TOTAL
                            string TotalIncentiveFactor =
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf1") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf1").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf2") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf2").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf3") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf3").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf4") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf4").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf5") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf5").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf6") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf6").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf7") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf7").ToString() : DefaultKoefiesien.ToString()) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf8") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf8").ToString() : DefaultKoefiesien.ToString());

                            Incentives["new_totalincentivefactorf1xf2xf3xf4xf5xf6xf7x"] = TotalIncentiveFactor;
                            #endregion

                            _organizationservice.Create(Incentives);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostUpdate_CPO_CPOIDSAP: " + ex.Message.ToString());
            }
        }

        public void PostCreate_CPO_CPOIDSAP(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                Entity CPO = _organizationservice.Retrieve(_entity.LogicalName, _entity.Id, new ColumnSet(true));

                QueryExpression qelines = new QueryExpression("salesorderdetail");
                qelines.ColumnSet = new ColumnSet(true);
                qelines.Criteria.AddCondition("salesorderid", ConditionOperator.Equal, CPO.Id);
                qelines.Criteria.AddCondition("new_parentnumber", ConditionOperator.Null);
                EntityCollection CPOLines = _organizationservice.RetrieveMultiple(qelines);

                if (CPOLines.Entities.Count() > 0)
                {
                    foreach (var CPOLinesItem in CPOLines.Entities)
                    {
                        QueryExpression incentives = new QueryExpression("new_incentive");
                        incentives.ColumnSet = new ColumnSet(true);
                        incentives.Criteria.AddCondition("new_cpo", ConditionOperator.Equal, CPO.Id);
                        incentives.Criteria.AddCondition("new_cpoitemnumber", ConditionOperator.Equal, CPOLinesItem.GetAttributeValue<decimal>("new_itemnumber"));
                        EntityCollection ec = _organizationservice.RetrieveMultiple(incentives);

                        if (ec.Entities.Count() == 0)
                        {
                            Entity Product = _organizationservice.Retrieve("product", CPOLinesItem.GetAttributeValue<EntityReference>("productid").Id, new ColumnSet(true));

                            #region Create Incentive
                            Entity Incentives = new Entity("new_incentive");
                            Incentives["new_cpo"] = new EntityReference(_entity.LogicalName, CPO.Id);
                            Incentives["new_name"] = CPO.GetAttributeValue<string>("ordernumber") + " ; " + CPO.GetAttributeValue<EntityReference>("ownerid").Name + " ; " + CPOLinesItem.GetAttributeValue<decimal>("new_itemnumber") + " ; " + Product.GetAttributeValue<String>("productnumber");
                            Incentives["new_cpoitemnumber"] = CPOLinesItem.GetAttributeValue<decimal>("new_itemnumber");

                            #region F1
                            if (Product.Contains("new_salesgroup"))
                            {
                                Entity SalesGroup = _organizationservice.Retrieve("new_salesgroup", Product.GetAttributeValue<EntityReference>("new_salesgroup").Id, new ColumnSet(true));

                                if (SalesGroup.Contains("new_salesgroupcode"))
                                    Incentives["new_productf1"] = SalesGroup.GetAttributeValue<String>("new_salesgroupcode");
                                if (SalesGroup.Contains("new_koefisien"))
                                    Incentives["new_koefisienf1"] = SalesGroup.GetAttributeValue<Decimal>("new_koefisien");
                                else
                                    Incentives["new_koefisienf1"] = new decimal(0);

                                #region F2
                                if (Product.Contains("new_category"))
                                {
                                    Entity MG = _organizationservice.Retrieve("new_f2incentiveparameter", Product.GetAttributeValue<EntityReference>("new_category").Id, new ColumnSet(true));

                                    Incentives["new_modelcategoryf2"] = MG.GetAttributeValue<String>("new_name");

                                    if (MG.Contains("new_koefisien"))
                                        Incentives["new_koefisienf2"] = MG.GetAttributeValue<Decimal>("new_koefisien");
                                    else
                                        Incentives["new_koefisienf2"] = new decimal(0);
                                }
                            }
                            else
                                Incentives["new_koefisienf2"] = new decimal(0);
                            #endregion
                            #endregion

                            #region F3
                            Entity BC = _organizationservice.Retrieve("systemuser", CPO.GetAttributeValue<EntityReference>("ownerid").Id, new ColumnSet(true));

                            if (BC.Contains("new_grade"))
                            {
                                Entity F3Parameter = _organizationservice.Retrieve("new_f3incentiveparameter", BC.GetAttributeValue<EntityReference>("new_grade").Id, new ColumnSet(true));

                                if (F3Parameter.Contains("new_name"))
                                    Incentives["new_salesforcegradef3"] = F3Parameter.GetAttributeValue<String>("new_name");
                                if (F3Parameter.Contains("new_koefisien"))
                                    Incentives["new_koefisienf3"] = F3Parameter.GetAttributeValue<Decimal>("new_koefisien");
                                else
                                    Incentives["new_koefisienf3"] = new decimal(0);
                            }
                            #endregion

                            #region Default F4 & F5
                            Incentives["new_koefisienf4"] = new decimal(1);
                            Incentives["new_koefisienf5"] = new decimal(1);

                            #region Get Term of Payment on Quote
                            Entity Quote = _organizationservice.Retrieve("quote", CPO.GetAttributeValue<EntityReference>("quoteid").Id, new ColumnSet(true));
                            bool TermOfPayment = Quote.GetAttributeValue<bool>("new_termofpayment");
                            string TermofPaymentText = string.Empty;

                            if (TermOfPayment == false)
                                TermofPaymentText = "CBD";
                            else
                                TermofPaymentText = "CAD";

                            Incentives["new_termofpaymentf4"] = TermofPaymentText;
                            #endregion
                            //Incentives["new_koefisienf4default"] = new decimal(1);
                            //Incentives["new_koefisienf5default"] = new decimal(1);
                            #endregion

                            #region F6
                            Entity Customer = _organizationservice.Retrieve("account", CPO.GetAttributeValue<EntityReference>("customerid").Id, new ColumnSet(true));

                            if (Customer.Contains("new_customercategory"))
                            {
                                Entity F6Parameter = _organizationservice.Retrieve("new_f6incentiveparameter", Customer.GetAttributeValue<EntityReference>("new_customercategory").Id, new ColumnSet(true));

                                if (F6Parameter.Contains("new_name"))
                                    Incentives["new_custcategoryf6"] = F6Parameter.GetAttributeValue<String>("new_name");
                                if (F6Parameter.Contains("new_name"))
                                    Incentives["new_koefisienf6"] = F6Parameter.GetAttributeValue<Decimal>("new_koefisien");
                                else
                                    Incentives["new_koefisienf6"] = new decimal(0);
                            }
                            #endregion

                            #region F7
                            DateTime CreatedOn = CPO.GetAttributeValue<DateTime>("createdon");
                            string CreatedYear = CreatedOn.Year.ToString(); // .ToString("YYYY");
                            int CreatedMonth = CreatedOn.Month;
                            decimal KoefisienF7 = 0;
                            int KoefisienF7Count = 0;
                            int BackMonth = 3;

                            for (int i = 1; i <= BackMonth; i++)
                            {
                                DateTime CreatedDateMin = CPO.GetAttributeValue<DateTime>("createdon").AddMonths(-i);
                                string CreatedYearMin = CreatedDateMin.Year.ToString(); //.ToString("YYYY");
                                int CreatedMonthMin = CreatedDateMin.Month;

                                //throw new InvalidPluginExecutionException(CreatedYearMin + " - " + CreatedMonthMin.ToString());

                                QueryExpression _queryexpression = new QueryExpression("ittn_historicalf7");
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("ittn_bc", ConditionOperator.Equal, BC.Id);
                                _queryexpression.Criteria.AddCondition("ittn_year", ConditionOperator.Equal, CreatedYearMin);
                                _queryexpression.Criteria.AddCondition("ittn_month", ConditionOperator.Equal, CreatedMonthMin);

                                Entity _historicalf7 = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                if (_historicalf7 != null)
                                {
                                    KoefisienF7 += _historicalf7.GetAttributeValue<decimal>("ittn_f7");
                                    KoefisienF7Count += 1;
                                }
                            }

                            if (KoefisienF7Count == BackMonth)
                            {
                                KoefisienF7 = KoefisienF7 / KoefisienF7Count;
                            }
                            else
                            {
                                KoefisienF7 = 0;
                            }

                            //decimal KoefisienF7 = 3;
                            //int HistoricalSum = 2;

                            //for (int i = 3; i == 0; i--)
                            //{
                            //    DateTime CreatedDateMin = CPO.GetAttributeValue<DateTime>("createdon").AddMonths(-i);
                            //    string CreatedYearMin = CreatedDateMin.Year.ToString("YYYY");
                            //    int CreatedMonthMin = CreatedDateMin.Month;

                            //    QueryExpression _queryexpression = new QueryExpression("ittn_historicalf7");
                            //    _queryexpression.ColumnSet = new ColumnSet(true);
                            //    _queryexpression.Criteria.AddCondition("ittn_bc", ConditionOperator.Equal, BC.Id);
                            //    _queryexpression.Criteria.AddCondition("ittn_year", ConditionOperator.Equal, CreatedYearMin);
                            //    _queryexpression.Criteria.AddCondition("ittn_month", ConditionOperator.Equal, CreatedMonthMin);

                            //    Entity _historicalf7 = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                            //    if (_historicalf7 != null)
                            //    {
                            //        KoefisienF7 += _historicalf7.GetAttributeValue<decimal>("ittn_f7");
                            //        HistoricalSum += 1;
                            //    }
                            //}

                            //if (HistoricalSum > 0)
                            //    KoefisienF7 = KoefisienF7 / HistoricalSum;

                            //Incentives["new_datamanagementf7"] = ;
                            Incentives["new_koefisienf7"] = KoefisienF7;
                            #endregion

                            #region F8
                            decimal ExtendedAmount = CPOLinesItem.GetAttributeValue<Money>("ittn_totalextendedamount").Value;
                            decimal MinimumPrice = CPOLinesItem.Contains("new_minimumprice") ? CPOLinesItem.GetAttributeValue<Money>("new_minimumprice").Value : 0;
                            decimal KoefisienF8 = 0;
                            string OverUnderPrice = "";

                            if (MinimumPrice > 0)
                            {
                                decimal PercentagePerMinimumPrice = ExtendedAmount / MinimumPrice * 100;

                                QueryExpression _queryexpression = new QueryExpression("new_f8incentiveparameter");
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("new_start", ConditionOperator.LessEqual, PercentagePerMinimumPrice);
                                _queryexpression.Criteria.AddCondition("new_end", ConditionOperator.GreaterThan, PercentagePerMinimumPrice);

                                Entity _f8incentiveparameter = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                                if (_f8incentiveparameter != null)
                                {
                                    OverUnderPrice = _f8incentiveparameter.GetAttributeValue<string>("new_name");
                                    KoefisienF8 = _f8incentiveparameter.GetAttributeValue<decimal>("new_koefisien");
                                }
                            }

                            Incentives["new_overunderpricef8"] = OverUnderPrice;
                            Incentives["new_koefisienf8"] = KoefisienF8;
                            #endregion

                            #region GROSS ( F9 )
                            decimal TaxAmount = (ExtendedAmount * 10 / 100);
                            decimal GrossSellingPrice = ExtendedAmount + TaxAmount;

                            Incentives["new_netsalesprice"] = new Money(ExtendedAmount);
                            Incentives["new_vat10"] = new Money(TaxAmount);
                            Incentives["new_grosssellingprice"] = new Money(GrossSellingPrice);
                            #endregion

                            #region KOEFISIEN TOTAL
                            decimal DefaultKoefiesien = 1;
                            decimal KoefisienTotal =
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf1") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf1") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf2") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf2") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf3") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf3") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf4") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf4") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf5") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf5") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf6") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf6") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf7") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf7") : DefaultKoefiesien) *
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf8") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf8") : DefaultKoefiesien);

                            Incentives["new_koefisientotal"] = KoefisienTotal;
                            #endregion

                            #region KOEFISIEN TOTAL
                            string TotalIncentiveFactor =
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf1") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf1").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf2") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf2").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf3") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf3").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf4") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf4").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf5") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf5").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf6") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf6").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf7") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf7").ToString("0.00") : DefaultKoefiesien.ToString("0.00")) + " x " +
                                (Incentives.GetAttributeValue<decimal>("new_koefisienf8") > 0 ? Incentives.GetAttributeValue<decimal>("new_koefisienf8").ToString("0.00") : DefaultKoefiesien.ToString("0.00"));

                            Incentives["new_totalincentivefactorf1xf2xf3xf4xf5xf6xf7x"] = TotalIncentiveFactor;
                            #endregion

                            _organizationservice.Create(Incentives);
                            #endregion

                            #region Create CPO Condition Type for Incentive
                            QueryExpression qecondtype = new QueryExpression("ittn_masterconditiontype");
                            qecondtype.ColumnSet = new ColumnSet(true);
                            qecondtype.Criteria.AddCondition("ittn_code", ConditionOperator.Equal, ConditionTypeCode_Incentive);
                            Entity ecc = _organizationservice.RetrieveMultiple(qecondtype).Entities.FirstOrDefault();

                            if (ecc != null)
                            {
                                Entity CPOCondType = new Entity("ittn_cpoconditiontype");
                                CPOCondType["ittn_cpo"] = new EntityReference("salesorder", CPO.Id);
                                CPOCondType["ittn_itemnumber"] = CPOLinesItem.GetAttributeValue<decimal>("new_itemnumber");
                                CPOCondType["ittn_conditiontype"] = new EntityReference("ittn_masterconditiontype", ecc.Id);
                                CPOCondType["ittn_name"] = ecc.GetAttributeValue<String>("ittn_description");
                                CPOCondType["ittn_amount"] = new Money(CPOLinesItem.GetAttributeValue<Money>("ittn_totalextendedamount").Value * KoefisienTotal / 100);
                                _organizationservice.Create(CPOCondType);
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException("Cannot found Master Condition Type for Incentives !");
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_CPO_CPOIDSAP: " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
