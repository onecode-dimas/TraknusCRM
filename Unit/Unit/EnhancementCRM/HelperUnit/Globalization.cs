using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancementCRM.HelperUnit
{
    public class Globalization
    {
        private const string _entityname_transactioncurrency = "transactioncurrency";

        public static Guid GetTransactionCurrencyID(IOrganizationService _organizationservice, String _transactioncurrencycode)
        {
            Guid _transactioncurrencyid = new Guid();

            QueryExpression _queryexpression = new QueryExpression(_entityname_transactioncurrency);
            _queryexpression.ColumnSet = new ColumnSet(true);
            _queryexpression.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, _transactioncurrencycode);
            EntityCollection _transactioncurrencies = _organizationservice.RetrieveMultiple(_queryexpression);

            if (_transactioncurrencies.Entities.Count == 0)
                throw new InvalidPluginExecutionException("'" + _transactioncurrencycode + "' is NOT found in Transaction Currency !");
            else
                _transactioncurrencyid = _transactioncurrencies.Entities.FirstOrDefault().Id;

            return _transactioncurrencyid;
        }

        public static void GetObjectTypeCode(IOrganizationService _organizationservice, string _logicalname, out string _objecttypecode)
        {
            try
            {
                Entity _entity = new Entity(_logicalname);
                RetrieveEntityRequest _entityrequest = new RetrieveEntityRequest();
                _entityrequest.LogicalName = _entity.LogicalName;
                _entityrequest.EntityFilters = EntityFilters.All;

                RetrieveEntityResponse _entityresponse = (RetrieveEntityResponse)_organizationservice.Execute(_entityrequest);

                EntityMetadata _metadata = _entityresponse.EntityMetadata;
                _objecttypecode = _metadata.ObjectTypeCode.ToString();
            }
            catch
            {
                throw new Exception("Entity with logical name " + _logicalname + " not found!");
            }
        }
    }
}
