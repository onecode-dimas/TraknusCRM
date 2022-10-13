/****************************************************************************************************
 * Created By   : Thomas Williem Effendi
 * Created Date : 13 May 2014
 * Description  : Class for extract entity
 * Compatibility: CRM 2011
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Client.Services;
using Microsoft.Crm.Sdk.Messages;

namespace SendEmail.Helper
{
    class EntityExtractor
    {
        private string _customEntityName = "tectura";

        public EntityCollection ExtractEntitiesFromXML(OrganizationService organizationService, string fetchXML)
        {
            FetchXmlToQueryExpressionRequest request = new FetchXmlToQueryExpressionRequest();
            request.FetchXml = fetchXML;
            FetchXmlToQueryExpressionResponse response = (FetchXmlToQueryExpressionResponse)organizationService.Execute(request);
            return organizationService.RetrieveMultiple(response.Query);
        }

        public EntityCollection ExtractEntitiesFromSQL(string connectionName, string query)
        {
            EntityCollection entityCollection = new EntityCollection();
            ODBCConnector odbcConnector = new ODBCConnector();
            odbcConnector.ConnectionName = connectionName;

            OdbcCommand odbcCommand = new OdbcCommand();
            odbcCommand.CommandText = query;
            DataTable dt = odbcConnector.Select(odbcCommand);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Entity entity = new Entity(_customEntityName);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    KeyValuePair<string, object> attribute = new KeyValuePair<string, object>(dt.Columns[j].ColumnName, dt.Rows[i][j]);
                    entity.Attributes.Add(attribute);
                }
                entityCollection.Entities.Add(entity);
            }
            return entityCollection;
        }

        public DataTable ExtractAttributesFromEntity(OrganizationService organizationService, Entity entity, int resourceType)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Schema Name");
            dt.Columns.Add("Display Name");
            dt.Columns.Add("Type");
            dt.Columns.Add("Value");

            foreach (KeyValuePair<string, object> attribute in entity.Attributes)
            {
                string attributeSchemaName = attribute.Key;
                string attributeDisplayName = attributeSchemaName == _customEntityName ? GetAttributeDisplayName(organizationService, entity.LogicalName, attributeSchemaName) : attributeDisplayName = attributeSchemaName;
                string attributeType = attribute.Value.GetType().ToString();
                string attributeValue;

                switch (attributeType.ToLower())
                {
                    case "microsoft.xrm.sdk.optionsetvalue":
                        attributeValue = entity.FormattedValues[attributeSchemaName].ToString();
                        break;
                    case "microsoft.xrm.sdk.entityreference":
                        attributeValue = ((EntityReference)attribute.Value).Id.ToString();
                        break;
                    case "system.datetime":
                        switch (resourceType)
                        {
                            case 1: //CRMService
                                attributeValue = ((DateTime)attribute.Value).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss.FFFFFFF");
                                break;
                            case 2: //Query
                                attributeValue = ((DateTime)attribute.Value).ToString("yyyy/MM/dd HH:mm:ss.FFFFFFF");
                                break;
                            default:
                                attributeValue = ((DateTime)attribute.Value).ToString("yyyy/MM/dd HH:mm:ss.FFFFFFF");
                                break;
                        }
                        break;
                    default:
                        attributeValue = attribute.Value.ToString();
                        break;
                }
                dt.Rows.Add(attributeSchemaName, attributeDisplayName, attributeType, attributeValue);
            }
            return dt;
        }

        private static string GetAttributeDisplayName(OrganizationService organizationService, string entitySchemaName, string attributeSchemaName)
        {
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entitySchemaName,
                LogicalName = attributeSchemaName,
                RetrieveAsIfPublished = true
            };
            RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)organizationService.Execute(retrieveAttributeRequest);
            AttributeMetadata retrievedAttributeMetadata = (AttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
            return retrievedAttributeMetadata.DisplayName.UserLocalizedLabel.Label;
        }
    }
}
