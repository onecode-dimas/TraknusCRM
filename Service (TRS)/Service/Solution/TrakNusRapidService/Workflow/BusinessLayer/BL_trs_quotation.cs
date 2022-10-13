using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TrakNusRapidService.DataLayer;
using TrakNusRapidService.Helper;

namespace TrakNusRapidService.Workflow.BusinessLayer
{
    public class BL_trs_quotation
    {
        #region Constants
        private const string _classname = "BL_trs_quotation";
        #endregion

        #region Depedencies
        private DL_trs_quotation _DL_trs_quotation = new DL_trs_quotation();
        private DL_trs_quotationcommercialheader _DL_trs_quotationcommercialheader = new DL_trs_quotationcommercialheader();
        private DL_trs_quotationpartdetail _DL_trs_quotationpartdetail = new DL_trs_quotationpartdetail();
        private DL_trs_quotationpartssummary _DL_trs_quotationpartssummary = new DL_trs_quotationpartssummary();
        private DL_trs_quotationsupportingmaterial _DL_trs_quotationsupportingmaterial = new DL_trs_quotationsupportingmaterial();
        private DL_trs_discountapproval _DL_trs_discountapproval = new DL_trs_discountapproval();
        private DL_systemuser _DL_systemuser = new DL_systemuser();

        private CRMConnector _crmConnector = new CRMConnector();
        #endregion

        #region privates

        private void SendEmail(IOrganizationService organizationService
            , Guid sender
            , Guid receiver
            , Guid quotationId
            , string quotationNo
            , string pmacttype)
        {
            Guid emailId = Guid.Empty;
            try
            {
                EmailAgent emailAgent = new EmailAgent();
                emailAgent.AddSender(sender);
                emailAgent.AddReceiver(_DL_systemuser.EntityName, receiver);

                /* Change by Thomas - 13 March 2015
                 * emailAgent.subject = subject;
                 * emailAgent.description = body;
                 * */
                emailAgent.subject = "Approval Discount";

                string description = "This is notification email to inform you :<br/><br/>";
                description += "For<br/>";
                description += "Quotation Number : " + quotationNo + "<br/>";
                description += "PMActType : " + pmacttype + "<br/>";
                //description += "Commercial Header : " + dataWO.WODescription + "<br/><br/>";
                description += "<a href='" + _crmConnector.GetCrmUriString() + "?etn=" + _DL_trs_quotation.EntityName + "&pagetype=entityrecord&id=%7B" + quotationId.ToString() + "%7D'><b><u>Need confirmation for Discounting</u></b></a><br/><br/>";
                description += "If you have any question regarding to this notification, please contact Administrator TRS.<br/><br/>";
                description += "Thank you<br/><br/>";
                description += DateTime.Now.ToString("dd-MM-yyyy   HH:mm:ss") + "<br/>";
                description += "Administrator TRS";

                emailAgent.description = description;
                emailAgent.priority = EmailAgent.Priority_High;
                //End change by Thomas - 13 March 2015

                emailAgent.Create(organizationService, out emailId);
                //throw new Exception(receiver);
                if (emailId != Guid.Empty)
                    emailAgent.Send(organizationService, emailId);
            }
            catch (Exception ex)
            {
                throw new Exception(_classname + ".SendEmail : " + ex.Message.ToString());
            }
            finally
            {
                if (emailId != Guid.Empty)
                    UpdateStatustoWaitingForApproval(organizationService, quotationId);
            }
        }

        private void UpdateStatustoWaitingForApproval(IOrganizationService organizationService, Guid id)
        {
            _DL_trs_quotation.WaitingApproval(organizationService, id);
        }
        #endregion

        #region Events
        public void ReviseQuotation_OnClick( IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                Entity eQuot = organizationService.Retrieve("trs_quotation", id, new ColumnSet(new string[] { "trs_revision", "statuscode" }));
                int scode = 0;
                
                if (eQuot.Attributes.Contains("statuscode") && eQuot.Attributes["statuscode"] != null)
                {
                    scode = eQuot.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }

                //Validasi Final Approve
                if (scode != 167630002)
                {
                    /* Change by Thomas - 16 March 2015
                    //Revise
                    _DL_trs_quotation.statuscode = 167630001;
                    _DL_trs_quotation.trs_revision = 1;
                    _DL_trs_quotation.Update(organizationService, id);
                     * */
                    _DL_trs_quotation.Revise(organizationService, id);
                    /* End change by Thomas - 16 March 2015 */
                }
                else if (scode == 167630002)
                {
                    throw new Exception("Final Approve Can't update status");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".ReviseQuotation_OnClick : " + ex.Message.ToString());
            }
	    }

        public void ApproveQuotation_OnClick(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {                
                Entity eQuot = organizationService.Retrieve("trs_quotation", id, new ColumnSet(new string[] { "trs_revision","statuscode" }));
                int scode = 0;
                if (eQuot.Attributes.Contains("statuscode") && eQuot.Attributes["statuscode"] != null)
                {

                    scode = eQuot.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }

                //Validasi Final Approve
                if (scode != 167630002)
                {
                    //Approve
                    _DL_trs_quotation.statuscode =  167630000;
                    _DL_trs_quotation.Update(organizationService, id);
                }
                else if (scode == 167630002)
                {
                    throw new Exception("Final Approve Can't update status");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".ApproveQuotation_OnClick : " + ex.Message.ToString());
            }
        }

        public void FinalApproveQuotation_OnClick(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                Entity eQuot = organizationService.Retrieve("trs_quotation", id, new ColumnSet(new string[] { "trs_revision","statuscode" }));
                int scode = 0;
                if (eQuot.Attributes.Contains("statuscode") && eQuot.Attributes["statuscode"] != null)
                {

                    scode = eQuot.GetAttributeValue<OptionSetValue>("statuscode").Value;
                }

                //Validasi Final Approve
                if (scode != 167630002)
                {
                    //Final Approve
                    _DL_trs_quotation.statuscode = 167630002;
                    _DL_trs_quotation.Update(organizationService, id);
                }
                else if (scode == 167630002)
                {
                    throw new Exception("Final Approve Can't update status");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".FinalApproveQuotation_OnClick : " + ex.Message.ToString());
            }
        }

        public void CalculateTotal(IOrganizationService organizationService, ITracingService tracingService, Guid id)
        {
            try
            {
                decimal subtotalAmount = 0;
                decimal discountAmount = 0;
                decimal totalAmount = 0;

                decimal subtotalServices = 0;
                decimal discountServices = 0;
                decimal totalServices = 0;

                decimal subtotalParts = 0;
                decimal discountParts = 0;
                decimal totalParts = 0;

                decimal totalSupportingMaterial = 0;
                decimal highestPercent = 0;

                Entity enQuotation = _DL_trs_quotation.Select(organizationService, id);
                
                QueryExpression qeQuotationCommercialHeader = new QueryExpression(_DL_trs_quotationcommercialheader.EntityName);
                qeQuotationCommercialHeader.ColumnSet = new ColumnSet(true);
                qeQuotationCommercialHeader.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, id);
                EntityCollection ecQuotationCommercialHeader = _DL_trs_quotationcommercialheader.Select(organizationService, qeQuotationCommercialHeader);
                foreach (Entity enQuotationCommercialHeader in ecQuotationCommercialHeader.Entities)
                {
                    decimal price = 0;
                    decimal discount = 0;
                    decimal totalPrice = 0;
                    if (enQuotationCommercialHeader.Contains("trs_price"))
                        price = ((Money)enQuotationCommercialHeader["trs_price"]).Value;

                    if (enQuotationCommercialHeader.Contains("trs_discountby"))
                    {
                        decimal percent = 0;
                        if ((bool)enQuotationCommercialHeader["trs_discountby"] == true && enQuotationCommercialHeader.Contains("trs_discountamount"))
                        {
                            discount = ((Money)enQuotationCommercialHeader["trs_discountamount"]).Value;
                            percent = discount / price * 100;
                            highestPercent = highestPercent < percent ? percent : highestPercent;
                        }
                        else if ((bool)enQuotationCommercialHeader["trs_discountby"] == false && enQuotationCommercialHeader.Contains("trs_discountpercent"))
                        {
                            percent = (decimal)enQuotationCommercialHeader["trs_discountpercent"];
                            discount = price * percent / 100;
                            highestPercent = highestPercent < percent ? percent : highestPercent;
                        }
                    }
                    if (enQuotationCommercialHeader.Contains("trs_totalprice"))
                        totalPrice = ((Money)enQuotationCommercialHeader["trs_totalprice"]).Value;

                    subtotalServices += price;
                    discountServices += discount;
                    totalServices += totalPrice;
                }

                //* MARK BY Santony (20/3/2015) get Total Part Quotation from Quotation Part Summary, not from Quotation Part Detail
                #region Quotation Part Detail - Comment
                //QueryExpression qeQuotationPartDetail = new QueryExpression(_DL_trs_quotationpartdetail.EntityName);
                //qeQuotationPartDetail.ColumnSet = new ColumnSet(true);
                //qeQuotationPartDetail.Criteria.AddCondition("trs_quotation", ConditionOperator.Equal, id);
                //EntityCollection ecQuotationPartDetail = _DL_trs_quotationpartdetail.Select(organizationService, qeQuotationPartDetail);
                //foreach (Entity enQuotationPartDetail in ecQuotationPartDetail.Entities)
                //{
                //    decimal price = 0;
                //    int quantity = 0;
                //    decimal discount = 0;
                //    decimal totalPrice = 0;
                //    if (enQuotationPartDetail.Contains("trs_price"))
                //        price = ((Money)enQuotationPartDetail["trs_price"]).Value;

                //    if (enQuotationPartDetail.Contains("trs_quantity"))
                //        quantity = (int)enQuotationPartDetail["trs_quantity"];

                //    if (enQuotationPartDetail.Contains("trs_discountby"))
                //    {
                //        decimal percent = 0;
                //        if ((bool)enQuotationPartDetail["trs_discountby"] == true && enQuotationPartDetail.Contains("trs_discountamount"))
                //        {
                //            discount = ((Money)enQuotationPartDetail["trs_discountamount"]).Value;
                //            percent = discount / (price * quantity) * 100;
                //            highestPercent = highestPercent < percent ? percent : highestPercent;
                //        }
                //        else if ((bool)enQuotationPartDetail["trs_discountby"] == false && enQuotationPartDetail.Contains("trs_discountpercent"))
                //        {
                //            percent = (decimal)enQuotationPartDetail["trs_discountpercent"];
                //            discount = (price * quantity) * percent / 100;
                //            highestPercent = highestPercent < percent ? percent : highestPercent;
                //        }
                //    }
                //    if (enQuotationPartDetail.Contains("trs_totalprice"))
                //        totalPrice = ((Money)enQuotationPartDetail["trs_totalprice"]).Value;

                //    subtotalParts += price * quantity;
                //    discountParts += discount;
                //    totalParts += totalPrice;
                //}
                #endregion

                QueryExpression qeQuotationPartSummary = new QueryExpression(_DL_trs_quotationpartssummary.EntityName);
                qeQuotationPartSummary.ColumnSet = new ColumnSet(true);
                qeQuotationPartSummary.Criteria.AddCondition("trs_quotationnumber", ConditionOperator.Equal, id);
                EntityCollection ecQuotationPartSummary = _DL_trs_quotationpartdetail.Select(organizationService, qeQuotationPartSummary);
                foreach (Entity enQuotationPartSummary in ecQuotationPartSummary.Entities)
                {
                    decimal price = 0;
                    int TaskListQuantity = 0;
                    int ManualQuantity = 0;
                    int QuantityTotal = 0;
                    decimal discount = 0;
                    if (enQuotationPartSummary.Contains("trs_price"))
                        price = ((Money)enQuotationPartSummary["trs_price"]).Value;

                    if (enQuotationPartSummary.Contains("trs_tasklistquantity"))
                        TaskListQuantity = (int)enQuotationPartSummary["trs_tasklistquantity"];

                    if (enQuotationPartSummary.Contains("trs_manualquantity"))
                        ManualQuantity = (int)enQuotationPartSummary["trs_manualquantity"];

                    QuantityTotal = TaskListQuantity + ManualQuantity;

                    if (enQuotationPartSummary.Contains("trs_discountby"))
                    {
                        decimal percent = 0;
                        if ((bool)enQuotationPartSummary["trs_discountby"] == true && enQuotationPartSummary.Contains("trs_discountamount"))
                        {
                            discount = ((Money)enQuotationPartSummary["trs_discountamount"]).Value;
                            percent = discount / (price * QuantityTotal) * 100;
                            highestPercent = highestPercent < percent ? percent : highestPercent;
                        }
                        else if ((bool)enQuotationPartSummary["trs_discountby"] == false && enQuotationPartSummary.Contains("trs_discountpercent"))
                        {
                            percent = (decimal)enQuotationPartSummary["trs_discountpercent"];
                            discount = (price * QuantityTotal) * percent / 100;
                            highestPercent = highestPercent < percent ? percent : highestPercent;
                        }
                    }

                    subtotalParts += price * QuantityTotal;
                    discountParts += discount;
                }
                totalParts += subtotalParts - discountParts;

                QueryExpression qeQuotationSupportingMaterial = new QueryExpression(_DL_trs_quotationsupportingmaterial.EntityName);
                qeQuotationSupportingMaterial.ColumnSet = new ColumnSet(true);
                qeQuotationSupportingMaterial.Criteria.AddCondition("trs_quotationid", ConditionOperator.Equal, id);
                EntityCollection ecQuotationSupportingMaterial = _DL_trs_quotationsupportingmaterial.Select(organizationService, qeQuotationSupportingMaterial);
                foreach (Entity enQuotationSupportingMaterial in ecQuotationSupportingMaterial.Entities)
                {
                    if (enQuotationSupportingMaterial.Contains("trs_totalprice"))
                    {
                        totalSupportingMaterial += ((Money)enQuotationSupportingMaterial["trs_totalprice"]).Value;
                    }
                }
                subtotalAmount = subtotalServices + subtotalParts + totalSupportingMaterial;
                discountAmount = discountServices + discountParts;
                totalAmount = subtotalAmount - discountAmount;
                _DL_trs_quotation.trs_subtotalservices = subtotalServices;
                _DL_trs_quotation.trs_discountservices = discountServices;
                _DL_trs_quotation.trs_totalservices = totalServices;
                _DL_trs_quotation.trs_subtotalparts = subtotalParts;
                _DL_trs_quotation.trs_discountparts = discountParts;
                _DL_trs_quotation.trs_totalparts = totalParts;
                _DL_trs_quotation.trs_totalsupportingmaterials = totalSupportingMaterial;
                _DL_trs_quotation.trs_subtotalamount = subtotalAmount;
                _DL_trs_quotation.trs_discountamount = discountAmount;
                _DL_trs_quotation.trs_totalamount = totalAmount;
                _DL_trs_quotation.Update(organizationService, id);

                // if found discount approval
                if (enQuotation.Contains("trs_branch"))
                {
                    Guid branchId = enQuotation.GetAttributeValue<EntityReference>("trs_branch").Id;

                    QueryExpression qeDiscountApproval = new QueryExpression(_DL_trs_discountapproval.EntityName);
                    qeDiscountApproval.ColumnSet = new ColumnSet(true);

                    FilterExpression feDiscountApproval = qeDiscountApproval.Criteria.AddFilter(LogicalOperator.And);
                    feDiscountApproval.AddCondition("trs_branch", ConditionOperator.Equal, branchId);
                    feDiscountApproval.AddCondition("trs_maximum", ConditionOperator.GreaterEqual, highestPercent);
                    feDiscountApproval.AddCondition("trs_minimum", ConditionOperator.LessEqual, highestPercent);
                    EntityCollection ecDiscountApproval = _DL_trs_discountapproval.Select(organizationService, qeDiscountApproval);

                    if (ecDiscountApproval.Entities.Count > 0)
                    {
                        Entity eDiscountApproval = ecDiscountApproval[0];
                        if (eDiscountApproval.Attributes.Contains("trs_picdiscount"))
                        {
                            SendEmail(organizationService
                                , ((EntityReference)enQuotation["owninguser"]).Id
                                , eDiscountApproval.GetAttributeValue<EntityReference>("trs_picdiscount").Id
                                , enQuotation.GetAttributeValue<Guid>("trs_quotationid")
                                , enQuotation.GetAttributeValue<string>("trs_quotationnumber").ToString()
                                , enQuotation.GetAttributeValue<EntityReference>("trs_pmactytype").Name.ToString());
                        }
                        else
                        {
                            throw new InvalidWorkflowException("Please setup PIC to handle this discount approval !");
                        }
                    }
                    else
                    {
                        _DL_trs_quotation.Approve(organizationService, id);
                    }
                }
                else
                {
                    _DL_trs_quotation.Approve(organizationService, id);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(_classname + ".CalculateTotal : " + ex.Message.ToString());
            }
        }
        #endregion
    }
}
