using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.Helper
{
    public class RetrieveHelper
    {
        public List<Entity> RetrieveMultiple(IOrganizationService organizationService, QueryExpression query)
        {
            List<Entity> result = new List<Entity>();
            int fetchCount = 5000;
            int pageNumber = 1;

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = fetchCount;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = null;

            while (true)
            {
                EntityCollection collections = organizationService.RetrieveMultiple(query);

                if (collections.Entities.Count > 0)
                {
                    result.AddRange(collections.Entities);
                }

                if (collections.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = collections.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public static void BulkCreate(IOrganizationService service, DataCollection<Entity> entities)
        {
            // Create an ExecuteMultipleRequest object.
            var multipleRequest = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a CreateRequest for each entity to the request collection.
            foreach (var entity in entities)
            {
                CreateRequest createRequest = new CreateRequest { Target = entity };
                multipleRequest.Requests.Add(createRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

        }

        public static void BulkUpdate(IOrganizationService service, DataCollection<Entity> entities)
        {
            // Create an ExecuteMultipleRequest object.
            var multipleRequest = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a UpdateRequest for each entity to the request collection.
            foreach (var entity in entities)
            {
                UpdateRequest updateRequest = new UpdateRequest { Target = entity };
                multipleRequest.Requests.Add(updateRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);

        }

        public static void BulkDelete(IOrganizationService service, DataCollection<EntityReference> entityReferences)
        {
            // Create an ExecuteMultipleRequest object.
            var multipleRequest = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = false,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            // Add a DeleteRequest for each entity to the request collection.
            foreach (var entityRef in entityReferences)
            {
                DeleteRequest deleteRequest = new DeleteRequest { Target = entityRef };
                multipleRequest.Requests.Add(deleteRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)service.Execute(multipleRequest);
        }
    }
}
