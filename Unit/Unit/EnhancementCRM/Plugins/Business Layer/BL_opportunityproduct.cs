using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.Plugins.Business_Layer
{
    public class BL_opportunityproduct
    {
        #region Properties
        private string _classname = "BL_opportunityproduct";
        private string _entityname_opportunityproduct = "opportunityproduct";

        private string _entityname_opportunity = "opportunity";
        private string _entityname_product = "product";
        private string _entityname_salesbom = "ittn_salesbom";
        private string _entityname_salesbomproducts = "ittn_salesbomproducts";
        private string _entityname_systemuser = "systemuser";
        private string _entityname_unitgroup = "uomschedule";
        private string _entityname_uom = "uom";
        private string _entityname_productpricelevel = "productpricelevel";
        #endregion

        public void CreateSalesBOM(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _entity.Id, new ColumnSet(true));

                if (_entity != null)
                {
                    Guid _productid = _entity.Attributes.Contains("new_product") ? _entity.GetAttributeValue<EntityReference>("new_product").Id : new Guid();
                    Guid _opportunityid = _entity.Attributes.Contains("opportunityid") ? _entity.GetAttributeValue<EntityReference>("opportunityid").Id : new Guid();

                    if (_productid != new Guid() && _opportunityid != new Guid())
                    {
                        Entity _opportunity = _organizationservice.Retrieve(_entityname_opportunity, _opportunityid, new ColumnSet(true));

                        decimal _itemnumber = _entity.GetAttributeValue<decimal>("new_itemnumber");
                        decimal? _parentnumber = _entity.Attributes.Contains("new_parentnumber") ? _entity.GetAttributeValue<decimal?>("new_parentnumber") : null;
                        Guid _unitgroupid = _opportunity.GetAttributeValue<EntityReference>("new_unitgroup").Id;

                        // CHECK IF EXIST IN MATRIX
                        #region CHECK IF EXIST IN MATRIX
                        QueryExpression _queryexpression = new QueryExpression(_entityname_salesbom);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("ittn_product", ConditionOperator.Equal, _productid);
                        EntityCollection _salesboms = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_salesboms.Entities.Count() > 0 && _parentnumber == null)
                        {
                            _queryexpression = new QueryExpression(_entityname_salesbom);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_product", ConditionOperator.Equal, _productid);
                            _queryexpression.Criteria.AddCondition("ittn_alternativebom", ConditionOperator.Equal, 1);
                            _salesboms = _organizationservice.RetrieveMultiple(_queryexpression);

                            if (_salesboms.Entities.Count() > 0)
                            {
                                Entity _salesbom = _salesboms.Entities.FirstOrDefault();
                                int _alternativebomnumber = _salesbom.GetAttributeValue<int>("ittn_alternativebom");

                                _queryexpression = new QueryExpression(_entityname_salesbomproducts);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("ittn_salesbom", ConditionOperator.Equal, _salesbom.Id);
                                EntityCollection _salesbomproductss = _organizationservice.RetrieveMultiple(_queryexpression);

                                foreach (var _salesbomproduct in _salesbomproductss.Entities)
                                {
                                    Entity _opportunityproduct = new Entity(_entityname_opportunityproduct);
                                    Guid _salesbomproduct_product = _salesbomproduct.GetAttributeValue<EntityReference>("ittn_product").Id;
                                    int _quantity = _salesbomproduct.GetAttributeValue<int>("ittn_quantity");
                                    bool _mandatory = _salesbomproduct.GetAttributeValue<bool>("ittn_mandatory");

                                    _queryexpression = new QueryExpression(_entityname_uom);
                                    _queryexpression.ColumnSet = new ColumnSet(true);
                                    _queryexpression.Criteria.AddCondition("uomscheduleid", ConditionOperator.Equal, _unitgroupid);
                                    Guid _uomid = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault().Id;

                                    _opportunityproduct["extendedamount"] = new Money(0);
                                    _opportunityproduct["ittn_totalextendedamount"] = new Money(0);
                                    _opportunityproduct["ittn_alternativebom"] = new EntityReference(_entityname_salesbom, _salesbom.Id);
                                    _opportunityproduct["ittn_alternativebomnumber"] = _alternativebomnumber;
                                    _opportunityproduct["ittn_mandatory"] = _mandatory;
                                    _opportunityproduct["new_parentnumber"] = _itemnumber;
                                    _opportunityproduct["new_product"] = new EntityReference(_entityname_product, _salesbomproduct_product);
                                    _opportunityproduct["new_quantity"] = decimal.Parse(_quantity.ToString());
                                    _opportunityproduct["new_unitgroup"] = new EntityReference(_entityname_unitgroup, _unitgroupid);
                                    _opportunityproduct["opportunityid"] = new EntityReference(_entityname_opportunity, _opportunityid);
                                    _opportunityproduct["productid"] = new EntityReference(_entityname_product, _salesbomproduct_product);
                                    _opportunityproduct["quantity"] = decimal.Parse(_quantity.ToString());
                                    _opportunityproduct["uomid"] = new EntityReference(_entityname_uom, _uomid);

                                    _organizationservice.Create(_opportunityproduct);
                                }

                                // UPDATE PROSPECT PRODUCT ( HEADER ) ALTERNATIVE BOM
                                Entity _opportunityproduct_update = new Entity(_entityname_opportunityproduct);
                                _opportunityproduct_update.Id = _entity.Id;
                                //_opportunityproduct_update["ittn_alternativebom"] = new EntityReference(_entityname_salesbom, _salesbom.Id);
                                _opportunityproduct_update["ittn_alternativebomnumber"] = _alternativebomnumber;
                                _opportunityproduct_update["ittn_mandatory"] = false;

                                _organizationservice.Update(_opportunityproduct_update);
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException("This product sales BOM doesn't have alternative 1 !");
                            }
                        }
                        #endregion ---
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CreateSalesBOM: " + ex.Message.ToString());
            }
        }

        public void UpdateAlternativeBOM(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _entity.Id, new ColumnSet(true));

                if (_entity != null)
                {
                    Guid _opportunityid = _entity.Attributes.Contains("opportunityid") ? _entity.GetAttributeValue<EntityReference>("opportunityid").Id : new Guid();

                    if (_opportunityid != new Guid())
                    {
                        Guid _alternativebomid = _entity.Attributes.Contains("ittn_alternativebom") ? _entity.GetAttributeValue<EntityReference>("ittn_alternativebom").Id : Guid.Empty;

                        // CHECK IF EXIST IN MATRIX
                        if (_alternativebomid != new Guid())
                        {
                            Entity _opportunity = _organizationservice.Retrieve(_entityname_opportunity, _opportunityid, new ColumnSet(true));

                            Guid _unitgroupid = _opportunity.GetAttributeValue<EntityReference>("new_unitgroup").Id;
                            decimal _itemnumber = _entity.GetAttributeValue<decimal>("new_itemnumber");

                            // DELETE PRODUCT ATTACHMENT
                            QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _opportunityid);
                            _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);
                            EntityCollection _opportunityproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                            foreach (var _opportunityproduct in _opportunityproducts.Entities)
                            {
                                Entity _opportunityproduct_updatetodelete = new Entity(_entityname_opportunityproduct);
                                _opportunityproduct_updatetodelete.Id = _opportunityproduct.Id;

                                _opportunityproduct_updatetodelete["ittn_mandatory"] = false;
                                _organizationservice.Update(_opportunityproduct_updatetodelete);

                                _organizationservice.Delete(_entityname_opportunityproduct, _opportunityproduct.Id);
                            }

                            Entity _salesbom = _organizationservice.Retrieve(_entityname_salesbom, _alternativebomid, new ColumnSet(true));
                            int _alternativebomnumber = _salesbom.GetAttributeValue<int>("ittn_alternativebom");

                            _queryexpression = new QueryExpression(_entityname_salesbomproducts);
                            _queryexpression.ColumnSet = new ColumnSet(true);
                            _queryexpression.Criteria.AddCondition("ittn_salesbom", ConditionOperator.Equal, _salesbom.Id);
                            EntityCollection _salesbomproductss = _organizationservice.RetrieveMultiple(_queryexpression);

                            foreach (var _salesbomproduct in _salesbomproductss.Entities)
                            {
                                Entity _opportunityproduct = new Entity(_entityname_opportunityproduct);
                                Guid _salesbomproduct_product = _salesbomproduct.GetAttributeValue<EntityReference>("ittn_product").Id;
                                int _quantity = _salesbomproduct.GetAttributeValue<int>("ittn_quantity");
                                bool _mandatory = _salesbomproduct.GetAttributeValue<bool>("ittn_mandatory");

                                _queryexpression = new QueryExpression(_entityname_uom);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("uomscheduleid", ConditionOperator.Equal, _unitgroupid);
                                Guid _uomid = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault().Id;

                                _opportunityproduct["extendedamount"] = new Money(0);
                                _opportunityproduct["ittn_totalextendedamount"] = new Money(0);
                                _opportunityproduct["ittn_alternativebom"] = new EntityReference(_entityname_salesbom, _salesbom.Id);
                                _opportunityproduct["ittn_alternativebomnumber"] = _alternativebomnumber;
                                _opportunityproduct["ittn_mandatory"] = _mandatory;
                                _opportunityproduct["new_parentnumber"] = _itemnumber;
                                _opportunityproduct["new_product"] = new EntityReference(_entityname_product, _salesbomproduct_product);
                                _opportunityproduct["new_quantity"] = decimal.Parse(_quantity.ToString());
                                _opportunityproduct["new_unitgroup"] = new EntityReference(_entityname_unitgroup, _unitgroupid);
                                _opportunityproduct["opportunityid"] = new EntityReference(_entityname_opportunity, _opportunityid);
                                _opportunityproduct["productid"] = new EntityReference(_entityname_product, _salesbomproduct_product);
                                _opportunityproduct["quantity"] = decimal.Parse(_quantity.ToString());
                                _opportunityproduct["uomid"] = new EntityReference(_entityname_uom, _uomid);

                                _organizationservice.Create(_opportunityproduct);
                            }

                            // UPDATE PROSPECT PRODUCT ( HEADER ) ALTERNATIVE BOM
                            Entity _opportunityproduct_update = new Entity(_entityname_opportunityproduct);
                            _opportunityproduct_update.Id = _entity.Id;
                            //_opportunityproduct_update["ittn_alternativebom"] = new EntityReference(_entityname_salesbom, _salesbom.Id);
                            _opportunityproduct_update["ittn_alternativebomnumber"] = _alternativebomnumber;
                            _opportunityproduct_update["ittn_mandatory"] = false;

                            _organizationservice.Update(_opportunityproduct_update);

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateAlternativeBOM: " + ex.Message.ToString());
            }
        }

        public void UpdateSalesBOMProduct(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _entity.Id, new ColumnSet(true));

                if (_entity != null)
                {
                    if (_entity.Attributes.Contains("ittn_alternativebomnumber"))
                    {
                        Entity _row = new Entity(_entityname_opportunityproduct);
                        _row.Id = _entity.Id;
                        _row["ittn_nondefaultbom"] = true;

                        _organizationservice.Update(_row);

                    }   

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateSalesBOMProduct: " + ex.Message.ToString());
            }
        }

        public void CreateOpportunityProductDiscountBasedOnParent(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _context.PrimaryEntityId, new ColumnSet(true));

                if (_entity != null)
                {
                    decimal? _itemnumber = _entity.GetAttributeValue<decimal?>("new_itemnumber");
                    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");
                    decimal _pricelist = 0;
                    decimal _priceperunit = _entity.GetAttributeValue<Money>("priceperunit").Value;
                    decimal _percentagediscount = new decimal(0);
                    decimal _manualdiscountamount = new decimal(0);
                    decimal _extendedamountparent = _entity.GetAttributeValue<Money>("extendedamount").Value;
                    decimal _extendedamount = new decimal(0);
                    decimal _quantity = 0;

                    if (_parentnumber == null)
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("opportunityid").Id);
                        _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);
                        EntityCollection _opportunityproductschild = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_opportunityproductschild.Entities.Count > 0)
                        {
                            bool _ispriceoverriden = _entity.GetAttributeValue<bool>("ispriceoverridden");
                            bool _typediscount = _entity.GetAttributeValue<bool>("new_typediscount");
                            _percentagediscount = _entity.Attributes.Contains("new_percentagediscount") ? _entity.GetAttributeValue<decimal>("new_percentagediscount") : new decimal(0);
                            _manualdiscountamount = _entity.Attributes.Contains("manualdiscountamount") ? _entity.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);

                            if (_ispriceoverriden)  // PRICING : OVERRIDE PRICE
                            {
                                _queryexpression = new QueryExpression(_entityname_productpricelevel);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("productid").Id);
                                _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id);

                                EntityCollection _productpricelevels = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_productpricelevels.Entities.Count() > 0)
                                {
                                    Entity _productpricelevel = _productpricelevels.Entities.FirstOrDefault();

                                    _pricelist = _productpricelevel.GetAttributeValue<Money>("amount").Value;
                                }

                                if (_pricelist > 0)
                                    _percentagediscount = (1 - (_priceperunit / _pricelist)) * 100;

                                foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                {
                                    decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                    if (_parentnumberchild != null)
                                    {
                                        _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                        _manualdiscountamount = _priceperunit * _percentagediscount / 100;
                                        _priceperunit = _priceperunit - _manualdiscountamount;
                                        _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);

                                        Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                        _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                        _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                        _opportunityproduct_child["new_typediscount"] = _typediscount;
                                        _opportunityproduct_child["priceperunit"] = new Money(_priceperunit >= 0 ? _priceperunit : 0);
                                        _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                        _extendedamount = _priceperunit * _quantity;

                                        _organizationservice.Update(_opportunityproduct_child);

                                        _extendedamountparent += _extendedamount;
                                    }
                                }
                            }
                            else  // PRICING : USE DEFAULT
                            {
                                if (_typediscount && _manualdiscountamount > 0) //TYPE DISCOUNT : WRITE-IN
                                {
                                    foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                    {
                                        decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                        if (_parentnumberchild != null)
                                        {
                                            _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                            _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);
                                            _manualdiscountamount = _entity.Attributes.Contains("manualdiscountamount") ? _entity.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);

                                            if (_manualdiscountamount > (_priceperunit * _quantity))
                                                _manualdiscountamount = (_priceperunit * _quantity);

                                            _manualdiscountamount = _manualdiscountamount * _quantity;
                                            _priceperunit = (_priceperunit * _quantity) - (_manualdiscountamount);

                                            Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                            _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                            _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                            _opportunityproduct_child["new_typediscount"] = _typediscount;
                                            _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                                            _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                            _extendedamount = _priceperunit;

                                            _organizationservice.Update(_opportunityproduct_child);

                                            _extendedamountparent += _extendedamount;
                                        }
                                    }
                                }
                                else if (!_typediscount && _percentagediscount > 0) //TYPE DISCOUNT : PERCENTAGE
                                {
                                    foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                    {
                                        decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                        if (_parentnumberchild != null)
                                        {
                                            _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                            _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);
                                            _manualdiscountamount = _priceperunit * _percentagediscount / 100;

                                            if (_manualdiscountamount > (_priceperunit * _quantity))
                                                _manualdiscountamount = (_priceperunit * _quantity);

                                            _manualdiscountamount = _manualdiscountamount * _quantity;
                                            _priceperunit = (_priceperunit * _quantity) - (_manualdiscountamount);

                                            Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                            _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                            _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                            _opportunityproduct_child["new_typediscount"] = _typediscount;
                                            _opportunityproduct_child["new_percentagediscount"] = _percentagediscount;
                                            _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                                            _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                            _extendedamount = _priceperunit;

                                            _organizationservice.Update(_opportunityproduct_child);

                                            _extendedamountparent += _extendedamount;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                    {
                                        decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                        if (_parentnumberchild != null)
                                        {
                                            _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                            _manualdiscountamount = _priceperunit * _percentagediscount / 100;
                                            _priceperunit = _priceperunit - _manualdiscountamount;
                                            _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);

                                            Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                            _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                            _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                            _opportunityproduct_child["new_typediscount"] = _typediscount;
                                            _opportunityproduct_child["priceperunit"] = new Money(_priceperunit >= 0 ? _priceperunit : 0);
                                            _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                            _extendedamount = _priceperunit * _quantity;

                                            _organizationservice.Update(_opportunityproduct_child);

                                            _extendedamountparent += _extendedamount;
                                        }
                                    }
                                }
                            }
                        }

                        Entity _opportunityproduct_parents = new Entity(_entityname_opportunityproduct);
                        _opportunityproduct_parents.Id = _entity.Id;

                        if (!_entity.Contains("new_parentnumber"))
                            _opportunityproduct_parents["ittn_totalextendedamount"] = new Money(_extendedamountparent >= 0 ? _extendedamountparent : 0);
                        else
                            _opportunityproduct_parents["ittn_totalextendedamount"] = new Money(0);

                        _organizationservice.Update(_opportunityproduct_parents);
                    }
                }

                #region Unused - 31/10/2019 by Santony
                //_entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _context.PrimaryEntityId, new ColumnSet(true));
                //decimal _percentagediscount_parent = new decimal(0);
                //decimal _manualdiscountamount_parent = new decimal(0);
                //decimal _extendedamount_parent = new decimal(0);
                //decimal _baseamount_parent = _entity.GetAttributeValue<Money>("baseamount").Value;
                //bool _typediscount_parent = _entity.GetAttributeValue<bool>("new_typediscount");

                //if (_entity != null)
                //{
                //    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");

                //    if (_parentnumber != null)
                //    {
                //        QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                //        _queryexpression.ColumnSet = new ColumnSet(true);
                //        _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("opportunityid").Id);
                //        _queryexpression.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, _parentnumber);
                //        Entity _opportunityproduct_parent = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                //        if (_opportunityproduct_parent != null)
                //        {
                //            bool _typediscount = _opportunityproduct_parent.GetAttributeValue<bool>("new_typediscount");
                //            decimal _baseamount = _entity.GetAttributeValue<Money>("baseamount").Value;
                //            decimal _percentagediscount = new decimal(0);
                //            decimal _manualdiscountamount = new decimal(0);
                //            decimal _extendedamount = new decimal(0);

                //            if (_typediscount)
                //            {
                //                // MANUAL
                //                _manualdiscountamount = _opportunityproduct_parent.Attributes.Contains("manualdiscountamount") ? _opportunityproduct_parent.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);
                //                _percentagediscount = _manualdiscountamount / _baseamount * 100;
                //                _extendedamount = _baseamount - _manualdiscountamount;

                //                Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                //                _opportunityproduct_child.Id = _entity.Id;

                //                _opportunityproduct_child["new_typediscount"] = _typediscount;
                //                _opportunityproduct_child["new_percentagediscount"] = _percentagediscount;
                //                _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                //                _opportunityproduct_child["extendedamount"] = new Money(_extendedamount >= 0 ? _extendedamount : 0);

                //                _organizationservice.Update(_opportunityproduct_child);
                //            }
                //            else
                //            {
                //                // PERCENTAGE
                //                _percentagediscount = _opportunityproduct_parent.Attributes.Contains("new_percentagediscount") ? _opportunityproduct_parent.GetAttributeValue<decimal>("new_percentagediscount") : new decimal(0);
                //                _manualdiscountamount = _baseamount * _percentagediscount / 100;
                //                _extendedamount = _baseamount - _manualdiscountamount;

                //                Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                //                _opportunityproduct_child.Id = _entity.Id;

                //                _opportunityproduct_child["new_typediscount"] = _typediscount;
                //                _opportunityproduct_child["new_percentagediscount"] = _percentagediscount;
                //                _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                //                _opportunityproduct_child["extendedamount"] = new Money(_extendedamount >= 0 ? _extendedamount : 0);

                //                _organizationservice.Update(_opportunityproduct_child);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (_typediscount_parent)
                //        {
                //            _manualdiscountamount_parent = _entity.Attributes.Contains("manualdiscountamount") ? _entity.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);
                //            _percentagediscount_parent = _manualdiscountamount_parent / _baseamount_parent * 100;
                //            _extendedamount_parent = _baseamount_parent - _manualdiscountamount_parent;

                //            Entity _opportunityproductparent = new Entity(_entityname_opportunityproduct);
                //            _opportunityproductparent.Id = _entity.Id;
                //            _opportunityproductparent["ittn_totalextendedamount"] = new Money(_extendedamount_parent);

                //            //_organizationservice.Update(_opportunityproductparent);
                //        }
                //        else
                //        {
                //            _percentagediscount_parent = _entity.Attributes.Contains("new_percentagediscount") ? _entity.GetAttributeValue<decimal>("new_percentagediscount") : new decimal(0);
                //            _manualdiscountamount_parent = _baseamount_parent * _percentagediscount_parent / 100;
                //            _extendedamount_parent = _baseamount_parent - _manualdiscountamount_parent;

                //            Entity _opportunityproductparent = new Entity(_entityname_opportunityproduct);
                //            _opportunityproductparent.Id = _entity.Id;
                //            _opportunityproductparent["ittn_totalextendedamount"] = new Money(_extendedamount_parent);

                //            //_organizationservice.Update(_opportunityproductparent);
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".CreateOpportunityProductDiscountBasedOnParent: " + ex.Message.ToString());
            }
        }

        public void UpdateOpportunityProductDiscountBasedOnParent(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _context.PrimaryEntityId, new ColumnSet(true));

                if (_entity != null)
                {
                    decimal? _itemnumber = _entity.GetAttributeValue<decimal?>("new_itemnumber");
                    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");
                    decimal _pricelist = 0;
                    decimal _priceperunit = _entity.GetAttributeValue<Money>("priceperunit").Value;
                    decimal _percentagediscount = new decimal(0);
                    decimal _manualdiscountamount = new decimal(0);
                    decimal _extendedamountparent = _entity.GetAttributeValue<Money>("extendedamount").Value;
                    decimal _extendedamount = new decimal(0);
                    decimal _quantity = 0;

                    if (_parentnumber == null)
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("opportunityid").Id);
                        _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);
                        EntityCollection _opportunityproductschild = _organizationservice.RetrieveMultiple(_queryexpression);

                        if (_opportunityproductschild.Entities.Count > 0)
                        {
                            bool _ispriceoverriden = _entity.GetAttributeValue<bool>("ispriceoverridden");
                            bool _typediscount = _entity.GetAttributeValue<bool>("new_typediscount");
                            _percentagediscount = _entity.Attributes.Contains("new_percentagediscount") ? _entity.GetAttributeValue<decimal>("new_percentagediscount") : new decimal(0);
                            _manualdiscountamount = _entity.Attributes.Contains("manualdiscountamount") ? _entity.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);

                            if (_ispriceoverriden)  // PRICING : OVERRIDE PRICE
                            {
                                _queryexpression = new QueryExpression(_entityname_productpricelevel);
                                _queryexpression.ColumnSet = new ColumnSet(true);
                                _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("productid").Id);
                                _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id);

                                EntityCollection _productpricelevels = _organizationservice.RetrieveMultiple(_queryexpression);

                                if (_productpricelevels.Entities.Count() > 0)
                                {
                                    Entity _productpricelevel = _productpricelevels.Entities.FirstOrDefault();

                                    _pricelist = _productpricelevel.GetAttributeValue<Money>("amount").Value;
                                }

                                if (_pricelist > 0)
                                    _percentagediscount = (1 - (_priceperunit / _pricelist)) * 100;

                                foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                {
                                    decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                    if (_parentnumberchild != null)
                                    {
                                        _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                        _manualdiscountamount = _priceperunit * _percentagediscount / 100;
                                        _priceperunit = _priceperunit - _manualdiscountamount;
                                        _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);

                                        Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                        _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                        _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                        _opportunityproduct_child["new_typediscount"] = _typediscount;
                                        _opportunityproduct_child["priceperunit"] = new Money(_priceperunit >= 0 ? _priceperunit : 0);
                                        _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                        _extendedamount = _priceperunit * _quantity;

                                        _organizationservice.Update(_opportunityproduct_child);

                                        _extendedamountparent += _extendedamount;
                                    }
                                }
                            }
                            else  // PRICING : USE DEFAULT
                            {
                                if (_typediscount && _manualdiscountamount > 0) //TYPE DISCOUNT : WRITE-IN
                                {
                                    foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                    {
                                        decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                        if (_parentnumberchild != null)
                                        {
                                            _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                            _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);
                                            _manualdiscountamount = _entity.Attributes.Contains("manualdiscountamount") ? _entity.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);

                                            if (_manualdiscountamount > (_priceperunit * _quantity))
                                                _manualdiscountamount = (_priceperunit * _quantity);

                                            _manualdiscountamount = _manualdiscountamount * _quantity;
                                            _priceperunit = (_priceperunit * _quantity) - (_manualdiscountamount);

                                            Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                            _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                            _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                            _opportunityproduct_child["new_typediscount"] = _typediscount;
                                            _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                                            _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                            _extendedamount = _priceperunit;

                                            _organizationservice.Update(_opportunityproduct_child);

                                            _extendedamountparent += _extendedamount;
                                        }
                                    }
                                }
                                else if (!_typediscount && _percentagediscount > 0) //TYPE DISCOUNT : PERCENTAGE
                                {
                                    foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                    {
                                        decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                        if (_parentnumberchild != null)
                                        {
                                            _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                            _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);
                                            _manualdiscountamount = _priceperunit * _percentagediscount / 100;

                                            if (_manualdiscountamount > (_priceperunit * _quantity))
                                                _manualdiscountamount = (_priceperunit * _quantity);

                                            _manualdiscountamount = _manualdiscountamount * _quantity;
                                            _priceperunit = (_priceperunit * _quantity) - (_manualdiscountamount);

                                            Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                            _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                            _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                            _opportunityproduct_child["new_typediscount"] = _typediscount;
                                            _opportunityproduct_child["new_percentagediscount"] = _percentagediscount;
                                            _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                                            _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                            _extendedamount = _priceperunit;

                                            _organizationservice.Update(_opportunityproduct_child);

                                            _extendedamountparent += _extendedamount;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var _opportunityproductchild in _opportunityproductschild.Entities)
                                    {
                                        decimal? _parentnumberchild = _opportunityproductchild.GetAttributeValue<decimal?>("new_parentnumber");

                                        if (_parentnumberchild != null)
                                        {
                                            _priceperunit = _opportunityproductchild.Attributes.Contains("priceperunit") ? _opportunityproductchild.GetAttributeValue<Money>("priceperunit").Value : new decimal(0);
                                            _manualdiscountamount = _priceperunit * _percentagediscount / 100;
                                            _priceperunit = _priceperunit - _manualdiscountamount;
                                            _quantity = _opportunityproductchild.Attributes.Contains("quantity") ? _opportunityproductchild.GetAttributeValue<decimal>("quantity") : new decimal(0);

                                            Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                                            _opportunityproduct_child.Id = _opportunityproductchild.Id;

                                            _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                                            _opportunityproduct_child["new_typediscount"] = _typediscount;
                                            _opportunityproduct_child["priceperunit"] = new Money(_priceperunit >= 0 ? _priceperunit : 0);
                                            _opportunityproduct_child["ittn_totalextendedamount"] = new Money(0);
                                            _extendedamount = _priceperunit * _quantity;

                                            _organizationservice.Update(_opportunityproduct_child);

                                            _extendedamountparent += _extendedamount;
                                        }
                                    }
                                }
                            }
                        }

                        Entity _opportunityproduct_parents = new Entity(_entityname_opportunityproduct);
                        _opportunityproduct_parents.Id = _entity.Id;

                        if (!_entity.Contains("new_parentnumber"))
                            _opportunityproduct_parents["ittn_totalextendedamount"] = new Money(_extendedamountparent >= 0 ? _extendedamountparent : 0);
                        else
                            _opportunityproduct_parents["ittn_totalextendedamount"] = new Money(0);

                        _organizationservice.Update(_opportunityproduct_parents);
                    }
                }

                #region Unused - 06/12/2019 by Santony
                //if (_entity != null)
                //{
                //    decimal _itemnumber = _entity.GetAttributeValue<decimal>("new_itemnumber");
                //    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");
                //    bool _ispriceoverriden = _entity.GetAttributeValue<bool>("ispriceoverridden");
                //    bool _typediscount = _entity.GetAttributeValue<bool>("new_typediscount");
                //    decimal _baseamount = _entity.GetAttributeValue<Money>("baseamount").Value;
                //    decimal _percentagediscount = new decimal(0);
                //    decimal _manualdiscountamount = new decimal(0);
                //    decimal _extendedamountparent = new decimal(0);
                //    decimal _extendedamount = new decimal(0);
                //    decimal _totalbaseamount = 0;

                //    QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                //    _queryexpression.ColumnSet = new ColumnSet(true);
                //    _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("opportunityid").Id);
                //    _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);
                //    EntityCollection _opportunityproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                //    if (_ispriceoverriden) // PRICING : OVERRIDE PRICE
                //    {
                //        _queryexpression = new QueryExpression(_entityname_productpricelevel);
                //        _queryexpression.ColumnSet = new ColumnSet(true);
                //        _queryexpression.Criteria.AddCondition("productid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("productid").Id);
                //        _queryexpression.Criteria.AddCondition("transactioncurrencyid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("transactioncurrencyid").Id);

                //        EntityCollection _productpricelevels = _organizationservice.RetrieveMultiple(_queryexpression);

                //        if (_productpricelevels.Entities.Count() > 0)
                //        {
                //            Entity _productpricelevel = _productpricelevels.Entities.FirstOrDefault();

                //            _totalbaseamount += _productpricelevel.GetAttributeValue<Money>("amount").Value;
                //        }

                //        if (_totalbaseamount > 0)
                //            _percentagediscount = (1 - (_baseamount / _totalbaseamount)) * 100;

                //        foreach (var _opportunityproduct in _opportunityproducts.Entities)
                //        {
                //            decimal? _parentnumberchild = _opportunityproduct.GetAttributeValue<decimal?>("new_parentnumber");

                //            if (_parentnumberchild != null)
                //            {
                //                _baseamount = _opportunityproduct.Attributes.Contains("baseamount") ? _opportunityproduct.GetAttributeValue<Money>("baseamount").Value : new decimal(0);
                //                _manualdiscountamount = _baseamount * _percentagediscount / 100;
                //                _extendedamount = _baseamount - _manualdiscountamount;

                //                Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                //                _opportunityproduct_child.Id = _opportunityproduct.Id;

                //                _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                //                _opportunityproduct_child["new_typediscount"] = _typediscount;
                //                _opportunityproduct_child["extendedamount"] = new Money(_extendedamount >= 0 ? _extendedamount : 0);

                //                _organizationservice.Update(_opportunityproduct_child);

                //                _extendedamountparent += _extendedamount;
                //            }
                //        }

                //        Entity _opportunityproduct_parents = new Entity(_entityname_opportunityproduct);
                //        _opportunityproduct_parents.Id = _entity.Id;

                //        if (!_entity.Contains("new_parentnumber"))
                //            _opportunityproduct_parents["ittn_totalextendedamount"] = new Money(_extendedamountparent >= 0 ? _extendedamountparent : 0);
                //        else
                //            _opportunityproduct_parents["ittn_totalextendedamount"] = new Money(0);

                //        _organizationservice.Update(_opportunityproduct_parents);
                //    }
                //    else // PRICING : USE DEFAULT
                //    {
                //        if (_typediscount)
                //        {
                //            // MANUAL
                //            _manualdiscountamount = _entity.Attributes.Contains("manualdiscountamount") ? _entity.GetAttributeValue<Money>("manualdiscountamount").Value : new decimal(0);
                //            _percentagediscount = _baseamount > 0 ? _manualdiscountamount / _baseamount * 100 : 0;
                //            _extendedamountparent = _baseamount - _manualdiscountamount;

                //            Entity _opportunityproduct_parent = new Entity(_entityname_opportunityproduct);
                //            _opportunityproduct_parent.Id = _entity.Id;

                //            _opportunityproduct_parent["new_percentagediscount"] = _percentagediscount;
                //            //_opportunityproduct_parent["manualdiscountamount"] = new Money(_manualdiscountamount);
                //            _opportunityproduct_parent["extendedamount"] = new Money(_extendedamountparent);

                //            foreach (var _opportunityproduct in _opportunityproducts.Entities)
                //            {
                //                decimal? _parentnumberchild = _opportunityproduct.GetAttributeValue<decimal?>("new_parentnumber");

                //                if (_parentnumberchild != null)
                //                {
                //                    _baseamount = _opportunityproduct.Attributes.Contains("baseamount") ? _opportunityproduct.GetAttributeValue<Money>("baseamount").Value : new decimal(0);
                //                    _manualdiscountamount = _baseamount * _percentagediscount / 100;
                //                    //_percentagediscount = _baseamount > 0 ? _manualdiscountamount / _baseamount * 100 : 0;
                //                    _extendedamount = _baseamount - _manualdiscountamount;

                //                    Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                //                    _opportunityproduct_child.Id = _opportunityproduct.Id;

                //                    _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                //                    _opportunityproduct_child["new_typediscount"] = _typediscount;
                //                    _opportunityproduct_child["new_percentagediscount"] = _percentagediscount;
                //                    _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                //                    _opportunityproduct_child["extendedamount"] = new Money(_extendedamount >= 0 ? _extendedamount : 0);

                //                    _organizationservice.Update(_opportunityproduct_child);

                //                    _extendedamountparent += _extendedamount;
                //                }
                //            }

                //            if (!_entity.Contains("new_parentnumber"))
                //                _opportunityproduct_parent["ittn_totalextendedamount"] = new Money(_extendedamountparent >= 0 ? _extendedamountparent : 0);
                //            else
                //                _opportunityproduct_parent["ittn_totalextendedamount"] = new Money(0);

                //            _organizationservice.Update(_opportunityproduct_parent);
                //        }
                //        else
                //        {
                //            // PERCENTAGE
                //            _percentagediscount = _entity.Attributes.Contains("new_percentagediscount") ? _entity.GetAttributeValue<decimal>("new_percentagediscount") : new decimal(0);
                //            _manualdiscountamount = _baseamount * _percentagediscount / 100;
                //            _extendedamountparent = _baseamount - _manualdiscountamount;

                //            Entity _opportunityproduct_parent = new Entity(_entityname_opportunityproduct);
                //            _opportunityproduct_parent.Id = _entity.Id;

                //            //_opportunityproduct_parent["new_percentagediscount"] = _percentagediscount;
                //            _opportunityproduct_parent["manualdiscountamount"] = new Money(_manualdiscountamount);
                //            _opportunityproduct_parent["extendedamount"] = new Money(_extendedamountparent);

                //            foreach (var _opportunityproduct in _opportunityproducts.Entities)
                //            {
                //                decimal? _parentnumberchild = _opportunityproduct.GetAttributeValue<decimal?>("new_parentnumber");

                //                if (_parentnumberchild != null)
                //                {
                //                    _baseamount = _opportunityproduct.Attributes.Contains("baseamount") ? _opportunityproduct.GetAttributeValue<Money>("baseamount").Value : new decimal(0);
                //                    //_percentagediscount = _opportunityproduct.GetAttributeValue<decimal>("new_percentagediscount");
                //                    _manualdiscountamount = _baseamount * _percentagediscount / 100;
                //                    _extendedamount = _baseamount - _manualdiscountamount;

                //                    Entity _opportunityproduct_child = new Entity(_entityname_opportunityproduct);
                //                    _opportunityproduct_child.Id = _opportunityproduct.Id;

                //                    _opportunityproduct_child["ispriceoverridden"] = _ispriceoverriden;
                //                    _opportunityproduct_child["new_typediscount"] = _typediscount;
                //                    _opportunityproduct_child["new_percentagediscount"] = _percentagediscount;
                //                    _opportunityproduct_child["manualdiscountamount"] = new Money(_manualdiscountamount);
                //                    _opportunityproduct_child["extendedamount"] = new Money(_extendedamount >= 0 ? _extendedamount : 0);

                //                    _organizationservice.Update(_opportunityproduct_child);

                //                    _extendedamountparent += _extendedamount;
                //                }
                //            }

                //            if (!_entity.Contains("new_parentnumber"))
                //                _opportunityproduct_parent["ittn_totalextendedamount"] = new Money(_extendedamountparent >= 0 ? _extendedamountparent : 0);
                //            else
                //                _opportunityproduct_parent["ittn_totalextendedamount"] = new Money(0);

                //            _organizationservice.Update(_opportunityproduct_parent);
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".UpdateOpportunityProductDiscountBasedOnParent: " + ex.Message.ToString());
            }
        }

        public void PreDelete_opportunityproduct(IOrganizationService _organizationservice, IPluginExecutionContext _context, ITracingService _tracer)
        {
            try
            {
                Entity _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _context.PrimaryEntityId, new ColumnSet(true));

                if (_entity != null)
                {
                    Guid _opportunityid = _entity.GetAttributeValue<EntityReference>("opportunityid").Id;
                    decimal _itemnumber = _entity.GetAttributeValue<decimal>("new_itemnumber");
                    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");
                    bool? _mandatory = _entity.GetAttributeValue<bool?>("ittn_mandatory");

                    if (_parentnumber != null && (bool)_mandatory)
                    {
                        throw new InvalidPluginExecutionException("Product is mandatory and cannot be deleted !");
                    }
                    else
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _opportunityid);
                        _queryexpression.Criteria.AddCondition("new_parentnumber", ConditionOperator.Equal, _itemnumber);
                        EntityCollection _opportunityproducts = _organizationservice.RetrieveMultiple(_queryexpression);

                        foreach (var _opportunityproduct in _opportunityproducts.Entities)
                        {
                            Entity _opportunityproduct_update = new Entity(_entityname_opportunityproduct);
                            _opportunityproduct_update.Id = _opportunityproduct.Id;

                            _opportunityproduct_update["ittn_mandatory"] = false;
                            _organizationservice.Update(_opportunityproduct_update);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PreDelete_opportunityproduct: " + ex.Message.ToString());
            }
        }

        public void PostCreate_OpportunityProductChild_RollupToMainProduct(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _context.PrimaryEntityId, new ColumnSet(true));

                if (_entity != null)
                {
                    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");
                    decimal _extendedamountchild = _entity.GetAttributeValue<Money>("extendedamount").Value;

                    if (_parentnumber != null && _extendedamountchild > 0)
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("opportunityid").Id);
                        _queryexpression.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, _parentnumber);
                        Entity _opportunityproduct_parent = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                        if (_opportunityproduct_parent != null)
                        {
                            decimal _extendedamountparent = 0;

                            if (_opportunityproduct_parent.Contains("ittn_totalextendedamount") && _opportunityproduct_parent.Attributes["ittn_totalextendedamount"] != null)
                                _extendedamountparent = _opportunityproduct_parent.GetAttributeValue<Money>("ittn_totalextendedamount").Value;
                            else
                                _extendedamountparent = _opportunityproduct_parent.GetAttributeValue<Money>("extendedamount").Value;

                            Entity _opportunityproduct_parent_update = new Entity(_entityname_opportunityproduct);
                            _opportunityproduct_parent_update.Id = _opportunityproduct_parent.Id;

                            _opportunityproduct_parent_update["ittn_totalextendedamount"] = new Money(_extendedamountparent + _extendedamountchild);

                            _organizationservice.Update(_opportunityproduct_parent_update);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostCreate_OpportunityProductChild_RollupToMainProduct: " + ex.Message.ToString());
            }
        }

        public void PostUpdate_OpportunityProductChild_RollupToMainProduct(IOrganizationService _organizationservice, IPluginExecutionContext _context, Entity _entity, ITracingService _tracer)
        {
            try
            {
                _entity = _organizationservice.Retrieve(_entityname_opportunityproduct, _context.PrimaryEntityId, new ColumnSet(true));

                if (_entity != null)
                {
                    decimal? _parentnumber = _entity.GetAttributeValue<decimal?>("new_parentnumber");
                    decimal _extendedamountchild = _entity.GetAttributeValue<Money>("extendedamount").Value;

                    if (_parentnumber != null && _extendedamountchild > 0)
                    {
                        QueryExpression _queryexpression = new QueryExpression(_entityname_opportunityproduct);
                        _queryexpression.ColumnSet = new ColumnSet(true);
                        _queryexpression.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, _entity.GetAttributeValue<EntityReference>("opportunityid").Id);
                        _queryexpression.Criteria.AddCondition("new_itemnumber", ConditionOperator.Equal, _parentnumber);
                        Entity _opportunityproduct_parent = _organizationservice.RetrieveMultiple(_queryexpression).Entities.FirstOrDefault();

                        if (_opportunityproduct_parent != null)
                        {
                            decimal _extendedamountparent = 0;

                            if (_opportunityproduct_parent.Contains("ittn_totalextendedamount") && _opportunityproduct_parent.Attributes["ittn_totalextendedamount"] != null)
                                _extendedamountparent = _opportunityproduct_parent.GetAttributeValue<Money>("ittn_totalextendedamount").Value;
                            else
                                _extendedamountparent = _opportunityproduct_parent.GetAttributeValue<Money>("extendedamount").Value;

                            Entity _opportunityproduct_parent_update = new Entity(_entityname_opportunityproduct);
                            _opportunityproduct_parent_update.Id = _opportunityproduct_parent.Id;

                            _opportunityproduct_parent_update["ittn_totalextendedamount"] = new Money(_extendedamountparent + _extendedamountchild);

                            _organizationservice.Update(_opportunityproduct_parent_update);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(_classname + ".PostUpdate_OpportunityProductChild_RollupToMainProduct: " + ex.Message.ToString());
            }
        }
    }
}
