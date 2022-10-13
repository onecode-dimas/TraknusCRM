using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.Workflow.BusinessLayer
{
    public class BL_tss_prospectpartheader
    {
        #region Constants
        private const string _classname = "BL_tss_prospectpartheader";

        private const int PIPELINEPHASE_QUOTATION = 865920001;
        private const int STATUSREASON_OPEN = 865920000;
        private const int STATUSREASON_WAITINGAPPROVALDISCOUNT = 865920002;
        private const int STATUSREASON_LOST = 865920007;
        private const int STATUSREASON_WON = 865920008;

        private const int TOP_CBD = 865920000;
        private const int TOP_CAD = 865920001;

        private const int STATUSCODE_DRAFT = 865920000;
        private const int STATUSCODE_INPROGRESS = 865920001;
        private const int ORDERREASON_OVERHAUL = 865920000;
        #endregion

        #region Depedencies
        private DL_tss_quotationpartheader _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private DL_account _DL_account = new DL_account();
        private DL_systemuser _DL_systemuser = new DL_systemuser();
        private DL_businessunit _DL_businessunit = new DL_businessunit();
        private DL_tss_prospectpartheader _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
        private DL_tss_quotationpartlines _DL_tss_quotationpartlines = new DL_tss_quotationpartlines();
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_contact _DL_contact = new DL_contact();
        #endregion

        #region Events
        public void CreateQuotation_OnClick(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                //throw new InvalidWorkflowException("Testing Create Quotation");
                //Created By Julius (Update JS to Workflow)
                var prospect = _DL_tss_prospectpartheader.Select(organizationService, id);
                Guid quotationpartID = Guid.Empty;
                if (prospect.Attributes.Contains("tss_estimationclosedate")) //Check contains date karena minimum date di CRM berbeda
                {
                    #region Create Quotation Part
                    //If New Customer (Flag New Customer on Entity Account), then set Term Of Payment to CBD
                    Entity Account = organizationService.Retrieve("account", prospect.GetAttributeValue<EntityReference>("tss_customer").Id, new ColumnSet(true));
                    bool isnewcustomer = true;
                    if (Account.Contains("tss_newcustomersp")) 
                    {
                        isnewcustomer = Account.GetAttributeValue<bool>("tss_newcustomersp");
                        //throw new InvalidWorkflowException("Customer: " + isnewcustomer);
                    }
                    //throw new InvalidWorkflowException("Testing Create Quotation: " + isnewcustomer);
                    //Create Quotation from Ribbon Button (Prospect Part Header)
                    _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                    //Updated by indra - set top to CBD if true;

                    //#region Unused due to another process by IBIZ [12/07/2018] - Update by Santony [12/07/2018] --> will be use on production (16/7/2018)
                    if (isnewcustomer)
                    {
                        _DL_tss_quotationpartheader.tss_top = TOP_CBD;
                        _DL_tss_quotationpartheader.tss_isnewcustomer = isnewcustomer;
                    }
                    else
                    {
                        _DL_tss_quotationpartheader.tss_top = TOP_CAD;
                        _DL_tss_quotationpartheader.tss_isnewcustomer = isnewcustomer;
                    }
                    //#endregion

                    _DL_tss_quotationpartheader.tss_quotationstatus = STATUSCODE_DRAFT;
                    _DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_OPEN;
                    if (prospect.Attributes.Contains("tss_underminimumprice"))
                    {
                        if (prospect.GetAttributeValue<bool>("tss_underminimumprice"))
                        {
                            _DL_tss_quotationpartheader.tss_underminprice = prospect.GetAttributeValue<bool>("tss_underminimumprice");
                            //_DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_WAITINGAPPROVALDISCOUNT;
                            
                        }
                        //else
                        //{
                        //    _DL_tss_quotationpartheader.tss_statusreason = STATUSREASON_OPEN;
                        //}

                    }
                    if (prospect.Contains("tss_customer"))
                        _DL_tss_quotationpartheader.tss_customer = prospect.GetAttributeValue<EntityReference>("tss_customer").Id;
                    _DL_tss_quotationpartheader.tss_prospectlink = id;
                    _DL_tss_quotationpartheader.tss_estimationclosedate = prospect.GetAttributeValue<DateTime>("tss_estimationclosedate");
                    if (prospect.Contains("tss_contact"))
                        _DL_tss_quotationpartheader.tss_contact = prospect.GetAttributeValue<EntityReference>("tss_contact").Id;
                    if (prospect.Contains("tss_currency"))
                        _DL_tss_quotationpartheader.tss_currency = prospect.GetAttributeValue<EntityReference>("tss_currency").Id;
                    if (prospect.Attributes.Contains("tss_totalamount"))
                    {
                        _DL_tss_quotationpartheader.tss_totalprice = prospect.GetAttributeValue<Money>("tss_totalamount").Value;
                    }
                    else
                    {
                        throw new InvalidWorkflowException("Total Price Amount does not contains on Prospect Part.");
                    }
                    if (prospect.Attributes.Contains("tss_orderreason"))
                    {
                        if (prospect.GetAttributeValue<OptionSetValue>("tss_orderreason").Value == ORDERREASON_OVERHAUL)
                        {
                            _DL_tss_quotationpartheader.tss_unit = prospect.GetAttributeValue<EntityReference>("tss_unit").Id;
                        }
                    }


                    if (prospect.Contains("tss_branch"))
                        _DL_tss_quotationpartheader.tss_branch = prospect.GetAttributeValue<EntityReference>("tss_branch").Id;
                    if (prospect.Contains("tss_pss"))
                        _DL_tss_quotationpartheader.tss_pss = prospect.GetAttributeValue<EntityReference>("tss_pss").Id;
                    if (prospect.Contains("tss_sourcetype"))
                        _DL_tss_quotationpartheader.tss_sourcetype = prospect.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
                    quotationpartID = _DL_tss_quotationpartheader.CreateQuotationFromProspect(organizationService, id);
                    if (quotationpartID == Guid.Empty)
                    {
                        throw new InvalidWorkflowException("Failed to create quotation part!");
                    }

                    #endregion
                    #region Create Quotation Part Lines from Prospect Lines
                    QueryExpression qProspectPartLines = new QueryExpression("tss_prospectpartlines")
                    {
                        ColumnSet = new ColumnSet(true),
                        Criteria = new FilterExpression()
                        {
                            Conditions =
                                {
                                    new ConditionExpression("tss_prospectpartheader",ConditionOperator.Equal,id)
                                }
                        }
                    };
                    var ecProspectPartLines = organizationService.RetrieveMultiple(qProspectPartLines);
                    foreach (var item in ecProspectPartLines.Entities)
                    {
                        _DL_tss_quotationpartlines = new DL_tss_quotationpartlines();
                        _DL_tss_quotationpartlines.tss_quopartheader = quotationpartID;
                        if (item.Contains("tss_sourcetype"))
                            _DL_tss_quotationpartlines.tss_sourcetype = item.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
                        if (item.Contains("tss_partnumber"))
                            _DL_tss_quotationpartlines.tss_partnumber = item.GetAttributeValue<EntityReference>("tss_partnumber").Id;
                        _DL_tss_quotationpartlines.tss_itemnumber = item.GetAttributeValue<int>("tss_itemnumber");
                        _DL_tss_quotationpartlines.tss_partdescription = item.GetAttributeValue<string>("tss_partdescription");
                        if (item.Attributes.Contains("tss_unitgroup") && item.Attributes.Contains("tss_pricetype"))
                        {
                            _DL_tss_quotationpartlines.tss_unitgroup = item.GetAttributeValue<EntityReference>("tss_unitgroup").Id;
                            _DL_tss_quotationpartlines.tss_pricetype = item.GetAttributeValue<EntityReference>("tss_pricetype").Id;
                        }
                        if (item.Attributes.Contains("tss_isinterchange"))
                        {
                            if (item.GetAttributeValue<bool>("tss_isinterchange"))
                            {
                                _DL_tss_quotationpartlines.tss_isinterchange = item.GetAttributeValue<bool>("tss_isinterchange");
                                if (item.GetAttributeValue<EntityReference>("tss_partnumberinterchange") != null)
                                {
                                    _DL_tss_quotationpartlines.tss_partnumberinterchange = item.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id;
                                }
                                else
                                {
                                    throw new InvalidWorkflowException("Cannot created Quotation, because Part Number Interchange has been deleted manually.");
                                }
                                
                            }
                            else
                            {
                                _DL_tss_quotationpartlines.tss_isinterchange = item.GetAttributeValue<bool>("tss_isinterchange");
                            }
                        }
                        _DL_tss_quotationpartlines.tss_qty = item.GetAttributeValue<int>("tss_quantity");
                        if (item.Contains("tss_price"))
                            _DL_tss_quotationpartlines.tss_price = item.GetAttributeValue<Money>("tss_price").Value;
                        if (item.Contains("tss_price") && item.Contains("tss_discountamount"))
                        {
                            decimal afterDisc = item.GetAttributeValue<Money>("tss_price").Value - item.GetAttributeValue<Money>("tss_discountamount").Value;
                            _DL_tss_quotationpartlines.tss_priceafterdisc = afterDisc;
                        }

                        _DL_tss_quotationpartlines.tss_discountby = item.GetAttributeValue<bool>("tss_discountby");
                        if (item.Contains("tss_discountamount"))
                            _DL_tss_quotationpartlines.tss_discamount = item.GetAttributeValue<Money>("tss_discountamount").Value;
                        _DL_tss_quotationpartlines.tss_discpercent = item.GetAttributeValue<decimal>("tss_discountpercent");
                        if (item.Contains("tss_priceamount"))
                            _DL_tss_quotationpartlines.tss_totalprice = item.GetAttributeValue<Money>("tss_priceamount").Value;
                        if (item.Contains("tss_minimumprice"))
                            _DL_tss_quotationpartlines.tss_minprice = item.GetAttributeValue<Money>("tss_minimumprice").Value;
                        _DL_tss_quotationpartlines.tss_underminprice = item.GetAttributeValue<bool>("tss_underminimumprice");
                        //_DL_tss_quotationpartlines.tss_totalfinalprice = afterDisc * item.GetAttributeValue<int>("tss_quantity");
                        _DL_tss_quotationpartlines.CreateQuotationLinesFromProspectLines(organizationService);


                    }
                    #endregion

                    #region Check Reason Discount Package & Indicator
                    DateTime today = DateTime.Now;
                    //Update Prospect Part Header
                    _DL_tss_prospectpartheader = new DL_tss_prospectpartheader();
                    _DL_tss_prospectpartheader.tss_pipelinephase = PIPELINEPHASE_QUOTATION;
                    _DL_tss_prospectpartheader.UpdateStatusReasonAndPipeline(organizationService, id);

                    QueryExpression querygetReasons = new QueryExpression("tss_reason");
                    querygetReasons.ColumnSet = new ColumnSet(true);
                    querygetReasons.Criteria.AddCondition("tss_reason", ConditionOperator.NotNull);
                    querygetReasons.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

                    QueryExpression querygetIndicators = new QueryExpression("tss_indicator");
                    querygetIndicators.ColumnSet = new ColumnSet(true);
                    querygetIndicators.Criteria.AddCondition("tss_indicator", ConditionOperator.NotNull);

                    EntityCollection reasons = organizationService.RetrieveMultiple(querygetReasons);
                    EntityCollection indicators = organizationService.RetrieveMultiple(querygetIndicators);


                    foreach (var reason in reasons.Entities)
                    {
                        Entity quotationreason = new Entity("tss_quotationpartreasondiscountpackage");
                        quotationreason["tss_reason"] = new EntityReference("tss_reason", reason.Id);
                        if (reason.Contains("tss_iscompetitor"))
                            quotationreason["tss_iscompetitor"] = reason.GetAttributeValue<bool>("tss_iscompetitor");
                        quotationreason["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", quotationpartID);
                        quotationreason["tss_result"] = null;
                        organizationService.Create(quotationreason);
                    }

                    foreach (var indicator in indicators.Entities)
                    {
                        Entity quotationindicator = new Entity("tss_quotationpartindicator");
                        quotationindicator["tss_indicator"] = new EntityReference("tss_indicator", indicator.Id);
                        if (indicator.Contains("tss_value"))
                            quotationindicator["tss_value"] = indicator.GetAttributeValue<int>("tss_value");
                        quotationindicator["tss_quotationpartheader"] = new EntityReference("tss_quotationpartheader", quotationpartID);
                        quotationindicator["tss_result"] = null;
                        organizationService.Create(quotationindicator);
                    }
                    #endregion



                    _DL_tss_quotationpartheader = new DL_tss_quotationpartheader();
                    
                    try
                    {
                        QueryExpression querygetContracts = new QueryExpression("contract");
                        querygetContracts.ColumnSet = new ColumnSet(true);
                        querygetContracts.Criteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                        {
                            new ConditionExpression("statecode", ConditionOperator.Equal, 2),
                            new ConditionExpression("trs_customer", ConditionOperator.Equal, prospect.GetAttributeValue<EntityReference>("tss_customer").Id),
                            new ConditionExpression("activeon", ConditionOperator.LessEqual, today),
                            new ConditionExpression("expireson", ConditionOperator.GreaterEqual, today)
                        }
                        };

                        EntityCollection contracs = organizationService.RetrieveMultiple(querygetContracts);
                        if (contracs.Entities.Count > 0)
                        {
                            Entity contract = contracs.Entities.First();
                            if (contract.Contains("trs_otif"))
                            {
                                //Entity Quotationheader = "";
                                int otif = contract.GetAttributeValue<int>("trs_otif");
                                //DateTime delivery = today.AddDays(otif);
                                _DL_tss_quotationpartheader.tss_requestdeliverydate = DateTime.Now.AddDays(otif);
                                updatesLinesRequestDeliveryDate(organizationService, quotationpartID, DateTime.Now.AddDays(otif));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidWorkflowException("Error in get contracts and update request delivery date : " + ex.ToString());
                    }
                    //update quotation
                    //_DL_tss_quotationpartheader.UpdateQuotationFromProspect(organizationService, quotationpartID);
                    
                }
                else
                {
                    throw new InvalidWorkflowException("Please set date Estimation Close Date on the form first !");
                }

                #region Create Quotation by Thomas
                
                // Entity eQuot = organizationService.Retrieve("trs_quotation", id, new ColumnSet(new string[] { "trs_revision", "statuscode" }));
                //int scode = 0;

                //if (eQuot.Attributes.Contains("statuscode") && eQuot.Attributes["statuscode"] != null)
                //{
                //    scode = eQuot.GetAttributeValue<OptionSetValue>("statuscode").Value;
                //}

                ////Validasi Final Approve
                //if (scode != 167630002)
                //{
                //    /* Change by Thomas - 16 March 2015
                //    //Revise
                //    _DL_trs_quotation.statuscode = 167630001;
                //    _DL_trs_quotation.trs_revision = 1;
                //    _DL_trs_quotation.Update(organizationService, id);
                //     * */
                //    //Reset quotation discount header.
                //    //_DL_trs_quotation.Revise(organizationService, id);
                //    Entity quotationUpdateEntity = new Entity(eQuot.LogicalName)
                //    {
                //        Id = eQuot.Id,
                //        Attributes =
                //        {
                //            new KeyValuePair<string, object>("trs_discountheader",false),
                //            new KeyValuePair<string, object>("trs_discountheaderamount",0m)
                //        }
                //    };
                //    organizationService.Update(quotationUpdateEntity);
                //    /* End change by Thomas - 16 March 2015 */
                //}
                //else if (scode == 167630002)
                //{
                //    throw new Exception("Final Approve Can't update status");
                //}
                #endregion                 
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".CreateQuotation_OnClick : " + ex.Message.ToString());
            }
        }

        public void updatesLinesRequestDeliveryDate(IOrganizationService organizationService, Guid QuotationHeader, DateTime ReqDevDate) 
        {
            try 
            {
                QueryExpression querygetContracts = new QueryExpression("tss_quotationpartlines");
                querygetContracts.ColumnSet = new ColumnSet(true);
                querygetContracts.Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                        {
                            new ConditionExpression("tss_quotationpartheader", ConditionOperator.Equal, QuotationHeader)
                        }
                };

                EntityCollection lines = organizationService.RetrieveMultiple(querygetContracts);
                if (lines.Entities.Count > 0)
                {
                    //looping updates every lines field tss_requestdeliverydate
                    foreach (var line in lines.Entities) 
                    {
                        line.Attributes["tss_requestdeliverydate"] = ReqDevDate;
                        organizationService.Update(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".updatesLinesRequestDeliveryDate in create quotation : " + ex.Message.ToString());
            }
        }

        
        #endregion
    }
}
