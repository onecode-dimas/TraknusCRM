using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrakNusSparepartSystem.DataLayer.Interface
{
    public interface IRetrieve
    {
        Entity Retrieve(Guid id);
        Entity Retrieve(Guid id, ColumnSet columnSet);
        Entity Retrieve(string entityName, Guid id);
        Entity Retrieve(string entityName, Guid id, ColumnSet columnSet);
        EntityCollection Retrieve(QueryExpression queryExpression);
    }
}
