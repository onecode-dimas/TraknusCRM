using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer.Interface
{
    public interface ICRMOperation
    {
        Guid Insert(Entity entityToInsert);
        void Delete(Entity entityToDelete);
        BulkDeleteResponse BulkDelete(BulkDeleteRequest bulkDeleteRequest);
        ExecuteMultipleResponse BulkRequest(ExecuteMultipleRequest bulkRequest);
        void Update(Entity entityToUpdate);
    }
}
