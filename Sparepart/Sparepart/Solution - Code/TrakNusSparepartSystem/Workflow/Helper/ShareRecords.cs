using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Workflow.Helper
{
    public class ShareRecords
    {
        public void ShareRecord(IOrganizationService service, Entity TargetEntity, Entity TargetShare)
        {
            GrantAccessRequest grant = new GrantAccessRequest();
            grant.Target = new EntityReference(TargetEntity.LogicalName, TargetEntity.Id);

            PrincipalAccess principal = new PrincipalAccess();
            principal.Principal = new EntityReference(TargetShare.LogicalName, TargetShare.Id);
            //List AccessRight
            principal.AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess;
            grant.PrincipalAccess = principal;

            try
            {
                GrantAccessResponse grant_response = (GrantAccessResponse)service.Execute(grant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ShareRecordReadOnly(IOrganizationService service, Entity TargetEntity, Entity TargetShare)
        {
            GrantAccessRequest grant = new GrantAccessRequest();
            grant.Target = new EntityReference(TargetEntity.LogicalName, TargetEntity.Id);

            PrincipalAccess principal = new PrincipalAccess();
            principal.Principal = new EntityReference(TargetShare.LogicalName, TargetShare.Id);
            //List AccessRight
            principal.AccessMask = AccessRights.ReadAccess;
            grant.PrincipalAccess = principal;

            try
            {
                GrantAccessResponse grant_response = (GrantAccessResponse)service.Execute(grant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UnShareRecord(IOrganizationService service, Entity TargetEntity, Entity TargetShare)
        {
            //no delete access
            ModifyAccessRequest modif = new ModifyAccessRequest(); ;
            modif.Target = new EntityReference(TargetEntity.LogicalName, TargetEntity.Id);

            PrincipalAccess principal = new PrincipalAccess();
            principal.Principal = new EntityReference(TargetShare.LogicalName, TargetShare.Id);
            principal.AccessMask = AccessRights.None;
            modif.PrincipalAccess = principal;

            try
            {
                ModifyAccessResponse modif_response = (ModifyAccessResponse)service.Execute(modif); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UnShareAllRecords(IOrganizationService service, Entity TargetEntity, Guid ownerid)
        {
            var accessRequest = new RetrieveSharedPrincipalsAndAccessRequest
            {
                Target = new EntityReference(TargetEntity.LogicalName, TargetEntity.Id)
            };

            var accessResponse = (RetrieveSharedPrincipalsAndAccessResponse)service.Execute(accessRequest);

            foreach (var principalAccess in accessResponse.PrincipalAccesses)
            {
                if (principalAccess.Principal.Id != ownerid)
                {
                    var revokeRequest = new RevokeAccessRequest
                    {
                        Revokee = principalAccess.Principal,
                        Target = new EntityReference(TargetEntity.LogicalName, TargetEntity.Id)
                    };
                    service.Execute(revokeRequest);
                }
            }
        }
    }
}
