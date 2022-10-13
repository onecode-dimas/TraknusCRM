using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrakNusSparepartSystem.DataLayer;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace TrakNusSparepartSystem.WorkflowActivity.Business_Layer
{
    public class BL_tss_marketsizesummarybygroupuiocommodity
    {
        #region CONSTANT
        private const int PART_COMMODITYTYPE_BATTERY = 865920000;
        private const int PART_COMMODITYTYPE_OIL = 865920001;
        private const int PART_COMMODITYTYPE_TYRE = 865920002;
        private const int PART_COMMODITYTYPE_BY_PART = 865920000;
        private const int PART_COMMODITYTYPE_BY_SPEC = 865920001;

        private const int MS_TYPE = 865920001;
        #endregion

        #region DEPENDENCIES
        DL_tss_sparepartpricemaster _DL_tss_sparepartpricemaster = new DL_tss_sparepartpricemaster();
        DL_tss_pricelistpart _DL_tss_pricelistpart = new DL_tss_pricelistpart();
        DL_tss_kagroupuiocommodity _DL_tss_kagroupuiocommodity = new DL_tss_kagroupuiocommodity();
        DL_tss_groupuiocommodityaccount _DL_tss_groupuiocommodityaccount = new DL_tss_groupuiocommodityaccount();
        DL_tss_groupuiocommodity _DL_tss_groupuiocommodity = new DL_tss_groupuiocommodity();
        #endregion

        //public void GenerateMarketSizeSummaryByGroupUIOCommodity(IOrganizationService organizationService, IWorkflowContext context, EntityCollection listKeyAccount)
        //{
        //    object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();

        //    #region GET KA GROUP UIO COMMODITY
        //    QueryExpression qKAGroupUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
        //    qKAGroupUIOCommodity.Criteria.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
        //    qKAGroupUIOCommodity.Criteria.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
        //    qKAGroupUIOCommodity.Criteria.AddCondition("tss_calculatestatus", ConditionOperator.Equal, true);
        //    qKAGroupUIOCommodity.ColumnSet = new ColumnSet(true);
        //    EntityCollection oKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qKAGroupUIOCommodity);
        //    #endregion

        //    #region GET GROUP UIO COMMODITY ACCOUNT
        //    object[] accountIds = oKAGroupUIOCommodity.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).Select(x => (object)x.First().GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

        //    if (accountIds.Count() > 0)
        //    {
        //        QueryExpression qGroupUIOCommodityAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
        //        qGroupUIOCommodityAccount.Criteria.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, accountIds);
        //        qGroupUIOCommodityAccount.ColumnSet = new ColumnSet(true);
        //        EntityCollection oGroupUIOCommodityAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupUIOCommodityAccount);
        //        #endregion

        //        #region GET GROUP UIO COMMODITY HEADER
        //        object[] headerIds = oGroupUIOCommodityAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id).Select(x => (object)x.First().GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id).ToArray();

        //        QueryExpression qGroupUIOCommodityHeader = new QueryExpression("tss_groupuiocommodityheader");
        //        qGroupUIOCommodityHeader.Criteria.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, headerIds);
        //        qGroupUIOCommodityHeader.ColumnSet = new ColumnSet(true);
        //        EntityCollection oGroupUIOCommodityHeader = organizationService.RetrieveMultiple(qGroupUIOCommodityHeader);
        //        #endregion

        //        foreach (var header in oGroupUIOCommodityHeader.Entities)
        //        {
        //            Guid oPN = new Guid();

        //            decimal battery = 0m;
        //            decimal oil = 0m;
        //            decimal tyre = 0m;

        //            int batteryQty = 0;
        //            int oilQty = 0;
        //            int tyreQty = 0;

        //            int batteryQtyTotal = 0;
        //            int oilQtyTotal = 0;
        //            int tyreQtyTotal = 0;

        //            #region GET GROUP UIO COMMODITY
        //            QueryExpression qCommodity = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
        //            qCommodity.Criteria.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.Equal, header.Id);
        //            qCommodity.ColumnSet = new ColumnSet(true);
        //            EntityCollection oCommodity = _DL_tss_groupuiocommodity.Select(organizationService, qCommodity);
        //            #endregion

        //            foreach (var currentGroupUIOCommodity in oCommodity.Entities)
        //            {
        //                if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_BATTERY)
        //                {
        //                    Guid oBatteryPartNumber = new Guid();

        //                    if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batteryby").Value == PART_COMMODITYTYPE_BY_PART)
        //                    {
        //                        oBatteryPartNumber = currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_batterypartnumber").Id;

        //                        if (oBatteryPartNumber != null)
        //                        {
        //                            oPN = oBatteryPartNumber;
        //                        }
        //                    }
        //                    else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batteryby").Value == PART_COMMODITYTYPE_BY_SPEC)
        //                    {
        //                        //BATTERY TYPE - TRACTION / CRANKING
        //                        if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batterytype").Value == 865920000)
        //                        {
        //                            QueryExpression oFilter = new QueryExpression("tss_partmasterlinesbattery");
        //                            oFilter.ColumnSet = new ColumnSet(true);
        //                            oFilter.Criteria = new FilterExpression
        //                            {
        //                                FilterOperator = LogicalOperator.And,
        //                                Conditions =
        //                                {
        //                                    new ConditionExpression("tss_batterymodeltraction", ConditionOperator.Equal, currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_batterytraction").Id)
        //                                }
        //                            };
        //                            EntityCollection oTraction = organizationService.RetrieveMultiple(oFilter);

        //                            if (oTraction.Entities.Count() > 0)
        //                            {
        //                                oPN = oTraction.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                                oBatteryPartNumber = oPN;
        //                            }
        //                        }
        //                        else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batterytype").Value == 865920001)
        //                        {
        //                            QueryExpression oFilter = new QueryExpression("tss_partmasterlinesbattery");
        //                            oFilter.ColumnSet = new ColumnSet(true);
        //                            oFilter.Criteria = new FilterExpression
        //                            {
        //                                FilterOperator = LogicalOperator.And,
        //                                Conditions =
        //                                {
        //                                    new ConditionExpression("tss_batterymodelcranking", ConditionOperator.Equal, currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_batterycranking").Id)
        //                                }
        //                            };
        //                            EntityCollection oCranking = organizationService.RetrieveMultiple(oFilter);

        //                            if (oCranking.Entities.Count() > 0)
        //                            {
        //                                oPN = oCranking.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                                oBatteryPartNumber = oPN;
        //                            }
        //                        }
        //                    }

        //                    batteryQty = currentGroupUIOCommodity.GetAttributeValue<int>("tss_battery");
        //                    batteryQtyTotal += batteryQty;

        //                    if (oBatteryPartNumber != new Guid())
        //                    {
        //                        DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oBatteryPartNumber);

        //                        battery += (batteryQty * oResult.tss_price);

        //                        //_DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
        //                        //_DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
        //                        //_DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
        //                    }
        //                }
        //                else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_OIL)
        //                {
        //                    Guid oOilPartNumber = new Guid();

        //                    if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilby").Value == PART_COMMODITYTYPE_BY_PART)
        //                    {
        //                        oOilPartNumber = currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_oilpartnumber").Id;

        //                        if (oOilPartNumber != null)
        //                        {
        //                            oPN = oOilPartNumber;
        //                        }
        //                    }
        //                    else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilby").Value == PART_COMMODITYTYPE_BY_SPEC)
        //                    {
        //                        QueryExpression oFilter = new QueryExpression("tss_partmasterlinesoil");
        //                        oFilter.ColumnSet = new ColumnSet(true);
        //                        oFilter.Criteria = new FilterExpression
        //                        {
        //                            FilterOperator = LogicalOperator.And,
        //                            Conditions =
        //                    {
        //                        new ConditionExpression("tss_oiltype", ConditionOperator.Equal, currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_jenisoil").Id)
        //                    }
        //                        };
        //                        EntityCollection oOilType = organizationService.RetrieveMultiple(oFilter);

        //                        if (oOilType.Entities.Count() > 0)
        //                        {
        //                            oPN = oOilType.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                            oOilPartNumber = oPN;
        //                        }
        //                    }

        //                    if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920000)
        //                    {
        //                        oilQty = currentGroupUIOCommodity.GetAttributeValue<int>("tss_oilqtypcs");
        //                    }
        //                    else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920001)
        //                    {
        //                        oilQty = currentGroupUIOCommodity.GetAttributeValue<int>("tss_oil");
        //                    }

        //                    oilQtyTotal += oilQty;

        //                    if (oOilPartNumber != new Guid())
        //                    {
        //                        DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oOilPartNumber);

        //                        oil += (oilQty * oResult.tss_price);

        //                        //_DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
        //                        //_DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
        //                        //_DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
        //                    }
        //                }
        //                else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_TYRE)
        //                {
        //                    Guid oTyrePartNumber = new Guid();

        //                    if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_tyreby").Value == PART_COMMODITYTYPE_BY_PART)
        //                    {
        //                        oTyrePartNumber = currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_tyrepartnumber").Id;

        //                        if (oTyrePartNumber != null)
        //                        {
        //                            oPN = oTyrePartNumber;
        //                        }
        //                    }
        //                    else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_tyreby").Value == PART_COMMODITYTYPE_BY_SPEC)
        //                    {
        //                        Entity _tss_partmasterlinestyretype = organizationService.Retrieve("tss_partmasterlinestyretype", currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_tyrespec").Id, new ColumnSet(true));

        //                        if (_tss_partmasterlinestyretype != null)
        //                        {
        //                            oPN = _tss_partmasterlinestyretype.GetAttributeValue<EntityReference>("tss_partmasterid").Id;
        //                            oTyrePartNumber = oPN;
        //                        }
        //                    }

        //                    tyreQty = currentGroupUIOCommodity.GetAttributeValue<int>("tss_tyre");
        //                    tyreQtyTotal += tyreQty;

        //                    if (oTyrePartNumber != new Guid())
        //                    {
        //                        DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oTyrePartNumber);

        //                        tyre += (tyreQty * oResult.tss_price);

        //                        //_DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
        //                        //_DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
        //                        //_DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
        //                    }
        //                }

        //                //if (commodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == COMMODITY_BATTERY)
        //                //{
        //                //    batteryQty += commodity.GetAttributeValue<int>("tss_battery");

        //                //    if (oBatteryPartNumber != new Guid())
        //                //    {
        //                //        DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oBatteryPartNumber);

        //                //        _DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
        //                //        _DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
        //                //        _DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
        //                //    }
        //                //}
        //                //else if (commodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == COMMODITY_OIL)
        //                //{
        //                //    if (commodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920000)
        //                //    {
        //                //        oilQty += commodity.GetAttributeValue<int>("tss_oilqtypcs");
        //                //    }
        //                //    else if (commodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920001)
        //                //    {
        //                //        oilQty += commodity.GetAttributeValue<int>("tss_oil");
        //                //    }
        //                //}
        //                //else if (commodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == COMMODITY_TYRE)
        //                //{
        //                //    tyreQty += commodity.GetAttributeValue<int>("tss_tyre");
        //                //}
        //            }

        //            #region INSERT
        //            FilterExpression fExist = new FilterExpression(LogicalOperator.And);
        //            fExist.AddCondition("tss_groupuiocommodityheader", ConditionOperator.Equal, header.Id);

        //            QueryExpression qExist = new QueryExpression("tss_marketsizesummarybygroupuiocommodity");
        //            qExist.Criteria.AddFilter(fExist);
        //            qExist.ColumnSet = new ColumnSet(true);

        //            EntityCollection totalSummaryByPartTypeQ = organizationService.RetrieveMultiple(qExist);

        //            if (totalSummaryByPartTypeQ.Entities.Count() == 0)
        //            {
        //                Entity en_MarketSizeSummarybyGroupUIOCommodity = new Entity("tss_marketsizesummarybygroupuiocommodity");

        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_groupuiocommodity"] = "";
        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_groupuiocommodityheader"] = new EntityReference("tss_groupuiocommodityheader", header.Id);

        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_amountbattery"] = new Money(battery);
        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_amountoil"] = new Money(oil);
        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_amounttyre"] = new Money(tyre);

        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_qtybattery"] = batteryQtyTotal;
        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_qtyoil"] = oilQtyTotal;
        //                en_MarketSizeSummarybyGroupUIOCommodity["tss_qtytyre"] = tyreQtyTotal;

        //                organizationService.Create(en_MarketSizeSummarybyGroupUIOCommodity);
        //            }
        //            else
        //            {
        //                totalSummaryByPartTypeQ.Entities[0]["tss_amountbattery"] = new Money(battery);
        //                totalSummaryByPartTypeQ.Entities[0]["tss_amountoil"] = new Money(oil);
        //                totalSummaryByPartTypeQ.Entities[0]["tss_amounttyre"] = new Money(tyre);

        //                totalSummaryByPartTypeQ.Entities[0]["tss_qtybattery"] = batteryQtyTotal;
        //                totalSummaryByPartTypeQ.Entities[0]["tss_qtyoil"] = oilQtyTotal;
        //                totalSummaryByPartTypeQ.Entities[0]["tss_qtytyre"] = tyreQtyTotal;

        //                organizationService.Update(totalSummaryByPartTypeQ.Entities[0]);
        //            }
        //            #endregion
        //        }
        //    }
        //}

        public void GenerateMarketSizeSummaryByGroupUIOCommodity(IOrganizationService organizationService, IWorkflowContext context, EntityCollection listKeyAccount)
        {
            object[] kaIds = listKeyAccount.Entities.Select(x => (object)x.Id).ToArray();

            #region GET KA GROUP UIO COMMODITY
            QueryExpression qKAGroupUIOCommodity = new QueryExpression(_DL_tss_kagroupuiocommodity.EntityName);
            qKAGroupUIOCommodity.Criteria.AddCondition("tss_keyaccountid", ConditionOperator.In, kaIds);
            qKAGroupUIOCommodity.Criteria.AddCondition("tss_calculatetoms", ConditionOperator.Equal, true);
            qKAGroupUIOCommodity.Criteria.AddCondition("tss_calculatestatus", ConditionOperator.Equal, true);
            qKAGroupUIOCommodity.ColumnSet = new ColumnSet(true);
            EntityCollection oKAGroupUIOCommodity = _DL_tss_kagroupuiocommodity.Select(organizationService, qKAGroupUIOCommodity);
            #endregion

            #region GET GROUP UIO COMMODITY ACCOUNT
            object[] accountIds = oKAGroupUIOCommodity.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).Select(x => (object)x.First().GetAttributeValue<EntityReference>("tss_groupuiocommodity").Id).ToArray();

            if (accountIds.Count() > 0)
            {
                QueryExpression qGroupUIOCommodityAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
                qGroupUIOCommodityAccount.Criteria.AddCondition("tss_groupuiocommodityaccountid", ConditionOperator.In, accountIds);
                qGroupUIOCommodityAccount.ColumnSet = new ColumnSet(true);
                EntityCollection oGroupUIOCommodityAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupUIOCommodityAccount);
                #endregion

                #region GET GROUP UIO COMMODITY HEADER
                object[] headerIds = oGroupUIOCommodityAccount.Entities.GroupBy(x => x.GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id).Select(x => (object)x.First().GetAttributeValue<EntityReference>("tss_groupuiocommodityheader").Id).ToArray();

                QueryExpression qGroupUIOCommodityHeader = new QueryExpression("tss_groupuiocommodityheader");
                qGroupUIOCommodityHeader.Criteria.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.In, headerIds);
                qGroupUIOCommodityHeader.ColumnSet = new ColumnSet(true);
                EntityCollection oGroupUIOCommodityHeader = organizationService.RetrieveMultiple(qGroupUIOCommodityHeader);
                #endregion

                foreach (var header in oGroupUIOCommodityHeader.Entities)
                {
                    Guid oPN = new Guid();

                    decimal battery = 0m;
                    decimal oil = 0m;
                    decimal tyre = 0m;

                    int batteryQty = 0;
                    int oilQty = 0;
                    int tyreQty = 0;

                    int batteryQtyTotal = 0;
                    int oilQtyTotal = 0;
                    int tyreQtyTotal = 0;

                    #region GET GROUP UIO COMMODITY
                    QueryExpression qCommodity = new QueryExpression(_DL_tss_groupuiocommodity.EntityName);
                    qCommodity.Criteria.AddCondition("tss_groupuiocommodityheaderid", ConditionOperator.Equal, header.Id);
                    qCommodity.ColumnSet = new ColumnSet(true);
                    EntityCollection oCommodity = _DL_tss_groupuiocommodity.Select(organizationService, qCommodity);
                    #endregion

                    qGroupUIOCommodityAccount = new QueryExpression(_DL_tss_groupuiocommodityaccount.EntityName);
                    qGroupUIOCommodityAccount.Criteria.AddCondition("tss_groupuiocommodityheader", ConditionOperator.Equal, header.Id);
                    qGroupUIOCommodityAccount.ColumnSet = new ColumnSet(true);
                    oGroupUIOCommodityAccount = _DL_tss_groupuiocommodityaccount.Select(organizationService, qGroupUIOCommodityAccount);
                    int oQtyAccount = oGroupUIOCommodityAccount.Entities[0].GetAttributeValue<int>("tss_qty");

                    foreach (var currentGroupUIOCommodity in oCommodity.Entities)
                    {
                        if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_BATTERY)
                        {
                            Guid oBatteryPartNumber = new Guid();

                            if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batteryby").Value == PART_COMMODITYTYPE_BY_PART)
                            {
                                oBatteryPartNumber = currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_batterypartnumber").Id;

                                if (oBatteryPartNumber != null)
                                {
                                    oPN = oBatteryPartNumber;
                                }
                            }
                            else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batteryby").Value == PART_COMMODITYTYPE_BY_SPEC)
                            {
                                //BATTERY TYPE - TRACTION / CRANKING
                                if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batterytype").Value == 865920000)
                                {
                                    QueryExpression oFilter = new QueryExpression("tss_partmasterlinesbattery");
                                    oFilter.ColumnSet = new ColumnSet(true);
                                    oFilter.Criteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_batterymodeltraction", ConditionOperator.Equal, currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_batterytraction").Id)
                                        }
                                    };
                                    EntityCollection oTraction = organizationService.RetrieveMultiple(oFilter);

                                    if (oTraction.Entities.Count() > 0)
                                    {
                                        oPN = oTraction.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                                        oBatteryPartNumber = oPN;
                                    }
                                }
                                else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_batterytype").Value == 865920001)
                                {
                                    QueryExpression oFilter = new QueryExpression("tss_partmasterlinesbattery");
                                    oFilter.ColumnSet = new ColumnSet(true);
                                    oFilter.Criteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                        Conditions =
                                        {
                                            new ConditionExpression("tss_batterymodelcranking", ConditionOperator.Equal, currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_batterycranking").Id)
                                        }
                                    };
                                    EntityCollection oCranking = organizationService.RetrieveMultiple(oFilter);

                                    if (oCranking.Entities.Count() > 0)
                                    {
                                        oPN = oCranking.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                                        oBatteryPartNumber = oPN;
                                    }
                                }
                            }

                            batteryQty = (oQtyAccount * currentGroupUIOCommodity.GetAttributeValue<int>("tss_battery"));
                            batteryQtyTotal += batteryQty;

                            if (oBatteryPartNumber != new Guid())
                            {
                                DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oBatteryPartNumber);

                                battery += (batteryQty * oResult.tss_price);

                                //_DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
                                //_DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
                                //_DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
                            }
                        }
                        else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_OIL)
                        {
                            Guid oOilPartNumber = new Guid();

                            if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilby").Value == PART_COMMODITYTYPE_BY_PART)
                            {
                                oOilPartNumber = currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_oilpartnumber").Id;

                                if (oOilPartNumber != null)
                                {
                                    oPN = oOilPartNumber;
                                }
                            }
                            else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilby").Value == PART_COMMODITYTYPE_BY_SPEC)
                            {
                                QueryExpression oFilter = new QueryExpression("tss_partmasterlinesoil");
                                oFilter.ColumnSet = new ColumnSet(true);
                                oFilter.Criteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                            {
                                new ConditionExpression("tss_oiltype", ConditionOperator.Equal, currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_jenisoil").Id)
                            }
                                };
                                EntityCollection oOilType = organizationService.RetrieveMultiple(oFilter);

                                if (oOilType.Entities.Count() > 0)
                                {
                                    oPN = oOilType.Entities[0].GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                                    oOilPartNumber = oPN;
                                }
                            }

                            if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920000)
                            {
                                oilQty = (oQtyAccount * currentGroupUIOCommodity.GetAttributeValue<int>("tss_oilqtypcs"));
                            }
                            else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920001)
                            {
                                oilQty = (oQtyAccount * currentGroupUIOCommodity.GetAttributeValue<int>("tss_oil"));
                            }

                            oilQtyTotal += oilQty;

                            if (oOilPartNumber != new Guid())
                            {
                                DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oOilPartNumber);

                                oil += (oilQty * oResult.tss_price);

                                //_DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
                                //_DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
                                //_DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
                            }
                        }
                        else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == PART_COMMODITYTYPE_TYRE)
                        {
                            Guid oTyrePartNumber = new Guid();

                            if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_tyreby").Value == PART_COMMODITYTYPE_BY_PART)
                            {
                                oTyrePartNumber = currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_tyrepartnumber").Id;

                                if (oTyrePartNumber != null)
                                {
                                    oPN = oTyrePartNumber;
                                }
                            }
                            else if (currentGroupUIOCommodity.GetAttributeValue<OptionSetValue>("tss_tyreby").Value == PART_COMMODITYTYPE_BY_SPEC)
                            {
                                Entity _tss_partmasterlinestyretype = organizationService.Retrieve("tss_partmasterlinestyretype", currentGroupUIOCommodity.GetAttributeValue<EntityReference>("tss_tyrespec").Id, new ColumnSet(true));

                                if (_tss_partmasterlinestyretype != null)
                                {
                                    oPN = _tss_partmasterlinestyretype.GetAttributeValue<EntityReference>("tss_partmasterid").Id;
                                    oTyrePartNumber = oPN;
                                }
                            }

                            tyreQty = (oQtyAccount * currentGroupUIOCommodity.GetAttributeValue<int>("tss_tyre"));
                            tyreQtyTotal += tyreQty;

                            if (oTyrePartNumber != new Guid())
                            {
                                DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oTyrePartNumber);

                                tyre += (tyreQty * oResult.tss_price);

                                //_DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
                                //_DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
                                //_DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
                            }
                        }

                        //if (commodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == COMMODITY_BATTERY)
                        //{
                        //    batteryQty += commodity.GetAttributeValue<int>("tss_battery");

                        //    if (oBatteryPartNumber != new Guid())
                        //    {
                        //        DL_tss_mastermarketsizesublines oResult = GetPartMaster(organizationService, oBatteryPartNumber);

                        //        _DL_tss_mastermarketsizesublines.tss_price = oResult.tss_price;
                        //        _DL_tss_mastermarketsizesublines.tss_minimumprice = oResult.tss_minimumprice;
                        //        _DL_tss_mastermarketsizesublines.tss_partdescription = oResult.tss_partdescription;
                        //    }
                        //}
                        //else if (commodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == COMMODITY_OIL)
                        //{
                        //    if (commodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920000)
                        //    {
                        //        oilQty += commodity.GetAttributeValue<int>("tss_oilqtypcs");
                        //    }
                        //    else if (commodity.GetAttributeValue<OptionSetValue>("tss_oilqtyby").Value == 865920001)
                        //    {
                        //        oilQty += commodity.GetAttributeValue<int>("tss_oil");
                        //    }
                        //}
                        //else if (commodity.GetAttributeValue<OptionSetValue>("tss_partcommoditytype").Value == COMMODITY_TYRE)
                        //{
                        //    tyreQty += commodity.GetAttributeValue<int>("tss_tyre");
                        //}
                    }

                    #region INSERT
                    FilterExpression fExist = new FilterExpression(LogicalOperator.And);
                    fExist.AddCondition("tss_groupuiocommodityheader", ConditionOperator.Equal, header.Id);

                    QueryExpression qExist = new QueryExpression("tss_marketsizesummarybygroupuiocommodity");
                    qExist.Criteria.AddFilter(fExist);
                    qExist.ColumnSet = new ColumnSet(true);

                    EntityCollection totalSummaryByPartTypeQ = organizationService.RetrieveMultiple(qExist);

                    if (totalSummaryByPartTypeQ.Entities.Count() == 0)
                    {
                        Entity en_MarketSizeSummarybyGroupUIOCommodity = new Entity("tss_marketsizesummarybygroupuiocommodity");

                        en_MarketSizeSummarybyGroupUIOCommodity["tss_groupuiocommodity"] = "";
                        en_MarketSizeSummarybyGroupUIOCommodity["tss_groupuiocommodityheader"] = new EntityReference("tss_groupuiocommodityheader", header.Id);

                        en_MarketSizeSummarybyGroupUIOCommodity["tss_amountbattery"] = new Money(battery);
                        en_MarketSizeSummarybyGroupUIOCommodity["tss_amountoil"] = new Money(oil);
                        en_MarketSizeSummarybyGroupUIOCommodity["tss_amounttyre"] = new Money(tyre);

                        en_MarketSizeSummarybyGroupUIOCommodity["tss_qtybattery"] = batteryQtyTotal;
                        en_MarketSizeSummarybyGroupUIOCommodity["tss_qtyoil"] = oilQtyTotal;
                        en_MarketSizeSummarybyGroupUIOCommodity["tss_qtytyre"] = tyreQtyTotal;

                        organizationService.Create(en_MarketSizeSummarybyGroupUIOCommodity);
                    }
                    else
                    {
                        totalSummaryByPartTypeQ.Entities[0]["tss_amountbattery"] = new Money(battery);
                        totalSummaryByPartTypeQ.Entities[0]["tss_amountoil"] = new Money(oil);
                        totalSummaryByPartTypeQ.Entities[0]["tss_amounttyre"] = new Money(tyre);

                        totalSummaryByPartTypeQ.Entities[0]["tss_qtybattery"] = batteryQtyTotal;
                        totalSummaryByPartTypeQ.Entities[0]["tss_qtyoil"] = oilQtyTotal;
                        totalSummaryByPartTypeQ.Entities[0]["tss_qtytyre"] = tyreQtyTotal;

                        organizationService.Update(totalSummaryByPartTypeQ.Entities[0]);
                    }
                    #endregion
                }
            }
        }

        public DL_tss_mastermarketsizesublines GetPartMaster(IOrganizationService organizationService, Guid oPartNumber)
        {
            decimal pr = 0m;
            decimal minpr = 0m;
            DL_tss_mastermarketsizesublines oResult = new DL_tss_mastermarketsizesublines();

            QueryExpression qPrice = new QueryExpression(_DL_tss_sparepartpricemaster.EntityName);
            qPrice.LinkEntities.Add(new LinkEntity(_DL_tss_sparepartpricemaster.EntityName, _DL_tss_pricelistpart.EntityName, "tss_pricelistpart", "tss_pricelistpartid", JoinOperator.Inner));
            qPrice.Criteria.AddCondition("tss_partmaster", ConditionOperator.Equal, oPartNumber);
            qPrice.LinkEntities[0].LinkCriteria.AddCondition("tss_type", ConditionOperator.Equal, MS_TYPE);
            qPrice.LinkEntities[0].EntityAlias = "tss_pricelistpart";
            qPrice.ColumnSet = new ColumnSet(true);
            EntityCollection price = _DL_tss_sparepartpricemaster.Select(organizationService, qPrice);

            if (price.Entities.Count > 0)
            {
                pr = price.Entities[0].Contains("tss_price") ? price.Entities[0].GetAttributeValue<Money>("tss_price").Value : 0m;
                minpr = price.Entities[0].Contains("tss_minimumprice") ? price.Entities[0].GetAttributeValue<Money>("tss_minimumprice").Value : 0m;

                oResult.tss_price = pr;
                oResult.tss_minimumprice = minpr;
            }
            else
            {
                oResult.tss_price = 0;
                oResult.tss_minimumprice = 0;
            }

            Entity oPN = organizationService.Retrieve("trs_masterpart", oPartNumber, new ColumnSet(true));
            oResult.tss_partdescription = oPN.GetAttributeValue<String>("trs_partdescription");

            return oResult;
        }


    }
}
