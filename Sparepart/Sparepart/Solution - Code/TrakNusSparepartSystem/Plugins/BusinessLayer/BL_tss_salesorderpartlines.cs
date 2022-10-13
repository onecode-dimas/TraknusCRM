using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

namespace TrakNusSparepartSystem.Plugins.BusinessLayer
{
    public class BL_tss_salesorderpartlines
    {
        #region Constants
        private const string _classname = "BL_tss_salesorderpartlines";
        private const int _depth = 1;
        #endregion

        #region Depedencies
        private DL_tss_salesorderpartheader _DL_tss_salesorderpartheader = new DL_tss_salesorderpartheader();
        private DL_tss_salesorderpartlines _DL_tss_salesorderpartlines = new DL_tss_salesorderpartlines();
        #endregion

        public void setPrice(IOrganizationService organizationService, Entity now)
        {
            try
            {
                var context = new OrganizationServiceContext(organizationService);

                if (now.Attributes.Contains("tss_sopartheaderid") && now.Attributes["tss_sopartheaderid"] != null)
                {
                    var header = (from c in context.CreateQuery("tss_sopartheader")
                                  where c.GetAttributeValue<Guid>("tss_sopartheaderid") == now.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id
                                  select c).ToList();

                    if (header.Count > 0)
                    {
                        if (!header[0].Attributes.Contains("tss_quotationlink") || header[0].Attributes["tss_quotationlink"] == null)
                        {
                            EntityReference customer = new EntityReference();
                            if (header[0].Attributes.Contains("tss_customer"))
                            {
                                customer = header[0].GetAttributeValue<EntityReference>("tss_customer");
                            }
                            Guid pricelistpartid = Guid.Empty;
                            var pricelistpart = (from c in context.CreateQuery("tss_pricelistpart")
                                                 where c.GetAttributeValue<string>("tss_name") == "Direct Sales Price List"
                                                 select c).ToList();
                            if (pricelistpart.Count > 0)
                            {
                                pricelistpartid = pricelistpart[0].Id;
                            }

                            Guid dealerheaderid = Guid.Empty;
                            if (customer.Id != Guid.Empty && pricelistpartid != Guid.Empty && now.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value == 865920004)
                            {
                                var dealerheader = (from c in context.CreateQuery("tss_dealerheader")
                                                    where c.GetAttributeValue<EntityReference>("tss_dealername").Id == customer.Id
                                                    where c.GetAttributeValue<EntityReference>("tss_pricelist").Id == pricelistpartid
                                                    select c).ToList();

                                for (int i = 0; i < dealerheader.Count; i++)
                                {
                                    if (dealerheader[i].GetAttributeValue<DateTime>("tss_startdate").ToLocalTime() <= DateTime.Now.ToLocalTime() && dealerheader[i].GetAttributeValue<DateTime>("tss_enddate").ToLocalTime() >= DateTime.Now.ToLocalTime())
                                    {
                                        dealerheaderid = dealerheader[i].Id;
                                        break;
                                    }
                                }
                            }

                            bool discBy = false;
                            int discPercent = 0;
                            decimal discAmount = 0;
                            if (dealerheaderid != Guid.Empty && now.Attributes.Contains("tss_unitgroup"))
                            {
                                var dealerline = (from c in context.CreateQuery("tss_dealerlines")
                                                  where c.GetAttributeValue<EntityReference>("tss_dealerheaderid").Id == dealerheaderid
                                                  where c.GetAttributeValue<EntityReference>("tss_materialgroup").Id == now.GetAttributeValue<EntityReference>("tss_unitgroup").Id
                                                  select c).ToList();
                                if(dealerline.Count > 0)
                                {
                                    if (dealerline[0].Attributes.Contains("tss_discountby"))
                                    {
                                        discBy = dealerline[0].GetAttributeValue<bool>("tss_discountby");
                                    }
                                    if (dealerline[0].Attributes.Contains("tss_discountpercent"))
                                    {
                                        discPercent = dealerline[0].GetAttributeValue<int>("tss_discountpercent");
                                    }
                                    if (dealerline[0].Attributes.Contains("tss_discountamount"))
                                    {
                                        discAmount = dealerline[0].GetAttributeValue<Money>("tss_discountamount").Value;
                                    }
                                }
                            }

                            if (now.Attributes.Contains("tss_isinterchange") && now.Attributes["tss_isinterchange"] != null)
                            {
                                if (!now.GetAttributeValue<bool>("tss_isinterchange") && now.Attributes.Contains("tss_partnumber") && now.Attributes["tss_partnumber"] != null)
                                {
                                    var price = (from c in context.CreateQuery("tss_sparepartpricemaster")
                                                 where c.GetAttributeValue<EntityReference>("tss_partmaster").Id == now.GetAttributeValue<EntityReference>("tss_partnumber").Id
                                                 where c.GetAttributeValue<EntityReference>("tss_pricelistpart").Id == pricelistpartid
                                                 select c).ToList();

                                    if (price.Count > 0 && price[0].Attributes.Contains("tss_price") && price[0].Attributes["tss_price"] != null)
                                    {
                                        decimal finalPrice = price[0].GetAttributeValue<Money>("tss_price").Value;
                                        Entity ent = new Entity("tss_sopartlines");
                                        ent.Id = now.Id;
                                        if (discBy == true && discAmount > 0)
                                        {
                                            finalPrice = price[0].GetAttributeValue<Money>("tss_price").Value - discAmount;
                                        }
                                        else if (discBy == false && discPercent > 0)
                                        {
                                            finalPrice = (price[0].GetAttributeValue<Money>("tss_price").Value * (100 - discPercent)) / 100;
                                        }
                                        else
                                        {
                                            finalPrice = price[0].GetAttributeValue<Money>("tss_price").Value;
                                        }

                                        if (finalPrice < 0) finalPrice = 0;
                                        ent.Attributes["tss_finalprice"] = new Money(finalPrice);

                                        if (now.Attributes.Contains("tss_qtyrequest") && now.Attributes["tss_qtyrequest"] != null)
                                        {
                                            ent.Attributes["tss_totalprice"] = new Money(now.GetAttributeValue<int>("tss_qtyrequest") * finalPrice);
                                        }
                                        organizationService.Update(ent);
                                    }
                                }
                                else if (now.GetAttributeValue<bool>("tss_isinterchange") && now.Attributes.Contains("tss_partnumberinterchange") && now.Attributes["tss_partnumberinterchange"] != null)
                                {
                                    //select tss_partnumberinterchange,* from tss_partmasterlinesinterchange
                                    var partnumber = (from c in context.CreateQuery("tss_partmasterlinesinterchange")
                                                      where c.GetAttributeValue<Guid>("tss_partmasterlinesinterchangeid") == now.GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id
                                                      select c).ToList();

                                    if (partnumber.Count > 0 && partnumber[0].Attributes.Contains("tss_partnumberinterchange") && partnumber[0].Attributes["tss_partnumberinterchange"] != null)
                                    {
                                        var price = (from c in context.CreateQuery("tss_sparepartpricemaster")
                                                     where c.GetAttributeValue<EntityReference>("tss_partmaster").Id == partnumber[0].GetAttributeValue<EntityReference>("tss_partnumberinterchange").Id
                                                     where c.GetAttributeValue<EntityReference>("tss_pricelistpart").Id == pricelistpartid
                                                     select c).ToList();

                                        if (price.Count > 0 && price[0].Attributes.Contains("tss_price") && price[0].Attributes["tss_price"] != null)
                                        {
                                            decimal finalPrice = 0;
                                            Entity ent = new Entity("tss_sopartlines");
                                            ent.Id = now.Id;
                                            if (discBy == true && discAmount > 0)
                                            {
                                                finalPrice = price[0].GetAttributeValue<Money>("tss_price").Value - discAmount;
                                            }
                                            else if (discBy == false && discPercent > 0)
                                            {
                                                finalPrice = (price[0].GetAttributeValue<Money>("tss_price").Value * (100 - discPercent)) / 100;
                                            }
                                            else
                                            {
                                                finalPrice = price[0].GetAttributeValue<Money>("tss_price").Value;
                                            }

                                            if (finalPrice < 0) finalPrice = 0;
                                            ent.Attributes["tss_finalprice"] = new Money(finalPrice);

                                            if (now.Attributes.Contains("tss_qtyrequest") && now.Attributes["tss_qtyrequest"] != null)
                                            {
                                                ent.Attributes["tss_totalprice"] = new Money(now.GetAttributeValue<int>("tss_qtyrequest") * finalPrice);
                                            }
                                            organizationService.Update(ent);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateTotalPrice(IOrganizationService organizationService, Entity now)
        {
            try
            {
                var context = new OrganizationServiceContext(organizationService);

                Entity ent = new Entity("tss_sopartlines");
                ent.Id = now.Id;

                //var sopartline = (from c in context.CreateQuery("tss_sopartlines")
                //                  where c.GetAttributeValue<EntityReference>("tss_sopartlinesid").Id == now.Id
                //                  select c).ToList();

                //var totalprice = sopartline[0].GetAttributeValue<Money>("tss_priceafterdiscount").Value * sopartline[0].GetAttributeValue<int>("tss_qtyrequest");
                if (now.Attributes.Contains("tss_priceafterdiscount") && now.Attributes["tss_priceafterdiscount"] != null)
                {
                    var totalprice = now.GetAttributeValue<Money>("tss_priceafterdiscount").Value * now.GetAttributeValue<int>("tss_qtyrequest");

                    ent.Attributes["tss_totalprice"] = new Money(totalprice);

                    organizationService.Update(ent);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Forms Event
        public void Form_OnCreate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                if (entity.LogicalName == _DL_tss_salesorderpartlines.EntityName)
                {
                    if(entity.Attributes.Contains("tss_sopartheaderid") && entity.Attributes["tss_sopartheaderid"] != null)
                    {
                        Guid headerId = entity.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id;
                        setPrice(organizationService, entity);
                        UpdateTotalPrice(organizationService, entity);
                        _DL_tss_salesorderpartheader.UpdateTotalAmount(organizationService, headerId);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnUpdate_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity entityToUpdate = organizationService.Retrieve(pluginExcecutionContext.PrimaryEntityName, pluginExcecutionContext.PrimaryEntityId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity.LogicalName == _DL_tss_salesorderpartlines.EntityName)
                {
                    var context = new OrganizationServiceContext(organizationService);
                    var line = (from c in context.CreateQuery(_DL_tss_salesorderpartlines.EntityName)
                                where c.GetAttributeValue<Guid>("tss_sopartlinesid") == entity.Id
                                select c).ToList();

                    if (line.Count > 0)
                    {
                        Guid headerId = line[0].GetAttributeValue<EntityReference>("tss_sopartheaderid").Id;
                        _DL_tss_salesorderpartheader.UpdateTotalAmount(organizationService, headerId);
                    }
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

        public void Form_OnUpdateStatus_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.InputParameters["Target"];
                Entity entityToUpdate = organizationService.Retrieve(pluginExcecutionContext.PrimaryEntityName, pluginExcecutionContext.PrimaryEntityId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (entity.LogicalName == _DL_tss_salesorderpartlines.EntityName)
                {
                    var context = new OrganizationServiceContext(organizationService);
                    var line = (from c in context.CreateQuery(_DL_tss_salesorderpartlines.EntityName)
                                where c.GetAttributeValue<Guid>("tss_sopartlinesid") == entity.Id
                                select c).ToList();

                    if (line.Count > 0)
                    {
                        if (entityToUpdate.GetAttributeValue<OptionSetValue>("tss_status").Value == 865920003)
                        {
                            // 2019.02.15 - UPDATE REMAINING QTY POTENTIAL PROSPECT PART SUBLINES DAN REMAINING QTY TOTAL PART CONSUMP - STATUS INVOICED
                            UpdateRemainingQtySparePartInvoiced(organizationService, entityToUpdate);
                        }
                        else if (entityToUpdate.GetAttributeValue<OptionSetValue>("tss_status").Value == 865920001)
                        {
                            // 2019.02.18 - UPDATE REMAINING QTY POTENTIAL PROSPECT PART SUBLINES DAN REMAINING QTY TOTAL PART CONSUMP - STATUS CANCELED
                            UpdateRemainingQtySparePartCanceled(organizationService, entityToUpdate);
                        }
                    }
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

        public void Form_OnCreate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext)
        {
            try
            {

                Entity entity = (Entity)pluginExceptionContext.InputParameters["Target"];
                String quotationNo = String.Empty;
                String noSequence = String.Empty;
                if (entity.LogicalName == _DL_tss_salesorderpartlines.EntityName)
                {
                    if (entity.Attributes.Contains("tss_sopartheaderid")){ 
                        Guid soPartHeaderId = entity.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id;
                        Entity so = organizationService.Retrieve("tss_sopartheader", soPartHeaderId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                        quotationNo = so.GetAttributeValue<String>("tss_sonumber");
                    };
                    if (entity.Attributes.Contains("tss_itemnumber")) noSequence = entity.GetAttributeValue<int>("tss_itemnumber").ToString();


                    if (entity.Attributes.Contains("tss_name"))
                        entity.Attributes["tss_name"] = quotationNo + " - " + noSequence;
                    else
                        entity.Attributes.Add("tss_name", quotationNo + " - " + noSequence);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnCreate_PreOperation: " + ex.Message.ToString());
            }
        }

        public void Form_OnDelete_PostOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.PreEntityImages["PreImage"];
                if (entity.LogicalName == _DL_tss_salesorderpartlines.EntityName)
                {
                    if (entity.Attributes.Contains("tss_sopartheaderid") && entity.Attributes["tss_sopartheaderid"] != null)
                    {
                        Guid headerId = entity.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id;
                        _DL_tss_salesorderpartheader.UpdateTotalAmount(organizationService, headerId);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnDelete_PostOperation : " + ex.Message.ToString());
            }
        }

        public void Form_OnDelete_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExcecutionContext)
        {
            try
            {
                Entity entity = (Entity)pluginExcecutionContext.PreEntityImages["PreImage"];
                if (entity.LogicalName == _DL_tss_salesorderpartlines.EntityName)
                {
                    if (entity.Attributes.Contains("tss_sopartheaderid") && entity.Attributes["tss_sopartheaderid"] != null)
                    {
                        Guid headerId = entity.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id;
                        var context = new OrganizationServiceContext(organizationService);
                        var header = (from c in context.CreateQuery(_DL_tss_salesorderpartheader.EntityName)
                                      where c.GetAttributeValue<Guid>("tss_sopartheaderid") == headerId
                                      select c).ToList();

                        if (header.Count > 0)
                        {
                            if (header[0].Attributes.Contains("tss_statecode") && header[0].Attributes["tss_statecode"] != null && header[0].Attributes.Contains("tss_statusreason") && header[0].Attributes["tss_statusreason"] != null
                                && header[0].Attributes.Contains("tss_sourcetype") && header[0].Attributes["tss_sourcetype"] != null)
                            {
                                //if (header[0].GetAttributeValue<OptionSetValue>("tss_statecode").Value != 865920002)
                                if (header[0].GetAttributeValue<OptionSetValue>("tss_statecode").Value != 865920000 && header[0].GetAttributeValue<OptionSetValue>("tss_statusreason").Value != 865920000 && (header[0].GetAttributeValue<OptionSetValue>("tss_sourcetype").Value != 865920004 || header[0].GetAttributeValue<OptionSetValue>("tss_sourcetype").Value != 865920006))
                                {
                                    throw new Exception("Can't delete sales order part lines!");
                                }
                            }
                        }
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

        #region UPDATE REMAINING QTY SPARE PART
        public void UpdateRemainingQtySparePartInvoiced(IOrganizationService organizationService, Entity _soline)
        {
            Entity _sopartheader = organizationService.Retrieve("tss_sopartheader", _soline.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id, new ColumnSet(true));
            Entity _prospectpartheader = new Entity();
            Entity _potentialprospectpart = new Entity();
            int _qtyrequest = _soline.GetAttributeValue<int>("tss_qtyrequest");

            if (_sopartheader.Attributes.Contains("tss_prospectlink"))
            {
                _prospectpartheader = organizationService.Retrieve("tss_prospectpartheader", _sopartheader.GetAttributeValue<EntityReference>("tss_prospectlink").Id, new ColumnSet(true));

                if (_prospectpartheader != null)
                {
                    _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", _prospectpartheader.GetAttributeValue<EntityReference>("tss_refmarketsize").Id, new ColumnSet(true));

                    if (_potentialprospectpart != null)
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.In, _potentialprospectpart.Id);
                        _filterexpression.AddCondition("tss_partnumber", ConditionOperator.In, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                        QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddFilter(_filterexpression);

                        EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

                        foreach (var _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
                        {
                            //int _remainingqty = _potentialprospectpartsublinesitem.GetAttributeValue<int>("tss_remainingqty");

                            //_potentialprospectpartsublinesitem["tss_remainingqty"] = _remainingqty;
                            _potentialprospectpartsublinesitem["tss_remainingqty"] = 0;

                            organizationService.Update(_potentialprospectpartsublinesitem);
                        }

                        #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE
                        _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
                        _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
                        _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                        _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);

                        EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

                        if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
                        {
                            Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
                            int _qtytotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqty");

                            if (_qtytotalpartconsumpmarketsize - _qtyrequest > 0)
                            {
                                _totalpartconsumpmarketsize["tss_remainingqty"] = _qtytotalpartconsumpmarketsize - _qtyrequest;
                            }
                            else
                            {
                                _totalpartconsumpmarketsize["tss_remainingqty"] = 0;
                            }

                            organizationService.Update(_totalpartconsumpmarketsize);

                            InsertTotalPartConsumpDetails(organizationService, _soline, _totalpartconsumpmarketsize);

                            Entity _update = new Entity("tss_sopartlines");
                            _update.Id = _soline.Id;
                            _update.Attributes["tss_totalpartconsumpmarketsize"] = new EntityReference("tss_totalpartconsumpmarketsize", _totalpartconsumpmarketsize.Id);
                            organizationService.Update(_update);
                        }
                        #endregion
                    }
                }
            }
        }

        //public void UpdateRemainingQtySparePart(IOrganizationService organizationService, Entity _soline)
        //{
        //    Entity _sopartheader = organizationService.Retrieve("tss_sopartheader", _soline.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id, new ColumnSet(true));
        //    Entity _prospectpartheader = new Entity();
        //    Entity _potentialprospectpart = new Entity();
        //    int _qtyrequest = _soline.GetAttributeValue<int>("tss_qtyrequest");

        //    if (_sopartheader.Attributes.Contains("tss_prospectlink"))
        //    {
        //        _prospectpartheader = organizationService.Retrieve("tss_prospectpartheader", _sopartheader.GetAttributeValue<EntityReference>("tss_prospectlink").Id, new ColumnSet(true));

        //        if (_prospectpartheader != null)
        //        {
        //            _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", _prospectpartheader.GetAttributeValue<EntityReference>("tss_refmarketsize").Id, new ColumnSet(true));

        //            if (_potentialprospectpart != null)
        //            {
        //                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.In, _potentialprospectpart.Id);
        //                _filterexpression.AddCondition("tss_partnumber", ConditionOperator.In, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                _queryexpression.Criteria.AddFilter(_filterexpression);

        //                EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

        //                foreach (var _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
        //                {
        //                    int _remainingqtyactual = _potentialprospectpartsublinesitem.GetAttributeValue<int>("tss_remainingqtyactual");

        //                    _potentialprospectpartsublinesitem["tss_remainingqty"] = _remainingqtyactual;

        //                    organizationService.Update(_potentialprospectpartsublinesitem);
        //                }

        //                #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE
        //                _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
        //                _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                _queryexpression.Criteria.AddFilter(_filterexpression);
        //                _queryexpression.ColumnSet = new ColumnSet(true);

        //                EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

        //                if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
        //                {
        //                    Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
        //                    int _qtytotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqty");

        //                    _totalpartconsumpmarketsize["tss_remainingqty"] = _qtytotalpartconsumpmarketsize - _qtyrequest;

        //                    organizationService.Update(_totalpartconsumpmarketsize);

        //                    InsertTotalPartConsumpDetails(organizationService, _soline, _totalpartconsumpmarketsize);
        //                }
        //                #endregion
        //            }
        //        }
        //    }
        //}

        public void InsertTotalPartConsumpDetails(IOrganizationService organizationService, Entity _soline, Entity _totalpartconsumpmarketsize)
        {
            Entity _totalpartconsumpmarketsizedetails = new Entity("tss_totalpartconsumpmarketsizedetails");

            _totalpartconsumpmarketsizedetails["tss_partnumber"] = new EntityReference("trs_masterpart", _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);
            _totalpartconsumpmarketsizedetails["tss_qty"] = _soline.GetAttributeValue<int>("tss_qtyrequest");
            _totalpartconsumpmarketsizedetails["tss_salesorderpart"] = new EntityReference("tss_sopartheader", _soline.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id);
            _totalpartconsumpmarketsizedetails["tss_salesorderpartlines"] = new EntityReference("tss_sopartlines", _soline.Id);
            _totalpartconsumpmarketsizedetails["tss_totalpartconsump"] = new EntityReference("tss_totalpartconsumpmarketsize", _totalpartconsumpmarketsize.Id);
            _totalpartconsumpmarketsizedetails["tss_totalprice"] = new Money( _soline.GetAttributeValue<Money>("tss_totalprice").Value);

            organizationService.Create(_totalpartconsumpmarketsizedetails);
        }

        public void UpdateRemainingQtySparePartCanceled(IOrganizationService organizationService, Entity _soline)
        {
            Entity _sopartheader = organizationService.Retrieve("tss_sopartheader", _soline.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id, new ColumnSet(true));
            Entity _prospectpartheader = new Entity();
            Entity _potentialprospectpart = new Entity();
            int _qtyrequest = _soline.GetAttributeValue<int>("tss_qtyrequest");
            int _differenceqty = _qtyrequest;

            if (_sopartheader.Attributes.Contains("tss_prospectlink"))
            {
                _prospectpartheader = organizationService.Retrieve("tss_prospectpartheader", _sopartheader.GetAttributeValue<EntityReference>("tss_prospectlink").Id, new ColumnSet(true));

                if (_prospectpartheader != null)
                {
                    _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", _prospectpartheader.GetAttributeValue<EntityReference>("tss_refmarketsize").Id, new ColumnSet(true));

                    if (_potentialprospectpart != null)
                    {
                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.In, _potentialprospectpart.Id);
                        _filterexpression.AddCondition("tss_partnumber", ConditionOperator.In, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                        QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddFilter(_filterexpression);

                        EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

                        foreach (var _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
                        {
                            int _qty = _potentialprospectpartsublinesitem.GetAttributeValue<int>("tss_qty");

                            //_potentialprospectpartsublinesitem["tss_remainingqty"] = _remainingqty;
                            _potentialprospectpartsublinesitem["tss_remainingqty"] = _qty;

                            organizationService.Update(_potentialprospectpartsublinesitem);
                        }

                        #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE
                        _filterexpression = new FilterExpression(LogicalOperator.And);
                        _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
                        _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
                        _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                        _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
                        _queryexpression.Criteria.AddFilter(_filterexpression);
                        _queryexpression.ColumnSet = new ColumnSet(true);

                        EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

                        if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
                        {
                            Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
                            int _remainingqty = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqty");
                            int _qty = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_qty");

                            if (_remainingqty + _qtyrequest >= _qty)
                            {
                                _totalpartconsumpmarketsize["tss_remainingqty"] = _qty;
                            }
                            else
                            {
                                _totalpartconsumpmarketsize["tss_remainingqty"] = _remainingqty + _qtyrequest;
                            }

                            organizationService.Update(_totalpartconsumpmarketsize);

                            DeleteTotalPartConsumpDetails(organizationService, _soline, _totalpartconsumpmarketsize);

                            Entity _update = new Entity("tss_sopartlines");
                            _update.Id = _soline.Id;
                            _update.Attributes["tss_totalpartconsumpmarketsize"] = null;
                            organizationService.Update(_update);
                        }
                        #endregion
                    }
                }
            }
        }

        public void DeleteTotalPartConsumpDetails(IOrganizationService organizationService, Entity _soline, Entity _totalpartconsumpmarketsize)
        {
            FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
            _filterexpression.AddCondition("tss_salesorderpartlines", ConditionOperator.Equal, _soline.Id);

            QueryExpression _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsizedetails");
            _queryexpression.Criteria.AddFilter(_filterexpression);
            _queryexpression.ColumnSet = new ColumnSet(true);

            EntityCollection _totalpartconsumpmarketsizedetailscollection = organizationService.RetrieveMultiple(_queryexpression);

            foreach (var _totalpartconsumpmarketsizedetailsitem in _totalpartconsumpmarketsizedetailscollection.Entities)
            {
                organizationService.Delete("tss_totalpartconsumpmarketsizedetails", _totalpartconsumpmarketsizedetailsitem.Id);
            }
        }

        //public void UpdateRemainingQtySparePartCanceled(IOrganizationService organizationService, Entity _soline)
        //{
        //    Entity _sopartheader = organizationService.Retrieve("tss_sopartheader", _soline.GetAttributeValue<EntityReference>("tss_sopartheaderid").Id, new ColumnSet(true));
        //    Entity _prospectpartheader = new Entity();
        //    Entity _potentialprospectpart = new Entity();
        //    int _qtyrequest = _soline.GetAttributeValue<int>("tss_qtyrequest");
        //    int _differenceqty = _qtyrequest;

        //    if (_sopartheader.Attributes.Contains("tss_prospectlink"))
        //    {
        //        _prospectpartheader = organizationService.Retrieve("tss_prospectpartheader", _sopartheader.GetAttributeValue<EntityReference>("tss_prospectlink").Id, new ColumnSet(true));

        //        if (_prospectpartheader != null)
        //        {
        //            _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", _prospectpartheader.GetAttributeValue<EntityReference>("tss_refmarketsize").Id, new ColumnSet(true));

        //            if (_potentialprospectpart != null)
        //            {
        //                FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.In, _potentialprospectpart.Id);
        //                _filterexpression.AddCondition("tss_partnumber", ConditionOperator.In, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");
        //                _queryexpression.ColumnSet = new ColumnSet(true);
        //                _queryexpression.Criteria.AddFilter(_filterexpression);

        //                EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

        //                //foreach (var _potentialprospectpartsublinesitem in _potentialprospectpartsublinescollection.Entities)
        //                //{
        //                //    int _remainingqtyactual = _potentialprospectpartsublinesitem.GetAttributeValue<int>("tss_remainingqtyactual");

        //                //    _potentialprospectpartsublinesitem["tss_remainingqty"] = _remainingqtyactual;

        //                //    organizationService.Update(_potentialprospectpartsublinesitem);
        //                //}

        //                int _counter = 0;

        //                while (_qtyrequest > 0 && _counter <= _potentialprospectpartsublinescollection.Entities.Count() - 1)
        //                {
        //                    int _qtymax = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqty");
        //                    int _qtyactual = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqtyactual");
        //                    int _qtyslot = _qtymax - _qtyactual;

        //                    if (_qtyslot > 0)
        //                    {
        //                        if (_qtyrequest <= _qtyslot)
        //                        {
        //                            _qtyactual = _qtyactual + _qtyrequest;
        //                            _qtyslot = _qtyslot - _qtyrequest;
        //                            _qtyrequest = 0;

        //                            _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyactual;
        //                            organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
        //                        }
        //                        else
        //                        {
        //                            _qtyactual = _qtyactual + _qtyslot;
        //                            _qtyrequest = _qtyrequest - _qtyslot;
        //                            _qtyslot = 0;

        //                            _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyactual;
        //                            organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
        //                        }
        //                    }

        //                    _counter += 1;
        //                }

        //                #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE
        //                _filterexpression = new FilterExpression(LogicalOperator.And);
        //                _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
        //                _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, _soline.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                _queryexpression.Criteria.AddFilter(_filterexpression);
        //                _queryexpression.ColumnSet = new ColumnSet(true);

        //                EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

        //                if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
        //                {
        //                    Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
        //                    int _qtytotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqtyactual");

        //                    _totalpartconsumpmarketsize["tss_remainingqtyactual"] = _qtytotalpartconsumpmarketsize + _differenceqty;

        //                    organizationService.Update(_totalpartconsumpmarketsize);
        //                }
        //                #endregion
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
