using Microsoft.Xrm.Sdk;
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
    public class BL_tss_prospectpartlines
    {
        #region Constants
        private const string _classname = "BL_tss_prospectpartlines";
        private const string _entityname = "tss_prospectpartlines";
        private const int _depth = 1;

        private int? _nullint = null;
        #endregion

        #region Dependencies
        #endregion

        #region Publics
        public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity entity, Entity preimageentity, ITracingService tracer)
        {
            try
            {
                if (entity.LogicalName == _entityname)
                {
                    //int _sourcetype;

                    //if (entity.Attributes.Contains("tss_sourcetype"))
                    //    _sourcetype = entity.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
                    //else
                    //    _sourcetype = preimageentity.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;

                    //int _quantity = entity.Attributes.Contains("tss_quantity") ? entity.GetAttributeValue<int>("tss_quantity") : 0;
                    //int? _quantityconvert = preimageentity.Attributes.Contains("tss_quantityconvert") ? preimageentity.GetAttributeValue<int>("tss_quantityconvert") : _nullint;

                    //if (_sourcetype == 865920001 && _quantityconvert != null)
                    //{
                    //    Entity _prospectpartheader = organizationService.Retrieve("tss_prospectpartheader", preimageentity.GetAttributeValue<EntityReference>("tss_prospectpartheader").Id, new ColumnSet(true));
                    //    Entity _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", _prospectpartheader.GetAttributeValue<EntityReference>("tss_refmarketsize").Id, new ColumnSet(true));

                    //    if (_potentialprospectpart != null)
                    //    {
                    //        int _quantity_old = preimageentity.GetAttributeValue<int>("tss_quantity");
                    //        int _differenceqty = 0;
                    //        bool _islessthanold = true;

                    //        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
                    //        _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.Equal, _potentialprospectpart.Id);
                    //        _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, preimageentity.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                    //        QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");

                    //        if (_quantity < _quantity_old)
                    //        {
                    //            // POTENTIAL PROSPECT DITAMBAH
                    //            _differenceqty = _quantity_old - _quantity;
                    //            _islessthanold = true;

                    //            _queryexpression.Orders.Add(new OrderExpression("tss_remainingqty", OrderType.Descending));
                    //        }
                    //        else if (_quantity > _quantity_old)
                    //        {
                    //            // POTENTIAL PROSPECT DIKURANGI
                    //            _differenceqty = _quantity - _quantity_old;
                    //            _islessthanold = false;

                    //            _queryexpression.Orders.Add(new OrderExpression("tss_remainingqty", OrderType.Ascending));
                    //        }

                    //        _queryexpression.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
                    //        _queryexpression.Criteria.AddFilter(_filterexpression);
                    //        _queryexpression.ColumnSet = new ColumnSet(true);

                    //        EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

                    //        if (_islessthanold)
                    //        {
                    //            // POTENTIAL PROSPECT DITAMBAH
                    //            int _qtyrequest = _differenceqty;
                    //            int _counter = 0;

                    //            while (_qtyrequest > 0 && _counter <= _potentialprospectpartsublinescollection.Entities.Count() - 1)
                    //            {
                    //                int _qtymax = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqty");
                    //                int _qtyactual = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqtyactual");
                    //                int _qtyslot = _qtymax - _qtyactual;

                    //                if (_qtyslot > 0)
                    //                {
                    //                    if (_qtyrequest <= _qtyslot)
                    //                    {
                    //                        _qtyactual = _qtyactual + _qtyrequest;
                    //                        _qtyslot = _qtyslot - _qtyrequest;
                    //                        _qtyrequest = 0;

                    //                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyactual;
                    //                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
                    //                    }
                    //                    else
                    //                    {
                    //                        _qtyactual = _qtyactual + _qtyslot;
                    //                        _qtyrequest = _qtyrequest - _qtyslot;
                    //                        _qtyslot = 0;

                    //                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyactual;
                    //                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
                    //                    }
                    //                }

                    //                _counter += 1;
                    //            }

                    //            //#region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE - PENAMBAHAN TOTAL PART CONSUMP
                    //            //_filterexpression = new FilterExpression(LogicalOperator.And);
                    //            //_filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
                    //            //_filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
                    //            //_filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, preimageentity.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                    //            //_queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
                    //            //_queryexpression.Criteria.AddFilter(_filterexpression);
                    //            //_queryexpression.ColumnSet = new ColumnSet(true);

                    //            //EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

                    //            //if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
                    //            //{
                    //            //    Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
                    //            //    int _qtyactualtotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqtyactual");

                    //            //    _totalpartconsumpmarketsize["tss_remainingqtyactual"] = _qtyactualtotalpartconsumpmarketsize + _differenceqty;

                    //            //    organizationService.Update(_totalpartconsumpmarketsize);
                    //            //}
                    //            //#endregion
                    //        }
                    //        else
                    //        {
                    //            // POTENTIAL PROSPECT DIKURANGI
                    //            int _qtyrequest = _differenceqty;
                    //            int _counter = 0;

                    //            while (_qtyrequest > 0 && _counter <= _potentialprospectpartsublinescollection.Entities.Count() - 1)
                    //            {
                    //                int _qtymax = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqty");
                    //                int _qtyactual = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqtyactual");
                    //                int _qtyslot = _qtyactual;

                    //                if (_qtyslot > 0)
                    //                {
                    //                    if (_qtyslot >= _qtyrequest)
                    //                    {
                    //                        _qtyslot = _qtyslot - _qtyrequest;
                    //                        _qtyrequest = 0;

                    //                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyslot;
                    //                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
                    //                    }
                    //                    else
                    //                    {
                    //                        _qtyrequest = _qtyrequest - _qtyslot;
                    //                        _qtyslot = 0;

                    //                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyslot;
                    //                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
                    //                    }
                    //                }

                    //                _counter += 1;
                    //            }

                    //            //#region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE - PENGURANGAN TOTAL PART CONSUMP
                    //            //_filterexpression = new FilterExpression(LogicalOperator.And);
                    //            //_filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
                    //            //_filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
                    //            //_filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, preimageentity.GetAttributeValue<EntityReference>("tss_partnumber").Id);

                    //            //_queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
                    //            //_queryexpression.Criteria.AddFilter(_filterexpression);
                    //            //_queryexpression.ColumnSet = new ColumnSet(true);

                    //            //EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

                    //            //if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
                    //            //{
                    //            //    Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
                    //            //    int _qtyactualtotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqtyactual");
                    //            //    int _assignqty = _qtyactualtotalpartconsumpmarketsize - _differenceqty;

                    //            //    if (_assignqty >= 0)
                    //            //    {
                    //            //        _totalpartconsumpmarketsize["tss_remainingqtyactual"] = _assignqty;

                    //            //        organizationService.Update(_totalpartconsumpmarketsize);
                    //            //    }
                    //            //    else
                    //            //        throw new InvalidPluginExecutionException("There are " + _qtyactualtotalpartconsumpmarketsize.ToString() + " remain for Total Part Consump.");
                    //            //}
                    //            //#endregion
                    //        }
                    //    }
                    //}
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation: " + ex.Message.ToString());
            }
        }
        #endregion


        //public void Form_OnUpdate_PreOperation(IOrganizationService organizationService, IPluginExecutionContext pluginExceptionContext, Entity entity, Entity preimageentity, ITracingService tracer)
        //{
        //    try
        //    {
        //        if (entity.LogicalName == _entityname)
        //        {
        //            int _sourcetype;

        //            if (entity.Attributes.Contains("tss_sourcetype"))
        //                _sourcetype = entity.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;
        //            else
        //                _sourcetype = preimageentity.GetAttributeValue<OptionSetValue>("tss_sourcetype").Value;

        //            int _quantity = entity.Attributes.Contains("tss_quantity") ? entity.GetAttributeValue<int>("tss_quantity") : 0;
        //            int? _quantityconvert = preimageentity.Attributes.Contains("tss_quantityconvert") ? preimageentity.GetAttributeValue<int>("tss_quantityconvert") : _nullint;

        //            if (_sourcetype == 865920001 && _quantityconvert != null)
        //            {
        //                if (_quantity <= _quantityconvert)
        //                {
        //                    Entity _prospectpartheader = organizationService.Retrieve("tss_prospectpartheader", preimageentity.GetAttributeValue<EntityReference>("tss_prospectpartheader").Id, new ColumnSet(true));
        //                    Entity _potentialprospectpart = organizationService.Retrieve("tss_potentialprospectpart", _prospectpartheader.GetAttributeValue<EntityReference>("tss_refmarketsize").Id, new ColumnSet(true));

        //                    if (_potentialprospectpart != null)
        //                    {
        //                        int _quantity_old = preimageentity.GetAttributeValue<int>("tss_quantity");
        //                        int _differenceqty = 0;
        //                        bool _islessthanold = true;

        //                        FilterExpression _filterexpression = new FilterExpression(LogicalOperator.And);
        //                        _filterexpression.AddCondition("tss_potentialprospectpartref", ConditionOperator.Equal, _potentialprospectpart.Id);
        //                        _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, preimageentity.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                        QueryExpression _queryexpression = new QueryExpression("tss_potentialprospectpartsublines");

        //                        if (_quantity < _quantity_old)
        //                        {
        //                            // POTENTIAL PROSPECT DITAMBAH
        //                            _differenceqty = _quantity_old - _quantity;
        //                            _islessthanold = true;

        //                            _queryexpression.Orders.Add(new OrderExpression("tss_remainingqtyactual", OrderType.Descending));
        //                        }
        //                        else if (_quantity > _quantity_old)
        //                        {
        //                            // POTENTIAL PROSPECT DIKURANGI
        //                            _differenceqty = _quantity - _quantity_old;
        //                            _islessthanold = false;

        //                            _queryexpression.Orders.Add(new OrderExpression("tss_remainingqtyactual", OrderType.Ascending));
        //                        }

        //                        _queryexpression.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
        //                        _queryexpression.Criteria.AddFilter(_filterexpression);
        //                        _queryexpression.ColumnSet = new ColumnSet(true);

        //                        EntityCollection _potentialprospectpartsublinescollection = organizationService.RetrieveMultiple(_queryexpression);

        //                        if (_islessthanold)
        //                        {
        //                            // POTENTIAL PROSPECT DITAMBAH
        //                            int _qtyrequest = _differenceqty;
        //                            int _counter = 0;

        //                            while (_qtyrequest > 0 && _counter <= _potentialprospectpartsublinescollection.Entities.Count() - 1)
        //                            {
        //                                int _qtymax = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqty");
        //                                int _qtyactual = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqtyactual");
        //                                int _qtyslot = _qtymax - _qtyactual;

        //                                if (_qtyslot > 0)
        //                                {
        //                                    if (_qtyrequest <= _qtyslot)
        //                                    {
        //                                        _qtyactual = _qtyactual + _qtyrequest;
        //                                        _qtyslot = _qtyslot - _qtyrequest;
        //                                        _qtyrequest = 0;

        //                                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyactual;
        //                                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
        //                                    }
        //                                    else
        //                                    {
        //                                        _qtyactual = _qtyactual + _qtyslot;
        //                                        _qtyrequest = _qtyrequest - _qtyslot;
        //                                        _qtyslot = 0;

        //                                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyactual;
        //                                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
        //                                    }
        //                                }

        //                                _counter += 1;
        //                            }

        //                            #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE - PENAMBAHAN TOTAL PART CONSUMP
        //                            _filterexpression = new FilterExpression(LogicalOperator.And);
        //                            _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
        //                            _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                            _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, preimageentity.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                            _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                            _queryexpression.Criteria.AddFilter(_filterexpression);
        //                            _queryexpression.ColumnSet = new ColumnSet(true);

        //                            EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

        //                            if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
        //                            {
        //                                Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
        //                                int _qtyactualtotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqtyactual");

        //                                _totalpartconsumpmarketsize["tss_remainingqtyactual"] = _qtyactualtotalpartconsumpmarketsize + _differenceqty;

        //                                organizationService.Update(_totalpartconsumpmarketsize);
        //                            }
        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            // POTENTIAL PROSPECT DIKURANGI
        //                            int _qtyrequest = _differenceqty;
        //                            int _counter = 0;

        //                            while (_qtyrequest > 0 && _counter <= _potentialprospectpartsublinescollection.Entities.Count() - 1)
        //                            {
        //                                int _qtymax = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqty");
        //                                int _qtyactual = _potentialprospectpartsublinescollection[_counter].GetAttributeValue<int>("tss_remainingqtyactual");
        //                                int _qtyslot = _qtyactual;

        //                                if (_qtyslot > 0)
        //                                {
        //                                    if (_qtyslot >= _qtyrequest)
        //                                    {
        //                                        _qtyslot = _qtyslot - _qtyrequest;
        //                                        _qtyrequest = 0;

        //                                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyslot;
        //                                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
        //                                    }
        //                                    else
        //                                    {
        //                                        _qtyrequest = _qtyrequest - _qtyslot;
        //                                        _qtyslot = 0;

        //                                        _potentialprospectpartsublinescollection[_counter]["tss_remainingqtyactual"] = _qtyslot;
        //                                        organizationService.Update(_potentialprospectpartsublinescollection[_counter]);
        //                                    }
        //                                }

        //                                _counter += 1;
        //                            }

        //                            #region LOOK UP TO TOTAL PART CONSUMP MARKET SIZE - PENGURANGAN TOTAL PART CONSUMP
        //                            _filterexpression = new FilterExpression(LogicalOperator.And);
        //                            _filterexpression.AddCondition("tss_marketsizeid", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_marketsizeid").Id);
        //                            _filterexpression.AddCondition("tss_pss", ConditionOperator.Equal, _potentialprospectpart.GetAttributeValue<EntityReference>("tss_pss").Id);
        //                            _filterexpression.AddCondition("tss_partnumber", ConditionOperator.Equal, preimageentity.GetAttributeValue<EntityReference>("tss_partnumber").Id);

        //                            _queryexpression = new QueryExpression("tss_totalpartconsumpmarketsize");
        //                            _queryexpression.Criteria.AddFilter(_filterexpression);
        //                            _queryexpression.ColumnSet = new ColumnSet(true);

        //                            EntityCollection _totalpartconsumpmarketsizecollection = organizationService.RetrieveMultiple(_queryexpression);

        //                            if (_totalpartconsumpmarketsizecollection.Entities.Count() > 0)
        //                            {
        //                                Entity _totalpartconsumpmarketsize = _totalpartconsumpmarketsizecollection.Entities[0];
        //                                int _qtyactualtotalpartconsumpmarketsize = _totalpartconsumpmarketsize.GetAttributeValue<int>("tss_remainingqtyactual");
        //                                int _assignqty = _qtyactualtotalpartconsumpmarketsize - _differenceqty;

        //                                if (_assignqty >= 0)
        //                                {
        //                                    _totalpartconsumpmarketsize["tss_remainingqtyactual"] = _assignqty;

        //                                    organizationService.Update(_totalpartconsumpmarketsize);
        //                                }
        //                                else
        //                                    throw new InvalidPluginExecutionException("There are " + _qtyactualtotalpartconsumpmarketsize.ToString() + " remain for Total Part Consump.");
        //                            }
        //                            #endregion
        //                        }
        //                    }


        //                }
        //                else
        //                {
        //                    throw new InvalidPluginExecutionException("Quantity must be <= " + _quantityconvert.ToString() + " (from Potential Prospect Part).");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidPluginExecutionException(_classname + ".Form_OnUpdate_PreOperation: " + ex.Message.ToString());
        //    }
        //}
        //#endregion
    }
}
