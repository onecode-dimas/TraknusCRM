using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using TrakNusSparepartSystem.DataLayer;
using TrakNusSparepartSystem.Helper;
using TrakNusSparepartSystem.WorkflowActivity.Helper;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using TrakNusSparepartSystem.WorkflowActivity.Business_Layer;

namespace TrakNusSparepartSystem.WorkflowActivity.BusinessLayer
{
    public class BL_tss_salestargetpss
    {
        #region Constant
        private const int STATUS_OPEN = 865920000;
        private const int STATUS_DRAFT = 865920000;
        private const int REVISION = 865920001;
        private const int STATUS_COMPLETED_MS = 865920000;
        private const int WAITING_APPROVAL_PDH = 865920001;
        private const int ST_PSS_WAITING_APPROVAL_KORWIL = 865920004;
        private const int ST_BRANCH_WAITING_APPROVAL_KORWIL = 865920001;
        private const int ST_SR_BRANCH_WAITING_APPROVAL_KORWIL = 865920001;
        

        #endregion
        #region Dependencies
        DL_user _DL_user = new DL_user();
        DL_tss_salestargetpss _DL_tss_salestargetpss = new DL_tss_salestargetpss();
        DL_tss_matrixapprovalmarketsize _DL_tss_matrixapprovalmarketsize = new DL_tss_matrixapprovalmarketsize();
        DL_tss_sparepartsetup _DL_tss_sparepartsetup = new DL_tss_sparepartsetup();
        DL_tss_keyaccount _DL_tss_keyaccount = new DL_tss_keyaccount();
        DL_tss_salestargetbranch _DL_tss_salestargetbranch = new DL_tss_salestargetbranch();
        BL_tss_salestargetbranch _BL_tss_salestargetbranch = new BL_tss_salestargetbranch();

        #endregion

        public void ConfirmSalesTargetPSS_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            Entity en_SalesTargetPSS =_DL_tss_salestargetpss.Select(organizationService, context.PrimaryEntityId);
            Entity en_User = new Entity(_DL_user.EntityName, context.UserId);

            //if (en_SalesTargetPSS.GetAttributeValue<Money>("tss_totalamountmarketsizeyearly").Value > 0 && en_SalesTargetPSS.GetAttributeValue<Money>("tss_totalpctgmarketsizeyearly").Value > 0 && en_SalesTargetPSS.GetAttributeValue<Money>("tss_totalyearly").Value > 0)
            //{
                FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
                fappMat.AddCondition("tss_branch", ConditionOperator.Equal, en_SalesTargetPSS.GetAttributeValue<EntityReference>("tss_branch").Id);
                fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920000);
                fappMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920001);

                QueryExpression qappMat = new QueryExpression("tss_matrixapprovalmarketsize");
                qappMat.Criteria.AddFilter(fappMat);
                qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
                qappMat.ColumnSet = new ColumnSet(true);

                EntityCollection en_approvalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);


                if (en_approvalMatrixMarketSize.Entities.Count > 0)
                {
                    en_SalesTargetPSS["tss_status"] = new OptionSetValue(WAITING_APPROVAL_PDH);
                    en_SalesTargetPSS["tss_currentapproval"] =new EntityReference("systemuser",en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id);
                    en_SalesTargetPSS["tss_currentapprovaldate"] = DateTime.Now.ToLocalTime();
                    en_SalesTargetPSS["tss_confirmdate"] = DateTime.Now.ToLocalTime();

                    en_User["tss_salestargetconfirmed"] = true;

                    organizationService.Update(en_SalesTargetPSS);
                    organizationService.Update(en_User);

                    foreach (Entity appMatrix in en_approvalMatrixMarketSize.Entities)
                    {
                    #region insert to list Approval Matrix
                    Entity ENT_approvalListMarketSize = new Entity("tss_approvallistmarketsize");
                    ENT_approvalListMarketSize["tss_approver"] = new EntityReference("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);
                    ENT_approvalListMarketSize["tss_salestargetpss"] =new EntityReference("tss_salestargetpss", context.PrimaryEntityId);
                    ENT_approvalListMarketSize["tss_type"] = new OptionSetValue(865920003);
                    organizationService.Create(ENT_approvalListMarketSize);
                    #endregion

                    #region share records
                    ShareRecords objShare = new ShareRecords();
                        Entity en_targetUser = new Entity("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);

                        objShare.ShareRecord(organizationService, en_SalesTargetPSS, en_targetUser);
                        #endregion
                        
                    }

                    TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                    string emailSubject = en_SalesTargetPSS.GetAttributeValue<string>("tss_name") + " is waiting for PDH approval";
                    string emailContent = "Please review and approve.";
                    objEmail.SendEmailNotif(context.UserId, en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);


                }
                else
                {
                    throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
                }
            //}
            //else
            //{
            //    throw new InvalidWorkflowException("Transaction Error. Sales Target PSS has no yearly data . Please re-check .");
            //}
        }

        public void ApproveSalesTargetPSS_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            Guid salesTaretBranchId = Guid.Empty;
            Entity entityToApprove = _DL_tss_salestargetpss.Select(organizationService, context.PrimaryEntityId);
            entityToApprove["tss_status"] = new OptionSetValue(ST_PSS_WAITING_APPROVAL_KORWIL);
            entityToApprove["tss_approvedate"] = DateTime.Now.ToLocalTime();
            organizationService.Update(entityToApprove);

            salesTaretBranchId =_BL_tss_salestargetbranch.GenerateSalesTargetBranch(organizationService, tracingService, context);
            
            Entity entitySalesTargetBranch = _DL_tss_salestargetbranch.Select(organizationService, salesTaretBranchId);
            entitySalesTargetBranch["tss_status"] = new OptionSetValue(ST_BRANCH_WAITING_APPROVAL_KORWIL);
            entitySalesTargetBranch["tss_statusreason"] = new OptionSetValue(ST_SR_BRANCH_WAITING_APPROVAL_KORWIL);
            if (!entitySalesTargetBranch.Attributes.Contains("tss_totalallsalesyearly"))
                entitySalesTargetBranch.Attributes.Add("tss_totalallsalesyearly",new Money(0));

            if (!entitySalesTargetBranch.Attributes.Contains("tss_totalamountmarketsizeyearly"))
                entitySalesTargetBranch.Attributes.Add("tss_totalamountmarketsizeyearly",new Money(0));

            entitySalesTargetBranch["tss_totalallsalesyearly"] =new Money(entityToApprove.GetAttributeValue<Money>("tss_totalallsalesyearly").Value+entitySalesTargetBranch.GetAttributeValue<Money>("tss_totalallsalesyearly").Value);
            entitySalesTargetBranch["tss_totalamountmarketsizeyearly"] =new Money((entityToApprove.GetAttributeValue<Money>("tss_totalamountmarketsizeyearly").Value)+ (entitySalesTargetBranch.GetAttributeValue<Money>("tss_totalamountmarketsizeyearly").Value));
            //
            //
            organizationService.Update(entitySalesTargetBranch);

            FilterExpression fappMat = new FilterExpression(LogicalOperator.And);
            fappMat.AddCondition("tss_branch", ConditionOperator.Equal, entityToApprove.GetAttributeValue<EntityReference>("tss_branch").Id);
            fappMat.AddCondition("tss_approvaltype", ConditionOperator.Equal, 865920000);
            fappMat.AddCondition("tss_approvalfor", ConditionOperator.Equal, 865920001);

            QueryExpression qappMat = new QueryExpression("tss_matrixapprovalmarketsize");
            qappMat.Criteria.AddFilter(fappMat);
            qappMat.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("tss_priorityno", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            qappMat.ColumnSet = new ColumnSet(true);

            EntityCollection en_approvalMatrixMarketSize = _DL_tss_matrixapprovalmarketsize.Select(organizationService, qappMat);


            if (en_approvalMatrixMarketSize.Entities.Count > 0)
            {
                DL_tss_marketsizeresultmapping _DL_tss_marketsizeresultmapping = new DL_tss_marketsizeresultmapping();
                Entity entitymsmp = new Entity(_DL_tss_marketsizeresultmapping.EntityName);
                entitymsmp["tss_salestargetpss"] =new EntityReference("tss_salestargetpss", context.PrimaryEntityId);
                entitymsmp["tss_salestargetbranch"] =new EntityReference("tss_salestargetbranch", salesTaretBranchId);
                organizationService.Create(entitymsmp);

                foreach (Entity appMatrix in en_approvalMatrixMarketSize.Entities)
                {
                    #region insert to list Approval Matrix
                    Entity ENT_approvalListMarketSize = new Entity("tss_approvallistmarketsize");
                    ENT_approvalListMarketSize["tss_approver"] =new EntityReference("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);
                    ENT_approvalListMarketSize["tss_salestargetbranch"] = new EntityReference("tss_salestargetbranch", salesTaretBranchId);
                    ENT_approvalListMarketSize["tss_type"] = new OptionSetValue(865920004);
                    organizationService.Create(ENT_approvalListMarketSize);
                    #endregion

                    #region share records
                    ShareRecords objShare = new ShareRecords();
                    Entity en_targetUser = new Entity("systemuser", appMatrix.GetAttributeValue<EntityReference>("tss_approver").Id);

                    objShare.ShareRecord(organizationService, entityToApprove, en_targetUser);
                    #endregion
                }

                TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
                string emailSubject = entityToApprove.GetAttributeValue<string>("tss_name") + " is waiting for KORWIL approval";
                string emailContent = "Please review and approve.";
                objEmail.SendEmailNotif(context.UserId, en_approvalMatrixMarketSize.Entities[0].GetAttributeValue<EntityReference>("tss_approver").Id, Guid.Empty, organizationService, emailSubject, emailContent);


            }
            else
            {
                throw new InvalidWorkflowException("No Approval Matrix found. Please contact your IT administrator.");
            }
        }

        public void ReviseSalesTargetPSS_OnClick(IOrganizationService organizationService, ITracingService tracingService, IWorkflowContext context)
        {
            Entity en_SalesTargetPSS = _DL_tss_salestargetpss.Select(organizationService, context.PrimaryEntityId);

            en_SalesTargetPSS["tss_status"] = new OptionSetValue(STATUS_DRAFT);
            organizationService.Update(en_SalesTargetPSS);

            TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent objEmail = new TrakNusSparepartSystem.WorkflowActivity.Helper.EmailAgent();
            string emailSubject = en_SalesTargetPSS.GetAttributeValue<string>("tss_name") + " was revised";
            string emailContent = "Please recheck and review.";
            objEmail.SendEmailNotif(context.UserId, en_SalesTargetPSS.GetAttributeValue<EntityReference>("tss_pss").Id, Guid.Empty, organizationService, emailSubject, emailContent);

        }
    }
}
